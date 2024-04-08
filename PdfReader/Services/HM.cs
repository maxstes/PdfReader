using HtmlAgilityPack;
using Microsoft.Extensions.Configuration;
using PdfSharp.Snippets.Drawing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.WebSockets;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace PdfReader.Services
{
    internal class HM
    {
        

        
        private readonly string ProxyKey ;
        private readonly string Proxy ;
        private const string Url = "https://metanit.com/sharp/";
        private string SectionP;
        private string CorrentPath = Url;
        readonly HttpClient httpClient = new HttpClient();
        public HM() {
            IConfigurationRoot config = new ConfigurationBuilder()
                .AddUserSecrets<Program>()
                .Build();
            ProxyKey = config["Proxy"];
            Proxy = $"https://proxy.scrapeops.io/v1/?api_key={ProxyKey}&url=";
        }
        private List<(string title, string url)> GetUrlsAndTitles(HtmlDocument document, string nodePath)
        {
            return document.DocumentNode
               .SelectNodes(nodePath)
               .Select(node => (title: node.InnerText, url: SectionP + node.Attributes["href"].Value)) //TODO URL IN ctor
               .ToList();

        }
        public async Task<ElementsList> GetElementsLists(HtmlDocument document)
        {

            // var titles = GetElements(document, "//div/ol/li/p/a");
            var sections = GetUrlsAndTitles(document, "//ol/li/ol/li/p/a");

            return new ElementsList
            { //Titles = titles,
                Sections = sections
            };
        }


        private async Task<string> GetStringPageAsync(string path)
        {

            CorrentPath = Url + path;//TODO URL ctor
                                     // var urlCor = new System.Uri(Url ,path);

            var metanit = await httpClient.GetAsync($"{Proxy}{CorrentPath}");
            var stringResult = await metanit.Content.ReadAsStringAsync();
            return stringResult;
        }
        private int[] SectionsCalculateInTitle(HtmlDocument document)
        {
            var olNode = document.DocumentNode.SelectNodes("//ol[@class='subsubcontent']").ToList();
            int[] ints = new int[olNode.Count];

            HtmlDocument doc = new HtmlDocument();
            /// var intsMas = olNode.Select(n => n.SelectNodes("/li").Count()).ToArray();

            for (int i = 0; i < olNode.Count; i++)
            {
                doc.LoadHtml(olNode[i].InnerHtml);

                var listcount = doc.DocumentNode
                    .SelectNodes("//li")
                    .Count();//ксть розділів в главі
                ints[i] = listcount;
            }
            return ints;
        }
        public async Task MainMyFunc(string path)
        {
            SectionP = path;
            string stringDoc = await GetStringPageAsync(path);

            var document = new HtmlDocument();
            document.LoadHtml(stringDoc);
            var elements = await GetElementsLists(document);

            await SaveHtml(elements);

            int[] ints = SectionsCalculateInTitle(document);

        }
        public async Task SaveHtml(ElementsList list)
        {
            var nodes = new List<(HtmlNode node, string title)>();
            foreach (var elementList in list.Sections)
            {
                var UrlPage = elementList.url;
                var StringPage = await GetStringPageAsync(UrlPage);
                HtmlDocument document = new HtmlDocument();
                document.LoadHtml(StringPage);
                var contentPage = document.DocumentNode.SelectSingleNode("//*[@id=\"container\"]/div/div/div");
                var imgs = contentPage.SelectNodes("//*[@id=\"container\"]/div/div/div/img");
                var src = imgs.Select(n => n.Attributes["src"].Value).ToList();

                await DownloadImgFromPageAsync(imgs);
              

                nodes.Add((contentPage, elementList.title));
            }

            HtmlDocument doc = new HtmlDocument();
            nodes.Select(n => doc.DocumentNode.AppendChild(n.node)).ToList();
            doc.Save("Content");
        }
        private async Task DownloadImgFromPageAsync(HtmlNodeCollection nodes)
        {
            
            foreach (var node in nodes)
            {
                var s = node.Attributes["src"].Value;
                using var stream = await GetStreamPageAsync(s);
                var fileName = Path.GetFileName(s);
                using var file =  File.Create(fileName);
                await stream.CopyToAsync(file);
                node.Attributes["src"].Value = fileName;
            }
            
        }
        private async Task<Stream> GetStreamPageAsync(string path)
        {
            path = path.Substring(2);
            CorrentPath =$" {Url}{SectionP}{path}";
            //TODO URL ctor
                                     // var urlCor = new System.Uri(Url ,path);

            var metanit = await httpClient.GetAsync($"{Proxy}{CorrentPath}");
#warning 

            var stringResult = await metanit.Content.ReadAsStreamAsync();
            return stringResult;
        }
    }
}
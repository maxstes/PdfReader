using HtmlAgilityPack;
using Microsoft.Extensions.Configuration;
using static System.Collections.Specialized.BitVector32;

namespace PdfReader.Services
{
    internal class PageReader
    { 
        internal readonly string ProxyKey ;
        internal readonly string Proxy ;
        private const string Url = "https://metanit.com/sharp/";
        private string SectionP;
        private Uri CorrentPath;
        private readonly HttpClient httpClient = new HttpClient();
        public PageReader() {
            IConfigurationRoot config = new ConfigurationBuilder()
                .AddUserSecrets<Program>()
                .Build();
            ProxyKey = config["Proxy"];
            Proxy = $"https://proxy.scrapeops.io/v1/?api_key={ProxyKey}&url=";
            CorrentPath = new($"{Proxy}{Url}");
        }

        internal List<(string title, string url)> GetUrlsAndTitles(HtmlDocument document, string nodePath)
        {
            return document.DocumentNode
               .SelectNodes(nodePath)
               .Select(node => (title: node.InnerText, url: SectionP + node.Attributes["href"].Value)) 
               .ToList(); 

        }
        public List<(string title, string url)> GetElementsLists(HtmlDocument document)
        {

            var sections = GetUrlsAndTitles(document, "//ol/li/ol/li/p/a");

            return sections;
        }

        internal async Task<string> GetStringPageAsync()
        {
            CorrentPath = new($"{CorrentPath}{SectionP}");

            var metanit = await httpClient.GetAsync(CorrentPath);
            var stringResult = await metanit.Content.ReadAsStringAsync();
            return stringResult;
        }

        internal HtmlDocument GetDocument(string stringDoc)
        {
            var doc = new HtmlDocument();
            doc.LoadHtml(stringDoc);
            return doc;
        }

        public async Task MainMyFunc(string path)
        {
            SectionP = path;
            string stringDoc = await GetStringPageAsync();

            var document = GetDocument(stringDoc);
            var elements = GetElementsLists(document);

            ImagesAndHtmlService imagesService = new ImagesAndHtmlService(new Uri($"{Proxy}{Url}"),path);
            
            await imagesService.SaveHtml(elements);

            await Console.Out.WriteLineAsync("The end");
        }
    }
}
        //private static int[] CountSectionInEveryTitles(HtmlDocument document)//Кількість розділів в кожній главі
        //{
        //    var olNode = document.DocumentNode.SelectNodes("//ol[@class='subsubcontent']").ToList();
        //    int[] ints = new int[olNode.Count];

        //    HtmlDocument doc = new HtmlDocument();

        //    for (int i = 0; i < olNode.Count; i++)
        //    {
        //        doc.LoadHtml(olNode[i].InnerHtml);

        //        var listcount = doc.DocumentNode
        //            .SelectNodes("//li")
        //            .Count();//ксть розділів в главі
        //        ints[i] = listcount;
        //    }
        //    return ints;
        //}
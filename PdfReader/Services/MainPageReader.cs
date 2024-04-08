using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace PdfReader.Services
{
    internal class MainPageReader
    {
        public MainPageReader()
        {

        }
        const string Url = "https://metanit.com/sharp/";
        private HttpClient httpClient = new();
        private async Task<HtmlDocument> GetHtmlDoc()
        {
            var metanit = httpClient.GetAsync($"https://proxy.scrapeops.io/v1/?api_key=eb3c35e0-8630-4739-9f3d-638e9c8f1e55&url={Url}");
            var stringResult = await metanit.Result.Content.ReadAsStringAsync();
            var document = new HtmlDocument();
            document.LoadHtml(stringResult);
            return document;
        }

        private List<(string title, string url)> GetElements(HtmlDocument document, string nodePath)
        {
            return document.DocumentNode
               .SelectNodes(nodePath)
               .Select(node => (title: node.InnerText, url: Url + node.Attributes["href"].Value))
               .ToList();
        }
        public async Task<List<(string title,string url)>> MainFunc()
        {
            var document = await GetHtmlDoc();
            
            var elements = GetElements(document, "//*[@id=\"container\"]/div/div/div/div/div/p/a");
            return elements; 
            
        }
    }
}

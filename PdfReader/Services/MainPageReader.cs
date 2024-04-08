using HtmlAgilityPack;
using Microsoft.Extensions.Configuration;
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
        const string Url = "https://metanit.com/sharp/";
        private readonly string ProxyKey;
        private HttpClient httpClient = new();
        public MainPageReader()
        {
            IConfigurationRoot config = new ConfigurationBuilder()
                .AddUserSecrets<Program>()
                .Build();
            ProxyKey = config["Proxy"];
        }
        private async Task<HtmlDocument> GetHtmlDoc()
        {
            var metanit = httpClient.GetAsync($"https://proxy.scrapeops.io/v1/?api_key={ProxyKey}&url={Url}");
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

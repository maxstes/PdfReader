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
        private PageReader hM = new();
        public MainPageReader()
        {
        }
 
        public async Task<List<(string title,string url)>> MainFunc()
        {
            var document = hM.GetDocument(await hM.GetStringPageAsync());
            
            var elements = hM.GetUrlsAndTitles(document, "//*[@id=\"container\"]/div/div/div/div/div/p/a");
            return elements; 
            
        }
    }
}

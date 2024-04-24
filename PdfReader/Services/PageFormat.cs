using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PdfReader.Services
{
    internal class PageFormat
    {
        List<string> BlackClassList = new() { "socBlock","nav", "commentABl" };
        public PageFormat()
        {
        }
        public HtmlDocument RemoveTrash(HtmlDocument doc)
        {
            HtmlNodeCollection[] nodes = new HtmlNodeCollection[BlackClassList.Count];
            for(int i = 0;i < BlackClassList.Count;i++)
            {
                nodes[i] = doc.DocumentNode.SelectNodes($"//div/div[@class='{BlackClassList[i]}']");
            }
            var trash = doc.DocumentNode.SelectNodes("//div/div[@id='yandex_rtb_R-A-201190-1']");
            foreach (var tras in trash)
            {
                tras.Remove();
            }

            foreach (var node in nodes.SelectMany(array => array))
            {
                node.Remove();
            }
            doc = RemoveStyleElements(doc);
            return doc;
        }
        private HtmlDocument RemoveStyleElements(HtmlDocument document)
        {
            var doc = document.DocumentNode.SelectNodes("//div/style");
            foreach (var item in doc)
            {
                item.Remove();
            }
            return document;
        }
    }
}

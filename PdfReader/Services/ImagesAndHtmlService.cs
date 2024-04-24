using HtmlAgilityPack;
using static GrapeCity.Documents.Pdf.MarkedContent.TagArtifact;

namespace PdfReader.Services
{
    internal class ImagesAndHtmlService
    {
        private PageFormat pageFormat = new();
        private PageReader pageReader = new();
        private readonly HttpClient httpClient = new();
        private Uri CorrentPath ;
        private string Section;
        public ImagesAndHtmlService(Uri correntPath,string section)
        {
            CorrentPath = new(correntPath.ToString());
            Section = section;
        }
        public async Task<string> SaveHtml(List<(string title, string url)> list)
        {
            var style = await GetStyle();
            var nodes = new List<(HtmlNode node, string title)>();
            foreach (var elementList in list)
            {
                var UrlPage = elementList.url;
                string StringPage = await GetStringPageAsync(UrlPage);
                HtmlDocument document = new HtmlDocument();
                document.LoadHtml(StringPage);

                var contentPage = document.DocumentNode.SelectSingleNode("//*[@id=\"container\"]/div/div/div");
                var imgs = contentPage.SelectNodes("//*[@id=\"container\"]/div/div/div/img");
                if (imgs != null)
                {
                    var src = imgs.Select(n => n.Attributes["src"].Value).ToList();

                    await DownloadImgFromPageAsync(imgs);

                }
                nodes.Add((contentPage, elementList.title));
            }

            HtmlDocument doc = new HtmlDocument();
            doc.LoadHtml("<html><head></head><body></body></html>");
      
            
            doc.DocumentNode.SelectSingleNode("//head").AppendChild(HtmlNode.CreateNode($"<style>{style}</style>"));
            nodes.Select(n => doc.DocumentNode.SelectSingleNode("//body").AppendChild(n.node)).ToList();

            doc = pageFormat.RemoveTrash(doc);
            doc.Save("Content.html");
            
            return doc.DocumentNode.OuterHtml;
        }
        private async Task DownloadImgFromPageAsync(HtmlNodeCollection nodes)
        {

            foreach (var node in nodes)
            {
                var SrcPath = node.Attributes["src"].Value;
                SrcPath = SrcPath.Substring(2);
                using var stream = await GetStreamPageAsync(SrcPath);//CorrectPath
                var fileName = Path.GetFileName(SrcPath);
                using var file = File.Create(fileName);
                await stream.CopyToAsync(file);
                node.Attributes["src"].Value = fileName;
            }

        }
        private async Task<Stream> GetStreamPageAsync(string path)
        {
            Uri NowPath = new($"{CorrentPath}{Section}{path}");
            var metanit = await httpClient.GetAsync(NowPath);
            var stringResult = await metanit.Content.ReadAsStreamAsync();
            return stringResult;
        }
        private async Task<string> GetStringPageAsync(string path)
        { 
            string nowPath = $"{CorrentPath}{path}";
            var metanit = await httpClient.GetAsync(nowPath);
            var stringResult = await metanit.Content.ReadAsStringAsync();
            return stringResult;
        }
        private async Task<string> GetStyle()
        {
            var styles = await pageReader.GetStringPageAsync($"{pageReader.Proxy}https://metanit.com/style44.css?v=2");
            return styles;
        }
    }
}

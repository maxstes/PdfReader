using HtmlAgilityPack;
using Microsoft.Extensions.Configuration;

namespace PdfReader.Services
{
    internal class HM
    { 
        private readonly string ProxyKey ;
        private readonly string Proxy ;
        private const string Url = "https://metanit.com/sharp/";
        private string SectionP;
        private Uri CorrentPath;
        private readonly HttpClient httpClient = new HttpClient();
        private ImagesService imageService;
        public HM() {
            IConfigurationRoot config = new ConfigurationBuilder()
                .AddUserSecrets<Program>()
                .Build();
            ProxyKey = config["Proxy"];
            Proxy = $"https://proxy.scrapeops.io/v1/?api_key={ProxyKey}&url=";
            CorrentPath = new($"{Proxy}{Url}");
        }
        private List<(string title, string url)> GetUrlsAndTitles(HtmlDocument document, string nodePath)
        {
            return document.DocumentNode
               .SelectNodes(nodePath)
               .Select(node => (title: node.InnerText, url: SectionP + node.Attributes["href"].Value)) 
               .ToList(); 

        }
        public ElementsList GetElementsLists(HtmlDocument document)
        {

            // var titles = GetElements(document, "//div/ol/li/p/a");
            var sections = GetUrlsAndTitles(document, "//ol/li/ol/li/p/a");

            return new ElementsList
            { //Titles = titles,
                Sections = sections
            };
        }

        private static int[] CountSectionInEveryTitles(HtmlDocument document)
        {
            var olNode = document.DocumentNode.SelectNodes("//ol[@class='subsubcontent']").ToList();
            int[] ints = new int[olNode.Count];

            HtmlDocument doc = new HtmlDocument();

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
        
        private async Task<string> GetStringPageAsync()
        {
            CorrentPath = new($"{CorrentPath}{SectionP}");

            var metanit = await httpClient.GetAsync(CorrentPath);
            var stringResult = await metanit.Content.ReadAsStringAsync();
            return stringResult;
        }
        public async Task MainMyFunc(string path)
        {
            SectionP = path;
            string stringDoc = await GetStringPageAsync();

            var document = GetDocument(stringDoc);
            var elements = GetElementsLists(document);

            ImagesService imagesService = new ImagesService(new Uri($"{Proxy}{Url}"),path);
            PageFormat pageFormat = new PageFormat();
            //await SaveHtml(elements);

            //int[] ints = CountSectionInEveryTitles(document);
            //string clearString = pageFormat.RemoveTrash(document);
            var stringPage =  await imagesService.SaveHtml(elements);
           //PdfGeneratorService.Generate(stringPage);

            await Console.Out.WriteLineAsync("The end");
        }
        private HtmlDocument GetDocument(string stringDoc)
        {
            var doc = new HtmlDocument();
            doc.LoadHtml(stringDoc);
            return doc;
        }
    }
}
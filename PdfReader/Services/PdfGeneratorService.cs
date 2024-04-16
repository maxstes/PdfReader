using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;
using GrapeCity.Documents.Html;
using System.Drawing;
using GrapeCity.Documents.Pdf;
using QuestPDF.Fluent;
using HTMLQuestPDF.Extensions;
using QuestPDF.Infrastructure;
using QuestPDF.Helpers;



namespace PdfReader.Services
{
    internal static class PdfGeneratorService
    {
        public static void DownloadPDF(string html,string path)
        {
            
            Document.Create(container =>
            {
                container
                .Page(page =>
                {
                    page.DefaultTextStyle(ts => ts.FontFamily("Cambria Math").Fallback(ts => ts.FontFamily("Microsoft PhagsPa")));
                    page.Margin(12,Unit.Point);
                    page.Size(PageSizes.A4);
                    page.Content().Column(col =>
                    {
                        col.Item().HTML(handler =>
                        {
                            
                            handler.SetHtml(html);
                        });
                    });
                });
            }
            
            ).GeneratePdf(path);
        }

    }
}

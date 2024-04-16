using PdfReader.Services;

PageReader hM = new PageReader();


var UI = new UIService();
var path = await UI.GetUrlChooseSection();

await hM.MainMyFunc(path);

var stringText = await File.ReadAllTextAsync("Content.html");
PdfGeneratorService.DownloadPDF(stringText,"Content.pdf");
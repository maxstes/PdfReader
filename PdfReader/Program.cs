using HtmlAgilityPack;
using PdfReader;
using PdfReader.Services;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;



//HttpClient httpClientProxy = new HttpClient();
//var result = await httpClientProxy.GetAsync("https://gimmeproxy.com/api/getProxy?get=true&supportsHttps=true&protocol=http&country=US");
//var json = await result.Content.ReadFromJsonAsync<JsonInfo>();
//string cUrl = json.curl;
HM hM = new HM();
await hM.MainMyFunc("msil/");

//var mp = new MainPageReader();
//mp.MainFunc();
//var UI = new UIService();
//await UI.GetUrlChooseSection();
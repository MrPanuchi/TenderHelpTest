using Microsoft.AspNetCore.Mvc;
using System.IO;
using System.Linq;
using System.Net;
using System.Text.Json;
using System.Text.Unicode;
using THTest.Services;

namespace THTest.Controllers
{
    public class NalogController : Controller
    {
        private readonly ICleaner _cleaner;
        public NalogController(ICleaner cleaner)
        {
            _cleaner = cleaner;
        }
        public IActionResult Get(string value)
        {
            _cleaner.Clean();
            string data = "";
            string valueWithoutSpace = "";
            if (value != null)
            {
                valueWithoutSpace = string.Concat(value.Where(x => !char.IsWhiteSpace(x)));
            }
            if (valueWithoutSpace.Length == 13 || valueWithoutSpace.Length == 10 || valueWithoutSpace.Length == 12)
            {

                data = JsonSerializer.Serialize(GetNalog(valueWithoutSpace).data,typeof(NalogInfoData[]), 
                    new JsonSerializerOptions() { Encoder = System.Text.Encodings.Web.JavaScriptEncoder.Create(UnicodeRanges.All)});
            }
            else
            {
                data = "Write INN or ORGN";
            }
            ViewBag.Data = data;
            return View();
        }
        private NalogInfo GetNalog(string data)
        {
            NalogBuffer nalogs = NalogBuffer.Init();
            NalogInfo selectedNalog = nalogs.GetNalog(data);
            if (selectedNalog == null)
            {
                selectedNalog = DownloadNalog(data);
                nalogs.AddRow(selectedNalog);
            }
            return selectedNalog;
        }
        private NalogInfo DownloadNalog(string data)
        {
            WebRequest request = WebRequest.Create("https://rmsp.nalog.ru/search-proc.json");

            string body = $"mode=quick&page=1&query={data}&pageSize=10&sortField=NAME_EX&sort=ASC";

            request.Method = "POST";
            request.Headers = GetDefaultHeaders();
            request.ContentType = "application/x-www-form-urlencoded";
            request.ContentLength = body.Length;
            InsertContent(body, request);

            WebResponse response = request.GetResponse();
            string returnedData = "";
            using (Stream stream = response.GetResponseStream())
            {
                using (StreamReader reader = new StreamReader(stream))
                {
                    returnedData = reader.ReadToEnd();
                }
            }
            response.Close();
            NalogInfo nalog = JsonSerializer.Deserialize<NalogInfo>(returnedData);
            if (nalog.rowCount == 0)
            {
                return null;
            }
            else
            {
                return nalog;
            }
        }
        private WebHeaderCollection GetDefaultHeaders()
        {
            WebHeaderCollection headers = new WebHeaderCollection();
            headers.Add("Host", "rmsp.nalog.ru");
            headers.Add("Connection", "keep-alive");
            headers.Add("Accept", "application/x-www-form-urlencoded, text/javascript, */*; q=0.01");
            headers.Add("X-Requested-With", "XMLHttpRequest");
            headers.Add("Origin", "https://rmsp.nalog.ru");
            headers.Add("Sec-Fetch-Site", "same-origin");
            headers.Add("Sec-Fetch-Mode", "cors");
            headers.Add("Sec-Fetch-Dest", "empty");
            headers.Add("Referer", "https://rmsp.nalog.ru/index.html");
            headers.Add("Accept-Encoding", "gzip, deflate, br");
            headers.Add("Accept-Language", "ru,en;q=0.9");
            headers.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/92.0.4515.159 YaBrowser/21.8.2.381 Yowser/2.5 Safari/537.36");
            return headers;
        }
        private void InsertContent(string content, WebRequest request)
        {
            byte[] byteArray = System.Text.Encoding.UTF8.GetBytes(content);
            using (Stream dataStream = request.GetRequestStream())
            {
                dataStream.Write(byteArray, 0, byteArray.Length);
            }
        }
    }
}

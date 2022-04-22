using AngleSharp;
using AngleSharp.Html.Dom;
using KintaiAuto.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace KintaiAuto.Controllers
{

    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        const string RECOLU = "[Recolu]";
        const string RAKURAKU = "[RakuRaku]";


        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            var login = LoginReadText();
            Main();
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        private async void Main()
        {
            var urlstring = "https://app.recoru.in/ap/";

            WebClient wc = new WebClient();
            try
            {
                string htmldocs = wc.DownloadString(urlstring);
                // Console.WriteLine(htmldocs);

                var config = Configuration.Default;
                var context = BrowsingContext.New(config);
                var document = await context.OpenAsync(req => req.Content(htmldocs));

                // Console.WriteLine(document.Title);

                // foreach (var item in document.QuerySelectorAll("h1.thumb"))
                List<string> str = new List<string>();
                foreach (var item in document.QuerySelectorAll("a"))
                {
                    Console.WriteLine(item.TextContent.Trim());
                    str.Add(item.TextContent.Trim());

                }
                ViewBag["str"] = str;

            }
            catch (System.Exception)
            {
                throw;
            }


        }

        private List<List<String>> LoginReadText()
        {
            List<List<string>> List = new List<List<string>>();
            List<string> recolu = new List<string>();
            List<string> rakuraku = new List<string>();
            string chk = string.Empty;
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            using (StreamReader sr = new StreamReader(@"login.txt", Encoding.GetEncoding("Shift_JIS")))
            {

                while (sr.Peek() != -1)
                {
                    var str = sr.ReadLine();
                    if (str.Contains(RECOLU))
                    {
                        chk = RECOLU;
                        continue;
                    }
                    else if (str.Contains(RAKURAKU))
                    {
                        chk = RAKURAKU;
                        continue;
                    }

                    if (chk.Contains(RECOLU) && str.Contains("KIGYO"))
                    {

                        str = str.Substring(str.IndexOf("[")+1);
                        recolu.Add(str.Replace("]", ""));
                    }

                    if (str.Contains("ID"))
                    {
                        if (chk == RECOLU)
                        {
                            str =  str.Substring(str.IndexOf("[") + 1);
                            recolu.Add(str.Replace("]", ""));
;                       }
                        else
                        {
                            str = (str.Substring(str.IndexOf("[") + 1));
                            rakuraku.Add(str.Replace("]", ""));
                        }
                    }
                    if (str.Contains("PASS"))
                    {

                        if (chk == RECOLU)
                        {
                            str = str.Substring(str.IndexOf("[") + 1);
                            recolu.Add(str.Replace("]", ""));
                            ;
                        }
                        else
                        {
                            str = (str.Substring(str.IndexOf("[") + 1));
                            rakuraku.Add(str.Replace("]", ""));
                        }
                    }

                }
                List.Add(recolu);
                List.Add(rakuraku);
            }
            return List;

        }


        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}

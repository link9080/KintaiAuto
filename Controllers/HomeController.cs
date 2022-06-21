using AngleSharp;
using AngleSharp.Dom;
using AngleSharp.Html.Dom;
using AngleSharp.Js;
using KintaiAuto.Models;
using KintaiAuto.ViewModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
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
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using OpenQA.Selenium;
using System.Threading;

namespace KintaiAuto.Controllers
{

    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        const string RECOLU = "[Recolu]";
        const string RAKURAKU = "[RakuRaku]";
        LoginsInfo recolu = new LoginsInfo();
        ChromeDriver chrome = new ChromeDriver();
        LoginsInfo rakuraku = new LoginsInfo();
       
        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public async Task<IActionResult>  Index()
        {
            LoginReadText();
            var model = await Main();
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Update([Bind()] KintaiView model)
        {

            Debug.WriteLine(model.Kintais.Count());
            return View("Index",model);
        }

        [HttpPost]
        public async Task<IActionResult> serch()
        {
            LoginReadText();
            var model = await Main();
            return View("Index",model);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        private async Task<KintaiView>  Main()
        {
            var urlstring = "https://app.recoru.in/ap/";
            var model = new KintaiView();
            model.Kintais = new List<Kintai>();
            

            WebClient wc = new WebClient();
            try
            {
                //string htmldocs = wc.DownloadString(urlstring);
                // Console.WriteLine(htmldocs);

                var config = Configuration.Default.WithDefaultLoader().WithDefaultCookies().WithJs();
                var context = BrowsingContext.New(config);

                var wait = new WebDriverWait(chrome, TimeSpan.FromSeconds(60));
                chrome.Url = urlstring;

                // 企業IDを入力
                var kigyoElement = wait.Until(drv => drv.FindElement(By.XPath("//input[@id='contractId']")));
                kigyoElement.SendKeys(recolu.Kigyo);
                //kigyoElement.SendKeys(Keys.Enter);

                // メールアドレスを入力
                var mailElement = wait.Until(drv => drv.FindElement(By.XPath("//input[@id='authId']")));
                mailElement.SendKeys(recolu.ID);
                //kigyoElement.SendKeys(Keys.Enter);

                // passを入力
                var passElement = wait.Until(drv => drv.FindElement(By.XPath("//input[@id='password']")));
                passElement.SendKeys(recolu.PASS);
                passElement.SendKeys(Keys.Enter);

                Thread.Sleep(2 * 1000);

                //ログイン後システム日の表が表示される日付を取得
                var Days = wait.Until(drv => drv.FindElements(By.ClassName("item-day")));

                //開始
                var start = wait.Until(drv => drv.FindElements(By.ClassName("item-worktimeStart")));

                //終了
                var end = wait.Until(drv => drv.FindElements(By.ClassName("item-worktimeEnd")));

                //休憩
                var kyu = wait.Until(drv => drv.FindElements(By.ClassName("item-breaktime")));

                //Thread.Sleep(2 * 1000);

                List<string> str = new List<string>() { "test"};
                var rakuPtn = new SelectList(str);
                for(int i=0;i < Days.Count();i++)
                {
                    var kintai = new Kintai();
                    if(DateTime.TryParse(Days[i].Text,out _))
                    {
                        kintai.Date = DateTime.Parse(Days[i].Text);
                        kintai.StrTime = start[i].Text;
                        kintai.EndTime = end[i].Text;
                        kintai.KyuStrTime = kyu[i].Text;
                        kintai.KyuEndTime = kyu[i].Text;
                        kintai.RakuPtn = "";
                        ViewData["Kintais[" + i + "].RakuPtn"] = rakuPtn;
                        model.Kintais.Add(kintai);
                    }
                    else
                    {
                        ViewData["Kintais[" + i + "].RakuPtn"] = rakuPtn;
                        continue;
                    }
                    
                }
                chromeend();
                return model;

            }
            catch (System.Exception e)
            {
                throw e;
            }


        }

        private List<LoginsInfo> LoginReadText()
        {
            List<LoginsInfo> List = new List<LoginsInfo>();
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
                        recolu.Kigyo=str.Replace("]", "");
                    }

                    if (str.Contains("ID"))
                    {
                        if (chk == RECOLU)
                        {
                            str =  str.Substring(str.IndexOf("[") + 1);
                            recolu.ID = str.Replace("]", "");
;                       }
                        else
                        {
                            str = (str.Substring(str.IndexOf("[") + 1));
                            rakuraku.ID = str.Replace("]", "");
                        }
                    }
                    if (str.Contains("PASS"))
                    {

                        if (chk == RECOLU)
                        {
                            str = str.Substring(str.IndexOf("[") + 1);
                            recolu.PASS = str.Replace("]", "");
                            ;
                        }
                        else
                        {
                            str = (str.Substring(str.IndexOf("[") + 1));
                            rakuraku.PASS = str.Replace("]", "");
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

        private void chromeend()
        {
            chrome.Quit();
            chrome.Dispose();
        }
    }
}

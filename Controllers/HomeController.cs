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
using KintaiAuto.Controllers.util;

namespace KintaiAuto.Controllers
{

    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        const string RECOLU = "[Recolu]";
        const string RAKURAKU = "[RakuRaku]";
        const string  urlstring = "https://app.recoru.in/ap/";
        LoginsInfo recolu = new LoginsInfo();
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
            ChromeDriver chrome = new ChromeDriver();
            LoginReadText();

            try
            {
                var wait = new WebDriverWait(chrome, TimeSpan.FromSeconds(60));

                //tr固定クラス
                const string TR_CLASS = "1717-";
                const string KBN_CLASS = "ID-attendKbn-{0} bg-err form_params";


                loginRecolu(chrome, wait);

                List<string> str = new List<string>() { "test" };
                var rakuPtn = new SelectList(str);
                for (int i = 0;i <  model.Kintais.Count();i++)
                {
                    
                    var _tr = wait.Until(drv => drv.FindElement(By.CssSelector($"[class='{TR_CLASS + model.Kintais[i].Date.ToString("yyyyMMdd")}']")));

                    //開始
                    if (!string.IsNullOrEmpty(model.Kintais[i].StrTime))
                    {
                        //勤務区分 オンサイト固定
                        var kbn = _tr.FindElement(By.TagName($"select"));
                        var select = new SelectElement(kbn);
                        select.SelectByIndex(1);
                        var start = _tr.FindElement(By.Id($"chartDto.attendanceDtos[{i}].worktimeStart"));
                        start.SendKeys(model.Kintais[i].StrTime);
                    }

                    //終了
                    if (!string.IsNullOrEmpty(model.Kintais[i].EndTime))
                    {
                        var end = _tr.FindElement(By.Id($"chartDto.attendanceDtos[{i}].worktimeEnd"));
                        end.SendKeys(model.Kintais[i].EndTime);
                    }

                    //休憩
                    if (!string.IsNullOrEmpty(model.Kintais[i].KyuStrTime) && !string.IsNullOrEmpty(model.Kintais[i].KyuEndTime))
                    {
                        breakTimewrite(_tr, model.Kintais[i], chrome);
                    }

                    ViewData["Kintais[" + i + "].RakuPtn"] = rakuPtn;

                }
                //更新押下
                var updbtn = wait.Until(drv => drv.FindElement(By.Id($"UPDATE-BTN")));
                updbtn.Click();
                Thread.Sleep(1 * 1000);

                chrome.SwitchTo().Alert().Accept();

                Thread.Sleep(1 * 1000);

                chromeend(chrome);
            }
            catch (System.Exception e)
            {
                throw e;
            }

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

        private async Task<KintaiView>  Main(DateTime? date = null)
        {
            var model = new KintaiView();
            model.Kintais = new List<Kintai>();
            ChromeDriver chrome = new ChromeDriver();

            WebClient wc = new WebClient();
            try
            {
                var wait = new WebDriverWait(chrome, TimeSpan.FromSeconds(60));

                //tr固定クラス
                const string TR_CLASS = "1717-";
                loginRecolu(chrome, wait);


                //ログイン後システム日の表が表示される日付を取得
                var Days = wait.Until(drv => drv.FindElements(By.ClassName("item-day")));

                //開始
                var start = wait.Until(drv => drv.FindElements(By.ClassName("item-worktimeStart")));

                //終了
                var end = wait.Until(drv => drv.FindElements(By.ClassName("item-worktimeEnd")));

                List<string> str = new List<string>() { "test"};
                var rakuPtn = new SelectList(str);
                for(int i=0;i < Days.Count();i++)
                {
                    var kintai = new Kintai();
                    if(DateTime.TryParse(Days[i].Text,out _))
                    {
                        kintai.Date = DateTime.Parse(Days[i].Text);

                        var _tr = wait.Until(drv => drv.FindElement(By.CssSelector($"[class='{TR_CLASS + kintai.Date.ToString("yyyyMMdd")}']")));

                        //休憩開始終了をセット
                        breakTimeRead(_tr, kintai,chrome);

                        Debug.WriteLine(start[i].GetAttribute("id"));
                        Debug.WriteLine(start[i].FindElement(By.Id($"chartDto.attendanceDtos[{i-1}].worktimeStart")).GetAttribute("id"));
                        if (start[i].FindElement(By.Id($"chartDto.attendanceDtos[{i - 1}].worktimeStart")).GetAttribute("value") != "")
                        {
                            kintai.StrTime = start[i].FindElement(By.Id($"chartDto.attendanceDtos[{i - 1}].worktimeStart")).GetAttribute("value");
                            kintai.strID = start[i].FindElement(By.Id($"chartDto.attendanceDtos[{i - 1}].worktimeStart")).GetAttribute("id");
                        }

                        Debug.WriteLine(start[i].FindElement(By.Id($"chartDto.attendanceDtos[{i - 1}].worktimeStart")).Text);
                        if (end[i].FindElement(By.Id($"chartDto.attendanceDtos[{i - 1}].worktimeEnd")).GetAttribute("value") != "")
                        {
                            kintai.EndTime = end[i].FindElement(By.Id($"chartDto.attendanceDtos[{i - 1}].worktimeEnd")).GetAttribute("value");
                            kintai.endID = end[i].FindElement(By.Id($"chartDto.attendanceDtos[{i - 1}].worktimeEnd")).GetAttribute("id");
                        }
                        ViewData["Kintais[" + i + "].RakuPtn"] = rakuPtn;
                        model.Kintais.Add(kintai);
                    }
                    else
                    {
                        ViewData["Kintais[" + i + "].RakuPtn"] = rakuPtn;
                        continue;
                    }
                    
                }
                chromeend(chrome);
                return model;

            }
            catch (System.Exception e)
            {
                throw e;
            }


        }

        private void loginRecolu(ChromeDriver chrome,WebDriverWait wait)
        {
            //Recolu
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

            //勤務表へ移動
            var editpage = wait.Until(drv => drv.FindElement(By.LinkText("勤務表")));

            editpage.Click();

            Thread.Sleep(1 * 1000);
        }

        private void breakTimeRead(IWebElement _tr,Kintai _kintai, ChromeDriver chrome)
        {
            var img = _tr.FindElement(By.TagName("img"));
            img.Click();

            Thread.Sleep(1 * 1000);

            var kyustr = chrome.FindElement(By.Id("breaktimeDtos[0].breaktimeStart"));
            _kintai.KyuStrTime = kyustr.GetAttribute("value");

            var kyuend = chrome.FindElement(By.Id("breaktimeDtos[0].breaktimeEnd"));
            _kintai.KyuEndTime = kyuend.GetAttribute("value");

            //chrome.ExecuteScript("updateBreaktimeEditDialog();");

            //Thread.Sleep(1 * 1000);

            //chrome.SwitchTo().Alert().Accept();

            var close = chrome.FindElement(By.CssSelector($"[class='common-btn close']"));
            close.Click();
        }

        private void breakTimewrite(IWebElement _tr, Kintai _kintai, ChromeDriver chrome)
        {
            var img = _tr.FindElement(By.TagName("img"));
            img.Click();

            Thread.Sleep(1 * 1000);

            var kyustr = chrome.FindElement(By.Id("breaktimeDtos[0].breaktimeStart"));
            kyustr.SendKeys(_kintai.KyuStrTime);

            var kyuend = chrome.FindElement(By.Id("breaktimeDtos[0].breaktimeEnd"));
            kyuend.SendKeys(_kintai.KyuEndTime);

            //chrome.ExecuteScript("updateBreaktimeEditDialog();");
            var btn = chrome.FindElements(By.Id("UPDATE-BTN"))[1];
            btn.Click();

            Thread.Sleep(1 * 1000);

            chrome.SwitchTo().Alert().Accept();

            Thread.Sleep(1 * 1000);
            if (chrome.FindElements(By.CssSelector($"[class='common-btn close']")).Count() > 0)
            {

                var close = chrome.FindElement(By.CssSelector($"[class='common-btn close']"));

                close.Click();

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

        private void chromeend(ChromeDriver chrome)
        {
            chrome.Quit();
            chrome.Dispose();
        }
    }
}

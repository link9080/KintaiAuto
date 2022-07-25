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
        const string  recourl = "https://app.recoru.in/ap/";
        const string rakuurl = "https://rsclef.rakurakuseisan.jp/CSR9KsE9qUa/";
        List<string> daylist = new List<string>();
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

        #region 反映
        [HttpPost]
        public async Task<IActionResult> Update([Bind()] KintaiView model)
        {

            Debug.WriteLine(model.Kintais.Count());
            LoginReadText();
            var raku = rakuPtn(model);
            var option = new ChromeOptions();
            //option.AddArgument("headless");
            ChromeDriver chrome = new ChromeDriver(option);

            try
            {
                var wait = new WebDriverWait(chrome, TimeSpan.FromSeconds(60));

                //tr固定クラス
                const string TR_CLASS = "1717-";


                loginRecolu(chrome, wait);

                for (int i = 0; i < model.Kintais.Count(); i++)
                {
                    if ((model.Kintais.Where(r => r.inputflg).Count() == 0) || (model.Kintais.Where(r => r.inputflg).Count() != 0 && model.Kintais[i].inputflg))
                    {
                        var _tr = wait.Until(drv => drv.FindElement(By.CssSelector($"[class='{TR_CLASS + model.Kintais[i].Date.ToString("yyyyMMdd")}']")));

                        //開始
                        if (!string.IsNullOrEmpty(model.Kintais[i].StrTime))
                        {
                            //勤務区分 オンサイト固定
                            var kbn = _tr.FindElement(By.TagName($"select"));
                            var select = new SelectElement(kbn);
                            var opt = select.SelectedOption.GetAttribute("value");
                            if (string.IsNullOrEmpty(opt))
                            {
                                select.SelectByIndex(1);
                            }

                            if (_tr.FindElements(By.CssSelector($"[class=\"ID-worktimeStart-{model.Kintais[i].Date.ToString("yyyyMMdd")}-1 worktimeStart timeText edited\"]")).Count() > 0)
                            {
                                var start = _tr.FindElement(By.CssSelector($"[class=\"ID-worktimeStart-{model.Kintais[i].Date.ToString("yyyyMMdd")}-1 worktimeStart timeText edited\"]"));
                                start.SendKeys(model.Kintais[i].StrTime);
                            }
                            else if (_tr.FindElements(By.CssSelector($"[class=\"ID-worktimeStart-{model.Kintais[i].Date.ToString("yyyyMMdd")}-1 bg-err worktimeStart timeText edited\"]")).Count() > 0)
                            {
                                var start = _tr.FindElement(By.CssSelector($"[class=\"ID-worktimeStart-{model.Kintais[i].Date.ToString("yyyyMMdd")}-1 bg-err worktimeStart timeText edited\"]"));
                                start.SendKeys(model.Kintais[i].StrTime);
                            }
                            else if (_tr.FindElements(By.CssSelector($"[class=\"ID-worktimeStart-{model.Kintais[i].Date.ToString("yyyyMMdd")}-1 bg-err worktimeStart timeText\"]")).Count() > 0)
                            {
                                var start = _tr.FindElement(By.CssSelector($"[class=\"ID-worktimeStart-{model.Kintais[i].Date.ToString("yyyyMMdd")}-1 bg-err worktimeStart timeText\"]"));
                                start.SendKeys(model.Kintais[i].StrTime);
                            }
                            else if (_tr.FindElements(By.CssSelector($"[class=\"ID-worktimeStart-{model.Kintais[i].Date.ToString("yyyyMMdd")}-1 worktimeStart timeText\"]")).Count() > 0)
                            {
                                var start = _tr.FindElement(By.CssSelector($"[class=\"ID-worktimeStart-{model.Kintais[i].Date.ToString("yyyyMMdd")}-1 worktimeStart timeText\"]"));
                                start.SendKeys(model.Kintais[i].StrTime);
                            }
                        }

                        //終了
                        if (!string.IsNullOrEmpty(model.Kintais[i].EndTime))
                        {
                            if (_tr.FindElements(By.CssSelector($"[class=\"ID-worktimeEnd-{model.Kintais[i].Date.ToString("yyyyMMdd")}-1 worktimeEnd timeText edited\"]")).Count() > 0)
                            {

                                var end = _tr.FindElement(By.CssSelector($"[class=\"ID-worktimeEnd-{model.Kintais[i].Date.ToString("yyyyMMdd")}-1 worktimeEnd timeText edited\"]"));
                                end.SendKeys(model.Kintais[i].EndTime);
                            }
                            else if (_tr.FindElements(By.CssSelector($"[class=\"ID-worktimeEnd-{model.Kintais[i].Date.ToString("yyyyMMdd")}-1 bg-err worktimeEnd timeText edited\"]")).Count() > 0)
                            {
                                var end = _tr.FindElement(By.CssSelector($"[class=\"ID-worktimeEnd-{model.Kintais[i].Date.ToString("yyyyMMdd")}-1 bg-err worktimeEnd timeText edited\"]"));
                                end.SendKeys(model.Kintais[i].EndTime);

                            }
                            else if (_tr.FindElements(By.CssSelector($"[class=\"ID-worktimeEnd-{model.Kintais[i].Date.ToString("yyyyMMdd")}-1 bg-err worktimeEnd timeText\"]")).Count() > 0)
                            {
                                var end = _tr.FindElement(By.CssSelector($"[class=\"ID-worktimeEnd-{model.Kintais[i].Date.ToString("yyyyMMdd")}-1 bg-err worktimeEnd timeText\"]"));
                                end.SendKeys(model.Kintais[i].EndTime);

                            }
                            else if (_tr.FindElements(By.CssSelector($"[class=\"ID-worktimeEnd-{model.Kintais[i].Date.ToString("yyyyMMdd")}-1 worktimeEnd timeText\"]")).Count() > 0)
                            {
                                var end = _tr.FindElement(By.CssSelector($"[class=\"ID-worktimeEnd-{model.Kintais[i].Date.ToString("yyyyMMdd")}-1 worktimeEnd timeText\"]"));
                                end.SendKeys(model.Kintais[i].EndTime);

                            }
                        }

                        //休憩
                        if (!string.IsNullOrEmpty(model.Kintais[i].KyuStrTime) && !string.IsNullOrEmpty(model.Kintais[i].KyuEndTime))
                        {
                            breakTimewrite(_tr, model.Kintais[i], chrome);
                        }
                    }

                    ViewData["Kintais[" + i + "].RakuPtn"] = raku;

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
            ViewData["Message"] = "登録完了";
            return View("Index",model);
        }
        #endregion

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
            var model = new KintaiView();
            model.Kintais = new List<Kintai>();
            var raku = rakuPtn();
            var option = new ChromeOptions();
            //option.AddArgument("headless");
            ChromeDriver chrome = new ChromeDriver(option);

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
                        ViewData["Kintais[" + i + "].RakuPtn"] = raku;
                        if(daylist.Count() > 0 && daylist.Where(r => r.Contains(kintai.Date.ToString("d"))).Count() > 0)
                        {
                            kintai.Rakutrue = false;
                        }
                        model.Kintais.Add(kintai);
                    }
                    else
                    {
                        ViewData["Kintais[" + i + "].RakuPtn"] = raku;
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

        #region 休憩開始終了
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
        #endregion



        #region　楽楽精算セレクトボックス作成更新
        private SelectList rakuPtn(KintaiView model = null)
        {
            var option = new ChromeOptions();
            //option.AddArgument("headless");
            ChromeDriver chrome = new ChromeDriver(option);

            try
            {
                var wait = new WebDriverWait(chrome, TimeSpan.FromSeconds(60));
                loginRaku(chrome, wait);

                //交通費精算クリック
                //var html = chrome.PageSource;
                var editpage = wait.Until(drv => drv.FindElements(By.LinkText ("交通費精算"))[1]);
                editpage.Click();
                Thread.Sleep(1 * 1000);


                var window = chrome.WindowHandles.Last();  
                chrome.SwitchTo().Window(window);

                //修正クリック
                editpage = wait.Until(drv => drv.FindElement(By.LinkText("修正")));
                editpage.Click();

                window = chrome.WindowHandles.Last();
                chrome.SwitchTo().Window(window);

                //すでに作成済みの日付を取得
                var daylists = wait.Until(drv => drv.FindElements(By.ClassName("labelColorDefault")));
                daylist.AddRange(daylists.Select(r => r.Text));

                //マイパターンクリック
                editpage = wait.Until(drv => drv.FindElements(By.CssSelector("[class=\"meisai-insert-button\"]"))[1]);
                editpage.Click();
                Thread.Sleep(1 * 1000);


                window = chrome.WindowHandles.Last();
                chrome.SwitchTo().Window(window);

                Thread.Sleep(1 * 1000);

                //チェックボックスを取得
                var tr = wait.Until(drv => drv.FindElements(By.ClassName("d_hover")));

                List<RakuPtn> list = new List<RakuPtn>();
                foreach(var item in tr)
                {
                    var ptn = new RakuPtn();
                    //チェックボックス
                    var chk = item.FindElement(By.Name("kakutei"));
                    ptn.Id = chk.GetAttribute("value");

                    //パタン名
                    var name = item.FindElements(By.TagName("td"))[1];
                    ptn.PtnName = name.Text;


                    list.Add(ptn);

                }
                if(model != null)
                {
                    for (int i = 0; i < model.Kintais.Count(); i++)
                    {
                        if(model.Kintais[i].Rakutrue != false && !(string.IsNullOrEmpty(model.Kintais[i].RakuPtn)))
                        {
                            //チェックボックスを取得
                            var chks = wait.Until(drv => drv.FindElements(By.Name("kakutei")));

                            foreach(var chk in chks)
                            {
                                if(chk.GetAttribute("value") == model.Kintais[i].RakuPtn)
                                {
                                    chk.Click();
                                    break;
                                }
                            }

                            //次へクリック
                            var nextbtn = wait.Until(drv => drv.FindElement(By.CssSelector("[class=\"common-btn accesskeyFix kakutei d_marginLeft5\"]")));
                            nextbtn.Click();

                            Thread.Sleep(1 * 1000);

                            //日付入力meisaiDate
                            var Dateinput = wait.Until(drv => drv.FindElements(By.Name("meisaiDate"))[1]);
                            Dateinput.SendKeys(model.Kintais[i].Date.ToString("d"));

                            //明細追加押下
                            nextbtn = wait.Until(drv => drv.FindElement(By.CssSelector("[class=\"button button--l button-primary accesskeyFix kakutei\"]")));
                            nextbtn.Click();
                            Thread.Sleep(1 * 1000);

                            window = chrome.WindowHandles.Last();
                            chrome.SwitchTo().Window(window);

                            if (i != model.Kintais.Count())
                            {

                                //マイパターンクリック
                                editpage = wait.Until(drv => drv.FindElements(By.CssSelector("[class=\"meisai-insert-button\"]"))[1]);
                                editpage.Click();
                                Thread.Sleep(1 * 1000);


                                window = chrome.WindowHandles.Last();
                                chrome.SwitchTo().Window(window);
                            }


                        }

                    }
                    //最後追加から入力がないパターンの考慮
                    if (wait.Until(drv => drv.FindElements(By.CssSelector("[class=\"common-btn accesskeyClose\"]"))).Count() > 0) 
                    {
                        var closebtn = wait.Until(drv => drv.FindElement(By.CssSelector("[class=\"common-btn accesskeyClose\"]")));
                        closebtn.Click();

                        window = chrome.WindowHandles.Last();
                        chrome.SwitchTo().Window(window);
                    }

                    //一次保存押下
                    if (wait.Until(drv => drv.FindElements(By.CssSelector("[class=\"button save accesskeyReturn\"]"))).Count() > 0)
                    {
                        var savebtn = wait.Until(drv => drv.FindElement(By.CssSelector("[class=\"button save accesskeyReturn\"]")));
                        savebtn.Click();
                    }
                }

                chromeend(chrome);
                return new SelectList(list,"Id","PtnName");
            } catch(SystemException e)
            {
                throw e;
            }
         }
        #endregion

        #region ログイン処理
        private void loginRaku(ChromeDriver chrome, WebDriverWait wait)
        {
            //Recolu
            chrome.Url = rakuurl;

            // 企業IDを入力
            var kigyoElement = wait.Until(drv => drv.FindElement(By.Name("loginId")));
            kigyoElement.SendKeys(rakuraku.ID);

            var passElement = wait.Until(drv => drv.FindElement(By.Name("password")));
            passElement.SendKeys(rakuraku.PASS);

            passElement.SendKeys(Keys.Enter);

            Thread.Sleep(1 * 1000);

            var frame = wait.Until(drv => drv.FindElement(By.Name("main")));
            chrome.SwitchTo().Frame(frame);
            Thread.Sleep(1 * 1000);
        }

        private void loginRecolu(ChromeDriver chrome, WebDriverWait wait)
        {
            //Recolu
            chrome.Url = recourl;

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
        #endregion


        #region ログイン情報読み込み
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

                        str = str.Substring(str.IndexOf("[") + 1);
                        recolu.Kigyo = str.Replace("]", "");
                    }

                    if (str.Contains("ID"))
                    {
                        if (chk == RECOLU)
                        {
                            str = str.Substring(str.IndexOf("[") + 1);
                            recolu.ID = str.Replace("]", "");
                            ;
                        }
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
        #endregion



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

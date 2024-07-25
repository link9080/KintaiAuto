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
using NLog;
using System.Security.Claims;

namespace KintaiAuto.Controllers
{

    public class HomeController : Controller
    {
        static Logger _logger = NLog.LogManager.GetCurrentClassLogger();
        const string RECOLU = "[Recolu]";
        const string RAKURAKU = "[RakuRaku]";
        const string recourl = "https://app.recoru.in/ap/";
        const string rakuurl = "https://rsclef.rakurakuseisan.jp/CSR9KsE9qUa/";
        List<string> daylist = new List<string>();
        LoginsInfo recolu = new LoginsInfo();
        LoginsInfo rakuraku = new LoginsInfo();

        public HomeController(ILogger<HomeController> logger)
        {
        }

        public async Task<IActionResult> Index()
        {
            _logger.Info("ログイン情報読込");
            LoginReadText();
            _logger.Info(recolu.ID);
            var model = await Main();
            return View(model);
        }

        #region 反映
        [HttpPost]
        public async Task<IActionResult> Update([Bind()] KintaiView model)
        {

            _logger.Info(model.Kintais.Count());
            LoginReadText();
            var raku = rakuPtn(model);
            ChromeDriver chrome = ChromeDriverUtil.Driver();

            try
            {
                var wait = ChromeDriverUtil.waitter(chrome);

                //tr固定クラス
                const string TR_CLASS = "1717-";


                loginRecolu(chrome, wait);
                _logger.Info("recoluログイン情報読込");

                for (int i = 0; i < model.Kintais.Count(); i++)
                {
                    if ((model.Kintais.Where(r => r.inputflg).Count() == 0) || (model.Kintais.Where(r => r.inputflg).Count() != 0 && model.Kintais[i].inputflg))
                    {
                        var _tr = wait.Until(drv => drv.FindElement(By.CssSelector($"[class='{TR_CLASS + model.Kintais[i].Date.ToString("yyyyMMdd")}']")));

                        //開始
                        if (!string.IsNullOrEmpty(model.Kintais[i].StrTime))
                        {
                            //勤務区分 
                            var kbn = _tr.FindElement(By.TagName($"select"));
                            var select = new SelectElement(kbn);
                            var opt = select.SelectedOption.GetAttribute("value");
                            if (string.IsNullOrEmpty(opt))
                            {
                                //楽楽在宅選択の場合は2つ目オフサイトを選択
                                if (raku.Where(r => r.Text == "在宅").First().Value == model.Kintais[i].RakuPtn ||
                                    raku.Where(r => r.Text == "在宅").First().Value == model.Kintais[i].RakuPtn2)
                                {
                                    select.SelectByIndex(2);
                                }
                                else
                                {
                                    select.SelectByIndex(1);
                                }

                            }

                            if (_tr.FindElements(By.CssSelector($"[class=\"ID-worktimeStart-{model.Kintais[i].Date.ToString("yyyyMMdd")}-1 worktimeStart timeText edited\"]")).Count() > 0)
                            {
                                var start = _tr.FindElement(By.CssSelector($"[class=\"ID-worktimeStart-{model.Kintais[i].Date.ToString("yyyyMMdd")}-1 worktimeStart timeText edited\"]"));
                                start.Clear();
                                start.SendKeys(model.Kintais[i].StrTime);
                            }
                            else if (_tr.FindElements(By.CssSelector($"[class=\"ID-worktimeStart-{model.Kintais[i].Date.ToString("yyyyMMdd")}-1 bg-err worktimeStart timeText edited\"]")).Count() > 0)
                            {
                                var start = _tr.FindElement(By.CssSelector($"[class=\"ID-worktimeStart-{model.Kintais[i].Date.ToString("yyyyMMdd")}-1 bg-err worktimeStart timeText edited\"]"));
                                start.Clear();
                                start.SendKeys(model.Kintais[i].StrTime);
                            }
                            else if (_tr.FindElements(By.CssSelector($"[class=\"ID-worktimeStart-{model.Kintais[i].Date.ToString("yyyyMMdd")}-1 bg-err worktimeStart timeText\"]")).Count() > 0)
                            {
                                var start = _tr.FindElement(By.CssSelector($"[class=\"ID-worktimeStart-{model.Kintais[i].Date.ToString("yyyyMMdd")}-1 bg-err worktimeStart timeText\"]"));
                                start.Clear();
                                start.SendKeys(model.Kintais[i].StrTime);
                            }
                            else if (_tr.FindElements(By.CssSelector($"[class=\"ID-worktimeStart-{model.Kintais[i].Date.ToString("yyyyMMdd")}-1 worktimeStart timeText\"]")).Count() > 0)
                            {
                                var start = _tr.FindElement(By.CssSelector($"[class=\"ID-worktimeStart-{model.Kintais[i].Date.ToString("yyyyMMdd")}-1 worktimeStart timeText\"]"));
                                start.Clear();
                                start.SendKeys(model.Kintais[i].StrTime);
                            }
                        }

                        //終了
                        if (!string.IsNullOrEmpty(model.Kintais[i].EndTime))
                        {
                            if (_tr.FindElements(By.CssSelector($"[class=\"ID-worktimeEnd-{model.Kintais[i].Date.ToString("yyyyMMdd")}-1 worktimeEnd timeText edited\"]")).Count() > 0)
                            {

                                var end = _tr.FindElement(By.CssSelector($"[class=\"ID-worktimeEnd-{model.Kintais[i].Date.ToString("yyyyMMdd")}-1 worktimeEnd timeText edited\"]"));
                                end.Clear();
                                end.SendKeys(model.Kintais[i].EndTime);
                            }
                            else if (_tr.FindElements(By.CssSelector($"[class=\"ID-worktimeEnd-{model.Kintais[i].Date.ToString("yyyyMMdd")}-1 bg-err worktimeEnd timeText edited\"]")).Count() > 0)
                            {
                                var end = _tr.FindElement(By.CssSelector($"[class=\"ID-worktimeEnd-{model.Kintais[i].Date.ToString("yyyyMMdd")}-1 bg-err worktimeEnd timeText edited\"]"));
                                end.Clear();
                                end.SendKeys(model.Kintais[i].EndTime);

                            }
                            else if (_tr.FindElements(By.CssSelector($"[class=\"ID-worktimeEnd-{model.Kintais[i].Date.ToString("yyyyMMdd")}-1 bg-err worktimeEnd timeText\"]")).Count() > 0)
                            {
                                var end = _tr.FindElement(By.CssSelector($"[class=\"ID-worktimeEnd-{model.Kintais[i].Date.ToString("yyyyMMdd")}-1 bg-err worktimeEnd timeText\"]"));
                                end.Clear();
                                end.SendKeys(model.Kintais[i].EndTime);

                            }
                            else if (_tr.FindElements(By.CssSelector($"[class=\"ID-worktimeEnd-{model.Kintais[i].Date.ToString("yyyyMMdd")}-1 worktimeEnd timeText\"]")).Count() > 0)
                            {
                                var end = _tr.FindElement(By.CssSelector($"[class=\"ID-worktimeEnd-{model.Kintais[i].Date.ToString("yyyyMMdd")}-1 worktimeEnd timeText\"]"));
                                end.Clear();
                                end.SendKeys(model.Kintais[i].EndTime);

                            }
                        }

                        //休憩
                        if (!string.IsNullOrEmpty(model.Kintais[i].KyuStrTime))
                        {
                            breakTimewrite(_tr, model.Kintais[i], chrome,wait);
                        }
                    }

                    ViewData["Kintais[" + i + "].RakuPtn"] = raku;
                    ViewData["Kintais[" + i + "].RakuPtn2"] = raku;

                }
                //更新押下
                var updbtn = wait.Until(drv => drv.FindElement(By.Id($"UPDATE-BTN")));
                updbtn.Click();
                var alert = wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.AlertIsPresent());
                alert.Accept();
                ChromeDriverUtil.sleep();

                ChromeDriverUtil.ChromeEnd(chrome);
            }
            catch (System.Exception e)
            {
                ChromeDriverUtil.ChromeEnd(chrome);
                _logger.Error(e.StackTrace);
                ViewData["ErrorMessage"] = e.Message;
                return View("Index", model);
            }
            ViewData["Message"] = "登録完了";
            return View("Index", model);
        }
        #endregion

        [HttpPost]
        public async Task<IActionResult> serch()
        {
            LoginReadText();
            var model = await Main();
            return View("Index", model);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        private async Task<KintaiView> Main()
        {
            var model = new KintaiView();
            model.Kintais = new List<Kintai>();
            var raku = rakuPtn();
            _logger.Info(raku.First().Text);
            if (raku == null)
            {
                ViewData["ErrorMessage"] = "楽楽精算のページで今月の交通費精算を作成してください。";
                return model;
            }

            ChromeDriver chrome = ChromeDriverUtil.Driver();

            try
            {
                var wait = ChromeDriverUtil.waitter(chrome);

                //tr固定クラス
                const string TR_CLASS = "1717-";
                loginRecolu(chrome, wait);
                _logger.Info("recoluログイン成功");

                //ログイン後システム日の表が表示される日付を取得
                var Days = wait.Until(drv => drv.FindElements(By.ClassName("item-day")));
                _logger.Info(Days.Count);

                //開始
                var start = wait.Until(drv => drv.FindElements(By.ClassName("item-worktimeStart")));

                //終了
                var end = wait.Until(drv => drv.FindElements(By.ClassName("item-worktimeEnd")));

                for (int i = 0; i < Days.Count(); i++)
                {
                    var kintai = new Kintai();
                    if (DateTime.TryParse(Days[i].Text, out _))
                    {
                        kintai.Date = DateTime.Parse(Days[i].Text);
                        _logger.Info(kintai.Date);

                        var _tr = wait.Until(drv => drv.FindElement(By.CssSelector($"[class='{TR_CLASS + kintai.Date.ToString("yyyyMMdd")}']")));

                        //休憩開始終了をセット
                        //breakTimeRead(_tr, kintai,chrome);

                        _logger.Debug(start[i].GetAttribute("id"));
                        _logger.Debug(start[i].FindElement(By.Id($"chartDto.attendanceDtos[{i - 1}].worktimeStart")).GetAttribute("id"));
                        if (start[i].FindElement(By.Id($"chartDto.attendanceDtos[{i - 1}].worktimeStart")).GetAttribute("value") != "")
                        {
                            kintai.StrTime = start[i].FindElement(By.Id($"chartDto.attendanceDtos[{i - 1}].worktimeStart")).GetAttribute("value");
                            kintai.strID = start[i].FindElement(By.Id($"chartDto.attendanceDtos[{i - 1}].worktimeStart")).GetAttribute("id");
                        }

                        _logger.Debug(start[i].FindElement(By.Id($"chartDto.attendanceDtos[{i - 1}].worktimeStart")).Text);
                        if (end[i].FindElement(By.Id($"chartDto.attendanceDtos[{i - 1}].worktimeEnd")).GetAttribute("value") != "")
                        {
                            kintai.EndTime = end[i].FindElement(By.Id($"chartDto.attendanceDtos[{i - 1}].worktimeEnd")).GetAttribute("value");
                            kintai.endID = end[i].FindElement(By.Id($"chartDto.attendanceDtos[{i - 1}].worktimeEnd")).GetAttribute("id");
                        }
                        ViewData["Kintais[" + i + "].RakuPtn"] = raku;
                        ViewData["Kintais[" + i + "].RakuPtn2"] = raku;
                        if (daylist.Count() > 0 && daylist.Where(r => r.Contains(kintai.Date.ToString("d"))).Count() > 0)
                        {
                            kintai.Rakutrue = false;
                        }

                        model.Kintais.Add(kintai);
                    }
                    else
                    {
                        ViewData["Kintais[" + i + "].RakuPtn"] = raku;
                        ViewData["Kintais[" + i + "].RakuPtn2"] = raku;
                        continue;
                    }

                }
                ChromeDriverUtil.ChromeEnd(chrome);
                _logger.Info(model.Kintais.Count());
                return model;

            }
            catch (Exception e)
            {
                _logger.Error(e.StackTrace);
                ViewData["ErrorMessage"] = e.Message;
                return model;
            }


        }

        #region 休憩開始終了
        private void breakTimeRead(IWebElement _tr, Kintai _kintai, ChromeDriver chrome)
        {
            var img = _tr.FindElement(By.TagName("img"));
            img.Click();


            var kyustr = chrome.FindElement(By.Id("breaktimeDtos[0].breaktimeStart"));
            _kintai.KyuStrTime = kyustr.GetAttribute("value");

            var kyuend = chrome.FindElement(By.Id("breaktimeDtos[0].breaktimeEnd"));
            _kintai.KyuEndTime = kyuend.GetAttribute("value");

            var close = chrome.FindElement(By.CssSelector($"[class='common-btn close']"));
            close.Click();
        }

        private void breakTimewrite(IWebElement _tr, Kintai _kintai, ChromeDriver chrome,WebDriverWait wait)
        {
            var img = _tr.FindElement(By.TagName("img"));
            img.Click();
            ChromeDriverUtil.sleep();

            var kyustr = wait.Until(drv => drv.FindElement(By.Id("breaktimeDtos[0].breaktimeStart")));
            kyustr.Clear();
            kyustr.SendKeys(_kintai.KyuStrTime);

            var kyuend = wait.Until(drv => drv.FindElement(By.Id("breaktimeDtos[0].breaktimeEnd")));
            kyuend.Clear();
            //休憩終了時間,勤務時間により計算するように
            var kyuEndCalc = DateTimeUtil.CalcKyukei(_kintai);
            kyuend.SendKeys(kyuEndCalc);

            //chrome.ExecuteScript("updateBreaktimeEditDialog();");
            var btn = wait.Until(drv => drv.FindElements(By.Id("UPDATE-BTN")))[1];
            btn.Click();


            var alert = wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.AlertIsPresent());
            alert.Accept();

            if (wait.Until(drv => drv.FindElements(By.CssSelector($"[class='common-btn close']"))).Count() > 0)
            {

                var close = wait.Until(drv => drv.FindElement(By.CssSelector($"[class='common-btn close']")));

                close.Click();

            }
            ChromeDriverUtil.sleep();
        }
        #endregion



        #region　楽楽精算セレクトボックス作成更新
        private SelectList rakuPtn(KintaiView model = null)
        {
            ChromeDriver chrome = ChromeDriverUtil.Driver();

            try
            {
                var wait = ChromeDriverUtil.waitter(chrome);
                loginRaku(chrome, wait);
                _logger.Info("楽楽清算ログイン成功");
                //交通費精算クリック
                //var html = chrome.PageSource;
                if (wait.Until(drv => drv.FindElements(By.LinkText("交通費精算"))).Count() == 1)
                {
                    //交通費精算作られていない場合
                    ChromeDriverUtil.ChromeEnd(chrome);
                    return null;

                }

                var editpage = wait.Until(drv => drv.FindElements(By.LinkText("交通費精算"))[1]);
                editpage.Click();

                ChromeDriverUtil.sleep();

                var window = wait.Until(drv => drv.WindowHandles.Last());
                wait.Until(drv => drv.SwitchTo().Window(window));
                _logger.Info("楽楽清算-一時保存");

                //修正クリックw_denpyo_l
                if (chrome.FindElements(By.LinkText("修正")).Count() > 0)
                {
                    editpage = wait.Until(drv => drv.FindElement(By.LinkText("修正")));
                    editpage.Click();

                }
                else
                {
                    editpage = wait.Until(drv => drv.FindElement(By.ClassName("w_denpyo_l")));
                    editpage.Click();
                }
                ChromeDriverUtil.sleep();
                var meisaiWindow = wait.Until(drv => drv.WindowHandles.Last());
                wait.Until(drv => drv.SwitchTo().Window(meisaiWindow));
                _logger.Info("楽楽清算-通勤費画面");
                //すでに作成済みの日付を取得
                var daylists = wait.Until(drv => drv.FindElements(By.ClassName("labelColorDefault")));
                daylist.AddRange(daylists.Select(r => r.Text));

                //マイパターンクリック
                editpage = wait.Until(drv => drv.FindElements(By.CssSelector("[class=\"meisai-insert-button\"]"))[1]);
                editpage.Click();

                ChromeDriverUtil.sleep();

                window = wait.Until(drv => drv.WindowHandles.Last());
                wait.Until(drv => drv.SwitchTo().Window(window));


                //チェックボックスを取得
                var tr = wait.Until(drv => drv.FindElements(By.ClassName("d_hover")));

                List<RakuPtn> list = new List<RakuPtn>();
                foreach (var item in tr)
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
                if (model != null)
                {
                    for (int i = 0; i < model.Kintais.Count(); i++)
                    {
                        if (model.Kintais[i].Rakutrue != false && !(string.IsNullOrEmpty(model.Kintais[i].RakuPtn)) && !(daylist.Where(r => r.StartsWith(model.Kintais[i].Date.ToString("d"))).Any()))
                        {
                            //チェックボックスを取得
                            var chks = wait.Until(drv => drv.FindElements(By.Name("kakutei")));

                            foreach (var chk in chks)
                            {
                                if (chk.GetAttribute("value") == model.Kintais[i].RakuPtn)
                                {
                                    chk.Click();
                                    break;
                                }
                            }

                            //次へクリック
                            var nextbtn = wait.Until(drv => drv.FindElement(By.CssSelector("[class=\"common-btn accesskeyFix kakutei d_marginLeft5\"]")));
                            nextbtn.Click();

                            ChromeDriverUtil.sleep();
                            //日付入力meisaiDate
                            var Dateinput = wait.Until(drv => drv.FindElements(By.Name("meisaiDate"))[1]);
                            Dateinput.SendKeys(model.Kintais[i].Date.ToString("d"));

                            //明細追加押下
                            nextbtn = wait.Until(drv => drv.FindElement(By.CssSelector("[class=\"button button--l button-primary accesskeyFix kakutei\"]")));
                            nextbtn.Click();

                            ChromeDriverUtil.sleep();
                            //window = wait.Until(drv => drv.WindowHandles.Last());
                            wait.Until(drv => drv.SwitchTo().Window(meisaiWindow));

                            if (i != model.Kintais.Count())
                            {

                                //マイパターンクリック
                                editpage = wait.Until(drv => drv.FindElements(By.CssSelector("[class=\"meisai-insert-button\"]"))[1]);
                                editpage.Click();

                                ChromeDriverUtil.sleep();

                                window = wait.Until(drv => drv.WindowHandles.Last());
                                wait.Until(drv => drv.SwitchTo().Window(window));
                            }


                        }
                        if (model.Kintais[i].Rakutrue != false && !(string.IsNullOrEmpty(model.Kintais[i].RakuPtn2)) && !(daylist.Where(r => r.StartsWith(model.Kintais[i].Date.ToString("d"))).Any()))
                        {
                            //チェックボックスを取得
                            var chks = wait.Until(drv => drv.FindElements(By.Name("kakutei")));

                            foreach (var chk in chks)
                            {
                                if (chk.GetAttribute("value") == model.Kintais[i].RakuPtn2)
                                {
                                    chk.Click();
                                    break;
                                }
                            }

                            //次へクリック
                            var nextbtn = wait.Until(drv => drv.FindElement(By.CssSelector("[class=\"common-btn accesskeyFix kakutei d_marginLeft5\"]")));
                            nextbtn.Click();

                            ChromeDriverUtil.sleep();

                            //日付入力meisaiDate
                            var Dateinput = wait.Until(drv => drv.FindElements(By.Name("meisaiDate"))[1]);
                            Dateinput.SendKeys(model.Kintais[i].Date.ToString("d"));
                            ChromeDriverUtil.sleep();
                            //明細追加押下
                            nextbtn = wait.Until(drv => drv.FindElement(By.CssSelector("[class=\"button button--l button-primary accesskeyFix kakutei\"]")));
                            nextbtn.Click();

                            ChromeDriverUtil.sleep();
                            //window = wait.Until(drv => drv.WindowHandles.Last());
                            wait.Until(drv => drv.SwitchTo().Window(meisaiWindow));

                            if (i != model.Kintais.Count())
                            {

                                //マイパターンクリック
                                editpage = wait.Until(drv => drv.FindElements(By.CssSelector("[class=\"meisai-insert-button\"]"))[1]);
                                editpage.Click();

                                ChromeDriverUtil.sleep();

                                window = wait.Until(drv => drv.WindowHandles.Last());
                                wait.Until(drv => drv.SwitchTo().Window(window));
                            }


                        }

                    }
                    //最後追加から入力がないパターンの考慮
                    if (wait.Until(drv => drv.FindElements(By.CssSelector("[class=\"common-btn accesskeyClose\"]"))).Count() > 0)
                    {
                        var closebtn = wait.Until(drv => drv.FindElement(By.CssSelector("[class=\"common-btn accesskeyClose\"]")));
                        closebtn.Click();
                        ChromeDriverUtil.sleep();
                        window = wait.Until(drv => drv.WindowHandles.Last());
                        wait.Until(drv => drv.SwitchTo().Window(window));
                    }

                    //一次保存押下
                    if (wait.Until(drv => drv.FindElements(By.CssSelector("[class=\"button save accesskeyReturn\"]"))).Count() > 0)
                    {
                        var savebtn = wait.Until(drv => drv.FindElement(By.CssSelector("[class=\"button save accesskeyReturn\"]")));
                        savebtn.Click();
                    }
                }
                ChromeDriverUtil.sleep();
                ChromeDriverUtil.ChromeEnd(chrome);
                return new SelectList(list, "Id", "PtnName");
            }
            catch (SystemException e)
            {
                _logger.Error(e.StackTrace);
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

            ChromeDriverUtil.sleep();


            var frame = wait.Until(drv => drv.FindElement(By.Name("main")));
            chrome.SwitchTo().Frame(frame);
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

            ChromeDriverUtil.sleep();

            //勤務表へ移動
            var editpage = wait.Until(drv => drv.FindElement(By.LinkText("勤務表")));

            editpage.Click();
            ChromeDriverUtil.sleep();

        }
        #endregion


        #region ログイン情報読み込み
        private void LoginReadText()
        {
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
            }

        }
        #endregion



        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        
    }
}

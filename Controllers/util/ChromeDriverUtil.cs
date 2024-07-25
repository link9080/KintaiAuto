using NLog;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using System;
using System.Threading;

namespace KintaiAuto.Controllers.util
{
    public class ChromeDriverUtil
    {

        private static ChromeOptions option = new ChromeOptions();
        static Logger _logger = LogManager.GetCurrentClassLogger();


        public static ChromeDriver Driver()
        {
            if (OperatingSystem.IsLinux())
            {
                option.AddArgument("--headless");
                //option.ImplicitWaitTimeout = TimeSpan.FromSeconds(40);
            }
            else
            {
                //option.ImplicitWaitTimeout = TimeSpan.FromSeconds(1);
            }
            
            return new ChromeDriver(option);
        }
        public static WebDriverWait waitter(ChromeDriver driver)
        {
            var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(60));
            return wait;
        }



        public static void ChromeEnd(ChromeDriver chrome)
        {
            _logger.Debug("chrome終了");
            chrome.Quit();
            chrome.Dispose();
        }

        public static void sleep()
        {
            
            if (OperatingSystem.IsLinux())
            {
                Thread.Sleep(TimeSpan.FromSeconds(5));
            }
            else
            {
                Thread.Sleep(TimeSpan.FromSeconds(2));
            }
        }

    }

}

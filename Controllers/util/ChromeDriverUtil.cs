using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using System;

namespace KintaiAuto.Controllers.util
{
    public class ChromeDriverUtil
    {

        private static ChromeOptions option = new ChromeOptions();


        public static ChromeDriver Driver()
        {
            if (OperatingSystem.IsLinux())
            {
                option.AddArgument("--headless");
            }
            option.ImplicitWaitTimeout = TimeSpan.FromSeconds(1);
            return new ChromeDriver(option);
        }
        public static WebDriverWait waitter(ChromeDriver driver)
        {
            var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));
            return wait;
        }



        public static void ChromeEnd(ChromeDriver chrome)
        {
            chrome.Quit();
            chrome.Dispose();
        }
    }

}

using OpenQA.Selenium.Edge;
using OpenQA.Selenium.Support.UI;
using OpenQA.Selenium;
using System;
using System.Threading;
using HtmlAgilityPack;

namespace ParserAvalonia.Services
{
    public class FetchDataService
    {
        private readonly EdgeDriverService _driverService;
        private readonly EdgeOptions _driverOptions = new();
        private const long STEP_FOR_SCROLLING = 1000;

        public FetchDataService()
        {
            _driverOptions.AddArgument("--headless=new");
            _driverOptions.AddArgument("--disable-gpu");
            _driverOptions.AddArgument("--no-sandbox");

            _driverService = EdgeDriverService.CreateDefaultService();
            _driverService.HideCommandPromptWindow = true;
        }

        public string FetchData(string urlString)
        {
            if (string.IsNullOrWhiteSpace(urlString)) return null;
            if (!Uri.IsWellFormedUriString(urlString, UriKind.Absolute)) return null; // TODO: Зробити екзепшн йоууу
            return GetDataForParse(urlString);
        }

        private string GetDataForParse(string urlString)
        {
            string pageSourceString = "";
            using (IWebDriver driver = new EdgeDriver(_driverService, _driverOptions))
            {
                driver.Navigate().GoToUrl(urlString);
                pageSourceString = ScrollPage(driver);
            }

            return pageSourceString;
        }

        private string ScrollPage(IWebDriver driver)
        {
            var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));
            wait.Until(d => ((IJavaScriptExecutor)d).ExecuteScript("return document.readyState").Equals("complete"));
            Thread.Sleep(1200);


            long height = (long)((IJavaScriptExecutor)driver).ExecuteScript("return document.body.scrollHeight;");
            long secondIterator = STEP_FOR_SCROLLING;
            long firstIterator = 0;
            string pageSource = "";


            while (firstIterator < height)
            {
                ((IJavaScriptExecutor)driver).ExecuteScript($"window.scrollTo({firstIterator}, {secondIterator});");
                Thread.Sleep(300);
                firstIterator += STEP_FOR_SCROLLING;
                secondIterator += STEP_FOR_SCROLLING;
                pageSource += driver.PageSource;
            }

            return pageSource;
        }
    }
}

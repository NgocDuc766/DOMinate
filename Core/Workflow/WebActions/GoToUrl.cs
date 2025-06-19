using Newtonsoft.Json.Linq;
using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Workflow.WebActions
{
    public class GoToUrl : BaseAction
    {
        public GoToUrl(IWebDriver driver) : base(driver)
        {
        }

        public override JObject GetDefaultPayload()
        {
            return new JObject
            {
                ["url"] = "https://google.com"
            };
        }

        protected override dynamic PerformAction()
        {
            ValidatePayload();
            string url = Payload["url"].ToString();
            WebDriver.Navigate().GoToUrl(url);
            return $"Navigate to {url} performed!";
        }

        protected internal override void ValidatePayload()
        {
            if (string.IsNullOrEmpty(Payload["url"].ToString()) || !Payload.ContainsKey("url"))
            {
                throw new ArgumentException("URL is required!");
            }
        }
    }
}

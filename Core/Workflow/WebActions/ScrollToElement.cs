using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using OpenQA.Selenium;

namespace Core.Workflow.WebActions
{
    public class ScrollToElement : BaseAction
    {
        public ScrollToElement(IWebDriver driver) : base(driver)
        {
        }

        public override JObject GetDefaultPayload()
        {
            return new JObject
            {
                ["DelayTime"] = 1000,
                ["Xpath"] = "//div"
            };
        }

        protected override dynamic PerformAction()
        {
            ValidatePayload();
            HandleScrollToElement(Payload["DelayTime"].ToString(), Payload["Xpath"].ToString());
            return "Scroll to element performed";
        }

        protected internal override void ValidatePayload()
        {
            if (!Payload.ContainsKey("DelayTime"))
            {
                throw new ArgumentException("DelayTime is required!");
            }
            if (!int.TryParse(Payload["DelayTime"].ToString(), out int delay) || delay < 0)
            {
                throw new ArgumentException("DelayTime must be a non-negative integer!");
            }
            if (!Payload.ContainsKey("Xpath"))
            {
                throw new ArgumentException("Xpath is required!");
            }
            if (string.IsNullOrEmpty(Payload["Xpath"].ToString()))
            {
                throw new ArgumentException("Xpath cannot be empty!");
            }
        }

        private void HandleScrollToElement(string delayStr, string xpath)
        {
            int delay = int.Parse(delayStr);
            IJavaScriptExecutor js = (IJavaScriptExecutor)WebDriver;
            var element = WebDriver.FindElement(By.XPath(xpath));
            js.ExecuteScript("arguments[0].scrollIntoView(true);", element);
            System.Threading.Thread.Sleep(delay);
        }
    }
}
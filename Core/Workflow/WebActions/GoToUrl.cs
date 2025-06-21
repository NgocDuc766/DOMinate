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
            string url = Payload["Url"].ToString();
            WebDriver.Navigate().GoToUrl(url);
            // delay
            int delayTime = int.Parse(Payload["Delay"].ToString());
            System.Threading.Thread.Sleep(delayTime);
            
            return $"Navigate to {url} performed!";
        }

        protected internal override void ValidatePayload()
        {
            if (string.IsNullOrEmpty(Payload["Url"].ToString()) || !Payload.ContainsKey("Url"))
            {
                throw new ArgumentException("URL is required!");
            }
            if (!int.TryParse(Payload["Delay"].ToString(), out int delay) || delay < 0)
            {
                throw new ArgumentException("Delay must be a non-negative integer!");
            }
        }
    }
}

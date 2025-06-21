using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using OpenQA.Selenium;


namespace Core.Workflow.WebActions
{
    public class ScrollToTop : BaseAction
    {

        public override JObject GetDefaultPayload()
        {
            return new JObject
            {
                ["Delay"] = 1000
            };
        }

        protected override dynamic PerformAction()
        {
            ValidatePayload();
            HandleScrollToTop(Payload["Delay"].ToString());
            return "Scroll to top performed";
        }

        protected internal override void ValidatePayload()
        {
            if (!Payload.ContainsKey("Delay"))
            {
                throw new ArgumentException("Delay is required!");
            }
            if (!int.TryParse(Payload["Delay"].ToString(), out int delay) || delay < 0)
            {
                throw new ArgumentException("Delay must be a non-negative integer!");
            }
        }

        private void HandleScrollToTop(string delayStr)
        {
            int delay = int.Parse(delayStr);
            IJavaScriptExecutor js = (IJavaScriptExecutor)WebDriver;
            js.ExecuteScript("window.scrollTo(0, 0);");
            System.Threading.Thread.Sleep(delay);
        }
    }
}
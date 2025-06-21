using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using OpenQA.Selenium;

namespace Core.Workflow.WebActions
{
    public class RandomScroll : BaseAction
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
            HandleRandomScroll(Payload["Delay"].ToString());
            return "Random scroll performed";
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

        private void HandleRandomScroll(string delayStr)
        {
            int delay = int.Parse(delayStr);
            IJavaScriptExecutor js = (IJavaScriptExecutor)WebDriver;
            Random random = new Random();
            int direction = random.Next(2); // 0 cho page up, 1 cho page down

            if (direction == 0)
            {
                js.ExecuteScript("window.scrollBy(0, -window.innerHeight);"); // Cuộn lên
            }
            else
            {
                js.ExecuteScript("window.scrollBy(0, window.innerHeight);"); // Cuộn xuống
            }

            System.Threading.Thread.Sleep(delay);
        }
    }
}
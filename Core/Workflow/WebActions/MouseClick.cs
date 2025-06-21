using Newtonsoft.Json.Linq;
using OpenQA.Selenium;
using OpenQA.Selenium.Interactions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Workflow.WebActions
{
    public class MouseClick : BaseAction
    {

        public override JObject GetDefaultPayload()
        {
            return new JObject
            {
                ["ClickType"] = "Xpath",
                ["Xpath"] = "//button"
            };
        }

        protected override dynamic PerformAction()
        {
            HandleWithClickType(Payload["ClickType"].ToString());
            return "Mouse click performed";
        }

        protected internal override void ValidatePayload()
        {
            if (!Payload.ContainsKey("ClickType"))
            {
                throw new ArgumentException("ClickType is required!");
            }
            if (!Payload.ContainsKey("Xpath") && Payload["ClickType"].ToString().Equals("Xpath"))
            {
                throw new ArgumentException("Xpath is required!");
            }
            if (!Payload.ContainsKey("Coordinates") && Payload["ClickType"].ToString().Equals("Coordinates"))
            {
                throw new ArgumentException("Coordinates is required!");
            }
            if (!int.TryParse(Payload["Delay"].ToString(), out int delay) || delay < 0)
            {
                throw new ArgumentException("Delay must be a non-negative integer!");
            }
        }

        private void HandleWithClickType(string clickType)
        {
            if (clickType == "Xpath")
            {
                // parse xpath
                string xpath = Payload["Xpath"].ToString();
                // find xpath
                WebDriver.FindElement(By.XPath(xpath)).Click();
            }
            else if (clickType == "Coordinates")
            {
                // retrieve coordinate
                string coordinates = Payload["Coordinates"].ToString();
                // split x and y
                var parseCoordinate = coordinates.Split(',');
                // check if coordinates valid
                if (parseCoordinate.Length != 2 || int.TryParse(parseCoordinate[0], out int x)
                    || int.TryParse(parseCoordinate[1], out int y))
                {
                    throw new ArgumentException("Coordinates must be in format 'x,y' with valid integers");
                }

                // Execute click by coordinates
                Actions actions = new Actions(WebDriver);
                actions.MoveByOffset(x, y).Click().Perform();
            }
            // delay
            int delayTime = int.Parse(Payload["Delay"].ToString());
            System.Threading.Thread.Sleep(delayTime);
        }
    }
}

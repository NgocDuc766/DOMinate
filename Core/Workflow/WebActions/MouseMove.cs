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
    public class MouseMove : BaseAction
    {
        public MouseMove(IWebDriver driver) : base(driver)
        {
        }

        public override JObject GetDefaultPayload()
        {
            throw new NotImplementedException();
        }

        protected override dynamic PerformAction()
        {
            ValidatePayload();
            HandleWithMoveType(Payload["MoveType"].ToString());
            return "Mouse click performed";
        }

        protected internal override void ValidatePayload()
        {
            if (!Payload.ContainsKey("MoveType"))
            {
                throw new ArgumentException("ClickType is requred!");
            }
            if (!Payload.ContainsKey("Xpath"))
            {
                throw new ArgumentException("Xpath is requred!");
            }
            if (!Payload.ContainsKey("Coordinates"))
            {
                throw new ArgumentException("Coordinates is required!");
            }
        }

        private void HandleWithMoveType(string clickType)
        {
            Actions actions = new Actions(WebDriver);
            if (clickType == "Xpath")
            {
                string xpath = Payload["Xpath"].ToString();
                // find element by xpath
                IWebElement element = WebDriver.FindElement(By.XPath(xpath));
                //move mouse to specific element
                actions.MoveToElement(element).Perform();
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

                // move by coordinates              
                actions.MoveByOffset(x, y).Perform();
            }
        }
    }
}

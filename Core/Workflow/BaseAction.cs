using Core.Models;
using Newtonsoft.Json.Linq;
using OpenQA.Selenium;
using OpenQA.Selenium.DevTools.V135.Browser;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Workflow
{
    public abstract class BaseAction
    {
        public Guid Id { get; set; }
        public string ActionName { get; set; }
        public string ActionType { get; set; }
        private JObject _payload;

        public JObject Payload
        {
            get => _payload;
            set => _payload = value;
        }
        public WorkflowModel WorkflowModel { get; set; }
        
        public IWebDriver WebDriver { get; set; }

        // constructor with web driver
        public void SetDriver(IWebDriver driver)
        {
            WebDriver = driver ?? throw new ArgumentNullException(nameof(driver), "Web driver cannot be null!");
        }

        // implement function
        protected abstract dynamic PerformAction();
        protected internal abstract void ValidatePayload();
        public abstract JObject GetDefaultPayload();

        public dynamic Execute()
        {
            if (WebDriver == null)
            {
                throw new NullReferenceException("Web driver cannot be null!");
            }
            Console.WriteLine($"Executing action: ID: '{Id}', Type: '{ActionType}' with payload: '{Payload}' at '{DateTime.Now}'");
            var resultAction = PerformAction();

            Console.WriteLine($"Executing action ID '{Id}' with result is '{resultAction}' at '{DateTime.Now}'");
            return resultAction;
        }

    }
}

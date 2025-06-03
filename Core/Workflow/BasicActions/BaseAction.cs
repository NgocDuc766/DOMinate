using Core.Models;
using Newtonsoft.Json.Linq;
using OpenQA.Selenium.DevTools.V135.Browser;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Workflow.BasicActions
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


        // implement function
        protected abstract dynamic PerformAction();
        protected internal abstract void ValidatePayload();
        public abstract JObject GetDefaultPayload();
    }
}

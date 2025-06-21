using Core.Models;
using System;
using Newtonsoft.Json.Linq;
using OpenQA.Selenium;

namespace Core.Workflow.BasicActions
{
    public class Loop : BaseAction
    {
        protected override dynamic PerformAction()
        {
            // parse workflow từ payload sang workflow object
            WorkflowModel workflowModel = Payload["Workflow"].ToObject<WorkflowModel>()
                ?? throw new ArgumentException("Workflow must provide in payload object");
            
            int iteration = 0;
            
            
            return "...";
        }
        // condition là kết quả của action khác
        // key: condition, value: xpath => cái này được lưu global variables
        // 
        protected internal override void ValidatePayload()
        {
            if (Payload == null)
            {
                throw new ArgumentException(nameof(Payload),"Payload is null");
            }

            if (!Payload.ContainsKey("Workflow") || Payload["Workflow"] == null)
            {
                throw new ArgumentException("Payload does not contain a Workflow object");
            }
        }

        public override JObject GetDefaultPayload() => new JObject();
    }
}

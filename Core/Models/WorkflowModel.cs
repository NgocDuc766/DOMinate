using Core.Workflow.BasicActions;
using OpenQA.Selenium.DevTools.V135.Runtime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Models
{
    public class WorkflowModel
    {
        private Guid _id = Guid.NewGuid();
        private string _name = "New Workflow";
        private List<BaseAction> _workflow = new List<BaseAction>();
        private List<Variables> _variables = new List<Variables>();

        public Guid Id
        {
            get => _id;
            set
            {
                if (value.Equals(_id))
                {
                    _id = value;
                }
            }
        }

        public string Name
        {
            get => _name;
            set
            {
                if (value.Equals(_name))
                {
                    return;
                }
                _name = value;
            }
        }
        
        public List<Variables> Variables
        {
            get => _variables;
            set
            {
                if(value != _variables)
                {
                    _variables = value;
                }
            }
        } 

        public List<BaseAction> Workflow
        {
            get => _workflow;
            set
            {
                if (value != _workflow)
                {
                    _workflow = value;
                }
            }
        }

        public void AddVariables(string key, string value)
        {
            if (string.IsNullOrEmpty(key))
            {
                throw new ArgumentException("Variables key is invalid");
            }
            if(string.IsNullOrEmpty(value))
            {
                throw new ArgumentException($"Variables value for key '{key}' is invalid");
            }
            Variables var = new Variables(key, value);
            Variables.Add(var);
        }

        public void AddAction(BaseAction action)
        {
            if(action == null)
            {
                throw new ArgumentException(nameof(action), "Action cannot be null");
            }
            if(action.Payload == null || !action.Payload.HasValues)
            {
                
            }
        }
    }
}

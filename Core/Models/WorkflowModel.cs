using Core.Workflow;
using Newtonsoft.Json.Linq;
using OpenQA.Selenium;
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
        private List<GlobalVariables> _globalVariables = new List<GlobalVariables>();
        private List<UserData> _userData = new List<UserData>();
        // khởi tạo sẵn 1 web driver cho mỗi 1 web driver
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
        
        public List<GlobalVariables> GlobalVariables
        {
            get => _globalVariables;
            set
            {
                if(value != _globalVariables)
                {
                    _globalVariables = value;
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

        public List<UserData> UserData
        {
            get => _userData;
            set
            {
                if(value != _userData)
                {
                    _userData = value;
                }
            }
        }

        public void AddGlobalVariables(string key, string value)
        {
            if (string.IsNullOrEmpty(key))
            {
                throw new ArgumentException("Variables key is invalid");
            }
            if(string.IsNullOrEmpty(value))
            {
                throw new ArgumentException($"Variables value for key '{key}' is invalid");
            }
            var variable = new GlobalVariables(key, value);
            GlobalVariables.Add(variable);
        }

        public void AddUserData(string key, JToken value)
        {
            if (string.IsNullOrEmpty(key))
            {
                throw new ArgumentException("UserData key is invalid");
            }
            if (value == null)
            {
                throw new ArgumentException($"UserData value for key '{key}' is invalid");
            }
            var userData = new UserData(key, value.ToString());
            UserData.Add(userData);
        }

        public void AddAction(BaseAction action)
        {
            if(action == null)
            {
                throw new ArgumentException(nameof(action), "Action cannot be null");
            }
            if(action.Payload == null || !action.Payload.HasValues)
            {
                action.Payload = action.GetDefaultPayload();
            }

            action.ValidatePayload();

            Workflow.Add(action);
        }

        public void ParseWorkflow(string jsonString)
        {
            try
            {
                JObject parsedJson = JObject.Parse(jsonString);
                // check if jsonString is empty or parsedJson is null
                if(parsedJson == null || string.IsNullOrEmpty(jsonString))
                {
                    throw new ArgumentException("JSON string is null or empty");
                }
                // if id and name are not null, then parse into Id and Name
                if (parsedJson["Id"] != null)
                {
                    Id = Guid.Parse(parsedJson["Id"].ToString());
                }

                if (parsedJson["Name"] != null)
                {
                    Name = parsedJson["Name"].ToString();
                }

                // handle Variables
                var variablesData = parsedJson["GlobalVariables"];
                if (variablesData != null)
                {
                    GlobalVariables.Clear();
                    foreach (var variable in (JObject)variablesData)
                    {
                        string key = variable.Key;
                        JToken value = variable.Value;
                        if (string.IsNullOrEmpty(key))
                        {
                            throw new ArgumentException("GlobalVariables key is not null!");
                        }
                        if(value == null)
                        {
                            throw new ArgumentException($"GlobalVariables value for key {key} is not null!");
                        }
                        // add GlobalVariables
                        AddGlobalVariables(key, value.ToString());
                    }   
                }

                // handle UserData
                var userData = parsedJson["UserData"];
                if(userData != null)
                {
                    UserData.Clear();
                    foreach(var user in (JObject)userData)
                    {
                        string key =  user.Key;
                        JToken value = user.Value;
                        if(string.IsNullOrEmpty(key))
                        {
                            throw new ArgumentException("UserData key is null or empty");
                        }
                        if(value == null)
                        {
                            throw new ArgumentException($"UserData value for key {key} is null or empty");
                        }
                        AddUserData(key, value);
                    }
                }

                // handle workflow
                var workflow = parsedJson["Workflow"];
                if(workflow != null)
                {
                    // clear workflow
                    Workflow.Clear();
                    // loop and retrieve each action in workflow
                    foreach(var action in workflow)
                    {
                        string actionId = action["Id"]?.ToString();
                        string actionName = action["ActionName"]?.ToString();
                        string actionType = action["Type"]?.ToString();
                        JObject payload = action["Payload"] as JObject;

                        if (string.IsNullOrEmpty(actionId))
                        {
                            throw new ArgumentException("Action ID is null or empty");
                        }
                        if (string.IsNullOrEmpty(actionType))
                        {
                            throw new ArgumentException($"Action Type for '{actionId}' is null or empty");
                        }
                        if (payload == null)
                        {
                            throw new ArgumentException($"Action payload for '{actionId}' is null or empty");
                        }
                        var workflowAction = CreateAction(Id, actionName, actionType, payload, this);
                        AddAction(workflowAction);
                    }
                }
            }
            catch (Exception e)
            {
                string errMsg = $"Error parsing workflow: {e.Message}";
                Console.WriteLine(errMsg);
                throw new Exception(errMsg, e);
            }
        }

        public static BaseAction CreateAction(Guid actionId, string actionName, string actionType, JObject payload,
            WorkflowModel workflowModel)
        {
            // get the current running assembly
            var assembly = System.Reflection.Assembly.GetExecutingAssembly();
            // map the type of action with the namespaces
            string[] namespaces = new[]
            {
                "Core.Workflow.WebActions",
                "Core.Workflow.BasicActions"
            };

            Type type = null;
            foreach(var ns in namespaces)
            {
                type = assembly.GetType($"{ns}.{actionType}");
                if(type != null || typeof(BaseAction).IsAssignableFrom(type))
                {
                    break;
                }
            }
            if(type == null )
            {
                throw new NotSupportedException($"Action type '{actionType}' is not found");
            }

            var instance = (BaseAction) Activator.CreateInstance(type);
            instance.Id = actionId;
            instance.ActionName = actionName;
            instance.ActionType = actionType;
            instance.Payload = payload;
            instance.WorkflowModel = workflowModel;

            if (payload == null || !payload.HasValues || payload.ToString().Equals("{}")) 
            {
                instance.Payload = instance.GetDefaultPayload();
            }

            return instance;
        }


        // parse workflow to Json
        public string ParseToJson()
        {
            var workflowJson = new JObject
            {
                ["Id"] = Id.ToString(),
                ["Name"] = Name,
            };

            // parse global variables
            var globalVariableJson = new JObject();
            foreach(var variable in GlobalVariables)
            {
                globalVariableJson[variable.Key] = variable.Value;
            }
            workflowJson["GlobalVariables"] = globalVariableJson;

            // parse user data
            var userDataJson = new JObject();
            foreach(var userData in UserData)
            {
                userDataJson[userData.Key] = userData.Value;
            }
            workflowJson["UserData"] = userDataJson;

            // parse workflow
            var workflowArray = new JArray();
            foreach(var action in Workflow)
            {
                var actionJson = new JObject()
                {
                    ["Id"]= action.Id.ToString(),
                    ["ActionName"] = action.ActionName,
                    ["ActionType"] = action.ActionType,
                    ["Payload"] = JToken.FromObject(action.Payload)
                };
                workflowArray.Add(actionJson);
            }
            workflowJson["Workflow"] = workflowArray;

            return workflowJson.ToString(Newtonsoft.Json.Formatting.Indented);
        }

    }
}

using Core.Browser;
using Core.Models;
using OpenQA.Selenium;

namespace Core
{
    public class Program
    {
        static void Main(string[] args)
        {
            IWebDriver webDriver = BrowserManager.CreateDriver();
            Console.WriteLine("Start the process...");
            // nhận 1 json string
            string jsonString = "{\"Id\":\"550e8400-e29b-41d4-a716-446655440000\",\"Name\":\"Test Workflow\",\"GlobalVariables\":{},\"UserData\":{},\"Workflow\":[{\"Id\":\"550e8400-e29b-41d4-a716-446655440001\",\"ActionName\":\"NavigateToGoogle\",\"ActionType\":\"GoToUrl\",\"Payload\":{\"Url\":\"https://www.google.com\",\"Delay\":\"1000\"}},{\"Id\":\"550e8400-e29b-41d4-a716-446655440002\",\"ActionName\":\"ClickSearchButton\",\"ActionType\":\"MouseClick\",\"Payload\":{\"ClickType\":\"Xpath\",\"Xpath\":\"/html/body/div[@class='L3eUgb']/div[@class='o3j99']/div[@class='c93Gbe']/div[@class='KxwPGc SSwjIe']/div[@class='KxwPGc AghGtd']/a[@class='pHiOh'][1]\",\"Coordinates\":\"100,200\",\"Delay\":\"1000\"}},{\"Id\":\"550e8400-e29b-41d4-a716-446655440231\",\"ActionName\":\"ScrollToBottom1\",\"ActionType\":\"ScrollToElement\",\"Payload\":{\"Xpath\":\"/html[@class='page']/body[@class='glue-body']/main/div[@id='page-content']/div[@id='google-around-the-globe']/div[@class='glue-page glue-grid']/div[@class='glue-content glue-grid__col glue-grid__col--span-10 glue-grid__col--span-10-md glue-grid__col--span-10-lg glue-grid__col--span-10-xl glue-text-center']/div[@class='glue-grid']/div[@class='glue-grid__col glue-grid__col--span-8 glue-grid__col--span-10-md']\",\"Delay\":\"2000\"}},{\"Id\":\"550e8400-e29b-41d4-a716-446655440002\",\"ActionName\":\"ClickSearchButton\",\"ActionType\":\"MouseClick\",\"Payload\":{\"ClickType\":\"Xpath\",\"Xpath\":\"/html[@class='page']/body[@class='glue-body']/main/div[@id='page-content']/div[@id='google-around-the-globe']/div[@class='glue-page glue-grid']/div[@class='glue-content glue-grid__col glue-grid__col--span-10 glue-grid__col--span-10-md glue-grid__col--span-10-lg glue-grid__col--span-10-xl glue-text-center']/div[@class='glue-grid']/div[@class='glue-grid__col glue-grid__col--span-8 glue-grid__col--span-10-md']/div[@class='cta-container glue-spacer-4-top centered']/a[@class='glue-button glue-button--high-emphasis']\",\"Delay\":\"1000\"}}]}";
            if(jsonString == null)
            {
                throw new ArgumentException("jsonString is not null!");
            }
            // tạo mới 1 workflow
            WorkflowModel workflowModel = new WorkflowModel();
            // chạy workflow
            workflowModel.ParseWorkflow(jsonString);
            
            if(workflowModel.Workflow == null || !workflowModel.Workflow.Any() ) 
            {
                Console.WriteLine("No actions to execute in the workflow");
                return;
            }
            // sẽ có 1 hàm executor nhận các biến như workflow, webdriver, inputdata
            // không thể biến web driver thành 1 thuộc tính của workflow được...
            Executor(webDriver, workflowModel);

            Console.WriteLine("End the process.");
            // end
        }

        public static void Executor(IWebDriver webDriver, WorkflowModel workflowModel)
        {
            foreach(var action in workflowModel.Workflow)
            {
                try
                {
                    var startTime = DateTime.Now;
                    Console.WriteLine($"Action {action.ActionName} is executing");
                    
                    action.SetDriver(webDriver);
                    action.Execute();
                    
                    var duration = DateTime.Now - startTime;
                    Console.WriteLine($"Action {action.ActionName} completed in {duration.TotalMilliseconds} ms");
                }
                catch(Exception ex) {
                    Console.Error.WriteLine($"Failed to execute action '{action.ActionName}': {ex.Message}");
                }
            }
        }
    }
}

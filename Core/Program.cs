using Core.Models;

namespace Core
{
    public class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Start the process...");
            // nhận 1 json string
            string jsonString = Console.ReadLine();
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

            foreach(var action in workflowModel.Workflow)
            {
                // sẽ có 1 hàm executor nhận các biến như workflow, webdriver, inputdata
                // không thể biến web driver thành 1 thuộc tính của workflow được...
                try
                {
                    var startTime = DateTime.Now;
                    Console.WriteLine($"Action {action.ActionName} is executing");
                    action.Execute();
                    var duration = DateTime.Now - startTime;
                    Console.WriteLine($"Action {action.ActionName} completed in {duration.TotalMilliseconds} ms");
                }
                catch(Exception ex) {
                    Console.Error.WriteLine($"Failed to execute action '{action.ActionName}': {ex.Message}");
                }
                
            }
            Console.WriteLine("End the process.");
            // end
        }
    }
}

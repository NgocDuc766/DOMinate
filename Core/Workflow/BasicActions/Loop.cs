using Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Workflow.BasicActions
{
    public class LoopExecutor
    {
        private readonly WorkflowModel _workflow;
        private readonly int _times;

        public LoopExecutor(WorkflowModel workflow, int times)
        {
            _workflow = workflow ?? throw new ArgumentNullException(nameof(workflow));
            _times = times;
        }

        public void Execute()
        {
            for (int i = 0; i < _times; i++)
            {
                Console.WriteLine($"🔁 Thực hiện loop lần thứ {i + 1}");

                foreach (var action in _workflow.Workflow)
                {
                    try
                    {
                        Console.WriteLine($"▶️  Thực hiện action: {action.ActionName} ({action.ActionType})");
                        // action.PerformAction(); // dùng hàm sẵn có
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"❌ Lỗi khi thực hiện action {action.ActionType}: {ex.Message}");
                    }
                }
            }

            Console.WriteLine("✅ Hoàn tất lặp workflow.");
        }
    }
}

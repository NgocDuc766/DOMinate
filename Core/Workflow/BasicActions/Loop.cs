using Core.Models;
using System;

namespace Core.Workflow.BasicActions
{
    public class Loop
    {
        private readonly WorkflowModel _workflow;
        private readonly int _times;

        public Loop(WorkflowModel workflow, int times)
        {
            _workflow = workflow ?? throw new ArgumentNullException(nameof(workflow));
            _times = times <= 0 ? throw new ArgumentException("Số lần lặp phải lớn hơn 0") : times;
        }

        public void Execute()
        {
            for (int i = 0; i < _times; i++)
            {
                Console.WriteLine($" Lặp lần thứ {i + 1}");

                foreach (var action in _workflow.Workflow)
                {
                    try
                    {
                        Console.WriteLine($" Thực hiện action: {action.ActionName} ({action.ActionType})");

                        var result = action.GetType()
                                           .GetMethod("PerformAction", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                                           ?.Invoke(action, null);

                        Console.WriteLine($" Kết quả: {result}");
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($" Lỗi ở action {action.ActionType}: {ex.Message}");
                    }
                }
            }

            Console.WriteLine(" Đã hoàn thành tất cả các vòng lặp.");
        }
    }
}

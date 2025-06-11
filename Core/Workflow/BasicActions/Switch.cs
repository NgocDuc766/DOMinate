using System;
using System.Collections.Generic;

namespace Core.Workflow.BasicActions
{
    public class Switch
    {
        private readonly string _inputCondition;
        private readonly Dictionary<string, List<BaseAction>> _branches;

        public Switch(string inputCondition, Dictionary<string, List<BaseAction>> branches)
        {
            _inputCondition = inputCondition ?? throw new ArgumentNullException(nameof(inputCondition));
            _branches = branches ?? throw new ArgumentNullException(nameof(branches));
        }

        public void Execute()
        {
            if (_branches.TryGetValue(_inputCondition, out var selectedActions))
            {
                Console.WriteLine($" Khớp điều kiện '{_inputCondition}'. Thực thi {selectedActions.Count} actions:");

                foreach (var action in selectedActions)
                {
                    try
                    {
                        Console.WriteLine($"▶  Thực hiện action: {action.ActionName} ({action.ActionType})");

                        var result = action.GetType()
                                           .GetMethod("PerformAction", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                                           ?.Invoke(action, null);

                        Console.WriteLine($" Kết quả: {result}");
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($" Lỗi khi thực hiện action {action.ActionType}: {ex.Message}");
                    }
                }
            }
            else
            {
                Console.WriteLine($" Không tìm thấy nhánh phù hợp cho điều kiện: '{_inputCondition}'.");
            }
        }
    }
}

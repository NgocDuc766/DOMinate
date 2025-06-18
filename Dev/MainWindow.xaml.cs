using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace Dev
{
    public partial class MainWindow : Window
    {
        private Button selectedButton = null; // Button đang được chọn

        public MainWindow()
        {
            InitializeComponent();
            this.KeyDown += MainWindow_KeyDown;
        }

        // Xoá dòng chứa nút đang chọn nếu nhấn Delete
        private void MainWindow_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Delete && selectedButton != null)
            {
                var parentRow = ActionListPanel.Children
                    .OfType<StackPanel>()
                    .FirstOrDefault(sp => sp.Children.Contains(selectedButton));

                if (parentRow != null && parentRow.Tag?.ToString() != "Default")
                {
                    ActionListPanel.Children.Remove(parentRow);
                    selectedButton = null;
                }
            }
        }

        private void btn_add_action_Click(object sender, RoutedEventArgs e)
        {
            ActionPopup.IsOpen = true; // Mở popup
        }

        private void btn_cancel_Click(object sender, RoutedEventArgs e)
        {
            ActionPopup.IsOpen = false; // Đóng popup
        }

        private void btn_confirm_Click(object sender, RoutedEventArgs e)
        {
            if (ActionComboBox.SelectedItem != null)
            {
                string selectedAction = (ActionComboBox.SelectedItem as ComboBoxItem)?.Content.ToString();
                var insertionReference = selectedButton;

                int newDepth = 0;
                Thickness buttonMargin;

                // Xác định độ sâu mới (nếu có cha)
                if (insertionReference != null && insertionReference.Tag is int selectedDepth)
                {
                    newDepth = selectedDepth + 1;
                    buttonMargin = new Thickness(20 * newDepth, 5, 0, 5);
                }
                else
                {
                    buttonMargin = new Thickness(10, 5, 0, 5);
                }

                // Tạo action button
                Button actionButton = new Button
                {
                    Content = selectedAction,
                    Tag = newDepth,
                    Margin = buttonMargin,
                    Padding = new Thickness(10),
                    Background = new SolidColorBrush(Color.FromRgb(29, 35, 38)),
                    Foreground = Brushes.White,
                    FontWeight = FontWeights.Bold
                };

                actionButton.Click += (s, ev) =>
                {
                    selectedButton = (Button)s;
                    HighlightSelectedButton();
                };

                // Header panel ngang
                StackPanel headerPanel = new StackPanel
                {
                    Orientation = Orientation.Horizontal
                };

                // Action row dọc
                StackPanel actionRow = new StackPanel
                {
                    Orientation = Orientation.Vertical,
                    Margin = new Thickness(0),
                    Background = Brushes.Transparent,
                    Tag = newDepth
                };

                // Tạo toggle button nếu cần
                Button toggleButton = null;

                if (insertionReference != null && insertionReference.Tag is int selectedDepth2)
                {
                    // Tìm StackPanel dòng cha chứa nút được chọn
                    StackPanel parentRow = ActionListPanel.Children
                        .OfType<StackPanel>()
                        .FirstOrDefault(sp => sp.Children
                            .OfType<StackPanel>()
                            .Any(hp => hp.Children.Contains(insertionReference)));

                    if (parentRow != null)
                    {
                        StackPanel parentHeaderPanel = parentRow.Children[0] as StackPanel;

                        // Chỉ tạo toggle nếu chưa có
                        if (parentHeaderPanel != null && parentHeaderPanel.Children.Count == 1)
                        {
                            toggleButton = new Button
                            {
                                Content = "▼",
                                Width = 20,
                                Height = 20,
                                Margin = new Thickness(0, 0, 5, 0),
                                Background = Brushes.White,
                                BorderBrush = Brushes.White,
                                Foreground = Brushes.White,
                                FontWeight = FontWeights.Bold
                            };

                            parentHeaderPanel.Children.Insert(0, toggleButton);

                            // Gắn sự kiện toggle
                            toggleButton.Click += (s, ev) =>
                            {
                                bool isExpanded = toggleButton.Content.ToString() == "▼";
                                toggleButton.Content = isExpanded ? "▶" : "▼";

                                int parentIndex = ActionListPanel.Children.IndexOf(parentRow);
                                int parentRowDepth = (parentRow.Tag is int d) ? d : 0;

                                for (int i = parentIndex + 1; i < ActionListPanel.Children.Count; i++)
                                {
                                    if (ActionListPanel.Children[i] is StackPanel childRow && childRow.Tag is int childDepth)
                                    {
                                        if (childDepth > parentRowDepth)
                                            childRow.Visibility = isExpanded ? Visibility.Collapsed : Visibility.Visible;
                                        else break;
                                    }
                                }
                            };
                        }
                    }
                }

                // Nếu không có toggle → thêm placeholder ẩn
                if (toggleButton == null)
                {
                    toggleButton = new Button
                    {
                        Width = 20,
                        Height = 20,
                        Margin = new Thickness(0, 0, 5, 0),
                        Visibility = Visibility.Hidden,
                        Background = Brushes.Transparent,
                        BorderBrush = Brushes.Transparent
                    };
                }

                headerPanel.Children.Add(toggleButton);
                headerPanel.Children.Add(actionButton);
                actionRow.Children.Add(headerPanel);

                // Tìm vị trí chèn thích hợp
                if (insertionReference != null)
                {
                    StackPanel parentRow = ActionListPanel.Children
                        .OfType<StackPanel>()
                        .FirstOrDefault(sp => sp.Children
                            .OfType<StackPanel>()
                            .Any(hp => hp.Children.Contains(insertionReference)) || sp.Children.Contains(insertionReference));

                    if (parentRow != null)
                    {
                        int insertIndex = ActionListPanel.Children.IndexOf(parentRow) + 1;

                        // Chèn sau cùng các con (nếu có)
                        for (int i = insertIndex; i < ActionListPanel.Children.Count; i++)
                        {
                            if (ActionListPanel.Children[i] is StackPanel sp && sp.Tag is int depth)
                            {
                                if (depth > newDepth - 1)
                                    insertIndex = i + 1;
                                else break;
                            }
                        }

                        ActionListPanel.Children.Insert(insertIndex, actionRow);
                        return;
                    }
                }

                // Nếu không có cha → thêm cuối danh sách
                ActionListPanel.Children.Add(actionRow);
            }
            else
            {
                MessageBox.Show("Please select an action before adding!");
            }
        }




        // Click các nút mặc định (ví dụ: "Before browser open")
        private void DefaultButton_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            selectedButton = button;
            HighlightSelectedButton();
        }

        // Tô đậm nút đang chọn, xóa tô của các nút khác
        private void HighlightSelectedButton()
        {
            foreach (var sp in ActionListPanel.Children.OfType<StackPanel>())
            {
                foreach (var b in sp.Children.OfType<Button>())
                {
                    b.Background = new SolidColorBrush(Color.FromRgb(29, 35, 38));
                }
            }

            if (selectedButton != null)
            {
                selectedButton.Background = new SolidColorBrush(Color.FromRgb(50, 50, 50));
            }
        }
    }
}

using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Dev
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {

        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {

        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {

        }

        private void btn_add_action_Click(object sender, RoutedEventArgs e)
        {
            ActionPopup.IsOpen = true; // Hiển thị popup khi nhấn nút
        }

        private void btn_cancel_Click(object sender, RoutedEventArgs e)
        {
            ActionPopup.IsOpen = false; // Đóng popup khi nhấn Cancel
        }

        private void btn_confirm_Click(object sender, RoutedEventArgs e)
        {
            if (ActionComboBox.SelectedItem != null)
            {
                string selectedAction = (ActionComboBox.SelectedItem as ComboBoxItem)?.Content.ToString();
                MessageBox.Show($"Action '{selectedAction}' added to workflow!");
                ActionPopup.IsOpen = false;
            }
            else
            {
                MessageBox.Show("Please select an action before adding!");
            }
        }

    }
}
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;

namespace WPF_ZipByLimit.Views
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

        //设置每一行行首的Index
        private void DG_ZipFilesInFolders_LoadingRow(object sender, System.Windows.Controls.DataGridRowEventArgs e)
        {
            e.Row.Header = (e.Row.GetIndex() + 1).ToString();
        }

        //设置在TextBox中只允许输入整数
        private void TextBox_AllowIntegerOnly(object sender, System.Windows.Input.TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }

        //面板展开
        private void DetialExpander_Expanded(object sender, RoutedEventArgs e)
        {
            if (sender is Expander expander)
            {
                var row = DataGridRow.GetRowContainingElement(expander);
                row.DetailsVisibility = Visibility.Visible;
            }
        }
        //面板关闭
        private void DetialExpander_Collapsed(object sender, RoutedEventArgs e)
        {
            if (sender is Expander expander)
            {
                var row = DataGridRow.GetRowContainingElement(expander);
                row.DetailsVisibility = Visibility.Collapsed;
            }
        }
    }
}

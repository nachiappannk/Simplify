using System.Windows;
using System.Windows.Controls;
using Microsoft.Win32;

namespace SimplifyUi.Common
{
    /// <summary>
    /// Interaction logic for ReadJournalUserControl.xaml
    /// </summary>
    public partial class ReadExcelUserControl : UserControl
    {
        public ReadExcelUserControl()
        {
            InitializeComponent();
        }

        private void OnPickerButtonClick(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.DefaultExt = ".xlsx";
            dialog.Filter = "Excel documents (.xlsx)|*.xlsx";
            var done = dialog.ShowDialog();
            if (done == true)
            { 

                FilePicker.Clear();
                FilePicker.AppendText(dialog.FileName);
                FilePicker.GetBindingExpression(TextBox.TextProperty).UpdateSource();
            }
        }
    }
}

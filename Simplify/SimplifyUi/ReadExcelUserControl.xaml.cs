using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.Win32;

namespace SimplifyUi
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

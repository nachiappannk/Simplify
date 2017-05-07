using System.Windows;
using System.Windows.Controls;

namespace SimplifyUi.BooksOfAccountGeneration
{
    /// <summary>
    /// Interaction logic for BooksOfAccountGenerationStatusUserControl.xaml
    /// </summary>
    public partial class BooksOfAccountGenerationStatusUserControl : UserControl
    {
        public BooksOfAccountGenerationStatusUserControl()
        {
            InitializeComponent();
        }

        private void ButtonBase_OnClick(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }
    }
}

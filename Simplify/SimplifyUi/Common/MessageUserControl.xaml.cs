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

namespace SimplifyUi.Common
{
    /// <summary>
    /// Interaction logic for MessageUserControl.xaml
    /// </summary>
    public partial class MessageUserControl : UserControl
    {
        public static readonly DependencyProperty MessageBrushProperty = DependencyProperty.Register(
            "MessageBrush", typeof(Brush), typeof(MessageUserControl), new PropertyMetadata(default(Brush)));

        public Brush MessageBrush
        {
            get { return (Brush) GetValue(MessageBrushProperty); }
            set { SetValue(MessageBrushProperty, value); }
        }
        public MessageUserControl()
        {
            InitializeComponent();
        }

        private void ButtonBase_OnClick(object sender, RoutedEventArgs e)
        {
            this.Visibility = Visibility.Collapsed;
        }
    }
}

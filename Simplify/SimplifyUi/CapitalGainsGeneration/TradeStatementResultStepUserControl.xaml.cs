using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
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

namespace SimplifyUi.CapitalGainsGeneration
{
    /// <summary>
    /// Interaction logic for TradeStatementResultStepUserControl.xaml
    /// </summary>
    public partial class TradeStatementResultStepUserControl : UserControl
    {
        public TradeStatementResultStepUserControl()
        {
            InitializeComponent();
        }

        private void OnAutoGeneratingColumn(object sender, DataGridAutoGeneratingColumnEventArgs e)
        {
            var name = GetColumnName(e.PropertyDescriptor);
            if (!string.IsNullOrEmpty(name))
            {
                e.Column.Header = name;
            }
            var displayFormat = GetColumnFormat(e.PropertyDescriptor);
            if (!string.IsNullOrEmpty(displayFormat))
            {
                (e.Column as DataGridTextColumn).Binding.StringFormat = displayFormat;
            }
        }

        public static string GetColumnName(object descriptor)
        {
            var propertyDescriptor = descriptor as PropertyDescriptor;

            if (propertyDescriptor != null)
            {
                var displayNameAttribute = propertyDescriptor.Attributes[typeof(DisplayNameAttribute)] as DisplayNameAttribute;

                if (displayNameAttribute != null && !Equals(displayNameAttribute, DisplayNameAttribute.Default))
                {
                    return displayNameAttribute.DisplayName;
                }
                return String.Empty;
            }
            var propertyInfo = descriptor as PropertyInfo;
            if (propertyInfo != null)
            {
                Object[] attributes = propertyInfo.GetCustomAttributes(typeof(DisplayNameAttribute), true);
                for (int i = 0; i < attributes.Length; ++i)
                {
                    var displayName = attributes[i] as DisplayNameAttribute;
                    if (displayName != null && !Equals(displayName, DisplayNameAttribute.Default))
                    {
                        return displayName.DisplayName;
                    }
                }
                return String.Empty;
            }
            return String.Empty;
        }

        public static string GetColumnFormat(object descriptor)
        {
            var propertyDescriptor = descriptor as PropertyDescriptor;

            if (propertyDescriptor != null)
            {
                var displayFormatAttribute = propertyDescriptor.Attributes[typeof(DisplayFormatAttribute)] as DisplayFormatAttribute;

                if (displayFormatAttribute != null)
                {
                    return displayFormatAttribute.DataFormatString;
                }
                return String.Empty;
            }
            var propertyInfo = descriptor as PropertyInfo;
            if (propertyInfo != null)
            {
                Object[] attributes = propertyInfo.GetCustomAttributes(typeof(DisplayFormatAttribute), true);
                for (int i = 0; i < attributes.Length; ++i)
                {
                    var displayName = attributes[i] as DisplayFormatAttribute;
                    if (displayName != null)
                    {
                        return displayName.DataFormatString;
                    }
                }
                return String.Empty;
            }
            return String.Empty;
        }
    }
}

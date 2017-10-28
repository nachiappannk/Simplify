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


        public static bool IsObjectOfType<T>(object obj) where T:class
        {
            var propertyDescriptor = obj as T;
            return propertyDescriptor != null;
        }

        public static T GetObjectAsType<T>(object obj) where T : class
        {
            var propertyDescriptor = obj as T;
            return propertyDescriptor;
        }

        public static bool TryGetAttribute<T>(PropertyDescriptor propertyDescriptor, out T t) where T:class
        {
            var attribute = propertyDescriptor.Attributes[typeof(T)] as T;
            t = attribute;
            return attribute != null;
        }

        public static bool TryGetAttribute<T>(PropertyInfo propertyInfo, out T t) where T : class
        {
            var attributes = propertyInfo.GetCustomAttributes(typeof(DisplayNameAttribute), true);
            if (attributes.Length == 0)
            {
                t = default(T);
                return false;
            }
            t = attributes.ElementAt(0) as T;
            return true;
        }

        public static string GetColumnName(object descriptor)
        {
            
            if (IsObjectOfType<PropertyDescriptor>(descriptor))
            {
                var propertyDescriptor = GetObjectAsType<PropertyDescriptor>(descriptor);
                DisplayNameAttribute displayNameAttribute = null;
                return TryGetAttribute(propertyDescriptor, out displayNameAttribute) ? displayNameAttribute.DisplayName : string.Empty;
            }
            
            if (IsObjectOfType<PropertyInfo>(descriptor))
            {
                var propertyInfo = GetObjectAsType<PropertyInfo>(descriptor);
                DisplayNameAttribute displayNameAttribute = null;
                return TryGetAttribute(propertyInfo, out displayNameAttribute) ? displayNameAttribute.DisplayName : string.Empty;
            }
            return string.Empty;
        }

        public static string GetColumnFormat(object descriptor)
        {
            if (IsObjectOfType<PropertyDescriptor>(descriptor))
            {
                var propertyDescriptor = GetObjectAsType<PropertyDescriptor>(descriptor);
                DisplayFormatAttribute displayFormatAttribute = null;
                return TryGetAttribute(propertyDescriptor, out displayFormatAttribute) ? displayFormatAttribute.DataFormatString : string.Empty;
            }

            if (IsObjectOfType<PropertyInfo>(descriptor))
            {
                var propertyInfo = GetObjectAsType<PropertyInfo>(descriptor);
                DisplayFormatAttribute displayFormatAttribute = null;
                return TryGetAttribute(propertyInfo, out displayFormatAttribute) ? displayFormatAttribute.DataFormatString : string.Empty;
            }
            return string.Empty;
        }
    }
}

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

namespace SimplifyUi.Common
{
    /// <summary>
    /// Follow steps 1a or 1b and then 2 to use this custom control in a XAML file.
    ///
    /// Step 1a) Using this custom control in a XAML file that exists in the current project.
    /// Add this XmlNamespace attribute to the root element of the markup file where it is 
    /// to be used:
    ///
    ///     xmlns:MyNamespace="clr-namespace:SimplifyUi.Common"
    ///
    ///
    /// Step 1b) Using this custom control in a XAML file that exists in a different project.
    /// Add this XmlNamespace attribute to the root element of the markup file where it is 
    /// to be used:
    ///
    ///     xmlns:MyNamespace="clr-namespace:SimplifyUi.Common;assembly=SimplifyUi.Common"
    ///
    /// You will also need to add a project reference from the project where the XAML file lives
    /// to this project and Rebuild to avoid compilation errors:
    ///
    ///     Right click on the target project in the Solution Explorer and
    ///     "Add Reference"->"Projects"->[Browse to and select this project]
    ///
    ///
    /// Step 2)
    /// Go ahead and use your control in the XAML file.
    ///
    ///     <MyNamespace:FormatableDataGrid/>
    ///
    /// </summary>
    public class FormatableDataGrid : DataGrid
    {
        public FormatableDataGrid() : base()
        {
            this.AutoGeneratingColumn += OnAutoGeneratingColumn;
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
                ((DataGridTextColumn) e.Column).Binding.StringFormat = displayFormat;
            }
            var isEditable = IsColumnEditable(e.PropertyDescriptor);
            e.Column.IsReadOnly = !isEditable;
            if(isEditable)
            {
                e.Column.Header = e.Column.Header + Environment.NewLine + "(Editable)";
                e.Column.CellStyle = new Style(typeof(DataGridCell));
                e.Column.CellStyle.Setters.Add(new Setter(BackgroundProperty, new SolidColorBrush(Colors.PeachPuff)));
            }
        }


        public static bool IsObjectOfType<T>(object obj) where T : class
        {
            var propertyDescriptor = obj as T;
            return propertyDescriptor != null;
        }

        public static T GetObjectAsType<T>(object obj) where T : class
        {
            var propertyDescriptor = obj as T;
            return propertyDescriptor;
        }

        public static bool TryGetAttribute<T>(PropertyDescriptor propertyDescriptor, out T t) where T : class
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

        public static bool IsColumnEditable(object descriptor)
        {

            if (IsObjectOfType<PropertyDescriptor>(descriptor))
            {
                var propertyDescriptor = GetObjectAsType<PropertyDescriptor>(descriptor);
                EditableAttribute editableAttribute = null;
                return TryGetAttribute(propertyDescriptor, out editableAttribute) && editableAttribute.AllowEdit;
            }

            if (IsObjectOfType<PropertyInfo>(descriptor))
            {
                var propertyInfo = GetObjectAsType<PropertyInfo>(descriptor);
                EditableAttribute editableAttribute = null;
                return TryGetAttribute(propertyInfo, out editableAttribute) && editableAttribute.AllowEdit;
            }
            return false;
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

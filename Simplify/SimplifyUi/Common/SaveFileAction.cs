using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Interactivity;
using Microsoft.SqlServer.Server;
using Prism.Interactivity.InteractionRequest;
using SimplifyUi.CapitalGainsGeneration.ViewModel;

namespace SimplifyUi.Common
{
    public class SaveFileAction : TriggerAction<FrameworkElement>
    {
        protected override void Invoke(object parameter)
        {
            InteractionRequestedEventArgs args = parameter as InteractionRequestedEventArgs;
            if (args == null) return;
            var fileSaveAsNotification = args.Context as FileSaveAsNotification;
            if (fileSaveAsNotification == null) return;
            Microsoft.Win32.SaveFileDialog dlg =
                new Microsoft.Win32.SaveFileDialog
                {
                    Title = fileSaveAsNotification.Title,
                    FileName = fileSaveAsNotification.DefaultFileName,
                    DefaultExt = ".xlsx",
                    Filter = "Excel File (.xlsx)|*.xlsx",
                    InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop)
                };

            fileSaveAsNotification.OutputFileName = String.Empty;
            bool? result = dlg.ShowDialog();
            if (result == true)
            {
                fileSaveAsNotification.OutputFileName = dlg.FileName;
            }
        }
    }
}

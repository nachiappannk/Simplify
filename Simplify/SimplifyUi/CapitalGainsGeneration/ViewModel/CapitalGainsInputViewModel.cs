using System;
using System.Collections.Generic;
using System.IO;
using Prism.Commands;
using Prism.Interactivity.InteractionRequest;
using Simplify.Application;
using Simplify.ExcelDataGateway;
using Simplify.Facade;
using SimplifyUi.Common.ViewModel;
using SimplifyUi.Common.ViewModelTools;

namespace SimplifyUi.CapitalGainsGeneration.ViewModel
{
    public class CapitalGainsInputViewModel
    {
        public InteractionRequest<FileSaveAsNotification> FileSaveAsRequest { get; private set; }

        private Action<List<string>, Logger> _completionCallBack;

        public CapitalGainsInputViewModel(Action<List<string>, Logger> completionCallBack)
        {
            FileSaveAsRequest = new InteractionRequest<FileSaveAsNotification>();

            _completionCallBack = completionCallBack;

            OpeningStockSelectorViewModel = new ExcelSheetSelectorViewModel();
            OpeningStockSelectorViewModel.Title = "Please provide opening stock";
            OpeningStockSelectorViewModel.ValidityChanged += () =>
            {
                GenerateCommand.RaiseCanExecuteChanged();
            };

            TradeLogExcelSheetSelectorViewModel = new ExcelSheetSelectorViewModel();
            TradeLogExcelSheetSelectorViewModel.Title = "Please provide trade log";
            TradeLogExcelSheetSelectorViewModel.ValidityChanged += () =>
            {
                GenerateCommand.RaiseCanExecuteChanged();
            };

            GenerateCommand = new DelegateCommand(Generate, CanGenerate);
        }

        
        private void Generate()
        {
            List<string> mainMessage = new List<string>();
            var logger = new Logger();
            var file = new FileSaveAsNotification()
            {
                Title = "Capital Gains Output File",
                DefaultFileName = "CapitalGainsOutput",
            };
            FileSaveAsRequest.Raise(file);
            var fullPath = file.OutputFileName;
            try
            {
                CapitalGainsStatementGenerationFacade facade = new CapitalGainsStatementGenerationFacade();
                facade.GenerateStatements(
                    OpeningStockSelectorViewModel.InputFileName,
                    OpeningStockSelectorViewModel.SelectedSheet,
                    TradeLogExcelSheetSelectorViewModel.InputFileName,
                    TradeLogExcelSheetSelectorViewModel.SelectedSheet,
                    fullPath, logger);
                mainMessage.Add("Successfully Generated Capital Gains File");
                mainMessage.Add("Output :"+fullPath);
            }
            catch (Exception e)
            {
                var errorFile = OutputNameComputer.ComputeOutputFile("SimplifyCapitalGainsError",".txt");
                File.WriteAllText(errorFile,
                    e.StackTrace);
                mainMessage.Add("Fully or Partially Failed to generate");
                mainMessage.Add("Error :"+errorFile);
                mainMessage.Add("Output (may be):" + fullPath);
            }
            _completionCallBack.Invoke(mainMessage,logger);
        }

        private bool CanGenerate()
        {
            return (OpeningStockSelectorViewModel.IsValid && TradeLogExcelSheetSelectorViewModel.IsValid);
        }

        public DelegateCommand GenerateCommand { get; set; }
        public ExcelSheetSelectorViewModel OpeningStockSelectorViewModel { get; set; }
        public ExcelSheetSelectorViewModel TradeLogExcelSheetSelectorViewModel { get; set; }

    }
}
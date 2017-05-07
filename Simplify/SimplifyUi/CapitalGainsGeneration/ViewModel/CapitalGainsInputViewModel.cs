using System;
using System.Collections.Generic;
using System.IO;
using Prism.Commands;
using Simplify.Application;
using Simplify.ExcelDataGateway;
using Simplify.Facade;
using SimplifyUi.Common.ViewModel;
using SimplifyUi.Common.ViewModelTools;

namespace SimplifyUi.CapitalGainsGeneration.ViewModel
{
    public class CapitalGainsInputViewModel
    {
        private Action<List<string>, Logger> _completionCallBack;

        public CapitalGainsInputViewModel(Action<List<string>, Logger> completionCallBack)
        {
            _completionCallBack = completionCallBack;

            ExcelSheetSelectorViewModel = new ExcelSheetSelectorViewModel();
            ExcelSheetSelectorViewModel.Title = "Please provide trade log";
            ExcelSheetSelectorViewModel.ValidityChanged += () =>
            {
                GenerateCommand.RaiseCanExecuteChanged();
            };

            GenerateCommand = new DelegateCommand(Generate, CanGenerate);
        }

        
        private void Generate()
        {
            List<string> mainMessage = new List<string>();
            var logger = new Logger();
            var fullPath = OutputNameComputer.ComputeOutputFile("CapitalGains", ".xlsx");
            try
            {
                CapitalGainsStatementGenerationFacade facade = new CapitalGainsStatementGenerationFacade();
                facade.GenerateStatements(ExcelSheetSelectorViewModel.InputFileName,
                    ExcelSheetSelectorViewModel.SelectedSheet,
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
            return ExcelSheetSelectorViewModel.IsValid;
        }

        public DelegateCommand GenerateCommand { get; set; }
        public ExcelSheetSelectorViewModel ExcelSheetSelectorViewModel { get; set; }
        
    }
}
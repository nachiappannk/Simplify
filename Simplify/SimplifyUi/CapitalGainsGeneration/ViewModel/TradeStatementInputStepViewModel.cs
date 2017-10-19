using System;
using SimplifyUi.Common.ViewModel;

namespace SimplifyUi.CapitalGainsGeneration.ViewModel
{
    public class TradeStatementInputStepViewModel : WorkFlowStepViewModel
    {
        public event Action<TradeStatementInput> InputChanged; 
        public ExcelSheetSelectorViewModel TradeLogExcelSheetSelectorViewModel { get; set; }

        public TradeStatementInputStepViewModel()
        {
            Name = "Inputs";
            TradeLogExcelSheetSelectorViewModel = new ExcelSheetSelectorViewModel();
            TradeLogExcelSheetSelectorViewModel.Title = "Please provide trade log";
            TradeLogExcelSheetSelectorViewModel.ValidityChanged += () =>
            {
                var input = new TradeStatementInput
                {
                    FileName = TradeLogExcelSheetSelectorViewModel.InputFileName,
                    SheetName = TradeLogExcelSheetSelectorViewModel.SelectedSheet
                };

                InputChanged?.Invoke(input);


                if (string.IsNullOrWhiteSpace(input.FileName)) CanGoToNext = false;
                else if (string.IsNullOrWhiteSpace(input.SheetName)) CanGoToNext = false;
                else CanGoToNext = true;

                FireStateChanged();
            };
        }
    }

    public class TradeStatementInput
    {
        public string FileName { get; set; }
        public string SheetName { get; set; }
    }
}
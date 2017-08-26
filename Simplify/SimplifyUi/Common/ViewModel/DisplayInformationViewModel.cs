using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Prism.Commands;
using Simplify.ExcelDataGateway;
using SimplifyUi.Common.ViewModelTools;

namespace SimplifyUi.Common.ViewModel
{
    public class DisplayInformationViewModel
    {
        public DisplayInformationViewModel(string title, List<string> mainInformation, List<Message> additionalInformation, 
            Action completedAction  )
        {
            AdditionalInformation = additionalInformation;
            Title = title;
            MainInformation = mainInformation;
            CompletedCommand = new DelegateCommand(completedAction.Invoke, () => true);
        }

        public string Title { get; set; }
        public List<string> MainInformation { get; set; }

        public List<Message> AdditionalInformation { get; set; }

        public DelegateCommand CompletedCommand { get; set; }
    }
}

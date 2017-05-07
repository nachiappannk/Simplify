using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Prism.Commands;

namespace SimplifyUi.Common.ViewModel
{
    public class DisplayInformationViewModel
    {
        public DisplayInformationViewModel(string title, List<string> mainInformation, List<string> additionalInformation, 
            Action completedAction  )
        {
            AdditionalInformation = additionalInformation;
            Title = title;
            MainInformation = mainInformation;
            CompletedCommand = new DelegateCommand(completedAction.Invoke, () => true);
        }

        public string Title { get; set; }
        public List<string> MainInformation { get; set; }

        public List<string> AdditionalInformation { get; set; }

        public DelegateCommand CompletedCommand { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Windows.Input;
using Prism.Commands;

namespace SimplifyUi.ViewModel
{
    public class DisplayMessagesViewModel
    {
        public DisplayMessagesViewModel(Bag bag, string key, string title, int nextStepIndex, Action<int> nextStepAction)
        {
            Title = title;
            Messages = bag.GetObject<List<string>>(key);
            NextStepCommand = new DelegateCommand(() =>
            {
                {
                    nextStepAction.Invoke(nextStepIndex);
                }

            });
        }

        public ICommand NextStepCommand { get; set; }
        public string Title { get; set; }

        public IList<string> Messages { get; set; }
    }
}
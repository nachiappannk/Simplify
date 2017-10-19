using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Simplify.Facade;
using SimplifyUi.Common.ViewModel;

namespace SimplifyUi.CapitalGainsGeneration.ViewModel
{
    public class TradeStatementWorkFlow : WorkflowViewModel
    {
        public TradeStatementWorkFlow() : base("Trade Statements")
        {
            var inputStep = new TradeStatementInputStepViewModel();
            var statementComputingStep = new TradeStatementComputationStepViewModel();
            var resultStep = new TradeStatementResultStepViewModel();
            inputStep.InputChanged += (x) => statementComputingStep.Compute(x.FileName, x.SheetName);
            statementComputingStep.StatementComputed += (x) => resultStep.SetStatements(x);
            AddWorkFlowStep(inputStep);
            AddWorkFlowStep(statementComputingStep);
            AddWorkFlowStep(resultStep);
        }
    }
}

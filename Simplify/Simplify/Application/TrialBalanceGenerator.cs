using System;
using System.Collections.Generic;
using System.Linq;
using OfficeOpenXml.FormulaParsing.Excel.Functions.Math;
using Simplify.Books;
using Simplify.ExcelDataGateway;

namespace Simplify.Application
{
    public class TrialBalanceGenerator
    {
        private readonly ILogger _logger;

        public TrialBalanceGenerator(ILogger logger)
        {
            _logger = logger;
        }
        public TrialBalanceBook Generate(IList<JournalStatement> journalStatements)
        {
            var statements = journalStatements.GroupBy(
                    x => x.Name, 
                    y => y.Value, 
                    (key, values) => new Statement()
                    {
                        Name = key,
                        Value = values.Sum(),
                    })
                .ToList();

            var trialBalanceValue = statements.Sum(x => x.Value);
            if (Math.Abs(trialBalanceValue) > 0.001)
            {
                _logger.Log(MessageType.IgnorableError, $"The value of the trial balace is {trialBalanceValue} expected was 0");
            }
            var trialBalance = new TrialBalanceBook();

            trialBalance.AddRange(statements.OrderBy(x=>x.Name));
            return trialBalance;
        }
    }
}
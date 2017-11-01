using System;
using System.Collections.Generic;

namespace Simplify.Trade
{
    public class QuotationRepository
    {

        Dictionary<string, Quote> quotes = new Dictionary<string, Quote>();

        public QuotationRepository(List<QuotationStatement> quotationStatements)
        {
            foreach (var quotationStatement in quotationStatements)
            {
                var quote = GetQuote(quotationStatement.Name);
                quote.QuotedValue = quotationStatement.CurrentValue;
            }
        }

        public Quote GetQuote(string name)
        {
            if (!quotes.ContainsKey(name))
            {
                var quote = new Quote()
                {
                    Name = name,
                    QuotedValue = null
                };
                quotes.Add(name, quote);
            }
            return quotes[name];
        }
    }

    public class Quote
    {
        public event Action Changed;

        public string Name { get; set; }

        private double? _quotedValue;

        public double? QuotedValue
        {
            get { return _quotedValue; }
            set
            {
                if (_quotedValue == null && value == null) return;
                if (value.HasValue && _quotedValue.HasValue && Math.Abs(value.Value - _quotedValue.Value) < 0.001) return;
                _quotedValue = value;
                Changed?.Invoke();
            }
        }
    }

}
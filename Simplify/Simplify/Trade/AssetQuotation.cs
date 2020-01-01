using System;

namespace Simplify.Trade
{
    public class AssetQuotation
    {
        public event Action Changed;

        private readonly Quote _quote;

        public AssetQuotation(Quote quote)
        {
            _quote = quote;
            _quote.Changed += () => Changed?.Invoke();
        }

        public string Name { get; set; }
        public bool IsOwned { get; set; }

        public double? QuotePerUnit
        {
            get { return _quote.QuotedValue; }
            set { _quote.QuotedValue = value; }
        }
    }
}
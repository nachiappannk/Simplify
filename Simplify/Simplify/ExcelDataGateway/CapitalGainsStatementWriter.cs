using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Simplify.Books;

namespace Simplify.ExcelDataGateway
{
    public class CapitalGainsStatementWriter
    {
        private readonly string _excelFileName;
        readonly string[] headings =
        {
            "S.No.",
            "Sale Date",
            "Purchase Date",
            "Holding Days",
            "Type",
            "Description",
            "Account",
            "Contract",
            "Stt",
            "Quantity",
            "Sale",
            "Cost",
            "Profit"
        };

        public CapitalGainsStatementWriter(string excelFileName)
        {
            _excelFileName = excelFileName;

        }

        public void WriteCapitalGains(IList<SquaredStatement> squaredStatements)
        {
            var index = 0;
            using (var writer = new ExcelWriter(_excelFileName, "CapitalGains"))
            {
                writer.Write(index++, headings.ToArray<object>());
                writer.SetColumnsWidth(6, 12,12, 8,4,30, 16, 16, 16,12, 12, 12, 12);
                writer.ApplyHeadingFormat(headings.Length);
                writer.WriteList(index, squaredStatements, (b, rowIndex) => new object[]
                {
                    rowIndex - 1,
                    b.GetSaleDate(),
                    b.GetPurchaseDate(),
                    b.GetHoldingDays(),
                    b.GetItemType(),
                    b.GetName(),
                    b.GetAccount(),
                    b.GetContract(),
                    b.GetStt(),
                    b.GetQuantity(),
                    b.GetSale(),
                    b.GetCost(),
                    b.GetProfit(),
                });
            }
        }
    }
}

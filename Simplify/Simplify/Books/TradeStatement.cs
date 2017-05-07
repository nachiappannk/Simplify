using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Media3D;

namespace Simplify.Books
{
    public class TradeStatement
    {
        public int SerialNumber { get; set; }
        public DateTime Date { get; set; }
        public bool IsPurchase { get; set; }
        public string ItemType { get; set; }
        public string Name { get; set; }
        public double Quantity { get; set; }
        public double Value { get; set; }
        public string Stt { get; set; }
        public string Account { get; set; }
        public string Contract { get; set; }
    }
}

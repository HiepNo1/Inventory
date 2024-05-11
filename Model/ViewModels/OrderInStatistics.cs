using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.ViewModels
{
    public class OrderInStatistics
    {
        public string OrderCode { get; set; }
        public string CustomerName { get; set; }
        public string CustomerPhone { get; set; }
        public decimal ShippingFee { get; set; }
        public DateTime OrderDate { get; set; }
        public decimal Revenue { get; set; }
        public int ProductCount { get; set; }
    }
}

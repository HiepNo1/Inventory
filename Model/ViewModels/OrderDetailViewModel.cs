using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.ViewModels
{
    public class OrderDetailViewModel
    {
        public long ID { get; set; }
        public long OrderID { get; set; }
        public string ProductName { get; set; }
        public string ProductImage { get; set; }
        public decimal? ProductPrice { get; set; }
        public int? Sale { get; set; }
        public int Quantity { get; set; }
        public long ProductID { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.ViewModels
{
    public class ReceiptDetailViewModel
    {
        public long ID { get; set; }
        public string ProductName { get; set; }
        public string ProductImage { get; set; }
        public decimal ImportPrice { get; set; }
        public int Quantity { get; set; }
        public long ProductID { get; set; }
        public long ReceiptID { get; set; }
    }
}

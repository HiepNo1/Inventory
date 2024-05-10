using Model.EF;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.ViewModels
{
    public class ProductDetailInReceipt
    {
        public long ProductID { get; set; }
        public decimal ImportPrice { get; set; }
        public int ImportQuantity { get; set; }
        public decimal TotalPrice { get; set; }
    }
}

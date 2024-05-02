using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Model.EF;

namespace Model.ViewModels
{
    public class CreateOrderViewModel
    {
        public Customer Customer { get; set; }
        public List<OrderDetail> LstOrderDetail { get; set; }
        public Order Order { get; set; }
        public List<Product> Products { get; set; }
        public string SearchProduct { get; set; }
        public string SearchCustomer { get; set; }
    }
}

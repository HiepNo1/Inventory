using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.ViewModels
{
    public class OrderViewModel
    {
        public long ID { get; set; }
        public string Code { get; set; }
        public string CustomerName { get; set; }
        public string CustomerPhone { get; set; }
        public string CustomerEmail { get; set; }
        public string CustomerAddress { get; set; }
        public int? Gender { get; set; }
        public decimal? TotalPrice { get; set; }
        public bool PaymentStatus { get; set; }
        public decimal ShippingFee { get; set; }
        public DateTime? ShippingDate { get; set; }
        public DateTime? ReceivedDate { get; set; }
        public DateTime CreateDate { get; set; }
        public int? Status { get; set; }
        public bool ShippingStatus { get; set; }
        public string Description { get; set; }
    }
}

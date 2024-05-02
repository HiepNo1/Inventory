namespace Model.EF
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;
    [Table("Order")]
    public class Order
    {
        public long ID { get; set; }

        [StringLength(50)]
        public string Code { get; set; }
        public long EmployeeID { get; set; }

        public long CustomerID { get; set; }     
      
        public decimal? TotalPrice { get; set; }

        public DateTime? ShippingDate { get; set; }

        public bool PaymentStatus { get; set; }

        public decimal ShippingFee { get; set; }

        public DateTime? ReceivedDate { get; set; }

        public bool ShippingStatus { get; set; }

        [StringLength(300)]
        public string Description { get; set; }

        public int? Status { get; set; }

        public DateTime CreateDate { get; set; }

        [StringLength(50)]
        public string CreateBy { get; set; }

        public DateTime? ModifieDate { get; set; }

        [StringLength(50)]
        public string ModifieBy { get; set; }

        [StringLength(250)]
        public string MetaKeywords { get; set; }

        [StringLength(250)]
        public string MetaDescription { get; set; }

    }
}

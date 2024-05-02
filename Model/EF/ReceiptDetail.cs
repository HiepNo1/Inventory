namespace Model.EF
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;
    [Table("ReceiptDetail")]
    public class ReceiptDetail
    {
        public long ID { get; set; }
        public long ReceiptID { get; set; }
        public long ProductID { get; set; }
        public int Quantity { get; set; }
        public decimal ImportPrice { get; set; }
    }
}

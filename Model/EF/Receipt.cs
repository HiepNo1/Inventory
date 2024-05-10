namespace Model.EF
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;
    [Table("Receipt")]
    public class Receipt
    {
        public long ID { get; set; }

        [StringLength(50)]
        public string Code { get; set; }
        public long EmployeeID { get; set; }

        public long SupplierID { get; set; }
     
        public bool? Payment { get; set; }
        public bool PaymentStatus { get; set; }

        [StringLength(300)]
        public string Description { get; set; }
        public bool? Status { get; set; }
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


namespace Model.EF
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;
    [Table("Customer")]
    public partial class Customer
    {
        public long ID { get; set; }

        [StringLength(250)]
        
        [Display(Name = "Tên")]
        public string Name { get; set; }

        [StringLength(50)]
        [Display(Name = "SĐT")]
        public string Phone { get; set; }

        [StringLength(50)]
        [Display(Name = "Địa chỉ")]
        public string Address { get; set; }
        
        [StringLength(50)]
        [EmailAddress(ErrorMessage = "Địa chỉ email không hợp lệ")]
        public string Email { get; set; }

        [Display(Name = "Giới tính")]
        public int? Gender { get; set; }

        public DateTime? CreateDate { get; set; }

        [StringLength(50)]
        public string CreateBy { get; set; }

        public DateTime? ModifieDate { get; set; }

        [StringLength(50)]
        public string ModifieBy { get; set; }

        [StringLength(250)]
        public string MetaKeywords { get; set; }

        [StringLength(250)]
        public string MetaDescription { get; set; }

        public bool? Status { get; set; }
    }
}

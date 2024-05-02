namespace Model.EF
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("User")]
    public partial class User
    {
        public long ID { get; set; }

        [Required(ErrorMessage ="Vui lòng nhập tài khoản")]
        [StringLength(50)]
        [Display(Name="Tài khoản")]
        public string UserName { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập mật khẩu")]
        [StringLength(32)]
        [Display(Name = "Mật khẩu")]
        public string Password { get; set; }

        [StringLength(20)]
        [Display(Name = "Nhóm người dùng")]
        public string GroupID { get; set; }

        [StringLength(20)]
        [Display(Name = "SĐT")]
        public string Phone { get; set; }
        [StringLength(50)]
        [Display(Name = "Tên đầy đủ")]
        public string Name { get; set; }
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

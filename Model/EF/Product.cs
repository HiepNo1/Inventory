namespace Model.EF
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Product")]
    public partial class Product
    {
            public long ID { get; set; }

            [StringLength(250)]
            [Required(ErrorMessage ="Bạn chưa nhập tên sản phẩm")]
            [Display(Name="Tên sản phẩm")]
            public string Name { get; set; }

            [StringLength(20)]
            [Display(Name = "Mã sản phẩm")]
            public string Code { get; set; }

            [StringLength(250)]
            public string MetaTitle { get; set; }

            [StringLength(250)]
            [Display(Name = "Hình ảnh")]
            public string Image { get; set; }

            [StringLength(250)]
            [Display(Name = "Mô tả")]
            public string Description { get; set; }

            [StringLength(50)]
            [Display(Name = "Chất liệu")]
            public string Material { get; set; }

            [StringLength(50)]
            [Display(Name = "Màu sắc")]
            public string Color { get; set; }

            [Display(Name = "Giá tiền")]
            public decimal Price { get; set; }

            [Display(Name = "Giảm giá")]
            public int Sale { get; set; }

            [Display(Name = "Số lượng")]
            public int Quantity { get; set; }

            [Display(Name = "Loại sản phẩm")]
            public long? CategoryID { get; set; }

            [Display(Name = "Chi tiết")]
            [Column(TypeName = "ntext")]
            public string Detail { get; set; }

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

            public bool Status { get; set; }
    }
}

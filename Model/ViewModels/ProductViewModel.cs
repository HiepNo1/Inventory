using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.ViewModels
{
    public class ProductViewModel
    {
        public long ID { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }
        public string MetaTitle { get; set; }
        public string Image { get; set; }
        public string Description { get; set; }
        public string Material { get; set; }
        public string Color { get; set; }
        public decimal? Price { get; set; }
        public int? Sale { get; set; }
        public int? Quantity { get; set; }      
        public string Detail { get; set; }
        public DateTime? CreateDate { get; set; }
        public DateTime? ModifieDate { get; set; }
        public bool Status { get; set; }
        public long? CateID { get; set; }
        public string CateName { get; set; }
        public string CateParent { get; set; }
    }
}

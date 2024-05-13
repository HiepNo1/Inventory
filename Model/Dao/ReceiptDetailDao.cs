using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Model.EF;
using Model.ViewModels;

namespace Model.Dao
{
    public class ReceiptDetailDao
    {
        InventoryDbContext db = null;
        public ReceiptDetailDao()
        {
            db = new InventoryDbContext();
        }
        public List<ReceiptDetailViewModel> GetList(long receiptID)
        {
            var model = from a in db.ReceiptDetails
                        join b in db.Products
                        on a.ProductID equals b.ID
                        select new ReceiptDetailViewModel()
                        {
                            ID = a.ID,
                            ReceiptID = a.ReceiptID,
                            ProductName = b.Name,
                            ProductImage = b.Image,
                            ImportPrice = a.ImportPrice,                          
                            Quantity = a.Quantity,
                            ProductID = b.ID,
                            ProductColor = b.Color,
                            ProductMarterial = b.Material
                        };
            return model.Where(x => x.ReceiptID == receiptID).ToList();
        }
    }
}

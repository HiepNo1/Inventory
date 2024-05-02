using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Model.EF;
using Model.ViewModels;
using PagedList;

namespace Model.Dao
{
    public class ReceiptDao
    {
        InventoryDbContext db = null;
        public ReceiptDao()
        {
            db = new InventoryDbContext();
        }
        private IQueryable<ReceiptViewModel> inputOrder()
        {
            return (from a in db.Receipts
                    join b in db.Suppliers
                    on a.SupplierID equals b.ID
                    select new ReceiptViewModel()
                    {
                        ID = a.ID,
                        Code = a.Code,
                        SupplierName = b.Name,
                        SupplierPhone = b.Phone,
                        SupplierEmail = b.Email,
                        SupplierAddress = b.Address,
                        Payment = a.Payment,
                        PaymentStatus = a.PaymentStatus,
                        Description = a.Description,
                        Status = a.Status,
                        CreateDate = a.CreateDate
                    });
        }
        public IEnumerable<ReceiptViewModel> GetList(string catePage, string searchString, DateTime? createDate, int page, int pageSize)
        {
            var receipt = inputOrder();
            if (catePage == "Trash")
            {
                receipt = receipt.Where(x => x.Status == false);
            }
            if (!string.IsNullOrEmpty(searchString))
            {
                receipt = receipt.Where(x => x.SupplierName.Contains(searchString) || x.SupplierPhone.Contains(searchString));
            }
            if (createDate != null)
            {
                var createDateValue = createDate.Value.Date;
                receipt = receipt.Where(x => x.CreateDate.Value.Day == createDateValue.Day
                                          && x.CreateDate.Value.Month == createDateValue.Month
                                          && x.CreateDate.Value.Year == createDateValue.Year);
            }
            
            return receipt.OrderByDescending(x => x.CreateDate).ToPagedList(page, pageSize);
        }

        public ReceiptViewModel Detail(long id)
        {
            var model = inputOrder();
            return model.SingleOrDefault(x => x.ID == id);
        }

        public void Insert(Receipt receipt)
        {
            receipt.CreateDate = DateTime.Now;
            var lastID = db.Orders.OrderByDescending(x => x.ID).Select(x => x.ID).FirstOrDefault();
            var newID = lastID + 1;
            receipt.Code = "PN" + newID.ToString("D4");
            db.Receipts.Add(receipt);
            db.SaveChanges();
        }
    }

}

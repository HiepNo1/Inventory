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
                    join b in db.Suppliers on a.SupplierID equals b.ID
                    join c in db.Employees on a.EmployeeID equals c.ID
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
                        CreateDate = a.CreateDate,
                        EmployeeName = c.Name,
                        EmployeePhone = c.Phone,
                        EmployeeAddress = c.Address,
                        EmployeeEmail = c.Email,
                        EmployeeGender = c.Gender
                    });
        }
        public IEnumerable<ReceiptViewModel> GetList(string catePage, string searchString, bool? paymentStatus, DateTime? createDate, int page, int pageSize)
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
                receipt = receipt.Where(x => x.CreateDate.Day == createDateValue.Day
                                          && x.CreateDate.Month == createDateValue.Month
                                          && x.CreateDate.Year == createDateValue.Year);
            }
            if(paymentStatus != null)
            {
                receipt = receipt.Where(x => x.PaymentStatus == paymentStatus);
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
            var lastID = db.Receipts.OrderByDescending(x => x.ID).Select(x => x.ID).FirstOrDefault();
            var newID = lastID + 1;
            receipt.Code = "PN" + newID.ToString("D4");
            db.Receipts.Add(receipt);
            db.SaveChanges();
        }
    }

}

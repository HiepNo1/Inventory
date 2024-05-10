using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Model.EF;
using PagedList;
using Model.ViewModels;

namespace Model.Dao
{
    public class OrderDao
    {
        InventoryDbContext db = null;
        public OrderDao()
        {
            db = new InventoryDbContext();
        }
        public IQueryable<OrderViewModel> inputOrder()
        {
            return (from a in db.Orders
                    join b in db.Customers
                    on a.CustomerID equals b.ID
                    select new OrderViewModel()
                    {
                        ID = a.ID,
                        Code = a.Code,
                        Description = a.Description,
                        ShippingFee = a.ShippingFee,
                        PaymentStatus = a.PaymentStatus,
                        ShippingDate = a.ShippingDate,
                        ReceivedDate = a.ReceivedDate,
                        ShippingStatus = a.ShippingStatus,
                        CustomerName = b.Name,
                        CustomerPhone = b.Phone,
                        CustomerEmail = b.Email,
                        CustomerAddress = b.Address,
                        Gender = b.Gender,
                        TotalPrice = a.TotalPrice,
                        Status = a.Status,
                        CreateDate = a.CreateDate
                    });
        }
        public IEnumerable<OrderViewModel> ListAll(string catePage, string searchString, DateTime? createDate, int? status, int page, int pageSize)
        {
            var model = inputOrder();
            if (catePage == "Trash")
            {
                model = model.Where(x => x.Status == 0);
            }

            if (!string.IsNullOrEmpty(searchString))
            {
                model = model.Where(x => x.CustomerName.Contains(searchString) || x.CustomerPhone.Contains(searchString));
            }
            if (createDate != null)
            {
                var createDateValue = createDate.Value.Date;
                model = model.Where(x => x.CreateDate.Year == createDateValue.Year
                                    && x.CreateDate.Month == createDateValue.Month
                                    && x.CreateDate.Day == createDateValue.Day);
            }
            if (status != null)
            {
                model = model.Where(x => x.Status == status);
            }
            return model.OrderByDescending(x => x.CreateDate).ToPagedList(page, pageSize);
        }
        public Order GetByID(long id)
        {
            return db.Orders.Find(id);
        }
        public OrderViewModel Detail(long id)
        {
            var model = inputOrder();
            return model.SingleOrDefault(x => x.ID == id);
        }
        public bool UpdateStatus(Order order)
        {
            var orderExist = db.Orders.Find(order.ID);
            if (orderExist == null)
            {
                return false;
            }
            else
            {
                orderExist.Status = order.Status;
                order.CreateDate = DateTime.Now;
                db.SaveChanges();
                return true;
            }
        }

        public int Destroy(long id)
        {
            var order = db.Orders.Find(id);
            if (order == null)
            {
                return 0;
            }
            else
            {
                if (order.Status == 1 || order.Status == 2 && !order.PaymentStatus)
                {
                    int term = (int)order.Status;
                    order.Status = 0;
                    order.ModifieDate = DateTime.Now;
                    db.SaveChanges();
                    return term;
                }
                else return -1;
            }
        }

        public int ChangeStatus(long id, int statusIn, int statusOut)
        {
            var order = db.Orders.Find(id);
            if (order == null)
            {
                return 0;
            }
            else
            {
                if (order.Status == statusIn)
                {
                    order.Status = statusOut;
                    order.ModifieDate = DateTime.Now;
                    db.SaveChanges();
                    return 1;
                }
                else return -1;
            }
        }
        public int Confirm(long id)
        {
            return ChangeStatus(id, 1, 2);
        }
        public int Transport(long id)
        {
            return ChangeStatus(id, 2, 3);
        }
        public int Success(long id)
        {
            return ChangeStatus(id, 3, 4);
        }
        public bool Delete(long id)
        {
            var order = db.Orders.Find(id);
            if (order == null)
            {
                return false;
            }
            else
            {
                var details = db.OrderDetails.Where(x => x.OrderID == id).ToList();
                db.OrderDetails.RemoveRange(details);
                db.Orders.Remove(order);
                db.SaveChanges();
                return true;
            }
        }

        public void Insert(Order order)
        {
            order.CreateDate = DateTime.Now;
            var lastID = db.Orders.OrderByDescending(x => x.ID).Select(x => x.ID).FirstOrDefault();
            var newID = lastID + 1;
            order.Code = "DH" + newID.ToString("D4");
            db.Orders.Add(order);
            db.SaveChanges();
        }
    }
}

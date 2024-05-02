using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Model.EF;
using Model.ViewModels;

namespace Model.Dao
{
    public class OrderDetailDao
    {
        InventoryDbContext db = null;
        public OrderDetailDao() 
        {
            db = new InventoryDbContext();
        }
        public List<OrderDetailViewModel> GetList(long orderID)
        {
            var model = from a in db.OrderDetails
                        join b in db.Products
                        on a.ProductID equals b.ID
                        select new OrderDetailViewModel()
                        {
                            ID = a.ID,
                            OrderID = a.OrderID,
                            ProductName = b.Name,
                            ProductImage = b.Image,
                            ProductPrice = b.Price,
                            Sale = b.Sale,
                            Quantity = a.Quantity,
                            ProductID = b.ID
                        };
            return model.Where(x => x.OrderID == orderID).ToList();
        }

        public void Insert(OrderDetail orderDetail)
        {
            db.OrderDetails.Add(orderDetail);
            db.SaveChanges();
        }
    }
}

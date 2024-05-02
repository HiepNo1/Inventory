using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PagedList;
using Model.EF;

namespace Model.Dao
{
    public class CustomerDao
    {
        InventoryDbContext db = null;
        public CustomerDao()
        {
            db = new InventoryDbContext();
        }

        public IEnumerable<Customer> ListAllString(string searchString, int page, int pageSize)
        {
            IQueryable<Customer> model = db.Customers;
            if(!string.IsNullOrEmpty(searchString))
            {
                model = model.Where(x => x.Name.Contains(searchString) || x.Phone.Contains(searchString));
            }
                return model.OrderByDescending(x => x.CreateDate).ToPagedList(page, pageSize);
        }

        public long Insert(Customer customer)
        {
            var exist = db.Customers.SingleOrDefault(x => x.Name == customer.Name && x.Phone == customer.Phone);
            if(exist != null)
            {
                return 0;
            }
            customer.CreateDate = DateTime.Now;
            customer.Status = true;
            db.Customers.Add(customer);
            db.SaveChanges();
            return customer.ID;
        }

        public Customer GetById(long id)
        {
            return db.Customers.Find(id);
        }

        public Customer GetByPhone(string phone)
        {
            return db.Customers.SingleOrDefault(x => x.Phone == phone);
        }
        public bool Update(Customer customer)
        {
            try
            {
                var cus = db.Customers.Find(customer.ID);
                cus.ModifieDate = DateTime.Now;
                cus.Name = customer.Name;
                cus.Address = customer.Address;
                cus.Phone = customer.Phone;
                cus.Email = customer.Email;
                cus.Gender = customer.Gender;
                db.SaveChanges();
                return true;
            }
            catch(Exception ex)
            {
                return false;
            }
        }

        public bool Delete(long id)
        {
            try
            {
                var cus = db.Customers.Find(id);
                db.Customers.Remove(cus);
                db.SaveChanges();
                return true;    
            }
            catch(Exception ex)
            {
                return false;
            }
        }
    }
}

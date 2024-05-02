using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Model.EF;
using PagedList;
using PagedList.Mvc;

namespace Model.Dao
{
    public class SupplierDao
    {
        InventoryDbContext db = null;
        public SupplierDao()
        {
            db = new InventoryDbContext();
        }
        public IEnumerable<Supplier> ListAllPaging(string catePage, string searchString, int page, int pageSize)
        {
            IQueryable<Supplier> model = db.Suppliers;
            if(catePage == "Trash")
            {
                model = model.Where(x => x.Status == false);
            }
            if (!string.IsNullOrEmpty(searchString))
            {
                model = model.Where(x => x.Name.Contains(searchString) || x.Phone.Contains(searchString));
            }
            return model.OrderByDescending(x => x.CreateDate).ToPagedList(page, pageSize);
        }

        public Supplier GetById(long id)
        {
            return db.Suppliers.Find(id);
        }
        public Supplier GetByPhone(string phone)
        {
            return db.Suppliers.SingleOrDefault(x => x.Phone == phone);
        }

        public bool Update(Supplier entity)
        {
            try
            {
                var supp = db.Suppliers.Find(entity.ID);
                supp.Name = entity.Name;
                supp.Address = entity.Address;
                supp.Phone = entity.Phone;
                supp.Representative = entity.Representative;
                supp.ModifieDate = DateTime.Now;          
                db.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public long Insert(Supplier supp)
        {
            var exist = db.Suppliers.SingleOrDefault(x => x.Name == supp.Name);
            if (exist != null)
            {
                return 0;
            }
            db.Suppliers.Add(supp);
            db.SaveChanges();
            return supp.ID;
        }

        public bool Delete(long id)
        {
            var supp = db.Suppliers.Find(id);
            if (supp != null)
            {
                db.Suppliers.Remove(supp);
                db.SaveChanges();
                return true;
            }
            else
            {
                return false;
            }
        }
    }

}

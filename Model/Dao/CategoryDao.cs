using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Model.EF;
using PagedList.Mvc;
using PagedList;

namespace Model.Dao
{
    public class CategoryDao
    {
        InventoryDbContext db = null;
        public CategoryDao()
        {
            db = new InventoryDbContext();
        }

        public IEnumerable<ProductCategory> ListAllPaging(string searchString, int page, int pageSizxe)
        {
            IQueryable<ProductCategory> model = db.ProductCategories;
            if (!string.IsNullOrEmpty(searchString))
            {
                model = model.Where(x => x.Name.Contains(searchString));
            }
            return model.OrderByDescending(x => x.CreateDate).ToPagedList(page, pageSizxe);
        }

        public ProductCategory GetById(long id)
        {
            return db.ProductCategories.Find(id);
        }

        public long Insert(ProductCategory category)
        {
            var exist = db.ProductCategories.SingleOrDefault(x => x.Name == category.Name);
            if (exist != null)
            {
                return 0;
            }
            db.ProductCategories.Add(category);
            db.SaveChanges();
            return category.ID;
        }

        public bool Update(ProductCategory category)
        {
            try
            {
                var cat = db.ProductCategories.Find(category.ID);
                cat.Name = category.Name;
                cat.ModifieDate = DateTime.Now;
                cat.ParentID = category.ParentID;
                db.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public bool Delete(long id)
        {
            try
            {
                var category = db.ProductCategories.Find(id);
                db.ProductCategories.Remove(category);
                db.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
    }
}

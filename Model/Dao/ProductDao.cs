using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Model.EF;
using PagedList;
using System.Web;
using Model.ViewModels;

namespace Model.Dao
{
    public class ProductDao
    {
        InventoryDbContext db = null;
        public ProductDao()
        {
            db = new InventoryDbContext();
        }

        private IQueryable<ProductViewModel> inputProduct()
        {
            return (from a in db.Products 
                    join b in db.ProductCategories on a.CategoryID equals b.ID
                    join c in db.ProductCategories on b.ParentID equals c.ID into categories
                    from c in categories.DefaultIfEmpty()
                    select new ProductViewModel()
                    {
                        ModifieDate = a.ModifieDate,
                        Status = a.Status,
                        CateID = b.ID,
                        CateName = b.Name,
                        CateParent = c != null ? c.Name : "",
                        ID = a.ID,
                        Name = a.Name,
                        Code = a.Code,
                        Image = a.Image,
                        Description = a.Description,
                        Material = a.Material,
                        Color = a.Color,
                        Price = a.Price,
                        Sale = a.Sale,
                        Quantity = a.Quantity,
                        Detail = a.Detail,
                        CreateDate = a.CreateDate
                    });
        }
        public IEnumerable<ProductViewModel> ListAllString(string searchString, decimal? firstMoney, decimal? lastMoney, string cateName, int page, int pageSize)
        {

            var model = inputProduct();
            if (!string.IsNullOrEmpty(searchString))
            {
                model = model.Where(x => x.Name.Contains(searchString));
            }
            if(firstMoney != null & lastMoney != null)
            {
                model = model.Where(x => x.Price >= firstMoney && x.Price <= lastMoney);
            }
            if(!string.IsNullOrEmpty(cateName))
            {
                model = model.Where(x => x.CateName == cateName || x.CateParent == cateName);
            }
            return model.OrderByDescending(x => x.CreateDate).ToPagedList(page, pageSize);
        }

        public IEnumerable<ProductViewModel> ListTrash(string searchString, int page, int pageSize)
        {
            var model = inputProduct();
            if (!string.IsNullOrEmpty(searchString))
            {
                model = model.Where(x => x.Name.Contains(searchString));
            }
            return model.Where(x => x.Status == false).OrderByDescending(x => x.ModifieDate).ToPagedList(page, pageSize);
        }

        public ProductViewModel Detail(long id)
        {
            var model = inputProduct();
            return model.SingleOrDefault(x => x.ID == id);
        }
        public Product GetByID(long id)
        {
            return db.Products.Find(id);
        }

        public List<Product> GetBySearch(string search) 
        {
            return db.Products.Where(x => x.Name.Contains(search) || x.Code == search).ToList();
        }
        public long Insert(Product product)
        {
            var exist = db.Products.SingleOrDefault(x => x.Name == product.Name);
            if (exist != null)
            {
                return 0;
            }
            product.CreateDate = DateTime.Now;
            product.Status = true;
            var lastID = db.Products.OrderByDescending(x => x.ID).Select(x => x.ID).FirstOrDefault();
            var newID = lastID + 1;     
            product.Code = "M" + newID.ToString("D4");
            db.Products.Add(product);
            db.SaveChanges();
            return product.ID;
        }

        public bool Update(Product product)
        {
            var pro = db.Products.Find(product.ID);
            if(pro == null) 
            {
                return false;
            }
            else
            {
                pro.Status = product.Status;
                pro.Name = product.Name;
                pro.Image = product.Image;
                pro.CategoryID = product.CategoryID;
                pro.Color = product.Color;
                pro.Price = product.Price;
                pro.Sale = product.Sale;
                pro.Quantity = product.Quantity;
                pro.Material = product.Material;
                db.SaveChanges();
                return true;
            }           
        }

        public bool Delete(long id)
        {
            var pro = db.Products.Find(id);
            if (pro == null)
            {
                return false;
            }
            else
            {
                db.Products.Remove(pro);
                db.SaveChanges();
                return true;
            }
            
        }
    }
}

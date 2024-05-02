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
    public class UserDao
    {
        InventoryDbContext db = null;
        public UserDao()
        {
            db = new InventoryDbContext();
        }
        public IEnumerable<User> ListAllPaging(string catePage, string searchString, int page, int pageSize)
        {
            IQueryable<User> model = db.Users;
            if(catePage == "Trash")
            {
                model = model.Where(x => x.Status == false);
            }
            if(!string.IsNullOrEmpty(searchString))
            {
                model = model.Where(x => x.Phone.Contains(searchString) || x.Name.Contains(searchString));
            }
            return model.OrderByDescending(x=>x.CreateDate).ToPagedList(page, pageSize);
        }
        public User GetById(long id)
        {
            return db.Users.Find(id);
        }
        public User GetByPhone(string phone)
        {
            return db.Users.SingleOrDefault(x => x.Phone == phone);
        }
        public bool Update(User entity)
        {
            try
            {
                var user = db.Users.Find(entity.ID);
                user.ModifieDate = DateTime.Now;
                db.SaveChanges();
                return true;
            }
            catch
            {
                return false;
            }
        }
        public long Insert(User user)
        {
            var exist = db.Users.SingleOrDefault(x => x.UserName == user.UserName);
            if(exist != null)
            {
                return 0;
            }
            user.CreateDate = DateTime.Now;
            db.Users.Add(user);
            db.SaveChanges();
            return user.ID;
        }
        
        public List<string> GetListCredential(string userName)
        {
            
            var user = db.Users.Single(x => x.UserName == userName);
            var data = (from a in db.Credentials                       
                        join b in db.Roles on a.RoleID equals b.ID
                        where a.UserID == user.ID
                        select new
                        {
                            RoleID = a.RoleID,
                            UserID = a.UserID
                        }).AsEnumerable().Select(x => new Credential() { 
                            RoleID = x.RoleID,
                            UserID = x.UserID
                        });
            return data.Select(x => x.RoleID).ToList();
        }
        public User GetByUserName(string userName)
        {
            return db.Users.SingleOrDefault(x => x.UserName == userName);
        }

        public int Login(string username, string password, bool isLoginAdmin = false)
        {
            var result = db.Users.SingleOrDefault(x => x.UserName == username);
            if(result == null)
            {
                return 0;
            }
            else
            {
                if(isLoginAdmin == true)
                {
                    if(result.GroupID == "ADMIN" || result.GroupID == "MOD")
                    {
                        if (result.Status == false)
                        {
                            return -1;
                        }
                        else
                        {
                            if (result.Password == password)
                                return 1;
                            else
                                return 0;
                        }                        
                    }
                    else
                    {
                        return -2;
                    }
                }
                else
                {
                    if (result.Status == false)
                    {
                        return -1;
                    }
                    else
                    {
                        if (result.Password == password)
                            return 1;
                        else
                            return 0;
                    }
                }               
            }
        }
                
        public bool Delete(long id)
        {
            try
            {
                var user = db.Users.Find(id);
                db.Users.Remove(user);
                db.SaveChanges();
                return true;
            }
            catch
            {
                return false;
            }       
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Model.EF;
using PagedList;

namespace Model.Dao
{
    public class UserGroupDao
    {
        InventoryDbContext db = null;
        public UserGroupDao()
        {
            db = new InventoryDbContext();
        }
        public IEnumerable<UserGroup> ListAll(int page, int pageSize) 
        {
            var model = db.UserGroups.OrderByDescending(x => x.ID).ToPagedList(page, pageSize);
            return model;
        }

        public bool Insert(UserGroup userGroup)
        {
            var exist = db.UserGroups.SingleOrDefault(x => x.ID == userGroup.ID);
            if(exist != null)
            {
                return false;
            }
            else
            {
                db.UserGroups.Add(userGroup);
                db.SaveChanges();
                return true;
            }
        }

        public UserGroup getByID(string id)
        {
            return db.UserGroups.SingleOrDefault(x => x.ID == id);
        }

        public bool Update(UserGroup userGroup)
        {
            try
            {
                return true;
            }
            catch
            {
                return false;
            }
        }
        public bool Delete(string id)
        {
            try
            {
                var userGroup = db.UserGroups.SingleOrDefault(x => x.ID == id);
                db.UserGroups.Remove(userGroup);
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

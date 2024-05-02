using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Model.EF;
using PagedList;

namespace Model.Dao
{
    public class RoleDao
    {
        InventoryDbContext db = null;
        public RoleDao()
        {
            db = new InventoryDbContext();
        }

        public IEnumerable<Role> GetAll(string searchString, int page, int pageSize)
        {
            IQueryable<Role> model = db.Roles;
            if (!string.IsNullOrEmpty(searchString))
            {
                model = model.Where(x => x.Name.Contains(searchString));
            }
            return model.OrderByDescending(x => x.ID).ToPagedList(page, pageSize);
        }

        public bool Insert(Role role)
        {
            var exist = db.Roles.SingleOrDefault(x => x.ID == role.ID);
            if (exist != null)
            {
                return false;
            }
            db.Roles.Add(role);
            db.SaveChanges();
            return true;
        }

        public Role GetByID(string id)
        {
            return db.Roles.SingleOrDefault(x => x.ID == id);
        }
        public bool Update(string oldRoleID, Role role)
        {
            try
            {
                if (role == null || oldRoleID == null)
                    return false;

                var credentials = db.Credentials.Where(x => x.RoleID == oldRoleID).ToList();
                foreach (var cre in credentials)
                {
                    cre.RoleID = role.ID;
                }

                var existRole = db.Roles.SingleOrDefault(x => x.ID == oldRoleID);
                if (existRole != null)
                {
                    existRole.ID = role.ID;
                    existRole.Name = role.Name;
                }

                db.SaveChanges();
                return true;
            }
            catch(Exception ex)
            {
                Console.WriteLine("Lỗi khi cập nhật dữ liệu: " + ex.Message);
                return false;
            }
        }

        public bool Delete(string id)
        {
            try
            {
                var role = db.Roles.SingleOrDefault(x => x.ID == id);
                var credentials = db.Credentials.Where(x => x.RoleID == id);
                db.Credentials.RemoveRange(credentials);
                db.Roles.Remove(role);
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

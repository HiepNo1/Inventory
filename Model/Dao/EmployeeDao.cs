using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PagedList;
using Model.EF;
namespace Model.Dao
{
    public class EmployeeDao
    {
        InventoryDbContext db = null;
        public EmployeeDao()
        {
            db = new InventoryDbContext();
        }

        public IEnumerable<Employee> ListAllString(string searchString, int page, int pageSize)
        {
            IQueryable<Employee> model = db.Employees;
            if (!string.IsNullOrEmpty(searchString))
            {
                model = model.Where(x => x.Name.Contains(searchString) || x.Phone.Contains(searchString));
            }
            return model.OrderByDescending(x => x.CreateDate).ToPagedList(page, pageSize);
        }

        public long Insert(Employee employee)
        {
            var exist = db.Employees.SingleOrDefault(x => x.Phone == employee.Phone);
            if (exist != null)
            {
                return 0;
            }
            employee.CreateDate = DateTime.Now;
            employee.Status = true;
            db.Employees.Add(employee);
            db.SaveChanges();
            return employee.ID;
        }

        public Employee GetById(long id)
        {
            return db.Employees.Find(id);
        }

        public Employee GetByPhone(string phone)
        {
            return db.Employees.SingleOrDefault(x => x.Phone == phone);
        }
        public bool Update(Employee employee)
        {
            try
            {
                var emp = db.Employees.Find(employee.ID);
                emp.ModifieDate = DateTime.Now;
                emp.Name = employee.Name;
                emp.Address = employee.Address;
                emp.Phone = employee.Phone;
                emp.Email = employee.Email;
                emp.Gender = employee.Gender;
                emp.StartDate = employee.StartDate;
                emp.UserID = employee.UserID;
                db.SaveChanges();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public bool Delete(long id)
        {
            try
            {
                var emp = db.Employees.Find(id);
                db.Employees.Remove(emp);
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

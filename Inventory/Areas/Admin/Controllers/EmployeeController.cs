using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Inventory.Common;
using Model.Dao;
using Model.EF;

namespace Inventory.Areas.Admin.Controllers
{
    [ClearSessions("SelectedProducts", "SelectedReceipts")]
    public class EmployeeController : BaseController
    {
        [HasCredential(RoleID = "VIEW_EMPLOYEE")]
        public ActionResult Index(string searchString, int page = 1, int pageSize = 10)
        {
            var dao = new EmployeeDao();
            var model = dao.ListAllString(searchString, page, pageSize);
            ViewBag.searchString = searchString;
            return View(model);
        }

        [HttpGet]
        [HasCredential(RoleID = "ADD_EMPLOYEE")]
        public ActionResult Create()
        {
            return View();
        }
        [HttpPost]
        [HasCredential(RoleID = "ADD_EMPLOYEE")]
        public ActionResult Create(Employee employee)
        {
            if (ModelState.IsValid)
            {
                var dao = new EmployeeDao();
                long id = dao.Insert(employee);
                if (id == 0)
                {
                    TempData["message"] = new XMessage("danger", "Nhân viên đã tồn tại");
                    return View(employee);
                }
                if (id > 0)
                {
                    TempData["message"] = new XMessage("success", "Thêm thành công");
                    return RedirectToAction("Index");
                }
                else
                {
                    TempData["message"] = new XMessage("danger", "Thêm không thành công");
                }
            }
            return View(employee);
        }

        [HttpGet]
        [HasCredential(RoleID = "EDIT_EMPLOYEE")]
        public ActionResult Edit(long id)
        {
            var cus = new EmployeeDao().GetById(id);
            return View(cus);
        }
        [HttpPost]
        [HasCredential(RoleID = "EDIT_EMPLOYEE")]
        public ActionResult Edit(Employee employee)
        {
            if (ModelState.IsValid)
            {
                var dao = new EmployeeDao();
                bool result = dao.Update(employee);
                if (result)
                {
                    TempData["message"] = new XMessage("success", "Sửa thành công");

                    return RedirectToAction("Index");
                }
                else
                {
                    TempData["message"] = new XMessage("danger", "Sửa không thành công");
                }
            }
            return View(employee);
        }

        [HasCredential(RoleID = "DELETE_EMPLOYEE")]
        public ActionResult Delete(long Id)
        {
            new EmployeeDao().Delete(Id);
            TempData["message"] = new XMessage("success", "Xóa thành công");
            return RedirectToAction("Index");
        }

        [HasCredential(RoleID = "ADD_USER")]
        [HttpPost]
        public ActionResult SaveUser(User user)
        {
            if (ModelState.IsValid)
            {
                var encriptedMd5Pass = Encriptor.MD5Hash(user.Password);
                user.Password = encriptedMd5Pass;
                user.Status = true;
                user.CreateBy = GetUserSession().UserName;
                var userDao = new UserDao();
                long id = userDao.Insert(user);
                if (id == 0)
                {
                    return Json(new { success = false, message = "Tài khoản này đã được sử dụng" });
                }
                if (id > 0)
                {
                    var empDao = new EmployeeDao();
                    var emp = empDao.GetByPhone(user.Phone);
                    if(emp != null)
                    {
                        emp.UserID = id;
                        empDao.Update(emp);
                    }
                    return Json(new { success = true, message = "Thêm tài khoản thành công" });
                }
                
            }
            return Json(new { success = true, message = "Thêm tài khoản thất bại" });
        }
    }
}
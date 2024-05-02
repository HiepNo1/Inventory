using Model.Dao;
using Model.EF;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Inventory.Areas.Admin.Controllers
{
    [ClearSessions("SelectedProducts", "SelectedReceipts")]
    public class RoleController : BaseController
    {
        // GET: Admin/Role
        public ActionResult Index(string searchString, int page = 1, int pageSize = 10)
        {
            var dao = new RoleDao();
            var roles = dao.GetAll(searchString, page, pageSize);
            ViewBag.searchString = searchString;
            return View(roles);
        }

        [HttpGet]
        public ActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Create(Role role)
        {
            if(ModelState.IsValid)
            {
                var dao = new RoleDao();
                bool result = dao.Insert(role);
                if (!result)
                {
                    ModelState.AddModelError("", "Quyền này đã tồn tại");
                    return View(role);
                }
               
                else
                {
                    return RedirectToAction("Index");
                }
            }
            return View(role);
        }

        [HttpGet]
        public ActionResult Edit(string id)
        {
            var role = new RoleDao().GetByID(id);
            return View(role);
        }
        [HttpPost]
        public ActionResult Edit(string oldID, Role role)
        {
            if (ModelState.IsValid)
            {
                var dao = new RoleDao();
                bool result = dao.Update(oldID, role);
                if (!result)
                {
                    return RedirectToAction("Index");
                }
                else
                {
                    ModelState.AddModelError("", "Sửa không thành công");
                }
            }
            return View(role);
        }

        public ActionResult Delete(string id)
        {
            new RoleDao().Delete(id);
            return RedirectToAction("Index");
        }

        public ActionResult Authorization(string userID)
        {
            return View();
        }
       
    }
}
using Inventory.Common;
using Model.Dao;
using Model.EF;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Inventory.Areas.Admin.Controllers
{
    public class RoleController : BaseController
    {
        // GET: Admin/Role
        [HasCredential(RoleID = "VIEW_ROLE")]
        public ActionResult Index(string searchString, int page = 1, int pageSize = 10)
        {
            var dao = new RoleDao();
            var roles = dao.GetAll(searchString, page, pageSize);
            ViewBag.searchString = searchString;
            return View(roles);
        }

        [HttpGet]
        [HasCredential(RoleID = "ADD_ROLE")]
        public ActionResult Create()
        {
            return View();
        }
        [HttpPost]
        [HasCredential(RoleID = "ADD_ROLE")]
        public ActionResult Create(Role role)
        {
            if(ModelState.IsValid)
            {
                var dao = new RoleDao();
                bool result = dao.Insert(role);
                if (!result)
                {
                    TempData["message"] = new XMessage("danger", "Quyền này đã tồn tại");
                    return View(role);
                }
               
                else
                {
                    TempData["message"] = new XMessage("success", "Thêm thành công");
                    return RedirectToAction("Index");
                }
            }
            return View(role);
        }

        [HttpGet]
        [HasCredential(RoleID = "EDIT_ROLE")]
        public ActionResult Edit(string id)
        {
            var role = new RoleDao().GetByID(id);
            return View(role);
        }
        [HttpPost]
        [HasCredential(RoleID = "EDIT_ROLE")]
        public ActionResult Edit(string oldID, Role role)
        {
            if (ModelState.IsValid)
            {
                var dao = new RoleDao();
                bool result = dao.Update(oldID, role);
                if (!result)
                {
                    TempData["message"] = new XMessage("success", "Sửa thành công");
                    return RedirectToAction("Index");
                }
                else
                {
                    TempData["message"] = new XMessage("danger", "Sửa không thành công");
                }
            }
            return View(role);
        }

        [HasCredential(RoleID = "DELETE_ROLE")]
        public ActionResult Delete(string id)
        {
            new RoleDao().Delete(id);
            TempData["message"] = new XMessage("success", "Xóa thành công");
            return RedirectToAction("Index");
        }     
    }
}
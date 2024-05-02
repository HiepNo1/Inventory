using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Model.Dao;
using Model.EF;

namespace Inventory.Areas.Admin.Controllers
{
    [ClearSessions("SelectedProducts", "SelectedReceipts")]
    public class UserGroupController : BaseController
    {
        // GET: Admin/UserGroup
        public ActionResult Index(int page = 1, int pageSize = 10)
        {
            var dao = new UserGroupDao();
            var model = dao.ListAll(page, pageSize);
            return View(model);
        }

        [HttpGet]
        public ActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Create(UserGroup userGroup)
        {
            if (ModelState.IsValid)
            {
                var dao = new UserGroupDao();
                bool result = dao.Insert(userGroup);
                if (!result)
                {
                    ModelState.AddModelError("", "Nhóm người dùng này đã tồn tại");
                    return View(userGroup);
                }

                else
                {
                    return RedirectToAction("Index");
                }
            }
            return View(userGroup);
        }

        [HttpGet]
        public ActionResult Edit(string id)
        {
            return View(new UserGroupDao().getByID(id));
        }
        [HttpPost]
        public ActionResult Edit(UserGroup userGroup)
        {
            if(ModelState.IsValid)
            {
                var dao = new UserGroupDao();
                bool result = dao.Update(userGroup);
                if(result)
                {
                    return RedirectToAction("Index");
                }
                else
                {
                    ModelState.AddModelError("", "Thêm ko thành công");
                }
            }
            return View("Index");
        }

        public ActionResult Delete(string id)
        {
            new UserGroupDao().Delete(id);
            return RedirectToAction("Index");
        }
    }
}
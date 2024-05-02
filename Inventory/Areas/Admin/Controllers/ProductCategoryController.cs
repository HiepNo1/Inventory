using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Inventory.Common;
using Model.Dao;
using Model.EF;
using PagedList;

namespace Inventory.Areas.Admin.Controllers
{
    [ClearSessions("SelectedProducts", "SelectedReceipts")]
    public class ProductCategoryController : Controller
    {
        // GET: Admin/ProductCategory
        [HasCredential(RoleID = "VIEW_CATEGORY")]
        public ActionResult Index(string searchString, int page = 1, int pageSize = 10)
        {
            var dao = new CategoryDao();
            var model = dao.ListAllPaging(searchString, page, pageSize);
            ViewBag.searchString = searchString;
            return View(model);
        }

        [HttpGet]
        [HasCredential(RoleID = "ADD_CATEGORY")]
        public ActionResult Create()
        {
            return View();
        }
        [HttpPost]
        [HasCredential(RoleID = "ADD_CATEGORY")]
        public ActionResult Create(ProductCategory category)
        {
            if(ModelState.IsValid)
            {
                var dao = new CategoryDao();
                category.Status = true;
                category.CreateDate = DateTime.Now;
                long id = dao.Insert(category);
                if(id == 0)
                {
                    ModelState.AddModelError("", "Danh mục sản phẩm này đã tồn tại");
                    return View(category);
                }
                if(id > 0)
                {
                    return RedirectToAction("Index");
                }
                else
                {
                    ModelState.AddModelError("", "Thêm ko thành công");
                }
            }
            return View(category);
        }

        [HttpGet]
        [HasCredential(RoleID = "EDIT_CATEGORY")]
        public ActionResult Edit(long id)
        {
            var category = new CategoryDao().GetById(id);
            return View(category);
        }

        [HttpPost]
        [HasCredential(RoleID = "EDIT_CATEGORY")]
        public ActionResult Edit(ProductCategory category)
        {
            if (ModelState.IsValid)
            {
                var dao = new CategoryDao();
                category.CreateDate = DateTime.Now;
                bool result = dao.Update(category);
                if (result)
                {
                    return RedirectToAction("Index");
                }
                else
                {
                    ModelState.AddModelError("", "Sửa ko thành công");
                }
            }
            return View(category);
        }

        [HasCredential(RoleID = "DELETE_CATEGORY")]
        public ActionResult Delete(long id)
        {
            new CategoryDao().Delete(id);
            return RedirectToAction("Index");
        }
    }
}
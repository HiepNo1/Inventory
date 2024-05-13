using Model.Dao;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PagedList;
using Model.EF;
using Inventory.Common;

namespace Inventory.Areas.Admin.Controllers
{
    public class SupplierController : BaseController
    {
        private SupplierDao supplierDao = new SupplierDao();
        // GET: Admin/Supplier
        [HasCredential(RoleID = "VIEW_SUPPLIER")]
        public ActionResult Index(string searchString, int page = 1, int pageSize = 10)
        {
            var model = supplierDao.ListAllPaging("Index",searchString, page, pageSize);
            ViewBag.searchString = searchString;
            return View(model);
        }

        [HasCredential(RoleID = "VIEW_SUPPLIER")]
        public ActionResult Trash(string searchString, int page = 1, int pageSize = 10)
        {

            var model = supplierDao.ListAllPaging("Trash", searchString, page, pageSize);
            ViewBag.searchString = searchString;
            return View(model);
        }

        [HasCredential(RoleID = "ADD_SUPPLIER")]
        [HttpGet]
        public ActionResult Create()            
        {
            return View();
        }
        [HasCredential(RoleID = "ADD_SUPPLIER")]
        [HttpPost] 
        public ActionResult Create(Supplier supplier)
        {
            if (ModelState.IsValid)
            {         
                supplier.Status = true;
                supplier.CreateDate = DateTime.Now;
                supplier.CreateBy = GetUserSession().UserName;
                long id = supplierDao.Insert(supplier);
                if(id == 0)
                {
                    TempData["message"] = new XMessage("danger", "Nhà cung cấp này đã tồn tại");
                    return View(supplier);
                }
                if(id > 0)
                {
                    TempData["message"] = new XMessage("success", "Thêm nhà cung cấp thành công");
                    return RedirectToAction("Index");
                }
                else
                {
                    TempData["message"] = new XMessage("danger", "Thêm nhà cung cấp không thành công");
                    return RedirectToAction("Index");
                }               
            }
            return View(supplier);
        }

        [HasCredential(RoleID = "EDIT_SUPPLIER")]
        [HttpGet]
        public ActionResult Edit(long id)
        {
            var supp = supplierDao.GetById(id);
            return View(supp);
        }
        [HasCredential(RoleID = "EDIT_SUPPLIER")]
        [HttpPost]
        public ActionResult Edit(Supplier supplier)
        {
            if (ModelState.IsValid)
            {
                supplier.Status = true;
                supplier.CreateDate = DateTime.Now;
                bool result = supplierDao.Update(supplier);           
                if (result)
                {
                    TempData["message"] = new XMessage("success", "Sửa nhà cung cấp thành công");
                    return RedirectToAction("Index");
                }
                else
                {
                    TempData["message"] = new XMessage("danger", "Sửa nhà cung cấp không thành công");
                    return RedirectToAction("Index");
                }
            }
            return View(supplier);
        }

        [HasCredential(RoleID = "DELETE_SUPPLIER")]
        public ActionResult Delete(long id)
        {
            bool result = supplierDao.Delete(id);
            if (!result)
            {
                TempData["message"] = new XMessage("success", "Xóa nhà cung cấp thành công");
                return RedirectToAction("Index");
            }
            else
            {
                TempData["message"] = new XMessage("danger", "Nhà cung cấp không tồn tại");
                return RedirectToAction("Index");
            }        
        }

        [HasCredential(RoleID = "EDIT_SUPPLIER")]
        public ActionResult Status(long id)
        {
            var supplier = supplierDao.GetById(id);
            if (supplier == null)
            {
                TempData["message"] = new XMessage("danger", "Nhà cung cấp không tồn tại");
                return RedirectToAction("Index");
            }
            supplier.Status = (supplier.Status == true) ? false : true;
            supplier.ModifieBy = GetUserSession().UserName;
            supplierDao.Update(supplier);
            TempData["message"] = new XMessage("success", "Thay đổi trạng thái thành công");
            return RedirectToAction("Index");
        }

        [HasCredential(RoleID = "EDIT_SUPPLIER")]
        public ActionResult DeleTrash(long id)
        {
            var supplier = supplierDao.GetById(id);
            if (supplier == null)
            {
                TempData["message"] = new XMessage("danger", "Nhà cung cấp không tồn tại");
                return RedirectToAction("Index");
            }
            supplier.Status = false;
            supplier.ModifieBy = GetUserSession().UserName;
            supplierDao.Update(supplier);
            TempData["message"] = new XMessage("success", "Thay đổi trạng thái thành công");
            return RedirectToAction("Index");
        }

        [HasCredential(RoleID = "EDIT_SUPPLIER")]
        public ActionResult ReTrash(long id)
        {
            var supplier = supplierDao.GetById(id);
            if (supplier == null)
            {
                TempData["message"] = new XMessage("danger", "Nhà cung cấp không tồn tại");
                return RedirectToAction("Index");
            }
            supplier.Status = true;
            supplier.ModifieBy = GetUserSession().UserName;
            supplierDao.Update(supplier);
            TempData["message"] = new XMessage("success", "Nhà cung cấp được khôi phục thành công");
            return RedirectToAction("Index");
        }
    }
}
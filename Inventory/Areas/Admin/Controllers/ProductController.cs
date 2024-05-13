using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Inventory.Common;
using Model.Dao;
using Model.EF;
using PagedList;

namespace Inventory.Areas.Admin.Controllers
{
    public class ProductController : BaseController
    {
        private ProductDao productDao = new ProductDao();
        private InventoryDbContext db = new InventoryDbContext();
        // GET: Admin/Product

        [HasCredential(RoleID = "VIEW_PRODUCT")]
        public ActionResult Index(string searchString, decimal? firstMoney, decimal? lastMoney, string cateName, int page = 1, int pageSize = 5)
        {
            var model = productDao.ListAllString(searchString, firstMoney, lastMoney, cateName, page, pageSize);
            ViewBag.firstMoney = firstMoney;
            ViewBag.lastmoney = lastMoney;
            ViewBag.searchString = searchString;
            ViewBag.cateName = cateName;
            return View(model);
        }

        [HasCredential(RoleID = "VIEW_PRODUCT")]
        [HttpGet]
        public ActionResult Detail(long id)
        {
            var model = productDao.Detail(id);
            return View(model);
        }

        [HasCredential(RoleID = "ADD_PRODUCT")]
        [HttpGet]
        public ActionResult Create()
        {
            return View();
        }

        [HasCredential(RoleID = "ADD_PRODUCT")]
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Create(Product product, HttpPostedFileBase fileImage)
        {
            if (ModelState.IsValid)
            {
                if (fileImage != null && fileImage.ContentLength > 0)
                {
                    string rootFolder = Server.MapPath("/Images/");
                    string pathImage = rootFolder + fileImage.FileName;
                    fileImage.SaveAs(pathImage);
                    product.Image = "/Images/" + fileImage.FileName;
                }
                product.CreateBy = GetUserSession().UserName;
                long id = productDao.Insert(product);
                if (id == 0)
                {
                    TempData["message"] = new XMessage("danger", "Sản phẩm này đã tồn tại");

                    return View(product);
                }
                if (id > 0)
                {
                    TempData["message"] = new XMessage("success", "Thêm sản phẩm thành công");
                    return RedirectToAction("Index");
                }
                else
                {
                    TempData["message"] = new XMessage("danger", "Thêm sản phẩm không thành công");
                    return RedirectToAction("Index");
                }
            }
            return View(product);
        }

        [HasCredential(RoleID = "EDIT_PRODUCT")]
        [HttpGet]
        public ActionResult Edit(long id)
        {
            var pro = productDao.GetByID(id);
            return View(pro);
        }
        [HasCredential(RoleID = "EDIT_PRODUCT")]
        [HttpPost]
        public ActionResult Edit(Product product, HttpPostedFileBase fileImage, string imageFileName)
        {
            if (ModelState.IsValid)
            {
                if (fileImage != null && fileImage.ContentLength > 0)
                {
                    string rootFolder = Server.MapPath("/Images/");
                    string pathImage = rootFolder + fileImage.FileName;
                    fileImage.SaveAs(pathImage);
                    product.Image = "/Images/" + fileImage.FileName;
                }
                else
                {
                    product.Image = imageFileName;
                }
                product.ModifieBy = GetUserSession().UserName;
                product.Status = true;
                product.ModifieDate = DateTime.Now;
                bool result = productDao.Update(product);
                if (result)
                {
                    TempData["message"] = new XMessage("success", "Sửa sản phẩm thành công");
                    return RedirectToAction("Index");
                }
                else
                {
                    TempData["message"] = new XMessage("danger", "Sản phẩm không tồn tại");
                    return RedirectToAction("Index");
                }
            }
            return View(product);
        }

        [HasCredential(RoleID = "DELETE_PRODUCT")]
        public ActionResult Delete(long id)
        {
            bool proToDelete = productDao.Delete(id);
            if (proToDelete)
            {
                TempData["message"] = new XMessage("success", "Xóa sản phẩm thành công");
            }
            else
            {
                TempData["message"] = new XMessage("danger", "Sản phẩm không tồn tại");
            }
            return RedirectToAction("Index");
        }

        [HasCredential(RoleID = "VIEW_PRODUCT")]
        public ActionResult Status(long id)
        {
            var product = productDao.GetByID(id);
            if (product == null)
            {
                TempData["message"] = new XMessage("danger", "Sản phẩm không tồn tại");
                return RedirectToAction("Index");
            }
            product.ModifieBy = GetUserSession().UserName;
            product.Status = (product.Status == true) ? false : true;
            productDao.Update(product);
            TempData["message"] = new XMessage("success", "Thay đổi trạng thái thành công");
            return RedirectToAction("Index");
        }

        [HasCredential(RoleID = "VIEW_PRODUCT")]
        public ActionResult DeleTrash(long id)
        {
            var product = productDao.GetByID(id);
            if (product == null)
            {
                TempData["message"] = new XMessage("danger", "Sản phẩm không tồn tại");
                return RedirectToAction("Index");
            }
            product.ModifieBy = GetUserSession().UserName;
            product.Status = false;
            productDao.Update(product);
            TempData["message"] = new XMessage("success", "Xóa vào thùng rác thành công");
            return RedirectToAction("Index");
        }

        [HasCredential(RoleID = "VIEW_PRODUCT")]
        public ActionResult Trash(string searchString, int page = 1, int pageSize = 5)
        {
            var model = productDao.ListTrash(searchString, page, pageSize);
            ViewBag.searchString = searchString;
            return View(model);
        }

        [HasCredential(RoleID = "VIEW_PRODUCT")]
        public ActionResult Retrash(long id)
        {
            var product = productDao.GetByID(id);
            if (product == null)
            {
                TempData["message"] = new XMessage("danger", "Sản phẩm không tồn tại");
                return RedirectToAction("Trash");
            }
            product.ModifieBy = GetUserSession().UserName;
            product.Status = true;
            productDao.Update(product);
            TempData["message"] = new XMessage("success", "Sản phẩm đã được khôi phục thành công");
            return RedirectToAction("Trash");
        }

    }

}
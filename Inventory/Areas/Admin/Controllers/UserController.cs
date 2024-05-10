using Model.Dao;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PagedList;
using Model.EF;
using Inventory.Common;
using Model.ViewModels;
using System.Diagnostics;

namespace Inventory.Areas.Admin.Controllers
{
    [ClearSessions("SelectedProducts", "SelectedReceipts")]
    public class UserController : BaseController
    {
        private UserDao userDao = new UserDao();
        private InventoryDbContext db = new InventoryDbContext();
        // GET: Admin/User
        [HasCredential(RoleID = "VIEW_USER")]
        public ActionResult Index(string searchString, int page = 1, int pageSize = 10)
        {
            var users = userDao.ListAllPaging("", searchString, page, pageSize);
            ViewBag.searchString = searchString;
            return View(users);
        }

        [HttpGet]
        [HasCredential(RoleID = "ADD_USER")]
        public ActionResult Create()
        {
            return View();
        }
        [HttpPost]
        [HasCredential(RoleID = "ADD_USER")]
        public ActionResult Create(User user)
        {
            if (ModelState.IsValid)
            {
                var encriptedMd5Pass = Encriptor.MD5Hash(user.Password);
                user.Password = encriptedMd5Pass;
                user.Status = true;
                user.CreateBy = GetUserSession().UserName;
                long id = userDao.Insert(user);
                if (id == 0)
                {
                    TempData["message"] = new XMessage("danger", "Tài khoản này đã được sử dụng");
                    return View(user);
                }
                if (id > 0)
                {
                    TempData["message"] = new XMessage("success", "Thêm tài khoản thành công");
                    return RedirectToAction("Index");
                }
                else
                {
                    TempData["message"] = new XMessage("danger", "Thêm tài khoản không thành công");
                    return View(user);
                }
            }
            return View(user);
        }

        [HttpGet]
        [HasCredential(RoleID = "EDIT_USER")]
        public ActionResult Edit(long id)
        {
            var user = userDao.GetById(id);
            return View(user);
        }

        [HttpPost]
        [HasCredential(RoleID = "EDIT_USER")]
        public ActionResult Edit(User user)
        {
            ModelState.Remove("UserName");
            ModelState.Remove("Password");
            if (ModelState.IsValid)
            {
                user.Status = true;
                user.ModifieBy = GetUserSession().UserName;
                var result = userDao.Update(user);
                if (result)
                {
                    TempData["message"] = new XMessage("success", "Sửa tài khoản thành công");
                    return RedirectToAction("Index");
                }
                else
                {
                    TempData["message"] = new XMessage("danger", "Sửa tài khoản không thành công");
                    return View(user);
                }
            }
            return View(user);
        }

        [HasCredential(RoleID = "DELETE_USER")]
        public ActionResult Delete(long id)
        {
            new UserDao().Delete(id);
            TempData["message"] = new XMessage("success", "Xóa tài khoản thành công");
            return RedirectToAction("Index");
        }

        [HasCredential(RoleID = "EDIT_USER")]
        public ActionResult Status(long id)
        {
            var user = userDao.GetById(id);
            if (user == null)
            {
                TempData["message"] = new XMessage("danger", "Người dùng không tồn tại");
                return RedirectToAction("Index");
            }
            else
            {
                user.ModifieBy = GetUserSession().UserName;
                user.Status = user.Status == true ? false : true;
                userDao.Update(user);
                TempData["message"] = new XMessage("success", "Thay đổi trạng thái thành công");
                return RedirectToAction("Index");
            }
        }
        [HasCredential(RoleID = "VIEW_USER")]
        public ActionResult Trash(string searchString, int page = 1, int pageSize = 10)
        {
            var users = userDao.ListAllPaging("Trash", searchString, page, pageSize);
            ViewBag.searchString = searchString;
            return View(users);
        }
        [HasCredential(RoleID = "EDIT_USER")]
        public ActionResult ReTrash(long id)
        {
            var user = userDao.GetById(id);
            if (user == null)
            {
                TempData["message"] = new XMessage("danger", "Người dùng không tồn tại");
                return RedirectToAction("Index");
            }
            else
            {
                user.ModifieBy = GetUserSession().UserName;
                user.Status = true;
                userDao.Update(user);
                TempData["message"] = new XMessage("success", "Tài khoản đã được khôi phục thành công");
                return RedirectToAction("Index");
            }
        }

        [HasCredential(RoleID = "EDIT_USER")]
        public ActionResult DeleTrash(long id)
        {
            var user = userDao.GetById(id);
            if (user == null)
            {
                TempData["message"] = new XMessage("danger", "Người dùng không tồn tại");
                return RedirectToAction("Index");
            }
            else
            {
                user.ModifieBy = GetUserSession().UserName;
                user.Status = false;
                userDao.Update(user);
                TempData["message"] = new XMessage("success", "Tài khoản đã được đưa vào thùng rác");
                return RedirectToAction("Index");
            }
        }

        public void GetAllPermission(long id, AllPermission allPermission)
        {
            allPermission.ProductPermission = GetChildPermissions("_PRODUCT", id);
            allPermission.SupplierPermission = GetChildPermissions("_SUPPLIER", id);
            allPermission.CustomerPermission = GetChildPermissions("_CUSTOMER", id);
            allPermission.CategoryPermission = GetChildPermissions("_CATEGORY", id);
            allPermission.UserPermission = GetChildPermissions("_USER", id);
            allPermission.OrderPermission = GetChildPermissions("_ORDER", id);
            allPermission.ReceiptPermission = GetChildPermissions("_RECEIPT", id);
            allPermission.StatisticPermission = GetChildPermissions("_STATISTIC", id);
            allPermission.EmployeePermission = GetChildPermissions("_EMPLOYEE", id);
            allPermission.RolePermission = GetChildPermissions("_ROLE", id);
            var user = userDao.GetById(id);
            ViewBag.UserName = user.UserName;           
        }
        [HttpGet]
        public ActionResult Permissions(long id)
        {
            AllPermission allPermission = new AllPermission();
            GetAllPermission(id, allPermission);
            return View(allPermission);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Permissions(long id, FormCollection form)
        {
            if (ModelState.IsValid)
            {
                var selectedPermissions = new List<string>();
                foreach (var key in form.AllKeys)
                {
                    if (key.Contains("IsChecked"))
                    {
                        if (form[key] == "true")
                        {
                            var permissionId = key.Split('.')[1];
                            selectedPermissions.Add(permissionId);
                        }
                    }
                }
                SaveCredentials(selectedPermissions, id);
                TempData["message"] = new XMessage("success", "Phân quyền cho người dùng thành công");
                return RedirectToAction("Index");
            }
            var allPermission = new AllPermission();
            GetAllPermission(id, allPermission);
            return View(allPermission);
        }

        public void SaveCredentials(List<string> selectedPermissions, long userId)
        {
            var oldCredentials = db.Credentials.Where(c => c.UserID == userId).ToList();
            db.Credentials.RemoveRange(oldCredentials);
            db.SaveChanges();

            foreach (var permissionId in selectedPermissions)
            {
                db.Credentials.Add(new Credential { UserID = userId, RoleID = permissionId });       
            }
            db.SaveChanges();
        }

        public List<PermissionViewModel> GetChildPermissions(string id, long userID)
        {
            var childPermissions = db.Roles.Where(r => r.ID.Contains(id)).Select(r => new PermissionViewModel
            {
                ID = r.ID,
                Name = r.Name,
                IsChecked = db.Credentials.Any(c => c.UserID == userID && c.RoleID == r.ID)
            })
        .ToList();
            return childPermissions;
        }
    }
}
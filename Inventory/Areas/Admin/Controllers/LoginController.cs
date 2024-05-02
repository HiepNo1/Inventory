using Inventory.Areas.Admin.Models;
using Inventory.Common;
using Model.Dao;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Inventory.Areas.Admin.Controllers
{
    public class LoginController : Controller
    {
        // GET: Admin/Login
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult Login(LoginModel model)
        {
            if(ModelState.IsValid)
            {
                var dao = new UserDao();
                var result = dao.Login(model.UserName, Encriptor.MD5Hash(model.PassWord),true);
                if (result == 1)
                {   
                    var user = dao.GetByUserName(model.UserName);
                    var userSession = new UserLogin();
                    userSession.UserName = user.UserName;
                    userSession.UserID = user.ID;
                    userSession.GroupID = user.GroupID;
                    var listCredentials = dao.GetListCredential(model.UserName);
                    Session.Add(CommonConstants.USER_SESSION, userSession);                    
                    Session.Add(CommonConstants.SESSION_CREDENTIALS, listCredentials);
                    return RedirectToAction("Index", "Home");
                }
                else if(result == 0)
                {
                    ModelState.AddModelError("", "Tài khoản hoặc mật khẩu không đúng");
                }
                else if(result == -1)
                {
                    ModelState.AddModelError("", "Tài khoản đang bị khóa");
                }               
                else if (result == -2)
                {
                    ModelState.AddModelError("", "Tài khoản của bạn không có quyền đăng nhập");
                }
                else
                {
                    ModelState.AddModelError("", "Đăng nhập ko đúng");
                }
            }
            return View("Index");
        }

        public ActionResult Logout()
        {
            Session.Clear();
            return RedirectToAction("Index");
        }
    }
}
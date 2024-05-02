using Inventory.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace Inventory.Areas.Admin.Controllers
{
    public class BaseController : Controller
    {
        // GET: Admin/Base
        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            var sess = (UserLogin)Session[CommonConstants.USER_SESSION];
            if (sess == null)
            {
                filterContext.Result = new RedirectToRouteResult(new
                    RouteValueDictionary(new { controller = "Login", action = "Index", Area = "Admin" }));
            }
            base.OnActionExecuting(filterContext);
        }

        protected UserLogin GetUserSession()
        {
            // Lấy thông tin người dùng đang đăng nhập từ session
            var userSession = Session[CommonConstants.USER_SESSION] as UserLogin;
            return userSession;
        }
    }
}
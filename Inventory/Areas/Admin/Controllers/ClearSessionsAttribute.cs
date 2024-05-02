using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Inventory.Areas.Admin.Controllers
{
    public class ClearSessionsAttribute : ActionFilterAttribute
    {
        private readonly string[] _sessionKeys;

        public ClearSessionsAttribute(params string[] sessionKeys)
        {
            _sessionKeys = sessionKeys;
        }

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            foreach (var key in _sessionKeys)
            {
                filterContext.HttpContext.Session.Remove(key);
            }

            base.OnActionExecuting(filterContext);
        }
    }
}
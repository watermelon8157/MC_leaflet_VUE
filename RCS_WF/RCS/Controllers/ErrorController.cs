using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace RCS.Controllers
{
    [HandleError]
    public class ErrorController : Controller
    {
        //
        // GET: /Error/

        public ActionResult General(String error)
        {
            ViewData["Title"] = "抱歉,處理你的請求發生錯誤";
            ViewData["Description"] = error;
            return View("Error");
        }

        public ActionResult StatusCode404(String error)
        {
            ViewData["Title"] = "抱歉, 處理你的請求發生404錯誤";
            ViewData["Description"] = error;
            return View("Error");
        }

        public ActionResult StatusCode500(String error)
        {
            ViewData["Title"] = "抱歉,處理你的請求發生500錯誤";
            ViewData["Description"] = error;
            return View("Error");
        }

    }
}

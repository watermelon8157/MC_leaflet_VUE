using RCS_Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace RCS.Areas.Backend.Controllers
{
    public class AdminController : RCS.Areas.Backend.Controllers.BaseController
    {
        //
        // GET: /Backend/Admin/
        [RCS.Controllers.Silencer(CheckPat_infoSession = false, CheckUser_infoSession = false)]
        public ActionResult Index()
        {
#if DEBUG
            ViewData["userid"] = "rcs";
            ViewData["userpwd"] = "!QAZ2wsx";
#else
            ViewData["userid"] = "";
            ViewData["userpwd"] = "";
#endif
            return View();
        }

       
        /// <summary>
        /// 進入模組
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ActionResult EnterModule()
        {
            if (!string.IsNullOrWhiteSpace(Request["loginMsg"]))
            {
                ViewData["loginMsg"] = Request["loginMsg"].ToString();
            }
            return RedirectToAction("Main");
        }


        /// <summary>
        /// 登出
        /// </summary>
        /// <returns></returns>
        [RCS.Controllers.Silencer(CheckPat_infoSession = false, CheckUser_infoSession = false)]
        public EmptyResult LogOut()
        {
            Session.RemoveAll();
            if (TempData["message"] == null)
            {
                TempData["message"] = "登出系統!";
            }
            else
            {
                TempData["message"] = TempData["message"];
            }
            return new EmptyResult();
        }


        public ActionResult Main()
        {
            return View();
        }
    }
}

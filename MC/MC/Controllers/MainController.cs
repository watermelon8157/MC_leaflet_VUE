using Com.Mayaminer;
using RCS_Data;
using System;
using System.Web.Mvc;
using RCSData.Models;
using RCS_Data.Models.DB;
using System.Collections.Generic;

namespace RCS.Controllers
{
    public class MainController : BaseController {
         
        /// <summary>
        /// 登入畫面
        /// </summary>
        /// <returns></returns>
        [Silencer(CheckPat_infoSession = false,CheckUser_infoSession =false)]
        public ActionResult Index() {

            //WebMethod wm = new WebMethod();

            //List<DB_MC_PATIENT_INFO> tempList = new List<DB_MC_PATIENT_INFO>();
            //SQLProvider dbLink = new SQLProvider();
            //string sql = "SELECT * FROM MC_PATIENT_INFO_TEMP";
            //// string sql = "SELECT * FROM " + DB_TABLE_NAME.DB_MC_PATIENT_INFO;
            //List<DB_MC_PATIENT_INFO> pList = dbLink.DBA.getSqlDataTable<DB_MC_PATIENT_INFO>(sql);
            //if (pList.Count > 0)
            //{
            //    foreach (DB_MC_PATIENT_INFO item in pList)
            //    {
            //        if (!tempList.Exists(x=>x.PATIENT_ID  == item.PATIENT_ID && x.SITE_ID == item.SITE_ID))
            //        {
            //            tempList.Add(item);
            //        }
            //    }
            //}
            //if (tempList.Count > 0)
            //{
            //    dbLink.DBA.DBExecInsert<DB_MC_PATIENT_INFO>(tempList);
            //}
            try
            {
                Session.RemoveAll();
                
                //傳入值
                //檢察是否有訊息
                if (Request["message"] != null)
                    TempData["message"] = Request["message"].ToString();
                if (TempData["message"] != null)
                    TempData["message"] = TempData["message"].ToString();

#if DEBUG
                ViewData["userid"] = "rcs";
                ViewData["userpwd"] = "!QAZ2wsx";
#else
            ViewData["userid"] = "";
            ViewData["userpwd"] = "";
#endif

                DateTime Jan1st1970 = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
                long ooo = (long)(DateTime.Now - Jan1st1970).TotalMilliseconds;
                var posixTime = DateTime.SpecifyKind(new DateTime(1970, 1, 1), DateTimeKind.Local);
                var time = posixTime.AddMilliseconds(1486450927320);
                string t = time.ToString("yyyy/MM/dd HH:mm:ss:fff");

            }
            catch (Exception ex)
            {
                LogTool.SaveLogMessage(ex, "Index", "MainController");
                TempData["message"] = "程式發生錯誤，請洽資訊人員!";
            }

            return View();
        }

        /// <summary>
        /// 進入模組
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ActionResult EnterModule()
        {
            //如果有登入訊息，成功登入後顯示
            if(!string.IsNullOrWhiteSpace(Request["loginMsg"]))
            {
                TempData["loginMsg"] = Request["loginMsg"].ToString();
            }
            //目前只有定義 RT
            if (Request["module"] != null && user_info != null)
            {
                switch (Request["module"].ToString())
                {
                    case "RT":
                        return RedirectToAction("Index", "RT");
                    case "Admin":
                        return RedirectToAction("Index", "Admin");
                    default:
                        return RedirectToAction("Index", "Main");
                }
            }
            else
            {
                return RedirectToAction("Index", "Main", new { message = "非法進入" });
            }
        }

        /// <summary>
        /// 登出
        /// </summary>
        /// <returns></returns>
        [Silencer(CheckPat_infoSession = false, CheckUser_infoSession = false)]
        public EmptyResult LogOut() {
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

  
         
    }
}

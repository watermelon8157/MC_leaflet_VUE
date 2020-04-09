using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using RCSData.Models;


namespace RCS.Areas.Backend.Controllers
{
    public class BaseController : Controller
    {
        /// <summary> 存放Session使用者基本資料 </summary>
        public  UserInfo user_info { get; set; }

        bool _IsAjaxRequest { get; set; }
        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {

            //取得使用者資料
            if (Session["backend_user_info"] != null)
                this.user_info = (UserInfo)Session["backend_user_info"];

            _IsAjaxRequest = filterContext.HttpContext.Request.IsAjaxRequest();//是否是 Ajax
            string act_name = RouteData.Values["Action"].ToString();//取得 Action 名稱
            string control_name = RouteData.Values["Controller"].ToString();//取的 Controller 名稱
            string HttpMethod = filterContext.HttpContext.Request.HttpMethod;

            RCS.Controllers.SilencerAttribute attr = new RCS.Controllers.SilencerAttribute();
            #region 檢查是否有設定 Attribute
            List<FilterAttribute> filters = new List<FilterAttribute>();
            filters.AddRange(filterContext.ActionDescriptor.GetFilterAttributes(false));
            filters.AddRange(filterContext.ActionDescriptor.ControllerDescriptor.GetFilterAttributes(false));
            if (filters.Count > 0)
            {
                attr = filters.OfType<RCS.Controllers.SilencerAttribute>().First();
            }
            #endregion


            if (attr.CheckPat_infoSession && attr.CheckUser_infoSession)
            {
                if (this.user_info == null || string.IsNullOrEmpty(this.user_info.user_id))
                {
                    TempData["message"] = "查無使用者身分或登入逾時，請重新登入!";
                    filterContext.Result = new RedirectResult("~/Backend/Admin/Index");//如果沒有使用者資料，回登入頁面
                }
            }
            base.OnActionExecuting(filterContext);
        }

    }
}

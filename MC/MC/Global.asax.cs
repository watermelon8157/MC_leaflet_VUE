using Com.Mayaminer;
using RCS_Data;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Routing;
using System.Reflection;
using RCS.Models;
using RCSData.Models;
using RCS_Data.Models.DB;

namespace RCS
{

    // 注意: 如需啟用 IIS6 或 IIS7 傳統模式的說明，
    // 請造訪 http://go.microsoft.com/?LinkId=9394801
    public class MvcApplication : System.Web.HttpApplication
    {

        static string csName { get { return "MvcApplication"; } }
        /// <summary> 更新病人清單的timer </summary>
        private static Timer timer_get_ptlist;

        /// <summary> 上傳簽章資料 </summary>
        private static Timer timer_upload;
         

        private static bool first_time { get; set; }
          

        /// <summary> 醫院資料 </summary>
        public static List<DB_MC_HOSP_INFO> hospList { get; set; }

        /// <summary>
        /// 從醫院取得最新的病患清單比對
        /// </summary>
        /// <param name="state"></param>
        private static void MC_Source(object state)
        {
            string actionName = "MC_Source";

            try
            {
                if (first_time)
                {
                    if (MvcApplication.hospList == null)
                    { 
                        MvcApplication.hospList.Clear();
                    }
                    else
                    {
                        MvcApplication.hospList = new List<DB_MC_HOSP_INFO>();
                    }
                    first_time = false;
                    new RCS.Models.MCSource().RunThread();
                }
                else
                {
                    //對應院內資料
                    new RCS.Models.MCSource().RunThread();
                }


            }
            catch (Exception ex)
            {
                LogTool.SaveLogMessage(ex, actionName, MvcApplication.csName);
            }

        }

        protected void Application_Start()
        {

            AreaRegistration.RegisterAllAreas();// 註冊所有區域的Route 
            GlobalConfiguration.Configure(WebApiConfig.Register);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);

            MvcApplication.hospList = new List<DB_MC_HOSP_INFO>();
            MvcApplication.first_time = true;
            //設定病人資料同步的 Schedule 開啟後五秒執行起來，每十分鐘跑一次
            TimerCallback tc = new TimerCallback(MC_Source);
            timer_get_ptlist = new Timer(tc, null, 5000, 10 * 60 * 1000);

            string log4netPath = Server.MapPath("~/App_Config/log4net.config");
            log4net.Config.XmlConfigurator.ConfigureAndWatch(new System.IO.FileInfo(log4netPath));

        }

        /// <summary>
        /// IIS 停止時會記錄停止原因
        /// </summary>
        protected void Application_End()
        { 
            
            if (MvcApplication.hospList != null)
            {
                MvcApplication.hospList.Clear();
            } 
            try
            {
                HttpRuntime runtime = (HttpRuntime)typeof(System.Web.HttpRuntime).InvokeMember("_theRuntime", BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.GetField, null, null, null);
                if (runtime == null)
                {
                    return;
                }
                string shutDownMessage = (string)runtime.GetType().InvokeMember("_shutDownMessage", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.GetField, null, runtime, null);

                // 寫入log
                LogTool.SaveLogMessage(shutDownMessage, "Application_End");
            }
            catch (Exception ex)
            {
                LogTool.SaveLogMessage(ex, "Application_End");
            }
        }

        protected void Application_Error(object sender, EventArgs e)
        {
            Exception exception = Server.GetLastError();

            RequestContext rc = ((MvcHandler)HttpContext.Current.CurrentHandler).RequestContext;
            bool _IsAjaxRequest = new HttpRequestWrapper(HttpContext.Current.Request).IsAjaxRequest();//是否是 Ajax
            string act_name = rc.RouteData.Values["Action"].ToString();//取得 Action 名稱
            string control_name = rc.RouteData.Values["Controller"].ToString();//取的 Controller 名稱
            Response.Clear();


            LogTool.SaveLogMessage(exception, "Application_End");
            HttpException httpException = exception as HttpException;

            if (httpException != null)
            {
                string action;

                switch (httpException.GetHttpCode())
                {
                    case 404:
                        // page not found
                        action = "StatusCode404";
                        break;
                    case 500:
                        // server error
                        action = "StatusCode500";
                        break;
                    default:
                        action = "General";
                        break;
                }

                // clear error on server
                Server.ClearError();

                Response.Redirect(String.Format("~/Error/{0}/?message={1}", action, exception.Message));
            }
        }
    }
}
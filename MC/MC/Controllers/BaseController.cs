using RCS_Data;
using System.Web.Mvc;
using Com.Mayaminer;
using System;
using RCSData.Models;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using RCS.Models;
using System.Web.Routing;
using mayaminer.com.jxDB;
using log4net;


namespace RCS.Controllers
{

    public class BaseController : Controller
    {
        string csName { get { return "BaseController"; } }
        public static string basic_model { get { return IniFile.GetConfig("System", "HOSP_ID"); } }

        private Models.HOSP.HospFactory _hospFactory { get; set; }
        protected Models.HOSP.HospFactory hospFactory
        {
            get
            {
                if (this._hospFactory == null)
                {
                    this._hospFactory = new Models.HOSP.HospFactory();
                }
                return this._hospFactory;
            }
        }

        #region switch

        #region isBasicMode
        private static string _isBasicMode { get; set; }
        /// <summary>
        /// 是否是馬雅標準版模式開關(true:開啟,false:關閉)
        /// </summary>
        public static bool isBasicMode
        {
            get
            {
#if DEBUG
                return false;
#endif
                if (!string.IsNullOrWhiteSpace(_isBasicMode))
                {
                    return bool.Parse(_isBasicMode);
                }
                _isBasicMode = IniFile.GetConfig("System", "isBasicMode");
                return bool.Parse(_isBasicMode);
            }
        }
        #endregion
        #region isDebuggerMode
        private static string _isDebuggerMode { get; set; }

        /// <summary>是否debug模式(true:顯示完整功能以及開發功能,false:顯示基本版功能)
        /// <para>開發中功能記得加入tag避免，發佈時顯示尚未完成的功能</para>
        /// </summary>
        public static bool isDebuggerMode
        {
            get
            {
#if DEBUG
                return true;
#endif
                if (!string.IsNullOrWhiteSpace(_isDebuggerMode))
                {
                    return bool.Parse(_isDebuggerMode);
                }
                _isDebuggerMode = IniFile.GetConfig("System", "isDebuggerMode");
                return bool.Parse(_isDebuggerMode);
            }
        }
        #endregion

        private static string _isTestWeb { get; set; }
        /// <summary>
        /// 是否是測試網站
        /// </summary>
        public static bool isTestWeb
        {
            get
            {
                if (!string.IsNullOrWhiteSpace(_isTestWeb))
                {
                    return bool.Parse(_isTestWeb);
                }
                _isTestWeb = IniFile.GetConfig("System", "isTestWeb");
                return bool.Parse(_isTestWeb);
            }
        }

        #endregion

        public BaseController()
        { 
            funSetting = new FunctionSetting();
            logger = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        }
        /// <summary>
        /// 記錄訊息
        /// </summary>
        public ILog logger { get; set; }
        /// <summary>設定醫院名稱</summary>
        public static string hospName { get { return IniFile.GetConfig("System", "setHospName"); } }
        /// <summary>院區 總院=K 員基=B 雲基=T 馬雅=maya</summary>
        public static string zon { get { return "K"; } } 
        /// <summary>
        /// 功能參數設定
        /// </summary>
        public static FunctionSetting funSetting { get; set; }
       

        #region 參數設定
        /// <summary>上傳空值設定預設值</summary>
        public static string upLoadNullStr { get { return " "; } }

        BaseModel _BaseModel;
        /// <summary>取得基本設定及資料</summary>
        protected BaseModel BaseModel
        {
            get
            {
                if (_BaseModel == null)
                {
                    _BaseModel = new BaseModel();
                }
                return _BaseModel;
            }
        }

        jxDBA _DBA;
        protected jxDBA DBA
        {
            get
            {
                if (_DBA == null)
                {
                    _DBA = new jxDBA();
                    _DBA.ConnectionString = IniFile.GetConfig("Connection", "DBAConnStr");
                    //_DBA.ConnectionString = System.Configuration.ConfigurationManager.ConnectionStrings["DBconnection"].ConnectionString.ToString();
                }
                return _DBA;
            }
        }

        /// <summary> 存放Session使用者基本資料 </summary>
        public UserInfo user_info { get; set; }

        /// <summary> 病人基本資料 </summary>
        public IPDPatientInfo pat_info { get; set; }

        #endregion
         
        bool _IsAjaxRequest = false;
        /// <summary> 執行前先讀取資料 </summary>
        /// <param name="filterContext"></param>
        protected override void OnActionExecuting(ActionExecutingContext filterContext) {

            //取得使用者資料
            if (Session["user_info"] != null)
                this.user_info = (UserInfo)Session["user_info"];
            if (Session["pat_info"] != null)
                this.pat_info = (IPDPatientInfo)Session["pat_info"];

            _IsAjaxRequest = filterContext.HttpContext.Request.IsAjaxRequest();//是否是 Ajax
            string act_name = RouteData.Values["Action"].ToString();//取得 Action 名稱
            string control_name = RouteData.Values["Controller"].ToString();//取的 Controller 名稱
            string HttpMethod = filterContext.HttpContext.Request.HttpMethod;

            SilencerAttribute attr = new SilencerAttribute();
            #region 檢查是否有設定 Attribute
            List<FilterAttribute> filters = new List<FilterAttribute>();
            filters.AddRange(filterContext.ActionDescriptor.GetFilterAttributes(false));
            filters.AddRange(filterContext.ActionDescriptor.ControllerDescriptor.GetFilterAttributes(false));
            if (filters.Count > 0)
            {
                attr = filters.OfType<SilencerAttribute>().First();
            }
            #endregion


            if(attr.CheckUser_infoSession)
            {
                if(this.user_info == null || string.IsNullOrEmpty(this.user_info.user_id))
                {
                    if (TempData["message"] == null || string.IsNullOrWhiteSpace(TempData["message"].ToString()))
                        TempData["message"] = "查無使用者身分或登入逾時，請重新登入!";
                    else
                        TempData["message"] = TempData["message"].ToString();
                    filterContext.Result = new RedirectResult("~/Main/Index");//如果沒有使用者資料，回登入頁面
                }
                else
                {
                    LogUserUseRecord logUserUseRecord = new LogUserUseRecord()
                    {
                        user_id = user_info.user_id,
                        use_action = act_name,
                        use_controller = control_name,
                        use_time = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")
                    };
                    Session["userUse"] = logUserUseRecord;
                    logger.Info(JsonConvert.SerializeObject(logUserUseRecord));
                }

            }
 
            base.OnActionExecuting(filterContext);
        }

        protected object returnObj(RESPONSE_MSG pRm)
        {
            if (pRm.status != RESPONSE_STATUS.SUCCESS)
            {
                LogTool.SaveLogMessage(pRm.lastError, "returnObj", this.csName);
            }
            return pRm.attachment;

        }
    }

    /// <summary>
    /// actionName監聽
    /// <para>確認是否檢查Session</para>
    /// </summary>
    public class SilencerAttribute : ActionFilterAttribute
    {
        public SilencerAttribute()
        {
            //預設要檢查 Session
            CheckUser_infoSession = true;
            CheckPat_infoSession = true;
        }
        /// <summary>
        /// 是否檢查使用者Session
        /// </summary>
        public bool CheckUser_infoSession { get; set; }
        /// <summary>
        /// 是否檢查病患Session
        /// </summary>
        public bool CheckPat_infoSession { get; set; }
    }
}
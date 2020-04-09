using RCS_Data;
using System.Web.Mvc;
using Com.Mayaminer;
using System.Data.Common;
using System;
using RCSData.Models;
using System.Collections.Generic;
using System.Linq;
using System.Data;
using Newtonsoft.Json;
using System.IO;
using System.Globalization;
using System.Web.Services.Protocols;
using System.Net;
using RCS.Models;
using Newtonsoft.Json.Linq;
using System.Xml.Linq;
using System.Web.Routing;
using mayaminer.com.jxDB;
using mayaminer.com.library;
using System.Web.Configuration;
using System.Xml;
using log4net;
using System.Reflection;
using RCS_Data.Controllers.RT;
using RCS_Data.Models.ViewModels;

namespace RCS.Controllers {

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


        #region Session設定

        /// <summary>
        /// 檢查使用者行為，判斷是否登出
        /// </summary>
        /// <returns></returns>
        [Silencer(CheckPat_infoSession = false, CheckUser_infoSession = false)]
        public JsonResult checkTimeout()
        {
            RESPONSE_MSG rm = new RESPONSE_MSG();
            string actionName = "checkTimeout";
            try
            {
                LogUserUseRecord Temp  = (LogUserUseRecord)Session["userUse"];
                if (Session != null && Temp != null)
                    rm.attachment = Temp;
            }
            catch (Exception ex)
            {
                rm.status = RESPONSE_STATUS.ERROR;
                rm.message = ex.Message;
                LogTool.SaveLogMessage(ex, actionName, this.csName);
            }
            return Json(rm);
        }

        /// <summary>變更Session <para> 2016.4.20 JoeShen</para></summary>
        /// <param name="ipd_no">住院序號</param>
        /// <param name="user_id">使用者帳號</param>
        /// <returns></returns>
        [Silencer(CheckPat_infoSession = false, CheckUser_infoSession = false)]
        [HttpPost]
        public JsonResult ChangeSession(string chart_no, string ipd_no, string user_id, UserInfo pUser_info)
        {
            RESPONSE_MSG rm = new RESPONSE_MSG();
            string actionName = "ChangeSession";
            try
            {
                IPDPatientInfo tmp_ipd_info = pat_info;
                UserInfo tmp_user_info = user_info;
                if (tmp_user_info != null && !string.IsNullOrWhiteSpace(tmp_user_info.user_id)  && tmp_user_info.user_id == user_id)
                {
                    if (!string.IsNullOrWhiteSpace(chart_no) && !string.IsNullOrWhiteSpace(ipd_no))
                    {

                        if (tmp_ipd_info != null && tmp_ipd_info.chart_no != null && tmp_ipd_info.ipd_no != null && pat_info.chart_no == chart_no && pat_info.ipd_no == ipd_no)
                        {
                            Session["pat_info"] = pat_info;
                            rm.status = RESPONSE_STATUS.SUCCESS;
                            rm.message = "session update to" + ipd_no;
                        }
                        else
                        {
                            rm.status = RESPONSE_STATUS.ERROR;
                            LogTool.SaveLogMessage(string.Format("帶錯病患資料，原病患({0})，變成({1}))，資料有問題，自動登出!", tmp_ipd_info.chart_no, chart_no), actionName, GetLogToolCS.BaseController);
                            rm.message = "病患資料有誤，請重新登入!";
                            TempData["message"] = "病患資料有誤，請重新登入!";
                        }
                    }
                    //Session["user_info"] = pUser_info;
                }
                else
                {
                    rm.status = RESPONSE_STATUS.ERROR;
                    LogTool.SaveLogMessage(string.Format("使用者重複登入，原使用者({0})，變成({1}))，資料有問題，自動登出!", tmp_user_info.user_id, user_id), actionName, GetLogToolCS.BaseController);
                    rm.message = "使用者重複登入，請重新登入!";
                    TempData["message"] = "使用者重複登入，請重新登入!";
                }

               
            }
            catch (Exception ex)
            {
                rm.status = RESPONSE_STATUS.EXCEPTION;
                rm.message = "程式發生錯誤，請洽資訊人員!";
                LogTool.SaveLogMessage(ex, actionName, GetLogToolCS.BaseController);
                TempData["message"] = "程式發生錯誤，請重新登入!";
            }
            return Json(rm);
        }

        /// <summary>
        /// 檢查Session是否存在
        /// </summary>
        /// <returns>String True or False</returns>
        [Silencer(CheckPat_infoSession = false, CheckUser_infoSession = false)]
        public ActionResult CheckSessionExist(string session_name)
        {
            bool exist_v = false;
            session_name = string.IsNullOrEmpty(session_name) ? "user_info" : session_name;
            if (Session[session_name] != null)
            {
                exist_v = true;
            }
            else
            {
                TempData["message"] = "登入逾時或重複登入，請重新登入!";
            }
            return Content(exist_v.ToString());
        }

        /// <summary>
        /// 寫入Session
        /// </summary>
        /// <param name="pChart_no"></param>
        /// <param name="typeMode"></param>
        /// <param name="pipd_no"></param>
        /// <param name="diag_date"></param>
        /// <returns>data_set_keys</returns>
        [Silencer(CheckPat_infoSession = false, CheckUser_infoSession = false)]
        public JsonResult SetSession(string pChart_no, string typeMode, string pipd_no = "", string diag_date ="")
        {
            IPDPatientInfo pat_info = new IPDPatientInfo();
            try
            {
                //pat_info = JsonConvert.DeserializeObject<RCS_Data.IPDPatientInfo>(json_data_set);
                RT model = new RT();
                List<PatientListItem> newPat_info = model.get_CareIList(user_info.user_id, typeMode, pChart_no, diag_date);
                if (newPat_info != null && newPat_info.Count > 0)
                {
                    if (!string.IsNullOrWhiteSpace(pipd_no))
                    {
                        if(newPat_info.Exists(x=>x.ipd_no == pipd_no))
                            pat_info = newPat_info.Find(x => x.ipd_no == pipd_no);
                        else
                            pat_info = newPat_info[0];
                    }
                    else
                        pat_info = newPat_info[0];
                    // pat_info = BaseModel.updateCaseData(this.hospFactory.webService.HISPatientInfo(), pat_info);
                    List<string> stringList = BaseModel.getCPTAssess(pChart_no);
                    if (!string.IsNullOrWhiteSpace(stringList[4]))
                    {
                        pat_info.diagnosis_code = BaseModel.getCPTAssess(pChart_no)[4];
                    }
                 
                    Session["pat_info"] = pat_info;
                }
                else
                {
                    //Session["pat_info"] = pat_info;
                    LogTool.SaveLogMessage("取得最新病患資料失敗!", "SetSession", GetLogToolCS.BaseController);
                }
            }
            catch (Exception ex)
            {
                LogTool.SaveLogMessage(ex.Message, "SetSession", GetLogToolCS.BaseController);
            }
            return Json(pat_info);
        }

        /// <summary>
        /// 檢查病患Session
        /// </summary>
        /// <param name="pChart_no"></param>
        /// <param name="pIpd_no"></param>
        /// <param name="typeMode"></param>
        /// <returns></returns>
        [Silencer(CheckPat_infoSession = false, CheckUser_infoSession = false)]
        public JsonResult checkPatSession(string pChart_no,string pIpd_no, string typeMode)
        {
            RESPONSE_MSG rm = new RESPONSE_MSG();
            string actionName = "checkPatSession";
            try
            {
                if(!string.IsNullOrWhiteSpace(pChart_no) && !string.IsNullOrWhiteSpace(typeMode))
                {
                    IPDPatientInfo tempPat_info = pat_info;
                    if (!string.IsNullOrWhiteSpace(pChart_no) && pat_info.chart_no != pChart_no)
                        tempPat_info = (IPDPatientInfo)SetSession(pChart_no, typeMode).Data;
                    if (tempPat_info != null && tempPat_info.chart_no == pChart_no)
                        rm.status = RESPONSE_STATUS.SUCCESS;
                    else
                    {
                        rm.status = RESPONSE_STATUS.ERROR;
                        rm.message = "檢查病患資料時，程式發生錯誤，請洽資訊人員!";
                        LogTool.SaveLogMessage(rm.message, actionName, GetLogToolCS.BaseController);
                    }
                }
                else
                {
                    Session.Remove("pat_info");
                    rm.status = RESPONSE_STATUS.SUCCESS;
                }
            }
            catch (Exception ex)
            {
                rm.status = RESPONSE_STATUS.EXCEPTION;
                rm.message = "程式發生錯誤，請洽資訊人員!錯誤訊息如下:" + ex.Message;
                LogTool.SaveLogMessage(ex, actionName, GetLogToolCS.BaseController);
            }
            return Json(rm);
        }

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
using Com.Mayaminer;
using Dapper;
using RCSData.Models;
using Newtonsoft.Json;
using RCS_Data;
using System.Linq;
using System;
using System.Collections.Generic;

namespace RCSData.Models
{
    public partial class WebMethod
    {
        /// <summary>登入者驗證</summary>
        /// <param name="pLoginName">登入者代碼，必填</param>
        /// <param name="pPassword">登入者密碼，必填</param>
        /// <param name="UserInfo">使用者資料</param>
        /// <param name="getAdmin">管理者權限</param>
        /// <returns></returns>
        public UserInfo checkLoginUser(IWebServiceParam iwp, string pLoginName, string pPassword, bool getAdmin = false)
        {
            UserInfo ui = null;
            ServiceResult<UserInfo> sr = new ServiceResult<UserInfo>();
            List<UserInfo> dataList = new List<UserInfo>();
            string actionName = "checkLoginUser";
            string sqlStr = "";
            DynamicParameters dp = null;
            try
            {
                #region 取得使用者資料
                //RCS_SYS_USER
                SQLProvider SQLProvider = new SQLProvider();
                ui = new UserInfo();
                bool sysLogin = bool.Parse(IniFile.GetConfig("System", "sysLogin"));
                if (sysLogin)
                {
                    #region 取得系統設定使用者資料
                    sqlStr = string.Concat("SELECT * FROM RCS_SYS_USER_LIST WHERE USER_ID = ", SQLProvider.namedArguments, "USER_ID");
                    if (getAdmin)
                    {
                        sqlStr += " AND USER_ROLE = 'admin'";
                    }
                    dp = new DynamicParameters(new { USER_ID = pLoginName });
                    List<RCS_SYS_USER_LIST> userList = SQLProvider.DBA.getSqlDataTable<RCS_SYS_USER_LIST>(sqlStr, dp);
                    //密碼加密
                    System.Security.Cryptography.SHA512 sha512 = new System.Security.Cryptography.SHA512CryptoServiceProvider();
                    string resultSha512 = Convert.ToBase64String(sha512.ComputeHash(System.Text.Encoding.Default.GetBytes(pPassword)));
                    //檢查資料
                    #region 檢查資料
                    if (userList.Count > 0)
                    {
                        ui.has_RCS_SYS_USER_LIST_UserInfo = true;
                        //檢察密碼是否正確
                        if (userList[0].USER_PWD == resultSha512)
                        {
                            SQLProvider.DBA.RESPONSE_MSG = userList[0].checkUserInfo();
                        }
                        else
                        {
                            SQLProvider.DBA.RESPONSE_MSG.status = RESPONSE_STATUS.ERROR;
                            SQLProvider.DBA.RESPONSE_MSG.message = "帳號或密碼錯誤，請重新輸入!";
                        }
                        //如果檢察通過，取得資料
                        if (SQLProvider.DBA.RESPONSE_MSG.status == RESPONSE_STATUS.SUCCESS)
                        {
                            ui.user_id = userList[0].USER_ID;
                            if (ui.user_id.Contains("_"))
                            {
                                ui.user_id = ui.user_id.Split('_')[1];
                                pLoginName = ui.user_id;
                            }

                            ui.user_name = userList[0].HOSP_NAME;
                            ui.authority = userList[0].USER_ROLE;
                            ui.sysAuthority = userList[0].USER_ROLE;
                            ui.user_idno = "A123456789";
                            ui.user_costcode = "maya";
                            SQLProvider.DBA.RESPONSE_MSG.status = RESPONSE_STATUS.SUCCESS;
                            ui.loginMsg = SQLProvider.DBA.RESPONSE_MSG.message;
                            SQLProvider.DBA.RESPONSE_MSG.message = "";
                        }
                    }
                    #endregion

                    #endregion
                }

                if (ui.has_RCS_SYS_USER_LIST_UserInfo)
                {
                    #region 是系統使用者資料
                    if (SQLProvider.RESPONSE_MSG.status == RESPONSE_STATUS.SUCCESS)
                    {
                        sr.datastatus = HISDataStatus.SuccessWithData;
                        dataList.Add(ui);
                        ui.hasUserData = true;
                    }
                    else
                    {
                        sr.datastatus = HISDataStatus.SuccessWithoutData;
                        sr.errorMsg = SQLProvider.RESPONSE_MSG.message; 
                    }
                    #endregion
                }
                else
                {
                    //如果是後台管理系統管理者，部取得WS資料
                    if (!getAdmin)
                    {
                        //WebService
                        #region 不是系統使用者資料，向WS取得資料 
                        WS_checkLoginUser cu = new WS_checkLoginUser(pLoginName, pPassword, iwp);
                        sr = HISData.getServiceResult(cu);
                        if (sr.datastatus == HISDataStatus.SuccessWithData)
                        {
                            dataList = JsonConvert.DeserializeObject<List<UserInfo>>(sr.returnJson);
                        }
                        if (sr.datastatus == HISDataStatus.SuccessWithData)
                        {
                            ui = dataList[0];
                            ui.hasUserData = true;
                        }
                        #endregion
                    }
                    else
                    { 
                        LogTool.SaveLogMessage(  string.Concat("此帳號(", pLoginName, ")非管理者權限，無法登入後台管理系統，請洽資訊人員!"), actionName, GetLogToolCS.WebMethod);
                    }
                }
                #endregion
                #region 檢查登入資料
                if (!ui.hasUserData)
                {
                    //沒有資料 
                    LogTool.SaveLogMessage("使用者(" + pLoginName + ")登入失敗。" + pLoginName, actionName, GetLogToolCS.WebMethod);
                }
                else
                {
                    #region 取得使用者系統權限
                    string uiAuthority = "";
                    sqlStr = string.Concat("select P_GROUP,P_NAME from RCS_SYS_PARAMS where P_MODEL = 'user' and P_VALUE= ", SQLProvider.namedArguments, "USER_ID AND P_STATUS='1'");
                    dp = new DynamicParameters(new { USER_ID = pLoginName });
                    List<RCS_SYS_PARAMS> tempList = SQLProvider.DBA.getSqlDataTable<RCS_SYS_PARAMS>(sqlStr, dp);

                    if (tempList.Count > 0)
                    {
                        uiAuthority = tempList[0].p_group;
                        ui.user_name = tempList[0].p_name;
                    }
                    else
                    {
                        ui.hasUserData = false;
                        if (SQLProvider.DBA.hasLastError)
                        {
                            SQLProvider.DBA.RESPONSE_MSG.message = string.Concat("取得使用者權限資料錯誤，錯誤訊息如下所示: ", SQLProvider.DBA.lastError);
                            SQLProvider.DBA.RESPONSE_MSG.status = RESPONSE_STATUS.EXCEPTION;
                            LogTool.SaveLogMessage(SQLProvider.DBA.lastError, actionName, GetLogToolCS.SQLProvider);
                        }
                    }

                    #endregion
                    if (string.IsNullOrWhiteSpace(uiAuthority))
                    {
                        ui.hasUserData = false; 
                    }
                    else
                    {
                        //檢查權限
                        ui.sysAuthority = uiAuthority;
                        if (string.IsNullOrWhiteSpace(ui.sysAuthority))
                        {
                            //沒有權限
                            this.datastatus = HISDataStatus.WebServiceError;
                            this.errorMsg = "使用者(" + pLoginName + ")登入失敗，查無系統使用權限，請管理者新增。";
                            LogTool.SaveLogMessage(this.errorMsg, actionName, GetLogToolCS.WebMethod);
                        }
                        else
                        {
                            //有權限
                            #region
                            if (ui.hasUserData)
                            {
                                //登入成功
                                #region 
                                ui.hasUserData = true;
                                LogTool.SaveLogMessage("使用者(" + pLoginName + ")登入成功", actionName, GetLogToolCS.WebMethod);
                                this.datastatus = HISDataStatus.SuccessWithData;
                                if (string.IsNullOrWhiteSpace(ui.loginMsg))
                                {
                                    this.errorMsg = ui.loginMsg;
                                }
                                #endregion
                            }
                            else
                            {
                                //登入失敗
                                this.datastatus = HISDataStatus.WebServiceError;
                                this.errorMsg = "使用者(" + pLoginName + ")登入失敗。" + this.errorMsg;
                                LogTool.SaveLogMessage(this.errorMsg, actionName, GetLogToolCS.WebMethod);
                                LogTool.SaveLogMessage(this.errorMsg, actionName, GetLogToolCS.WebMethod);
                            }
                            #endregion
                        }
                    }

                }
                #endregion

            }
            catch (Exception ex)
            { 
                LogTool.SaveLogMessage(ex, actionName, GetLogToolCS.WebMethod);
            }
            return ui;
        }
    }

    /// <summary>
    /// 登入者驗證，取得用者資料
    /// </summary>
    public class WS_checkLoginUser : AwebMethod< UserInfo>, IwebMethod< UserInfo>
    {

        public string webMethodName { get { return this.iwp.webMethodName; } }

        public override string wsSession { get { return "RCS_WS_BASIC"; } }
        /// <summary>
        /// 登入者驗證，取得用者資料
        /// </summary>
        /// <param name="pLoginName">使用者代碼</param>
        /// <param name="pPassword">使用者密碼</param>
        /// <param name="wsSession"></param>
        public WS_checkLoginUser(string pLoginName, string pPassword, IWebServiceParam pIwp )
        {
            this.iwp = pIwp;
            setParam();
            if (pLoginName != null && pPassword != null && pLoginName.Length > 0 && pPassword.Length > 0)
            {
                #region 整理傳入參數
                this.paramList["user_id"].paramValue = pLoginName;
                this.paramList["user_pwd"].paramValue = pPassword;
                #endregion
            }
            else
            {
                this.ServiceResult.datastatus = RCS_Data.HISDataStatus.ParametersError;
                if (pLoginName == null || pLoginName.Length == 0)
                    this.ServiceResult.errorMsg = "登入者代碼不可為空值!";
                if (pPassword == null || pPassword.Length == 0)
                    this.ServiceResult.errorMsg = "登入者密碼不可為空值!";
                if ((pLoginName == null || pLoginName.Length == 0) && (pPassword == null || pPassword.Length == 0))
                    this.ServiceResult.errorMsg = "登入者代碼及密碼不可為空值!";
            }
        }


        public void setParam()
        {
            this.ServiceResult = new ServiceResult< UserInfo>();
            this.paramList = this.iwp.paramList;
            this.returnValue = this.iwp.returnValue;
        }

        public override void setReturnValue()
        {
            base.setReturnValue();  
            //如果沒有資料
            if (this.ServiceResult.datastatus == RCS_Data.HISDataStatus.SuccessWithoutData)
            {
                this.ServiceResult.returnList = new System.Data.DataTable();
                this.ServiceResult.errorMsg = string.Format("使用者{0}查無使用權限({1})，請洽資訊室!<br />WS網址:{2}",
                    this.paramList["user_id"].paramValue, this.webMethodName, Com.Mayaminer.IniFile.GetConfig(this.wsSession, "WebServiceUrl"));

            }
        }
    }


    /// <summary>
    /// 使用者資料
    /// </summary>
    public class UserInfo : UserInfoBasic
    {
        private string _user_name { get; set; }
        /// <summary>
        /// 是否是外部網址登入
        /// </summary>
        public bool isGuide { get; set; }
        public bool hasUserData { get; set; }
        /// <summary>
        /// 登入訊息
        /// </summary>
        public string loginMsg { get; set; }
        /// <summary>
        /// 可以看尚未上傳的資料
        /// </summary>
        public bool canCheckNotUpLoadData
        {
            get
            {
                try
                {
                    List<string> canCheckNotUpLoadAuthority = IniFile.GetConfig("System", "canCheckNotUpLoadUser").Split(',').ToList();
                    if (canCheckNotUpLoadAuthority.Contains(sysAuthority))
                    {
                        return true;
                    }
                }
                catch
                {

                    throw;
                }
                return false;
            }
        }
        /// <summary>
        /// 是否有RCS_SYS_USER_LIST使用者資料
        /// </summary>
        public bool has_RCS_SYS_USER_LIST_UserInfo { get; set; }

        /// <summary>
        /// 權限設定
        /// </summary>
        public Authority hasAuthority { get { return new Authority(sysAuthority); } }
        /// <summary>
        /// 使用者功能清單
        /// </summary>
        public Dictionary<string, UserFunctionAction> functionMenu { get; set; }

        public override string hosp_id
        {
            get
            {
                return IniFile.GetConfig("System", "HOSP_ID");
            }
        }

        #region 方法

        /// <summary>
        /// 取得使用者資料
        /// <param name="pUiser_id">使用者帳號</param>
        /// <param name="pUser_pwd">使用者密碼</param>
        /// <param name="getAdmin">管理者權限</param>
        /// </summary>
        /// <returns></returns>
        public UserInfo getUserInfo(IWebServiceParam iwp, string pUiser_id, string pUser_pwd, bool getAdmin = false)
        {
            UserInfo ui = this;
            string actionName = "getUserInfo";
            WebMethod web_method = new WebMethod();
            try
            {
                //傳入值
                this.user_id = pUiser_id;
                this.user_pwd = pUser_pwd;

                //工作
                //如果沒有預設系統登入者，用HIS登入
                #region 如果沒有預設系統登入者，用HIS登入
                ui = web_method.checkLoginUser(iwp, this.user_id, this.user_pwd, getAdmin);
                #endregion
                //結果
                if (web_method.datastatus != HISDataStatus.SuccessWithData)
                {
                    ui.hasUserData = false;
                    ui.loginMsg = web_method.errorMsg;
                } 
            }
            catch (Exception ex)
            {
                ui.hasUserData = false;
                ui.loginMsg = ex.Message.ToString();
                LogTool.SaveLogMessage(web_method.errorMsg, actionName, GetLogToolCS.System);
            }
            return ui;
        }
         
 
        #endregion
 
    }

    /// <summary>
    /// 使用者工能動作
    /// </summary>
    public class UserFunctionAction
    {
        public UserFunctionAction()
        {

        }
        /// <summary>
        /// 設定
        /// </summary>
        /// <param name="pList"></param>
        public UserFunctionAction(List<string> pList, string pFUNCTION_LOCATION)
        {
            this.FUNCTION_LOCATION = pFUNCTION_LOCATION;
            this.FunctionAction = pList;
            if (pList.Contains("瀏覽"))
            {
                this.canRead = true;
            }
            if (pList.Contains("寫"))
            {
                this.canWrite = true;
            }
            if (pList.Contains("改"))
            {
                this.canEdit = true;
            }
            if (pList.Contains("刪"))
            {
                this.canDelete = true;
            }
            if (pList.Contains("停"))
            {
                this.canStopFunction = true;
            }
        }
        /// <summary>
        /// 瀏覽
        /// </summary>
        public bool canRead { get; set; }

        /// <summary>
        /// 新增(寫)
        /// </summary>
        public bool canWrite { get; set; }

        /// <summary>
        /// 編輯
        /// </summary>
        public bool canEdit { get; set; }

        /// <summary>
        /// 停用功能
        /// </summary>
        public bool canStopFunction { get; set; }

        /// <summary>
        /// 刪除
        /// </summary>
        public bool canDelete { get; set; }

        /// <summary>
        /// 可以編輯或新增(寫)
        /// </summary>
        public bool canEditOrWrite { get { if (canWrite || canEdit) { return true; } else { return false; } } }
        /// <summary>
        /// 功能清單
        /// <para>new List<string>() { "瀏覽", "寫", "改", "刪", "停" };</para>
        /// </summary>
        public List<string> FunctionAction { get; private set; }

        public string FUNCTION_LOCATION { get; private set; }
    }

    /// <summary>
    /// 功能使用權限
    /// </summary>
    public class Authority
    {
        public Authority(string pAuthority)
        {
            //是否有權限
            if (!string.IsNullOrWhiteSpace(pAuthority))
            {
                switch (pAuthority)
                {
                    case "RT_admin"://呼吸治療小組長
                    case "admin"://系統管理者
                        canSaveScheduling = true;
                        canUseUpLoadData = true;
                        canUsePatlist = true;
                        canCheckNotUpLoadData = true;
                        canUseSetIndexDevice = true;
                        canUseAssignment = true;
                        canUseMeasuresForm = true;
                        canUseSystemManage = true;
                        canUseCenterMonitor = true;
                        break;
                    case "doctor"://主治醫生
                    case "RT"://呼吸治療師
                        canUseUpLoadData = true;
                        canUsePatlist = true;
                        canCheckNotUpLoadData = true;
                        canUseAssignment = true;
                        canUseMeasuresForm = true;
                        canUseSystemManage = true;
                        canUseCenterMonitor = true;
                        break;
                    case "medical"://相關醫療人員
                        canCheckNotUpLoadData = true;
                        canUsePatlist = true;
                        notFixOrNewRecord = true;
                        break;
                    case "inquirer"://病歷課保險組人員
                        notFixOrNewRecord = true;
                        canOnlyReadRecord = true;
                        break;
                    case "isGuide"://外部網站連結權限
                        canCheckNotUpLoadData = true;
                        notFixOrNewRecord = true;
                        canOnlyReadRecord = true;
                        break;
                    default:
                        break;
                }
            }
        }

        #region 功能使用權限變數

        /// <summary>
        /// 儲存排班表權限
        /// </summary>
        public bool canSaveScheduling { get; set; }
        /// <summary>
        /// 可以使用上傳清單
        /// </summary>
        public bool canUseUpLoadData { get; set; }
        /// <summary>
        /// 可以使用照護病患清單
        /// </summary>
        public bool canUsePatlist { get; set; }
        /// <summary>
        /// 可以看尚未上傳的資料
        /// </summary>
        public bool canCheckNotUpLoadData { get; set; }
        /// <summary>
        /// 可以使用呼吸維護記錄單新增/停用等功能
        /// </summary>
        public bool canUseSetIndexDevice { get; set; }
        /// <summary>
        /// 可以使用區域分派
        /// </summary>
        public bool canUseAssignment { get; set; }
        /// <summary>
        /// 可以使用輔具評估單
        /// </summary>
        public bool canUseMeasuresForm { get; set; }
        /// <summary>
        /// 可以使用系統管理
        /// </summary>
        public bool canUseSystemManage { get; set; }
        /// <summary>
        /// 是否可以修改或新增表單(true:不行:false:可以)
        /// </summary>
        public bool notFixOrNewRecord { get; set; }
        /// <summary>
        /// 是否可以修改或新增表單(true:不行:false:可以)
        /// </summary>
        public bool canOnlyReadRecord { get; set; }
        /// <summary>
        /// 可以使用中央監控站
        /// </summary>
        public bool canUseCenterMonitor { get; set; }

        #endregion
    }


}
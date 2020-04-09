using Com.Mayaminer;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RCS.Models;
using RCS_Data;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Mvc;
using RCSData.Models;
using System.Net.Http;
using System.Net;
using System.Web.Http;
using RCS_Data.Models.DB;

namespace RCS.Controllers
{
    /// <summary> 外部連結 </summary>
    public class ExternalController : Controller
    {
        string csName = "ExternalController";
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

        /// <summary>可否搜尋急診門診病患</summary>
        public static bool SearchErAndOpPatient = bool.Parse(IniFile.GetConfig("SystemConfig", "SearchErAndOpPatient"));
        /// <summary> 對外網站入口 </summary>
        /// 公司測試網址:http://localhost:1899/External/Guide?user=2F3D4DFE8DF39C0DE1C75029A7FC5F9A&feeno=I0332937&chrno=01008470
        /// http://demo.mayaminer.com.tw/RCS/External/Guide?user=0A7B2050F313828D7243A7155BD7B76C&feeno=I0333070&chrno=06851741&sDate=2014-01-01&eDate=2018-01-01
        /// <returns></returns>外部連結 
        public ActionResult Guide(string user, string feeno, string chrno, string sDate, string eDate)
        {
            string msg = "參數錯誤、權限不足";

            List<string> msgList = new List<string>();
            string actionName = "Guide";
            RESPONSE_MSG rm = new RESPONSE_MSG();
            try
            {
                //檢查查詢日期區間
                if (!string.IsNullOrWhiteSpace(sDate) && !string.IsNullOrWhiteSpace(eDate))
                {
                    if (!mayaminer.com.library.DateHelper.isDate(sDate, "yyyy-MM-dd")
                        || !mayaminer.com.library.DateHelper.isDate(eDate, "yyyy-MM-dd"))
                    {
                        msgList.Add("日期格式有誤，請洽資訊人員!");
                    }
                    else
                    {

                        Session["searchDate"] = string.Concat(sDate, "|", eDate);
                    }
                }
                else if (!string.IsNullOrWhiteSpace(sDate) || !string.IsNullOrWhiteSpace(eDate))
                {
                    msgList.Add("查詢起訖日期不完整，請洽資訊人員!");
                }
                if (msgList.Count > 0)
                    return View(actionName, "", msgList[0]);
                // parse user information
                // 傳入值
                string[] uinfo = SecurityTool.DecodeDES(Request["user"].ToString(), IniFile.GetConfig("ExternalLink", "key"), IniFile.GetConfig("ExternalLink", "iv")).Split('|');
                string user_id = "";
                string user_pwd = "";
                if (uinfo.Length > 0)
                {
                    user_id = uinfo[0];
                    if (uinfo.Length > 1)
                        user_pwd = uinfo[1];
                }
                if (!string.IsNullOrWhiteSpace(user_id) && !string.IsNullOrWhiteSpace(user_pwd))
                {
                    // create userinfo object for checkLoginUser
                    //工作
                    UserInfo ui = new UserInfo();
                    ui = ui.getUserInfo(this.hospFactory.webService.HisLoginUser(), user_id, user_pwd);

                    //取得結果
                    if (ui.hasUserData)
                    {
                        WebMethod WebMethod = new WebMethod();
                        if (!string.IsNullOrWhiteSpace(chrno) && !string.IsNullOrWhiteSpace(feeno))
                        {
                            SQLProvider SQL = new SQLProvider();
                            List<string> tempList = new List<string>();
                            string _query = "SELECT CHART_NO FROM RCS_RT_CASE WHERE CHART_NO = @CHART_NO AND IPD_NO = @IPD_NO";
                            Dapper.DynamicParameters dp = new Dapper.DynamicParameters();
                            dp.Add("CHART_NO", chrno);
                            dp.Add("IPD_NO", feeno);
                            tempList = SQL.DBA.getSqlDataTable<string>(_query, dp);
                            if (tempList.Count > 0)
                            {
                                List<IPDPatientInfo> pat_info = WebMethod.getPatientInfoList(this.hospFactory.webService.HISPatientInfo(), chrno, feeno);
                                if (pat_info != null && pat_info.Count > 0)
                                {
                                    ui.authority = "isGuide";
                                    ui.isGuide = true;
                                    Session["user_info"] = ui;
                                    Session["pat_info"] = pat_info[0];
                                    Session["ca_check"] = true;
                                    TempData["isGuide"] = true;
                                    return RedirectToAction("Index", "RT");
                                }
                                msg = "查無此病患(" + chrno + ")資料，請洽資訊室!";
                                LogTool.SaveLogMessage(WebMethod.errorMsg, actionName, GetLogToolCS.ExternalController);
                                return View(actionName, "", msg);
                            }
                            else
                            {
                                msg = "此系統查無此病患(" + chrno + ")收案資料!";
                                LogTool.SaveLogMessage(WebMethod.errorMsg, actionName, GetLogToolCS.ExternalController);
                                return View(actionName, "", msg);
                            }
                        }
                        else
                        {
                            msg = "參數錯誤、權限不足，請洽資訊室!";
                            LogTool.SaveLogMessage(WebMethod.errorMsg, actionName, GetLogToolCS.ExternalController);
                            return View(actionName, "", msg);
                        }

                    }
                    else
                    {
                        if (!string.IsNullOrWhiteSpace(ui.loginMsg))
                        {
                            msg = ui.loginMsg;
                        }
                        else
                        {
                            msg = "查無使用者(" + user_id + ")此系統使用權限!";
                        }
                        LogTool.SaveLogMessage(ui.loginMsg, actionName, GetLogToolCS.ExternalController);
                        return View(actionName, "", msg);
                    }
                }
                else
                {
                    msg = "user資訊不完整，無法登入系統!";
                    return View(actionName, "", msg);
                }



            }
            catch (Exception ex)
            {
                msg = "程式發生錯誤" + ex.Message;
                LogTool.SaveLogMessage(ex, actionName, GetLogToolCS.ExternalController);
                return View(actionName, "", msg);
            }
        }

        /// <summary>
        /// 取得Guide user加密資料
        /// </summary>
        /// <param name="userid"></param>
        /// <param name="userpwd"></param>
        /// <returns></returns>
        public string getcodeDES(string userid, string userpwd)
        {
            return SecurityTool.EncodeDES(userid + "|" + userpwd, IniFile.GetConfig("ExternalLink", "key"), IniFile.GetConfig("ExternalLink", "iv"));
        }

#if DEBUG
        /// <summary>
        /// 取得Guide user加密資料
        /// </summary>
        /// <param name="userid"></param>
        /// <param name="userpwd"></param>
        /// <returns></returns>
        public string DecodeDES(string hexString)
        {
            return SecurityTool.DecodeDES(hexString, IniFile.GetConfig("ExternalLink", "key"), IniFile.GetConfig("ExternalLink", "iv"));
        }
#endif




        private bool LoginCheck()
        {
            if (Session["ca_check"] == null || (bool)Session["ca_check"] == false)
            {
                Response.Write("<h2>參數錯誤</h2>");
                return false;
            }
            else
            {
                return true;
            }
        }
 
        public ActionResult OpenSystem()
        {
            return View();
        } 

        // GET: External
        /// <summary>
        /// 本地測試機
        /// http://localhost:5467/External/RCS/Guide?
        /// char_no=05945519
        /// token=eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJ1c2VyX2lkIjoicmNzIiwidXNlcl9uYW1lIjoi6aas6ZuF566h55CG6ICFIiwicm9sZSI6IklDTSIsImlhdCI6IjIwMTktMDctMDggMTM6MzM6MTEifQ.U9faCmOB8SapQ0pp33WTc4pgv-XrMTEL8KwZGfKs12o
        /// </summary>
        /// <param name="char_no">病歷號</param>
        /// <param name="token">驗證令牌</param>
        /// <returns></returns>
        public ActionResult RCS(string char_no, string token)
        {
            string actionName = "RCS";
            if (string.IsNullOrWhiteSpace(char_no))
            {
                return Content(this.returnErrorMsg("無病例號資料，請洽資訊人員! ", char_no, token)); 
            }
            try
            {
                PAYLOAD tokenObj = JwtAuthActionFilterAttribute.DecodeToken(token);
                try
                {
                    // 當天登入的token
                    if (!string.IsNullOrWhiteSpace(tokenObj.iat))
                    {
                        string iat = tokenObj.iat;
                        if (DateTime.Parse(iat).Date != DateTime.Now.Date)
                        {
                            return Content(this.returnErrorMsg("Token 格式錯誤或Token 過期!! ", char_no, token)); 
                        }
                        if (!string.IsNullOrWhiteSpace(tokenObj.user_id) && !string.IsNullOrWhiteSpace(tokenObj.user_name))
                        {
                            SQLProvider dbLink = new SQLProvider();
                            Dapper.DynamicParameters dp = new Dapper.DynamicParameters();
                            List<DB_RCS_RT_CASE> pList = new List<DB_RCS_RT_CASE>();
                            string query = string.Concat("SELECT * FROM ", DB_TABLE_NAME.DB_RCS_RT_CASE, " WHERE CHART_NO =@CHART_NO ORDER BY CARE_DATE DESC");
                            dp.Add("CHART_NO", char_no);
                            pList = dbLink.DBA.getSqlDataTable<DB_RCS_RT_CASE>(query, dp);
                            if (dbLink.DBA.hasLastError)
                            { 
                                LogTool.SaveLogMessage(dbLink.DBA.lastError, actionName, this.csName);
                                return Content(this.returnErrorMsg("程式發生錯誤，請洽資訊人員!", char_no, token)); 
                            }
                            if (pList.Count > 0)
                            {
                                tokenObj.role = "hosp";
                                TempData["CHART_NO"] = char_no;
                                if (pList.Exists(x=>x.PATIENT_SOURCE == "I"))
                                { 
                                    TempData["IPD_NO"] = pList.First(x => x.PATIENT_SOURCE == "I").IPD_NO;
                                }
                                else
                                { 
                                    TempData["IPD_NO"] = pList[0].IPD_NO;
                                }  
                                TempData["token"] = JwtAuthActionFilterAttribute.EncodeToken(tokenObj);
                                return View();
                            }
                            else
                            {
                                return Content(this.returnErrorMsg(string.Concat("無病例號(", char_no, ")呼吸照護系統資料!"), char_no, token));
                            }
                        }else
                        {
                            return Content(this.returnErrorMsg(string.Concat("無登入者帳號或登入者名稱!請洽資訊人員!"), char_no, token));  
                        }
                    }
                    else
                    {
                        return Content(this.returnErrorMsg(string.Concat("Token 格式錯誤或Token 過期!"), char_no, token)); 
                    }
                }
                catch (Exception ex1)
                { 
                    LogTool.SaveLogMessage(ex1, actionName, this.csName);
                    return Content(this.returnErrorMsg("登入驗證錯誤，請洽資訊人員! ", char_no, token));
                }
               
            }
            catch (Exception ex)
            { 
                LogTool.SaveLogMessage(ex, actionName, this.csName);
                return Content(this.returnErrorMsg("Token 格式錯誤或 無Token ! ", char_no, token));
            } 
            return Content(this.returnErrorMsg("Token 格式錯誤或 無Token ! ", char_no, token));
        }

        private string returnErrorMsg(string msg, string char_no, string token)
        {
            string actionName = "returnErrorMsg";
            LogTool.SaveLogMessage(char_no, actionName, this.csName);
            LogTool.SaveLogMessage(token, actionName, this.csName);
            return msg + string.Concat(" [發生時間:", DateTime.Now.ToString("yyyy - MM - dd HH: mm:ss"), "]");
        }

        /// <summary>
        /// 程式發生錯誤，回拋錯誤訊息給使用者
        /// </summary>
        /// <param name="msg"></param>
        [JwtAuthActionFilterAttribute(notVerification = true)]
        protected void throwHttpResponseException(string msg)
        {
            var resp = new HttpResponseMessage(HttpStatusCode.NotFound)
            {
                Content = new StringContent(string.Format(msg))
            };
            throw new HttpResponseException(resp);
        }

    }
}

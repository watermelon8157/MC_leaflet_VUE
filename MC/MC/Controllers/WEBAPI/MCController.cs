using RCS.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http; 
using RCS_Data;
using RCS_Data.Models.DB;
using RCSData.Models;
using RCS.Models.ViewModel;
using RCS_Data.Models;

namespace RCS.Controllers.WEBAPI
{

    [JwtAuthActionFilterAttribute(notVerification =true)]
    public class MCController : DefaultController
    {
        string csName = "MCController";


        MCModel _model = new MCModel();
        private UserInfo userinfo { get; set; }

        public MCController()
        {
            this.userinfo = new UserInfo()
            {
                user_id = "admin",
                user_name = "admin"
            };
        }

        public string HelloWord()
        { 
            return "HelloWord";
        }

        #region  action Function

        /// <summary>
        /// 新增病患資料
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public string INSERT_PAT_DATA(DB_MC_PATIENT_INFO model)
        {
            string actionName = "INSERT_PAT_DATA";
            this.DBLink.DBA.DBExecInsert<DB_MC_PATIENT_INFO>(new List<DB_MC_PATIENT_INFO>() { model });
            if (this.DBLink.DBA.hasLastError)
            {
                Com.Mayaminer.LogTool.SaveLogMessage(this.DBLink.DBA.lastError, actionName, this.csName);
            }

            return "儲存成功!";
        }


        /// <summary>
        /// 更新病患資料
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public string UPDATE_PAT_DATA(DB_MC_PATIENT_INFO model)
        {
            string actionName = "UPDATE_PAT_DATA";
            this.DBLink.DBA.DBExecUpdate<DB_MC_PATIENT_INFO>(new List<DB_MC_PATIENT_INFO>() { model });
            if (this.DBLink.DBA.hasLastError)
            {
                Com.Mayaminer.LogTool.SaveLogMessage(this.DBLink.DBA.lastError, actionName, this.csName);
            }

            return "儲存成功!";
        }

        #endregion

        #region  Pat Info Function

        /// <summary>
        /// 病患清單
        /// </summary>
        /// <param name="form"></param>
        /// <returns></returns>
        [JwtAuthActionFilterAttribute]
        [HttpPost]
        public List<DB_MC_PATIENT_INFO> GetPatList(JWT_Form_Body form)
        {
            string actionName = "GetPatList";
            List<DB_MC_PATIENT_INFO> pList = new List<DB_MC_PATIENT_INFO>();
            string sql = "SELECT * FROM " + DB_TABLE_NAME.DB_MC_PATIENT_INFO;
            pList = DBLink.DBA.getSqlDataTable<DB_MC_PATIENT_INFO>(sql);
            return pList;
        }

        /// <summary>
        /// 病患所有資料
        /// </summary>
        /// <param name="form"></param>
        /// <returns></returns>
        [JwtAuthActionFilterAttribute]
        [HttpPost]
        public List<DB_MC_PATIENT_INFO> GetPatListAll(JWT_Form_Body form)
        {
            string actionName = "GetPatListAll";
            List<DB_MC_PATIENT_INFO> pList = new List<DB_MC_PATIENT_INFO>();
            string sql = "SELECT * FROM " + DB_TABLE_NAME.DB_MC_PATIENT_INFO;
            pList = DBLink.DBA.getSqlDataTable<DB_MC_PATIENT_INFO>(sql);
            return pList;
        }

        #endregion

          
        /// <summary>
        /// 登入驗證
        /// </summary>
        /// <param name="form">登入資料</param>
        /// <returns></returns>
        [JwtAuthActionFilterAttribute(notVerification = true)]
        public object Login(Login_Form_Body form)
        {
            if (!string.IsNullOrWhiteSpace(form.site_id) || !string.IsNullOrWhiteSpace(form.hosp_id))
            {
                if (!string.IsNullOrWhiteSpace(form.site_id))
                {
                    if (_model.getMC_SITE_INFO(form.site_id).Count == 0)
                    { 
                        if (!string.IsNullOrWhiteSpace(form.LATITUDE) && !string.IsNullOrWhiteSpace(form.LONGITUDE))
                        {
                            List<DB_MC_SITE_INFO> pList = new List<DB_MC_SITE_INFO>();
                            pList.Add(new DB_MC_SITE_INFO() {
                                SITE_ID = form.site_id, 
                                CREATE_DATE = Function_Library.getDateNowString(DATE_FORMAT.yyyy_MM_dd_HHmmss),
                                CREATE_ID = this.userinfo.user_id,
                                CREATE_NAME = this.userinfo.user_name,
                                MODIFY_DATE = Function_Library.getDateNowString(DATE_FORMAT.yyyy_MM_dd_HHmmss),
                                MODIFY_ID = this.userinfo.user_id,
                                MODIFY_NAME = this.userinfo.user_name,
                                DATASTATUS = "1",
                                LATITUDE = form.LATITUDE,
                                LONGITUDE = form.LONGITUDE,
                            });
                            this.DBLink.DBA.DBExecInsert<DB_MC_SITE_INFO>(pList);
                            if (this.DBLink.DBA.hasLastError)
                            {
                                this.throwHttpResponseException("程式發生錯誤，請洽資訊人員!");
                                Com.Mayaminer.LogTool.SaveLogMessage(this.DBLink.DBA.lastError, "Login", this.csName);
                            }
                        }
                        else
                        {
                            this.throwHttpResponseException("查無經緯度資料無法填寫資料!!");
                        }
                    } 
                }
                return new
                {
                    Result = true,
                    token = JwtAuthActionFilterAttribute.EncodeToken(new PAYLOAD()
                    {
                        hosp_id = form.hosp_id,
                        site_id = form.site_id,
                        user_name = this.userinfo.user_name,
                        user_id = this.userinfo.user_id,
                    })
                };
            }
            else
            {
                this.throwHttpResponseException("請輸入資料!!");
            }
            this.throwHttpResponseException("請輸入資料!!");
            return false;
        }

       

        /// <summary>
        /// 驗證Auth
        /// </summary>
        /// <param name="form"></param>
        /// <returns></returns>
        [JwtAuthActionFilterAttribute]
        [HttpPost]
        public string JwtAuthCheck(JWT_Form_Body form)
        {
            return "合法登入!";
        }

        #region hosp function

        /// <summary>
        /// 醫院清單功能
        /// </summary>
        /// <param name="form"></param>
        /// <returns></returns>
        [JwtAuthActionFilterAttribute]
        [HttpPost]
        public List<DB_MC_HOSP_INFO> HospInfoList(JWT_Form_Body form)
        {
            List<DB_MC_HOSP_INFO> pList = new List<DB_MC_HOSP_INFO>();

            return pList;
         }

         
        #endregion


         
    }
}

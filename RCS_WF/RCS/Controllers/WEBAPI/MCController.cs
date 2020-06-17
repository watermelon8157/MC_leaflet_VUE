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

namespace RCS.Controllers.WEBAPI
{

    [JwtAuthActionFilterAttribute(notVerification =true)]
    public class MCController : DefaultController
    {
        string csName = "MCController";


        MCModel _model = new MCModel();


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
            if (!string.IsNullOrWhiteSpace(form.site_id))
            {

                return new
                {
                    Result = true,
                    token = JwtAuthActionFilterAttribute.EncodeToken(new PAYLOAD()
                    {
                        site_id = form.site_id,
                        user_name = "aaaa",
                        user_id = "aaaa"
                    })
                };
            }
            else
            {
                this.throwHttpResponseException("請輸入帳號或密碼!!");
            }
            this.throwHttpResponseException("帳號或密碼錯誤!!");
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

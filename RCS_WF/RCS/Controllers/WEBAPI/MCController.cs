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

namespace RCS.Controllers.WEBAPI
{

    [JwtAuthActionFilterAttribute(notVerification =true)]
    public class MCController : ApiController
    {
        string csName = "MCController";
        private SQLProvider _DBLink { get; set; }
        protected SQLProvider DBLink
        {
            get
            {
                if (this._DBLink == null)
                {
                    this._DBLink = new SQLProvider();
                }
                return this._DBLink;
            }
        }

        public string HelloWord()
        { 
            return "HelloWord";
        }

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
        /// 登入驗證
        /// </summary>
        /// <param name="form">登入資料</param>
        /// <returns></returns>
        [JwtAuthActionFilterAttribute(notVerification = true)]
        public object Login(Login_Form_Body form)
        {
            if (!string.IsNullOrWhiteSpace(form.site_id) )
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
        /// 程式發生錯誤，回拋錯誤訊息給使用者
        /// </summary>
        /// <param name="msg"></param>
        [JwtAuthActionFilterAttribute(notVerification = true)]
        protected void throwHttpResponseException(string msg)
        {
            var resp = new System.Net.Http.HttpResponseMessage(System.Net.HttpStatusCode.NotFound)
            {
                Content = new System.Net.Http.StringContent(string.Format(msg))
            };
            throw new System.Web.Http.HttpResponseException(resp);
        }
    }
}

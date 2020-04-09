using RCS.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http; 
using RCS_Data;
using RCS_Data.Models.DB;

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

    }
}

using RCS.Models;
using RCS_Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace RCS.Controllers.WEBAPI
{
    public class DefaultController : ApiController
    {
         
        string csName = "DefaultController"; 
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

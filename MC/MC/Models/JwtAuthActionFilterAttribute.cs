using Com.Mayaminer;
using Jose;
using RCSData.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Web;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;

namespace RCS.Models
{
    /// <summary>
    /// API驗證Token
    /// https://dotblogs.com.tw/wellwind/2016/11/24/jwt-auth-web-api
    /// </summary>
    public class JwtAuthActionFilterAttribute : ActionFilterAttribute
    {
        /// <summary>
        /// secret key source
        /// </summary>
        static string _secret { get; set; }
        static string secret
        {
            get
            {
                if (string.IsNullOrWhiteSpace(_secret))
                {
                    JwtAuthActionFilterAttribute._secret = "tokenSecret";
                }
                return JwtAuthActionFilterAttribute._secret;
            }
        }

        /// <summary>
        /// 不驗證Token判斷
        /// </summary>
        public bool notVerification { get; set; }

        /// <summary>
        /// OnAction 判斷
        /// </summary>
        /// <param name="actionContext"></param>
        public override void OnActionExecuting(HttpActionContext actionContext)
        {
            var auth = actionContext.Request.Headers.Authorization;
            if (!notVerification)
            {
                // 驗證Token是否合法
                RCS.Models.JwtAuthActionFilterAttribute.VerificationToken(actionContext);
            }

            base.OnActionExecuting(actionContext);
        }

        /// <summary>
        ///  Verification Token
        /// </summary>
        /// <param name="actionContext"></param>
        public static void VerificationToken(HttpActionContext actionContext)
        {
            if (actionContext.Request.Headers.Authorization == null || actionContext.Request.Headers.Authorization.Scheme != "Bearer")
            {
                RCS.Models.JwtAuthActionFilterAttribute.SetErrorResponse(actionContext, "驗證錯誤，請重新登入!");
            }
            else
            {
                try
                {
                    PAYLOAD jwtObject = DecodeToken(actionContext.Request.Headers.Authorization.Parameter);
                }
                catch (Exception ex)
                {
                    RCS.Models.JwtAuthActionFilterAttribute.SetErrorResponse(actionContext, "不合法的登入方式，請重新登入!");
                }
            }
        }

        /// <summary>
        /// Encode token
        /// </summary>
        /// <param name="payload"></param>
        /// <returns></returns>
        public static string EncodeToken(PAYLOAD payload)
        {
            return Jose.JWT.Encode(payload,
                Encoding.UTF8.GetBytes(IniFile.GetConfig("System", JwtAuthActionFilterAttribute.secret)),
                JwsAlgorithm.HS256);
        }

        /// <summary>
        /// Decode token
        /// </summary>
        /// <param name="Authorization"></param>
        /// <returns></returns>
        public static PAYLOAD DecodeToken(string Authorization)
        {
            return Jose.JWT.Decode<PAYLOAD>(
                        Authorization,
                        Encoding.UTF8.GetBytes(IniFile.GetConfig("System", JwtAuthActionFilterAttribute.secret)),
                        JwsAlgorithm.HS256); ;
        }

        /// <summary>
        /// 驗證錯誤回傳 error
        /// </summary>
        /// <param name="actionContext"></param>
        /// <param name="message"></param>
        private static void SetErrorResponse(HttpActionContext actionContext, string message)
        {
            var resp = new HttpResponseMessage(HttpStatusCode.Unauthorized)
            {
                Content = new StringContent(string.Format(message))
            };
            throw new HttpResponseException(resp);
        }
    }
}
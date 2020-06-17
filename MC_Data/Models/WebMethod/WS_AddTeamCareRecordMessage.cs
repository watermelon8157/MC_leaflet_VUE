using Newtonsoft.Json;
using RCS_Data;
using RCSData.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RCSData.Models
{
    public partial class WebMethod
    {
        /// <summary>
        /// UploadEMRFile
        /// </summary>
        /// <param name="iwp"></param>
        /// <param name="OrderStr"></param>
        /// <param name="Base64Str"></param>
        public string AddTeamCareRecordMessage(IWebServiceParam iwp, string pUser, string pUserPWD, string pFee_NO, string pRecordMsg, string pPrecorddatetime, string pReceiveDept)
        {
            WS_AddTeamCareRecordMessage ba = new WS_AddTeamCareRecordMessage(iwp,   pUser,   pUserPWD,   pFee_NO,   string.Concat("<![CDATA[", pRecordMsg, "]]>"),   pPrecorddatetime,   pReceiveDept);
            ServiceResult<string> sr = HISData.getTeamCareRecordServiceResult(ba);
            if (sr.errorMsg.Contains("Success"))
            { 
                /// 成功回傳結果１：Success
                /// 失敗回傳結果1：使用者密碼錯誤
                /// 失敗回傳結果2：查無使用者
                /// 失敗回傳結果3：查無此住院序號 
                return "上傳成功";
            } 
            return sr.errorMsg;
        }
    }
    public class WS_AddTeamCareRecordMessage : AwebMethod<string>, IwebMethod<string>
    {
        public string webMethodName { get { return this.iwp.webMethodName; } }
        public override string wsSession { get { return "RCS_WS_TeamCare"; } }

        public WS_AddTeamCareRecordMessage(IWebServiceParam pIwp, string pUser, string pUserPWD, string pFee_NO, string pRecordMsg, string pPrecorddatetime, string pReceiveDept, string pMessageType = "")
        {
            this.iwp = pIwp;
            setParam();
            if (!string.IsNullOrWhiteSpace(pUser) && !string.IsNullOrWhiteSpace(pUserPWD))
            {
                #region 整理傳入參數
                this.paramList["pUser"].paramValue = pUser;
                this.paramList["pUserPWD"].paramValue = pUserPWD;
                this.paramList["pFee_NO"].paramValue = pFee_NO;
                this.paramList["pRecordMsg"].paramValue = pRecordMsg;
                this.paramList["pPrecorddatetime"].paramValue = pPrecorddatetime;
                this.paramList["pReceiveDept"].paramValue = pReceiveDept;
                this.paramList["pMessageType"].paramValue = pMessageType;
                #endregion
            }
            else
            {
                this.ServiceResult.datastatus = RCS_Data.HISDataStatus.ParametersError;
                if (string.IsNullOrWhiteSpace(pUser))
                    this.ServiceResult.errorMsg = "上傳參數不能為空值!"; 
            }
        }
        public void setParam()
        {
            this.ServiceResult = new ServiceResult<string>();
            this.paramList = this.iwp.paramList;
            this.returnValue = this.iwp.returnValue;
        }

    }
}

using RCS.Models.ViewModel;
using RCS_Data.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;

namespace RCS
{
    /// <summary>
    ///RCSWebService 的摘要描述
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // 若要允許使用 ASP.NET AJAX 從指令碼呼叫此 Web 服務，請取消註解下列一行。
    // [System.Web.Script.Services.ScriptService]
    public class RCSWebService : System.Web.Services.WebService
    {
        string csName = "RCSWebService";

        [WebMethod]
        public string HelloWorld()
        {
            return "Hello World";
        }
        [WebMethod]
        public string UploadEMRFile(string OrderStr, string Base64Str)
        {
            return "-1";
        }

        /// <summary>
        /// 雙和-拋跨團隊訊息
        /// </summary>
        /// <param name="pUser">使用者員編</param>
        /// <param namse="pUserPWD">密碼</param>
        /// <param name="pFee_NO">病人批序</param>
        /// <param name="pRecordMsg">拋轉內容訊息</param>
        /// <param name="pReceiveDept">使用者科別代碼</param>
        /// <param name="pMessageType">空值</param>
        /// <param name="pPrecorddatetime">日期時間，型態(date)</param>
        /// <returns>
        /// 成功回傳結果１：Success
        /// 失敗回傳結果1：使用者密碼錯誤
        /// 失敗回傳結果2：查無使用者
        /// 失敗回傳結果3：查無此住院序號
        /// </returns>
        [WebMethod]
        public string AddTeamCareRecordMessage_note(string pUser, string pUserPWD, string pFee_NO, string pRecordMsg, string pReceiveDept, string pMessageType, string pPrecorddatetime)
        {
            return "";
        }

        /// <summary>
        /// 取得OnePage網址清單
        /// </summary>
        /// <param name="chrNo">病歷號</param>
        /// <param name="token">token</param>
        /// <returns></returns>
        [WebMethod]
        public string OnePage(string chrNo, string token)
        {
            WSresponse rm = new WSresponse();
            List<OnePagePDF> pList = new List<OnePagePDF>();
            string actionName = "OnePage";
            try
            { 
                pList.AddRange(new RtAssess().OnePagePDFList(chrNo));// 呼吸治療評估單
                pList.AddRange(new RtRecord().OnePagePDFList(chrNo));// 呼吸照護紀錄單
                pList.AddRange(new RTTakeoff().OnePagePDFList(chrNo));// 呼吸脫離評估單 
                rm.OnePageList = pList;

            }
            catch (Exception ex)
            {
                Com.Mayaminer.LogTool.SaveLogMessage(ex, actionName, this.csName);
                rm.hasError = true;
                rm.msg = "程式碼發生錯誤，請下資訊人員!";
            }
            
            return Newtonsoft.Json.JsonConvert.SerializeObject(rm);
        }
    }

    public class WSresponse
    {
        public Boolean hasError { get; set; }

        public string msg { get; set; }

        public  List<OnePagePDF> OnePageList { get; set; }
    }
}

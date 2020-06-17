//**************************************************
//2016/08/16
//#2172 建立DB Model
//功能 產生HttpWebRequest及取得WebResponse(串字串的方式)
//2016/08/16 補充:說明此方法的註解
//**************************************************
using RCS_Data;
using System;
using System.IO;
using System.Net;
using System.Xml;

namespace Com.Mayaminer
{
    public class APIConnector {

        /// <summary> 錯誤訊息</summary>
        public static string lastError = "";
        /// <summary> 資料目前狀態 </summary>
        public static RCS_Data.HISDataStatus nowDataStatus { get; set; }

        /// <summary>呼叫WebService</summary>
        /// <param name="url">Web Service 網址</param>
        /// <param name="SoapAction">WebServiceSoapAction</param>
        /// <param name="XmlStr">XMLResponses</param>
        /// <returns></returns>
        public static string CallWebService(string url, string SoapAction, string XmlStr) {
            XmlDocument soapEnvelopeXml = CreateSoapEnvelope(XmlStr);
            HttpWebRequest webRequest = CreateWebRequest(url, SoapAction);
            string soapResult = "";
            lastError = "";
            nowDataStatus = RCS_Data.HISDataStatus.SuccessWithoutData;
            try {
                LogTool.SaveLogMessage(string.Concat("SoapAction:", SoapAction), "CallWebServiceTime", "APIConnector");
                InsertSoapEnvelopeIntoWebRequest(soapEnvelopeXml, webRequest);
                // begin async call to web request.
                LogTool.SaveLogMessage(string.Concat("START:", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")), "CallWebServiceTime", "APIConnector");
                IAsyncResult asyncResult = webRequest.BeginGetResponse(null, null);
                asyncResult.AsyncWaitHandle.WaitOne();

                //completed web request.
                using (WebResponse webResponse = webRequest.EndGetResponse(asyncResult)) {
                    using (StreamReader rd = new StreamReader(webResponse.GetResponseStream())) {
                        soapResult = rd.ReadToEnd();
                    }
                }
                LogTool.SaveLogMessage(soapResult, "CallWebServiceTime", "APIConnector");
                LogTool.SaveLogMessage(string.Concat("END:", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")), "CallWebServiceTime", "APIConnector");
            }
            catch(WebException ex){
                if (ex.Status == WebExceptionStatus.ProtocolError )
                {
                    // http://stackoverflow.com/questions/2470933/what-exception-is-thrown-if-a-web-service-im-using-times-out
                    // stackoverflow tell us. need check InnerException
                    lastError =  "WebService發生錯誤!錯誤訊息如下，" + ex.Message;
                    nowDataStatus = RCS_Data.HISDataStatus.WebServiceError;
                }
                else
                {
                    // 處理上傳物件資訊
                    lastError = "程式發生錯誤!錯誤訊息如下，" + ex.Message;
                    nowDataStatus = RCS_Data.HISDataStatus.ExceptionError;
                }
                LogTool.SaveLogMessage("URL=" + url + ",SoapAction=" + SoapAction + ",status(" + ex.Status.ToString()+ ")" + lastError, "CallWebService", GetLogToolCS.APIConnector);
                LogTool.SaveLogMessage(XmlStr, "CallWebService", GetLogToolCS.APIConnector);
                LogTool.SaveLogMessage(ex, "CallWebService",GetLogToolCS.APIConnector);
            }
            return soapResult;
        }

        private static HttpWebRequest CreateWebRequest(string url, string action) {
            HttpWebRequest webRequest = (HttpWebRequest)WebRequest.Create(url);
            webRequest.Headers.Add("SOAPAction", action);
            webRequest.ContentType = @"text/xml;charset=""utf-8""";
            webRequest.Accept = "text/xml";
            webRequest.Method = "POST";
            return webRequest;
        }

        private static XmlDocument CreateSoapEnvelope(string XmlStr) {
            XmlDocument soapEnvelop = new XmlDocument();
            soapEnvelop.LoadXml(@"<?xml version=""1.0"" encoding=""utf-8""?><soap:Envelope xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"" xmlns:xsd=""http://www.w3.org/2001/XMLSchema"" xmlns:soap=""http://schemas.xmlsoap.org/soap/envelope/""><soap:Body>" + XmlStr + "</soap:Body></soap:Envelope>");
            return soapEnvelop;
        }

        private static void InsertSoapEnvelopeIntoWebRequest(XmlDocument soapEnvelopeXml, HttpWebRequest webRequest) {
            using (Stream stream = webRequest.GetRequestStream()) {
                soapEnvelopeXml.Save(stream);
            }
        }

        /// <summary>
        /// 檢查指定網址連線
        /// </summary>
        /// <param name="URL"></param>
        /// <param name="port"></param>
        /// <returns></returns>
        public static RESPONSE_MSG checkURLConnection(string URL, int port = 80)
        {
            RESPONSE_MSG rm = new RESPONSE_MSG();
            string actionName = "checkHisConnection";
            try
            {
                try
                {
                    System.Net.Sockets.TcpClient clnt = new System.Net.Sockets.TcpClient(URL, port);
                    clnt.Close();
                    rm.status = RESPONSE_STATUS.SUCCESS;
                }
                catch (System.Exception ex)
                {
                    LogTool.SaveLogMessage(ex, actionName, GetLogToolCS.HISDataExchange);
                    rm.status = RESPONSE_STATUS.EXCEPTION;
                    rm.message = ex.Message;
                }
            }
            catch (Exception ex)
            {
                LogTool.SaveLogMessage(ex, actionName, GetLogToolCS.HISDataExchange);
            }
            return rm;
        }

        /// <summary>
        /// 檢查指定網址連線
        /// </summary>
        /// <param name="URL"></param>
        /// <param name="port"></param>
        /// <returns></returns>
        public static RESPONSE_MSG checkWebServiceConnection(string URL)
        {
            RESPONSE_MSG rm = new RESPONSE_MSG();
            string actionName = "checkWebServiceConnection";
            try
            {
                var myRequest1 = (HttpWebRequest)WebRequest.Create("http://10.168.30.11/NISWS/nis.asmx?op=GetCostCenterList");

                var response1 = (HttpWebResponse)myRequest1.GetResponse();
                var myRequest2 = (HttpWebRequest)WebRequest.Create("http://10.168.30.11/NISWS/nis.asmx?op=Rcs_UserLogin");

                var response2 = (HttpWebResponse)myRequest2.GetResponse();


                var myRequest = (HttpWebRequest)WebRequest.Create(URL);

                var response = (HttpWebResponse)myRequest.GetResponse();

                if (response.StatusCode == HttpStatusCode.OK)
                {
                    rm.status = RESPONSE_STATUS.SUCCESS;
                }
                else
                {
                    rm.status = RESPONSE_STATUS.ERROR;
                    rm.message = response.StatusDescription;
                }
            }
            catch (Exception ex)
            {
                LogTool.SaveLogMessage(ex, actionName, GetLogToolCS.HISDataExchange);
                rm.status = RESPONSE_STATUS.EXCEPTION;
                rm.message = ex.Message;
            }
            return rm;
        }
    }

    public class ASHXConnector
    {

        /// <summary> 錯誤訊息</summary>
        public static string lastError = "";
        /// <summary> 資料目前狀態 </summary>
        public static RCS_Data.HISDataStatus nowDataStatus { get; set; }

        /// <summary>呼叫WebService</summary>
        /// <param name="url">Web Service 網址</param>
        /// <param name="SoapAction">WebServiceSoapAction</param>
        /// <param name="XmlStr">XMLResponses</param>
        /// <returns></returns>
        public static string CallWebService(string url, string route)
        {
            HttpWebRequest webRequest = CreateWebRequest(url, route);
            string soapResult = "";
            lastError = "";
            nowDataStatus = RCS_Data.HISDataStatus.SuccessWithoutData;
            try
            {
                LogTool.SaveLogMessage(string.Concat("SoapAction:", route), "CallWebServiceTime", "APIConnector");
                LogTool.SaveLogMessage(string.Concat("START:", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")), "CallWebServiceTime", "APIConnector");
                IAsyncResult asyncResult = webRequest.BeginGetResponse(null, null);
                asyncResult.AsyncWaitHandle.WaitOne();

                //completed web request.
                using (WebResponse webResponse = webRequest.EndGetResponse(asyncResult))
                {
                    using (StreamReader rd = new StreamReader(webResponse.GetResponseStream()))
                    {
                        soapResult = rd.ReadToEnd();
                    }
                }
                LogTool.SaveLogMessage(string.Concat("END:", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")), "CallWebServiceTime", "APIConnector");
            }
            catch (WebException ex)
            {
                if (ex.Status == WebExceptionStatus.ProtocolError)
                {
                    // http://stackoverflow.com/questions/2470933/what-exception-is-thrown-if-a-web-service-im-using-times-out
                    // stackoverflow tell us. need check InnerException
                    lastError = "WebService發生錯誤!錯誤訊息如下，" + ex.Message;
                    nowDataStatus = RCS_Data.HISDataStatus.WebServiceError;
                }
                else
                {
                    // 處理上傳物件資訊
                    lastError = "程式發生錯誤!錯誤訊息如下，" + ex.Message;
                    nowDataStatus = RCS_Data.HISDataStatus.ExceptionError;
                }
                LogTool.SaveLogMessage("URL=" + url + ",route=" + route + ",status(" + ex.Status.ToString() + ")" + lastError, "CallWebService", GetLogToolCS.APIConnector);
                LogTool.SaveLogMessage(ex, "CallWebService", GetLogToolCS.APIConnector);
            }
            return soapResult;
        }

        private static HttpWebRequest CreateWebRequest(string url, string action)
        {
            HttpWebRequest webRequest = (HttpWebRequest)WebRequest.Create(url+ action);
            //webRequest.Headers.Add("SOAPAction", action);
            webRequest.ContentType = @"application/x-www-form-urlencoded; charset=UTF-8";
            webRequest.Method = "GET";
            return webRequest;
        }

        private static XmlDocument CreateSoapEnvelope(string XmlStr)
        {
            XmlDocument soapEnvelop = new XmlDocument();
            soapEnvelop.LoadXml(@"<?xml version=""1.0"" encoding=""utf-8""?><soap:Envelope xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"" xmlns:xsd=""http://www.w3.org/2001/XMLSchema"" xmlns:soap=""http://schemas.xmlsoap.org/soap/envelope/""><soap:Body>" + XmlStr + "</soap:Body></soap:Envelope>");
            return soapEnvelop;
        }

        private static void InsertSoapEnvelopeIntoWebRequest(XmlDocument soapEnvelopeXml, HttpWebRequest webRequest)
        {
            using (Stream stream = webRequest.GetRequestStream())
            {
                soapEnvelopeXml.Save(stream);
            }
        }

        /// <summary>
        /// 檢查指定網址連線
        /// </summary>
        /// <param name="URL"></param>
        /// <param name="port"></param>
        /// <returns></returns>
        public static RESPONSE_MSG checkURLConnection(string URL, int port = 80)
        {
            RESPONSE_MSG rm = new RESPONSE_MSG();
            string actionName = "checkHisConnection";
            try
            {
                try
                {
                    System.Net.Sockets.TcpClient clnt = new System.Net.Sockets.TcpClient(URL, port);
                    clnt.Close();
                    rm.status = RESPONSE_STATUS.SUCCESS;
                }
                catch (System.Exception ex)
                {
                    LogTool.SaveLogMessage(ex, actionName, GetLogToolCS.HISDataExchange);
                    rm.status = RESPONSE_STATUS.EXCEPTION;
                    rm.message = ex.Message;
                }
            }
            catch (Exception ex)
            {
                LogTool.SaveLogMessage(ex, actionName, GetLogToolCS.HISDataExchange);
            }
            return rm;
        }

        /// <summary>
        /// 檢查指定網址連線
        /// </summary>
        /// <param name="URL"></param>
        /// <param name="port"></param>
        /// <returns></returns>
        public static RESPONSE_MSG checkWebServiceConnection(string URL)
        {
            RESPONSE_MSG rm = new RESPONSE_MSG();
            string actionName = "checkWebServiceConnection";
            try
            {
                var myRequest1 = (HttpWebRequest)WebRequest.Create("http://10.168.30.11/NISWS/nis.asmx?op=GetCostCenterList");

                var response1 = (HttpWebResponse)myRequest1.GetResponse();
                var myRequest2 = (HttpWebRequest)WebRequest.Create("http://10.168.30.11/NISWS/nis.asmx?op=Rcs_UserLogin");

                var response2 = (HttpWebResponse)myRequest2.GetResponse();


                var myRequest = (HttpWebRequest)WebRequest.Create(URL);

                var response = (HttpWebResponse)myRequest.GetResponse();

                if (response.StatusCode == HttpStatusCode.OK)
                {
                    rm.status = RESPONSE_STATUS.SUCCESS;
                }
                else
                {
                    rm.status = RESPONSE_STATUS.ERROR;
                    rm.message = response.StatusDescription;
                }
            }
            catch (Exception ex)
            {
                LogTool.SaveLogMessage(ex, actionName, GetLogToolCS.HISDataExchange);
                rm.status = RESPONSE_STATUS.EXCEPTION;
                rm.message = ex.Message;
            }
            return rm;
        }
    }
}
using Com.Mayaminer;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Web;

namespace RCSData.Models
{
    public partial class HISDataExchange
    {
        string csName { get { return "HISDataExchange"; } }

        /// <summary>
        /// 取得WS ServiceResult的資料
        /// </summary>
        /// <param name="webMethod"></param>
        /// <returns></returns>
        public ServiceResult<T> getServiceResult<T>(IwebMethod<T> webMethod)
        {
            string actionName = "getServiceResult";
            try
            { 
                if (webMethod.ServiceResult.datastatus == RCS_Data.HISDataStatus.SuccessWithoutData)
                {
                    webMethod.ServiceResult = setGetServiceResultFormat<T>( webMethod);
                    if (webMethod.ServiceResult.datastatus == RCS_Data.HISDataStatus.SuccessWithData)
                    { 
                        webMethod.setReturnValue();
                        Com.Mayaminer.LogTool.SaveLogMessage(webMethod.ServiceResult.returnJson, webMethod.webMethodName, "getData");
                    }
                    else
                    {
                        if(webMethod.ServiceResult.datastatus == RCS_Data.HISDataStatus.SuccessWithoutData)
                        {
                            Com.Mayaminer.LogTool.SaveLogMessage("查無資料", actionName, csName);
                        }
                        else
                        {
                            Com.Mayaminer.LogTool.SaveLogMessage(webMethod.ServiceResult.errorMsg, actionName, csName);
                        }
                    }
                  
                }
            }
            catch (Exception ex)
            {
                webMethod.ServiceResult.datastatus = RCS_Data.HISDataStatus.ExceptionError;
                webMethod.ServiceResult.errorMsg = ex.Message;
                Com.Mayaminer.LogTool.SaveLogMessage(ex, actionName, csName);
            }
            return webMethod.ServiceResult;
        }

        private ServiceResult<T> setGetServiceResultFormat<T>(string wsSession,string webMethodName, Dictionary<string, webParam> paramList)
        {
            ServiceResult<T> sr = new ServiceResult<T>();
            string actionName = "setGetServiceResultFormat";
            try
            {
                string WebServiceUrl = Com.Mayaminer.IniFile.GetConfig(wsSession, "WebServiceUrl");
                string SoapActionUrl = Com.Mayaminer.IniFile.GetConfig(wsSession, "SoapActionUrl");
                string theResponseFormat = Com.Mayaminer.IniFile.GetConfig(wsSession, "ResponseFormat");
                string WebServiceSoapAction = SoapActionUrl + webMethodName;

                System.Text.StringBuilder sb = new System.Text.StringBuilder();
                foreach (KeyValuePair<string, webParam> item in paramList)
                {
                    sb.Append(string.IsNullOrEmpty(item.Value.paramValue) ? "<" + item.Value.paramName + "/>" : string.Format("<{0}>{1}</{0}>", item.Value.paramName, item.Value.paramValue));
                }
                string xml_str = string.Format("<{0} xmlns=\"{1}\">{2}</{0}>", webMethodName, SoapActionUrl, sb.ToString());
                string xml_responses = Com.Mayaminer.APIConnector.CallWebService(WebServiceUrl, WebServiceSoapAction, xml_str);
                if (string.IsNullOrWhiteSpace(Com.Mayaminer.APIConnector.lastError))
                {
                    System.Xml.Linq.XDocument xd = System.Xml.Linq.XDocument.Parse(xml_responses);
                    string json_str = "";
                    switch (theResponseFormat)
                    {
                        case "2":
                            byte[] bytes = Convert.FromBase64String(xd.Descendants().First(d => d.Name.LocalName == webMethodName + "Response").Value);
                            if (bytes.Any())
                            {
                                json_str = System.Text.Encoding.UTF8.GetString(bytes);
                                sr.datastatus = RCS_Data.HISDataStatus.SuccessWithData;
                            }
                            else
                            {
                                Com.Mayaminer.LogTool.SaveLogMessage(string.Format("方法[{0}]沒有取得資料!", webMethodName, Newtonsoft.Json.JsonConvert.SerializeObject(paramList)), actionName, csName);
                                sr.datastatus = RCS_Data.HISDataStatus.SuccessWithoutData;
                            }
                            break;
                        case "3":
                            System.IO.StringReader theReader = new System.IO.StringReader(xd.Descendants().First(d => d.Name.LocalName == webMethodName + "Response").FirstNode.ToString());
                            DataSet theDataSet = new DataSet();
                            theDataSet.ReadXml(theReader);
                            json_str = Newtonsoft.Json.JsonConvert.SerializeObject(theDataSet.Tables[0]); 
                            sr.datastatus = RCS_Data.HISDataStatus.SuccessWithData;
                            break;
                        default:
                            json_str = xd.Descendants().First(d => d.Name.LocalName == webMethodName + "Response").Value;
                            sr.datastatus = RCS_Data.HISDataStatus.SuccessWithData;
                            break;
                    }
                    if (sr.datastatus == RCS_Data.HISDataStatus.SuccessWithData)
                    {

                        sr.returnList = Newtonsoft.Json.JsonConvert.DeserializeObject<DataTable>(json_str);
                        if (sr.returnList.Rows.Count == 0)
                        {
                            sr.datastatus = RCS_Data.HISDataStatus.SuccessWithoutData;
                        }
                    }
                }
                else
                {
                    sr.errorMsg = Com.Mayaminer.APIConnector.lastError;
                    sr.datastatus = RCS_Data.HISDataStatus.ExceptionError;
                }

                

            }
            catch (Exception ex)
            {
                sr.datastatus = RCS_Data.HISDataStatus.ExceptionError;
                sr.errorMsg = ex.Message;
                Com.Mayaminer.LogTool.SaveLogMessage(ex, actionName, csName);
            }
            return sr;
        }

        private ServiceResult<T> setGetServiceResultFormat<T>(IwebMethod<T> webMethod)
        {
            ServiceResult<T> sr = new ServiceResult<T>();
            string actionName = "setGetServiceResultFormat";
            try
            {
                string WebServiceUrl = Com.Mayaminer.IniFile.GetConfig(webMethod.wsSession, "WebServiceUrl");
                string SoapActionUrl = Com.Mayaminer.IniFile.GetConfig(webMethod.wsSession, "SoapActionUrl");
                string theResponseFormat = Com.Mayaminer.IniFile.GetConfig(webMethod.wsSession, "ResponseFormat");
                string WebServiceSoapAction = SoapActionUrl + webMethod.webMethodName;

                System.Text.StringBuilder sb = new System.Text.StringBuilder();
                foreach (KeyValuePair<string, webParam> item in webMethod.paramList)
                {
                    sb.Append(string.IsNullOrEmpty(item.Value.paramValue) ? "<" + item.Value.paramName + "/>" : string.Format("<{0}>{1}</{0}>", item.Value.paramName, item.Value.paramValue));
                }
                string xml_str = string.Format("<{0} xmlns=\"{1}\">{2}</{0}>", webMethod.webMethodName, SoapActionUrl, sb.ToString());
                string xml_responses = Com.Mayaminer.APIConnector.CallWebService(WebServiceUrl, WebServiceSoapAction, xml_str);
                if (string.IsNullOrWhiteSpace(Com.Mayaminer.APIConnector.lastError))
                {
                    System.Xml.Linq.XDocument xd = System.Xml.Linq.XDocument.Parse(xml_responses);
                    string json_str = ""; 
                    byte[] bytes;
                    switch (theResponseFormat)
                    {
                        case "2":
                             bytes = Convert.FromBase64String(xd.Descendants().First(d => d.Name.LocalName == webMethod.webMethodName + "Response").Value);
                            if (bytes.Any())
                            {
                                json_str = System.Text.Encoding.UTF8.GetString(bytes);
                                sr.datastatus = RCS_Data.HISDataStatus.SuccessWithData;
                            }
                            else
                            {
                                Com.Mayaminer.LogTool.SaveLogMessage(string.Format("方法[{0}]沒有取得資料!", webMethod.webMethodName, Newtonsoft.Json.JsonConvert.SerializeObject(webMethod.paramList)), actionName, csName);
                                sr.datastatus = RCS_Data.HISDataStatus.SuccessWithoutData;
                            }
                            break;
                        case "4":
                            bytes = Convert.FromBase64String(xd.Descendants().First(d => d.Name.LocalName == webMethod.webMethodName + "Response").Value);
                            if (bytes.Any())
                            {
                                json_str = Com.Mayaminer.CompressTool.DecompressString(bytes);
                                sr.datastatus = RCS_Data.HISDataStatus.SuccessWithData;
                            }
                            else
                            {
                                LogTool.SaveLogMessage(string.Format("方法[{0}]沒有取得資料!", webMethod.webMethodName, JsonConvert.SerializeObject(webMethod.paramList)), actionName, GetLogToolCS.getData);
                                sr.datastatus = RCS_Data.HISDataStatus.SuccessWithoutData;
                            }
                            break;
                        case "3":
                            System.IO.StringReader theReader = new System.IO.StringReader(xd.Descendants().First(d => d.Name.LocalName == webMethod.webMethodName + "Response").FirstNode.ToString());
                            DataSet theDataSet = new DataSet();
                            theDataSet.ReadXml(theReader);
                            json_str = Newtonsoft.Json.JsonConvert.SerializeObject(theDataSet.Tables[0]);
                            sr.datastatus = RCS_Data.HISDataStatus.SuccessWithData;
                            break;
                        default:
                            json_str = xd.Descendants().First(d => d.Name.LocalName == webMethod.webMethodName + "Response").Value;
                            sr.datastatus = RCS_Data.HISDataStatus.SuccessWithData;
                            break;
                    }
                    if (sr.datastatus == RCS_Data.HISDataStatus.SuccessWithData)
                    {
                        if (webMethod.wsSession == "RCS_WS_EMR")
                        {
                            Com.Mayaminer.LogTool.SaveLogMessage(json_str, actionName, csName);
                            sr.datastatus = RCS_Data.HISDataStatus.SuccessWithoutData; 
                        }
                        else
                        {
                            switch (webMethod.hosp_id)
                            {
                                case "WF":
                                case "ECK":
                                case "SHH":
                                    sr.returnList = Newtonsoft.Json.JsonConvert.DeserializeObject<DataTable>(json_str);
                                    if (sr.returnList.Rows.Count == 0)
                                    {
                                        sr.datastatus = RCS_Data.HISDataStatus.SuccessWithoutData;
                                    }
                                    break;
                                default:
                                    sr = Newtonsoft.Json.JsonConvert.DeserializeObject<ServiceResult<T>>(json_str);
                                    break;
                            }
                        }
                         
                       
                    }
                }
                else
                {
                    sr.errorMsg = Com.Mayaminer.APIConnector.lastError;
                    sr.datastatus = RCS_Data.HISDataStatus.ExceptionError;
                }



            }
            catch (Exception ex)
            {
                sr.datastatus = RCS_Data.HISDataStatus.ExceptionError;
                sr.errorMsg = ex.Message;
                Com.Mayaminer.LogTool.SaveLogMessage(ex, actionName, csName);
            }
            return sr;
        }

        /// <summary>
        /// 取得WS ServiceResult的資料
        /// </summary>
        /// <param name="webMethod"></param>
        /// <returns></returns>
        public ServiceResult<T> getTeamCareRecordServiceResult<T>(IwebMethod<T> webMethod)
        {
            string actionName = "getServiceResult";
            try
            {
                if (webMethod.ServiceResult.datastatus == RCS_Data.HISDataStatus.SuccessWithoutData)
                {
                    webMethod.ServiceResult = setTeamCareRecordServiceResultFormat<T>(webMethod);
                    Com.Mayaminer.LogTool.SaveLogMessage(Newtonsoft.Json.JsonConvert.SerializeObject(webMethod.ServiceResult), actionName, csName);

                }
            }
            catch (Exception ex)
            {
                webMethod.ServiceResult.datastatus = RCS_Data.HISDataStatus.ExceptionError;
                webMethod.ServiceResult.errorMsg = ex.Message;
                Com.Mayaminer.LogTool.SaveLogMessage(ex, actionName, csName);
            }
            return webMethod.ServiceResult;
        }

        private ServiceResult<T> setTeamCareRecordServiceResultFormat<T>(IwebMethod<T> webMethod)
        {
            ServiceResult<T> sr = new ServiceResult<T>();
            string actionName = "setGetServiceResultFormat";
            try
            {
                string WebServiceUrl = Com.Mayaminer.IniFile.GetConfig(webMethod.wsSession, "WebServiceUrl");
                string SoapActionUrl = Com.Mayaminer.IniFile.GetConfig(webMethod.wsSession, "SoapActionUrl");
                string theResponseFormat = Com.Mayaminer.IniFile.GetConfig(webMethod.wsSession, "ResponseFormat");
                string WebServiceSoapAction = SoapActionUrl + webMethod.webMethodName;

                System.Text.StringBuilder sb = new System.Text.StringBuilder();
                foreach (KeyValuePair<string, webParam> item in webMethod.paramList)
                {
                    sb.Append(string.IsNullOrEmpty(item.Value.paramValue) ? "<" + item.Value.paramName + "/>" : string.Format("<{0}>{1}</{0}>", item.Value.paramName, item.Value.paramValue));
                }
                string xml_str = string.Format("<{0} xmlns=\"{1}\">{2}</{0}>", webMethod.webMethodName, SoapActionUrl, sb.ToString());
                string xml_responses = Com.Mayaminer.APIConnector.CallWebService(WebServiceUrl, WebServiceSoapAction, xml_str);
                if (string.IsNullOrWhiteSpace(Com.Mayaminer.APIConnector.lastError))
                {
                    System.Xml.Linq.XDocument xd = System.Xml.Linq.XDocument.Parse(xml_responses);
                    string json_str = ""; 
                    json_str = xd.Descendants().First(d => d.Name.LocalName == webMethod.webMethodName + "Response").Value;
                    sr.datastatus = RCS_Data.HISDataStatus.SuccessWithData;
                    if (sr.datastatus == RCS_Data.HISDataStatus.SuccessWithData)
                    {
                        Com.Mayaminer.LogTool.SaveLogMessage(json_str, actionName, csName);
                        sr.datastatus = RCS_Data.HISDataStatus.SuccessWithData;
                        sr.errorMsg = json_str;
                    }
                }
                else
                {
                    sr.errorMsg = Com.Mayaminer.APIConnector.lastError;
                    sr.datastatus = RCS_Data.HISDataStatus.ExceptionError;
                } 
            }
            catch (Exception ex)
            {
                sr.datastatus = RCS_Data.HISDataStatus.ExceptionError;
                sr.errorMsg = ex.Message;
                Com.Mayaminer.LogTool.SaveLogMessage(ex, actionName, csName);
            }
            return sr;
        }


    }



    #region WebService 定義物件介面
    /// <summary>
    /// 回傳結果
    /// </summary>
    public class ServiceResult<T>
    {
        /// <summary>以代碼表示狀態</summary>
        public RCS_Data.HISDataStatus datastatus { set; get; }

        /// <summary>處理過程中的錯誤訊息</summary>
        public string errorMsg { set; get; }

        /// <summary>以代碼表示狀態</summary>
        public System.Data.DataTable returnList { set; get; }

        /// <summary>以代碼表示狀態</summary>
        public string returnJson { get { return Newtonsoft.Json.JsonConvert.SerializeObject(returnList); } }
    }

    /// <summary> webParam 
    /// <para>參數</para>
    /// </summary>
    public class webParam
    {
        /// <summary>
        /// 院方提供參數名稱
        /// </summary>
        public string paramName { get; set; }
        /// <summary>
        /// 參數值
        /// </summary>
        public string paramValue { get; set; }
    }

    /// <summary>
    /// WS介面定義
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IwebMethod<T>
    {
        IWebServiceParam iwp { get; set; }

        ServiceResult<T> ServiceResult { get; set; }
        /// <summary>
        /// 使用WS
        /// </summary>
        string wsSession { get; set; }
        /// <summary>
        /// 醫院代碼
        /// </summary>
        string hosp_id { get;  }

        /// <summary>
        /// 院方提供webMethod名稱
        /// </summary>
        string webMethodName { get; }
        /// <summary>
        /// 傳入參數paramList
        /// <para>設定傳入參數</para>
        /// </summary>
        Dictionary<string, webParam> paramList { get; set; }
        /// <summary>
        /// 回傳取得valueList
        /// <para>取得回傳資料</para>
        /// </summary>
        Dictionary<string, webParam> returnValue { get; set; }

        /// <summary>
        /// 設定傳入值以及回傳值
        /// </summary>
        void setParam();

        /// <summary>
        /// 回傳前設定
        /// </summary>
        void setReturnValue( );
 

    }

    /// <summary>
    /// webService 基本設定
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class AwebMethod<T>
    { 
        /// <summary>
        /// 醫院代碼
        /// </summary>
        public virtual string hosp_id { get { return Com.Mayaminer.IniFile.GetConfig("System", "HOSP_ID"); } }
        /// <summary>
        /// 介面
        /// </summary>
        public IWebServiceParam iwp { get; set; }

        public ServiceResult<T> ServiceResult { get; set; }

        public Dictionary<string, webParam> paramList { get; set; }

        public Dictionary<string, webParam> returnValue { get; set; }

        public virtual string wsSession { get; set; }

        public virtual void setReturnValue()
        {
            //如果有資料，對應系統資料結構
            if (this.ServiceResult.datastatus == RCS_Data.HISDataStatus.SuccessWithData)
            {
                foreach (DataRow dr in this.ServiceResult.returnList.Rows)
                {
                    foreach (DataColumn dc in this.ServiceResult.returnList.Columns)
                    {
                        if (!DBNull.ReferenceEquals(dr[dc.ColumnName],DBNull.Value))
                        {
                            dr[dc.ColumnName] = dr[dc.ColumnName].ToString().Trim();
                        }
                    }
                }
                foreach (KeyValuePair<string, webParam> item in this.returnValue)
                {
                    //如果有查詢到欄位資料，將欄位名稱取代
                    if (this.ServiceResult.returnList.Columns.Contains(item.Value.paramName))
                    {
                        this.ServiceResult.returnList.Columns[item.Value.paramName].ColumnName = item.Key;
                    }
                } 
                
            }
        } 
    }

    #endregion



}
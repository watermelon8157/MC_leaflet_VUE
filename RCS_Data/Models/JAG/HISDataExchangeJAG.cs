using RCSData.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RCSData.Models
{
    public class HISDataExchangeJAG : HISDataExchange
    {
        string csName { get { return "HISDataExchangeJAG"; } }

        /// <summary>
        /// 取得WS ServiceResult的資料
        /// </summary>
        /// <param name="webMethod"></param>
        /// <returns></returns>
        public ServiceResult<T> getEMRServiceResult<T>(IwebMethod<T> webMethod)
        {
            string actionName = "getServiceResult";
            try
            {
                if (webMethod.ServiceResult.datastatus == RCS_Data.HISDataStatus.SuccessWithoutData)
                {
                    webMethod.ServiceResult = setJAGEMRServiceResultFormat<T>(webMethod);
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

        private ServiceResult<T> setJAGEMRServiceResultFormat<T>(IwebMethod<T> webMethod)
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
                        switch (json_str)
                        {
                            case "-1":
                                sr.datastatus = RCS_Data.HISDataStatus.SuccessWithData;
                                break;
                            case "1":
                                sr.errorMsg = "無資料,指令內文或本文 Base64 字串為空值";
                                sr.datastatus = RCS_Data.HISDataStatus.ParametersError;
                                Com.Mayaminer.LogTool.SaveLogMessage(json_str, actionName, csName);
                                Com.Mayaminer.LogTool.SaveLogMessage(xml_str, actionName, csName);
                                break;
                            case "2":
                                sr.errorMsg = "本文檔格式不符(送簽檔不為 xml 或 pdf)";
                                sr.datastatus = RCS_Data.HISDataStatus.ParametersError;
                                Com.Mayaminer.LogTool.SaveLogMessage(json_str, actionName, csName);
                                Com.Mayaminer.LogTool.SaveLogMessage(xml_str, actionName, csName);
                                break;
                            case "3":
                                sr.errorMsg = "指令檔格式不符,指令內文 Tag 欄位有少或是 Tag 欄位為空值";
                                sr.datastatus = RCS_Data.HISDataStatus.ParametersError;
                                Com.Mayaminer.LogTool.SaveLogMessage(json_str, actionName, csName);
                                Com.Mayaminer.LogTool.SaveLogMessage(xml_str, actionName, csName);
                                break;
                            case "4":
                                sr.errorMsg = "其他錯誤";
                                sr.datastatus = RCS_Data.HISDataStatus.ParametersError;
                                Com.Mayaminer.LogTool.SaveLogMessage(json_str, actionName, csName);
                                Com.Mayaminer.LogTool.SaveLogMessage(xml_str, actionName, csName);
                                break;
                            default:
                                sr.errorMsg = "其他錯誤,請洽資訊人員!";
                                sr.datastatus = RCS_Data.HISDataStatus.ParametersError;
                                Com.Mayaminer.LogTool.SaveLogMessage(json_str, actionName, csName);
                                Com.Mayaminer.LogTool.SaveLogMessage(xml_str, actionName, csName);
                                break;
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

    }
}

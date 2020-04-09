using Newtonsoft.Json;
using RCS_Data.Controllers.Upload;
using RCSData.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

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
        public string UploadEMRFile<T>(IWebServiceParam iwp, T ijr ,string XMLRoot  , byte[] pdf )
        {
            string actioName = "UploadEMRFile";
            string OrderStr = "", Base64Str ="";
            ServiceResult<string> sr = new ServiceResult<string>();
            if (pdf == null)
            {
                sr.datastatus = RCS_Data.HISDataStatus.ParametersError;
                sr.errorMsg = "";
            }
            else
            { 
                XmlSerializer syphy_collect = new XmlSerializer(typeof(T));
                StringBuilder sb = new StringBuilder();
                // Remove XML declaration
                XmlWriterSettings settings = new XmlWriterSettings();
                settings.Indent = true;
                settings.OmitXmlDeclaration = true;
                XmlWriter writer = XmlWriter.Create(sb, settings);
                XmlSerializerNamespaces ns = new XmlSerializerNamespaces();
                ns.Add("", "");
                syphy_collect.Serialize(writer, ijr, ns);
                writer.Flush(); 
                string rawXml = sb.ToString().Replace("<" + XMLRoot + ">", "").Replace("</" + XMLRoot + ">", "").Replace(" ", "").Trim();
                OrderStr = string.Concat("<![CDATA[", rawXml, "]]>");
                Base64Str = Convert.ToBase64String(pdf); 
                WS_UploadEMRFile eo = new WS_UploadEMRFile(OrderStr, Base64Str, iwp);
                sr = HISDataJAG.getEMRServiceResult(eo);
            }
            this.datastatus = sr.datastatus;
            this.errorMsg = sr.errorMsg;
            if (sr.datastatus == RCS_Data.HISDataStatus.SuccessWithData)
            {
                return "-1";
            }
            return "";
        }
    }

    public class WS_UploadEMRFile : AwebMethod<string>, IwebMethod<string>
    {
        public string webMethodName { get { return this.iwp.webMethodName; } }
        public override string wsSession { get { return "RCS_WS_EMR"; } }


        /// <summary>
        /// WS_UploadEMRFile
        /// </summary>
        /// <param name="OrderStr"></param>
        /// <param name="Base64Str"></param>
        /// <param name="pIwp"></param>
        public WS_UploadEMRFile(string OrderStr, string Base64Str, IWebServiceParam pIwp )
        {
            this.iwp = pIwp;
            setParam();
            if (!string.IsNullOrWhiteSpace(OrderStr) && !string.IsNullOrWhiteSpace(Base64Str))
            {

                this.paramList["OrderStr"].paramValue = OrderStr;
                this.paramList["Base64Str"].paramValue = Base64Str;
            }
            else
            {
                this.ServiceResult.datastatus = RCS_Data.HISDataStatus.ParametersError;
                if (string.IsNullOrWhiteSpace(OrderStr))
                {
                    this.ServiceResult.errorMsg = "OrderStr不可為空值!";
                }
                if (string.IsNullOrWhiteSpace(Base64Str))
                {
                        this.ServiceResult.errorMsg = "Base64Str不可為空值!";
                }
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

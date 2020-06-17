using System;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace RCSData.Models
{
    public partial class WebMethod
    {
         
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

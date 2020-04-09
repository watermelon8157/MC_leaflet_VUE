using RCSData.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RCSData.Models.WebService
{ 
    public class CreateEMRSign : WebServiceParam
    {
        public override string webMethodName
        {
            get
            {
                return "CreateEMRSign";
            }
        }

        public CreateEMRSign()
        {
            this.paramList = new Dictionary<string, webParam>();
            this.paramList.Add("EncntNo", new webParam() { paramName = "EncntNo" });
            this.paramList.Add("HHISNum", new webParam() { paramName = "HHISNum" });
            this.paramList.Add("ApplyNO", new webParam() { paramName = "ApplyNO" });
            this.paramList.Add("SignID", new webParam() { paramName = "SignID" });
            this.paramList.Add("FormID", new webParam() { paramName = "FormID" });
            this.paramList.Add("FilePath", new webParam() { paramName = "FilePath" });
            this.paramList.Add("PDFFileName", new webParam() { paramName = "PDFFileName" });
            this.paramList.Add("SYSTEMID", new webParam() { paramName = "SYSTEMID" });

            this.returnValue = new Dictionary<string, webParam>();

        }
    }
}

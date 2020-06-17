using RCSData.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RCSData.Models.WebService
{ 
    public class UploadEMRFile : WebServiceParam
    {
        public override string webMethodName
        {
            get
            {
                return "UploadEMRFile";
            }
        }

        public UploadEMRFile()
        {
            this.paramList = new Dictionary<string, webParam>();
            this.paramList.Add("OrderStr", new webParam() { paramName = "OrderStr" });
            this.paramList.Add("Base64Str", new webParam() { paramName = "Base64Str" });

            this.returnValue = new Dictionary<string, webParam>(); 

        }
    }
}

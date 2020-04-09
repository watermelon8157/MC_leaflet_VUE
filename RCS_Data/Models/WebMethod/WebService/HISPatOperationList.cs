using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RCSData.Models.WebService
{
    public class HISPatOperationList : WebServiceParam
    {
        public override string webMethodName
        {
            get
            {
                return "getPatOperationList";
            }
        }
        public HISPatOperationList()
        {
            this.paramList = new Dictionary<string, webParam>();
            this.paramList.Add("ipdNo", new webParam() { paramName = "pIpdNo" });//住院序號
            this.paramList.Add("ChartNo", new webParam() { paramName = "pChartNo" });//住院序號

            this.returnValue = new Dictionary<string, webParam>();
            this.returnValue.Add("OP_DATE", new webParam() { paramName = "SurgeryDate" });//手術日期 
            this.returnValue.Add("OP_CODE", new webParam() { paramName = "SurgeryNo" });//手術序號
            this.returnValue.Add("DESCRIPTION", new webParam() { paramName = "SurgeryContent" });//手術內容
        }
    }
}
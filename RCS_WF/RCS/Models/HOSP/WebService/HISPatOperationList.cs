using System.Collections.Generic;
using RCSData.Models;

namespace RCS.Models.HOSP.WF.WebService
{
    public class HISPatOperationList : RCSData.Models.WebService.HISPatOperationList
    {
        public HISPatOperationList()
        {
            this.paramList = new Dictionary<string, webParam>();
            this.paramList.Add("ipdNo", new webParam() { paramName = "pIpdNo" });//住院序號
            this.paramList.Add("ChartNo", new webParam() { paramName = "pChartNo" });//住院序號

            this.returnValue = new Dictionary<string, webParam>();
            this.returnValue.Add("OP_DATE", new webParam() { paramName = "OP_DATE" });//手術日期 
            this.returnValue.Add("OP_TIME", new webParam() { paramName = "OP_TIME" });//手術日期 
            this.returnValue.Add("OP_CODE", new webParam() { paramName = "OP_CODE" });//手術序號
            this.returnValue.Add("DESCRIPTION", new webParam() { paramName = "DESCRIPTION" });//手術內容
            this.returnValue.Add("OP_END_DATE", new webParam() { paramName = "OP_END_DATE" });//手術日期 
            this.returnValue.Add("OP_END_TIME", new webParam() { paramName = "OP_END_TIME" });//手術日期 
        }
    }
}
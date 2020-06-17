using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RCSData.Models.WebService
{
    public class HISInputOutput :WebServiceParam
    {
        public override string webMethodName
        {
            get
            {
                return "getInputOutput";
            }
        }

        public HISInputOutput()
        {
            this.paramList = new Dictionary<string, webParam>();
            this.paramList.Add("chartNo", new webParam() { paramName = "pChartNo" });
            this.paramList.Add("ipdNo", new webParam() { paramName = "pIpdNo" });
            this.paramList.Add("sDate", new webParam() { paramName = "pSDate" });
            this.paramList.Add("eDate", new webParam() { paramName = "pEDate" });

            this.returnValue = new Dictionary<string, webParam>();
            this.returnValue.Add("EXAM_DATE", new webParam() { paramName = "EXAM_DATE" });// 
            this.returnValue.Add("IO_TYPE", new webParam() { paramName = "IO_TYPE" });// 
            this.returnValue.Add("IO_ITEM", new webParam() { paramName = "IO_ITEM" });// 
            this.returnValue.Add("RESULT", new webParam() { paramName = "RESULT" });// 
            this.returnValue.Add("UNIT", new webParam() { paramName = "UNIT" });// 
        }
    }
}
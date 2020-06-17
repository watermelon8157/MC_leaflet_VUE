using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RCSData.Models.WebService
{
    public class RCSConsultList :WebServiceParam
    {
        public override string webMethodName
        {
            get
            {
                return "getConsultList";
            }
        }

        public RCSConsultList()
        {
            this.paramList = new Dictionary<string, webParam>();
            this.paramList.Add("sDate", new webParam() { paramName = "pSDate" });
            this.paramList.Add("eDate", new webParam() { paramName = "pEDate" });

            this.returnValue = new Dictionary<string, webParam>();
            this.returnValue.Add("START_DATE", new webParam() { paramName = "START_DATE" });//會診日期 
            this.returnValue.Add("START_TIME", new webParam() { paramName = "START_TIME" });//會診時間
            this.returnValue.Add("BED_NO", new webParam() { paramName = "BED_NO" });//床號
            this.returnValue.Add("CHART_NO", new webParam() { paramName = "CHART_NO" });//病歷號 
            this.returnValue.Add("PT_NAME", new webParam() { paramName = "PT_NAME" });//病人姓名
            this.returnValue.Add("DIV_NO", new webParam() { paramName = "DIV_NO" });//科別代碼
            this.returnValue.Add("DIV_SHORT_NAME", new webParam() { paramName = "DIV_SHORT_NAME" });//科別簡稱 
            this.returnValue.Add("VS_NO", new webParam() { paramName = "VS_ID" });//醫師代碼
            this.returnValue.Add("DOCTOR_NAME", new webParam() { paramName = "DOCTOR_NAME" });//醫師姓名
            this.returnValue.Add("ADMIT_NO", new webParam() { paramName = "ADMIT_NO" });//住院編號
            this.returnValue.Add("Prescription", new webParam() { paramName = "PRESCRIPTION" });//處方

        }
    }
}
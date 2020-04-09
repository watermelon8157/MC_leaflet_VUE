using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RCSData.Models.WebService
{
    public class HISReportData : WebServiceParam
    {
        public override string webMethodName
        {
            get
            {
                return "getReportData";
            }
        }

        public HISReportData()
        {
            this.paramList = new Dictionary<string, webParam>();
            this.paramList.Add("chartNo", new webParam() { paramName = "pChartNo" });
            this.paramList.Add("ipdNo", new webParam() { paramName = "pIpdNo" });
            this.paramList.Add("sDate", new webParam() { paramName = "pSDate" });
            this.paramList.Add("eDate", new webParam() { paramName = "pEDate" });

            this.returnValue = new Dictionary<string, webParam>();
            this.returnValue.Add("SEQ", new webParam() { paramName = "ExamNo" });//檢查單號(需為一值)
            this.returnValue.Add("TYPE", new webParam() { paramName = "TYPE_ID" });//報告類型代碼(院內代碼) ==================> 。(先帶入空值)
            this.returnValue.Add("TYPE_NAM", new webParam() { paramName = "TYPE_NAME" });//檢查名稱(報告類型)
            this.returnValue.Add("STAT", new webParam() { paramName = "TYPE_STATUS" });//狀態代碼 ==================> 。(先帶入空值)
            this.returnValue.Add("STATNAME", new webParam() { paramName = "TYPE_STATUS_NAME" });//狀態  ==================> 。(先帶入空值)
            this.returnValue.Add("ORDERDR", new webParam() { paramName = "ORDERDR" });//開單醫師代碼(院內代碼) ==================> 。(先帶入空值)
            this.returnValue.Add("ORDERDRNAME", new webParam() { paramName = "ORDERDRNAME" });//開單醫師姓名 ==================> 。(先帶入空值)
            this.returnValue.Add("ORDERTIME", new webParam() { paramName = "ORDERTIME" });//開單日期。
            this.returnValue.Add("ENTERDR", new webParam() { paramName = "ENTERDR" });//登打報告醫師代碼(院內代碼) ==================> 。(先帶入空值)
            this.returnValue.Add("ENTERDRNAME", new webParam() { paramName = "ENTERDRNAME" });//登打報告醫師姓名 ==================> 。(先帶入空值)
            this.returnValue.Add("COMPLETTIME", new webParam() { paramName = "COMPLETTIME" });//完成日期 ==================> 。(先帶入空值)
            this.returnValue.Add("ENTERTIME", new webParam() { paramName = "ENTERTIME" });//登打日期 
            this.returnValue.Add("REPORT", new webParam() { paramName = "REPORT" });//檢查報告內容 
        }

    }
}
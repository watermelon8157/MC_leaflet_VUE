using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RCSData.Models.WebService
{
    public class HISPatientHistory : WebServiceParam
    {
        public override string webMethodName
        {
            get
            {
                return "getPatientHistory";
            }
        }
        public HISPatientHistory()
        {

            this.paramList = new Dictionary<string, webParam>();
            this.paramList.Add("ChartNo", new webParam() { paramName = "pChartNo" });//病歷號

            this.returnValue = new Dictionary<string, webParam>();
            this.returnValue.Add("InTime", new webParam() { paramName = "InTime" });//InTime
            this.returnValue.Add("indate", new webParam() { paramName = "DIAG_DATE" });//indate
            this.returnValue.Add("outdate", new webParam() { paramName = "PRE_DISCHARGE_DATE" });//outdate
            this.returnValue.Add("Description", new webParam() { paramName = "Description" });//Description
            this.returnValue.Add("IPD_NO", new webParam() { paramName = "IPD_NO" });//IPD_NO
            this.returnValue.Add("IpdFlag", new webParam() { paramName = "IpdFlag" });//IpdFlag
            this.returnValue.Add("CostCode", new webParam() { paramName = "CostCode" });//CostCode
            this.returnValue.Add("DeptName", new webParam() { paramName = "DeptName" });//DeptName
        }
    }
}
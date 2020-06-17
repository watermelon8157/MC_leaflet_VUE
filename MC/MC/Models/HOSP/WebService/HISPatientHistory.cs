using System.Collections.Generic;
using RCSData.Models;

namespace RCS.Models.HOSP.WF.WebService
{
    public class HISPatientHistory : RCSData.Models.WebService.HISPatientHistory
    {
        public HISPatientHistory()
        {

            this.paramList = new Dictionary<string, webParam>();
            this.paramList.Add("ChartNo", new webParam() { paramName = "pChartNo" });//病歷號

            this.returnValue = new Dictionary<string, webParam>();
            this.returnValue.Add("InTime", new webParam() { paramName = "InTime" });//InTime
            this.returnValue.Add("indate", new webParam() { paramName = "INDATE" });//indate
            this.returnValue.Add("outdate", new webParam() { paramName = "OUT_DATE" });//outdate 
            this.returnValue.Add("IPD_NO", new webParam() { paramName = "IPD_NO" });//IPD_NO 
            this.returnValue.Add("CostCode", new webParam() { paramName = "COSTCODE" });//CostCode
            this.returnValue.Add("DeptCode", new webParam() { paramName = "DEPTCODE" });//CostCode
            this.returnValue.Add("DeptName", new webParam() { paramName = "DEPTNAME" });//DeptName
        }
    }
}
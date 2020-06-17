using System.Collections.Generic;
using RCSData.Models;

namespace RCS.Models.HOSP.WF.WebService
{
    public class AreaPatientList : RCSData.Models.WebService.AreaPatientList 
    {
        public AreaPatientList()
        {
            this.paramList = new Dictionary<string, webParam>();
            this.paramList.Add("areaCode", new webParam() { paramName = "pAreaCode" });//住院序號

            this.returnValue = new Dictionary<string, webParam>();
            this.returnValue.Add("ipd_no", new webParam() { paramName = "IPD_NO" });//批價序號
            this.returnValue.Add("chart_no", new webParam() { paramName = "CHART_NO" });//病歷號
            this.returnValue.Add("bed_no", new webParam() { paramName = "BED_NO" });//床號
            this.returnValue.Add("patient_name", new webParam() { paramName = "PATIENT_NAME" });//患者姓名
        }
    }
}
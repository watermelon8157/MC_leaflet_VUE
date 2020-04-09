using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RCSData.Models.WebService
{
    public class AreaPatientList : WebServiceParam
    {
        public override string webMethodName
        {
            get
            {
                return "getAreaPatientList";
            }
        }
        public AreaPatientList()
        {
            this.paramList = new Dictionary<string, webParam>();
            this.paramList.Add("areaCode", new webParam() { paramName = "pAreaCode" });//住院序號

            this.returnValue = new Dictionary<string, webParam>();
            this.returnValue.Add("ipd_no", new webParam() { paramName = "ipd_no" });//批價序號
            this.returnValue.Add("chart_no", new webParam() { paramName = "ChrNo" });//病歷號
            this.returnValue.Add("bed_no", new webParam() { paramName = "BedNo" });//床號
            this.returnValue.Add("patient_name", new webParam() { paramName = "PatientName" });//患者姓名
        }
    }
}
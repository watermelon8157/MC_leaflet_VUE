using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RCSData.Models.WebService
{
    public class HISPatientInfo : WebServiceParam
    {
        public override string webMethodName
        {
            get
            {
                return "getPatientInfo";
            }
        }
        public HISPatientInfo()
        {
            this.paramList = new Dictionary<string, webParam>();
            this.paramList.Add("ipdNo", new webParam() { paramName = "pIpdNo" });//住院序號
            this.paramList.Add("chart_no", new webParam() { paramName = "pChartNo" });//住院序號

            this.returnValue = new Dictionary<string, webParam>();
            this.returnValue.Add("ipd_no", new webParam() { paramName = "FeeNo" });//批價序號
            this.returnValue.Add("chart_no", new webParam() { paramName = "ChartNo" });//病歷號
            this.returnValue.Add("bed_no", new webParam() { paramName = "BedNo" });//床號
            this.returnValue.Add("gender", new webParam() { paramName = "PatientGender" });//病人性別，男=”M”、女=”F”、其他=””
            this.returnValue.Add("source_type", new webParam() { paramName = "source_type" });//病人來源，急診=”E”、門診=”O”、住院=”I”
            this.returnValue.Add("patient_name", new webParam() { paramName = "PatientName" });//患者姓名
            this.returnValue.Add("birth_day", new webParam() { paramName = "Birth_date" });//生日
            this.returnValue.Add("vs_doc", new webParam() { paramName = "DocName" });//主治醫師姓名
            this.returnValue.Add("vs_id", new webParam() { paramName = "DocNo" });//主治醫師醫事人員編號
            this.returnValue.Add("diag_date", new webParam() { paramName = "InDate" });//入院日期
            this.returnValue.Add("diagnosis_code", new webParam() { paramName = "ICD9_code1" });//主診斷碼
            this.returnValue.Add("dept_code", new webParam() { paramName = "DeptNo" });//科別代碼
            this.returnValue.Add("dept_desc", new webParam() { paramName = "DeptName" });//科別名稱
            this.returnValue.Add("cost_code", new webParam() { paramName = "CostCenterNo" });//成本中心代碼
            this.returnValue.Add("cost_desc", new webParam() { paramName = "CostCenterName" });//成本中心名稱
            this.returnValue.Add("patId", new webParam() { paramName = "PatientID" });//病患身份證字號
            this.returnValue.Add("dnr_mark", new webParam() { paramName = "dnr_mark" });//是否為DNR病患(1:是，0否，預設為:否)
        }
    }
}
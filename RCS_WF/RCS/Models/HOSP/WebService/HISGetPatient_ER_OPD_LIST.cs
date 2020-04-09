using System.Collections.Generic;
using RCSData.Models;

namespace RCS.Models.HOSP.WF.WebService
{
    public class HISGetPatient_ER_OPD_LIST : RCSData.Models.WebService.HISGetPatient_ER_OPD_LIST
    {
        public override bool WSIsOK { get { return true; } }

        public HISGetPatient_ER_OPD_LIST()
        { 
            this.paramList = new Dictionary<string, webParam>();
            this.paramList.Add("ChartNo", new webParam() { paramName = "pChartNo" });//住院序號

            this.returnValue = new Dictionary<string, webParam>();
            this.returnValue.Add("ipd_no", new webParam() { paramName = "IPD_NO" });//批價序號
            this.returnValue.Add("chart_no", new webParam() { paramName = "CHART_NO" });//病歷號
            this.returnValue.Add("bed_no", new webParam() { paramName = "BED_NO" });//床號
            this.returnValue.Add("gender", new webParam() { paramName = "GENDER" });//病人性別，男=”M”、女=”F”、其他=””
            this.returnValue.Add("source_type", new webParam() { paramName = "SOURCE_TYPE" });//病人來源，急診=”E”、門診=”O”、住院=”I”
            this.returnValue.Add("patient_name", new webParam() { paramName = "PATIENT_NAME" });//患者姓名
            this.returnValue.Add("birth_day", new webParam() { paramName = "BIRTH_DATE" });//生日
            this.returnValue.Add("diag_date", new webParam() { paramName = "DIAG_DATE" });//入院日期
            this.returnValue.Add("diagnosis_code", new webParam() { paramName = "DIAGNOSIS_DESC" });//主診斷碼
            this.returnValue.Add("idno", new webParam() { paramName = "PATID" });//身分證字號
            this.returnValue.Add("vs_doc", new webParam() { paramName = "VS_NAME" });// 
            this.returnValue.Add("vs_id", new webParam() { paramName = "VS_ID" });// 
        }
    }
}
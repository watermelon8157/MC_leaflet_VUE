using System.Collections.Generic;
using RCSData.Models;

namespace RCS.Models.HOSP.WF.WebService
{
    public class HISPatientInfo : RCSData.Models.WebService.HISPatientInfo
    {
        public HISPatientInfo()
        {
            this.paramList = new Dictionary<string, webParam>();
            this.paramList.Add("ipdNo", new webParam() { paramName = "pIpdNo" });//住院序號
            this.paramList.Add("chart_no", new webParam() { paramName = "pChartNo" });//住院序號

            this.returnValue = new Dictionary<string, webParam>();
            this.returnValue.Add("ipd_no", new webParam() { paramName = "IPD_NO" });//批價序號
            this.returnValue.Add("chart_no", new webParam() { paramName = "CHART_NO" });//病歷號
            this.returnValue.Add("bed_no", new webParam() { paramName = "BED_NO" });//床號
            this.returnValue.Add("gender", new webParam() { paramName = "GENDER" });//病人性別，男=”M”、女=”F”、其他=””
            // this.returnValue.Add("source_type", new webParam() { paramName = "SOURCE_TYPE" });//病人來源，急診=”E”、門診=”O”、住院=”I”
            this.returnValue.Add("patient_name", new webParam() { paramName = "PATIENT_NAME" });//患者姓名
            this.returnValue.Add("birth_day", new webParam() { paramName = "BIRTH_DAY" });//生日
            this.returnValue.Add("vs_doc", new webParam() { paramName = "VS_NAME" });//主治醫師姓名
            this.returnValue.Add("vs_id", new webParam() { paramName = "VS_ID" });//主治醫師醫事人員編號
            this.returnValue.Add("diag_date", new webParam() { paramName = "DIAG_DATE" });//入院日期
            this.returnValue.Add("diagnosis_code", new webParam() { paramName = "DIAGNOSIS_DESC" });//主診斷碼
            this.returnValue.Add("dept_code", new webParam() { paramName = "DEPT_CODE" });//科別代碼
            this.returnValue.Add("dept_desc", new webParam() { paramName = "DEP_DESC" });//科別名稱
            this.returnValue.Add("cost_code", new webParam() { paramName = "COST_CODE" });//成本中心代碼
            this.returnValue.Add("cost_desc", new webParam() { paramName = "COST_DESC" });//成本中心名稱
            this.returnValue.Add("patId", new webParam() { paramName = "PATID" });//病患身份證字號
            this.returnValue.Add("dnr_mark", new webParam() { paramName = "DNR_MARK" });//是否為DNR病患(1:是，0否，預設為:否)
            this.returnValue.Add("pre_discharge_date", new webParam() { paramName = "DISCHARGE_DATE" });//pre_discharge_date
            this.returnValue.Add("body_height", new webParam() { paramName = "ADMISSION_HEIGHT" });//入院身高
            this.returnValue.Add("BSA", new webParam() { paramName = "BSA" });//體表面積(入院)
            this.returnValue.Add("BLOOD_TYPE", new webParam() { paramName = "BLOOD_TYPE" });  
            this.returnValue.Add("ICU_BODY_WEIGHT", new webParam() { paramName = "ICU_BODY_WEIGHT" });//入ICU體重 
            this.returnValue.Add("CLINICAL_DIAGNOSIS", new webParam() { paramName = "CLINICAL_DIAGNOSIS" });//臨床診斷 
            this.returnValue.Add("SMOKES", new webParam() { paramName = "SMOKES" });//是否抽菸 1:是,0:否 
            this.returnValue.Add("MDRO_MARK", new webParam() { paramName = "MDRO_MARK" });//MDRO
        }
    }
}
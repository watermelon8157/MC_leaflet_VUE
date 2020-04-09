using RCS_Data.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RCS_Data.Models.ViewModels
{
    /// <summary>
    /// CPT 紀錄單欄位名稱(cpt_item)
    /// </summary>
    public class RTCptReq 
    {
        /// <summary> 入院日期 </summary>
        public string diag_date { set; get; }
        /// <summary>
        /// 填單日期
        /// </summary>
        public string rec_date { set; get; }
        public string record_id { set; get; }
        public string now_diagnosis_display { set; get; }
        public string CXR_key { get; set; }
        public string CXR_result_json { get; set; } //Cxr繪圖物件 2018.08.25 (Save儲存功能OK，Get讀取有錯誤) CxrResultJson_cls
        public string CxrImageBase64_str { get; set; }
        public string PDF_CXR_Date_Str { get; set; }
        public string PDF_CXR_Result_Str { get; set; }
        /// <summary> 主治醫師姓名 </summary>
        public string vs_doc { set; get; }
        /// <summary> 填單醫師姓名 </summary>
        public string set_doc { set; get; }
        /// <summary> 身高 </summary>
        public string height { set; get; }
        /// <summary> 體重 </summary>
        public string weight { set; get; }
        public string gcs { set; get; }
        /// <summary>
        /// 最近一次X-RAY
        /// </summary>
        public string X_Ray_date { get; set;}
        public string cpt_check { get; set; }

        public string Percussor_check { get; set; }

        public string HFCWO_check { get; set; }

        public string Bronchial_otrher { get; set; }

        public string cpt_strengthen_part { get; set; }

        public string Past_medical_history { get; set; }
        public string Past_medical_history_other { get; set; }

        public string cpt_forbidden_treatment { get; set; }
        public string cpt_days { get; set; }
        public string ippb_check { get; set; }
        public string ippb_conscious { get; set; }
        public string ippb_inhalation { get; set; }
        public string ippb_hours { get; set; }
        public string ippb_days { get; set; }
        public string ippb_days_2 { get; set; }
        public string Negative_Pressure_Ventilation { get; set; }
        public string Negative_inhalation { get; set; }
        public string Negative_hours { get; set; }
        public string Negative_days { get; set; }
        public string Negative_days_2 { get; set; }

        public string reconditioning_exercise { get; set; }
        public string reconditioning_exercise_days { get; set; }
        public string reconditioning_exercise_days_2 { get; set; }


        public string Sand_Bag_Training { get; set; }
        public string Sand_Bag_Training_inhalation { get; set; }
        public string Sand_Bag_Training_hours { get; set; }

        public string Cough_Machine { get; set; }
        public string Cough_inhalation { get; set; }
        public string Cough_hours { get; set; }
        public string Cough_days { get; set; }
        public string Cough_days_2 { get; set; }

        public string SV_Training { get; set; }
        public string SV_Training_inhalation { get; set; }

        public string triflow_check { get; set; }
        public string triflow_date { get; set; }
        public string speaking_valve_check { get; set; }
        public string reconditioning_string { get; set; }
        public string CMIET_check { get; set; }
        public string Other_check { get; set; }
        public string IS { get; set; }
        public string surgery_date { get; set; }
        public string cpt { get; set; }
        public string cpt_days_2 { get; set; }
        public string Other { get; set; }
        public string String_date { get; set; }
        public string End_date { get; set; }
        public string CREATE_NAME { get; set; }
        public string Remark { get; set; }
    }
}

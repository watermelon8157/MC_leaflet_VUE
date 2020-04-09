using Newtonsoft.Json;
using RCS_Data.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RCS_Data.Models.ViewModels
{
    public class RCS_RTTakeoff
    {
        /// <summary> 入院日期 </summary>
        public string diag_date { set; get; }
        /// <summary> 主檔編號 </summary>
        public string tk_id { set; get; }
        /// <summary> 記錄日期 </summary>
        public string rec_date { get; set; }
        /// <summary> 記錄人員 </summary>
        public string create_name { get; set; }
        /// <summary> 記錄人員ID </summary>
        public string create_id { get; set; }

        public string DATASTATUS { get; set; }
        /// <summary>   </summary>
        public string UPLOAD_STATUS { get; set; }
        /// <summary> 記錄人員ID </summary>
        public string UPLOAD_ID { get; set; }

        /// <summary> 呼吸器使用模式 model </summary>
        public string rt_start_model { set; get; } 
        /// <summary> 呼吸器使用模式 FIO2</summary>
        public string rt_start_fio2 { set; get; }
        /// <summary> 呼吸器使用模式 PEEP</summary>
        public string rt_start_PEEP { set; get; }
        /// <summary> 呼吸器使用模式 VT/PC</summary>
        public string rt_start_VT_PC { set; get; }
        public string rt_start_PC { set; get; }
        /// <summary> 呼吸器使用模式 PS</summary>
        public string rt_start_PS { set; get; }
        /// <summary> 呼吸器使用模式 RR</summary>
        public string rt_start_RR { set; get; }
        /// <summary> 呼吸器使用模式 IPAP</summary>
        public string rt_start_IPAP { set; get; }
        /// <summary> 呼吸器使用模式 EPAP</summary>
        public string rt_start_EPAP { set; get; }
        /// <summary> 呼吸器使用模式 phigh</summary>
        public string rt_start_phigh { set; get; }
        /// <summary> 呼吸器使用模式 plow</summary>
        public string rt_start_plow { set; get; }
        /// <summary> 呼吸器使用模式 thigh</summary>
        public string rt_start_thigh { set; get; }
        /// <summary> 呼吸器使用模式 tlow</summary>
        public string rt_start_tlow { set; get; }
        /// <summary> 呼吸器使用模式 delta_p</summary>
        public string rt_start_delta_p { set; get; }
        /// <summary> 呼吸器使用模式 hfov_mean</summary>
        public string rt_start_hfov_mean { set; get; }
        /// <summary> 呼吸器使用模式 hfov_hz</summary>
        public string rt_start_hfov_hz { set; get; }


        #region 病患評估
        /// <summary>
        /// 病患主要呼吸問題
        /// </summary>
        public string breathingQuestion_data { get; set; }
        /// <summary>
        /// 胸腔病史
        /// </summary>
        public string chest_history_data { get; set; }

        /// <summary>
        /// 意識
        /// </summary>
        public string conscious_data { get; set; }
        /// <summary>
        /// 皮膚
        /// </summary>
        public string skin_data { get; set; }
        /// <summary>
        /// 呼吸型態(病患自然呼吸狀態)
        /// </summary>
        public string breathing_patterns_data { get; set; }
        /// <summary>
        /// 呼吸音
        /// </summary>
        public string breath_sounds_data { get; set; }
        /// <summary>
        /// 咳嗽能力
        /// </summary>
        public string cough_data { get; set; }        
        /// <summary>
        /// 其他藥物使用
        /// </summary>
        public string other_drugs_data { get; set; }

        #endregion

        #region 呼吸治療項目
        /// <summary>
        /// 噴霧治療
        /// </summary>
        public string spray_treatment_data { get; set; }        
        /// <summary>
        /// 目前呼吸器使用情形_Mode
        /// </summary>
        public string now_breathing_patterns_Mode { get; set; }
        /// <summary>
        /// 目前呼吸器使用情形_Tidal volume
        /// </summary>
        public string now_breathing_patterns_Tidalvolume { get; set; }
        /// <summary>
        /// 目前呼吸器使用情形_RR set
        /// </summary>
        public string now_breathing_patterns_RRset{ get; set; }
        /// <summary>
        /// 目前呼吸器使用情形_RR total
        /// </summary>
        public string now_breathing_patterns_RRtotal { get; set; }
        /// <summary>
        /// 目前呼吸器使用情形_MV set
        /// </summary>
        public string now_breathing_patterns_MVset { get; set; }
        /// <summary>
        /// 目前呼吸器使用情形_MV total
        /// </summary>
        public string now_breathing_patterns_MVtotal { get; set; }
        /// <summary>
        /// 目前呼吸器使用情形_Pressure peak
        /// </summary>
        public string now_breathing_patterns_Ppeak { get; set; }
        /// <summary>
        /// 目前呼吸器使用情形_Pressure plateau
        /// </summary>
        public string now_breathing_patterns_Pplateau { get; set; }
        /// <summary>
        /// 目前呼吸器使用情形_Mean
        /// </summary>
        public string now_breathing_patterns_Mean { get; set; }
        /// <summary>
        /// 目前呼吸器使用情形_PEEP
        /// </summary>
        public string now_breathing_patterns_PEEP { get; set; }
        /// <summary>
        /// 目前呼吸器使用情形_PS
        /// </summary>
        public string now_breathing_patterns_PS { get; set; }
        /// <summary>
        /// 目前呼吸器使用情形_PC
        /// </summary>
        public string now_breathing_patterns_PC { get; set; }
        /// <summary>
        /// 目前呼吸器使用情形_FiO2
        /// </summary>
        public string now_breathing_patterns_FiO2 { get; set; }
        /// <summary>
        /// 目前呼吸器使用情形_Raw
        /// </summary>
        public string now_breathing_patterns_Raw { get; set; }
        /// <summary>
        /// 目前呼吸器使用情形_Compliance
        /// </summary>
        public string now_breathing_patterns_Compliance { get; set; }
        /// <summary>
        /// 胸腔復健治療
        /// </summary>
        public string cpt_treatment_data { get; set; }
        #endregion

        #region 呼吸器脫離困難原因

        /// <summary>
        /// Unstable vital signs
        /// </summary>
        public string Unstable_vital_signs_data { get; set; }
        /// <summary>
        /// Gas exchange
        /// </summary>
        public string gas_exchange_data { get; set; }
        /// <summary>
        /// underlying_disease
        /// </summary>
        public string underlying_disease_data { get; set; }
        /// <summary>
        /// weaning_drug
        /// </summary>
        public string weaning_drug_data { get; set; }
        /// <summary>
        /// poor_respiratory_drive
        /// </summary>
        public string poor_respiratory_drive_data { get; set; }
        /// <summary>
        /// poor_respirator_muscle
        /// </summary>
        public string poor_respirator_muscle_data { get; set; }
        /// <summary>
        /// poor_cough_fuction
        /// </summary>
        public string poor_cough_fuction_data { get; set; }
        /// <summary>
        /// malnutrition
        /// </summary>
        public string malnutrition_data { get; set; }
        /// <summary>
        /// weaning_other
        /// </summary>
        public string weaning_other_data { get; set; }
        public string weaning_other_data_txt { get; set; }

        //2019/12/11新增
        public string respiration_related_data { get; set; }
        public string respiration_related_other_txext { get; set; }
        public string circulation_related_data { get; set; }
        public string infection_related_data { get; set; }
        public string neural_muscular_related_data { get; set; }
        public string GI_related_data { get; set; }
        public string nutrition_related_data { get; set; }
        public string weaning_program_others_data { get; set; }
        #endregion

        #region 呼吸器脫離計畫

        /// <summary>
        /// try_weaning_mode
        /// </summary>
        public string try_weaning_mode_data { get; set; }
        /// <summary>
        /// try_weaning_mode SIMV
        /// </summary>
        public string try_weaning_mode_data_SIMV { get; set; }
        /// <summary>
        /// try_weaning_mode Try PS
        /// </summary>
        public string try_weaning_mode_data_TryPS { get; set; }
        /// <summary>
        /// try_weaning_mode Try PS
        /// </summary>
        public string try_weaning_mode_data_TryCPAP { get; set; }
        /// <summary>
        /// try_t_piece
        /// </summary>
        public string try_t_piece_data { get; set; }
        /// <summary>
        /// try_t_piece_over_night
        /// </summary>
        public string try_t_piece_over_night_data { get; set; }
        /// <summary>
        /// 呼吸器脫離計畫 其他
        /// </summary>
        public string weaning_plan_other_data { get; set; }

        #endregion               

        public string try_breath_reason_data { get; set; }

        public string isweaning_data { get; set; }

        public string body_height { get; set; }
        /// <summary>
        /// 體重kg
        /// </summary>
        public string body_weight { get; set; }
        /// <summary>
        /// 體重g
        /// </summary>
        public string body_weight_g { get; set; }
        /// <summary>
        /// 理想體重
        /// </summary>
        public string f_body_weight { get; set; }
        /// <summary>
        /// 呼吸器使用天數
        /// </summary>
        public string use_days_how { get; set; }

        public string goal_of_weaning_program { get; set; }
        //2019/12/11新增
        public string discontinuation_of_MV { get; set; }
        public string discontinuation_of_MV_mode_txext { get; set; }
        public string discontinuation_of_MV_low_PSV_txext { get; set; }
        public string discontinuation_of_MV_CPAP_mode_txext { get; set; }
        public string discontinuation_of_MV_TPiece_txext { get; set; }
        public string discontinuation_of_MV_Trmask_txext { get; set; }
        public string discontinuation_of_MV_oxygen_therapy_txext { get; set; }
        public string discontinuation_of_MV_other_txext { get; set; }
        public string chest_physica_therapy { get; set; }        
        public string chest_physica_therapy_percussion_txext { get; set; }
        public string chest_physica_therapy_percussor_txext { get; set; }
        public string chest_physica_therapy_VEST_txext { get; set; }
        public string chest_physica_therapy_other_txext { get; set; }
        public string pulmonary_recondition { get; set; }
        public string pulmonary_recondition_sandbag_txext { get; set; }
        public string pulmonary_recondition_binder_txext { get; set; }
        public string pulmonary_recondition_IPPB_txext { get; set; }
        public string pulmonary_recondition_IS_txext { get; set; }
        public string pulmonary_recondition_speaking_valve_txext { get; set; }
        public string pulmonary_recondition_exercise_training { get; set; }
        public string pulmonary_recondition_exercise_training_txext { get; set; }
        public string pulmonary_recondition_NPV_txext { get; set; }
        public string approved_respiratory_weaning_progra_others { get; set; }
        public string present_ventilator_setting_data { get; set; }
        /// <summary> 呼吸器開始使用日期 </summary>
        public string on_breath_date { get; set; }
        /// <summary> Weaning Profile </summary>
        public string weaningTable_data { get; set; }
        public string labDataTable_data { get; set; }

        // 2020/01/02新增
        /// <summary> 藥物 SVN </summary>
        public string drug_SVN_data { get; set; }
        /// <summary> 藥物_SVN other </summary>
        public string drug_svn_other { get; set; }
        /// <summary> 藥物 MDI/DPI </summary>
        public string drug_MDIDPI_data { get; set; }
        /// <summary> 藥物_MDI/DPI other </summary>
        public string drug_mdidpi_other { get; set; }
        /// <summary> 意識_E </summary>
        public string conscious_E { get; set; }
        /// <summary> 意識_V </summary>
        public string conscious_V { get; set; }
        /// <summary> 意識_M </summary>
        public string conscious_M { get; set; }
        /// <summary> 痰液_色 </summary>
        public string sputum_color { get; set; }
        /// <summary> 痰液_量 </summary>
        public string sputum_quantity { get; set; }
        /// <summary> 痰液_質 </summary>
        public string sputum_quality { get; set; }
        /// <summary> 呼吸器脫離困難原因 airway_lung </summary>
        public string airway_lung_data { get; set; }
        /// <summary> 呼吸器脫離困難原因 brain_neurological </summary>
        public string brain_neurological_data { get; set; }
        /// <summary> 呼吸器脫離困難原因 cardiac </summary>
        public string cardiac_data { get; set; }
        /// <summary> 呼吸器脫離困難原因 diaphragm_respiratory_muscle_function </summary>
        public string diaphragm_respiratory_muscle_function_data { get; set; }
        /// <summary> 呼吸器脫離困難原因 metabolic_status </summary>
        public string metabolic_status_data { get; set; }
        /// <summary> 呼吸器脫離困難原因 Abnormal endocrine </summary>
        public string abnormal_endocrine_data { get; set; }
        /// <summary> 呼吸器脫離困難原因 Abnormal electrolyte</summary>
        public string abnormal_electrolyte_data { get; set; }
        /// <summary> 呼吸器脫離困難原因 infection </summary>
        public string infection_data { get; set; }
        /// <summary> 呼吸器脫離困難原因 infection Lung</summary>
        public string infection_lung_data { get; set; }
        /// <summary> 呼吸器脫離困難原因 infection Urinary tract</summary>
        public string infection_urinary_tract_data { get; set; }
        /// <summary> 呼吸器脫離困難原因 infection Blood</summary>
        public string infection_blood_data { get; set; }
        /// <summary> 呼吸器脫離困難原因 infection Others</summary>
        public string infection_others_data { get; set; }
        /// <summary> 呼吸器脫離困難原因 nutrition </summary>
        public string nutrition_data { get; set; }
        /// <summary> 呼吸器脫離計畫 Control underlying disease </summary>
        public string control_underlying_disease_data { get; set; }
        /// <summary> 呼吸器脫離計畫 Control mode → SIMV≦10次/min </summary>
        public string control_mode_data { get; set; }
        /// <summary> 呼吸器脫離計畫 Try weaning mode SIMV→PS </summary>
        public string simv_ps_data { get; set; }
        /// <summary> 呼吸器脫離計畫 Try weaning mode SIMV→ASV </summary>
        public string simv_asv_data { get; set; }
        /// <summary> 呼吸器脫離計畫 Try PS cmH2O </summary>
        public string try_ps_cmh2o_data { get; set; }
        /// <summary> 呼吸器脫離計畫 Try PS 小時/天 </summary>
        public string try_ps_hr_data { get; set; }
        /// <summary> 呼吸器脫離計畫 Try CPAP cmH2O </summary>
        public string try_cpap_cmh2o_data { get; set; }
        /// <summary> 呼吸器脫離計畫 Try CPAP 小時/天 </summary>
        public string try_cpap_hr_data { get; set; }
        /// <summary> 呼吸器脫離計畫 Try T-piece (or Collar) 小時/天 </summary>
        public string try_t_piece_hr_data { get; set; }
        /// <summary> 呼吸器脫離計畫 執行復健運動  次/天</summary>
        public string try_t_piece_over_night_day_data { get; set; }
        
    }

    #region 尚未整理
    // TODO:尚未整理

    /// <summary>
    /// DB儲存項目 For RCS_WEANING_ASSESS_DTL
    /// </summary>
    public class VM_RTTakeoffAssess : RCS_RTTakeoff
    {

        #region 病患基本資料
        /// <summary>
        /// 病歷號
        /// </summary>
        public string chart_no { get; set; }

        /// <summary>
        /// 床號
        /// </summary>
        public string bed_no { get; set; }
        /// <summary>
        /// 姓名
        /// </summary>
        public string patient_name { get; set; }
        /// <summary>
        /// 性別
        /// </summary>
        public string genderCHT { get; set; }
        /// <summary>
        /// 出生
        /// </summary>
        public string birth_day { get; set; }
        /// <summary>
        /// 年齡
        /// </summary>
        public int age
        {
            get
            {
                DateTime dt = DateTime.Now;
                bool db_psuccess = DateTime.TryParse(this.birth_day, out dt);
                if (db_psuccess)
                {
                    int d = 0;
                    return int.TryParse(((DateTime.Now - dt).Days / 365).ToString(), out d) ? d : 0;
                }
                else
                    return -1;
            }
        }
        /// <summary>
        /// 轉入日期
        /// </summary>
        public string tran_date { get; set; }
        /// <summary>
        /// 使用呼吸器原因
        /// </summary>
        public string use_breath_reason { get; set; }

        /// <summary>
        /// 使用呼吸器原因(其他)
        /// </summary>
        public string use_breath_reason_txt { get; set; }


        /// <summary>
        /// 診斷
        /// </summary>
        public string diagnosis_code { get; set; }
        /// <summary>
        /// 入院主訴
        /// </summary>
        public string in_hosp_complaint { get; set; }

        #endregion

        #region 病患評估

        public string _breathingQuestion
        {
            get { return JsonConvert.SerializeObject(breathingQuestion); }
            set
            {
                if (!string.IsNullOrWhiteSpace(value))
                {
                    breathingQuestion = JsonConvert.DeserializeObject<List<JSON_DATA>>(value);
                }
            }
        }

        public string _chest_history
        {
            get { return JsonConvert.SerializeObject(chest_history); }
            set
            {
                if (!string.IsNullOrWhiteSpace(value))
                {
                    chest_history = JsonConvert.DeserializeObject<List<JSON_DATA>>(value);
                }
            }
        }


        public string _conscious
        {
            get { return JsonConvert.SerializeObject(conscious); }
            set
            {
                if (!string.IsNullOrWhiteSpace(value))
                {
                    conscious = JsonConvert.DeserializeObject<List<JSON_DATA>>(value);
                }
            }
        }

        public string _skin
        {
            get { return JsonConvert.SerializeObject(skin); }
            set
            {
                if (!string.IsNullOrWhiteSpace(value))
                {
                    skin = JsonConvert.DeserializeObject<List<JSON_DATA>>(value);
                }
            }
        }



        public string _breathing_patterns
        {
            get { return JsonConvert.SerializeObject(breathing_patterns); }
            set
            {
                if (!string.IsNullOrWhiteSpace(value))
                {
                    breathing_patterns = JsonConvert.DeserializeObject<List<JSON_DATA>>(value);
                }
            }
        }


        public string _breath_sounds
        {
            get { return JsonConvert.SerializeObject(breath_sounds); }
            set
            {
                if (!string.IsNullOrWhiteSpace(value))
                {
                    breath_sounds = JsonConvert.DeserializeObject<List<JSON_DATA>>(value);
                }
            }
        }



        public string _cough
        {
            get { return JsonConvert.SerializeObject(cough); }
            set
            {
                if (!string.IsNullOrWhiteSpace(value))
                {
                    cough = JsonConvert.DeserializeObject<List<JSON_DATA>>(value);
                }
            }
        }

        public string _sputum
        {
            get { return JsonConvert.SerializeObject(sputum); }
            set
            {
                if (!string.IsNullOrWhiteSpace(value))
                {
                    sputum = JsonConvert.DeserializeObject<List<JSON_DATA>>(value);
                }
            }
        }

        public string _other_drugs
        {
            get { return JsonConvert.SerializeObject(other_drugs); }
            set
            {
                if (!string.IsNullOrWhiteSpace(value))
                {
                    other_drugs = JsonConvert.DeserializeObject<List<JSON_DATA>>(value);
                }
            }
        }

        #endregion

        #region 呼吸治療項目

        public string _spray_treatment
        {
            get { return JsonConvert.SerializeObject(spray_treatment); }
            set
            {
                if (!string.IsNullOrWhiteSpace(value))
                {
                    spray_treatment = JsonConvert.DeserializeObject<List<JSON_DATA>>(value);
                }
            }
        }

        public string _drug
        {
            get { return JsonConvert.SerializeObject(drug); }
            set
            {
                if (!string.IsNullOrWhiteSpace(value))
                {
                    drug = JsonConvert.DeserializeObject<List<JSON_DATA>>(value);
                }
            }
        }

        public string _now_breathing_patterns
        {
            get { return JsonConvert.SerializeObject(now_breathing_patterns); }
            set
            {
                if (!string.IsNullOrWhiteSpace(value))
                {
                    now_breathing_patterns = JsonConvert.DeserializeObject<List<JSON_DATA>>(value);
                }
            }
        }

        public string _cpt_treatment
        {
            get { return JsonConvert.SerializeObject(cpt_treatment); }
            set
            {
                if (!string.IsNullOrWhiteSpace(value))
                {
                    cpt_treatment = JsonConvert.DeserializeObject<List<JSON_DATA>>(value);
                }
            }
        }

        #endregion

        #region 呼吸器脫離困難原因

        public string _Unstable_vital_signs
        {
            get { return JsonConvert.SerializeObject(Unstable_vital_signs); }
            set
            {
                if (!string.IsNullOrWhiteSpace(value))
                {
                    Unstable_vital_signs = JsonConvert.DeserializeObject<List<JSON_DATA>>(value);
                }
            }
        }


        public string _gas_exchange
        {
            get { return JsonConvert.SerializeObject(gas_exchange); }
            set
            {
                if (!string.IsNullOrWhiteSpace(value))
                {
                    gas_exchange = JsonConvert.DeserializeObject<List<JSON_DATA>>(value);
                }
            }
        }



        public string _underlying_disease
        {
            get { return JsonConvert.SerializeObject(underlying_disease); }
            set
            {
                if (!string.IsNullOrWhiteSpace(value))
                {
                    underlying_disease = JsonConvert.DeserializeObject<List<JSON_DATA>>(value);
                }
            }
        }



        public string _weaning_drug
        {
            get { return JsonConvert.SerializeObject(weaning_drug); }
            set
            {
                if (!string.IsNullOrWhiteSpace(value))
                {
                    weaning_drug = JsonConvert.DeserializeObject<List<JSON_DATA>>(value);
                }
            }
        }

        public string _poor_respiratory_drive
        {
            get { return JsonConvert.SerializeObject(poor_respiratory_drive); }
            set
            {
                if (!string.IsNullOrWhiteSpace(value))
                {
                    poor_respiratory_drive = JsonConvert.DeserializeObject<List<JSON_DATA>>(value);
                }
            }
        }



        public string _poor_respirator_muscle
        {
            get { return JsonConvert.SerializeObject(poor_respirator_muscle); }
            set
            {
                if (!string.IsNullOrWhiteSpace(value))
                {
                    poor_respirator_muscle = JsonConvert.DeserializeObject<List<JSON_DATA>>(value);
                }
            }
        }


        public string _poor_cough_fuction
        {
            get { return JsonConvert.SerializeObject(poor_cough_fuction); }
            set
            {
                if (!string.IsNullOrWhiteSpace(value))
                {
                    poor_cough_fuction = JsonConvert.DeserializeObject<List<JSON_DATA>>(value);
                }
            }
        }


        public string _malnutrition
        {
            get { return JsonConvert.SerializeObject(malnutrition); }
            set
            {
                if (!string.IsNullOrWhiteSpace(value))
                {
                    malnutrition = JsonConvert.DeserializeObject<List<JSON_DATA>>(value);
                }
            }
        }

        public string _weaning_other
        {
            get { return JsonConvert.SerializeObject(weaning_other); }
            set
            {
                if (!string.IsNullOrWhiteSpace(value))
                {
                    weaning_other = JsonConvert.DeserializeObject<List<JSON_DATA>>(value);
                }
            }
        }

        #endregion

        #region 呼吸器脫離計畫

        public string _try_weaning_mode
        {
            get { return JsonConvert.SerializeObject(try_weaning_mode); }
            set
            {
                if (!string.IsNullOrWhiteSpace(value))
                {
                    try_weaning_mode = JsonConvert.DeserializeObject<List<JSON_DATA>>(value);
                }
            }
        }


        public string _try_t_piece
        {
            get { return JsonConvert.SerializeObject(try_t_piece); }
            set
            {
                if (!string.IsNullOrWhiteSpace(value))
                {
                    try_t_piece = JsonConvert.DeserializeObject<List<JSON_DATA>>(value);
                }
            }
        }

        public string _try_t_piece_over_night
        {
            get { return JsonConvert.SerializeObject(try_t_piece_over_night); }
            set
            {
                if (!string.IsNullOrWhiteSpace(value))
                {
                    try_t_piece_over_night = JsonConvert.DeserializeObject<List<JSON_DATA>>(value);
                }
            }
        }

        public string _weaning_plan_other
        {
            get { return JsonConvert.SerializeObject(weaning_plan_other); }
            set
            {
                if (!string.IsNullOrWhiteSpace(value))
                {
                    weaning_plan_other = JsonConvert.DeserializeObject<List<JSON_DATA>>(value);
                }
            }
        }

        #endregion

        public string _weaningTable
        {
            get { return JsonConvert.SerializeObject(weaningTable); }
            set
            {
                if (!string.IsNullOrWhiteSpace(value))
                {
                    weaningTable = JsonConvert.DeserializeObject<List<RCS_WEANING_ITEM>>(value);
                }
            }
        }

        public string _labDataTable
        {
            get { return JsonConvert.SerializeObject(labDataTable); }
            set
            {
                if (!string.IsNullOrWhiteSpace(value))
                {
                    labDataTable = JsonConvert.DeserializeObject<List<RCS_LAB_ITEM>>(value);
                }
            }
        }

        public string _try_breath_reason
        {
            get { return JsonConvert.SerializeObject(try_breath_reason); }
            set
            {
                if (!string.IsNullOrWhiteSpace(value))
                {
                    try_breath_reason = JsonConvert.DeserializeObject<List<JSON_DATA>>(value);
                }
            }
        }

        public string _isweaning
        {
            get { return JsonConvert.SerializeObject(isweaning); }
            set
            {
                if (!string.IsNullOrWhiteSpace(value))
                {
                    isweaning = JsonConvert.DeserializeObject<List<JSON_DATA>>(value);
                }
            }
        }

        #region 病患評估

        /// <summary>
        /// 病患主要呼吸問題
        /// </summary>
        public List<JSON_DATA> breathingQuestion { get; set; }
        /// <summary>
        /// 胸腔病史
        /// </summary>
        public List<JSON_DATA> chest_history { get; set; }

        /// <summary>
        /// 意識
        /// </summary>
        public List<JSON_DATA> conscious { get; set; }
        /// <summary>
        /// 皮膚
        /// </summary>
        public List<JSON_DATA> skin { get; set; }
        /// <summary>
        /// 呼吸型態(病患自然呼吸狀態)
        /// </summary>
        public List<JSON_DATA> breathing_patterns { get; set; }
        /// <summary>
        /// 呼吸音
        /// </summary>
        public List<JSON_DATA> breath_sounds { get; set; }
        /// <summary>
        /// 咳嗽能力
        /// </summary>
        public List<JSON_DATA> cough { get; set; }
        /// <summary>
        /// 痰液
        /// </summary>
        public List<JSON_DATA> sputum { get; set; }
        /// <summary>
        /// 其他藥物使用
        /// </summary>
        public List<JSON_DATA> other_drugs { get; set; }

        #endregion

        #region 呼吸治療項目
        /// <summary>
        /// 噴霧治療
        /// </summary>
        public List<JSON_DATA> spray_treatment { get; set; }
        /// <summary>
        /// 藥物
        /// </summary>
        public List<JSON_DATA> drug { get; set; }
        /// <summary>
        /// 目前呼吸器使用情形
        /// </summary>
        public List<JSON_DATA> now_breathing_patterns { get; set; }
        /// <summary>
        /// 胸腔復健治療
        /// </summary>
        public List<JSON_DATA> cpt_treatment { get; set; }
        #endregion

        #region 呼吸器脫離困難原因

        /// <summary>
        /// Unstable vital signs
        /// </summary>
        public List<JSON_DATA> Unstable_vital_signs { get; set; }
        /// <summary>
        /// Gas exchange
        /// </summary>
        public List<JSON_DATA> gas_exchange { get; set; }
        /// <summary>
        /// underlying_disease
        /// </summary>
        public List<JSON_DATA> underlying_disease { get; set; }
        /// <summary>
        /// weaning_drug
        /// </summary>
        public List<JSON_DATA> weaning_drug { get; set; }
        /// <summary>
        /// poor_respiratory_drive
        /// </summary>
        public List<JSON_DATA> poor_respiratory_drive { get; set; }
        /// <summary>
        /// poor_respirator_muscle
        /// </summary>
        public List<JSON_DATA> poor_respirator_muscle { get; set; }
        /// <summary>
        /// poor_cough_fuction
        /// </summary>
        public List<JSON_DATA> poor_cough_fuction { get; set; }
        /// <summary>
        /// malnutrition
        /// </summary>
        public List<JSON_DATA> malnutrition { get; set; }
        /// <summary>
        /// weaning_other
        /// </summary>
        public List<JSON_DATA> weaning_other { get; set; }

        #endregion

        #region 呼吸器脫離計畫

        /// <summary>
        /// try_weaning_mode
        /// </summary>
        public List<JSON_DATA> try_weaning_mode { get; set; }
        /// <summary>
        /// try_t_piece
        /// </summary>
        public List<JSON_DATA> try_t_piece { get; set; }
        /// <summary>
        /// try_t_piece_over_night
        /// </summary>
        public List<JSON_DATA> try_t_piece_over_night { get; set; }
        /// <summary>
        /// weaning_plan_other
        /// </summary>
        public List<JSON_DATA> weaning_plan_other { get; set; }

        #endregion

        public List<RCS_WEANING_ITEM> weaningTable { get; set; }

        public List<RCS_LAB_ITEM> labDataTable { get; set; }

        public List<JSON_DATA> try_breath_reason { get; set; }

        public List<JSON_DATA> isweaning { get; set; }


 
        #region 病患評估

    
 
        #endregion

        #region 呼吸治療項目

 

        #endregion

        #region 呼吸器脫離困難原因

 
        //2019/11/06新增
        public string _airway_lung
        {
            get { return JsonConvert.SerializeObject(airway_lung); }
            set
            {
                if (!string.IsNullOrWhiteSpace(value))
                {
                    airway_lung = JsonConvert.DeserializeObject<List<JSON_DATA>>(value);
                }
            }
        }
        public string _brain_neurological
        {
            get { return JsonConvert.SerializeObject(brain_neurological); }
            set
            {
                if (!string.IsNullOrWhiteSpace(value))
                {
                    brain_neurological = JsonConvert.DeserializeObject<List<JSON_DATA>>(value);
                }
            }
        }
        public string _cardiac
        {
            get { return JsonConvert.SerializeObject(cardiac); }
            set
            {
                if (!string.IsNullOrWhiteSpace(value))
                {
                    cardiac = JsonConvert.DeserializeObject<List<JSON_DATA>>(value);
                }
            }
        }
        public string _diaphragm_respiratory_muscle_function
        {
            get { return JsonConvert.SerializeObject(diaphragm_respiratory_muscle_function); }
            set
            {
                if (!string.IsNullOrWhiteSpace(value))
                {
                    diaphragm_respiratory_muscle_function = JsonConvert.DeserializeObject<List<JSON_DATA>>(value);
                }
            }
        }
        public string _metabolic_status
        {
            get { return JsonConvert.SerializeObject(metabolic_status); }
            set
            {
                if (!string.IsNullOrWhiteSpace(value))
                {
                    metabolic_status = JsonConvert.DeserializeObject<List<JSON_DATA>>(value);
                }
            }
        }
        public string _infection
        {
            get { return JsonConvert.SerializeObject(infection); }
            set
            {
                if (!string.IsNullOrWhiteSpace(value))
                {
                    infection = JsonConvert.DeserializeObject<List<JSON_DATA>>(value);
                }
            }
        }
        public string _nutrition
        {
            get { return JsonConvert.SerializeObject(nutrition); }
            set
            {
                if (!string.IsNullOrWhiteSpace(value))
                {
                    nutrition = JsonConvert.DeserializeObject<List<JSON_DATA>>(value);
                }
            }
        }




        #endregion

        #region 呼吸器脫離計畫
 
 

        //2019/11/11新增
        public string _control_underlying_disease
        {
            get { return JsonConvert.SerializeObject(control_underlying_disease); }
            set
            {
                if (!string.IsNullOrWhiteSpace(value))
                {
                    control_underlying_disease = JsonConvert.DeserializeObject<List<JSON_DATA>>(value);
                }
            }
        }
        public string _control_mode
        {
            get { return JsonConvert.SerializeObject(control_mode); }
            set
            {
                if (!string.IsNullOrWhiteSpace(value))
                {
                    control_mode = JsonConvert.DeserializeObject<List<JSON_DATA>>(value);
                }
            }
        }

        #endregion




        #region 呼吸器脫離困難原因



        //2019/11/06新增
        public List<JSON_DATA> airway_lung { get; set; }
        public List<JSON_DATA> brain_neurological { get; set; }
        public List<JSON_DATA> cardiac { get; set; }
        public List<JSON_DATA> diaphragm_respiratory_muscle_function { get; set; }
        public List<JSON_DATA> metabolic_status { get; set; }
        public List<JSON_DATA> infection { get; set; }
        public List<JSON_DATA> nutrition { get; set; }
        public List<JSON_DATA> control_underlying_disease { get; set; }
        public List<JSON_DATA> control_mode { get; set; }

        #endregion


        
    }

    /// <summary>
    /// 脫離狀態資料
    /// </summary>
    public class RCS_WEANING_ITEM
    {
        public int seq { get; set; }

        public string date { get; set; }

        public string pi_max { get; set; }

        public string mv_value { get; set; }

        public string vt_value { get; set; }

        public string srr { get; set; }

        public string rsbi { get; set; }
         

        //2019/11/11新增
        public string rsi_srr { get; set; }
        public string pe_max { get; set; }

        public string cuff_leak_ml { get; set; }

        public string cuff_leak_sound { get; set; }

        //2019/12/20萬芳新增
        public string mode { get; set; }
        

    }

    /// <summary>
    /// 檢驗資料項目
    /// </summary>
    public class RCS_LAB_ITEM
    {
        public int seq { get; set; }
        public string date { get; set; }
        public string hb { get; set; }

        public string ht { get; set; }

        public string wbc { get; set; }

        public string Platelet { get; set; }

        public string na { get; set; }

        public string k { get; set; }

        public string bun { get; set; }

        public string cr { get; set; }

        public string albumin { get; set; }
    }

    public class ViewModelRTTakeoffAssess
    {

        /// <summary>
        /// 查詢開始日期
        /// </summary>
        public string sDate { get; set; }
        /// <summary>
        /// 查詢結束日期
        /// </summary>
        public string eDate { get; set; }
    } 

    #endregion
}

using RCS_Data.Controllers;
using System.Collections.Generic;


namespace RCS_Data.Models.ViewModels
{

    /// <summary>
    /// RTCPTASS 和 RTCPTRECORD 共用
    /// </summary>
    public class CPTNewRecord 
    {

        public CPTNewRecord()
        {
            this.ReconditioningExercise = new List<string>();
            this.treatment_check = new List<string>();
            this.treatment_Drug_suction_str = new List<string>();
            this.Guardian = new List<string>();
            this.Guardian_Aims = new List<string>();
            this.Sport_Training = new List<string>();
            this.Sport_Training_Aims = new List<string>();
            this.Respiratory_Muscle = new List<string>();
            this.Respiratory_Muscle_Aims = new List<string>();
            this.Lung_Expansion = new List<string>();
            this.Lung_Expansion_Aims = new List<string>();
            this.cough_aims = new List<string>();
            this.coucough_device_aimsgh = new List<string>();
            this.coucough_aimsgh = new List<string>();
            this.Breathing_Training = new List<string>();
            this.Breathing_Training_Aims = new List<string>();
            this.Drug_Inhalation = new List<string>();
            this.Drug_Inhalation_Aims = new List<string>();
            this.cpt_reason = new List<string>();
            this.Activity_ability_after = new List<string>();
            this.Activity_ability = new List<string>();
            this.ass_ev = new List<string>();
            this.breathe_type = new List<string>();
            this.breathe_sound = new List<string>();
            this.tube_pat = new List<string>();
            this.cough = new List<string>();
            this.sputum = new List<string>();
            this.rt_reason = new List<string>();
            this.cpt_tip_limbs = new List<string>();
            this.cpt_tip_temperature = new List<string>();
            this.cpt_tip_color = new List<string>();
            this.sputum_draw = new List<string>();
            this.ass_patterns_Breathe = new List<string>();
            this.ass_patterns_speed = new List<string>();
            this.ass_patterns = new List<string>();
            this.sputum_count = new List<string>();
            this.sputum_status = new List<string>();
            this.sputum_small_list = new List<string>();
            this.sputum_color = new List<string>();
            this.treatment = new List<string>();
            this.treatment_tube_pat = new List<string>();
            this.treatment_PD_side = new List<string>();
            this.treatment_PD_lobe = new List<string>();
            this.conscious_after = new List<string>();
            this.patterns = new List<string>();
            this.breathe_sound_after = new List<string>();
            this.LUNG_EXTEND_REGULAR = new List<string>();
            this.sputum_after = new List<string>();
            this.sputum_count_after = new List<string>();
            this.sputum_status_after = new List<string>();
            this.sputum_small_after_list = new List<string>();
            this.sputum_color_after = new List<string>();
            this.pat_compatibility = new List<string>();
            this.pat_effect = new List<string>();
        }

        public string rec_date { get; set; }

        public string treatment_items { get; set; }

        public string rec_date_next { get; set; }

        public string cpt_date
        {
            get
            {
                if (!string.IsNullOrWhiteSpace(rec_date) && mayaminer.com.library.DateHelper.isDate(rec_date))
                {
                    return rec_date.Split(' ')[0];
                }
                return "";
            }
        }

        public string cpt_time
        {
            get
            {
                if (!string.IsNullOrWhiteSpace(rec_date) && mayaminer.com.library.DateHelper.isDate(rec_date))
                {
                    return rec_date.Split(' ')[1];
                }
                return "";
            }
        }

        public string cpt_id { get; set; }


        public string CXR_key { get; set; }
        public string CXR_result_json { get; set; } //Cxr繪圖物件 2018.08.25 (Save儲存功能OK，Get讀取有錯誤) CxrResultJson_cls
        public string CxrImageBase64_str { get; set; }
        public string PDF_CXR_Date_Str { get; set; }
        public string PDF_CXR_Result_Str { get; set; } // 6.Cxr檢查結果 (Cxr資料庫) 2018.09.27
        public string breathe_mode { get; set; }

        /// <summary>
        /// 胸廓形狀
        /// </summary>
        public string thorax_shape { get; set; }

        /// <summary>
        /// 意識
        /// </summary>
        public List<string> ass_ev { get; set; }

        /// <summary>
        /// 意識
        /// </summary>
        public string ass_ev_str { get; set; }
        /// <summary>
        /// 呼吸型態
        /// </summary>
        public List<string> breathe_type { get; set; }
        /// <summary>
        /// 呼吸型態
        /// </summary>
        public string breathe_type_str { get; set; }

        public string breathe_type_txt { get; set; }

        /// <summary>
        /// 呼吸音
        /// </summary>
        public List<string> breathe_sound { get; set; }
        /// <summary>
        /// 呼吸音
        /// </summary>
        public string breathe_sound_str { get; set; }
        /// <summary> 呼吸音 Coarse </summary>
        public string breath_sound_Coarse { set; get; }
        /// <summary> 呼吸音 Crackle </summary>
        public string breath_sound_Crackle { set; get; }
        /// <summary> 呼吸音 Crackle </summary>
        public string breath_sound_Crackle_txt { set; get; }
        /// <summary> 呼吸音 Decrease </summary>
        public string breath_sound_Decrease { set; get; }
        /// <summary> 呼吸音 Decrease </summary>
        public string breath_sound_Decrease_txt { set; get; }
        /// <summary> 呼吸音 Absent </summary>
        public string breath_sound_Absent { set; get; }  
        /// <summary> 呼吸音 Decrease </summary>
        public string breath_sound_Absent_txt { set; get; }
        /// <summary> 呼吸音 Fine_Crackles </summary>
        public string breath_sound_Fine_Crackles { set; get; }
        /// <summary> 呼吸音 Fine_Crackles </summary>
        public string breath_sound_Fine_Crackles_txt { set; get; }
        /// <summary> 呼吸音 Wheezing </summary>
        public string breath_sound_Wheezing { set; get; }
        /// <summary> 呼吸音 Wheezing </summary>
        public string breath_sound_Wheezing_txt { set; get; }
        /// <summary> 呼吸音 Rhonchi </summary>
        public string breath_sound_Rhonchi { set; get; }
        /// <summary> 呼吸音 Rhonchi </summary>
        public string breath_sound_Rhonchi_txt { set; get; }
        /// <summary> 呼吸音 Decreased </summary>
        public string breath_sound_Decreased { set; get; }
        /// <summary> 呼吸音 Decreased </summary>
        public string breath_sound_Decreased_txt { set; get; }
        /// <summary> 呼吸音 other </summary>
        public string breath_sound_other_str { set; get; }
        public string breathe_sound_other { get; set; }

        // <summary>
        // 目前使用FiO2
        // </summary>
        public string fio { get; set; }

        /// <summary>
        /// 使用呼吸器
        /// </summary>
        public List<string> rt_reason { get; set; }
        /// <summary>
        /// 使用呼吸器
        /// </summary>
        public string rt_reason_str { get; set; }

        /// <summary>
        /// 四肢末稍情形 , 溫度 , 膚色
        /// </summary>
        public List<string> cpt_tip_limbs { get; set; }
        /// <summary>
        /// 四肢末稍情形 , 溫度 , 膚色
        /// </summary>
        public string cpt_tip_limbs_str { get; set; }
        public string cpt_Edema { get; set; }

        /// <summary>
        /// 四肢末稍情形 , 溫度 , 膚色
        /// </summary>
        public List<string> cpt_tip_temperature { get; set; }
        /// <summary>
        /// 四肢末稍情形 , 溫度 , 膚色
        /// </summary>
        public string cpt_tip_temperature_str { get; set; }

        /// <summary>
        /// 四肢末稍情形 , 溫度 , 膚色
        /// </summary>
        public List<string> cpt_tip_color { get; set; }

        /// <summary>
        /// 四肢末稍情形 , 溫度 , 膚色
        /// </summary>
        public string cpt_tip_color_str { get; set; }

        /// <summary>
        /// 是否需抽痰
        /// </summary>
        public List<string> sputum { get; set; }
        /// <summary>
        /// 是否需抽痰
        /// </summary>
        public string sputum_str { get; set; }

        #region 插管病人

        public string FiO2 { get; set; }

        public string Vt { get; set; }

        public string rate { get; set; }

        public string PEEP { get; set; }

        public string SpO2 { get; set; }

        public string tube_pat_txt { get; set; }

        public List<string> tube_pat { get; set; }

        public string tube_pat_str { get; set; }
        #endregion

        #region 咳嗽能力


        public List<string> cough { get; set; }

        public string cough_str { get; set; }

        public string chest_tube_ml { get; set; }

        public string chest_tube_color { get; set; }

        #endregion

        #region 抽煙史
        public bool cpt_ass_smoke_history_1 { get; set; }
        public bool cpt_ass_smoke_history_2 { get; set; }
        public string cpt_ass_smoke_history_3 { get; set; }
        public string cpt_ass_smoke_history_4 { get; set; }
        public bool cpt_ass_smoke_history_5 { get; set; }
        public string cpt_ass_smoke_history_6 { get; set; }

        public string smoke_history_data { set; get; }
        /// <summary> 抽菸史PPD </summary>
        public string smoke_history_PPD { set; get; }
        /// <summary> 抽菸史 年 </summary>
        public string smoke_history_year { set; get; }
        /// <summary> 已戒菸 </summary>
        public string smoke_history_end { set; get; }
        /// <summary> 已戒菸 年 </summary>
        public string smoke_history_end_year { set; get; }

        public string smoke_Guardianship { get; set; }
        public string smoke_Guardianship_other { get; set; }
        #endregion

        #region 是否需抽痰

        public List<string> sputum_draw { get; set; }
        public string sputum_draw_str { get; set; }
        #endregion

        #region 呼吸型態
        public List<string> ass_patterns_Breathe { get; set; }
        public List<string> ass_patterns_speed { get; set; }
        public List<string> ass_patterns { get; set; }

        public string ass_patterns_Breathe_str { get; set; }
        public string ass_patterns_speed_str { get; set; }
        public string ass_patterns_str { get; set; }

        public string ass_patterns_other { get; set; }
        #endregion

        public string sputum_Quantity_str { get; set; }

        #region 痰液 (量)

        public List<string> sputum_count { get; set; }
        public string sputum_count_str { get; set; }
        #endregion

        #region 痰液 (性狀)

        public List<string> sputum_status { get; set; }
        public string sputum_status_str { get; set; }
        #endregion

        public bool sputum_small { get; set; }
        public List<string> sputum_small_list { get; set; }
        public string sputum_small_list_str { get; set; }
        public string sputum_small_list_string { get; set; }

        /// <summary>
        /// 診斷
        /// </summary>
        public string now_diagnosis_display { get; set; }

        /// <summary>
        /// 身高(CM)
        /// </summary>
        public string Height { get; set; }

        /// <summary>
        /// 體重(KG)
        /// </summary>
        public string Weight { get; set; }

        public string BMI { get; set; }

        public string mMRC { get; set; }

        public string Borg_scale { get; set; }

        public string COPD_Cough { get; set; }
        public string COPD_sputum { get; set; }
        public string COPD_stuffy { get; set; }
        public string COPD_asthma { get; set; }
        public string COPD_limit { get; set; }
        public string COPD_outside { get; set; }
        public string COPD_sleep { get; set; }
        public string COPD_live { get; set; }

        public string COPD_total
        {
            get
            {
                int total = 0;
                if (!string.IsNullOrWhiteSpace(COPD_Cough)) {
                    total += int.Parse(COPD_Cough);
                }
                if (!string.IsNullOrWhiteSpace(COPD_sputum)){total += int.Parse(COPD_sputum);}
                if (!string.IsNullOrWhiteSpace(COPD_stuffy)) { total += int.Parse(COPD_stuffy); }
                if (!string.IsNullOrWhiteSpace(COPD_asthma)) { total += int.Parse(COPD_asthma); }
                if (!string.IsNullOrWhiteSpace(COPD_limit)) { total += int.Parse(COPD_limit); }
                if (!string.IsNullOrWhiteSpace(COPD_outside)) { total += int.Parse(COPD_outside); }
                if (!string.IsNullOrWhiteSpace(COPD_sleep)) { total += int.Parse(COPD_sleep); }
                if (!string.IsNullOrWhiteSpace(COPD_live)) { total += int.Parse(COPD_live); }
                return total.ToString();
            }
        }

        public string COPD_mMRC { get; set; }
        public string COPD_Borg_scale { get; set; }
        public string COPD_LBS_LU { get; set; }
        public string COPD_LBS_LD { get; set; }
        public string COPD_LBS_RU { get; set; }
        public string COPD_LBS_RD { get; set; }
        public string COPD_CM_LU { get; set; }
        public string COPD_CM_LD { get; set; }
        public string COPD_CM_RU { get; set; }
        public string COPD_CM_RD { get; set; }
        public string COPD_30S_muscle { get; set; }
        public string COPD_Moderate_AE { get; set; }
        public string COPD_hospitalized_AE { get; set; }
        public string COPD_group { get; set; }
        #region 意識
        public string conscious_e { get; set; }
        public string conscious_v { get; set; }
        public string conscious_m { get; set; }
        #endregion

        #region Temp / HR / RR / BP
        public string Temp { get; set; }
        public string HR { get; set; }
        public string RR { get; set; }
        public string BP { get; set; }
        public string BP_2 { get; set; }
        #endregion

        #region 痰液 (顏色)

        public List<string> sputum_color { get; set; }
        public string sputum_color_str { get; set; }
        public string sputum_color_string { get; set; }
        #endregion
        public string sputum_Consistency { get; set; }
        public string sputum_Smell { get; set; }

        #region 呼吸問題

        public List<string> cpt_reason { get; set; }
        public string cpt_reason_str { get; set; }
        public string cpt_reason_string { get; set; }
        #endregion

        #region 活動能力

        public List<string> Activity_ability { get; set; }

        public string Activity_ability_str { get; set; }

        public string Activity_ability_other { get; set; }
        #endregion

        #region 藥物吸入

        public List<string> Drug_Inhalation { get; set; }
        public string Drug_Inhalation_str { get; set; }
        public List<string> Drug_Inhalation_Aims { get; set; }
        public string Drug_Inhalation_Aims_str { get; set; }
        #endregion
        #region 呼吸訓練

        public List<string> Breathing_Training { get; set; }
        public string Breathing_Training_str { get; set; }
        public string Breathing_Training_other { get; set; }
        public List<string> Breathing_Training_Aims { get; set; }
        public string Breathing_Training_Aims_str { get; set; }
        #endregion
        #region 痰液清除 咳嗽訓練

        public List<string> cough_aims { get; set; }
        public List<string> coucough_aimsgh { get; set; }
        public List<string> coucough_device_aimsgh { get; set; }

        public string cough_aims_str { get; set; }
        public string coucough_aimsgh_str { get; set; }
        public string coucough_device_aimsgh_str { get; set; }

        #endregion
        #region 肺擴張運動

        public List<string> Lung_Expansion { get; set; }
        public List<string> Lung_Expansion_Aims { get; set; }

        public string Lung_Expansion_str { get; set; }
        public string Lung_Expansion_Aims_str { get; set; }
        #endregion
        #region 呼吸肌訓練

        public List<string> Respiratory_Muscle { get; set; }

        public List<string> Respiratory_Muscle_Aims { get; set; }

        public string Respiratory_Muscle_str { get; set; }            
        public string Respiratory_Muscle_Aims_str { get; set; }

        public string Respiratory_Muscle_Pimax { get; set; }

        public string Respiratory_Muscle_Pemax { get; set; }
        public string Respiratory_Muscle_Setting_Frequency { get; set; }
        public string Respiratory_Muscle_Setting_Time { get; set; }
        public string Respiratory_Muscle_Setting_Pressure { get; set; }
        public string Respiratory_Muscle_expiration_Frequency { get; set; }
        public string Respiratory_Muscle_expiration_Time { get; set; }
        public string Respiratory_Muscle_expiration_Pressure { get; set; }
        #endregion

        #region 運動訓練

        public List<string> Sport_Training { get; set; }
        public List<string> Sport_Training_Aims { get; set; }
        public string Sport_Training_str { get; set; }
        public string Sport_Training_Aims_str { get; set; }

        public string Sport_Training_Handcart_Min { get; set; }

        public string Sport_Training_Handcart_Resistance { get; set; }
        public string Sport_Training_Lifting { get; set; }

        public string Sport_Training_elasticBand_time { get; set; }
        public string Sport_Training_elasticBand_round { get; set; }
        public string Sport_Training_elasticBand_day { get; set; }
        public string Sport_Training_Bick_Min { get; set; }
        public string Sport_Training_Bick_Resistance { get; set; }
        public string Sport_Training_Other { get; set; }

        public string Sport_Training_home_Min { get; set; }
        public string Sport_Training_home_frequency { get; set; }
        public string Sport_Training_home_other { get; set; }
        #endregion

        #region 運動訓練

        public List<string> Guardian { get; set; }
        public List<string> Guardian_Aims { get; set; }
        public string Guardian_str { get; set; }
        public string Guardian_Aims_str { get; set; }

        public string Guardian_1 { get; set; }

        public string Guardian_2 { get; set; }
        public string Guardian_3 { get; set; }
        public string Guardian_4 { get; set; }
        public string Guardian_5 { get; set; }
        public string Guardian_6 { get; set; }
        public string Guardian_7 { get; set; }
        public string Guardian_8 { get; set; }
        public string Guardian_9 { get; set; }
        public string Guardian_10 { get; set; }
        public string Guardian_11 { get; set; }
        public string Guardian_12 { get; set; }
        #endregion

        #region 執行治療項目

        public List<string> treatment_check { get; set; }
        public string treatment_check_str { get; set; }

        public string treatment_CPT_txt { get; set; }
        public string treatment_CPT_end { get; set; }

        public string treatment_PD_txt { get; set; }
        public string treatment_PD_end { get; set; }
        public string treatment_HFCWO_Hz_txt { get; set; }
        public string treatment_HFCWO_Hz_end { get; set; }

        public string treatment_HFCWO_Min_txt { get; set; }

        public string treatment_Incentive_spirometry_ml_txt { get; set; }

        public string treatment_Incentive_spirometry_end { get; set; }

        public string treatment_Incentive_spirometry_sec_txt { get; set; }

        public string treatment_HFCWO_Hz2_txt { get; set; }

        public string treatment_HFCWO_Min2_txt { get; set; }

        public string treatment_Incentive_spirometry_ml2_txt { get; set; }

        public string treatment_Incentive_spirometry_sec2_txt { get; set; }
        public string treatment_MDI_txt { get; set; }
        public string treatment_other_txt { get; set; }
        public string treatment_DPI_txt { get; set; }
        public string treatment_Guardian_txt1 { get; set; }
        public string treatment_Guardian_txt2 { get; set; }
        public string treatment_Guardian_txt3 { get; set; }
        public string treatment_Guardian_txt4 { get; set; }
        public string treatment_IMT_times_txt { get; set; }
        public string treatment_IMT_cycle_txt { get; set; }
        public string treatment_IMT_cmH2O_txt { get; set; }
        public string treatment_EMT_times_txt { get; set; }
        public string treatment_EMT_cycle_txt { get; set; }
        public string treatment_EMT_cmH2O_txt { get; set; }
        public string treatment_MIP_cmH2O_txt { get; set; }
        public string treatment_MEP_cmH2O_txt { get; set; }
        public string treatment_Flutter_cmH2O_txt { get; set; }
        public string treatment_PEP_cmH2O_txt { get; set; }
        public string treatment_Inspiratory_time_txt { get; set; }
        public string treatment_Inspiratory_count_txt { get; set; }
        public string treatment_Handcart_resistance_txt { get; set; }
        public string treatment_Handcart_min_txt { get; set; }
        public string treatment_Bick_resistance_txt { get; set; }
        public string treatment_Bick_min_txt { get; set; }
        #region 舉水瓶
        public string treatment_Lifting_gm_txt { get; set; }
        public string treatment_Lifting_time_txt { get; set; }
        public string treatment_Lifting_round_txt { get; set; }
        public string treatment_Lifting_day_txt { get; set; }


        #endregion

        public string treatment_elasticBand_count_txt { get; set; }
        public string treatment_elasticBand_time_txt { get; set; }
        public string treatment_elasticBand_day_txt { get; set; }
        public string treatment_Daily_Walking_txt { get; set; }

        public string treatment_CoughMachine_cycle_txt { get; set; }
        public string treatment_CoughMachine_IPAP_txt { get; set; }
        public string treatment_CoughMachine_EPAP_txt { get; set; }
        public string treatment_CoughMachine_Ti_txt { get; set; }
        public string treatment_CoughMachine_Te_txt { get; set; }
        public string treatment_CoughMachine_Tp_txt { get; set; }
        public string treatment_Diaphragmatic_kg_txt { get; set; }

        public string treatment_Diaphragmatic_min_txt { get; set; }
        public string treatment_inhalation_name_txt { get; set; }
        public string treatment_inhalation_dose_txt { get; set; }
        public string treatment_inhalation_amp_txt { get; set; }

        #region inhalation
        public string treatment_inhalation_name_txt_1 { get; set; }
        public string treatment_inhalation_dose_txt_1 { get; set; }
        public string treatment_inhalation_amp_txt_1 { get; set; }
        public string treatment_inhalation_name_txt_2 { get; set; }
        public string treatment_inhalation_dose_txt_2 { get; set; }
        public string treatment_inhalation_amp_txt_2 { get; set; }
        public string treatment_inhalation_name_txt_3 { get; set; }
        public string treatment_inhalation_dose_txt_3 { get; set; }
        public string treatment_inhalation_amp_txt_3 { get; set; }
        #endregion

        public List<string> ReconditioningExercise { get; set; }

        public string ReconditioningExercise_str { get; set; }
        public string Mobilization_Exe_end { get; set; }

        public string Exercise { get; set; }

        public string Exercise_other { get; set; }

        //public bool treatment_Drug_suction { get; set; }

        public List<string> treatment_Drug_suction_str { get; set; }

        public string treatment_Drug_suction_txt { get; set; }
        public string treatment_Drug_suction_end { get; set; }
        public string treatment_PEFR_txt { get; set; }


        public List<string> treatment { get; set; }
        public string treatment_str { get; set; }
        public string treatment_txt { get; set; }

        public List<string> treatment_tube_pat { get; set; }
        public string treatment_tube_pat_str { get; set; }
        public string treatment_tube_pat_txt { get; set; }

        public string treatment_depp_breath { get; set; }
        public string treatment_volume_type { get; set; }
        public string treatment_hold { get; set; }

        public List<string> treatment_PD_side { get; set; }
        public string treatment_PD_side_str { get; set; }
        public List<string> treatment_PD_lobe { get; set; }
        public string treatment_PD_lobe_str { get; set; }

        public string treatment_segment { get; set; }

        #endregion

        #region 病人配合度 病人學習效果

        public List<string> pat_compatibility { get; set; }
        public string pat_compatibility_str { get; set; }

        public List<string> pat_effect { get; set; }
        public string pat_effect_str { get; set; }

        #endregion

        #region 意識

        public List<string> conscious_after { get; set; }
        public string conscious_after_str { get; set; }

        public string conscious_E_after { get; set; }
        public string conscious_V_after { get; set; }
        public string conscious_M_after { get; set; }

        #endregion


        #region 監測
        public string HR_rest { get; set; }
        public string HR_exercise { get; set; }
        public string BP_rest { get; set; }
        public string BP_rest_2 { get; set; }
        public string BP_exercise { get; set; }
        public string BP_exercise_2 { get; set; }
        public string SPO2_rest { get; set; }
        public string SPO2_exercise { get; set; }
        public string Borg_rest { get; set; }
        public string Borg_exercise { get; set; }
        #endregion
        #region 治療後呼吸型態
        public List<string> patterns { get; set; }
        public string patterns_str { get; set; }
        public string patterns_txt { get; set; }
        #endregion

        #region 治療後呼吸音
        public List<string> breathe_sound_after { get; set; }
        public string breathe_sound_after_str { get; set; }
        #endregion

        #region 活動能力

        public List<string> Activity_ability_after { get; set; }
        public string Activity_ability_after_str { get; set; }
        #endregion


        #region 肺擴張
        public string atelectasis_txt { get; set; }
        public List<string> LUNG_EXTEND_REGULAR { get; set; }
        public string LUNG_EXTEND_REGULAR_str { get; set; }

        #endregion

        #region 治療後痰

        public List<string> sputum_after { get; set; }
        public string sputum_after_str { get; set; }
        #endregion

        #region 痰液 (量)
        public List<string> sputum_count_after { get; set; }
        public string sputum_count_after_str { get; set; }
        #endregion

        #region 痰液 (性狀)
        public List<string> sputum_status_after { get; set; }
        public string sputum_status_after_str { get; set; }

        public bool sputum_small_after { get; set; }

        public List<string> sputum_small_after_list { get; set; }

        public string sputum_small_after_list_str { get; set; }

        public string SpO2_after { get; set; }

        #endregion

        #region 痰液 (顏色)

        public List<string> sputum_color_after { get; set; }
        public string sputum_color_after_str { get; set; }

        #endregion

        public string CREATE_NAME { get; set; }

        /// <summary>
        /// 備註
        /// </summary>
        public string Remark { get; set; }


        /// <summary>
        /// IPPB pressure(cmH2O)
        /// </summary>
        public string IPPB_pressure_cmH2O { get; set; }

        /// <summary>
        /// IPPB pressure(cmH2O)
        /// </summary>
        public string HFCWO_P { get; set; }

        /// <summary>
        /// IPPB pressure(cmH2O)
        /// </summary>
        public string HFCWO_Frequency { get; set; }
        /// <summary>
        /// IPPB pressure(cmH2O)
        /// </summary>
        public string HFCWO_Time { get; set; }
        /// <summary>
        /// SMI：ml
        /// </summary>
        public string SMI_ml { get; set; }
        /// <summary>
        /// SMI：sec
        /// </summary>
        public string SMI_sec { get; set; }
        /// <summary>
        /// 上肢運動
        /// </summary>
        public string up_Exercise { get; set; }
        public string up_course { get; set; }
        public string up_Frequency { get; set; }
        /// <summary>
        /// 下肢運動
        /// </summary>
        public string down_Exercise { get; set; }
        public string down_course { get; set; }
        public string down_Frequency { get; set; }


        public string Devices_str { get; set; }
        public string Devices_other_txt { get; set; }


        #region Breath Exercise
        public string BreathExercise_Pursed_lip { get; set; }

        public string BreathExercise_Diaphragmatic { get; set; }
        public string BreathExercise_Huffing { get; set; }
        public string BreathExercise_Cough { get; set; }
        #endregion

        public string ReconditioningExe { get; set; }

        public string PEP { get; set; }

        #region Cough Machine

        public string CoughMachine_Inhale_P { get; set; }

        public string CoughMachine_Exhale_P { get; set; }
        public string CoughMachine_Ti { get; set; }
        public string CoughMachine_Te { get; set; }
        public string CoughMachine_Tp { get; set; }

        #endregion

        #region NPV

        public string NPV_Pinsp { get; set; }

        public string NPV_Pexp { get; set; }
        public string NPV_RR { get; set; }
        public string NPV_IE { get; set; }

        #endregion


        #region 衛教
        /// <summary>
        /// 居家氧氣治療
        /// </summary>
        public string Home_Drug_Inhalation { get; set; }
        /// <summary>
        /// 居家氧氣治療
        /// </summary>
        public string Home_Oxygen_Therapy { get; set; }
        /// <summary>
        /// 氣道清潔技術
        /// </summary>
        public string Airway_cleaning_technology { get; set; }
        /// <summary>
        /// 節能指導
        /// </summary>
        public string Energy_Saving_Guidance { get; set; }
        #endregion

        public string O2_Device { get; set; }

        public string Fio2_Device { get; set; }

        #region 休息時

        public string Break_O2_flow { get; set; }
        public string Break_SpO2 { get; set; }
        public string Break_RR { get; set; }
        public string Break_HR { get; set; }
        public string Break_SBP { get; set; }
        public string Break_DBP { get; set; }
        #endregion

        #region 治療中

        public string In_treatment_O2_flow { get; set; }
        public string In_treatment_SpO2 { get; set; }
        public string In_treatment_RR { get; set; }
        public string In_treatment_HR { get; set; }
        public string In_treatment_SBP { get; set; }
        public string In_treatment_DBP { get; set; }
        #endregion

        #region 治療後

        public string End_treatment_O2_flow { get; set; }
        public string End_treatment_SpO2 { get; set; }
        public string End_treatment_RR { get; set; }
        public string End_treatment_HR { get; set; }
        public string End_treatment_SBP { get; set; }
        public string End_treatment_DBP { get; set; }
        #endregion

        public string Brog_score { get; set; }

        public string Brog_score_after { get; set; }

        public string Vital_sign_SpO2 { get; set; }

        public string Vital_sign_FiO2 { get; set; }

        public string Breathing_Sound { get; set; }
        public string Breathing_Side { get; set; }
        public string Breathing_pattern { get; set; }

        public string Duration_mins { get; set; }
        public string Duration_week { get; set; }

        public string Muscle_Strength_LU {get;set;}
        public string Muscle_Strength_LD { get; set; }
        public string Muscle_Strength_RU { get; set; }
        public string Muscle_Strength_RD { get; set; }

        public string In_Rest_str { get; set; }
        public string In_Rest_other { get; set; }

        public string Break_Devic { get; set; }
        public string Break_FIO2 { get; set; }
        public string Break_O2 { get; set; }
        public string daily_Devic { get; set; }
        public string daily_FIO2 { get; set; }
        public string daily_O2 { get; set; }
        public string Present_Situation { get; set; }

        public string Objective_Data_MRC { get; set; }
        public string Objective_Data_CAT { get; set; }
        public string Objective_Data_VRI { get; set; }

        public string Respiration_related_str { get; set; }
        public string Respiration_related_other { get; set; }

        public string Frequency_str { get; set; }
        public string Frequency_other { get; set; }
        public string Short_term_Goal { get; set; }
        public string Medium_term_Goal { get; set; }
        public string Long_term_Goal { get; set; }
        public string Aerosol_Therapy{get;set;}
        public string Humidity_Therapy{get;set;}
        public string Airway_Clearance_Therapy_str { get; set; }
        public string Airway_Clearance_Therapy_other { get; set; }
        public string Lung_Expansion_Therapy_str { get; set; }
        public string Lung_Expansion_Therapy_other { get; set; }

        public string Oxygen_Break_Devic { get; set; }
        public string Oxygen_Break_FIO2 { get; set; }
        public string Oxygen_Break_O2 { get; set; }
        public string Oxygen_daily_Devic { get; set; }
        public string Oxygen_daily_FIO2 { get; set; }
        public string Oxygen_daily_O2 { get; set; }
        public string Oxygen_Therapy { get; set; }
        public string Inspiratory_Muscle {get;set;}
        public string Inspiratory_Setting { get; set; }
        public string Inspiratory_Frequency { get; set; }
        public string Expiratory_Muscle { get; set; }
        public string Expiratory_Setting { get; set; }
        public string Expiratory_Frequency { get; set; }
        public string VPN_textarea { get; set; }
        public string UPLOAD_STATUS { get; set; }
        public string DATASTATUS { get; set; }



    }
}

/// <summary>
/// 病患手術碼
/// </summary>
public class CxrData
{
    /// <summary>檢查日期 (Cxr資料庫)</summary>
    public string Result_Date { set; get; }
    /// <summary>檢查結果 (Cxr資料庫)</summary>
    public string Result_Str { set; get; }

    /// <summary>singJsonImage</summary>
    public string singJsonImageBase64 { set; get; }


}


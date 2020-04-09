using RCS_Data.Controllers;
using RCSData.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RCS_Data.Models.ViewModels
{
    /// <summary>
    /// CPT 紀錄單欄位名稱(cpt_item)
    /// </summary>
    public class RCS_RTAsses 
    {
        /// <summary> 入院日期 </summary>
        public string diag_date { set; get; }
        /// <summary> 記錄日期 </summary>
        public string cpt_date { set; get; }
        /// <summary> 記錄時間 </summary>
        public string record_date { set; get; }
        /// <summary> 主檔編號 </summary>
        public string cpt_id { set; get; }
        /// <summary>
        /// VPN
        /// </summary>
        public string VPN { get; set; }
        /// <summary>
        /// VPN文字
        /// </summary>
        public string VPN_textarea { get; set; }
        /// <summary> 病患診斷 </summary>
        public string now_pat_diagnosis { set; get; }
        /// <summary> 手術 </summary>
        public string operation_data { set; get; }
        /// <summary> 手術備註 </summary>
        public string operation_memo { set; get; }
        /// <summary>
        /// 麻醉方式小時
        /// </summary>
        public string hocus_hours { set; get; }

        /// <summary> 其他病史 </summary>
        public string other_history { set; get; }
        /// <summary> 入院經過 </summary>
        public string diagnosis { set; get; }
        /// <summary> 轉入前病房分類 </summary>
        public string from_unit_data { set; get; }
        /// <summary> 轉入前病房分類其他 </summary>
        public string from_unit_other_txext { set; get; }
        /// <summary> 肺部病史 </summary>
        public string cpt_history_data { set; get; }
        /// <summary> Heart Disease－AMI </summary>
        public string cpt_ami { set; get; }
        /// <summary> 肺部病史 </summary>
        public string cpt_history_other_txext { set; get; }
        /// <summary> 使用呼吸器原因 </summary>
        public string rt_reason_data { set; get; }
        /// <summary> 使用呼吸器原因  Trauma </summary>
        public string rt_reason_trauma { set; get; }
        /// <summary> 使用呼吸器原因   Post-op(手術名稱) </summary>
        public string rt_reason_postop { set; get; }
        /// <summary> 使用呼吸器原因  Other  </summary>
        public string rt_reason_other { set; get; }
        /// <summary> 插管時間 </summary>
        public string tube_time { set; get; }
        /// <summary> ETT 管徑 </summary>
        public string tube_width { set; get; }
        /// <summary> 深度 </summary>
        public string tube_deep { set; get; }
        /// <summary> 氣切時間 </summary>
        public string shiley_time { set; get; }
        /// <summary> Tr. 管徑 </summary>
        public string shiley_width { set; get; }
        /// <summary> 廠牌 </summary>
        public string shiley_kind { set; get; }
        /// <summary> 呼吸器開始使用日期 </summary>
        public string rt_start_time { set; get; }
        /// <summary> 呼吸器機型 </summary>
        public string rt_start_respirator_model { set; get; }
        /// <summary> 呼吸器使用介面 </summary>
        public string rt_start_if { set; get; }
        /// <summary> 呼吸器使用模式 model </summary>
        public string rt_start_model { set; get; }
        /// <summary> 呼吸器使用模式 FIO2</summary>
        public string rt_start_fio2 { set; get; }
        /// <summary> 呼吸器使用模式 PEEP</summary>
        public string rt_start_PEEP { set; get; }
        /// <summary> 呼吸器使用模式 VT/PC</summary>
        public string rt_start_VT_PC { set; get; }
        /// <summary>
        /// 呼吸器使用模式  PC
        /// </summary>
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

        /// <summary> ABG日期 </summary>
        public string abg_date { set; get; }
        /// <summary> ABG  pH </summary>
        public string abg_data_pH { set; get; }
        /// <summary> ABG  PaCO2 </summary>
        public string abg_data_PaCO2 { set; get; }
        /// <summary> ABG  PaO2 </summary>
        public string abg_data_PaO2 { set; get; }
        /// <summary> ABG  ,HCO3 </summary>
        public string abg_data_HCO3 { set; get; }
        /// <summary> ABG  BE </summary>
        public string abg_data_BE { set; get; }
        /// <summary> ABG  SaO2 </summary>
        public string abg_data_SaO2 { set; get; }
        /// <summary> ABG  FiO2 </summary>
        public string abg_data_FiO2 { set; get; }
        /// <summary>
        /// ABG 備註
        /// </summary>
        public string abg_data_remark { set; get; }
        /// <summary> ABG  O2 </summary>
        public string abg_data_O2 { set; get; }
        /// <summary> ABG  Device </summary>
        public string abg_data_Device { set; get; }

        /// <summary> 體溫 </summary>
        public string base_body_temperature { set; get; }
        /// <summary> 脈膊 </summary>
        public string base_pulse { set; get; }
        /// <summary> 呼吸</summary>
        public string base_Breathe { set; get; }
        /// <summary> 收縮壓 </summary>
        public string base_Systolic_pressure { set; get; }
        /// <summary> 舒張壓 </summary>
        public string base_Diastolic_pressure { set; get; }

        /// <summary> 意識LLIST </summary>
        public string conscious_data_list { set; get; }
        /// <summary> 意識 E </summary>
        public string conscious_E { set; get; }
        /// <summary> 意識 V </summary>
        public string conscious_V { set; get; }
        /// <summary> 意識 M </summary>
        public string conscious_M { set; get; }
        /// <summary> 意識 </summary>
        public string conscious_data { set; get; }
        /// <summary> 身高 </summary>
        public string body_height { set; get; }
        /// <summary> 體重 </summary>
        public string body_weight { set; get; }
        /// <summary>
        /// 體重G
        /// </summary>
        public string body_weight_g { set; get; }
        /// <summary>
        /// 四肢末梢情形
        /// </summary>
        public string from_tip_data { set; get; }
        /// <summary>
        /// 四肢末梢情形 其他
        /// </summary>
        public string from_tip_data_txext { set; get; }

        /// <summary>
        /// 溫度
        /// </summary>
        public string temperature { set; get; }
        /// <summary>
        /// 膚色
        /// </summary>
        public string color { set; get; }

        /// <summary> IBW </summary>
        public string body_IBW { set; get; }
        /// <summary> 抽菸史 </summary>
        public string smoke_history_data { set; get; }
        /// <summary> 抽菸史PPD </summary>
        public string smoke_history_PPD { set; get; }
        /// <summary> 抽菸史 年 </summary>
        public string smoke_history_year { set; get; }
        /// <summary> 已戒菸 </summary>
        public string smoke_history_end { set; get; }
        /// <summary> 已戒菸 年 </summary>
        public string smoke_history_end_year { set; get; }
        /// <summary> 呼吸型態 </summary>
        public string patterns_data { set; get; }
        /// <summary> 呼吸型態 其他 </summary>
        public string patterns_other { set; get; }
        /// <summary>
        /// 呼吸音
        /// </summary>
        public string breath_sound_data { set; get; }
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
        /// <summary> 呼吸音 Absent </summary>
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
        /// <summary> 呼吸音 Decreased </summary>
        public string breath_sound_Decreased { set; get; }
        /// <summary> 呼吸音 other </summary>
        public string breath_sound_other_text { set; get; }
        /// <summary> 呼吸音 other </summary>
        public string breath_sound_other { set; get; }
        /// <summary> 咳嗽能力 </summary>
        public string cough_data { set; get; }
        /// <summary> 痰液評估 排出方式 </summary>
        public string sputum_assess_way { set; get; }
        /// <summary> 痰液評估 量 </summary>
        public string sputum_assess_amount { set; get; }
        /// <summary> 痰液評估 質 </summary>
        public string sputum_assess_quality { set; get; }
        /// <summary> 痰液評估 色 </summary>
        public string sputum_assess_color { set; get; }
        /// <summary>
        /// 特殊狀況描述 
        /// </summary>
        public string FeaturesRemark { set; get; }
        /// <summary> 痰液評估 味 </summary>
        public string sputum_assess_test { set; get; }
        /// <summary> 備註 </summary>
        public string remark { set; get; } 
        /// <summary>
        /// 資料狀態(1:新增,9:刪除,2:歷史記錄)
        /// </summary>
        public string DATASTATUS { set; get; }

        /// <summary>
        /// 上傳狀態(1:已上傳,0:尚未上傳)
        /// </summary>
        public string UPLOAD_STATUS { set; get; }

        /// <summary>
        /// 上傳者代碼
        /// </summary>
        public string UPLOAD_ID { set; get; }

        /// <summary> 特殊狀況描述 </summary>
        public string cpt_memo { set; get; }
        public string history_diag { set; get; }
        public string operation_string { set; get; }
    }



    /// <summary>
    /// RAAsses 表單回傳
    /// </summary>
    public class RTAssesDetail
    {
        /// <summary>
        /// 取得資料
        /// </summary>
        public RCS_RTAsses model { get; set; }

        public string CREATE_ID { get; set; }

        public string CREATE_NAME { get; set; }
        /// <summary>
        /// 最後一筆ID
        /// </summary>
        public string last_cpt_id { get; set; }

        /// <summary>
        /// 手術LIST
        /// </summary>
        public List<PatOperation> operation_List { get; set; }

        /// <summary>
        /// 意識EVM下拉選單
        /// </summary>
        public List<DDLItem> conscious_e { get; set; }
        /// <summary>
        /// 意識EVM下拉選單
        /// </summary>
        public List<DDLItem> conscious_v { get; set; }
        /// <summary>
        /// 意識EVM下拉選單
        /// </summary>
        public List<DDLItem> conscious_m { get; set; }


        public string DATASTATUS { get; set; } 
        public string UPLOAD_STATUS { get; set; }
        public string UPLOAD_ID { get; set; }

    }
}

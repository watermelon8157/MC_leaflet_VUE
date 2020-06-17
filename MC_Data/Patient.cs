using System;
using System.Collections.Generic;
using System.Linq;
using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;
using System.Data;
using mayaminer.com.jxDB;

namespace RCS_Data
{

    /// <summary>
    /// 院內病床資訊
    /// </summary>
    public class GetBed {
        private string _NHI_OPD_ER = "I";
        /// <summary>診別 E:急診、O:門診、I:住院(預設)</summary>
        public string NHI_OPD_ER {
            get {
                return _NHI_OPD_ER;
            }
            set {
                _NHI_OPD_ER = value;
            }
        }
        /// <summary>急門診日</summary>
        public string VS_DAT { set; get; }
        /// <summary>護理站代碼</summary>
        public string NS { set; get; }
        /// <summary>護理站名稱</summary>
        public string NAM_C { set; get; }
        /// <summary>院區</summary>
        public string LOC { set; get; }
        /// <summary>房號</summary>
        public string ROOM { set; get; }
        /// <summary>床號</summary>
        public string BED { set; get; }
        /// <summary> 住院帳號或門診帳號 </summary>
        public string ACC_NO { set; get; }
        /// <summary>身分證字號</summary>
        public string IDNO { set; get; }
        /// <summary>病歷號</summary>
        public string CHART_NO { set; get; }
        /// <summary>病患姓名</summary>
        public string NAME { set; get; }
        /// <summary>性別</summary>
        public string SEX { set; get; }
        /// <summary>民國生日YYMMDD</summary>
        public string BORN_YYMMDD { set; get; }
        public string AGE { set; get; }
        /// <summary>住院時間</summary>
        public string ADMIN_TIME { set; get; }
        /// <summary>預計出院時間</summary>
        public string PRE_DISCHARGE_TIME { set; get; }

    }

   
    public class Patient 
    {

        /// <summary> 病人姓名 </summary>
        public virtual string patient_name { set; get; }
        /// <summary> 身份證字號 </summary>
        public string idno { set; get; }

        /// <summary> 病歷號 </summary>
        public string chart_no { set; get; }

        /// <summary> 住院帳號 </summary>
        public string ipd_no { set; get; }

        /// <summary> 生日 </summary>
        public string birth_day { set; get; }
        /// <summary> 年齡 </summary>
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

        private string _sex;
        /// <summary> 性別 </summary>
        public string gender
        {
            set { _sex = value; }
            get
            {
                switch (_sex)
                {
                    case "男":
                    case "M":
                    case "1":
                        return "1";
                    case "女":
                    case "0":
                    case "F":
                        return "0";
                    default:
                        return "2";
                }
            }
        }
        public string genderEN
        {
            set { _sex = value; }
            get
            {
                switch (_sex)
                {
                    case "男":
                    case "M":
                    case "1":
                        return "M";
                    case "女":
                    case "0":
                    case "F":
                        return "F";
                    default:
                        return "";
                }
            }
        }
        /// <summary> 性別(男:女) </summary>
        public string genderCHT
        {
            get
            {
                switch (gender)
                {
                    case "男":
                    case "M":
                    case "1":
                        return "男";
                    case "女":
                    case "0":
                    case "F":
                        return "女";
                    default:
                        return "未知";
                }
            }
        }


        public string bed_no { set; get; }
        public string diagnosis_code { set; get; }
    }

    public class RecordList : List<RecordItem> {
        public void Add(string record_id, string record_date) {
            base.Add(new RecordItem(record_id, record_date));
        }

        public void Add(string record_id, Dictionary<string, string> content) {
            base.Add(new RecordItem(record_id, content));
        }
    }

    /// <summary>
    /// 呼吸記錄
    /// </summary>
    public class RecordItem {

        public RecordItem(string record_id, string record_date) {
            this.record_id = record_id;
            this.record_date = record_date;
        }

        public RecordItem(string record_id, Dictionary<string, string> content) {
            this.record_id = record_id;
            this.content = content;
        }

        /// <summary>記錄ID</summary>
        public string record_id { set; get; }
        /// <summary>記錄時間</summary>
        public string record_date { set; get; }
        /// <summary>記錄內容</summary>
        public Dictionary<string, string> content = new Dictionary<string, string>();
    }

    /// <summary>
    /// 醫療輔具評估報告
    /// </summary>
    public class MeasuresForm {
        public MeasuresForm() {

        }

        public void setSaveData() {
            setTrimEnd();

        }
        public void setTrimEnd() {
            if (oldManual != null && oldManual.Length > 0) oldManual = oldManual.Substring(0, oldManual.Length - 1);
            if (oldManual_1 != null && oldManual.Length > 0) oldManual_1 = oldManual_1.Substring(0, oldManual_1.Length - 1);
            if (oldManual_2 != null && oldManual_2.Length > 0) oldManual_2 = oldManual_2.Substring(0, oldManual_2.Length - 1);
            if (oldManual_txt != null && oldManual_txt.Length > 0) oldManual_txt = oldManual_txt.Substring(0, oldManual_txt.Length - 1);
            if (newManual != null && newManual.Length > 0) newManual = newManual.Substring(0, newManual.Length - 1);
            if (newManual_txt != null && newManual_txt.Length > 0) newManual_txt = newManual_txt.Substring(0, newManual_txt.Length - 1);
            if (AdSituation_text != null && AdSituation_text.Length > 0) AdSituation_text = AdSituation_text.Substring(0, AdSituation_text.Length - 1);
            if (assessType != null && assessType.Length > 0) assessType = assessType.Substring(0, assessType.Length - 1);
            if (breatheWeak != null && breatheWeak.Length > 0) breatheWeak = breatheWeak.Substring(0, breatheWeak.Length - 1);
            if (breatheWeak_1 != null && breatheWeak_1.Length > 0) breatheWeak_1 = breatheWeak_1.Substring(0, breatheWeak_1.Length - 1);
            if (breatheWeak_txt != null && breatheWeak_txt.Length > 0) breatheWeak_txt = breatheWeak_txt.Substring(0, breatheWeak_txt.Length - 1);
            if (MachineItem != null && MachineItem.Length > 0) MachineItem = MachineItem.Substring(0, MachineItem.Length - 1);
            if (MachineItem_txt != null && MachineItem_txt.Length > 0) MachineItem_txt = MachineItem_txt.Substring(0, MachineItem_txt.Length - 1);
            if (O2assess != null && O2assess.Length > 0) O2assess = O2assess.Substring(0, O2assess.Length - 1);
            if (O2assess_1 != null && O2assess_1.Length > 0) O2assess_1 = O2assess_1.Substring(0, O2assess_1.Length - 1);
            if (O2assess_txt != null && O2assess_txt.Length > 0) O2assess_txt = O2assess_txt.Substring(0, O2assess_txt.Length - 1);
            if (Machine != null && Machine.Length > 0) Machine = Machine.Substring(0, Machine.Length - 1);
            if (invasive != null && invasive.Length > 0) invasive = invasive.Substring(0, invasive.Length - 1);
            if (invasive_mode != null && invasive_mode.Length > 0) invasive_mode = invasive_mode.Substring(0, invasive_mode.Length - 1);
            if (invasive_O2density != null && invasive_O2density.Length > 0) invasive_O2density = invasive_O2density.Substring(0, invasive_O2density.Length - 1);
        }

        public void setExportData() {
            chkAd = chkAd.Replace("1", "■").Replace("0", "□");
            oldManual = oldManual.Replace("1", "■").Replace("0", "□");
            oldManual_1 = oldManual_1.Replace("1", "■").Replace("0", "□");
            oldManual_2 = oldManual_2.Replace("1", "■").Replace("0", "□");
            newManual = newManual.Replace("1", "■").Replace("0", "□");
            assessType = assessType.Replace("1", "■").Replace("0", "□");
            breatheWeak = breatheWeak.Replace("1", "■").Replace("0", "□");
            breatheWeak_1 = breatheWeak_1.Replace("1", "■").Replace("0", "□");
            MachineItem = MachineItem.Replace("1", "■").Replace("0", "□");
            O2assess = O2assess.Replace("1", "■").Replace("0", "□");
            O2assess_1 = O2assess_1.Replace("1", "■").Replace("0", "□");
            Machine = Machine.Replace("1", "■").Replace("0", "□");
            invasive = invasive.Replace("1", "■").Replace("0", "□");

        }

        public void setNullData() {
            chkAd = "□";
            int cnt = 15;
            for (int i = 0; i < cnt + 1; i++) {
                oldManual += "□,";
                if (i == cnt) oldManual = oldManual.TrimEnd(',');
            }
            cnt = 2;
            for (int i = 0; i < cnt + 1; i++) {
                oldManual_1 += "□,";
                if (i == cnt) oldManual_1 = oldManual_1.TrimEnd(',');
            }
            cnt = 3;
            for (int i = 0; i < cnt + 1; i++) {
                oldManual_2 += "□,";
                if (i == cnt) oldManual_2 = oldManual_2.TrimEnd(',');
            }
            cnt = 8;
            for (int i = 0; i < cnt + 1; i++) {
                newManual += "□,";
                if (i == cnt) newManual = newManual.TrimEnd(',');
            }
            newManual_txt = ",";
            cnt = 2;
            for (int i = 0; i < cnt + 1; i++) {
                assessType += "□,";
                if (i == cnt) assessType = assessType.TrimEnd(',');
            }
            cnt = 5;
            for (int i = 0; i < cnt + 1; i++) {
                breatheWeak += "□,";
                if (i == cnt) breatheWeak = breatheWeak.TrimEnd(',');
            }
            cnt = 1;
            for (int i = 0; i < cnt + 1; i++) {
                breatheWeak_1 += "□,";
                if (i == cnt) breatheWeak_1 = breatheWeak_1.TrimEnd(',');
            }
            AdSituation_text = ",";
            breatheWeak_txt = ",,,";
            cnt = 1;
            for (int i = 0; i < cnt + 1; i++) {
                invasive += "□,";
                if (i == cnt) invasive = invasive.TrimEnd(',');
            }
            invasive_mode = ",";
            invasive_O2density = ",";
            cnt = 3;
            for (int i = 0; i < cnt + 1; i++) {
                MachineItem += "□,";
                if (i == cnt) MachineItem = MachineItem.TrimEnd(',');
            }
            MachineItem_txt = ",";
            cnt = 3;
            for (int i = 0; i < cnt + 1; i++) {
                O2assess += "□,";
                if (i == cnt) O2assess = O2assess.TrimEnd(',');
            }
            cnt = 2;
            for (int i = 0; i < cnt + 1; i++) {
                O2assess_1 += "□,";
                if (i == cnt) O2assess_1 = O2assess_1.TrimEnd(',');
            }
            O2assess_txt = ",,";
            cnt = 3;
            for (int i = 0; i < cnt + 1; i++) {
                Machine += "□,";
                if (i == cnt) Machine = Machine.TrimEnd(',');
            }

        }



        public bool hasData = false;

        public string tempid { get; set; }
        public string name { get; set; }
        public string sex { get; set; }
        public string id_num { get; set; }
        public string birth { get; set; }
        public string county_1 { get; set; }
        public string district_1 { get; set; }
        public string li_1 { get; set; }
        public string rd_1 { get; set; }
        public string dan_1 { get; set; }
        public string ln_1 { get; set; }
        public string aly_1 { get; set; }
        public string no_1 { get; set; }
        public string f_1 { get; set; }
        public string chkAd { get; set; }
        public string county_2 { get; set; }
        public string district_2 { get; set; }
        public string li_2 { get; set; }
        public string rd_2 { get; set; }
        public string dan_2 { get; set; }
        public string ln_2 { get; set; }
        public string aly_2 { get; set; }
        public string no_2 { get; set; }
        public string f_2 { get; set; }
        //public string ad1 { get; set; }
        //public string ad2 { get; set; }
        public string isObstacle { get; set; }
        public string hasManual { get; set; }
        public string oldManual { get; set; }
        public string oldManual_1 { get; set; }
        public string oldManual_2 { get; set; }
        public string oldManual_txt { get; set; }
        public string newManual { get; set; }
        public string newManual_txt { get; set; }
        public string obstacleLevel { get; set; }
        public string AdSituation { get; set; }
        public string AdSituation_text { get; set; }
        public string contactMan { get; set; }
        public string withRelationship { get; set; }
        public string contactPhone { get; set; }
        public string contactCellPhone { get; set; }
        public string assessType { get; set; }
        public string breatheWeak { get; set; }
        public string breatheWeak_1 { get; set; }
        public string breatheWeak_txt { get; set; }
        public string yyy { get; set; }
        public string mm { get; set; }
        public string dd { get; set; }
        public string yyy_3 { get; set; }
        public string mm_3 { get; set; }
        public string dd_3 { get; set; }
        public string height { get; set; }
        public string weight { get; set; }
        public string DrawDate_1 { get; set; }
        public string O2density_1 { get; set; }
        public string ph_1 { get; set; }
        public string PaCO2_1 { get; set; }
        public string PaO2_1 { get; set; }
        public string HCO3_1 { get; set; }
        public string SaO2_1 { get; set; }
        public string gasAnalysis { get; set; }
        public string gasDrawDate { get; set; }
        public string invasive { get; set; }
        public string invasive_mode { get; set; }
        public string invasive_O2density { get; set; }
        public string gas_ph { get; set; }
        public string gas_PaCO2 { get; set; }
        public string gas_PaO2 { get; set; }
        public string gas_HCO3 { get; set; }
        public string gas_SaO2 { get; set; }
        public string breathVoice { get; set; }
        public string breathVoice_txt { get; set; }
        public string breathingStatus { get; set; }
        public string breathingStatus_txt { get; set; }
        public string cough { get; set; }
        public string phlegm { get; set; }
        public string MachineItem { get; set; }
        public string MachineItem_txt { get; set; }
        public string O2assess { get; set; }
        public string O2assess_1 { get; set; }
        public string O2assess_txt { get; set; }
        public string deployDate { get; set; }
        public string Machine { get; set; }
        public string O2machine_4 { get; set; }
        public string O2machine_txt_4 { get; set; }
        public string duplexBreathMachine_4 { get; set; }
        public string duplexBreathMachine_txt_4 { get; set; }
        public string simplexBreathMachine_4 { get; set; }
        public string simplexBreathMachine_txt_4 { get; set; }
        public string useTraining { get; set; }
        public string suggest { get; set; }
        public string track { get; set; }

        public string assess { get; set; }
        public string assess_person { get; set; }
        public string job { get; set; }
        public string assess_date { get; set; }

        public string medical_tool_5 { get; set; }
        public string medical_tool_txt_5 { get; set; }
        public string useTraining_5 { get; set; }

        public string check { get; set; }
        public string check_person { get; set; }
        public string check_job { get; set; }
        public string check_date { get; set; }
    }


    /// <summary>
    /// 呼吸器脫離評估項目
    /// </summary>
    public class TakeOffItem {
        public string Name { get; set; }
        public string lableName { get; set; }
        public string Value { get; set; }
    }

    /// <summary>
    /// 呼吸器脫離評估第二階
    /// </summary>
    public class TakeOffSub {
        public string sub_name { get; set; }
        public List<TakeOffItem> sub_content { get; set; }
    }

    /// <summary>
    /// 呼吸器脫離評估第一階
    /// </summary>
    public class TakeOff {
        public string level1_name { get; set; }
        public string type { get; set; }
        public List<TakeOffSub> content { get; set; }
    }

    /// <summary>
    /// 目前On Mode狀態
    /// </summary>
    public class OnTypeMode {
        /// <summary>呼吸器流水號</summary>
        public string ONMODE_ID { get; set; }
        /// <summary>On類型(1:插管、2:呼吸器、3:氧療)</summary>
        public string on_type { get; set; }
        /// <summary>使用狀態(true:使用中、false:非使用中)</summary>
        public bool use_status { get; set; }
        /// <summary>開始時間的限制</summary>
        public string start_limit { get; set; }
        /// <summary>結束時間的限制</summary>
        public string end_limit { get; set; }
        /// <summary>結束時間判斷(true:使用結束、false:使用中)</summary>
        public bool hasEnd_Date { get; set; }
    }

    /// <summary>
    /// 全部On Mode狀態與病患資訊
    /// </summary>
    public class AllOnModeList {
        /// <summary>房間區域</summary>
        public string ROOM_LOC { get; set; }
        /// <summary>房號</summary>
        public string ROOM_NO { get; set; }
        /// <summary>床號</summary>
        public string BED_NO { get; set; }
        /// <summary>病患姓名</summary>
        public string PATIENT_NAME { get; set; }
        /// <summary>護理站代碼</summary>
        public string NS { get; set; }
        /// <summary>ON MODE 流水號</summary>
        public string ONMODE_ID { get; set; }
        /// <summary>住院帳號</summary>
        public string FEE_NO { get; set; }
        /// <summary>病歷號</summary>
        public string CHART_NO { get; set; }
        /// <summary>ON類型{1:插管、2:呼吸器、3:氧療}</summary>
        public string ON_TYPE { get; set; }
    }

    public class AssignmentRemark {
        /// <summary>區域名稱</summary>
        public string area_name { get; set; }
        /// <summary>區域值</summary>
        public string area_value { get; set; }
        /// <summary>分配數量</summary>
        public int dispatch_count { get; set; }
    }

    public class AssignmentObject {
        /// <summary>班別</summary>
        public string work_type { get; set; }
        /// <summary>更新時間</summary>
        public string update_time { get; set; }
        /// <summary>區域名稱</summary>
        public string area_name { get; set; }
        /// <summary>區域值</summary>
        public string area_value { get; set; }
        private List<AssignmentRemark> _remark = new List<AssignmentRemark>();
        /// <summary>病患來源</summary>
        public List<AssignmentRemark> remark {
            get {
                return _remark;
            }
            set {
                if (value != null) {
                    _remark = value;
                }
            }
        }
        /// <summary>分配前</summary>
        public int move_before { get; set; }
        /// <summary>分配後</summary>
        public int move_after { get; set; }
        /// <summary>插管總數</summary>
        public int intubate_total { get; set; }
        /// <summary>氧療總數</summary>
        public int oxygen_total { get; set; }
        /*以下為合併欄位*/
        /// <summary>分配前/分配後</summary>
        public string merger_before_after { set; get; }
        /// <summary>設定其他欄位(合併欄位)</summary>
        public void setOtherValue() {
            this.merger_before_after = this.move_before.ToString() + "/" + this.move_after.ToString();
        }
    }

    public class AssignmentDB {
        public string A_ID { get; set; }
        /// <summary>班別</summary>
        public string WORK_TYPE { get; set; }
        public List<AssignmentObject> A_RESULT { get; set; }
        public string YYYYMMDD_DATE { get; set; }
        public string CREATE_TIME { get; set; }
        public string UPDATE_TIME { get; set; }
        public string UPDATE_USER { get; set; }
    } 
    public class RT_TAKEOFF_ASSESSMENT {
        public string record_date { set; get; }
        public string pi { set; get; }
        public string pe { set; get; }
        public string rsbi { set; get; }
        public string cuff_leak { set; get; }
        public string cuff_leak_ml { set; get; }
        public string mode { set; get; }
        public string vt_value { set; get; }
        public string srr { set; get; }
        public string cuff_leak_sound { set; get; }        
    }

    #region 呼吸紀錄單
 
    /// <summary>
    /// 動脈血氣體分析
    /// </summary>
    public interface IABG_LAB_DATA
    {
        string abg_time { get; set; }
        string abg_ph { get; set; }
        string abg_paco2 { get; set; }
        string abg_pao2 { get; set; }
        string abg_sao2 { get; set; }
        string abg_hco3 { get; set; }
        string abg_be { get; set; }
        string abg_paado2 { get; set; }
        string abg_shunt { get; set; }

        //20180126 新增欄位

        string abg_pulse_oximeter { get; set; }

        //20191104 新增欄位
        string abg_device { get; set; }
    }

    /// <summary>
    /// 血液生化
    /// </summary>
    public interface IBIOCHEM_LAB_DATA
    {
        string biochem_hb { get; set; }
        string biochem_ht { get; set; }
        string biochem_wbc { get; set; }
        string biochem_p { get; set; }
        string biochem_na { get; set; }
        string biochem_k { get; set; }
        string biochem_bun { get; set; }
        string biochem_cr { get; set; }
        string biochem_albumin_sugar { get; set; }
    }

    /// <summary>
    /// 檢驗資料
    /// </summary>
    public class LAB_DATA: IABG_LAB_DATA, IBIOCHEM_LAB_DATA
    {

        public string abg_time { get; set; }
        public string abg_ph { get; set; }
        public string abg_paco2 { get; set; }
        public string abg_pao2 { get; set; }
        public string abg_sao2 { get; set; }
        public string abg_hco3 { get; set; }
        public string abg_be { get; set; }
        public string abg_paado2 { get; set; }
        public string abg_shunt { get; set; }
        public string abg_pulse_oximeter { get; set; }
        public string abg_device { get; set; }

        public string biochem_hb { get; set; }
        public string biochem_ht { get; set; }
        public string biochem_wbc { get; set; }
        public string biochem_p { get; set; }
        public string biochem_na { get; set; }
        public string biochem_k { get; set; }
        public string biochem_bun { get; set; }
        public string biochem_cr { get; set; }
        public string biochem_albumin_sugar { get; set; }

    }


    /// <summary> 插管紀錄 </summary>
    public class ONINTUBATE {
        public string intubate_onmode_id { get; set; }
        public string intubate_start_date { get; set; }
        public string intubate_route { get; set; }
        public string intubate_route_date { get; set; }
        public string intubate_tube_diameter { get; set; }
        public string intubate_tube_depth { get; set; }
        public string intubate_end_date { get; set; }
        public string intubate_use { get; set; }
        public string intubate_reason { get; set; }
        public string intubate_result { get; set; }
        /// <summary>限制插管最小日期</summary>
        public string limitIntubateDate { get; set; }
        /// <summary>插管Detail</summary>
        public List<JSON_DATA> detail { get; set; }
    }

    /// <summary> 嘗試脫離紀錄 </summary>
    public class BREATHWEANING {
        public string date_time { get; set; }
        public string result { get; set; }
    }

    /// <summary> 呼吸器紀錄 </summary>
    public class ONBREATH {
        public string breath_onmode_id { get; set; }
        public List<BREATHWEANING> breath_weaning { get; set; }
        public string breath_start_date { get; set; }
        public string breath_indications { get; set; }
        public string breath_reason { get; set; }
        public string breath_end_date { get; set; }
        public string breath_result { get; set; }
        /// <summary>限制呼吸最小日期</summary>
        public string limitBreathDate { get; set; }
        /// <summary>呼吸器Detail</summary>
        public List<JSON_DATA> detail { get; set; }
    }

    /// <summary> 氧療紀錄 </summary>
    public class ONOXYGEN {
        public string oxygen_onmode_id { get; set; }
        public string oxygen_start_date { get; set; }
        public string oxygen_type { get; set; }
        public string oxygen_end_date { get; set; }
        /// <summary>限制氧療最小日期</summary>
        public string limitOxygenDate { get; set; }
        /// <summary>氧療Detail</summary>
        public List<JSON_DATA> detail { get; set; }
    }

    public class ONMODE_MASTER {
        public string ONMODE_ID { get; set; }
        public string FEE_NO { get; set; }
        public string CHART_NO { get; set; }
        public string STARTDATE { get; set; }
        public string WEANINGDATE { get; set; }
        public string ENDDATE { get; set; }
        public string ON_TYPE { get; set; }
        public string CONTENT { get; set; }
        public string CREATEUSER { get; set; }
        public string CREATEDATE { get; set; }
        public string UPDATEUSER { get; set; }
        public string UPDATEDATE { get; set; }
        /// <summary>1:有效,2:無效(刪除)</summary>
        public string dc_flag { get; set; }
        /// <summary>
        /// 顯示類型
        /// </summary>
        public string on_type_desc
        {
            // (1:插管、2:呼吸器、3:氧療)
            get
            {
                string returnVal = "";
                showCONTENT = new List<onmodeList>();
                switch (this.ON_TYPE)
                {
                    case "1":
                        showCONTENT.Add(new onmodeList { title = "開始", type = "value", id = "intubate_start_date", txt = STARTDATE });
                        showCONTENT.Add(new onmodeList { title = "經由", type = "chk", id = "intubate_route" });
                        //showCONTENT.Add(new onmodeList { title = "氣管切開術", type = "txt", id = "intubate_route_date" });
                        showCONTENT.Add(new onmodeList { title = "插管直徑", type = "txt", id = "intubate_tube_diameter" });
                        showCONTENT.Add(new onmodeList { title = "插管深度", type = "txt", id = "intubate_tube_depth" });
                        showCONTENT.Add(new onmodeList { title = "拔管時間", type = "value", id = "intubate_end_date", txt = ENDDATE });
                        showCONTENT.Add(new onmodeList { title = "拔管時", type = "chk", id = "intubate_use" });
                        showCONTENT.Add(new onmodeList { title = "把管理由", type = "chk", id = "intubate_reason" });
                        showCONTENT.Add(new onmodeList { title = "結果", type = "chk", id = "intubate_result" });
                        returnVal = "插管";
                        break;
                    case "2":
                        showCONTENT.Add(new onmodeList { title = "開始", type = "value", id = "breath_start_date", txt = STARTDATE });
                        showCONTENT.Add(new onmodeList { title = "適應症", type = "txt", id = "breath_indications" });
                        showCONTENT.Add(new onmodeList { title = "引發呼吸衰竭原因", type = "txt", id = "breath_reason" });
                        showCONTENT.Add(new onmodeList { title = "嘗試脫離", type = "weaning", id = "breath_weaning" });
                        showCONTENT.Add(new onmodeList { title = "結束呼吸器時間", type = "value", id = "breath_end_date", txt = ENDDATE });
                        showCONTENT.Add(new onmodeList { title = "治療結果", type = "chk", id = "breath_result" });
                        returnVal = "呼吸器";
                        break;
                    case "3":
                        showCONTENT.Add(new onmodeList { title = "開始時間", type = "value", id = "oxygen_start_date", txt = STARTDATE });
                        showCONTENT.Add(new onmodeList { title = "治療種類", type = "txt", id = "oxygen_type" });
                        showCONTENT.Add(new onmodeList { title = "結束時間", type = "value", id = "oxygen_end_date", txt = ENDDATE });
                        returnVal = "氧療";
                        break;
                    default:
                        returnVal = "";
                        break;
                }
                this.setCONTENT();
                return returnVal;
            }
        }
        /// <summary> 使用天數 </summary>
        public string usage_days
        {
            get
            {
                DateTime st = DateTime.Parse(this.STARTDATE);
                DateTime ed;
                try
                {
                    ed = DateTime.Parse(this.ENDDATE);
                }
                catch
                {
                    ed = DateTime.Now;
                }
                TimeSpan ts = ed - st;
                //當天也算
                string use_day = "1";
                if ((int)ts.TotalDays > 0)
                {
                    use_day = ((int)ts.TotalDays).ToString();
                }
                return use_day;
            }
        }
        /// <summary>
        /// 顯示歷史紀錄
        /// </summary>
        public List<onmodeList> showCONTENT { get; set; }
        /// <summary>
        /// 設定顯示歷史紀錄
        /// </summary>
        private void setCONTENT()
        {
            List<JSON_DATA> data = new List<JSON_DATA>();
            if (CONTENT != null && CONTENT != "")
                data = JsonConvert.DeserializeObject<List<JSON_DATA>>(CONTENT);
            if (data.Count > 0)
            {
                foreach (onmodeList item in showCONTENT)
                {
                    if (item.txt == null) item.txt = "";
                    if (data.Exists(x => x.id.Contains(item.id)))
                    {
                        List<JSON_DATA> temp = data.FindAll(x => x.id.Contains(item.id));
                        switch (item.type)
                        {
                            case "txt":
                                item.txt = string.Join(",", temp.Select(x => x.txt).ToList());
                                break;
                            case "chk":
                                item.txt = string.Join(",", temp.FindAll(x => x.chkd).Select(x => x.txt).ToList());
                                break;
                            case "weaning":
                                break;
                            case "value":
                                //帶入時已設定
                                break;
                            default:
                                break;
                        }
                    }
                }
                //如果有氣管切開術
                if (data.Exists(x => x.id == "intubate_route_date" && !string.IsNullOrWhiteSpace(x.val)))
                    showCONTENT.Find(x => x.id == "intubate_route").txt += " " + data.Find(x => x.id == "intubate_route_date" && !string.IsNullOrWhiteSpace(x.val)).val;
            }
        } 
    }

    public class onmodeList
    {
        /// <summary>
        /// 標題
        /// </summary>
        public string title { get;set; }
        /// <summary>
        /// id
        /// </summary>
        public string id { get; set; }
        /// <summary>
        /// 顯示文字
        /// </summary>
        public string txt = "";
        /// <summary>
        /// 資料類型
        /// </summary>
        public string type { get; set; }
    }

    
    #endregion

    #region 呼吸器脫離評估

    /// <summary> 呼吸器脫離評估表 </summary>
    public class RCS_DATA_TK_ASSESS : BASE_DATA,IEditPower_base {
        /// <summary> 脫離評估單單號 </summary>
        [DisplayFormat(ConvertEmptyStringToNull = true)]
        public string tk_id { set; get; }

        public string idno { set; get; }
        /// <summary> 住院序號 </summary>
        public string ipd_no { set; get; }
        /// <summary> 病歷號 </summary>
        public string chart_no { set; get; }
        /// <summary> 記錄時間 </summary>
        public string rec_date { set; get; }

        public string _tk_reason
        {
            get { return JsonConvert.SerializeObject(this.tk_reason); }
            set
            {
                if (!string.IsNullOrWhiteSpace(value))
                {
                    tk_reason = JsonConvert.DeserializeObject<List<JSON_DATA>>(value);
                }
            }
        }
        /// <summary> 無法脫離呼吸器原因 </summary>
        public List<JSON_DATA> tk_reason { set; get; }

        public string _st_reason { get { return JsonConvert.SerializeObject(this.st_reason); }
            set
            {
                if (!string.IsNullOrWhiteSpace(value))
                {
                    st_reason = JsonConvert.DeserializeObject<List<JSON_DATA>>(value);
                }
            }
        }
        /// <summary> 停止呼吸器脫離的原因 </summary>
        public List<JSON_DATA> st_reason { set; get; }
        public string _tk_plan { get { return JsonConvert.SerializeObject(this.tk_plan); }
            set
            {
                if (!string.IsNullOrWhiteSpace(value))
                {
                    tk_plan = JsonConvert.DeserializeObject<List<JSON_DATA>>(value);
                }
            }
        }
        /// <summary> 呼吸器脫離計畫(依病情，可複選) </summary>
        public List<JSON_DATA> tk_plan { set; get; }

        /// <summary>  是否有修改資料的權限  </summary>
        public bool hasPowerEdit { set; get; }
    }

    /// <summary> 呼吸器脫離評估儲存選項 </summary>
    public class RCS_DATA_TK_ITEM {
        /// <summary> 項目分類 </summary>
        public string item_cate { set; get; }
        /// <summary> 項目編號 </summary>
        public string item_id { set; get; }
        /// <summary> 項目類型 </summary>
        public string item_type { set; get; }
        /// <summary> 選項文字或值 </summary>
        public string item_text { set; get; }
        /// <summary> 依照各物件自行定義 </summary>
        public string item_value { set; get; }
        /// <summary> 是否選取 </summary>
        public bool is_checked { set; get; }
    }

    /// <summary>
    /// 呼吸嘗試脫離紀錄
    /// </summary>
    public class TryTKInfo {
        /// <summary>
        /// 嘗試脫離時間
        /// </summary>
        public string date_time { set; get; }
        /// <summary>
        /// 嘗試脫離結果
        /// </summary>
        public string result { set; get; }
        /// <summary>
        /// 嘗試脫離結果描述
        /// </summary>
        public string result_desc {
            get {
                if (result == "1")
                    return "成功";
                else
                    return "失敗";
            }
        }
    }

    #endregion

    #region 呼吸患者評估單

    /// <summary> 呼吸患者評估單 </summary>
    public class RCS_DATA_RT_ASSESS : BASE_DATA,  IEditPower_base {

        [DisplayFormat(ConvertEmptyStringToNull = true)]
        /// <summary> 患者評估單編號 </summary>
        public string ass_id { set; get; }

        /// <summary> 住院序號 </summary>
        public string ipd_no { set; get; }

        /// <summary> 病歷號 </summary>
        public string chart_no { set; get; }

        /// <summary> 記錄時間 </summary>
        public string rec_date { set; get; }

        /// <summary> 呼吸模式 </summary>
        public string breathe_mode { set; get; }

        /// <summary> 呼吸型態 </summary>
        public List<JSON_DATA> breathe_type { set; get; }

        /// <summary> 意識 </summary>
        public List<JSON_DATA> ass_ev { set; get; }

        /// <summary> 呼吸異音位置 </summary>
        public List<JSON_DATA> breathe_sound { set; get; }

        /// <summary> 咳嗽能力 </summary>
        public List<JSON_DATA> cough { set; get; }

        /// <summary> 是否需要抽痰 </summary>
        public List<JSON_DATA> sputum_draw { set; get; }

        /// <summary> 痰量 </summary>
        public List<JSON_DATA> sputum_count { set; get; }

        /// <summary> 痰性狀 </summary>
        public List<JSON_DATA> sputum_status { set; get; }

        /// <summary> 痰顏色 </summary>
        public List<JSON_DATA> sputum_color { set; get; }

        /// <summary> CXR圖片檔名 </summary>
        public string image_file_key { set; get; }

        /// <summary> CXR日期 </summary>
        public string cxr_date { set; get; }

        /// <summary> CXR座標 </summary>
        public List<RCS_DATA_CXR_POSITION> cxr_position { set; get; }

        /// <summary> CXR原因 </summary>
        public List<JSON_DATA> cxr_reason { set; get; }

        /// <summary> 刪除註記 { Y:刪除 N:未刪除} </summary>
        public string dc_flag { set; get; }

        /// <summary> base64圖片 </summary>
        public string base64Str { set; get; }

       
        /// <summary>  是否有修改資料的權限  </summary>
        public bool hasPowerEdit { set; get; }

    }

    /// <summary> 呼吸音位置套件 </summary>
    public class RCS_BREATHE_SOUND : RCS_MAXFORM_ITEM {

        /// <summary> 子項目 </summary>
        public List<RCS_DATA_TK_ITEM> sub_item { set; get; }
    }

    public class RCS_MAXFORM_ITEM {
        public string c_tag { set; get; }
        public string c_type { set; get; }
        public string c_name { set; get; }
        public string c_sub_name { set; get; }
        public string c_value { set; get; }
        public string c_sub_value { set; get; }
        public bool c_is_checked { set; get; }
    }


    /// <summary> CXR位置資訊 </summary>
    public class RCS_DATA_CXR_POSITION {
        /// <summary> CXR X座標 </summary>
        public string x_loc { get; set; }

        /// <summary> CXR Y座標 </summary>
        public string y_loc { get; set; }

        /// <summary> 粗細 </summary>
        public string size { get; set; }
    }

    #endregion

    #region CPT記錄單
 
     

    /// <summary> CPT記錄單 </summary>

    public class RCS_DATA_CR_MASTER : BASE_DATA  {
        /// <summary> 脫離評估單單號 </summary>
        [DisplayFormat(ConvertEmptyStringToNull = true)]
        public string cpt_id { set; get; }
        /// <summary> 病歷號 </summary>
        public string chart_no { set; get; }
        /// <summary> 住院序號 </summary>
        public string ipd_no { set; get; }
        /// <summary> Y:記錄中,N:結束 </summary>
        public string cpt_status { set; get; }
        /// <summary> CPT記錄單 </summary>
        public RCS_DATA_CR_RECORD cpt_record { set; get; }
 

    }

    /// <summary> CPT記錄單 </summary>
    public class RCS_DATA_CR_RECORD : BASE_DATA,IEditPower_base {
        /// <summary> 住院序號 </summary>
        public string ipd_no { set; get; }
        /// <summary> 病歷號 </summary>
        public string chart_no { set; get; }
        /// <summary> 脫離評估單單號 </summary>
        [DisplayFormat(ConvertEmptyStringToNull = true)]
        public string cptr_id { set; get; }
        /// <summary> 記錄日期 </summary>
        public string cpt_date { set; get; }
        /// <summary> 記錄時間 </summary>
        public string cpt_time { set; get; }
        /// <summary> 顯示日期時間 </summary>
        public string show_date { get { return cpt_date+" "+ cpt_time; } }
        /// <summary> 治療項目 </summary>
        public List<JSON_DATA> treatment { set; get; }
        /// <summary> 意識 / EVM </summary>
        public List<JSON_DATA> conscious { set; get; }
        /// <summary> 呼吸型態 </summary>
        public List<JSON_DATA> patterns { set; get; }
        /// <summary> 肺擴張 </summary>
        public List<JSON_DATA> atelectasis { set; get; }
        /// <summary> 呼吸音 </summary>
        public List<JSON_DATA> sound { set; get; }
        /// <summary> 是否需抽痰 </summary>
        public List<JSON_DATA> sputum { set; get; }
        /// <summary> 痰液評估 </summary>
        public List<JSON_DATA> sputum_assess { set; get; }
        /// <summary> 授權代碼 </summary>
        public string RtGrantListtempid { set; get; }
        /// <summary>  是否有修改資料的權限  </summary>
        public bool hasPowerEdit { set; get; }
        /// <summary> 監測 </summary>
        public List<JSON_DATA> monitor { set; get; }
    }

    /// <summary>
    /// 呼吸音結構
    /// </summary>
    public class RCS_DATA_CR_SOUND : RCS_DATA_CR_ITEM {
        public List<RCS_DATA_CR_ITEM> sub_item { get; set; }
    }

    /// <summary> CPT記錄單儲存選項 </summary>
    public class RCS_DATA_CR_ITEM {
        /// <summary> 項目類型 </summary>
        public string item_type { set; get; }
        /// <summary> 項目分類 </summary>
        public string item_cate { set; get; }
        /// <summary> 項目id </summary>
        public string item_id { set; get; }
        /// <summary> 是否選取 </summary>
        public bool is_checked { set; get; }
        /// <summary> 選項文字或值 </summary>
        public string item_text { set; get; }
        /// <summary> 選項值 </summary>
        public string sub_text { set; get; }
    }

    #endregion

    #region 繪圖物件
    public class saveCxrSqlMsgObj // CXR 畫線資料 2018.08.24
    {
        public saveCxrSqlMsgObj()
        {
            cxrResponseMsg = new RESPONSE_MSG();
            cxrDbResultMsg = new dbResultMessage();
            cxrResultList = new List<CxrResultJson_cls>();
            cxrXuwmcList = new List<CXR_XYwmc>();
        }
        public RESPONSE_MSG cxrResponseMsg { get; set; }
        public dbResultMessage cxrDbResultMsg { get; set; }
        public DataTable cxrDataTable { get; set; }
        public DataRow cxrDataRow { get; set; }
        public List<CxrResultJson_cls> cxrResultList { get; set; }
        public CxrResultJson_cls cxrResultNode { get; set; }
        public List<CXR_XYwmc> cxrXuwmcList { get; set; }
    }
    public class CxrResult_SplitedObj // CXR 畫線資料 2018.09.21
    {
        public CxrResult_SplitedObj()
        {
            cxrResult_SplitedNode = new CxrResultJson_cls();
            cxrXuwmcList = new List<CXR_XYwmc>();
        }
        public string Cxr_CJID { get; set; } // 3.Cxr流水號 [資料表名稱] + [主表ID流水號]
        public CxrResultJson_cls cxrResult_SplitedNode { get; set; } //"CxrXuwmc_List": [18] → "CxrXuwmc_List": null
        public List<CXR_XYwmc> cxrXuwmcList { get; set; }
    }
    public class CxrResultJson_cls // CXR 畫線資料
    {
        public string SqlTableName { get; set; }        // 1.細項 SQL 資料庫名稱 (主資料庫)
        public string SqlMasterDetail_ID { get; set; }  // 2.細項 ID 流水號 (主資料庫)
        public string Cxr_CJID { get; set; }            // 3.Cxr流水號 [資料表名稱] + [主表ID流水號]
        public string image_file_key { set; get; }      // 4.CXR圖片檔名 (Cxr資料庫)
        public string Result_Date { get; set; }         // 5.檢查日期 (Cxr資料庫)
        public string Result_Str { get; set; }          // 6.Cxr檢查結果 (Cxr資料庫)
        public List<CXR_XYwmc> CxrXuwmc_List { get; set; }  // 7.座標 (Cxr資料庫)
        public List<string> ResultStr_Dropdownlist { get; set; }  // 8.CXR下拉清單 
        public bool hasSingJson
        {
            get
            {
                if (singJson != null && singJson.Count > 0)
                    return true;
                return false;
            }
        }
        public List<List<SIGNATURE_JSON>> singJson { get; set; }  //SIGNATURE_JSON

        public string singJsonImageBase64 { get; set; }
    }
    public class CXR_XYwmc // CXR 畫線資料
    {
        public string x { get; set; }
        public string y { get; set; }
        public string width { get; set; }
        public string mouse { get; set; }
        public string color { get; set; }
    }

    public class SIGNATURE_JSON
    {
        public string x { get; set; }
        public string y { get; set; }
        public string time { get; set; }
        public string color { get; set; }
       
    }

    #endregion

    #region 呼吸使用紀錄

    public class RT_USER_RECORD_MASTER {
        /// <summary>
        /// 需要呼吸紀錄單
        /// </summary>
        public bool need_maintain { get; set; }
        /// <summary>
        /// 有呼吸紀錄單
        /// </summary>
        public bool has_maintain { get; set; }
        /// <summary>
        /// 呼吸紀錄單流水號
        /// </summary>
        public string tempid { get; set; }
        /// <summary>
        /// 呼吸器編號
        /// </summary>
        public string rt_no { get; set; }
        /// <summary>
        /// 批價序號
        /// </summary>
        public string fee_no { get; set; }
        /// <summary>
        /// 病歷號
        /// </summary>
        public string chr_no { get; set; }
        /// <summary>
        /// 資料
        /// </summary>
        public List<JSON_DATA> detail { get; set; }
    }

    #endregion

      
    /// <summary>交班率</summary>
    public class SHIFT_RATE {
        /// <summary>區域</summary>
        public string area_name { get; set; }
        /// <summary>應交班數</summary>
        public double must_shift_count { get; set; }
        /// <summary>交班數</summary>
        public double shift_count { get; set; }
        /// <summary>交班率</summary>
        public double shift_rate { get; set; }
    }

    /// <summary>交班資料DB</summary>
    public class SHIFT_DATA {
        /// <summary>住院帳號</summary>
        public string IPD_NO { get; set; }
        /// <summary>交班人ID</summary>
        public string SHIFT_ID { get; set; }
        /// <summary>交班人名稱</summary>
        public string SHIFT_NAME { get; set; }
        /// <summary>0:系統建立 1:暫存 2:交班</summary>
        public string STATUS { get; set; }
        /// <summary>護理站編號</summary>
        public string COST_CODE { get; set; }
    }
     
    /// <summary>
    /// 是否有修改資料的權限
    /// </summary>
    interface IEditPower_base
    {
        /// <summary>  是否有修改資料的權限  </summary>
        bool hasPowerEdit { set; get; }
    }
}

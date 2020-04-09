using RCS_Data.Models.DB;
using RCSData.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RCS_Data.Models.ViewModels
{
    public class SHIFTViewModels 
    {
        public IPDPatientInfo patInfo { get; set; }
        public RCS_CPT_DTL_NEW_ITEMS cptData { get; set; }
        public DB_RCS_RT_ISBAR_SHIFT model { get; set; }
        /// <summary>
        /// Introduction介紹(自我介紹與確認交班對象)
        /// </summary>
        public List<RCS_RT_ISBAR_SHIFT_S_VALUE> S_VALUE { get; set; }
        public List<string> HIS_DATA_UPLOAD_List { get; set; }
    }

    #region ISBAR交班

    public class RCS_RT_ISBAR_SHIFT  
    {
        /// <summary>流水號</summary>
        public string S_ID { get; set; }
        /// <summary>住院帳號</summary>
        public string IPD_NO { get; set; }
        /// <summary>病歷號</summary>
        public string chart_no { get; set; }
        /// <summary>Introduction</summary>
        public string I_VALUE { get; set; }
        /// <summary>Stituation</summary>
        public List<RCS_RT_ISBAR_SHIFT_S_VALUE> S_VALUE { get; set; }
        /// <summary>Background 背景(特殊用藥及治療情形)</summary>
        public List<PatOrder> B_VALUE { get; set; }
        /// <summary>Assessment</summary>s
        public RCS_A_VALUE A_VALUE { get; set; }
        /// <summary>Recommandation</summary>
        public List<JSON_DATA> R_VALUE { get; set; }
        /// <summary>交班人工號</summary>
        public string CREATE_ID { get; set; }
        /// <summary>交班人姓名</summary>
        public string CREATE_NAME { get; set; }
        /// <summary> 交班人時間</summary>
        public string CREATE_DATE { get; set; }
        /// <summary>接班人工號</summary>
        public string SHIFT_ID { get; set; }
        /// <summary>接班人姓名</summary>
        public string SHIFT_NAME { get; set; }
        /// <summary>接班人時間</summary>
        public string SHIFT_DATE { get; set; }
        /// <summary>交班狀態{0:系統建立 1:暫存 2:交班}</summary>
        public string STATUS { get; set; }
        /// <summary>呼吸紀錄單 ID</summary>
        public string trUR_ID { get; set; }

        public bool hasEditRWC { get; set; }

        /// <summary>Background 背景(過去病史/入院經過)</summary>
        public string B_VALUE_1 { get; set; }
        /// <summary>Background背景(病情處置追蹤)</summary>
        public string B_VALUE_2 { get; set; }

        /// <summary>原 Background 背景(過去病史/入院經過)</summary>
        public string B_VALUE_1_old { get; set; }
        /// <summary>原 Background背景(病情處置追蹤)</summary>
        public string B_VALUE_2_old { get; set; }

        //更新轉床用
        /// <summary>床號</summary>
        public string bed_no { get; set; }
        /// <summary>院區</summary>
        public string loc { get; set; }
        /// <summary>資料類型(C=照護病患清單，H=歷史病患清單)</summary>
        public string type_mode { get; set; }
        public string patient_name { get; set; }

        /// <summary>清單備註</summary>
        public string list_memo { get; set; }
        /// <summary>醫院跨團隊備註</summary>
        public string his_memo { get; set; }
        /// <summary>醫院跨團隊備註</summary>
        public string HIS_DATA { get; set; }

    }

    /// <summary> S value </summary>
    public class RCS_RT_ISBAR_SHIFT_S_VALUE
    {
        public string name { set; get; }
        public string data { set; get; }
    }

    /// <summary> 交班表A_VALUE物件 </summary>
    public class RCS_A_VALUE
    {
        /// <summary> 無法脫離呼吸器原因 </summary>
        public List<JSON_DATA> tk_reason { set; get; }
        /// <summary> 停止呼吸器脫離的原因 </summary>
        public List<JSON_DATA> st_reason { set; get; }
    }

    #endregion
}

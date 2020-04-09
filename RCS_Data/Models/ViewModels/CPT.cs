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
    public class RCS_CPT_DTL_NEW_ITEMS : RCS_RTAsses  
    {
        /// <summary> VPN/機構 </summary>
        public string VPN_mechanism { set; get; }

        
        /// <summary> VPN </summary>
        public List<JSON_DATA> from_VPN { set; get; }
        /// <summary> 轉入前病房分類 </summary>
        public List<JSON_DATA> from_unit { set; get; }
        /// <summary> 胸腔病史 </summary>
        public List<JSON_DATA> cpt_history { set; get; }
        /// <summary> 抽菸史 </summary>
        public List<JSON_DATA> smoke_history { set; get; }

        public string history_diag { set; get; }

        /// <summary> 使用呼吸器原因 </summary>
        public List<JSON_DATA> rt_reason { set; get; }
        /// <summary> 呼吸氣道狀況 </summary>
        public List<JSON_DATA> brief_status { set; get; }
        /// <summary> 手術 </summary>
        public List<JSON_DATA> operation { set; get; }

        /// <summary> 麻醉方式 </summary>
        public List<JSON_DATA> hocus { set; get; }
        /// <summary> ABG </summary>
        public List<JSON_DATA> abg_data { set; get; }
        /// <summary> 肺功能檢查 </summary>
        public List<JSON_DATA> lung { set; get; }
        /// <summary> 肺功能檢查_結論 </summary>
        public string lung_conclusion { set; get; }
        /// <summary> 體溫、脈搏、呼吸、BP、體溫 </summary>
        public List<JSON_DATA> base_data { set; get; }
        /// <summary> 意識 </summary>
        public List<JSON_DATA> conscious { set; get; }
        /// <summary> 四肢末梢 </summary>
        public List<JSON_DATA> tip { set; get; }

        /// <summary> 皮膚 </summary>
        public List<JSON_DATA> skin { set; get; }
        /// <summary> 人工氣道 </summary>
        public List<JSON_DATA> tube { set; get; }
        /// <summary> 使用呼吸器 </summary>
        public List<JSON_DATA> machine { set; get; }
        /// <summary> 呼吸型態 </summary>
        public List<JSON_DATA> patterns { set; get; }
        /// <summary> 肺擴張 </summary>
        public List<JSON_DATA> atelectasis { set; get; }
        /// <summary> 呼吸音 </summary>
        public List<JSON_DATA> breath_sound { set; get; }
        /// <summary> 咳嗽能力 </summary>
        public List<JSON_DATA> cough { set; get; }
        /// <summary> 是否需抽痰 </summary>
        public List<JSON_DATA> sputum { set; get; }
        /// <summary> 痰液評估 </summary>
        public List<JSON_DATA> sputum_assess { set; get; }
        /// <summary> 病人主要呼吸問題 </summary>
        public List<JSON_DATA> pat_problem { set; get; }

        /// <summary> CXR存取內容 </summary>
        public string obj { set; get; }
        /// <summary> CXR圖片 </summary>
        public string imageData { set; get; }
        /// <summary> CXR讀取內容 RCS_CXR_JSON </summary>
        public List<CxrResultJson_cls> CXR_result_json { set; get; }
        public string CXR_result_json_str { set; get; }

        /// <summary>
        /// 胸廓
        /// </summary>
        public List<JSON_DATA> thorax { set; get; }

         
        public string CREATE_ID { set; get; } 
        public string CREATE_NAME { set; get; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RCS_Data.Models.ViewModels
{
    public class Ventilator : Vicrt_CCH
    {

        public string ONMODE_TYPE3 { set; get; }
        /// <summary>
        /// 呼吸器表單流水號
        /// </summary>
        public string ONMODE_ID_2 { set; get; }
        /// <summary> RECORD_ID </summary>
        public string RECORD_ID { set; get; }
        /// <summary> 記錄日期時間 </summary>
        public string RECORDDATE
        {
            get
            {
                return string.Format("{0} {1}".Trim(), recorddate, recordtime);
            }
        }
        /// <summary> 是否有資料 </summary>
        public bool hasData { set; get; }
        public string model { set; get; }
        public string status_desc { set; get; }
        public TagColor tag_color { set; get; }
    }

    public class RTVentilator : Ventilator
    {
        public string ipd_no { set; get; }
        public string chart_no { set; get; }

    }

}

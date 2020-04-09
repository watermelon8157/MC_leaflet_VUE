using RCSData.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RCS_Data.Models.ViewModels
{
    public class ExamView
    {
        /// <summary>回傳訊息</summary>
        public RESPONSE_MSG RESPONSE_MSG { get; set; }
        /// <summary>
        /// 項目、檢體、單位等項目
        /// </summary>
        public List<RCS_ExamData_Common> thHeadList { get; set; }
        /// <summary>
        /// 資料來源
        /// </summary>
        public List<ExamViewList> List { get; set; }
    }

    public class ExamViewList
    {
        /// <summary> 檢驗日期(yyyy-MM-dd) </summary>
        public string exam_date { set; get; }
        /// <summary>
        /// 檢驗資料
        /// </summary>
        public List<RCS_ExamData_Common> dataList { get; set; }

    }
}

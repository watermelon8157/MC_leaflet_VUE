using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RCS_Data.Models.ViewModels
{
    public class OnePagePDF
    {
        /// <summary>
        ///  電子病歷編號  可以用這個歸戶同一個報告
        /// </summary>
        public string emrid { get; set; }
        /// <summary>
        /// 電子報告名稱 可以直接用這個呈現在 STORY 上 這樣子查找人才知道他要看的報告
        /// </summary>
        public string emrname { get; set; }
        /// <summary>
        /// URL
        /// </summary>
        public string url { get; set; }
        /// <summary>
        /// 批價序號
        /// </summary>
        public string fee_no { get; set; } 
    }
}

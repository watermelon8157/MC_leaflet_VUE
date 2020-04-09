using RCS_Data.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RCS_Data.Models.ViewModels
{
    public class ListHistoryViewModels 
    {
        public ListHistoryViewModels()
        {
            this.docList = new List<DDLItem>();
            this.bedList = new List<DDLItem>();
        } 

        public List<DDLItem> docList { get; set; }
        public List<DDLItem> bedList { get; set; }

        /// <summary>
        /// 帶入批價序號
        /// </summary>
        public string ipd_no { get; set; }
        /// <summary>
        /// 查詢病歷號
        /// </summary>
        public string chart_no { get; set; }
        /// <summary>
        /// 查詢入院日期起
        /// </summary>
        public string searchipdSDate { get; set; }
        /// <summary>
        /// 查詢入院日期迄
        /// </summary>
        public string searchipdEDate { get; set; }
        /// <summary>
        /// 主治醫生
        /// </summary>
        public string vs_doc { get; set; }
        /// <summary>
        /// 護理站
        /// </summary>
        public string cost_center { get; set; }
    }
}

using RCS_Data;
using RCS_Data.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RCS.Models
{
    /// <summary>
    /// VIP畫面model
    /// </summary>
    public class VIPViewModel
    {
        /// <summary>
        /// 病歷號
        /// </summary>
        public string patient_id { get; set; }
        /// <summary>
        /// 顯示畫面
        /// </summary>
        public string viewPage { get; set; }
        /// <summary>
        /// 院內所有呼吸器編號
        /// </summary>
        public RESP_COLLECTION resplist { get; set; }
        /// <summary>
        /// 設定清單
        /// </summary>
        public SysParamCollection sys_params_list { get; set; }

        public string lastRecordID { get; set; }
    }
}
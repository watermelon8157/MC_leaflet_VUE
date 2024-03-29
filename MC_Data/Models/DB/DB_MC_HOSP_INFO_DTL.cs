﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;

namespace RCS_Data.Models.DB
{ 
    public static partial class DB_TABLE_NAME
    {
        /// <summary>
        /// DB_MC_HOSP_INFO_DTL
        /// </summary>
        public static string DB_MC_HOSP_INFO_DTL { get { return Function_Library.GetClassDisplayName<DB_MC_HOSP_INFO_DTL>(); } }
    }

    [DisplayName("MC_HOSP_INFO_DTL")]

    public class DB_MC_HOSP_INFO_DTL : T4Json
    {
        [Key]
        public string HOSP_KEY { get; set; }
        [Key]
        public string SITE_ID { get; set; }
        public string CREATE_ID { get; set; }
        public string CREATE_NAME { get; set; }
        public string CREATE_DATE { get; set; }
        public string MODIFY_ID { get; set; }
        public string MODIFY_NAME { get; set; }
        public string MODIFY_DATE { get; set; }
        public string DATASTATUS { get; set; }
        public string CV { get; set; }
        public string PS { get; set; }
        public string SEVERE_PAT { get; set; }
        public string MODERATE_PAT { get; set; }
        public string MILD_PAT { get; set; }
        public string SOURCE { get; set; }


    }

    /// <summary>
    /// 畫面顯示資料
    /// </summary>
    public class VIEW_DB_HOSP_INFO_DTL: DB_MC_HOSP_INFO_DTL
    {
             }
}

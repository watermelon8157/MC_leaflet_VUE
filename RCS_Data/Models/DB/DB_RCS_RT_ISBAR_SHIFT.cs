using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;

namespace RCS_Data.Models.DB
{
    public static partial class DB_TABLE_NAME
    {
        public static string DB_RCS_RT_ISBAR_SHIFT { get { return Function_Library.GetClassDisplayName<DB_RCS_RT_ISBAR_SHIFT>(); } }

    }

    [DisplayName("RCS_RT_ISBAR_SHIFT")]
    public class DB_RCS_RT_ISBAR_SHIFT
    {
        [Key]
        public string ISBAR_ID { get; set; }
        public string IPD_NO { get; set; }
        public string CHART_NO { get; set; }
        public string I_VALUE { get; set; }
        public string S_VALUE { get; set; }
        public string B_VALUE { get; set; }
        public string A_VALUE { get; set; }
        public string R_VALUE { get; set; }
        public string CREATE_ID { get; set; }
        public string CREATE_NAME { get; set; }
        public string CREATE_DATE { get; set; }
        public string SHIFT_ID { get; set; }
        public string SHIFT_NAME { get; set; }
        public string SHIFT_DATE { get; set; }
        public string STATUS { get; set; }
        public string DATASTATUS { get; set; }
        public string trUR_ID { get; set; }
        public string B_VALUE_1 { get; set; }
        public string B_VALUE_2 { get; set; }
        public string MODIFY_ID { get; set; }
        public string MODIFY_NAME { get; set; }
        public string MODIFY_DATE { get; set; }
        public string LIST_MEMO { get; set; }
        public string HIS_MEMO { get; set; }
        public string UPLOAD_STATUS { get; set; }
        public string UPLOAD_ID { get; set; }
        public string UPLOAD_NAME { get; set; }
        public string UPLOAD_DATE { get; set; }
        public string HIS_DATA { get; set; }
        public string UPLOAD_PGUID { get; set; }


    }
}

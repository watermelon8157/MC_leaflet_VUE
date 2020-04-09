using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace RCS_Data.Models.DB
{
    public static partial class DB_TABLE_NAME
    {
        public static string DB_RCS_RT_MV_DAY { get { return Function_Library.GetClassDisplayName<DB_RCS_RT_MV_DAY>(); } }
    }

    [DisplayName("RCS_RT_MV_DAY")]
    public class DB_RCS_RT_MV_DAY
    {
        [Key]
        public string IPD_NO { get; set; }
        [Key]
        public string CHART_NO { get; set; }
        [Key]
        public string RECORDDATE { get; set; }
        public string MV_DAY { get; set; }
    }

     
    public class DB_RCS_ON_MODE
    {
        public string RECORD_ID { get; set; }
        public string recorddate { get; set; }
        public string artificial_airway_type { get; set; }
        public string respid { get; set; } 
    }
}
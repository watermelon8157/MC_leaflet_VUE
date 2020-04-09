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
        public static string DB_RCS_VENTILATOR_SCHEDULING_CHECKLIST { get { return Function_Library.GetClassDisplayName<DB_RCS_VENTILATOR_SCHEDULING_CHECKLIST>(); } }
    }

    [DisplayName("RCS_VENTILATOR_SCHEDULING_CHECKLIST")]
    public class DB_RCS_VENTILATOR_SCHEDULING_CHECKLIST
    {
        [Key]
        public string V_ID { get; set; }
        public string RECORD_DATE { get; set; }
        public string MV_NO { get; set; }
        public string V_TYPE { get; set; }
        public string V_STATUS { get; set; }
        public string MEMO { get; set; }
        public string CREATE_ID { get; set; }
        public string CREATE_NAME { get; set; }
        public string CREATE_DATE { get; set; }
        public string MODIFY_ID { get; set; }
        public string MODIFY_NAME { get; set; }
        public string MODIFY_DATE { get; set; }
        public string DATASTATUS { get; set; } 
        /// <summary>
        /// 呼吸器模式
        /// </summary>
        public string V_MODE { get; set; }


    }
}

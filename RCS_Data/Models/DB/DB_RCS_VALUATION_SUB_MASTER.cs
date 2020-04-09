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
        public static string DB_RCS_VALUATION_SUB_MASTER { get { return Function_Library.GetClassDisplayName<DB_RCS_VALUATION_SUB_MASTER>(); } }

    }

    [DisplayName("RCS_VALUATION_SUB_MASTER")]

    public class DB_RCS_VALUATION_SUB_MASTER
    {
        [Key]
        public string V_SUB_ID { get; set; }
        [Key]
        public string V_ID { get; set; }

        public string WORK_TYPE { get; set; }
    } 

    public class DB_RCS_VALUATION_SUB_MASTER_BY_V_SUB_ID
    {
        [Key]
        public string V_SUB_ID { get; set; }

        public string V_ID { get; set; }

        public string WORK_TYPE { get; set; }
    }
}

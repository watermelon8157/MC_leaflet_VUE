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
        public static string DB_RCS_VALUATION_MASTER { get { return Function_Library.GetClassDisplayName<DB_RCS_VALUATION_MASTER>(); } }

    }

    [DisplayName("RCS_VALUATION_MASTER")]

    public class DB_RCS_VALUATION_MASTER
    {
        [Key]
        public string V_ID { get; set; }
        public string IPD_NO { get; set; }
        public string CHART_NO { get; set; }
        public string RECORD_DATE { get; set; }
        public string CREATE_ID { get; set; }
        public string CREATE_NAME { get; set; }
        public string CREATE_DATE { get; set; }
        public string MODIFY_ID { get; set; }
        public string MODIFY_NAME { get; set; }
        public string MODIFY_DATE { get; set; }
        public string DATASTATUS { get; set; }
        public string UPLOAD_STATUS { get; set; }
        public string UPLOAD_ID { get; set; }
        public string UPLOAD_NAME { get; set; }
        public string UPLOAD_DATE { get; set; }
        public string NEW_V_ID { get; set; }

    }
     
}

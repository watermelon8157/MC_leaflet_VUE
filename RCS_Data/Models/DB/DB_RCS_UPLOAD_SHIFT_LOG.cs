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
        public static string DB_RCS_UPLOAD_SHIFT_LOG { get { return Function_Library.GetClassDisplayName<DB_RCS_UPLOAD_SHIFT_LOG>(); } }
    }

    [DisplayName("RCS_UPLOAD_SHIFT_LOG")]
    public class DB_RCS_UPLOAD_SHIFT_LOG
    {
        [Key] 
        public string TEMP_ID { set; get; }
        public string RECORD_ID { set; get; } 
        public string CREATE_ID { set; get; }
        public string CREATE_NAME { set; get; }
        public string CREATE_DATE { set; get; }

    }
 
}

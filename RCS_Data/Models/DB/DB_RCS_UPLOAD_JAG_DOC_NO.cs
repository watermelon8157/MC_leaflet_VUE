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
        /// <summary>
        /// DB_RCS_CPT_ASS_MASTER
        /// </summary>
        public static string DB_RCS_UPLOAD_JAG_DOC_NO { get { return Function_Library.GetClassDisplayName<DB_RCS_UPLOAD_JAG_DOC_NO>(); } }
    }

    [DisplayName("RCS_UPLOAD_JAG_DOC_NO")]
    public class DB_RCS_UPLOAD_JAG_DOC_NO
    {
        [Key]
        public string TEMP_ID { get; set; }
        public string RECORD_ID { get; set; }
        public string DOC_NO { get; set; }
    }
}

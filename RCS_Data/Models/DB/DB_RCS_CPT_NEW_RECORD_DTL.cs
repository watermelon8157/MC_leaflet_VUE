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
        public static string DB_RCS_CPT_NEW_RECORD_DTL { get { return Function_Library.GetClassDisplayName<DB_RCS_CPT_NEW_RECORD_DTL>(); } }
    }

    [DisplayName("RCS_CPT_NEW_RECORD_DTL")]

    public class DB_RCS_CPT_NEW_RECORD_DTL
    {
        [Key]
        public string CPT_ID { get; set; }
        [Key]
        public string ITEM_NAME { get; set; }

        public string ITEM_VALUE { get; set; }

    }

    [DisplayName("RCS_CPT_NEW_RECORD_DTL")]
    public class DB_RCS_CPT_NEW_RECORD_DTL_BY_CPT_ID
    {
        [Key]
        public string CPT_ID { get; set; }
        public string ITEM_NAME { get; set; }

        public string ITEM_VALUE { get; set; }

    }
}

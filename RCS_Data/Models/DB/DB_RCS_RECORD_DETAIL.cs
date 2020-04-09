using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace RCS_Data.Models.DB
{
    public static partial class DB_TABLE_NAME
    {
        public static string DB_RCS_RECORD_DETAIL { get { return Function_Library.GetClassDisplayName<DB_RCS_RECORD_DETAIL>(); } }
        /// <summary>
        /// RECORD_ID ONLY KEY
        /// </summary>
        public static string DB_RCS_RECORD_DETAIL_BY_RECORD_ID { get { return Function_Library.GetClassDisplayName<DB_RCS_RECORD_DETAIL_BY_RECORD_ID>(); } }
    }

    [DisplayName("RCS_RECORD_DETAIL")]
    public class DB_RCS_RECORD_DETAIL
    {
        [Key]
        public string RECORD_ID { get; set; }
        [Key]
        public string ITEM_NAME { get; set; }
        public string ITEM_VALUE { get; set; }

    }

    [DisplayName("RCS_RECORD_DETAIL")]
    public class DB_RCS_RECORD_DETAIL_BY_RECORD_ID
    {
        [Key]
        public string RECORD_ID { get; set; }
        public string ITEM_NAME { get; set; }
        public string ITEM_VALUE { get; set; }

    }
}
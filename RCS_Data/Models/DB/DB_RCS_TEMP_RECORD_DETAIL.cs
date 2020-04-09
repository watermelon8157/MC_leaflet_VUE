
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace RCS_Data.Models.DB
{
    public static partial class DB_TABLE_NAME
    {
        /// <summary>
        /// DB_RCS_TEMP_RECORD_MASTER
        /// </summary>
        public static string DB_RCS_TEMP_RECORD_DETAIL { get { return Function_Library.GetClassDisplayName<DB_RCS_TEMP_RECORD_DETAIL>(); } }
    }

    [DisplayName("RCS_TEMP_RECORD_DETAIL")]
    public class DB_RCS_TEMP_RECORD_DETAIL
    {
        [Key]
        public string TEMP_ID { get; set; }
        [Key]
        public string RECORD_ID { get; set; }
        [Key]
        public string ITEM_NAME { get; set; }
        public string ITEM_VALUE { get; set; }

    }
}


using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace RCS_Data.Models.DB
{
    public static partial class DB_TABLE_NAME
    {
        public static string DB_RCS_CPT_REQ_DETAIL { get { return Function_Library.GetClassDisplayName<DB_RCS_CPT_REQ_DETAIL>(); } }
        /// <summary>
        /// cpt_id ONLY KEY
        /// </summary>
        public static string DB_RCS_CPT_REQ_DEL_BY_CPT_ID { get { return Function_Library.GetClassDisplayName<DB_RCS_CPT_REQ_DEL_BY_CPT_ID>(); } }
    }

    [DisplayName("RCS_CPT_REQ_DETAIL")]
    public class DB_RCS_CPT_REQ_DETAIL
    {
        [Key]
        /// <summary> 主檔編號 </summary>
        public string RECORD_ID { set; get; }
        [Key]
        /// <summary> 項目名稱 </summary>
        public string ITEM_NAME { set; get; }

        /// <summary> 項目值 </summary>
        public string ITEM_VALUE { set; get; }

    }


    [DisplayName("RCS_CPT_REQ_DETAIL")]
    public class DB_RCS_CPT_REQ_DEL_BY_CPT_ID
    {
        [Key]
        /// <summary> 主檔編號 </summary>
        public string RECORD_ID { set; get; }

        /// <summary> 項目名稱 </summary>
        public string ITEM_NAME { set; get; }

        /// <summary> 項目值 </summary>
        public string ITEM_VALUE { set; get; }

    }

}

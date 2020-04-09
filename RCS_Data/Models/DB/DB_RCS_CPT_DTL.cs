
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace RCS_Data.Models.DB
{
    public static partial class DB_TABLE_NAME
    {
        public static string DB_RCS_CPT_DTL { get { return Function_Library.GetClassDisplayName<DB_RCS_CPT_DTL>(); } }
        /// <summary>
        /// cpt_id ONLY KEY
        /// </summary>
        public static string DB_RCS_CPT_DTL_BY_CPT_ID { get { return Function_Library.GetClassDisplayName<DB_RCS_CPT_DTL_BY_CPT_ID>(); } }
    }

    [DisplayName("RCS_CPT_DTL")]
    public class DB_RCS_CPT_DTL
    {
        [Key]
        /// <summary> 主檔編號 </summary>
        public string cpt_id { set; get; }
        [Key]
        /// <summary> 項目名稱 </summary>
        public string cpt_item { set; get; }

        /// <summary> 項目值 </summary>
        public string cpt_value { set; get; }

    }


    [DisplayName("RCS_CPT_DTL")]
    public class DB_RCS_CPT_DTL_BY_CPT_ID
    {
        [Key]
        /// <summary> 主檔編號 </summary>
        public string cpt_id { set; get; }

        /// <summary> 項目名稱 </summary>
        public string cpt_item { set; get; }

        /// <summary> 項目值 </summary>
        public string cpt_value { set; get; }

    }

    public static partial class DB_TABLE_NAME
    {
        /// <summary>
        /// DB_RCS_CPT_ASS_DETAIL
        /// </summary>
        public static string DB_RCS_CPT_ASS_DETAIL { get { return Function_Library.GetClassDisplayName<DB_RCS_CPT_ASS_DETAIL>(); } }
    }

    [DisplayName("RCS_CPT_ASS_DETAIL")]
    public class DB_RCS_CPT_ASS_DETAIL
    {
        [Key]
        public string CPT_ID { get; set; }
        public string CPT_ITEM { get; set; }
        public string CPT_VALUE { get; set; }

    }
}

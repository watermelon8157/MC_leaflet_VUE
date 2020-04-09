using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace RCS_Data.Models.DB
{
    public static partial class DB_TABLE_NAME
    {
        public static string DB_RCS_SYS_PARAMS { get { return Function_Library.GetClassDisplayName<DB_RCS_SYS_PARAMS>(); } }
    }

    [DisplayName("RCS_SYS_PARAMS")]
    public class DB_RCS_SYS_PARAMS
    {
        [Key]
        public string P_ID { get; set; }
        public string P_MODEL { get; set; }
        public string P_GROUP { get; set; }
        public string P_NAME { get; set; }
        public string P_VALUE { get; set; }
        public string P_LANG { get; set; }
        public string P_SORT { get; set; }
        public string P_MEMO { get; set; }
        public string P_STATUS { get; set; }
        public string P_MANAGE { get; set; }
        public string MODIFY_ID { get; set; }
        public string MODIFY_NAME { get; set; }
        public string MODIFY_DATE { get; set; }

    }

    /// <summary> RCS_SYS_PARAMS 參數類別 </summary>
    public enum GetP_MODEL
    {
        /// <summary> 使用者 </summary>
        user,
        /// <summary> 成員 </summary>
        role,
        /// <summary> 表單類型 </summary>
        rt_form,
        /// <summary> 表單類型 </summary>
        phrase
    }
}

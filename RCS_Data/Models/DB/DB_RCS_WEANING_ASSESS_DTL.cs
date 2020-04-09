
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace RCS_Data.Models.DB
{
    public static partial class DB_TABLE_NAME
    {
        public static string DB_RCS_WEANING_ASSESS_DTL { get { return Function_Library.GetClassDisplayName<DB_RCS_WEANING_ASSESS_DTL>(); } }
      
    }

    [DisplayName("RCS_WEANING_ASSESS_DTL")]

    public class DB_RCS_WEANING_ASSESS_DTL
    {
        [Key]
        public string TK_ID { get; set; }

        public string ITEM_NAME { get; set; }

        public string ITEM_VALUE { get; set; }
    }
}

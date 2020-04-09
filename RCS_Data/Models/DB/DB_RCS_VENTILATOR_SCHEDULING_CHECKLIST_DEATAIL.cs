
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace RCS_Data.Models.DB
{
    public static partial class DB_TABLE_NAME
    {
        public static string DB_RCS_VENTILATOR_SCHEDULING_CHECKLIST_DEATAIL { get { return Function_Library.GetClassDisplayName<DB_RCS_VENTILATOR_SCHEDULING_CHECKLIST_DEATAIL>(); } }
    }

    [DisplayName("RCS_VENTILATOR_SCHEDULING_CHECKLIST_DEATAIL")]
    public class DB_RCS_VENTILATOR_SCHEDULING_CHECKLIST_DEATAIL
    {
        [Key]
        public string V_ID { get; set; }

        public string ITEM_NAME { get; set; }

        public string ITEM_VALUE { get; set; }

    }
}

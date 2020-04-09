 
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace RCS_Data.Models.DB
{
    public static partial class DB_TABLE_NAME
    {
        public static string DB_RCS_VENTILATOR_SETTINGS { get { return Function_Library.GetClassDisplayName<DB_RCS_VENTILATOR_SETTINGS>(); } }
    }

    [DisplayName("RCS_VENTILATOR_SETTINGS")]
    public class DB_RCS_VENTILATOR_SETTINGS
    {
        [Key]
        public string DEVICE_SEQ { get; set; }
        public string DEVICE_NO { get; set; }
        public string ROOM { get; set; }
        public string DEVICE_MODEL { get; set; }
        public string CREATE_ID { get; set; }
        public string CREATE_NAME { get; set; }
        public string CREATE_DATE { get; set; }
        public string MODIFY_ID { get; set; }
        public string MODIFY_NAME { get; set; }
        public string MODIFY_DATE { get; set; }
        public string USE_STATUS { get; set; }
        public string PURCHASE_DATE { get; set; }


    }
}

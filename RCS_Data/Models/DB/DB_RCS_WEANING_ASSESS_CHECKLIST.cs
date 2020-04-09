using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace RCS_Data.Models.DB
{
    public static partial class DB_TABLE_NAME
    {
        public static string DB_RCS_WEANING_ASSESS_CHECKLIST { get { return Function_Library.GetClassDisplayName<DB_RCS_WEANING_ASSESS_CHECKLIST>(); } }
         
    }

    [DisplayName("RCS_WEANING_ASSESS_CHECKLIST")]
    public class DB_RCS_WEANING_ASSESS_CHECKLIST 
    {
        [Key]
        public string IPD_NO { get; set; }
        [Key]
        public string CHART_NO { get; set; }

        public string RWAC01 { get; set; }

        public string RWAC02 { get; set; }

        public string RWAC03 { get; set; }

        public string RWAC04 { get; set; }

        public string RWAC05 { get; set; }

        public string RWAC06 { get; set; }

        public string RWAC07 { get; set; }

        public string RWAC08 { get; set; }

        public string RWAC09 { get; set; }

        public string DATASTATUS { get; set; }

        public string CREATE_ID { get; set; }

        public string CREATE_NAME { get; set; }
        [Key]
        public string CREATE_DATE { get; set; }

        public string MODIFY_ID { get; set; }

        public string MODIFY_NAME { get; set; }

        public string MODIFY_DATE { get; set; }
         

    }//DB_RCS_WEANING_ASSESS_CHECKLIST
}

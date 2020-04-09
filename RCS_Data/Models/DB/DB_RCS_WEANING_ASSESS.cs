using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace RCS_Data.Models.DB
{
    public static partial class DB_TABLE_NAME
    {
        /// <summary>
        /// DB_RCS_WEANING_ASSESS
        /// </summary>
        public static string DB_RCS_WEANING_ASSESS { get { return Function_Library.GetClassDisplayName<DB_RCS_WEANING_ASSESS>(); } }
    }

    [DisplayName("RCS_WEANING_ASSESS")]
    public class DB_RCS_WEANING_ASSESS
    {
        [Key]
        public string TK_ID { get; set; }
        public string IPD_NO { get; set; }
        public string CHART_NO { get; set; }
        public string REC_DATE { get; set; }
        public string TK_REASON { get; set; }
        public string ST_REASON { get; set; }
        public string TK_PLAN { get; set; }
        public string CREATE_DATE { get; set; }
        public string CREATE_ID { get; set; }
        public string CREATE_NAME { get; set; }
        public string MODIFY_DATE { get; set; }
        public string MODIFY_ID { get; set; }
        public string MODIFY_NAME { get; set; }
        public string DATASTATUS { get; set; }
        public string UPLOAD_STATUS { get; set; }
        public string UPLOAD_ID { get; set; }
        public string UPLOAD_NAME { get; set; }
        public string UPLOAD_DATE { get; set; }
        public string NEW_TK_ID { get; set; }
        public string PAT_SOURCE { get; set; }
        public string PAT_DATA_DATE { get; set; }
        public string UPLOAD_FILENAME { get; set; }
        public string ONMODE_ID { get; set; }
        public string COST_CODE { get; set; }
        public string BED_NO { get; set; }
        public string DEPT_CODE { get; set; }
    }
}

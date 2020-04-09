using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;

namespace RCS_Data.Models.DB
{
 

    public static partial class DB_TABLE_NAME
    {
        /// <summary>
        /// DB_RCS_CPT_ASS_MASTER
        /// </summary>
        public static string DB_RCS_CPT_NEW_ASS { get { return Function_Library.GetClassDisplayName<DB_RCS_CPT_NEW_ASS>(); } }
    }

    [DisplayName("RCS_CPT_NEW_ASS")]

    public class DB_RCS_CPT_NEW_ASS
    {
        [Key]
        public string CPT_ID { get; set; }
        public string REC_DATE { get; set; }
        public string IPD_NO { get; set; }
        public string CHART_NO { get; set; }
        public string CREATE_ID { get; set; }
        public string CREATE_NAME { get; set; }
        public string CREATE_DATE { get; set; }
        public string MODIFY_ID { get; set; }
        public string MODIFY_NAME { get; set; }
        public string MODIFY_DATE { get; set; }
        public string DATASTATUS { get; set; }
        public string UPLOAD_STATUS { get; set; }
        public string UPLOAD_ID { get; set; }
        public string UPLOAD_NAME { get; set; }
        public string UPLOAD_DATE { get; set; }
        public string NEW_CPT_ID { get; set; }
        public string PAT_SOURCE { get; set; }
        public string PAT_DATA_DATE { get; set; }
        public string UPLOAD_FILENAME { get; set; }
        public string COST_CODE { get; set; }
        public string BED_NO { get; set; }
        public string DEPT_CODE { get; set; }


    }


}

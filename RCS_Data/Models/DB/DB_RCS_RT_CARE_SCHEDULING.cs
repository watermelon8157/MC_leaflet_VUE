using RCS_Data.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace RCS_Data.Models.DB
{
    public static partial class DB_TABLE_NAME
    {
        public static string DB_RCS_RT_CARE_SCHEDULING { get { return Function_Library.GetClassDisplayName<DB_RCS_RT_CARE_SCHEDULING>(); } }
    }

    [DisplayName("RCS_RT_CARE_SCHEDULING")]
    public class DB_RCS_RT_CARE_SCHEDULING
    {
        [Key]
        public string CARE_ID { get; set; }
        public string RT_ID { get; set; }
        public string CHART_NO { get; set; }
        public string IPD_NO { get; set; }
        public string PATIENT_IDNO { get; set; }
        public string PATIENT_NAME { get; set; }
        public string GENDER { get; set; }
        public string BIRTH_DAY { get; set; }
        public string COST_CODE { get; set; }
        public string BED_NO { get; set; }
        public string DIAG_DATE { get; set; }
        public string CARE_DATE { get; set; }
        public string CREATE_DATE { get; set; }
        public string VS_DOC_NAME { get; set; }
        public string PRE_DISCHARGE_DATE { get; set; }
        public string ROOM_NO { get; set; }
        public string LOC { get; set; }
        public string TYPE_MODE { get; set; }
        public string VS_DOC_ID { get; set; }
        public string NHI_OPD_ER { get; set; }
        public string VS_DAT { get; set; }

    }
}
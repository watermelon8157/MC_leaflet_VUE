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
        public static string DB_RCS_RT_CASE { get { return Function_Library.GetClassDisplayName<DB_RCS_RT_CASE>(); } }
    }

    [DisplayName("RCS_RT_CASE")]
    public class DB_RCS_RT_CASE
    {
        [Key]
        public string CASE_ID { get; set; }
        public string CHART_NO { get; set; }
        public string IPD_NO { get; set; }
        public string PATIENT_NAME { get; set; }
        public string ACCEPT_STATUS { get; set; }
        public string CREATE_ID { get; set; }
        public string CREATE_NAME { get; set; }
        public string CREATE_DATE { get; set; }
        public string MODIFY_ID { get; set; }
        public string MODIFY_NAME { get; set; }
        public string MODIFY_DATE { get; set; }
        public string CASE_DOC_ID { get; set; }
        public string BODY_HEIGHT { get; set; }
        public string COST_CODE { get; set; }
        public string VS_DOC_ID { get; set; }
        public string VS_DOC_NAME { get; set; }
        public string GENDER { get; set; }
        public string BIRTH_DAY { get; set; }
        public string BED_NO { get; set; }
        public string DIAG_DATE { get; set; }
        public string CARE_DATE { get; set; }
        public string PATIENT_SOURCE { get; set; }
        public string CPT_STATUS { get; set; }
        public string DIAG_DESC { get; set; }
        public string DEPT_CODE { get; set; }
        public string DEPT_NAME { get; set; }
        public string PATIENT_IDNO { get; set; }
        public string DNR_MARK { get; set; }
        public string VPN_MARK { get; set; }
        public string PRE_DISCHARGE_DATE { get; set; }
        public string ROOM_NO { get; set; }
        public string LOC { get; set; }
        public string BODY_WEIGHT { get; set; }
        public string VPN_PRINT { get; set; }
        public string OPD_COUNT { get; set; }
        public string TK_RESULT { get; set; }
        public string MDRO_MARK { get; set; }

    }
}
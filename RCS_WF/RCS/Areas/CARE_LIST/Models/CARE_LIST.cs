using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RCS.Areas.CARE_LIST.Models
{
    public class CARE_LIST
    {
        public CARE_LIST()
        {
            RCS_Data.SQLProvider SQL = new RCS_Data.SQLProvider();
            //P_NAME健保碼  P_VALUE說明
            this.sysParam = SQL.DBA.getSqlDataTable<RCS.Models.SysParams>("SELECT P_NAME, P_VALUE FROM RCS_SYS_PARAMS  WHERE P_MODEL = 'RT' AND P_GROUP = 'care_list'");
            //PATIENT_NAME姓名  BED_NO床號  CHART_NO病歷號
            this.patInfo = SQL.DBA.getSqlDataTable<PatInfo>("SELECT top 5 PATIENT_NAME, BED_NO, CHART_NO FROM RCS_RT_CASE");
        }

        /// <summary>
        /// 日期
        /// </summary>
        public string date { get; set; }
        /// <summary>
        /// 病患資料
        /// </summary>
        public List<PatInfo> patInfo { get; set; }
        /// <summary>
        /// 健保碼
        /// </summary>
        public List<RCS.Models.SysParams> sysParam { get; set; }
    }

    public class PatInfo : RCS_Data.PatientInfo
    {

    }
}
using RCSData.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace RCS_Data.Models.DB
{
    public static partial class DB_TABLE_NAME  
    {
        public static string DB_RCS_RECORD_MASTER { get { return Function_Library.GetClassDisplayName<DB_RCS_RECORD_MASTER>(); } }
        
    }


    public class DBA_RCS_RECORD_MASTER : IDBA<DB_RCS_RECORD_MASTER>
    {
        public string SQL_MAIN { get {
                return string.Concat(
                            "SELECT RECORDDATE as RECORD_DATE, * FROM ", DB_TABLE_NAME.DB_RCS_RECORD_MASTER, " WHERE RECORD_ID =@RECORD_ID;",
                            "SELECT * FROM ", DB_TABLE_NAME.DB_RCS_RECORD_DETAIL, " WHERE RECORD_ID =@RECORD_ID;");
            } }
        public List<DB_RCS_RECORD_MASTER> InsertTenpMaster(string record_id, string record_date, IPDPatientInfo pat_info, UserInfo user_info)
        {
            DateTime recordDate = DateTime.Parse(record_date);
            return new List<DB_RCS_RECORD_MASTER>() { new DB_RCS_RECORD_MASTER() {
                 RECORD_ID = record_id,
                 DATASTATUS = "2",
                 UPLOAD_STATUS = "0",
                 RECORDDATE = Function_Library.getDateString( recordDate,DATE_FORMAT.yyyy_MM_dd_HHmmss),
                 CREATE_DATE = Function_Library.getDateString( recordDate, DATE_FORMAT.yyyy_MM_dd_HHmmss),
                 CREATE_ID = user_info.user_id,
                 CREATE_NAME = user_info.user_name,
                 MODIFY_DATE = Function_Library.getDateString( recordDate, DATE_FORMAT.yyyy_MM_dd_HHmmss),
                 MODIFY_ID = user_info.user_id,
                 MODIFY_NAME = user_info.user_name,
                 IPD_NO = pat_info.ipd_no,
                 CHART_NO = pat_info.chart_no,
                 PAT_SOURCE = pat_info.source_type,
                 PAT_DATA_DATE = pat_info.diag_date,
                 BED_NO = pat_info.bed_no,
                 DEPT_CODE = pat_info.dept_code,
                 COST_CODE = pat_info.cost_code,
            } };
        }
    }

    [DisplayName("RCS_RECORD_MASTER")]
    public class DB_RCS_RECORD_MASTER
    {
        [Key]
        public string RECORD_ID { get; set; }
        public string CREATE_ID { get; set; }
        public string CREATE_NAME { get; set; }
        public string CREATE_DATE { get; set; }
        public string MODIFY_ID { get; set; }
        public string MODIFY_NAME { get; set; }
        public string MODIFY_DATE { get; set; }
        public string IPD_NO { get; set; }
        public string CHART_NO { get; set; }
        public string RECORDDATE { get; set; } 
        public string DATASTATUS { get; set; }
        public string UPLOAD_STATUS { get; set; }
        public string UPLOAD_ID { get; set; }
        public string UPLOAD_NAME { get; set; }
        public string UPLOAD_DATE { get; set; }
        public string NEW_RECORD_ID { get; set; }
        public string PAT_SOURCE { get; set; }
        public string PAT_DATA_DATE { get; set; }
        public string UPLOAD_FILENAME { get; set; }
        public string COST_CODE { get; set; }
        public string BED_NO { get; set; }
        public string DEPT_CODE { get; set; }



    }


}

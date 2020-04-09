using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using mayaminer.com.library;
using Com.Mayaminer;
using RCS_Data;
using RCS.Models.ViewModel;
using RCS_Data.Controllers.RT;
using RCS_Data.Models.ViewModels;

namespace RCS.Models.DB
{
    public class Shift : BaseModel
    {
        string csName { get { return "Shift"; } }
        /// <summary>
        /// 取得交班表
        /// </summary>
        /// <param name="pWhere">查詢條件</param>
        /// <returns></returns>
        public DataTable getShift(string pWhere)
        {
            DataTable dt = this.DBA.getDataTable(GetTableName.RCS_RT_ISBAR_SHIFT.ToString(), pWhere);
            return dt;
        }

        /// <summary>
        /// 取得交班清單
        /// </summary>
        /// <param name="pIpdNo">勾選的病人住院號</param>
        /// <returns></returns>
        public DataTable getShiftList(string pIpdNoStr)
        {
            string sql = "SELECT CREATE_ID,A.ipd_no,A.CHART_NO,ISBAR_ID,I_VALUE,S_VALUE,B_VALUE,A_VALUE ,R_VALUE ,B.CREATE_ID ,B.CREATE_NAME  ,B.CREATE_DATE ,SHIFT_ID,SHIFT_NAME,SHIFT_DATE,STATUS,B_VALUE_1,B_VALUE_2,BED_NO,LOC,TYPE_MODE"+
                " FROM " + GetTableName.RCS_RT_CARE_SCHEDULING + " A LEFT JOIN " + GetTableName.RCS_RT_ISBAR_SHIFT + " B ON A.IPD_NO = B.IPD_NO";
            sql += " WHERE A.IPD_NO IN (" + pIpdNoStr + ") AND A.RT_ID = '" + user_info.user_id + "' AND B.STATUS = '1'";
            sql += " ORDER BY B.create_date DESC";
            DataTable dt = this.DBA.getSqlDataTable(sql);
            return dt;
        }

        /// <summary>
        /// 取得該住院號下的已交班的記錄與當日RT的暫存紀錄
        /// </summary>
        /// <param name="pIpdNo">批價序號</param>
        /// <param name="pCreateDate">建立日期</param>
        /// <param name="pCreateId">建立者ID</param>
        /// <returns></returns>
        public DataTable getShiftTemp(string pIpdNo)
        {
            string sql = @"SELECT * FROM {1} WHERE IPD_NO = {0} ORDER BY CREATE_DATE DESC";
            DataTable dt = this.DBA.getSqlDataTable(string.Format(sql, SQLDefend.SQLString(pIpdNo),  GetTableName.RCS_RT_ISBAR_SHIFT.ToString()));
            return dt;
        }
        /// <summary>
        /// 取得上次入院交班資料
        /// </summary>
        /// <param name="pIpdNo">批價序號</param>
        /// <param name="pChartNo">病歷號</param>
        /// <param name="pDate">入院日期</param>
        /// <returns></returns>
        public DataTable getLastShift(string pChartNo, string pIpdNo, string pDate)
        {
            DataTable SHIFT_dt = new DataTable();

            string RT_CASE_sql = @"SELECT * FROM {3} WHERE CHART_NO = {0} AND (PATIENT_SOURCE = 'I' OR PATIENT_SOURCE = 'E') AND IPD_NO <> {1} AND DIAG_DATE <= {2} ORDER BY DIAG_DATE DESC";
            RT_CASE_sql = string.Format(RT_CASE_sql, SQLDefend.SQLString(pChartNo), SQLDefend.SQLString(pIpdNo), SQLDefend.SQLString(pDate), GetTableName.RCS_RT_CASE.ToString());
            DataTable RT_dt = this.DBA.getSqlDataTable(RT_CASE_sql);
            List<RCS_RT_ISBAR_SHIFT> rris = new List<RCS_RT_ISBAR_SHIFT>();
            
            if (RT_dt.Rows!=null && RT_dt.Rows.Count > 0)
            {
                foreach (DataRow Dr in RT_dt.Rows)
                {
                    rris.Add(new RCS_RT_ISBAR_SHIFT()
                    {
                        IPD_NO = Dr["IPD_NO"].ToString()
                    });
                }
                string RT_ISBAR_SHIFT_sql = @"SELECT * FROM {1} WHERE IPD_NO = {0} AND STATUS = '2' ORDER BY CREATE_DATE DESC";
                RT_ISBAR_SHIFT_sql = string.Format(RT_ISBAR_SHIFT_sql, SQLDefend.SQLString(rris[0].IPD_NO), GetTableName.RCS_RT_ISBAR_SHIFT.ToString());
                SHIFT_dt = this.DBA.getSqlDataTable(RT_ISBAR_SHIFT_sql);
            }
            return SHIFT_dt;
        }
        /// <summary>
        /// 取得上一筆交班記錄
        /// </summary>
        /// <param name="pIpdNo">批價序號</param>
        /// <remarks>之後會擺在pat_info</remarks>
        public DataTable getPrevShift(string pIpdNo,string user_id)
        {
        
             
            DataTable dt = new DataTable();
            string actionName = "getPrevShift";
            SQLProvider SQL = new SQLProvider();
            string sqlWhere = "";
            string str = string.Format("SELECT * FROM " + GetTableName.RCS_RT_ISBAR_SHIFT +
            " WHERE IPD_NO = '{0}' AND ( STATUS='2'" +" OR ( STATUS in('1') " + sqlWhere + ")) ORDER BY CREATE_DATE DESC",
            pIpdNo);
            dt = SQL.DBA.getSqlDataTable(str);
            if (SQL.DBA.hasLastError)
            {
                LogTool.SaveLogMessage(SQL.DBA.lastError, actionName, this.csName);
            }
            return dt;
        }


        /// <summary>
        /// 取得最後一筆呼吸器脫離評估資料
        /// </summary>
        /// <param name="pIpdNo">批價序號</param>   
        /// <returns></returns>
        public DataTable getFinalWeaning(string pChartNo)
        {
            string sql = @"SELECT * FROM {1} WHERE CHART_NO = {0} ORDER BY CREATE_DATE DESC";
            DataTable dt = this.DBA.getSqlDataTable(string.Format(sql, SQLDefend.SQLString(pChartNo), GetTableName.RCS_WEANING_ASSESS.ToString()));
            return dt;
        }
    }

    public class SHIFT_LIST: PatientListItem
    {
 
        public string B_VALUE_1 { get; set; }
        public string B_VALUE_2 { get; set; }

    }
}
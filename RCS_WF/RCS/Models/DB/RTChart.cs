using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using mayaminer.com.library;
using Newtonsoft.Json.Linq;

namespace RCS.Models.DB
{
    public class RTChart : BaseModel
    {
        public DataTable getChartData(string pChartNo, string pIpdNo, string pStartDate, string pEndDate)
        {
            string sql = @"SELECT A.RECORD_ID, A.RECORDDATE, B.ITEM_NAME, B.ITEM_VALUE FROM {3} A, {4} B
                           WHERE A.RECORD_ID = B.RECORD_ID AND A.CHART_NO = {0} AND RECORDDATE >= {1} AND RECORDDATE <= {2} ";
            sql = string.Format(sql, SQLDefend.SQLString(pChartNo), SQLDefend.SQLString(pStartDate), SQLDefend.SQLString(pEndDate), GetTableName.RCS_RECORD_MASTER.ToString(), GetTableName.RCS_RECORD_DETAIL.ToString());
            if (!string.IsNullOrWhiteSpace(pIpdNo))
            {
                sql += " AND A.IPD_NO =" + SQLDefend.SQLString(getHistoryList(pIpdNo,0));
            }
            DataTable dt = this.DBA.getSqlDataTable(sql);
            return dt;
        }
    } 
     
}
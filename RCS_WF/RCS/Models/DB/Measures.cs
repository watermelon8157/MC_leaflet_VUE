using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using mayaminer.com.library;
namespace RCS.Models.DB
{
    public class Measures : BaseModel
    {
        /// <summary>
        /// 取得輔具清單
        /// </summary>
        /// <returns></returns>
        public DataTable getUserMeasuresList()
        {
            string sql = @"SELECT M.MEASURE_ID,D.ITEM_VALUE,CREATE_NAME,CREATE_DATE,MODIFY_NAME,MODIFY_DATE
                           FROM {0} M
                           LEFT JOIN {1} D
                           ON M.MEASURE_ID = D.MEASURE_ID AND D.ITEM_NAME = 'name'";
            DataTable dt = this.DBA.getSqlDataTable(string.Format(sql, GetTableName.RT_MEASURES_FORM_MASTER.ToString(), GetTableName.RT_MEASURES_FORM_DETAIL.ToString()));
            return dt;
        }

        /// <summary>
        /// 取得輔具筆數
        /// </summary>
        /// <param name="pMeasureId">輔具流水號</param>
        /// <returns></returns>
        public string getCountMeasures(string pMeasureId){
            string Cnt = "";
            string sql = "SELECT COUNT(*) CNT FROM {1} WHERE MEASURE_ID = {0}";
            object Obj = this.DBA.ExecuteScalar(string.Format(sql, SQLDefend.SQLString(pMeasureId), GetTableName.RT_MEASURES_FORM_MASTER.ToString()));
            if (Obj != null && Obj.ToString().Trim() != "" && (int)Obj > 0) Cnt = Obj.ToString().Trim();
            return Cnt;
        }

        /// <summary>
        /// 取得輔具主檔
        /// </summary>
        /// <param name="pWhere">查詢條件</param>
        /// <returns></returns>
        public DataTable getMeasuresMaster(string pWhere)
        {
            DataTable dt = this.DBA.getDataTable(GetTableName.RT_MEASURES_FORM_MASTER.ToString(), pWhere);
            return dt;
        }

        /// <summary>
        /// 取得輔具明細檔
        /// </summary>
        /// <param name="pWhere">查詢條件</param>
        /// <returns></returns>
        public DataTable getMeasuresDetail(string pWhere)
        {
            DataTable dt = this.DBA.getDataTable(GetTableName.RT_MEASURES_FORM_DETAIL.ToString(), pWhere);
            return dt;
        }

        /// <summary>
        /// 取得輔具欄位與名稱
        /// </summary>
        /// <param name="pMeasureId">輔具流水號</param>
        /// <returns></returns>
        public DataTable getMeasuresDetailColumns(string pMeasureId)
        {
            string sql = "SELECT ITEM_NAME,ITEM_VALUE FROM {1} WHERE MEASURE_ID = {0}";
            DataTable dt = this.DBA.getSqlDataTable(string.Format(sql, SQLDefend.SQLString(pMeasureId), GetTableName.RT_MEASURES_FORM_DETAIL.ToString()));
            return dt;
        }

        /// <summary>
        /// 刪除輔具主檔
        /// </summary>
        /// <param name="pWhere">刪除條件</param>
        /// <returns></returns>
        public bool deleteMeasureMaster(string pWhere)
        {
            try
            {
                this.DBA.ExecuteNonQuery(string.Format("DELETE {1} {0}", pWhere, GetTableName.RT_MEASURES_FORM_MASTER.ToString()));
                if (this.DBA.LastError != "") return false;
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// 刪除輔具明細檔
        /// </summary>
        /// <param name="pWhere">刪除條件</param>
        /// <returns></returns>
        public bool deleteMeasureDetail(string pWhere)
        {
            try
            {
                this.DBA.ExecuteNonQuery(string.Format("DELETE {1} {0}", pWhere, GetTableName.RT_MEASURES_FORM_DETAIL.ToString()));
                if (this.DBA.LastError != "") return false;
                return true;
            }catch {
                return false;
            }            
        }

    }
}
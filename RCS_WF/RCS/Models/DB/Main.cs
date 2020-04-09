using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;

namespace RCS.Models.DB
{
    public class Main : BaseModel
    {
        /// <summary>
        /// 取得系統錯誤訊息
        /// </summary>
        /// <param name="pWhere">查詢條件</param>
        /// <returns></returns>
        public DataTable getSystemLog(string pWhere)
        {
            DataTable dt = this.DBA.getDataTable(GetTableName.RCS_SYS_LOG.ToString(), pWhere);
            return dt;
        }
    }
}
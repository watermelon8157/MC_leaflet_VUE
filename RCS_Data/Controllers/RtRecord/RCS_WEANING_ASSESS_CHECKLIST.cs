using Com.Mayaminer;
using RCS_Data.Models.DB;
using RCS_Data.Models.ViewModels;
using RCSData.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace RCS_Data.Controllers.RtRecord
{
    /// <summary>
    /// 
    /// </summary>
    public partial class Models : BaseModels, Interface
    {
        public RESPONSE_MSG save_RCS_WEANING_ASSESS_CHECKLIST(IPDPatientInfo pat_info, UserInfo user_info, DB_RCS_WEANING_ASSESS_CHECKLIST vm)
        {
            RESPONSE_MSG rm = new RESPONSE_MSG();
            string actionName = "save_RCS_WEANING_ASSESS_CHECKLIST";
            try
            { 
                vm.CREATE_DATE = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                vm.CREATE_ID = user_info.user_id;
                vm.CREATE_NAME = user_info.user_name;
                vm.MODIFY_DATE = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                vm.MODIFY_ID = user_info.user_id;
                vm.MODIFY_NAME = user_info.user_name;
                vm.DATASTATUS = "1";
                vm.CHART_NO = pat_info.chart_no;
                vm.IPD_NO = pat_info.ipd_no;
                List<DB_RCS_WEANING_ASSESS_CHECKLIST> pList = new List<DB_RCS_WEANING_ASSESS_CHECKLIST>() { vm };
                this.DBLink.DBA.DBExecInsert<DB_RCS_WEANING_ASSESS_CHECKLIST>(pList);
                if (this.DBLink.DBA.hasLastError)
                {
                    LogTool.SaveLogMessage(this.DBLink.DBA.lastError, actionName, this.csName);
                    rm.status = RESPONSE_STATUS.ERROR;
                    rm.message = "儲存失敗!";
                }
            }
            catch (Exception ex)
            {
                rm.message = "儲存失敗!t程式發生錯誤，請洽資訊人員!";
                rm.status = RESPONSE_STATUS.EXCEPTION;
                LogTool.SaveLogMessage(ex, actionName, this.csName);
            }
            rm.attachment = "儲存成功!";
            return rm;
        }

        public RESPONSE_MSG<DB_RCS_WEANING_ASSESS_CHECKLIST> getRWCA(string chart_no, string ipd_no, string pDate)
        {
            RESPONSE_MSG<DB_RCS_WEANING_ASSESS_CHECKLIST> rm = new RESPONSE_MSG<DB_RCS_WEANING_ASSESS_CHECKLIST>();
            string actionName = "getRWCA"; 
            Dapper.DynamicParameters dp = new Dapper.DynamicParameters();
            string _query = "SELECT * FROM RCS_WEANING_ASSESS_CHECKLIST WHERE CREATE_DATE LIKE @date AND chart_no = @chart_no ORDER BY CREATE_DATE DESC";
            dp = new Dapper.DynamicParameters();
            dp.Add("date", string.Concat(pDate, "%"));
            dp.Add("chart_no", chart_no);
            List<DB_RCS_WEANING_ASSESS_CHECKLIST> pList = this.DBLink.DBA.getSqlDataTable<DB_RCS_WEANING_ASSESS_CHECKLIST>(_query, dp);
            if (this.DBLink.DBA.hasLastError)
            {
                rm.message = this.DBLink.DBA.lastError;
                LogTool.SaveLogMessage(this.DBLink.DBA.lastError, actionName, this.csName);
            }
            else
            {
                pList = pList.OrderByDescending(x => DateTime.Parse(x.CREATE_DATE)).ToList();
            }
            if (pList.Count > 0)
            {
                rm.attachment = pList[0];
            }
            else
            {
                rm.attachment = new DB_RCS_WEANING_ASSESS_CHECKLIST();
            } 
            return rm;
        }
    }
}

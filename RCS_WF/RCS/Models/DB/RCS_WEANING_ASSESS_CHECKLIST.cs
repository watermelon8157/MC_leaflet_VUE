using RCS_Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Com.Mayaminer;
using RCS_Data.Models.DB;

namespace RCS.Models
{
    public class RCS_WEANING_ASSESS_CHECKLIST : DB_RCS_WEANING_ASSESS_CHECKLIST
    {

    }

    public class MODEL_RCS_WEANING_ASSESS_CHECKLIST:BaseModel
    {
        string csName { get { return "MODEL_RCS_WEANING_ASSESS_CHECKLIST"; } }

        public bool hasEdithasTodayRWC(string chart_no,string ipd_no)
        {
            bool hasRWC = false;
            string actionName = "hasEdithasTodayRWC";
            SQLProvider SQL = new SQLProvider();
            Dapper.DynamicParameters dp = new Dapper.DynamicParameters();
            string _query = "SELECT * FROM RCS_ONMODE_MASTER WHERE CHART_NO = @CHART_NO AND IPD_NO = @IPD_NO AND  ON_TYPE = '2' AND DATASTATUS = '1' AND ISNULL(ENDDATE,'') = ''";
            dp.Add("CHART_NO", chart_no);
            dp.Add("IPD_NO", ipd_no);
            System.Data.DataTable on_List = SQL.DBA.getSqlDataTable(_query, dp);
            if (SQL.DBA.hasLastError)
            {
                LogTool.SaveLogMessage(SQL.DBA.lastError, actionName, this.csName);
            }
            else
            {
                if (on_List.Rows.Count > 0 )
                {
                    List<RCS_WEANING_ASSESS_CHECKLIST> pList = getRWCA(chart_no, ipd_no, DateTime.Now.ToString("yyyy-MM-dd"));
                    if (pList.Count == 0) hasRWC = true;
                }
               
            }
            if (DateTime.Now > DateTime.Parse(DateTime.Now.ToString("yyyy-MM-dd 16:00:00")) || DateTime.Now < DateTime.Parse(DateTime.Now.ToString("yyyy-MM-dd 08:00:00")))
            {
                return false;
            }
            return hasRWC;
        }

        public List<RCS_WEANING_ASSESS_CHECKLIST> getRWCA(string chart_no, string ipd_no,string pDate)
        {
            string actionName = "getRWCA";
            SQLProvider SQL = new SQLProvider();
            Dapper.DynamicParameters dp = new Dapper.DynamicParameters();
            string _query = "SELECT * FROM RCS_WEANING_ASSESS_CHECKLIST WHERE CREATE_DATE LIKE @date AND chart_no = @chart_no";
            dp = new Dapper.DynamicParameters();
            dp.Add("date", string.Concat(pDate, "%"));
            dp.Add("chart_no", chart_no);
            List<RCS_WEANING_ASSESS_CHECKLIST> pList = SQL.DBA.getSqlDataTable<RCS_WEANING_ASSESS_CHECKLIST>(_query, dp);
            if (SQL.DBA.hasLastError)
            {
                pList = new List<RCS_WEANING_ASSESS_CHECKLIST>();
                LogTool.SaveLogMessage(SQL.DBA.lastError, actionName, this.csName);
            }
            else
            {
                pList = pList.OrderByDescending(x => DateTime.Parse(x.CREATE_DATE)).ToList();
            }
           
            return pList;
        }

        public RESPONSE_MSG save_RCS_WEANING_ASSESS_CHECKLIST(RCS_WEANING_ASSESS_CHECKLIST vm)
        {
            RESPONSE_MSG rm = new RESPONSE_MSG();
            string actionName = "save_RCS_WEANING_ASSESS_CHECKLIST";
            try
            {
                SQLProvider SQL = new SQLProvider();
                string _noneQeury = string.Concat("INSERT INTO RCS_WEANING_ASSESS_CHECKLIST (IPD_NO,CHART_NO,RWAC01,RWAC02,RWAC03,RWAC04,RWAC05,RWAC06,RWAC07,RWAC08,RWAC09,DATASTATUS,CREATE_ID,CREATE_NAME,CREATE_DATE) VALUES",
                    " (@IPD_NO,@CHART_NO,@RWAC01,@RWAC02,@RWAC03,@RWAC04,@RWAC05,@RWAC06,@RWAC07,@RWAC08,@RWAC09,@DATASTATUS,@CREATE_ID,@CREATE_NAME,@CREATE_DATE)");
                vm.CREATE_DATE = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                vm.CREATE_ID = user_info.user_id;
                vm.CREATE_NAME = user_info.user_name;
                vm.DATASTATUS = "1";

                List<RCS_WEANING_ASSESS_CHECKLIST> pList = new List<RCS_WEANING_ASSESS_CHECKLIST>() { vm };
                SQL.DBA.DBExecute<RCS_WEANING_ASSESS_CHECKLIST>(_noneQeury, pList);
                if (SQL.DBA.hasLastError)
                {
                    LogTool.SaveLogMessage(SQL.DBA.lastError, actionName, this.csName);
                    rm.status = RESPONSE_STATUS.ERROR;
                    rm.message = "儲存失敗!";

                }
                else
                {
                    rm.status = RESPONSE_STATUS.SUCCESS;
                    rm.message = "儲存成功!";


                }
            }
            catch (Exception ex)
            {
                rm.message = "儲存失敗!t程式發生錯誤，請洽資訊人員!";
                rm.status = RESPONSE_STATUS.EXCEPTION;
                LogTool.SaveLogMessage(ex, actionName, this.csName);
            }

            return rm;
        }
    }
  
}
using Com.Mayaminer;
using mayaminer.com.library;
using RCS_Data.Controllers.RtRecord;
using RCS_Data.Models;
using RCS_Data.Models.DB;
using RCS_Data.Models.ViewModels;
using RCSData.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text.RegularExpressions;

namespace RCS_Data.Controllers.Upload
{
    public partial class Models : BaseModels, RCS_Data.Controllers.Upload.Interface
    {
        string csName { get { return "Upload.Models"; } }

        public RESPONSE_MSG UploadList(UserInfo user_info, string sDate, string eDate)
        {
            string actionName = "UploadList";
            RESPONSE_MSG rm = new RESPONSE_MSG();
            List<UPLOADLIST> pList = this.getUploadList(ref rm, sDate, eDate, null);
            if (!string.IsNullOrWhiteSpace(user_info.user_id)  )
            {
                pList = pList.FindAll(x => x.CREATE_ID == user_info.user_id);
            }
            rm.attachment = pList;
            return rm;
        }

        public RESPONSE_MSG UserUploadList(UserInfo user_info, string sDate, string eDate, List<string> typeList)
        {
            string actionName = "UserUploadList";
            RESPONSE_MSG rm = new RESPONSE_MSG();
            List<UPLOADLIST> pList = this.getUploadList(ref rm, sDate, eDate, null);
            if (!string.IsNullOrWhiteSpace(user_info.user_id) )
            {
                pList = pList.FindAll(x => x.CREATE_ID == user_info.user_id);
            }
            if (typeList.Count > 0 && pList.Exists(x => x.CREATE_ID == user_info.user_id))
            {
                pList = pList.FindAll(x => typeList.Contains(x.UPLOAD_STATUS));
            }
            rm.attachment = pList;
            return rm;
        }

        public RESPONSE_MSG NotUploadCnt(UserInfo user_info, string sDate, string eDate, List<string> typeList)
        {
            string actionName = "NotUploadCnt";
            RESPONSE_MSG rm = new RESPONSE_MSG();
            List<UPLOADLIST> pList = this.getUploadList(ref rm, sDate, eDate, null);
            if (!string.IsNullOrWhiteSpace(user_info.user_id)  )
            {
                pList = pList.FindAll(x => x.CREATE_ID == user_info.user_id);
            }
            if (typeList.Count > 0 && pList.Exists(x => x.CREATE_ID == user_info.user_id))
            {
                pList = pList.FindAll(x => typeList.Contains(x.UPLOAD_STATUS));
            }
            rm.attachment = pList.Count;
            return rm;
        }

        /// <summary>
        /// 取得上傳清單
        /// </summary>
        /// <param name="rm"></param>
        /// <param name="sDate"></param>
        /// <param name="eDate"></param>
        /// <param name="statusList"></param>
        /// <returns></returns>
        protected virtual List<UPLOADLIST> getUploadList(ref RESPONSE_MSG rm, string sDate, string eDate, List<string> statusList)
        {
            if (statusList == null || statusList.Count == 0)
            {
                statusList = new List<string>() { "0",  "2", "3", "4", "5" };
            }
            string actionName = "getUploadList";
            List<UPLOADLIST> pList = new List<UPLOADLIST>();
            try
            {
                DateTime dateNow = DateTime.Now;
                sDate = string.IsNullOrWhiteSpace(sDate) && !mayaminer.com.library.DateHelper.isDate(sDate) ?
                     Function_Library.getDateString(dateNow.AddDays(-1), DATE_FORMAT.yyyy_MM_dd_000000) : Function_Library.getDateString(DateTime.Parse(sDate), DATE_FORMAT.yyyy_MM_dd_000000);
                eDate = string.IsNullOrWhiteSpace(eDate) && !mayaminer.com.library.DateHelper.isDate(eDate) ?
                    Function_Library.getDateString(dateNow.AddDays(1), DATE_FORMAT.yyyy_MM_dd_125959) : Function_Library.getDateString(DateTime.Parse(eDate).AddDays(1), DATE_FORMAT.yyyy_MM_dd_125959);

                Dapper.DynamicParameters dp = new Dapper.DynamicParameters();
                List<string> sqlList = new List<string>();
                sqlList.Add(string.Format(string.Concat(
                    "SELECT '{0},' + {1} + ',{1}' as RECORD_KEY,'{0}' as RECORD,'DB_RCS_RECORD_MASTER' as RECORD_NAME,{1} as RECORD_ID,{2} as RECORDDATE",
                    ",CHART_NO,IPD_NO,CREATE_ID,CREATE_NAME ,UPLOAD_STATUS,MODIFY_DATE",
                    " FROM {0} WHERE {2} BETWEEN @sDate AND @eDate AND DATASTATUS = '1' AND UPLOAD_STATUS in @UPLOAD_STATUS"),
                    DB_TABLE_NAME.DB_RCS_RECORD_MASTER, "RECORD_ID", "RECORDDATE"));
                sqlList.Add(string.Format(string.Concat(
                    "SELECT '{0},' + {1} + ',{1}' as RECORD_KEY,'{0}' as RECORD,'DB_RCS_CPT_ASS_MASTER' as RECORD_NAME,{1} as RECORD_ID,{2} as RECORDDATE",
                    ",CHART_NO,IPD_NO,CREATE_ID,CREATE_NAME ,UPLOAD_STATUS,MODIFY_DATE",
                    " FROM {0} WHERE {2} BETWEEN @sDate AND @eDate AND DATASTATUS = '1' AND UPLOAD_STATUS in @UPLOAD_STATUS"),
                    DB_TABLE_NAME.DB_RCS_CPT_ASS_MASTER, "CPT_ID", "RECORD_DATE"));
                //sqlList.Add(string.Format(string.Concat(
                //    "SELECT '{0},' + {1} + ',{1}' as RECORD_KEY,'{0}' as RECORD,'DB_RCS_WEANING_ASSESS' as RECORD_NAME,{1} as RECORD_ID,{2} as RECORDDATE",
                //    ",CHART_NO,IPD_NO,CREATE_ID,CREATE_NAME ,UPLOAD_STATUS,MODIFY_DATE",
                //    " FROM {0} WHERE {2} BETWEEN @sDate AND @eDate AND DATASTATUS = '1' AND UPLOAD_STATUS in @UPLOAD_STATUS"),
                //    DB_TABLE_NAME.DB_RCS_WEANING_ASSESS, "TK_ID", "REC_DATE"));
                //sqlList.Add(string.Format(string.Concat(
                //    "SELECT '{0},' + {1} + ',{1}' as RECORD_KEY,'{0}' as RECORD,'DB_RCS_CPT_NEW_RECORD' as RECORD_NAME,{1} as RECORD_ID,{2} as RECORDDATE",
                //    ",CHART_NO,IPD_NO,CREATE_ID,CREATE_NAME ,UPLOAD_STATUS,MODIFY_DATE",
                //    " FROM {0} WHERE {2} BETWEEN @sDate AND @eDate AND DATASTATUS = '1' AND UPLOAD_STATUS in @UPLOAD_STATUS"),
                //    DB_TABLE_NAME.DB_RCS_CPT_NEW_RECORD, "CPT_ID", "REC_DATE"));
                dp.Add("sDate", sDate);
                dp.Add("eDate", eDate);
                dp.Add("UPLOAD_STATUS", statusList);
                string query = string.Join(" UNION ", sqlList);
                pList = this.DBLink.DBA.getSqlDataTable<UPLOADLIST>(query, dp);

                List<DB_RCS_RT_CASE> patList = new List<DB_RCS_RT_CASE>();
                string querySql = string.Format("SELECT * FROM {0} WHERE IPD_NO in @IPD_NO", DB_TABLE_NAME.DB_RCS_RT_CASE);
                dp.Add("IPD_NO", pList.Select(x => x.IPD_NO).Distinct());
                patList = this.DBLink.DBA.getSqlDataTable<DB_RCS_RT_CASE>(querySql, dp);
                foreach (UPLOADLIST item in pList)
                {
                    if (patList.Exists(x=> x.IPD_NO == item.IPD_NO))
                    {
                        item.patData = patList.Find(x => x.IPD_NO == item.IPD_NO);
                    } 
                }
                if (this.DBLink.DBA.hasLastError)
                {
                    rm.status = RESPONSE_STATUS.ERROR;
                    rm.messageList.Add(this.DBLink.DBA.lastError);
                    LogTool.SaveLogMessage(this.DBLink.DBA.lastError, actionName, csName);
                }
            }
            catch (Exception ex)
            {
                rm.status = RESPONSE_STATUS.ERROR;
                rm.messageList.Add(ex.Message);
                LogTool.SaveLogMessage(ex, actionName, csName);
            }

            return pList;
        }

        /// <summary>
        /// 取得上傳清單
        /// </summary>
        /// <param name="rm"></param>
        /// <param name="statusList"></param>
        /// <returns></returns>
        protected List<UPLOADLIST> getYearUploadList(ref RESPONSE_MSG rm, List<string> statusList)
        {
            return this.getUploadList(ref rm, Function_Library.getDateString(DateTime.Now.AddYears(-1), DATE_FORMAT.yyyy_MM_dd_000000), "", statusList);
        }

        /// <summary>
        /// 重新產生簽章
        /// </summary>
        /// <param name="user_info"></param>
        /// <param name="recprd_id"></param>
        /// <param name="table_name"></param>
        /// <returns></returns>
        public RESPONSE_MSG ReUpDateData(UserInfo user_info, string recprd_id,string table_name, string recprd_id_colunm_name)
        { 
            return this.UpDateData(user_info, new List<string>() { string.Concat(table_name,",", recprd_id, ",", recprd_id_colunm_name) }, "5");
        }

        public RESPONSE_MSG UpDateData(UserInfo user_info,   List<string> keyList, string status_tpye)
        {
            string actionName = "UpDateData";
            RESPONSE_MSG rm = new RESPONSE_MSG();
            if (keyList != null && keyList.Count > 0)
            {
                Dapper.DynamicParameters dp = new Dapper.DynamicParameters();
                string query = "UPDATE {0} SET UPLOAD_STATUS = @UPLOAD_STATUS,UPLOAD_ID =@UPLOAD_ID,UPLOAD_NAME =@UPLOAD_NAME,UPLOAD_DATE =@UPLOAD_DATE WHERE {1} = @record_id AND DATASTATUS = '1';";
                foreach (string pKey in keyList)
                {
                    dp = new Dapper.DynamicParameters();
                    string[] strlist = pKey.Split(',');
                    if (strlist.Length > 2)
                    {
                        dp.Add("record_id", strlist[1]);
                        dp.Add("UPLOAD_STATUS", status_tpye);
                        dp.Add("UPLOAD_ID", user_info.user_id);
                        dp.Add("UPLOAD_NAME", user_info.user_name);
                        dp.Add("UPLOAD_DATE", Function_Library.getDateNowString( DATE_FORMAT.yyyy_MM_dd_HHmmss));
                        this.DBLink.DBA.DBExecute(string.Format(query, strlist[0], strlist[2]), dp);
                    }
                }
                if (this.DBLink.DBA.hasLastError)
                {
                    rm.status = RESPONSE_STATUS.ERROR;
                    rm.messageList.Add(this.DBLink.DBA.lastError);
                    LogTool.SaveLogMessage(this.DBLink.DBA.lastError, actionName, csName);
                }else
                {
                    switch (status_tpye)
                    {
                        case "0":
                            rm.attachment = "回復成功!";
                            break;
                        case "1":
                            rm.attachment = "上傳成功!";
                            break;
                        case "2":
                            rm.attachment = "修改成功!";
                            break; 
                        case "3":
                            rm.attachment = "忽略成功!";
                            break; 
                        default:
                            rm.attachment = "修改成功!";
                            break;
                    }
                }
               
            }
            else
            {
                rm.status = RESPONSE_STATUS.ERROR;
                rm.messageList.Add("請選擇一筆資料");
            }
             
            return rm;
        }

        public List<UPLOADLIST> UpLoadDataList()
        {
            string actionName = "UpLoadDataList";
            RESPONSE_MSG rm = new RESPONSE_MSG(); 
            List<UPLOADLIST> pList = new List<UPLOADLIST>();
            List<string> sqlList = new List<string>();
            sqlList.Add(string.Format(string.Concat(
                "SELECT '{0},' + {1} + ',{1}' as RECORD_KEY,'{0}' as RECORD,{1} as RECORD_ID,{2} as RECORDDATE",
                ",CHART_NO,IPD_NO,CREATE_ID,CREATE_NAME ,UPLOAD_STATUS,MODIFY_DATE",
                " FROM {0} WHERE DATASTATUS = '1' AND UPLOAD_STATUS = '2'"),
                DB_TABLE_NAME.DB_RCS_RECORD_MASTER, "RECORD_ID", "RECORDDATE"));
            sqlList.Add(string.Format(string.Concat(
                "SELECT '{0},' + {1} + ',{1}' as RECORD_KEY,'{0}' as RECORD,{1} as RECORD_ID,{2} as RECORDDATE",
                ",CHART_NO,IPD_NO,CREATE_ID,CREATE_NAME ,UPLOAD_STATUS,MODIFY_DATE",
                " FROM {0} WHERE DATASTATUS = '1' AND UPLOAD_STATUS = '2'"),
                DB_TABLE_NAME.DB_RCS_CPT_ASS_MASTER, "CPT_ID", "RECORD_DATE"));  
            string query = string.Join(" UNION ", sqlList);
            pList = this.DBLink.DBA.getSqlDataTable<UPLOADLIST>(query);
            if (this.DBLink.DBA.hasLastError)
            {
                rm.status = RESPONSE_STATUS.ERROR;
                rm.messageList.Add(this.DBLink.DBA.lastError);
                LogTool.SaveLogMessage(this.DBLink.DBA.lastError, actionName, csName);
            }
            return pList;
        }

        public virtual void RunThread()
        {
            string actionName = "RunThread";
            
        }
    }

    public class FormUpload : AUTH
    {
        public string sDate { get; set; }
        public string eDate { get; set; }
        public List<string> keyList { get; set; }
        public string status_type { get; set; }
    }

    
}

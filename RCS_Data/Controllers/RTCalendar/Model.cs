using RCSData.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace RCS_Data.Controllers.RTCalendar
{
    public partial class Models
    {
        string csName = "RCS_Data.Controllers.RTCalendar.Models";



        SQLProvider dbLink = new SQLProvider();

        /// <summary>
        /// 取得RT日曆資料清單
        /// </summary>
        /// <param name="psDate">開始日期</param>
        /// <param name="peDate">結束日期</param>
        /// <param name="pRespid">呼吸器編號</param>
        /// <param name="pDatastatus">資料狀況</param>
        /// <param name="pUserId">使用者帳號</param>
        /// <param name="pCost_code">護理站代碼</param>
        /// <returns></returns>
        public RESPONSE_MSG getRTCalList(FormBody_getRTCalList model)
        {
            RESPONSE_MSG rm = new RESPONSE_MSG();
            List<RTCalList> pList = new List<RTCalList>();
            string actionName = "getRTCalList"; 
            pList = this.RTCalList<RTCalList>(model); 
            if (this.dbLink.DBA.hasLastError)
            {
                rm.status = RESPONSE_STATUS.ERROR;
                rm.messageList.Add("取得資料發生錯誤，請下資訊人員!");
                Com.Mayaminer.LogTool.SaveLogMessage(this.dbLink.DBA.lastError, actionName, this.csName);
            } 
            rm.attachment = pList;

            return rm;
        }

        public RESPONSE_MSG getRTCalWeek(FormBody_getRTCalList model)
        {
            RESPONSE_MSG rm = new RESPONSE_MSG();
            List<RTCalWeek> weekList = new List<RTCalWeek>();
            List<RTCalWeek> pList = new List<RTCalWeek>();
            string actionName = "getRTCalWeek";
            for (DateTime i = this.sDate(model.psDate); i <= DateTime.Parse(model.peDate).Date; i = i.AddDays(1))
            {
                weekList.Add(new RTCalWeek()
                {
                    start = i.ToString("yyyy-MM-dd 08:00"),
                    end = i.ToString("yyyy-MM-dd 17:00"),
                    date = i.ToString("yyyy-MM-dd"),
                    title = "D"
                }); ;
                weekList.Add(new RTCalWeek()
                {
                    start = i.ToString("yyyy-MM-dd 17:00"),
                    end = i.ToString("yyyy-MM-dd 23:00"),
                    date = i.ToString("yyyy-MM-dd"),
                    title = "E"
                }); ;
                weekList.Add(new RTCalWeek()
                {
                    start = i.ToString("yyyy-MM-dd 23:00"),
                    end = i.ToString("yyyy-MM-dd 08:00"),
                    date = i.ToString("yyyy-MM-dd"),
                    title = "N"
                }); ;
            } 
            rm.attachment = weekList;
            return rm;
        } 
        protected virtual List<T> RTCalList<T>(FormBody_getRTCalList model)
        {
            List<T> pList = new List<T>();
            string actionName = "RTCalList";
            string sql = ""; 
            List<string> plist = new List<string>();
            List<string> sqlList = new List<string>();
            List<string> whereList = new List<string>();
            Dapper.DynamicParameters dp = new Dapper.DynamicParameters();
            #region dp 
            // 前14天內的資料
            dp.Add("psDate", !string.IsNullOrWhiteSpace(model.psDate) ? RCS_Data.Models.Function_Library.getDateString(this.sDate(model.psDate), RCS_Data.Models.DATE_FORMAT.yyyy_MM_dd_000000) : RCS_Data.Models.Function_Library.getDateNowString(RCS_Data.Models.DATE_FORMAT.yyyy_MM_dd_000000)); // 2017-03-01 00:00:00
            dp.Add("peDate", !string.IsNullOrWhiteSpace(model.peDate) ? RCS_Data.Models.Function_Library.getDateString(DateTime.Parse(model.peDate), RCS_Data.Models.DATE_FORMAT.yyyy_MM_dd_125959) : RCS_Data.Models.Function_Library.getDateNowString(RCS_Data.Models.DATE_FORMAT.yyyy_MM_dd_125959)); // 2020-03-20 23:59:00
            dp.Add("pDatastatus", model.pDatastatus);
            dp.Add("pRespid", model.pRespid);
            dp.Add("pUserId", model.pUserId);
            dp.Add("pCost_code", model.pCost_code);
            dp.Add("pChr_no", model.pChr_no);

            #region whereList 
            if (string.IsNullOrWhiteSpace(model.pDatastatus))
            {
                whereList.Add("AND m.DATASTATUS = '1'");
            }
            else
            {
                whereList.Add("AND m.DATASTATUS in ('1','9')");
            }
            if (!string.IsNullOrWhiteSpace(model.pUserId))
            {
                whereList.Add("AND m.CREATE_ID = @pUserId");
            }
            if (!string.IsNullOrWhiteSpace(model.pCost_code))
            {
                whereList.Add("AND m.COST_CODE = @pCost_code");
            } 
            if (!string.IsNullOrWhiteSpace(model.pChr_no))
            {
                whereList.Add("AND m.CHART_NO = @pChr_no");
            }
            #endregion
            #endregion
            #region SQL 

            #region SQL

            // SELECT DISTINCT '呼吸照護記錄單' RECORD,m.RECORD_ID,RECORDDATE,c.PATIENT_NAME,m.CHART_NO,m.IPD_NO,m.CREATE_ID,m.CREATE_NAME,m.CREATE_DATE,PAT_SOURCE,m.COST_CODE,m.BED_NO,m.DEPT_CODE,l.ITEM_VALUE as RESPID
            // FROM RCS_RECORD_MASTER as m
            // LEFT JOIN RCS_RECORD_DETAIL as L ON m.RECORD_ID = l.RECORD_ID AND l.ITEM_NAME = 'respid'
            // LEFT JOIN RCS_RT_CASE as c ON c.IPD_NO = m.IPD_NO
            // WHERE RECORDDATE between '2017-03-01 00:00:00' AND '2020-03-20 23:59:00' AND DATASTATUS in ('1', '9')
            // UNION
            // SELECT DISTINCT '胸腔物理治療單' RECORD, CPT_ID as RECORD_ID,REC_DATE as RECORDDATE,c.PATIENT_NAME,m.CHART_NO,m.IPD_NO,m.CREATE_ID,m.CREATE_NAME,m.CREATE_DATE,PAT_SOURCE,m.COST_CODE,m.BED_NO,m.DEPT_CODE,'' as RESPID
            // FROM RCS_CPT_NEW_RECORD as m
            // LEFT JOIN RCS_RT_CASE as c ON c.IPD_NO = m.IPD_NO
            // WHERE REC_DATE between '2017-03-01 00:00:00' AND '2020-03-20 23:59:00' AND DATASTATUS in ('1', '9')
            // UNION
            // SELECT DISTINCT '脫離評估單' RECORD, TK_ID as RECORD_ID,REC_DATE as RECORDDATE,c.PATIENT_NAME,m.CHART_NO,m.IPD_NO,m.CREATE_ID,m.CREATE_NAME,m.CREATE_DATE,PAT_SOURCE,m.COST_CODE,m.BED_NO,m.DEPT_CODE,'' as RESPID   FROM RCS_WEANING_ASSESS as m
            // LEFT JOIN RCS_RT_CASE as c ON c.IPD_NO = m.IPD_NO
            // WHERE REC_DATE between '2017-03-01 00:00:00' AND '2020-03-20 23:59:00' AND DATASTATUS in ('1', '9')
            // UNION
            // SELECT DISTINCT '門診記錄單' RECORD, OA_ID as RECORD_ID,RECORDDATE,c.PATIENT_NAME,m.CHART_NO,m.IPD_NO,m.CREATE_ID,m.CREATE_NAME,m.CREATE_DATE,PAT_SOURCE,m.COST_CODE,m.BED_NO,m.DEPT_CODE,'' as RESPID   FROM RCS_RT_OPD_ASS_MASTER as m
            // LEFT JOIN RCS_RT_CASE as c ON c.IPD_NO = m.IPD_NO
            // WHERE RECORDDATE between '2017-03-01 00:00:00' AND '2020-03-20 23:59:00' AND DATASTATUS in ('1', '9')
            // UNION
            // SELECT DISTINCT '門診交班' RECORD, OS_ID as RECORD_ID,RECORDDATE,c.PATIENT_NAME,m.CHART_NO,m.IPD_NO,m.CREATE_ID,m.CREATE_NAME,m.CREATE_DATE,PAT_SOURCE,m.COST_CODE,m.BED_NO,m.DEPT_CODE,'' as RESPID   FROM RCS_RT_OPD_SHIFT_MASTER as m
            // LEFT JOIN RCS_RT_CASE as c ON c.IPD_NO = m.IPD_NO
            // WHERE RECORDDATE between '2017-03-01 00:00:00' AND '2020-03-20 23:59:00' AND DATASTATUS in ('1', '9')
            // UNION
            // SELECT DISTINCT '呼吸治療評估單' RECORD, CPT_ID as RECORD_ID,RECORD_DATE as RECORDDATE,c.PATIENT_NAME,m.CHART_NO,m.IPD_NO,m.CREATE_ID,m.CREATE_NAME,m.CREATE_DATE,PAT_SOURCE,m.COST_CODE,m.BED_NO,m.DEPT_CODE,'' as RESPID  FROM RCS_CPT_ASS_MASTER as m
            // LEFT JOIN RCS_RT_CASE as c ON c.IPD_NO = m.IPD_NO
            // WHERE RECORD_DATE between '2017-03-01 00:00:00' AND '2020-03-20 23:59:00' AND DATASTATUS in ('1', '9')

            #endregion

            // 呼吸照護記錄單 SQL
            #region 呼吸照護記錄單 SQL
            sqlList.Add(string.Format(string.Concat(
                "SELECT DISTINCT '呼吸照護記錄單' RECORD, m.RECORD_ID,RECORDDATE,c.PATIENT_NAME,m.CHART_NO,m.IPD_NO,m.CREATE_ID,m.CREATE_NAME,m.CREATE_DATE,PAT_SOURCE,m.COST_CODE,m.BED_NO,m.DEPT_CODE,l.ITEM_VALUE as RESPID"
                , " FROM RCS_RECORD_MASTER as M"
                , " LEFT JOIN RCS_RECORD_DETAIL as L ON m.RECORD_ID = l.RECORD_ID AND l.ITEM_NAME = 'respid'"
                , " LEFT JOIN RCS_RT_CASE as c ON c.IPD_NO = m.IPD_NO"
                , " WHERE RECORDDATE between @psDate AND @peDate"
                , " {0}"), string.Join(" ", whereList)));
            if (!string.IsNullOrWhiteSpace(model.pRespid))
            {
                sqlList.Add(" AND RESID = @pRespid");
            }
            #endregion

            // 胸腔物理治療單 SQL
            #region 胸腔物理治療單 SQL
            sqlList.Add(string.Format(string.Concat(
                "SELECT DISTINCT '胸腔物理治療單' RECORD, CPT_ID as RECORD_ID,REC_DATE as RECORDDATE,c.PATIENT_NAME,m.CHART_NO,m.IPD_NO,m.CREATE_ID,m.CREATE_NAME,m.CREATE_DATE,PAT_SOURCE,m.COST_CODE,m.BED_NO,m.DEPT_CODE,'' as RESPID"
                , " FROM RCS_CPT_NEW_RECORD as m"
                , " LEFT JOIN RCS_RT_CASE as c ON c.IPD_NO = m.IPD_NO"
                , " WHERE REC_DATE between @psDate AND @peDate"
                , " {0}"), string.Join(" ", whereList)));

            #endregion

            // 脫離評估單 SQL
            #region 脫離評估單 SQL

            sqlList.Add(string.Format(string.Concat(
                "SELECT DISTINCT '脫離評估單' RECORD, TK_ID as RECORD_ID,REC_DATE as RECORDDATE,c.PATIENT_NAME,m.CHART_NO,m.IPD_NO,m.CREATE_ID,m.CREATE_NAME,m.CREATE_DATE,PAT_SOURCE,m.COST_CODE,m.BED_NO,m.DEPT_CODE,'' as RESPID"
                , " FROM RCS_WEANING_ASSESS as m"
                , " LEFT JOIN RCS_RT_CASE as c ON c.IPD_NO = m.IPD_NO"
                , " WHERE REC_DATE between @psDate AND @peDate"
                , " {0}"), string.Join(" ", whereList)));

            #endregion

            // 門診記錄單 SQL
            #region 門診記錄單 SQL 

            sqlList.Add(string.Format(string.Concat(
                "SELECT DISTINCT '門診記錄單' RECORD, OA_ID as RECORD_ID,RECORDDATE,c.PATIENT_NAME,m.CHART_NO,m.IPD_NO,m.CREATE_ID,m.CREATE_NAME,m.CREATE_DATE,PAT_SOURCE,m.COST_CODE,m.BED_NO,m.DEPT_CODE,'' as RESPID"
                , " FROM RCS_RT_OPD_ASS_MASTER as m"
                , " LEFT JOIN RCS_RT_CASE as c ON c.IPD_NO = m.IPD_NO"
                , " WHERE RECORDDATE between @psDate AND @peDate"
                , " {0}"), string.Join(" ", whereList)));

            #endregion

            // 門診交班 SQL
            #region 門診交班 SQL 

            sqlList.Add(string.Format(string.Concat(
                "SELECT DISTINCT '門診交班' RECORD, OS_ID as RECORD_ID,RECORDDATE,c.PATIENT_NAME,m.CHART_NO,m.IPD_NO,m.CREATE_ID,m.CREATE_NAME,m.CREATE_DATE,PAT_SOURCE,m.COST_CODE,m.BED_NO,m.DEPT_CODE,'' as RESPID"
                , " FROM RCS_RT_OPD_SHIFT_MASTER as m"
                , " LEFT JOIN RCS_RT_CASE as c ON c.IPD_NO = m.IPD_NO"
                , " WHERE RECORDDATE between @psDate AND @peDate"
                , " {0}"), string.Join(" ", whereList)));

            #endregion

            // 呼吸治療評估單 SQL
            #region 呼吸治療評估單 SQL

            sqlList.Add(string.Format(string.Concat(
                "SELECT DISTINCT '呼吸治療評估單' RECORD, CPT_ID as RECORD_ID,RECORD_DATE as RECORDDATE,c.PATIENT_NAME,m.CHART_NO,m.IPD_NO,m.CREATE_ID,m.CREATE_NAME,m.CREATE_DATE,PAT_SOURCE,m.COST_CODE,m.BED_NO,m.DEPT_CODE,'' as RESPID"
                , " FROM RCS_CPT_ASS_MASTER as m"
                , " LEFT JOIN RCS_RT_CASE as c ON c.IPD_NO = m.IPD_NO"
                , " WHERE RECORD_DATE between @psDate AND @peDate"
                , " {0}"), string.Join(" ", whereList)));

            #endregion
            #endregion 
            sql = string.Join(" UNION ", sqlList);
            pList = dbLink.DBA.getSqlDataTable<T>(sql, dp); 
            return pList;
        }

        private DateTime sDate(string sDate)
        {
            DateTime nowDate = DateTime.Now;
            if (DateTime.Parse(sDate).Date < nowDate)
            { 
                return DateTime.Parse(sDate).AddDays(-14);
            }
            return   nowDate.AddDays(-14);
        }

    } 

    public class FormBody_getRTCalList : AUTH
    {
        public string psDate { get; set; }
        public string peDate { get; set; }
        public string pDatastatus { get; set; }
        public string pRespid { get; set; }
        public string pUserId { get; set; }
        public string pCost_code { get; set; }
        public string pChr_no { get; set; }
    }

    public class VM_RTCal
    {
        public List<RTCalWeek> calList { get; set; }
        public List<RTCalList> RTList { get; set; }
    }

    public class RTCalWeekList : RTCalWeek
    {
        public List<RTCalList> RTList { get; set; }
    }

    public class RTCalWeek 
    { 
        public string start { get; set; }
        public string end { get; set; }
        public string title { get; set; }
        public string date { get; set; }
    }

    public class RTCalList
    {
        public string RECORD { get; set; }
        public string RECORD_ID { get; set; }
        public string RECORDDATE { get; set; }
        public string PATIENT_NAME { get; set; }
        public string CHART_NO { get; set; }
        public string IPD_NO { get; set; }
        public string CREATE_ID { get; set; }
        public string CREATE_NAME { get; set; }
        public string CREATE_DATE { get; set; }
        public string PAT_SOURCE { get; set; }
        public string COST_CODE { get; set; }
        public string BED_NO { get; set; }
        public string DEPT_CODE { get; set; }
        public string RESPID { get; set; }
        public string date {
            get
            {
                if (!string.IsNullOrWhiteSpace(this.RECORDDATE))
                {
                    DateTime temDate = DateTime.Parse(this.RECORDDATE);
                    return temDate.ToString("yyyy-MM-dd");
                }
                return "";
            }
        }
        public string work_type { 
            get {
                if (!string.IsNullOrWhiteSpace(this.RECORDDATE))
                {
                    DateTime temDate = DateTime.Parse(this.RECORDDATE);
                    if (temDate.Hour > 8 && temDate.Hour <= 16)
                    {
                        return "D";
                    }
                    if (temDate.Hour > 16　&& temDate.Hour <= 23)
                    {
                        return "E";
                    } 
                    if (temDate.Hour > 23 ||temDate.Hour <= 8)
                    {
                        return "N";
                    }
                }
                return "";
            }

        }
        public string item_key { get {
                return this.CREATE_ID + this.work_type + this.COST_CODE + this.PAT_SOURCE;    
            }
        }
    }
}

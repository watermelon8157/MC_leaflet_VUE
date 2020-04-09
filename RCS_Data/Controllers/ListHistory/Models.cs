using Com.Mayaminer;
using Dapper;
using log4net;
using mayaminer.com.library;
using Newtonsoft.Json;
using RCS_Data.Models;
using RCS_Data.Models.DB;
using RCS_Data.Models.ViewModels;
using RCS_Data.Models.VIP;
using RCSData.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using static Dapper.SqlMapper;

namespace RCS_Data.Controllers.ListHistory
{
    public class ListHistoryModels : BaseModels
    {
        #region 參數
        private ILog _logger { get; set; }

        private string csName = "ListHistoryModels";
        protected ILog logger
        {
            get
            {
                if (this._logger == null)
                    this._logger = LogManager.GetLogger("ListHistoryModels");
                return this._logger;
            }
        }
 
  
        private RCS_Data.Models.BasicFunction _bf { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public RCS_Data.Models.BasicFunction basicfunction
        {
            get
            {
                if (this._bf == null)
                {
                    this._bf = new RCS_Data.Models.BasicFunction();
                }
                return this._bf;
            }
        }

        #endregion

        public RESPONSE_MSG Pathistory(ListHistoryViewModels form)
        {
            List<PatientListItem> pList = new List<PatientListItem>();
            RESPONSE_MSG rm = new RESPONSE_MSG();
            List<string> msg = new List<string>();
            string actionName = "getPathistory";
            try
            {
                //檢查病歷號、入院日期起訖日
                if (string.IsNullOrWhiteSpace(form.chart_no) && (string.IsNullOrWhiteSpace(form.searchipdSDate) && string.IsNullOrWhiteSpace(form.searchipdEDate)))
                    msg.Add("請輸入院日期區間或病歷號");
                if (!string.IsNullOrWhiteSpace(form.chart_no) && (string.IsNullOrWhiteSpace(form.searchipdSDate) || string.IsNullOrWhiteSpace(form.searchipdEDate)))
                    msg.Add("請輸入院日期區間");
                if (msg.Count == 0)
                {
                    string sql = "SELECT case_id,PATIENT_SOURCE,IPD_NO,CHART_NO,PATIENT_NAME,GENDER,BIRTH_DAY,BODY_HEIGHT,DIAG_DATE,VS_DOC_NAME FROM " + DB_TABLE_NAME.DB_RCS_RT_CASE + " A WHERE DIAG_DATE >= '{0}' AND DIAG_DATE <= '{1}'";
                    sql = string.Format(sql, DateHelper.Parse(form.searchipdSDate).ToString("yyyy-MM-dd 00:00:00"), DateHelper.Parse(form.searchipdEDate).ToString("yyyy-MM-dd 23:59:59"));
                    if (!string.IsNullOrWhiteSpace(form.chart_no)) sql += string.Format(" AND CHART_NO LIKE '{0}%'", form.chart_no);
                    if (!string.IsNullOrWhiteSpace(form.vs_doc)) sql += string.Format(" AND VS_DOC_ID = '{0}'", form.vs_doc);
                    if (!string.IsNullOrWhiteSpace(form.cost_center)) sql += string.Format(" AND COST_CODE = '{0}'", form.cost_center);
                    //sql += " AND CREATE_DATE = (SELECT MAX(CREATE_DATE) FROM RCS_RT_CASE WHERE IPD_NO = A.IPD_NO)";
                    DataTable dt = this.DBLink.DBA.getSqlDataTable(sql);
                    if (this.DTNotNullAndEmpty(dt))
                    {
                        foreach (DataRow dr in dt.Rows)
                        {
                            PatientListItem item = new PatientListItem();
                            item.case_id = this.checkDataColumn(dr, "case_id");
                            item.source_type = this.checkDataColumn(dr, "PATIENT_SOURCE");
                            item.ipd_no = this.checkDataColumn(dr, "IPD_NO");
                            item.chart_no = this.checkDataColumn(dr, "CHART_NO");
                            item.patient_name = this.checkDataColumn(dr, "PATIENT_NAME");
                            item.gender = this.checkDataColumn(dr, "GENDER");
                            item.birth_day = this.checkDataColumn(dr, "BIRTH_DAY");
                            if (mayaminer.com.library.DateHelper.isDate(item.birth_day, "yyyyMMdd"))
                            {
                                item.birth_day = DateHelper.Parse(item.birth_day).ToString("yyyy/MM/dd");
                            }
                            if (mayaminer.com.library.DateHelper.isDate(item.birth_day, "yyyMMdd"))
                            {
                                item.birth_day = DateHelper.ParseTWDate(item.birth_day).ToString("yyyy/MM/dd");
                            }
                            item.body_height = this.checkDataColumn(dr, "BODY_HEIGHT");
                            item.diag_date = this.checkDataColumn(dr, "DIAG_DATE");
                            item.vs_doc = this.checkDataColumn(dr, "VS_DOC_NAME");
                            pList.Add(item);
                        }
                        pList = pList.GroupBy(x => new { x.case_id, x.chart_no, x.ipd_no, x.patient_name, x.gender, x.birth_day, x.diag_date, x.vs_doc, x.source_type }).Select(y => new PatientListItem()
                        {
                            case_id = y.Key.case_id,
                            source_type = y.Key.source_type,
                            chart_no = y.Key.chart_no,
                            ipd_no = y.Key.ipd_no,
                            patient_name = y.Key.patient_name,
                            gender = y.Key.gender,
                            birth_day = y.Key.birth_day,
                            diag_date = y.Key.diag_date,
                            vs_doc = y.Key.vs_doc
                        }).ToList();
                    }
                    else if (this.DBLink.DBA.hasLastError)
                    {
                        rm.status = RESPONSE_STATUS.ERROR;
                        msg.Add("取的歷史清單錯誤，" + this.DBLink.DBA.lastError);
                        LogTool.SaveLogMessage(this.DBLink.DBA.lastError, actionName, GetLogToolCS.RTController);
                    }
                }
                else
                {
                    rm.status = RESPONSE_STATUS.ERROR;
                }
            }
            catch (Exception ex)
            {
                rm.status = RESPONSE_STATUS.EXCEPTION;
                msg.Add("取的歷史清單錯誤，" + ex.Message);
                LogTool.SaveLogMessage(ex, actionName, GetLogToolCS.RTController);
            }
            finally
            {
                if (msg != null && msg.Count > 0)
                    rm.message = string.Join("\n", msg);
                rm.attachment = pList;
            }

            return rm;
        }

    }

   

}

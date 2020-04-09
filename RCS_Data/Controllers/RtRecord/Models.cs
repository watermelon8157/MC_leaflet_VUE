using Com.Mayaminer;
using mayaminer.com.library;
using Newtonsoft.Json;
using RCS_Data.Models;
using RCS_Data.Models.DB;
using RCS_Data.Models.ViewModels;
using RCSData.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text.RegularExpressions;

namespace RCS_Data.Controllers.RtRecord
{
    public partial class Models : BaseModels, Interface
    {
        string csName { get { return "RtRecord.Models"; } }

        public RESPONSE_MSG getBloodBiochemicalData(IPDPatientInfo pat_info, UserInfo user_info, IWebServiceParam iwp)
        {
            RESPONSE_MSG rm = new RESPONSE_MSG();
            List<ExamBloodBiochemical> ExamBloodBiochemical = new List<ExamBloodBiochemical>();
            try
            {
                if (pat_info.chart_no != null && pat_info.ipd_no != null)
                    ExamBloodBiochemical = this.webmethod.getBloodBiochemicalData(iwp, pat_info.chart_no, pat_info.ipd_no);
                ExamBloodBiochemical = ExamBloodBiochemical.OrderByDescending(x => DateTime.Parse(x.examDate)).ToList();
            }
            catch (Exception ex)
            {
                rm.status = RESPONSE_STATUS.EXCEPTION;
                rm.message = "程式發生錯誤，請洽資訊人員!" + ex.Message;
                LogTool.SaveLogMessage(string.Format("使用者({0})病歷號({1})，錯誤訊息=({2})", user_info.user_id, pat_info.chart_no, ex.Message), "getBloodBiochemicalData");
            }
            rm.attachment = ExamBloodBiochemical;
            return rm;
        }

        public RESPONSE_MSG getBreathList(IPDPatientInfo pat_info, UserInfo user_info)
        {
            RESPONSE_MSG rm = new RESPONSE_MSG();
            string actioName = "getBreathList";
            List<BREATH_LIST> breathList = new List<BREATH_LIST>();
            try
            {
                SQLProvider SQL = new SQLProvider();
                string sql = "SELECT top 20 RECORD_ID,RECORDDATE,CREATE_NAME FROM RCS_RECORD_MASTER WHERE DATASTATUS = '1' AND IPD_NO = @IPD_NO AND CHART_NO = @CHART_NO ORDER BY RECORDDATE DESC";
                Dapper.DynamicParameters dp = new Dapper.DynamicParameters();
                dp.Add("IPD_NO", pat_info.ipd_no);
                dp.Add("CHART_NO", pat_info.chart_no);
                breathList = SQL.DBA.getSqlDataTable<BREATH_LIST>(sql, dp);
                if (breathList.Count > 0)
                {
                    sql = "SELECT * FROM RCS_RECORD_DETAIL WHERE RECORD_ID in @RECORD_ID AND ITEM_NAME in ('device','device_o2','mode')";
                    dp = new Dapper.DynamicParameters();
                    dp.Add("RECORD_ID", breathList.Select(x => x.RECORD_ID).ToList());
                    DataTable dt = SQL.DBA.getSqlDataTable(sql, dp);
                    foreach (BREATH_LIST item in breathList)
                    {
                        try
                        {
                            if (dt.AsEnumerable().ToList().Exists(x => x["RECORD_ID"].ToString() == item.RECORD_ID))
                            {
                                List<DataRow> drList = dt.AsEnumerable().ToList().FindAll(x => x["RECORD_ID"].ToString() == item.RECORD_ID).ToList();
                                item.mode = drList.Exists(x => x["ITEM_NAME"].ToString() == "mode") ? drList.Find(x => x["ITEM_NAME"].ToString() == "mode")["ITEM_VALUE"].ToString() : "";
                                item.device = drList.Exists(x => x["ITEM_NAME"].ToString() == "device") ? drList.Find(x => x["ITEM_NAME"].ToString() == "device")["ITEM_VALUE"].ToString() : "";
                                item.device_o2 = drList.Exists(x => x["ITEM_NAME"].ToString() == "device_o2") ? drList.Find(x => x["ITEM_NAME"].ToString() == "device_o2")["ITEM_VALUE"].ToString() : "";
                            }
                        }
                        catch (Exception ex)
                        {

                            LogTool.SaveLogMessage(ex, actioName, this.csName);
                        }

                    }
                }
                else
                {
                    if (SQL.DBA.hasLastError)
                    {
                        rm.message = "程式發生錯誤，請洽資訊人員!" + SQL.DBA.lastError;
                        LogTool.SaveLogMessage(SQL.DBA.lastError, actioName, this.csName);
                    }
                }
            }
            catch (Exception ex)
            {
                rm.message = "程式發生錯誤，請洽資訊人員!" + ex.Message;
                LogTool.SaveLogMessage(ex, actioName, this.csName);
            }
            rm.attachment = breathList;
            return rm;
        }

        /// <summary>
        /// 取得前三天的監控資料
        /// </summary>
        /// <returns></returns>
        public RESPONSE_MSG getMonitor(IPDPatientInfo pat_info, UserInfo user_info)
        {
            RESPONSE_MSG rm = new RESPONSE_MSG();
            string actioName = "getMonitor";
            List<CPTNewRecord> pList = new List<CPTNewRecord>();

            try
            {
                List<DB_RCS_CPT_NEW_RECORD_DTL> _temp = new List<DB_RCS_CPT_NEW_RECORD_DTL>();
                SQLProvider link = new SQLProvider();
                string _sql = " SELECT * FROM RCS_CPT_NEW_RECORD_DTL WHERE CPT_ID in( SELECT CPT_ID FROM RCS_CPT_NEW_RECORD WHERE REC_DATE BETWEEN @sDate AND @eDate AND CHART_NO = @CHART_NO AND DATASTATUS = '1')";
                Dapper.DynamicParameters dp = new Dapper.DynamicParameters();
                dp.Add("sDate", DateTime.Now.AddDays(-3).ToString("yyyy-MM-dd HH:mm"));
                dp.Add("eDate", DateTime.Now.AddDays(1).ToString("yyyy-MM-dd HH:mm"));
                dp.Add("CHART_NO", pat_info.chart_no);
                _temp = link.DBA.getSqlDataTable<DB_RCS_CPT_NEW_RECORD_DTL>(_sql, dp);
                if (_temp.Count > 0)
                {
                    foreach (string s in _temp.Select(x => x.CPT_ID).Distinct().ToList())
                    {
                        CPTNewRecord item = new CPTNewRecord();
                        item.cpt_id = s;
                        item.rec_date = _temp.Find(x => x.CPT_ID == s && x.ITEM_NAME == "rec_date").ITEM_VALUE;
                        item.HR_rest = _temp.Exists(x => x.CPT_ID == s && x.ITEM_NAME == "HR_rest") ? _temp.Find(x => x.CPT_ID == s && x.ITEM_NAME == "HR_rest").ITEM_VALUE : "";
                        item.HR_exercise = _temp.Exists(x => x.CPT_ID == s && x.ITEM_NAME == "HR_exercise") ? _temp.Find(x => x.CPT_ID == s && x.ITEM_NAME == "HR_exercise").ITEM_VALUE : "";
                        item.BP_rest = _temp.Exists(x => x.CPT_ID == s && x.ITEM_NAME == "BP_rest") ? _temp.Find(x => x.CPT_ID == s && x.ITEM_NAME == "BP_rest").ITEM_VALUE : "";
                        item.BP_exercise = _temp.Exists(x => x.CPT_ID == s && x.ITEM_NAME == "BP_exercise") ? _temp.Find(x => x.CPT_ID == s && x.ITEM_NAME == "BP_exercise").ITEM_VALUE : "";
                        item.SPO2_rest = _temp.Exists(x => x.CPT_ID == s && x.ITEM_NAME == "SPO2_rest") ? _temp.Find(x => x.CPT_ID == s && x.ITEM_NAME == "SPO2_rest").ITEM_VALUE : "";
                        item.SPO2_exercise = _temp.Exists(x => x.CPT_ID == s && x.ITEM_NAME == "SPO2_exercise") ? _temp.Find(x => x.CPT_ID == s && x.ITEM_NAME == "SPO2_exercise").ITEM_VALUE : "";
                        item.Borg_rest = _temp.Exists(x => x.CPT_ID == s && x.ITEM_NAME == "Borg_rest") ? _temp.Find(x => x.CPT_ID == s && x.ITEM_NAME == "Borg_rest").ITEM_VALUE : "";
                        item.Borg_exercise = _temp.Exists(x => x.CPT_ID == s && x.ITEM_NAME == "Borg_exercise") ? _temp.Find(x => x.CPT_ID == s && x.ITEM_NAME == "Borg_exercise").ITEM_VALUE : "";
                        pList.Add(item);
                    }
                }
                if (pList.Count > 0)
                {
                    pList = pList.OrderByDescending(x => DateTime.Parse(x.rec_date)).ToList();
                }

            }
            catch (Exception ex)
            {
                rm.status = RESPONSE_STATUS.EXCEPTION;
                rm.message = "程式發生錯誤，請洽資訊人員!" + ex.Message;
                LogTool.SaveLogMessage(ex, actioName, this.csName);
            }

            rm.attachment = pList;
            return rm;
        }

        public RESPONSE_MSG GetLastAbgList(IPDPatientInfo pat_info, UserInfo user_info, IWebServiceParam iwp)
        {
            RESPONSE_MSG rm = new RESPONSE_MSG();
            List<ExamABG> abglist = new List<ExamABG>();
            try
            {
                if (pat_info.chart_no != null && pat_info.ipd_no != null)
                    abglist = this.webmethod.getAVBGData(iwp, pat_info.chart_no, pat_info.ipd_no);
                //ABGdata排序
                if (abglist.Count > 0) abglist = abglist.OrderByDescending(x => DateTime.Parse(x.Date)).ToList();
            }
            catch (Exception ex)
            {
                rm.status = RESPONSE_STATUS.EXCEPTION;
                rm.message = "程式發生錯誤，請洽資訊人員!" + ex.Message;
                LogTool.SaveLogMessage(string.Format("使用者({0})病歷號({1})，錯誤訊息=({2})", user_info.user_id, pat_info.chart_no, ex.Message), "GetLastAbgList");
            }
            rm.attachment = abglist;
            return rm;
        }

        public virtual RESPONSE_MSG GetPhraseData(string pType)
        {
            RESPONSE_MSG rm = new RESPONSE_MSG();
            List<DB_RCS_SYS_PARAMS> RTRecordAllList = new List<DB_RCS_SYS_PARAMS>();
            rm.attachment = RTRecordAllList;
            return rm;
        }


        /// <summary>取得呼吸照護清單</summary> 
        /// <param name="StartDate">開始時間</param> 
        /// <param name="EndDate">結束時間</param> 
        /// <param name="EndDate">結束時間</param>
        /// <returns></returns>
        public RESPONSE_MSG RtRecordListData(ref List<RT_RECORD_MAIN> rtrecord_items, string pSDate, string pEDate, IPDPatientInfo pat_info, string pId, string record_id,  bool getOnModel = false)
        {
            RESPONSE_MSG rm = new RESPONSE_MSG();
            try
            { 
                rtrecord_items = this.getDuringRTRecord(pSDate, pEDate, pat_info, record_id,   getOnModel);

                foreach (RT_RECORD_MAIN item in rtrecord_items)
                {
                    item.rt_record._is_humidifier = item.rt_record.device;
                    List<string> on_dateLIst = new List<string>(); 
                }
            }
            catch (Exception ex)
            {
                LogTool.SaveLogMessage(ex, "RecordListData", GetLogToolCS.RTRecordController);
            }
            return rm;
        }

        /// <summary>CXR</summary>
        /// <returns></returns>
        public List<string> getResultDropdownlist()
        {
            List<string> SelectListItem = new List<string>();
            try
            {
                SelectListItem = this.getRCS_SYS_PARAMS("", "cxr_result", @pStatus: "1").Select(x => x.P_VALUE).ToList();
            }
            catch (Exception ex)
            {
                LogTool.SaveLogMessage(ex, "getRCS_SYS_PARAMS1", GetLogToolCS.BaseModel);
            }
            return SelectListItem;
        }

        public virtual RT_RECORD_DATA<RT_RECORD_MUST_DATA> changeRTRecordData(RT_RECORD_DATA<RT_RECORD_MUST_DATA> List)
        {
            return List;
        }

        /// <summary>
        /// 取得 OnePage 所需要的資料
        /// </summary>
        /// <param name="chrNo">病歷號</param>
        /// <returns></returns>
        public virtual List<OnePagePDF> OnePagePDFList(string chrNo)
        {
            DateTime nowDate = DateTime.Now;
            string getUrl = IniFile.GetConfig("System", "PDFURL");
            string emrid = "RTRecordPageForm", emrname = "呼吸照護記錄單";
            string parameter = "", sDate = "" , eDate ="";
            List<OnePagePDF> pList = new List<OnePagePDF>();
            List<DB_RCS_RT_CASE> patList = this.getDB_RCS_RT_CASEList(chrNo);
            foreach (DB_RCS_RT_CASE item in patList)
            {
                // &sDate=2020-02-23 00:00&eDate=2020-03-05 23:59
                sDate = string.IsNullOrWhiteSpace(item.CARE_DATE) ?
                    Function_Library.getDateString(nowDate, DATE_FORMAT.yyyy_MM_dd_0000) : Function_Library.getDateString(DateTime.Parse(item.CARE_DATE), DATE_FORMAT.yyyy_MM_dd_0000);
                eDate = Function_Library.getDateString(nowDate, DATE_FORMAT.yyyy_MM_dd_2359); 
                parameter =string.Format("&sDate={0}&eDate={1}", sDate, eDate);
                pList.Add(new OnePagePDF()
                {
                    emrid = emrid,
                    emrname = emrname,
                    fee_no = item.IPD_NO,
                    url = string.Concat(getUrl, string.Format("PDFPage?chart_no={0}&ipd_no={1}&parameter={2}&form_id={3}&form_name={4}", item.CHART_NO, item.IPD_NO, RCS_Data.Models.Function_Library.getcodeDES(parameter), emrid, emrname))
                });
            }
            return pList;
        }

    }

    public class SelectItemDetail 
    {
        /// <summary>
        /// 意識EVM下拉選單
        /// </summary>
        public List<DDLItem> conscious_e { get; set; }
        /// <summary>
        /// 意識EVM下拉選單
        /// </summary>
        public List<DDLItem> conscious_v { get; set; }
        /// <summary>
        /// 意識EVM下拉選單
        /// </summary>
        public List<DDLItem> conscious_m { get; set; }
        public List<DeviceMaster> devicelist { get; set; } 
        public List<DDLItem> device_O2DLL { get; set; }

    }

    public class FormBody : AUTH
    {
        public string type { get; set; }
    }
    public class Form_SelectListItem : AUTH, IForm_SelectListItem
    {
        public string P_MODE { get; set; }
        public string P_GROUP { get; set; }
    }

    public class FormWEANING_ASSESS_CHECKLIST : AUTH
    {
        public DB_RCS_WEANING_ASSESS_CHECKLIST vm { get; set; }
    }

    public class FormBodyRT_RECORD_VIEW : AUTH
    {
        public bool space_page { get; set; }
        public bool isVIP { get; set; }
        public string record_id { get; set; }

        public RT_RECORD model { get; set; }

    }

    public class FormGetMVdays : AUTH
    {
        public string pDate { get; set; }
    }

    public class BREATH_LIST
    {
        /// <summary>流水號</summary>
        public string RECORD_ID { set; get; }
        /// <summary>記錄單日期時間</summary>
        public string RECORDDATE { set; get; }
        /// <summary>建立者姓名</summary>
        public string CREATE_NAME { set; get; }

        public string device { get; set; }
        public string device_o2 { get; set; }
        public string mode { get; set; }
    }

    public class Form_RtRecordList : AUTH, IForm_RtRecordList
    {
        public string pSDate { get; set; }

        public string pEDate { get; set; }
        public string pipd_no { get; set; }
        public string pId { get; set; }
        public string record_id { get; set; }

    }
}

//**************************************************
//2016/08/19
//#2172 建立DB Model
//功能 照護病患清單、照護病患紀錄、授權清單功能
//2016/08/19 整理:將不用使用的程式碼移除
//2016/08/19 整理:取得資料移動到DB/RT.cs裡面
//**************************************************
using Com.Mayaminer;
using iTextSharp.text;
using iTextSharp.text.pdf;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RCS_Data;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data;
using mayaminer.com.library;
using RCS.Models;
using mayaminer.com.jxDB;
using Dapper;
using System.Text.RegularExpressions;
using RCS.Models.DB;
using RCS_Data.Models.DB;
using RCSData.Models;
using RCS_Data.Models.ViewModels;
using RCS_Data.Models.VIP;
using RCS_Data.Controllers.RT;
using RCS_Data.Controllers.ListHistory;

namespace RCS.Controllers {
    public class RTController : BaseController {

        public ListHistoryModels _model { get; set; }
        protected ListHistoryModels listHistoryModels
        {
            get
            {
                if (this._model == null)
                {
                    this._model = new ListHistoryModels();
                }
                return this._model;
            }
        }

        string csName { get { return "RTController"; } }
        /// <summary>
        /// 顯示手輸資料
        /// </summary>
        bool showManually = true;
        /// <summary>取得照護病患清單、照護病患紀錄、授權清單資料</summary>
        RT model = new RT();

        #region 主畫面 View

        /// <summary>第一階網站(最外接網站)
        /// <para>MainLayout，最上面的Tab(呼吸治療管理、區域分派、輔具評估單、系統管理和登出系統等功能)</para>
        /// </summary>
        /// <returns>View</returns>
        public ActionResult Index()
        {
            ViewData["user_info"] = user_info;
            return View();
        }

        /// <summary>顯示側邊選單</summary>
        /// <param name="isGuide">是否用網址導入網頁</param>
        /// <returns>View</returns>
        public ActionResult RT_Index(bool isGuide)
        {
            RT_index RT_index = new RT_index();
            RT_index.bed_area = model.web_method.getHisBedAreaList(this.hospFactory.webService.HisBedAreaList());//呼吸記錄單相關下拉列表清單 
            RT_index.upLoadData = new upLoadIndex();
            RT_index.searchipdSDate = DateTime.Now.AddDays(-7).ToString("yyyy-MM-dd");
            RT_index.searchipdEDate = DateTime.Now.ToString("yyyy-MM-dd");
            RT_index.tabList = BaseModel.getRCS_SYS_PARAMS("RT", "tablist", @pStatus: "1");
            RT_index.vs_doc_list = BaseModel.getRCS_SYS_PARAMS("Shared", "doctor_name", @pStatus: "1");

            RT_index.isGuide = isGuide;
            if (isGuide)
            {
                RT_index.ipd_no = pat_info.ipd_no;
                RT_index.chart_no = pat_info.chart_no;
                if (!string.IsNullOrWhiteSpace(pat_info.diag_date) && DateHelper.isDate(pat_info.diag_date))
                {
                    RT_index.searchipdSDate = DateTime.Parse(pat_info.diag_date).ToString("yyyy-MM-dd");
                }
                ViewData["pat_info"] = pat_info;
            }
            return View("RT_Index", RT_index);
        }

        #endregion

        #region 照護清單功能

        /// <summary> 取得病人清單 </summary>
        /// <param name="data_type"></param>
        /// <returns></returns>
        public ActionResult PatientList(string data_type = "")
        {
            try
            {
                model.PatientList = model.get_CareIList(user_info.user_id);
                if (model.PatientList != null && model.PatientList.Count > 0)
                {
                    VIPRTTBL VIP_DATA = new VIPRTTBL();
                    //查詢每一次病患的暫存交班單
                    SQLProvider SQL = new SQLProvider();
                    Dapper.DynamicParameters dp = new DynamicParameters();
                    dp.Add("IPD_NO", model.PatientList.Select(x=>x.ipd_no).ToList());
                    string _query = "SELECT IPD_NO FROM RCS_RT_ISBAR_SHIFT WHERE IPD_NO in @IPD_NO AND STATUS = '1'";
                    List<string> ipd_list = SQL.DBA.getSqlDataTable<string>(_query, dp);

                    foreach (PatientListItem item in model.PatientList)
                    {
                        //查詢每一次病患的暫存交班單
                        if (funSetting.shiftWorkSwitch)
                        {
                            //檢查是否有暫存交班單
                            if (ipd_list.Exists(x=>x == item.ipd_no))
                                item.hadShift_record = true;
                        }
                        RESPONSE_MSG rm = VIP_DATA.checkVIPDataHasRepeat(item.chart_no);
                        if (rm.status != RESPONSE_STATUS.SUCCESS)
                        {
                            item.hadAlarm_msg = true;
                            item.alarm_msg = rm.message.Replace("<br \\>", "&#10;");
                        }
                        if (MvcApplication.ipd_list.Exists(x=>x.chart_no == item.chart_no && x.ipd_no == item.ipd_no))
                        {
                            #region 轉床
                            IPDPatientInfo _pat_info = MvcApplication.ipd_list.Find(x => x.chart_no == item.chart_no && x.ipd_no == item.ipd_no);
                            if (!string.IsNullOrWhiteSpace(_pat_info.bed_no) && item.bed_no != _pat_info.bed_no)
                                item.new_bed_no = _pat_info.bed_no;
                            if (!string.IsNullOrWhiteSpace(_pat_info.cost_desc) && item.cost_desc != _pat_info.cost_desc)
                                item.new_cost_desc = _pat_info.cost_desc;
                            if (!string.IsNullOrWhiteSpace(_pat_info.cost_code) && item.cost_code != _pat_info.cost_code)
                                item.new_cost_code = _pat_info.cost_code;
                            if (!string.IsNullOrWhiteSpace(_pat_info.vs_doc) && item.vs_doc != _pat_info.vs_doc)
                                item.new_vs_doc = _pat_info.vs_doc;
                            if (!string.IsNullOrWhiteSpace(_pat_info.vs_id) && item.vs_id != _pat_info.vs_id)
                                item.new_vs_id = _pat_info.vs_id;
                            if (!string.IsNullOrWhiteSpace(_pat_info.dept_code) && item.dept_code != _pat_info.dept_code)
                                item.new_dept_code = _pat_info.dept_code;
                            if (!string.IsNullOrWhiteSpace(_pat_info.dept_desc) && item.dept_desc != _pat_info.dept_desc)
                                item.new_dept_desc = _pat_info.dept_desc;
                            if (!string.IsNullOrWhiteSpace(_pat_info.MDRO_MARK) && item.MDRO_MARK != _pat_info.MDRO_MARK)
                                item.MDRO_MARK = _pat_info.MDRO_MARK;
                            if (!string.IsNullOrWhiteSpace(_pat_info.new_ipd_no) && item.ipd_no != _pat_info.new_ipd_no)
                                item.new_ipd_no = _pat_info.new_ipd_no;
                            //測試轉單位樣式
                            //if (RCS.Controllers.BaseController.isDebuggerMode)
                            //{
                            //    item.new_bed_no = "aaa";
                            //    item.new_cost_desc = "bbb";
                            //    item.new_cost_code = "ccc";
                            //    item.new_vs_doc = "ddd";
                            //    item.new_vs_id = "eee";
                            //    item.new_dept_code = "fff";
                            //    item.new_dept_desc = "ggg";
                            //}
                            #endregion

                        } 
 
                    }
                }
                model.PatientList.ForEach(x => x.setOnMode());
            }
            catch (Exception ex)
            {
                LogTool.SaveLogMessage(ex, "PatientList",GetLogToolCS.RTController);
            }
            if (data_type.ToLower() == "json")
            {
                this.getPatDeatailData(ref model.PatientList);
                return Content(JsonConvert.SerializeObject(model.PatientList));
            }
            else
            {
                ViewData["patlist"] = model.PatientList;
                return View();
            }
        }

        #region 功能 RT action function

        /// <summary>搜尋病患</summary>
        /// <param name="chart_no">病歷號</param>
        /// <param name="ipd_no">住院帳號</param>
        /// <returns>Json</returns>
        public ActionResult SearchPatient(string chart_no, string ipd_no = "")
        {
            List<IPDPatientInfo> return_list = new List<IPDPatientInfo>();
            try
            {
                if (funSetting.aadPatientCheckChartNo)
                {
                    int chart_no_NumBit = int.Parse(IniFile.GetConfig("System", "ChartNoNNumBits"));
                    if (chart_no.Length > 0 && chart_no.Length < chart_no_NumBit)
                    {
                        chart_no = chart_no.ToString().PadLeft(chart_no_NumBit, '0');
                    }
                }
                if (ipd_no == "")
                {
                    List<PatientHistory> PatientHistory = model.web_method.getPatientHistory(this.hospFactory.webService.HISPatientHistory(), chart_no, ref ipd_no);
                }
                if (!string.IsNullOrWhiteSpace(ipd_no))
                {
                    List<IPDPatientInfo> PtInfo = model.web_method.getPatientInfoList(this.hospFactory.webService.HISPatientInfo(), chart_no, ipd_no);
                    if (PtInfo != null && PtInfo.Count> 0)
                    {
                        PtInfo.ForEach(x => x.source_type = "I");
                        return_list.AddRange(PtInfo);
                    }
                         
                }
                //收尋急診門診病患
                if (funSetting.SearchErAndOpPatient)
                {
                    List<IPDPatientInfo> erAndOPDList = model.web_method.getERAndOPDListbyChartNo(chart_no, this.hospFactory.webService.HISGetPatient_ER_OPD_LIST());
                    if (erAndOPDList.Count>0)
                    {
                        return_list.AddRange(erAndOPDList);
                    }
                }

            }
            catch (Exception ex)
            {
                LogTool.SaveLogMessage(ex, "SearchPatient",GetLogToolCS.RTController);
            }
           
            return Json(return_list, JsonRequestBehavior.AllowGet);
        }

        /// <summary>取得病床清單</summary>
        /// <param name="area_code">區域代碼</param>
        /// <returns>Json</returns>
        public ActionResult GetBedList(string area_code, string case_code)
        {
            List<IPDPatientInfo> PatientList_F = model.GetBedListData(this.hospFactory.webService.AreaPatientList(), area_code, case_code);
            List<SysParams> doctor_list = model.GetGroupList(BaseModel.getRCS_SYS_PARAMS("Shared", ""), "doctor_name");
            PatientList_F.ForEach(x => x.source_type = "I");
            return Json(PatientList_F, JsonRequestBehavior.AllowGet);
        }

        /// <summary>加入照護清單</summary>
        /// <param name="join_json">病患資料組</param>
        /// <returns>Json</returns>
        public ActionResult JoinCareItem(string join_json)
        {
            WebServiceResponse return_ws = model.JoinCareItem(this.hospFactory.webService.HISPatientInfo(), join_json);
            return Json(return_ws, JsonRequestBehavior.AllowGet);
        }

        /// <summary>刪除照護病患</summary>
        /// <param name="delete_json">刪除病患r_id組</param>
        /// <returns>Json</returns>
        public ActionResult DeleteItem(string delete_json, string ipd_json)
        {
            WebServiceResponse return_ws = model.DeleteItem(delete_json, ipd_json);
            return Json(return_ws, JsonRequestBehavior.AllowGet);
        }

        /// <summary> 結案照護病患</summary>
        /// <param name="delete_json">結案病患r_id組</param>
        /// <returns>Json</returns>
        public ActionResult EndItem(string end_json)
        {
            WebServiceResponse return_ws = model.EndItem(end_json);
            return Json(return_ws, JsonRequestBehavior.AllowGet);
        }

        /// <summary> 設定更新照護病患基本資料</summary>
        /// <param name="json"></param>
        /// <returns></returns>
        public ActionResult set_CarePatPatientInfo(IPDPatientInfo data)
        {
            RESPONSE_MSG rm = new RESPONSE_MSG();
            dbResultMessage dbMsg = new dbResultMessage();
            try
            {
                // TODO: 尚未完整測試 改dapper 設定更新照護病患基本資料
                SQLProvider SQL = new SQLProvider();
                string sqlStr = string.Concat("UPDATE " + GetTableName.RCS_RT_CASE + " SET BODY_HEIGHT = ", SQL.namedArguments, "BODY_HEIGHT  WHERE IPD_NO = ", SQL.namedArguments, "IPD_NO");
                DynamicParameters dp = new DynamicParameters();
                dp.Add("BODY_HEIGHT", data.body_height);
                dp.Add("IPD_NO", data.ipd_no);
                SQL.DBA.DBExecute(sqlStr, dp);
                if (SQL.DBA.hasLastError)
                {
                    LogTool.SaveLogMessage(dbMsg.dbErrorMessage, "set_CarePatPatientInfo", GetLogToolCS.RTRecordController);
                    rm.status = RESPONSE_STATUS.ERROR;
                    rm.message = "儲存失敗";
                }
                else
                {
                    rm.status = RESPONSE_STATUS.SUCCESS;
                    rm.message = "儲存成功";
                }
            }
            catch (Exception ex)
            {
                rm.status = RESPONSE_STATUS.EXCEPTION;
                rm.message = "程式發生錯誤，請洽資訊人員!";
                LogTool.SaveLogMessage(ex, "set_CarePatPatientInfo", GetLogToolCS.RTRecordController);
            }
            return Content(JsonConvert.SerializeObject(rm));
        }

      
        #endregion

        #endregion

        #region 病患歷史記錄功能
        /// <summary>
        /// 取得病患歷史紀錄
        /// </summary>
        /// <param name="chart_no">病歷號(必填)</param>
        /// <param name="sDate">入院日期起(必填)</param>
        /// <param name="eDate">入院日期訖(必填)</param>
        /// <returns></returns>
        public ActionResult getPathistory(RT_index_pathistory pValue)
        { 
            RESPONSE_MSG rm = this.listHistoryModels.Pathistory(pValue); 
            Response.Write(JsonConvert.SerializeObject(rm));
            return new EmptyResult();
        }

        #endregion 

        #region 呼吸器狀況StatusIndex(Machine function)

        /// <summary>呼吸器狀況</summary>
        /// <param name="before_day">幾天前，預設7天前</param>
        /// <param name="data_type">資料類型</param>
        /// <returns></returns>
        public ActionResult StatusIndex(int before_day = 2, string data_type = "", string search_text = "一般")
        {
            VIPRTTBL VIPRTTBL = new VIPRTTBL();
            RESPONSE_MSG rm = VIPRTTBL.checkVIPDataHasRepeat(pat_info.chart_no);
            return View("Status", rm);
        }

        /// <summary>呼吸器狀態(新版結構) </summary>
        /// <param name="before_day">取得幾天前</param>
        /// <returns>RT_RECORD_MAIN[]</returns>
        [HttpPost]
        public string FormStatusData(int before_day = 2)
        {
            List<RT_RECORD_MAIN> rt_record_main_list = new List<RT_RECORD_MAIN>();
            try
            {
                VIPRTTBL VIPRTTBL = new VIPRTTBL();
                VIPRTTBL.getRt_Record_Main_List(before_day, pat_info.chart_no);
                return JsonConvert.SerializeObject(VIPRTTBL.rt_record_main_list);
            }
            catch (Exception ex)
            {
                LogTool.SaveLogMessage(ex, "FormStatusData", GetLogToolCS.RTController);
            }
            return JsonConvert.SerializeObject(rt_record_main_list);
        }

        #endregion

        #region 列印病人清單
        public ActionResult exportExcel(RTListjsonData Data)
        {
            string actionName = "exportExcel";
            string user_id = string.IsNullOrEmpty(user_info.user_id) ? "" : user_info.user_id;
            string user_name = string.IsNullOrEmpty(user_info.user_name) ? "" : user_info.user_name;
            try
            {
                List<PatientListItem> pli = Data.pli;
                this.getPatDeatailData(ref pli);
                if (pli != null && pli.Count > 0)
                {
                    pli.ForEach(x => { x.setOnMode("\r\n");x.o2_device = string.IsNullOrWhiteSpace(x.o2_device) ? "" : x.o2_device.Replace("&#10;", "\r\n"); });
                  
                    ExcelSetting em = new ExcelSetting();
                    em.bindColName = "bed_no,dept_code,vs_doc,patient_name,chart_no,diagnosis_code,device,on_mode,o2_device,memo";
                    em.colTitleName = "床號,科別,主治醫生,姓名,病歷號,診斷,Ventilator Type,on機時間,O2 device,備註";
                    em.titleName = "病人清單";
                    em.sheetName = "sheet1";
                    em.FileName = "病人清單.xls";
                    em.exportActionName = "ExportExcel_RT_Index";
                    exportFile exportFile = new exportFile( em.FileName);
                    return exportFile.exportExcel<PatientListItem>(pli, em);
                }
            }
            catch (Exception ex)
            {
                LogTool.SaveLogMessage(ex, actionName, GetLogToolCS.RTController);
            }
            return new EmptyResult();
        }

        /// <summary>
        /// 取得病患資料詳細資料
        /// </summary>
        /// <param name="pli"></param>
        private void getPatDeatailData(ref List<PatientListItem> pli)
        {
            string actionName = "getPatDeatailData";
            try
            {
                #region 整理資料
                List<string> ipd_noList = pli.Select(x => SQLDefend.SQLString(x.ipd_no)).ToList();
                if (ipd_noList != null && ipd_noList.Count > 0)
                {
                    string ipd_str = string.Join(",", ipd_noList);

                    string sql = "";
 
                    if (this.DBA.LastError != null && this.DBA.LastError != "")
                    {
                        LogTool.SaveLogMessage(this.DBA.LastError, actionName, GetLogToolCS.RTController);
                    }
                    foreach (PatientListItem item in pli)
                    {
                        item.respid = item.machine.respid;
                        item.mode = item.machine.mode;
                       
                        List<string> cptData = BaseModel.getCPTAssess(item.chart_no);
                        if (cptData.Count> 0)
                        {
                            item.memo = cptData[2];
                            if (!string.IsNullOrWhiteSpace(cptData[4]))
                                item.diagnosis_code = cptData[4];
                        }
                    }
                }
                #endregion
                pli = pli.OrderBy(x => x.bed_no).ThenBy(x => x.chart_no).ToList();
            }
            catch (Exception ex)
            {
                LogTool.SaveLogMessage(ex, actionName,this.csName);
            }
           
        }


        //頁首頁尾的page事件
        public class HandleFooter : PdfPageEventHelper
        {
            public string UserName;
            PdfContentByte cb;
            PdfTemplate tmp;
            BaseFont bf = null;
            DateTime prtTime = DateTime.Now;

            public override void OnOpenDocument(PdfWriter writer, Document document)
            {
                try
                {
                    string fontPath = Environment.GetFolderPath(Environment.SpecialFolder.System) + @"\..\Fonts\kaiu.ttf";
                    prtTime = DateTime.Now;
                    bf = BaseFont.CreateFont(fontPath, BaseFont.IDENTITY_H, BaseFont.NOT_EMBEDDED);
                    cb = writer.DirectContent;
                    tmp = cb.CreateTemplate(50, 50);
                }
                catch { }
            }

            public override void OnEndPage(PdfWriter writer, Document document)
            {
                base.OnEndPage(writer, document);
                int pageN = writer.PageNumber;
                Rectangle PageSize = document.PageSize;
                cb.SetRGBColorFill(100, 100, 100);

                //頁首
                cb.BeginText();
                cb.SetFontAndSize(bf, 15);
                cb.ShowTextAligned(PdfContentByte.ALIGN_CENTER, "彰化基督教醫院", PageSize.GetRight(300), PageSize.GetTop(20), 0);
                cb.ShowTextAligned(PdfContentByte.ALIGN_CENTER, "病人清單", PageSize.GetRight(300), PageSize.GetTop(40), 0);
                cb.EndText();

                cb.BeginText();
                cb.SetFontAndSize(bf, 12);
                cb.ShowTextAligned(PdfContentByte.ALIGN_LEFT, "製表時間：" + DateTime.Now.ToString("HH:mm"), PageSize.GetLeft(45), PageSize.GetTop(40), 0);
                cb.ShowTextAligned(PdfContentByte.ALIGN_LEFT, "製表日期：" + DateTime.Now.ToString("yyyy-MM-dd"), PageSize.GetLeft(45), PageSize.GetTop(20), 0);
                cb.ShowTextAligned(PdfContentByte.ALIGN_LEFT, "列印人員：" + UserName, PageSize.GetRight(160), PageSize.GetTop(40), 0);
                cb.EndText();

                //頁尾右下方資訊
                cb.BeginText();
                cb.SetFontAndSize(bf, 8);
                cb.ShowTextAligned(PdfContentByte.ALIGN_RIGHT, "第  " + pageN.ToString() + "  頁", PageSize.GetRight(10), PageSize.GetBottom(10), 0);
                cb.EndText();
            }

            public override void OnCloseDocument(PdfWriter writer, Document document)
            {
                base.OnCloseDocument(writer, document);
                tmp.BeginText();
                tmp.SetFontAndSize(bf, 8);
                tmp.SetTextMatrix(0, 0);
                tmp.EndText();
            }
        }
        
        #endregion
         

        #region 取得病患基本資料相關

        /// <summary>
        /// 取得病患現在狀態
        /// (INTUBE)插管
        /// (MV)呼吸
        /// (O2)氧療
        /// (DNR)DNR_MARK註記，拒絕心肺復甦術
        /// (VPN)VPN_MARK註記，呼吸器依賴患者連續使用21 天以上
        /// (CPT)CPT註記
        /// </summary>
        /// <returns>Json</returns>
        public string GetPatStatus(string pList)
        {
            string actionName = "GetPatStatus";
            if (string.IsNullOrWhiteSpace(pList)) pList = "[]";
             SetPatNowStatus stauts = new SetPatNowStatus();
            try
            {
                List<PatNowStatus> pLIst = JsonConvert.DeserializeObject<List<PatNowStatus>>(pList);
                if (pLIst.Count > 0)
                {
                    if (pLIst.Exists(x=>x.StatusDesc=="isVPN"))
                    {
                        if (!string.IsNullOrWhiteSpace(pat_info.chart_no) && !string.IsNullOrWhiteSpace(pat_info.case_id))
                        {
                            string isShow = "";
                            if(pLIst.Find(x => x.StatusDesc == "isVPN").showStatus)
                            {
                                isShow = "1";
                            }
                            else
                            {
                                isShow = "2";
                            }
                            string sql = string.Format("UPDATE {1} SET VPN_MARK = '"+ isShow + "' WHERE CHART_NO={0} AND case_id = {2} ", SQLDefend.SQLString(pat_info.chart_no), GetTableName.RCS_RT_CASE.ToString(), SQLDefend.SQLString(pat_info.case_id));
                            this.DBA.ExecuteNonQuery(sql);
                            if(!string.IsNullOrWhiteSpace(this.DBA.LastError))
                            {
                                LogTool.SaveLogMessage(this.DBA.LastError, actionName,GetLogToolCS.RTController);
                            }
                        }
                       
                    }
                }
                // Request["status"] => 2:記錄中, 1:結束
                if (!string.IsNullOrWhiteSpace(pat_info.chart_no))
                {

                    #region CPT DNR VPN

                    //CPT
                    Dictionary<string,bool> statusList = BaseModel.getCaseStatus(pat_info.chart_no);
                    foreach (KeyValuePair<string,bool> item in statusList)
                    {
                        stauts.Find(x => x.StatusCode == item.Key).showStatus = item.Value;
                    }
                    #endregion

                    #region 產生OnTypeMode
                    DataTable Dt = new RTRecord().getFinalRTRecord(pat_info.chart_no);
                    Dictionary<string, string> dictionary = new Dictionary<string, string>();

                    if (Dt != null && Dt.Rows.Count > 0)
                    {
                        List<DB_RCS_RECORD_DETAIL> recordList = Newtonsoft.Json.JsonConvert.DeserializeObject<List<DB_RCS_RECORD_DETAIL>>(Newtonsoft.Json.JsonConvert.SerializeObject(Dt));
                        dictionary = recordList.ToDictionary(row => row.ITEM_NAME, row => row.ITEM_VALUE);
                    }
                    List<string> valKeyName = new List<string>();
                    //插管
                    valKeyName = new List<string>() { "artificial_airway_type" };
                    if (dictionary.Keys.ToList().Exists(x => valKeyName.Contains(x)) &&
                        !dictionary.Where(x => valKeyName.Contains(x.Key)).ToDictionary(x => x.Key, x => x.Value).Values.ToList().Exists(x => string.IsNullOrWhiteSpace(x)))
                        stauts.Find(x => x.StatusCode == "INTUBE").showStatus = true;
                    //呼吸
                    valKeyName = new List<string>() { "respid" };
                    if (dictionary.Keys.ToList().Exists(x=> valKeyName.Contains(x)) && 
                         !dictionary.Where( x=> valKeyName.Contains(x.Key)).ToDictionary(x=>x.Key,x=>x.Value).Values.ToList().Exists(x=> string.IsNullOrWhiteSpace(x))  )
                        stauts.Find(x => x.StatusCode == "MV").showStatus = true;
                    //養療
                    valKeyName = new List<string>() { "device_o2" };
                    if (dictionary.Keys.ToList().Exists(x => valKeyName.Contains(x)) &&
                        !dictionary.Where(x => valKeyName.Contains(x.Key)).ToDictionary(x => x.Key, x => x.Value).Values.ToList().Exists(x => string.IsNullOrWhiteSpace(x)))
                        stauts.Find(x => x.StatusCode == "O2").showStatus = true; 
                    #endregion 
                }
 

            }
            catch (Exception ex)
            {
                LogTool.SaveLogMessage(ex, "GetPatStatus", GetLogToolCS.RTController);
            }
            return JsonConvert.SerializeObject(stauts);
        }

        #endregion

        public ActionResult RT_modal()
        {
            return View();
        }
        

        /// <summary>
        /// 取得輸入輸出資料
        /// </summary>
        /// <param name="io_sdate"></param>
        /// <param name="io_edate"></param>
        /// <returns></returns>
        public JsonResult search_io(string io_sdate, string io_edate)
        {

            RCS_Data.Controllers.HisData.Models HisDataModel = new RCS_Data.Controllers.HisData.Models();


            List<IO_PUT> finalIO_PUTList = new List<IO_PUT>();

            if (!string.IsNullOrWhiteSpace(io_sdate) && !string.IsNullOrWhiteSpace(io_edate))
            {
                IPDPatientInfo pat_info = this.pat_info;
                var rm = HisDataModel.Search_io(ref finalIO_PUTList, io_sdate, io_edate, pat_info, this.hospFactory.webService.HISInputOutput());
            }

            return Json(finalIO_PUTList);
        }//search_io

        /// <summary>
        /// 取得詳細輸入輸出資料
        /// </summary>
        /// <param name="io_sdate"></param>
        /// <param name="io_edate"></param>
        /// <returns></returns>
        public JsonResult detail_search_io(string io_sdate, string io_edate)
        {
            List<IO_PUT> plist = new List<IO_PUT>();
            List<IO_PUT> TempRangelist = new List<IO_PUT>();
            List<IO_PUT> Rangelist = new List<IO_PUT>();
            string actionName = "detail_search_io";
            try
            {
                TempRangelist = BaseModel.web_method.getInputOutput(this.hospFactory.webService.HISInputOutput(), pat_info.chart_no, pat_info.ipd_no, io_sdate, DateTime.Parse(io_edate).AddDays(+1).ToString("yyyy-MM-dd"));
                io_sdate = io_sdate + " 07:00:00";
                io_edate = io_edate + " 07:00:00";
                Rangelist = TempRangelist.OrderByDescending(x => x.EXAM_DATE).ToList();
                foreach (IO_PUT itemA in Rangelist)
                {
                    if (DateTime.Parse(itemA.EXAM_DATE) > DateTime.Parse(io_sdate)
                        && DateTime.Parse(itemA.EXAM_DATE) <= DateTime.Parse(io_edate).AddDays(+1)
                    )
                    {
                        plist.Add(itemA);
                    }
                }
            }
            catch (Exception ex)
            {
                LogTool.SaveLogMessage(ex, actionName, this.csName);
            }
            return Json(plist);
        }//detail_search_io

        /// <summary>
        /// 取得vitalSing的資料
        /// </summary>
        /// <param name="vs_sdate"></param>
        /// <param name="vs_edate"></param>
        /// <returns></returns>
        public JsonResult search_vs(string vs_sdate, string vs_edate)
        {
            List<VitalSign> returnVitalSignList = new List<VitalSign>();
            returnVitalSignList = BaseModel.web_method.getVitalSign(this.hospFactory.webService.HISVitalSign(), pat_info.chart_no, pat_info.ipd_no, vs_sdate , vs_edate);
            LogTool.SaveLogMessage(Newtonsoft.Json.JsonConvert.SerializeObject(returnVitalSignList), "search_vs", "SYS"); 
            return Json(returnVitalSignList);
        }//search_vs

        public JsonResult cpt_data()
        {
            string actionName = "cpt_data";
            try
            {
                List<string> cptData = BaseModel.getCPTAssess(pat_info.chart_no);
                return Json(cptData[0]);
            }
            catch (Exception ex)
            {
                LogTool.SaveLogMessage(ex, actionName, this.csName);
            }
            return Json("");
        }

        /// <summary>
        /// COPD系統外部連結
        /// 測試連結
        /// </summary> 
        /// <returns></returns>
        public ActionResult openSystem(string user_id, string user_name)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(user_id) || string.IsNullOrWhiteSpace(user_name))
                {
                    return Content("無登入資料，請洽資訊人員!");
                }
                string uiAuthority = "";
                Dapper.DynamicParameters dp = new DynamicParameters();
                string sqlStr = string.Concat("select P_GROUP,P_NAME,p_value from ", GetTableName.RCS_SYS_PARAMS.ToString(), " where P_MODEL = 'user' and P_VALUE= @USER_ID AND P_STATUS='1'");
                dp = new DynamicParameters(new { USER_ID = user_id });
                List<RCS_SYS_PARAMS> tempList = new SQLProvider().DBA.getSqlDataTable<RCS_SYS_PARAMS>(sqlStr, dp);

                if (tempList.Count > 0)
                {
                    PAYLOAD payload = new PAYLOAD()
                    {
                        user_id = user_id,
                        user_name = user_name,
                        role = tempList.Exists(x => x.p_value == user_id) ? tempList.Find(x => x.p_value == user_id).p_group : "",
                        iat = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")
                    };
                    string token = RCS.Models.JwtAuthActionFilterAttribute.EncodeToken(payload);
                    TempData["token"] = token;
                   return RedirectToAction("OpenSystem", "External");
                } 
               
            }
            catch (Exception ex)
            {
                LogTool.SaveLogMessage(ex, "COPD", "External");
                return Content("登入驗證錯誤，請洽資訊人員!");
            }
            return Content("登入驗證錯誤，請洽資訊人員!");
        }
    }
}

//**************************************************
//CPT記錄表與患者評估共用部分皆放在Models/RTAssess.cs
//共用部分：列印、刪除
//**************************************************
using RCS_Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Com.Mayaminer;
using System.Data.Common;
using System.Web.Script.Serialization;
using System.Web.Configuration;
using System.Data;
using System.Text;
using System.Reflection;
using Newtonsoft.Json;
using System.Net;
using System.IO;
using RCS.Models;
using RCS.Models.DB;
using mayaminer.com.library;
using Dapper;
using RCSData.Models;
using RCS_Data.Models.DB;
using RCS_Data.Models.ViewModels;
using RCS_Data.Controllers.HisData;
using RCS.Controllers.WEBAPI;
using RCS.Models.ViewModel;
using RCS_Data.Controllers;

namespace RCS.Controllers
{
    public class CPTController : BaseController
    {
        private CPT CPTModel; 
        private CxrCanvasLineDraw CxrCanvasLineDrawMdl; //2018.07.20 CXR畫線
        private RTRecordController RTRecordCtrl;
        private RtAssessController RtAssessController;

        public CPTController()
        {
            CPTModel = new CPT(); 
            CxrCanvasLineDrawMdl = new CxrCanvasLineDraw(); //2018.07.20 CXR畫線
            RTRecordCtrl = new RTRecordController();
            RtAssessController = new RtAssessController();
        }
        RtAssess _model;
        RtAssess rtAssessModel
        {
            get
            {
                if (_model == null)
                {
                    this._model = new RtAssess();
                }
                return this._model;
            }
        }
  

        #region 般ECK

        public ActionResult IndexTet()
        {
            List<SelectListItem> getPatientHistoryList = BaseModel.getPatientHistoryList(pat_info.chart_no, pat_info.ipd_no);
            ViewData["getPatientHistoryList"] = getPatientHistoryList;
            if (getPatientHistoryList.Exists(x => x.Selected))
            {
                SelectListItem item = new SelectListItem();
                item = getPatientHistoryList.Find(x => x.Selected);
                ViewData["sDate"] = BaseModel.getHistoryList(item.Value, 1);
                ViewData["eDate"] = BaseModel.getHistoryList(item.Value, 2);
            }
            return View();
        }

        public ActionResult OpenCPTindexTetForm(string rowstr)
        {
            string actionName = "OpenCPTindexTetForm";
            //List<RCS_CPT_DTL_NEW_ITEMS> cpt_dtl_new_items = new List<RCS_CPT_DTL_NEW_ITEMS>();
            RCS_CPT_DTL_NEW_ITEMS row = new RCS_CPT_DTL_NEW_ITEMS();
            string rowStr = HttpUtility.UrlDecode(rowstr);
            row = JsonConvert.DeserializeObject<RCS_CPT_DTL_NEW_ITEMS>(rowStr);

            //相關下拉列表清單          
            //CXR下拉清單
            List<SysParams> RTRecordAllList = BaseModel.getRCS_SYS_PARAMS("Shared", "");
            ViewData["ResultStrList"] = GetGroupList(RTRecordAllList, "cxr_result");
            //病人主要呼吸問題清單
            ViewData["pat_problem"] = BaseModel.getRCS_SYS_PARAMS("CPT_Assess", "pat_problem");
            //意識EVM下拉選單
            ViewData["conscious_e"] = CPTModel.getConsciousEVM("E");
            ViewData["conscious_v"] = CPTModel.getConsciousEVM("V");
            ViewData["conscious_m"] = CPTModel.getConsciousEVM("M");


            //ViewData["New_Cpt_ID"] = JsonConvert.SerializeObject(row);
            ViewData["New_Cpt_ID"] = row.cpt_id;
            ViewData["cpt_date"] = string.IsNullOrWhiteSpace(row.cpt_date) ? "" : row.cpt_date;
            //取得last_cpt_id
            ViewData["last_cpt_id"] = "";
            List<RCS_CPT_DTL_NEW_ITEMS> temp = new List<RCS_CPT_DTL_NEW_ITEMS>();
            SQLProvider SQL = new SQLProvider();
            string sql = string.Concat("SELECT CPT_ID FROM RCS_CPT_ASS_MASTER WHERE CHART_NO = @CHART_NO AND datastatus = '1' AND RECORD_DATE in(SELECT MAX(RECORD_DATE) RECORD_DATE FROM RCS_CPT_ASS_MASTER WHERE CHART_NO = @CHART_NO AND datastatus = '1' AND  ISNULL(RECORD_DATE, '') <> '')");
            DynamicParameters dp = new DynamicParameters();
            dp.Add("CHART_NO", pat_info.chart_no);
            temp = SQL.DBA.getSqlDataTable<RCS_CPT_DTL_NEW_ITEMS>(sql, dp);
            if (temp.Count > 0 && !string.IsNullOrWhiteSpace(temp[0].cpt_id))
            {
                ViewData["last_cpt_id"] = temp[0].cpt_id;
            }
            string creater_user = "";
            if (!string.IsNullOrWhiteSpace(row.cpt_id))
            {
                sql = string.Concat("SELECT CREATE_ID FROM RCS_CPT_ASS_MASTER WHERE CPT_ID = @CPT_ID AND datastatus = '1' ");
                dp = new DynamicParameters();
                dp.Add("CPT_ID", row.cpt_id);
                creater_user = SQL.DBA.getSqlDataTable<string>(sql, dp).First();
            }
            ViewBag.edit = creater_user == user_info.user_id;


            return View();
        }

        /// <summary>
        /// 取得系統設定中的下拉項目清單
        /// </summary>
        /// <param name="RTRecordAllList">List SysParams</param>
        /// <param name="group">群組分類</param>
        /// <returns>List SysParams</returns>
        public List<SysParams> GetGroupList(List<SysParams> RTRecordAllList, string group_v)
        {
            IEnumerable<SysParams> get_select_data = from r in RTRecordAllList
                                                     where r.P_GROUP == group_v
                                                     orderby int.Parse(r.P_SORT)
                                                     select r;
            return get_select_data.ToList();
        }

        /// <summary>
        /// 塞資料給CPT的Index頁面
        /// </summary>
        ///<param name="pSDate">搜尋開始日期</param>
        ///<param name="pEDate">搜尋結束日期</param>
        /// <returns></returns>
        public JsonResult getIndexTet(string pSDate, string pEDate)
        {
            RESPONSE_MSG rm = new RESPONSE_MSG();
            List<RCS_CPT_DTL_NEW_ITEMS> cpt_dtl_new_items = new List<RCS_CPT_DTL_NEW_ITEMS>();

            rm = rtAssessModel.CPTRecordList(ref cpt_dtl_new_items, pSDate, pEDate, pat_info.ipd_no);

            return Json(cpt_dtl_new_items, JsonRequestBehavior.AllowGet);
        }

        public JsonResult cpt_data_del(string CPT_ID)
        {
            string actionName = "cpt_data_del";
            RESPONSE_MSG rm = new RESPONSE_MSG();
            SQLProvider SQL = new SQLProvider();
            if (this.TempInsert(CPT_ID, true))
            {
                DynamicParameters dp = new DynamicParameters();
                dp.Add("CPT_ID", CPT_ID);
                string _sql = "UPDATE RCS_CPT_ASS_MASTER SET datastatus = '9' WHERE CPT_ID = @CPT_ID";
                SQL.DBA.DBExecute(_sql, dp);
                if (!SQL.DBA.hasLastError)
                {
                    rm.message = "刪除成功";
                    rm.status = RESPONSE_STATUS.SUCCESS;
                }
                else
                {
                    LogTool.SaveLogMessage(SQL.DBA.lastError, actionName, "BaseController");
                    rm.message = "刪除失敗";
                    rm.status = RESPONSE_STATUS.ERROR;
                }
            }
            else
            {
                LogTool.SaveLogMessage(SQL.DBA.lastError, actionName, "BaseController");
                rm.message = "刪除失敗";
                rm.status = RESPONSE_STATUS.ERROR;
            }

            return Json(rm);
        }

        /// <summary>
        /// NEW CPT評估表單-Form資料來源
        /// </summary>
        /// <remarks>取代原CPTData()功能，塞被選去的表單資料 AlanHuang 20171124</remarks>
        /// <returns></returns>
        [HttpPost]
        public string CPTindexData(string CPT_ID, string cpt_date)
        {
            CPT_ID = CPT_ID ?? "";
            List<RCS_CPT_DTL_NEW_ITEMS> cpt_dtl_new_items = new List<RCS_CPT_DTL_NEW_ITEMS>();
            try
            {

                //if (Dt != null && Dt.Rows.Count > 0)
                if (!string.IsNullOrWhiteSpace(CPT_ID))
                {

                    var getData = rtAssessModel.RTAssessData(new List<string>() { CPT_ID });

                    cpt_dtl_new_items = rtAssessModel.changeJson(getData.detailList);



                    if (!string.IsNullOrWhiteSpace(cpt_date) && cpt_dtl_new_items.Count > 0 && DateHelper.isDate(cpt_date))
                    {
                        cpt_dtl_new_items[0].cpt_date = DateHelper.Parse(cpt_date).ToString("yyyy-MM-dd HH:mm");
                    }

                }
                else if (CPT_ID == "")
                {
                    //新增介接資料
                    // ABG
                    string CLINICAL_DIAGNOSIS = "";
                    if (pat_info.source_type != "I")
                    {
                        List<IPDPatientInfo> tempPat_info = BaseModel.web_method.getPatientInfoList(this.hospFactory.webService.HISPatientInfo(), pat_info.chart_no, pat_info.ipd_no);
                        CLINICAL_DIAGNOSIS = tempPat_info.Count > 0 ? tempPat_info[0].CLINICAL_DIAGNOSIS : "";
                    } 
                    Ventilator lastRec = BaseModel.basicfunction.GetLastRTRec(pat_info.chart_no, pat_info.ipd_no);
                    #region ABG 
                    ExamViewList getVitalSignList = new ExamViewList();
                    List<JSON_DATA> abg_data = new List<JSON_DATA>();
                    //ABG
                    List<ExamABG> abgList = BaseModel.web_method.getAVBGData(this.hospFactory.webService.HISABGLabData(), pat_info.chart_no, pat_info.ipd_no);
                    if (abgList.Count > 0)
                    {
                        foreach (ExamABG item in abgList)
                        {
                            if (!string.IsNullOrWhiteSpace(lastRec.fio2_set))
                            {
                                abg_data.Add(new JSON_DATA()
                                {
                                    id = "cpt_ass_abg_12",
                                    txt = lastRec.fio2_set,
                                    val = lastRec.fio2_set
                                });
                            }

                            if (!string.IsNullOrWhiteSpace(lastRec.device))
                            {
                                abg_data.Add(new JSON_DATA()
                                {
                                    id = "cpt_ass_abg_13",
                                    txt = lastRec.device,
                                    val = lastRec.device
                                });
                            }
                            if (!string.IsNullOrWhiteSpace(item.Date))
                            {
                                abg_data.Add(new JSON_DATA()
                                {
                                    id = "cpt_ass_abg_1",
                                    txt = item.Date,
                                    val = item.Date
                                });
                            }
                            if (!string.IsNullOrWhiteSpace(item.pH))
                            {
                                abg_data.Add(new JSON_DATA()
                                {
                                    id = "cpt_ass_abg_2",
                                    txt = item.pH,
                                    val = item.pH
                                });
                            }
                            if (!string.IsNullOrWhiteSpace(item.PO2))
                            {
                                abg_data.Add(new JSON_DATA()
                                {
                                    id = "cpt_ass_abg_3",
                                    txt = item.PO2,
                                    val = item.PO2
                                });
                            }
                            if (!string.IsNullOrWhiteSpace(item.PCO2))
                            {
                                abg_data.Add(new JSON_DATA()
                                {
                                    id = "cpt_ass_abg_10",
                                    txt = item.PCO2,
                                    val = item.PCO2
                                });
                            }
                            if (!string.IsNullOrWhiteSpace(item.HCO3))
                            {
                                abg_data.Add(new JSON_DATA()
                                {
                                    id = "cpt_ass_abg_7",
                                    txt = item.HCO3,
                                    val = item.HCO3
                                });
                            }
                            if (!string.IsNullOrWhiteSpace(item.BaseExcess))
                            {
                                abg_data.Add(new JSON_DATA()
                                {
                                    id = "cpt_ass_abg_9",
                                    txt = item.BaseExcess,
                                    val = item.BaseExcess
                                });
                            }
                            if (!string.IsNullOrWhiteSpace(item.SaO2))
                            {
                                abg_data.Add(new JSON_DATA()
                                {
                                    id = "cpt_ass_abg_8",
                                    txt = item.SaO2,
                                    val = item.SaO2
                                });
                            }
                        }
                    }
                    #endregion

                    #region 體溫、脈膊、呼吸、BP
                    //體溫、脈膊、呼吸、BP
                    Dictionary<string, string> vitalSingKey = new Dictionary<string, string>();
                    List<VitalSign> sqlVitalSignList = BaseModel.web_method.getVitalSign(this.hospFactory.webService.HISVitalSign(), pat_info.chart_no, pat_info.ipd_no, pat_info.diag_date);
                    List<JSON_DATA> base_data = new List<JSON_DATA>();
                    foreach (VitalSign item in sqlVitalSignList)
                    {
                        if (item != null)
                        {
                            JSON_DATA addData = new JSON_DATA();
                            if (!string.IsNullOrWhiteSpace(item.RESULT_TEMP))
                            {
                                base_data.Add(new JSON_DATA()
                                {
                                    id = "cpt_ass_base_1",
                                    txt = item.RESULT_TEMP,
                                    val = item.RESULT_TEMP
                                });
                            }

                            if (!string.IsNullOrWhiteSpace(item.RESULT_HB))
                            {
                                base_data.Add(new JSON_DATA()
                                {
                                    id = "cpt_ass_base_2",
                                    txt = item.RESULT_HB,
                                    val = item.RESULT_HB
                                });
                            }

                            if (!string.IsNullOrWhiteSpace(item.RESULT_SBP))
                            {
                                base_data.Add(new JSON_DATA()
                                {
                                    id = "cpt_ass_base_4",
                                    txt = item.RESULT_SBP,
                                    val = item.RESULT_SBP
                                });
                            }

                            if (!string.IsNullOrWhiteSpace(item.RESULT_DBP))
                            {
                                base_data.Add(new JSON_DATA()
                                {
                                    id = "cpt_ass_base_5",
                                    txt = item.RESULT_DBP,
                                    val = item.RESULT_DBP
                                });
                            }
                            break;
                        }
                    }

                    #endregion

                    cpt_dtl_new_items.Add(new RCS_CPT_DTL_NEW_ITEMS()
                    {
                        diag_date = pat_info.diag_date,
                        base_data = base_data,
                        abg_data = abg_data,
                        now_pat_diagnosis = pat_info.diagnosis_code,
                        diagnosis = CLINICAL_DIAGNOSIS
                    });

                }

            }
            catch (Exception ex)
            {
                LogTool.SaveLogMessage(ex, "CPTindexData");
            }
            return JsonConvert.SerializeObject(cpt_dtl_new_items);
        }

        /// <summary> 呼吸治療評估記錄單 - 首頁 </summary>
        /// <param name="cpt_id"></param>
        /// <param name="chart_no"></param>
        /// <param name="ipd_no"></param>
        /// <returns></returns>
        public ActionResult cpt_page(string cpt_id, string chart_no = "", string ipd_no = "")
        {
            CPTAssessViewModel ViewModel = new CPTAssessViewModel();
            try
            { 
                var getData = rtAssessModel.RTAssessData(new List<string>() { cpt_id });

                // master
                ViewModel.master = getData.masterList.FirstOrDefault();

                // detail
                List<RCS_CPT_DTL_NEW_ITEMS> data = getData.detailList;
                data = rtAssessModel.changeJson(data);

                if (data.Count > 0)
                {
                    ViewModel.cpt_data = data[0];
                }
                else
                {
                    ViewModel.cpt_data = new RCS_CPT_DTL_NEW_ITEMS();

                }

                BaseModels baseModels = new BaseModels();
                IPDPatientInfo patientInfo = baseModels.SelectPatientInfo(ViewModel.master.IPD_NO, ViewModel.master.CHART_NO);

                ViewModel.pat_info = patientInfo;
                ViewModel.user_info = user_info;
            }
            catch (Exception ex)
            {
                LogTool.SaveLogMessage(ex, "cpt_page");
            } 
            return View("~/Views/PDFHtml/CPTPageForm.cshtml", ViewModel);
        }


        //DownloadPdf_CPTPage

        /// <summary>
        /// 下載PDF檔案
        /// </summary>
        /// <param name="data">預備列印之HTML資料包</param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult DownloadPdf_CPTPage(pdfModel data)
        {
            exportFile efm = new exportFile("DownloadPdf_CPTPage.pdf");
            IDocSetting ds = new CPTPageFormPDFDocSetting();
            return efm.exportPDF(data.HtmlStrCXR2, ds);
        }

        /// <summary> CPT 評估表 - 儲存 </summary>
        [HttpPost]
        public JsonResult CPTAssess_Save(RCS_CPT_DTL_NEW_ITEMS rdta)
        {
            RESPONSE_MSG rm = new RESPONSE_MSG();
            RESPONSE_MSG reMSG = new RESPONSE_MSG(); //沒用到待刪除?? 2018.08.25
            string ExecFlag = string.Empty, cpt_id = rdta.cpt_id ?? "";
            try
            {
                SQLProvider SQL = new SQLProvider();
                //寫入CPT 評估表數據回寫院內
                string nowDate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                rm.status = RESPONSE_STATUS.SUCCESS;

                // 主檔儲存
                if (rm.status == RESPONSE_STATUS.SUCCESS)
                {
                    DataTable dt;
                    string[] DtlArray = CPTModel.getCPTAssessDetailColumns();
                    DataTable DelDt = new DataTable();
                    DataTable InsDt = CPTModel.getCPTAssessDetail("WHERE 1<>1");

                    if (string.IsNullOrWhiteSpace(cpt_id))
                    {
                        dt = CPTModel.getCPTAssessMaster("WHERE 1<>1");
                        cpt_id = SQL.GetFixedStrSerialNumber();
                        DataRow dr = dt.NewRow();
                        dr["IPD_NO"] = pat_info.ipd_no;
                        dr["CPT_ID"] = cpt_id;
                        dr["CPT_STATUS"] = "Y";
                        dr["CREATE_ID"] = user_info.user_id;
                        dr["CREATE_NAME"] = user_info.user_name;
                        dr["CREATE_DATE"] = nowDate;
                        dr["CHART_NO"] = pat_info.chart_no;
                        dr["BED_NO"] = pat_info.bed_no;
                        dr["ADMISSION_DATE"] = pat_info.diag_date;
                        dr["DATASTATUS"] = "1";
                        dr["UPLOAD_STATUS"] = "0";
                        dr["RECORD_DATE"] = rdta.record_date;
                        dt.Rows.Add(dr);
                    }
                    else
                    {
                        //判斷是否已經上傳
                        DelDt = CPTModel.getCPTAssessDetail(string.Format("WHERE CPT_ID = {0}", SQLDefend.SQLString(cpt_id)));
                        dt = CPTModel.getCPTAssessMaster(string.Format("WHERE CPT_ID = {0}", SQLDefend.SQLString(cpt_id)));
                        DataRow dr = dt.Rows[0];
                        dr["MODIFY_ID"] = user_info.user_id;
                        dr["MODIFY_NAME"] = user_info.user_name;
                        dr["MODIFY_DATE"] = nowDate;
                        dr["RECORD_DATE"] = string.IsNullOrEmpty(rdta.record_date) ? "" : rdta.record_date;
                    }

                    this.DBA.BeginTrans();

                    mayaminer.com.jxDB.dbResultMessage rc = this.DBA.UpdateResult(dt, GetTableName.RCS_CPT_ASS_MASTER.ToString());
                    if (rc.State != mayaminer.com.jxDB.enmDBResultState.Success)
                    {
                        LogTool.SaveLogMessage(this.DBA.LastError, "CPTAssess_Save");
                        rm.message = "儲存失敗!";
                        rm.status = RESPONSE_STATUS.ERROR;
                    }
                    else
                    {
                        rm.status = RESPONSE_STATUS.SUCCESS;
                        // 明細儲存 (先清空在儲存)
                        foreach (DataRow DelDr in DelDt.Rows)
                            DelDr.Delete();

                        rc = this.DBA.UpdateResult(DelDt, GetTableName.RCS_CPT_ASS_DETAIL.ToString());
                        if (rc.State == mayaminer.com.jxDB.enmDBResultState.Success)
                        {
                            if (rdta != null)
                            {
                                string[] CPTColArray = {
                                    "VPN_mechanism",
                                    "diag_date",
                                    "record_date"
                                    , "from_unit"
                                    , "cpt_history"
                                    , "smoke_history"
                                    , "now_pat_diagnosis"
                                    , "diagnosis"
                                    , "history_diag"
                                    , "other_history"
                                    , "rt_reason"
                                    , "brief_status"
                                    , "operation"
                                    , "operation_memo"
                                    , "hocus"
                                    , "abg_data"
                                    , "lung"
                                    , "lung_conclusion"
                                    , "base_data"
                                    , "conscious"
                                    , "tip"
                                    , "skin"
                                    , "tube"
                                    , "machine"
                                    , "patterns"
                                    , "atelectasis"
                                    , "breath_sound"
                                    , "cough"
                                    , "sputum"
                                    , "sputum_assess"
                                    , "pat_problem"
                                    , "cpt_memo"
                                    , "thorax" };
                                string[] CPTValArray = {
                                    string.IsNullOrEmpty(rdta.VPN_mechanism) ? "" : rdta.VPN_mechanism,
                                    string.IsNullOrEmpty(rdta.diag_date) ? "" : rdta.diag_date,
                                    rdta.record_date
                                    , JsonConvert.SerializeObject(rdta.from_unit)
                                    , JsonConvert.SerializeObject(rdta.cpt_history)
                                    , JsonConvert.SerializeObject(rdta.smoke_history)
                                    , rdta.now_pat_diagnosis
                                    , rdta.diagnosis
                                    , rdta.history_diag
                                    , rdta.other_history
                                    , JsonConvert.SerializeObject(rdta.rt_reason)
                                    , JsonConvert.SerializeObject(rdta.brief_status)
                                    , JsonConvert.SerializeObject(rdta.operation)
                                    , rdta.operation_memo
                                    , JsonConvert.SerializeObject(rdta.hocus)
                                    , JsonConvert.SerializeObject(rdta.abg_data)
                                    , JsonConvert.SerializeObject(rdta.lung)
                                    , rdta.lung_conclusion
                                    , JsonConvert.SerializeObject(rdta.base_data)
                                    , JsonConvert.SerializeObject(rdta.conscious)
                                    , JsonConvert.SerializeObject(rdta.tip)
                                    , JsonConvert.SerializeObject(rdta.skin)
                                    , JsonConvert.SerializeObject(rdta.tube)
                                    , JsonConvert.SerializeObject(rdta.machine)
                                    , JsonConvert.SerializeObject(rdta.patterns)
                                    , JsonConvert.SerializeObject(rdta.atelectasis)
                                    , JsonConvert.SerializeObject(rdta.breath_sound)
                                    , JsonConvert.SerializeObject(rdta.cough)
                                    , JsonConvert.SerializeObject(rdta.sputum)
                                    , JsonConvert.SerializeObject(rdta.sputum_assess)
                                    , JsonConvert.SerializeObject(rdta.pat_problem)
                                    , rdta.cpt_memo
                                    , JsonConvert.SerializeObject(rdta.thorax)};
                                this.SetInsTable(ref InsDt, cpt_id, CPTColArray, CPTValArray);

                                Dictionary<string, object> FormList = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(rdta.obj);
                                foreach (var CPT in FormList)
                                {
                                    if (CPT.Value == null || CPT.Value.ToString() == "")
                                        continue;

                                    string cpt_value = CPT.Value.ToString();
                                    switch (CPT.Key)
                                    {
                                        case "CXR_result_json":
                                            DataRow InsDr = InsDt.NewRow();
                                            InsDr["CPT_ID"] = cpt_id;
                                            InsDr["CPT_ITEM"] = "CXR_result_json";
                                            InsDr["CPT_VALUE"] = cpt_value;
                                            InsDt.Rows.Add(InsDr);
                                            break;
                                    }
                                }
                                rc = this.DBA.UpdateResult(InsDt, GetTableName.RCS_CPT_ASS_DETAIL.ToString());
                                if (rc.State != mayaminer.com.jxDB.enmDBResultState.Success)
                                {
                                    rm.status = RESPONSE_STATUS.ERROR;
                                    rm.message = "上傳系統發生例外，請洽資訊人員!";
                                }
                            }
                            if (rm.status == RESPONSE_STATUS.SUCCESS)
                                rm.message = "儲存成功";
                        }
                        else
                        {
                            LogTool.SaveLogMessage(this.DBA.LastError, "CPTAssess_Save");
                            rm.status = RESPONSE_STATUS.ERROR;
                            rm.message = "儲存失敗!";
                        }
                    }
                }
                else
                {
                    if (reMSG.message == null || reMSG.message == "")
                    {
                        rm.message = "上傳系統發生例外，請洽資訊人員!";
                    }
                    else
                    {
                        rm.message = reMSG.message;
                    }
                    rm.status = RESPONSE_STATUS.ERROR;
                }
            }
            catch (Exception ex)
            {
                string tmp = ex.Message;
                LogTool.SaveLogMessage(tmp, "CPTAssess_Save");
                rm.status = RESPONSE_STATUS.EXCEPTION;
                rm.message = "系統發生例外，請聯繫資訊人員";
            }
            finally
            {
                if (rm.status == RESPONSE_STATUS.SUCCESS)
                {
                    this.DBA.Commit();
                    this.TempInsert(cpt_id, false);
                }
                else
                    this.DBA.Rollback();
            }
            return Json(rm);
        }


        /// <summary>
        /// 暫存新增
        /// </summary>
        /// <param name="pRECORD_ID"></param>
        /// <param name="isDel"></param>
        /// <returns></returns>
        private bool TempInsert(string pRECORD_ID, bool isDel)
        {
            SQLProvider SQL = new SQLProvider();
            DynamicParameters dp = new DynamicParameters();
            string query = string.Concat("SELECT * FROM ", DB_TABLE_NAME.DB_RCS_CPT_ASS_MASTER, " WHERE CPT_ID = @CPT_ID;",
                "SELECT CPT_ID as RECORD_ID,CPT_ITEM as ITEM_NAME,CPT_VALUE as ITEM_VALUE,* FROM ", DB_TABLE_NAME.DB_RCS_CPT_ASS_DETAIL, " WHERE CPT_ID = @CPT_ID;");
            dp.Add("CPT_ID", pRECORD_ID);
            if (isDel)
            {
                return SQL.DELTableData(pRECORD_ID, DB_TABLE_NAME.DB_RCS_CPT_ASS_MASTER, user_info, query, dp);
            }
            return SQL.EditTableData(pRECORD_ID, DB_TABLE_NAME.DB_RCS_CPT_ASS_MASTER, user_info, query, dp);
        }


        #endregion

        #region 整理完成

        /// <summary> CPT評估主表-資料來源 </summary>
        /// <remarks>Joe Shen 2016-5-10(解決原先換行問題)
        /// janchiu 20191122 (列印使用)
        /// </remarks>
        /// <returns></returns>
        [HttpPost]
        public string CPTData(string pCpdId)
        {
            pCpdId = pCpdId ?? "";
            List<RCS_CPT_DTL_NEW_ITEMS> cpt_dtl_new_items = new List<RCS_CPT_DTL_NEW_ITEMS>();
            try
            {
                cpt_dtl_new_items = this.CPTDataDTL(pCpdId);

                List<IPDPatientInfo> patInfo = new WebMethod().getPatientInfoList(this.hospFactory.webService.HISPatientInfo(), pat_info.chart_no, pat_info.ipd_no);
                if (patInfo.Count > 0)
                {
                    IPDPatientInfo _patInfo = patInfo[0];
                    if (!string.IsNullOrWhiteSpace(pCpdId))
                        cpt_dtl_new_items.Add(new RCS_CPT_DTL_NEW_ITEMS());
                    if (cpt_dtl_new_items.Count > 0)
                    {
                        if (cpt_dtl_new_items[0].smoke_history == null) cpt_dtl_new_items[0].smoke_history = new List<JSON_DATA>();
                    }
                }

            }
            catch (Exception ex)
            {
                LogTool.SaveLogMessage(ex, "CPTData");
            }
            return JsonConvert.SerializeObject(cpt_dtl_new_items);
        }

        /// <summary> CPT評估主表-資料來源 </summary>
        /// <remarks>Joe Shen 2016-5-10(解決原先換行問題)</remarks>
        /// <returns></returns>
        [HttpPost]
        public List<RCS_CPT_DTL_NEW_ITEMS> CPTDataDTL(string pCpdId)
        {
            pCpdId = pCpdId ?? "";
            List<RCS_CPT_DTL_NEW_ITEMS> cpt_dtl_new_items = new List<RCS_CPT_DTL_NEW_ITEMS>();
            try
            {
                DataTable Dt = CPTModel.getCPTAssessDetailNewItems(pCpdId);
                //SQL [Save儲存]
                //參考: rc = CxrCanvasLineDrawMdl.saveSqlCxrTable(rdta.obj, "CXR_result_json", "Cxr_Result_XYwmc"); 
                if (Dt != null && Dt.Rows.Count > 0)
                {
                    // [Read讀取] 需確認 [Dt] 資料來源 2018.07.20
                    // Table資料表格 [CXR_result_json] 欄位有 [Cxr_CJID] 流水號
                    // 輸出 [Cxr_Result_XYwmc] 新增欄位
                    Dt = CxrCanvasLineDrawMdl.getCxrResultTable_byDataTable(Dt, GetTableName.RCS_CPT_ASS_DETAIL.ToString());

                    foreach (DataRow Dr in Dt.Rows)
                    {
                        cpt_dtl_new_items.Add(new RCS_CPT_DTL_NEW_ITEMS()
                        {
                            cpt_id = Dr["cpt_id"].ToString().Trim(),
                            from_unit = JsonConvert.DeserializeObject<List<JSON_DATA>>(Dr["from_unit"].ToString().Trim()),
                            cpt_history = JsonConvert.DeserializeObject<List<JSON_DATA>>(Dr["cpt_history"].ToString().Trim()),
                            smoke_history = JsonConvert.DeserializeObject<List<JSON_DATA>>(Dr["smoke_history"].ToString().Trim()),
                            diagnosis = Dr["diagnosis"].ToString().Trim(),
                            history_diag = Dr["history_diag"].ToString().Trim(),
                            now_pat_diagnosis = Dr["now_pat_diagnosis"].ToString().Trim(),
                            other_history = Dr["other_history"].ToString().Trim(),
                            rt_reason = JsonConvert.DeserializeObject<List<JSON_DATA>>(Dr["rt_reason"].ToString().Trim()),
                            brief_status = JsonConvert.DeserializeObject<List<JSON_DATA>>(Dr["brief_status"].ToString().Trim()),
                            operation = JsonConvert.DeserializeObject<List<JSON_DATA>>(Dr["operation"].ToString().Trim()),
                            operation_memo = Dr["operation_memo"].ToString().Trim(),
                            hocus = JsonConvert.DeserializeObject<List<JSON_DATA>>(Dr["hocus"].ToString().Trim()),
                            abg_data = JsonConvert.DeserializeObject<List<JSON_DATA>>(Dr["abg_data"].ToString().Trim()),
                            lung = JsonConvert.DeserializeObject<List<JSON_DATA>>(Dr["lung"].ToString().Trim()),
                            lung_conclusion = Dr["lung_conclusion"].ToString().Trim(),
                            base_data = JsonConvert.DeserializeObject<List<JSON_DATA>>(Dr["base_data"].ToString().Trim()),
                            conscious = JsonConvert.DeserializeObject<List<JSON_DATA>>(Dr["conscious"].ToString().Trim()),
                            tip = JsonConvert.DeserializeObject<List<JSON_DATA>>(Dr["tip"].ToString().Trim()),
                            skin = JsonConvert.DeserializeObject<List<JSON_DATA>>(Dr["skin"].ToString().Trim()),
                            tube = JsonConvert.DeserializeObject<List<JSON_DATA>>(Dr["tube"].ToString().Trim()),
                            machine = JsonConvert.DeserializeObject<List<JSON_DATA>>(Dr["machine"].ToString().Trim()),
                            patterns = JsonConvert.DeserializeObject<List<JSON_DATA>>(Dr["patterns"].ToString().Trim()),
                            atelectasis = JsonConvert.DeserializeObject<List<JSON_DATA>>(Dr["atelectasis"].ToString().Trim()),
                            breath_sound = JsonConvert.DeserializeObject<List<JSON_DATA>>(Dr["breath_sound"].ToString().Trim()),
                            cough = JsonConvert.DeserializeObject<List<JSON_DATA>>(Dr["cough"].ToString().Trim()),
                            sputum = JsonConvert.DeserializeObject<List<JSON_DATA>>(Dr["sputum"].ToString().Trim()),
                            sputum_assess = JsonConvert.DeserializeObject<List<JSON_DATA>>(Dr["sputum_assess"].ToString().Trim()),
                            DATASTATUS = Dr["datastatus"].ToString().Trim(),
                            UPLOAD_STATUS = Dr["upload_status"].ToString().Trim(),
                            pat_problem = JsonConvert.DeserializeObject<List<JSON_DATA>>(Dr["pat_problem"].ToString().Trim()),
                            /*----------------------------- [Read讀取] Cxr畫線功能 [下]-----------------------------*/
                            CXR_result_json = JsonConvert.DeserializeObject<List<CxrResultJson_cls>>(Dr["CXR_result_json"].ToString().Trim()),
                            /*----------------------------- [Read讀取] Cxr畫線功能 [上]-----------------------------*/
                            cpt_memo = DBNull.ReferenceEquals(Dr["cpt_memo"], DBNull.Value) ? "" : Dr["cpt_memo"].ToString().Trim(),
                            thorax = JsonConvert.DeserializeObject<List<JSON_DATA>>(Dr["thorax"].ToString().Trim()),
                        });

                    }
                }

            }
            catch (Exception ex)
            {
                LogTool.SaveLogMessage(ex, "CPTData");
            }
            return cpt_dtl_new_items;
        }


        /// <summary> 設定儲存CPT評估明細表內容 </summary>
        /// <param name="pDt">明細表Table</param>
        /// <param name="pId">評估表編號</param>
        /// <param name="pCols">評估表明細檔ITEM名稱</param>
        /// <param name="pVals">評估表明細檔ITEM值</param>
        /// <returns></returns>
        private void SetInsTable(ref DataTable pDt, string pId, string[] pCols, string[] pVals)
        {
            for (int u = 0; u < pCols.Length; u++)
            {
                DataRow InsDr = pDt.NewRow();
                InsDr["CPT_ID"] = pId;
                InsDr["CPT_ITEM"] = pCols[u];
                InsDr["CPT_VALUE"] = pVals[u];
                pDt.Rows.Add(InsDr);
                InsDr = null;
            }
        }

        /// <summary> 更新HIS手術資料選取畫面 </summary>
        /// <param name="pCptId">CPT評估單編號</param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult CPTAssessOP(string pCptId)
        {
            //找到該筆CPT_ID對應之IPD_NO及CHART_NO
            string tmp_ipd_no = pat_info.ipd_no;
            string tmp_chart_no = pat_info.chart_no;
            if (pCptId != null && pCptId != "")
            {
                DataTable ptDt = CPTModel.getCPTAssessIpdNoChartNo(pCptId);
                if (ptDt != null && ptDt.Rows.Count > 0)
                {
                    tmp_ipd_no = ptDt.Rows[0]["IPD_NO"].ToString().Trim();
                    tmp_chart_no = ptDt.Rows[0]["CHART_NO"].ToString().Trim();
                }
            }
            List<PatOperation> ORList = BaseModel.web_method.getPatOperationList(this.hospFactory.webService.HISPatOperationList(), tmp_chart_no, tmp_ipd_no);

            ViewData["ORList"] = ORList;
            return PartialView("CPTAssessOP");
        }
        #endregion

    }
}

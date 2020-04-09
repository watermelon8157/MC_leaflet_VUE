using Com.Mayaminer;
using Newtonsoft.Json;
using RCS_Data;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Web.Mvc;
using iTextSharp.text.pdf;
using iTextSharp.text;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using iTextSharp.tool.xml;
using System.Text.RegularExpressions;
using RCS.Models;
using System.Data;
using System.Web;
using System.Reflection;
using mayaminer.com.library;
using Newtonsoft.Json.Linq;
using mayaminer.com.jxDB;
using RCSData.Models;
using RCS_Data.Models.ViewModels;
using RCS_Data.Models.VIP;
using RCS.Models.ViewModel;
using RCS_Data.Controllers.RtRecord;
using RCS_Data.Models.DB;
using RCS_Data.Controllers;

namespace RCS.Controllers {
    public class RTRecordController : BaseController
    {

        public static List<SelectListItem> qc_item
        {
            get
            {
                return new List<SelectListItem>() {
                    new SelectListItem() { Text = "成功", Value = "1" },
                    new SelectListItem() { Text = "死亡", Value = "2" },
                    new SelectListItem() { Text = "AAD", Value = "3" },
                    new SelectListItem() { Text = "轉院", Value = "4" },
                    new SelectListItem() { Text = "自拔", Value = "5" },
                    new SelectListItem() { Text = "重插管(>5天)", Value = "6" },
                    new SelectListItem() { Text = "重插管(>24H)", Value = "6A" },
                    new SelectListItem() { Text = "重插管(>48H)", Value = "6B" },
                    new SelectListItem() { Text = "重插管(>72H)", Value = "6C" },
                    new SelectListItem() { Text = "轉病房", Value = "7" },
                    new SelectListItem() { Text = "Home Care", Value = "8" },
                    new SelectListItem() { Text = "轉ICU", Value = "9" },
                    new SelectListItem() { Text = "氣切脫離", Value = "10" },
                    new SelectListItem() { Text = "氣切脫離失敗", Value = "11" },
                    new SelectListItem() { Text = "on ET", Value = "12" },
                    new SelectListItem() { Text = "on BIPAP", Value = "13" }
                };
            }
        }


        string csName { get { return "RTRecordController"; } }

        RtRecord _model;
        RtRecord rtRecordModel
        {
            get
            {
                if (_model == null)
                {
                    this._model = new RtRecord();
                }
                return this._model;
            }
        }

        /// <summary>RTRecordModel</summary>
        private RTRecord RTRecordModel = new RTRecord();
        private HisDataModel HisDataModel = new HisDataModel();
        private CxrCanvasLineDraw CxrCanvasLineDrawMdl; //2018.08.17 CXR畫線

        #region 記錄單清單 List
        /// <summary>呼吸照護記錄單清單</summary>
        /// <returns></returns>
        public ActionResult List()
        {
            List<SelectListItem> getPatientHistoryList = BaseModel.getPatientHistoryList(pat_info.chart_no, pat_info.ipd_no);
            ViewData["getPatientHistoryList"] = getPatientHistoryList;
             
            if (getPatientHistoryList.Exists(x => x.Selected))
            {
                SelectListItem item = new SelectListItem();
                item = getPatientHistoryList.Find(x => x.Selected);
                string _sDate = BaseModel.getHistoryList(item.Value, 1);
                string _eDate = BaseModel.getHistoryList(item.Value, 2);
                ViewData["sDate"] = _sDate;
                ViewData["eDate"] = _eDate == _sDate ? DateTime.Parse(_eDate).ToString("yyyy-MM-dd 23:59") : _eDate;
            }
            if (Session["searchDate"] != null)
            {
                string[] date = Session["searchDate"].ToString().Split('|');
                if (date.Length > 1)
                {
                    ViewData["sDate"] = date[0];
                    ViewData["eDate"] = date[1];
                }
                Session.Remove("searchDate");
            }
            return View();
        }
        /// <summary>取得呼吸照護清單</summary> 
        /// <param name="StartDate">開始時間</param> 
        /// <param name="EndDate">結束時間</param> 
        /// <param name="EndDate">結束時間</param>
        /// <returns></returns>
        public string RecordListData(string StartDate, string EndDate, string setIpdno, string record_id, bool getOnModel = false)
        {
            List<RT_RECORD_MAIN> rt_record_main_list = new List<RT_RECORD_MAIN>();
            try
            {
                StartDate = (string.IsNullOrWhiteSpace(StartDate)) ? DateTime.Now.ToString("yyyy-MM-dd 00:00:00") : Convert.ToDateTime(StartDate).ToString("yyyy-MM-dd HH:mm:ss");
                EndDate = (string.IsNullOrWhiteSpace(EndDate)) ? DateTime.Now.ToString("yyyy-MM-dd 23:59:59") : Convert.ToDateTime(EndDate).ToString("yyyy-MM-dd HH:mm:ss");
                rt_record_main_list = RTRecordModel.getDuringRTRecord(StartDate, EndDate, setIpdno, record_id, getOnModel);

                foreach (RT_RECORD_MAIN item in rt_record_main_list)
                {
                    if (!String.IsNullOrWhiteSpace(item.rt_record.is_humidifier) && item.rt_record.is_humidifier == "1")
                    {
                        item.rt_record._is_humidifier = item.rt_record.device ;
                        //item.rt_record._is_humidifier = item.rt_record.device + "\\H";
                        //row.rt_record.device = row.rt_record._is_humidifier;
                    }//if
                    else
                    {
                        item.rt_record._is_humidifier = item.rt_record.device;
                    }//else
                    List<string> on_dateLIst = new List<string>();
                    item.rt_record.use_days_how = BaseModel.getUseDays(pat_info.ipd_no, pat_info.chart_no, out on_dateLIst).ToString();
                }
            }
            catch (Exception ex)
            {
                LogTool.SaveLogMessage(ex, "RecordListData", GetLogToolCS.RTRecordController);
            }
            finally
            {
                rt_record_main_list = (List<RT_RECORD_MAIN>)BaseModel.set_modify_power(rt_record_main_list, "RT_RECORD_MAIN");
                
            }
            return JsonConvert.SerializeObject(rt_record_main_list);
        }
        #endregion

        #region 記錄單畫面 RTRecordForm
 
        /// <summary>
        /// 開啟呼吸照護記錄單畫面
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public ActionResult openRTRecordFormView(string rowStr)
        {
            // Views\RTRecord\List.cshtml
            string actionName = "openRTRecordFormView";
            RT_RECORD_MAIN data = new RT_RECORD_MAIN();
            try
            {
                rowStr = HttpUtility.UrlDecode(rowStr);
                data = JsonConvert.DeserializeObject<RT_RECORD_MAIN>(rowStr);
                data.rt_record._is_humidifier = "";//20181106避免特定字元造成錯誤，所以先清空
                if (pat_info != null && !string.IsNullOrWhiteSpace(data.chart_no) && pat_info.chart_no != data.chart_no)
                {
                    IPDPatientInfo tempPat_info = (IPDPatientInfo)SetSession(data.chart_no, "C").Data;
                    if (tempPat_info == null || tempPat_info.chart_no != data.chart_no)
                        ViewData["errorMsg"] = "開啟記錄單時，程式發生錯誤，請洽資訊人員";
                }
                SysParamCollection sys_params_list = new SysParamCollection();

                //VIP資料，檢查財產編號是否存在，自動新增呼吸器維護資料
                if (data.isVIP)
                {
                    if (BaseController.isDebuggerMode || BaseController.isBasicMode)
                    {
                        if (string.IsNullOrWhiteSpace(data.rt_record.device))
                        {
                            data.rt_record.respid = "000157";
                            data.rt_record.device = "G5";
                        }
                    }
                    //沒有輸入財產編號 無法動新增
                    new VIPRTTBL().checkDEVICE_NO(data.rt_record.respid, data.rt_record.device, user_info);
                }

                sys_params_list.append_modal(BaseModel.GetModelListCollection("Index_device"));
                sys_params_list.append_modal(BaseModel.GetModelListCollection("RTRecord_Detail"));
                sys_params_list.append_modal(BaseModel.GetModelListCollection("Shared"));
                ViewData["sys_params_list"] = sys_params_list;
                ViewData["VIPtag"] = data.isVIP; 
                data.rt_record.memo = BaseModel.trans_special_code(data.rt_record.memo);
                data.rt_record.drug_memo = BaseModel.trans_special_code(data.rt_record.drug_memo);
                ViewData["RECORDData"] = JsonConvert.SerializeObject(data);
                ViewData["pat_info"] = pat_info;

                // 院內所有呼吸器編號
                RESP_COLLECTION resplist = RTRecordModel.getVENTILATORList("", "Y");
                string tempStr = string.Format("{0}-{1}", data.rt_record.device, data.rt_record.respid);
                if (!resplist.Exists(x => x.DEVICE_SEQ == tempStr))
                {
                    DeviceMaster item = new DeviceMaster();
                    item.DEVICE_SEQ = tempStr;
                    item.DEVICE_NO = data.rt_record.device;
                    item.DEVICE_MODEL = data.rt_record.mode;
                    item.USE_STATUS = "N";
                    resplist.Add(item);
                }
                ViewData["resp_list"] = resplist;

                data.hasRWC = new MODEL_RCS_WEANING_ASSESS_CHECKLIST().hasEdithasTodayRWC(pat_info.chart_no, pat_info.ipd_no);


            }
            catch (Exception ex)
            {
                LogTool.SaveLogMessage(ex, actionName, GetLogToolCS.RTRecordController);
            }
            if (!string.IsNullOrWhiteSpace(data.rt_record.work_data))
            {
                data.rt_record.work_data = "";
            }
            return View("RTRecordForm", data);
        }

        #region 新增修改刪除
        /// <summary>取得單筆呼吸照護記錄資料</summary>
        /// <param name="row">資料</param>
        /// <returns></returns>
        public JsonResult getThisRecordData(string rowStr,string setIpdno, string target)
        {
            RT_RECORD_MAIN row = new RT_RECORD_MAIN();
            RT_RECORD_MAIN last = new RT_RECORD_MAIN();
            try
            {
                rowStr = HttpUtility.UrlDecode(rowStr);
                row = JsonConvert.DeserializeObject<RT_RECORD_MAIN>(rowStr);

                //如果是帶入最後一筆資料的時候，產生比較判斷的物件
                if (!string.IsNullOrWhiteSpace(target) && target == "get_last")
                {
                    last = JsonConvert.DeserializeObject<RT_RECORD_MAIN>(rowStr);
                }

                if (row.isVIP)
                {
                    setIpdno = pat_info.ipd_no;
                }
                else
                {
                    setIpdno = RTRecordModel.getHistoryList(setIpdno, 0);
                }
                RTRecordModel.getDuringRTRecord(ref row, setIpdno);
                #region 回傳前判斷
                 

                //檢查是否有秒數
                if (!string.IsNullOrWhiteSpace(row.rt_record.recordtime))
                    row.rt_record.recordtime = DateHelper.isDate(row.rt_record.recordtime, "HH:mm:ss") ? DateHelper.Parse(row.rt_record.recordtime, "HH:mm:ss").ToString("HH:mm") : row.rt_record.recordtime;
                
                //設定儀器顯示開關
                if (row.onBreath._isShow)row.onBreath.isShow = new List<JSON_DATA>() { new JSON_DATA() { id = "isShow", chkd = true, val = "1" } };
                if (row.onIntubate._isShow)row.onIntubate.isShow = new List<JSON_DATA>() { new JSON_DATA() { id = "isShow", chkd = true, val = "1" } };
                if (row.onOxygen._isShow)row.onOxygen.isShow = new List<JSON_DATA>() { new JSON_DATA() { id = "isShow", chkd = true, val = "1" } };

                List<RT_RECORD_MAIN> tempList = new List<RT_RECORD_MAIN>();
                tempList.Add(row);
                tempList = (List<RT_RECORD_MAIN>)BaseModel.set_modify_power(tempList, "RT_RECORD_MAIN");
                if (tempList.Count > 0) row = tempList[0];
                if (!string.IsNullOrWhiteSpace(target) && target == "get_last")
                {
                    row.hasPowerEdit = true;
                }
                #endregion
            }
            catch (Exception ex)
            {
                row.isWsError = true;
                row.wsError = "程式錯誤，請洽資訊人員。錯誤訊息如下所示:" + ex.Message;
                LogTool.SaveLogMessage(ex, "getThisRecordData", GetLogToolCS.RTRecordController);
            }



            return Json(row);
        }

        /// <summary>檢核資料</summary>
        /// <param name="pModel"></param>
        /// <returns></returns>
        private RESPONSE_MSG SaveCheck(RT_RECORD_MAIN pModel)
        {
            RESPONSE_MSG rm = new RESPONSE_MSG();
            try
            {
                rm.status = RESPONSE_STATUS.ERROR;
                rm.message = "";
                List<string> errlist = new List<string>();
                if (pModel != null)
                {
 
                    //主表
                    if (string.IsNullOrWhiteSpace(pModel.rt_record.recorddate) || string.IsNullOrWhiteSpace(pModel.rt_record.recordtime))
                    {
                        errlist.Add("請輸入記錄時間");
                    }
                    else
                    {
                        string datetime = pModel.rt_record.recorddate + " " + pModel.rt_record.recordtime;

                        if (!DateHelper.isDate(datetime, "yyyy-MM-dd HH:mm"))
                        {
                            errlist.Add("記錄時間格式錯誤(yyyy-mm-dd hh:mm)");
                        }
                        
                        if (!string.IsNullOrWhiteSpace(pModel.onBreath.STARTDATE) && string.IsNullOrWhiteSpace(pModel.rt_record.respid))
                        {
                            errlist.Add("請選擇呼吸器編號");
                        }
                        if (!string.IsNullOrWhiteSpace(pModel.onOxygen.STARTDATE) && string.IsNullOrWhiteSpace(pModel.rt_record.device_o2))
                        {
                            errlist.Add("請選擇O2 equipment");
                        }
                    }
                }

                //如果有氧氣治療設備
                #region 檢查氧氣是否需要結束
                if (pModel.onOxygen != null && !string.IsNullOrWhiteSpace(pModel.onOxygen.STARTDATE)
                && !string.IsNullOrWhiteSpace(pModel.onOxygen.ONMODE_ID)
                && pModel.onOxygen.STARTDATE != string.Concat(pModel.rt_record.recorddate, " ", pModel.rt_record.recordtime)
                )
                {
                    //檢查以下數值是否有資料
                    //如果有資料要結束氧氣治療
                    List<string> _colList = new List<string>();
                    if (!string.IsNullOrWhiteSpace(pModel.rt_record.vr_set)) _colList.Add("Ventilation rate set");
                    if (!string.IsNullOrWhiteSpace(pModel.rt_record.simv_rate)) _colList.Add("SIMV");
                    if (!string.IsNullOrWhiteSpace(pModel.rt_record.mv_set)) _colList.Add("MV");
                    if (!string.IsNullOrWhiteSpace(pModel.rt_record.mv_percent)) _colList.Add("MV%");
                    if (!string.IsNullOrWhiteSpace(pModel.rt_record.vt_set)) _colList.Add("TV");
                    if (!string.IsNullOrWhiteSpace(pModel.rt_record.pressure_pc)) _colList.Add("Insp pr.");
                    if (!string.IsNullOrWhiteSpace(pModel.rt_record.pressure_ps)) _colList.Add("Ps");
                    if (!string.IsNullOrWhiteSpace(pModel.rt_record.pressure_peep)) _colList.Add("PEEP");
                    if (!string.IsNullOrWhiteSpace(pModel.rt_record.ti)) _colList.Add("I-Time% ");
                    if (!string.IsNullOrWhiteSpace(pModel.rt_record.insp_time)) _colList.Add("Ti");
                    if (!string.IsNullOrWhiteSpace(pModel.rt_record.pause_time)) _colList.Add("Pause Time%");
                    if (!string.IsNullOrWhiteSpace(pModel.rt_record.ramp)) _colList.Add("Ramp");
                    if (!string.IsNullOrWhiteSpace(pModel.rt_record.rise_time)) _colList.Add("Rise time");
                    if (!string.IsNullOrWhiteSpace(pModel.rt_record.ie_ratio)) _colList.Add("I/E Ratio");
                    if (!string.IsNullOrWhiteSpace(pModel.rt_record.e_sense)) _colList.Add("cycle off");
                    if (!string.IsNullOrWhiteSpace(pModel.rt_record.flow)) _colList.Add("flow");
                    if (!string.IsNullOrWhiteSpace(pModel.rt_record.flow_pattern)) _colList.Add("Flow Pattern");
                    if (!string.IsNullOrWhiteSpace(pModel.rt_record.pressure_sensitivity)) _colList.Add("Sensitivity Pressure");
                    if (!string.IsNullOrWhiteSpace(pModel.rt_record.sensitivity_flow)) _colList.Add("Sensitivity flow");
                    if (!string.IsNullOrWhiteSpace(pModel.rt_record.paw_alarm)) _colList.Add("Pres Limit H.");
                    if (!string.IsNullOrWhiteSpace(pModel.rt_record.Pres_Limit_L)) _colList.Add("Pres Limit L.");

                    if (_colList.Count > 0 && string.IsNullOrWhiteSpace(pModel.onOxygen.ENDDATE))
                        errlist.Add(string.Concat("以下欄位 ", string.Join(" ,", _colList), " 有資料，請記得結束氧氣治療!"));

                }
                #endregion 
                #region 檢查插管是否需要結束
                if (pModel.onIntubate != null && !string.IsNullOrWhiteSpace(pModel.onIntubate.STARTDATE)
               && !string.IsNullOrWhiteSpace(pModel.onIntubate.ONMODE_ID)
               && pModel.onIntubate.STARTDATE != string.Concat(pModel.rt_record.recorddate, " ", pModel.rt_record.recordtime)
               )
                {
                    //檢查以下數值是否有資料
                    //如果無資料要結束插管
                    List<string> _colList = new List<string>();
                    if (string.IsNullOrWhiteSpace(pModel.rt_record.artificial_airway_type)) _colList.Add("E.T. Tube");
                    if (string.IsNullOrWhiteSpace(pModel.rt_record.et_size)) _colList.Add("E.T. size");
                    if (string.IsNullOrWhiteSpace(pModel.rt_record.et_mark)) _colList.Add("E.T. mark");
                    if (_colList.Count == 3 && string.IsNullOrWhiteSpace(pModel.onIntubate.ENDDATE))
                        errlist.Add(string.Concat("以下欄位 ", string.Join(" ,", _colList), " 沒有資料，請記得結束插管!"));
                }
                    #endregion 
                    if (errlist.Count > 0)
                    rm.message = string.Join("\n", errlist);
                else
                    rm.status = RESPONSE_STATUS.SUCCESS;

            }
            catch (Exception ex)
            {
                LogTool.SaveLogMessage(ex, "SaveCheck", GetLogToolCS.RTRecordController);
            }

            return rm;
        }
        /// <summary>
        /// 設定儀器要檢查的資料
        /// </summary>
        /// <param name="pValList"></param>
        /// <param name="chkStr"></param>
        /// <param name="txtStr"></param>
        /// <param name="setStr"></param>
        private void setCheckData(ref Dictionary<string, List<string>> pValList,string chkStr = "", string txtStr = "", string setStr = "")
        {
            pValList["chk"].Clear();
            pValList["txt"].Clear();
            pValList["set"].Clear();
            if (!string.IsNullOrWhiteSpace(chkStr)) pValList["chk"] = chkStr.Split(',').ToList();
            if (!string.IsNullOrWhiteSpace(txtStr)) pValList["txt"] = txtStr.Split(',').ToList();
            if (!string.IsNullOrWhiteSpace(setStr)) pValList["set"] = setStr.Split(',').ToList();

        } 
        /// <summary> 儲存呼吸照護紀錄單 </summary>
        /// <param name="row"></param>
        /// <returns></returns>
        public JsonResult RTRecordFormSave(string rowStr,string setIpdno)
        {
            bool noMode_memo = false;
            RESPONSE_MSG rm = new RESPONSE_MSG();
            RT_RECORD_MAIN row = new RT_RECORD_MAIN();
            string actionName = "RTRecordFormSave";
            try
            {
                rowStr = HttpUtility.UrlDecode(rowStr);
                row = JsonConvert.DeserializeObject<RT_RECORD_MAIN>(rowStr); 
                // rm = this.SaveCheck(row);
                if (rm.status != RESPONSE_STATUS.SUCCESS)
                {
                    return Json(rm);
                }
                //如果不是VIP資料，要檢查
                if (!row.isVIP)
                {
                    rm = BaseModel.saveRecordDateCheck(setIpdno, string.Format("{0} {1}", row.rt_record.recorddate, row.rt_record.recordtime), pat_info);
                    if (rm.status != RESPONSE_STATUS.SUCCESS) return Json(rm);//檢查錯誤
                }
                rm = RTRecordModel.RTRecordFormSave(row, setIpdno);
            }
            catch (Exception ex)
            {
                rm.status = RESPONSE_STATUS.EXCEPTION;
                rm.message = string.Format("使用者({2})(病歷號{1})流水號({3})" + "程式錯誤，請洽資訊人員處理，錯誤訊息如下所示:{0}", ex.Message.ToString(), pat_info.chart_no, user_info.user_id, row.RECORD_ID);
                LogTool.SaveLogMessage(rm.message, actionName, GetLogToolCS.RTRecordController);
                rm.message = string.Format("程式錯誤，請洽資訊人員處裡。", ex.Message.ToString());
                return Json(rm);
            }

            if (rm.status == RESPONSE_STATUS.SUCCESS)
            {
                if (rm.message == null || rm.message == "") rm.message = "儲存成功!";
                if (noMode_memo && funSetting.checkMode_memo)
                {
                    rm.status = RESPONSE_STATUS.WARN;
                    rm.message = "呼吸器模式  "+ row.rt_record.mode + "  尚未維護，資料儲存成功!";
                }
            }
            return Json(rm);
        }

        /// <summary>刪除呼吸照護紀錄單</summary>
        /// <param name="record_id">呼吸記錄單編號</param>
        /// <param name="breath_start_date">呼吸開始日期</param>
        /// <param name="recorddate">呼吸記錄單的對應調整時間</param>
        /// <returns></returns>
        public JsonResult RTRecordFormDel(string record_id, string breath_start_date, string oxygen_start_date, string recorddate)
        {
            RESPONSE_MSG rm = new RESPONSE_MSG();
            dbResultMessage dbResultMsg = new dbResultMessage();
            CxrResultJson_cls CxrResult_Node = new CxrResultJson_cls();
            CxrCanvasLineDrawMdl = new CxrCanvasLineDraw(); //2018.07.20 CXR畫線
            try
            {
                this.DBA.BeginTrans();
                this.DBA.ExecuteNonQuery(string.Format("UPDATE {1} SET DATASTATUS = '9' WHERE RECORD_ID = {0}", SQLDefend.SQLString(record_id), GetTableName.RCS_RECORD_MASTER.ToString()));
                //刪除成功
                if (this.DBA.LastError == null || this.DBA.LastError == "")
                {
                    rm.status = RESPONSE_STATUS.SUCCESS;
                    rm.message = "刪除成功!";
                    this.DBA.Commit();
                }
                else
                {
                    rm.message = "刪除失敗!" + this.DBA.LastError;
                    rm.status = RESPONSE_STATUS.ERROR;
                    this.DBA.Rollback();
                }

            }
            catch (Exception ex)
            {
                rm.status = RESPONSE_STATUS.EXCEPTION;
                rm.message = string.Format("使用者({1})(病歷號{0})流水號({2})系統發生例外，請聯絡資訊人員!錯誤訊息如下所示:" + ex.Message.ToString(), pat_info.chart_no, user_info.user_id, record_id);
                LogTool.SaveLogMessage(ex, "RTRecordFormDel");
                rm.message = "系統發生例外，請聯繫資訊人員，錯誤訊息如下" + ex.ToString();
            }
            return Json(rm);
        }
#endregion
#endregion

#region 功能
        /// <summary>取得片語資烙</summary>
        /// <returns></returns>
        [HttpPost]
        public JsonResult GetPhraseData(string type)
        {
            List<DB_RCS_SYS_PARAMS> RTRecordAllList = new List<DB_RCS_SYS_PARAMS>();
            RESPONSE_MSG rm = this.rtRecordModel.GetPhraseData(type);
            if (rm.hasLastError)
            {
                LogTool.SaveLogMessage(string.Format("使用者({0})病歷號({1})，錯誤訊息=({2})", user_info.user_id, pat_info.chart_no, rm.message), "getBloodBiochemicalData");
            }
            return Json((List<DB_RCS_SYS_PARAMS>)rm.attachment);
        }

        /// <summary>取得最後一筆呼吸紀錄或byTurn的資料</summary>
        /// <param name="buttonStr"></param>
        /// <returns></returns>
        public JsonResult RecordLastData(string buttonStr)
        {
            RecordList rl = new RecordList();

            DataTable Dt = new DataTable();
            if (buttonStr != null && buttonStr == "last_data")
            {
                // 2016.4.18 joeshen fix get the same time patient record problem.
                Dt = RTRecordModel.getFinalRTRecord(pat_info.chart_no);
            }
            else
            {
                // 2016.6.17 byTurn Data
                Dt = RTRecordModel.getByTurnData(pat_info.chart_no);
            }

            Dictionary<string, List<string>> dictionary = new Dictionary<string, List<string>>();

            if (Dt != null && Dt.Rows.Count > 0)
            {
                int cnt = 0;

                foreach (DataRow Dr in Dt.Rows)
                {
                    string record_id = Dr["RECORD_ID"].ToString().Trim();
                    if (!rl.Exists(x => x.record_id == record_id))
                    {
                        rl.Add(record_id, new Dictionary<string, string>());
                    }

                    rl.Find(x => x.record_id == record_id).content.Add(Dr["ITEM_NAME"].ToString().ToLower(), Dr["ITEM_VALUE"].ToString());
                    cnt++;
                }
            }


            List<RT_RECORD_MAIN> list = new List<RT_RECORD_MAIN>();
            if (rl.Count > 0)
            {

                if (rl.Count > 5)
                {
                    RecordList rl2 = new RecordList();
                    for (int i = 0; i < 5; i++)
                    {
                        rl2.Add(rl[i]);
                    }
                    rl.Clear();
                    rl = rl2;
                }

                foreach (var item in rl)
                {
                    RT_RECORD_MAIN row = new RT_RECORD_MAIN();
                    string temp_json = JsonConvert.SerializeObject(item.content);
                    row.rt_record = new RT_RECORD();
                    row.on_breath = new ONBREATH();
                    row.on_breath.breath_weaning = new List<BREATHWEANING>();
                    row.on_intubate = new ONINTUBATE();
                    row.on_oxygen = new ONOXYGEN();
                    row.rt_record = JsonConvert.DeserializeObject<RT_RECORD>(temp_json);
                    list.Add(row);
                }
            }
            else
            {
                if (buttonStr != null && buttonStr == "last_data")
                {
                    RT_RECORD_MAIN row = new RT_RECORD_MAIN();
                    row.rt_record = new RT_RECORD();
                    row.on_breath = new ONBREATH();
                    row.on_breath.breath_weaning = new List<BREATHWEANING>();
                    row.on_intubate = new ONINTUBATE();
                    row.on_oxygen = new ONOXYGEN();
                    list.Add(row);
                }
            }

            if (buttonStr != null && buttonStr == "last_data")
                return Json(list[0]);
            else
            {
                return Json(list);
            }
        }
        /// <summary>
        /// 取得血液生化資料(取得30天內檢驗資料)
        /// </summary>
        /// <returns></returns>
        public JsonResult getBloodBiochemicalData()
        {
            List<ExamBloodBiochemical> ExamBloodBiochemical = new List< ExamBloodBiochemical>();
            RESPONSE_MSG rm = this.rtRecordModel.getBloodBiochemicalData(pat_info, user_info, this.hospFactory.webService.HISBloodBiochemicalLabData());
            if (rm.hasLastError)
            {
                LogTool.SaveLogMessage(string.Format("使用者({0})病歷號({1})，錯誤訊息=({2})", user_info.user_id, pat_info.chart_no, rm.message), "getBloodBiochemicalData");
            }
            return Json((List<ExamBloodBiochemical>)rm.attachment);
        }


        /// <summary>取得三天內的Abg資料</summary>
        /// <returns></returns>
        [HttpPost]
        public JsonResult GetLastAbgList()
        {
            List<ExamABG> abglist = new List<ExamABG>();
            RESPONSE_MSG rm = this.rtRecordModel.GetLastAbgList(pat_info, user_info, this.hospFactory.webService.HISABGLabData());
            if (rm.hasLastError)
            {
                LogTool.SaveLogMessage(string.Format("使用者({0})病歷號({1})，錯誤訊息=({2})", user_info.user_id, pat_info.chart_no, rm.message), "GetLastAbgList");
            }
            return Json((List<ExamABG>)rm.attachment);
        } 
        

        //getBreathList
        /// <summary>
        /// 取得呼吸器清單
        /// </summary>
        /// <returns></returns>
        public JsonResult getBreathList()
        {
            string actioName = "getBreathList"; 
            RESPONSE_MSG rm = this.rtRecordModel.getBreathList(pat_info, user_info);
            if (rm.hasLastError)
            {
                LogTool.SaveLogMessage(string.Format("使用者({0})病歷號({1})，錯誤訊息=({2})", user_info.user_id, pat_info.chart_no, rm.message), "getBloodBiochemicalData");
            }
            return Json((List<BREATH_LIST>)rm.attachment); 
        }


#endregion

#region 列印記錄單清單
        public JsonResult checkRTRecord(string sDate, string eDate, string setIpdno, string record_id)
        {
            RESPONSE_MSG rm = new RESPONSE_MSG();
            string actionName = "checkRTRecord";
            try
            {
                string tempList = this.RecordListData(sDate, eDate, setIpdno, record_id);
                List<RT_RECORD_MAIN> rtRecordList = JsonConvert.DeserializeObject<List<RT_RECORD_MAIN>>(tempList);
                if(rtRecordList != null && rtRecordList.Count > 0)
                {
                    rm.status = RESPONSE_STATUS.SUCCESS;
                }
                else
                {
                    rm.message = "查無相符呼吸照護記錄單對應列印資料";
                    rm.status = RESPONSE_STATUS.ERROR;
                }
            }
            catch (Exception ex)
            {
                rm.message = "程式發生錯誤，請洽資訊人員，錯誤訊息如下所示:"+ ex.Message;
                rm.status = RESPONSE_STATUS.EXCEPTION;
                LogTool.SaveLogMessage(ex, actionName,GetLogToolCS.RTRecordController);
            }
            return  Json(rm);
        }

        public ActionResult showRTRecord(string sDate, string eDate, string setIpdno, string record_id)
        {
            bool isEmptyPDF = false;
            RTRecordModel.pdfModel = new rtRecordPDFModel();
            BaseModels base_models = new BaseModels();
            IPDPatientInfo patientInfo = new IPDPatientInfo();
            string actionName = "showRTRecord";
            try
            {
                string tempList = this.RecordListData(sDate,eDate, setIpdno, record_id, true);
                RTRecordModel.bindpdfModel(tempList, setIpdno);                
                if (string.IsNullOrWhiteSpace(sDate) && string.IsNullOrWhiteSpace(eDate))
                {
                    if (!string.IsNullOrWhiteSpace(record_id) && RTRecordModel.pdfModel.List.Count > 0)
                    {
                        // 使用record_id找到的資料
                        ViewData["isEmptyPDF"] = isEmptyPDF;
                        List<RT_RECORD_MAIN> getPList = RTRecordModel.pdfModel.List[0];
                        patientInfo = base_models.SelectPatientInfo(getPList[0].IPD_NO, getPList[0].CHART_NO);
                    }
                    else
                    {
                        // 列印空白單
                        RTRecordModel.rtRecordList = new List<RT_RECORD_MAIN>();
                        for (int i = 0; i < 5; i++)
                        {
                            RTRecordModel.rtRecordList.Add(new RT_RECORD_MAIN());
                        }
                        ViewData["rt_record_main"] = RTRecordModel.rtRecordList;
                        ViewData["isEmptyPDF"] = true;
                    }                    
                }
                else
                {
                    ViewData["isEmptyPDF"] = isEmptyPDF;
                    patientInfo = base_models.SelectPatientInfo(RTRecordModel.pdfModel.ipd_no, RTRecordModel.pdfModel.chart_no);
                }
                ViewData["patientInfo"] = patientInfo;
            }
            catch (Exception ex)
            {
                LogTool.SaveLogMessage(ex, actionName, GetLogToolCS.RTRecordController);
            }
            //_FJURTRecordForm.cshtml
            string tempStr = string.Concat("~/Views/RT/exportFile/CHGH/", "_CHGHRTRecordForm.cshtml");
            //RTRecordModel.pdfModel.alarm_msg = "";
            return View(tempStr, RTRecordModel.pdfModel);
        }

        /// <summary>執行此Url，下載PDF檔案</summary>
        /// <returns></returns>
        [HttpPost]
        public ActionResult DownloadPdf() {
            string actionName = "DownloadPdf";
            string HtmlStr = "";
            exportFile efm = new exportFile("DownloadPdf.pdf");
            IDocSetting ds = new RTRecordFormPDFDocSetting();
            try
            {
                HtmlStr = HttpUtility.UrlDecode(Request["HtmlStr"]); 
            }
            catch (Exception ex)
            {
                LogTool.SaveLogMessage(ex, actionName, this.csName);
            }
            //https://stackoverflow.com/questions/3724278/asp-net-mvc-how-can-i-get-the-browser-to-open-and-display-a-pdf-instead-of-disp
            //Response.AppendHeader("Content-Disposition", "inline; filename=foo.pdf");
            ds = new CHGHRTRecordFormPDFDocSetting();
            return efm.exportPDF(HtmlStr, ds);
        }
#endregion

#region 脫離評估 WeaningIndex

        /// <summary>
        /// 脫離評估介面
        /// </summary>
        /// <returns></returns>
        public ActionResult WeaningIndex()
        {
            return View();
        }

        /// <summary>
        /// 脫離評估資料
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public string TakeOffAssessmentData()
        {
            List<RT_TAKEOFF_ASSESSMENT> tk_report = new List<RT_TAKEOFF_ASSESSMENT>();
            try
            { 
                RESPONSE_MSG rm = new RESPONSE_MSG(); 
                rm = this.HisDataModel.TakeOffAssessmentData( pat_info.chart_no);
                tk_report = (List<RT_TAKEOFF_ASSESSMENT>)rm.attachment;
                if (rm.hasLastError)
                {
                    LogTool.SaveLogMessage(rm.lastError, "TakeOffAssessmentData");
                }
            }
            catch (Exception ex)
            {
                LogTool.SaveLogMessage(ex, "TakeOffAssessmentData");
            }
            return JsonConvert.SerializeObject(tk_report);
        }
 
        /// <summary>
        /// ABG Data資料
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public string ABGData()
        {
            List<RT_RECORD> tk_report = new List<RT_RECORD>();
            try
            {
                RESPONSE_MSG rm = new RESPONSE_MSG();
                rm = this.HisDataModel.getABGData(pat_info.chart_no);
                tk_report = (List<RT_RECORD>)rm.attachment;
                if (rm.hasLastError)
                {
                    LogTool.SaveLogMessage(rm.lastError, "TakeOffAssessmentData");
                }
            }
            catch (Exception ex)
            {
                LogTool.SaveLogMessage(ex, "ABGData");
            }
            return JsonConvert.SerializeObject(tk_report); 
        }

#endregion

#region 歷史紀錄 OnModeList

        public ActionResult OnModeList()
        {
            return View();
        }

        /// <summary> 取得目前病患的onmode list </summary>
        /// <returns></returns>
        [HttpPost]
        public JsonResult OnModeHistoryList()
        {
            List<ONMODE_MASTER> onmode_history_list = new List<ONMODE_MASTER>();
            try
            {               
                 
            }
            catch (Exception ex)
            {
                LogTool.SaveLogMessage(ex, "OnModeHistoryList");
            }
            return Json(onmode_history_list);
        }

#endregion
         

#region 每日呼吸器脫離評估

        /// <summary>
        /// 每日呼吸器脫離評估畫面
        /// </summary>
        /// <returns></returns>
        public ActionResult RWAC_VIEW()
        {
            return View("RCS_WEANING_ASSESS_CHECKLIST");
        }

        public ActionResult CxrVue_Load() //2018.08.20 AAA
        {
            // Views\RTRecord\RtRecord_OpenCxrEditForm.cshtml
            // Views\RT\RT_Index2.cshtml
            // $('#RtRecord_CxrVue_DivTagID').load('@Url.Content("~/RTRecord/CxrVue_Load")');
            return View("RtRecord_OpenCxrEditForm");
        }//CxrVue_Load

        /// <summary>
        /// 儲存美日呼吸器脫離評估
        /// </summary>
        /// <returns></returns>
        public JsonResult saveRWAC(string vmStr)
        {
            vmStr = HttpUtility.UrlDecode(vmStr);
            string actionName = "saveRWAC";
            RESPONSE_MSG rm = new RESPONSE_MSG();
            try
            {
                RCS_WEANING_ASSESS_CHECKLIST vm = JsonConvert.DeserializeObject<RCS_WEANING_ASSESS_CHECKLIST>(vmStr); 
                rm = this.rtRecordModel.save_RCS_WEANING_ASSESS_CHECKLIST( pat_info, user_info, vm);
                if (rm.status == RESPONSE_STATUS.SUCCESS)
                { 
                    rm.message = "儲存成功!"; 
                }
                else
                {
                    LogTool.SaveLogMessage(rm.lastError, actionName, this.csName);
                    rm.status = RESPONSE_STATUS.ERROR;
                    rm.message = "儲存失敗!";
                } 
            }
            catch (Exception ex)
            {
                rm.status = RESPONSE_STATUS.EXCEPTION;
                LogTool.SaveLogMessage(ex, actionName, this.csName);
            }
            
            return Json(rm);
        }

        public JsonResult getTodayRWAC(string chart_no, string ipd_no)
        {
            string actionName = "getTodayRWAC"; 
            RCS_WEANING_ASSESS_CHECKLIST vm = new RCS_WEANING_ASSESS_CHECKLIST();
            RESPONSE_MSG<DB_RCS_WEANING_ASSESS_CHECKLIST> rm = new RESPONSE_MSG<DB_RCS_WEANING_ASSESS_CHECKLIST>();
            rm = this.rtRecordModel.getRWCA(chart_no, ipd_no, DateTime.Now.ToString("yyyy-MM-dd"));
            if (rm.status != RESPONSE_STATUS.SUCCESS)
            {
                LogTool.SaveLogMessage(rm.lastError, actionName, this.csName);
            } 
            return Json(rm.attachment);
        }//getTodayRWAC

        /// <summary>       
        /// getCXR 
        /// </summary>
        /// <param name="input_RecordDetail_ID">[RCS_CPT_ASS_DETAIL] [RCS_RECORD_DETAIL]</param>
        /// <returns></returns>
        public JsonResult getCxrResultNode_byIdRecordDetail(string input_RecordDetail_ID)
        {
            // Views\RTRecord\RtRecord_OpenCxrEditForm.cshtml
            CxrCanvasLineDrawMdl = new CxrCanvasLineDraw(); //2018.07.20 CXR畫線
            CxrResultJson_cls returnCxrResult_Node = new CxrResultJson_cls();
            try
            {
                //輸入不得為 [空值]
                if (!string.IsNullOrWhiteSpace(input_RecordDetail_ID))
                {
                    //搜尋 [Cxr資料庫]
                    CxrCanvasLineDrawMdl = new CxrCanvasLineDraw(); //2018.07.20 CXR畫線
                    returnCxrResult_Node = CxrCanvasLineDrawMdl.getCxrResultList_byIdDetail(
                            input_RecordDetail_ID.ToString().Trim() //["CPT_ID"] ["RECORD_ID"] 主檔編號
                            , GetTableName.RCS_RECORD_DETAIL.ToString().Trim() //[RCS_CPT_ASS_DETAIL] [RCS_RECORD_DETAIL]
                        ).ToList().FirstOrDefault();
                    //回傳以前，先確認SQL讀取(保護機制)
                    if (returnCxrResult_Node != null 
                        && returnCxrResult_Node.SqlMasterDetail_ID.ToString().Trim() != input_RecordDetail_ID.ToString().Trim()
                    ){
                        LogTool.SaveLogMessage("資料筆數有錯誤，請確認Cxr是否有錯誤!!", "getCxrResultNode_byIdRecordDetail", this.csName);
                        returnCxrResult_Node = null; //先清空 (保護機制)
                    }
                    //若 [Detail] 沒有有資料，[returnCxrResult_Node]會[null]，仍要 [new] 一個新的
                    else if (returnCxrResult_Node == null)
                    {
                        returnCxrResult_Node = new CxrResultJson_cls();
                    }
                }
                //CXR下拉清單
                returnCxrResult_Node.ResultStr_Dropdownlist = CxrCanvasLineDrawMdl.getCxrResultStr_DropdownList();
                if (returnCxrResult_Node != null && !string.IsNullOrWhiteSpace(returnCxrResult_Node.Cxr_CJID))
                {
                    SQLProvider _sql = new SQLProvider();
                    Dapper.DynamicParameters dp = new Dapper.DynamicParameters();
                    dp.Add("S_ID", returnCxrResult_Node.Cxr_CJID);
                    List<string> pSinJsonList = _sql.DBA.getSqlDataTable<string>("SELECT S_JSON FROM RCS_SIGNATURE_JSON WHERE S_ID = @S_ID AND S_NAME = 'JSON'", dp);
                    if (pSinJsonList != null && pSinJsonList.Count> 0 && !string.IsNullOrWhiteSpace(pSinJsonList[0]) 
                        && pSinJsonList[0].StartsWith("[") && pSinJsonList[0].EndsWith("]"))
                    {
                        returnCxrResult_Node.singJson = JsonConvert.DeserializeObject<List<List<SIGNATURE_JSON>>>(pSinJsonList[0]);
                    }
                }
            }//try
            catch (Exception ex)
            {
                LogTool.SaveLogMessage(ex, "getCxrResultNode_byIdRecordDetail", this.csName);
            }//catch
            return Json(returnCxrResult_Node);
        }//getCxrResultNode_byIdRecordDetail

        #endregion

        [Silencer(CheckPat_infoSession = false, CheckUser_infoSession = false)]
        [HttpGet]
        public ActionResult cxr_paint()
        {

            return View();
        }

        public JsonResult getResultDropdownlist()
        {
            string actionName = "getResultDropdownlist";
            List<string> pList = new List<string>();
            try
            {
                pList = BaseModel.getRCS_SYS_PARAMS("", "cxr_result", @pStatus: "1").Select(x => x.P_VALUE).ToList();
            }
            catch (Exception ex)
            {
                LogTool.SaveLogMessage(ex, actionName, this.csName);
            }
            return Json(pList);
        }
    }
}
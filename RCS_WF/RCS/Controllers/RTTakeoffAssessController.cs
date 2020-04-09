using Com.Mayaminer;
using Newtonsoft.Json;
using RCS_Data;
using System;
using System.Collections.Generic;
using System.Web.Mvc;
using System.Web;
using System.Data;
using RCS.Models;
using mayaminer.com.library;
using Dapper;
using System.Linq;
using RCSData.Models;
using RCS_Data.Models.ViewModels;
using RCS_Data.Models.DB;
using RCS.Models.ViewModel;

namespace RCS.Controllers
{
    public class RTTakeoffAssessController : BaseController
    {
        string csName { get { return "RTTakeoffAssessController"; } }
        //RTTakeoffAssess
        #region 呼吸器脫離評估
        string csNmae { get { return "RTTakeoffAssessController"; } }
        private RTTakeoffAssess RTTakeoffAssessModel;
        private RTRecordController RTRecordCtrl;
        public RTTakeoffAssessController()
        {
            RTTakeoffAssessModel = new RTTakeoffAssess();
            RTRecordCtrl = new RTRecordController();
        }

        RTTakeoff _model;
        RTTakeoff RtTakeoffModel
        {
            get
            {
                if (_model == null)
                {
                    this._model = new RTTakeoff();
                }
                return this._model;
            }
        }

        /// <summary>
        /// 開啟脫離評估
        /// </summary>
        /// <param name="row"></param>
        /// <returns></returns>
        public ActionResult openRTTakeoffAssessForm(string rowStr)
        {
            string actionName = "openRTTakeoffAssessForm";
            try
            {
                RCS_DATA_TK_ASSESS row = new RCS_DATA_TK_ASSESS();
                rowStr = HttpUtility.UrlDecode(rowStr);
                row = JsonConvert.DeserializeObject<RCS_DATA_TK_ASSESS>(rowStr);
                //檢查病患Session
                if (row != null && !string.IsNullOrWhiteSpace(row.chart_no))
                {
                    IPDPatientInfo tempPat_info = (IPDPatientInfo)SetSession(row.chart_no, "C").Data;
                    if (tempPat_info == null || tempPat_info.chart_no != row.chart_no)
                        ViewData["errorMsg"] = "開啟記錄單時，程式發生錯誤，請洽資訊人員!";
                }
                List<SysParams> doc_list = BaseModel.getRCS_SYS_PARAMS("user", "doctor");
                ViewData["doc_list"] = doc_list;
                ViewData["RECORDData"] = JsonConvert.SerializeObject(row);
            }
            catch (Exception ex)
            {
                ViewData["errorMsg"] = "開啟記錄單時，程式發生錯誤，請洽資訊人員!錯誤內容:" + ex.Message;
                LogTool.SaveLogMessage(ex, actionName, GetLogToolCS.RTTakeoffAssessController);
            }
            return View("RTTakeoffAssessForm");
        }

        /// <summary>
        /// 呼吸器脫離評估預設資料
        /// </summary>
        /// <returns></returns>
        public ActionResult RTTakeoffAssess()
        {
            ViewModelBasic vm = new ViewModelBasic();
            string actionName = "RTTakeoffAssess";
            try
            {
                vm.getPatientHistoryList = BaseModel.getPatientHistoryList(pat_info.chart_no, pat_info.ipd_no);
                if (vm.getPatientHistoryList.Exists(x => x.Selected))
                {
                    SelectListItem item = new SelectListItem();
                    item = vm.getPatientHistoryList.Find(x => x.Selected);
                    vm.sDate = BaseModel.getHistoryList(item.Value, 1);
                    vm.eDate = BaseModel.getHistoryList(item.Value, 2);
                } 
            }
            catch (Exception ex)
            {
                LogTool.SaveLogMessage(ex, actionName, GetLogToolCS.RTTakeoffAssessController);
            }
            return View(vm);
        }

 
 
        #endregion

      

        #region 新呼吸器脫離評估

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult RTTakeoffAssessNewForm()
        {
            List<SelectListItem> getPatientHistoryList = BaseModel.getPatientHistoryList(pat_info.chart_no, pat_info.ipd_no);
            ViewData["getPatientHistoryList"] = getPatientHistoryList;
            List<string> cptData = BaseModel.getCPTAssess(pat_info.chart_no);
            SysParamCollection sys_params_list = new SysParamCollection();
            sys_params_list.append_modal(BaseModel.GetModelListCollection("RTRecord_Detail"));
            ViewData["sys_params_list"] = sys_params_list;
           
            return View();
        }

        /// <summary>
        /// 設定呼吸器脫離評估單
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public string RTTakeoffDataNew(string pSDate, string pEDate, string pIpdno, string pId, bool isLast = false)
        {
            List<RTTakeoffAssessViewModel> rdta_list = new List<RTTakeoffAssessViewModel>();
            rdta_list = getRTTakeoffData(pSDate, pEDate, pIpdno, pId, isLast);
            if (rdta_list.Count > 0 && !string.IsNullOrWhiteSpace(pId))
            {//編輯
                if (user_info.user_id == rdta_list[0].create_id)
                {
                    rdta_list[0].hasPowerEdit = true;
                }
            }

            return JsonConvert.SerializeObject(rdta_list);
        }

        private List<RTTakeoffAssessViewModel> getRTTakeoffData(string pSDate, string pEDate, string pIpdno, string pId, bool isLast = false)
        {
            List<RTTakeoffAssessViewModel> rdta_list = new List<RTTakeoffAssessViewModel>();
            try
            {
                string sql = "";
                SQLProvider SQL = new SQLProvider();
                DynamicParameters dp = null;
                if (!isLast)
                {
                    //編輯
                    //取得List清單
                    string wSql = "";
                    pSDate = pSDate ?? "";pEDate = pEDate ?? "";pId = pId ?? "";

                    if (pId != "")
                        wSql = string.Format(" AND TK_ID = {0}", SQLDefend.SQLString(pId));//編輯
                    else if (pSDate != "" && pEDate != "")
                        wSql = string.Format(" AND REC_DATE >= {0} AND REC_DATE <= {1}", SQLDefend.SQLString(pSDate), SQLDefend.SQLString(pEDate));//取得List清單
                    if (!string.IsNullOrWhiteSpace(pIpdno))
                        wSql += string.Format(" AND IPD_NO = {0}", SQLDefend.SQLString(BaseModel.getHistoryList(pIpdno, 0)));
                    if (user_info.authority == "readonly")
                        wSql += " AND UPLOAD_STATUS = '1'";

                    sql = @"SELECT (
                                SELECT tk_id 
                                FROM RCS_WEANING_ASSESS 
                                WHERE DATASTATUS = '1' 
                                    AND CHART_NO = " + SQLDefend.SQLString(pat_info.chart_no) + @" 
                                    AND REC_DATE in(
                                        SELECT MAX(REC_DATE) 
                                        FROM RCS_WEANING_ASSESS 
                                        WHERE DATASTATUS = '1' 
                                            AND CHART_NO = " + SQLDefend.SQLString(pat_info.chart_no) + @")) 
                            as last_tk_id,tk_id,ipd_no,chart_no,rec_date,tk_reason as _tk_reason,st_reason as _st_reason,tk_plan as _tk_plan,UPLOAD_STATUS,CREATE_NAME  ,CREATE_ID 
                            FROM RCS_WEANING_ASSESS" + string.Format(@" 
                            WHERE DATASTATUS = '1' 
                                AND CHART_NO = {0} {1}", SQLDefend.SQLString(pat_info.chart_no), wSql);
                }
                else
                {//帶入上一筆
                    sql = @"SELECT tk_id,ipd_no,chart_no,rec_date 
                            FROM RCS_WEANING_ASSESS 
                            WHERE  DATASTATUS = '1' 
                                AND CHART_NO = @CHART_NO 
                                AND IPD_NO = @IPD_NO 
                                AND REC_DATE in ( 
                                    SELECT MAX(REC_DATE) 
                                    FROM RCS_WEANING_ASSESS 
                                    WHERE DATASTATUS = '1' 
                                        AND CHART_NO = @CHART_NO 
                                        AND IPD_NO = @IPD_NO)";
                    dp = new DynamicParameters();
                    dp.Add("CHART_NO", pat_info.chart_no);
                    dp.Add("IPD_NO", pat_info.ipd_no);
                }

                rdta_list = SQL.DBA.getSqlDataTable<RTTakeoffAssessViewModel>(sql, dp);
                sql = string.Concat(
                    @"SELECT * FROM RCS_WEANING_ASSESS_DTL 
                        WHERE TK_ID in('", string.Join("','", rdta_list.Select(x => x.tk_id).ToList()), "')");
                List<DB_RCS_WEANING_ASSESS_DTL> _temp = SQL.DBA.getSqlDataTable<DB_RCS_WEANING_ASSESS_DTL>(sql);

                if (!string.IsNullOrWhiteSpace(pSDate) && !string.IsNullOrWhiteSpace(pEDate))
                {
                    //取得List清單
                    foreach (RTTakeoffAssessViewModel o in rdta_list)
                    {
                        List<DB_RCS_WEANING_ASSESS_DTL> _item = _temp.FindAll(x => x.TK_ID == o.tk_id);
                        string tst = JsonConvert.SerializeObject(_item);
                        if (_item.Count> 0)
                        {
                            o.DTL = new VM_RTTakeoffAssess()
                            {

                                //呼吸器脫離困難原因_Unstable_vital_signs
                                Unstable_vital_signs = JsonConvert.DeserializeObject<List<JSON_DATA>>(_item.Find(x => x.ITEM_NAME == "_Unstable_vital_signs").ITEM_VALUE),
                                //呼吸器脫離困難原因_gas_exchange
                                gas_exchange = JsonConvert.DeserializeObject<List<JSON_DATA>>(_item.Find(x => x.ITEM_NAME == "_gas_exchange").ITEM_VALUE),
                                //呼吸器脫離困難原因_underlying_disease
                                underlying_disease = JsonConvert.DeserializeObject<List<JSON_DATA>>(_item.Find(x => x.ITEM_NAME == "_underlying_disease").ITEM_VALUE),
                                //呼吸器脫離困難原因_weaning_drug
                                weaning_drug = JsonConvert.DeserializeObject<List<JSON_DATA>>(_item.Find(x => x.ITEM_NAME == "_weaning_drug").ITEM_VALUE),
                                //呼吸器脫離困難原因_poor_respiratory_drive
                                poor_respiratory_drive = JsonConvert.DeserializeObject<List<JSON_DATA>>(_item.Find(x => x.ITEM_NAME == "_poor_respiratory_drive").ITEM_VALUE),
                                //呼吸器脫離困難原因_poor_respirator_muscle
                                poor_respirator_muscle = JsonConvert.DeserializeObject<List<JSON_DATA>>(_item.Find(x => x.ITEM_NAME == "_poor_respirator_muscle").ITEM_VALUE),
                                //呼吸器脫離困難原因_poor_cough_fuction
                                poor_cough_fuction = JsonConvert.DeserializeObject<List<JSON_DATA>>(_item.Find(x => x.ITEM_NAME == "_poor_cough_fuction").ITEM_VALUE),
                                //呼吸器脫離困難原因_malnutrition
                                malnutrition = JsonConvert.DeserializeObject<List<JSON_DATA>>(_item.Find(x => x.ITEM_NAME == "_malnutrition").ITEM_VALUE),
                                //呼吸器脫離困難原因_weaning_other
                                weaning_other = JsonConvert.DeserializeObject<List<JSON_DATA>>(_item.Find(x => x.ITEM_NAME == "_weaning_other").ITEM_VALUE),
                                //呼吸器脫離計畫_try_weaning_mode
                                try_weaning_mode = JsonConvert.DeserializeObject<List<JSON_DATA>>(_item.Find(x => x.ITEM_NAME == "_try_weaning_mode").ITEM_VALUE),
                                //呼吸器脫離計畫_try_t_piece
                                try_t_piece = JsonConvert.DeserializeObject<List<JSON_DATA>>(_item.Find(x => x.ITEM_NAME == "_try_t_piece").ITEM_VALUE),
                                //呼吸器脫離計畫_try_t_piece_over_night
                                try_t_piece_over_night = JsonConvert.DeserializeObject<List<JSON_DATA>>(_item.Find(x => x.ITEM_NAME == "_try_t_piece_over_night").ITEM_VALUE),
                                //呼吸器脫離計畫_weaning_plan_other
                                weaning_plan_other = JsonConvert.DeserializeObject<List<JSON_DATA>>(_item.Find(x => x.ITEM_NAME == "_weaning_plan_other").ITEM_VALUE),

                                try_breath_reason = JsonConvert.DeserializeObject<List<JSON_DATA>>(_item.Find(x => x.ITEM_NAME == "_try_breath_reason").ITEM_VALUE),

                                //1.病患主要呼吸問題
                                breathingQuestion = JsonConvert.DeserializeObject<List<JSON_DATA>>(_item.Find(x => x.ITEM_NAME == "_breathingQuestion").ITEM_VALUE),
                                //2.胸腔病史
                                chest_history = JsonConvert.DeserializeObject<List<JSON_DATA>>(_item.Find(x => x.ITEM_NAME == "_chest_history").ITEM_VALUE),
                                //3.意識
                                conscious = JsonConvert.DeserializeObject<List<JSON_DATA>>(_item.Find(x => x.ITEM_NAME == "_conscious").ITEM_VALUE),
                                //4.皮膚
                                skin = JsonConvert.DeserializeObject<List<JSON_DATA>>(_item.Find(x => x.ITEM_NAME == "_skin").ITEM_VALUE),
                                //5.呼吸型態
                                breathing_patterns = JsonConvert.DeserializeObject<List<JSON_DATA>>(_item.Find(x => x.ITEM_NAME == "_breathing_patterns").ITEM_VALUE),
                                //6.呼吸音
                                breath_sounds = JsonConvert.DeserializeObject<List<JSON_DATA>>(_item.Find(x => x.ITEM_NAME == "_breath_sounds").ITEM_VALUE),
                                //7.咳嗽能力
                                cough = JsonConvert.DeserializeObject<List<JSON_DATA>>(_item.Find(x => x.ITEM_NAME == "_cough").ITEM_VALUE),
                                //8.痰液
                                sputum = JsonConvert.DeserializeObject<List<JSON_DATA>>(_item.Find(x => x.ITEM_NAME == "_sputum").ITEM_VALUE),
                                //9.性質
                                //sputum = JsonConvert.DeserializeObject<List<JSON_DATA>>(_item.Find(x => x.ITEM_NAME == "_sputum").ITEM_VALUE),
                                //10.其它藥物使用
                                other_drugs = JsonConvert.DeserializeObject<List<JSON_DATA>>(_item.Find(x => x.ITEM_NAME == "_other_drugs").ITEM_VALUE),
                                //11.噴霧治療
                                spray_treatment = JsonConvert.DeserializeObject<List<JSON_DATA>>(_item.Find(x => x.ITEM_NAME == "_spray_treatment").ITEM_VALUE),
                                //12.藥物
                                drug = JsonConvert.DeserializeObject<List<JSON_DATA>>(_item.Find(x => x.ITEM_NAME == "_drug").ITEM_VALUE),
                                //13.胸腔復健治療
                                cpt_treatment = JsonConvert.DeserializeObject<List<JSON_DATA>>(_item.Find(x => x.ITEM_NAME == "_cpt_treatment").ITEM_VALUE)
                            };
                            //新增欄位
                            if (_item.Exists(x => x.ITEM_NAME == "_isweaning"))
                                o.DTL.isweaning = JsonConvert.DeserializeObject<List<JSON_DATA>>(_item.Find(x => x.ITEM_NAME == "_isweaning").ITEM_VALUE);

                        }
                        else
                        {
                            o.DTL = new VM_RTTakeoffAssess();
                        }
                      
                    }
                    rdta_list = rdta_list.OrderByDescending(x => DateTime.Parse(x.rec_date)).ToList();
                }

                #region 取得單一筆資料
                sql = "";
                dp = new DynamicParameters();

                if (!string.IsNullOrWhiteSpace(pId))
                {//編輯
                    sql = "SELECT * FROM RCS_WEANING_ASSESS_DTL WHERE TK_ID =@TK_ID";
                    dp.Add("TK_ID", pId);
                }
                else if (isLast)
                {//帶入上一筆
                    sql = "SELECT * FROM RCS_WEANING_ASSESS_DTL WHERE TK_ID in(SELECT MAX(TK_ID) FROM RCS_WEANING_ASSESS WHERE REC_DATE in(SELECT MAX(REC_DATE) FROM RCS_WEANING_ASSESS WHERE DATASTATUS = '1' AND CHART_NO = @CHART_NO))";
                    dp.Add("CHART_NO", pat_info.chart_no);
                }

                if (sql.Length > 0)
                {
                    //帶入上一筆
                    //編輯

                    //詳細項目
                    List<DB_RCS_WEANING_ASSESS_DTL> tempList = SQL.DBA.getSqlDataTable<DB_RCS_WEANING_ASSESS_DTL>(sql, dp);
                    if (tempList.Count > 0)
                    {//帶入上一筆
                        //編輯
                        Dictionary<string, string> toDic = new Dictionary<string, string>();
                        foreach (DB_RCS_WEANING_ASSESS_DTL item in tempList)
                        {
                            toDic.Add(item.ITEM_NAME, item.ITEM_VALUE);
                        }
                        rdta_list[0].DTL = JsonConvert.DeserializeObject<VM_RTTakeoffAssess>(JsonConvert.SerializeObject(toDic));
                    }
                    rdta_list[0].DTL.chart_no = pat_info.chart_no;
                    rdta_list[0].DTL.birth_day = pat_info.birth_day;
                    rdta_list[0].DTL.patient_name = pat_info.patient_name;
                    rdta_list[0].DTL.genderCHT = pat_info.genderCHT;
                    List<string> cptData = BaseModel.getCPTAssess(pat_info.chart_no);
                    if (cptData.Count > 0)
                    {
                        rdta_list[0].DTL.in_hosp_complaint = cptData[0];
                        rdta_list[0].DTL.diagnosis_code = cptData[4];
                    }
 
                    rdta_list[0].DTL.bed_no = pat_info.bed_no;
                    rdta_list[0].DTL.tran_date = string.IsNullOrWhiteSpace(rdta_list[0].DTL.tran_date)? pat_info.diag_date : rdta_list[0].DTL.tran_date;
                   
                    
                    rdta_list = new List<RTTakeoffAssessViewModel>() { rdta_list[0] };

                }
                #endregion


            }
            catch (Exception ex)
            {
                LogTool.SaveLogMessage(ex, "RTTakeoffData");
            }
            finally
            {
                rdta_list = (List<RTTakeoffAssessViewModel>)BaseModel.set_modify_power(rdta_list, "RTTakeoffAssessViewModel");
            }
            return rdta_list;
        }

        public JsonResult getCPTAssessData()
        {
            VM_RTTakeoffAssess vm = new VM_RTTakeoffAssess();
            vm.bed_no = pat_info.bed_no;
            vm.patient_name = pat_info.patient_name;
            vm.chart_no = pat_info.chart_no;
            vm.genderCHT = pat_info.genderCHT;
            vm.birth_day = pat_info.birth_day;
           
            vm.tran_date = pat_info.diag_date;
            List<string> cptData = BaseModel.getCPTAssess(pat_info.chart_no);
            if (cptData.Count > 0)
            {
                vm.diagnosis_code = cptData[4];
                vm.in_hosp_complaint = cptData[0];
            }
               
            return Json(vm);
        }

        public JsonResult RTTakeoffAssessViewModel()
        {
            ViewModelBasic vm = new ViewModelBasic();
            string actionName = "RTTakeoffAssess";
            try
            {
                vm.getPatientHistoryList = BaseModel.getPatientHistoryList(pat_info.chart_no, pat_info.ipd_no);
                if (vm.getPatientHistoryList.Exists(x => x.Selected))
                {
                    SelectListItem item = new SelectListItem();
                    item = vm.getPatientHistoryList.Find(x => x.Selected);
                    vm.sDate = BaseModel.getHistoryList(item.Value, 1);
                    vm.eDate = BaseModel.getHistoryList(item.Value, 2);
                } 
            }
            catch (Exception ex)
            {
                LogTool.SaveLogMessage(ex, actionName, GetLogToolCS.RTTakeoffAssessController);
            }
            return Json(vm);
        }

        public JsonResult Getweaning_select_tb()
        {
            List<RCS_WEANING_ITEM> pList = new List<RCS_WEANING_ITEM>();
            string actioName = "Getweaning_select_tb";
            try
            {
                SQLProvider SQL = new SQLProvider();
                string sql = @"
                    SELECT * FROM RCS_RECORD_DETAIL 
                    WHERE RECORD_ID in(
                    SELECT RECORD_ID FROM RCS_RECORD_MASTER WHERE IPD_NO = @IPD_NO AND CHART_NO = @CHART_NO 
                    AND RECORD_ID in(
                    SELECT RECORD_ID FROM RCS_RECORD_DETAIL WHERE ITEM_NAME  = 'is_weaning' AND  ITEM_VALUE = '1')
                    )
                    AND ITEM_NAME in('pi_max','mv_value','vt_value','rsi_srr','rsbi','recorddate','recordtime') 
                    ";
                DynamicParameters dp = new DynamicParameters();
                dp.Add("IPD_NO", pat_info.ipd_no);
                dp.Add("CHART_NO", pat_info.chart_no);
                List<RTRECORD_DETAIL> dt = SQL.DBA.getSqlDataTable<RTRECORD_DETAIL>(sql, dp);
                List<string> RECORD_ID_List = dt.Select(x => x.RECORD_ID).Distinct().ToList();
                foreach (string str in RECORD_ID_List)
                {
                    List<RTRECORD_DETAIL> temp = dt.FindAll(x => x.RECORD_ID == str);
                    RCS_WEANING_ITEM item = new RCS_WEANING_ITEM();
                    item.seq = pList.Count;
                    item.date = string.Concat(temp.Find(x => x.ITEM_NAME == "recorddate").ITEM_VALUE, " ", temp.Find(x => x.ITEM_NAME == "recordtime").ITEM_VALUE);
                    if (temp.Exists(x => x.ITEM_NAME == "pi_max")) item.pi_max = temp.Find(x => x.ITEM_NAME == "pi_max").ITEM_VALUE;
                    if (temp.Exists(x => x.ITEM_NAME == "mv_value")) item.mv_value = temp.Find(x => x.ITEM_NAME == "mv_value").ITEM_VALUE;
                    if (temp.Exists(x => x.ITEM_NAME == "vt_value")) item.vt_value = temp.Find(x => x.ITEM_NAME == "vt_value").ITEM_VALUE;
                    if(temp.Exists(x => x.ITEM_NAME == "rsi_srr")) item.srr = temp.Find(x => x.ITEM_NAME == "rsi_srr").ITEM_VALUE;
                    if (temp.Exists(x => x.ITEM_NAME == "rsbi")) item.rsbi = temp.Find(x => x.ITEM_NAME == "rsbi").ITEM_VALUE;
                    pList.Add(item);
                }
                if (pList.Count > 0)
                    pList = pList.OrderByDescending(x => DateTime.Parse(x.date)).ToList();



            }
            catch (Exception ex)
            {
                LogTool.SaveLogMessage(ex, actioName, this.csNmae);
            }
            return Json(pList);
        }

        public JsonResult Getlabdata_select_tb()
        {
            List<RCS_LAB_ITEM> pList = new List<RCS_LAB_ITEM>();
            try
            {
                List<ExamBloodBiochemical> ExamBloodBiochemical = new List< ExamBloodBiochemical>();
                if (pat_info.chart_no != null && pat_info.ipd_no != null)
                    ExamBloodBiochemical = BaseModel.web_method.getBloodBiochemicalData(this.hospFactory.webService.HISBloodBiochemicalLabData(), pat_info.chart_no, pat_info.ipd_no);
                ExamBloodBiochemical = ExamBloodBiochemical.OrderByDescending(x => DateTime.Parse(x.examDate)).ToList();
                foreach (ExamBloodBiochemical o in ExamBloodBiochemical)
                {
                    RCS_LAB_ITEM item = new RCS_LAB_ITEM();
                    item.seq = pList.Count;
                    item.date = o.examDate;
                    item.hb = o.examContent["Hb"].result;
                    item.ht = o.examContent["Ht"].result;
                    item.wbc = o.examContent["WBC"].result;
                    item.Platelet = o.examContent["Platelet"].result;
                    item.na = o.examContent["Na"].result;
                    item.k = o.examContent["K"].result;
                    item.bun = o.examContent["BUN"].result;
                    item.cr = o.examContent["Cr"].result;
                    item.albumin = o.examContent["Albumin"].result;
                    pList.Add(item);
                }
                if (pList.Count > 0)
                    pList = pList.OrderByDescending(x => DateTime.Parse(x.date)).ToList();
            }
            catch (Exception ex)
            {
                LogTool.SaveLogMessage(string.Format("使用者({0})病歷號({1})，錯誤訊息=({2})", user_info.user_id, pat_info.chart_no, ex.Message), "Getlabdata_select_tb");
            }
            //string temp = JsonConvert.SerializeObject(ExamBloodBiochemical);

            return Json(pList);
        }

        public JsonResult RTTakeoffNewSave(string rowStr, string setIpdno)
        {
            RESPONSE_MSG rm = new RESPONSE_MSG();
            string actionName = "RTTakeoffNewSave";
            RTTakeoffAssessViewModel rdta = new RTTakeoffAssessViewModel();
            try
            {
                rowStr = HttpUtility.UrlDecode(rowStr);
                rdta = JsonConvert.DeserializeObject<RTTakeoffAssessViewModel>(rowStr);
                rdta.BED_NO = pat_info.bed_no;
                rdta.COST_CODE = pat_info.cost_code;
                rdta.DEPT_CODE = pat_info.dept_code;
                rdta.DATASTATUS = "1";
                rdta.PAT_SOURCE = pat_info.source_type;
                rdta.ipd_no = pat_info.ipd_no;
                rdta.chart_no = pat_info.chart_no;

                //整理儲存資料
                #region 整理儲存資料
                System.Reflection.PropertyInfo[] infos = rdta.DTL.GetType().GetProperties();
                List<DB_RCS_WEANING_ASSESS_DTL> dix = new List<DB_RCS_WEANING_ASSESS_DTL>();
                foreach (System.Reflection.PropertyInfo info in infos)
                {
                    System.Type s = info.PropertyType;
                    string name = s.Name;
                    try
                    {
                        string temp = null;
                        switch (name)
                        {
                            case "String":
                                if (info.GetValue(rdta.DTL, null) != null)
                                    temp = info.GetValue(rdta.DTL, null).ToString();
                                dix.Add(new DB_RCS_WEANING_ASSESS_DTL() { TK_ID = rdta.tk_id, ITEM_NAME = info.Name, ITEM_VALUE = temp });
                                break;
                            default:
                                break;
                        }
                    }
                    catch (Exception ex)
                    {
                        rm.status = RESPONSE_STATUS.EXCEPTION;
                        rm.message = "程式發生錯誤，請洽資訊人員!" + ex.Message;
                        LogTool.SaveLogMessage(ex, actionName, GetLogToolCS.RTTakeoffAssessController);
                    }

                }
                #endregion
                SQLProvider SQL = new SQLProvider();
                if (dix.Count > 0)
                {
                    SQL.DBA.BeginTrans();
                    string sql = "";
                    if (!string.IsNullOrWhiteSpace(rdta.tk_id))
                    {
                        //修改
                        sql = "UPDATE RCS_WEANING_ASSESS SET REC_DATE = @REC_DATE,MODIFY_ID = @MODIFY_ID,MODIFY_NAME = @MODIFY_NAME,MODIFY_DATE = @MODIFY_DATE  WHERE TK_ID = @TK_ID";
                        Dapper.DynamicParameters dp = new Dapper.DynamicParameters();
                        dp.Add("REC_DATE", rdta.rec_date);
                        dp.Add("MODIFY_ID", user_info.user_id);
                        dp.Add("MODIFY_NAME", user_info.user_name);
                        dp.Add("MODIFY_DATE", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
                        dp.Add("TK_ID", rdta.tk_id);
                        SQL.DBA.DBExecute(sql, dp);
                        //刪除舊資料
                        sql = "DELETE RCS_WEANING_ASSESS_DTL WHERE TK_ID = @TK_ID";
                        dp = new Dapper.DynamicParameters();
                        dp.Add("TK_ID", rdta.tk_id);
                        SQL.DBA.DBExecute(sql, dp);
                    }
                    else
                    {
                        rdta.tk_id = SQL.GetFixedStrSerialNumber();
                        dix.ForEach(x => x.TK_ID = rdta.tk_id);
                        rdta.create_date = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                        rdta.create_id = user_info.user_id;
                        rdta.create_name = user_info.user_name;

                        //新增
                        sql = "INSERT INTO RCS_WEANING_ASSESS (TK_ID,IPD_NO,CHART_NO,REC_DATE,TK_REASON,ST_REASON,TK_PLAN,CREATE_DATE,CREATE_ID,CREATE_NAME,DATASTATUS,PAT_SOURCE,COST_CODE,BED_NO,DEPT_CODE)VALUES(@TK_ID,@IPD_NO,@CHART_NO,@REC_DATE,@_tk_reason,@_st_reason,@_tk_plan,@CREATE_DATE,@CREATE_ID,@CREATE_NAME,@DATASTATUS,@PAT_SOURCE,@COST_CODE,@BED_NO,@DEPT_CODE);";
                        SQL.DBA.DBExecute(sql, new List<RCS_DATA_TK_ASSESS>() { rdta });
                    }
                    sql = "INSERT INTO  RCS_WEANING_ASSESS_DTL(TK_ID, ITEM_NAME, ITEM_VALUE) VALUES(@TK_ID, @ITEM_NAME, @ITEM_VALUE)";
                    SQL.DBA.DBExecute(sql, dix);
                }
                if (SQL.DBA.hasLastError)
                {
                    rm.status = RESPONSE_STATUS.ERROR;
                    rm.message = "儲存失敗，請洽資訊人員!" + SQL.DBA.lastError;
                    LogTool.SaveLogMessage(SQL.DBA.lastError, actionName, this.csNmae);
                    SQL.DBA.Rollback();
                }
                else
                {
                    rm.status = RESPONSE_STATUS.SUCCESS;
                    rm.message = "儲存成功!";
                    SQL.DBA.Commit();
                }
            }
            catch (Exception ex)
            {
                rm.status = RESPONSE_STATUS.EXCEPTION;
                rm.message = "程式發生錯誤，請洽資訊人員!" + ex.Message;
                LogTool.SaveLogMessage(ex, actionName, GetLogToolCS.RTTakeoffAssessController);

            }
            return Json(rm);
        }

        public JsonResult RTTakeoffNewDel(string tk_id)
        {
            RESPONSE_MSG rm = new RESPONSE_MSG();
            string actionName = "RTTakeoffNewDel";
            try
            {
                string sql = "UPDATE RCS_WEANING_ASSESS SET DATASTATUS = '9' WHERE TK_ID = @TK_ID";
                DynamicParameters dp = new DynamicParameters();
                dp.Add("TK_ID", tk_id);
                SQLProvider SQL = new SQLProvider();
                SQL.DBA.DBExecute(sql, dp);
                if (SQL.DBA.hasLastError)
                {
                    rm.status = RESPONSE_STATUS.ERROR;
                    rm.message = "刪除失敗!";
                    LogTool.SaveLogMessage(SQL.DBA.lastError, actionName, this.csNmae);
                }
                else
                {
                    rm.status = RESPONSE_STATUS.SUCCESS;
                    rm.message = "刪除成功!";
                }
            }
            catch (Exception ex)
            {
                rm.status = RESPONSE_STATUS.EXCEPTION;
                LogTool.SaveLogMessage(ex, actionName, this.csNmae);
            }
            return Json(rm);
        }

        public ActionResult showRTTakeoffAssess(string pId)
        {
            List<RTTakeoffAssessViewModel> rdta_list = new List<RTTakeoffAssessViewModel>();
            string actionName = "showRTTakeoffAssess";
            //string tempStr = string.Concat("~/Views/RT/exportFile/CHGH/", "_CHGHRTTakeoffForm.cshtml");
            string tempStr = string.Concat("~/Views/RTTakeoffAssess/", "RTTakeoffAssessNewForm.cshtml");
            try
            {
                rdta_list = getRTTakeoffData("", "", "", pId); //pId = 2018050214184452152
            }
            catch (Exception ex)
            {
                LogTool.SaveLogMessage(ex, actionName, this.csName);
            }

            return View(tempStr, rdta_list);
        }

        /// <summary>執行此Url，下載PDF檔案</summary>
        /// <returns></returns>
        [HttpPost]
        public ActionResult DownloadPdf()
        {
            string actionName = "DownloadPdf";
            string HtmlStr = "";
            exportFile efm = new exportFile("DownloadPdf.pdf");
            IDocSetting ds = new RTTakeoffAssessPDFDocSetting();
            try
            {
                HtmlStr = HttpUtility.UrlDecode(Request["HtmlStr"]);
            }
            catch (Exception ex)
            {
                LogTool.SaveLogMessage(ex, actionName, this.csName);
            }

            return efm.exportPDF(HtmlStr, ds);
        }

        #endregion 新呼吸器脫離評估

    }//RTTakeoffAssessController
}//namespace RCS.Controllers

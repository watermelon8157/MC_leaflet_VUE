using Com.Mayaminer;
using RCS.Controllers.WEBAPI;
using RCS.Models;
using RCS.Models.DB;
using RCS.Models.ViewModel;
using RCS_Data;
using RCS_Data.Controllers;
using RCS_Data.Controllers.OPDScheduling;
using RCS_Data.Controllers.RtAssess;
using RCS_Data.Controllers.RtOPDShift;
using RCS_Data.Controllers.RtRecord;
using RCS_Data.Models.DB;
using RCS_Data.Models.ViewModels;
using RCSData.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace RCS.Controllers
{
    [AllowCrossSite]
    public class PDFHtmlController : Controller
    {
        string csName = "PDFHtmlController";
        private RtAssessController RtAssessController = new RtAssessController();
        private SQLProvider _DBLink { get; set; }
        protected SQLProvider DBLink
        {
            get
            {
                if (this._DBLink == null)
                {
                    this._DBLink = new SQLProvider();
                }
                return this._DBLink;
            }
        }

        // GET: PDFHtml

        /// <summary>
        /// Index
        /// </summary>
        /// <returns></returns>  
        public ActionResult Index(string id)
        {
            return View();
        }

        /// <summary>
        /// CPTPageForm
        /// </summary>
        /// <returns></returns> 
        public ActionResult CPTPageForm(string cpt_id, string paylaod, string chart_no, string ipd_no, string pSDate = "", string pEDate = "")
        {
            changeDateCHT<string> changeDate = new changeDateCHT<string>();
            List<CPTAssessViewModel> ViewModel = new List<CPTAssessViewModel>();
            try
            {

                RtAssess rtAssessModel = new RtAssess();

                List<string> idList = new List<string>();

                List<RCS_CPT_DTL_NEW_ITEMS> cpt_dtl_new_items = new List<RCS_CPT_DTL_NEW_ITEMS>();

                if (!string.IsNullOrWhiteSpace(pSDate) && !string.IsNullOrWhiteSpace(pEDate))
                {
                    var rm = rtAssessModel.CPTRecordList(ref cpt_dtl_new_items, pSDate, pEDate, ipd_no);

                    idList = cpt_dtl_new_items.Select(x => x.cpt_id).ToList();
                }

                if (!string.IsNullOrWhiteSpace(cpt_id))
                {
                    idList = cpt_id.Split(',').ToList();
                }


                if (idList.Any())
                {

                    var getData = rtAssessModel.RTAssessData(idList);

                    foreach (var detailData in getData.masterList)
                    {
                        CPTAssessViewModel addData = new CPTAssessViewModel();
                        addData.master = detailData;

                        List<RCS_CPT_DTL_NEW_ITEMS> data = getData.detailList.Where(x => x.cpt_id == detailData.CPT_ID).ToList();

                        data = rtAssessModel.changeJson(data);


                        if (data.Count > 0)
                        {
                            addData.cpt_data = data[0];
                            addData.cpt_data.record_date = changeDate.GetValue(addData.cpt_data.record_date);
                            addData.cpt_data.tube_time = changeDate.GetValue(addData.cpt_data.tube_time);
                            addData.cpt_data.shiley_time = changeDate.GetValue(addData.cpt_data.shiley_time);
                            addData.cpt_data.rt_start_time = changeDate.GetValue(addData.cpt_data.rt_start_time);
                            addData.cpt_data.abg_date = changeDate.GetValue(addData.cpt_data.abg_date);
                        }
                        else
                        {
                            addData.cpt_data = new RCS_CPT_DTL_NEW_ITEMS();
                        }

                        BaseModels baseModels = new BaseModels();
                        if (!string.IsNullOrWhiteSpace(paylaod))
                        {
                            paylaod = HttpUtility.UrlDecode(paylaod);
                            addData.user_info = Newtonsoft.Json.JsonConvert.DeserializeObject<UserInfo>(paylaod);
                        }
                        IPDPatientInfo patientInfo = baseModels.SelectPatientInfo(detailData.IPD_NO, detailData.CHART_NO);
                        patientInfo.diag_date = changeDate.GetValue(patientInfo.diag_date);

                        addData.pat_info = patientInfo;

                        ViewModel.Add(addData);
                    }
                }
                else
                {
                    CPTAssessViewModel addNewData = new CPTAssessViewModel();
                    addNewData.cpt_data = new RCS_CPT_DTL_NEW_ITEMS();
                    addNewData.cpt_data.cpt_id = "";
                    ViewModel.Add(addNewData);
                }

            }
            catch (Exception ex)
            {
                LogTool.SaveLogMessage(ex, "cpt_page");
            }
            return View(new ListCPTAssessViewModel() { List = ViewModel });
        }

        /// <summary>
        /// CPTPageForm
        /// </summary>
        /// <returns></returns> 
        public ActionResult OPDShiftForm(string paylaod, string chart_no, string ipd_no, string pSDate = "", string pEDate = "")
        {
            changeDateCHT<string> changeDate = new changeDateCHT<string>();
            OpdShift_MainTable_PDF ViewModel = new OpdShift_MainTable_PDF();
            try
            {

                RtOPDShift rtOPDShift = new RtOPDShift();

                List<string> idList = new List<string>();

                List<RCS_RT_OPD_SHIFT_DETAIL> getList = new List<RCS_RT_OPD_SHIFT_DETAIL>();

                if (!string.IsNullOrWhiteSpace(pSDate) && !string.IsNullOrWhiteSpace(pEDate))
                {
                    getList = rtOPDShift.getIndexOPDshift(pSDate, pEDate, chart_no);

                    idList = getList.Select(x => x.OS_ID).ToList();
                }


                if (idList.Any())
                {
                    ViewModel.OpdShiftDetail_rList = getList.OrderBy(x => x.opt_Shift_date).ToList(); ;
                    ViewModel.OpdShiftDetail_rList = ViewModel.OpdShiftDetail_rList.OrderBy(x => x.opt_Shift_date).ToList();
                    if (ViewModel.OpdShiftDetail_rList != null && ViewModel.OpdShiftDetail_rList.Count % 6 > 0)
                    {
                        for (int ii = 0; ii < ViewModel.OpdShiftDetail_rList.Count % 6; ii++)
                        {
                            ViewModel.OpdShiftDetail_rList.Add(new RCS_RT_OPD_SHIFT_DETAIL());
                        }
                    }
                    List<int> CAT_cnt = new List<int>();
                    foreach (RCS_RT_OPD_SHIFT_DETAIL temp_CAT in ViewModel.OpdShiftDetail_rList)
                    {
                        if (!string.IsNullOrWhiteSpace(temp_CAT.CAT_1) && temp_CAT.CAT_1.Length > 0)
                        {
                            CAT_cnt.Add(Int32.Parse(temp_CAT.CAT_1));
                        }
                        if (!string.IsNullOrWhiteSpace(temp_CAT.CAT_2) && temp_CAT.CAT_2.Length > 0)
                        {
                            CAT_cnt.Add(Int32.Parse(temp_CAT.CAT_2));
                        }
                        if (!string.IsNullOrWhiteSpace(temp_CAT.CAT_3) && temp_CAT.CAT_3.Length > 0)
                        {
                            CAT_cnt.Add(Int32.Parse(temp_CAT.CAT_3));
                        }
                        if (!string.IsNullOrWhiteSpace(temp_CAT.CAT_4) && temp_CAT.CAT_4.Length > 0)
                        {
                            CAT_cnt.Add(Int32.Parse(temp_CAT.CAT_4));
                        }
                        if (!string.IsNullOrWhiteSpace(temp_CAT.CAT_5) && temp_CAT.CAT_5.Length > 0)
                        {
                            CAT_cnt.Add(Int32.Parse(temp_CAT.CAT_5));
                        }
                        if (!string.IsNullOrWhiteSpace(temp_CAT.CAT_6) && temp_CAT.CAT_6.Length > 0)
                        {
                            CAT_cnt.Add(Int32.Parse(temp_CAT.CAT_6));
                        }
                        if (!string.IsNullOrWhiteSpace(temp_CAT.CAT_7) && temp_CAT.CAT_7.Length > 0)
                        {
                            CAT_cnt.Add(Int32.Parse(temp_CAT.CAT_6));
                        }
                        if (!string.IsNullOrWhiteSpace(temp_CAT.CAT_8) && temp_CAT.CAT_8.Length > 0)
                        {
                            CAT_cnt.Add(Int32.Parse(temp_CAT.CAT_8));
                        }
                        temp_CAT.CAT_Total = CAT_cnt.Sum().ToString();
                        CAT_cnt = new List<int>();
                    }

                    BaseModels baseModels = new BaseModels();
                    IPDPatientInfo patientInfo = baseModels.SelectPatientInfo(ipd_no, chart_no);
                    patientInfo.diag_date = changeDate.GetValue(patientInfo.diag_date);
                    ViewModel.PatientInformation = patientInfo;

                }
                else
                {
                    if (ViewModel.OpdShiftDetail_rList != null && ViewModel.OpdShiftDetail_rList.Count % 6 > 0)
                    {
                        for (int ii = 0; ii < ViewModel.OpdShiftDetail_rList.Count % 6; ii++)
                        {
                            ViewModel.OpdShiftDetail_rList.Add(new RCS_RT_OPD_SHIFT_DETAIL());
                        }
                    }
                    ViewModel.PatientInformation = new IPDPatientInfo();

                }

            }
            catch (Exception ex)
            {
                LogTool.SaveLogMessage(ex, "OPDShiftForm");
            }
            return View("~/Views/PDFHtml/OpdShiftMainTable_Pdf.cshtml", ViewModel);
        }

        public ActionResult rtCPTReqPageForm(string record_id, string paylaod, string chart_no, string ipd_no)
        {
            changeDateCHT<string> changeDate = new changeDateCHT<string>();
            List<printRtCptReq> ViewModel = new List<printRtCptReq>();
            try
            {

                RCS_Data.Controllers.RtCptReq.Models RtCptReq = new RCS_Data.Controllers.RtCptReq.Models();

                List<string> idList = new List<string>();

                if (!string.IsNullOrWhiteSpace(record_id))
                {
                    idList = record_id.Split(',').ToList();

                    var getData = RtCptReq.RtCptReqListByID<RTCptReq>(idList);

                    // detail
                    List<RTCptReq> data = getData;

                    foreach (var detailData in data)
                    {
                        printRtCptReq addData = new printRtCptReq();


                        paylaod = HttpUtility.UrlDecode(paylaod);
                        IPDPatientInfo patientInfo = RtCptReq.SelectPatientInfo(ipd_no, chart_no);

                        addData.data = detailData;
                        addData.data.rec_date = changeDate.GetValue(addData.data.rec_date);
                        addData.data.X_Ray_date = changeDate.GetValue(addData.data.X_Ray_date);
                        addData.data.triflow_date = changeDate.GetValue(addData.data.triflow_date);
                        addData.data.String_date = changeDate.GetValue(addData.data.String_date);
                        addData.data.End_date = changeDate.GetValue(addData.data.End_date);

                        addData.user_info = Newtonsoft.Json.JsonConvert.DeserializeObject<UserInfo>(paylaod);

                        addData.pat_info = patientInfo;

                        ViewModel.Add(addData);
                    }

                }
                else
                {
                    printRtCptReq addData = new printRtCptReq();
                    addData.data = new RTCptReq();

                    ViewModel.Add(addData);
                }



            }
            catch (Exception ex)
            {
                LogTool.SaveLogMessage(ex, "cpt_page");
            }
            //_FJUCPTPageForm
            return View("~/Views/RT/exportFile/BASIC/_CPTReqForm.cshtml", new ListprintRtCptReq() { List = ViewModel });
        }

        /// <summary>
        /// rtCPTAssPageForm
        /// </summary>
        /// <returns></returns> 
        public ActionResult rtCPTAssPageForm(string cpt_id, string paylaod, string chart_no, string ipd_no, string sDate = "", string eDate = "")
        {
            changeDateCHT<string> changeDate = new changeDateCHT<string>();
            List<printCPTNewData> ViewModel = new List<printCPTNewData>(); 
            try
            {

                RtCptAssess RtCptAss = new RtCptAssess();

                List<string> idList = new List<string>();

                if (!string.IsNullOrWhiteSpace(sDate) && !string.IsNullOrWhiteSpace(eDate))
                {
                    List<CPTNewRecord> Result = new List<CPTNewRecord>();

                    IPDPatientInfo pat_info = new IPDPatientInfo() { ipd_no = ipd_no, chart_no = chart_no };
                    var rm = RtCptAss.RtCptRecordList(ref Result, sDate, eDate, pat_info);

                    idList = Result.Select(x => x.cpt_id).ToList();
                }
                if (!string.IsNullOrWhiteSpace(cpt_id))
                {
                    idList = cpt_id.Split(',').ToList();
                }


                if (idList.Any())
                {

                    var getData = RtCptAss.RtCptRecordListByID(idList);

                    foreach (var detailData in getData)
                    {
                        printCPTNewData addData = new printCPTNewData();
                        addData.data = detailData;
                        addData.data.rec_date = changeDate.GetValue(addData.data.rec_date);
                        addData.data.rec_date_next = changeDate.GetValue(addData.data.rec_date_next);
                        paylaod = HttpUtility.UrlDecode(paylaod);
                        IPDPatientInfo patientInfo = RtCptAss.SelectPatientInfo(ipd_no, chart_no);


                        addData.user_info = Newtonsoft.Json.JsonConvert.DeserializeObject<UserInfo>(paylaod);

                        addData.pat_info = patientInfo;
                        ViewModel.Add(addData);
                    }

                }
                else
                {
                    printCPTNewData addNewData = new printCPTNewData();
                    addNewData.data = new CPTNewRecord();
                    ViewModel.Add(addNewData);
                }

            }
            catch (Exception ex)
            {
                LogTool.SaveLogMessage(ex, "cpt_page");
            }
            //_FJUCPTPageForm
            return View(new ListprintCPTNewData() { List = ViewModel });
        }

        /// <summary>
        /// CPTRecord
        /// </summary>
        /// <returns></returns> 
        public ActionResult rtCPTRecordPageForm(string sDate, string eDate, string id, string paylaod, string chart_no, string ipd_no, bool isEmpty = false)
        {
            changeDateCHT<string> changeDate = new changeDateCHT<string>();
            List<CPTNewRecord> table = new List<CPTNewRecord>();

            printCPTNewRecordData model = new printCPTNewRecordData();
            RtCptRecord RtCptRecord = new RtCptRecord();

            IPDPatientInfo patientInfo = RtCptRecord.SelectPatientInfo(ipd_no, chart_no);

            model.data = new List<List<CPTNewRecord>>();
            if (!isEmpty)
            {
                var rm = RtCptRecord.RtCptRecordList(ref table, sDate, eDate, patientInfo, id: id);
                table = table.OrderBy(x => DateTime.Parse(x.rec_date)).ToList();
                table = RtCptRecord.changeData(table);
            }

            for (int j = 0; j < table.Count() / 5 + 1; j++)
            {
                List<CPTNewRecord> addData = new List<CPTNewRecord>();
                for (int x = j * 5; x < j * 5 + 5; x++)
                {
                    if (table.Count() > x)
                    {
                        addData.Add(table[x]);
                    }
                    else
                    {
                        addData.Add(new CPTNewRecord());
                    }

                }

                foreach (var item in addData)
                {
                    item.rec_date = changeDate.GetValue(item.rec_date);
                }
                model.data.Add(addData);
            }

            if (!string.IsNullOrWhiteSpace(paylaod))
            {
                paylaod = HttpUtility.UrlDecode(paylaod);

                model.user_info = Newtonsoft.Json.JsonConvert.DeserializeObject<UserInfo>(paylaod);
            }
            else
            {
                model.user_info = new UserInfo();
            } 
            model.pat_info = patientInfo;


            return View(model);
        }

        public ActionResult rtVentilatorStatusPageForm(string record_id, string paylaod, string chart_no, string ipd_no)
        {
            VentilatorStatusPrintData ViewModel = new VentilatorStatusPrintData();
            ViewModel.All = new List<RCS_Data.Controllers.RtValuation.DB_RCS_VALUATION_DTL>();
            RtCptRecord RtCptRecord = new RtCptRecord();
            IPDPatientInfo patientInfo = RtCptRecord.SelectPatientInfo(ipd_no, chart_no);
            try
            {

                RCS_Data.Controllers.RtValuation.Models RtValuation = new RCS_Data.Controllers.RtValuation.Models();


                if (!string.IsNullOrWhiteSpace(record_id))
                {

                    var getData = RtValuation.EditConsumableList(record_id);

                    ViewModel.V_ID = getData.V_ID;
                    ViewModel.RECORD_DATE = getData.RECORD_DATE;
                    ViewModel.CONSUMABLE_LIST = getData.CONSUMABLE_LIST;
                    ViewModel.All.AddRange(getData.D);
                    ViewModel.All.AddRange(getData.E);
                    ViewModel.All.AddRange(getData.N);
                }

                paylaod = HttpUtility.UrlDecode(paylaod);

                ViewModel.user_info = Newtonsoft.Json.JsonConvert.DeserializeObject<UserInfo>(paylaod);

                ViewModel.pat_info = patientInfo;

                //foreach (var item in ViewModel.All) {

                //    if (item.ITEM_VALUE == "0") {
                //        item.ITEM_VALUE = "";
                //    }
                //}

            }
            catch (Exception ex)
            {
                LogTool.SaveLogMessage(ex, "cpt_page");
            }
            //_FJUCPTPageForm
            return View("~/Views/RT/exportFile/BASIC/_ValuationForm.cshtml", ViewModel);
        }
        public ActionResult weekindex_print(string weakenSDate, string weakenEDate, int weanType, bool isCopy, int copysetCnt, bool isList = false)
        {
            string actionName = "weekindex_print";
            RESPONSE_MSG rm = new RESPONSE_MSG();
            SQLProvider SQL = new SQLProvider();
            OPDScheduling OPDScheduling = new OPDScheduling();
            try
            {
                JsonResult cpt_history_new_items = Json(OPDScheduling.week_data(weakenSDate, weakenEDate, weanType, isCopy, copysetCnt, isList = false));
                ViewBag.OPD_TreatmentNode = cpt_history_new_items.Data;

            }
            catch (Exception ex)
            {
                SQL.DBA.Rollback();
                rm.status = RESPONSE_STATUS.EXCEPTION;
                rm.message = "列印失敗!程式碼發生錯誤!";
                LogTool.SaveLogMessage(ex, actionName, this.csName);
            }
            return View("~/Views/OPDScheduling/OPDScheduling_WeekIndex_Pdf.cshtml");
        }

        public ActionResult week_print(string weakenSDate, string weakenEDate, int weanType, bool isCopy, int copysetCnt, bool isList = false)
        {
            string actionName = "week_print";
            RESPONSE_MSG rm = new RESPONSE_MSG();
            SQLProvider SQL = new SQLProvider();
            OPD_TREATMENT_VM vm = new OPD_TREATMENT_VM();
            OPDScheduling OPDScheduling = new OPDScheduling();
            try
            {
                JsonResult cpt_history_new_items = Json(OPDScheduling.week_data(weakenSDate, weakenEDate, weanType, isCopy, copysetCnt, isList = false));
                ViewBag.OPD_TreatmentNode = cpt_history_new_items.Data;
            }
            catch (Exception ex)
            {
                SQL.DBA.Rollback();
                rm.status = RESPONSE_STATUS.EXCEPTION;
                rm.message = "列印失敗!程式碼發生錯誤!";
                LogTool.SaveLogMessage(ex, actionName, this.csName);
            }
            return View("~/Views/OPDScheduling/OPDScheduling_Index_Pdf.cshtml");
        }

        /// <summary>
        /// RtTakeOffPageForm
        /// </summary>
        /// <returns></returns> 
        public ActionResult RtTakeOffPageForm(string tk_id, string paylaod, string chart_no, string ipd_no, string pSDate = "", string pEDate = "")
        {
            changeDateCHT<string> changeDate = new changeDateCHT<string>();
            List<RTTakeoffAssessViewModel> ViewModel = new List<RTTakeoffAssessViewModel>();
            try
            {
                RTTakeoff RtTakeoffModel = new RTTakeoff();
                List<VM_RTTakeoffAssess> rttakeoff_db_items = new List<VM_RTTakeoffAssess>();
                List<string> HasIdDataList = new List<string>();
                if (!string.IsNullOrWhiteSpace(pSDate) && !string.IsNullOrWhiteSpace(pEDate))
                { 
                    // 病人資料
                    IPDPatientInfo pat_info = new IPDPatientInfo() { ipd_no = ipd_no, chart_no = chart_no };
                    var rm = RtTakeoffModel.RtTakeOffList(ref rttakeoff_db_items, pSDate, pEDate, pat_info, "", null);

                    HasIdDataList = rttakeoff_db_items.Select(x => x.tk_id).ToList();
                }
                if (!string.IsNullOrWhiteSpace(tk_id))
                {
                    HasIdDataList = tk_id.Split(',').ToList();
                }
                if (HasIdDataList.Any())
                {
                    var getData = RtTakeoffModel.RtTakeoffData(HasIdDataList);
                    getData.masterList = getData.masterList.OrderBy(x => x.REC_DATE).ToList();
                    foreach (var dtlData in getData.masterList)
                    {
                        RTTakeoffAssessViewModel addData = new RTTakeoffAssessViewModel();

                        // 使用者資料
                        if (!string.IsNullOrWhiteSpace(paylaod))
                        {
                            paylaod = HttpUtility.UrlDecode(paylaod);
                            addData.user_info = Newtonsoft.Json.JsonConvert.DeserializeObject<UserInfo>(paylaod);
                        }
                        // 病人基本資料
                        IPDPatientInfo patientInfo = RtTakeoffModel.SelectPatientInfo(ipd_no, chart_no);
                        addData.pat_info = patientInfo;
                        addData.pat_info.diag_date = changeDate.GetValue(addData.pat_info.diag_date);

                        // master
                        addData.master = dtlData;

                        // detail
                        List<VM_RTTakeoffAssess> data = getData.detailList.Where(x => x.tk_id == dtlData.TK_ID).ToList();
                        data = RtTakeoffModel.changeJson(data);

                        if (data.Count > 0)
                        {
                            addData.DTL = data[0];
                            addData.DTL.rec_date = changeDate.GetValue(addData.DTL.rec_date);
                            addData.DTL.on_breath_date = changeDate.GetValue(addData.DTL.on_breath_date);
                        }

                        if (!String.IsNullOrWhiteSpace(addData.DTL.weaningTable_data))
                        {
                            var getWeaningData = this.DBLink.Select_JSONData<DB_RCS_RT_RECORD_JSON>(addData.DTL.weaningTable_data);

                            if (getWeaningData.Any())
                            {
                                addData.WeaningProfile_List = Newtonsoft.Json.JsonConvert.DeserializeObject<List<RCS_WEANING_ITEM>>(getWeaningData.First().JSON_VALUE);
                            }

                            foreach (var WP_item in addData.WeaningProfile_List)
                            {
                                WP_item.date = changeDate.GetValue(WP_item.date);
                            }
                        }

                        ViewModel.Add(addData);
                    }//foreach (var dtlData in getData.masterList)
                }//if (!string.IsNullOrWhiteSpace(tk_id))
                else
                {
                    RTTakeoffAssessViewModel addNewData = new RTTakeoffAssessViewModel();
                    addNewData.DTL = new VM_RTTakeoffAssess();
                    addNewData.DTL.tk_id = "";
                    ViewModel.Add(addNewData);
                }
            }
            catch (Exception ex)
            {
                LogTool.SaveLogMessage(ex, "RtTakeOff_page");
            }
            //_FJUCPTPageForm
            return View(new ListRTTakeoffAssessViewModel() { List = ViewModel });
        }

        /// <summary>
        /// RTRecordPageForm
        /// </summary>
        /// <returns></returns> 
        public ActionResult RTRecordPageForm(string record_id, string paylaod, string chart_no, string ipd_no, string sDate, string eDate, bool isEmpty = false)
        {
            ViewData["isEmptyPDF"] = isEmpty;
            rtRecordPDFViewModel vm = new rtRecordPDFViewModel();
            List<RT_RECORD_MAIN> rtrecord_items = new List<RT_RECORD_MAIN>();
            RtRecord rtRecordModel = new RtRecord();
            BaseModels base_models = new BaseModels();
            IPDPatientInfo patientInfo = new IPDPatientInfo();
            changeDateCHT<string> changeDate = new changeDateCHT<string>();
            string actionName = "RTRecordPageForm";
            try
            {
                if (!string.IsNullOrWhiteSpace(paylaod))
                {
                    paylaod = HttpUtility.UrlDecode(paylaod);
                    UserInfo user_info = Newtonsoft.Json.JsonConvert.DeserializeObject<UserInfo>(paylaod);
                }
                patientInfo = rtRecordModel.SelectPatientInfo(ipd_no, chart_no);
                ViewData["patientInfo"] = patientInfo;
                if (!isEmpty)
                {
                    RESPONSE_MSG rm = rtRecordModel.RtRecordListData(ref rtrecord_items, sDate, eDate, patientInfo, "", record_id, true);
                    rtRecordModel.bindpdfModel(ref rtrecord_items, ipd_no, chart_no);
                }
                if (rtrecord_items.Count() == 0)
                {
                    for (int i = 0; i < 5; i++)
                    {
                        rtrecord_items.Add(new RT_RECORD_MAIN());
                    }
                }
                vm.data = rtrecord_items;
                foreach (var vm_item in vm.data)
                {
                    vm_item.RECORDDATE = changeDate.GetValue(vm_item.RECORDDATE);
                    vm_item.rt_record.recorddate = changeDate.GetValue(vm_item.rt_record.recorddate);
                }
            }
            catch (Exception ex)
            {
                LogTool.SaveLogMessage(ex, actionName, this.csName);
            }
            //RTRecordPageForm
            return View(vm);
        }

        public ActionResult RCSPageForm(string form_id)
        {
            string actionName = "DownloadPdf";
            string HtmlStr = "";
            exportFile efm = new exportFile(form_id + "Pdf.pdf");
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

        /// <summary>
        /// 匯出PDFPage
        /// </summary>
        /// <param name="form_id"></param>
        /// <returns></returns>
        public ActionResult PDFPage(string form_name, string form_id, string chart_no, string ipd_no, string parameter)
        {
            string actionName = "PDFPage";
            string HtmlStr = "", url = "";
            List<byte[]> byteList = new List<byte[]>();
            string getUrl = IniFile.GetConfig("System", "PDFURL");
            exportFile efm = new exportFile(form_id + "Pdf.pdf");
            IDocSetting ds = new RTRecordFormPDFDocSetting();
            try
            {
                if (!string.IsNullOrWhiteSpace(parameter))
                {
                    parameter = RCS_Data.Models.Function_Library.DecodeDES(parameter);
                }
                efm.FileDownloadName = "";//列印表單名稱
                switch (form_id)
                {
                    case "RTRecordPageForm":
                        efm.FileDownloadName = form_name;//列印表單名稱
                        ds = new CPTNewRecordFormPDFDocSetting();
                        url = string.Concat(getUrl, form_id, string.Format("?chart_no={0}&ipd_no={1}{2}",
                                chart_no, ipd_no, parameter));
                        break;
                    case "CPTPageForm":
                        efm.FileDownloadName = form_name;//列印表單名稱
                        ds = new CPTNewRecordFormPDFDocSetting();
                        url = string.Concat(getUrl, form_id, string.Format("?chart_no={0}&ipd_no={1}{2}",
                                chart_no, ipd_no, parameter));
                        break;
                    case "RtTakeOffPageForm":
                        efm.FileDownloadName = form_name;//列印表單名稱
                        ds = new CPTNewRecordFormPDFDocSetting();
                        url = string.Concat(getUrl, form_id, string.Format("?chart_no={0}&ipd_no={1}{2}",
                                chart_no, ipd_no, parameter));
                        break;
                    default:
                        var resp = new System.Net.Http.HttpResponseMessage(System.Net.HttpStatusCode.NotFound)
                        {
                            Content = new System.Net.Http.StringContent(string.Concat("form_id沒有資料，無法懺生列印畫面，請洽資訊人員! [發生時間:", RCS_Data.Models.Function_Library.getDateNowString(RCS_Data.Models.DATE_FORMAT.yyyy_MM_ddTHHmmss), "]"))
                        };
                        throw new System.Web.Http.HttpResponseException(resp);
                        break;
                }

                using (System.Net.WebClient client = new System.Net.WebClient())
                {
                    client.Encoding = Encoding.UTF8; // 設定Webclient.Encoding
                    HtmlStr = client.DownloadString(url);
                }
            }
            catch (Exception ex)
            {
                LogTool.SaveLogMessage(ex, actionName, this.csName);
                var resp = new System.Net.Http.HttpResponseMessage(System.Net.HttpStatusCode.NotFound)
                {
                    Content = new System.Net.Http.StringContent(string.Concat("程式發生錯誤，請洽資訊人員! [發生時間:", RCS_Data.Models.Function_Library.getDateNowString(RCS_Data.Models.DATE_FORMAT.yyyy_MM_ddTHHmmss), "]"))
                };
                throw new System.Web.Http.HttpResponseException(resp);
            }
            return efm.exportPDF(HtmlStr, ds);
        }

        public class changeDateCHT<T>
        {
            private T changedata;

            public T GetValue(T data)
            {
                return data;
                if (data is string)
                {
                    DateTime result = DateTime.Parse(Convert.ToString(data)).AddYears(-1911);
                    if (Convert.ToString(data).Split(' ').Length > 1)
                    {
                        changedata = (T)Convert.ChangeType(result.ToString("yyy-MM-dd HH:mm"), typeof(T));
                    }
                    else
                    {
                        changedata = (T)Convert.ChangeType(result.ToString("yyy-MM-dd"), typeof(T));
                    }

                    return changedata;
                }

                if (data is DateTime)
                {
                    DateTime result = Convert.ToDateTime(data).AddYears(-1911);
                    changedata = (T)Convert.ChangeType(result, typeof(T));
                    return changedata;
                }

                if (string.IsNullOrWhiteSpace(Convert.ToString(data)))
                {
                    changedata = data;
                }

                return changedata;
            }
        }
    }

    public class AllowCrossSiteAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            filterContext.RequestContext.HttpContext.Response.AddHeader("Access-Control-Allow-Origin", "*");
            filterContext.RequestContext.HttpContext.Response.AddHeader("Access-Control-Allow-Headers", "*");
            filterContext.RequestContext.HttpContext.Response.AddHeader("Access-Control-Allow-Methods", "*");
            filterContext.RequestContext.HttpContext.Response.AddHeader("Access-Control-Allow-Credentials", "true");
            filterContext.RequestContext.HttpContext.Response.AddHeader("content-type", "application/x-www-form-urlencoded");
            filterContext.RequestContext.HttpContext.Response.AddHeader("Access-Control-Max-Age", "86400");
            base.OnActionExecuting(filterContext);
        }
    }
}
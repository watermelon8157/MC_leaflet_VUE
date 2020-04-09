using Com.Mayaminer;
using Newtonsoft.Json;
using RCS.Models;
using RCS_Data;
using RCSData.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using RCS_Data.Models.VIP;
using RCS_Data.Models.ViewModels;

namespace RCS.Controllers
{
    public class VIPController : Controller
    {
        //
        // GET: /VIP/
        string csName { get { return "VIPController"; } }
        WebMethod _WebMethod;
        public WebMethod WebMethod
        {
            get
            {
                if (_WebMethod == null)
                {
                    _WebMethod = new WebMethod();
                }
                return _WebMethod;
            }
        }

        private Models.HOSP.HospFactory _hospFactory { get; set; }
        protected Models.HOSP.HospFactory hospFactory
        {
            get
            {
                if (this._hospFactory == null)
                {
                    this._hospFactory = new Models.HOSP.HospFactory();
                }
                return this._hospFactory;
            }
        }

        /// <summary>
        /// VIP2記錄單畫面
        /// </summary>
        /// <param name="patient_id"></param>
        /// <returns></returns>
        public ActionResult Index(string patient_id)
        {
            string actionName = "Index";
            VIPViewModel vm = new VIPViewModel();
            vm.viewPage = actionName;
            string msg = setViewModel(patient_id, actionName, ref vm);
            if (string.IsNullOrWhiteSpace(msg))
            {
                // 院內所有呼吸器編號
                RESP_COLLECTION resplist = new RTRecord().getVENTILATORList("", "");
                vm.resplist = resplist;
                SysParamCollection sys_params_list = new SysParamCollection();
                BaseModel BaseModel = new BaseModel();
                sys_params_list.append_modal(BaseModel.GetModelListCollection("Index_device"));
                sys_params_list.append_modal(BaseModel.GetModelListCollection("RTRecord_Detail"));
                sys_params_list.append_modal(BaseModel.GetModelListCollection("Shared"));
                vm.sys_params_list = sys_params_list;
                return View(vm);
            }
            Response.Write(string.Concat("<h2>", msg, "</h2>"));
            return new EmptyResult();            
        }

        /// <summary>
        /// VIP2病患歷史記錄單
        /// </summary>
        /// <param name="patient_id"></param>
        /// <returns></returns>
        public ActionResult List(string patient_id)
        {
            string actionName = "List";
            VIPViewModel vm = new VIPViewModel();
            string msg = setViewModel(patient_id, actionName,ref vm);
            if (string.IsNullOrWhiteSpace(msg))
            {
                return View(vm);
            }
            Response.Write(string.Concat("<h2>", msg, "</h2>"));
            return new EmptyResult();
        }

        /// <summary>
        /// 設定view要顯示的物件
        /// </summary>
        /// <param name="patient_id"></param>
        /// <param name="action"></param>
        /// <param name="vm"></param>
        /// <returns></returns>
        private string setViewModel(string patient_id,string action,ref VIPViewModel vm)
        {
            string actionName = "setViewModel";
            string msg = "";
            try
            {
                if (!string.IsNullOrWhiteSpace(patient_id))
                {
                    VIPRTTBL VIPRTTBL = new VIPRTTBL();
                    if (VIPRTTBL.getVIPRTTBL_count(2, patient_id) > 0)
                    {
                        vm.viewPage = action;
                        vm.patient_id = patient_id;
                    }
                    else
                    {
                        msg = string.Concat("查無病歷號(", patient_id, ")的上傳資料，請確認病例號是否正確!");
                    }
                }
                else
                {
                    msg = "設備並未提供病歷號，無法開啟畫面，請洽資訊人員!";
                }
            }
            catch (Exception ex)
            {
                LogTool.SaveLogMessage(ex, actionName, csName);
                msg = "程式發生錯誤，請洽資訊人員!";
            }
            return msg;
        }

        /// <summary>呼吸器狀態(新版結構) </summary>
        /// <param name="before_day">取得幾天前</param>
        /// <returns>RT_RECORD_MAIN[]</returns>
        [HttpPost]
        public string FormStatusData(string patient_id, int before_day = 2)
        {
            List<RT_RECORD_MAIN> rt_record_main_list = new List<RT_RECORD_MAIN>();
            try
            {
                VIPRTTBL VIPRTTBL = new VIPRTTBL();
                VIPRTTBL.getRt_Record_Main_List(before_day, patient_id);
                return JsonConvert.SerializeObject(VIPRTTBL.rt_record_main_list);
            }
            catch (Exception ex)
            {
                LogTool.SaveLogMessage(ex, "FormStatusData", csName);
            }
            return JsonConvert.SerializeObject(rt_record_main_list);
        }

        /// <summary>
        /// 取得三日內ABG資料
        /// </summary>
        /// <param name="chart_no"></param>
        /// <returns></returns>
        public string GetAbgList(string chart_no)
        {
            string ipd_no = "";
            string actionName = "GetAbgList";
            List<ExamABG> data = new List<ExamABG>();
            try
            {
                List<PatientHistory> PatientHistory = WebMethod.getPatientHistory(this.hospFactory.webService.HISPatientHistory(), chart_no, ref ipd_no);
                data = WebMethod.getAVBGData(this.hospFactory.webService.HISABGLabData(), chart_no, ipd_no); 
            }
            catch (Exception ex)
            {
                LogTool.SaveLogMessage(ex, actionName, csName);
            }
            return JsonConvert.SerializeObject(data);
        }

        /// <summary>取得片語資烙</summary>
        /// <returns></returns>
        [HttpPost]
        public JsonResult GetPhraseData(string type)
        {
            List<SysParams> RTRecordAllList = new BaseModel().getRCS_SYS_PARAMS("RTRecord_Detail", "");
            if (!string.IsNullOrWhiteSpace(type))
            {
                return Json(new RTRecord().GetGroupList(RTRecordAllList, type));
            }
            else
            {
                return Json("");
            }
        }

        /// <summary>
        /// 取得最後一筆VIP上傳資料
        /// </summary>
        /// <param name="rowStr"></param>
        /// <param name="setIpdno"></param>
        /// <param name="target"></param>
        /// <returns></returns>
        public JsonResult getThisRecordData(string patient_id)
        {
            RESPONSE_MSG rm = new RESPONSE_MSG();
            RT_RECORD_MAIN data = new RT_RECORD_MAIN();
            string actionName = "getThisRecordData";
            rm.attachment = data;
            try
            {
                if (!string.IsNullOrWhiteSpace(patient_id))
                {
                    VIPRTTBL VIPRTTBL = new VIPRTTBL();
                    VIPRTTBL.getRt_Record_Main_List(1, patient_id,true);
                    if (VIPRTTBL.RESPONSE_MSG.status == RESPONSE_STATUS.SUCCESS)
                    {
                        if (VIPRTTBL.rt_record_main_list != null && VIPRTTBL.rt_record_main_list.Count > 0)
                        {
                            data = VIPRTTBL.rt_record_main_list[0];
                        }
                        rm.attachment = data.rt_record;
                    }
                    else
                    {
                        rm.status = RESPONSE_STATUS.ERROR;
                        rm.message = VIPRTTBL.RESPONSE_MSG.message;
                    }
                }
                else
                {
                    rm.status = RESPONSE_STATUS.ERROR;
                    rm.message = "沒有病歷號，無法帶入呼吸狀態資料!";
                }
            }
            catch (Exception ex)
            {
                rm.status = RESPONSE_STATUS.EXCEPTION;
                rm.message = "程式發生錯誤，請洽資訊人員!";
                LogTool.SaveLogMessage(ex, actionName, csName);
            }

            return Json(rm);
        }
    }
}

using Com.Mayaminer;
using RCS.Models;
using RCS.Models.ViewModel;
using RCS_Data;
using RCS_Data.Controllers.RtRecord;
using RCS_Data.Models.DB;
using RCS_Data.Models.ViewModels;
using RCSData.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web.Http;

namespace RCS.Controllers.WEBAPI
{
    public class RTRecordController : BasicController, IRtRecordController
    {
        private RT_RECORD RTRecordModel = new RT_RECORD(); 
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

        RCS.Models.ViewModel.Upload _uploadmodel;
        RCS.Models.ViewModel.Upload UploadModel
        {
            get
            {
                if (_uploadmodel == null)
                {
                    this._uploadmodel = new RCS.Models.ViewModel.Upload();
                }
                return this._uploadmodel;
            }
        }

        /// <summary>
        /// 取得畫面下拉選單
        /// </summary>
        /// <param name="form"></param>
        /// <returns></returns>
        [HttpPost]
        [JwtAuthActionFilter]
        public object SelectListItem(Form_SelectListItem form)
        {
            SelectItemDetail result = new SelectItemDetail();

            result.conscious_e = this.select.getConsciousEVM("E");
            result.conscious_v = this.select.getConsciousEVM("V");
            result.conscious_m = this.select.getConsciousEVM("M");
            result.devicelist = new SystemController().getDevice("");
            result.device_O2DLL = this.select.getdevice_O2DLL();
            return result;
        }

        /// <summary>
        /// 取得檢驗資料
        /// </summary>
        /// <param name="form"></param>
        /// <returns></returns>
        [HttpPost]
        [JwtAuthActionFilter]
        public List<ExamBloodBiochemical> BloodBiochemicalData(FormBody form)
        {  
            return (List<ExamBloodBiochemical>)this.returnObj(this.rtRecordModel.getBloodBiochemicalData(form.pat_info, form.user_info, this.hospFactory.webService.HISBloodBiochemicalLabData())); 
        }

        /// <summary>
        /// 取得呼吸照護記錄
        /// </summary>
        /// <param name="form"></param>
        /// <returns></returns>
        [HttpPost]
        [JwtAuthActionFilter]
        public List<BREATH_LIST> RtRrecordList(FormBody form)
        {
            return (List<BREATH_LIST>)this.returnObj(this.rtRecordModel.getBreathList(form.pat_info, form.user_info ));
        }
                
        /// <summary>
        /// 取得呼吸照護記錄List清單
        /// </summary>
        /// <param name="form"></param>
        /// <returns></returns>
        [HttpPost]
        [JwtAuthActionFilter]
        public List<RT_RECORD_MAIN> getRtRrecordList(Form_RtRecordList form)
        {
            RESPONSE_MSG rm = new RESPONSE_MSG();
            List<RT_RECORD_MAIN> rtrecord_items = new List<RT_RECORD_MAIN>();

            UserInfoBasic user_info = form.user_info;
            IPDPatientInfo pat_info = form.pat_info;
            rm = rtRecordModel.RtRecordListData(ref rtrecord_items, form.pSDate, form.pEDate, pat_info, form.pId,form.record_id  );

            return rtrecord_items.OrderByDescending(x => x.RECORDDATE).ToList();
        }

        /// <summary>取得三天內的Abg資料</summary>
        /// <returns></returns>
        [HttpPost]
        [JwtAuthActionFilter]
        public List<CPTNewRecord> LastMonitorList(FormBody form)
        { 
            return (List<CPTNewRecord>)this.returnObj(this.rtRecordModel.getMonitor(form.pat_info, form.user_info));
        }

        /// <summary>取得三天內的Abg資料</summary>
        /// <returns></returns>
        [HttpPost]
        [JwtAuthActionFilter]
        public List<ExamABG> LastAbgList(FormBody form)
        {
            return (List<ExamABG>)this.returnObj(this.rtRecordModel.GetLastAbgList(form.pat_info, form.user_info, this.hospFactory.webService.HISABGLabData()));
        }

        /// <summary>取得片語資料</summary>
        /// <returns></returns>
        [HttpPost]
        [JwtAuthActionFilter]
        public List<DB_RCS_SYS_PARAMS> PhraseData(FormBody form)
        {
            return (List<DB_RCS_SYS_PARAMS>)this.returnObj(this.rtRecordModel.GetPhraseData(form.type));
        }


        /// <summary>取得新呼吸照護紀錄單</summary>
        /// <returns></returns>
        [HttpPost]
        [JwtAuthActionFilter]
        public RT_RECORD_DATA<RT_RECORD> RTRecordNewData(FormBodyRT_RECORD_VIEW form)
        {
            RT_RECORD_DATA<RT_RECORD> vm = (RT_RECORD_DATA<RT_RECORD>)this.returnObj(this.rtRecordModel.GetRTRecordNewData(form.isVIP, form.pat_info, form.user_info, form.model));
            Ventilator lastRecord = this.basicfunction.GetLastRTRec(form.pat_info.chart_no, form.pat_info.ipd_no);
            vm.last_record_id = lastRecord.RECORD_ID;
            if (!form.isVIP)
            {

            }
            return vm;
        }

        /// <summary>取得三天內的Abg資料</summary>
        /// <returns></returns>
        [HttpPost]
        [JwtAuthActionFilter]
        public string save_RCS_WEANING_ASSESS_CHECKLIST(FormWEANING_ASSESS_CHECKLIST form )
        { 
            return (string)this.returnObj(this.rtRecordModel.save_RCS_WEANING_ASSESS_CHECKLIST(form.pat_info, form.user_info, form.vm));
        }

        [HttpPost]
        [JwtAuthActionFilter]
        public DB_RCS_WEANING_ASSESS_CHECKLIST TodayRWAC(FormBody form)
        { 
            return this.returnObj<DB_RCS_WEANING_ASSESS_CHECKLIST>(this.rtRecordModel.getRWCA(form.pat_info.chart_no, form.pat_info.ipd_no, DateTime.Now.ToString("yyyy-MM-dd")));
        }//TodayRWAC

        /// <summary>
        /// 帶入指定ID資料
        /// </summary>
        /// <param name="form"></param>
        /// <returns></returns>
        [HttpPost]
        [JwtAuthActionFilter]
        public RT_RECORD_DATA<RT_RECORD> RecordById(FormBodyRT_RECORD_VIEW form)
        {
            return (RT_RECORD_DATA<RT_RECORD>)this.returnObj(this.rtRecordModel.getRecordById(form.pat_info, form.user_info, form.record_id));
        }//RecordById

        /// <summary>
        /// 帶入指定ID資料 for 合併
        /// </summary>
        /// <param name="form"></param>
        /// <returns></returns>
        [HttpPost]
        [JwtAuthActionFilter]
        public RT_RECORD_DATA<RT_RECORD_MUST_DATA> RecordByIdWithConbine(FormBodyRT_RECORD_VIEW form)
        {
            RT_RECORD_DATA<RT_RECORD_MUST_DATA> RecordData = (RT_RECORD_DATA<RT_RECORD_MUST_DATA>)this.returnObj(this.rtRecordModel.getRecordByIdWithConbine(form.pat_info, form.user_info, form.record_id));
            RecordData = rtRecordModel.changeRTRecordData(RecordData);

            return RecordData;

        }//RecordByIdWithConbine

        /// <summary>
        /// 帶入指定ID資料 for 合併
        /// </summary>
        /// <param name="form"></param>
        /// <returns></returns>
        [HttpPost]
        [JwtAuthActionFilter]
        public string RTRecordFormSave(FormBodyRT_RECORD_VIEW form)
        {
            return (string)this.returnObj(this.rtRecordModel.RTRecordFormSave(form.pat_info, form.user_info, form.record_id, form.isVIP, form.model)); 
        }//RTRecordFormSave


         /// <summary>
         /// 帶入指定ID資料 for 合併
         /// </summary>
         /// <param name="form"></param>
         /// <returns></returns>
        [HttpPost]
        [JwtAuthActionFilter]
        public string RTRecordFormDel(FormBodyRT_RECORD_VIEW form)
        {
            return  (string)this.returnObj(this.rtRecordModel.RTRecordFormDel(form.pat_info, form.user_info, form.record_id)); 
        }//RTRecordFormDel

        /// <summary>
        /// HTML匯出PDF
        /// </summary>
        /// <param name="form"></param>
        /// <returns></returns>
        [JwtAuthActionFilterAttribute(notVerification = true)]
        [HttpPost]
        public HttpResponseMessage RTRecordExportPDF(PDF_FORM_BODY form)
        {
            HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.BadRequest);

            IDocSetting ds = new CHGHRTRecordFormPDFDocSetting();

            byte[] buf = RCS.Controllers.WEBAPI.BasicController.exportPDF(form.HtmlStr, ds , "<div>我是分頁</div>");
            if (buf != null)
            {
                var contentLength = buf.Length;
                var statuscode = HttpStatusCode.OK;
                response = Request.CreateResponse(statuscode);
                response.Content = new StreamContent(new MemoryStream(buf));
                response.Content.Headers.ContentType = new MediaTypeHeaderValue("application/pdf");
                response.Content.Headers.ContentLength = contentLength;
                ContentDispositionHeaderValue contentDisposition = null;
                if (ContentDispositionHeaderValue.TryParse("inline; filename=" + form.pdfFIleName + ".pdf", out contentDisposition))
                {
                    response.Content.Headers.ContentDisposition = contentDisposition;
                }

            }
            else
            {
                this.throwHttpResponseException("程式發生錯誤，請洽資訊人員!");
            }

            return response;
        }

        /// <summary>
        /// CXR 結果
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [JwtAuthActionFilter]
        public List<string> getResultDropdownlist()
        {
            string actionName = "getResultDropdownlist";
            List<string> pList = new List<string>();
            try
            {
                pList = this.rtRecordModel.getResultDropdownlist();
            }
            catch (Exception ex)
            {
                LogTool.SaveLogMessage(ex, actionName, this.csName);
            }
            return pList;
        }

        /// <summary>
        /// 帶入指定ID資料 for 合併
        /// </summary>
        /// <param name="form"></param>
        /// <returns></returns>
        [HttpPost]
        [JwtAuthActionFilter]
        public string Reupload(FormBodyRT_RECORD_VIEW form)
        {
            RESPONSE_MSG rm = new RESPONSE_MSG();
            rm = this.UploadModel.ReUpDateData(form.user_info, form.record_id, DB_TABLE_NAME.DB_RCS_RECORD_MASTER, "RECORD_ID");
            rm.attachment = "修改後記得儲存此筆資料!並重新上傳簽章!";
            return (string)this.returnObj(rm);
        }//RTRecordFormSave

    }


}

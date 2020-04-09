using Dapper;
using Newtonsoft.Json;
using RCS.Models;
using RCS.Models.ViewModel;
using RCS_Data;
using RCS_Data.Controllers.RtAssess;
using RCS_Data.Models.DB;
using RCS_Data.Models.ViewModels;
using RCSData.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace RCS.Controllers.WEBAPI
{
    [JwtAuthActionFilter]
    public class RtAssessController : BasicController , IRtAssessController
    {
        string csName { get { return "rtAssessController"; } }

        RtAssess _model;
        RtAssess  rtAssessModel
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
        /// CPTRecord資料清單
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public List<RCS_CPT_DTL_NEW_ITEMS> CPTRecordList(Form_CPTRecordList form)
        {
            RESPONSE_MSG rm = new RESPONSE_MSG();
            List<RCS_CPT_DTL_NEW_ITEMS> cpt_dtl_new_items = new List<RCS_CPT_DTL_NEW_ITEMS>();

            IPDPatientInfo pat_info = form.pat_info;
            rm = rtAssessModel.CPTRecordList(ref cpt_dtl_new_items, form.pSDate,form.pEDate, pat_info.ipd_no);

            return cpt_dtl_new_items;
        }

        /// <summary>
        /// 儲存呼吸治療評估單
        /// </summary>
        /// <param name="form"></param>
        /// <returns></returns>
        [HttpPost]
        public RESPONSE_MSG CPTAssess_Save(Form_CPTAssess_Save form)
        { 
            RESPONSE_MSG rm = new RESPONSE_MSG();

            rm = rtAssessModel.CPTAssess_Save(form);
            this.returnObj(rm);
            rm.message = (string)rm.attachment;
            return rm;
        }

        /// <summary>
        /// 刪除
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public RESPONSE_MSG CPTRecordDelete(Form_CPTDetail form)
        {
            RESPONSE_MSG rm = new RESPONSE_MSG();

            rm = rtAssessModel.CPTRecordDelete(new List<string>() { form.CPT_ID },form.user_info);
            this.returnObj(rm);
            return rm;
        }

        /// <summary>
        /// NEW CPT評估表單-Form資料來源
        /// </summary>
        /// <remarks>取代原CPTData()功能，塞被選去的表單資料 AlanHuang 20171124</remarks>
        /// <returns></returns>
        [HttpPost]
        public object CPTDetail(Form_CPTDetail form) {
            RTAssesDetail result = new RTAssesDetail();
            result.model = new RCS_RTAsses();
            if (form.CPT_ID == null || form.CPT_ID == "")
            {
                //新增介接資料
                // ABG
                List<IPDPatientInfo> tempPat_info = this.webmethod.getPatientInfoList(this.hospFactory.webService.HISPatientInfo(), form.pat_info.chart_no, form.pat_info.ipd_no);
                string CLINICAL_DIAGNOSIS = tempPat_info.Count > 0 ? tempPat_info[0].CLINICAL_DIAGNOSIS : "";
                Ventilator lastRec = this.basicfunction.GetLastRTRec(form.pat_info.chart_no, form.pat_info.ipd_no);
                #region ABG 
                ExamViewList getVitalSignList = new ExamViewList();
                
                #endregion

                result.model.diag_date = form.pat_info.diag_date;
                result.model.now_pat_diagnosis = form.pat_info.diagnosis_code;
                result.model.diagnosis = CLINICAL_DIAGNOSIS;
                result.model.record_date = DateTime.Now.ToString("yyyy-MM-dd HH:mm");
                result.model.body_height = form.pat_info.body_height;
                result.model.body_weight = form.pat_info.body_weight;

            }
            else {
                //讀取資料

                var getData = rtAssessModel.RTAssessData(new List<string>() { form.CPT_ID });

                if (getData.detailList.Any())
                {
                    result.model = rtAssessModel.changeJson(getData.detailList).First();
                }
                if (getData.masterList.Any())
                {
                    result.CREATE_ID = getData.masterList[0].CREATE_ID;
                    result.CREATE_NAME = getData.masterList[0].CREATE_NAME;
                    result.DATASTATUS = getData.masterList[0].DATASTATUS;
                    result.UPLOAD_STATUS = getData.masterList[0].UPLOAD_STATUS;
                    result.UPLOAD_ID = getData.masterList[0].UPLOAD_ID;
                }

            }

            //手術
            if (!String.IsNullOrWhiteSpace(result.model.operation_data)) {

                var getData = this.DBLink.Select_JSONData<DB_RCS_RT_RECORD_JSON>(result.model.operation_data);

                if (getData.Any()) {

                    result.operation_List = JsonConvert.DeserializeObject<List<PatOperation>>(getData.First().JSON_VALUE);
                }   
            }

            //意識EVM下拉選單
            result.conscious_e = this.select.getConsciousEVM("E");
            result.conscious_v = this.select.getConsciousEVM("V");
            result.conscious_m = this.select.getConsciousEVM("M");

            result.last_cpt_id = rtAssessModel.CPTLastData(form.pat_info).cpt_id;

            return result;
        }


        /// <summary>
        ///  
        /// </summary>
        /// <remarks> </remarks>
        /// <returns></returns>
        [HttpPost]
        public string Reupload(Form_CPTDetail form)
        {
            RESPONSE_MSG rm = new RESPONSE_MSG();
            rm = this.UploadModel.ReUpDateData(form.user_info, form.CPT_ID, DB_TABLE_NAME.DB_RCS_CPT_ASS_MASTER , "CPT_ID");
            rm.attachment = "修改後記得儲存此筆資料!並重新上傳簽章!";
            return (string)this.returnObj(rm); 
        }

    }

 

}

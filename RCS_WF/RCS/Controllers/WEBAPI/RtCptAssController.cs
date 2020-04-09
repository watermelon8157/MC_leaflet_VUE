using RCS.Models;
using System.Collections.Generic;
using System.Web.Http;
using RCS_Data.Models.ViewModels;
using RCS_Data;
using RCSData.Models;
using RCS_Data.Controllers.RtCptRecord;
using RCS_Data.Controllers.RtCptAss;
using System;
using System.Linq;
using RCS.Models.ViewModel;

namespace RCS.Controllers.WEBAPI
{
    [JwtAuthActionFilter]
    public class RtCptAssController : BasicController, IRtCptAssController
    {
        string csName { get { return "rtAssessController"; } }

        RtCptAssess _model;
        RtCptAssess rtCptAss
        {
            get
            {
                if (_model == null)
                {
                    this._model = new RtCptAssess();
                }
                return this._model;
            }
        }

        [HttpPost]
        public object CPTDetail(FormRtCPTAssDetail form)
        {
            RTCptAssesDetail result = new RTCptAssesDetail();
            result.model = new CPTNewRecord();
            if (form.CPT_ID == null || form.CPT_ID == "")
            {
                RtAssess RtAssess = new RtAssess();

                //TODO: 取得FIO2 janchiu
                //BaseModel BaseModel = new BaseModel();

                var pat_info = form.pat_info;

                Ventilator last_record = this.basicfunction.GetLastRTRec(pat_info.chart_no, pat_info.ipd_no);

                List<RCS_CPT_DTL_NEW_ITEMS> getCPTData = new List<RCS_CPT_DTL_NEW_ITEMS>();

                var rm = RtAssess.CPTRecordList(ref getCPTData, "2014-11-01 10:28", DateTime.Now.AddMonths(1).ToString("yyyy-MM-dd hh:mm"), form.pat_info.ipd_no);
                getCPTData = getCPTData.OrderByDescending(x => x.cpt_date).ToList();

                result.model.FiO2 = last_record.fio2_set;

                if (getCPTData.Any())
                {
                    var getDatas = getCPTData[0];


                    //診斷
                    result.model.now_diagnosis_display = getDatas.now_pat_diagnosis;

                    //抽菸歷史
                    result.model.smoke_history_data = getDatas.smoke_history_data;

                    result.model.smoke_history_PPD = getDatas.smoke_history_PPD;
                    result.model.smoke_history_year = getDatas.smoke_history_year;
                    result.model.smoke_history_end = getDatas.smoke_history_end;
                    result.model.smoke_history_end_year = getDatas.smoke_history_end_year;
                }
                result.model.rec_date = DateTime.Now.ToString("yyyy-MM-dd HH:mm");

            }
            else
            {
                //讀取資料

                List<CPTNewRecord> getData = rtCptAss.RtCptRecordListByID(new List<string>() { form.CPT_ID });

                if (getData.Any())
                {
                    result.model = getData.First();
                }

            }

            result.last_cpt_id = rtCptAss.CPTAssesData(form.pat_info).cpt_id;
            rtCptAss.reRTCptAssesDetail(ref result, new List<string>() { form.CPT_ID }); 
            return result;
        }

        [HttpGet]
        public RESPONSE_MSG CPTDel(string cpt_id)
        {

            RESPONSE_MSG rm = new RESPONSE_MSG();

            rm = rtCptAss.CPTDel(cpt_id);

            return rm;
        }

        /// <summary>
        /// 存檔
        /// </summary>
        /// <param name="form"></param>
        /// <returns></returns>
        [HttpPost]
        public RESPONSE_MSG saveCPT(RTCptAsses_Save form)
        {
            RESPONSE_MSG rm = new RESPONSE_MSG();

            rm = rtCptAss.saveCPT(form.model, form.pat_info, form.user_info);

            return rm;

        }

        /// <summary>
        /// CPTRecord資料清單
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public List<CPTNewRecord> RtCptAssList(FormRtCPTAssList form)
        {
            RESPONSE_MSG rm = new RESPONSE_MSG();
            List<CPTNewRecord> Result = new List<CPTNewRecord>();

            IPDPatientInfo pat_info = form.pat_info;
            rm = rtCptAss.RtCptRecordList(ref Result, form.sDate, form.eDate, pat_info);


            Result = rtCptAss.changeData(Result);

            return Result;
        }
    }
}

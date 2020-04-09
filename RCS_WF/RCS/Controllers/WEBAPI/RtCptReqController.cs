using Newtonsoft.Json;
using RCS.Models;
using RCS.Models.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using RCS_Data.Models.ViewModels;
using RCS_Data;
using RCSData.Models;
using RCS_Data.Models.DB;
using RCS_Data.Controllers.RtCptRecord;
using RCS_Data.Controllers.RtCptAss;
using RCS_Data.Controllers.RtCptReq;

namespace RCS.Controllers.WEBAPI
{
    [JwtAuthActionFilter]
    public class RtCptReqController : BasicController, IRtCptReqController
    {
        string csName { get { return "RtCptReqController"; } }

        RtCptReq _model;
        RtCptReq rtCptReq
        {
            get
            {
                if (_model == null)
                {
                    this._model = new RtCptReq();
                }
                return this._model;
            }
        }

        [HttpPost]
        public object GetDetail(FormRtCPTReqDetail form)
        {
            RTCptReqDetail result = new RTCptReqDetail();
            result.model = new RTCptReq();
            if (form.RECORD_ID == null || form.RECORD_ID == "")
            {
                RtAssess RtAssess = new RtAssess();

                //TODO: 取得FIO2 janchiu
                //BaseModel BaseModel = new BaseModel();

                var pat_info = form.pat_info;

                Ventilator last_record = this.basicfunction.GetLastRTRec(pat_info.chart_no, pat_info.ipd_no);

                List<RCS_CPT_DTL_NEW_ITEMS> getCPTData = new List<RCS_CPT_DTL_NEW_ITEMS>();

                var rm = RtAssess.CPTRecordList(ref getCPTData, "2014-11-01 10:28", DateTime.Now.AddMonths(1).ToString("yyyy-MM-dd hh:mm"), form.pat_info.ipd_no);
                getCPTData = getCPTData.OrderByDescending(x => x.cpt_date).ToList();

                result.model.vs_doc = form.pat_info.vs_doc;

                if (getCPTData.Any())
                {
                    var getDatas = getCPTData[0];


                    //診斷
                    result.model.now_diagnosis_display = getDatas.now_pat_diagnosis;

                }
                result.model.rec_date = DateTime.Now.ToString("yyyy-MM-dd HH:mm");

            }
            else
            {
                //讀取資料

                List<RTCptReq> getData = rtCptReq.RtCptReqListByID(new List<string>() { form.RECORD_ID });

                if (getData.Any())
                {
                    result.model = rtCptReq.changeData(getData).First();
                }

            }

            result.last_record_id = rtCptReq.CPTReqData(form.pat_info).record_id;

            return result;
        }

        [HttpGet]
        public RESPONSE_MSG CPTDel(string cpt_id)
        {

            RESPONSE_MSG rm = new RESPONSE_MSG();

            rm = rtCptReq.CPTDel(cpt_id);

            return rm;
        }

        /// <summary>
        /// 存檔
        /// </summary>
        /// <param name="form"></param>
        /// <returns></returns>
        [HttpPost]
        public RESPONSE_MSG saveCPT(RTCptReq_Save form)
        {
            RESPONSE_MSG rm = new RESPONSE_MSG();

            rm = rtCptReq.saveCPT(form.model, form.pat_info, form.user_info);

            return rm;

        }

        /// <summary>
        /// CPTRecord資料清單
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public List<RTCptReq> RtCptReqList(FormRtCPTAssList form)
        {
            RESPONSE_MSG rm = new RESPONSE_MSG();
            List<RTCptReq> Result = new List<RTCptReq>();

            IPDPatientInfo pat_info = form.pat_info;
            rm = rtCptReq.RtCptReqList(ref Result, form.sDate, form.eDate, pat_info);


            return Result;
        }
    }
}

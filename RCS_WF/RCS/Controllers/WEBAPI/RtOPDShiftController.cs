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
using RCS_Data.Controllers.RtOPDShift;
using Com.Mayaminer;

namespace RCS.Controllers.WEBAPI
{
    [JwtAuthActionFilter]
    public class RtOPDShiftController : BasicController, IRtOPDShiftController
    {

        string csName { get { return "RtOPDShiftController"; } }

        RtOPDShift _model;
        RtOPDShift rtOPDShift
        {
            get
            {
                if (_model == null)
                {
                    this._model = new RtOPDShift();
                }
                return this._model;
            }
        }

        [HttpGet]
        public RESPONSE_MSG Delete(string OS_ID)
        {

            RESPONSE_MSG rm = new RESPONSE_MSG();

            rm = rtOPDShift.Del(OS_ID);

            return rm;
        }

        /// <summary>
        /// 存檔
        /// </summary>
        /// <param name="form"></param>
        /// <returns></returns>
        [HttpPost]
        public RESPONSE_MSG OPDShift_Save(RtOPDShift_Save form)
        {
            RESPONSE_MSG rm = new RESPONSE_MSG();

            rm = rtOPDShift.OPDShift_Save(form.model, form.pat_info, form.user_info);

            return rm;

        }

        [HttpPost]
        public object GetDetail(FormRtOPDShiftDetail form)
        {
            RtOPDShiftDetail result = new RtOPDShiftDetail();
            result.model = new RCS_RT_OPD_SHIFT_DETAIL();
            if (form.ID == null || form.ID == "")
            {

                result.model.opt_Shift_date = DateTime.Now.ToString("yyyy-MM-dd hh:mm");

            }
            else
            {
                //讀取資料

                List<RCS_RT_OPD_SHIFT_DETAIL> getData = rtOPDShift.RtCptReqListByID(new List<string>() { form.ID });

                if (getData.Any()) {

                    result.model = getData.First();
                }

            }

            result.last_id = rtOPDShift.getLastData(form.pat_info).OS_ID;
            result.drug_inhalation = getDrugInhalation();

            if (string.IsNullOrWhiteSpace(result.last_id))
            {
                //收尋急診門診病患
                if (bool.Parse(IniFile.GetConfig("SystemConfig", "SearchErAndOpPatient")))
                {
                    // TODO:janchiu HISSQL如何搬?
                    //HISData<IPDPatientInfo> erAndOPDList = HISSQL.getERAndOPDListbyChartNo(pat_info.chart_no);
                    //if (erAndOPDList.datastatus == HISDataStatus.SuccessWithData)
                    //{
                    //    if (erAndOPDList.dataList.Count > 0)
                    //    {
                    //        if (erAndOPDList.dataList.Exists(x => !string.IsNullOrWhiteSpace(x.source_count) && !string.IsNullOrWhiteSpace(x.diag_date) && (x.source_count == "0" || x.source_count == "1")))
                    //        {
                    //            result.model.opd_date = erAndOPDList.dataList.FindAll(x => !string.IsNullOrWhiteSpace(x.source_count) && !string.IsNullOrWhiteSpace(x.diag_date) && (x.source_count == "0" || x.source_count == "1")).OrderByDescending(x => DateTime.Parse(x.diag_date)).First().diag_date;
                    //        }
                    //    }
                    //}
                }
            }


            return result;
        }

        /// <summary>
        /// 取得執行治療項目Drug Inhalation下拉選單來源
        /// </summary>
        /// <returns></returns>
        public List<string> getDrugInhalation()
        {
            List<string> DrugInhalation = new List<string>();
            DrugInhalation.Add("");
            DrugInhalation.Add("A:Atrovent");
            DrugInhalation.Add("B:Bricanyl");
            DrugInhalation.Add("C:Combivent");
            DrugInhalation.Add("F:Flumucil");
            DrugInhalation.Add("M:Mistabron");
            DrugInhalation.Add("V:Ventolin");
            DrugInhalation.Add("N/S:Normal Saline");
            DrugInhalation.Add("P:Pulmicort");
            DrugInhalation.Add("H/S:Half Saline");
            DrugInhalation.Add("colistin");
            DrugInhalation.Add("Gentamycine");
            DrugInhalation.Add("Bosmine");
            DrugInhalation.Add("Encore");
            DrugInhalation.Add("Siruta");
            return DrugInhalation;
        }

        /// <summary>
        /// 資料清單
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public List<RCS_RT_OPD_SHIFT_DETAIL> getIndexOPDshift(FormRtOPDShiftList form)
        {
            RESPONSE_MSG rm = new RESPONSE_MSG();
            List<RCS_RT_OPD_SHIFT_DETAIL> Result = new List<RCS_RT_OPD_SHIFT_DETAIL>();

            IPDPatientInfo pat_info = form.pat_info;
            Result = rtOPDShift.getIndexOPDshift(form.sDate, form.eDate, pat_info.chart_no);


            return Result;
        }
    }
}

using Newtonsoft.Json;
using RCS.Models;
using RCS.Models.ViewModel;
using RCS_Data;
using RCS_Data.Controllers.RtTakeoff;
using RCS_Data.Controllers.RtValuation;
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
    public class RtValuationController : BasicController
    {
        string csName { get { return "RtValuationController"; } }

        RtValuation _model;
        RtValuation RtTakeoffModel
        {
            get
            {
                if (_model == null)
                {
                    this._model = new RtValuation();
                }
                return this._model;
            }
        }

        [HttpPost]
        public string TEST()
        {
            return "";
        }

        /// <summary>
        /// 資料清單
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public RESPONSE_MSG SaveValuationConsumableItem(SaveForm form)
        {
            RESPONSE_MSG rm = new RESPONSE_MSG();

            rm = RtTakeoffModel.SaveValuationConsumableItem(form);

            return rm;
        }

        /// <summary>
        /// 詳細資料
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public RCS_VALUATION_DETAIL_MODEL GetValuationDetail(FormRtValuationDetail form) {
            RCS_VALUATION_DETAIL_MODEL result = new RCS_VALUATION_DETAIL_MODEL();


            result = RtTakeoffModel.EditConsumableList(form.VID);

            return result;
        }

        /// <summary>
        /// 資料清單
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public List<RCS_VALUATION_ITEMS> GetValuationTableList(FormRtValuationList form)
        {
            string actioName = "GetValuationTableList";
            List<RCS_VALUATION_ITEMS> show_valuation_table = new List<RCS_VALUATION_ITEMS>();
            IPDPatientInfo pat_info = form.pat_info;
            show_valuation_table = RtTakeoffModel.GetValuationTableList(pat_info.ipd_no,pat_info.chart_no, form.sDate, form.eDate);


            return show_valuation_table;
        }

    }

}

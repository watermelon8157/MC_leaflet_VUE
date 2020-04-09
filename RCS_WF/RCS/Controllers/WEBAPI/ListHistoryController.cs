using RCS_Data;
using RCS_Data.Controllers.ListHistory;
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
    public class ListHistoryController : BasicController
    { 

        public ListHistoryModels _model { get; set; }
        protected ListHistoryModels model
        {
            get
            {
                if (this._model == null)
                {
                    this._model = new ListHistoryModels();
                }
                return this._model;
            }
        }

        public ListHistoryViewModels index()
        {
            ListHistoryViewModels vm = new ListHistoryViewModels();
            vm.docList = this.select.getRCS_SYS_PARAMS("Shared", "doctor_name", @pStatus: "1");
            vm.bedList = this.select.getHisBedAreaList(this.hospFactory.webService.HisBedAreaList()); 

            return vm;
        }

        [HttpPost]
        public List<PatientListItem> Pathistory(ListHistoryViewModels form)
        {   
            return (List<PatientListItem>)this.returnObj(this.model.Pathistory(form));
        }


        /// <summary>
        /// 選擇病患資料
        /// </summary>
        /// <param name="form"></param>
        /// <returns></returns>
        public IPDPatientInfo SelectPatientInfo(RCS_Data.Controllers.RT.FormRT form)
        {
            RESPONSE_MSGLIST rm = new RESPONSE_MSGLIST();
            IPDPatientInfo pItem = new IPDPatientInfo();
            if (!string.IsNullOrWhiteSpace(form.join_json))
            {
                pItem = Newtonsoft.Json.JsonConvert.DeserializeObject<IPDPatientInfo>(form.join_json);
                pItem = this.model.SelectPatientInfo(pItem.ipd_no, pItem.chart_no);
            }
            return pItem;
        }
    }
}

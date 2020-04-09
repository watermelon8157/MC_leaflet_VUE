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
using RCS_Data.Controllers.OPDScheduling;
using Com.Mayaminer;
using Newtonsoft.Json;
using System.Web;

namespace RCS.Controllers.WEBAPI
{
    [JwtAuthActionFilter]
    public class OPDSchedulingController : BasicController, IOPDSchedulingController
    {
        string csName { get { return "OPDSchedulingController"; } }

        OPDScheduling _model;
        OPDScheduling OPDScheduling
        {
            get
            {
                if (_model == null)
                {
                    this._model = new OPDScheduling();
                }
                return this._model;
            }
        }

        /// <summary>
        /// 資料清單
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public OPD_TREATMENT_VM week_data(FormOPDList form)
        {
            OPD_TREATMENT_VM rm = new OPD_TREATMENT_VM();

            IPDPatientInfo pat_info = form.pat_info;
            rm = OPDScheduling.week_data(form.weakenSDate,form.weakenEDate,form.weanType,form.isCopy,form.copysetCnt,form.isList==null?false: form.isList);

            return rm;
        }

        [HttpPost]
        public RESPONSE_MSG week_save(OPD_Save form)
        {
            RESPONSE_MSG rm = new RESPONSE_MSG();
            string actionName = "week_save";
            rm = OPDScheduling.week_save(form);
            return rm;
        }


        public RESPONSE_MSG update_Data(OPD_Save form)
        {
            RESPONSE_MSG rm = new RESPONSE_MSG();
            rm = OPDScheduling.update_Data(form);
            return rm;
        }

        /// <summary>
        /// 查詢病患
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public List<OPD_INFO> search_chart_no(OPD_Save form)
        {
            List<OPD_INFO> List = new List<OPD_INFO>();
            string actionName = "search_chart_no";
            string chart_no = form.model;
            try
            {
                SQLProvider SQL = new SQLProvider();
                if (bool.Parse(IniFile.GetConfig("System", "aadPatientCheckChartNo")))
                {
                    int chart_no_NumBit = int.Parse(IniFile.GetConfig("System", "ChartNoNNumBits"));
                    if (chart_no.Length > 0 && chart_no.Length < chart_no_NumBit)
                    {
                        chart_no = chart_no.ToString().PadLeft(chart_no_NumBit, '0');
                    }
                }

                List = OPDScheduling.search_chart_no(chart_no);
            }
            catch (Exception ex)
            {
                LogTool.SaveLogMessage(ex, actionName, this.csName);
            }
            return List;
        }

    }

}

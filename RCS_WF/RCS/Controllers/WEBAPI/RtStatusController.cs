using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using RCS_Data.Models.VIP;
using RCS_Data;
using RCSData.Models;
using RCS_Data.Models.ViewModels;
using Com.Mayaminer;

namespace RCS.Controllers.WEBAPI
{
    public class RtStatusController : BasicController
    {
        string csName { get { return "RtStatusController"; } }

        [HttpPost]
        public RESPONSE_MSG StatusIndex(FormRtStatusList  form )
        {
            if (form.before_day == 0)
            {
                form.before_day = -2;
            }
            VIPRTTBL VIPRTTBL = new VIPRTTBL();
            RESPONSE_MSG rm = VIPRTTBL.checkVIPDataHasRepeat(form.pat_info.chart_no);
            return rm;
        }

        [HttpPost]
        public List<RT_RECORD_MAIN> List(FormRtStatusList form )
        {
            if (form.before_day == 0)
            {
                form.before_day = 2;
            }
            string actionName = "List";
            List<RT_RECORD_MAIN> rt_record_main_list = new List<RT_RECORD_MAIN>();
            VIPRTTBL VIPRTTBL = new VIPRTTBL();
            try
            { 
                VIPRTTBL.getRt_Record_Main_List(form.before_day, form.pat_info.chart_no); 
            }
            catch (Exception ex)
            {
                LogTool.SaveLogMessage(ex, actionName, this.csName);
            }
            return VIPRTTBL.rt_record_main_list;
        }
    }


    public class FormRtStatusList :AUTH
    {
        public int before_day { get; set; }
        public string data_type { get; set; }
        public string search_text { get; set; }
    }
}

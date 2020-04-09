using Com.Mayaminer;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RCS_Data;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Mvc;
using System.Xml;
using System.Xml.Linq;
using System.Data;
using RCS.Models.DB;
using RCS.Models;
using RCS_Data.Controllers.HisData;
using RCS.Models.ViewModel;

namespace RCS.Controllers
{
    public class RTChartController : BaseController
    {

        private HisDataModel RTChartModel;
        public RTChartController()
        {
            RTChartModel = new HisDataModel();
        }

        /// <summary>
        /// RT參數趨勢圖
        /// </summary>
        /// <returns></returns>
        public ActionResult RTChart()
        {
            List<SelectListItem> getPatientHistoryList = BaseModel.getPatientHistoryList(pat_info.chart_no, pat_info.ipd_no);
            ViewData["getPatientHistoryList"] = getPatientHistoryList;
            SelectListItem item = new SelectListItem();
            if (getPatientHistoryList.Exists(x => x.Selected))
                item = getPatientHistoryList.Find(x => x.Selected);
            ViewData["sDate"] = BaseModel.getHistoryList(item.Value, 1);
            ViewData["eDate"] = BaseModel.getHistoryList(item.Value, 2);
            return View();
        }

        public ActionResult RTChartData(string StartDate, string EndDate,string ipdNo, string chart_no)
        {
            try
            {
                if (pat_info.chart_no != null)
                {
      
                    ViewData["chartValue"] = JsonConvert.SerializeObject(RTChartModel.RTChartData(StartDate, EndDate, chart_no));
                    return View();
                }
            }
            catch(Exception ex)
            {
                LogTool.SaveLogMessage(ex, MethodInfo.GetCurrentMethod().Name,
                    MethodInfo.GetCurrentMethod().DeclaringType.Name);
            }           
            return new EmptyResult();
        }
    }
}

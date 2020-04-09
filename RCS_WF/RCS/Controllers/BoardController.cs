using Newtonsoft.Json;
using RCS.Models.ViewModel;
using RCS_Data;
using RCS_Data.Controllers.RT;
using RCS_Data.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace RCS.Controllers
{
    public class BoardController : BaseController
    {
        //
        // GET: /Board/
        string csName { get { return "BoardController"; } }

        public ActionResult Index()
        {
            return View();
        }

        public JsonResult getWearningTable(string jsondata)
        {
            string actionName = "getWearningTable";
            List<PatientListItem> row = new List<PatientListItem>();
            List<WPTable> pList = new List<Models.ViewModel.WPTable>();
            try
            {
                pList = new List<Models.ViewModel.WPTable>();
            }
            catch (Exception ex)
            {
                Com.Mayaminer.LogTool.SaveLogMessage(ex, actionName, this.csName);
            }
            return Json(pList);

        }
        public JsonResult getWPCnt(string jsondata)
        {
            int cnt = 0;
            string actionName = "getWearningTable";
            
            try
            {
                List<WPTable> pList = new List<Models.ViewModel.WPTable>();
                if (pList.Count> 0)
                {
                    cnt = pList.Count;
                }
            }
            catch (Exception ex)
            {
                Com.Mayaminer.LogTool.SaveLogMessage(ex, actionName, this.csName);
            }
            return Json(cnt);
        } 
    }
}

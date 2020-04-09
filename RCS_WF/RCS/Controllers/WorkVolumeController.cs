using RCS.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data;
using Com.Mayaminer;
using Dapper;
using RCS_Data;

namespace RCS.Controllers
{
    public class WorkVolumeController : BaseController
    {
        //業務量統計
        // GET: /WorkVolume/
        string csName { get { return "WorkVolumeController"; } }
        public ActionResult Index()
        {
            WorkVolumeViewModel vm = new WorkVolumeViewModel();
            vm.sDate = DateTime.Now.AddDays(-7).ToString("yyyy-MM-dd");
            vm.eDate = DateTime.Now.ToString("yyyy-MM-dd");
            return View(vm);
        }

        public JsonResult QueryResult(WorkVolumeViewModel vm)
        {
            WorkVolume wv = new WorkVolume(vm);
            wv.getWorkVolume();

            return Json(wv.WorkVolumeList);
        }

        public ActionResult mainBreathReport()
        {
            VM_mainBreathReport vm = new VM_mainBreathReport();
            vm.yearList = new List<SelectListItem>();
            for (int i = 2018; i < DateTime.Now.AddYears(4).Year; i++)
            {
                vm.yearList.Add(new SelectListItem() { Value = i.ToString(), Text = i.ToString() });
            }
            vm.monthList = new List<SelectListItem>();
            for (int i = 1; i <= 12; i++)
            {
                vm.monthList.Add(new SelectListItem() { Value = i.ToString().PadLeft(2,'0'), Text = i.ToString().PadLeft(2, '0') });
            }
            vm.year = DateTime.Now.Year.ToString();
            vm.month = DateTime.Now.Month.ToString().PadLeft(2, '0');
            return View(vm);
        }
  
    }
}

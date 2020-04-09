using Com.Mayaminer;
using RCS_Data;
using System;
using System.Collections.Generic;
using System.Data;
using System.Web.Mvc;
using System.Linq;
using RCS.Models;
using RCSData.Models;
using RCS_Data.Models.ViewModels;
using RCS_Data.Controllers.HisData;

namespace RCS.Controllers
{
    public class ExamLabController : BaseController {

        private Exam model = new Exam();

        RCS_Data.Controllers.HisData.Models _model;
        RCS_Data.Controllers.HisData.Models HisDataModel
        {
            get
            {
                if (_model == null)
                {
                    this._model = new RCS_Data.Controllers.HisData.Models();
                }
                return this._model;
            }
        }

        #region ExamViewer 檢驗
        /// <summary>
        /// 檢驗顯示畫面
        /// </summary>
        /// <returns></returns>
        public ActionResult ExamViewer() {
            return View(true);
        }

        /// <summary>
        /// 取得檢驗資料
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ActionResult ExamData() {
 
            RESPONSE_MSG rm = new RESPONSE_MSG();
            ExamView ExamViewData = new ExamView();
            DateTime start_date = DateTime.Parse(Request["st_data"].ToString());
            FormExamData form = new FormExamData() {
                pSDate = Request["st_data"].ToString(),
                 pEDate = start_date.ToString("yyyy-MM-dd")
            };
            if (!string.IsNullOrWhiteSpace(form.pSDate) && !string.IsNullOrWhiteSpace(form.pEDate))
            { 
                rm = this.HisDataModel.Exam_Data(ref ExamViewData, form.pSDate, form.pEDate, pat_info, this.hospFactory.webService.HISLabData());
            }
            return View("_ExamViewTable", ExamViewData);
        }

        #endregion

        #region ExamList 檢查
        // 檢查清單
        public ActionResult ExamList()
        {
            List<ExamReport> list = model.web_method.getReportData(this.hospFactory.webService.HISReportData(), pat_info.chart_no, pat_info.ipd_no, DateTime.Now.AddMonths(-1).ToString("yyyy-MM-dd"), "");
            list = list.OrderByDescending(x => x.COMPLETTIME).ToList();
            if (list.Count > 0)
                ViewData["ExamList"] = list;
            else
                ViewData["ExamList"] = null;
            return View();
        }

        #endregion

    }

}

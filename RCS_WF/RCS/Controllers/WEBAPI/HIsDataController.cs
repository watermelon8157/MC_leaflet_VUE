
using RCS_Data;
using RCSData.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using static RCS_Data.Controllers.HisData.Models;
using RCS_Data.Models.ViewModels;
using RCS_Data.Controllers.HisData;
using RCS_Data.Controllers.RtRecord;

namespace RCS.Controllers.WEBAPI
{
    public class HisDataController : BasicController
    {

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
        /// <summary>
        /// 取得輸入輸出資料
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public List<IO_PUT> search_io(FormSearch_io form)
        {
            RESPONSE_MSG rm = new RESPONSE_MSG();
            List<IO_PUT> finalIO_PUTList = new List<IO_PUT>();


            if (!string.IsNullOrWhiteSpace(form.io_sdate) && !string.IsNullOrWhiteSpace(form.io_edate))
            {
                IPDPatientInfo pat_info = form.pat_info;
                rm = this.HisDataModel.Search_io(ref finalIO_PUTList, form.io_sdate, form.io_edate, pat_info, this.hospFactory.webService.HISInputOutput());
            }
            if (rm.hasLastError)
            {
                this.throwHttpResponseException(rm.lastError);
            }
            return finalIO_PUTList;
        }


        /// <summary>
        /// 取得vitalSing的資料
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public List<VitalSign> search_vs(FormExamData form)
        {
            RESPONSE_MSG rm = new RESPONSE_MSG();
            List<VitalSign> returnVitalSignList = new List<VitalSign>();
            rm = this.HisDataModel.Search_vs(ref returnVitalSignList, this.hospFactory.webService.HISVitalSign(), form.pat_info.chart_no, form.pat_info.ipd_no, form.pSDate,form.pEDate);

            if (rm.hasLastError)
            {
                this.throwHttpResponseException(rm.lastError);
            }
            return returnVitalSignList;
        }


        /// <summary>
        /// ExamList 檢查
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public List<ExamReport> ExamList(FormExamReport form)
        {
            RESPONSE_MSG rm = new RESPONSE_MSG();
            List<ExamReport> ExamReportData = new List<ExamReport>();

            form.pSDate = DateTime.Now.AddMonths(-6).ToString("yyyy-MM-dd");
            if (!string.IsNullOrWhiteSpace(form.pSDate))
            {
                IPDPatientInfo pat_info = form.pat_info;
                rm = this.HisDataModel.Exam_List(ref ExamReportData, form.pSDate, "", form.pat_info, this.hospFactory.webService.HISReportData());
            }
            if (rm.hasLastError)
            {
                this.throwHttpResponseException(rm.lastError);
            }

            ExamReportData = ExamReportData.OrderByDescending(x => x.COMPLETTIME).ToList();

            return ExamReportData;
        }

        /// <summary>
        /// 取得檢驗資料
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ExamView ExamData(FormExamData form)
        {
            RESPONSE_MSG rm = new RESPONSE_MSG();
            ExamView ExamViewData = new  ExamView(); 

            if (!string.IsNullOrWhiteSpace(form.pSDate) && !string.IsNullOrWhiteSpace(form.pEDate))
            {
                IPDPatientInfo pat_info = form.pat_info;
                rm = this.HisDataModel.Exam_Data(ref ExamViewData, form.pSDate, form.pEDate, pat_info, this.hospFactory.webService.HISLabData());
            }
            if (rm.hasLastError)
            {
                this.throwHttpResponseException(rm.lastError);
            }
            return ExamViewData;
        }
         
        /// <summary>
        /// 取得特殊用藥及治療情形
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public List<PatOrder> search_drog(FormExamData form)
        {
            RESPONSE_MSG rm = new RESPONSE_MSG();
            List<PatOrder> returnList = new List<PatOrder>();
  
            rm = this.HisDataModel.search_drog(ref returnList, this.hospFactory.webService.HISOrderList(), form.pat_info.chart_no, form.pat_info.ipd_no, form.pSDate, form.pEDate);
            if (rm.hasLastError)
            {
                this.throwHttpResponseException(rm.lastError);
            }
            return returnList;
        }
        /// <summary>
        /// 取得特殊用藥及治療情形
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public List<RT_TAKEOFF_ASSESSMENT> TakeOffAssessmentData(FormExamData form)
        {
            RESPONSE_MSG rm = new RESPONSE_MSG();
            List<RT_TAKEOFF_ASSESSMENT> returnList = new List<RT_TAKEOFF_ASSESSMENT>();

            rm = this.HisDataModel.TakeOffAssessmentData(form.pat_info.chart_no);
            if (rm.hasLastError)
            {
                this.throwHttpResponseException(rm.lastError);
            }
            returnList = (List<RT_TAKEOFF_ASSESSMENT>)rm.attachment;
            return returnList;
             
        }
        /// <summary>
        /// 取得特殊用藥及治療情形
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public List<RT_RECORD> ABGData(FormExamData form)
        {
            RESPONSE_MSG rm = new RESPONSE_MSG();
            List<RT_RECORD> returnList = new List<RT_RECORD>();

            rm = this.HisDataModel.getABGData(form.pat_info.chart_no);
            if (rm.hasLastError)
            {
                this.throwHttpResponseException(rm.lastError);
            }
            returnList = (List<RT_RECORD>)rm.attachment;
            return returnList;

        }
        /// <summary> 更新HIS手術資料選取畫面 </summary>
        /// <returns></returns>
        [HttpPost]
        public List<PatOperation> gatPatOperation(FormAssessOPData form)
        {
            RESPONSE_MSGLIST rm = new RESPONSE_MSGLIST();
            //找到該筆CPT_ID對應之IPD_NO及CHART_NO
            string tmp_ipd_no = form.pat_info.ipd_no;
            string tmp_chart_no = form.pat_info.chart_no;

            List<PatOperation> ORList = new List<PatOperation>();

            rm = this.HisDataModel.getPatOperationList(ref ORList, this.hospFactory.webService.HISPatOperationList(), tmp_chart_no, tmp_ipd_no);

            return ORList;
        }

        /// <summary>
        /// search_Chart
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public List<RTCHartData> search_Chart(FormExamData form)
        {
            RESPONSE_MSG rm = new RESPONSE_MSG();
            List<RTCHartData> returnList = new List<RTCHartData>();

            rm = this.HisDataModel.RTChartDataList(form.pSDate ,form.pEDate, form.pat_info.chart_no);
            if (rm.hasLastError)
            {
                this.throwHttpResponseException(rm.lastError);
            }
            returnList = (List<RTCHartData>)rm.attachment;
            string tempstr = Newtonsoft.Json.JsonConvert.SerializeObject(returnList);
            return returnList;

        }
    }
}

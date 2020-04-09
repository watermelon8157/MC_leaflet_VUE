using Com.Mayaminer;
using RCS.Models;
using RCS.Models.ViewModel;
using RCS_Data;
using RCS_Data.Controllers.RTCalendar;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web.Http; 

namespace RCS.Controllers.WEBAPI
{
    public class RTCalendarController : BasicController
    {
        string csName = "RTCalendarController";

        RTCalendarModel _model { get; set; }

        RTCalendarModel model
        {
            get
            {
                if (_model == null)
                {
                    this._model = new RTCalendarModel();
                }
                return this._model;
            }
        }


        /// <summary>
        /// getRTCalList
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public List<RTCalList> getRTCalList(FormBody_getRTCalList model)
        {
            List<RTCalList> _list = new List<RTCalList>();
             _list = (List<RTCalList>)this.returnObj(this.model.getRTCalList(model));


            return _list;
        }


        /// <summary>
        /// getRTCalWeek
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public List<RTCalWeek> getRTCalWeek(FormBody_getRTCalList model)
        {
            List<RTCalWeek> _list = new List<RTCalWeek>();
            _list = (List<RTCalWeek>)this.returnObj(this.model.getRTCalWeek(model));


            return _list;
        }

        [HttpPost]
        public HttpResponseMessage exportExcel(FormBody_getRTCalList model)
        {
            string actionName = "exportExcel";
            List<RTCalList> _list = new List<RTCalList>();
            HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.BadRequest);
            response.StatusCode = HttpStatusCode.OK;
            try
            {
                _list = (List<RTCalList>)this.returnObj(this.model.getRTCalList(model));
                if (_list.Count > 0)
                {
                    if (_list != null && _list.Count > 0)
                    {
                        ExcelSetting em = new ExcelSetting();
                        em.bindColName = "RECORD,RECORDDATE,PATIENT_NAME,CHART_NO,CREATE_NAME,COST_CODE,RESPID";
                        em.colTitleName = "紀錄表單,記錄日期,病患姓名,病歷號,記錄人,單位代碼,呼吸器編號";
                        em.titleName = "RT接觸近況";
                        em.sheetName = "sheet1";
                        em.FileName = "RT接觸近況.xls";
                        exportFile exportFile = new exportFile(em.FileName);
                        DataTable dt = _list.ToDataTable();
                        byte[] buf = exportFile.exportExcel(dt, em);
                        if (buf != null)
                        {
                            var contentLength = buf.Length;
                            var statuscode = HttpStatusCode.OK;
                            response = Request.CreateResponse(statuscode);
                            response.Content = new StreamContent(new MemoryStream(buf));
                            response.Content.Headers.ContentType = new MediaTypeHeaderValue("application/vnd.ms-excel");
                            response.Content.Headers.ContentLength = contentLength;
                            ContentDispositionHeaderValue contentDisposition = null;
                            if (ContentDispositionHeaderValue.TryParse("inline; filename=" + em.FileName + ".xls", out contentDisposition))
                            {
                                response.Content.Headers.ContentDisposition = contentDisposition;
                            }
                        }
                        else
                        {
                            response.Content = new StringContent("程式發生錯誤，請洽資訊人員! 108");
                        }
                    }
                }
                else
                {
                    response.Content = new StringContent("查無資料!");
                }

            }
            catch (Exception ex)
            {
                response.Content = new StringContent("程式發生錯誤，請洽資訊人員! 120");
                LogTool.SaveLogMessage(ex, actionName, this.csName);
            }
            return response;
        }
    }
}

using Com.Mayaminer;
using mayaminer.com.library;
using RCS.Models;
using RCS.Models.ViewModel;
using RCS_Data.Controllers.System;
using RCS_Data.Models;
using RCS_Data.Models.DB;
using RCS_Data.Models.ViewModels;
using RCSData.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web.Http;

namespace RCS.Controllers.WEBAPI
{ 
    /// <summary>
    /// 後臺管理 API
    /// </summary>
    public class SystemController : BasicController, ISystemController
    {

        string csName { get { return "SystemController"; } }

        RCS_Data.Controllers.System.Models _model;
        RCS_Data.Controllers.System.Models SystemModel
        {
            get
            {
                if (_model == null)
                {
                    this._model = new RCS_Data.Controllers.System.Models();
                }
                return this._model;
            }
        }

        [HttpPost]
        public List<DB_RCS_SYS_PARAMS> getUserMaintainList(getUserMaintain_List_BODY form)
        {
            return (List<DB_RCS_SYS_PARAMS>)this.returnObj(this.SystemModel.getUserMaintainList(form));
        }

        [JwtAuthActionFilter]
        [HttpPost]
        public string saveUser(USER_FORM_BODY form)
        {
            return (string)this.returnObj(this.SystemModel.saveUser(form));
        }

        [HttpGet]
        /// <summary>使用者群組清單</summary>
        /// <returns></returns>
        public List<DB_RCS_SYS_PARAMS> getUserGroupList()
        {
            return (List<DB_RCS_SYS_PARAMS>)this.returnObj(this.SystemModel.getUserGroupList());
        }

        [HttpGet]
        public List<PatProgress> getCount(string pSDate, string pEDate)
        {
            List<PatProgress> pList = new List<PatProgress>();
            pList = this.webmethod.get7190OrderList(this.hospFactory.webService.RCSConsultList(), pSDate, pEDate);
            return pList;
        }

        [HttpGet]
        public List<DeviceMaster> getDevice(string pDeviceNo)
        {
            return (List<DeviceMaster>)this.returnObj(this.SystemModel.getDeviceList(true, pDeviceNo));
        }

        public List<DB_RCS_SYS_PARAMS> getDeviceType()
        {
            return (List<DB_RCS_SYS_PARAMS>)this.returnObj(this.SystemModel.getDeviceType());
        }

        [JwtAuthActionFilter]
        [HttpPost]
        public string saveVENTILATOR(VENTILATOR_FORM_BODY form)
        {
            return (string)this.returnObj(this.SystemModel.saveVENTILATOR(form));
        }

        [HttpGet]
        public List<DeviceMaster> getDeviceList(bool pShowDel, string pDeviceNo = "")
        {
            return (List<DeviceMaster>)this.returnObj(this.SystemModel.getDeviceList(pShowDel, pDeviceNo));
        }

        [HttpGet]
        public List<DB_RCS_SYS_PARAMS> ParamData(string p_model, string p_group, string P_ID = "")
        {
            return (List<DB_RCS_SYS_PARAMS>)this.returnObj(this.SystemModel.ParamData(p_model, p_group, P_ID));
        }

        [JwtAuthActionFilter]
        [HttpPost]
        public string saveParamSetting(USER_FORM_BODY form)
        {
            return (string)this.returnObj(this.SystemModel.saveParamSetting(form));
        }

        [JwtAuthActionFilter]
        [HttpPost]
        public string saveParamSettingSort(Param_Setting_FORM_BODY form)
        {
            return (string)this.returnObj(this.SystemModel.saveParamSettingSort(form));
        }

        [HttpGet]
        public List<VENTILATORGroupModel> GetVENTILATOR(string sDate, string eDate)
        {
            return (List<VENTILATORGroupModel>)this.returnObj(this.SystemModel.GetVENTILATOR(sDate, eDate));
        }

        [HttpGet]
        public List<DB_RCS_VENTILATOR_SETTINGS> GetSelectList()
        {
            return (List<DB_RCS_VENTILATOR_SETTINGS>)this.returnObj(this.SystemModel.GetSelectList());
        }

        [HttpGet]
        public VENTILATORViewModel GetVENTILATORCheckList(string id)
        {
            return (VENTILATORViewModel)this.returnObj(this.SystemModel.GetVENTILATORCheckList(id));
        }


        [JwtAuthActionFilter]
        [HttpPost]
        public VENTILATORModel saveVENTILATORSCHEDULING(VENTILATOR_DETAIL_FORM_BODY form)
        {
            return (VENTILATORModel)this.returnObj(this.SystemModel.saveVENTILATORSCHEDULING(form));
        }

        [JwtAuthActionFilter]
        [HttpPost]
        public string delVENTILATORSCHEDULING(VENTILATOR_DETAIL_FORM_BODY form)
        {
            return (string)this.returnObj(this.SystemModel.delVENTILATORSCHEDULING(form));
        }

        [JwtAuthActionFilterAttribute(notVerification = true)]
        [HttpPost]
        public HttpResponseMessage exportVENTILATORExcel(VENTILATORExcel_FORM_BODY form)
        {
            string actionName = "exportExcel";
            HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.BadRequest);
            response.StatusCode = HttpStatusCode.OK;
            try
            {
                var sql = string.Concat("SELECT ROW_NUMBER() OVER(ORDER BY a.V_ID DESC) AS row_no, a.*, b.* FROM RCS_VENTILATOR_SCHEDULING_CHECKLIST as a LEFT JOIN RCS_VENTILATOR_SETTINGS as b on a.MV_NO = b.DEVICE_NO WHERE DATASTATUS = '1' AND RECORD_DATE BETWEEN @sDate AND @eDate  ");

                Dapper.DynamicParameters dp = new Dapper.DynamicParameters();

                int getMonth = Int16.Parse(form.month) + 1;
                string monthString = getMonth.ToString();

                if (getMonth < 10)
                {
                    monthString = "0" + monthString;
                }

                string sDate = DateHelper.Parse(form.year + "-" + monthString + "-01").ToString("yyyy-MM-dd");
                string eDate = DateHelper.Parse(form.year + "-" + monthString + "-01").AddMonths(1).AddDays(-1).ToString("yyyy-MM-dd");
                dp.Add("sDate", sDate);
                dp.Add("eDate", eDate);

                var list = this.DBLink.DBA.getSqlDataTable(sql, dp);

                if (list != null && list.Rows.Count > 0)
                {
                    string title = form.year + "年" + monthString + "月呼吸器記錄表";
                    NPOITool NPOITool = new Models.NPOITool();
                    string pBindColName = "row_no,RECORD_DATE,MV_NO,V_TYPE,V_STATUS,DEVICE_MODEL";
                    string pColTitleName = "編號,保養日期, 呼吸器編號, 檢核單類型, 檢核單狀態, 呼吸器機型";
                    byte[] buf = NPOITool.ExportExcelTable(list, title, title + ".xls", "sheet1", pBindColName, pColTitleName);

                    if (buf != null)
                    {
                        var contentLength = buf.Length;
                        var statuscode = HttpStatusCode.OK;
                        response = Request.CreateResponse(statuscode);
                        response.Content = new StreamContent(new MemoryStream(buf));
                        response.Content.Headers.ContentType = new MediaTypeHeaderValue("application/vnd.ms-excel");
                        response.Content.Headers.ContentLength = contentLength;
                        ContentDispositionHeaderValue contentDisposition = null;
                        if (ContentDispositionHeaderValue.TryParse("inline; filename=" + title + ".xls", out contentDisposition))
                        {
                            response.Content.Headers.ContentDisposition = contentDisposition;
                        } 
                    }
                    else
                    {
                         
                        response.Content = new StringContent("程式發生錯誤，請洽資訊人員! 202"); 
                    }
                }
                else
                {
                    response.Content = new StringContent("查無資料!"); 
                }
            }
            catch (Exception ex)
            {
                response.Content = new StringContent("程式發生錯誤，請洽資訊人員! 212"); 
                LogTool.SaveLogMessage(ex, actionName, this.csName);
            }
            return response;
        }

    }
}

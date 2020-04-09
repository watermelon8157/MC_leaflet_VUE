using RCS.Models;
using RCS_Data;
using RCS.Models.ViewModel;
using RCS_Data.Models.ViewModels;
using RCSData.Models;
using System;
using System.Collections.Generic;
using System.Web.Http;
using RCS_Data.Controllers.RT;
using System.Net.Http;
using Newtonsoft.Json;
using Com.Mayaminer;
using System.Data;
using System.Net;
using System.IO;
using System.Net.Http.Headers;
using System.Web;

namespace RCS.Controllers.WEBAPI
{ 
    public class RTController : BasicController
    {
        public RTViewModels _model { get; set; }
        protected RTViewModels model
        {
            get
            {
                if (this._model == null)
                {
                    this._model = new RTViewModels();
                }
                return this._model;
            }
        }

        /// <summary>
        /// 照護清單資料
        /// </summary>
        /// <param name="form"></param>
        /// <returns></returns>
        [JwtAuthActionFilterAttribute]
        [HttpPost]
        public List<PatientListItem> PatientList(FormRT form)
        {
            
            List<PatientListItem> pList = (List<PatientListItem>)this.returnObj(this.model.get_CareIList(form));
            foreach (PatientListItem item in pList)
            {
                if (MvcApplication.ipd_list.Exists(x => x.chart_no == item.chart_no && x.ipd_no == item.ipd_no))
                {
                    #region 轉床
                    IPDPatientInfo _pat_info = MvcApplication.ipd_list.Find(x => x.chart_no == item.chart_no && x.ipd_no == item.ipd_no);
                    if (!string.IsNullOrWhiteSpace(_pat_info.bed_no) && item.bed_no != _pat_info.bed_no)
                        item.new_bed_no = _pat_info.bed_no; 
                    if (!string.IsNullOrWhiteSpace(_pat_info.cost_code) && item.cost_code != _pat_info.cost_code)
                        item.new_cost_code = _pat_info.cost_code; 
                    if (!string.IsNullOrWhiteSpace(_pat_info.dept_code) && item.dept_code != _pat_info.dept_code)
                        item.new_dept_code = _pat_info.dept_code;
                    if (!string.IsNullOrWhiteSpace(_pat_info.dept_desc) && item.dept_desc != _pat_info.dept_desc)
                        item.new_dept_desc = _pat_info.dept_desc;
                    if (!string.IsNullOrWhiteSpace(_pat_info.MDRO_MARK) && item.MDRO_MARK != _pat_info.MDRO_MARK)
                        item.MDRO_MARK = _pat_info.MDRO_MARK;
                    if (!string.IsNullOrWhiteSpace(_pat_info.new_ipd_no) && item.ipd_no != _pat_info.new_ipd_no)
                        item.new_ipd_no = _pat_info.new_ipd_no;
                   
                    #endregion 
                }
                ////測試轉單位樣式
                //if (RCS.Controllers.BaseController.isDebuggerMode)
                //{
                //    item.new_bed_no = "aaa";
                //    item.new_cost_desc = "bbb";
                //    item.new_cost_code = "ccc";
                //    item.new_vs_doc = "ddd";
                //    item.new_vs_id = "eee";
                //    item.new_dept_code = "fff";
                //    item.new_dept_desc = "ggg";
                //}
            } 

            return pList;
        }


        /// <summary>
        /// 選擇病患資料
        /// </summary>
        /// <param name="form"></param>
        /// <returns></returns>
        [JwtAuthActionFilterAttribute]
        [HttpPost]
        public IPDPatientInfo SelectPatientInfo(FormRT form)
        {
            RESPONSE_MSGLIST rm = new RESPONSE_MSGLIST();
            PatientListItem pItem = new PatientListItem();
            if (!string.IsNullOrWhiteSpace(form.join_json))
            {
                pItem = Newtonsoft.Json.JsonConvert.DeserializeObject<PatientListItem>(form.join_json);
                List<PatientListItem> pList = (List<PatientListItem>)this.returnObj(this.model.get_CareIList(form, null, pItem.chart_no, pItem.ipd_no)); 
                if (pList.Count > 0)
                {
                    pItem = pList[0];
                } 
            } 
            return pItem;

        }

        /// <summary>
        /// 取得病床單位
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [JwtAuthActionFilter(notVerification = true)]
        public List<BedArea> HisBedAreaList()
        {
            return this.webmethod.getHisBedAreaList(this.hospFactory.webService.HisBedAreaList());
        }

        /// <summary>
        /// 搜尋病人資料 病歷號
        /// </summary>
        /// <param name="form"></param>
        /// <returns></returns>
        [JwtAuthActionFilterAttribute]
        [HttpPost]
        public List<PatientListItem> SearchPatient(FormSearchPatientList form)
        {
            List<PatientListItem> pList = null;
            if (!string.IsNullOrWhiteSpace(form.chart_no))
            {
                pList = (List<PatientListItem>)this.returnObj(this.model.SearchPatient(
                    this.hospFactory.webService.HISPatientHistory(), 
                    this.hospFactory.webService.HISPatientInfo(),
                    this.hospFactory.webService.HISGetPatient_ER_OPD_LIST(),
                    form));
            }
            else
            {
                this.throwHttpResponseException("請輸入病歷號!"); 
            } 
            return pList;
        }

        /// <summary>
        /// 搜尋病人資料  床號/記錄註記
        /// </summary>
        /// <param name="form"></param>
        /// <returns></returns>
        [JwtAuthActionFilterAttribute]
        [HttpPost]
        public List<PatientListItem> SearchBedList(FormSearchPatientList form)
        {
            List<PatientListItem> pList = new List<PatientListItem>();
            if (!string.IsNullOrWhiteSpace(form.area_code))
            {
                List<IPDPatientInfo> temp = this.webmethod.getPatientInfoListByAreaCode(this.hospFactory.webService.AreaPatientList(), form.area_code) ;
                pList = Newtonsoft.Json.JsonConvert.DeserializeObject<List<PatientListItem>>(Newtonsoft.Json.JsonConvert.SerializeObject(temp));
            }
            else
            {
                this.throwHttpResponseException("請選擇單位!");
            }

            return pList;
        }

        /// <summary>
        /// 新增病人至照護清單
        /// </summary>
        /// <param name="form"></param>
        /// <returns></returns>
        [JwtAuthActionFilterAttribute]
        [HttpPost]
        public List<string> JoinCareItem(FormRT form)
        {
            RESPONSE_MSG rm = new RESPONSE_MSG(); 
            List<string> msgList = new List<string>(); 
            List<IPDPatientInfo> pList = new List<IPDPatientInfo>();
            if (!string.IsNullOrWhiteSpace(form.join_json))
            { 
                pList = Newtonsoft.Json.JsonConvert.DeserializeObject<List<IPDPatientInfo>>(form.join_json);   
            }
            
            msgList = (List<string>)this.returnObj(this.model.JoinCareItem(this.hospFactory.webService.HISPatientInfo()
                , this.hospFactory.webService.HISGetPatient_ER_OPD_LIST()
                , ref pList, form.payload));
            foreach (var patInfo in pList)
            {
                if (MvcApplication.ipd_list.Exists(x => x.chart_no == patInfo.chart_no && x.ipd_no == patInfo.ipd_no))
                {
                    // patInfo
                    #region 更新資料
                    MvcApplication.ipd_list.Find(x => x.chart_no == patInfo.chart_no && x.ipd_no == patInfo.ipd_no).bed_no = patInfo.bed_no; 
                    MvcApplication.ipd_list.Find(x => x.chart_no == patInfo.chart_no && x.ipd_no == patInfo.ipd_no).cost_code = patInfo.cost_code;
                    MvcApplication.ipd_list.Find(x => x.chart_no == patInfo.chart_no && x.ipd_no == patInfo.ipd_no).dept_code = patInfo.dept_code;
                    MvcApplication.ipd_list.Find(x => x.chart_no == patInfo.chart_no && x.ipd_no == patInfo.ipd_no).dept_desc = patInfo.dept_desc;  
                    #endregion
                }
            }
            return msgList;
        }


        /// <summary>
        /// 移除病人
        /// </summary>
        /// <param name="form"></param>
        /// <returns></returns>
        [JwtAuthActionFilterAttribute]
        [HttpPost]
        public List<string> DeleteItem(FormDeleteItem form)
        {
            RESPONSE_MSG rm = new RESPONSE_MSG();
            List<string> pList = new List<string>();
            if (!string.IsNullOrWhiteSpace(form.delete_json))
            { 
                pList = Newtonsoft.Json.JsonConvert.DeserializeObject<List<string>>(form.delete_json); 
            } 
            return (List<string>)this.returnObj(this.model.DeleteItem(pList));
        }

        /// <summary>
        /// 修改病人
        /// </summary>
        /// <param name="form"></param>
        /// <returns></returns> 
        [JwtAuthActionFilterAttribute]
        [HttpPost]
        public string EditPat(FormEditPat form)
        {
            return (string)this.returnObj(this.model.EditPat(form));
        }

        /// <summary>
        /// 列印病患清單
        /// </summary>
        /// <param name="Data"></param>
        /// <returns></returns>
        [JwtAuthActionFilterAttribute(notVerification = true)]
        [HttpPost]
        public HttpResponseMessage exportExcel(RTexportExcel Data)
        {
            string actionName = "exportExcel";
            HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.BadRequest);
            response.StatusCode = HttpStatusCode.OK;
            try
            {
                List<PatientListItem> pli = new List<PatientListItem>();
                Data.jsonData = HttpUtility.UrlDecode(Data.jsonData);
                pli = JsonConvert.DeserializeObject<List<PatientListItem>>(Data.jsonData);
                if (pli.Count > 0)
                {
                    this.model.getPatDeatailData(ref pli);
                    if (pli != null && pli.Count > 0)
                    {
                        pli.ForEach(x => { x.setOnMode("\r\n"); x.o2_device = string.IsNullOrWhiteSpace(x.o2_device) ? "" : x.o2_device.Replace("&#10;", "\r\n"); });

                        ExcelSetting em = new ExcelSetting();
                        em.bindColName = "bed_no,dept_code,vs_doc,patient_name,chart_no,diagnosis_code,device,on_mode,o2_device,memo";
                        em.colTitleName = "床號,科別,主治醫生,姓名,病歷號,診斷,Ventilator Type,on機時間,O2 device,備註";
                        em.titleName = "病人清單";
                        em.sheetName = "sheet1";
                        em.FileName = "病人清單.xls";
                        em.exportActionName = "ExportExcel_RT_Index";
                        exportFile exportFile = new exportFile(em.FileName);
                        DataTable dt = pli.ToDataTable();
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
                            response.Content = new StringContent("程式發生錯誤，請洽資訊人員! 247");
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
                response.Content = new StringContent("程式發生錯誤，請洽資訊人員! 254");
                LogTool.SaveLogMessage(ex, actionName, GetLogToolCS.RTController);
            }
            return response;
        }

        /// <summary>
        /// 顯示病人入院清單
        /// </summary>
        /// <param name="form"></param>
        /// <returns></returns> 
        [JwtAuthActionFilterAttribute]
        [HttpPost]
        public List<PatientListItem> showSelectList(FormRT form)
        {
            return (List<PatientListItem>)this.model.SelectPatientInfoListForSelect("", form.pat_info.chart_no);
        }


        /// <summary>
        /// 切換病人資料
        /// </summary>
        /// <param name="form"></param>
        /// <returns></returns>
        [JwtAuthActionFilterAttribute]
        [HttpPost]
        public IPDPatientInfo SelectPatientInfoWithSelect(FormRT form)
        {
            RESPONSE_MSGLIST rm = new RESPONSE_MSGLIST();
            PatientListItem pItem = new PatientListItem();
            if (!string.IsNullOrWhiteSpace(form.join_json))
            {
                pItem = Newtonsoft.Json.JsonConvert.DeserializeObject<PatientListItem>(form.join_json);
                pItem = this.model.SelectPatientInfo(pItem.ipd_no, pItem.chart_no); 
            }
            return pItem;

        }
    }


    public class RTexportExcel  
    {  
        public string jsonData { get; set; } 

    }




}

using Com.Mayaminer;
using iTextSharp.text;
using iTextSharp.text.pdf;
using iTextSharp.tool.xml;
using RCS_Data;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Text.RegularExpressions;
using System.Data;
using RCS.Models;
using RCS.Models.DB;
using Newtonsoft.Json;
using mayaminer.com.library;

namespace RCS.Controllers {
    public class MeasuresFormController : BaseController {

        private Measures MeasuresFormModel;
        public MeasuresFormController()
        {
            MeasuresFormModel = new Measures();
        }

        public ActionResult Index() {
            return View("List");
        }

        public ActionResult showNew(string tempid = "") {
            if (tempid != "") {
                MeasuresForm model = getData(tempid);
                if (model.hasData)
                    ViewData["Data"] = model;
                else
                    ViewData["Data"] = null;
            } else {
                ViewData["Data"] = new MeasuresForm();
            }
            return View("Index");
        }

        [HttpPost]
        public string UserList()
        {
            DataTable MeaDt = MeasuresFormModel.getUserMeasuresList();
            List<Dictionary<string, string>> Dt = new List<Dictionary<string, string>>();
            Dictionary<string, string> Temp = null;
            if (MeaDt != null && MeaDt.Rows.Count > 0)
            {
                foreach (DataRow MeaDr in MeaDt.Rows)
                {
                    string name = MeaDr["item_value"].ToString();
                    string insopname = MeaDr["CREATE_NAME"].ToString();
                    string insdt = MeaDr["CREATE_DATE"].ToString();
                    string modopname = MeaDr["MODIFY_NAME"].ToString();
                    string moddt = MeaDr["MODIFY_DATE"].ToString();
                    Temp = new Dictionary<string, string>();
                    Temp.Add("tempid", MeaDr["MEASURE_ID"].ToString());
                    Temp.Add("name", name);
                    Temp.Add("insopname", modopname == "" ? insopname : modopname);
                    Temp.Add("insdt", moddt == "" ? insdt : moddt);
                    Dt.Add(Temp);
                }
            }
            return JsonConvert.SerializeObject(Dt);
        }

        private MeasuresForm getData(string tempid) {
            MeasuresForm model = new MeasuresForm();
            Dictionary<string, string> temp = new Dictionary<string, string>();
            string Cnt = MeasuresFormModel.getCountMeasures(tempid);
            if (!string.IsNullOrWhiteSpace(Cnt) && Cnt == "1") {
                DataTable Dt = MeasuresFormModel.getMeasuresDetailColumns(tempid);
                if (Dt != null && Dt.Rows.Count > 0)
                {
                    model = new MeasuresForm();
                    foreach (DataRow Dr in Dt.Rows)
                    {
                        string name = Dr["item_name"].ToString();
                        string value = Dr["item_value"].ToString();
                        temp.Add(name, value);
                        PropertyInfo propertyInfo = model.GetType().GetProperty(name);
                        propertyInfo.SetValue(model, Convert.ChangeType(value, propertyInfo.PropertyType), null);
                    }
                }
                PropertyInfo[] props = null;
                props = model.GetType().GetProperties();
                foreach (var pi in props) {
                    string name = pi.Name;
                    if (temp.Keys.Contains(name)) pi.SetValue(model, temp[name], null);
                }
                model.hasData = true;
            } else {
                PropertyInfo[] props = null;
                props = model.GetType().GetProperties();
                foreach (var pi in props) {
                    pi.SetValue(model, "", null);
                }
            }
            return model;
        }

        public JsonResult save_MeasuresForm(MeasuresForm model) {
            object obj = new object();
            model.setSaveData();
            bool isModify = false;
            if (model.tempid != null && model.tempid != "") isModify = true;            
            string user_name = user_info.user_name;
            string user_id = user_info.user_id;           
            bool Success = false;            
            try {
                //新增RT_USER_RECORD_MASTER              
                string fee_no = "00000000";
                string date = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                DataTable DtlDt = MeasuresFormModel.getMeasuresDetail("WHERE 1<>1");
                DataRow DtlDr;
                DataTable MstDt = new DataTable();
                DataRow MstDr;
                if (isModify) {
                    MstDt = MeasuresFormModel.getMeasuresMaster(string.Format("WHERE MEASURE_ID = {0}", SQLDefend.SQLString(model.tempid)));
                    MstDr = MstDt.Rows[0];
                }
                else
                {
                    MstDt = MeasuresFormModel.getMeasuresMaster("WHERE 1<>1");
                    MstDr = MstDt.NewRow();
                }
                MstDr["IPD_NO"] = fee_no;
                MstDr["CHART_NO"] = "00000000";
                if (isModify) {
                    MstDr["MEASURE_ID"] = model.tempid;
                    MstDr["MODIFY_NAME"] = user_name;
                    MstDr["MODIFY_DATE"] = date;
                    MstDr["MODIFY_ID"] = user_id; 
                } else {
                    model.tempid = DateTime.Now.ToString("yyyyMMddHHmmss") + fee_no;
                    MstDr["MEASURE_ID"] = model.tempid;
                    MstDr["CREATE_NAME"] = user_name;
                    MstDr["CREATE_DATE"] = date;
                    MstDr["CREATE_ID"] = user_id;
                    MstDt.Rows.Add(MstDr);
                }
                this.DBA.BeginTrans();
                mayaminer.com.jxDB.dbResultMessage rc = this.DBA.UpdateResult(MstDt, GetTableName.RT_MEASURES_FORM_MASTER.ToString());
                if (rc.State == mayaminer.com.jxDB.enmDBResultState.Success)
                    Success = true;
                else
                    Success = false;

                if (Success) {                 
                    if (MeasuresFormModel.deleteMeasureDetail(string.Format("WHERE MEASURE_ID = {0}", SQLDefend.SQLString(model.tempid))))
                    {
                        //新增RT_USER_RECORD_DETAIL
                        PropertyInfo[] props = null;
                        props = model.GetType().GetProperties();                        
                        foreach (var pi in props)
                        {
                            DtlDr = DtlDt.NewRow();
                            string name = pi.Name;
                            object value = pi.GetValue(model, null);
                            DtlDr["item_name"] = name;
                            if (value != null) DtlDr["item_value"] = value.ToString();
                            DtlDr["MEASURE_ID"] = model.tempid;
                            DtlDt.Rows.Add(DtlDr);
                        }
                        rc = this.DBA.UpdateResult(DtlDt, GetTableName.RT_MEASURES_FORM_DETAIL.ToString());
                        if (rc.State == mayaminer.com.jxDB.enmDBResultState.Success)
                            Success = true;
                        else                        
                            Success = false;
                    }
                    else
                        Success = false;
                }
                else
                    Success = false;
            } catch (Exception ex) {
                LogTool.SaveLogMessage(ex, "UserRecord");
                Success = false;
            } finally {
                if (Success) {
                    this.DBA.Commit();
                    obj = new {
                        success = true,
                        msg = "存檔成功!"
                    };
                } else {
                    this.DBA.Rollback();
                    obj = new {
                        success = false,
                        msg = "存檔失敗!"
                    };
                }
            }
            return Json(obj);
        }

        //刪除
        [HttpPost]
        public bool DelMeasuresForm(string JsonList) {
            List<Dictionary<string, string>> DtList = JsonConvert.DeserializeObject<List<Dictionary<string, string>>>(JsonList);
            bool Success = false;
            if (DtList.Count > 0) {
                Success = true;          
                try {
                    this.DBA.BeginTrans();
                    foreach (var Dt in DtList) {
                        if (Success) {
                            if (MeasuresFormModel.deleteMeasureMaster(string.Format("WHERE MEASURE_ID = {0}",SQLDefend.SQLString(Dt["tempid"]))))
                                Success = true;
                            else
                                Success = false;
                        }

                        if (Success) {
                            if (MeasuresFormModel.deleteMeasureDetail(string.Format("WHERE MEASURE_ID = {0}", SQLDefend.SQLString(Dt["tempid"]))))
                                Success = true;
                            else
                                Success = false;
                        }
                    }
                } catch (Exception ex) {
                    LogTool.SaveLogMessage(ex, "SystemManage");
                    Success = false;
                } finally {
                    if (!Success)                    
                        this.DBA.Rollback();                    
                    else
                        this.DBA.Commit();
                }
            }
            return Success;
        }

        public ActionResult showMeasuresForm(string tempid = "") {
            MeasuresForm model = getData(tempid);
            if (model.hasData)
                model.setExportData();
            else {
                model = new MeasuresForm();
                model.setNullData();
            }
            ViewBag.css = false;
            setDownLine(ref model);
            ViewData["Data"] = model;
            return View("_MeasuresForm");
        }

        public void setDownLine(ref  MeasuresForm pModel) {
            PropertyInfo[] props = null;
            props = pModel.GetType().GetProperties();
            foreach (var pi in props) {
                System.Type s = pi.PropertyType;
                string name = s.Name;
                string objName = pi.Name;
                bool isOk = checkObj(objName);
                string str = setLenghth(objName);
                if (name == "String" && isOk) {
                    object val = pi.GetValue(pModel, null);
                    if (val == null || val.ToString() == "")
                        pi.SetValue(pModel, str, null);
                }
            }
        }

        private bool checkObj(string pName) {
            bool isOk = true;
            switch (pName) {
                case "chkAd":
                    isOk = false;
                    break;
                default:
                    break;
            }
            return isOk;
        }

        private string setLenghth(string pName) {
            string str = "";
            //int len = 2;
            switch (pName) {
                case "assess":
                    str = "____________";
                    break;
                case "assess_person":
                    str = "____________";
                    break;
                case "job":
                    str = "____________";
                    break;
                case "assess_date":
                    str = "____________";
                    break;
                case "check":
                    str = "____________";
                    break;
                case "check_person":
                    str = "____________";
                    break;
                case "check_job":
                    str = "____________";
                    break;
                case "check_date":
                    str = "____________";
                    break;
                case "name":
                    str = "________";
                    break;
                case "id_num":
                    str = "________";
                    break;
                case "birth":
                    str = "________";
                    break;
                case "oldManual_txt":
                    str = "________";
                    break;
                case "contactMan":
                    str = "_____";
                    break;
                case "withRelationship":
                    str = "_____";
                    break;
                case "contactPhone":
                    str = "______";
                    break;
                case "contactCellPhone":
                    str = "______";
                    break;
                case "yyy":
                    str = "______";
                    break;
                case "mm":
                    str = "______";
                    break;
                case "dd":
                    str = "______";
                    break;
                case "height":
                    str = "______";
                    break;
                case "weight":
                    str = "______";
                    break;
                case "DrawDate_1":
                    str = "________";
                    break;
                case "O2density_1":
                    str = "________";
                    break;
                case "ph_1":
                    str = "________";
                    break;
                case "PaCO2_1":
                    str = "____";
                    break;
                case "PaO2_1":
                    str = "____";
                    break;
                case "HCO3_1":
                    str = "____";
                    break;
                case "SaO2_1":
                    str = "____";
                    break;
                case "gasDrawDate":
                    str = "________";
                    break;
                case "gas_ph":
                    str = "____";
                    break;
                case "gas_PaCO2":
                    str = "____";
                    break;
                case "gas_PaO2":
                    str = "____";
                    break;
                case "gas_HCO3":
                    str = "____";
                    break;
                case "gas_SaO2":
                    str = "____";
                    break;
                case "breathVoice_txt":
                    str = "________";
                    break;
                case "breathingStatus_txt":
                    str = "________";
                    break;
                case "O2machine_txt_4":
                    str = "________";
                    break;
                case "duplexBreathMachine_txt_4":
                    str = "________";
                    break;
                case "simplexBreathMachine_txt_4":
                    str = "________";
                    break;
                case "suggest":
                    str = "________";
                    break;
                case "medical_tool_txt_5":
                    str = "________";
                    break;
                default:
                    break;
            }
            return str;
        }

        public ActionResult showMeasuresForm2(string tempid = "") {
            MeasuresForm model = getData(tempid);
            if (model.hasData)
                model.setExportData();
            else {
                model = new MeasuresForm();
                model.setNullData();
            }
            setDownLine(ref model);
            ViewData["Data"] = model;
            ViewBag.css = true;
            return View("_MeasuresForm");
        }

        /// <summary>
        /// 執行此Url，下載PDF檔案
        /// </summary>
        /// <returns></returns>
        public ActionResult DownloadPdf(pdfModel data) {
            if (data == null) {
                data = new pdfModel();
            }
            exportFile efm = new exportFile("DownloadPdf.pdf");
            MeasuresFormPDFDocSetting ds = new MeasuresFormPDFDocSetting();
            return efm.exportPDF(data.HtmlStr, ds);
        }

    }
}

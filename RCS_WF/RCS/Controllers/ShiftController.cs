using Com.Mayaminer;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RCS_Data;
using RCS.Models;
using RCS.Models.DB;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Data;
using mayaminer.com.library;
using mayaminer.com.jxDB;
using RCS.Models.ViewModel;
using RCSData.Models;
using RCS_Data.Controllers.RT;
using RCS_Data.Models.ViewModels;

namespace RCS.Controllers {
    public class ShiftController : BaseController {

        string csName { get { return "ShiftController"; } }

        private Shift ShiftModel;
        public ShiftController()
        {
            ShiftModel = new Shift();
        }

        #region 交班作業

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public bool checknowHour() {
            DateTime pDate = DateTime.Now;
            int hour = pDate.Hour;
            if (hour >= 8 && hour <= 20)
                return true;
            else
                return false;
        }


        //交班作業首頁
        [HttpPost]
        public ActionResult CheckList(string pList) {
            checkListViewer checkListViewer = new Models.checkListViewer();
            checkListViewer.UserName = user_info.user_name;
            List<Dictionary<string, string>> Dt = new SystemManage().UserMaintain_List(true);
            checkListViewer.UserList.Add(new SelectListItem() { Text ="請選擇", Value = "" });
            foreach (Dictionary<string, string> dic in Dt)
            {
                if (dic["P_GROUP"] != "doctor")
                    checkListViewer.UserList.Add(new SelectListItem() { Text = dic["P_NAME"], Value = dic["P_VALUE"] });
            }
            return View(checkListViewer);
        }


        /// <summary>
        /// FromView 依選擇的PtList抓取交班資料
        /// </summary>
        /// <param name="PtList">交班清單</param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult CheckListDetail(string PtList) {
            List<RCS_RT_ISBAR_SHIFT> rris = new List<RCS_RT_ISBAR_SHIFT>();
            try
            {
                if (!string.IsNullOrWhiteSpace(PtList))
                {
                    List<IPDPatientInfo> PtList_ = JsonConvert.DeserializeObject<List<IPDPatientInfo>>(PtList);
                    string ipd_noStr = "";
                    if (PtList_.Count > 0)
                    {
                        //取得批價序號
                        foreach (IPDPatientInfo item in PtList_)
                        {
                            //ShiftCheck(item);
                            ipd_noStr += SQLDefend.SQLString(item.ipd_no) + ",";
                            //自動產生暫存資料....
                            this.ShiftCheck(item, true);
                        }
                        if (ipd_noStr.Length > 0)
                        {
                            ipd_noStr = ipd_noStr.TrimEnd(',');
                             
                            DataTable Dt = ShiftModel.getShiftList(ipd_noStr);
                            foreach (DataRow Dr in Dt.Rows)
                            {
                                bool FillImmediateData = true;
                                string ipd_no = Dr["ipd_no"].ToString().Trim();
                                string CHART_NO = Dr["CHART_NO"].ToString().Trim();
                                if (rris.Exists(x => x.IPD_NO == ipd_no && x.chart_no == CHART_NO)) continue;
                                RCS_RT_ISBAR_SHIFT TmpShift = new RCS_RT_ISBAR_SHIFT()
                                {
                                    S_ID = Dr["ISBAR_ID"].ToString().Trim(),
                                    IPD_NO = ipd_no,
                                    chart_no = CHART_NO,
                                    I_VALUE = Dr["I_VALUE"].ToString().Trim(),
                                    S_VALUE = JsonConvert.DeserializeObject<List<RCS_RT_ISBAR_SHIFT_S_VALUE>>(Dr["S_VALUE"].ToString().Trim()),
                                    B_VALUE = JsonConvert.DeserializeObject<List<PatOrder>>(Dr["B_VALUE"].ToString().Trim()),
                                    A_VALUE = JsonConvert.DeserializeObject<RCS_A_VALUE>(Dr["A_VALUE"].ToString().Trim()),
                                    R_VALUE = JsonConvert.DeserializeObject<List<JSON_DATA>>(Dr["R_VALUE"].ToString().Trim()),
                                    CREATE_ID = Dr["CREATE_ID"].ToString().Trim(),
                                    CREATE_NAME = Dr["CREATE_NAME"].ToString().Trim(),
                                    CREATE_DATE = Dr["CREATE_DATE"].ToString().Trim(),
                                    SHIFT_ID = Dr["SHIFT_ID"].ToString().Trim(),
                                    SHIFT_NAME = Dr["SHIFT_NAME"].ToString().Trim(),
                                    SHIFT_DATE = Dr["SHIFT_DATE"].ToString().Trim(),
                                    STATUS = Dr["STATUS"].ToString().Trim(),
                                    trUR_ID = false.ToString(),
                                    B_VALUE_1 = Dr["B_VALUE_1"].ToString().Trim().Replace("\n", "&lt;p&gt;"),
                                    B_VALUE_2 = Dr["B_VALUE_2"].ToString().Trim(),
                                    bed_no = Dr["BED_NO"].ToString().Trim(),
                                    loc = Dr["LOC"].ToString().Trim(),
                                    type_mode = Dr["TYPE_MODE"].ToString().Trim(),
                                    hasEditRWC = new MODEL_RCS_WEANING_ASSESS_CHECKLIST().hasEdithasTodayRWC(CHART_NO, ipd_no)
                                };
                                if (FillImmediateData)
                                {
                                    //2016/08/22 開會，決定來源與交班表頁籤一致
                                    //將即時資訊帶入第一筆資料
                                    TmpShift = ShiftReloadInfo(TmpShift, PtList_.Find(x => x.ipd_no == ipd_no));
                                    FillImmediateData = false;
                                }
                                rris.Add(TmpShift);
                                TmpShift = null;
                            }
                            rris = rris.OrderBy(x => x.bed_no).ToList();
                            return Json(rris);
                        }
                        return Json(rris);
                    }
                }
            }
            catch (Exception ex)
            {
                LogTool.SaveLogMessage(ex, "CheckListDetail", GetLogToolCS.ShiftController);
            }
            return Json(rris);
        }

        /// <summary> ForView 驗證帳密 </summary>
        /// <returns>bool</returns>
        [HttpPost]
        public bool CheckPwd(string UserNo, string UserPwd) {
            WebServiceResponse wr = new WebServiceResponse();
            UserInfo ui = new UserInfo();
            ui = ui.getUserInfo(this.hospFactory.webService.HisLoginUser(), UserNo, UserPwd);
            if (ui != null && ui.hasUserData)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary> ForView 交班 </summary>
        /// <param name="UserNo">交班者</param>
        /// <param name="PtList">交接PtList</param>
        /// <returns>bool</returns>
        [HttpPost]
        public bool SaveShiftHandOver(string UserNo, string UserName, string PtList) {
            bool Success = true;
            string actionName = "SaveShiftHandOver";
            PtList = HttpUtility.UrlDecode(PtList);
            if (!string.IsNullOrWhiteSpace(PtList)) {
                List<RCS_RT_ISBAR_SHIFT> PtList_ = JsonConvert.DeserializeObject<List<RCS_RT_ISBAR_SHIFT>>(PtList);
                List<DataTable> ExcuteSchList = new List<DataTable>();
                List<DataTable> ExcuteShiList = new List<DataTable>();
                if (PtList_.Count > 0) {
                    try {
                    
                        List<string> strIpdno = PtList_.Select(x=>x.IPD_NO).ToList();
                        List<string> strS_ID = PtList_.Select(x => x.S_ID).ToList();
                        string sql = "SELECT * FROM " + GetTableName.RCS_RT_CARE_SCHEDULING + " WHERE RT_ID in('" + UserNo + "','" + user_info.user_id + "') AND IPD_NO in('" + string.Join("','", strIpdno) + "') AND TYPE_MODE = 'C'";
                      
                        DataTable dtSch = this.DBA.getSqlDataTable(sql);
                        if (!BaseModel.DTNotNullAndEmpty(dtSch))
                        {
                            LogTool.SaveLogMessage(this.DBA.LastError, actionName, GetLogToolCS.ShiftController);
                            Success = false;
                        }
                        sql = "SELECT * FROM " + GetTableName.RCS_RT_ISBAR_SHIFT + " WHERE ISBAR_ID in('" + string.Join("','", strS_ID) + "')";
                        
                        DataTable dtShi = this.DBA.getSqlDataTable(sql);
                        if (!BaseModel.DTNotNullAndEmpty(dtShi))
                        {
                            LogTool.SaveLogMessage(this.DBA.LastError, actionName, GetLogToolCS.ShiftController);
                            Success = false;
                        }
                        if (Success)
                        {
                            
                            foreach (RCS_RT_ISBAR_SHIFT Pt in PtList_)
                            {
                                
                                //照護清單移轉
                                DataRow[] tempSch = dtSch.AsEnumerable().Where(x => x["IPD_NO"].ToString() == Pt.IPD_NO).ToArray();
                                
                                if (tempSch.Any())
                                {
                                    //判斷是否對方有此病患
                                    bool hasThisPt = false;
                                    foreach (DataRow dr in tempSch)
                                    {
                                        if (dr["RT_ID"].ToString() == UserNo)
                                            hasThisPt = true;
                                    }
                                    if (!hasThisPt)
                                    {
                                        dtSch.AsEnumerable().Where(x => x["RT_ID"].ToString().Trim() == user_info.user_id && x["IPD_NO"].ToString() == Pt.IPD_NO).SingleOrDefault()["BED_NO"] = Pt.bed_no;
                                        dtSch.AsEnumerable().Where(x => x["RT_ID"].ToString().Trim() == user_info.user_id && x["IPD_NO"].ToString() == Pt.IPD_NO).SingleOrDefault()["LOC"] = Pt.loc;
                                        dtSch.AsEnumerable().Where(x => x["RT_ID"].ToString().Trim() == user_info.user_id && x["IPD_NO"].ToString() == Pt.IPD_NO).SingleOrDefault()["TYPE_MODE"] = Pt.type_mode;
                                        dtSch.AsEnumerable().Where(x => x["RT_ID"].ToString().Trim() == user_info.user_id && x["IPD_NO"].ToString() == Pt.IPD_NO).SingleOrDefault()["RT_ID"] = UserNo;
                                    }
                                }

                                //紀錄交班
                                DataRow[] tempShi = dtShi.AsEnumerable().Where(x => x["ISBAR_ID"].ToString() == Pt.S_ID).ToArray();
                                
                                if (tempShi.Any())
                                {
                                    DataRow ShiDr = null;
                                    dtShi.AsEnumerable().Where(x => x["ISBAR_ID"].ToString() == Pt.S_ID).SingleOrDefault()["SHIFT_ID"] = UserNo;
                                    dtShi.AsEnumerable().Where(x => x["ISBAR_ID"].ToString() == Pt.S_ID).SingleOrDefault()["SHIFT_NAME"] = UserName;
                                    dtShi.AsEnumerable().Where(x => x["ISBAR_ID"].ToString() == Pt.S_ID).SingleOrDefault()["SHIFT_DATE"] = DateTime.Now.ToString("yyyy-MM-dd HH:mm");
                                    dtShi.AsEnumerable().Where(x => x["ISBAR_ID"].ToString() == Pt.S_ID).SingleOrDefault()["STATUS"] ="2";
                                    ShiDr = null;
                                }
                               
                            }
                           
                            //刪除自己的
                            //照護清單移轉
                            DataRow[] tempDr = dtSch.AsEnumerable().Where(x => x["RT_ID"].ToString() == user_info.user_id).ToArray();
                            if (tempDr.Any())
                            {
                                for (int i = 0; i <  dtSch.Rows.Count; i++)
                                {
                                    if(!DBNull.ReferenceEquals( dtSch.Rows[i]["RT_ID"],DBNull.Value) && dtSch.Rows[i]["RT_ID"].ToString().Trim() == user_info.user_id )
                                    {
                                        dtSch.Rows[i].Delete();
                                    }
                                }
                            }
                            try
                            {
                                this.DBA.BeginTrans();
                                dbResultMessage msg = this.DBA.UpdateResult(dtSch, GetTableName.RCS_RT_CARE_SCHEDULING.ToString());
                                if (msg.State != enmDBResultState.Success)
                                {
                                    LogTool.SaveLogMessage(msg.dbErrorMessage, actionName, GetLogToolCS.ShiftController);
                                }
                                else
                                {
                                    msg = this.DBA.UpdateResult(dtShi, GetTableName.RCS_RT_ISBAR_SHIFT.ToString());
                                    if (msg.State != enmDBResultState.Success)
                                    {
                                        LogTool.SaveLogMessage(msg.dbErrorMessage, actionName, GetLogToolCS.ShiftController);
                                    }
                                }
                                if (msg.State == enmDBResultState.Success)
                                    this.DBA.Commit();
                                else
                                    this.DBA.Rollback();
                               
                            }
                            catch (Exception ex)
                            {
                                LogTool.SaveLogMessage(ex, actionName, GetLogToolCS.ShiftController);
                                Success = false;
                                this.DBA.Rollback();
                            }
                        }
                        
                    } catch (Exception ex) {
                        LogTool.SaveLogMessage(ex, actionName, GetLogToolCS.ShiftController);
                        Success = false;
                    }
                }
            }
            return Success;
        }
       
        #endregion

        #region 交班joe版本

        /// <summary>
        /// 交班介面
        /// </summary>
        /// <returns></returns>
        public ActionResult DailyShift() {
            return View("DailyShiftBasic", true);
        }       

        /// <summary>
        /// 取得交班表頁籤資料
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public string GetShiftData() {
            List<RCS_RT_ISBAR_SHIFT> rris = new List<RCS_RT_ISBAR_SHIFT>();
            List<RCS_RT_ISBAR_SHIFT> temprris = new List<RCS_RT_ISBAR_SHIFT>();
            try
            {
                ShiftCheck(pat_info);//檢查是否需要新增一筆資料
                //取得該住院號下的已交班的記錄與當日RT的暫存紀錄
                DataTable Dt = ShiftModel.getShiftTemp(pat_info.ipd_no);
                foreach (DataRow Dr in Dt.Rows)
                {
                    rris.Add(new RCS_RT_ISBAR_SHIFT()
                    {
                        S_ID = Dr["ISBAR_ID"].ToString().Trim(),
                        IPD_NO = Dr["ipd_no"].ToString().Trim(),
                        I_VALUE = Dr["I_VALUE"].ToString().Trim(),
                        S_VALUE = JsonConvert.DeserializeObject<List<RCS_RT_ISBAR_SHIFT_S_VALUE>>(Dr["S_VALUE"].ToString().Trim()),
                        B_VALUE = JsonConvert.DeserializeObject<List<PatOrder>>(Dr["B_VALUE"].ToString().Trim()),
                        A_VALUE = JsonConvert.DeserializeObject<RCS_A_VALUE>(Dr["A_VALUE"].ToString().Trim()),
                        R_VALUE = JsonConvert.DeserializeObject<List<JSON_DATA>>(Dr["R_VALUE"].ToString().Trim()),
                        CREATE_ID = Dr["CREATE_ID"].ToString().Trim(),
                        CREATE_NAME = Dr["CREATE_NAME"].ToString().Trim(),
                        CREATE_DATE = Dr["CREATE_DATE"].ToString().Trim(),
                        SHIFT_ID = Dr["SHIFT_ID"].ToString().Trim(),
                        SHIFT_NAME = Dr["SHIFT_NAME"].ToString().Trim(),
                        SHIFT_DATE = Dr["SHIFT_DATE"].ToString().Trim(),
                        STATUS = Dr["STATUS"].ToString().Trim(),
                        chart_no = pat_info.chart_no,
                        B_VALUE_1 = Dr["B_VALUE_1"].ToString().Replace("\n", "&lt;p&gt;"),
                        B_VALUE_2 = Dr["B_VALUE_2"].ToString().Trim(),
                        B_VALUE_1_old = Dr["B_VALUE_1"].ToString().Replace("\n", "&lt;p&gt;"),
                        B_VALUE_2_old = Dr["B_VALUE_2"].ToString().Trim()
                    });
                    break;
                }
                rris[0] = ShiftReloadInfo(rris[0], pat_info);
            }
            catch (Exception ex)
            {
                LogTool.SaveLogMessage(ex, "GetShiftData");
            }
            return JsonConvert.SerializeObject(rris);
        }

        /// <summary>
        /// 取得上次交班紀錄
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public string GetLastShiftData(string shift_chart_no, string shift_IPD_NO, string shift_PAT_DATA_DATE)
        {
            List<RCS_RT_ISBAR_SHIFT> rris = new List<RCS_RT_ISBAR_SHIFT>();
            List<RCS_RT_ISBAR_SHIFT> temprris = new List<RCS_RT_ISBAR_SHIFT>();
            try
            {
                DataTable Dt = new DataTable();
                //取得該住院號下的已交班的記錄
                if (!String.IsNullOrWhiteSpace(shift_chart_no))
                {
                    Dt = ShiftModel.getLastShift(shift_chart_no, shift_IPD_NO, shift_PAT_DATA_DATE);

                }
                if (Dt.Rows != null && Dt.Rows.Count >0)
                {
                    foreach (DataRow Dr in Dt.Rows)
                    {
                        rris.Add(new RCS_RT_ISBAR_SHIFT()
                        {
                            B_VALUE_1 = Dr["B_VALUE_1"].ToString().Replace("\n", "&lt;p&gt;"),
                            B_VALUE_2 = Dr["B_VALUE_2"].ToString().Trim(),
                            B_VALUE_1_old = Dr["B_VALUE_1"].ToString().Replace("\n", "&lt;p&gt;"),
                            B_VALUE_2_old = Dr["B_VALUE_2"].ToString().Trim()
                        });
                        break;
                    }
                    rris[0] = ShiftReloadInfo(rris[0], pat_info);
                }                
            }
            catch (Exception ex)
            {
                LogTool.SaveLogMessage(ex, "GetLastShiftData");
            }
            return JsonConvert.SerializeObject(rris);
        }

        /// <summary>
        /// 檢查是否該建立新的交班資料
        /// </summary>
        /// <returns></returns>
        private void ShiftCheck(IPDPatientInfo pPatInfo,bool isShitWork = false)
        {
            RCS_RT_ISBAR_SHIFT rris = new RCS_RT_ISBAR_SHIFT();
            RESPONSE_MSG rm = new RESPONSE_MSG();
            SQLProvider SQL = new SQLProvider();
            try {
                //取得上一筆交班記錄                
                DataTable Dt = ShiftModel.getPrevShift(pPatInfo.ipd_no, user_info.user_id);
                if (Dt != null && Dt.Rows.Count > 0)
                {
                    DataRow dr = Dt.AsEnumerable().ToList().OrderByDescending(x => x["CREATE_DATE"]).First();
                    rris = (new RCS_RT_ISBAR_SHIFT() {
                        S_ID = dr["ISBAR_ID"].ToString().Trim(),
                        IPD_NO = dr["ipd_no"].ToString().Trim(),
                        I_VALUE = dr["I_VALUE"].ToString().Trim(),
                        S_VALUE = JsonConvert.DeserializeObject<List<RCS_RT_ISBAR_SHIFT_S_VALUE>>(dr["S_VALUE"].ToString().Trim()),
                        B_VALUE = JsonConvert.DeserializeObject<List<PatOrder>>(dr["B_VALUE"].ToString().Trim()),
                        A_VALUE = JsonConvert.DeserializeObject<RCS_A_VALUE>(dr["A_VALUE"].ToString().Trim()),
                        R_VALUE = JsonConvert.DeserializeObject<List<JSON_DATA>>(dr["R_VALUE"].ToString().Trim()),
                        CREATE_ID = dr["CREATE_ID"].ToString().Trim(),
                        CREATE_NAME = dr["CREATE_NAME"].ToString().Trim(),
                        CREATE_DATE = dr["CREATE_DATE"].ToString().Trim(),
                        SHIFT_ID = dr["SHIFT_ID"].ToString().Trim(),
                        SHIFT_NAME = dr["SHIFT_NAME"].ToString().Trim(),
                        SHIFT_DATE = dr["SHIFT_DATE"].ToString().Trim(),
                        STATUS = dr["STATUS"].ToString().Trim(),
                        B_VALUE_1 = dr["B_VALUE_1"].ToString().Trim().Replace("\n", "&lt;p&gt;"),
                        B_VALUE_2 = dr["B_VALUE_2"].ToString().Trim()
                    });
                }
            } catch (Exception ex) {
                LogTool.SaveLogMessage(ex, "GetShiftData");
            }
            //判斷是否新增一筆新資料：沒舊資料 || 已交班 || 或超過12小時的資料
            if (string.IsNullOrWhiteSpace(rris.S_ID) || rris.STATUS == "2")
            {
                //取得新的S_ID
                string strS_ID = SQL.GetFixedStrSerialNumber();
                try {
                    DataTable Dt = ShiftModel.getShift("WHERE 1<>1");
                    DataRow Dr = Dt.NewRow();
                    //若有舊資料需抄舊有資料
                    Dr["b_value_1"] = string.IsNullOrWhiteSpace(rris.B_VALUE_1) ? "" : rris.B_VALUE_1;
                    Dr["b_value_2"] = string.IsNullOrWhiteSpace(rris.B_VALUE_2) ? "" : rris.B_VALUE_2;
                    //更新即時資訊
                    rris = ShiftReloadInfo(rris, pPatInfo);
                    Dr["ISBAR_ID"] = strS_ID;
                    Dr["IPD_NO"] = pPatInfo.ipd_no;
                    Dr["I_VALUE"] = rris.I_VALUE;
                    Dr["S_VALUE"] = JsonConvert.SerializeObject(rris.S_VALUE);
                    Dr["B_VALUE"] = JsonConvert.SerializeObject(rris.B_VALUE);
                    Dr["A_VALUE"] = JsonConvert.SerializeObject(rris.A_VALUE);
                    Dr["R_VALUE"] = JsonConvert.SerializeObject(rris.R_VALUE);
                    Dr["CREATE_ID"] = user_info.user_id;
                    Dr["CREATE_NAME"] = user_info.user_name;
                    Dr["CREATE_DATE"] = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                    Dr["STATUS"] = "1";
                    Dt.Rows.Add(Dr);
                    this.DBA.BeginTrans();
                    mayaminer.com.jxDB.dbResultMessage rc = this.DBA.UpdateResult(Dt, GetTableName.RCS_RT_ISBAR_SHIFT.ToString());              
                    if (rc.State == mayaminer.com.jxDB.enmDBResultState.Success) {
                        rm.status = RESPONSE_STATUS.SUCCESS;
                        rm.message = "儲存成功";
                        this.DBA.Commit();
                    } else {
                        rm.status = RESPONSE_STATUS.ERROR;
                        rm.message = "儲存失敗";
                        this.DBA.Rollback();
                    }
                } catch (Exception ex) {
                    rm.status = RESPONSE_STATUS.EXCEPTION;
                    rm.message = "發生內部錯誤請參考log";
                    this.DBA.Rollback();
                    LogTool.SaveLogMessage(ex, "ShiftCheck");
                }
            }
        }

        /// <summary>
        /// 取得ISBAR自動資料部分
        /// </summary>
        /// <param name="shift_data">交班資料</param>
        /// <returns></returns>
        private RCS_RT_ISBAR_SHIFT ShiftReloadInfo(RCS_RT_ISBAR_SHIFT shift_data, IPDPatientInfo pPatInfo) {
            try
            {
                //設定I_VALUE
                shift_data.I_VALUE = string.Format("交班記錄人員：{0}　主治醫師：{1}", user_info.user_name, pPatInfo.vs_doc);
                //取得病人資料
                List<RCS_RT_ISBAR_SHIFT_S_VALUE> svlist = new List<RCS_RT_ISBAR_SHIFT_S_VALUE>();
                svlist.Add(new RCS_RT_ISBAR_SHIFT_S_VALUE() { name = "病歷號碼", data = pPatInfo.chart_no });
                svlist.Add(new RCS_RT_ISBAR_SHIFT_S_VALUE() { name = "姓名", data = pPatInfo.patient_name });
                svlist.Add(new RCS_RT_ISBAR_SHIFT_S_VALUE() { name = "性別", data = pPatInfo.genderCHT });
                svlist.Add(new RCS_RT_ISBAR_SHIFT_S_VALUE() { name = "床號", data = pPatInfo.bed_no });
                svlist.Add(new RCS_RT_ISBAR_SHIFT_S_VALUE() { name = "生日", data = pPatInfo.birth_day });
                svlist.Add(new RCS_RT_ISBAR_SHIFT_S_VALUE() { name = "年齡", data = pPatInfo.age.ToString() });
                svlist.Add(new RCS_RT_ISBAR_SHIFT_S_VALUE() { name = "入院日", data = pPatInfo.diag_date });
                List<string> diag_desc_e = new List<string>();
                foreach (Diag dg in pPatInfo.diag_list)
                {
                    if (!string.IsNullOrWhiteSpace(dg.diag_desc_e))
                    {
                        diag_desc_e.Add(dg.diag_desc_e);
                    }
                }
                svlist.Add(new RCS_RT_ISBAR_SHIFT_S_VALUE() { name = "診斷", data = string.Join(",", diag_desc_e.ToArray()) });
                // 取得最後呼吸器狀態
                Ventilator last_rt_record = BaseModel.basicfunction.GetLastRTRec(pPatInfo.chart_no, pPatInfo.ipd_no); 
                List<string> on_dateLIst = new List<string>();
                if (!string.IsNullOrWhiteSpace(BaseModel.getUseDays(pPatInfo.ipd_no, pPatInfo.chart_no, out on_dateLIst).ToString()))
                {
                    svlist.Add(new RCS_RT_ISBAR_SHIFT_S_VALUE() { name = "使用天數", data = BaseModel.getUseDays(pPatInfo.ipd_no, pPatInfo.chart_no, out on_dateLIst).ToString() });
                }//使用天數
                if (!string.IsNullOrWhiteSpace(last_rt_record.device_o2))
                {
                    svlist.Add(new RCS_RT_ISBAR_SHIFT_S_VALUE() { name = "O2 equipment", data = last_rt_record.device_o2 });
                }//O2 equipment
                if (!string.IsNullOrWhiteSpace(last_rt_record.mode))
                {
                    svlist.Add(new RCS_RT_ISBAR_SHIFT_S_VALUE() { name = "mode", data = last_rt_record.mode });
                }//mode
                if (!string.IsNullOrWhiteSpace(last_rt_record.fio2_set))
                {
                    svlist.Add(new RCS_RT_ISBAR_SHIFT_S_VALUE() { name = "FiO2", data = last_rt_record.fio2_set });
                }//FiO2
                if (!string.IsNullOrWhiteSpace(last_rt_record.vr_set))
                {
                    svlist.Add(new RCS_RT_ISBAR_SHIFT_S_VALUE() { name = "Ventilation rate set", data = last_rt_record.vr_set });
                }//vr_set
                if (!string.IsNullOrWhiteSpace(last_rt_record.fio2_measured))
                {
                    svlist.Add(new RCS_RT_ISBAR_SHIFT_S_VALUE() { name = "Measured", data = last_rt_record.fio2_measured });
                }//Measured
                if (!string.IsNullOrWhiteSpace(last_rt_record.o2flow))
                {
                    svlist.Add(new RCS_RT_ISBAR_SHIFT_S_VALUE() { name = "O2 flow", data = last_rt_record.o2flow });
                }//O2 flow
                if (!string.IsNullOrWhiteSpace(last_rt_record.mv_set))
                {
                    svlist.Add(new RCS_RT_ISBAR_SHIFT_S_VALUE() { name = "MV", data = last_rt_record.mv_set });
                }//MV
                if (!string.IsNullOrWhiteSpace(last_rt_record.mv_percent))
                {
                    svlist.Add(new RCS_RT_ISBAR_SHIFT_S_VALUE() { name = "MV %", data = last_rt_record.mv_percent });
                }//MV %
                if (!string.IsNullOrWhiteSpace(last_rt_record.vt_set))
                {
                    svlist.Add(new RCS_RT_ISBAR_SHIFT_S_VALUE() { name = "TV", data = last_rt_record.vt_set });
                }//TV
                if (!string.IsNullOrWhiteSpace(last_rt_record.pressure_pc))
                {
                    svlist.Add(new RCS_RT_ISBAR_SHIFT_S_VALUE() { name = "PC", data = last_rt_record.pressure_pc });
                }//PC
                if (!string.IsNullOrWhiteSpace(last_rt_record.pressure_ps))
                {
                    svlist.Add(new RCS_RT_ISBAR_SHIFT_S_VALUE() { name = "PS", data = last_rt_record.pressure_ps });
                }//PS
                if (!string.IsNullOrWhiteSpace(last_rt_record.pressure_peep))
                {
                    svlist.Add(new RCS_RT_ISBAR_SHIFT_S_VALUE() { name = "PEEP", data = last_rt_record.pressure_peep });
                }//PEEP
                if (!string.IsNullOrWhiteSpace(last_rt_record.flow))
                {
                    svlist.Add(new RCS_RT_ISBAR_SHIFT_S_VALUE() { name = "flow", data = last_rt_record.flow });
                }//flow
                if (!string.IsNullOrWhiteSpace(last_rt_record.gcse) || !string.IsNullOrWhiteSpace(last_rt_record.gcsm) || !string.IsNullOrWhiteSpace(last_rt_record.gcsv))
                {
                    svlist.Add(new RCS_RT_ISBAR_SHIFT_S_VALUE() { name = "GCS：E/M/V", data = last_rt_record.gcse + "/" + last_rt_record.gcsm + "/" + last_rt_record.gcsv });
                }//GCS：E/M/V
                if (!string.IsNullOrWhiteSpace(last_rt_record.conscious))
                {
                    svlist.Add(new RCS_RT_ISBAR_SHIFT_S_VALUE() { name = "意識", data = last_rt_record.conscious });
                }//vr_set
                

                // 將病人及取得最後呼吸器狀態加入S_VALUE
                shift_data.S_VALUE = svlist.FindAll(x => !(string.IsNullOrWhiteSpace(x.data) || x.data == "-"));

                // 取得藥囑
                // shift_data.B_VALUE = ShiftModel.web_method.getShiftOrderList(pPatInfo.chart_no, pPatInfo.ipd_no);
                shift_data.B_VALUE = new List<PatOrder>();
                shift_data.A_VALUE = new RCS_A_VALUE();
                shift_data.R_VALUE = new List<JSON_DATA>();
            }
            catch (Exception ex)
            {
                LogTool.SaveLogMessage(ex, "ShiftReloadInfo",this.csName);
            }
           
            return shift_data;
        }

        public JsonResult ShiftcheckData(string shift_str)
        {
            RESPONSE_MSG rm = new RESPONSE_MSG();
            shift_str = HttpUtility.UrlDecode(shift_str);
            RCS_RT_ISBAR_SHIFT shift_data = JsonConvert.DeserializeObject<RCS_RT_ISBAR_SHIFT>(shift_str);
            DataTable Dt = ShiftModel.getShift(string.Format("WHERE ISBAR_ID ={0}", SQLDefend.SQLString(shift_data.S_ID)));
            DataRow Dr = Dt.Rows[0];
            string _b_value_1 = string.IsNullOrWhiteSpace(Dr["b_value_1"].ToString())? "": Dr["b_value_1"].ToString().TrimEnd('\n');
            string _b_value_2 = string.IsNullOrWhiteSpace(Dr["b_value_2"].ToString()) ? "" : Dr["b_value_2"].ToString().TrimEnd('\n');
            if ((!string.IsNullOrWhiteSpace(_b_value_1) && shift_data.B_VALUE_1_old != _b_value_1) ||
                (!string.IsNullOrWhiteSpace(_b_value_2) && shift_data.B_VALUE_2_old != _b_value_2))
            {
                rm.status = RESPONSE_STATUS.ERROR;
                string _temp = string.Concat(Dr["MODIFY_NAME"].ToString()," ", Dr["MODIFY_DATE"].ToString()).Trim();
                rm.message = string.Concat("資料已經被 ", _temp, " 修改過，是否覆蓋掉?");
                rm.attachment = new List<string>() {
                   _b_value_1,
                   _b_value_2,
                    Dr["MODIFY_ID"].ToString(),
                    Dr["MODIFY_NAME"].ToString(),
                    Dr["MODIFY_DATE"].ToString()
                };
            }
            else
            {
                rm.status = RESPONSE_STATUS.SUCCESS;
            }
            return Json(rm);
        }


        /// <summary>
        /// 交班資料儲存
        /// </summary>
        /// <param name="shift_data"></param>
        /// <returns></returns>
        public string ShiftSave(string shift_str) {
            RESPONSE_MSG rm = new RESPONSE_MSG();

            try {
                shift_str = HttpUtility.UrlDecode(shift_str);
                RCS_RT_ISBAR_SHIFT shift_data = JsonConvert.DeserializeObject<RCS_RT_ISBAR_SHIFT>(shift_str);
                DataTable Dt = ShiftModel.getShift(string.Format("WHERE ISBAR_ID ={0}", SQLDefend.SQLString(shift_data.S_ID)));
                DataRow Dr = Dt.Rows[0];
                Dr["I_VALUE"] = shift_data.I_VALUE;
                if (shift_data.S_VALUE == null) shift_data.S_VALUE = new List<RCS_RT_ISBAR_SHIFT_S_VALUE>();
                Dr["S_VALUE"] = JsonConvert.SerializeObject(shift_data.S_VALUE);
                if (shift_data.B_VALUE == null) shift_data.B_VALUE = new List<PatOrder>();
                Dr["B_VALUE"] = JsonConvert.SerializeObject(shift_data.B_VALUE);
                if (shift_data.A_VALUE == null) shift_data.A_VALUE = new RCS_A_VALUE();
                Dr["A_VALUE"] = JsonConvert.SerializeObject(shift_data.A_VALUE);
                if (shift_data.R_VALUE == null) shift_data.R_VALUE = new List<JSON_DATA>();
                Dr["R_VALUE"] = JsonConvert.SerializeObject(shift_data.R_VALUE);
                Dr["STATUS"] = "1";
                Dr["b_value_1"] = shift_data.B_VALUE_1.Replace("\n","");
                Dr["b_value_2"] = shift_data.B_VALUE_2;
                Dr["MODIFY_ID"] = user_info.user_id;
                Dr["MODIFY_NAME"] = user_info.user_name;
                Dr["MODIFY_DATE"] = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                this.DBA.BeginTrans();
                mayaminer.com.jxDB.dbResultMessage rc = this.DBA.UpdateResult(Dt, GetTableName.RCS_RT_ISBAR_SHIFT.ToString());
                if (rc.State == mayaminer.com.jxDB.enmDBResultState.Success) {
                    this.DBA.Commit();
                    rm.status = RESPONSE_STATUS.SUCCESS;
                    rm.message = "暫存成功";
                } else {
                    this.DBA.Rollback();
                    rm.status = RESPONSE_STATUS.ERROR;
                    rm.message = "暫存失敗";
                }
            } catch (Exception ex) {
                this.DBA.Rollback();
                rm.status = RESPONSE_STATUS.EXCEPTION;
                rm.message = "發生內部伺服器錯誤，請聯絡資訊人員處理";
                LogTool.SaveLogMessage(ex, "ShiftSave");
            }
            return rm.get_json();
        }

        

        #endregion

        /// <summary>取得交班表格式</summary>
        /// <param name="index">id變數(交班作業用)</param>
        /// <returns></returns>
        public ActionResult getShift_view(string index, string chart_no)
        {
            List<string> cptData = BaseModel.getCPTAssess(chart_no);
            ViewData["CPTData"] = cptData;
            ViewData["index"] = index;
            return View("_shift_viewBasic");
        }

        #region 新的 交班作業

        /// <summary>
        /// 取得上次交班紀錄
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public string GetLastShiftDataToView(string shift_chart_no, string shift_IPD_NO, string shift_PAT_DATA_DATE)
        {
            List<RCS_RT_ISBAR_SHIFT> rris = new List<RCS_RT_ISBAR_SHIFT>();
            List<RCS_RT_ISBAR_SHIFT> temprris = new List<RCS_RT_ISBAR_SHIFT>();
            try
            {
                DataTable Dt = new DataTable();
                //取得該住院號下的已交班的記錄
                if (!String.IsNullOrWhiteSpace(shift_chart_no))
                {
                    Dt = ShiftModel.getLastShift(shift_chart_no, shift_IPD_NO, shift_PAT_DATA_DATE);

                }
                if (Dt.Rows != null && Dt.Rows.Count > 0)
                {
                    foreach (DataRow Dr in Dt.Rows)
                    {
                        rris.Add(new RCS_RT_ISBAR_SHIFT()
                        {
                            B_VALUE_1 = Dr["B_VALUE_1"].ToString().Replace("\n", "&lt;p&gt;"),
                            B_VALUE_2 = Dr["B_VALUE_2"].ToString().Trim(),
                            B_VALUE_1_old = Dr["B_VALUE_1"].ToString().Replace("\n", "&lt;p&gt;"),
                            B_VALUE_2_old = Dr["B_VALUE_2"].ToString().Trim()
                        });
                        break;
                    }
                    rris[0] = ShiftReloadInfo(rris[0], pat_info);
                }
            }
            catch (Exception ex)
            {
                LogTool.SaveLogMessage(ex, "GetLastShiftData");
            }
            return JsonConvert.SerializeObject(rris);
        }


        /// <summary>
        /// 交班作業畫面所需要資料
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public JsonResult CheckListViewData()
        {
            checkListViewer checkListViewer = new Models.checkListViewer();
            checkListViewer.UserName = user_info.user_name;
            List<Dictionary<string, string>> Dt = new SystemManage().UserMaintain_List(true);
            checkListViewer.UserList.Add(new SelectListItem() { Text = "請選擇", Value = "" });
            foreach (Dictionary<string, string> dic in Dt)
            {
                if (dic["P_GROUP"] != "doctor")
                    checkListViewer.UserList.Add(new SelectListItem() { Text = dic["P_NAME"], Value = dic["P_VALUE"] });
            }
            return Json(checkListViewer);
        }

        /// <summary>
        /// FromView 依選擇的PtList抓取交班資料
        /// </summary>
        /// <param name="PtList">交班清單</param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult CheckListTable(string PtList)
        {
            List<RCS_RT_ISBAR_SHIFT> rris = new List<RCS_RT_ISBAR_SHIFT>();
            try
            {
                if (!string.IsNullOrWhiteSpace(PtList))
                {
                    List<IPDPatientInfo> PtList_ = JsonConvert.DeserializeObject<List<IPDPatientInfo>>(PtList);
                    if (PtList_.Count > 0)
                    {

                        foreach (IPDPatientInfo item in PtList_)
                        {
                            RCS_RT_ISBAR_SHIFT TmpShift = new RCS_RT_ISBAR_SHIFT();
                            TmpShift.chart_no = item.chart_no;
                            TmpShift.IPD_NO = item.ipd_no;
                            TmpShift.bed_no = item.bed_no;
                            TmpShift.patient_name = item.patient_name;
                            // TmpShift.pat_data = item;
                            rris.Add(TmpShift);
                        }
                        rris = rris.OrderBy(x => x.bed_no).ToList();
                    }
                }
            }
            catch (Exception ex)
            {
                LogTool.SaveLogMessage(ex, "CheckListTable", GetLogToolCS.ShiftController);
            }
            return Json(rris);
        }

        public JsonResult GetShiftForm(string pPatInfo_str)
        {
            RCS_RT_ISBAR_SHIFT item = new RCS_RT_ISBAR_SHIFT();
            try
            {
                pPatInfo_str = HttpUtility.UrlDecode(pPatInfo_str);
                IPDPatientInfo pPatInfo = JsonConvert.DeserializeObject<IPDPatientInfo>(pPatInfo_str);
                RT model = new RT();
                List<PatientListItem> newPat_info = model.get_CareIList(user_info.user_id, "C", pPatInfo.chart_no, "");
                if (newPat_info.Count > 0)
                {
                    if (newPat_info.Exists(x => x.ipd_no == pPatInfo.ipd_no))
                        pPatInfo = newPat_info.Find(x => x.ipd_no == pPatInfo.ipd_no);
                    item = this.SetShiftData(pPatInfo);
                }
                
            }
            catch (Exception ex)
            {
                LogTool.SaveLogMessage(ex, "GetShiftForm", GetLogToolCS.ShiftController);
            }
           
            return Json(item);
        }

        private RCS_RT_ISBAR_SHIFT SetShiftData(IPDPatientInfo pPat_info)
        {
            RCS_RT_ISBAR_SHIFT item = new RCS_RT_ISBAR_SHIFT();
            try
            {
                ShiftCheckData(pPat_info);//檢查是否需要新增一筆資料
                //取得該住院號下的已交班的記錄與當日RT的暫存紀錄
                DataTable Dt = ShiftModel.getShiftTemp(pPat_info.ipd_no);
                foreach (DataRow Dr in Dt.Rows)
                {
                    item = new RCS_RT_ISBAR_SHIFT()
                    {
                        S_ID = Dr["ISBAR_ID"].ToString().Trim(),
                        IPD_NO = Dr["ipd_no"].ToString().Trim(),
                        I_VALUE = Dr["I_VALUE"].ToString().Trim(),
                        S_VALUE = JsonConvert.DeserializeObject<List<RCS_RT_ISBAR_SHIFT_S_VALUE>>(Dr["S_VALUE"].ToString().Trim()),
                        B_VALUE = JsonConvert.DeserializeObject<List<PatOrder>>(Dr["B_VALUE"].ToString().Trim()),
                        A_VALUE = JsonConvert.DeserializeObject<RCS_A_VALUE>(Dr["A_VALUE"].ToString().Trim()),
                        R_VALUE = JsonConvert.DeserializeObject<List<JSON_DATA>>(Dr["R_VALUE"].ToString().Trim()),
                        CREATE_ID = Dr["CREATE_ID"].ToString().Trim(),
                        CREATE_NAME = Dr["CREATE_NAME"].ToString().Trim(),
                        CREATE_DATE = Dr["CREATE_DATE"].ToString().Trim(),
                        SHIFT_ID = Dr["SHIFT_ID"].ToString().Trim(),
                        SHIFT_NAME = Dr["SHIFT_NAME"].ToString().Trim(),
                        SHIFT_DATE = Dr["SHIFT_DATE"].ToString().Trim(),
                        STATUS = Dr["STATUS"].ToString().Trim(),
                        chart_no = pPat_info.chart_no,
                        B_VALUE_1 = Dr["B_VALUE_1"].ToString().Replace("\n", "&lt;p&gt;"),
                        B_VALUE_2 = Dr["B_VALUE_2"].ToString().Trim(),
                        B_VALUE_1_old = Dr["B_VALUE_1"].ToString().Replace("\n", "&lt;p&gt;"),
                        B_VALUE_2_old = Dr["B_VALUE_2"].ToString().Trim()
                    };
                    break;
                }
                item = ShiftReloadInfo(item, pPat_info);
            }
            catch (Exception ex)
            {
                LogTool.SaveLogMessage(ex, "SetShiftData");
                return null;
            }
            return item;
        }

        /// <summary>
        /// 檢查是否該建立新的交班資料
        /// </summary>
        /// <returns></returns>
        private void ShiftCheckData(IPDPatientInfo pPatInfo, bool isShitWork = false)
        {
            RCS_RT_ISBAR_SHIFT rris = new RCS_RT_ISBAR_SHIFT();
            RESPONSE_MSG rm = new RESPONSE_MSG();
            SQLProvider SQL = new SQLProvider();
            try
            {
                //取得上一筆交班記錄                
                DataTable Dt = ShiftModel.getPrevShift(pPatInfo.ipd_no, user_info.user_id);
                if (Dt != null && Dt.Rows.Count > 0)
                {
                    DataRow dr = Dt.AsEnumerable().ToList().OrderByDescending(x => x["CREATE_DATE"]).First();
                    rris = (new RCS_RT_ISBAR_SHIFT()
                    {
                        S_ID = dr["ISBAR_ID"].ToString().Trim(),
                        IPD_NO = dr["ipd_no"].ToString().Trim(),
                        I_VALUE = dr["I_VALUE"].ToString().Trim(),
                        S_VALUE = JsonConvert.DeserializeObject<List<RCS_RT_ISBAR_SHIFT_S_VALUE>>(dr["S_VALUE"].ToString().Trim()),
                        B_VALUE = JsonConvert.DeserializeObject<List<PatOrder>>(dr["B_VALUE"].ToString().Trim()),
                        A_VALUE = JsonConvert.DeserializeObject<RCS_A_VALUE>(dr["A_VALUE"].ToString().Trim()),
                        R_VALUE = JsonConvert.DeserializeObject<List<JSON_DATA>>(dr["R_VALUE"].ToString().Trim()),
                        CREATE_ID = dr["CREATE_ID"].ToString().Trim(),
                        CREATE_NAME = dr["CREATE_NAME"].ToString().Trim(),
                        CREATE_DATE = dr["CREATE_DATE"].ToString().Trim(),
                        SHIFT_ID = dr["SHIFT_ID"].ToString().Trim(),
                        SHIFT_NAME = dr["SHIFT_NAME"].ToString().Trim(),
                        SHIFT_DATE = dr["SHIFT_DATE"].ToString().Trim(),
                        STATUS = dr["STATUS"].ToString().Trim(),
                        B_VALUE_1 = dr["B_VALUE_1"].ToString().Trim().Replace("\n", "&lt;p&gt;"),
                        B_VALUE_2 = dr["B_VALUE_2"].ToString().Trim()
                    });
                }
            }
            catch (Exception ex)
            {
                LogTool.SaveLogMessage(ex, "GetShiftData");
            }
            //判斷是否新增一筆新資料：沒舊資料 || 已交班  
            if (string.IsNullOrWhiteSpace(rris.S_ID) || rris.STATUS == "2")
            {
                //取得新的S_ID
                string strS_ID = SQL.GetFixedStrSerialNumber();
                try
                {
                    DataTable Dt = ShiftModel.getShift("WHERE 1<>1");
                    DataRow Dr = Dt.NewRow();
                    //若有舊資料需抄舊有資料
                    Dr["b_value_1"] = string.IsNullOrWhiteSpace(rris.B_VALUE_1) ? "" : rris.B_VALUE_1;
                    Dr["b_value_2"] = string.IsNullOrWhiteSpace(rris.B_VALUE_2) ? "" : rris.B_VALUE_2;
                    //更新即時資訊
                    rris = ShiftReloadInfoData(rris, pPatInfo);
                    Dr["ISBAR_ID"] = strS_ID;
                    Dr["IPD_NO"] = pPatInfo.ipd_no;
                    Dr["I_VALUE"] = rris.I_VALUE;
                    Dr["S_VALUE"] = JsonConvert.SerializeObject(rris.S_VALUE);
                    Dr["B_VALUE"] = JsonConvert.SerializeObject(rris.B_VALUE);
                    Dr["A_VALUE"] = JsonConvert.SerializeObject(rris.A_VALUE);
                    Dr["R_VALUE"] = JsonConvert.SerializeObject(rris.R_VALUE);
                    Dr["CREATE_ID"] = user_info.user_id;
                    Dr["CREATE_NAME"] = user_info.user_name;
                    Dr["CREATE_DATE"] = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                    Dr["STATUS"] = "1";
                    Dt.Rows.Add(Dr);
                    this.DBA.BeginTrans();
                    mayaminer.com.jxDB.dbResultMessage rc = this.DBA.UpdateResult(Dt, GetTableName.RCS_RT_ISBAR_SHIFT.ToString());
                    if (rc.State == mayaminer.com.jxDB.enmDBResultState.Success)
                    {
                        rm.status = RESPONSE_STATUS.SUCCESS;
                        rm.message = "儲存成功";
                        this.DBA.Commit();
                    }
                    else
                    {
                        rm.status = RESPONSE_STATUS.ERROR;
                        rm.message = "儲存失敗";
                        this.DBA.Rollback();
                    }
                }
                catch (Exception ex)
                {
                    rm.status = RESPONSE_STATUS.EXCEPTION;
                    rm.message = "發生內部錯誤請參考log";
                    this.DBA.Rollback();
                    LogTool.SaveLogMessage(ex, "ShiftCheck");
                }
            }
        }


        /// <summary>
        /// 取得ISBAR自動資料部分
        /// </summary>
        /// <param name="shift_data">交班資料</param>
        /// <returns></returns>
        private RCS_RT_ISBAR_SHIFT ShiftReloadInfoData(RCS_RT_ISBAR_SHIFT shift_data, IPDPatientInfo pPatInfo)
        {
            try
            {
                //設定I_VALUE
                shift_data.I_VALUE = string.Format("交班記錄人員：{0}　主治醫師：{1}", user_info.user_name, pPatInfo.vs_doc);
                //取得病人資料
                List<RCS_RT_ISBAR_SHIFT_S_VALUE> svlist = new List<RCS_RT_ISBAR_SHIFT_S_VALUE>();
                svlist.Add(new RCS_RT_ISBAR_SHIFT_S_VALUE() { name = "病歷號碼", data = pPatInfo.chart_no });
                svlist.Add(new RCS_RT_ISBAR_SHIFT_S_VALUE() { name = "姓名", data = pPatInfo.patient_name });
                svlist.Add(new RCS_RT_ISBAR_SHIFT_S_VALUE() { name = "性別", data = pPatInfo.genderCHT });
                svlist.Add(new RCS_RT_ISBAR_SHIFT_S_VALUE() { name = "床號", data = pPatInfo.bed_no });
                svlist.Add(new RCS_RT_ISBAR_SHIFT_S_VALUE() { name = "生日", data = pPatInfo.birth_day });
                svlist.Add(new RCS_RT_ISBAR_SHIFT_S_VALUE() { name = "年齡", data = pPatInfo.age.ToString() });
                svlist.Add(new RCS_RT_ISBAR_SHIFT_S_VALUE() { name = "入院日", data = pPatInfo.diag_date });
                List<string> diag_desc_e = new List<string>();
                foreach (Diag dg in pPatInfo.diag_list)
                {
                    if (!string.IsNullOrWhiteSpace(dg.diag_desc_e))
                    {
                        diag_desc_e.Add(dg.diag_desc_e);
                    }
                }
                svlist.Add(new RCS_RT_ISBAR_SHIFT_S_VALUE() { name = "診斷", data = string.Join(",", diag_desc_e.ToArray()) });
                // 取得最後呼吸器狀態
                Ventilator last_rt_record = BaseModel.basicfunction.GetLastRTRec(pPatInfo.chart_no, pPatInfo.ipd_no); 
                List<string> on_dateLIst = new List<string>();
                if (!string.IsNullOrWhiteSpace(BaseModel.getUseDays(pPatInfo.ipd_no, pPatInfo.chart_no, out on_dateLIst).ToString()))
                {
                    svlist.Add(new RCS_RT_ISBAR_SHIFT_S_VALUE() { name = "使用天數", data = BaseModel.getUseDays(pPatInfo.ipd_no, pPatInfo.chart_no, out on_dateLIst).ToString() });
                }//使用天數
                if (!string.IsNullOrWhiteSpace(last_rt_record.device_o2))
                {
                    svlist.Add(new RCS_RT_ISBAR_SHIFT_S_VALUE() { name = "O2 equipment", data = last_rt_record.device_o2 });
                }//O2 equipment
                if (!string.IsNullOrWhiteSpace(last_rt_record.mode))
                {
                    svlist.Add(new RCS_RT_ISBAR_SHIFT_S_VALUE() { name = "mode", data = last_rt_record.mode });
                }//mode
                if (!string.IsNullOrWhiteSpace(last_rt_record.fio2_set))
                {
                    svlist.Add(new RCS_RT_ISBAR_SHIFT_S_VALUE() { name = "FiO2", data = last_rt_record.fio2_set });
                }//FiO2
                if (!string.IsNullOrWhiteSpace(last_rt_record.vr_set))
                {
                    svlist.Add(new RCS_RT_ISBAR_SHIFT_S_VALUE() { name = "Ventilation rate set", data = last_rt_record.vr_set });
                }//vr_set
                if (!string.IsNullOrWhiteSpace(last_rt_record.fio2_measured))
                {
                    svlist.Add(new RCS_RT_ISBAR_SHIFT_S_VALUE() { name = "Measured", data = last_rt_record.fio2_measured });
                }//Measured
                if (!string.IsNullOrWhiteSpace(last_rt_record.o2flow))
                {
                    svlist.Add(new RCS_RT_ISBAR_SHIFT_S_VALUE() { name = "O2 flow", data = last_rt_record.o2flow });
                }//O2 flow
                if (!string.IsNullOrWhiteSpace(last_rt_record.mv_set))
                {
                    svlist.Add(new RCS_RT_ISBAR_SHIFT_S_VALUE() { name = "MV", data = last_rt_record.mv_set });
                }//MV
                if (!string.IsNullOrWhiteSpace(last_rt_record.mv_percent))
                {
                    svlist.Add(new RCS_RT_ISBAR_SHIFT_S_VALUE() { name = "MV %", data = last_rt_record.mv_percent });
                }//MV %
                if (!string.IsNullOrWhiteSpace(last_rt_record.vt_set))
                {
                    svlist.Add(new RCS_RT_ISBAR_SHIFT_S_VALUE() { name = "TV", data = last_rt_record.vt_set });
                }//TV
                if (!string.IsNullOrWhiteSpace(last_rt_record.pressure_pc))
                {
                    svlist.Add(new RCS_RT_ISBAR_SHIFT_S_VALUE() { name = "PC", data = last_rt_record.pressure_pc });
                }//PC
                if (!string.IsNullOrWhiteSpace(last_rt_record.pressure_ps))
                {
                    svlist.Add(new RCS_RT_ISBAR_SHIFT_S_VALUE() { name = "PS", data = last_rt_record.pressure_ps });
                }//PS
                if (!string.IsNullOrWhiteSpace(last_rt_record.pressure_peep))
                {
                    svlist.Add(new RCS_RT_ISBAR_SHIFT_S_VALUE() { name = "PEEP", data = last_rt_record.pressure_peep });
                }//PEEP
                if (!string.IsNullOrWhiteSpace(last_rt_record.flow))
                {
                    svlist.Add(new RCS_RT_ISBAR_SHIFT_S_VALUE() { name = "flow", data = last_rt_record.flow });
                }//flow
                if (!string.IsNullOrWhiteSpace(last_rt_record.gcse) || !string.IsNullOrWhiteSpace(last_rt_record.gcsm) || !string.IsNullOrWhiteSpace(last_rt_record.gcsv))
                {
                    svlist.Add(new RCS_RT_ISBAR_SHIFT_S_VALUE() { name = "GCS：E/M/V", data = last_rt_record.gcse + "/" + last_rt_record.gcsm + "/" + last_rt_record.gcsv });
                }//GCS：E/M/V
                if (!string.IsNullOrWhiteSpace(last_rt_record.conscious))
                {
                    svlist.Add(new RCS_RT_ISBAR_SHIFT_S_VALUE() { name = "意識", data = last_rt_record.conscious });
                }//vr_set


                // 將病人及取得最後呼吸器狀態加入S_VALUE
                shift_data.S_VALUE = svlist.FindAll(x => !(string.IsNullOrWhiteSpace(x.data) || x.data == "-"));

                // 取得藥囑
                shift_data.B_VALUE = ShiftModel.web_method.getShiftOrderList(this.hospFactory.webService.HISOrderList(), pPatInfo.chart_no, pPatInfo.ipd_no);
                shift_data.A_VALUE = new RCS_A_VALUE();
                shift_data.R_VALUE = new List<JSON_DATA>();
            }
            catch (Exception ex)
            {
                LogTool.SaveLogMessage(ex, "ShiftReloadInfo", this.csName);
            }

            return shift_data;
        }



        /// <summary>取得交班表格式</summary>
        /// <param name="index">id變數(交班作業用)</param>
        /// <returns></returns>
        public ActionResult getShift_viewHtml(string index, string chart_no)
        {
            List<string> cptData = BaseModel.getCPTAssess(chart_no);
            ViewData["CPTData"] = cptData;
            ViewData["index"] = index;
            return View("_shift_viewBasic");
        }


        /// <summary>
        /// 交班資料儲存
        /// </summary>
        /// <param name="shift_data"></param>
        /// <returns></returns>
        public string ShiftSaveData(string shift_str)
        {
            RESPONSE_MSG rm = new RESPONSE_MSG();

            try
            {
                shift_str = HttpUtility.UrlDecode(shift_str);
                RCS_RT_ISBAR_SHIFT shift_data = JsonConvert.DeserializeObject<RCS_RT_ISBAR_SHIFT>(shift_str);
                DataTable Dt = ShiftModel.getShift(string.Format("WHERE ISBAR_ID ={0}", SQLDefend.SQLString(shift_data.S_ID)));
                DataRow Dr = Dt.Rows[0];
                Dr["I_VALUE"] = shift_data.I_VALUE;
                if (shift_data.S_VALUE == null) shift_data.S_VALUE = new List<RCS_RT_ISBAR_SHIFT_S_VALUE>();
                Dr["S_VALUE"] = JsonConvert.SerializeObject(shift_data.S_VALUE);
                if (shift_data.B_VALUE == null) shift_data.B_VALUE = new List<PatOrder>();
                Dr["B_VALUE"] = JsonConvert.SerializeObject(shift_data.B_VALUE);
                if (shift_data.A_VALUE == null) shift_data.A_VALUE = new RCS_A_VALUE();
                Dr["A_VALUE"] = JsonConvert.SerializeObject(shift_data.A_VALUE);
                if (shift_data.R_VALUE == null) shift_data.R_VALUE = new List<JSON_DATA>();
                Dr["R_VALUE"] = JsonConvert.SerializeObject(shift_data.R_VALUE);
                Dr["STATUS"] = "1";
                Dr["b_value_1"] = shift_data.B_VALUE_1.Replace("\n", "");
                Dr["b_value_2"] = shift_data.B_VALUE_2;
                Dr["MODIFY_ID"] = user_info.user_id;
                Dr["MODIFY_NAME"] = user_info.user_name;
                Dr["MODIFY_DATE"] = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                this.DBA.BeginTrans();
                mayaminer.com.jxDB.dbResultMessage rc = this.DBA.UpdateResult(Dt, GetTableName.RCS_RT_ISBAR_SHIFT.ToString());
                if (rc.State == mayaminer.com.jxDB.enmDBResultState.Success)
                {
                    this.DBA.Commit();
                    rm.status = RESPONSE_STATUS.SUCCESS;
                    rm.message = "暫存成功";
                }
                else
                {
                    this.DBA.Rollback();
                    rm.status = RESPONSE_STATUS.ERROR;
                    rm.message = "暫存失敗";
                }
            }
            catch (Exception ex)
            {
                this.DBA.Rollback();
                rm.status = RESPONSE_STATUS.EXCEPTION;
                rm.message = "發生內部伺服器錯誤，請聯絡資訊人員處理";
                LogTool.SaveLogMessage(ex, "ShiftSave");
            }
            return rm.get_json();
        }

        /// <summary> ForView 交班 </summary>
        /// <param name="UserNo">交班者</param>
        /// <param name="PtList">交接PtList</param>
        /// <returns>bool</returns>
        [HttpPost]
        public bool SaveShiftDataHandOver(string UserNo, string UserName, string PtList)
        {
            bool Success = true;
            string actionName = "SaveShiftHandOver";
            PtList = HttpUtility.UrlDecode(PtList);
            if (!string.IsNullOrWhiteSpace(PtList))
            {

                List<RCS_RT_ISBAR_SHIFT> PtList_ = new List<RCS_RT_ISBAR_SHIFT>();
                try
                {
                    PtList_ = JsonConvert.DeserializeObject<List<RCS_RT_ISBAR_SHIFT>>(PtList);
                }
                catch (Exception ex)
                {
                    LogTool.SaveLogMessage(ex, "SaveShiftHandOver", "Shift");
                }
                List<DataTable> ExcuteSchList = new List<DataTable>();
                List<DataTable> ExcuteShiList = new List<DataTable>();
                if (PtList_.Count > 0)
                {
                    try
                    {

                        List<string> strIpdno = PtList_.Select(x => x.IPD_NO).ToList();

                        SQLProvider DBLink = new SQLProvider();
                        string _query = string.Concat("SELECT rm.ISBAR_ID S_ID ,rl.IPD_NO FROM RCS_RT_ISBAR_SHIFT as rm  RIGHT JOIN(SELECT L.IPD_NO, MAX(M.CREATE_DATE) CREATE_DATE",
" FROM RCS_RT_CARE_SCHEDULING as L LEFT JOIN RCS_RT_ISBAR_SHIFT AS M ON L.IPD_NO = M.IPD_NO",
" WHERE L.IPD_NO in @IPD_NO",
" GROUP BY L.IPD_NO) as rl on rm.CREATE_DATE = rl.CREATE_DATE AND rm.IPD_NO = rl.IPD_NO AND rm.STATUS = '1'");
                        Dapper.DynamicParameters dp = new Dapper.DynamicParameters();
                        dp.Add("IPD_NO", strIpdno);
                        List<RCS_RT_ISBAR_SHIFT> shift_idList = DBLink.DBA.getSqlDataTable<RCS_RT_ISBAR_SHIFT>(_query, dp);

                        string sql = "SELECT * FROM " + GetTableName.RCS_RT_CARE_SCHEDULING + " WHERE RT_ID in('" + UserNo + "','" + user_info.user_id + "') AND IPD_NO in('" + string.Join("','", strIpdno) + "') AND TYPE_MODE = 'C'";

                        DataTable dtSch = this.DBA.getSqlDataTable(sql);
                        if (!string.IsNullOrWhiteSpace(this.DBA.LastError))
                        {
                            LogTool.SaveLogMessage(this.DBA.LastError, actionName, GetLogToolCS.ShiftController);
                            Success = false;
                        }
                        sql = "SELECT * FROM " + GetTableName.RCS_RT_ISBAR_SHIFT + " WHERE ISBAR_ID in('" + string.Join("','", shift_idList.Select(x => x.S_ID).Distinct()) + "')";

                        DataTable dtShi = this.DBA.getSqlDataTable(sql);
                        if (!string.IsNullOrWhiteSpace(this.DBA.LastError))
                        {
                            LogTool.SaveLogMessage(this.DBA.LastError, actionName, GetLogToolCS.ShiftController);
                            Success = false;
                        }
                        if (Success)
                        {

                            foreach (RCS_RT_ISBAR_SHIFT Pt in PtList_)
                            {

                                //照護清單移轉
                                DataRow[] tempSch = dtSch.AsEnumerable().Where(x => x["IPD_NO"].ToString() == Pt.IPD_NO).ToArray();

                                if (tempSch.Any())
                                {
                                    //判斷是否對方有此病患
                                    bool hasThisPt = false;
                                    foreach (DataRow dr in tempSch)
                                    {
                                        if (dr["RT_ID"].ToString() == UserNo)
                                            hasThisPt = true;
                                    }
                                    if (!hasThisPt)
                                    {
                                        dtSch.AsEnumerable().Where(x => x["RT_ID"].ToString().Trim() == user_info.user_id && x["IPD_NO"].ToString() == Pt.IPD_NO).SingleOrDefault()["BED_NO"] = Pt.bed_no;
                                        dtSch.AsEnumerable().Where(x => x["RT_ID"].ToString().Trim() == user_info.user_id && x["IPD_NO"].ToString() == Pt.IPD_NO).SingleOrDefault()["LOC"] = Pt.loc;
                                        dtSch.AsEnumerable().Where(x => x["RT_ID"].ToString().Trim() == user_info.user_id && x["IPD_NO"].ToString() == Pt.IPD_NO).SingleOrDefault()["TYPE_MODE"] = "C";
                                        dtSch.AsEnumerable().Where(x => x["RT_ID"].ToString().Trim() == user_info.user_id && x["IPD_NO"].ToString() == Pt.IPD_NO).SingleOrDefault()["RT_ID"] = UserNo;
                                    }
                                }
                                if (shift_idList.Exists(x => x.IPD_NO == Pt.IPD_NO))
                                {
                                    Pt.S_ID = shift_idList.Find(y => y.IPD_NO == Pt.IPD_NO).S_ID;
                                    //紀錄交班
                                    DataRow[] tempShi = dtShi.AsEnumerable().Where(x => x["ISBAR_ID"].ToString() == Pt.S_ID).ToArray();

                                    if (tempShi.Any())
                                    {
                                        DataRow ShiDr = null;
                                        dtShi.AsEnumerable().Where(x => x["ISBAR_ID"].ToString() == Pt.S_ID).SingleOrDefault()["SHIFT_ID"] = UserNo;
                                        dtShi.AsEnumerable().Where(x => x["ISBAR_ID"].ToString() == Pt.S_ID).SingleOrDefault()["SHIFT_NAME"] = UserName;
                                        dtShi.AsEnumerable().Where(x => x["ISBAR_ID"].ToString() == Pt.S_ID).SingleOrDefault()["SHIFT_DATE"] = DateTime.Now.ToString("yyyy-MM-dd HH:mm");
                                        dtShi.AsEnumerable().Where(x => x["ISBAR_ID"].ToString() == Pt.S_ID).SingleOrDefault()["STATUS"] = "2";
                                        ShiDr = null;
                                    }
                                }



                            }

                            //刪除自己的
                            //照護清單移轉
                            DataRow[] tempDr = dtSch.AsEnumerable().Where(x => x["RT_ID"].ToString() == user_info.user_id).ToArray();
                            if (tempDr.Any())
                            {
                                for (int i = 0; i < dtSch.Rows.Count; i++)
                                {
                                    if (!DBNull.ReferenceEquals(dtSch.Rows[i]["RT_ID"], DBNull.Value) && dtSch.Rows[i]["RT_ID"].ToString().Trim() == user_info.user_id)
                                    {
                                        dtSch.Rows[i].Delete();
                                    }
                                }
                            }
                            try
                            {
                                this.DBA.BeginTrans();
                                dbResultMessage msg = this.DBA.UpdateResult(dtSch, GetTableName.RCS_RT_CARE_SCHEDULING.ToString());
                                if (msg.State != enmDBResultState.Success)
                                {
                                    LogTool.SaveLogMessage(msg.dbErrorMessage, actionName, GetLogToolCS.ShiftController);
                                }
                                else
                                {
                                    msg = this.DBA.UpdateResult(dtShi, GetTableName.RCS_RT_ISBAR_SHIFT.ToString());
                                    if (msg.State != enmDBResultState.Success)
                                    {
                                        LogTool.SaveLogMessage(msg.dbErrorMessage, actionName, GetLogToolCS.ShiftController);
                                    }
                                }
                                if (msg.State == enmDBResultState.Success)
                                    this.DBA.Commit();
                                else
                                    this.DBA.Rollback();

                            }
                            catch (Exception ex)
                            {
                                LogTool.SaveLogMessage(ex, actionName, GetLogToolCS.ShiftController);
                                Success = false;
                                this.DBA.Rollback();
                            }
                        }

                    }
                    catch (Exception ex)
                    {
                        LogTool.SaveLogMessage(ex, actionName, GetLogToolCS.ShiftController);
                        Success = false;
                    }
                }
            }
            return Success;
        }

        #endregion

        public JsonResult GetBackgroundTable()
        {
            // 取得藥囑
            List<PatOrder> B_VALUE = ShiftModel.web_method.getShiftOrderList(this.hospFactory.webService.HISOrderList(), pat_info.chart_no, pat_info.ipd_no);
            return Json(B_VALUE);
        }

    }
}

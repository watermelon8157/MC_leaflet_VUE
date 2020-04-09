using Com.Mayaminer;
using Dapper;
using log4net;
using mayaminer.com.library;
using Newtonsoft.Json;
using RCS_Data.Models;
using RCS_Data.Models.DB;
using RCS_Data.Models.ViewModels;
using RCS_Data.Models.VIP;
using RCSData.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using static Dapper.SqlMapper;
using RCS_Data.Controllers.RtRecord;

namespace RCS_Data.Controllers.RT
{
    public class RTModels : BaseModels
    {
        #region 參數
        private ILog _logger { get; set; }

        private string csName = "RTModels";
        protected ILog logger
        {
            get
            {
                if (this._logger == null)
                    this._logger = LogManager.GetLogger("RT");
                return this._logger;
            }
        }

        #endregion

        /// <summary>
        /// 取得病患資料詳細資料
        /// </summary>
        /// <param name="pli"></param>
        public void getPatDeatailData(ref List<PatientListItem> pli)
        {
            string actionName = "getPatDeatailData";
            try
            {
                #region 整理資料
                List<string> ipd_noList = pli.Select(x => SQLDefend.SQLString(x.ipd_no)).ToList();
                if (ipd_noList != null && ipd_noList.Count > 0)
                {
                    string ipd_str = string.Join(",", ipd_noList);

                    string sql = "";

                    if (this.DBLink.DBA.hasLastError)
                    {
                        LogTool.SaveLogMessage(this.DBLink.DBA.lastError, actionName, this.csName);
                    }
                    foreach (PatientListItem item in pli)
                    {
                        item.respid = item.machine.respid;
                        item.mode = item.machine.mode;
                        RCS_CPT_DTL_NEW_ITEMS cptData = this.getCPTAssess(item.chart_no);
                        item.memo = string.IsNullOrWhiteSpace(cptData.other_history) ? "" : cptData.other_history;
                        item.diagnosis_code = string.IsNullOrWhiteSpace(cptData.now_pat_diagnosis) ? "" : cptData.now_pat_diagnosis;
                    }
                }
                #endregion
                pli = pli.OrderBy(x => x.bed_no).ThenBy(x => x.chart_no).ToList();
            }
            catch (Exception ex)
            {
                LogTool.SaveLogMessage(ex, actionName, this.csName);
            }

        }

        /// <summary>
        /// 取得RT照護清單
        /// </summary>
        /// <param name="op_id"></param>
        /// <param name="pChart_no"></param>
        /// <returns></returns>
        public RESPONSE_MSG get_CareIList(FormRT form, List<string> Artificial_airway_typeList = null, string pChart_no = "", string pIPD_NO = "")
        {
            string actionName = "get_CareIList";
            RESPONSE_MSG rm = new RESPONSE_MSG();
            List<PatientListItem> pList = new List<PatientListItem>();
            try
            {  
                pList = this.getUserCareList(form.payload.user_id, pChart_no, pIPD_NO); 
                string[] getList = new string[9] { "device", "respid", "mode", "device_o2", "memo", "recorddate", "recordtime", "is_humidifier", "artificial_airway_type" };//取得Ventilator項目
                List<RTVentilator> ventilatorList = this.GetLastRTRecList(form.payload.user_id, pList.Select(x => x.ipd_no).Distinct().ToList(), getList);

                VIPRTTBL VIP_DATA = new VIPRTTBL();
                //查詢每一次病患的暫存交班單 
                Dapper.DynamicParameters dp = new Dapper.DynamicParameters();
                dp = new DynamicParameters();
                dp.Add("IPD_NO", pList.Select(x => x.ipd_no).ToList());
                string _query = "SELECT IPD_NO FROM RCS_RT_ISBAR_SHIFT WHERE IPD_NO in @IPD_NO AND STATUS = '1'";
                List<string> ipd_list = this.DBLink.DBA.getSqlDataTable<string>(_query, dp);

                foreach (PatientListItem inst in pList)
                {
                    inst.accept_status = "";
                    List<string> on_dateLIst = new List<string>();
                    inst.use_days = this.basicfunction.showUseDays(inst.ipd_no, inst.chart_no, "", Artificial_airway_typeList, out on_dateLIst); 
                    if (mayaminer.com.library.DateHelper.isDate(inst.birth_day, "yyyyMMdd"))
                    {
                        inst.birth_day = DateHelper.Parse(inst.birth_day).ToString("yyyy/MM/dd");
                    }
                    if (mayaminer.com.library.DateHelper.isDate(inst.birth_day, "yyyMMdd"))
                    {
                        inst.birth_day = DateHelper.ParseTWDate(inst.birth_day).ToString("yyyy/MM/dd");
                    }
                    if (ventilatorList.Exists(x => x.chart_no == inst.chart_no && x.ipd_no == inst.ipd_no))
                    {
                        inst.machine = ventilatorList.Find(x => x.chart_no == inst.chart_no && x.ipd_no == inst.ipd_no);
                        inst.accept_status = "1";
                    }
                    //查詢每一次病患的暫存交班單
                    //檢查是否有暫存交班單
                    if (ipd_list.Exists(x => x == inst.ipd_no))
                        inst.hadShift_record = true;
                    RESPONSE_MSG VIPrm = VIP_DATA.checkVIPDataHasRepeat(inst.chart_no);
                    if (VIPrm.status != RESPONSE_STATUS.SUCCESS)
                    {
                        inst.hadAlarm_msg = true;
                        inst.alarm_msg = VIPrm.message.Replace("<br \\>", "&#10;");
                    }
                    inst.setOnMode();


                    inst.respid = inst.machine.respid;
                    inst.mode = inst.machine.mode; 

                    #region getCPTAssess
                    // TODO: getCPTAssess 取得備註尚未完成 
                    //List<string> cptData = BaseModel.getCPTAssess(inst.chart_no);
                    //if (cptData.Count > 0)
                    //{
                    //    item.memo = cptData[2];
                    //    if (!string.IsNullOrWhiteSpace(cptData[4]))
                    //        item.diagnosis_code = cptData[4];
                    //}
                    #endregion

                }
                pList = pList.OrderBy(x => x.bed_no).ThenBy(x => x.chart_no).ToList();
                rm.attachment = pList;
            }
            catch (Exception ex)
            {
                rm.status = RESPONSE_STATUS.EXCEPTION;
                rm.message = "查詢失敗!";
                LogTool.SaveLogMessage(ex, actionName,this.csName);
            }
            if (this.DBLink.DBA.hasLastError)
            {
                rm.status = RESPONSE_STATUS.ERROR;
                rm.message = this.DBLink.DBA.lastError;
            }
            return rm;
        } 

        private List<RTVentilator> GetLastRTRecList(string op_id, List<string> IpdnoList, string[] getItem = null)
        {
            List<RTVentilator> _list = new List<RTVentilator>();
            List<string> sqlList = new List<string>();
            DataTable dt = null; 
            Dapper.DynamicParameters dp = new Dapper.DynamicParameters();
            string pIpdnoSql = "";
            if (IpdnoList.Count > 0)
            {
                pIpdnoSql = " AND L.IPD_NO in('" + string.Join("','", IpdnoList) + "') ";
            }
            string whereSql = "";
            if (getItem != null && getItem.Length > 0)
                whereSql = string.Format(" AND ITEM_NAME in('{0}') ", string.Join("','", getItem));
            string mainSql = string.Concat("SELECT rm.RECORD_ID,rd.ITEM_NAME,rd.ITEM_VALUE,rm.IPD_NO,rm.CHART_NO,rm.ONMODE_TYPE2_ID FROM RCS_RECORD_MASTER as rm RIGHT JOIN (SELECT L.IPD_NO,L.CHART_NO, MAX(M.RECORDDATE) RECORDDATE   FROM RCS_RT_CARE_SCHEDULING as L ",
                                             "LEFT JOIN RCS_RECORD_MASTER AS M ON L.IPD_NO = M.IPD_NO AND L.CHART_NO = M.CHART_NO AND M.DATASTATUS = '1' WHERE L.RT_ID = '", op_id, "'", pIpdnoSql,
                                             "GROUP BY L.IPD_NO, L.CHART_NO) as rl on rm.RECORDDATE = rl.RECORDDATE AND rm.IPD_NO = rl.IPD_NO AND rm.CHART_NO = rl.CHART_NO AND rm.DATASTATUS = '1' LEFT JOIN RCS_RECORD_DETAIL as rd ON rm.RECORD_ID = rd.RECORD_ID ",
                                             whereSql);
            dt = this.DBLink.DBA.getSqlDataTable(mainSql);
            if (DTNotNullAndEmpty(dt))
            {
                foreach (string ipdno in IpdnoList)
                {
                    List<DataRow> drList = new List<DataRow>();
                    try
                    {
                        if (dt.AsEnumerable().ToList().Exists(x => !DBNull.ReferenceEquals(x["IPD_NO"], DBNull.Value) && x["IPD_NO"].ToString() == ipdno))
                        {
                            drList = dt.AsEnumerable().ToList().FindAll(x => x["IPD_NO"].ToString() == ipdno).ToList();
                            Dictionary<string, string> vcch = new Dictionary<string, string>();
                            foreach (DataRow dr in drList)
                            {
                                if (!DBNull.ReferenceEquals(dr["ITEM_NAME"], DBNull.Value) && !DBNull.ReferenceEquals(dr["ITEM_VALUE"], DBNull.Value))
                                {
                                    try
                                    {
                                        vcch.Add(dr["ITEM_NAME"].ToString(), dr["ITEM_VALUE"].ToString());
                                    }
                                    catch (Exception ex1)
                                    {
                                        LogTool.SaveLogMessage(JsonConvert.SerializeObject(dr), "GetLastRTRecList", this.csName);
                                        this.logger.Fatal(ex1);
                                    }
                                }
                                   
                            }
                            RTVentilator vObj = JsonConvert.DeserializeObject<RTVentilator>(JsonConvert.SerializeObject(vcch));
                            vObj.RECORD_ID = checkDataColumn(drList[0], "RECORD_ID");
                            vObj.chart_no = checkDataColumn(drList[0], "chart_no");
                            vObj.ipd_no = checkDataColumn(drList[0], "ipd_no");
                            vObj.hasData = true;
                            vObj.ONMODE_ID_2 = checkDataColumn(drList[0], "ONMODE_TYPE2_ID");
                            vObj._is_humidifier = vObj.device;
                            _list.Add(vObj);
                        }
                    }
                    catch (Exception ex)
                    {
                        LogTool.SaveLogMessage(ex, "GetLastRTRecList", this.csName);
                        this.logger.Fatal(ex);
                    }
                    
                }
            }
            else
            {
                if (this.DBLink.DBA.hasLastError )
                    LogTool.SaveLogMessage(this.DBLink.DBA.lastError, "GetLastRTRecList", "RTModels");
            }
            return _list;
        }
         
        public RESPONSE_MSG DeleteItem(List<string> pKeyList)
        {
            RESPONSE_MSG rm = new RESPONSE_MSG();
            int delect_count = 0;
            List<string> msgList = new List<string>();
            List<DB_RCS_RT_CARE_SCHEDULING> pList = new List<DB_RCS_RT_CARE_SCHEDULING>();
            foreach (string key in pKeyList)
            {
                pList.Add(new DB_RCS_RT_CARE_SCHEDULING() { CARE_ID = key });
            }
            delect_count = this.DBLink.DBA.DBExecDelete<DB_RCS_RT_CARE_SCHEDULING>(pList);
            if (this.DBLink.DBA.hasLastError)
            {

                rm.status = RESPONSE_STATUS.ERROR;
                this.logger.Error(this.DBLink.DBA.lastError);
                msgList.Add("刪除失敗，請洽資訊人員!");
            }
            else
            {
                rm.status = RESPONSE_STATUS.SUCCESS;
                msgList.Add("共刪除" + delect_count + "筆!");
            }
            if (rm.status != RESPONSE_STATUS.SUCCESS)
            { 
                rm.messageList = msgList;
            }
            else
            {
                rm.attachment = msgList;
            } 
            return rm;
        }

        public RESPONSE_MSG JoinCareItem(IWebServiceParam iwp, IWebServiceParam ERAndOPD, ref List<IPDPatientInfo> pList, PAYLOAD payload)
        {
            RESPONSE_MSG rm = new RESPONSE_MSG();
            List<string> msgList = new List<string>();
            List<string> in_care_list = new List<string>(); //自己的照護清單
            List<string> is_care_list = new List<string>();//已被照護的清單列表
            List<IPDPatientInfo> other_Care_list = new List<IPDPatientInfo>(); //其他使用者也照護此病患 
            List<DB_RCS_RT_CASE> incaseList = new List<DB_RCS_RT_CASE>();
            List<DB_RCS_RT_CASE> updatecaseList = new List<DB_RCS_RT_CASE>();
            List<DB_RCS_RT_CARE_SCHEDULING> rtList = new List<DB_RCS_RT_CARE_SCHEDULING>();
            List<DB_RCS_RT_CASE> caseList = new List<DB_RCS_RT_CASE>();

            string _query = "";
            #region _query
            Dapper.DynamicParameters dp = new Dapper.DynamicParameters();
            try
            {
                _query = string.Concat(
                     "SELECT IPD_NO FROM " + DB_TABLE_NAME.DB_RCS_RT_CARE_SCHEDULING + " WHERE RT_ID = @RT_ID;", //自己的照護清單
                     "SELECT * FROM " + DB_TABLE_NAME.DB_RCS_RT_CARE_SCHEDULING + " WHERE RT_ID <> @RT_ID AND CHART_NO in @CHART_NO;", //其他使用者也照護此病患
                     "SELECT * FROM " + DB_TABLE_NAME.DB_RCS_RT_CASE + " WHERE IPD_NO in @IPD_NO;" // 收案過的病人資料
                 );

                dp.Add("RT_ID", payload.user_id);
                dp.Add("CHART_NO", pList.Select(x => x.chart_no).ToList());
                dp.Add("IPD_NO", pList.Select(x => x.ipd_no).ToList());

                this.DBLink.DBA.Open();
                GridReader gr = this.DBLink.DBA.dbConnection.QueryMultiple(_query, dp);
                in_care_list = gr.Read<string>().ToList();
                other_Care_list = gr.Read<IPDPatientInfo>().ToList();
                incaseList = gr.Read<DB_RCS_RT_CASE>().ToList();
            }
            catch (Exception ex)
            {
                rm.status = RESPONSE_STATUS.EXCEPTION;
                msgList.Add("程式發生錯誤，請洽資訊人員!");
                this.logger.Fatal(ex);
            }
            finally
            {
                this.DBLink.DBA.Close();
            }
            #endregion  
            if (rm.status == RESPONSE_STATUS.SUCCESS)
            {
                //加入清單的時候,取得webService病患資料
                for (int i = 0; i < pList.Count; i++)
                {
                    string chart_no = pList[i].chart_no;
                    string ipd_no = pList[i].ipd_no;
                    #region 判斷病人資料
                    //門診急診病患資料跳過
                    if (!string.IsNullOrWhiteSpace(pList[i].source_type) && pList[i].source_type != "I")
                    { 
                        List<IPDPatientInfo> eropdList = this.webmethod.getERAndOPDListbyChartNo(pList[i].chart_no, ERAndOPD);
                        if (!string.IsNullOrWhiteSpace(pList[i].chart_no) && !string.IsNullOrWhiteSpace(pList[i].ipd_no))
                        {
                            if (eropdList != null && eropdList.Count > 0 && eropdList.Exists(x => x.chart_no == chart_no && x.ipd_no == ipd_no))
                                pList[i] = eropdList.Find(x => x.chart_no == chart_no && x.ipd_no == ipd_no);
                        }
                        if (pList[i].source_type == "O")
                        {
                            pList[i].cost_code = "OPD";
                        }
                        if (pList[i].source_type == "E")
                        {
                            pList[i].cost_code = "ER";
                        }
                    }
                    else
                    {
                        #region webmethod 取得資料
                        if (!string.IsNullOrWhiteSpace(pList[i].chart_no) && !string.IsNullOrWhiteSpace(pList[i].ipd_no))
                        {
                            List<IPDPatientInfo> pat_info = this.webmethod.getPatientInfoListByIpdNo(iwp, pList[i].chart_no, pList[i].ipd_no);
                            if (pat_info != null && pat_info.Count > 0) pList[i] = pat_info[0];
                        }
                        else
                        {
                            this.logger.Fatal("病患資料缺少病歷號和批價序號，無法正常取得資料<JsonData>:" + JsonConvert.SerializeObject(pList[i]));
                        }
                        #endregion
                    }
                    string diag_date = "", source_type = "";
                    source_type = pList[i].source_type;
                    if (!string.IsNullOrEmpty(pList[i].diag_date) && !string.IsNullOrWhiteSpace(pList[i].diag_date) && DateHelper.isDate(pList[i].diag_date))
                    {
                        diag_date = DateHelper.Parse(pList[i].diag_date).ToString("yyyy-MM-dd HH:mm:ss");
                    }
                    if (string.IsNullOrEmpty(pList[i].source_type))
                    {
                        source_type = "I";
                    }
                    #region 加入收案資料
                    string tempIPD = pList[i].ipd_no;
                    if (!incaseList.Exists(x => x.IPD_NO == tempIPD))
                    {
                        caseList.Add(new DB_RCS_RT_CASE()
                        {
                            CASE_ID = this.DBLink.GetFixedStrSerialNumber(tempIPD),
                            CHART_NO = pList[i].chart_no,
                            IPD_NO = pList[i].ipd_no,
                            PATIENT_NAME = pList[i].patient_name,
                            ACCEPT_STATUS = "1",
                            CREATE_ID = payload.user_id,
                            CREATE_NAME = payload.user_name,
                            CREATE_DATE = Function_Library.getDateNowString(DATE_FORMAT.yyyy_MM_dd_HHmmss),
                            MODIFY_ID = payload.user_id,
                            MODIFY_NAME = payload.user_name,
                            MODIFY_DATE = Function_Library.getDateNowString(DATE_FORMAT.yyyy_MM_dd_HHmmss),
                            COST_CODE = pList[i].cost_code,
                            VS_DOC_ID = pList[i].vs_id,
                            VS_DOC_NAME = pList[i].vs_doc,
                            GENDER = pList[i].gender,
                            BIRTH_DAY = pList[i].birth_day,
                            PRE_DISCHARGE_DATE = pList[i].pre_discharge_date,
                            BED_NO = pList[i].bed_no,
                            DIAG_DATE = diag_date,
                            CARE_DATE = Function_Library.getDateNowString(DATE_FORMAT.yyyy_MM_dd),
                            PATIENT_SOURCE = source_type.Trim(),
                            DIAG_DESC = pList[i].diagnosis_code,
                            DEPT_CODE = pList[i].dept_code,
                            DEPT_NAME = pList[i].dept_desc, 
                            MDRO_MARK = pList[i].MDRO_MARK
                        });
                    }else
                    {
                        incaseList.Find(x => x.IPD_NO == tempIPD).PATIENT_NAME = pList[i].patient_name;
                        incaseList.Find(x => x.IPD_NO == tempIPD).MODIFY_ID = payload.user_id;
                        incaseList.Find(x => x.IPD_NO == tempIPD).MODIFY_NAME = payload.user_name;
                        incaseList.Find(x => x.IPD_NO == tempIPD).MODIFY_DATE = Function_Library.getDateNowString(DATE_FORMAT.yyyy_MM_dd_HHmmss);
                        incaseList.Find(x => x.IPD_NO == tempIPD).COST_CODE = pList[i].cost_code;
                        incaseList.Find(x => x.IPD_NO == tempIPD).VS_DOC_ID = pList[i].vs_id;
                        incaseList.Find(x => x.IPD_NO == tempIPD).VS_DOC_NAME = pList[i].vs_doc;
                        incaseList.Find(x => x.IPD_NO == tempIPD).BED_NO = pList[i].bed_no;
                        incaseList.Find(x => x.IPD_NO == tempIPD).BIRTH_DAY = pList[i].birth_day;
                        incaseList.Find(x => x.IPD_NO == tempIPD).DIAG_DATE = pList[i].diag_date;
                        incaseList.Find(x => x.IPD_NO == tempIPD).DEPT_CODE = pList[i].dept_code;
                        incaseList.Find(x => x.IPD_NO == tempIPD).DEPT_NAME = pList[i].dept_desc;
                        incaseList.Find(x => x.IPD_NO == tempIPD).MDRO_MARK = pList[i].MDRO_MARK;
                        incaseList.Find(x => x.IPD_NO == tempIPD).PRE_DISCHARGE_DATE = pList[i].pre_discharge_date;
                        updatecaseList.Add(Newtonsoft.Json.JsonConvert.DeserializeObject<DB_RCS_RT_CASE>(Newtonsoft.Json.JsonConvert.SerializeObject(incaseList.Find(x => x.IPD_NO == tempIPD))));
                      
                    }
                    #endregion
                    #region 加入照護清單
                    if (in_care_list.IndexOf(pList[i].ipd_no) >= 0)
                    {
                        // 已在清單中的不加入
                        is_care_list.Add(pList[i].patient_name);
                    }
                    else
                    {
                        // 加入照護清單 
                        rtList.Add(this.getDB_RCS_RT_CARE_SCHEDULING(payload.user_id, pList[i], diag_date));
                    }
                    #endregion
                    #endregion
                }
                int effRow = 0;
                try
                {
                    this.DBLink.DBA.Open();
                    effRow = this.DBLink.DBA.DBExecInsert<DB_RCS_RT_CARE_SCHEDULING>(rtList);
                    this.DBLink.DBA.DBExecInsert<DB_RCS_RT_CASE>(caseList);
                    this.DBLink.DBA.DBExecUpdate<DB_RCS_RT_CASE>(updatecaseList);
                    if (this.DBLink.DBA.hasLastError)
                    {
                        rm.status = RESPONSE_STATUS.ERROR;
                        msgList.Add("程式發生錯誤，請洽資訊人員!");
                        this.DBLink.DBA.Rollback();
                        this.logger.Fatal(this.DBLink.DBA.lastError);
                    }
                    else
                    { 
                        this.DBLink.DBA.Commit(); 
                    }
                }
                catch (Exception ex)
                {
                    rm.status = RESPONSE_STATUS.EXCEPTION;
                    msgList.Add("程式發生錯誤，請洽資訊人員!");
                    this.DBLink.DBA.Rollback();
                    this.logger.Fatal(ex);
                }
                finally
                {
                    this.DBLink.DBA.Close();
                }
                if (rm.status == RESPONSE_STATUS.SUCCESS)
                {
                    if (is_care_list.Count > 0)
                    {
                        msgList.Add(string.Join("、", is_care_list) + "已在自己的照護清單中");
                    }
                    if (other_Care_list != null && other_Care_list.Count > 0)
                    {
                        msgList.Add(string.Join("、", other_Care_list.Select(x => x.chart_no).Distinct().ToList()) + "已在其他RT的照護清單中");
                    }
                    if (effRow > 0)
                    {
                        msgList.Add("共加入" + effRow + "筆");
                    }
                }
            }
            if (rm.status != RESPONSE_STATUS.SUCCESS)
            {
                rm.messageList = msgList;
            }
            else
            {
                rm.attachment = msgList;
            } 
            return rm;
        }


        public RESPONSE_MSG SearchPatient(IWebServiceParam IHistory, IWebServiceParam IPatientInfo, IWebServiceParam erAndopd, FormSearchPatientList form)
        {
            string actionName = "SearchPatient";
            RESPONSE_MSG rm = new RESPONSE_MSG();
            List<PatientListItem> pList = new List<PatientListItem>();
            try
            {
                string ipd_no = "";
                if (!string.IsNullOrWhiteSpace(form.chart_no))
                {
                    List<PatientHistory> hisList = this.webmethod.getPatientHistory(IHistory, form.chart_no, ref ipd_no);
                    if (hisList.Count > 0)
                    {
                        if (hisList.Exists(x => x.outdate.Trim() == ""))
                        {
                            hisList.FindAll(x => x.outdate.Trim() == "").ForEach(x => x.outdate = Function_Library.getDateNowString(DATE_FORMAT.yyyy_MM_dd_125959));
                        }
                        if (hisList.Exists(x => x.indate != null && x.indate.Trim() != ""))
                        {
                            ipd_no = hisList.FindAll(x => x.indate != null && x.indate.Trim() != "").OrderByDescending(x => DateTime.Parse(x.outdate)).ThenByDescending(x => DateTime.Parse(x.indate)).ToList()[0].IPD_NO;
                        }
#if DEBUG
                        // ipd_no = hisList[0].IPD_NO;
#endif
                    }
                    if (!string.IsNullOrWhiteSpace(ipd_no))
                    {
                        List<IPDPatientInfo> PtInfo = this.webmethod.getPatientInfoListByIpdNo(IPatientInfo, form.chart_no, ipd_no);
                        if (PtInfo != null && PtInfo.Count > 0)
                            pList.AddRange(Newtonsoft.Json.JsonConvert.DeserializeObject<List<PatientListItem>>(Newtonsoft.Json.JsonConvert.SerializeObject(PtInfo)));
                   
                    }
                    List<IPDPatientInfo> erAndOPDList = this.webmethod.getERAndOPDListbyChartNo(form.chart_no, erAndopd);
                    if (erAndOPDList.Count > 0)
                    {
                        pList.AddRange(Newtonsoft.Json.JsonConvert.DeserializeObject<List<PatientListItem>>(Newtonsoft.Json.JsonConvert.SerializeObject(erAndOPDList)));
                    }
                    rm.attachment = pList;
                }
                else
                {
                    rm.status = RESPONSE_STATUS.ERROR;
                    rm.message = "請輸入病歷號!"; 
                }
            }
            catch (Exception ex)
            {
                rm.status = RESPONSE_STATUS.EXCEPTION;
                rm.message = "查詢失敗!";
                LogTool.SaveLogMessage(ex, actionName, this.csName);
            }

            return rm;
         
        }

        public RESPONSE_MSG EditPat(FormEditPat form)
        {
            RESPONSE_MSG rm = new RESPONSE_MSG();
            List<DB_RCS_RT_CASE> pList = new List<DB_RCS_RT_CASE>();
            DynamicParameters dp = new DynamicParameters();
            string query = "SELECT * FROM " + DB_TABLE_NAME.DB_RCS_RT_CASE + " A WHERE CHART_NO =@CHART_NO AND IPD_NO = @IPD_NO ";
            query += " ORDER BY CREATE_DATE DESC";
            dp.Add("CHART_NO", form.chart_no);
            dp.Add("IPD_NO", form.ipd_no);
            pList = this.DBLink.DBA.getSqlDataTable<DB_RCS_RT_CASE>(query, dp);
            if (pList.Count > 0)
            {
                pList.ForEach(x=>x.DIAG_DESC = form.diagnosis_code);
            }
            this.DBLink.DBA.DBExecUpdate<DB_RCS_RT_CASE>(pList);
            if (this.DBLink.DBA.hasLastError)
            {
                LogTool.SaveLogMessage(this.DBLink.DBA.lastError, "EditPat", "RTModels");
                rm.message = "修改失敗!";
            }
            rm.attachment = "修改成功";
            return rm;
        }


        public virtual List<PatientListItem> getRtCareData(List<PatientListItem> List)
        {
            return List;
        }
    }

    public class FormSearchPatientList : AUTH
    {
        public string chart_no { get; set; }
        public string case_code { get; set; }
        public string area_code { get; set; }
    }

    public class FormRT : AUTH
    {
        public string join_json { get; set; }
    }

    public class FormDeleteItem : AUTH
    {
        public string delete_json { get; set; }
    }
    public class FormEditPat : AUTH
    {
        public string chart_no { get; set; }
        public string ipd_no { get; set; }
        public string diagnosis_code { get; set; }
    }
     
}

//**************************************************
//2016/08/19
//#2172 建立DB Model
//功能 照護病患清單、照護病患紀錄、授權清單取得資料
//2016/08/19 整理:將RTController的資料移動至這裡
//2016/08/22 整理:取得支援清單
//2016/08/23 整理:加入、刪除及結案照護清單
//2016/09/09 #2180 修改照護清單SQL
//**************************************************
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using mayaminer.com.library;
using RCS_Data;
using Newtonsoft.Json;
using Com.Mayaminer;
using System.Globalization;
using mayaminer.com.jxDB;
using System.Web.Mvc;
using RCS.Models.DB;
using System.Reflection;
using System.Xml.Serialization;
using System.IO;
using System.Xml;
using System.Text;
using Dapper;
using RCSData.Models;
using RCS_Data.Controllers.RT;
using RCS_Data.Models.ViewModels;

namespace RCS.Models
{
    /// <summary>照護病患清單、照護病患紀錄、授權清單取得資料</summary>
    public class RT:BaseModel
    {
        /// <summary>回傳訊息</summary>
        public List<string> errorMsg { get; set; }
        /// <summary>照護清單</summary>
        public List<PatientListItem> PatientList = new List<PatientListItem>();
        /// <summary>授權清單</summary>
        public List<RCS_DATA_RtGrantList> RtGrantList = new List<RCS_DATA_RtGrantList>();

        /// <summary>照護病患清單、照護病患紀錄、授權清單取得資料</summary>
        public RT()
        {

        }

        #region 取得照護清單

        /// <summary>取得照護清單</summary>
        /// <param name="op_id">護理人員ID(RT員編)</param>
        /// <param name="type_mode">{ C: 照護病人清單 , H: 出院病人清單 }</param>
        /// <param name="pChart_no">病歷號</param>
        /// <param name="pDiag_date">入院日期</param>
        /// <returns>List</returns>
        public List<PatientListItem> get_CareIList(string op_id, string type_mode = "C", string pChart_no = "", string pDiag_date = "")
        {
            string actionName = "get_CareIList";
            List<PatientListItem> pat_list = new List<PatientListItem>();
            try
            {
                string sqlstatment = "";
                string sql_str = "";
                if (type_mode == "H")
                {
                    sql_str = "SELECT * FROM  " + GetTableName.RCS_RT_CASE + " a " +
                                   " left join " + GetTableName.RCS_RT_CARE_SCHEDULING + " b on a.IPD_NO = b.IPD_NO ";
                    if (pChart_no != null && pChart_no != "") sql_str += " where a.CHART_NO = " + SQLDefend.SQLString(pChart_no);
                    if (pDiag_date != null && pDiag_date != "") sql_str += " AND a.diag_date = " + SQLDefend.SQLString(pDiag_date);
                }
                else
                {
                    sql_str = "SELECT * FROM  " + GetTableName.RCS_RT_CARE_SCHEDULING + " a " +
                                   " left join " + GetTableName.RCS_RT_CASE + " b on a.IPD_NO = b.IPD_NO and b.CREATE_DATE = (SELECT MAX(CREATE_DATE) FROM " + GetTableName.RCS_RT_CASE + " WHERE IPD_NO=b.IPD_NO )" +
                                   " where a.RT_ID = {0}";
                    if (pChart_no != null && pChart_no != "") sql_str += " AND a.CHART_NO = " + SQLDefend.SQLString(pChart_no);
                }
               
                sqlstatment = string.Format(sql_str, SQLDefend.SQLString(op_id));
                pat_list = getPatientListData(sqlstatment);
                if (pat_list == null)
                    LogTool.SaveLogMessage(this.DBA.LastError, actionName, GetLogToolCS.RT);

            }
            catch (Exception ex)
            {
                LogTool.SaveLogMessage(ex, actionName, GetLogToolCS.RT);
            }
            finally
            {
                //判斷是否是病患記錄資料
                if (type_mode == "H")pat_list.ForEach(x => x.isHistoryData = true);
            }
            return pat_list;
        }

        /// <summary>取得照護清單</summary>
        /// <param name="sql">sql</param>
        /// <returns></returns>
        public List<PatientListItem> getPatientListData(string sql)
        {
            string actionName = "getPatientListData";
            List<PatientListItem> pat_list = new List<PatientListItem>();
            try
            {
                DataTable dt = this.DBA.getSqlDataTable(sql);
                if (dt != null && dt.Rows.Count > 0)
                { 
                    string[] getList = new string[8] { "device", "respid", "mode", "device_o2", "memo", "recorddate", "recordtime", "is_humidifier" };//取得Ventilator項目
                    List<RTVentilator> ventilatorList = this.GetLastRTRecList(dt.AsEnumerable().Select(x=>x["ipd_no"].ToString()).Distinct().ToList(), getList);
                    List<string> ipd_no_list = new List<string>();
                    foreach (DataRow dr in dt.Rows)
                    {
                        string chart_no = DBNull.ReferenceEquals(dr["CHART_NO"], DBNull.Value) ? null : dr["CHART_NO"].ToString().Trim();
                        string ipd_no = DBNull.ReferenceEquals(dr["ipd_no"], DBNull.Value) ? null : dr["ipd_no"].ToString().Trim();
                        //List<IPDPatientInfo> pat_info = web_method.getPatientInfoList(chart_no, ipd_no);
                        //IPDPatientInfo patInfo = new IPDPatientInfo();
                        //if (pat_info != null && pat_info.Count > 0) patInfo = pat_info[0];
                        //Ventilator LastRTRecord = this.GetLastRTRec(chart_no, ipd_no, getList);
                        List<string> on_dateLIst = new List<string>();
                        int use_days = this.getUseDays(ipd_no, chart_no, out on_dateLIst);
                        PatientListItem inst = new PatientListItem();
                        inst.r_id = DBNull.ReferenceEquals(dr["CARE_ID"], DBNull.Value) ? "" : dr["CARE_ID"].ToString().Trim();
                        inst.type_mode = DBNull.ReferenceEquals(dr["TYPE_MODE"], DBNull.Value) ? "" : dr["TYPE_MODE"].ToString().Trim();
                        inst.case_id = DBNull.ReferenceEquals(dr["CASE_ID"], DBNull.Value) ? "" : dr["CASE_ID"].ToString().Trim();//收案編號
                        inst.bed_no = DBNull.ReferenceEquals(dr["bed_no"], DBNull.Value) ? "" : dr["bed_no"].ToString().Trim();//床號
                        inst.cost_code = DBNull.ReferenceEquals(dr["COST_CODE"], DBNull.Value) ? "" : dr["COST_CODE"].ToString().Trim(); 
                        inst.dept_code = DBNull.ReferenceEquals(dr["DEPT_CODE"], DBNull.Value) ? "" : dr["DEPT_CODE"].ToString().Trim();//護理站
                        inst.dept_desc = DBNull.ReferenceEquals(dr["DEPT_NAME"], DBNull.Value) ? "" : dr["DEPT_NAME"].ToString().Trim();//護理站;
                        inst.MDRO_MARK = DBNull.ReferenceEquals(dr["MDRO_MARK"], DBNull.Value) ? "" : dr["MDRO_MARK"].ToString().Trim();//MDRO_MARK;
                        inst.ipd_no = ipd_no;//
                        inst.chart_no = chart_no;//病歷號
                        inst.patient_name = DBNull.ReferenceEquals(dr["PATIENT_NAME"], DBNull.Value) ? "" : dr["PATIENT_NAME"].ToString().Trim();//姓名
                        inst.use_days = use_days;//天數
                        for (int i = 0; i < on_dateLIst.Count; i++)
                        {
                            if (DateHelper.isDate(on_dateLIst[i]))
                            {
                                on_dateLIst[i] = DateTime.Parse(on_dateLIst[i]).ToString("yyyy-MM-dd");
                            }
                        }
                        inst._on_mode = on_dateLIst;
                        inst.diag_date = DBNull.ReferenceEquals(dr["DIAG_DATE"], DBNull.Value) ? "" : DateHelper.Parse(dr["DIAG_DATE"].ToString().Trim(), true).ToString("yyyy-MM-dd HH:mm");//入院時間
                        inst.vs_doc = DBNull.ReferenceEquals(dr["VS_DOC_NAME"], DBNull.Value) ? "" : dr["VS_DOC_NAME"].ToString().Trim();//主治醫師
                        inst.vs_id = DBNull.ReferenceEquals(dr["VS_DOC_ID"], DBNull.Value) ? "" : dr["VS_DOC_ID"].ToString().Trim();//主治醫師
                        inst.case_vs_name = DBNull.ReferenceEquals(dr["VS_DOC_NAME"], DBNull.Value) ? "" : dr["VS_DOC_NAME"].ToString().Trim();//主治醫師
                        inst.birth_day = DBNull.ReferenceEquals(dr["BIRTH_DAY"], DBNull.Value) ? "" : dr["BIRTH_DAY"].ToString().Trim();//生日
                        if (mayaminer.com.library.DateHelper.isDate(inst.birth_day, "yyyyMMdd"))
                        {
                            inst.birth_day = DateHelper.Parse(inst.birth_day).ToString("yyyy/MM/dd");
                        }
                        if (mayaminer.com.library.DateHelper.isDate(inst.birth_day, "yyyMMdd"))
                        {
                            inst.birth_day = DateHelper.ParseTWDate(inst.birth_day).ToString("yyyy/MM/dd");
                        }
                        inst.gender = DBNull.ReferenceEquals(dr["GENDER"], DBNull.Value) ? "" : dr["GENDER"].ToString().Trim();//性別
                        inst.body_height = DBNull.ReferenceEquals(dr["body_height"], DBNull.Value) ? "" : dr["body_height"].ToString().Trim();//身高
                        inst.pre_discharge_date = DBNull.ReferenceEquals(dr["PRE_DISCHARGE_DATE"], DBNull.Value) || !mayaminer.com.library.DateHelper.isDate(dr["PRE_DISCHARGE_DATE"].ToString()) ? "" : DateHelper.Parse(dr["PRE_DISCHARGE_DATE"].ToString().Trim(), true).ToString("yyyy-MM-dd HH:mm");//入院時間
                        inst.diagnosis_code = DBNull.ReferenceEquals(dr["DIAG_DESC"], DBNull.Value) ? "" : dr["DIAG_DESC"].ToString().Trim();//DIAG_DESC
                        inst.source_type = DBNull.ReferenceEquals(dr["PATIENT_SOURCE"], DBNull.Value) ? "" : dr["PATIENT_SOURCE"].ToString().Trim();
                        if (ventilatorList.Exists(x=>x.chart_no == chart_no && x.ipd_no == ipd_no))
                        {
                            inst.machine = ventilatorList.Find(x => x.chart_no == chart_no && x.ipd_no == ipd_no);
                            inst.accept_status = "1";
                        }
                        //判斷是否需要填寫呼吸維護單
                        //inst.need_maintain = Check_breath_rㄎecord(inst.ipd_no, inst.chart_no).need_maintain;
                        pat_list.Add(inst);
                    }
                }
                else if (dt == null)
                    LogTool.SaveLogMessage(this.DBA.LastError, actionName, GetLogToolCS.RT);
            }
            catch (Exception ex)
            {
                LogTool.SaveLogMessage(ex, actionName, GetLogToolCS.RT);
            }

            return pat_list;
        }


        #endregion

        #region 照護清單功能(加入、刪除、結案)
        /// <summary>加入照護、歷史清單</summary>
        /// <param name="join_json">病患資料組</param>
        /// <param name="type_mode"></param>
        /// <returns></returns>
        public WebServiceResponse JoinCareItem(IWebServiceParam iwp, string join_json)
        {
            string actionName = "JoinCareItem";
            WebServiceResponse return_ws = new WebServiceResponse();
            try
            {
                SQLProvider SQL = new SQLProvider();
                List<IPDPatientInfo> join_obj = new List<IPDPatientInfo>();
                join_json = HttpUtility.UrlDecode(join_json);
                join_obj = JsonConvert.DeserializeObject<List<IPDPatientInfo>>(join_json);
                DataTable dt = this.DBA.getSqlDataTable("SELECT * FROM " + GetTableName.RCS_RT_CARE_SCHEDULING + " WHERE 1 <> 1");
                DataTable dtRCS_RT_CASE = this.DBA.getSqlDataTable("SELECT * FROM " + GetTableName.RCS_RT_CASE + " WHERE 1 <> 1"); 
                List<string> in_care_list = new List<string>(); //自己的照護清單
                List<string> is_care_list = new List<string>();//已被照護的清單列表
                                                               //System.Threading.Thread.Sleep(1000);//模擬等待
                int effRow = 0;
                if (!string.IsNullOrEmpty(user_info.user_id) && join_obj != null && join_obj.Count>0)
                {

                    //取得現有照護清單
                    Dapper.DynamicParameters dp = new DynamicParameters();
                    string _query = "SELECT IPD_NO FROM RCS_RT_CARE_SCHEDULING WHERE RT_ID = @RT_ID";
                    dp.Add("RT_ID", user_info.user_id);
                    in_care_list = SQL.DBA.getSqlDataTable<string>(_query, dp);
                    
                    //檢查是否有其他使用者也照護此病患
                    string sql = "SELECT * FROM " + GetTableName.RCS_RT_CARE_SCHEDULING + " WHERE RT_ID <>" + SQLDefend.SQLString(user_info.user_id) + " AND CHART_NO in('"+string.Join("','", join_obj.Select(x=>x.chart_no).ToList()) +"')";
                    List<PatientListItem> other_Care_list = this.DBA.Connection.Query<PatientListItem>(sql).ToList();
                    if(!string.IsNullOrWhiteSpace(this.DBA.LastError))
                    {
                        LogTool.SaveLogMessage(this.DBA.LastError,actionName,GetLogToolCS.RT);
                    }
                    //將照護清單寫入DB
                    int join_count = join_obj.Count;
                    //加入清單的時候,取得webService病患資料
                    for (int i = 0; i < join_obj.Count; i++)
                    {
                        //門診急診病患資料跳過
                        if (!string.IsNullOrWhiteSpace(join_obj[i].source_type) && join_obj[i].source_type != "I")
                        {
                            if (join_obj[i].source_type == "O")
                            {
                                join_obj[i].cost_code = "9999";
                            }
                            if (join_obj[i].source_type == "E")
                            {
                                join_obj[i].cost_code = "8888";
                            }
                            continue;
                        }
                        if (!string.IsNullOrWhiteSpace(join_obj[i].chart_no) && !string.IsNullOrWhiteSpace(join_obj[i].ipd_no))
                        {
                            List<IPDPatientInfo> pat_info = web_method.getPatientInfoList(iwp, join_obj[i].chart_no, join_obj[i].ipd_no);
                            if (pat_info != null && pat_info.Count > 0) join_obj[i] = pat_info[0];
                        }
                        else
                            LogTool.SaveLogMessage("病患資料缺少病歷號和批價序號，無法正常取得資料<JsonData>:" + JsonConvert.SerializeObject(join_obj[i]), actionName, GetLogToolCS.RT);
                    }
                    foreach (IPDPatientInfo j_obj in join_obj)
                    {
                        DataRow dr = null;
                        dbResultMessage msg = new dbResultMessage();
                        string DIAG_DATE = "";
                        if (join_count == 0) break;
                        
                        if (in_care_list.IndexOf(j_obj.ipd_no) == -1)
                        {
                            effRow++;
                            #region 加入照護清單

                            dr = dt.NewRow();
                            dr["CARE_ID"] = SQL.GetFixedStrSerialNumber(j_obj.ipd_no);
                            dr["RT_ID"] = user_info.user_id;
                            dr["CHART_NO"] = j_obj.chart_no;
                            dr["IPD_NO"] = j_obj.ipd_no;
                            dr["PATIENT_NAME"] = j_obj.patient_name;
                            dr["GENDER"] = j_obj.gender;
                            dr["BIRTH_DAY"] = j_obj.birth_day;
                            dr["COST_CODE"] = j_obj.cost_code;
                            dr["BED_NO"] = j_obj.bed_no; 
                            if (mayaminer.com.library.DateHelper.isDate(j_obj.diag_date, "yyyMMddHHmm" ))
                            {
                                j_obj.diag_date = DateHelper.ParseTWDate(j_obj.diag_date).ToString("yyyy/MM/dd HH:mm");
                            }
                            DIAG_DATE = DateHelper.toTWDate(j_obj.diag_date, "yyyy-MM-dd HH:mm");
                            dr["DIAG_DATE"] = DIAG_DATE;
                            dr["CARE_DATE"] = DateTime.Now.Date.ToString("yyyy-MM-dd");
                            dr["CREATE_DATE"] = DateTime.Now.Date.ToString("yyyy-MM-dd HH:mm:ss");
                            dr["VS_DOC_NAME"] = j_obj.vs_doc;
                            dr["PRE_DISCHARGE_DATE"] = j_obj.pre_discharge_date;
                            dr["ROOM_NO"] = j_obj.room_no;
                            dr["loc"] = j_obj.loc;
                            dr["TYPE_MODE"] = "C";
                            dr["VS_DOC_ID"] = j_obj.vs_id;
                            dr["NHI_OPD_ER"] = j_obj.source_type.Trim();

                            #endregion
                            // insert
                            dt.Rows.Add(dr);
                        }
                        else
                        {
                            //已在清單中的不加入
                            is_care_list.Add(j_obj.patient_name);
                            join_count = join_count - 1;
                        }
                        #region //檢查師否收案過
                        string table_name = GetTableName.RCS_RT_CASE.ToString();
                        bool exist_data = false;
                        dr = null;
                        string BODY_HEIGHT = "";
                        DataTable tempdtRCS_RT_CASE = new DataTable();
                        object tempobj = this.DBA.ExecuteScalar(string.Format("SELECT BODY_HEIGHT FROM {0} WHERE IPD_NO = {1} AND ACCEPT_STATUS = '2'", table_name, j_obj.ipd_no));
                        if (!string.ReferenceEquals(tempobj, DBNull.Value)) BODY_HEIGHT = (string)tempobj;
                        string sqlstr = string.Format(" select * from {0} where IPD_NO = '{1}' and ACCEPT_STATUS = '1' ", new string[] { table_name, j_obj.ipd_no });
                        tempdtRCS_RT_CASE = this.DBA.getSqlDataTable(sqlstr);
                        if (tempdtRCS_RT_CASE != null && tempdtRCS_RT_CASE.Rows.Count > 0)
                            exist_data = true;
                        else if (this.DBA.LastError != null && this.DBA.LastError != "")
                        {
                            LogTool.SaveLogMessage(this.DBA.LastError, "JoinCareItem2", GetLogToolCS.BaseModel);
                        }

                        if (!exist_data && (this.DBA.LastError == null || this.DBA.LastError == ""))
                        {
                            //未收案時，新增收案
                            #region 未收案時，新增收案
                            string case_id = SQL.GetFixedStrSerialNumber(j_obj.ipd_no);
                            dr = dtRCS_RT_CASE.NewRow();
                            dr["CASE_ID"] = case_id;
                            dr["CHART_NO"] = string.IsNullOrEmpty(j_obj.chart_no) ? "" : j_obj.chart_no.Trim();
                            dr["IPD_NO"] = string.IsNullOrEmpty(j_obj.ipd_no) ? "" : j_obj.ipd_no.Trim();
                            dr["PATIENT_NAME"] = string.IsNullOrEmpty(j_obj.patient_name) ? "" : j_obj.patient_name.Trim();
                            dr["ACCEPT_STATUS"] = "1";
                            dr["CREATE_ID"] = user_info.user_id.Trim();
                            dr["CREATE_NAME"] = user_info.user_name.Trim();
                            dr["CREATE_DATE"] = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                            dr["MODIFY_NAME"] = user_info.user_name.Trim();
                            dr["MODIFY_ID"] = user_info.user_id.Trim();
                            dr["MODIFY_DATE"] = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                            if (BODY_HEIGHT != null) dr["BODY_HEIGHT"] = BODY_HEIGHT.Trim();
                            dr["COST_CODE"] = string.IsNullOrEmpty(j_obj.cost_code) ? "" : j_obj.cost_code.Trim();
                            dr["VS_DOC_ID"] = string.IsNullOrEmpty(j_obj.vs_id) ? "" : j_obj.vs_id.Trim();
                            dr["VS_DOC_NAME"] = string.IsNullOrEmpty(j_obj.vs_doc) ? "" : j_obj.vs_doc.Trim();
                            dr["GENDER"] = string.IsNullOrEmpty(j_obj.gender) ? "" : j_obj.gender.Trim();
                            dr["BIRTH_DAY"] = string.IsNullOrEmpty(j_obj.birth_day) ? "" : j_obj.birth_day.Trim();
                            dr["BED_NO"] = string.IsNullOrEmpty(j_obj.bed_no) ? "" : j_obj.bed_no.Trim();
                            if (!string.IsNullOrEmpty(j_obj.diag_date) && !string.IsNullOrWhiteSpace(j_obj.diag_date) && DateHelper.isDate(j_obj.diag_date))
                                dr["DIAG_DATE"] = DateHelper.Parse(j_obj.diag_date).ToString("yyyy-MM-dd HH:mm:ss");
                            dr["CARE_DATE"] = DateTime.Now.ToString("yyyy-MM-dd");
                            if (string.IsNullOrEmpty(j_obj.source_type))
                            {
                                dr["PATIENT_SOURCE"] = "I";//空值給住院"I" 20170213
                            }
                            else
                            {
                                dr["PATIENT_SOURCE"] = j_obj.source_type.Trim();
                            }
                            if (!string.IsNullOrWhiteSpace(j_obj.diagnosis_cname))
                            {
                                j_obj.diagnosis_code = j_obj.diagnosis_code +  j_obj.diagnosis_cname;
                            }
                            dr["DIAG_DESC"] = string.IsNullOrEmpty(j_obj.diagnosis_code) ? "" : j_obj.diagnosis_code.Trim();
                            dr["DEPT_CODE"] = string.IsNullOrEmpty(j_obj.dept_code) ? "" : j_obj.dept_code.Trim();
                            dr["DEPT_NAME"] = string.IsNullOrEmpty(j_obj.dept_desc) ? "" : j_obj.dept_desc.Trim();
                            dr["PATIENT_IDNO"] = string.IsNullOrEmpty(j_obj.idno) ? "" : j_obj.idno.Trim();
                            dr["MDRO_MARK"] = string.IsNullOrEmpty(j_obj.MDRO_MARK) ? "" : j_obj.MDRO_MARK.Trim();
                            #endregion
                            dtRCS_RT_CASE.Rows.Add(dr);
                        }
                        #endregion 
                       
                    }

                    List<string> ws_result_list = new List<string>();
                    this.DBA.BeginTrans();
                    if (string.IsNullOrWhiteSpace(this.DBA.LastError))
                        this.DBA.Update(dt, GetTableName.RCS_RT_CARE_SCHEDULING.ToString());
                    if (string.IsNullOrWhiteSpace(this.DBA.LastError))
                        this.DBA.Update(dtRCS_RT_CASE, GetTableName.RCS_RT_CASE.ToString()); 
                    if (string.IsNullOrWhiteSpace(this.DBA.LastError))
                        this.DBA.Commit();
                    else
                    {
                        ws_result_list.Add("程式發生錯誤，請洽資訊人員!錯誤訊息如下所示!" + this.DBA.LastError);
                        return_ws.error_msg = string.Join(",", ws_result_list);
                        LogTool.SaveLogMessage(this.DBA.LastError, actionName, GetLogToolCS.RT);
                        this.DBA.Rollback();
                    }

                    if (is_care_list.Count > 0)
                    {
                        ws_result_list.Add(string.Join("、", is_care_list) + "已在自己的照護清單中");
                    }
                    if (other_Care_list != null && other_Care_list.Count > 0)
                    {
                        ws_result_list.Add(string.Join("、", other_Care_list.Select(x=>x.chart_no).Distinct().ToList()) + "已在其他RT的照護清單中");
                    }
                    if (effRow > 0 && (this.DBA.LastError == null || this.DBA.LastError == ""))
                    {
                        ws_result_list.Add("共加入" + effRow + "筆");
                    }
                    return_ws.ws_result = string.Join("<br />", ws_result_list);
                    if (this.DBA.LastError == null || this.DBA.LastError == "")
                    {
                        return_ws.error_msg = "";
                        return_ws.execute_state = true;
                    }
                }
                else
                {
                    return_ws.ws_result = "失敗!";
                    return_ws.error_msg = "您尚未登入!";
                    return_ws.execute_state = false;
                }
            }
            catch (Exception ex)
            {
                LogTool.SaveLogMessage(ex.Message, actionName, GetLogToolCS.RT);
                return_ws.error_msg = "加入照護、歷史清單" + ExceptionMsg;
                return_ws.ws_result = "失敗!";
                return_ws.execute_state = false;
                this.DBA.Rollback();
            }



            return return_ws;
        }

        /// <summary>刪除照護病患
        /// <para>ipd_json有資料，代表為結案，需刪除其他人的清單</para>
        /// </summary>
        /// <param name="delete_json">刪除病患r_id組</param>
        /// <param name="ipd_json">批價序號json</param>
        /// <returns></returns>
        public WebServiceResponse DeleteItem(string delete_json, string ipd_json)
        {
            string actionName = "DeleteItem";
            WebServiceResponse return_ws = new WebServiceResponse();
            try
            {
                List<string> delect_obj = JsonConvert.DeserializeObject<List<string>>(delete_json);

                int delect_count = delect_obj.Count;
                int effRow = 0;
                if (user_info != null)
                {
                    try
                    {
                        string sql = string.Format("SELECT * FROM " + GetTableName.RCS_RT_CARE_SCHEDULING + " WHERE CARE_ID in('{0}')", string.Join("','", delect_obj));
                        //如果批價序號有資料，結案時要刪除其他人的資料
                        if (ipd_json != null && ipd_json != "")
                        {
                            List<string> ipd_jsonList = JsonConvert.DeserializeObject<List<string>>(ipd_json);
                            sql += " OR IPD_NO in('" + string.Join("','", ipd_jsonList) + "')";
                        }
                        DataTable dt = this.DBA.getSqlDataTable(sql);
                        this.DBA.BeginTrans();
                        foreach (DataRow dr in dt.Rows)
                        {
                            dr.Delete();
                        }
                        dbResultMessage msg = this.DBA.UpdateResult(dt, GetTableName.RCS_RT_CARE_SCHEDULING.ToString());
                        if (msg.State == enmDBResultState.Success)
                        {
                            this.DBA.Commit();
                            return_ws.ws_result = "共刪除" + delect_obj.Count + "筆!";
                            return_ws.error_msg = "共刪除" + delect_obj.Count + "筆!";
                            return_ws.execute_state = true;
                        }
                        else
                        {
                            this.DBA.Rollback();
                            return_ws.ws_result = "刪除失敗，請洽資訊人員!";
                            return_ws.error_msg = "共" + delect_count + "筆資料，刪除" + effRow + "筆，刪除失敗，請洽資訊人員!";
                            return_ws.execute_state = false;
                            LogTool.SaveLogMessage(string.Format("使用者({0})病歷號{1}({2})錯誤訊息={3}", user_info.user_id, pat_info.chart_no, pat_info.ipd_no, msg.dbErrorMessage), actionName, GetLogToolCS.RT);
                        }
                    }
                    catch (Exception ex)
                    {
                        LogTool.SaveLogMessage(ex.Message, actionName, GetLogToolCS.RT);
                        return_ws.ws_result = ExceptionMsg;
                        return_ws.execute_state = false;
                    }
                }
                else
                {
                    return_ws.ws_result = "失敗!";
                    return_ws.error_msg = "您尚未登入!";
                    return_ws.execute_state = false;
                }
            }
            catch (Exception ex)
            {
                LogTool.SaveLogMessage(ex.Message, actionName, GetLogToolCS.RT);
                return_ws.error_msg = "刪除照護病患" + ExceptionMsg;
                return_ws.ws_result = "失敗!";
                return_ws.execute_state = false;
            }

            return return_ws;
        }

        /// <summary>結案照護病患</summary>
        /// <param name="end_json">結案病患r_id組</param>
        /// <returns></returns>
        public WebServiceResponse EndItem(string end_json)
        {
            string actionName = "EndItem";
            WebServiceResponse return_ws = new WebServiceResponse();
            bool canEndItem = true;
            return_ws.error_msg = "";
            try
            {
                List<string> end_obj = JsonConvert.DeserializeObject<List<string>>(end_json);
                int end_count = end_obj.Count;
                string sql = string.Format(@"SELECT 'RECORD' RECORD, RECORD_ID RECORD_ID,UPLOAD_STATUS FROM RCS_RECORD_MASTER WHERE IPD_NO in('{0}') 
UNION SELECT  'CPT' RECORD,CPTR_ID RECORD_ID,UPLOAD_STATUS FROM RCS_CPT_RECORD WHERE IPD_NO in('{0}')
UNION SELECT 'WEANING' RECORD, TK_ID RECORD_ID,UPLOAD_STATUS FROM RCS_WEANING_ASSESS WHERE IPD_NO in('{0}')
UNION SELECT 'ONMODE' RECORD, ONMODE_ID RECORD_ID,UPLOAD_STATUS FROM RCS_ONMODE_MASTER WHERE IPD_NO in('{0}') AND ON_TYPE = '2'", string.Join("','", end_obj));
                DataTable dt = this.DBA.getSqlDataTable(sql);
                if (DTNotNullAndEmpty(dt))
                {
                    //if (dt.AsEnumerable().ToList().Exists(x => !DBNull.ReferenceEquals(x["UPLOAD_STATUS"], DBNull.Value) && x["UPLOAD_STATUS"].ToString() == "0"))
                    //{
                    //    //return_ws.execute_state = false;
                    //    //List<string> tempList = dt.AsEnumerable().ToList().FindAll(x => !DBNull.ReferenceEquals(x["UPLOAD_STATUS"], DBNull.Value) && x["UPLOAD_STATUS"].ToString() == "0").Select(x => x["RECORD"].ToString()).Distinct().ToList();
                    //    //return_ws.error_msg += string.Join(",", tempList) + "的資料尚未上傳，無法結案!\n";
                    //}
                    //else
                    //{
                    //    if (dt.AsEnumerable().ToList().Exists(x => !DBNull.ReferenceEquals(x["UPLOAD_STATUS"], DBNull.Value) && x["UPLOAD_STATUS"].ToString() == "1"))
                    //    {
                    //        List<string> tempList = dt.AsEnumerable().ToList().FindAll(x => !DBNull.ReferenceEquals(x["UPLOAD_STATUS"], DBNull.Value) && x["UPLOAD_STATUS"].ToString() == "1").Select(x => x["RECORD"].ToString()).Distinct().ToList();
                    //        List<string> recordList = new List<string>();
                    //        recordList.Add("呼吸照護記錄單");
                    //        recordList.Add("胸腔記錄單");
                    //        recordList.Add("呼吸脫離評估單");
                    //        if (!(tempList.Contains(recordList[0]) || tempList.Contains(recordList[1]) || tempList.Contains(recordList[2])))
                    //        {
                    //            return_ws.execute_state = false;
                    //            return_ws.error_msg += "沒有記錄資料，無法結案!\n";
                    //        }
                    //    }
                    //    else
                    //    {
                    //        //return_ws.execute_state = false;
                    //        //return_ws.error_msg += "沒有上傳資料，無法結案!\n";
                    //    }
                    //}
                }
                else
                {
                    canEndItem = false;
                    if (!string.IsNullOrWhiteSpace(this.DBA.LastError)){
                         LogTool.SaveLogMessage(this.DBA.LastError, actionName,GetLogToolCS.RT);
                        return_ws.execute_state = false;
                        return_ws.error_msg += "結案發生錯誤，無法結案!\n";
                    }
                    else
                    {
                        return_ws.execute_state = false;
                        return_ws.error_msg += "沒有記錄資料，無法結案!\n";
                    }
                }
                if (canEndItem)
                {
                    if (user_info != null)
                    {
                        sql = string.Format("SELECT * FROM " + GetTableName.RCS_RT_CASE + " WHERE IPD_NO in('{0}') AND ACCEPT_STATUS = '1'", string.Join("','", end_obj));
                        dt = this.DBA.getSqlDataTable(sql);
                        this.DBA.BeginTrans();
                        foreach (DataRow dr in dt.Rows)
                        {
                            dr["MODIFY_ID"] = user_info.user_id;
                            dr["MODIFY_NAME"] = user_info.user_name;
                            dr["MODIFY_DATE"] = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                            dr["ACCEPT_STATUS"] = "2";
                        }
                        dbResultMessage DBAMsg = this.DBA.UpdateResult(dt, GetTableName.RCS_RT_CASE.ToString());
                        if (DBAMsg.State == enmDBResultState.Success)
                        {
                            this.DBA.Commit();
                            return_ws.ws_result = "共結案" + DBAMsg.dbReturnValue + "筆!";
                            return_ws.error_msg = "";
                            return_ws.execute_state = true;
                        }
                        else
                        {
                            this.DBA.Rollback();
                            return_ws.ws_result = "結案失敗!";
                            return_ws.error_msg = "結案失敗，請洽資訊人員!";
                            return_ws.execute_state = false;
                            LogTool.SaveLogMessage(string.Format("使用者({0})病歷號{1}({2})錯誤訊息={3}", user_info.user_id, pat_info.chart_no, pat_info.ipd_no, DBAMsg.dbErrorMessage), actionName, GetLogToolCS.RT);
                        }
                    }
                    else
                    {
                        return_ws.ws_result = "失敗!";
                        return_ws.error_msg = "您尚未登入!";
                        return_ws.execute_state = false;
                    }
                }
            }
            catch (Exception ex)
            {
                LogTool.SaveLogMessage(ex.Message, actionName, GetLogToolCS.RT);
                return_ws.error_msg = "結案照護病患" + ExceptionMsg;
                return_ws.ws_result = "失敗!";
                return_ws.execute_state = false;
            }
            return return_ws;
        }
        #endregion  
    }

    /// <summary>
    /// RT_index ViewModel(view顯示)
    /// </summary>
    public class RT_index: RT_index_pathistory
    {

        #region view變數 傳入值

        /// <summary>
        /// 是否用網址導入網站
        /// </summary>
        public bool isGuide { get; set; }
        /// <summary>
        /// 錯誤訊息
        /// </summary>
        public RESPONSE_MSG rm { get; set; }
        /// <summary>
        /// 功能順序清單
        /// </summary>
        public List<SysParams> tabList { get; set; }

        #region 呼吸照護Source

        private List< BedArea> _bed_area { get; set; }
        /// <summary>
        /// 院內床號區域清單
        /// </summary>
        public List< BedArea> bed_area { get; set; }

        #endregion


        #region 上傳Source

        /// <summary>
        /// 上傳資料
        /// </summary>
        public upLoadIndex upLoadData { get; set; }

        #endregion

        #endregion

        public RT_index()
        {
            upLoadData = new upLoadIndex();
            rm = new RESPONSE_MSG();
            vs_doc_list = new List<SysParams>();
            tabList = new List<SysParams>();
        }

        /// <summary>
        /// 取得自訂表單順序
        /// </summary>
        /// <param name="tabSort"></param>
        /// <returns></returns>
        public int getTabSort(int tabSort)
        {
            if(tabList != null && tabList.Count >0)
            {
                try
                {
                    tabSort = int.Parse(tabList[tabSort].P_VALUE);
                }
                catch (Exception ex)
                {
                    //功能順序清單尚未維護
                    LogTool.SaveLogMessage("功能順序清單(" + tabSort.ToString() + ")尚未維護", "getTabSort", GetLogToolCS.RT);
                    LogTool.SaveLogMessage(ex, "getTabSort", GetLogToolCS.RT);
                }
                return tabSort;
            }
            return tabSort;
        }

    }

    public class RT_index_pathistory : ListHistoryViewModels
    {
        #region 照護病患記錄
 
        /// <summary>
        /// 醫生清單
        /// </summary>
        public List<SysParams> vs_doc_list { get; set; }
        #endregion
    }

    #region 上傳Class
    /// <summary>顯示上傳資料</summary>
    public class upLoadIndex
    {
        /// <summary>
        /// 錯誤訊息
        /// </summary>
        public RESPONSE_MSG rm { get; set; }
        public upLoadIndex()
        {
            rm = new RESPONSE_MSG();
            RT_RECORD = new upLoadData() { titleTable = "呼吸照護記錄單", recordId = "RT_RECORD", color = "#F00" };
            CPT_RECORD = new upLoadData() { titleTable = "呼吸治療胸腔復健記錄單", recordId = "CPT_RECORD", color = "#F00" };
            WEANING_ASSESS = new upLoadData() { titleTable = "呼吸脫離評估單", recordId = "WEANING_ASSESS", color = "#F00" };
        }
        /// <summary>
        /// 呼吸照護記錄單
        /// </summary>
        public upLoadData RT_RECORD { get; set; }
        /// <summary>
        /// 呼吸治療胸腔復健記錄單
        /// </summary>
        public upLoadData CPT_RECORD { get; set; }
        /// <summary>
        /// 呼吸脫離評估單
        /// </summary>
        public upLoadData WEANING_ASSESS { get; set; }
        /// <summary>
        /// 呼吸脫離評估單
        /// </summary>
        public upLoadData OnModel { get; set; }
        /// <summary>
        /// 是否有適應症的資料
        /// </summary>
        public bool hasOnModel { get; set; }
        /// <summary>
        /// 適應症筆數
        /// </summary>
        public int OnModelCnt { get {
            if (OnModel != null && OnModel.dataList.Count > 0)
                return OnModel.dataList.Count;
            else
                return 0;
        } }
        public List<upLoadData> cntList 
        {
            get
            {
                List<upLoadData> list = new List<upLoadData>();
                try
                {
                    if (RT_RECORD != null) list.Add(RT_RECORD); else list.Add(new upLoadData());
                    if (CPT_RECORD != null) list.Add(CPT_RECORD); else list.Add(new upLoadData());
                    if (WEANING_ASSESS != null) list.Add(WEANING_ASSESS); else list.Add(new upLoadData());
                }
                catch (Exception ex)
                {
                    LogTool.SaveLogMessage(ex, "getCntList", GetLogToolCS.RT);
                }
                return list;
            }
        }
        /// <summary>
        /// 上有24小時尚未上傳的筆數
        /// </summary>
        public int notUpLoadCountOver24
        {
            get
            {
                int cnt = 0;
                try
                {
                    if (cntList != null && cntList.Exists(x => x.hasNotUpLoadCountOver24))
                        cnt = cntList.FindAll(x => x.hasNotUpLoadCountOver24).Sum(x=>x.notUpLoadCountOver24); 
                }
                catch (Exception ex)
                {
                    LogTool.SaveLogMessage(ex, "getALLnotUpLoadCountOver24", GetLogToolCS.RT);
                }
                return cnt;
            }
        }
        /// <summary>
        /// 上傳清單(唯讀)
        /// </summary>
        public Dictionary<string, Dictionary<string, showUpLoadDetail>> upLoadList
        {
            get
            {
                Dictionary<string, Dictionary<string, showUpLoadDetail>> dic = new Dictionary<string, Dictionary<string, showUpLoadDetail>>();
                try
                {
                    List<upLoadData> list = new List<upLoadData>();
                    list.Add(RT_RECORD);
                    list.Add(CPT_RECORD);
                    list.Add(WEANING_ASSESS);
                    list.Add(OnModel);
                    foreach (upLoadData item in list)
                    {
                        if (item != null && item.dataList.Count > 0)
                        {
                            Dictionary<string, showUpLoadDetail> tempDic = new Dictionary<string, showUpLoadDetail>();
                            foreach (showUpLoadDetail value in item.dataList)
                            {
                                tempDic.Add(value.RECORD_ID, value);
                            }
                            dic.Add(item.recordId, tempDic);
                        }
                        else
                            dic.Add(item.recordId, new Dictionary<string, showUpLoadDetail>());
                    }

                }
                catch (Exception ex)
                {
                    LogTool.SaveLogMessage(ex, "upLoadList", GetLogToolCS.RT);
                    dic = new Dictionary<string, Dictionary<string, showUpLoadDetail>>();
                }
                return dic;
            }
        }
        /// <summary>
        /// 回傳資料按鈕類型(0="上傳",1="忽略",2="回復")
        /// </summary>
        public string btnType { get; set; }
        /// <summary>
        /// 按鈕顯示內容(0="上傳",1="忽略",2="回復")
        /// </summary>
        public string btnValue { get; set; }
    }
    /// <summary>上傳清單顯示資料</summary>
    public class upLoadData
    {
        public upLoadData()
        {
            dataList = new List<showUpLoadDetail>();
        }
        /// <summary>
        /// 資料表標題
        /// </summary>
        public string titleTable { get; set; }
        /// <summary>
        /// 記錄ID
        /// </summary>
        public string recordId { get; set; }
        /// <summary>
        /// 未上傳筆數
        /// </summary>
        public int notUpLoadCount { 
            get 
            {
                int cnt = 0;
                try
                {
                    if (dataList != null && dataList.Count > 0)
                    {
                        if (dataList.Exists(x => (x.UPLOAD_STATUS == "0" && x.datastatus == "1") || (x.UPLOAD_STATUS == "1" && x.datastatus == "9")))
                            cnt = dataList.FindAll(x => (x.UPLOAD_STATUS == "0" && x.datastatus == "1") || (x.UPLOAD_STATUS == "1" && x.datastatus == "9")).Count;
                    }
                }
                catch (Exception ex)
                {
                    LogTool.SaveLogMessage(ex, "getNotUpLoadCount", GetLogToolCS.RT);
                }
                return cnt;
            } 
        }
        /// <summary>
        /// 有未上傳筆數(超過24小時)
        /// </summary>
        public bool hasNotUpLoadCountOver24
        {
            get
            {
                bool hasCnt = false;
                try
                {
                     if (dataList != null && dataList.Count > 0)
                {
                    if (dataList.Exists(x => x.UPLOAD_STATUS == "0" && DateTime.Parse(x.RECORDDATE) < DateTime.Now.AddHours(-24)))
                        hasCnt = true;
                }
                }
                catch (Exception ex)
                {
                    LogTool.SaveLogMessage(ex, "getHasNotUpLoadCountOver24", GetLogToolCS.RT);
                }
                return hasCnt;
            }
        }
        /// <summary>
        /// 未上傳筆數(超過24小時)
        /// </summary>
        public int notUpLoadCountOver24
        {
            get
            {
                int cnt = 0;
                try
                {
                     if (dataList != null && dataList.Count > 0)
                    {
                        if (dataList.Exists(x => x.UPLOAD_STATUS == "0" && DateTime.Parse(x.RECORDDATE)< DateTime.Now.AddHours(-24)))
                            cnt = dataList.FindAll(x => x.UPLOAD_STATUS == "0" && DateTime.Parse(x.RECORDDATE) < DateTime.Now.AddHours(-24)).Count;
                    }
                }
                catch (Exception ex)
                {
                    LogTool.SaveLogMessage(ex, "getNotUpLoadCountOver24", GetLogToolCS.RT);
                }
                return cnt;
            }
        }
        /// <summary>
        /// 設定顏色(分辨用)
        /// </summary>
        public string color { get; set; }
        /// <summary>
        /// 清單筆數
        /// </summary>
        public int count { get { return dataList.Count; } }
        /// <summary>
        /// 顯示清單資料
        /// </summary>
        public List<showUpLoadDetail> dataList { get; set; }
    }
    /// <summary>上傳資料Deail </summary>
    public class showUpLoadDetail : xmlBaseData
    {
        /// <summary>
        /// 產生指令瑪路徑
        /// </summary>
        public string RECORD_ORDERPATH { get; set; }
        /// <summary>
        /// 產生檔案路徑
        /// </summary>
        public string RECORD_FILEPATH { get; set; }
        /// <summary>
        /// 產生檔名
        /// </summary>
        public List<string> RECORD_FILENAME { get; set; }
        /// <summary>
        /// 判別資料表(RT_RECORD="呼吸照護記錄單",CPT_RECORD="呼吸治療胸腔復健記錄單",WEANING_ASSESS="呼吸脫離評估單")
        /// </summary>
        public string RECORD { get; set; }
        /// <summary>
        /// 記錄單流水號(唯一值)
        /// </summary>
        public string RECORD_ID { get; set; }
        /// <summary>
        /// 顯示記錄單記錄時間
        /// </summary>
        public string RECORDDATE { get; set; }
        /// <summary>
        /// 新增人員ID
        /// </summary>
        public string CREATE_ID { get; set; }
        /// <summary>
        /// 新增人員
        /// </summary>
        public string CREATE_NAME { get; set; }
        /// <summary>
        /// 新增時間
        /// </summary>
        public string CREATE_DATE { get; set; }
        /// <summary>
        /// 修改時間
        /// </summary>
        public string MODIFY_DATE { get; set; }
        /// <summary>
        /// 顯示時間
        /// </summary>
        public string SHOW_DATE
        {
            get
            {
                if (MODIFY_DATE != null && MODIFY_DATE != "")
                    return MODIFY_DATE;
                else
                    return CREATE_DATE;

            }
        }
        /// <summary>
        /// 顯示資料狀態
        /// </summary>
        public string SHOW_DATASTATUS
        {
            get
            {
                string tempStr = "";
                if (UPLOAD_STATUS != "2")
                {
                    if (datastatus == "1") tempStr = "新增";
                    if (datastatus == "9") tempStr = "刪除";
                }
                else
                    tempStr = "忽略";
                return tempStr;
            }
        }
        /// <summary> 診別中文 </summary>
        public string show_sourceType
        {
            get
            {
                switch (this.sourceType)
                {
                    case "E":
                        return "急診";
                        break;
                    case "O":
                        return "門診";
                        break;
                    case "I":
                    default:
                        return "住院";
                        break;
                }
                return this.sourceType;
            }
        }
        private string _birthDay { get; set; }
        /// <summary>
        /// 生日資料
        /// </summary>
        public string birthDay
        {
            get { return _birthDay; }
            set
            {
                _birthDay = value;
                try
                {
                    TimeSpan ts = DateTime.Now.Date - DateTime.ParseExact(this._birthDay, "yyyy-MM-dd", new CultureInfo("zh-TW")).Date;
                    double age_v = ts.Days / 365;
                    this.age = Convert.ToInt32(Math.Round(age_v, 1)).ToString();
                }
                catch
                {
                    throw;
                }
            }
        }
        /// <summary>
        /// 顯示性別
        /// </summary>
        public string showSex
        {
            get
            {
                switch (this.sex)
                {
                    case "男":
                    case "M":
                    case "1":
                        return "男";
                    case "女":
                    case "0":
                    case "F":
                        return "女";
                    default:
                        return "未知";
                }
            }
        }
        /// <summary>
        /// 上傳狀態，0=未上傳、1=已上傳成功
        /// </summary>
        public string UPLOAD_STATUS { get; set; }
        /// <summary>
        /// 診斷
        /// </summary>
        public string DIAG_DESC { get; set; }
        /// <summary>
        /// 主治醫生代碼
        /// </summary>
        public string vs_doc_id { get; set; }
        /// <summary>
        /// 主治醫生
        /// </summary>
        public string vs_doc_name { get; set; }
        /// <summary>
        /// 科別代碼
        /// </summary>
        public string DEPT_CODE { get; set; }
        /// <summary>
        /// 科別名稱
        /// </summary>
        public string DEPT_NAME { get; set; }
        /// <summary>
        /// 病患身分證
        /// </summary>
        public string PatientID { get; set; }

        /// <summary>
        /// 附加變數(適應症使用)
        /// </summary>
        public string additional { get; set; }
    }
    #endregion

    /// <summary>
    /// 列印照護清單
    /// </summary>
    public class RTListjsonData
    {
        /// <summary>
        /// 回傳Json
        /// </summary>
        [AllowHtml]
        public string jsonData { get; set; }
        /// <summary>
        /// 列印資料
        /// </summary>
        public List<PatientListItem> pli { get { 
            List<PatientListItem> temp = new List<PatientListItem>();
            if (!string.IsNullOrEmpty(jsonData)){
                try 
	            {
                    temp = JsonConvert.DeserializeObject<List<PatientListItem>>(jsonData);
	            }
	            catch (Exception ex)
	            {
                    LogTool.SaveLogMessage(ex, "pliDeserializeObject",GetLogToolCS.RT);
                    temp = new List<PatientListItem>();
	            }
                return temp;
            }
            else
                return temp;
        
        } }
    }
    /// <summary>
    /// 交班作業viewModel
    /// </summary>
    public class checkListViewer
    {
        public checkListViewer()
        {
            UserList = new List<SelectListItem>();
        }
        /// <summary>
        /// 交班者List
        /// </summary>
        public List<SelectListItem> UserList { get; set; }
        /// <summary>
        /// 交班人
        /// </summary>
        public string UserName { get; set; }
    }

    public class VIPRowData
    {
        /// <summary>
        /// RowData
        /// </summary>
        public string rowData { get; set; }
        /// <summary>
        /// 設備
        /// </summary>
        public string device { get; set; }
        /// <summary>
        /// 模式
        /// </summary>
        public string model { get; set; }
        /// <summary>
        /// 設定欄位資料
        /// </summary>
        public Dictionary<string, object> setDic {get;set;}
    }
}
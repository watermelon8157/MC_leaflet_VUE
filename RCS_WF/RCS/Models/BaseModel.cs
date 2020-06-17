//**************************************************
//2016/08/12
//#2172 建立DB Model
//功能 DB Model 常用基本設定(繼承既可使用)
//2016/08/12 建立:建立 getRCS_SYS_PARAMS 取的下拉選單
//2016/08/12 建立:取得GetTableName
//2016/08/12 建立:建立 GetFixedStrSerialNumber 取得流水號
//2016/08/15 修改:CustomSelectItem的資料結構
//2016/08/16 修改:使用者資料及病患資料取得方式
//**************************************************
using Com.Mayaminer;
using mayaminer.com.library;
using RCS_Data;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Newtonsoft.Json;
using System.Reflection;
using System.Xml.Serialization;
using log4net;
using RCSData.Models;

namespace RCS.Models
{

    public class BaseModel: DBA_Provider, ILogger
    {
        string csName { get { return "BaseModel"; } }

        private RCS.Models.HospSetDDL _select { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public RCS.Models.HospSetDDL select
        {
            get
            {
                if (this._select == null)
                {
                    this._select = new RCS.Models.HospSetDDL();
                }
                return this._select;
            }
        }

        private RCS_Data.Models.BasicFunction _bf { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public RCS_Data.Models.BasicFunction basicfunction
        {
            get
            {
                if (this._bf == null)
                {
                    this._bf = new RCS_Data.Models.BasicFunction();
                }
                return this._bf;
            }
        }

        public override ILog logger
        {
            get
            {
                _logger = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
                return _logger;
            }
            set
            {
                _logger = value;
            }
        }
        /// <summary>
        /// 是否不檢查超過24小時可否修改
        /// </summary>
        public bool isNotcheck24HrNotFix = true;
        #region 變數

        /// <summary> 存放Session使用者基本資料 </summary>
        protected UserInfo user_info { get; set; }
        /// <summary> 病人基本資料 </summary>
        protected IPDPatientInfo pat_info { get; set; }
        /// <summary>內容:程式錯誤，請洽資訊人員!</summary>
        public string ExceptionMsg { get { return "程式錯誤，請洽資訊人員!"; } }

        #endregion

        public BaseModel()
        {
            //取得使用者資料
            if (HttpContext.Current.Session["user_info"] != null)
                user_info = (UserInfo)HttpContext.Current.Session["user_info"];
            //取得病患資料
            if (HttpContext.Current.Session["pat_info"] != null)
                pat_info = (IPDPatientInfo)HttpContext.Current.Session["pat_info"];
        }



        public SysParamCollection GetModelListCollection(string model)
        {
            List<SysParams> list = this.getRCS_SYS_PARAMS(model, "", @pStatus: "1");
            SysParamCollection sys_params_list = new SysParamCollection();
            list.ForEach(x => {
                sys_params_list.Add(x);
            });
            return sys_params_list;
        }

        /// <summary>
        /// 取得系統設定中的下拉項目清單
        /// </summary>
        /// <param name="RTRecordAllList">List SysParams</param>
        /// <param name="group">群組分類</param>
        /// <returns>List SysParams</returns>
        public List<SysParams> GetGroupList(List<SysParams> RTRecordAllList, string group_v)
        {
            IEnumerable<SysParams> get_select_data = from r in RTRecordAllList
                                                     where r.P_GROUP == group_v
                                                     select r;
            return get_select_data.ToList();
        }


        /// <summary>取得系統設定參數</summary>
        /// <param name="pModel">參數類別</param>
        /// <param name="pGroup">參數群組</param>
        /// <param name="pLang">語系(預設:zh-tw)</param>
        /// <param name="pStatus">資料狀態：1=正常，9=刪除/停用</param>
        /// <param name="pManage">資料狀態：1=管理者才顯示，0=非管理者顯示</param>
        /// <returns></returns>
        public List<SysParams> getRCS_SYS_PARAMS(string pModel = "", string pGroup = "", string pLang = "zh-tw", string pStatus = "", string pManage = "")
        {
            List<SysParams> SelectListItem = new List<SysParams>();
            try
            {
                DataTable dt = this.getRCS_SYS_PARAMS_DT("", pModel, pGroup, pLang, "", pStatus, pManage);
                if (dt != null && dt.Rows.Count > 0) SelectListItem = dt.ToList<SysParams>();
            }
            catch (Exception ex)
            {
                LogTool.SaveLogMessage(ex, "getRCS_SYS_PARAMS1", GetLogToolCS.BaseModel);
            }
            return SelectListItem;
        }

        /// <summary>取得系統設定參數</summary>
        /// <param name="pModel">參數類別</param>
        /// <param name="pGroup">參數群組</param>
        /// <param name="pLang">語系(預設:zh-tw)</param>
        /// <param name="pStatus">資料狀態：1=正常，9=刪除/停用</param>
        /// <param name="pManage">資料狀態：1=管理者才顯示，0=非管理者顯示</param>
        /// <returns></returns>
        public List<SysParams> getRCS_SYS_PARAMS(List<string> pModelList, string pGroup = "", string pLang = "zh-tw", string pStatus = "", string pManage = "")
        {
            List<SysParams> SelectListItem = new List<SysParams>();
            try
            {
                DataTable dt = this.getRCS_SYS_PARAMS_DT("", "", pGroup, pLang, "", pStatus, pManage, pModelList);
                if (dt != null && dt.Rows.Count > 0) SelectListItem = dt.ToList<SysParams>();
            }
            catch (Exception ex)
            {
                LogTool.SaveLogMessage(ex, "getRCS_SYS_PARAMS1", GetLogToolCS.BaseModel);
            }
            return SelectListItem;
        }

        public DataTable getRCS_SYS_PARAMS_DT(string Id = "", string pModel = "", string pGroup = "", string pLang = "zh-tw", string pValue = "", string pStatus = "", string pManage = "",List<string> pModelList = null)
        {
            List<string> wSql = new List<string>();
            string sql = "SELECT * FROM {0} WHERE {1} ORDER BY P_SORT";
            List<SysParams> SelectListItem = new List<SysParams>();
            try
            {
                if (!string.IsNullOrEmpty(Id))
                    wSql.Add(string.Format(" P_ID = {0}", SQLDefend.SQLString(Id)));
                if (!string.IsNullOrEmpty(pModel))
                {
                    wSql.Add(string.Format(" P_MODEL = {0}", SQLDefend.SQLString(pModel)));

                }
                if (pModelList != null && pModelList.Count>0)
                {
                    wSql.Add(string.Format(" P_MODEL in ('{0}')", string.Join("','", pModelList)));
                }
                if (!string.IsNullOrEmpty(pGroup))
                    wSql.Add(string.Format(" P_GROUP = {0}", SQLDefend.SQLString(pGroup)));
                if (!string.IsNullOrEmpty(pLang))
                    wSql.Add(string.Format(" P_LANG = {0}", SQLDefend.SQLString(pLang)));
                if (!string.IsNullOrEmpty(pValue))
                    wSql.Add(string.Format(" P_VALUE = {0}", SQLDefend.SQLString(pValue)));
                if (!string.IsNullOrEmpty(pStatus))
                    wSql.Add(string.Format(" P_STATUS = {0}", SQLDefend.SQLString(pStatus)));
                if (!string.IsNullOrEmpty(pManage))
                    wSql.Add(string.Format(" P_MANAGE = {0}", SQLDefend.SQLString(pManage)));
                sql = string.Format(sql, GetTableName.RCS_SYS_PARAMS.ToString(), string.Join(" AND ", wSql));
                DataTable dt = this.DBA.getSqlDataTable(sql);
                return dt;
            }
            catch (Exception ex)
            {
                LogTool.SaveLogMessage(ex, MethodInfo.GetCurrentMethod().Name, GetLogToolCS.BaseModel);
            }
            return null;
        }

        ///<summary>收案清單比對 </summary>
        /// <param name="pIpd_nos"></param>
        /// <returns></returns>
        public List<string> CheckInCase(List<string> pIpd_nos)
        {
            List<string> InCaseList = new List<string>();
            string sqlstatment = string.Format("select ipd_no from " + GetTableName.RCS_RT_CASE + " where ipd_no in ('{0}') and accept_status = '1'", string.Join("','", pIpd_nos));
            DataTable dt = this.DBA.getSqlDataTable(sqlstatment);
            if (dt != null && dt.Rows.Count > 0)
            {
                foreach (DataRow dr in dt.Rows)
                {
                    if (!DBNull.ReferenceEquals(dr["ipd_no"], DBNull.Value)) 
                        InCaseList.Add(dr["ipd_no"].ToString());
                }
            }
            return InCaseList;
        }

      
        /// <summary>字串小數點無條件捨去</summary>
        /// <param name="input">輸入字串</param>
        /// <param name="digit">小數點位數</param>
        /// <returns></returns>
        public string GetStringFrag(string input, int digit)
        {
            double double_number;
            if (double.TryParse(input, out double_number))
            {
                if (digit < 0)
                {
                    return Math.Floor(double_number).ToString();
                }
                double pow = Math.Pow(10, digit);
                double sign = double_number >= 0 ? 1 : -1;
                return (sign * Math.Floor(sign * double_number * pow) / pow).ToString();
            }
            else
            {
                return input;
            }
        }

        /// <summary>檢查DataColumn是否是空值(預設 DBNull.Value 回傳 = "")</summary>
        /// <param name="pDr">資料列</param>
        /// <param name="columnName">DataColumn名稱</param>
        ///  /// <param name="returnStr">預設(DBNull.Value 回傳 = "")</param>
        /// <returns>string</returns>
        public string checkDataColumn(DataRow pDr, string columnName, string returnStr = "")
        {
            if (!DBNull.ReferenceEquals(pDr[columnName], DBNull.Value))
            {
                string columnStr = pDr[columnName].ToString().Trim();
                if (columnStr != "")
                    returnStr = columnStr;
            } 
            return returnStr;
        }
        /// <summary>
        /// 將Datatable轉成List<Dictionary<string, string>>
        /// Dictionary key=DataRow.columnName, value=DataRow[columnName]的值(string)
        /// </summary>
        /// <param name="pDT"></param>
        /// <returns></returns>
        public List<Dictionary<string, string>> fillDtToDictionaryList(DataTable pDT)
        {
            List<Dictionary<string, string>> list = new List<Dictionary<string, string>>();
            Dictionary<string, string> Temp = null;
            if (DTNotNullAndEmpty(pDT))
            {
                foreach (DataRow dr in pDT.Rows)
                {
                    Temp = new Dictionary<string, string>();
                    foreach (DataColumn col in pDT.Columns)
                    {
                        Temp.Add(col.ColumnName, checkDataColumn(dr, col.ColumnName));
                    }
                    list.Add(Temp);
                }
            }
            return list;
        }
        /// <summary>
        /// 將List<T>轉成List<Dictionary<string, string>>
        /// Dictionary key=T.Property.Name, value=T.Property.Name.Value
        /// </summary>
        /// <param name="pDT"></param>
        /// <returns></returns>
        public static List<Dictionary<string, string>> fillListToDictionaryList<T>(List<T> pList)
        {
            List<Dictionary<string, string>> list = new List<Dictionary<string, string>>();
            Dictionary<string, string> Temp = null;
            if (pList != null && pList.Count > 0)
            {
                pList.ForEach(x =>
                {
                    Temp = new Dictionary<string, string>();
                    foreach (PropertyInfo pi in x.GetType().GetProperties())
                    {
                        object value = pi.GetValue(x, null);
                        Temp.Add(pi.Name, value != null ? value.ToString() : null);
                    }
                    list.Add(Temp);
                });
            }
            return list;
        }
        /// <summary>帶入新增修改人員</summary>
        /// <param name="pDr">資料Row</param>
        public void updateDataRowUser(ref DataRow pDr)
        {
            if (pDr.RowState == DataRowState.Detached || pDr.RowState == DataRowState.Added)
            {
                pDr["CREATE_ID"] = user_info.user_id;
                pDr["CREATE_NAME"] = user_info.user_name;
                pDr["CREATE_DATE"] = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            }
            else if (pDr.RowState == DataRowState.Modified)
            {
                pDr["MODIFY_ID"] = user_info.user_id;
                pDr["MODIFY_NAME"] = user_info.user_name;
                pDr["MODIFY_DATE"] = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            }
        } 

      
        /// <summary>取得使用天數</summary>
        /// <param name="pIpdNo">批價序號</param>
        /// <returns></returns>
        public int getUseDays(string pIpdNo,string pChart_no, out List<string> on_dateList)
        {
            on_dateList = new List<string>();
            int days = 0;
            return days;  
        }


        /// <summary>取得住院病床清單 </summary>
        /// <param name="cost_code">分區代碼</param>
        /// <param name="case_code">是否已收案</param>
        /// <returns></returns>
        public List<IPDPatientInfo> GetBedListData(IWebServiceParam iwp, string cost_code, string case_code = "")
        {
            List<string> pre_in_case_check = new List<string>();
            List<IPDPatientInfo> PatientList = new List<IPDPatientInfo>();

            if (string.IsNullOrWhiteSpace(cost_code))
            {
                List<BedArea> BedAreaList = web_method.getHisBedAreaList(new RCS.Models.HOSP.HospFactory().webService.HisBedAreaList());

                List<GetBed> BedList = new List<GetBed>();
                foreach (BedArea item in BedAreaList)
                {
                    List<IPDPatientInfo> tempPatientList = web_method.getPatientInfoList(iwp, "", "", item.area_code);
                    if (tempPatientList != null)
                    {
                        foreach (IPDPatientInfo temp in tempPatientList)
                        {
                            PatientList.Add(temp);
                        }
                    }
                }
            }
            else
            {
                List<IPDPatientInfo> tempPatientList = web_method.getPatientInfoList(iwp, "", "", cost_code);
                if (tempPatientList != null)
                {
                    foreach (IPDPatientInfo temp in tempPatientList)
                    {
                        PatientList.Add(temp);
                    }
                }
            }

            //不取空資料
            PatientList = (from pti in PatientList
                           where !string.IsNullOrEmpty(pti.patient_name) && !string.IsNullOrEmpty(pti.chart_no)
                           select pti).ToList();

            foreach (IPDPatientInfo pti in PatientList)
            {
                if (!string.IsNullOrEmpty(pti.ipd_no))
                    pre_in_case_check.Add(pti.ipd_no);
            }

            //篩選有無收案後的清單
            List<IPDPatientInfo> PatientList_F = new List<IPDPatientInfo>();

            //加入收案資訊
            if (!string.IsNullOrEmpty(case_code))
            {
                List<string> in_cases = this.CheckInCase(pre_in_case_check);
                foreach (IPDPatientInfo pti in PatientList)
                {
                    if (in_cases.Contains(pti.ipd_no))
                    {
                        pti.accept_status = "1";
                    }
                }
                foreach (var pti_list in PatientList)
                {
                    if (pti_list.accept_status == "1" && case_code == "1")
                    {
                        //收案
                        PatientList_F.Add(pti_list);
                    }
                    else if (pti_list.accept_status != "1" && case_code == "0")
                    {
                        //未收案
                        PatientList_F.Add(pti_list);
                    }
                }
            }
            else
            {
                //全部(未經篩選)
                PatientList_F = PatientList;
            }
            return PatientList_F;
        } 
        /// <summary>
        /// 判斷表單是否為現在時間往前推24小時內(或超過現在時間)的資料
        /// </summary>
        /// <param name="pRecordDate">記錄日期</param>
        /// <remarks>2016/11/17 Vanda Add</remarks>
        /// <returns></returns>
        public bool checkFormRecordWithin24hr(ref string pErrorMsg, string pRecordDate)
        {
            //-----2017/02/09 Vanda Add
            isNotcheck24HrNotFix = RCS.Controllers.BaseController.funSetting.isClose24hours;
            //-----
            if (isNotcheck24HrNotFix) return true;
            try
            {
                DateTime RecordDate = Convert.ToDateTime(pRecordDate);
                DateTime Before24Time = DateTime.Now.AddHours(-168);
                if (Before24Time <= RecordDate)
                    return true;
                else
                    pErrorMsg = "該筆記錄時間未在168時以內，無法增修表單，請檢查!";
            }
            catch (Exception ex)
            {
                pErrorMsg = "儲存失敗!";
                LogTool.SaveLogMessage("判斷時間內可否增修表單失敗，錯誤內容如下所示:" + ex.Message, "saveLog");
            }
            return false;
        }

        
        /// <summary>
        /// 轉換特殊字元
        /// </summary>
        public string trans_special_code(string content)
        {
            if (!string.IsNullOrEmpty(content))
            {
                content = content.Trim();
                return content;
                content = content.Replace("&", "&amp;");
                content = content.Replace("<", "&lt;");
                content = content.Replace(">", "&gt;");
                content = content.Replace("'", "&apos;");
                content = content.Replace("\"", "&quot;");

                content = content.Replace("\u0001", "");
                content = content.Replace("\u000B", "");
                content = content.Replace("\r\n", "&#xD;&#xA;");
                content = content.Replace("\n", "&#xA;");                
            }
            return content;
        }
        /// <summary>
        /// 更新收案資料
        /// </summary>
        /// <param name="pPatInfo"></param>
        /// <returns></returns>
        public IPDPatientInfo updateCaseData(IWebServiceParam iwp, IPDPatientInfo pPatInfo)
        {
            string actionName = "updateCaseData";
            RESPONSE_MSG rm = new RESPONSE_MSG();
            try
            {
                if(!string.IsNullOrWhiteSpace(pPatInfo.source_type) && pPatInfo.source_type != "I")
                {
                    return pPatInfo;
                }
                List<IPDPatientInfo> tempPat_info = web_method.getPatientInfoList(iwp, pPatInfo.chart_no, pPatInfo.ipd_no);
                IPDPatientInfo patInfo = new IPDPatientInfo();
                if (tempPat_info != null && tempPat_info.Count > 0) patInfo = tempPat_info[0];
                if ((!string.IsNullOrWhiteSpace(patInfo.bed_no) && pPatInfo.bed_no != patInfo.bed_no)
                    || (!string.IsNullOrWhiteSpace(patInfo.vs_id) && pPatInfo.vs_id != patInfo.vs_id)
                    || (!string.IsNullOrWhiteSpace(patInfo.vs_doc) && pPatInfo.vs_doc != patInfo.vs_doc)
                     || (!string.IsNullOrWhiteSpace(patInfo.cost_code) && pPatInfo.cost_code != patInfo.cost_code)
                     || (!string.IsNullOrWhiteSpace(patInfo.dept_code) && pPatInfo.dept_code != patInfo.dept_code)
                     || (!string.IsNullOrWhiteSpace(patInfo.MDRO_MARK) && pPatInfo.dept_desc != patInfo.MDRO_MARK)
                     || (!string.IsNullOrWhiteSpace(patInfo.dept_desc) && pPatInfo.dept_desc != patInfo.dept_desc)
                     || (!string.IsNullOrWhiteSpace(patInfo.idno) && pPatInfo.idno != patInfo.idno))
                {
                    string sql = "SELECT * FROM " + GetTableName.RCS_RT_CASE + " WHERE CASE_ID =" + SQLDefend.SQLString(pPatInfo.case_id);
                    DataTable dtCASE = this.DBA.getSqlDataTable(sql);
                    sql = "SELECT * FROM " + GetTableName.RCS_RT_CARE_SCHEDULING + " WHERE CARE_ID =" + SQLDefend.SQLString(pPatInfo.r_id);
                    DataTable dtSCHEDULING = this.DBA.getSqlDataTable(sql);
                    if (DTNotNullAndEmpty(dtCASE) && DTNotNullAndEmpty(dtSCHEDULING))
                    {
                        dtCASE.TableName = GetTableName.RCS_RT_CASE.ToString();
                        dtSCHEDULING.TableName = GetTableName.RCS_RT_CARE_SCHEDULING.ToString();
                        DataSet ds = new DataSet();
                        dtCASE.Rows[0]["BED_NO"] = patInfo.bed_no;//更新床號
                        dtCASE.Rows[0]["VS_DOC_ID"] = patInfo.vs_id;//更新主治醫生ID
                        dtCASE.Rows[0]["VS_DOC_NAME"] = patInfo.vs_doc;//更新主治醫生名字
                        dtSCHEDULING.Rows[0]["BED_NO"] = patInfo.bed_no;//更新床號
                        dtSCHEDULING.Rows[0]["cost_code"] = patInfo.cost_code;//護理站代碼
                        dtSCHEDULING.Rows[0]["VS_DOC_NAME"] = patInfo.vs_doc;//更新主治醫生名字
                        dtSCHEDULING.Rows[0]["VS_DOC_ID"] = patInfo.vs_id;//更新主治醫生ID
                        dtCASE.Rows[0]["cost_code"] = patInfo.cost_code;//護理站代碼
                        dtCASE.Rows[0]["MDRO_MARK"] = patInfo.MDRO_MARK;//MDRO_MARK
                        dtCASE.Rows[0]["DEPT_CODE"] = patInfo.dept_code;//科別代碼
                        dtCASE.Rows[0]["DEPT_NAME"] = patInfo.dept_desc;//科別名稱
                        dtCASE.Rows[0]["PATIENT_IDNO"] = patInfo.idno;// 
                        ds.Tables.Add(dtCASE);
                        ds.Tables.Add(dtSCHEDULING);
                        this.DBA.BeginTrans();
                        foreach (DataTable dt in ds.Tables)
                        {
                            this.DBA.Update(dt);
                            if (!string.IsNullOrWhiteSpace(this.DBA.LastError))
                            {
                                rm.message = "更新收案資料失敗!錯誤訊息如下所示:" + this.DBA.LastError;
                                rm.status = RESPONSE_STATUS.ERROR;
                                LogTool.SaveLogMessage(this.DBA.LastError, actionName, GetLogToolCS.BaseModel);
                                break;
                            }
                            
                        }
                        if (!string.IsNullOrWhiteSpace(this.DBA.LastError))
                        {
                            this.DBA.Rollback();
                        }
                        else
                        {
                            this.DBA.Commit();
                            pPatInfo.bed_no = patInfo.bed_no;
                            pPatInfo.vs_doc = patInfo.vs_doc;
                            pPatInfo.vs_id = patInfo.vs_id;
                            pPatInfo.case_vs_name = patInfo.case_vs_name;
                            pPatInfo.diagnosis_code = patInfo.diagnosis_code;
                            pPatInfo.dept_code = patInfo.dept_code;
                            pPatInfo.dept_desc = patInfo.dept_desc;
                            pPatInfo.idno = patInfo.idno;
                            pPatInfo.cost_code = patInfo.cost_code;
                            pPatInfo.cost_desc = patInfo.cost_desc;
                        }
                    }
                    if (!string.IsNullOrWhiteSpace(this.DBA.LastError))
                    {
                        rm.message = "更新收案資料失敗!錯誤訊息如下所示:" + this.DBA.LastError;
                        rm.status = RESPONSE_STATUS.ERROR;
                        LogTool.SaveLogMessage(this.DBA.LastError,actionName,GetLogToolCS.BaseModel);
                    }

                }
            }
            catch (Exception ex)
            {
                rm.message = "更新收案資料失敗!錯誤訊息如下所示:" + ex.Message;
                rm.status = RESPONSE_STATUS.EXCEPTION;
                LogTool.SaveLogMessage(ex, actionName, GetLogToolCS.BaseModel);
            }
            return pPatInfo;
        }

        /// <summary>
        /// 序列化MD5的資料，XML檔案名稱如果有問題的話(NIS的方法)
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public string GetMd5Hash(string input)
        {
            System.Security.Cryptography.MD5 md5 = System.Security.Cryptography.MD5.Create();
            byte[] data = md5.ComputeHash(System.Text.Encoding.UTF8.GetBytes(input));

            System.Text.StringBuilder sBuilder = new System.Text.StringBuilder();
            for (int i = 0; i < data.Length; i++)
                sBuilder.Append(data[i].ToString("x2"));

            return sBuilder.ToString();
        }

        /// <summary>
        /// 取得歷史記錄清單
        /// </summary>
        /// <param name="pChart_no">病歷號</param>
        /// <param name="pIpd_no">批價序號</param>
        /// <returns></returns>
        public List<SelectListItem> getPatientHistoryList(string pChart_no,string pIpd_no)
        {
            List<SelectListItem> ipdnoList = new List<SelectListItem>();
            string actionName = "getPatientHistoryList";
            try
            {
                string sql = "SELECT DISTINCT IPD_NO,DIAG_DATE,PATIENT_SOURCE FROM " + GetTableName.RCS_RT_CASE + " WHERE CHART_NO =" + SQLDefend.SQLString(pChart_no) + " ORDER BY DIAG_DATE DESC";
                DataTable dt = this.DBA.getSqlDataTable(sql);
                if (DTNotNullAndEmpty(dt))
                {
                    string tempDate = DateTime.Now.ToString("yyyy-MM-dd 23:59");
                    foreach (DataRow dr in dt.Rows)
                    {
                        string IPD_NO = checkDataColumn(dr, "IPD_NO");
                        string DIAG_DATE = checkDataColumn(dr, "DIAG_DATE");
                        string PATIENT_SOURCE = checkDataColumn(dr, "PATIENT_SOURCE");
                        IPDPatientInfo tempInfo = new IPDPatientInfo();
                        tempInfo.source_type = PATIENT_SOURCE;
                        DIAG_DATE = DateTime.Parse(DIAG_DATE).ToString("yyyy-MM-dd HH:mm");
                        bool isChecked = IPD_NO == pIpd_no;
                        SelectListItem item = new SelectListItem() { Text = string.Format("{1}", IPD_NO, DIAG_DATE), Value = string.Format("{0}|{1}|{2}|{3}|{4}", IPD_NO, DIAG_DATE, tempDate, PATIENT_SOURCE, tempInfo.showSource), Selected = isChecked };
                        ipdnoList.Add(item);
                    }
#if DEBUG
                    ipdnoList.Add(new SelectListItem() { Text = string.Format("{1}", "123", DateTime.Now.ToString("yyyy-MM-dd 23:59")), Value = string.Format("{0}|{1}|{2}|I|住", "123", DateTime.Now.ToString("yyyy-MM-dd 23:59"), DateTime.Now.ToString("yyyy-MM-dd 23:59")) });
#endif
                }
                else
                {
                    if (!string.IsNullOrWhiteSpace(this.DBA.LastError))
                    {
                        LogTool.SaveLogMessage("取得住院歷程失敗，錯誤訊息如下所示:" + this.DBA.LastError, actionName, GetLogToolCS.BaseModel);
                    }

                }
            }
            catch (Exception ex)
            {
                LogTool.SaveLogMessage("取得住院歷程失敗，錯誤訊息如下所示:" + ex,actionName,GetLogToolCS.BaseModel);
                throw;
            }
            return ipdnoList;
        }
        /// <summary>
        /// 取得資訊
        /// </summary>
        /// <param name="pSetIpdNo"></param>
        /// <param name="getLocation">0:批價序號，1:起始日期，2:結束日期，3:來源</param>
        /// <returns></returns>
        public string getHistoryList(string pSetIpdNo,int getLocation)
        {
            string tempVal = "";
            try
            {
                if (!string.IsNullOrWhiteSpace(pSetIpdNo))
                {
                    if(getLocation == 1)
                    {
                        string data1 = pSetIpdNo.Split('|').ToArray<string>()[1];
                        string data2 = pSetIpdNo.Split('|').ToArray<string>()[2];
                        if(DateHelper.isDate(data1) && DateHelper.isDate(data2))
                        {
                            if(DateHelper.Parse(data1) < DateHelper.Parse(data2))
                            {
                                tempVal = DateHelper.Parse(data2).AddDays(-7).ToString("yyyy-MM-dd 00:00");
                            }
                            else
                            {
                                tempVal = data1;
                            }
                        }
                        else
                        {
                            tempVal = data1;
                        }

                    }
                    else
                    {
                        tempVal = pSetIpdNo.Split('|').ToArray<string>()[getLocation];
                    }
                }
            }
            catch (Exception ex)
            {
                if (getLocation > 0)
                {
                    tempVal = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                }
                LogTool.SaveLogMessage(pSetIpdNo+"("+ getLocation + ")"+"轉換住院序號失敗，錯誤訊息如下所示:" + ex, "getSetIpdNo", GetLogToolCS.BaseModel);
            }
            return tempVal;
        }

        /// <summary>
        /// 檢查紀錄日期
        /// <para>門診病人紀錄單時，只可填寫當天紀錄日期</para>
        /// <para>急診病人紀錄單時，只可填寫急診區間的紀錄日期</para>
        /// </summary>
        /// <param name="setIpdno"></param>
        /// <param name="pRecordDate"></param>
        /// <param name="pPatInfo"></param>
        /// <returns></returns>
        public RESPONSE_MSG saveRecordDateCheck(string setIpdno, string pRecordDate,IPDPatientInfo pPatInfo)
        {
            List<string> msgList = new List<string>();
            RESPONSE_MSG rm = new RESPONSE_MSG();
            rm.status = RESPONSE_STATUS.SUCCESS;
            string actionName = "saveRecordDateCheck";
            try
            {
                //記錄單日期不可以大於今天
                if (DateHelper.isDate(pRecordDate) && DateHelper.Parse(pRecordDate) > DateTime.Parse(DateTime.Now.ToString("yyyy-MM-dd 23:59:59")))
                {
                    msgList.Add("記錄單日期(" + pRecordDate + ")不可大於今天(" + DateTime.Now.ToString("yyyy-MM-dd 23:59:59") + ")");
                }
                if (!string.IsNullOrWhiteSpace(setIpdno))
                {
                    string pIpd_no = getHistoryList(setIpdno, 0);
                    string pDiag_date = getHistoryList(setIpdno, 1);
                    string endDate = getHistoryList(setIpdno, 2);
                    string pSource = getHistoryList(setIpdno, 3);
                   
                }
                else
                {
                    rm.status = RESPONSE_STATUS.ERROR;
                    msgList.Add("取得病患資料失敗!請洽資訊人員");
                    LogTool.SaveLogMessage("setIpdno為空值", actionName, GetLogToolCS.BaseModel);
                }

            }
            catch (Exception ex)
            {
                rm.status = RESPONSE_STATUS.EXCEPTION;
                rm.message = "存檔檢查發生錯誤，請洽資訊人員";
                LogTool.SaveLogMessage(ex, actionName, GetLogToolCS.BaseModel);
            }
            finally
            {
                if (msgList != null && msgList.Count > 0)
                {
                    rm.status = RESPONSE_STATUS.ERROR;
                    rm.message = string.Join("\n", msgList);
                }
            }
            return rm;
        }

        /// <summary>
        /// 取得收案相關註記
        /// </summary>
        /// <param name="pChartNo">病歷號</param>
        /// <returns></returns>
        public Dictionary<string, bool> getCaseStatus(string pChartNo)
        {
            Dictionary<string, bool> statusList = new Dictionary<string, bool>();
            statusList.Add("CPT", false);
            statusList.Add("DNR", false);
            statusList.Add("VPN", false);
            string actionName = "getCaseStatus";
            try
            {
                string sql = string.Format("SELECT CPT_STATUS,DNR_MARK,VPN_MARK FROM {1} WHERE CHART_NO={0} AND case_id = {2}", SQLDefend.SQLString(pChartNo), GetTableName.RCS_RT_CASE.ToString(), SQLDefend.SQLString(pat_info.case_id));
                DataTable dt = this.DBA.getSqlDataTable(sql);
                if (DTNotNullAndEmpty(dt))
                {
                    foreach (DataRow dr in dt.Rows)
                    {
                        if (!DBNull.ReferenceEquals(dr["CPT_STATUS"], DBNull.Value) && !string.IsNullOrWhiteSpace(dr["CPT_STATUS"].ToString().Trim()))
                        {
                            if (dr["CPT_STATUS"].ToString().Trim() == "1") statusList["CPT"] = true;
                        }
                        if (!DBNull.ReferenceEquals(dr["DNR_MARK"], DBNull.Value) && !string.IsNullOrWhiteSpace(dr["DNR_MARK"].ToString().Trim()))
                        {
                            if (dr["DNR_MARK"].ToString().Trim() == "1") statusList["DNR"] = true;
                        }
                        if (!DBNull.ReferenceEquals(dr["VPN_MARK"], DBNull.Value) && !string.IsNullOrWhiteSpace(dr["VPN_MARK"].ToString().Trim()))
                        {
                            if (dr["VPN_MARK"].ToString().Trim() == "1") statusList["VPN"] = true;
                        }
                        //是否有CPT記錄
                        object Obj = this.DBA.ExecuteScalar(string.Format("SELECT COUNT(*) FROM {1} WHERE CHART_NO={0}", SQLDefend.SQLString(pChartNo), GetTableName.RCS_CPT_RECORD.ToString()));
                        if (Obj != null && Obj.ToString().Trim() != "" && int.Parse(Obj.ToString().Trim()) > 0 && dr["CPT_STATUS"].ToString().Trim() == "1")
                            statusList["CPT"] = true;
                    }
                }
                if(!string.IsNullOrWhiteSpace(this.DBA.LastError))
                {
                    LogTool.SaveLogMessage(this.DBA.LastError, actionName, GetLogToolCS.BaseModel);
                }

            }
            catch (Exception ex)
            {
                LogTool.SaveLogMessage(ex, actionName, GetLogToolCS.BaseModel);
            }
          
            return statusList;
        }
 
    }

    /// <summary> 系統參數集合 </summary>
    public class SysParamCollection : List<SysParams>
    {

        /// <summary> 將參數群組加入目前的collection </summary>
        /// <param name="params_group"></param>
        public void append_modal(SysParamCollection params_group)
        {
            this.AddRange(params_group);
        }

        /// <summary> 找出群組參數 </summary>
        /// <param name="modal"></param>
        /// <param name="group"></param>
        /// <returns></returns>
        public List<SysParams> get_group_list(string modal, string group)
        {
            return this.FindAll(x => x.P_MODEL == modal && x.P_GROUP == group && x.P_NAME != "NULL" && !string.IsNullOrWhiteSpace(x.P_NAME));
        }
        public string get_modal_data(string modal)
        {
            return JsonConvert.SerializeObject(this.FindAll(x => x.P_MODEL == modal && x.P_NAME != "NULL" && !string.IsNullOrWhiteSpace(x.P_NAME)));
        }
        public List<SysParams> get_group_list(string modal, List<string> group)
        {
            return this.FindAll(x => x.P_MODEL == modal && group.Contains(x.P_GROUP) && x.P_NAME != "NULL" && !string.IsNullOrWhiteSpace(x.P_NAME));
        }
        /// <summary> 找出群組參數 </summary>
        /// <param name="modal"></param>
        /// <param name="group"></param>
        /// <returns></returns>
        public List<SelectListItem> getGroupList(string modal, string group)
        {
            return this.FindAll(x => x.P_MODEL == modal && x.P_GROUP == group).GroupBy(x => new { x.P_NAME, x.P_VALUE }).Select(y => new SelectListItem()
            {
                Text = y.Key.P_NAME,
                Value = y.Key.P_VALUE,
            }).ToList();
        }
    }


    /// <summary>
    /// 系統組態設定
    /// </summary>
    public class SysParams:SelectListItem
    {
        public string P_ID { set; get; }
        public string P_MODEL { set; get; }
        public string P_GROUP { set; get; }
        public string P_NAME { set; get; }
        public string P_VALUE { set; get; }
        public string P_LANG { set; get; }
        public string P_SORT { set; get; }
        public string P_MEMO { set; get; }
        public string P_STATUS { set; get; }
        public string P_MANAGE { set; get; }
        public string P_GROUP_NAME { set; get; }
        public bool disabled { get; set; }
    }

    /// <summary> RCS_SYS_PARAMS 參數類別 </summary>
    public enum GetP_MODEL
    {
        /// <summary> 使用者 </summary>
        user,
        /// <summary> 成員 </summary>
        role,
        /// <summary> 表單類型 </summary>
        rt_form,
        /// <summary> 表單類型 </summary>
        phrase
    }

    /// <summary> TableName </summary>
    public enum GetTableName
    {
        /// <summary> 呼吸照護清單 </summary>
        RCS_RT_CARE_SCHEDULING,
        /// <summary> 病人接收案狀態清單 </summary>
        RCS_RT_CASE,
        /// <summary> 自訂醫師 </summary>
        RCS_RT_CASE_DOC,
        /// <summary> 授權清單 </summary>
        RCS_DATA_RtGrantList,
        /// <summary> 呼吸照護紀錄單主表 </summary>
        RCS_RECORD_MASTER,
        /// <summary> 呼吸照護紀錄單名細 </summary>
        RCS_RECORD_DETAIL, 
        /// <summary> ABG資料使用紀錄 </summary>
        RCS_DATA_ABG_USAGE,
        /// <summary> 呼吸器功能檢核表 </summary>
        RCS_VENTILATOR_CHECKLIST,
        /// <summary> 呼吸器患者評估 </summary>
        RCS_PATIENT_ASSESS,
        /// <summary> 呼吸器脫離評估 </summary>
        RCS_WEANING_ASSESS,
        /// <summary> CPT評估表主檔 </summary>
        RCS_CPT_ASS_MASTER,
        /// <summary> CPT評估表明細 </summary>
        RCS_CPT_ASS_DETAIL,
        /// <summary> CPT記錄表 </summary>
        RCS_CPT_RECORD,
        /// <summary> 交班表暫存 </summary>
        RCS_RT_ISBAR_SHIFT,
        /// <summary> 病患統計(7190) </summary>
        RCS_ORDER_TEMP,
        /// <summary> 區域分派 </summary>
        RCS_RT_ASSIGNMENT,
        /// <summary> 呼吸器維護明細 </summary>
        RCS_VENTILATOR_MAINTAIN,
        /// <summary> 呼吸器維護主表 </summary>
        RCS_VENTILATOR_SETTINGS,
        /// <summary> 排班表 </summary>
        RCS_SYS_SCHEDULING,
        /// <summary> 輔具評估單明細 </summary>
        RT_MEASURES_FORM_DETAIL,
        /// <summary> 輔具評估單主表 </summary>
        RT_MEASURES_FORM_MASTER,
        /// <summary> 記錄前端錯誤訊息 </summary>
        RCS_SYS_LOG,
        /// <summary> 系統設定參數 </summary>
        RCS_SYS_PARAMS,
        /// <summary> 上傳院內資料錯誤記錄檔 </summary>
        RCS_SYS_UPLOAD_LOG,
        /// <summary>呼吸器數值顯示設定檔</summary>
        RCS_VIP_DATA_SETTINGS,
        /// <summary>
        /// 使用者權限
        /// </summary>
        RCS_SYS_USER_POWER,
        /// <summary>
        /// 決策資源主表
        /// </summary>
        RCS_SYS_DECISION_SUPPORT_MASTER,
        /// <summary>
        /// 決策資源明細檔
        /// </summary>
        RCS_SYS_DECISION_SUPPORT_DETAIL,
        /// <summary>
        /// 功能清單
        /// </summary>
        RCS_SYS_FUNCTION_LIST,
        /// <summary>
        /// CXR 繪圖畫線 2018.07.02
        /// </summary>
        RCS_CXR_JSON

    }


    /// <summary>
    /// 上傳XML基本資料
    /// </summary>
    public class xmlBaseData
    {
        /// <summary>
        /// 病患姓名
        /// </summary>
        [XmlElement(ElementName = "patName")]
        //[XmlElement(Namespace = "姓名", ElementName = "patName")]
        public string patName = "";
        /// <summary>
        /// 來源資料(R:急診、O:門診、I:住院)
        /// </summary>
        [XmlElement(ElementName = "sourceType")]
        //[XmlElement(Namespace = "來源資料(E:急診、O:門診、I:住院)", ElementName = "sourceType")]
        public string sourceType = "";
        /// <summary>
        /// 性別(1:男生、0:女生、2:未知)
        /// </summary>
        [XmlElement(ElementName = "sex")]
        //[XmlElement(Namespace = "性別(1:男生、0:女生、2:未知)", ElementName = "sex")]
        public string sex = "";
        /// <summary>
        /// 年齡
        /// </summary>
        [XmlElement(ElementName = "age")]
        //[XmlElement(Namespace = "年齡", ElementName = "age")]
        public string age = "";
        /// <summary>
        /// 病歷號
        /// </summary>
        [XmlElement(ElementName = "chartNo")]
        //[XmlElement(Namespace = "病歷號", ElementName = "chartNo")]
        public string chartNo = "";
        /// <summary>
        /// 批價序號
        /// </summary>
        [XmlElement(ElementName = "ipdNo")]
        //[XmlElement(Namespace = "批價序號", ElementName = "ipdNo")]
        public string ipdNo = "";
        /// <summary>
        /// 床號
        /// </summary>
        [XmlElement(ElementName = "bedNo")]
        //[XmlElement(Namespace = "床號", ElementName = "bedNo")]
        public string bedNo = "";
        /// <summary>
        /// 新增人員(代碼) 
        /// </summary>
        [XmlElement(ElementName = "Signature")]
        //[XmlElement(Namespace = "新增人員(代碼)", ElementName = "Signature")]
        public string Signature = "";
        /// <summary>
        /// 資料狀態(1="新增",2="歷史資料",9="刪除")
        /// </summary>
        [XmlElement(ElementName = "datastatus")]
        //[XmlElement(Namespace = "(1=新增,9=刪除)", ElementName = "datastatus")]
        public string datastatus = "";

    }

    public class AllowedIpOnlyAttribute : FilterAttribute, IAuthorizationFilter
    {
        private string[] ipList = new string[] { };
        //建構式接收以逗號或分號分隔的IP清單，限定存取來源
        //TODO: 如要方便事後修改，可擴充成由config讀取IP清單，但會增加被破解風險
        public AllowedIpOnlyAttribute(string allowedIps)
        {
            ipList = allowedIps.Split(',', ';');
        }
        #region IAuthorizationFilter Members
        public void OnAuthorization(AuthorizationContext filterContext)
        {
            //實作OnAuthorization，當來源IP不在清單上，彈出錯誤
            string clientIp = filterContext.HttpContext.Request.UserHostAddress;
            if (!ipList.Contains(clientIp))
                throw new ApplicationException("Disallowed Client IP!");
        }
        #endregion
    }


    //限定本機存取為AllowedIpOnlyAttribute的特殊情境，限定IP=::1或127.0.0.1
    public class LocalhostOnlyAttribute : AllowedIpOnlyAttribute
    {
        public LocalhostOnlyAttribute()
            : base("::1;127.0.0.1")
        {
        }
    }

    

}
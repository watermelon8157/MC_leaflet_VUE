using System;
using System.Collections.Generic;
using System.Linq;
using RCS_Data.Models.DB;
using Com.Mayaminer;
using Newtonsoft.Json;
using System.Data;
using log4net;
using System.IO;
using RCSData.Models;
using System.Reflection;
using mayaminer.com.library;
using RCS_Data.Models.ViewModels;

namespace RCS_Data.Models
{
    /// <summary>
    /// 基本方法
    /// </summary>
    public class BasicFunction : BASIC_PARAMS
    {
        private string csName { get { return "BasicFunction"; } }

        /// <summary>
        /// 取得使用天數
        /// </summary>
        /// <param name="pIpdNo">批價序號</param>
        /// <param name="pChart_no">病歷號</param>
        /// <param name="pDate">日期</param>
        /// <param name="on_dateList">回傳日期List</param>
        /// <returns></returns>
        public int showUseDays(string pIpdNo, string pChart_no, string pDate, List<string> pArtificial_airway_typeList, out List<string> on_dateList)
        {
            List<DB_RCS_ON_MODE> pList = new List<DB_RCS_ON_MODE>();
            on_dateList = new List<string>();
            int days = 0;
            SQLProvider dbLink = new SQLProvider();
            DateTime nowDate = DateTime.Now; 
            if (!string.IsNullOrWhiteSpace(pDate))
            {
                nowDate = DateTime.Parse(pDate);
            }
            if (string.IsNullOrWhiteSpace(pDate))
            {
                pDate = nowDate.ToString(Function_Library.getDateNowString(DATE_FORMAT.yyyy_MM_dd));
            }
            try
            {
                days = this.countDays(pIpdNo, pChart_no, pDate, pArtificial_airway_typeList, out on_dateList, out pList, false);
            }
            catch (Exception ex)
            {
                LogTool.SaveLogMessage("取得使用天數失敗" + ex, "getUseDays", this.csName);
            }
            return days;
        }
        /// <summary>
        /// 取得使用天數
        /// </summary>
        /// <param name="pIpdNo">批價序號</param>
        /// <param name="pChart_no">病歷號</param>
        /// <param name="pDate">日期</param>
        /// <param name="on_dateList">回傳日期List</param>
        /// <returns></returns>
        public int showUseDays(string pIpdNo, string pChart_no, string pDate , out List<string> on_dateList)
        {
            List<DB_RCS_ON_MODE> pList = new List<DB_RCS_ON_MODE>(); 
            return  this.showUseDays(pIpdNo, pChart_no, pDate, null, out on_dateList );
        }


        /// <summary>
        /// 取得使用天數
        /// </summary>
        /// <param name="pIpdNo">批價序號</param>
        /// <param name="pChart_no">病歷號</param>
        /// <param name="pDate">日期</param>
        /// <param name="on_dateList">回傳日期List</param>
        /// <returns></returns>
        public int getUseDays(string pIpdNo, string pChart_no, string pDate, List<string> pArtificial_airway_typeList, out List<string> on_dateList)
        {
            List<DB_RCS_ON_MODE> pList = new List<DB_RCS_ON_MODE>();
            on_dateList = new List<string>();
            int days = 0; 
            SQLProvider dbLink = new SQLProvider(); 
            DateTime nowDate = DateTime.Now; 
            if (string.IsNullOrWhiteSpace(pDate))
            {
                pDate = nowDate.ToString(Function_Library.getDateNowString(DATE_FORMAT.yyyy_MM_dd));
            }
            try
            {

                days = this.countDays(pIpdNo, pChart_no, pDate, pArtificial_airway_typeList, out on_dateList, out pList, false);
                if (DateTime.Parse(pDate).Date == nowDate.Date && pList.Count > 0 && !pList.Exists(x => DateTime.Parse(x.recorddate).Date == nowDate.Date))
                {
                    days++;
                }

            }
            catch (Exception ex)
            {
                LogTool.SaveLogMessage("取得使用天數失敗" + ex, "getUseDays", this.csName);
            }
            return days;
        }

        /// <summary>
        /// 取得使用天數
        /// </summary>
        /// <param name="pIpdNo">批價序號</param>
        /// <param name="pChart_no">病歷號</param>
        /// <param name="pDate">日期</param>
        /// <param name="on_dateList">回傳日期List</param>
        /// <returns></returns>
        public int getUseDays(string pIpdNo, string pChart_no, string pDate , out List<string> on_dateList)
        {
            List<DB_RCS_ON_MODE> pList = new List<DB_RCS_ON_MODE>();
            return this.getUseDays(pIpdNo, pChart_no, pDate, null, out on_dateList );
        }

        /// <summary>檢查呼吸器治療評估單是否有初次呼吸器開始使用日期</summary>
        /// <param name="pIpdNo">批價序號</param>
        /// <returns></returns>
        protected string CheckCptAssCase_HaveStartTime(string pIpdNo)
        {

            string sDate = "";
            string cpt_id = "";
            string FromUnit = "";
            SQLProvider dbLink = new SQLProvider();
            string CptAss_CASE_sql = @"SELECT * FROM "+ DB_TABLE_NAME.DB_RCS_CPT_ASS_MASTER + " WHERE IPD_NO = '" + pIpdNo + "' ORDER BY CREATE_DATE DESC";
            DataTable CptAssCASE_dt = dbLink.DBA.getSqlDataTable(CptAss_CASE_sql);
            if (DTNotNullAndEmpty(CptAssCASE_dt))
            {
                cpt_id = CptAssCASE_dt.Rows[0]["cpt_id"].ToString();
                string CptAss_DTL_sql = @"SELECT * FROM "+ DB_TABLE_NAME.DB_RCS_CPT_ASS_DETAIL + " WHERE cpt_id = '" + cpt_id + "' AND CPT_ITEM = 'rt_start_time'";
                DataTable CptAssDTL_dt = dbLink.DBA.getSqlDataTable(CptAss_DTL_sql);

                if (DTNotNullAndEmpty(CptAssDTL_dt))
                {
                    FromUnit = CptAssDTL_dt.Rows[0]["CPT_VALUE"].ToString(); 
                }
            }

            if (FromUnit != null && FromUnit.Length > 0)
            {
                sDate = FromUnit;
            }
            else
            {
                sDate = "";
            }

            return sDate;
        }

        protected virtual int countDays(string pIpdNo, string pChart_no, string pDate, List<string> pArtificial_airway_typeList, out List<string> on_dateList, out List<DB_RCS_ON_MODE> pModeList, bool insert = true )
        {
            on_dateList = new List<string>();
            pModeList = new List<DB_RCS_ON_MODE>();
            DateTime nowDate = DateTime.Now;
            DateTime pDateTime = string.IsNullOrWhiteSpace(pDate) ? nowDate : DateTime.Parse(pDate); 
            DateTime sDate = pDateTime; 
            SQLProvider dbLink = new SQLProvider();
            Dapper.DynamicParameters dp = new Dapper.DynamicParameters();
            int days = 0; 
            try
            {
                if (string.IsNullOrWhiteSpace(pDate))
                {
                    pDate = pDateTime.ToString(Function_Library.getDateNowString(DATE_FORMAT.yyyy_MM_dd));
                }
                DateTime CptAssCase_sDate = pDateTime;
                string CptAssCase_HaveStartTime = "";
                CptAssCase_HaveStartTime = CheckCptAssCase_HaveStartTime(pIpdNo); //檢查呼吸器治療評估單是否有初次呼吸器開始使用日期(2019/09/02_FJU新加規則)
                if (!string.IsNullOrWhiteSpace(CptAssCase_HaveStartTime))
                {
                    // 如果呼吸器治療評估單『有』【初次呼吸器開始使用日期】，以【初次呼吸器開始使用日期】為主，不管【呼吸照護紀錄單】在【初次呼吸器開始使用日期】之前是否有使用呼吸器
                    CptAssCase_sDate = DateTime.Parse(CptAssCase_HaveStartTime);
                    days = (pDateTime.Date - CptAssCase_sDate.Date).Days + 1;
                    on_dateList.Add(CptAssCase_HaveStartTime);
                }
                else
                {
                    #region 非VPN病患計算方式 
                    string query = "";
                    List<DB_RCS_ON_MODE> recordList = new List<DB_RCS_ON_MODE>();
                    //沒有計算過重新計算MV 天數
                    query = string.Concat("SELECT * FROM ", DB_TABLE_NAME.DB_RCS_RECORD_DETAIL, " WHERE RECORD_ID in( ",
                        " SELECT RECORD_ID FROM ", DB_TABLE_NAME.DB_RCS_RECORD_MASTER, " WHERE IPD_NO = @IPD_NO AND CHART_NO = @CHART_NO AND DATASTATUS = '1'",
                        ") AND ITEM_NAME in ('respid','artificial_airway_type','recorddate')");
                    dp.Add("IPD_NO", pIpdNo);
                    dp.Add("CHART_NO", pChart_no);
                    dp.Add("RECORDDATE", pDate);
                    List<DB_RCS_RECORD_DETAIL> dList = dbLink.DBA.getSqlDataTable<DB_RCS_RECORD_DETAIL>(query, dp);
                    foreach (string record_id in dList.Select(x => x.RECORD_ID).Distinct())
                    {

                        recordList.Add(new DB_RCS_ON_MODE()
                        {
                            RECORD_ID = record_id,
                            artificial_airway_type =
                             dList.Exists(x => x.RECORD_ID == record_id && x.ITEM_NAME == "artificial_airway_type") ?
                             dList.Find(x => x.RECORD_ID == record_id && x.ITEM_NAME == "artificial_airway_type").ITEM_VALUE : "",
                            recorddate =
                             dList.Exists(x => x.RECORD_ID == record_id && x.ITEM_NAME == "recorddate") ?
                             dList.Find(x => x.RECORD_ID == record_id && x.ITEM_NAME == "recorddate").ITEM_VALUE : "",
                            respid =
                             dList.Exists(x => x.RECORD_ID == record_id && x.ITEM_NAME == "respid") ?
                             dList.Find(x => x.RECORD_ID == record_id && x.ITEM_NAME == "respid").ITEM_VALUE : ""
                        });
                    } 
                    recordList = recordList.FindAll(x => !string.IsNullOrWhiteSpace(x.artificial_airway_type) || !string.IsNullOrWhiteSpace(x.respid))
                        .OrderByDescending(x => DateTime.Parse(x.recorddate)).ToList();
                    if (recordList.Exists(x => !string.IsNullOrWhiteSpace(x.artificial_airway_type)))
                    {
                        recordList = recordList.FindAll(x =>
                         DateTime.Parse(x.recorddate) >= DateTime.Parse(
                             recordList[recordList.FindLastIndex(y => !string.IsNullOrWhiteSpace(y.artificial_airway_type))].recorddate)
                         && (pArtificial_airway_typeList == null || pArtificial_airway_typeList.Count == 0 || pArtificial_airway_typeList.Contains(x.artificial_airway_type))
                         && DateTime.Parse(x.recorddate) <= pDateTime
                             ).ToList();
                    }
                    else
                    {
                        recordList = new List<DB_RCS_ON_MODE>();
                    }

                    if (recordList.Exists(x => !string.IsNullOrWhiteSpace(x.respid)))
                    {
                        recordList = recordList.FindAll(x =>
                        !string.IsNullOrWhiteSpace(x.respid)
                        && DateTime.Parse(x.recorddate) <= pDateTime
                           ).ToList();
                    }
                    else
                    {
                        recordList = new List<DB_RCS_ON_MODE>();
                    } 
                    pModeList.AddRange(recordList);
                    string tempDate = "";
                    foreach (DB_RCS_ON_MODE item in recordList)
                    {

                        if (string.IsNullOrWhiteSpace(tempDate))
                        {
                            if ((pDateTime.Date - DateTime.Parse(item.recorddate).Date).Days + 1 > 5)
                            {
                                break;
                            }
                            tempDate = item.recorddate;
                            days++;
                        }
                        else
                        {
                            if ((DateTime.Parse(tempDate).Date - DateTime.Parse(item.recorddate).Date).Days > 5)
                            {
                                break;
                            }
                            else
                            {
                                days = days + (DateTime.Parse(tempDate).Date - DateTime.Parse(item.recorddate).Date).Days;
                                tempDate = item.recorddate;

                            }
                        }
                    }
                    on_dateList.Add(tempDate);
                    if (days == 0)
                    {
                        on_dateList = new List<string>();
                    }


                    #endregion
                } 
            }
            catch (Exception ex)
            {
                LogTool.SaveLogMessage("取得使用天數失敗" + ex, "countDays", this.csName);
            } 
            return days;
        }

        /// <summary>
        /// 取得最後一次的呼吸記錄單
        /// </summary>
        /// <param name="ipd_no">病歷號</param>
        /// <param name="getItem">取得項目(預設null，全部取得)</param>
        /// <returns></returns>
        public Ventilator GetLastRTRec(string chart_no, string pIpdno, string[] getItem = null)
        {
            string RECORD_ID = "";
            Ventilator v = new Ventilator();
            DataTable dt = null;
            SQLProvider dbLink = new SQLProvider();
            string sql = "";
            try
            {
                string pIpdnoSql = "";
                if (string.IsNullOrWhiteSpace(pIpdno))
                {
                    pIpdnoSql = " AND IPD_NO = " + SQLDefend.SQLString(pIpdno);
                }
                string whereSql = "";
                if (getItem != null && getItem.Length > 0)
                    whereSql = string.Format(" ITEM_NAME in('{0}') AND ", string.Join("','", getItem));
                Dictionary<string, string> VCCH = new Dictionary<string, string>(); 
                sql = string.Concat(" select * from " + DB_TABLE_NAME.DB_RCS_RECORD_DETAIL + " where " + whereSql + " RECORD_ID = (",
                    " SELECT MAX(RECORD_ID) FROM ", DB_TABLE_NAME.DB_RCS_RECORD_MASTER, "  as rm WHERE DATASTATUS = '1' AND CHART_NO = " + SQLDefend.SQLString(chart_no),
                    pIpdnoSql,
                    " AND recorddate = (select MAX(recorddate) from ", DB_TABLE_NAME.DB_RCS_RECORD_MASTER, " where IPD_NO = rm.IPD_NO AND DATASTATUS = '1'))  ",
                    " UNION",
                    " SELECT RECORD_ID,'ONMODE_ID_2' ITEM_NAME ,ONMODE_TYPE2_ID ITEM_VALUE FROM ", DB_TABLE_NAME.DB_RCS_RECORD_MASTER, " ",
                    " WHERE RECORD_ID = ( SELECT MAX(RECORD_ID) FROM ", DB_TABLE_NAME.DB_RCS_RECORD_MASTER, " as rm WHERE DATASTATUS = '1' AND CHART_NO = ", SQLDefend.SQLString(chart_no), pIpdnoSql,
                    " AND recorddate = (select MAX(recorddate) from ", DB_TABLE_NAME.DB_RCS_RECORD_MASTER, " where IPD_NO = rm.IPD_NO AND DATASTATUS = '1'))");

                dt = dbLink.DBA.getSqlDataTable(sql);
                if (DTNotNullAndEmpty(dt))
                {
                    RECORD_ID = checkDataColumn(dt.Rows[0], "RECORD_ID");
                    foreach (DataRow dr in dt.Rows)
                    {
                        if (!DBNull.ReferenceEquals(dr["ITEM_NAME"], DBNull.Value) && !DBNull.ReferenceEquals(dr["ITEM_VALUE"], DBNull.Value))
                            VCCH.Add(dr["ITEM_NAME"].ToString(), dr["ITEM_VALUE"].ToString());
                    }
                }
                else
                {
                    if (dbLink.DBA.lastError != null && dbLink.DBA.lastError != "")
                        LogTool.SaveLogMessage(dbLink.DBA.lastError, "GetLastRTRec", GetLogToolCS.BaseModel);
                } 
                if (dt != null && dt.Rows.Count > 0) v.hasData = true;
                v = JsonConvert.DeserializeObject<Ventilator>(JsonConvert.SerializeObject(VCCH));
                v.RECORD_ID = RECORD_ID; 
                v._is_humidifier = v.device;
            }
            catch (Exception ex)
            {
                LogTool.SaveLogMessage("sql=" + sql + ",ex=" + ex, "GetLastRTRec", GetLogToolCS.BaseModel);
            }

            return v;
        }

       
    }

    /// <summary>
    /// 基本方法
    /// </summary>
    public class BASIC_PARAMS
    { 
        #region switch


        /// <summary>
        /// 是否使用測試資料
        /// </summary>
        public bool useTestData { get { return bool.Parse(IniFile.GetConfig("System", "useTestData")); } }

        #region isBasicMode
        private static string _isBasicMode { get; set; }
        /// <summary>
        /// 是否是馬雅標準版模式開關(true:開啟,false:關閉)
        /// </summary>
        public static bool isBasicMode
        {
            get
            {
#if DEBUG
                return false;
#endif
                if (!string.IsNullOrWhiteSpace(_isBasicMode))
                {
                    return bool.Parse(_isBasicMode);
                }
                _isBasicMode = IniFile.GetConfig("System", "isBasicMode");
                return bool.Parse(_isBasicMode);
            }
        }
        #endregion
        #region isDebuggerMode
        private static string _isDebuggerMode { get; set; }

        /// <summary>是否debug模式(true:顯示完整功能以及開發功能,false:顯示基本版功能)
        /// <para>開發中功能記得加入tag避免，發佈時顯示尚未完成的功能</para>
        /// </summary>
        public static bool isDebuggerMode
        {
            get
            {
#if DEBUG
                return true;
#endif
                if (!string.IsNullOrWhiteSpace(_isDebuggerMode))
                {
                    return bool.Parse(_isDebuggerMode);
                }
                _isDebuggerMode = IniFile.GetConfig("System", "isDebuggerMode");
                return bool.Parse(_isDebuggerMode);
            }
        }
        #endregion

        private static string _isTestWeb { get; set; }
        /// <summary>
        /// 是否是測試網站
        /// </summary>
        public static bool isTestWeb
        {
            get
            {
                if (!string.IsNullOrWhiteSpace(_isTestWeb))
                {
                    return bool.Parse(_isTestWeb);
                }
                _isTestWeb = IniFile.GetConfig("System", "isTestWeb");
                return bool.Parse(_isTestWeb);
            }
        }

        private static string _VipColSettingsSwitch { get; set; }
        /// <summary>
        /// vip欄位設定開關(true:開啟檢查vip欄位資料,false:預設關閉)
        /// </summary>
        public static bool VipColSettingsSwitch
        {
            get
            {
                if (!string.IsNullOrWhiteSpace(_VipColSettingsSwitch))
                {
                    return bool.Parse(_VipColSettingsSwitch);
                }
                _VipColSettingsSwitch = IniFile.GetConfig("SystemConfig", "VipColSettingsSwitch");
                return bool.Parse(_VipColSettingsSwitch);
            }
        }

        #endregion

        /// <summary>
        /// 檢查Datatable是否不為Null且有Rows
        /// </summary>
        /// <param name="pDT"></param>
        /// <returns></returns>
        public bool DTNotNullAndEmpty(DataTable pDT)
        {
            return (pDT != null && pDT.Rows.Count > 0);
        }

        RESPONSE_MSG _RESPONSE_MSG;
        /// <summary> 訊息 </summary>
        public RESPONSE_MSG RESPONSE_MSG
        {
            get
            {
                if (_RESPONSE_MSG == null)
                {
                    _RESPONSE_MSG = new RESPONSE_MSG();
                }
                return _RESPONSE_MSG;
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

    }


    /// <summary>
    /// 基本方法
    /// </summary>
    public abstract class BASIC_FUNCTION : BASIC_PARAMS, ILogger
    {  
        protected ILog _logger;
        /// <summary> 記錄log </summary>
        public virtual ILog logger
        {
            get
            {
                if (_logger == null)
                {
                    _logger = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
                }
                return _logger;
            }
            set
            {
                _logger = value;
            }
        }
      
        /// <summary>
        /// 檢查Datatable是否不為Null且有Rows
        /// </summary>
        /// <param name="pDT"></param>
        /// <returns></returns>
        public string getJsonStr(dynamic obj)
        {
            return JsonConvert.SerializeObject(obj);
        }

        /// <summary>
        /// 取得檔案的物件轉取得jsonStr
        /// <para>路徑為App_Data\\" + fileName + ".json"</para>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="fileName">檔案名稱</param>
        /// <param name="obj"></param>
        public void getJsonObj<T>(string fileName, ref List<T> obj)
        {
            string actionName = "getJsonObj";
            try
            {
                LogTool.SaveLogMessage(AppDomain.CurrentDomain.BaseDirectory, "test", "test");
                obj = JsonConvert.DeserializeObject<List<T>>(File.ReadAllText(AppDomain.CurrentDomain.BaseDirectory + "App_Data\\" + fileName + ".json"));
            }
            catch (Exception ex)
            {
                LogTool.SaveLogMessage(ex, actionName, GetLogToolCS.FunctionFactory);
            }
        }

    }

    /// <summary>
    /// 資料庫連線設定
    /// </summary>
    public class DBA_SETTING : BASIC_FUNCTION, ILogger
    {
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

    }

    /// <summary>
    /// 取的資料物件
    /// <para>1.取得web service資料</para>
    /// <para>2.資料庫連線</para>
    /// </summary>
    public class SYS_BASIC : DBA_SETTING, ILogger
    {
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
        WebMethod _web_method;
        /// <summary> 取得web service資料 </summary>
        public WebMethod web_method
        {
            get
            {
                if (_web_method == null)
                {
                    _web_method = new WebMethod();
                }
                return _web_method;
            }
        }


    }
}

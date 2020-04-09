using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using Com.Mayaminer;
using Newtonsoft.Json;
using RCS_Data;
using mayaminer.com.library;
using RCS.Controllers;
using mayaminer.com.jxDB;
using System.Reflection;
using System.Text;
using RCSData.Models;
using RCS_Data.Models.ViewModels;

namespace RCS.Models.DB
{
    public class SystemManage : BaseModel
    {
        static string P_LANG = "zh-tw";
        public string LastError { get; private set; }
        
        /// <summary>取得區域資料</summary>
        /// <returns></returns>
        public List<KeyValuePair<string, string>> getAreaList()
        {
            var area_list = new List<KeyValuePair<string, string>>();
            List<SysParams> list = this.getRCS_SYS_PARAMS_GName("system", "area", @pStatus: "1");
            list.ForEach(x => {
                area_list.Add(new KeyValuePair<string, string>(x.P_NAME, x.P_VALUE));            
            });
            return area_list;
        }
  
        #region 排班管理

        /// <summary>取得排班表(View or Json)</summary>
        /// <param name="year">年</param>
        /// <param name="month">月</param>
        /// <returns></returns>
        public scheduling getSchedulingData(string year = "", string month = "")
        {
            scheduling sch_data = new scheduling();
            try
            {
                DateTime now_date = DateTime.Now;
                year = year == "" ? now_date.Year.ToString() : year;
                month = month == "" ? now_date.Month.ToString() : month;
                sch_data.year = int.Parse(year);
                sch_data.month = int.Parse(month);
                List<Dictionary<string, string>> now_where_list = new List<Dictionary<string, string>>();
                List<Dictionary<string, string>> where_list = new List<Dictionary<string, string>>();

                string sql = "SELECT RT_ID OP_ID, RT_NAME OP_NAME, REMARK, WORK_DATA FROM " + GetTableName.RCS_SYS_SCHEDULING + " WHERE yyyymm_date ='" + year + month.PadLeft(2, '0') + "'";
                DataTable dt = this.DBA.getSqlDataTable(sql);
                if (DTNotNullAndEmpty(dt))
                {
                    now_where_list = fillDtToDictionaryList(dt);
                    Dictionary<string, string> extra = JsonConvert.DeserializeObject<Dictionary<string, string>>(dt.Rows[0]["REMARK"].ToString());
                    if (extra != null)
                    {
                        if (extra.ContainsKey("month_off")) sch_data.month_off = extra["month_off"];
                        if (extra.ContainsKey("working_hr")) sch_data.working_hr = extra["working_hr"];
                    }
                }
                if (this.DBA.LastError != "")
                    LogTool.SaveLogMessage(this.DBA.LastError, MethodInfo.GetCurrentMethod().Name, MethodInfo.GetCurrentMethod().DeclaringType.Name);

                //取得排班人員清單
                Dictionary<string, string> Temp_1 = null;
                List<Dictionary<string, string>> user_list = UserMaintain_List(false);
                //排除主治醫生後的User清單
                user_list = user_list.Where(x => x["P_GROUP"] != "doctor").ToList();
                foreach (var item in user_list)
                {
                    string op_id = item["P_VALUE"].ToString();
                    Dictionary<string, string> sel_wl = now_where_list.Where<Dictionary<string, string>>(ww => ww["OP_ID"] == op_id).FirstOrDefault();
                    if (sel_wl != null)
                    {
                        //已有當月資料
                        where_list.Add(sel_wl);
                    }
                    else
                    {
                        //無當月資料時
                        Temp_1 = new Dictionary<string, string>();
                        Temp_1.Add("OP_ID", op_id);
                        Temp_1.Add("OP_NAME", item["P_NAME"].ToString());
                        Temp_1.Add("REMARK", "");
                        Temp_1.Add("WORK_DATA", "");
                        where_list.Add(Temp_1);
                    }
                }

                List<SchedulData> schedul_data = new List<SchedulData>();
                foreach (var item in where_list)
                {
                    SchedulData sd = new SchedulData();
                    sd.op_id = item["OP_ID"];
                    sd.op_name = item["OP_NAME"];
                    sd.remark = item["REMARK"];
                    Dictionary<string, DayData> day_data = JsonConvert.DeserializeObject<Dictionary<string, DayData>>(item["WORK_DATA"]);
                    sd.day_data = day_data;
                    schedul_data.Add(sd);
                }

                sch_data.schedul_data = schedul_data;

                //取得排班表相關設定資料
                List<SysParams> shiftdt = this.getRCS_SYS_PARAMS(@pModel: "RT_scheduling"); //班別資料
                List<SysParams> areadt = this.getRCS_SYS_PARAMS("system", "area"); //區域資料
                shiftdt.ForEach(x => {
                    sch_data.shift_data.Add(new ShiftData()
                    {
                        name = x.P_NAME,
                        shortcut = x.P_VALUE,
                        work_type = x.P_GROUP == "shift_holiday" ? "R" : "W"
                    });
                });
                areadt.ForEach(x =>
                {
                    sch_data.area_data.Add(new AreaData()
                    {
                        name = x.P_NAME,
                        color = x.P_VALUE
                    });
                });
            }
            catch (Exception ex)
            {
                LogTool.SaveLogMessage(ex, MethodInfo.GetCurrentMethod().Name, MethodInfo.GetCurrentMethod().DeclaringType.Name);
            }
            return sch_data;
        }

        /// <summary>取得該月排班表DT</summary>
        /// <param name="year">年</param>
        /// <param name="month">月</param>
        /// <returns></returns>
        public DataTable getSchedulingDT(string year, string month)
        {
            string table_name = GetTableName.RCS_SYS_SCHEDULING.ToString();
            string SQLStr = string.Format("SELECT * FROM {0} WHERE YYYYMM_DATE LIKE {1}", table_name, SQLDefend.SQLLikeEndString(year + month));
            DataTable dt = this.DBA.getSqlDataTable(SQLStr);
            if (!DTNotNullAndEmpty(dt))
            {
                SQLStr = string.Format("SELECT * FROM {0} WHERE 1<>1", table_name);
                dt = this.DBA.getSqlDataTable(SQLStr);
            }
            dt.TableName = table_name;
            return dt;
        }
        #endregion

        #region 呼吸器維護

        public DataTable getDeviceType()
        {
            string sqlstatment = "select P_NAME,P_VALUE from RCS_SYS_PARAMS where P_MODEL = 'Index_device' and P_GROUP='device_model' order by P_SORT";
            DataTable dt = this.DBA.getSqlDataTable(sqlstatment);
            if (this.DBA.LastError != "")
            {
                LastError = this.DBA.LastError;
                LogTool.SaveLogMessage(this.DBA.LastError, MethodInfo.GetCurrentMethod().Name, MethodInfo.GetCurrentMethod().DeclaringType.Name);
            }
            return dt;
        }
        public DataTable getNewDeviceDT()
        {
            string strsql = "SELECT * FROM " + GetTableName.RCS_VENTILATOR_SETTINGS + " WHERE 1<>1";
            DataTable dt = this.DBA.getSqlDataTable(strsql);
            return dt;
        }
        public DataTable getDeviceDT(List<string> pDeviceNo, bool pCheckDup = false)
        {
            if (pDeviceNo.Count > 0)pDeviceNo = pDeviceNo.Select(x => string.Format("DEVICE_NO = '{0}'", x)).ToList();
            string deviceNo = string.Join(" OR ", pDeviceNo);
            string strsql = "SELECT *, CASE ISNULL(ROOM, '') WHEN '' THEN '' ELSE (SELECT TOP 1 '床號：' + BED_NO + ' 姓名：|' +PATIENT_NAME + '| 病歷號：' + CHART_NO FROM RCS_RT_CASE WHERE CHART_NO = A.ROOM ORDER BY CREATE_DATE DESC) END ON_POSITION FROM " + GetTableName.RCS_VENTILATOR_SETTINGS + " A WHERE 1=1";
            if (!pCheckDup) strsql += " AND USE_STATUS = 'Y'";
            if (pDeviceNo.Count > 0) strsql += string.Format(" AND ({0})", deviceNo);
            strsql += " ORDER BY DEVICE_NO";
            DataTable dt = this.DBA.getSqlDataTable(strsql);
            return dt;
        }

        /// <summary>呼吸器清單</summary>
        /// <param name="pDeviceNo">呼吸器編號DEVICE_NO</param>
        /// <returns></returns>
        public List<DeviceMaster> getDeviceList(bool pShowDel, string pDeviceNo = "")
        {
            List<DeviceMaster> deviceList = new List<DeviceMaster>();
            LastError = "";
            try
            {
                List<string> nolist = new List<string>();
                if (!string.IsNullOrEmpty(pDeviceNo)) nolist.Add(pDeviceNo);
                DataTable dt = this.getDeviceDT(nolist, pShowDel);
                if (DTNotNullAndEmpty(dt))
                {
                    foreach (DataRow dr in dt.Rows)
                    {
                        deviceList.Add(new DeviceMaster()
                        {
                            DEVICE_SEQ = checkDataColumn(dr, "DEVICE_SEQ"),
                            DEVICE_NO = checkDataColumn(dr, "DEVICE_NO"),
                            ROOM = checkDataColumn(dr, "ROOM"),
                            DEVICE_MODEL = checkDataColumn(dr, "DEVICE_MODEL"),
                            USE_STATUS = checkDataColumn(dr, "USE_STATUS"),
                            PURCHASE_DATE = checkDataColumn(dr, "PURCHASE_DATE"),
                            ON_POSITION = checkDataColumn(dr, "ON_POSITION")
                        });
                    }
                }
                if (this.DBA.LastError != "")
                {
                    LastError = this.DBA.LastError;
                    LogTool.SaveLogMessage(this.DBA.LastError, MethodInfo.GetCurrentMethod().Name, MethodInfo.GetCurrentMethod().DeclaringType.Name);
                }
            }
            catch (Exception ex)
            {
                LastError = ex.Message;
                LogTool.SaveLogMessage(ex, MethodInfo.GetCurrentMethod().Name, MethodInfo.GetCurrentMethod().DeclaringType.Name);
            }
            return deviceList;
        }

        /// <summary>取得呼吸器資料</summary>
        /// <param name="pDeviceNo"></param>
        public List<DeviceDetail> getMaintainList(List<string> pDeviceNo, string pSdate, string pEdate)
        {
            List<DeviceDetail> dtlList = new List<DeviceDetail>();
            LastError = "";
            try
            {
                pDeviceNo = pDeviceNo.Select(x => string.Format("A.DEVICE_NO = '{0}'", x)).ToList();
                string deviceNo = string.Join(" OR ", pDeviceNo);
                StringBuilder sb = new StringBuilder();
                sb.Append("SELECT A.DEVICE_NO, A.DEVICE_MODEL, A.DEVICE_SEQ, B.STATUS, B.START_DATE, B.END_DATE, B.REMARK, B.CREATE_DATE, B.CREATE_NAME, B.RUNNING_HR, B.SYSTEM_TEST, B.CLEAN_STATUS, ");
                sb.Append("ps.P_NAME STATUS_DESC, pt.P_NAME TEST_DESC, pc.P_NAME CLEAN_DESC FROM ");
                sb.Append(GetTableName.RCS_VENTILATOR_SETTINGS);
                sb.Append(" A JOIN ");
                sb.Append(GetTableName.RCS_VENTILATOR_MAINTAIN);
                sb.Append(" B ON A.DEVICE_NO = B.DEVICE_NO LEFT JOIN ");
                sb.Append(GetTableName.RCS_SYS_PARAMS);
                //-----2016/12/30 Vanda Mod
                //sb.Append(" ps ON B.STATUS = ps.P_VALUE AND ps.P_MODEL = 'Index_device' AND ps.P_GROUP = 'device_status' LEFT JOIN ");
                //sb.Append(GetTableName.RCS_SYS_PARAMS);
                //sb.Append(" pt ON B.STATUS = pt.P_VALUE AND pt.P_MODEL = 'Index_device' AND pt.P_GROUP = 'system_test' LEFT JOIN ");
                //sb.Append(GetTableName.RCS_SYS_PARAMS);
                //sb.Append(" pc ON B.STATUS = pc.P_VALUE AND pc.P_MODEL = 'Index_device' AND pc.P_GROUP = 'clean_status' ");
                sb.Append(" ps ON B.STATUS = ps.P_VALUE AND ps.P_MODEL = 'Index_device' AND ps.P_GROUP = 'device_status' LEFT JOIN ");
                sb.Append(GetTableName.RCS_SYS_PARAMS);
                sb.Append(" pt ON B.SYSTEM_TEST = pt.P_VALUE AND pt.P_MODEL = 'Index_device' AND pt.P_GROUP = 'system_test' LEFT JOIN ");
                sb.Append(GetTableName.RCS_SYS_PARAMS);
                sb.Append(" pc ON B.CLEAN_STATUS = pc.P_VALUE AND pc.P_MODEL = 'Index_device' AND pc.P_GROUP = 'clean_status' ");
                //-----
                sb.Append(string.Format(" WHERE B.STATUS <> 'N' AND ({0})", deviceNo));
                if (!string.IsNullOrEmpty(pSdate))
                    sb.Append(string.Format(" AND SUBSTRING(B.CREATE_DATE, 1,10) >= '{0}'", pSdate));
                if (!string.IsNullOrEmpty(pEdate))
                    sb.Append(string.Format(" AND SUBSTRING(B.CREATE_DATE, 1,10) <= '{0}'", pEdate));

                sb.Append(" ORDER BY START_DATE DESC");
                DataTable dt = this.DBA.getSqlDataTable(sb.ToString());
                if (DTNotNullAndEmpty(dt))
                {
                    dt.AsEnumerable().ToList().ForEach(x =>
                    {
                        dtlList.Add(new DeviceDetail()
                        {
                            DEVICE_NO = checkDataColumn(x, "DEVICE_NO"),
                            DEVICE_SEQ = checkDataColumn(x, "DEVICE_SEQ"),
                            STATUS = checkDataColumn(x, "STATUS_DESC"),
                            START_DATE = checkDataColumn(x, "START_DATE"),
                            END_DATE = checkDataColumn(x, "END_DATE"),
                            REMARK = checkDataColumn(x, "REMARK"),
                            CREATE_NAME = checkDataColumn(x, "CREATE_NAME"),
                            CREATE_DATE = checkDataColumn(x, "CREATE_DATE"),
                            RUNNING_HR = checkDataColumn(x, "RUNNING_HR"),
                            CLEAN_STATUS = checkDataColumn(x, "CLEAN_DESC"),
                            SYSTEM_TEST = checkDataColumn(x, "TEST_DESC")
                        });
                    });
                }
                if (this.DBA.LastError != "")
                {
                    LastError = this.DBA.LastError;
                    LogTool.SaveLogMessage(this.DBA.LastError, MethodInfo.GetCurrentMethod().Name, MethodInfo.GetCurrentMethod().DeclaringType.Name);
                }
            }
            catch (Exception ex)
            {
                LastError = ex.Message;
                LogTool.SaveLogMessage(ex, MethodInfo.GetCurrentMethod().Name, MethodInfo.GetCurrentMethod().DeclaringType.Name);
            }
            return dtlList;
        }

        public DataTable getMaintainDT(string pDeviceNo, List<string> pCreateDate)
        {
            string strsql = "SELECT * FROM " + GetTableName.RCS_VENTILATOR_MAINTAIN + " WHERE 1=1";
            if (!string.IsNullOrEmpty(pDeviceNo)) strsql += " AND DEVICE_NO = " + SQLDefend.SQLString(pDeviceNo);
            if (pCreateDate != null && pCreateDate.Count > 0)
            {
                pCreateDate = pCreateDate.Select(x => string.Format("CREATE_DATE = '{0}'", x)).ToList();
                //-----2016/12/30 Vanda Mod
                //string create_date = string.Join(" OR ", pDeviceNo);
                string create_date = string.Join(" OR ", pCreateDate);
                //-----
                strsql += string.Format(" AND ({0})", create_date);
            } 
            DataTable dt = this.DBA.getSqlDataTable(strsql);
            return dt;
        }

        public DataTable getMaintainDT(string pYearMoth, string pEndYearMonth)
        {
            string year = pYearMoth.Substring(0, 4);
            string month = pYearMoth.Substring(4, 2);
            string sdate = year + "-" + month + "-01";
            year = pEndYearMonth.Substring(0, 4);
            month = pEndYearMonth.Substring(4, 2);
            string edate = year + "-" + month + "-31";
            string strsql = @"SELECT START_DATE 開始時間, DEVICE_NO 呼吸器編號, RUNNING_HR 運轉時數, 
                statustxt.P_NAME 狀態, END_DATE 結束時間, systest.P_NAME 機器系統檢測, clnstatus.P_NAME 保養清潔, 
                CASE WHEN MODIFY_NAME IS NULL OR MODIFY_NAME = '' THEN CREATE_NAME ELSE MODIFY_NAME END 人員, REMARK 備註 
                FROM {0} A 
                JOIN {1} systest ON A.SYSTEM_TEST = systest.P_VALUE AND systest.P_MODEL = 'Index_device' AND systest.P_GROUP = 'system_test'
                JOIN {1} clnstatus ON A.CLEAN_STATUS = clnstatus.P_VALUE AND clnstatus.P_MODEL = 'Index_device' AND clnstatus.P_GROUP = 'clean_status'
                JOIN {1} statustxt ON A.STATUS = statustxt.P_VALUE AND statustxt.P_MODEL = 'Index_device' AND statustxt.P_GROUP = 'device_status'
                WHERE A.START_DATE >= '{2}' AND A.START_DATE <= '{3}' ORDER BY A.START_DATE";
            strsql = string.Format(strsql, GetTableName.RCS_VENTILATOR_MAINTAIN, 
                GetTableName.RCS_SYS_PARAMS, sdate, edate);
            DataTable dt = this.DBA.getSqlDataTable(strsql);
            return dt;
        }
        public DataTable getNewMaintainDT()
        {
            string strsql = "SELECT * FROM " + GetTableName.RCS_VENTILATOR_MAINTAIN + " WHERE 1<>1";
            DataTable dt = this.DBA.getSqlDataTable(strsql);
            return dt;
        }
        /// <summary>取得呼吸器檢測紀錄</summary>
        /// <param name="pDeviceNo">呼吸器編號DEVICE_NO</param>
        /// <param name="pSdate">查詢開始日期(yyyy-MM-dd)</param>
        /// <param name="pEdate">查詢結束日期(yyyy-MM-dd)</param>
        /// <returns></returns>
        public List<DeviceChecklist> getVRChecklist(List<string> pDeviceNo, string pSdate, string pEdate)
        {
            List<DeviceChecklist> list = new List<DeviceChecklist>();
            LastError = "";
            try
            {
                pDeviceNo = pDeviceNo.Select(x => string.Format("A.DEVICE_NO = '{0}'", x)).ToList();
                string deviceNo = string.Join(" OR ", pDeviceNo);
                StringBuilder sb = new StringBuilder();
                sb.Append("SELECT B.DEVICE_SEQ, A.CHART_NO, A.CREATE_NAME, A.DETAIL, A.CREATE_DATE FROM ");
                sb.Append(GetTableName.RCS_VENTILATOR_CHECKLIST);
                sb.Append(" A JOIN ");
                sb.Append(GetTableName.RCS_VENTILATOR_SETTINGS);
                sb.Append(" B ON A.DEVICE_NO = B.DEVICE_NO ");
                sb.Append(string.Format(" WHERE ({0})", deviceNo));
                if (!string.IsNullOrEmpty(pSdate))
                    sb.Append(string.Format(" AND SUBSTRING(A.CREATE_DATE, 1,10) >= '{0}'", pSdate));
                if (!string.IsNullOrEmpty(pEdate))
                    sb.Append(string.Format(" AND SUBSTRING(A.CREATE_DATE, 1,10) <= '{0}'", pEdate));
                
                DataTable dt = this.DBA.getSqlDataTable(sb.ToString());
                if (DTNotNullAndEmpty(dt))
                {
                    foreach (DataRow ro in dt.Rows)
                    {
                        DeviceChecklist chklist = new DeviceChecklist();
                        List<string> resultlist = new List<string>();
                        List<JSON_DATA> detailjson = JsonConvert.DeserializeObject<List<JSON_DATA>>(checkDataColumn(ro, "DETAIL"));
                        detailjson.FindAll(x => x.chkd == true).ForEach(x => {
                            resultlist.Add(x.txt);  
                        });
                        chklist.CHART_NO = ro["CHART_NO"].ToString();
                        chklist.DEVICE_SEQ = ro["DEVICE_SEQ"].ToString();
                        chklist.CREATE_NAME = ro["CREATE_NAME"].ToString();
                        chklist.DETAIL = resultlist;
                        chklist.CREATE_DATE = ro["CREATE_DATE"].ToString();
                        list.Add(chklist);
                    }
                }
                if (this.DBA.LastError != "")
                {
                    LastError = this.DBA.LastError;
                    LogTool.SaveLogMessage(this.DBA.LastError, MethodInfo.GetCurrentMethod().Name, MethodInfo.GetCurrentMethod().DeclaringType.Name);
                }
            }
            catch (Exception ex)
            {
                LastError = ex.Message;
                LogTool.SaveLogMessage(ex, MethodInfo.GetCurrentMethod().Name, MethodInfo.GetCurrentMethod().DeclaringType.Name);
            }
            return list;
        }
        #endregion

        #region 使用者管理
        public List<SysParams> getRCS_SYS_PARAMS_GName(string pGrpModel, string pGrpGroup, string pModel = "", string pGroup = "", string pLang = "zh-tw", string pStatus = "", string pManage = "")
        {
            List<string> wSql = new List<string>();
            string whereSql = "";
            List<SysParams> SelectListItem = new List<SysParams>();
            try
            {
                if (!string.IsNullOrEmpty(pModel)) 
                    wSql.Add(string.Format(" A.P_MODEL = {0}", SQLDefend.SQLString(pModel)));
                if (!string.IsNullOrEmpty(pGroup)) 
                    wSql.Add(string.Format(" A.P_GROUP = {0}", SQLDefend.SQLString(pGroup)));
                if (!string.IsNullOrEmpty(pLang))
                {
                    wSql.Add(string.Format(" A.P_LANG = {0}", SQLDefend.SQLString(pLang)));
                    whereSql = " AND B.P_LANG = " + SQLDefend.SQLString(pLang);
                }
                if (!string.IsNullOrEmpty(pStatus)) 
                    wSql.Add(string.Format(" A.P_STATUS = {0}", SQLDefend.SQLString(pStatus)));
                if (!string.IsNullOrEmpty(pManage)) 
                    wSql.Add(string.Format(" A.P_MANAGE = {0}", SQLDefend.SQLString(pManage)));
                string sql = string.Format("SELECT A.*, B.P_NAME P_GROUP_NAME FROM {0} A JOIN {0} B ON B.P_VALUE = A.P_GROUP AND B.P_MODEL = {1} AND B.P_GROUP = {2}{3} WHERE {4} ORDER BY A.P_SORT",
                    GetTableName.RCS_SYS_PARAMS.ToString(), SQLDefend.SQLString(pGrpModel), SQLDefend.SQLString(pGrpGroup)
                    , whereSql, string.Join(" AND ", wSql));
                DataTable dt = this.DBA.getSqlDataTable(sql);
                if (dt != null && dt.Rows.Count > 0) SelectListItem = dt.ToList<SysParams>();
            }
            catch (Exception ex)
            {
                LogTool.SaveLogMessage(ex, MethodInfo.GetCurrentMethod().Name, GetLogToolCS.BaseModel);
            }
            return SelectListItem;
        }
       /// <summary>
        /// 取得使用者清單
       /// </summary>
       /// <param name="isYourSelfExcept">自己是否排除</param>
       /// <returns></returns>
        public List<Dictionary<string, string>> UserMaintain_List(bool isYourSelfExcept)
        {
            List<Dictionary<string, string>> list = new List<Dictionary<string, string>>();
            List<SysParams> _sysList = new List<SysParams>();
            try
            {
                List<SysParams> sysList = this.getRCS_SYS_PARAMS_GName(GetP_MODEL.role.ToString(), "role_type", GetP_MODEL.user.ToString(), @pStatus: "1");
                if (sysList.Exists(x => x.P_VALUE == user_info.user_id) && isYourSelfExcept)
                    sysList.Remove(sysList.Find(x => x.P_VALUE == user_info.user_id));
                _sysList = sysList.FindAll(x => x.P_MANAGE == "0");
                if (RCS.Controllers.BaseController.isDebuggerMode || RCS.Controllers.BaseController.isBasicMode)
                {
                    if (sysList.Exists(x=> x.P_VALUE == "rcs"))
                    {
                        _sysList.Add(sysList.Find(x => x.P_VALUE == "rcs"));
                    }
                }
 
                list = BaseModel.fillListToDictionaryList(_sysList);
                return list;
            }
            catch (Exception ex)
            {
                LogTool.SaveLogMessage(ex, MethodInfo.GetCurrentMethod().Name, GetLogToolCS.SystemManage);
            }
            return list;
        }

        #endregion

        #region  表單管理

        /// <summary>取得表單管理清單</summary>
        /// <param name="P_Model"></param>
        /// <returns></returns>
        public List<Dictionary<string, string>> PhraseMaintain_List(string P_Model)
        {
            string oo = MethodInfo.GetCurrentMethod().DeclaringType.Name;
            try
            {
                List<SysParams> sysList = this.getRCS_SYS_PARAMS_GName(P_Model, "phrase",@pStatus:"1");
                sysList = sysList.Where(x => x.P_GROUP != "phrase").
                    OrderBy(x => x.P_GROUP).ThenBy(x => int.Parse(x.P_SORT)).ToList();
                return BaseModel.fillListToDictionaryList(sysList);
            }
            catch (Exception ex)
            {
                LogTool.SaveLogMessage(ex, MethodInfo.GetCurrentMethod().Name, GetLogToolCS.SystemManage);
            }
            return null;
        }

        #endregion

        /// <summary>刪除</summary>
        /// <param name="DtList">刪除清單</param>
        /// <returns></returns>
        public dbResultMessage SysParams_Del(List<Dictionary<string, string>> DtList)
        {//2016/9/8 從直接刪除改為註記P_STATUS='9'
            dbResultMessage dbMsg = new dbResultMessage();
            try
            {
                DataTable delDT = null;
                if (DtList.Count > 0)
                {
                    foreach (var Dt in DtList)
                    {
                        string SQLStr = string.Format("SELECT * FROM {0} WHERE P_ID = {1}",
                            GetTableName.RCS_SYS_PARAMS, SQLDefend.SQLString(Dt["P_ID"]));
                        DataTable DT = this.DBA.getSqlDataTable(SQLStr);
                        if (DTNotNullAndEmpty(DT))
                        {
                            if (delDT == null) delDT = DT.Clone();
                            delDT.Merge(DT);
                        }
                    }
                    if (DTNotNullAndEmpty(delDT))
                    {
                        foreach (DataRow dr in delDT.Rows)
                            dr["P_STATUS"] = "9";
                        dbMsg = this.DBA.UpdateResult(delDT, GetTableName.RCS_SYS_PARAMS.ToString());
                    }
                }
            }
            catch (Exception ex)
            {
                LogTool.SaveLogMessage(ex, MethodInfo.GetCurrentMethod().Name, MethodInfo.GetCurrentMethod().DeclaringType.Name);
                dbMsg.State = enmDBResultState.Fail;
                dbMsg.dbErrorMessage = ex.Message;
            }
            return dbMsg;
        }

        #region 病患統計(7190)

        /// <summary>
        /// 取得病患統計資料
        /// </summary>
        /// <param name="pWhere">查詢條件</param>
        /// <returns></returns>
        public DataTable getOrderTemp(string pWhere)
        {
            DataTable dt = this.DBA.getDataTable(GetTableName.RCS_ORDER_TEMP.ToString(), pWhere);
            return dt;
        }

        /// <summary>
        /// 取得現有統計暫存資料
        /// </summary>
        /// <param name="pWhere">查詢條件</param>
        /// <returns></returns>
        public int getCountOrderTemp(string pWhere)
        {
            int Cnt = 0;
            string sql = "SELECT COUNT(*) FROM {1} {0}";
            object Obj = this.DBA.ExecuteScalar(string.Format(sql, pWhere, GetTableName.RCS_ORDER_TEMP.ToString()));
            if (Obj != null && Obj.ToString().Trim() != "" && (int)Obj > 0) Cnt = (int)Obj;
            return Cnt;
        }

        /// <summary>
        /// 取一筆未完成資料
        /// </summary>
        /// <param name="pWhere">查詢條件</param>
        /// <returns></returns>
        public DataTable getUnfinishedOrderTemp(string pWhere)
        {
            string sql = "SELECT TOP 1 * FROM {1} {0}";
            DataTable dt = this.DBA.getSqlDataTable(string.Format(sql, pWhere, GetTableName.RCS_ORDER_TEMP.ToString()));
            return dt;
        } 
        /// <summary>
        /// 取得會診資料
        /// </summary>
        /// <returns></returns>
        public Count7190List getCount7190Data(string pSDate, string pEDate)
        {
            Count7190List model = new Count7190List();
            model.RESPONSE_MSG = new RCS_Data.RESPONSE_MSG();
            model.listCount7190 = new List<PatProgress>();
            try
            {
                model.listCount7190 = web_method.get7190OrderList(new RCS.Models.HOSP.HospFactory().webService.RCSConsultList(), pSDate, pEDate);
            }
            catch (Exception ex)
            {
                model.RESPONSE_MSG.status = RESPONSE_STATUS.EXCEPTION;
                model.RESPONSE_MSG.message = "程式錯誤，請洽資訊人員，錯誤訊息如下所示:" + ex.Message;
                LogTool.SaveLogMessage(ex, "getCount7190Data", GetLogToolCS.SystemManage);
            }
            return model;
        }

        #endregion

    }

    public class Count7190List
    {
        /// <summary>
        /// 回傳訊息
        /// </summary>
        public RESPONSE_MSG RESPONSE_MSG { get;set; }

        /// <summary>
        /// 7190清單資料
        /// </summary>
        public List<PatProgress> listCount7190 { get; set; }
        /// <summary>
        /// 7190清單資料
        /// </summary>
        public List<PatOrder> listCount7190Basic { get; set; }
    }

}
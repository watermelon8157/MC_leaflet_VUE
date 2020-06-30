using RCS.Models.ViewModel;
using RCS_Data.Models;
using RCS_Data.Models.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;

namespace RCS
{
    /// <summary>
    ///RCSWebService 的摘要描述
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // 若要允許使用 ASP.NET AJAX 從指令碼呼叫此 Web 服務，請取消註解下列一行。
    // [System.Web.Script.Services.ScriptService]
    public class MCWebService : System.Web.Services.WebService
    {
        string csName = "MCWebService";

        [WebMethod]
        public string HelloWorld()
        {
            return "Hello World";
        }

        // 新增資料
        [WebMethod]
        public string INSERT_PAT_DATA(
string PATIENT_NAME,
string PATIENT_ID,
string LOCATION,
string LATITUDE,
string LONGITUDE,
string AGE,
string GENDER,
string CITY,
string COUNTRY,
string TRIAGE,
string EXPECTED_ARRIVAL_DATETIME,
string LOGIN_DATETIME,
string SELECTION_DATETIME,
string HOSP_KEY,
string HOSPITAL_SHOW_NAME)
        {
            RCS_Data.SQLProvider dblink = new RCS_Data.SQLProvider("MC_DbConnection");
            string SITE_ID = RCS_Data.Models.Function_Library.getDateNowString(RCS_Data.Models.DATE_FORMAT.yyyyMMddHHmmssfffff);
            #region 民眾版 新增  lcoation
            List<DB_MC_SITE_INFO> sList = new List<DB_MC_SITE_INFO>();
            sList.Add(new DB_MC_SITE_INFO()
            {
                SITE_ID = SITE_ID,
                CREATE_DATE = Function_Library.getDateNowString(DATE_FORMAT.yyyy_MM_dd_HHmmss),
                CREATE_ID = "ws",
                CREATE_NAME = "ws",
                MODIFY_DATE = Function_Library.getDateNowString(DATE_FORMAT.yyyy_MM_dd_HHmmss),
                MODIFY_ID = "ws",
                MODIFY_NAME = "ws",
                DATASTATUS = "1",
                LATITUDE = LATITUDE,
                LONGITUDE = LONGITUDE,
            });
            dblink.DBA.DBExecInsert<DB_MC_SITE_INFO>(sList);
            #endregion  
            List<string> msg = new List<string>();
            WSresponse rm = new WSresponse(); 
            if (string.IsNullOrWhiteSpace(PATIENT_NAME))
            {
                msg.Add("請輸入病患姓名!");
            }
            if (msg.Count > 0)
            {
                rm.success = false;
                rm.msg = string.Join(",", msg);
                return Newtonsoft.Json.JsonConvert.SerializeObject(rm);
            } 
            List<DB_MC_PATIENT_INFO> pList = new List<DB_MC_PATIENT_INFO>();
            pList.Add(new DB_MC_PATIENT_INFO()
            {
                PATIENT_ID = PATIENT_ID,
                PATIENT_NAME = PATIENT_NAME,
                SITE_ID = SITE_ID,
                LOCATION = LOCATION,
                AGE = AGE,
                GENDER = GENDER,
                CITY = CITY,
                COUNTRY = COUNTRY,
                TRIAGE = TRIAGE,
                TRANSPORTATION = "",
                AMB_ID = "",
                EXPECTED_ARRIVAL_DATETIME = EXPECTED_ARRIVAL_DATETIME,
                LOGIN_DATETIME = LOGIN_DATETIME,
                SELECTION_DATETIME = SELECTION_DATETIME,
                HOSP_KEY = HOSP_KEY,
                HOSPITAL_SHOW_NAME = HOSPITAL_SHOW_NAME,
                CREATE_ID = "ws",
                CREATE_NAME = "ws",
                CREATE_DATE = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                MODIFY_ID = "ws",
                MODIFY_NAME = "ws",
                MODIFY_DATE = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                DATASTATUS = "1",
            });
            dblink.DBA.DBExecInsert<DB_MC_PATIENT_INFO>(pList);
            if (dblink.DBA.hasLastError)
            {
                rm.success = false;
                rm.msg = dblink.DBA.lastError;
                return Newtonsoft.Json.JsonConvert.SerializeObject(rm);
            }
            if (msg.Count > 0)
            {
                rm.success = false;
                rm.msg = string.Join(",", msg);
                return Newtonsoft.Json.JsonConvert.SerializeObject(rm);
            }
            rm.success = true;
            rm.msg = "success";
            rm.PATIENT_ID = PATIENT_ID;
            return Newtonsoft.Json.JsonConvert.SerializeObject(rm);
        }

        // 些改病人資料
        [WebMethod]
        public string UPDATE_PAT_DATA(string PATIENT_ID,
string PATIENT_NAME,
string AGE,
string GENDER,
string CITY,
string COUNTRY,
string TRIAGE,
string EXPECTED_ARRIVAL_DATETIME,
string LOGIN_DATETIME,
string SELECTION_DATETIME,
string HOSP_KEY,
string HOSPITAL_SHOW_NAME,
string HOSP_TO_PAT_SCORE,
string HOSP_TO_PAT_SCORE_LEVEL)
        {
            List<string> msg = new List<string>();
            WSresponse rm = new WSresponse();
            RCS_Data.SQLProvider dblink = new RCS_Data.SQLProvider("MC_DbConnection");
            if (string.IsNullOrWhiteSpace(PATIENT_ID))
            {
                msg.Add("沒有傷患編號!無法修改!");
            }
            if (msg.Count > 0)
            {
                rm.success = false;
                rm.msg = string.Join(",", msg);
                return Newtonsoft.Json.JsonConvert.SerializeObject(rm);
            }
            List<DB_MC_PATIENT_INFO> pList = dblink.DBA.getSqlDataTable<DB_MC_PATIENT_INFO>(string.Format("SELECT * FROM MC_PATIENT_INFO WHERE PATIENT_ID = '{0}';", PATIENT_ID));
            if (pList.Count > 0)
            {
                pList[0].PATIENT_ID = dblink.GetFixedStrSerialNumber();
                pList[0].PATIENT_NAME = PATIENT_NAME; 
                pList[0].AGE = AGE;
                pList[0].GENDER = GENDER;
                pList[0].CITY = CITY;
                pList[0].COUNTRY = COUNTRY;
                pList[0].TRIAGE = TRIAGE;
                pList[0].EXPECTED_ARRIVAL_DATETIME = EXPECTED_ARRIVAL_DATETIME;
                pList[0].LOGIN_DATETIME = LOGIN_DATETIME;
                pList[0].SELECTION_DATETIME = SELECTION_DATETIME;
                pList[0].HOSP_KEY = HOSP_KEY;
                pList[0].HOSPITAL_SHOW_NAME = HOSPITAL_SHOW_NAME;
                pList[0].MODIFY_ID = "SYS";
                pList[0].MODIFY_NAME = "SYS";
                pList[0].MODIFY_DATE = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                pList[0].DATASTATUS = "1";
                pList[0].HOSP_TO_PAT_SCORE = HOSP_TO_PAT_SCORE;
                pList[0].HOSP_TO_PAT_SCORE_LEVEL = HOSP_TO_PAT_SCORE_LEVEL;
            }

            pList.Add(new DB_MC_PATIENT_INFO()
            {
                PATIENT_ID = dblink.GetFixedStrSerialNumber(),
                PATIENT_NAME = PATIENT_NAME,
                SITE_ID = "9999",
                AGE = AGE,
                GENDER = GENDER,
                CITY = CITY,
                COUNTRY = COUNTRY,
                TRIAGE = TRIAGE,
                TRANSPORTATION = "",
                AMB_ID = "",
                EXPECTED_ARRIVAL_DATETIME = EXPECTED_ARRIVAL_DATETIME,
                LOGIN_DATETIME = LOGIN_DATETIME,
                SELECTION_DATETIME = SELECTION_DATETIME,
                HOSP_KEY = HOSP_KEY,
                HOSPITAL_SHOW_NAME = HOSPITAL_SHOW_NAME,
                CREATE_ID = "SYS",
                CREATE_NAME = "SYS",
                CREATE_DATE = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                MODIFY_ID = "SYS",
                MODIFY_NAME = "SYS",
                MODIFY_DATE = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                DATASTATUS = "1",
                ARRIVAL_FLAG = "",
                GUEST_FLAG = "",
                HOSP_TO_PAT_SCORE = HOSP_TO_PAT_SCORE,
                HOSP_TO_PAT_SCORE_LEVEL = HOSP_TO_PAT_SCORE_LEVEL
            });
            dblink.DBA.DBExecInsert<DB_MC_PATIENT_INFO>(pList);
            if (dblink.DBA.hasLastError)
            {
                rm.success = false;
                rm.msg = dblink.DBA.lastError;
                return Newtonsoft.Json.JsonConvert.SerializeObject(rm);
            }
            if (msg.Count > 0)
            {
                rm.success = false;
                rm.msg = string.Join(",", msg);
                return Newtonsoft.Json.JsonConvert.SerializeObject(rm);
            }
            rm.success = true;
            rm.msg = "success";
            rm.PATIENT_ID = PATIENT_ID;
            return Newtonsoft.Json.JsonConvert.SerializeObject(rm);
        }

        /// <summary>
        /// 取得醫院資料
        /// </summary>
        /// <param name="PATIENT_ID"></param>
        /// <returns></returns>
        [WebMethod]
        public List<DB_MC_PATIENT_INFO> GET_PAT_DATA(string PATIENT_ID)
        {
            List<DB_MC_PATIENT_INFO> patList = new List<DB_MC_PATIENT_INFO>();
            RCS_Data.SQLProvider dblink = new RCS_Data.SQLProvider("MC_DbConnection");
            List<pat_data> pList = new List<pat_data>();
            Dapper.DynamicParameters dp = new Dapper.DynamicParameters();
            string sql = "SELECT * FROM " + DB_TABLE_NAME.DB_MC_PATIENT_INFO + " WHERE PATIENT_ID = @PATIENT_ID AND DATASTATUS = '1'";
            dp.Add("PATIENT_ID", PATIENT_ID);
            patList = dblink.DBA.getSqlDataTable<DB_MC_PATIENT_INFO>(sql, dp);
            return patList;
        }


        /// <summary>
        /// 取得醫院資料
        /// </summary>
        /// <returns></returns>
        [WebMethod]
        public List<DB_MC_HOSP_INFO> GET_HOSP_DATA()
        { 
            return MvcApplication.hospList.ToList();
        }


        /// <summary>
        /// 取得醫院資料 詳細內容
        /// </summary>
        /// <returns></returns>
        [WebMethod]
        public List<VIEW_DB_HOSP_INFO_DTL> GET_HOSP_DATA_DTL(string PATIENT_ID)
        {
            RCS_Data.SQLProvider dblink = new RCS_Data.SQLProvider("MC_DbConnection");
            List<DB_MC_PATIENT_INFO> patList = new List<DB_MC_PATIENT_INFO>();
            Dapper.DynamicParameters dp = new Dapper.DynamicParameters();
            string sql = "SELECT * FROM " + DB_TABLE_NAME.DB_MC_PATIENT_INFO + " WHERE PATIENT_ID = @PATIENT_ID AND DATASTATUS = '1'";
            dp.Add("PATIENT_ID", PATIENT_ID);
            patList = dblink.DBA.getSqlDataTable<DB_MC_PATIENT_INFO>(sql,dp);
            List<VIEW_DB_HOSP_INFO_DTL> pList = new List<VIEW_DB_HOSP_INFO_DTL>();
            
            return pList;  
        }

    }

    public class WSresponse
    { 
        /// <summary>
        /// 是否成功
        /// </summary>
        public bool success { get; set; }
        /// <summary>
        /// 回復訊息
        /// </summary>
        public string msg { get; set; }
        /// <summary>
        /// 病歷號資料
        /// </summary>
        public  string PATIENT_ID { get; set; }
    }
    public class pat_data
    {
        public string success { get; set; }

        public string PATIENT_ID { get; set; }
    }
}

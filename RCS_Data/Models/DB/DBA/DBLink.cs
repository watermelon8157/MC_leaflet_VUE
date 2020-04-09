using Com.Mayaminer;
using Dapper;
using log4net;
using mayaminer.com.jxDB;
using RCS_Data.Models;
using RCS_Data.Models.DB;
using RCSData.App_Config;
using System;
using System.Collections.Generic;
using static Dapper.SqlMapper;
using RCS_Data;
using System.Linq;
using RCSData.Models;
using System.Text;
using System.Security.Cryptography;

namespace RCS_Data
{
    /// <summary>
    /// 資料庫連線設定
    /// </summary>
    public class DBA_Provider : SYS_BASIC, ILogger
    {

        jxDBA _m;
        /// <summary> 連接資料庫 </summary>
        public jxDBA DBA
        {
            get
            {
                if (_m == null)
                {
                    _m = new jxDBA();
                    _m.ConnectionString = IniFile.GetConfig("Connection", "DBAConnStr");
                    //_m.ConnectionString = System.Web.Configuration.WebConfigurationManager.ConnectionStrings["DBconnection"].ConnectionString.ToString();
                }
                return _m;
            }
        }

    }

    /// <summary>
    /// SQL
    /// <para>相關資料庫處理</para>
    /// </summary>
    public class SQLProvider : DBLink
    {
        string csName = "SQLProvider";
        #region 參數

        /// <summary>
        /// 錯誤訊息
        /// </summary>
        public RESPONSE_MSG RESPONSE_MSG
        {
            get
            {
                if (!string.IsNullOrWhiteSpace(this.DBA.RESPONSE_MSG.message))
                {
                    this.DBA.RESPONSE_MSG.status = RESPONSE_STATUS.ERROR;
                }
                return this.DBA.RESPONSE_MSG;
            }
        }

        /// <summary>
        /// 具名參數
        /// </summary>
        public string namedArguments
        {
            get
            {
                switch (this.DBA.dbProvider.enmDBType)
                {
                    case enmDBAProvider.Oracle:
                        return ":";
                    case enmDBAProvider.MS:
                        return "@";
                    default:
                        return "";
                }
            }
        }
        /// <summary>
        /// SQL語法字串
        /// </summary>
        private string sqlStr { get; set; }

        #endregion

        /// <summary>
        /// 建立SQLProvider
        /// <para>MS_SQL :MS_DbConnection</para>
        /// <para>Oracle :Oracle_DbConnection</para>
        /// </summary>
        /// <param name="ConnectionSession">預設:MS_DbConnection</param>
        public SQLProvider(string pConnectionSession) : base(pConnectionSession)
        {

        }

        public SQLProvider() : base("MS_DbConnection")
        {

        }

        /// <summary>
        /// 取得Table流水號(yyyyMMddHHmmssfffff)(如果有傳入值，加入流水號)
        /// </summary>
        /// <param name="user_id">使用者代碼</param>
        /// <param name="pIpdno">批價序號</param>
        /// <returns></returns>
        public string GetFixedStrSerialNumber(string user_id = "", string pIpdno = "")
        {
            // https://www.itread01.com/content/1541375586.html
            // C#生成唯一值的方法彙總
            return DateTime.Now.ToString("yyyyMMddHHmmssfffff") + user_id + pIpdno;
        } 

        /// <summary> 
        /// 取得捷格用流水號對應 16碼
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public string GetJAR_DOC_NO(string text)
        {
            string actionName = "GetJAR_DOC_NO";
            string str = Function_Library.HashString16(text); 
            this.DBA.DBExecInsert<DB_RCS_UPLOAD_JAG_DOC_NO>(
                new List<DB_RCS_UPLOAD_JAG_DOC_NO>() {
                    new DB_RCS_UPLOAD_JAG_DOC_NO() {
                        TEMP_ID =string.Concat(Function_Library.getDateNowString( DATE_FORMAT.yyyyMMddHHmmssfffff),"_",text ),
                        RECORD_ID = text,
                        DOC_NO = str
                    }
            });
            if (this.DBA.hasLastError)
            {
                Com.Mayaminer.LogTool.SaveLogMessage(this.DBA.lastError, actionName, this.csName);
                Com.Mayaminer.LogTool.SaveLogMessage(string.Concat("RECORD_ID:", text), actionName, this.csName);
                Com.Mayaminer.LogTool.SaveLogMessage(string.Concat("DOC_NO:", str), actionName, this.csName);
            }
            return str;
        }

        /// <summary>
        /// 編輯資料並新增暫存
        /// </summary>
        /// <param name="pRECORD_ID"></param>
        /// <param name="pMaterTableName"></param>
        /// <param name="pUser"></param>
        /// <param name="pSQL"></param>
        /// <param name="pDp"></param>
        /// <returns></returns>
        public bool EditTableData(string pRECORD_ID, string pMaterTableName, RCSData.Models.UserInfo pUser, string pSQL, DynamicParameters pDp = null)
        { 
            return InsertTempTable(pRECORD_ID, pMaterTableName, pUser, pSQL, pDp, "2");
        }
        /// <summary>
        /// 刪除資料並新增暫存
        /// </summary>
        /// <param name="pRECORD_ID"></param>
        /// <param name="pMaterTableName"></param>
        /// <param name="pUser"></param>
        /// <param name="pSQL"></param>
        /// <param name="pDp"></param>
        /// <returns></returns>
        public bool DELTableData(string pRECORD_ID, string pMaterTableName, RCSData.Models.UserInfo pUser, string pSQL, DynamicParameters pDp = null)
        {  
            return InsertTempTable(pRECORD_ID, pMaterTableName, pUser, pSQL, pDp,"9");
        }

        /// <summary>
        /// 新增暫存資料
        /// </summary>
        /// <param name="pRECORD_ID"></param>
        /// <param name="pMaterTableName"></param>
        /// <param name="pUser"></param>
        /// <param name="pSQL"></param>
        /// <param name="pDp"></param>
        /// <param name="pDatastatus">9: 刪除,2:歷程</param>
        /// <returns></returns>
        private bool InsertTempTable(string pRECORD_ID, string pMaterTableName, RCSData.Models.UserInfo pUser, string pSQL, DynamicParameters pDp = null , string pDatastatus = "2")
        {
            bool isOK = false;
            List<DB_RCS_TEMP_RECORD_MASTER> tempM = new List<DB_RCS_TEMP_RECORD_MASTER>();
            List<DB_RCS_TEMP_RECORD_DETAIL> tempD = new List<DB_RCS_TEMP_RECORD_DETAIL>();
            string temp_id = string.Concat(pRECORD_ID, DateTime.Now.ToString("_HHmmssfffff"));

            #region 取得 SQL 資料
            this.DBA.Open();
            GridReader gr = this.DBA.dbConnection.QueryMultiple(pSQL, pDp);
            tempM = gr.Read<DB_RCS_TEMP_RECORD_MASTER>().ToList();
            tempD = gr.Read<DB_RCS_TEMP_RECORD_DETAIL>().ToList();
            this.DBA.Close();
            #endregion
            tempM.ForEach(x => {
                x.RECORD_ID = pRECORD_ID;
                x.TEMP_ID = temp_id;
                x.DATASTATUS = pDatastatus;
                x.MODIFY_DATE = RCS_Data.Models.Function_Library.getDateNowString(DATE_FORMAT.yyyy_MM_dd_HHmmss);
                x.MODIFY_ID = pUser.user_id;
                x.MODIFY_NAME = pUser.user_name;
            });
            tempD.ForEach(x => {
                x.RECORD_ID = pRECORD_ID;
                x.TEMP_ID = temp_id;
            });
            this.DBA.BeginTrans();
            #region 寫入暫存資料表 
            this.DBA.DBExecInsert(tempM);
            this.DBA.DBExecInsert(tempD);
            #endregion 
            if (this.DBA.hasLastError)
            {
                this.DBA.Rollback();
            }
            else
            {
                isOK = true;
                this.DBA.Commit();
            }
            this.DBA.Close();
            return isOK;
        }

        /// <summary>
        /// 新增共用顯示 Json資料
        /// RESPONSE_MSG rm =  dbLin.Insert_JSONData("record_id", pList);
        /// </summary>
        /// <param name="record_id"></param>
        /// <param name="pList"></param>
        /// <returns></returns>
        public RESPONSE_MSG Insert_JSONData(string record_id, List<DB_RCS_RT_RECORD_JSON> pList)
        {
            RESPONSE_MSG rm = new RESPONSE_MSG();  
            List<DB_RCS_RT_RECORD_JSON_BY_RECORD_ID> _list = this.Select_JSONData<DB_RCS_RT_RECORD_JSON_BY_RECORD_ID>(record_id);
            if (_list.Count > 0)
            {
                this.DBA.DBExecDelete<DB_RCS_RT_RECORD_JSON_BY_RECORD_ID>(_list);
            }
            if (!this.DBA.hasLastError)
            {
                pList.ForEach(x=>x.RECORD_ID = record_id);
                this.DBA.DBExecInsert<DB_RCS_RT_RECORD_JSON>(pList);
            } 
            if (this.DBA.hasLastError)
            {
                rm.status = RESPONSE_STATUS.ERROR;
                rm.message = this.DBA.lastError;
                LogTool.SaveLogMessage("Insert_JSONData", this.csName);
            }
            return rm;
        }
        /// <summary>
        /// 取得共用顯示 Json資料
        /// List<DB_RCS_RT_RECORD_JSON> pList = dbLin.Select_JSONData<DB_RCS_RT_RECORD_JSON>("record_id");
        /// </summary>
        /// <param name="record_id"></param>
        /// <returns></returns>
        public List<T> Select_JSONData<T>(string record_id)
        { 
            string sql = "";
            Dapper.DynamicParameters dp = new DynamicParameters();
            dp.Add("RECORD_ID", record_id);
            sql = string.Concat("SELECT * FROM ", DB_TABLE_NAME.DB_RCS_RT_RECORD_JSON, " WHERE RECORD_ID = @RECORD_ID");
            List<T> _list = this.DBA.getSqlDataTable<T>(sql, dp);
            return _list;
        }


        /// <summary>
        /// 新增暫存Master
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="idb"></param>
        /// <param name="pat_info"></param>
        /// <param name="user_info"></param>
        /// <returns></returns>
        public string InsertMaster<T>(RCS_Data.Models.DB.IDBA<T> idb,IPDPatientInfo pat_info, UserInfo user_info, string record_date)
        {
            string record_id = this.GetFixedStrSerialNumber(user_info.user_id,pat_info.ipd_no);
            this.DBA.DBExecInsert<T>(idb.InsertTenpMaster(record_id, record_date, pat_info, user_info));
            if (this.DBA.hasLastError)
            {
                record_id = "";
                LogTool.SaveLogMessage(this.DBA.lastError, "InsertMaster", this.csName);
            }
            return record_id;
        }


    }


    /// <summary>
    /// 資料庫連線，useDapper
    /// </summary>
    public class DBLink
    {
        private ILog logger
        {
            get
            {
                if (this._logger == null)
                    this._logger = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
                return this._logger;
            }
        }
        private ILog _logger { get; set; }

        private DBConnection _settiing { get; set; }
        private DBConnection settiing
        {
            get
            {
                if (this._settiing == null)
                {
                    this._settiing = new DBConnection();
                }
                return this._settiing;
            }
        }
        /// <summary>
        /// 資料庫連線
        /// </summary>
        protected string ConnectionSession { get; set; }

        /// <summary> DB Object, 對 DB 的操作都由此 Object 處理 </summary>
        public DBConnector DBA { get; set; }

        /// <summary>
        /// 建立資料庫連線
        /// <para>MS_SQL :MS_DbConnection</para>
        /// <para>Oracle :Oracle_DbConnection</para>
        /// </summary>
        /// <param name="ConnectionSession">預設:MS_DbConnection</param>
        public DBLink(string pConnectionSession)
        {
            this.ConnectionSession = pConnectionSession;
            this.DBConnection();
        }

        public DBLink()
        {
            this.ConnectionSession = this.settiing.session;
            this.DBConnection();
        }

        private void DBConnection()
        {
            try
            {
                if (!string.IsNullOrWhiteSpace(this.ConnectionSession))
                {
                    //取得資料庫連線字串
                    //1.DBProvider
                    //2.DBConnStr
                    string db_base = this.settiing.getDBProvider(this.ConnectionSession).Trim()
                    , strConn = this.settiing.getDBAConnStr(this.ConnectionSession).Trim();

                    //initial db object.
                    switch (db_base)
                    {
                        case "Oracle":
                            DBA = new OracleDBConnector(strConn);
                            break;
                        case "MS":
                            DBA = new MSSQLDBConnector(strConn);
                            break;
                        default:
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                this.logger.Error(ex);
            }
        }


    }
}

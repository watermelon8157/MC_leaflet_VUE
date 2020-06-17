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

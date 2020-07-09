using System;
using System.Collections.Generic;
using Com.Mayaminer;
using Newtonsoft.Json;
using System.Data;
using log4net;
using System.IO;
using RCSData.Models;
using System.Reflection;

namespace RCS_Data.Models
{
    /// <summary>
    /// 基本方法
    /// </summary>
    public class BasicFunction : BASIC_PARAMS
    {
        private string csName { get { return "BasicFunction"; } }

          
       
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
  


    }
}

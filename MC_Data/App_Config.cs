using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RCSData.App_Config
{

    /// <summary>
    /// 系統設定
    /// </summary>
    public class System
    {
        private string session { get { return "System"; } }
        /// <summary>
        /// 連線方式
        /// </summary>
        public string DBConnection
        {
            get
            {
                return Com.Mayaminer.IniFile.GetConfig(this.session, "DBConnection");
            }
        }
        /// <summary>
        /// 介接WS
        /// </summary>
        public string WebServcie
        {
            get
            {
                return Com.Mayaminer.IniFile.GetConfig(this.session, "WebServcie");
            }
        }
        /// <summary>
        /// 語系
        /// </summary>
        public string Language
        {
            get
            {
                return Com.Mayaminer.IniFile.GetConfig(this.session, "Language");
            }
        }
        /// <summary>
        /// 醫院抬頭
        /// </summary>
        public string setHospName
        {
            get
            {
                return Com.Mayaminer.IniFile.GetConfig(this.session, "setHospName");
            }
        }
        /// <summary>
        /// 是否是測試網站 (true:是測試網站,false:不是測試網站)
        /// </summary>
        public bool isTestWeb
        {
            get
            {
                return bool.Parse(Com.Mayaminer.IniFile.GetConfig(this.session, "isTestWeb"));
            }
        }
        /// <summary>
        /// 是否使用測試資料  (true:開啟,false:關閉)
        /// </summary>
        public bool useTestData
        {
            get
            {
                return bool.Parse(Com.Mayaminer.IniFile.GetConfig(this.session, "useTestData"));
            }
        }
        /// <summary>
        /// 是否是馬雅標準版模式開關  (true:開啟,false:關閉)
        /// </summary>
        public bool isBasicMode
        {
            get
            {
                return bool.Parse(Com.Mayaminer.IniFile.GetConfig(this.session, "isBasicMode"));
            }
        }
        /// <summary>
        /// 是否使用Debugger模式  (true:開啟,false:關閉)
        /// </summary>
        public bool isDebuggerMode
        {
            get
            {
                return bool.Parse(Com.Mayaminer.IniFile.GetConfig(this.session, "isDebuggerMode"));
            }
        }
        /// <summary>
        /// token Encryption 加密
        /// </summary>
        public bool tokenEncryption
        {
            get
            {
                return bool.Parse(Com.Mayaminer.IniFile.GetConfig(this.session, "tokenEncryption"));
            }
        }
        /// <summary>
        /// token Secret
        /// </summary>
        public string tokenSecret
        {
            get
            {
                if (this.tokenEncryption)
                {
                    //如果有加密

                }
                return Com.Mayaminer.IniFile.GetConfig(this.session, "tokenSecret");
            }
        }
    }
    /// <summary>
    /// 連線設定
    /// </summary>
    public class DBConnection
    {
        private System _system { get; set; }
        /// <summary>
        /// 系統DB連線預設 session
        /// </summary>
        public string session
        {
            get
            {
                if (this._system == null)
                {
                    this._system = new System();
                }
                return this._system.DBConnection;
            }
        }
        /// <summary>
        /// 連線方式
        /// </summary>
        public string DBProvider
        {
            get
            {
                return Com.Mayaminer.IniFile.GetConfig(this.session, "DBProvider");
            }
        }

        /// <summary>
        /// 連線方式
        /// </summary>
        public string getDBProvider(string pSession)
        {
            return Com.Mayaminer.IniFile.GetConfig(pSession, "DBProvider");
        }

 
        /// <summary>
        /// 連線方式
        /// </summary>
        public string getDBAConnStr(string pSession)
        {
            return Com.Mayaminer.IniFile.GetConfig(pSession, "DBConnStr");
        }
    }

    public class WebServcie
    {
        public WebServcie(string pSession)
        {
            this.session = pSession;
            if (string.IsNullOrWhiteSpace(this.session))
            {
                this.session = new System().WebServcie;
            }
        }
        /// <summary>
        /// 系統預設WS 連線
        /// </summary>
        public string session { get; set; }

        /// <summary>
        /// WebServiceUrl
        /// </summary>
        public string WebServiceUrl
        {
            get
            {
                return Com.Mayaminer.IniFile.GetConfig(this.session, "WebServiceUrl");
            }
        }
        /// <summary>
        /// SoapActionUrl
        /// </summary>
        public string SoapActionUrl
        {
            get
            {
                return Com.Mayaminer.IniFile.GetConfig(this.session, "SoapActionUrl");
            }
        }
        /// <summary>
        /// ResponseFormat 格式 (1:取得string,2:取得Byte,3:取得dataTable)
        /// </summary>
        public string ResponseFormat
        {
            get
            {
                return Com.Mayaminer.IniFile.GetConfig(this.session, "ResponseFormat");
            }
        }

        /// <summary>
        /// WebServiceUrl
        /// </summary>
        public string getWebServiceUrl(string pSession)
        {
            return Com.Mayaminer.IniFile.GetConfig(pSession, "WebServiceUrl");
        }
        /// <summary>
        /// SoapActionUrl
        /// </summary>
        public string getSoapActionUrl(string pSession)
        {
            return Com.Mayaminer.IniFile.GetConfig(pSession, "SoapActionUrl");
        }
        /// <summary>
        /// ResponseFormat 格式 (1:取得string,2:取得Byte,3:取得dataTable)
        /// </summary>
        public string getResponseFormat(string pSession)
        {
            return Com.Mayaminer.IniFile.GetConfig(pSession, "ResponseFormat");
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RCS_Data
{
    #region 回傳訊息

    public enum RESPONSE_STATUS
    {
        SUCCESS = 0,
        ERROR = 1,
        EXCEPTION = 2,
        DUPLICATE = 3,
        /// <summary>
        /// 提醒 , 注意
        /// </summary>
        WARN = 4
    }

    public enum RESPONSE_ACTION
    {
        INSERT = 0,
        UPDATE = 1
    }

    /// <summary> 回傳值 </summary>
    public class RESPONSE_MSG
    {
        public RESPONSE_MSG()
        {
            this.messageList = new List<string>();
            this.message = "";
        }

        /// <summary> 處理狀態 </summary>
        public RESPONSE_STATUS status { set; get; }

        /// <summary> 傳回訊息或內容 </summary>
        public string message { set; get; }

        public List<string> messageList { set; get; }

        public bool hasLastError { get { return !string.IsNullOrWhiteSpace(this.message) || this.messageList.Count > 0; } }

        public string lastError
        { get
            { 
                if (this.messageList.Count > 0)
                {
                    return string.Join(",", this.messageList);
                }
                return this.message;
            }
        }

        /// <summary> 附帶物件 </summary>
        public object attachment { set; get; }

        public void setErrorMsg(string msgStr)
        {
            this.status = RESPONSE_STATUS.ERROR;
            this.message = msgStr;
        }
 
        /// <summary> 取得序列化結果 </summary>
        public string get_json()
        {
            return Newtonsoft.Json.JsonConvert.SerializeObject(this);
        }
    }

    public class RESPONSE_MSGLIST : RESPONSE_MSG
    { 
    
    }

    public class RESPONSE_MSG<T>
    {
        public RESPONSE_MSG()
        {
            this.messageList = new List<string>();
            this.message = "";
        }

        /// <summary> 處理狀態 </summary>
        public RESPONSE_STATUS status { set; get; }

        /// <summary> 傳回訊息或內容 </summary>
        public string message { set; get; }

        public List<string> messageList { set; get; }

        public bool hasLastError { get { return !string.IsNullOrWhiteSpace(this.message) || this.messageList.Count > 0; } }

        public string lastError
        {
            get
            {
                if (this.messageList.Count > 0)
                {
                    return string.Join(",", this.messageList);
                }
                return this.message;
            }
        }

        /// <summary> 附帶物件 </summary>
        public T attachment { set; get; }

        public void setErrorMsg(string msgStr)
        {
            this.status = RESPONSE_STATUS.ERROR;
            this.message = msgStr;
        }

        /// <summary> 取得序列化結果 </summary>
        public string get_json()
        {
            return Newtonsoft.Json.JsonConvert.SerializeObject(this);
        }
    }

    #endregion

    #region ErrorMsg

    public class ERROR_MSG
    {
        public string label { get; set; }

        public string actionName { get; set; }

        public string msgType { get; set; }

        public string controllerName { get; set; }

        public string description { get; set; }

        public Exception exception { get; set; }

        public bool showBtn { get; set; }

        public int httpCode { get; set; }
    }

    /// <summary> 連線至 DB 時發生的錯誤 </summary>
    public class DBException : Exception
    {

        public DBException(Exception innerException)
            : base($"Connect to db failed.", innerException) { }

    }


    /// <summary> 使用驗證碼登入時發生的錯誤 </summary>
    public class VuelidateException : Exception
    {

        public string RedirectURL { get; private set; }

        public VuelidateException(string Message, string RedirectURL)
            : base(Message)
        {
            this.RedirectURL = RedirectURL;
        }

    }

    #endregion
}
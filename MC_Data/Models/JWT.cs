using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RCSData.Models
{
    /// <summary>
    /// 繼承使用者資料
    /// </summary>
    public abstract class AUTH
    {
        /// <summary>
        /// 登入者資料
        /// </summary>
        public PAYLOAD payload { get; set; }

       
    }

    public class PAYLOAD
    {
        public string hosp_id { get; set; }
        public string site_id { get; set; }
        public string site_desc { get; set; }
        /// <summary>
        /// 使用者帳號
        /// </summary>
        public string user_id { get; set; }
        /// <summary>
        /// 使用者姓名
        /// </summary>
        public string user_name { get; set; }
        /// <summary>
        /// 使用者角色
        /// </summary>
        public string role { get; set; } 
        /// <summary>
        /// token 創建時間 yyyy/MM/dd HH:mm:ss
        /// </summary>
        public string iat { get; set; }

    }

    public class UserInfo
    {
        public string user_id { get; set; }
        public string user_name { get; set; }
    }

    public class IPDPatientInfo
    {

    }
}
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

        /// <summary>
        /// 使用者資料
        /// </summary>
        public UserInfo user_info
        {
            get
            {
                if (this.payload == null)
                {
                    return new UserInfo();
                } 
                 return new UserInfo() {
                    user_id = this.payload.user_id,
                    user_name = this.payload.user_name,
                    authority = this.payload.role,
                };

            }

        }

        /// <summary>
        /// 病患資料
        /// </summary>
        public IPDPatientInfo pat_info { get; set; }
    }

    public class PAYLOAD
    {
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
}
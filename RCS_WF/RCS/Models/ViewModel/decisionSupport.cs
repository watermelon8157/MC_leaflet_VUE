using RCS_Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace RCS.Models
{
    /// <summary>
    /// 決策資源
    /// </summary>
    public class DecisionSupport
    {

    }
    public class DecisionSupportViewModel
    {
        public DecisionSupportViewModel()
        {

        }
        /// <summary>
        /// 決策資源設定資料
        /// </summary>
        public SysParamCollection DS_SysParams { get; set;}


    }


    public class RCS_SYS_DECISION_SUPPORT_MASTER : DB_RCS_SYS_DECISION_SUPPORT_MASTER
    {
        /// <summary>
        /// 險是權重判斷
        /// </summary>
        public string SHOW_IDEA
        {
            get
            {
                string str = "未知";
                switch (DS_IDEA)
                {
                    case "1":
                        str = "大於權重總分";
                        break;
                    case "2":
                        str = "小於權重總分";
                        break;
                    default:
                        break;
                }
                return str;
            }
        }

        /// <summary>
        /// 顯示狀態
        /// </summary>
        public string SHOW_STATUS
        {
            get
            {
                string str = "未知";
                switch (DS_STATUS)
                {
                    case "1":
                        str = "使用中";
                        break;
                    case "9":
                        str = "停用";
                        break;
                    default:
                        break;
                }
                return str;
            }
        }

        /// <summary>
        /// 判斷項目
        /// </summary>
        public List<RCS_SYS_DECISION_SUPPORT_DETAIL> DETAIL { get; set;}
    }

    public class RCS_SYS_DECISION_SUPPORT_DETAIL: DB_RCS_SYS_DECISION_SUPPORT_DETAIL
    {

    }
}
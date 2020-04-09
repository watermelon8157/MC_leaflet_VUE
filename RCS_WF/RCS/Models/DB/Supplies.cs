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

namespace RCS.Models.DB
{
    /// <summary>
    /// 耗材清單
    /// </summary>
    public class RCS_SYS_CONSUMABLE_LIST
    {
        public string C_ID { get; set; }
        public string C_CNAME { get; set; }
        public string C_ENAME { get; set; }
        public string VENDER_CODE { get; set; }
        public string C_MEMO { get; set; }
        public string CREATE_ID { get; set; }
        public string CREATE_NAME { get; set; }
        public string CREATE_DATE { get; set; }
        public string MODIFY_ID { get; set; }
        public string MODIFY_NAME { get; set; }
        public string MODIFY_DATE { get; set; }
        public string DATASTATUS { get; set; }

    }
    public class RCS_SYS_CONSUMABLE_PACKAGE_MASTER
    {
        public string CM_ID { get; set; }
        public string CM_CNAME { get; set; }
        public string C_MEMO { get; set; }
        public string CREATE_ID { get; set; }
        public string CREATE_NAME { get; set; }
        public string CREATE_DATE { get; set; }
        public string MODIFY_ID { get; set; }
        public string MODIFY_NAME { get; set; }
        public string MODIFY_DATE { get; set; }
        public string DATASTATUS { get; set; }

    }
    public class RCS_SYS_CONSUMABLE_PACKAGE_DTL
    {
        public string CM_ID { get; set; }
        public string C_ID { get; set; }
        public string C_QTY { get; set; }
        public string DATASTATUS { get; set; }

    }
    public class CM_RCS_SYS_CONSUMABLE_PACKAGE_DTL: RCS_SYS_CONSUMABLE_PACKAGE_DTL
    {
        public List<RCS_SYS_CONSUMABLE_PACKAGE_DTL> DETAIL { get; set; }

    }

}
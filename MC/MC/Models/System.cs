using System.Collections.Generic;

/// <summary>
/// 系統基本物件
/// </summary>
namespace RCS_Data
{
     
    #region 授權清單

    public class RCS_DATA_RtGrantList : BASE_DATA
    {
        /// <summary>
        /// 流水號
        /// </summary>
        public string tempid { get; set; }
        /// <summary>
        /// 表單名稱
        /// </summary>
        public string recordName { get; set; }
        /// <summary>
        /// 表單記錄/評估時間
        /// </summary>
        public string recordTime { get; set; }
        /// <summary>
        /// 表單id
        /// </summary>
        public string recordid { get; set; }
        /// <summary>
        /// 病歷號
        /// </summary>
        public string chid { get; set; }
        /// <summary>
        /// 批價序號
        /// </summary>
        public string ipd_no { get; set; }
        /// <summary>
        /// 病患姓名
        /// </summary>
        public string pat_name { get; set; }
        /// <summary>
        /// 授權學姊
        /// </summary>
        public string op_id { get; set; }
        /// <summary>
        /// 授權狀態 0:未授權 1:已授權
        /// </summary>
        public string status { get; set; }
        /// <summary>
        /// 備註
        /// </summary>
        public string memo { get; set; }
        /// <summary>
        /// 未授權webserviceTempid
        /// </summary>
        public string wsTempid { get; set; }
        /// <summary>
        /// 未授權webservice回傳訊息
        /// </summary>
        public string wsMsg { get; set; }
        /// <summary>
        /// 授權webserviceTempid
        /// </summary>
        public string wsTempid_RtGrant { get; set; }
        /// <summary>
        /// 授權webservice回傳訊息
        /// </summary>
        public string wsMsg_RtGrant { get; set; } 
        /// <summary>
        /// 性別
        /// </summary>
        public string gender { get; set; }
    }

    #endregion

    public class RCS_SYS_TEST
    {
        public string TEST_ID { get; set; }

        public string TEST_VALUE { get; set; }
    }

     
}
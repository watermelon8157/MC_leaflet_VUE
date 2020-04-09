using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;

namespace RCS_Data.Models.DB
{
    

    public static partial class DB_TABLE_NAME
    {
        public static string DB_TCRecord { get { return Function_Library.GetClassDisplayName<DB_TCRecord>(); } }

    }

    /// <summary>
    /// SELECT 
    /// PGUID, varchar(64)  (要串字串編碼 W-批價序號-員工編號--14141F93A35463-CFB6) 後面數字請再自己組合
    /// RefPatient, (批價序號)
    /// RecordDate, (回覆日期 EX：2020/01/25　)
    /// RecordTime, (回覆時間 EX：13:41  24小時時間制)
    /// RecordContent, (回覆內容)
    /// TeamGroupID, (固定填寫：0320    補充說明：呼吸治療室: 0320)
    /// MessageType, (空)
    /// MessageType2,(空)
    /// Creator, (員工編號)
    /// CreatorName, (員工姓名)
    /// CreateTime, (更新日期 EX：2020/01/25 13:41:37)
    /// Editor,(員工編號)
    /// EditorName,(員工姓名)
    /// EditTime(編輯日期 EX：2020/01/25 13:41:37)
    /// FROM TCRecord
    /// </summary>
    [DisplayName("TCRecord")]
    public class DB_TCRecord
    {
        [Key]
        public string PGUID { get; set; }
        public string RefPatient { get; set; }
        public string RecordDate { get; set; }
        public string RecordTime { get; set; }
        public string RecordContent { get; set; }
        public string TeamGroupID { get; set; }
        public string MessageType { get; set; }
        public string MessageType2 { get; set; }
        public string Creator { get; set; }
        public string CreatorName { get; set; }
        public string CreateTime { get; set; }
        public string Editor { get; set; }
        public string EditorName { get; set; }
        public string EditTime { get; set; }  
    }

}

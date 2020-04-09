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
        public static string DB_RCS_SYS_CONSUMABLE_LIST { get { return Function_Library.GetClassDisplayName<DB_RCS_SYS_CONSUMABLE_LIST>(); } }

    }

    [DisplayName("RCS_SYS_CONSUMABLE_LIST")]

    public class DB_RCS_SYS_CONSUMABLE_LIST
    {
        [Key]
        public string C_ID { get; set; }
        [Key]
        public string C_TYPE { get; set; }
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
        public string ORDERBY { get; set; }
        public string ORDERBY2 { get; set; }


    }
}

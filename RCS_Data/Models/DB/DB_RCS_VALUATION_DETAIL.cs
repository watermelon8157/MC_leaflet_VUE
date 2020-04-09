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
        public static string DB_RCS_VALUATION_DETAIL { get { return Function_Library.GetClassDisplayName<DB_RCS_VALUATION_DETAIL>(); } }

    }

    [DisplayName("RCS_VALUATION_DETAIL")]

    public class DB_RCS_VALUATION_DETAIL
    {
        [Key]
        public string V_SUB_ID { get; set; }
        [Key]
        public string ITEM_NAME { get; set; }
        public string ITEM_VALUE { get; set; }

    }

    public class DB_RCS_VALUATION_DETAIL_BY_V_SUB_ID
    {
        [Key]
        public string V_SUB_ID { get; set; }
        public string ITEM_NAME { get; set; }
        public string ITEM_VALUE { get; set; }
    }
}

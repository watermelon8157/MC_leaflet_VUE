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
        /// <summary>
        /// DB_MC_HOSP_INFO_DTL
        /// </summary>
        public static string DB_MC_SITE_INFO { get { return Function_Library.GetClassDisplayName<DB_MC_SITE_INFO>(); } }
    }

    [DisplayName("MC_SITE_INFO")]

    public class DB_MC_SITE_INFO : T4Json
    {
        [Key]
        public string SITE_ID { get; set; }
        public string SITE_DESC { get; set; }
        public string CREATE_ID { get; set; }
        public string CREATE_NAME { get; set; }
        public string CREATE_DATE { get; set; }
        public string MODIFY_ID { get; set; }
        public string MODIFY_NAME { get; set; }
        public string MODIFY_DATE { get; set; }
        public string DATASTATUS { get; set; }
        public string LATITUDE { get; set; }
        public string LONGITUDE { get; set; }






    }
}

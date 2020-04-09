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
        /// DB_MC_HOSP_INFO
        /// </summary>
        public static string DB_MC_HOSP_INFO { get { return Function_Library.GetClassDisplayName<DB_MC_HOSP_INFO>(); } }
    }

    [DisplayName("MC_HOSP_INFO")]

    public class DB_MC_HOSP_INFO : T4Json
    {
        [Key]
        public string HOSP_KEY { get; set; }
        public string FULL_NAME { get; set; }
        public string SHOW_NAME { get; set; }
        public string LATITUDE { get; set; }
        public string LONGITUDE { get; set; }
        public string NHI_REGIONAL_DIVISION { get; set; }
        public string CITY_COUNTRY { get; set; }
        public string ERH { get; set; }
        public string ERH_LEVEL { get; set; }
        public string NEW_RANKING { get; set; }
        public string PARAMETER_SEV { get; set; }
        public string PARAMETER_MOD { get; set; }
        public string PARAMETER_MILD { get; set; }
        public string BED_EDOBSERV { get; set; }
        public string BED_ACUTEBEDS { get; set; }
        public string ERH_URL { get; set; }
        public string CREATE_ID { get; set; }
        public string CREATE_NAME { get; set; }
        public string CREATE_DATE { get; set; }
        public string MODIFY_ID { get; set; }
        public string MODIFY_NAME { get; set; }
        public string MODIFY_DATE { get; set; }
        public string DATASTATUS { get; set; }


    }
}

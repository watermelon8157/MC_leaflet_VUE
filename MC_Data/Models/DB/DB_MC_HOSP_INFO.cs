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
        public string HOSP_NAME { get; set; }
        public string SHOW_NAME { get; set; }
        public string CITY { get; set; }
        public string NEW_RANKING { get; set; }
        public string ERH_URL { get; set; }
        public string CREATE_ID { get; set; }
        public string CREATE_NAME { get; set; }
        public string CREATE_DATE { get; set; }
        public string MODIFY_ID { get; set; }
        public string MODIFY_NAME { get; set; }
        public string MODIFY_DATE { get; set; }
        public string DATASTATUS { get; set; }
        public string LATITUDE { get; set; }
        public string LONGITUDE { get; set; }
        public string ID { get; set; }
        public string MOHWDIVISION { get; set; }
        public string CONTYPE { get; set; }
        public string ORIGINAL_RANKING { get; set; }
        public string SEVERE { get; set; }
        public string MODERATE { get; set; }
        public string MILD { get; set; }
        public string EDOBSERVBEDS { get; set; }
        public string EDRELATEDBEDS { get; set; }
        public string MEDICALSTAFF { get; set; }
        public string W2 { get; set; }
        public string CV { get; set; }
        public string DIVISION { get; set; }


    }


    public class VIEW_MC_HOSP_INFO : DB_MC_HOSP_INFO
    {
        public string hosp_desc { get; set; }
        public string hosp_name { get; set; }
        public string hosp_class { get; set; }
        public string hosp_city { get; set; }  
        public string hosp_injury { get; set; }
        public string hosp_ranking { get; set; }
        public string hosp_erbed { get; set; }
        public string hosp_source { get; set; }
        public string hosp_ihp { get; set; }
        public string hosp_whp { get; set; }
        public List<double> location { get { return new List<double>() { double.Parse(this.LATITUDE), double.Parse(this.LONGITUDE) } ; } }
    }

    public class MC_HOSP_INFO
    {
        public string HOSP_KEY { get; set; }
        public string HOSP_NAME { get; set; }
        public string CITY { get; set; }
        public string NEW_RANKING { get; set; } 
        public string LATITUDE { get; set; }
        public string LONGITUDE { get; set; }  
        public string ORIGINAL_RANKING { get; set; }
        public string SEVERE { get; set; }
        public string MODERATE { get; set; }
        public string MILD { get; set; }
        public string EDOBSERVBEDS { get; set; }
        public string EDRELATEDBEDS { get; set; }
        public string MEDICALSTAFF { get; set; }
        public int ARRIVAL_PATIENT { get; set; }
        public int SELECT_PATIENT { get; set; }
        public int ALL_PATIENT { get; set; }
        public string W2 { get; set; }
        public string CV { get; set; }
    }
}

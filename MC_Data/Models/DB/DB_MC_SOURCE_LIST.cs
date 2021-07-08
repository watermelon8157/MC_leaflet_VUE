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
        /// DB_MC_SOURCE_LIST
        /// </summary>
        public static string DB_MC_SOURCE_LIST { get { return Function_Library.GetClassDisplayName<DB_MC_SOURCE_LIST>(); } }
    }

    [DisplayName("MC_SOURCE_LIST")]

    public class DB_MC_SOURCE_LIST : T4Json
    {
        [Key]
        public string HOSP_KEY { get; set; }

        [Key]
        public string SITE_ID { get; set; } 
        public string CREATE_ID { get; set; }
        public string CREATE_NAME { get; set; }
        public string CREATE_DATE { get; set; }
        public string MODIFY_ID { get; set; }
        public string MODIFY_NAME { get; set; }
        public string MODIFY_DATE { get; set; }
        public string DATASTATUS { get; set; }
        public string DRIVING_SOURCE { get; set; }
        public string W2 { get; set; }
        public string CV { get; set; }
        public string SEVERE { get; set; }
        public string MODERATE { get; set; }
        public string MILD { get; set; }
        public string SEVERE_SOURCE { get; set; } 
        public string MODERATE_SOURCE { get; set; } 
        public string MILD_SOURCE { get; set; } 



    }
    public class VIEW_MC_SOURCE_LIST : DB_MC_SOURCE_LIST
    {

        public double SEVERE_S { get { return double.Parse(this.SEVERE_SOURCE); } }
        public double MODERATE_S { get { return double.Parse(this.MODERATE_SOURCE); } }
        public double MILD_S { get { return double.Parse(this.MILD_SOURCE); } }
         
        public string SEVERE_LEVEL { get; set; }
        public string MODERATE_LEVEL { get; set; }
        public string MILD_LEVEL { get; set; }
    }

    public class MC_SOURCE_LIST
    {
        [Key]
        public string HOSP_KEY { get; set; }  
        public string DRIVING_SOURCE { get; set; }
        public string W2 { get; set; }
        public string CV { get; set; }
        public string SEVERE { get; set; }
        public string MODERATE { get; set; }
        public string MILD { get; set; }
        public string SEVERE_SOURCE { get; set; }
        public string MODERATE_SOURCE { get; set; }
        public string MILD_SOURCE { get; set; }
    }

}

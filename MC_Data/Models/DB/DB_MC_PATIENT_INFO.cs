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
        /// DB_MC_PATIENT_INFO
        /// </summary>
        public static string DB_MC_PATIENT_INFO { get { return Function_Library.GetClassDisplayName<DB_MC_PATIENT_INFO>(); } }
    }

    [DisplayName("MC_PATIENT_INFO")]

    public class DB_MC_PATIENT_INFO: T4Json
    {
        [Key]
        public string PATIENT_ID { get; set; }
        public string PATIENT_NAME { get; set; }
        [Key]
        public string SITE_ID { get; set; }
        public string LOCATION { get; set; }
        public string AGE { get; set; }
        public string GENDER { get; set; }
        public string COUNTRY { get; set; }
        public string TRIAGE { get; set; }
        public string TRANSPORTATION { get; set; }
        public string AMB_ID { get; set; }
        public string EXPECTED_ARRIVAL_DATETIME { get; set; }
        public string LOGIN_DATETIME { get; set; }
        public string SELECTION_DATETIME { get; set; }
        public string HOSP_ID { get; set; }
        public string HOSPITAL_SHOW_NAME { get; set; }
        public string CREATE_ID { get; set; }
        public string CREATE_NAME { get; set; }
        public string CREATE_DATE { get; set; }
        public string MODIFY_ID { get; set; }
        public string MODIFY_NAME { get; set; }
        public string MODIFY_DATE { get; set; }
        public string DATASTATUS { get; set; }
        public string ARRIVAL_FLAG { get; set; }
        public string GUEST_FLAG { get; set; }
        public string SCORE { get; set; }
        public string CITY { get; set; }
        public string HOSP_KEY { get; set; }
        public string HOSP_TO_PAT_SCORE { get; set; }
        public string HOSP_TO_PAT_SCORE_LEVEL { get; set; }



    }

    public class MC_PATIENT_INFO_VIEW : DB_MC_PATIENT_INFO
    {
        public string TRIAGE_CHT { 
            get {
                string val = "受傷";
                if (!string.IsNullOrWhiteSpace(this.TRIAGE))
                {
                    switch (this.TRIAGE)
                    {
                        case "Severe":
                            val = "重傷";
                            break;
                        case "Moderate":
                            val = "中傷";
                            break;
                        case "Mild":
                            val = "輕傷";
                            break;
                        default:
                            break;
                    }
                }
                return val;
            }
        }

    }

    public class MC_PATIENT_INFO
    {
        [Key]
        public string PATIENT_ID { get; set; }
        public string PATIENT_NAME { get; set; }
        public string AGE { get; set; }
        public string GENDER { get; set; }
        public string COUNTRY { get; set; }
        public string TRIAGE { get; set; } 
        public string EXPECTED_ARRIVAL_DATETIME { get; set; }
        public string SELECTION_DATETIME { get; set; } 
        public string CITY { get; set; }
        public string HOSP_KEY { get; set; } 

    }
}

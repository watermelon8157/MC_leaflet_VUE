using RCS_Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using RCSData.Models;
using RCS_Data.Models.ViewModels;

namespace RCS.Models.ViewModel
{
    public class VM_CPTNewRecord: VM_BASIC
    {
        public VM_CPTNewRecord()
        {
            this.model = new CPTNewRecord();
            this.ls_cxr_reason = new List<SysParams>();
            this.ls_chest_tube = new List<SysParams>();
            this.ls_pat_problem = new List<SysParams>();
            this.ls_drug_inhalation = new List<SysParams>();
            this.ls_conscious_e = new List<SysParams>();
            this.ls_conscious_v = new List<SysParams>();
            this.ls_conscious_m = new List<SysParams>();
            this.ls_VentilationModeList = new List<SysParams>();
            this.list = new List<CPTNewList>();
            
        }
        public IPDPatientInfo pat_info { get; set; }
        public List<SysParams> ls_cxr_reason { get; set; }
        public List<SysParams> ls_chest_tube { get; set; }
        public List<SysParams> ls_pat_problem { get; set; }
        public List<SysParams> ls_drug_inhalation { get; set; }
        public List<SysParams> ls_conscious_e { get; set; }
        public List<SysParams> ls_conscious_v { get; set; }
        public List<SysParams> ls_conscious_m { get; set; }
        public List<SysParams> ls_VentilationModeList { get; set; }
        public string tmpReasonString
        {
            get
            {
                if (this.ls_cxr_reason != null && this.ls_cxr_reason.Count> 0)
                {
                    List<SysParams> tmpReasonDisable = this.ls_cxr_reason.Where(x => x.P_STATUS == "0").ToList();
                    if (tmpReasonDisable != null && tmpReasonDisable.Count > 0)
                    {
                        return string.Join(",",tmpReasonDisable.Select(x => x.P_VALUE.ToString()).ToArray());
                    }
                }
                return "";
            }
        }

        public string breath_mode { get; set; }

        public List<CPTNewList> list { get; set; }

        public CPTNewRecord model { get; set; }

        public string showCheckBox(string ischeckVal, string checkVal)
        {
            string returnVal = "□";
            if (ischeckVal != null && ischeckVal.Trim() == checkVal)
            {
                return "■";
            }
            return returnVal;
        }

    }

    public class printCPTNewData {
        public CPTNewRecord data { get; set; }

        public RCS_CPT_DTL_NEW_ITEMS cpt_data { get; set; }

        public IPDPatientInfo pat_info { get; set; }

        public UserInfo user_info { get; set; }
    }
    public class ListprintCPTNewData
    {
        public List<printCPTNewData> List { get; set; }
    }

    public class VM_BASIC
    {
        public string sDate { get; set; }
        public string eDate { get; set; }
    }

    public class CPTNewList
    {
        public CPTNewList()
        {
            list = new List<CPTNewRecord>();
        }
        public string source_type { get; set; }
        public string genderCHT { get; set; }
        public string patient_name { get; set; }
        public string bed_no { get; set; }
        public string age { get; set; }
        public string diag_date { get; set; }
        public string chart_no { get; set; }
        public List<CPTNewRecord> list { get; set; }
    }

    public class printCPTNewRecordData
    {
        public List<List<CPTNewRecord>> data { get; set; }

        public IPDPatientInfo pat_info { get; set; }

        public UserInfo user_info { get; set; }
    }

}
using System.Collections.Generic;
using RCS_Data.Models.DB;
using RCSData.Models;
using RCS_Data.Models.ViewModels;
using Newtonsoft.Json.Linq;
using System;
using System.Linq;

namespace RCS_Data.Controllers.RtRecord
{
    public interface Interface
    {
        RT_RECORD_DATA<RT_RECORD_MUST_DATA> changeRTRecordData(RT_RECORD_DATA<RT_RECORD_MUST_DATA> List);
    }

    public interface IRtRecordController
    {
        object SelectListItem(Form_SelectListItem form);
    }

    public interface IForm_SelectListItem
    {
        string P_MODE { get; set; }
        string P_GROUP { get; set; }
    }

    public interface IRT_RECORD_GET_LAST
    {
        string respid { get; set; }
        string device { get; set; }
        string mode { get; set; }
        string device_o2 { get; set; }
        string vt_set { get; set; }
        string vr_set { get; set; }
        string vr { get; set; }
        string flow { get; set; }
        string mv_set { get; set; }
        /// <summary>Flow / Flow Pattern  Flow Pattern</summary>
        string flow_pattern { get; set; } 
        string insp_time { get; set; }
        string ie_ratio { get; set; }
        string thigh { get; set; }
        string tlow { get; set; }
        string ipap { set; get; }
        string epap { set; get; }
        string phigh { get; set; }
        string plow { get; set; }
        string spo2 { get; set; }
        /// <summary>NO</summary>
        string no { get; set; }
        string artificial_airway_type { get; set; } 
        string et_size { get; set; }
        string et_mark { get; set; }
        string cuff { get; set; }
        string breath_sound { get; set; }
        /// <summary>GCS：E </summary>
        string gcse { get; set; }
        /// <summary>GCS：V </summary>
        string gcsv { get; set; }
        /// <summary>GCS：M </summary>
        string gcsm { get; set; } 
        /// <summary>意識</summary>
        string conscious { get; set; }
    }

    public interface IForm_RtRecordList
    {
        string pSDate { get; set; }
        string pEDate { get; set; }
        string pipd_no { get; set; }
        string pId { get; set; }
    }
}

using RCS_Data.Controllers.RtValuation;
using RCS_Data.Models.DB;
using RCSData.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RCS_Data
{
    public class VentilatorStatus
    { 
        public VentilatorStatus()
        {
            alarmList = new Dictionary<string, bool>();
            isgone = false;
        }
        public VentilatorStatus(System.Data.DataRow pRow)
        {
            cost_code = pRow["cost_code"].ToString();
            patient_name = pRow["patient_name"].ToString();
            bedno = pRow["bedno"].ToString();
            chart_no = pRow["chart_no"].ToString();
            device_type = pRow["device_type"].ToString();
        } 
        public string patient_name { get; set; }
        public string cost_code { get; set; }
        public string bedno { get; set; }
        public string chart_no { get; set; }
        public string device_no { get; set; }
        public string device_type { get; set; }
        public string alarm_message { get; set; }
        public string this_alarm { get; set; }
        public string this_alarm_time { get; set; }
        public string lastCheckTime { get; set; }
        public bool isgone { get; set; }
        public bool isInit { get; set; }
        public Dictionary<string, bool> alarmList { get; set; }
    }

    public class VentilatorStatusPrintData
    {

        public string V_ID { get; set; }

        public string RECORD_DATE { get; set; }

        public List<DB_RCS_SYS_CONSUMABLE_LIST> CONSUMABLE_LIST { get; set; }

        public List<DB_RCS_VALUATION_DTL> All { get; set; }

        public IPDPatientInfo pat_info { get; set; }
        public UserInfo user_info { get; set; }
    }
}

using RCS_Data.Controllers.RtRecord;
using RCSData.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RCS_Data.Models.ViewModels
{
    public class RT_RECORD_MAIN : RT_RECORD_MAIN_BASIC
    {
        /// <summary>
        /// 病患資料
        /// </summary>
        public IPDPatientInfo pat_info { get; set; }
        public RT_RECORD_MAIN()
        {
            onIntubate = new RCS_ONMODE_MASTER();
            onBreath = new RCS_ONMODE_MASTER();
            onOxygen = new RCS_ONMODE_MASTER();
            rt_record = new RT_RECORD();
            pat_info = new IPDPatientInfo();
        }

        public string pRWCA_memo { get; set; }

        public string pRWCA_memo_Plan { get; set; }
    }
}

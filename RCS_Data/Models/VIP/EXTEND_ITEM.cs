using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RCS_Data.Models.VIP
{
    /// <summary>
    /// VIPRTTBL extendItems
    /// </summary>
    public class EXTEND_ITEM
    {
        /// <summary>
        /// vt_insp 
        /// </summary>
        public string vt_insp { get; set; }

        /// <summary>
        /// Complance
        /// </summary>
        public string compl { get; set; }

        /// <summary>
        /// Resistance
        /// </summary>
        public string resist { get; set; }

        /// <summary>
        /// Pres Limit L
        /// </summary>
        public string min_pr_alarm { get; set; }

        /// <summary>
        /// Flow pattern
        /// </summary>
        public string flow_pattern { get; set; }

        /// <summary>
        /// Pause(S)
        /// </summary>
        public string pause { get; set; }

        /// <summary>
        /// MV%
        /// </summary>
        public string mv_percent { get; set; }

        /// <summary>
        /// I-Time%  (某一台)
        /// </summary>
        public string i_time__percent { get; set; }

        /// <summary>
        /// ramp
        /// </summary>
        public string ramp { get; set; }

        /// <summary>
        /// MVi  Carina
        /// </summary>
        public string mvi { get; set; }

        /// <summary>
        /// Low MVi  Carina
        /// </summary>
        public string low_mvi { get; set; }

        /// <summary>
        /// Trigger   Carina
        /// </summary>
        public string trigger { get; set; }

        /// <summary>
        /// ampl(DeltaP) SLE6000   
        /// </summary>
        public string ampl { get; set; }

        /// <summary>
        /// Hz(HFO Rate) SLE6000   
        /// </summary>
        public string hz { get; set; }

        /// <summary>
        /// Flow Assist  
        /// </summary>
        public string fa { get; set; }

        /// <summary>
        /// Volume Assist
        /// </summary>
        public string va { get; set; }

        /// <summary>
        /// ETS
        /// </summary>
        public string ets { get; set; }

        /// <summary>
        /// Tube Compensation
        /// </summary>
        public string compen { get; set; }

        /// <summary>
        /// HiFlowO2 Flow
        /// </summary>
        public string o2_flow { get; set; }
        /// <summary>
        /// fSpont
        /// </summary>
        public string fspont { get; set; }
        /// <summary>
        /// Pinsp
        /// </summary>
        public string pinsp { get; set; }
        /// <summary>
        /// Ti(Monitor)
        /// </summary>
        public string insp_t_mntr { get; set; }
        /// <summary>
        /// TPEEP
        /// </summary>
        public string TPEEP { get; set; }
    }
}

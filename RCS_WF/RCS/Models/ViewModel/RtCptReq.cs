using RCS_Data.Models.ViewModels;
using RCSData.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RCS.Models.ViewModel
{
    public class RtCptReq : RCS_Data.Controllers.RtCptReq.Models, RCS_Data.Controllers.RtCptReq.Interface
    {

      
    }
    public class printRtCptReq
    {
        public RTCptReq data { get; set; }


        public IPDPatientInfo pat_info { get; set; }

        public UserInfo user_info { get; set; }
    }
    public class ListprintRtCptReq {
      public  List<printRtCptReq> List { get; set; }
    }
}
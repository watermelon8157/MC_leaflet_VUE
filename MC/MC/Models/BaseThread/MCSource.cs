using RCS_Data;
using RCS_Data.Models;
using RCS_Data.Models.DB;
using RCSData.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Device.Location;
using RCS.Models.ViewModel;

namespace RCS.Models
{
    /// <summary>
    /// 計算醫院分數
    /// </summary>
    public class MCSource : BaseThread
    { 

        string csName = "MCSource";

        MCModel _model = new MCModel();
 

        public override void RunThread()
        {
           _model.RunThread();
             

        }
 
 

    }



}
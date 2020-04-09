using Com.Mayaminer;
using Newtonsoft.Json;
using RCS_Data;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Web.Mvc;
using System.Web;
using System.Web.Configuration;
using System.Globalization;
using System.Text;
using System.ComponentModel;
using System.Net;
using System.IO;
using System.Data;
using mayaminer.com.library;
using RCS.Models;
using RCS.Models.DB;
using RCSData.Models;
using RCS_Data.Models.ViewModels;

namespace RCS.Controllers {
    public class RTAssessController : BaseController {

 
        /// <summary>
        /// 取得最新呼吸模式
        /// </summary>
        /// <returns>string</returns>
        public string getRT_Mode(string setIpdno) {
            string mode = "";
            Ventilator last_record = BaseModel.basicfunction.GetLastRTRec(pat_info.chart_no, pat_info.ipd_no);
            mode = last_record.mode;
            return mode;
        }
         
    }
}

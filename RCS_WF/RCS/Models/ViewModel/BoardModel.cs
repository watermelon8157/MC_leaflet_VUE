using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RCS.Models.ViewModel
{
    public class BoardModel
    {
    }

    public class WPTable
    {
        public string bed_no { get; set; }
        public string patient_name { get; set; }
        public string chart_no { get; set; }

        public string ipd_no { get; set; }

        public int use_day { get; set; }
        public string msgType { get; set; }
        public string msg { get; set; }

        public bool hasData { get; set; }
    }
}
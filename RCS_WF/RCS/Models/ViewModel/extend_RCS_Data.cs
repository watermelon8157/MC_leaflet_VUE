using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace RCS_Data
{
    public class DeviceModel
    {
        public DeviceModel() {
            modeltypelist = new List<SelectListItem>();
            hospdevicelist = new List<SelectListItem>();
            systemtestlist = new List<SelectListItem>();
            cleanstatuslist = new List<SelectListItem>();
            statuslist = new List<SelectListItem>();
        }
        public List<SelectListItem> modeltypelist { get; set; }
        public List<SelectListItem> hospdevicelist { get; set; }
        public List<SelectListItem> systemtestlist { get; set; }
        public List<SelectListItem> cleanstatuslist { get; set; }
        public List<SelectListItem> statuslist { get; set; }
    }

    public class DeviceChecklist
    {
        public string DEVICE_SEQ { get; set; }
        public string CHART_NO { get; set; }
        public string CREATE_NAME { get; set; }
        public List<string> DETAIL { get; set; }
        public string CREATE_DATE { get; set; }

    }

    public class ErrStrBuilder : List<string>
    {
        public ErrStrBuilder()
        {
            CustomSeparator = "\n";
        }
        public string CustomSeparator { private get; set; }
        public string errStr
        {
            get
            {
                return this == null ? "" : string.Join(CustomSeparator, this);
            }
        }
        public bool HasError { 
            get 
            { 
                return errStr != ""; 
            } 
        }
    }
}
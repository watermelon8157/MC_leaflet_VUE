using RCS.Models.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace RCS.Models
{
    public class SuppliesViewModel 
    {
        public List<SelectListItem> package_name
        {
            get;set;
        }
        public List<SelectListItem> suplies_name
        {
            get; set;
        }
        public List<RCS_SYS_CONSUMABLE_PACKAGE_DTL> package_dtl
        {
            get; set;
        }

    }
}
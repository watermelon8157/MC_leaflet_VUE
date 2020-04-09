using RCS_Data.Models;
using RCS_Data.Models.JAG;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RCS.Models.HOSP.JAG
{
    public class JAGRoot : JAGOrderRoot
    {
        public JAGRoot()
        {
            this.RequestDate = Function_Library.getDateNowString( DATE_FORMAT.yyyyMMdd);
            this.RequestTime = Function_Library.getDateNowString(DATE_FORMAT.HHmmss);  
            this.RequestDivision = "0320";
            this.SignSystem = "RT";
            this.DocCharge = "001";
            this.InHospital = "1";
        }
    }
}
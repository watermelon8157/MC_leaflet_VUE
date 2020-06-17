using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RCS.Models.HOSP.WF.WebService
{
    public class HISABGLabData : HISLabData
    {
        public override Dictionary<string, string> itemKey
        {
            get
            {
                Dictionary<string, string> pDic = new Dictionary<string, string>();
                pDic.Add("pH", "PH"); // OK
                pDic.Add("PCO2", "PCO2"); // ok
                pDic.Add("PO2", "PO2"); // OK
                pDic.Add("BaseExcess", "BE(E"); // ok
                pDic.Add("HCO3", "HCO3"); // ok
                pDic.Add("TotalCarbonDioxide", "TCO2");  //  OK
                pDic.Add("SaO2", "SO2"); // OK
                pDic.Add("SPEM", "生化組");//檢體名稱
                return pDic;
            }
        }

    }
}
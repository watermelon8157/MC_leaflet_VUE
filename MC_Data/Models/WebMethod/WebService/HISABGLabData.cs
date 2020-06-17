using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RCSData.Models.WebService
{
    public class HISABGLabData : HISLabData , IWebServiceParam
    {
        public override Dictionary<string, string> itemKey
        {
            get
            {
                Dictionary<string, string> pDic = new Dictionary<string, string>();
                pDic.Add("pH", "pH");
                pDic.Add("PCO2", "pCO2");
                pDic.Add("PO2", "pO2");
                pDic.Add("BaseExcess", "BE(B)");
                pDic.Add("HCO3", "HCO3-ACT");
                pDic.Add("TotalCarbonDioxide", "CTCO2");
                pDic.Add("SaO2", "O2SAT");
                pDic.Add("SPEM", "ARTERIAL BLOOD");//檢體名稱
                return pDic;
            }
        }

    }
}

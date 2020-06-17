using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RCS.Models.HOSP.WF.WebService
{
    public class HISBloodBiochemicalLabData : HISLabData
    {
        public override Dictionary<string, string> itemKey
        {
            get
            {
                Dictionary<string, string> pDic = new Dictionary<string, string>();
                pDic.Add("Hb", "HGB");
                pDic.Add("Ht", "HCT");
                pDic.Add("WBC", "WBC");
                pDic.Add("Platelet", "PLT");
                pDic.Add("Na", "NA");
                pDic.Add("K", "K");
                pDic.Add("Ca", "CA");
                pDic.Add("Cl", "CL");
                pDic.Add("BUN", "BUN");
                pDic.Add("Cr", "CREB");
                pDic.Add("Mg", "MG");
                pDic.Add("Albumin", "ALB");
                pDic.Add("Prealbumin", "PALB");
                pDic.Add("FBS", "FBS");
                return pDic;
            }
        }
    }
}
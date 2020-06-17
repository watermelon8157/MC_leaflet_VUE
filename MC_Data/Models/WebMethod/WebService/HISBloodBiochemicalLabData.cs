using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RCSData.Models.WebService
{
    public class HISBloodBiochemicalLabData : HISLabData
    {
        public override Dictionary<string, string> itemKey
        {
            get
            {
                Dictionary<string, string> pDic = new Dictionary<string, string>();
                pDic.Add("Hb", "Hb");
                pDic.Add("Ht", "Ht");
                pDic.Add("WBC", "WBC");
                pDic.Add("Platelet", "Platelet");
                pDic.Add("Na", "Na");
                pDic.Add("K", "K");
                pDic.Add("Ca", "Ca");
                pDic.Add("Cl", "Cl");
                pDic.Add("BUN", "BUN");
                pDic.Add("Cr", "Creatinine");
                pDic.Add("Mg", "Mg");
                pDic.Add("Albumin", "Albumin");
                pDic.Add("Prealbumin", "Prealbumin");
                pDic.Add("FBS", "FBS");
                return pDic;
            }
        }
    }
}

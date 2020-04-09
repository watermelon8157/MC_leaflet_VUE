using RCS_Data.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RCS.Models
{
    /// <summary>
    /// 醫院下拉選單
    /// </summary>
    public class HospSetDDL : SetDDL
    {
        public override List<DDLItem> getConsciousEVM(string pType)
        {
            List<DDLItem> ConsciousList = base.getConsciousEVM(pType);
            switch (pType)
            {
                case "E": 
                    ConsciousList.Add(new DDLItem() { key = "C", label = "C" });
                    break;
                case "V":
                    ConsciousList.Add(new DDLItem() { key = "T", label = "T" });
                    ConsciousList.Add(new DDLItem() { key = "A", label = "A" });
                    break;
                case "M": 
                    break;
            }
            return ConsciousList;
        }
    }
}
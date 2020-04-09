using mayaminer.com.library;
using RCS_Data.Controllers;
using RCS_Data.Models.DB;
using RCSData.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RCS_Data.Models.ViewModels
{
    /// <summary>
    /// 下拉選單
    /// </summary>
    public class SetDDL : BaseModels
    {
         
        public List<DDLItem> Empty()
        {
            return new List<DDLItem>();
        }
         
        /// <summary>
        /// 取得意識EVM下拉選單來源
        /// </summary>
        /// <param name="pType">意識類型</param>
        /// <returns></returns>
        public virtual List<DDLItem> getConsciousEVM(string pType)
        {
            List<DDLItem> ConsciousList = new List<DDLItem>();
            switch (pType)
            {
                case "E":
                    ConsciousList.Add(new DDLItem() { key = "1", label = "1" });
                    ConsciousList.Add(new DDLItem() { key = "2", label = "2" });
                    ConsciousList.Add(new DDLItem() { key = "3", label = "3" });
                    ConsciousList.Add(new DDLItem() { key = "4", label = "4" }); 
                    break;
                case "V":
                    ConsciousList.Add(new DDLItem() { key = "1", label = "1" });
                    ConsciousList.Add(new DDLItem() { key = "2", label = "2" });
                    ConsciousList.Add(new DDLItem() { key = "3", label = "3" });
                    ConsciousList.Add(new DDLItem() { key = "4", label = "4" });
                    ConsciousList.Add(new DDLItem() { key = "5", label = "5" });
                    ConsciousList.Add(new DDLItem() { key = "E", label = "E" });
                    ConsciousList.Add(new DDLItem() { key = "Tr", label = "Tr" });
                    ConsciousList.Add(new DDLItem() { key = "Tr.", label = "Tr." }); 
                    break;
                case "M":
                    ConsciousList.Add(new DDLItem() { key = "1", label = "1" });
                    ConsciousList.Add(new DDLItem() { key = "2", label = "2" });
                    ConsciousList.Add(new DDLItem() { key = "3", label = "3" });
                    ConsciousList.Add(new DDLItem() { key = "4", label = "4" });
                    ConsciousList.Add(new DDLItem() { key = "5", label = "5" });
                    ConsciousList.Add(new DDLItem() { key = "6", label = "6" });
                    break;
            }
            return ConsciousList;
        }

      
        public List<DDLItem> getHisBedAreaList(IWebServiceParam iwp)
        {
            return this.webmethod.getHisBedAreaList(iwp).GroupBy(x => new { x.area_code, x.area_title }).Select(x => new DDLItem() { key = x.Key.area_code, label = x.Key.area_title }).ToList();
        }

        public List<DDLItem> getRCS_SYS_PARAMS(string pModel = "", string pGroup = "", string pLang = "zh-tw", string pStatus = "", string pManage = "")
        { 
            List<DB_RCS_SYS_PARAMS> SelectListItem = base.getRCS_SYS_PARAMS(pModel, pGroup, pLang, pStatus, pManage); 
            return SelectListItem.GroupBy(x=> new  {  x.P_VALUE ,  x.P_NAME, x.P_STATUS, x.P_SORT }).Select(x=> new DDLItem() {  key = x.Key.P_VALUE, label =x.Key.P_NAME , datastatus  = x.Key.P_STATUS, sort = x.Key.P_SORT }).ToList();
        }

        /// <summary>
        /// 取得氧氣治療設備
        /// </summary>
        /// <param name="pLang"></param>
        /// <param name="pStatus"></param>
        /// <param name="pManage"></param>
        /// <returns></returns>
        public List<DDLItem> getdevice_O2DLL( string pLang = "zh-tw", string pStatus = "", string pManage = "")
        {
            List<DB_RCS_SYS_PARAMS> SelectListItem = base.getRCS_SYS_PARAMS("RTRecord_Detail", "device_o2", pLang, pStatus, pManage);
            return SelectListItem.GroupBy(x => new { x.P_VALUE, x.P_NAME, x.P_STATUS, x.P_SORT }).Select(x => new DDLItem() { key = x.Key.P_VALUE, label = x.Key.P_NAME, datastatus = x.Key.P_STATUS, sort = x.Key.P_SORT }).ToList().OrderBy(x=>x.sort).ToList();
        }
    }

    public class DDLItem
    {
        public string key { get; set; }

        public string label { get; set; }
        public string datastatus { get; set; }
        private string _sort { get; set; }
        public string sort
        {
            get
            {
                return string.IsNullOrWhiteSpace(this._sort) ? "0".PadLeft(10,'0') : this._sort.PadLeft(10, '0');
            }
            set { this._sort = value; }
        }
    }
}

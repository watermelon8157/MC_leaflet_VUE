using System.Collections.Generic;
using RCSData.Models;

namespace RCS.Models.HOSP.WF.WebService
{
    public class HisBedAreaList : RCSData.Models.WebService.HisBedAreaList
    {
        public HisBedAreaList() 
        { 
            this.paramList = new Dictionary<string, webParam>();
            this.returnValue = new Dictionary<string, webParam>();
            this.returnValue.Add("area_code", new webParam() { paramName = "AREA_CODE" });//成本中心代碼
            this.returnValue.Add("area_name", new webParam() { paramName = "AREA_DESC" });//成本中心描述
        }
    }
}
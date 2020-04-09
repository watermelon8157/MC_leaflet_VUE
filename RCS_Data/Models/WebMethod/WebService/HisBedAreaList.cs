using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RCSData.Models.WebService
{
    public class HisBedAreaList : WebServiceParam, IWebServiceParam
    {
        public override string webMethodName
        {
            get
            {
                return "getHisBedAreaList";
            }
        }
        public HisBedAreaList()
        {
            this.paramList = new Dictionary<string, webParam>();
            this.returnValue = new Dictionary<string, webParam>();
            this.returnValue.Add("area_code", new webParam() { paramName = "area_code" });//成本中心代碼
            this.returnValue.Add("area_name", new webParam() { paramName = "area_name" });//成本中心描述
        }
    }
}
using RCS.Models.ViewModel;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;

namespace RCS
{
    /// <summary>
    ///RCSWebService 的摘要描述
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // 若要允許使用 ASP.NET AJAX 從指令碼呼叫此 Web 服務，請取消註解下列一行。
    // [System.Web.Script.Services.ScriptService]
    public class MCWebService : System.Web.Services.WebService
    {
        string csName = "MCWebService";

        [WebMethod]
        public string HelloWorld()
        {
            return "Hello World";
        }

        [WebMethod]
        public WSresponse INSERT_PAT_DATA(string OrderStr, string Base64Str)
        {
            WSresponse rm = new WSresponse();

            return rm;
        }
        [WebMethod]
        public List<pat_data> GET_PAT_DATA(string PATIENT_ID)
        {
            List<pat_data> pList = new List<pat_data>();

            return pList;
        }
         
    }

    public class WSresponse
    { 
        public string msg { get; set; }

        public  string PATIENT_ID { get; set; }
    }
    public class pat_data
    {
        public string success { get; set; }

        public string PATIENT_ID { get; set; }
    }
}

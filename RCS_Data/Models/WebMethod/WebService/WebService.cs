using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RCSData.Models
{
    

    public abstract class WebServiceParam : IWebServiceParam
    {
        public virtual bool WSIsOK { get { return true; } }
        public virtual Dictionary<string, string> itemKey { get; }
        public virtual string webMethodName { get;  }

        public Dictionary<string, webParam> paramList { get; set; }

        public Dictionary<string, webParam> returnValue { get; set; }

        public virtual bool settingResult { get;  }
         
    }

    public interface IWebServiceParam
    {
        /// <summary>
        /// WS是否可以使用
        /// </summary>
        bool WSIsOK { get;  }
        Dictionary<string, string> itemKey { get; } 
        string webMethodName { get; }

        Dictionary<string, webParam> paramList { get; set; }

        Dictionary<string, webParam> returnValue { get; set; }

        bool settingResult { get;  }
    }

}
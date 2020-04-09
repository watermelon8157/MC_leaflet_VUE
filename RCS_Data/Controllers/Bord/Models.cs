using Com.Mayaminer;
using mayaminer.com.library;
using RCS_Data.Controllers.RtRecord;
using RCS_Data.Models;
using RCS_Data.Models.DB;
using RCS_Data.Models.ViewModels;
using RCSData.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text.RegularExpressions;

namespace RCS_Data.Controllers.Bord
{
    public partial class Models : BaseModels,RCS_Data.Controllers.Bord.Interface
    { 
        string csName { get { return "Bord.Models"; } }

        public RESPONSE_MSG BordList(UserInfo user_info)
        {
            string actionName = "UploadList"; 
            RESPONSE_MSG rm = new RESPONSE_MSG();
            List<BordItem> pList = new List<BordItem>();
            
            rm.attachment = pList;
            return rm; 
        } 
       
    }

    public class FormBordList : AUTH
    {
       
    }

    public class BordItem
    {
        public string type { get; set; }
        public string msg { get; set; }
        public List<object>  itemDetail { get; set; }

    }

    
}

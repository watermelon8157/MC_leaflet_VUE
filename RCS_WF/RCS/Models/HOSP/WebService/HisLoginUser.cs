using System.Collections.Generic;
using RCSData.Models;

namespace RCS.Models.HOSP.WF.WebService
{
    public class HisLoginUser : RCSData.Models.WebService.HisLoginUser
    {
        public HisLoginUser()
        {
            this.paramList = new Dictionary<string, webParam>();
            this.paramList.Add("user_id", new webParam() { paramName = "UserID" });//登入者代碼
            this.paramList.Add("user_pwd", new webParam() { paramName = "UserPWD" });//登入者密碼

            this.returnValue = new Dictionary<string, webParam>();
            this.returnValue.Add("user_id", new webParam() { paramName = "USER_ID" });//員工編號
            this.returnValue.Add("user_name", new webParam() { paramName = "USER_NAME" });//員工姓名
            this.returnValue.Add("authority", new webParam() { paramName = "AUTHORITY" });//職稱 
            this.returnValue.Add("cost_center_code", new webParam() { paramName = "COST_CENTER_CODE" });//員工成本中心
            this.returnValue.Add("user_idno", new webParam() { paramName = "IDNO" });//身分證
        }
    }
}
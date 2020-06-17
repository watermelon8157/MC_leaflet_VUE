using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RCSData.Models.WebService
{
    public class HisLoginUser : WebServiceParam
    {
        public override string webMethodName
        {
            get
            {
                return "getUserLogin";
            }
        }
        public HisLoginUser()
        {
            this.paramList = new Dictionary<string, webParam>();
            this.paramList.Add("user_id", new webParam() { paramName = "UserID" });//登入者代碼
            this.paramList.Add("user_pwd", new webParam() { paramName = "UserPWD" });//登入者密碼

            this.returnValue = new Dictionary<string, webParam>();
            this.returnValue.Add("user_id", new webParam() { paramName = "UserID" });//員工編號
            this.returnValue.Add("user_name", new webParam() { paramName = "HOSP_NAME" });//員工姓名
            this.returnValue.Add("authority", new webParam() { paramName = "USER_ROLE" });//職稱
            this.returnValue.Add("card_id", new webParam() { paramName = "card_id" });//CardID
            this.returnValue.Add("cost_center_code", new webParam() { paramName = "CostCenterCode" });//員工成本中心
            this.returnValue.Add("user_idno", new webParam() { paramName = "UserID" });//身分證
        }
    }
}
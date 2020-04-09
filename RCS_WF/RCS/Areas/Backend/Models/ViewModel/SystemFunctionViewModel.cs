using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using RCS_Data;
using Com.Mayaminer;
using Dapper;
using mayaminer.com.library;
using System.Web.Mvc;
using RCS.Models;
using Newtonsoft.Json;
using RCSData.Models;

/// <summary>
/// https://www.base64-image.de/
/// 產生圖片 base64網址
/// </summary>
namespace RCS.Areas.Backend.Models
{
    public class SystemFunctionVM
    {
        /// <summary>
        /// 使用者功能清單
        /// </summary>
        public List<SysParams> sysList { get; set; }
        /// <summary>
        /// 系統功能顯示清單
        /// </summary>
        public List<SystemList> systemList { get; set; }
        /// <summary>
        /// 版本清單
        /// </summary>
        public List<SelectListItem> userlist { get; set; }
    }

    /// <summary>
    /// 系統功能顯示清單
    /// </summary>
    public class SystemList
    {
        /// <summary>
        /// RCS_SYS_FUNCTION_LIST.FUN_GROUP
        /// </summary>
        public string sysId { get; set; }
        /// <summary>
        /// 顯示名稱
        /// </summary>
        public string sysName { get; set; }
        /// <summary>
        /// 功能說明
        /// </summary>
        public string sysDesc { get; set; }
        /// <summary>
        /// 主要功能
        /// </summary>
        public List<RCS_SYS_FUNCTION_LIST> SysMainList { get; set; }
        /// <summary>
        /// 是否有有次要功能
        /// </summary>
        public bool haveSubFun {
            get { if (this.SysSubList != null && this.SysSubList.Count > 0) return true; else return false; }
            set { }
        }
        /// <summary>
        /// 版本清單
        /// </summary>
        public List<RCS_SYS_FUNCTION_LIST> SysSubList { get; set; }
    }

    /// <summary>
    /// 系統功能
    /// </summary>
    public class SystemFunction: SystemFunctionVM
    {
        /// <summary>
        /// 系統功能清單
        /// </summary>
        public List<RCS_SYS_FUNCTION_LIST> list { get; set; }
        #region 傳入值
        /// <summary>
        /// 次功能系統變數
        /// </summary>
        public const string p_MODEL = "SystemFunction";
        /// <summary>
        /// 使用者權限清單
        /// </summary>
        private List<RCS_SYS_USER_POWER> USER_POWER_List { get; set; }
        #endregion

        #region 方法
        /// <summary>
        /// 設定後台系統功能顯示清單
        /// </summary>
        public void setSystemList()
        {
            foreach (SysParams item in this.sysList)
            {
                if(this.list.Exists(x=>x.FUN_ID == item.P_VALUE))
                    this.list.Find(x => x.FUN_ID == item.P_VALUE).isChecked = true;
            }
            systemList = new List<SystemList>();
            _setSystemList("system_manage", "系統功能","");
            _setSystemList("left_tab", "標籤主選單", "");
            _setSystemList("main_function", "功能畫面", "");
            _setSystemList("pat_function", "病患主功能", "");
            _setSystemList("RTRecord", "呼吸照護記錄單列印畫面", "");
            _setSystemList("CPTAssess", "呼吸治療評估單列印畫面", ""); 
            _setSystemList("NewCPTRecord", "呼吸治療/胸腔復原治療單列印畫面", "");
            _setSystemList("RTTakeoffAssess", "吸脫離評估單列印畫面", "");
        }

        private void _setSystemList(string sysId,string sysName,string sysDesc)
        {
            SystemList item = new SystemList() { sysId = sysId, sysName = sysName, sysDesc = sysDesc,
                                                 SysMainList = new List<RCS_SYS_FUNCTION_LIST>(),SysSubList = new List<RCS_SYS_FUNCTION_LIST>() };
            //主要功能清單
            if (this.list.Exists(x => x.FUN_GROUP == item.sysId && !x.isSubFunction && !x.isNotShowFunction))
                item.SysMainList = this.list.FindAll(x => x.FUN_GROUP == item.sysId && !x.isSubFunction && !x.isNotShowFunction);
            else
            {
                //次要功能預設選擇基本版
                if (this.list.Exists(x => x.FUN_GROUP == item.sysId && x.isSubFunction && !x.isNotShowFunction))
                    item.SysMainList.Add(this.list.Find(x => x.FUN_GROUP == item.sysId && x.isSubFunction && !x.isNotShowFunction));
            }
            //次要功能清單
            if (this.list.Exists(x => x.FUN_GROUP == item.sysId && x.isSubFunction && !x.isNotShowFunction))
                item.SysSubList = this.list.FindAll(x => x.FUN_GROUP == item.sysId && x.isSubFunction && !x.isNotShowFunction);
            this.systemList.Add(item);
        }

        /// <summary>
        /// 設定基本功能清單
        /// </summary>
        public void setList()
        {
            this.list = JsonConvert.DeserializeObject<List<RCS_SYS_FUNCTION_LIST>>(System.IO.File.ReadAllText(AppDomain.CurrentDomain.BaseDirectory + "App_Data\\RCS_SYS_FUNCTION_LIST.json"));
        }

        /// <summary>
        /// 取得此醫院設定的功能
        /// </summary>
        /// <param name="p_GROUP">選擇醫院權限</param>
        /// <param name="hasBasic">是否帶入基本版權限</param>
        public void getFunctionList(List< string> p_GROUP,bool hasBasic = false)
        {
            string basicStr = RCS.Controllers.BaseController.basic_model;
            LogTool.SaveLogMessage(String.Join(",", p_GROUP), "getFunctionList");
            List<string> GROUP = new List<string>();
            GROUP.AddRange(p_GROUP);
            //是否使用基本版
            if ((p_GROUP.Count>0 &&( RCS.Controllers.BaseController.isBasicMode || hasBasic || RCS.Controllers.BaseController.isDebuggerMode)))
            {
                GROUP.Add(basicStr);
            }
            SQLProvider SYS = new SQLProvider();
            string sql = "SELECT * FROM " + GetTableName.RCS_SYS_PARAMS + " WHERE P_MODEL = " + SQLDefend.SQLString(p_MODEL) + " AND P_GROUP in('"+string.Join("','",GROUP) +"') AND P_STATUS = '1'";
            this.sysList = SYS.DBA.dbConnection.Query<SysParams>(sql).ToList();
            if((p_GROUP.Count > 0 && this.sysList.Exists(x=> p_GROUP.Contains(x.P_GROUP) && !p_GROUP.Contains(basicStr)) ))
            {
                if (!hasBasic)
                {
                    this.sysList.RemoveAll(x => x.P_GROUP == basicStr);
                    LogTool.SaveLogMessage(String.Join(",",this.sysList.Select(x=>x.P_ID).ToList()), "getFunctionList");
                }
            }
        }

        /// <summary>
        /// 取得此帳號權限的清單
        /// </summary>
        /// <param name="sysAuthority"></param>
        protected void getAuthorityList()
        {
            RCS.Models.Authority Authority = new RCS.Models.Authority();
            Authority.setRCS_SYS_USER_POWER();
            this.USER_POWER_List = Authority.RCS_SYS_USER_POWER;
        }

        #endregion

        #region 結果
        /// <summary>
        /// 設定權限清單
        /// </summary>
        /// <param name="hosp_id">醫院代碼</param>
        /// <param name="sysAuthority">權限</param>
        /// <returns></returns>
        public Dictionary<string, UserFunctionAction> getUserFunctionMenu(string hosp_id, string sysAuthority)
        {
            Dictionary<string, UserFunctionAction> dic = new Dictionary<string, UserFunctionAction>();
            try
            {
                if (RCS.Controllers.BaseController.funSetting == null)
                    RCS.Controllers.BaseController.funSetting = new FunctionSetting();
                this.setList();
                LogTool.SaveLogMessage(hosp_id, "getUserFunctionMenu");
                this.getFunctionList(new List<string>() { hosp_id },true);
                this.getAuthorityList();

                if (sysList.Count > 0)
                {
                    
                    foreach (SysParams item in sysList)
                    {
                        UserFunctionAction actionList = new UserFunctionAction();
                        if (RCS.Controllers.BaseController.isDebuggerMode && sysAuthority == "admin")
                        {
                            actionList = new UserFunctionAction(new List<string>() { "瀏覽", "寫", "改", "刪", "停" }, list.Find(x=>x.FUN_ID == item.P_VALUE).FUN_GROUP);
                            dic.Add(item.P_VALUE, actionList);
                        }
                        else
                        {
                            if (USER_POWER_List.Exists(x => x.FUNCTION_NAME == item.P_VALUE && x.ROLE_TYPE == sysAuthority))
                            {
                                RCS_SYS_USER_POWER USER_POWER = USER_POWER_List.Find(x => x.FUNCTION_NAME == item.P_VALUE && x.ROLE_TYPE == sysAuthority);
                                string ACTION_Str = USER_POWER_List.Find(x => x.FUNCTION_NAME == item.P_VALUE && x.ROLE_TYPE == sysAuthority).ACTION;
                                if (!string.IsNullOrWhiteSpace(ACTION_Str))
                                {
                                    actionList = new UserFunctionAction(USER_POWER.ACTION.Split(',').ToList(), USER_POWER.FUNCTION_LOCATION);
                                }
                            }
                            dic.Add(item.P_VALUE, actionList);
                        }
                    }
                    if (RCS.Controllers.BaseController.isDebuggerMode && sysAuthority == "admin")
                    {
                        //隱藏功能
                        setUserFunctionMenu(ref dic, "assignment", "main_function");
                        setUserFunctionMenu(ref dic, "Scheduling", "system_manage");
                    }
                }
            }
            catch (Exception ex)
            {
                LogTool.SaveLogMessage(ex, "getUserFunctionMenu", "SystemFunctionViewModel");
            }
            return dic;
        }
        #endregion

        private void setUserFunctionMenu(ref Dictionary<string, UserFunctionAction> pDic,string pName,string pGroup)
        {
            if (!pDic.Keys.Contains(pName))
            {
                pDic.Add(pName, new UserFunctionAction(new List<string>() { "瀏覽", "寫", "改", "刪", "停" }, pGroup));
            }
        }
    }

    /// <summary>
    /// 系統功能清單
    /// </summary>
    public class RCS_SYS_FUNCTION_LIST : DB_RCS_SYS_FUNCTION_LIST
    {
        /// <summary>
        /// 必須有的基本功能
        /// </summary>
        public bool isMustFunction { get; set; }
        /// <summary>
        /// 此功能是否勾選
        /// </summary>
        public bool isChecked { get; set; }
        /// <summary>
        /// 是否不顯示此功能(可能分標準版功能或是開發中的功能)
        /// </summary>
        public bool isNotShowFunction { get; set; }
        /// <summary>
        /// 排序
        /// </summary>
        public int FUN_SORT { get; set; }
        /// <summary>
        /// 是否是次功能
        /// </summary>
        public bool isSubFunction { get; set; }
        /// <summary>
        /// 次排序
        /// </summary>
        public int FUN_SUB_SORT { get; set; }
    }
}
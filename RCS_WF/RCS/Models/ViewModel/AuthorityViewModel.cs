using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Dapper;
using RCS_Data;
using System.Reflection;
using Newtonsoft.Json;
using Com.Mayaminer;
using mayaminer.com.library;
using System.Data;

namespace RCS.Models
{
    public class AuthorityViewModel : Authority
    {
        /// <summary>
        /// 取得權限設定檔
        /// </summary>
        public List<SysParams> viewSetting { get; set; }

        public AuthorityViewModel()
        {
            viewSetting = new List<SysParams>();
        }

        /// <summary>
        /// 取得設定權限基本資料
        /// </summary>
        public void getViewSetting()
        {
            //取得設定資料
            if (viewSetting == null) viewSetting = new List<SysParams>();
            viewSetting = getRCS_SYS_PARAMS("Authority", @pStatus: "1");
            List<string> tempList = new List<string>();
            if (viewSetting.Count > 0)
            {
                tempList = viewSetting.Select(x => x.P_VALUE).ToList();
                viewSetting.AddRange(getRCS_SYS_PARAMS(tempList, @pStatus: "1"));
                if (viewSetting.Exists(x => x.P_GROUP == "sys_function"))
                {
                    viewSetting.AddRange(getRCS_SYS_PARAMS(viewSetting.FindAll(x => x.P_GROUP == "sys_function").Select(x => x.P_VALUE).Distinct().ToList(), @pStatus: "1"));
                }
                viewSetting.AddRange(getRCS_SYS_PARAMS("user_type", @pStatus: "1"));
            }
            viewSetting = viewSetting.OrderBy(x => x.P_MODEL).ThenBy(x => x.P_GROUP).ThenBy(x => string.IsNullOrWhiteSpace(x.P_STATUS) ? 0 : int.Parse(x.P_STATUS)).ToList();
            //取得使用者權限
            setRCS_SYS_USER_POWER();
        }

        /// <summary>
        /// 確認功能是否存在
        /// </summary>
        /// <param name="cntNow">目前行數</param>
        /// <param name="colorSet"></param>
        /// <returns></returns>
        public void isFunctionExists(ref List<SysParams> list)
        {
            FunctionSetting FunSetting = RCS.Controllers.BaseController.funSetting;
            List<SysParams> tempList = list;
            string actionName = "isFunctionExists";
            try
            {
                if (FunSetting.GetType().GetProperties().ToList().Exists(x => tempList.Exists(s => !string.IsNullOrWhiteSpace(s.P_MEMO) && s.P_MEMO == x.Name)))
                {

                    List<PropertyInfo> piList = FunSetting.GetType().GetProperties().ToList().FindAll(x => tempList.Exists(s => !string.IsNullOrWhiteSpace(s.P_MEMO) && s.P_MEMO == x.Name));
                    foreach (PropertyInfo pi in piList)
                    {
                        bool value = false;
                        bool.TryParse(pi.GetValue(FunSetting, null) == null ? "false" : pi.GetValue(FunSetting, null).ToString(), out value);
                        if (!value)
                        {
                            SysParams item = tempList.Find(x => x.P_MEMO == pi.Name);
                            tempList.Remove(item);
                        }
                    }

                }
            }
            catch (Exception ex)
            {
                LogTool.SaveLogMessage(ex, actionName, GetLogToolCS.AuthorityViewModel);
            }
           
            list = tempList;
        }

        /// <summary>
        /// 判斷是否有此功能
        /// </summary>
        /// <param name="functionName"></param>
        /// <returns></returns>
        public bool isFunctionExists(string functionName)
        {
            FunctionSetting FunSetting = RCS.Controllers.BaseController.funSetting;
            bool hasFunc = false;
            string actionName = "isFunctionExists1";
            try
            {

            }
            catch (Exception ex)
            {
                LogTool.SaveLogMessage(ex, actionName,GetLogToolCS.AuthorityViewModel);
            }
            return hasFunc; 
        }



        #region 使用者管理


        #endregion
    }


    public class Authority: BaseModel
    {
        /// <summary>
        /// 取得權限設定檔
        /// </summary>
        public List<RCS_SYS_USER_POWER> RCS_SYS_USER_POWER { get; set; }

        /// <summary>
        /// 取得權限清單
        /// </summary>
        /// <param name="ROLE_TYPE">指定權限</param>
        public void setRCS_SYS_USER_POWER(string ROLE_TYPE = "")
        {
            //取得使用者權限
            if (RCS_SYS_USER_POWER == null) RCS_SYS_USER_POWER = new List<RCS_SYS_USER_POWER>();
            string sql = "SELECT ROLE_TYPE,FUNCTION_LOCATION,FUNCTION_NAME,ACTION FROM " + GetTableName.RCS_SYS_USER_POWER;
            if (!string.IsNullOrWhiteSpace(ROLE_TYPE))
            {
                sql += " WHERE ROLE_TYPE =" + SQLDefend.SQLString(ROLE_TYPE);
            }
            RCS_SYS_USER_POWER = this.DBA.Connection.Query<RCS_SYS_USER_POWER>(sql).ToList();
        }
    }
    /// <summary>
    /// 使用者權限
    /// </summary>
    public class RCS_SYS_USER_POWER : DB_RCS_SYS_USER_POWER
    {
        /// <summary>
        /// 設定畫面權限設定的權限
        /// </summary>
        public List<string> ACTION_VALUE
        {
            get
            {
                if (!string.IsNullOrWhiteSpace(ACTION))
                {
                    return ACTION.Split(',').ToArray<string>().ToList();
                }
                else
                {
                    return new List<string>();
                }
            }
            
        }
        /// <summary>
        /// 取得畫面上權限設定的資料
        /// </summary>
        public List<string> ACTION_List { get; set; }
        private string _setAuthority {get;set;}
        /// <summary>
        /// 取得畫面權限設定的PK資料
        /// </summary>
        public string setAuthority
        {
            get
            {
                return _setAuthority;
            }
            set
            {
                _setAuthority = value;
                try
                {
                    if (!string.IsNullOrWhiteSpace(_setAuthority))
                    {
                        string[] tempStr = _setAuthority.Split(' ').ToArray<string>();
                        FUNCTION_LOCATION = tempStr[0];
                        FUNCTION_NAME = tempStr[1];
                    }
                }
                catch (Exception ex)
                {
                    LogTool.SaveLogMessage(ex, "setAuthority_set", GetLogToolCS.AuthorityViewModel);
                }
            }
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data;
using RCS_Data;
using Com.Mayaminer;
using Newtonsoft.Json;
using mayaminer.com.library;
using mayaminer.com.jxDB;
using Dapper;
using RCS.Areas.Backend.Models;
using RCS.Models;
using RCS_Data.Models;
/// <summary>
/// 顯示功能清單
/// <para>可以自動設定系統有哪些功能</para>
/// <para>根據使用者設定的功能就有哪些功能</para>
/// </summary>
namespace RCS.Areas.Backend.Controllers
{
    public class SystemFunctionController : RCS.Areas.Backend.Controllers.BaseController
    {
        string csName = "SystemFunctionController";
        // GET: /Backend/SystemFunction/
        private SYS_BASIC FUN = new SYS_BASIC();
        //系統設定功能明稱為SystemFunction
        //P_GROUP 各家醫院不同版本
        
        /// <summary>
        /// 系統功能清單
        /// </summary>
        /// <returns></returns>
        public ActionResult Index()
        {
            return View();
        }

        public JsonResult SystemFunctionViewModel()
        {
            SystemFunctionVM sfvm = new SystemFunctionVM();
            SystemFunction vm = new SystemFunction();
            string actionName = "SystemFunctionViewModel";
            string basic_model = RCS.Controllers.BaseController.basic_model;
            try
            {
                //取得功能清單
                vm.setList();
                vm.list.FindAll(x => x.isMustFunction).ForEach(x => x.isChecked = true);
                //取得登入者功能清單
                string sql = "SELECT DISTINCT USER_ID Value,HOSP_NAME Text,CREATE_DATE FROM RCS_SYS_USER_LIST WHERE USER_ROLE <> 'admin' ORDER BY CREATE_DATE ";
                SQLProvider SQL = new SQLProvider();
                vm.userlist = SQL.DBA.getSqlDataTable< SelectListItem >(sql);//版本清單
                vm.getFunctionList(SQL.DBA.getSqlDataTable<string>(sql), true);
                vm.setSystemList();
                if (!vm.userlist.Exists(x=>x.Value == basic_model))
                    vm.userlist.Insert(0,new SelectListItem() { Text = "基本版", Value = basic_model });
                if(!vm.userlist.Exists(x => x.Value == user_info.user_id))
                    vm.userlist.Insert(0, new SelectListItem() { Text = user_info.user_name, Value = user_info.user_id });
                sfvm = vm;
            }
            catch (Exception ex)
            {
                LogTool.SaveLogMessage(ex, actionName, csName);
            }

            JsonResult jsonresults = Json(sfvm);
            jsonresults.MaxJsonLength = int.MaxValue;
            jsonresults.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
            return jsonresults;
        }

        [HttpPost]
        public JsonResult save(string objStr, string hosp_id)
        {
            RESPONSE_MSG rm = new RESPONSE_MSG();
            string actionName = "save";
            SQLProvider SQL = new SQLProvider();
            string p_MODEL = RCS.Areas.Backend.Models.SystemFunction.p_MODEL;
            try
            {
                //傳入清單
                List<SystemList> systemList = new List<SystemList>();
                objStr = HttpUtility.UrlDecode(objStr);
                systemList = JsonConvert.DeserializeObject<List<SystemList>>(objStr);

                if (systemList.Count > 0)
                {
                    //整理資料
                    DateTime dateTime = DateTime.Now;
                    List<SysParams> list = new List<SysParams>();
                    #region 整理資料
                    foreach (SystemList item in systemList)
                    {
                        if (item.SysMainList.Exists(x => x.isChecked))
                        {
                            if (!item.haveSubFun)
                            {
                                #region 主要功能
                                foreach (RCS_SYS_FUNCTION_LIST Main in item.SysMainList.FindAll(x => x.isChecked))
                                {
                                    dateTime = dateTime.AddSeconds(1);
                                    list.Add(new SysParams()
                                    {
                                        P_ID = dateTime.ToString("yyyyMMddHHmmss"),
                                        P_GROUP = hosp_id,
                                        P_MODEL = p_MODEL,
                                        P_VALUE = Main.FUN_ID,
                                        P_NAME = Main.FUN_NAME,
                                        P_STATUS = "1",
                                        P_MANAGE = "1"
                                    });
                                }
                                #endregion
                            }
                            else
                            {
                                #region 版本功能
                                foreach (RCS_SYS_FUNCTION_LIST sub in item.SysSubList.FindAll(x => x.isChecked))
                                {
                                    dateTime = dateTime.AddSeconds(1);
                                    list.Add(new SysParams()
                                    {
                                        P_ID = dateTime.ToString("yyyyMMddHHmmss"),
                                        P_GROUP = hosp_id,
                                        P_MODEL = p_MODEL,
                                        P_VALUE = sub.FUN_ID,
                                        P_NAME = sub.FUN_NAME,
                                        P_STATUS = "1",
                                        P_MANAGE = "1"
                                    });
                                }
                                #endregion
                            }
                        }
                    }

                    #endregion
                    //交易開始
                    SQL.DBA.BeginTrans();
                    string sql =string.Concat( "DELETE ", GetTableName.RCS_SYS_PARAMS.ToString(), " WHERE P_MODEL = @p_MODEL AND P_GROUP = @hosp_id");
                    Dapper.DynamicParameters dp = new DynamicParameters();
                    dp.Add("hosp_id", hosp_id);
                    dp.Add("p_MODEL", p_MODEL);
                    dp.Add("TableName", GetTableName.RCS_SYS_PARAMS.ToString(), direction:  ParameterDirection.Input);
                    SQL.DBA.DBExecute(sql, dp);
                    sql = string.Concat("INSERT INTO ",GetTableName.RCS_SYS_PARAMS.ToString(),
                         " (P_ID,P_MODEL,P_GROUP,P_NAME,P_VALUE,P_STATUS,P_MANAGE) values (@P_ID,@P_MODEL,@P_GROUP,@P_NAME,@P_VALUE,@P_STATUS,@P_MANAGE)");
                    int cnt = SQL.DBA.DBExecute<SysParams>(sql, list);
                    //交易結束

                    //結果
                    if (!SQL.DBA.hasLastError)
                    {
                        rm.message = "儲存成功";
                        rm.status = RESPONSE_STATUS.SUCCESS;
                        //SQL.DBA.Rollback();
                        SQL.DBA.Commit();
                        sql = "SELECT * FROM " + GetTableName.RCS_SYS_PARAMS + " WHERE P_MODEL = @P_MODEL AND P_GROUP in(@hosp_id,@basic_model)";
                        dp = new DynamicParameters();
                        dp.Add("P_MODEL", p_MODEL);
                        dp.Add("hosp_id", hosp_id);
                        dp.Add("basic_model", RCS.Controllers.BaseController.basic_model);
                        rm.attachment = SQL.DBA.getSqlDataTable<SysParams>(sql, dp).ToList();
                    }
                    else
                    {
                        rm.message = "儲存失敗";
                        rm.status = RESPONSE_STATUS.ERROR;
                        LogTool.SaveLogMessage(SQL.DBA.lastError, actionName, GetLogToolCS.SystemFunctionController);
                        SQL.DBA.Rollback();
                    }
                }
                else
                {
                    rm.status = RESPONSE_STATUS.ERROR;
                    rm.message = "設定選擇功能!";
                }
            }
            catch (Exception ex)
            {
                LogTool.SaveLogMessage(ex, actionName, csName);
                rm.message = "程式發生錯誤，請洽資訊人員!";
                rm.status = RESPONSE_STATUS.EXCEPTION;
                SQL.DBA.Rollback();
            }
            return Json(rm);
        }

        public JsonResult hosp_idList_change(List<SysParams> pSysList,string hosp_id)
        {
            RESPONSE_MSG rm = new RESPONSE_MSG();
            string actionName = "hosp_idList_change";
            try
            {
                if(pSysList != null)
                {
                    pSysList = pSysList.FindAll(x => x.P_GROUP == hosp_id);
                    if (pSysList.Count > 0)
                    {
                        SystemFunction vm = new SystemFunction();
                        //取得功能清單
                        vm.setList();
                        vm.list.FindAll(x => x.isMustFunction).ForEach(x => x.isChecked = true);
                        vm.sysList = pSysList;
                        vm.setSystemList();
                        rm.attachment = vm.systemList;
                    }
                    else
                    {
                        rm.status = RESPONSE_STATUS.ERROR;
                        rm.message = "查無此版本使用權限!";
                    }
                }
                else
                {
                    rm.status = RESPONSE_STATUS.ERROR;
                    rm.message = "查無此版本使用權限!";
                }
            }
            catch (Exception ex)
            {
                rm.status = RESPONSE_STATUS.EXCEPTION;
                rm.message = ex.Message;
                LogTool.SaveLogMessage(ex, actionName,this.csName);
            }

            JsonResult jsonresults = Json(rm);
            jsonresults.MaxJsonLength = int.MaxValue;
            jsonresults.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
            return jsonresults;
        }

    }
}

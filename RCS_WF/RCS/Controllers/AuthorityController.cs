using Com.Mayaminer;
using Dapper;
using mayaminer.com.jxDB;
using mayaminer.com.library;
using Newtonsoft.Json;
using RCS.Models;
using RCS_Data;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;


namespace RCS.Controllers
{
    public class AuthorityController : BaseController
    {
        //權限設定維護
        // GET: /Authority/

        #region 權限設定維護
         /// <summary>
        /// 顯示權限設定的畫面
        /// </summary>
        /// <returns></returns>
        public ActionResult Index()
        {
            AuthorityViewModel vm = new AuthorityViewModel();
            vm.getViewSetting();
            return View(vm);
        }

        /// <summary>
        /// 儲存權限功能設定
        /// </summary>
        /// <returns></returns>
        public JsonResult AuthoritySave(string objStr, string RT_role)
        {
            string actionName = "AuthoritySave";
            RESPONSE_MSG rm = new RESPONSE_MSG();
            try
            {
                List<RCS_SYS_USER_POWER> list = new List<RCS_SYS_USER_POWER>();
                objStr = HttpUtility.UrlDecode(objStr);
                list = JsonConvert.DeserializeObject<List<RCS_SYS_USER_POWER>>(objStr);
                list.ForEach(x => {
                    x.ACTION = string.Join(",", x.ACTION_List);
                    x.CREATE_DATE = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                    x.CREATE_ID = user_info.user_id;
                    x.CREATE_NAME = user_info.user_name;
                });
                SQLProvider SQL = new SQLProvider();
                List<RCS_SYS_USER_POWER> RCS_SYS_USER_POWER = new List<Models.RCS_SYS_USER_POWER>();

                #region AuthoritySave
                string sqlStr1 = "", sqlStr2 = "";

                #region sqlStr1
                sqlStr1 = string.Concat("DELETE ", GetTableName.RCS_SYS_USER_POWER.ToString(), " WHERE ROLE_TYPE = ", SQL.namedArguments, "ROLE_TYPE");
                DynamicParameters dp = new DynamicParameters();
                dp.Add("ROLE_TYPE", RT_role);
                #endregion
                #region sqlStr2
                System.Text.StringBuilder sb = new System.Text.StringBuilder();
                sb.Append(string.Concat("INSERT INTO ", GetTableName.RCS_SYS_USER_POWER.ToString()));
                sb.Append(" (ROLE_TYPE, FUNCTION_LOCATION, FUNCTION_NAME, ACTION, CREATE_ID, CREATE_NAME, CREATE_DATE)");
                sb.Append(string.Concat(" VALUES (", SQL.namedArguments, "ROLE_TYPE"));
                sb.Append(string.Concat(" ,", SQL.namedArguments, "FUNCTION_LOCATION"));
                sb.Append(string.Concat(" ,", SQL.namedArguments, "FUNCTION_NAME"));
                sb.Append(string.Concat(" ,", SQL.namedArguments, "ACTION"));
                sb.Append(string.Concat(" ,", SQL.namedArguments, "CREATE_ID"));
                sb.Append(string.Concat(" ,", SQL.namedArguments, "CREATE_NAME"));
                sb.Append(string.Concat(" ,", SQL.namedArguments, "CREATE_DATE"));
                sb.Append(" )");
                sqlStr2 = sb.ToString();
                #endregion

                SQL.DBA.BeginTrans();
                SQL.DBA.DBExecute(sqlStr1, dp);
                SQL.DBA.DBExecute<RCS_SYS_USER_POWER>(sqlStr2, list);
                SQL.DBA.Close();

                if (!SQL.DBA.hasLastError)
                {
                    //取得修改後的權限清單
                    string sqlStr = string.Concat("SELECT * FROM ", GetTableName.RCS_SYS_USER_POWER.ToString(), " WHERE 1=1");
                    RCS_SYS_USER_POWER = SQL.DBA.getSqlDataTable<RCS_SYS_USER_POWER>(sqlStr, null);
                }
                #endregion


                rm = SQL.RESPONSE_MSG;
                if (rm.status == RESPONSE_STATUS.SUCCESS)
                {
                    rm.message = "儲存成功，請使用者重新登入，權限設定才會更新!";
                    rm.attachment = RCS_SYS_USER_POWER;
                }
                else
                {
                    LogTool.SaveLogMessage(rm.message, actionName, GetLogToolCS.AuthorityController);
                }


                //string sql = "SELECT * FROM " + GetTableName.RCS_SYS_USER_POWER + " WHERE ROLE_TYPE =" + SQLDefend.SQLString(RT_role);
                //DataTable dt = this.DBA.getSqlDataTable(sql);

                //if (BaseModel.DTNotNullAndEmpty(dt))
                //{
                //    dt.AsEnumerable().ToList().ForEach(x=>x.Delete());
                //}
                //else
                //{
                //    if (!string.IsNullOrWhiteSpace(this.DBA.LastError))
                //    {
                //        rm.status = RESPONSE_STATUS.ERROR;
                //        rm.message = "儲存失敗，請洽資訊人員!";
                //        LogTool.SaveLogMessage(this.DBA.LastError, actionName, GetLogToolCS.AuthorityController);
                //        return Json(rm);
                //    }
                //}
                //list.ForEach(x => { x.ACTION = string.Join(",", x.ACTION_List); x.CREATE_DATE = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"); x.CREATE_ID = user_info.user_id; x.CREATE_NAME = user_info.user_name; });
                //foreach (RCS_SYS_USER_POWER item in list)
                //{
                //    DataRow dr = dt.NewRow();
                //    dr["ROLE_TYPE"] = item.ROLE_TYPE;
                //    dr["FUNCTION_LOCATION"] = item.FUNCTION_LOCATION;
                //    dr["FUNCTION_NAME"] = item.FUNCTION_NAME;
                //    dr["ACTION"] = string.Join(",", item.ACTION_List);
                //    dr["CREATE_ID"] = user_info.user_id;
                //    dr["CREATE_NAME"] = user_info.user_id;
                //    dr["CREATE_DATE"] = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                //    dt.Rows.Add(dr);
                //}
                //dbResultMessage msg = this.DBA.UpdateResult(dt,GetTableName.RCS_SYS_USER_POWER.ToString());
                //if(msg.State == enmDBResultState.Success)
                //{
                //    sql = "SELECT * FROM " + GetTableName.RCS_SYS_USER_POWER + " WHERE 1=1";
                //    List<RCS_SYS_USER_POWER> RCS_SYS_USER_POWER = this.DBA.Connection.Query<RCS_SYS_USER_POWER>(sql).ToList();
                //    rm.message = "儲存成功，請使用者重新登入，權限設定才會更新!";
                //    rm.attachment = JsonConvert.SerializeObject(RCS_SYS_USER_POWER);
                //}
                //else
                //{
                //    rm.message = msg.dbErrorMessage;
                //    rm.status = RESPONSE_STATUS.ERROR;
                //    LogTool.SaveLogMessage(msg.dbErrorMessage, actionName, GetLogToolCS.AuthorityController);
                //}
            }
            catch (Exception ex)
            {
                rm.status = RESPONSE_STATUS.EXCEPTION;
                rm.message = "儲存失敗，請洽資訊人員!";
                LogTool.SaveLogMessage(ex, actionName, GetLogToolCS.AuthorityController);
            }
            return Json(rm);
        }
        /// <summary>
        /// 使用者權限設定
        /// </summary>
        /// <param name="objStr"></param>
        /// <returns></returns>
        public JsonResult Role_typeSave(string objStr)
        {
            RESPONSE_MSG rm = new RESPONSE_MSG();
            string actionName = "AuthoritySave";
            try
            {
                // TODO: DBA 修改成dapper
                SysParams ITEM = new SysParams();
                objStr = HttpUtility.UrlDecode(objStr);
                ITEM = JsonConvert.DeserializeObject<SysParams>(objStr);
                SQLProvider SQL = new SQLProvider();
                SysParams temp = new SysParams();
                string  sql =  string.Concat("SELECT * FROM " ,GetTableName.RCS_SYS_PARAMS.ToString(), " WHERE P_ID = ",SQL.namedArguments, "P_ID");
                DynamicParameters dp = new DynamicParameters();
                dp.Add("P_ID", ITEM.P_ID);
                temp = SQL.DBA.getSqlDataTable<SysParams>(sql, dp).First();
                List<SysParams> pList = SQL.DBA.getSqlDataTable<SysParams>(sql, dp);
                if(pList.Count>0)
                {
                    sql = string.Concat( "UPDATE ", GetTableName.RCS_SYS_PARAMS.ToString() , " SET P_VALUE = ",SQL.namedArguments, "P_VALUE WHERE P_ID = ", SQL.namedArguments, "P_ID");
                    dp = new DynamicParameters();
                    dp.Add("P_ID", ITEM.P_ID);
                    dp.Add("P_VALUE", ITEM.P_VALUE);
                    SQL.DBA.DBExecute(sql, dp);
                    if (SQL.DBA.RESPONSE_MSG.status  == RESPONSE_STATUS.SUCCESS)
                    {
                        sql = string.Concat("SELECT * FROM ", GetTableName.RCS_SYS_PARAMS.ToString(), " WHERE P_MODEL = ",SQL.namedArguments,"P_MODEL");
                        dp = new DynamicParameters();
                        dp.Add("P_MODEL", ITEM.P_MODEL);
                        List<SysParams> list = SQL.DBA.dbConnection.Query<SysParams>(sql, dp).ToList();
                        if (list != null && list.Count > 0)
                        {
                            rm.status = RESPONSE_STATUS.SUCCESS;
                            rm.message = "儲存成功!";
                            rm.attachment = JsonConvert.SerializeObject(list);
                        }
                        else
                        {
                            rm.status = RESPONSE_STATUS.ERROR;
                            rm.message = "修改失敗!";
                            LogTool.SaveLogMessage(SQL.DBA.lastError, actionName, GetLogToolCS.AuthorityController);
                        }
                    }
                    else
                    {
                        rm.status = RESPONSE_STATUS.ERROR;
                        rm.message ="修改失敗，請洽資訊人員!";
                        LogTool.SaveLogMessage(SQL.DBA.lastError, actionName, GetLogToolCS.AuthorityController);
                    }
                }
                else
                {
                    if(!string.IsNullOrWhiteSpace(SQL.DBA.lastError))
                    {
                        rm.status = RESPONSE_STATUS.ERROR;
                        rm.message = "修改失敗，請洽資訊人員!";
                        LogTool.SaveLogMessage(SQL.DBA.lastError, actionName, GetLogToolCS.AuthorityController);
                    }
                    else
                    {
                        rm.status = RESPONSE_STATUS.ERROR;
                        rm.message = "查無此使用者權限("+ temp.P_NAME+ ")，請洽資訊人員!";
                    }
                }

            }
            catch (Exception ex)
            {
                rm.status = RESPONSE_STATUS.EXCEPTION;
                rm.message = "程式發生錯誤，請洽資訊人員!";
                LogTool.SaveLogMessage(ex,actionName,GetLogToolCS.AuthorityController);
            }
            return Json(rm);
        }
        #endregion

       
    }
}

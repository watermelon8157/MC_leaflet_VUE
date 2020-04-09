using RCS.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Dapper;
using mayaminer.com.library;
using Com.Mayaminer;
using RCS_Data;
using Newtonsoft.Json;
using System.Data;
using mayaminer.com.jxDB;


/// <summary>
/// 決策資源功能控制項
/// </summary>
namespace RCS.Controllers
{
    public class DecisionSupportController : BaseController
    {
        // 決策資源維護設定
        // GET: /decisionSupport/

        public ActionResult Index()
        {
            DecisionSupportViewModel vm = new DecisionSupportViewModel();
            vm.DS_SysParams = new SysParamCollection();
            vm.DS_SysParams.append_modal(BaseModel.GetModelListCollection("RTRecord_Item"));
            vm.DS_SysParams.append_modal(BaseModel.GetModelListCollection("RTRecord_Detail"));
            vm.DS_SysParams.append_modal(BaseModel.GetModelListCollection("RTRecord_group"));
            return View(vm);
        }

        public JsonResult getTable()
        {
            string sqlstr = "SELECT DS_ID,DS_DESC,DS_SUM,DS_IDEA,DS_MEMO,DS_STATUS,CREATE_ID,CREATE_NAME,CREATE_DATE,MODIFY_ID,MODIFY_NAME,MODIFY_DATE FROM "+ GetTableName.RCS_SYS_DECISION_SUPPORT_MASTER;
            List<RCS_SYS_DECISION_SUPPORT_MASTER> table = new List<RCS_SYS_DECISION_SUPPORT_MASTER>();
            List<RCS_SYS_DECISION_SUPPORT_DETAIL> DETAIL = new List<RCS_SYS_DECISION_SUPPORT_DETAIL>();
            string actionName = "getTable";
            try
            {
                SQLProvider SQL = new SQLProvider();
                string sqlStr = string.Concat("SELECT DS_ID,DS_DESC,DS_SUM,DS_IDEA,DS_MEMO,DS_STATUS,CREATE_ID,CREATE_NAME,CREATE_DATE,MODIFY_ID,MODIFY_NAME,MODIFY_DATE FROM ",
                    GetTableName.RCS_SYS_DECISION_SUPPORT_MASTER.ToString());
                table = SQL.DBA.getSqlDataTable<RCS_SYS_DECISION_SUPPORT_MASTER>(sqlStr, null);

                if (table != null&& table.Count > 0)
                {
                    sqlStr = string.Concat("SELECT DS_ID,DS_ITEM,DS_VALUE,DS_SCORE FROM "
                   , GetTableName.RCS_SYS_DECISION_SUPPORT_DETAIL.ToString(), " WHERE DS_ID in('" + string.Join("','", table.Select(x => x.DS_ID).ToList()) + "')");
                    DETAIL = SQL.DBA.getSqlDataTable<RCS_SYS_DECISION_SUPPORT_DETAIL>(sqlStr, null);

                    foreach (RCS_SYS_DECISION_SUPPORT_MASTER item in table)
                    {
                        if(DETAIL.Exists(x=>x.DS_ID == item.DS_ID))
                        {
                            item.DETAIL = DETAIL.FindAll(x => x.DS_ID == item.DS_ID);
                        }
                        else
                        {
                            item.DETAIL = new List<RCS_SYS_DECISION_SUPPORT_DETAIL>();
                        }
                        
                    }
                }
                if(!string.IsNullOrWhiteSpace(this.DBA.LastError))
                {
                    LogTool.SaveLogMessage(this.DBA.LastError, actionName,GetLogToolCS.DecisionSupportController);
                }
            }
            catch (Exception ex)
            {
                LogTool.SaveLogMessage(ex, actionName, GetLogToolCS.DecisionSupportController);
            }
            return Json(table);
        }

        /// <summary>
        /// 儲存決策資源維護設定
        /// </summary>
        /// <param name="objStr"></param>
        /// <returns></returns>
        public JsonResult decisionSupportSave(string objStr)
        {
            RESPONSE_MSG rm = new RESPONSE_MSG();
            string actionName = "decisionSupportSave";
            try
            {
                SQLProvider SQL = new SQLProvider();
                string saveDate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                string sqlMain = "SELECT * FROM " + GetTableName.RCS_SYS_DECISION_SUPPORT_MASTER;
                string sqlDetail = "SELECT * FROM " + GetTableName.RCS_SYS_DECISION_SUPPORT_DETAIL;
                DataTable dtMain = new DataTable(), dtDetail = new DataTable();
                DataRow drMain = null, drDetail = null;

                RCS_SYS_DECISION_SUPPORT_MASTER item = new RCS_SYS_DECISION_SUPPORT_MASTER();
                objStr = HttpUtility.UrlDecode(objStr);
                item = JsonConvert.DeserializeObject<RCS_SYS_DECISION_SUPPORT_MASTER>(objStr);
                if(!string.IsNullOrWhiteSpace(item.DS_ID))
                {
                    sqlMain += " WHERE DS_ID =" +SQLDefend.SQLString(item.DS_ID);
                    sqlDetail += " WHERE DS_ID =" + SQLDefend.SQLString(item.DS_ID);
                }
                dtMain = this.DBA.getSqlDataTable(sqlMain);
                if( !string.IsNullOrWhiteSpace(this.DBA.LastError))
                {
                    rm.setErrorMsg("取得資料失敗，請洽資訊人員!");
                    LogTool.SaveLogMessage(this.DBA.LastError, actionName, GetLogToolCS.DecisionSupportController);
                    return Json(rm);
                }
                dtDetail = this.DBA.getSqlDataTable(sqlDetail);
                if (!string.IsNullOrWhiteSpace(this.DBA.LastError))
                {
                    rm.setErrorMsg("取得資料失敗，請洽資訊人員!");
                    LogTool.SaveLogMessage(this.DBA.LastError, actionName, GetLogToolCS.DecisionSupportController);
                    return Json(rm);
                }
                if (item != null)
                {
                    if (!string.IsNullOrWhiteSpace(item.DS_ID))
                    {
                        drMain = dtMain.Rows[0];
                        drMain["MODIFY_DATE"] = saveDate;
                        drMain["MODIFY_ID"] = user_info.user_id;
                        drMain["MODIFY_NAME"] = user_info.user_name;
                    }
                    else
                    {
                        drMain = dtMain.NewRow();
                        dtMain.Rows.Add(drMain);
                        drMain = dtMain.AsEnumerable().ToList().Find(x=>x == drMain);
                        item.DS_ID = SQL.GetFixedStrSerialNumber();
                        drMain["DS_ID"] = item.DS_ID;
                        drMain["CREATE_DATE"] = saveDate;
                        drMain["CREATE_ID"] = user_info.user_id;
                        drMain["CREATE_NAME"] = user_info.user_name;
                    }
                    drMain["DS_IDEA"] = item.DS_IDEA;
                    drMain["DS_MEMO"] = item.DS_MEMO;
                    drMain["DS_STATUS"] = item.DS_STATUS;
                    drMain["DS_SUM"] = item.DS_SUM;
                    drMain["DS_DESC"] = item.DS_DESC;
                    //判斷項目
                    if (item.DETAIL != null)
                    {
                        dtDetail.AsEnumerable().ToList().ForEach(x => x.Delete());
                        foreach (RCS_SYS_DECISION_SUPPORT_DETAIL d in item.DETAIL)
                        {
                            drDetail = dtDetail.NewRow();
                            drDetail["DS_ID"] = item.DS_ID;
                            drDetail["DS_ITEM"] = d.DS_ITEM;
                            drDetail["DS_VALUE"] = d.DS_VALUE;
                            drDetail["DS_SCORE"] = d.DS_SCORE;
                            dtDetail.Rows.Add(drDetail);
                        }
                    }
                    else
                    {
                        rm.status = RESPONSE_STATUS.ERROR;
                        rm.message = "請選擇至少一個判斷項目!";
                        return Json(rm);
                    }
                    DataSet ds = new DataSet();
                    dtMain.TableName = GetTableName.RCS_SYS_DECISION_SUPPORT_MASTER.ToString();
                    dtDetail.TableName = GetTableName.RCS_SYS_DECISION_SUPPORT_DETAIL.ToString();
                    ds.Tables.Add(dtMain);
                    ds.Tables.Add(dtDetail);
                    this.DBA.BeginTrans();
                    foreach (DataTable dt in ds.Tables)
                    {
                        dbResultMessage msg = this.DBA.UpdateResult(dt, dt.TableName);
                        if(msg.State != enmDBResultState.Success)
                        {
                            rm.status = RESPONSE_STATUS.ERROR;
                            rm.message = "儲存發生錯誤，請洽資訊人員!";
                            LogTool.SaveLogMessage(msg.dbErrorMessage, actionName, GetLogToolCS.DecisionSupportController);
                            this.DBA.Rollback();
                            break;
                        }
                    }
                    if(rm.status == RESPONSE_STATUS.SUCCESS)
                    {
                        rm.message = "儲存成功!";
                        this.DBA.Commit();
                    }
                }

            }
            catch (Exception ex)
            {
                rm.status = RESPONSE_STATUS.EXCEPTION;
                rm.message = "程式發生錯誤，請洽資訊人員!";
                LogTool.SaveLogMessage(ex,actionName,GetLogToolCS.DecisionSupportController);
            }
            return Json(rm);
        }
    }
}

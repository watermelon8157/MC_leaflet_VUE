using Com.Mayaminer;
using log4net.Repository.Hierarchy;
using Newtonsoft.Json; 
using RCS.Models;
using RCS_Data;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace RCS.Areas.Backend.Controllers
{
    public class RCS_USERController : BaseController
    {
        //
        // GET: /Backend/RCS_USER/

        string csName { get { return "RCS_USERController"; } }

        public ActionResult Index()
        {
            return View();
        }
        /// <summary>
        /// 取得使用者清單
        /// </summary>
        /// <returns></returns>
        public JsonResult userList()
        {
            string actionName = "userList";
            RESPONSE_MSG rm = new RESPONSE_MSG();
            List<RCS_SYS_USER_LIST> list = new List<RCS_SYS_USER_LIST>();
            try
            {
                SQLProvider SQL = new SQLProvider();

                string sql = @"SELECT 
SYS_ID,HOSP_NAME,USER_ID,USER_PWD,START_DATE,END_DATE,USER_ROLE,DATASTATUS,CREATE_ID,CREATE_NAME,CREATE_DATE,MODIFY_DATE
 FROM RCS_SYS_USER_LIST WHERE USER_ROLE <> 'admin' ORDER BY DATASTATUS,CREATE_DATE DESC,MODIFY_DATE DESC ";
                list = SQL.DBA.getSqlDataTable<RCS_SYS_USER_LIST>(sql);
                list.ForEach(x => x.checkUserInfo());
            }
            catch (Exception ex)
            {
                LogTool.SaveLogMessage(ex, actionName, csName);
                rm.message = "程式發生錯誤，請洽資訊人員!";
                rm.status = RESPONSE_STATUS.EXCEPTION;
            }
            return Json(list);
        }

        /// <summary>
        /// 儲存使用者
        /// </summary>
        /// <param name="objStr"></param>
        /// <returns></returns>
        public JsonResult userSave(string objStr)
        {
            RESPONSE_MSG rm = new RESPONSE_MSG();
            RCS_SYS_USER_LIST item = new RCS_SYS_USER_LIST();
            string actionName = "userSave";
            try
            {
                //傳入值
                objStr = HttpUtility.UrlDecode(objStr);
                item = JsonConvert.DeserializeObject<RCS_SYS_USER_LIST>(objStr);
                DataTable dt = null;
                DataTable dtAuthority = null;
                SQLProvider SQL = new SQLProvider();
                if (item != null)
                {
                    if (string.IsNullOrWhiteSpace(item.HOSP_NAME)||
                        string.IsNullOrWhiteSpace(item.USER_ID)||
                        string.IsNullOrWhiteSpace(item.START_DATE)||
                        string.IsNullOrWhiteSpace(item.END_DATE)||
                        string.IsNullOrWhiteSpace(item.USER_PWD)||
                        string.IsNullOrWhiteSpace(item.USER_ROLE)||
                        string.IsNullOrWhiteSpace(item.DATASTATUS))
                    {
                        rm.message = "有未填寫的欄位資料!";
                        rm.status = RESPONSE_STATUS.ERROR;
                        return Json(rm);
                    }

                    //檢查帳號使用日期區間
                    #region 檢查帳號使用日期區間
                    DateTime nowDate = DateTime.Now;
                    //檢察是否在區間內
                    if (item.DATASTATUS == "1" && !(item.S_DATE < nowDate && item.E_DATE.AddDays(1) > nowDate))
                    {
                        rm.status = RESPONSE_STATUS.ERROR;
                        rm.message = "無法設定\"使用中\"權限，會讓帳號立即停用，請檢查此帳號權限日期區間是否正確!";
                    }
                    //DATASTATUS == "2" 延長使用一個月，是否有在區間內
                    if (item.DATASTATUS == "2" && !(item.S_DATE < nowDate && item.E_DATE.AddMonths(1).AddDays(1) > nowDate))
                    {
                        rm.status = RESPONSE_STATUS.ERROR;
                        rm.message = "無法設定\"延長使用一個月\"權限，會讓帳號立即停用，請檢查此帳號權限日期區間是否正確!";
                    }
                    #endregion
                    if (rm.status == RESPONSE_STATUS.ERROR)
                    {
                        return Json(rm);
                    }

                    string sql = "";
                    //SqlParameter sp = new SqlParameter();
                    if (string.IsNullOrWhiteSpace(item.SYS_ID))
                    {
                        sql = "SELECT * FROM RCS_SYS_USER_LIST WHERE 1 = 1 OR USER_ID = '" + item.USER_ID + "'";
                    }
                    else
                    {
                        sql = "SELECT * FROM RCS_SYS_USER_LIST WHERE SYS_ID = '" + item.SYS_ID + "'";
                    }
                    dt = SQL.DBA.getSqlDataTable(sql);
                    sql = string.Format("select * from RCS_SYS_PARAMS where P_MODEL = 'user' and P_VALUE='{0}' AND P_STATUS='1'", item.USER_ID);
                    dtAuthority = SQL.DBA.getSqlDataTable(sql);
                    if (dt != null)
                    {
                        DataRow dr = null;
                        item.START_DATE = DateTime.Parse(item.START_DATE).ToString("yyyy-MM-dd");
                        item.END_DATE = DateTime.Parse(item.END_DATE).ToString("yyyy-MM-dd");
                        if (string.IsNullOrWhiteSpace(item.SYS_ID))
                        {
                            if (dt.Rows.Count > 0 && dt.AsEnumerable().ToList().Exists(x => x["USER_ID"].ToString() == item.USER_ID))
                            {
                                rm.message = "使用者帳號重複，請重新輸入!";
                                rm.status = RESPONSE_STATUS.ERROR;
                                LogTool.SaveLogMessage(rm.message, actionName,csName);
                                return Json(rm);
                            }
                            item.SYS_ID = "RCS" + (dt.Rows.Count + 1).ToString();
                            dr = dt.NewRow();
                            dr["SYS_ID"] = item.SYS_ID;
                            item.CREATE_DATE = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                            dr["CREATE_DATE"] = item.CREATE_DATE;
                            dr["CREATE_ID"] = user_info.user_id;
                            dr["CREATE_NAME"] = user_info.user_name;
                            dr["HOSP_NAME"] = item.HOSP_NAME;
                            dr["USER_ID"] = item.USER_ID;
                            if (!string.IsNullOrWhiteSpace(item.USER_PWD))
                            {
                                System.Security.Cryptography.SHA512 sha512 = new SHA512CryptoServiceProvider();
                                string resultSha512 = Convert.ToBase64String(sha512.ComputeHash(Encoding.Default.GetBytes(item.USER_PWD)));
                                dr["USER_PWD"] = resultSha512;
                            }
                            dr["START_DATE"] = item.START_DATE;
                            dr["END_DATE"] = item.END_DATE;
                            dr["USER_ROLE"] = item.USER_ROLE;
                            dr["DATASTATUS"] = item.DATASTATUS;
                            dt.Rows.Add(dr);
                            dr = dt.AsEnumerable().ToList().Find(x => x["SYS_ID"].ToString() == item.SYS_ID);
                        }
                        else
                        {
                            dr = dt.Rows[0];
                            item.MODIFY_DATE = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                            dr["MODIFY_DATE"] = item.MODIFY_DATE;
                            dr["MODIFY_ID"] = user_info.user_id;
                            dr["MODIFY_NAME"] = user_info.user_name;
                        }
                        dr["HOSP_NAME"] = item.HOSP_NAME;
                        dr["USER_ID"] = item.USER_ID;
                        if (!string.IsNullOrWhiteSpace(item.USER_PWD))
                        {
                            System.Security.Cryptography.SHA512 sha512 = new SHA512CryptoServiceProvider();
                            string resultSha512 = Convert.ToBase64String(sha512.ComputeHash(Encoding.Default.GetBytes(item.USER_PWD)));
                            dr["USER_PWD"] = resultSha512;
                        }
                        dr["START_DATE"] = item.START_DATE;
                        dr["END_DATE"] = item.END_DATE;
                        dr["USER_ROLE"] = item.USER_ROLE;
                        dr["DATASTATUS"] = item.DATASTATUS;

                        if (dtAuthority != null)
                        {
                            if (dtAuthority.AsEnumerable().ToList().Exists(x => x["P_VALUE"].ToString() == item.USER_ID))
                            {
                                dtAuthority.AsEnumerable().ToList().Find(x => x["P_VALUE"].ToString() == item.USER_ID)["P_GROUP"] = item.USER_ROLE;
                            }
                            else
                            {
                                dr = dtAuthority.NewRow();
                                dr["P_ID"] = DateTime.Now.ToString("yyyyMMddHHmmss") + user_info.user_id;
                                dr["P_MODEL"] = "user";
                                dr["P_GROUP"] = item.USER_ROLE;
                                dr["P_NAME"] = item.HOSP_NAME;
                                dr["P_VALUE"] = item.USER_ID;
                                dr["P_LANG"] = "zh-tw";
                                dr["P_MEMO"] = "";
                                dr["P_STATUS"] = "1";
                                dr["P_MANAGE"] = "0";
                                dtAuthority.Rows.Add(dr);
                            }
                        }
                        else
                        {
                            rm.message = "儲存失敗!請洽資訊人員!";
                            rm.status = RESPONSE_STATUS.ERROR;
                            LogTool.SaveLogMessage(SQL.DBA.lastError, actionName, csName);
                        }

                        if (rm.status == RESPONSE_STATUS.SUCCESS)
                        {
                            bool isOK = true;
                            SQL.DBA.BeginTrans();
                            SQL.DBA.Update(dt, "RCS_SYS_USER_LIST");
                            if (!string.IsNullOrWhiteSpace(SQL.DBA.lastError)) isOK = false;
                            SQL.DBA.Update(dtAuthority, "RCS_SYS_PARAMS");
                            if (!string.IsNullOrWhiteSpace(SQL.DBA.lastError)) isOK = false;

                            if (isOK)
                            {
                                rm.message = "儲存成功!";
                                SQL.DBA.Commit();
                                rm.attachment = item;
                            }
                            else
                            {
                                rm.message = "儲存失敗!請洽資訊人員!";
                                rm.status = RESPONSE_STATUS.ERROR;
                                LogTool.SaveLogMessage(SQL.DBA.lastError, actionName, csName);
                                //logger.Error(this.DBA.LastError);
                                SQL.DBA.Rollback();
                            }
                            SQL.DBA.Close();
                        }
                    }
                    else
                    {
                        rm.message = "儲存失敗!請洽資訊人員!";
                        rm.status = RESPONSE_STATUS.ERROR;
                        LogTool.SaveLogMessage(SQL.DBA.lastError, actionName, csName);
                        //logger.Error(this.DBA.LastError);
                    }
                }
                else
                {
                    rm.message = "儲存失敗!請洽資訊人員!";
                    rm.status = RESPONSE_STATUS.ERROR;
                    LogTool.SaveLogMessage("儲存失敗", actionName, csName);
                    //logger.Error("儲存失敗!");
                }
            }
            catch (Exception ex)
            {
                LogTool.SaveLogMessage(ex, actionName, csName);
                rm.message = "儲存失敗!請洽資訊人員!";
                rm.status = RESPONSE_STATUS.EXCEPTION;
            }
            return Json(rm);
        }


        public JsonResult userDel(string SYS_ID)
        {
            RESPONSE_MSG rm = new RESPONSE_MSG();
            string actionName = "userDel";
            try
            {
                if (!string.IsNullOrWhiteSpace(SYS_ID))
                {
                    SQLProvider SQL = new SQLProvider();
                    string sql = "UPDATE RCS_SYS_USER_LIST SET DATASTATUS = '9',MODIFY_DATE=@MODIFY_DATE,MODIFY_ID=@MODIFY_ID,MODIFY_NAME=@MODIFY_NAME WHERE SYS_ID = @SYS_ID";
                    Dapper.DynamicParameters dp = new Dapper.DynamicParameters();
                    dp.Add("SYS_ID", SYS_ID);
                    dp.Add("MODIFY_DATE", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
                    dp.Add("MODIFY_ID", user_info.user_id);
                    dp.Add("MODIFY_NAME", user_info.user_name);
                    SQL.DBA.DBExecute(sql, dp);
                    if (SQL.DBA.hasLastError)
                    {
                        rm.status = RESPONSE_STATUS.ERROR;
                        rm.message = "刪除失敗!";
                        LogTool.SaveLogMessage(SQL.DBA.lastError, actionName, csName);
                    }
                    else
                    {
                        rm.status = RESPONSE_STATUS.SUCCESS;
                        rm.message = "刪除成功!";
                    }
                }
                else
                {
                    rm.status = RESPONSE_STATUS.ERROR;
                    rm.message = "沒有傳入值!";
                }
            }
            catch (Exception ex)
            {
                rm.status = RESPONSE_STATUS.EXCEPTION;
                rm.message = "系統發生錯誤，請洽資訊人員!";
                LogTool.SaveLogMessage(ex, actionName, csName);
            }
            return Json(rm);
        }
    }
}

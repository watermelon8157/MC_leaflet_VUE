﻿using RCS_Data.Models;
using RCS_Data.Models.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Script.Services;
using System.Web.Services; 
namespace RCS
{
    /// <summary>
    ///MC_WebService 的摘要描述
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // 若要允許使用 ASP.NET AJAX 從指令碼呼叫此 Web 服務，請取消註解下列一行。
    // [System.Web.Script.Services.ScriptService]
    public class MC_WebService : System.Web.Services.WebService
    {

        string csName = "MC_WebService";

        RCS_Data.SQLProvider _dblink { get; set; }

        RCS_Data.SQLProvider dblink { 
            get 
            {
                if (this._dblink == null)
                {
                    this._dblink = new RCS_Data.SQLProvider("MC_DbConnection");
                }
                return this._dblink; 
            } 
        }

        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public void HelloWorld()
        {
            // 指定編碼
            Context.Response.HeaderEncoding = Encoding.UTF8;
            // 加入BOM
            Context.Response.BinaryWrite(new byte[] { 0xEF, 0xBB, 0xBF });
            Context.Response.Write("Hello World"); 
        }


        [WebMethod(Description = "全台灣醫院清單 MASTER")]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public void GET_ALL_HOSPDATA()
        {
            // 指定編碼
            Context.Response.HeaderEncoding = Encoding.UTF8;
            // 加入BOM
            Context.Response.BinaryWrite(new byte[] { 0xEF, 0xBB, 0xBF });
            string actionName = "GET_ALL_HOSP_DATA";
            WSresponse rm = new WSresponse();
            List<MC_HOSP_INFO> pList = new List<MC_HOSP_INFO>();
            List<MC_PATIENT_INFO> tempList = new List<MC_PATIENT_INFO>();
            string sql =string.Concat("SELECT * FROM " , DB_TABLE_NAME.DB_MC_HOSP_INFO);
            pList = this.dblink.DBA.getSqlDataTable<MC_HOSP_INFO>(sql);
            
            if (this.dblink.DBA.hasLastError)
            {
                rm.success = false;
                rm.msg = "查詢失敗!";
                Com.Mayaminer.LogTool.SaveLogMessage(this.dblink.DBA.lastError, actionName,this.csName);
            }
            else
            {
                if (pList.Count > 0)
                {

                    sql = string.Concat("SELECT * FROM ", DB_TABLE_NAME.DB_MC_PATIENT_INFO, " WHERE HOSP_KEY in @HOSP_KEY");
                    Dapper.DynamicParameters dp = new Dapper.DynamicParameters();
                    dp.Add("HOSP_KEY", pList.Select(x => x.HOSP_KEY).Distinct().ToList()) ;
                    tempList = this.dblink.DBA.getSqlDataTable<MC_PATIENT_INFO>(sql,dp);
                    foreach (MC_HOSP_INFO item in pList)
                    {
                        if (tempList.Exists(x=>x.HOSP_KEY == item.HOSP_KEY))
                        {
                            item.ALL_PATIENT = tempList.FindAll(x => x.HOSP_KEY == item.HOSP_KEY).ToList().Count;
                            if (tempList.Exists(x => x.HOSP_KEY == item.HOSP_KEY && !string.IsNullOrWhiteSpace(x.EXPECTED_ARRIVAL_DATETIME)))
                            { 
                                item.ARRIVAL_PATIENT = tempList.FindAll(x => x.HOSP_KEY == item.HOSP_KEY && !string.IsNullOrWhiteSpace(x.EXPECTED_ARRIVAL_DATETIME)).Select(x => DateTime.Parse(x.EXPECTED_ARRIVAL_DATETIME) <= DateTime.Now).Count();
                            }
                            item.SELECT_PATIENT = item.ALL_PATIENT - item.ARRIVAL_PATIENT; 
                        }
                    }
                }
                rm.success = true;
                rm.msg = "查詢成功!";
                rm.data = pList;
            } 
            Context.Response.Write(Newtonsoft.Json.JsonConvert.SerializeObject(rm)); 
        }

        [WebMethod(Description = "HOSPDATA BY HOSP_KEY")]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public void GET_HOSPDATA_BY_HOSP_KEY(string HOSP_KEY)
        {
            Dapper.DynamicParameters dp = new Dapper.DynamicParameters();
            // 指定編碼
            Context.Response.HeaderEncoding = Encoding.UTF8;
            // 加入BOM
            Context.Response.BinaryWrite(new byte[] { 0xEF, 0xBB, 0xBF });
            string actionName = "GET_ALL_HOSP_DATA";
            WSresponse rm = new WSresponse();
            List<MC_HOSP_INFO> pList = new List<MC_HOSP_INFO>();
            List<MC_PATIENT_INFO> tempList = new List<MC_PATIENT_INFO>();
            string sql = string.Concat("SELECT * FROM ", DB_TABLE_NAME.DB_MC_HOSP_INFO); 
            dp = new Dapper.DynamicParameters();
            if (!string.IsNullOrWhiteSpace(HOSP_KEY))
            { 
                dp.Add("HOSP_KEY", HOSP_KEY);
                sql += " WHERE HOSP_KEY =@HOSP_KEY";
            }  
            pList = this.dblink.DBA.getSqlDataTable<MC_HOSP_INFO>(sql, dp);

            if (this.dblink.DBA.hasLastError)
            {
                rm.success = false;
                rm.msg = "查詢失敗!";
                Com.Mayaminer.LogTool.SaveLogMessage(this.dblink.DBA.lastError, actionName, this.csName);
            }
            else
            {
                if (pList.Count > 0)
                { 
                    sql = string.Concat("SELECT * FROM ", DB_TABLE_NAME.DB_MC_PATIENT_INFO, " WHERE HOSP_KEY in @HOSP_KEY");
                     dp = new Dapper.DynamicParameters();
                    dp.Add("HOSP_KEY", pList.Select(x => x.HOSP_KEY).Distinct().ToList());
                    tempList = this.dblink.DBA.getSqlDataTable<MC_PATIENT_INFO>(sql, dp);
                    foreach (MC_HOSP_INFO item in pList)
                    {
                        if (tempList.Exists(x => x.HOSP_KEY == item.HOSP_KEY))
                        {
                            item.ALL_PATIENT = tempList.FindAll(x => x.HOSP_KEY == item.HOSP_KEY).ToList().Count;
                            if (tempList.Exists(x => x.HOSP_KEY == item.HOSP_KEY && !string.IsNullOrWhiteSpace(x.EXPECTED_ARRIVAL_DATETIME)))
                            {
                                item.ARRIVAL_PATIENT = tempList.FindAll(x => x.HOSP_KEY == item.HOSP_KEY && !string.IsNullOrWhiteSpace(x.EXPECTED_ARRIVAL_DATETIME)).Select(x => DateTime.Parse(x.EXPECTED_ARRIVAL_DATETIME) <= DateTime.Now).Count();
                            }
                            item.SELECT_PATIENT = item.ALL_PATIENT - item.ARRIVAL_PATIENT;
                        }
                    }
                }
                rm.success = true;
                rm.msg = "查詢成功!";
                rm.data = pList;
            }
            Context.Response.Write(Newtonsoft.Json.JsonConvert.SerializeObject(rm));
        }

        /// <summary>
        /// 新增資料
        /// </summary>
        /// <param name="PATIENT_NAME"></param>
        /// <param name="PATIENT_ID"></param> 
        /// <param name="LATITUDE"></param>
        /// <param name="LONGITUDE"></param>
        /// <param name="AGE"></param>
        /// <param name="GENDER"></param>
        /// <param name="CITY"></param>
        /// <param name="COUNTRY"></param>
        /// <param name="TRIAGE"></param>
        /// <param name="EXPECTED_ARRIVAL_DATETIME"></param> 
        /// <param name="SELECTION_DATETIME"></param>
        /// <param name="HOSP_KEY"></param> 
        /// <param name="SCORE"></param>
        /// <returns></returns>
        [WebMethod(Description = "新增傷患資料")]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public void INSERT_PATDATA(
                string PATIENT_NAME,
                string PATIENT_ID, 
                string LATITUDE,
                string LONGITUDE,
                string AGE,
                string GENDER,
                string CITY,
                string COUNTRY,
                string TRIAGE,
                string EXPECTED_ARRIVAL_DATETIME, 
                string SELECTION_DATETIME,
                string HOSP_KEY,
                string SCORE)
        {
            // 指定編碼
            Context.Response.HeaderEncoding = Encoding.UTF8;
            // 加入BOM
            Context.Response.BinaryWrite(new byte[] { 0xEF, 0xBB, 0xBF });
            string actionName = "INSERT_PAT_DATA";
            WSresponse rm = new WSresponse(); 
            List<string> msg = new List<string>();
            if (string.IsNullOrWhiteSpace(PATIENT_ID))
            {
                msg.Add("請輸入傷患ID!");
            }
            if (string.IsNullOrWhiteSpace(PATIENT_NAME))
            {
                msg.Add("請輸入傷患姓名!");
            }
            if (msg.Count > 0)
            {
                rm.success = false;
                rm.msg = string.Join(",", msg);
                Context.Response.Write(Newtonsoft.Json.JsonConvert.SerializeObject(rm));
            }
            else
            {
                string SITE_ID = RCS_Data.Models.Function_Library.getDateNowString(RCS_Data.Models.DATE_FORMAT.yyyyMMddHHmmssfffff);
                #region 民眾版 新增  lcoation
                List<DB_MC_SITE_INFO> sList = new List<DB_MC_SITE_INFO>();
                sList.Add(new DB_MC_SITE_INFO()
                {
                    SITE_ID = SITE_ID,
                    CREATE_DATE = Function_Library.getDateNowString(DATE_FORMAT.yyyy_MM_dd_HHmmss),
                    CREATE_ID = "ws",
                    CREATE_NAME = "ws",
                    MODIFY_DATE = Function_Library.getDateNowString(DATE_FORMAT.yyyy_MM_dd_HHmmss),
                    MODIFY_ID = "ws",
                    MODIFY_NAME = "ws",
                    DATASTATUS = "1",
                    LATITUDE = string.IsNullOrWhiteSpace(LATITUDE) ? "25.0169826" : LATITUDE,
                    LONGITUDE = string.IsNullOrWhiteSpace(LONGITUDE) ? "121.46278679" : LONGITUDE,
                });
                #endregion
                this.dblink.DBA.DBExecInsert<DB_MC_SITE_INFO>(sList);

                List<DB_MC_PATIENT_INFO> pList = new List<DB_MC_PATIENT_INFO>();
                #region 新增傷患資料
                pList.Add(new DB_MC_PATIENT_INFO()
                {
                    PATIENT_ID = PATIENT_ID,
                    PATIENT_NAME = PATIENT_NAME,
                    SITE_ID = SITE_ID,
                    AGE = AGE,
                    GENDER = GENDER,
                    CITY = CITY,
                    COUNTRY = COUNTRY,
                    TRIAGE = string.IsNullOrWhiteSpace(TRIAGE) ? "Moderate" : TRIAGE,
                    TRANSPORTATION = "",
                    AMB_ID = "",
                    EXPECTED_ARRIVAL_DATETIME = EXPECTED_ARRIVAL_DATETIME,
                    SELECTION_DATETIME = string.IsNullOrWhiteSpace(SELECTION_DATETIME) ? Function_Library.getDateNowString(DATE_FORMAT.yyyy_MM_dd_HHmmss) : SELECTION_DATETIME,
                    HOSP_KEY = HOSP_KEY,
                    SCORE = SCORE,
                    CREATE_ID = "ws",
                    CREATE_NAME = "ws",
                    CREATE_DATE = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                    MODIFY_ID = "ws",
                    MODIFY_NAME = "ws",
                    MODIFY_DATE = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                    DATASTATUS = "1",
                });
                #endregion
                this.dblink.DBA.DBExecInsert<DB_MC_PATIENT_INFO>(pList);
                if (this.dblink.DBA.hasLastError)
                {
                    rm.success = false;
                    rm.msg = this.dblink.DBA.lastError;
                    Com.Mayaminer.LogTool.SaveLogMessage(this.dblink.DBA.lastError, actionName, this.csName);
                    Context.Response.Write(Newtonsoft.Json.JsonConvert.SerializeObject(rm));
                }
                else
                {

                    if (msg.Count > 0)
                    {
                        rm.success = false;
                        rm.msg = string.Join(",", msg);
                        Com.Mayaminer.LogTool.SaveLogMessage(rm.msg, actionName, this.csName);
                        Context.Response.Write(Newtonsoft.Json.JsonConvert.SerializeObject(rm));
                    }
                    else
                    {

                        rm.success = true;
                        rm.msg = "新增成功";
                        rm.PATIENT_ID = PATIENT_ID;
                        Context.Response.Write(Newtonsoft.Json.JsonConvert.SerializeObject(rm));
                    } 
                } 
            }
            
        }


        /// <summary>
        /// 取得醫院分數
        /// </summary>
        /// <param name="PATIENT_ID"></param>
        /// <returns></returns>
        [WebMethod(Description = "取得醫院分數")]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public void GET_HOSPSOURCE_BY_PATIENT_ID(string PATIENT_ID)
        {
            // 指定編碼
            Context.Response.HeaderEncoding = Encoding.UTF8;
            // 加入BOM
            Context.Response.BinaryWrite(new byte[] { 0xEF, 0xBB, 0xBF });
            string actionName = "GET_HOSPSOURCE_BY_PATIENT_ID";
            WSresponse rm = new WSresponse();
            DateTime dateNow = DateTime.Parse("2020-07-08");
            Dapper.DynamicParameters dp = new Dapper.DynamicParameters();
            List<MC_SOURCE_LIST> pList = new List<MC_SOURCE_LIST>();
            List<DB_MC_PATIENT_INFO> tempList = new List<DB_MC_PATIENT_INFO>();
            string sql = string.Concat("SELECT * FROM ", DB_TABLE_NAME.DB_MC_PATIENT_INFO, " WHERE PATIENT_ID =@PATIENT_ID");
            dp.Add("PATIENT_ID", PATIENT_ID);
            tempList = this.dblink.DBA.getSqlDataTable<DB_MC_PATIENT_INFO>(sql, dp);
            if (tempList.Count > 0)
            { 
                sql = string.Concat("SELECT * FROM ", DB_TABLE_NAME.DB_MC_SOURCE_LIST, " WHERE SITE_ID =@SITE_ID"); 
                dp.Add("SITE_ID", tempList[0].SITE_ID);
                pList = this.dblink.DBA.getSqlDataTable<MC_SOURCE_LIST>(sql, dp);
            }
            if (this.dblink.DBA.hasLastError)
            {
                rm.success = false;
                rm.msg = this.dblink.DBA.lastError;
                Com.Mayaminer.LogTool.SaveLogMessage(this.dblink.DBA.lastError, actionName, this.csName); 
            }
            else
            { 
                rm.success = true;
                rm.msg = "查詢成功!";
                rm.data = pList;
            }
            Context.Response.Write(Newtonsoft.Json.JsonConvert.SerializeObject(rm));
            // return Newtonsoft.Json.JsonConvert.SerializeObject(rm);
        }



        /// <summary>
        /// 修改傷患資料
        /// </summary>
        /// <param name="PATIENT_ID"></param>
        /// <param name="PATIENT_NAME"></param>
        /// <param name="AGE"></param>
        /// <param name="GENDER"></param>
        /// <param name="CITY"></param>
        /// <param name="COUNTRY"></param>
        /// <param name="TRIAGE"></param>
        /// <param name="EXPECTED_ARRIVAL_DATETIME"></param>
        /// <param name="LOGIN_DATETIME"></param>
        /// <param name="SELECTION_DATETIME"></param>
        /// <param name="HOSP_KEY"></param>
        /// <param name="HOSPITAL_SHOW_NAME"></param>
        /// <param name="HOSP_TO_PAT_SCORE"></param>
        /// <param name="HOSP_TO_PAT_SCORE_LEVEL"></param>
        /// <param name="SCORE"></param>
        /// <returns></returns>
        [WebMethod(Description = "修改傷患資料")]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public void UPDATE_PATDATA(string PATIENT_ID,
                                    string PATIENT_NAME,
                                    string AGE,
                                    string GENDER,
                                    string CITY,
                                    string COUNTRY,
                                    string TRIAGE,
                                    string EXPECTED_ARRIVAL_DATETIME, 
                                    string SELECTION_DATETIME,
                                    string HOSP_KEY,
                                    string SCORE)
        {
            // 指定編碼
            Context.Response.HeaderEncoding = Encoding.UTF8;
            // 加入BOM
            Context.Response.BinaryWrite(new byte[] { 0xEF, 0xBB, 0xBF });
            string actionName = "UPDATE_PAT_DATA";
            List<string> msg = new List<string>();
            WSresponse rm = new WSresponse();
            RCS_Data.SQLProvider dblink = new RCS_Data.SQLProvider("MC_DbConnection");
            if (string.IsNullOrWhiteSpace(PATIENT_ID))
            {
                msg.Add("沒有傷患編號!無法修改!");
            }
            if (msg.Count > 0)
            {
                rm.success = false;
                rm.msg = string.Join(",", msg);
                Com.Mayaminer.LogTool.SaveLogMessage(this.dblink.DBA.lastError, actionName, this.csName);
                // return Newtonsoft.Json.JsonConvert.SerializeObject(rm);
            }
            else
            {

                List<DB_MC_PATIENT_INFO> pList = dblink.DBA.getSqlDataTable<DB_MC_PATIENT_INFO>(string.Format("SELECT * FROM MC_PATIENT_INFO WHERE PATIENT_ID = '{0}';", PATIENT_ID));
                if (pList.Count > 0)
                {
                    if (!string.IsNullOrWhiteSpace(PATIENT_NAME)) pList[0].PATIENT_NAME = PATIENT_NAME;
                    if (!string.IsNullOrWhiteSpace(AGE)) pList[0].AGE = AGE;
                    if (!string.IsNullOrWhiteSpace(GENDER)) pList[0].GENDER = GENDER;
                    if (!string.IsNullOrWhiteSpace(CITY)) pList[0].CITY = CITY;
                    if (!string.IsNullOrWhiteSpace(COUNTRY)) pList[0].COUNTRY = COUNTRY;
                    if (!string.IsNullOrWhiteSpace(TRIAGE)) pList[0].TRIAGE = TRIAGE;
                    if (!string.IsNullOrWhiteSpace(EXPECTED_ARRIVAL_DATETIME)) pList[0].EXPECTED_ARRIVAL_DATETIME = EXPECTED_ARRIVAL_DATETIME;
                    if (!string.IsNullOrWhiteSpace(SELECTION_DATETIME)) pList[0].SELECTION_DATETIME = SELECTION_DATETIME;
                    if (!string.IsNullOrWhiteSpace(HOSP_KEY)) pList[0].HOSP_KEY = HOSP_KEY;
                    if (!string.IsNullOrWhiteSpace(SCORE)) pList[0].SCORE = SCORE;
                    pList[0].MODIFY_ID = "SYS";
                    pList[0].MODIFY_NAME = "SYS";
                    pList[0].MODIFY_DATE = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                    pList[0].DATASTATUS = "1";
                }
                dblink.DBA.DBExecUpdate<DB_MC_PATIENT_INFO>(pList);
                if (dblink.DBA.hasLastError)
                {
                    rm.success = false;
                    rm.msg = dblink.DBA.lastError;
                    Context.Response.Write(Newtonsoft.Json.JsonConvert.SerializeObject(rm));
                    //  return Newtonsoft.Json.JsonConvert.SerializeObject(rm);
                }
                else
                {

                    if (msg.Count > 0)
                    {
                        rm.success = false;
                        rm.msg = string.Join(",", msg);
                        Context.Response.Write(Newtonsoft.Json.JsonConvert.SerializeObject(rm));
                        // return Newtonsoft.Json.JsonConvert.SerializeObject(rm);
                    }
                    else
                    { 
                        rm.success = true;
                        rm.msg = "success";
                        rm.PATIENT_ID = PATIENT_ID;
                        Context.Response.Write(Newtonsoft.Json.JsonConvert.SerializeObject(rm));
                        // return Newtonsoft.Json.JsonConvert.SerializeObject(rm);
                    }
                }
            }
        }

        /// <summary>
        /// 取得傷患清單
        /// </summary>
        /// <returns></returns>
        [WebMethod(Description = "取得傷患清單")]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public void GET_PATDATA_LIST()
        {
            // 指定編碼
            Context.Response.HeaderEncoding = Encoding.UTF8;
            // 加入BOM
            Context.Response.BinaryWrite(new byte[] { 0xEF, 0xBB, 0xBF });
            WSresponse rm = new WSresponse();
            string actionName = "GET_PAT_DATA";
            List<MC_PATIENT_INFO> patList = new List<MC_PATIENT_INFO>();
            List<pat_data> pList = new List<pat_data>();
            string sql = "SELECT * FROM " + DB_TABLE_NAME.DB_MC_PATIENT_INFO + " WHERE DATASTATUS = '1'";
            patList = this.dblink.DBA.getSqlDataTable<MC_PATIENT_INFO>(sql);
            if (this.dblink.DBA.hasLastError)
            {
                rm.success = false;
                rm.msg = this.dblink.DBA.lastError;
                Com.Mayaminer.LogTool.SaveLogMessage(this.dblink.DBA.lastError, actionName, this.csName);
            }
            else
            {
                rm.success = true;
                rm.msg = "查詢成功!";
                rm.data = patList;
            }
            Context.Response.Write(Newtonsoft.Json.JsonConvert.SerializeObject(rm));
            // return Newtonsoft.Json.JsonConvert.SerializeObject(rm);
        }


        /// <summary>
        /// 取得傷患清單
        /// </summary>
        /// <returns></returns>
        [WebMethod(Description = "取得傷患清單 BY HOSP_KEY")]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public void GET_PATDATA_LIST_BY_HOSP_KEY(string HOSP_KEY)
        {
            // 指定編碼
            Context.Response.HeaderEncoding = Encoding.UTF8;
            // 加入BOM
            Context.Response.BinaryWrite(new byte[] { 0xEF, 0xBB, 0xBF });
            WSresponse rm = new WSresponse();
            string actionName = "GET_PAT_DATA";
            List<MC_PATIENT_INFO> patList = new List<MC_PATIENT_INFO>();
            Dapper.DynamicParameters dp = new Dapper.DynamicParameters();
            List<pat_data> pList = new List<pat_data>();
            string sql = "SELECT * FROM " + DB_TABLE_NAME.DB_MC_PATIENT_INFO + " WHERE DATASTATUS = '1'";
            if (!string.IsNullOrEmpty(HOSP_KEY))
            {
                sql += " AND HOSP_KEY =@HOSP_KEY";
                dp.Add("HOSP_KEY", HOSP_KEY);
            }
            patList = this.dblink.DBA.getSqlDataTable<MC_PATIENT_INFO>(sql, dp);
            if (this.dblink.DBA.hasLastError)
            {
                rm.success = false;
                rm.msg = this.dblink.DBA.lastError;
                Com.Mayaminer.LogTool.SaveLogMessage(this.dblink.DBA.lastError, actionName, this.csName);
            }
            else
            {
                rm.success = true;
                rm.msg = "查詢成功!";
                rm.data = patList;
            }
            Context.Response.Write(Newtonsoft.Json.JsonConvert.SerializeObject(rm));
            // return Newtonsoft.Json.JsonConvert.SerializeObject(rm);
        }

        /// <summary>
        /// 修改傷患資料
        /// </summary>
        /// <param name="PATIENT_ID"></param>
        /// <param name="PATIENT_NAME"></param>
        /// <param name="AGE"></param>
        /// <param name="GENDER"></param>
        /// <param name="CITY"></param>
        /// <param name="COUNTRY"></param>
        /// <param name="TRIAGE"></param>
        /// <param name="EXPECTED_ARRIVAL_DATETIME"></param>
        /// <param name="LOGIN_DATETIME"></param>
        /// <param name="SELECTION_DATETIME"></param>
        /// <param name="HOSP_KEY"></param>
        /// <param name="HOSPITAL_SHOW_NAME"></param>
        /// <param name="HOSP_TO_PAT_SCORE"></param>
        /// <param name="HOSP_TO_PAT_SCORE_LEVEL"></param>
        /// <returns></returns>
        [WebMethod(Description = "選擇醫院資料")]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public void SELECT_HOSP_BY_PATIENT_ID(string PATIENT_ID, string HOSP_KEY )
        {
            // 指定編碼
            Context.Response.HeaderEncoding = Encoding.UTF8;
            // 加入BOM
            Context.Response.BinaryWrite(new byte[] { 0xEF, 0xBB, 0xBF });
            string actionName = "SELECT_HOSP_BY_PATIENT_ID";
            List<string> msg = new List<string>();
            WSresponse rm = new WSresponse();
            RCS_Data.SQLProvider dblink = new RCS_Data.SQLProvider("MC_DbConnection");
            if (string.IsNullOrWhiteSpace(PATIENT_ID))
            {
                msg.Add("沒有傷患編號!無法修改!");
            }
            if (string.IsNullOrWhiteSpace(HOSP_KEY))
            {
                msg.Add("沒有選擇醫院!無法修改!");
            }
            if (msg.Count > 0)
            {
                rm.success = false;
                rm.msg = string.Join(",", msg);
                Com.Mayaminer.LogTool.SaveLogMessage(this.dblink.DBA.lastError, actionName, this.csName);
                Context.Response.Write(Newtonsoft.Json.JsonConvert.SerializeObject(rm));
                // return Newtonsoft.Json.JsonConvert.SerializeObject(rm);
            }
            else
            {

                List<DB_MC_PATIENT_INFO> pList = dblink.DBA.getSqlDataTable<DB_MC_PATIENT_INFO>(string.Format("SELECT * FROM MC_PATIENT_INFO WHERE PATIENT_ID = '{0}';", PATIENT_ID));
                if (pList.Count > 0)
                {
                    pList[0].HOSP_KEY = HOSP_KEY;
                    pList[0].MODIFY_ID = "SYS";
                    pList[0].MODIFY_NAME = "SYS";
                    pList[0].MODIFY_DATE = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                    pList[0].DATASTATUS = "1";
                }
                dblink.DBA.DBExecUpdate<DB_MC_PATIENT_INFO>(pList);
                if (dblink.DBA.hasLastError)
                {
                    rm.success = false;
                    rm.msg = dblink.DBA.lastError;
                    Context.Response.Write(Newtonsoft.Json.JsonConvert.SerializeObject(rm));
                    // return Newtonsoft.Json.JsonConvert.SerializeObject(rm);
                }
                else
                {

                    if (msg.Count > 0)
                    {
                        rm.success = false;
                        rm.msg = string.Join(",", msg);
                        Context.Response.Write(Newtonsoft.Json.JsonConvert.SerializeObject(rm));
                        // return Newtonsoft.Json.JsonConvert.SerializeObject(rm);
                    }
                    else
                    {
                        rm.success = true;
                        rm.msg = "修改醫院成功!";
                        rm.PATIENT_ID = PATIENT_ID;
                        Context.Response.Write(Newtonsoft.Json.JsonConvert.SerializeObject(rm));
                        // return Newtonsoft.Json.JsonConvert.SerializeObject(rm);
                    }

                }
            }
        }


        /// <summary>
        /// 取得傷患資料BY ID
        /// </summary>
        /// <param name="PATIENT_ID"></param>
        /// <returns></returns>
        [WebMethod(Description = "取得傷患資料")]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public void GET_PATDATA_BY_PATIENT_ID(string PATIENT_ID)
        {
            // 指定編碼
            Context.Response.HeaderEncoding = Encoding.UTF8;
            // 加入BOM
            Context.Response.BinaryWrite(new byte[] { 0xEF, 0xBB, 0xBF });
            WSresponse rm = new WSresponse();
            string actionName = "GET_PAT_DATA";
            List<MC_PATIENT_INFO> patList = new List<MC_PATIENT_INFO>(); 
            List<pat_data> pList = new List<pat_data>();
            Dapper.DynamicParameters dp = new Dapper.DynamicParameters();
            string sql = "SELECT * FROM " + DB_TABLE_NAME.DB_MC_PATIENT_INFO + " WHERE PATIENT_ID = @PATIENT_ID AND DATASTATUS = '1'";
            dp.Add("PATIENT_ID", PATIENT_ID);
            patList = this.dblink.DBA.getSqlDataTable<MC_PATIENT_INFO>(sql, dp); 
            if (this.dblink.DBA.hasLastError)
            {
                rm.success = false;
                rm.msg = this.dblink.DBA.lastError;
                Com.Mayaminer.LogTool.SaveLogMessage(this.dblink.DBA.lastError, actionName, this.csName);
            }
            else
            {
                rm.success = true;
                rm.msg = "查詢成功!";
                rm.data = patList;
            }
            Context.Response.Write(Newtonsoft.Json.JsonConvert.SerializeObject(rm));
            // return Newtonsoft.Json.JsonConvert.SerializeObject(rm);
        }




       
    }

    public class WSresponse
    {
        /// <summary>
        /// 是否成功
        /// </summary>
        public bool success { get; set; }
        /// <summary>
        /// 回復訊息
        /// </summary>
        public string msg { get; set; }
        /// <summary>
        /// 病歷號資料
        /// </summary>
        public string PATIENT_ID { get; set; }
        /// <summary>
        /// 病歷號資料
        /// </summary>
        public object data { get; set; }
    }
    public class pat_data
    {
        public string success { get; set; }

        public string PATIENT_ID { get; set; }
    }
}

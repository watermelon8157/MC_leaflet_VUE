using RCS.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http; 
using RCS_Data;
using RCS_Data.Models.DB;
using RCSData.Models;
using RCS.Models.ViewModel;
using RCS_Data.Models;

namespace RCS.Controllers.WEBAPI
{

    [JwtAuthActionFilterAttribute(notVerification =true)]
    public class MCController : DefaultController
    {
        string csName = "MCController";


        MCModel _model = new MCModel();
        private UserInfo userinfo { get; set; }

        public MCController()
        {
            this.userinfo = new UserInfo()
            {
                user_id = "admin",
                user_name = "admin"
            };
        }

        public string HelloWord()
        { 
            return "HelloWord";
        }

        #region  action Function

        /// <summary>
        /// 新增病患資料
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public string INSERT_PAT_DATA(DB_MC_PATIENT_INFO model)
        {
            string actionName = "INSERT_PAT_DATA"; 
            model.CREATE_DATE = Function_Library.getDateString(DateTime.Parse(model.SELECTION_DATETIME),DATE_FORMAT.yyyy_MM_dd_HHmmss);
            model.CREATE_ID = this.userinfo.user_id;
            model.CREATE_NAME = this.userinfo.user_name;
            model.MODIFY_DATE = Function_Library.getDateString(DateTime.Parse(model.SELECTION_DATETIME), DATE_FORMAT.yyyy_MM_dd_HHmmss);
            model.MODIFY_ID = this.userinfo.user_id;
            model.MODIFY_NAME = this.userinfo.user_name;
            model.SELECTION_DATETIME = "";
            this.DBLink.DBA.DBExecInsert<DB_MC_PATIENT_INFO>(new List<DB_MC_PATIENT_INFO>() { model });
            if (this.DBLink.DBA.hasLastError)
            {
                Com.Mayaminer.LogTool.SaveLogMessage(this.DBLink.DBA.lastError, actionName, this.csName);
            } 
            if (!MvcApplication.MCSourceThread.IsAlive)
            {
                MvcApplication.MCSourceThread.Start();
            }
            return "儲存成功!";
        }


        /// <summary>
        /// 更新病患資料
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public string UPDATE_PAT_DATA(DB_MC_PATIENT_INFO model)
        {
            string actionName = "UPDATE_PAT_DATA";
            this.DBLink.DBA.DBExecUpdate<DB_MC_PATIENT_INFO>(new List<DB_MC_PATIENT_INFO>() { model });
            if (this.DBLink.DBA.hasLastError)
            {
                Com.Mayaminer.LogTool.SaveLogMessage(this.DBLink.DBA.lastError, actionName, this.csName);
            }

            return "儲存成功!";
        }

        #endregion

        #region  Pat Info Function

        /// <summary>
        /// 病患清單
        /// </summary>
        /// <param name="form"></param>
        /// <returns></returns>
        [JwtAuthActionFilterAttribute]
        [HttpPost]
        public List<DB_MC_PATIENT_INFO> GetPatList(JWT_Form_Body form)
        {
            string actionName = "GetPatList";
            List<DB_MC_PATIENT_INFO> pList = new List<DB_MC_PATIENT_INFO>();
            string sql = "SELECT * FROM " + DB_TABLE_NAME.DB_MC_PATIENT_INFO;
            pList = DBLink.DBA.getSqlDataTable<DB_MC_PATIENT_INFO>(sql);
            return pList;
        }

        /// <summary>
        /// 病患所有資料
        /// </summary>
        /// <param name="form"></param>
        /// <returns></returns>
        [JwtAuthActionFilterAttribute]
        [HttpPost]
        public List<DB_MC_PATIENT_INFO> GetPatListAll(JWT_Form_Body form)
        {
            string actionName = "GetPatListAll";
            List<DB_MC_PATIENT_INFO> pList = new List<DB_MC_PATIENT_INFO>();
            string sql = "SELECT * FROM " + DB_TABLE_NAME.DB_MC_PATIENT_INFO;
            pList = DBLink.DBA.getSqlDataTable<DB_MC_PATIENT_INFO>(sql);
            return pList;
        }

        /// <summary>
        /// 病患 GetPatListByID
        /// </summary>
        /// <param name="form"></param>
        /// <returns></returns>
        [JwtAuthActionFilterAttribute]
        [HttpPost]
        public List<MC_PATIENT_INFO_VIEW> GetPatListByID(JWT_Form_Body form)
        {
            string actionName = "GetPatListByID";
            List<MC_PATIENT_INFO_VIEW> pList = new List<MC_PATIENT_INFO_VIEW>();
            Dapper.DynamicParameters dp = new Dapper.DynamicParameters();
            string sql = "SELECT * FROM " + DB_TABLE_NAME.DB_MC_PATIENT_INFO + " WHERE 1=1 ";
            if (!string.IsNullOrWhiteSpace(form.site_id))
            {
                sql += " AND SITE_ID =@SITE_ID";
                dp.Add("SITE_ID", form.site_id);
            }
            if (!string.IsNullOrWhiteSpace(form.hosp_id))
            {
                sql += " AND HOSP_KEY LIKE @HOSP_KEY";
                dp.Add("HOSP_KEY", form.hosp_id + '%');
            } 
            pList = DBLink.DBA.getSqlDataTable<MC_PATIENT_INFO_VIEW>(sql, dp);
            pList.ForEach(x=> {
                x.EXPECTED_ARRIVAL_DATETIME = !string.IsNullOrWhiteSpace(x.EXPECTED_ARRIVAL_DATETIME) ?  Function_Library.getDateString(DateTime.Parse(x.EXPECTED_ARRIVAL_DATETIME), DATE_FORMAT.yyyy_MM_dd_HHmm) : "";
                x.SELECTION_DATETIME = !string.IsNullOrWhiteSpace(x.SELECTION_DATETIME) ? Function_Library.getDateString(DateTime.Parse(x.SELECTION_DATETIME), DATE_FORMAT.yyyy_MM_dd_HHmm) : "";
            });
            return pList;
        } 

        /// <param name="form"></param>
        /// <returns></returns>
        [JwtAuthActionFilterAttribute(notVerification =true)]
        [HttpPost]
        public List<VIEW_MC_HOSP_INFO> GetHospList(JWT_Form_Body form)
        {
            List < VIEW_MC_HOSP_INFO > pList = Newtonsoft.Json.JsonConvert.DeserializeObject<List<VIEW_MC_HOSP_INFO>>(Newtonsoft.Json.JsonConvert.SerializeObject(MvcApplication.hospList.ToList()));
            string actionName = "GetHospList";
            pList.ForEach(x=> {
                x.hosp_desc = x.HOSP_KEY;
                x.hosp_name = x.HOSP_NAME;
                x.hosp_class = x.DIVISION.Trim();
                x.hosp_city = x.CITY;
                x.hosp_injury = x.CITY;
                x.hosp_ranking = x.ORIGINAL_RANKING;
                x.hosp_erbed = x.ORIGINAL_RANKING; 
            });
            return pList;
        }

        /// <param name="form"></param>
        /// <returns></returns>
        [JwtAuthActionFilterAttribute]
        [HttpPost]
        public Dictionary<string, DB_MC_SOURCE_LIST> GetHospListDTLByID(JWT_Form_Body form)
        {
            Dictionary<string, DB_MC_SOURCE_LIST> dic = new Dictionary<string, DB_MC_SOURCE_LIST>();
            string actionName = "GetHospListDTLByID";
            List<DB_MC_SOURCE_LIST> pList = new List<DB_MC_SOURCE_LIST>();
            Dapper.DynamicParameters dp = new Dapper.DynamicParameters();
            string sql = "SELECT * FROM " + DB_TABLE_NAME.DB_MC_SOURCE_LIST + " WHERE DATASTATUS = '1' AND  SITE_ID =@SITE_ID";
            dp.Add("SITE_ID", form.site_id); 
            pList = DBLink.DBA.getSqlDataTable<DB_MC_SOURCE_LIST>(sql, dp);
            foreach (DB_MC_SOURCE_LIST item in pList)
            {
                dic.Add(item.HOSP_KEY, item);
            }
            return dic;
        }
        #endregion


        /// <summary>
        /// 登入驗證
        /// </summary>
        /// <param name="form">登入資料</param>
        /// <returns></returns>
        [JwtAuthActionFilterAttribute(notVerification = true)]
        public object Login(Login_Form_Body form)
        {
            if (!string.IsNullOrWhiteSpace(form.site_id) || !string.IsNullOrWhiteSpace(form.hosp_id))
            {
                if (!string.IsNullOrWhiteSpace(form.site_id))
                {
                    if (_model.getMC_SITE_INFO(form.site_id).Count == 0)
                    { 
                        if (!string.IsNullOrWhiteSpace(form.LATITUDE) && !string.IsNullOrWhiteSpace(form.LONGITUDE))
                        {
                            List<DB_MC_SITE_INFO> pList = new List<DB_MC_SITE_INFO>();
                            pList.Add(new DB_MC_SITE_INFO() {
                                SITE_ID = form.site_id, 
                                CREATE_DATE = Function_Library.getDateNowString(DATE_FORMAT.yyyy_MM_dd_HHmmss),
                                CREATE_ID = this.userinfo.user_id,
                                CREATE_NAME = this.userinfo.user_name,
                                MODIFY_DATE = Function_Library.getDateNowString(DATE_FORMAT.yyyy_MM_dd_HHmmss),
                                MODIFY_ID = this.userinfo.user_id,
                                MODIFY_NAME = this.userinfo.user_name,
                                DATASTATUS = "1",
                                LATITUDE = form.LATITUDE,
                                LONGITUDE = form.LONGITUDE,
                            });
                            this.DBLink.DBA.DBExecInsert<DB_MC_SITE_INFO>(pList);
                            if (this.DBLink.DBA.hasLastError)
                            {
                                this.throwHttpResponseException("程式發生錯誤，請洽資訊人員!");
                                Com.Mayaminer.LogTool.SaveLogMessage(this.DBLink.DBA.lastError, "Login", this.csName);
                            }
                        }
                        else
                        {
                            this.throwHttpResponseException("查無經緯度資料無法填寫資料!!");
                        }
                    } 
                }
                return new
                {
                    Result = true,
                    token = JwtAuthActionFilterAttribute.EncodeToken(new PAYLOAD()
                    {
                        hosp_id = form.hosp_id,
                        site_id = form.site_id,
                        user_name = this.userinfo.user_name,
                        user_id = this.userinfo.user_id,
                    })
                };
            }
            else
            {
                this.throwHttpResponseException("請輸入資料!!");
            }
            this.throwHttpResponseException("請輸入資料!!");
            return false;
        }



        /// <summary>
        /// 登入驗證
        /// </summary>
        /// <param name="form">登入資料</param>
        /// <returns></returns>
        [JwtAuthActionFilterAttribute(notVerification = true)]
        public object hospLogin(Login_Form_Body form)
        {
            if (!string.IsNullOrWhiteSpace(form.hosp_id)  )
            {
                string sql = "SELECT * FROM " + DB_TABLE_NAME.DB_MC_HOSP_INFO + "  WHERE HOSP_KEY LIKE @HOSP_KEY ";
                Dapper.DynamicParameters dp = new Dapper.DynamicParameters();
                dp.Add("HOSP_KEY", form.hosp_id + "%");
                List<DB_MC_HOSP_INFO> pList = DBLink.DBA.getSqlDataTable<DB_MC_HOSP_INFO>(sql, dp);
                if (pList.Count == 0)
                {
                    this.throwHttpResponseException("查無此事件資料!!");
                }
                return new
                {
                    Result = true,
                    token = JwtAuthActionFilterAttribute.EncodeToken(new PAYLOAD()
                    { 
                        hosp_id = pList[0].HOSP_KEY,
                        site_id = "",
                        site_desc = "",
                        user_name = this.userinfo.user_name,
                        user_id = this.userinfo.user_id,
                    })
                };
            }
            else
            {
                this.throwHttpResponseException("請輸入資料!!");
            }
            this.throwHttpResponseException("請輸入資料!!");
            return false;
        }
        /// <summary>
        /// 登入驗證
        /// </summary>
        /// <param name="form">登入資料</param>
        /// <returns></returns>
        [JwtAuthActionFilterAttribute(notVerification = true)]
        public object SiteLogin(Login_Form_Body form)
        {
            if (!string.IsNullOrWhiteSpace(form.site_id))
            {
                List<DB_MC_SITE_INFO> pList = _model.getMC_SITE_INFO(form.site_id);
                if (pList.Count == 0)
                {
                    this.throwHttpResponseException("查無此事件資料!!");
                }
                return new
                {
                    Result = true,
                    token = JwtAuthActionFilterAttribute.EncodeToken(new PAYLOAD()
                    {
                        hosp_id = form.hosp_id,
                        site_id = form.site_id,
                        site_desc = pList[0].SITE_DESC,
                        user_name = this.userinfo.user_name,
                        user_id = this.userinfo.user_id,
                    })
                };
            }
            else
            {
                this.throwHttpResponseException("請輸入資料!!");
            }
            this.throwHttpResponseException("請輸入資料!!");
            return false;
        }


        /// <summary>
        /// 登入驗證
        /// </summary>
        /// <param name="form">登入資料</param>
        /// <returns></returns> 
        public List<DB_MC_SITE_INFO> getMC_SITE_INFO()
        {
            
            return _model.getMC_SITE_INFO();
        }

        /// <summary>
        /// 登入驗證
        /// </summary>
        /// <param name="form">登入資料</param>
        /// <returns></returns>
        [JwtAuthActionFilterAttribute(notVerification = true)]
        public object SiteLoginNew(Login_Form_Body form)
        {
            if (!string.IsNullOrWhiteSpace(form.site_desc))
            {
                List<DB_MC_SITE_INFO> pList = new List<DB_MC_SITE_INFO>();
                if (pList.Count == 0)
                {
                    if (!string.IsNullOrWhiteSpace(form.LATITUDE) && !string.IsNullOrWhiteSpace(form.LONGITUDE))
                    {
                        pList.Add(new DB_MC_SITE_INFO()
                        {
                            SITE_ID = this.DBLink.GetFixedStrSerialNumber(),
                            SITE_DESC = form.site_desc,
                            CREATE_DATE = Function_Library.getDateNowString(DATE_FORMAT.yyyy_MM_dd_HHmmss),
                            CREATE_ID = this.userinfo.user_id,
                            CREATE_NAME = this.userinfo.user_name,
                            MODIFY_DATE = Function_Library.getDateNowString(DATE_FORMAT.yyyy_MM_dd_HHmmss),
                            MODIFY_ID = this.userinfo.user_id,
                            MODIFY_NAME = this.userinfo.user_name,
                            DATASTATUS = "1",
                            LATITUDE = form.LATITUDE,
                            LONGITUDE = form.LONGITUDE,
                        });
                        this.DBLink.DBA.DBExecInsert<DB_MC_SITE_INFO>(pList);
                        if (this.DBLink.DBA.hasLastError)
                        {
                            this.throwHttpResponseException("程式發生錯誤，請洽資訊人員!");
                            Com.Mayaminer.LogTool.SaveLogMessage(this.DBLink.DBA.lastError, "Login", this.csName);
                        }
                    }
                    else
                    {
                        this.throwHttpResponseException("查無經緯度資料無法填寫資料!!");
                    }
                }
                return new
                {
                    Result = true,
                    token = JwtAuthActionFilterAttribute.EncodeToken(new PAYLOAD()
                    {
                        site_id = pList[0].SITE_ID,
                        site_desc = pList[0].SITE_DESC,
                        user_name = this.userinfo.user_name,
                        user_id = this.userinfo.user_id,
                    })
                };
            }
            else
            {
                this.throwHttpResponseException("請輸入資料!!");
            }
            this.throwHttpResponseException("請輸入資料!!");
            return false;
        }

        /// <summary>
        /// 驗證Auth
        /// </summary>
        /// <param name="form"></param>
        /// <returns></returns>
        [JwtAuthActionFilterAttribute]
        [HttpPost]
        public string JwtAuthCheck(JWT_Form_Body form)
        {
            return "合法登入!";
        }

        #region hosp function

        /// <summary>
        /// 醫院清單功能
        /// </summary>
        /// <param name="form"></param>
        /// <returns></returns>
        [JwtAuthActionFilterAttribute]
        [HttpPost]
        public List<DB_MC_HOSP_INFO> HospInfoList(JWT_Form_Body form)
        {
            List<DB_MC_HOSP_INFO> pList = new List<DB_MC_HOSP_INFO>();

            return pList;
         }

        public string saveSite(DB_MC_SITE_INFO form)
        {
            List<DB_MC_SITE_INFO> pList = new List<DB_MC_SITE_INFO>();
            if (string.IsNullOrWhiteSpace(form.LATITUDE) || string.IsNullOrWhiteSpace(form.LONGITUDE))
            {
                this.throwHttpResponseException("查無經緯度資料無法填寫資料!!");
            }
            if (string.IsNullOrEmpty(form.SITE_ID))
            { 
                this.throwHttpResponseException("請填寫事件代碼!!");
            }
            if (string.IsNullOrEmpty(form.SITE_DESC))
            {
                this.throwHttpResponseException("請填寫事件名稱!!");
            }
            if (string.IsNullOrEmpty(form.SITE_AREA))
            {
                this.throwHttpResponseException("請填寫事件地區!!");
            }
            if (!string.IsNullOrEmpty(form.SITE_ID))
            {
                pList = _model.getMC_SITE_INFO(form.SITE_ID);
                if (pList.Count == 0)
                {
                    pList.Add(new DB_MC_SITE_INFO() { 
                        SITE_DESC = form.SITE_DESC,
                        SITE_ID = form.SITE_ID,
                        CREATE_DATE = Function_Library.getDateNowString(DATE_FORMAT.yyyy_MM_dd_HHmmss),
                        CREATE_ID = this.userinfo.user_id,
                        CREATE_NAME = this.userinfo.user_name,
                        MODIFY_DATE = Function_Library.getDateNowString(DATE_FORMAT.yyyy_MM_dd_HHmmss),
                        MODIFY_ID = this.userinfo.user_id,
                        MODIFY_NAME = this.userinfo.user_name,
                        DATASTATUS = "1",
                        CLASS_TYPE = form.CLASS_TYPE,
                        LATITUDE = form.LATITUDE,
                        LONGITUDE = form.LONGITUDE,
                    });
                    this.DBLink.DBA.DBExecInsert<DB_MC_SITE_INFO>(pList);
                    if (this.DBLink.DBA.hasLastError)
                    {
                        this.throwHttpResponseException("程式發生錯誤，請洽資訊人員!");
                        Com.Mayaminer.LogTool.SaveLogMessage(this.DBLink.DBA.lastError, "Login", this.csName);
                    }
                }
                else
                {
                    foreach (DB_MC_SITE_INFO item in pList)
                    {
                        item.SITE_DESC = form.SITE_DESC;
                        item.SITE_AREA = form.SITE_AREA;
                        item.LATITUDE = form.LATITUDE;
                        item.LONGITUDE = form.LONGITUDE;
                        item.CLASS_TYPE = form.CLASS_TYPE;
                        item.MODIFY_DATE = Function_Library.getDateNowString(DATE_FORMAT.yyyy_MM_dd_HHmmss);
                        item.MODIFY_ID = this.userinfo.user_id;
                        item.MODIFY_NAME = this.userinfo.user_name;
                    }
                    this.DBLink.DBA.DBExecUpdate<DB_MC_SITE_INFO>(pList);
                    if (this.DBLink.DBA.hasLastError)
                    {
                        this.throwHttpResponseException("程式發生錯誤，請洽資訊人員!");
                        Com.Mayaminer.LogTool.SaveLogMessage(this.DBLink.DBA.lastError, "Login", this.csName);
                    }
                }

            }

            if (this.DBLink.DBA.hasLastError)
            { 
                this.throwHttpResponseException("儲存失敗!");
            }  
            return "儲存成功";
        }

        #endregion



    }
}

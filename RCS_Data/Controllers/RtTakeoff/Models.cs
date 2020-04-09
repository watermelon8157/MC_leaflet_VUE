using Com.Mayaminer;
using Dapper;
using mayaminer.com.library;
using Newtonsoft.Json;
using RCS_Data.Models;
using RCS_Data.Models.DB;
using RCS_Data.Models.ViewModels;
using RCSData.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace RCS_Data.Controllers.RtTakeoff
{
    public class Models :BaseModels, Interface
    {
        string csName = "RtTakeoff.Models";
        const string dateFormat = "yyyy-MM-dd HH:mm:ss";

        private WebMethod _webmethod { get; set; }
        protected WebMethod webmethod
        {
            get
            {
                if (this._webmethod == null)
                    this._webmethod = new WebMethod();
                return this._webmethod;
            }
        }

        /// <summary>
        /// 呼吸器脫離評估LIST
        /// </summary>
        /// <returns></returns>
        public RESPONSE_MSG RtTakeOffList(ref List<VM_RTTakeoffAssess> rttakeoff_items, string pSDate, string pEDate, IPDPatientInfo pat_info, string pId, UserInfoBasic user_info, bool isLast = false)
        {
            RESPONSE_MSG rm = new RESPONSE_MSG();
            Dapper.DynamicParameters dp = new Dapper.DynamicParameters();           
            string actionName = "RtTakeOffList";
            try
            {
                //取得主表清單
                string wSql = ""; 
                List<RCS_DATA_TK_ASSESS> RtTakeOffMaster = new List<RCS_DATA_TK_ASSESS>();
                string sql = @"SELECT (
                                SELECT tk_id 
                                FROM RCS_WEANING_ASSESS 
                                WHERE DATASTATUS = '1' 
                                    AND CHART_NO = " + SQLDefend.SQLString(pat_info.chart_no) + @" 
                                    AND REC_DATE in(
                                        SELECT MAX(REC_DATE) 
                                        FROM RCS_WEANING_ASSESS 
                                        WHERE DATASTATUS = '1' 
                                            AND CHART_NO = " + SQLDefend.SQLString(pat_info.chart_no) + @")) 
                            as last_tk_id,tk_id,ipd_no,chart_no,rec_date,tk_reason as _tk_reason,st_reason as _st_reason,tk_plan as _tk_plan,DATASTATUS,UPLOAD_STATUS,CREATE_NAME,CREATE_ID 
                            FROM RCS_WEANING_ASSESS" + string.Format(@" 
                            WHERE DATASTATUS = '1' 
                                AND CHART_NO = {0} AND REC_DATE >= {1} AND REC_DATE <= {2} {3}", 
                                SQLDefend.SQLString(pat_info.chart_no), 
                                SQLDefend.SQLString(Function_Library.getDateString(DateTime.Parse(pSDate), DATE_FORMAT.yyyy_MM_dd_HHmm)), 
                                SQLDefend.SQLString(Function_Library.getDateString(DateTime.Parse(pEDate), DATE_FORMAT.yyyy_MM_dd_2359)  ), wSql);
                
                RtTakeOffMaster = this.DBLink.DBA.getSqlDataTable<RCS_DATA_TK_ASSESS>(sql, dp);

                //取得明細清單
                sql = string.Concat("SELECT * FROM RCS_WEANING_ASSESS_DTL  WHERE TK_ID in @TK_ID");
                dp = new Dapper.DynamicParameters();
                dp.Add("TK_ID", RtTakeOffMaster.Select(x => x.tk_id).ToList());

                List<DB_RCS_WEANING_ASSESS_DTL> dtl = this.DBLink.DBA.getSqlDataTable<DB_RCS_WEANING_ASSESS_DTL>(sql, dp);

                foreach (RCS_DATA_TK_ASSESS item in RtTakeOffMaster)
                {
                    
                    var getList = dtl.Where(x => x.TK_ID == item.tk_id).ToList();

                    Dictionary<string, object> dict = new Dictionary<string, object>();
                    foreach (DB_RCS_WEANING_ASSESS_DTL val in getList)
                    {
                        if (val.ITEM_NAME == "create_name" && string.IsNullOrWhiteSpace(val.ITEM_VALUE))
                        {
                            val.ITEM_VALUE = item.create_name;
                        }

                        if (val.ITEM_NAME.StartsWith("_"))
                        {
                            //因當時MVC的model中airway_lung轉換成_airway_lung並且_airway_lung不是使用一般的get;set;，導致程式碼無法對_airway_lung進行資料存入，因此需取得轉換前的名稱(airway_lung)。
                            val.ITEM_NAME = val.ITEM_NAME.Substring(1, val.ITEM_NAME.Length - 1);
                        }


                        if (!string.IsNullOrWhiteSpace(val.ITEM_VALUE) && val.ITEM_VALUE != "null")
                        {
                            try
                            {
                                if (val.ITEM_VALUE.StartsWith("[") && val.ITEM_VALUE.EndsWith("]"))
                                {
                                    if (val.ITEM_NAME =="weaningTable")
                                    {
                                        List<RCS_WEANING_ITEM> JSON_RCS_WEANING_ITEM = JsonConvert.DeserializeObject<List<RCS_WEANING_ITEM>>(val.ITEM_VALUE); 
                                        dict.Add(val.ITEM_NAME, JSON_RCS_WEANING_ITEM);
                                    }
                                    else if (val.ITEM_NAME == "labDataTable")
                                    {
                                        List<RCS_LAB_ITEM> JSON_RCS_LAB_ITEM = JsonConvert.DeserializeObject<List<RCS_LAB_ITEM>>(val.ITEM_VALUE);
                                        dict.Add(val.ITEM_NAME, JSON_RCS_LAB_ITEM);
                                    }
                                    else
                                    {
                                        List<JSON_DATA> JSON_DATA = JsonConvert.DeserializeObject<List<JSON_DATA>>(val.ITEM_VALUE); 
                                        dict.Add(val.ITEM_NAME, JSON_DATA);
                                    }
                                }
                                else
                                {
                                    if (val.ITEM_NAME != "tk_id")
                                    {
                                        dict.Add(val.ITEM_NAME, val.ITEM_VALUE);
                                    }
                                }
                            }
                            catch (Exception ex)
                            {
                                LogTool.SaveLogMessage(ex, actionName, this.csName);
                            }
                        }//if (!string.IsNullOrWhiteSpace(val.ITEM_VALUE) && val.ITEM_VALUE != "null")

                    }//foreach (DB_RCS_WEANING_ASSESS_DTL val in getList)

                    dict.Add("tk_id", item.tk_id);
                    VM_RTTakeoffAssess addData = Newtonsoft.Json.JsonConvert.DeserializeObject<VM_RTTakeoffAssess>(
                       Newtonsoft.Json.JsonConvert.SerializeObject(dict));
                    addData.DATASTATUS = item.DATASTATUS;
                    addData.UPLOAD_STATUS = item.UPLOAD_STATUS;
                    List<VM_RTTakeoffAssess> changeJsonList = new List<VM_RTTakeoffAssess>();
                    changeJsonList.Add(addData);
                    rttakeoff_items.Add(changeJson(changeJsonList).First());
                }

                rttakeoff_items = rttakeoff_items.OrderByDescending(x => x.rec_date).ToList();
            }
            catch (Exception ex)
            {
                LogTool.SaveLogMessage(ex, "RtTakeOffList");
            }

            return rm;
        }

        /// <summary>
        /// 取得呼吸器脫離評估單
        /// </summary>
        /// <param name="form"></param>
        /// <returns></returns>
        public getRtTakeOff RtTakeoffData(List<string> RT_ID_LIST)
        {
            string actionName = "RtTakeoffData";
            List<VM_RTTakeoffAssess> result = new List<VM_RTTakeoffAssess>();
            List<DB_RCS_WEANING_ASSESS> pLIst = new List<DB_RCS_WEANING_ASSESS>();

            Dapper.DynamicParameters dp = new Dapper.DynamicParameters();
            List<DB_RCS_WEANING_ASSESS_DTL> detailLIst = new List<DB_RCS_WEANING_ASSESS_DTL>();


            dp.Add("TK_ID", RT_ID_LIST);

            detailLIst = this.DBLink.DBA.getSqlDataTable<DB_RCS_WEANING_ASSESS_DTL>(string.Concat("SELECT * FROM ",
               DB_TABLE_NAME.DB_RCS_WEANING_ASSESS_DTL,
               " WHERE TK_ID in @TK_ID"), dp);

            pLIst = this.DBLink.DBA.getSqlDataTable<DB_RCS_WEANING_ASSESS>(string.Concat("SELECT * FROM ",
               DB_TABLE_NAME.DB_RCS_WEANING_ASSESS,
               " WHERE TK_ID in @TK_ID"), dp);

            foreach (string item in RT_ID_LIST)
            {

                var getList = detailLIst.Where(x => x.TK_ID == item).ToList();


                Dictionary<string, object> dict = new Dictionary<string, object>();

                foreach (DB_RCS_WEANING_ASSESS_DTL val in getList)
                {
                    if (val.ITEM_NAME.StartsWith("_"))
                    {
                        // 因當時MVC的model中airway_lung轉換成_airway_lung並且_airway_lung不是使用一般的get;set;，導致程式碼無法對_airway_lung進行資料存入，因此需取得轉換前的名稱(airway_lung)。
                        val.ITEM_NAME = val.ITEM_NAME.Substring(1, val.ITEM_NAME.Length - 1);
                    }

                    if (!string.IsNullOrWhiteSpace(val.ITEM_VALUE) && val.ITEM_VALUE != "null")
                    {
                        try
                        {
                            if (val.ITEM_VALUE.StartsWith("[") && val.ITEM_VALUE.EndsWith("]"))
                            {
                                if (val.ITEM_NAME == "weaningTable")
                                {
                                    // 因weaningTable的值不是JSON_DATA格式，所以用RCS_WEANING_ITEM來轉換資料
                                    List<RCS_WEANING_ITEM> JSON_RCS_WEANING_ITEM = JsonConvert.DeserializeObject<List<RCS_WEANING_ITEM>>(val.ITEM_VALUE);
                                    dict.Add(val.ITEM_NAME, JSON_RCS_WEANING_ITEM);
                                }
                                else if (val.ITEM_NAME == "labDataTable")
                                {
                                    List<RCS_LAB_ITEM> JSON_RCS_LAB_ITEM = JsonConvert.DeserializeObject<List<RCS_LAB_ITEM>>(val.ITEM_VALUE);
                                    dict.Add(val.ITEM_NAME, JSON_RCS_LAB_ITEM);
                                }
                                else
                                {
                                    List<JSON_DATA> JSON_DATA = JsonConvert.DeserializeObject<List<JSON_DATA>>(val.ITEM_VALUE);
                                    dict.Add(val.ITEM_NAME, JSON_DATA);
                                }
                            }
                            else
                            {
                                if (val.ITEM_NAME != "tk_id")
                                {
                                    dict.Add(val.ITEM_NAME, val.ITEM_VALUE);
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            LogTool.SaveLogMessage(ex, actionName, this.csName);
                        }
                    }//if (!string.IsNullOrWhiteSpace(val.ITEM_VALUE) && val.ITEM_VALUE != "null")                    
                }
                dict.Add("tk_id", item);


                VM_RTTakeoffAssess addData = Newtonsoft.Json.JsonConvert.DeserializeObject<VM_RTTakeoffAssess>(
                       Newtonsoft.Json.JsonConvert.SerializeObject(dict));

                addData.tk_id = item;

                result.Add(addData);
            }

            return new getRtTakeOff() { detailList = result, masterList = pLIst };
        }

        /// <summary>
        /// 儲存呼吸器脫離評估單
        /// </summary>
        /// <param name="form"></param>
        /// <returns></returns>
        public RESPONSE_MSG RtTakeOff_Save(Form_RtTakeOff_Save form)
        {
            RESPONSE_MSG rm = new RESPONSE_MSG();
            string tk_id = form.rdta.tk_id ?? "";
            try
            {
                SQLProvider SQL = new SQLProvider();
                //寫入呼吸器脫離評估單數據回寫院內
                string nowDate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                rm.status = RESPONSE_STATUS.SUCCESS;
                form.rdta.create_id = form.user_info.user_id;
                form.rdta.create_name = form.user_info.user_name;
                // 主檔儲存
                if (rm.status == RESPONSE_STATUS.SUCCESS)
                {
                    Dapper.DynamicParameters WEANING_dp = new Dapper.DynamicParameters();
                    List<DB_RCS_WEANING_ASSESS> pLIst = new List<DB_RCS_WEANING_ASSESS>();
                    List<DB_RCS_WEANING_ASSESS> RtTakeOffMaster = new List<DB_RCS_WEANING_ASSESS>();
                    this.DBLink.DBA.BeginTrans();

                    string WEANING_sql = @"SELECT * FROM RCS_WEANING_ASSESS WHERE REC_DATE = " + SQLDefend.SQLString(form.rdta.rec_date) + " AND IPD_NO = " + SQLDefend.SQLString(form.pat_info.ipd_no) + " AND DATASTATUS = '1'";
                    RtTakeOffMaster = this.DBLink.DBA.getSqlDataTable<DB_RCS_WEANING_ASSESS>(WEANING_sql, WEANING_dp);

                    if (RtTakeOffMaster.Count > 0 && string.IsNullOrWhiteSpace(tk_id))
                    {                       
                        rm.status = RESPONSE_STATUS.ERROR;
                        rm.message = DateHelper.Parse(form.rdta.rec_date).ToString("yyyy-MM-dd HH:mm") + "已存在此記錄單時間，請重新輸入!";
                        LogTool.SaveLogMessage(this.DBLink.DBA.lastError, "RtTakeOff_Save");
                    }
                    else
                    {
                        if (string.IsNullOrWhiteSpace(tk_id))
                        {
                            DB_RCS_WEANING_ASSESS addMasterData = new DB_RCS_WEANING_ASSESS();

                            tk_id = SQL.GetFixedStrSerialNumber();
                            addMasterData.IPD_NO = form.pat_info.ipd_no;
                            addMasterData.TK_ID = tk_id;
                            addMasterData.CREATE_ID = form.user_info.user_id;
                            addMasterData.CREATE_NAME = form.user_info.user_name;
                            addMasterData.CREATE_DATE = nowDate;
                            addMasterData.MODIFY_ID = form.user_info.user_id;
                            addMasterData.MODIFY_NAME = form.user_info.user_name;
                            addMasterData.MODIFY_DATE = nowDate;
                            addMasterData.CHART_NO = form.pat_info.chart_no;
                            addMasterData.BED_NO = form.pat_info.bed_no;
                            addMasterData.COST_CODE = form.pat_info.cost_code;
                            addMasterData.DEPT_CODE = form.pat_info.dept_code;
                            addMasterData.DATASTATUS = "1";
                            addMasterData.UPLOAD_STATUS = "0";
                            addMasterData.PAT_SOURCE = form.pat_info.source_type;
                            addMasterData.REC_DATE = form.rdta.rec_date;
                            pLIst.Add(addMasterData);
                            this.DBLink.DBA.DBExecInsert<DB_RCS_WEANING_ASSESS>(pLIst);
                        }
                        else
                        {
                            //判斷是否已經上傳
                            Dapper.DynamicParameters dp = new Dapper.DynamicParameters();
                            dp.Add("TK_ID", tk_id);
                            pLIst = this.DBLink.DBA.getSqlDataTable<DB_RCS_WEANING_ASSESS>(string.Concat("SELECT * FROM RCS_WEANING_ASSESS WHERE TK_ID = @TK_ID"), dp);
                            pLIst.ForEach(x => {
                                x.MODIFY_DATE = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                                x.MODIFY_ID = form.payload.user_id;
                                x.MODIFY_NAME = form.payload.user_name;
                                x.REC_DATE = string.IsNullOrEmpty(form.rdta.rec_date) ? "" : form.rdta.rec_date;
                                x.UPLOAD_STATUS = "0";
                            });
                            this.DBLink.DBA.DBExecUpdate<DB_RCS_WEANING_ASSESS>(pLIst);
                        }

                        if (this.DBLink.DBA.hasLastError)
                        {
                            LogTool.SaveLogMessage(this.DBLink.DBA.lastError, "RtTakeOff_Save");
                            rm.message = "儲存失敗!";
                            rm.status = RESPONSE_STATUS.ERROR;
                        }
                        else
                        {

                            List<DB_RCS_WEANING_ASSESS_DTL> pDetailLIst = new List<DB_RCS_WEANING_ASSESS_DTL>();
                            // 明細儲存 (先清空在儲存)
                            this.DBLink.DBA.DBExecDelete<DB_RCS_WEANING_ASSESS_DTL>(new DB_RCS_WEANING_ASSESS_DTL() { TK_ID = tk_id });

                            rm.status = RESPONSE_STATUS.SUCCESS;

                            Dictionary<string, object> formDeatail = new Dictionary<string, object>();

                            form.rdta.tk_id = tk_id;

                            //WeaningProfile JSON判斷
                            if (form.WeaningProfileList != null && form.WeaningProfileList.Any())
                            {
                                form.rdta.weaningTable_data = this.saveData(form, "WeaningProfile");
                            }
                            else
                            {
                                form.rdta.weaningTable_data = "";
                            }

                            //LabData JSON判斷
                            if (form.LabDataList != null && form.LabDataList.Any())
                            {
                                form.rdta.labDataTable_data = this.saveData(form, "LabData");
                            }
                            else
                            {
                                form.rdta.labDataTable_data = "";
                            }

                            formDeatail = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(Newtonsoft.Json.JsonConvert.SerializeObject(form.rdta));
                            foreach (KeyValuePair<string, object> item in formDeatail)
                            {
                                if (item.Value != null)
                                {
                                    pDetailLIst.Add(new DB_RCS_WEANING_ASSESS_DTL()
                                    {
                                        TK_ID = tk_id,
                                        ITEM_NAME = item.Key,
                                        ITEM_VALUE = item.Value is string ?
                                        item.Value.ToString() : Newtonsoft.Json.JsonConvert.SerializeObject(item.Value)
                                    });
                                }
                                else
                                {
                                    pDetailLIst.Add(new DB_RCS_WEANING_ASSESS_DTL()
                                    {
                                        TK_ID = tk_id,
                                        ITEM_NAME = item.Key,
                                        ITEM_VALUE = ""
                                    });
                                }
                            }
                            this.DBLink.DBA.DBExecInsert<DB_RCS_WEANING_ASSESS_DTL>(pDetailLIst);
                        }
                    }
                }
                else
                {
                    rm.message = "上傳系統發生例外，請洽資訊人員!";
                    rm.status = RESPONSE_STATUS.ERROR;
                }
            }
            catch (Exception ex)
            {
                string tmp = ex.Message;
                LogTool.SaveLogMessage(tmp, "RtTakeOff_Save");
                rm.status = RESPONSE_STATUS.EXCEPTION;
                rm.message = "系統發生例外，請聯繫資訊人員";
            }
            finally
            {
                if (rm.status == RESPONSE_STATUS.SUCCESS && !this.DBLink.DBA.hasLastError)
                {
                    this.DBLink.DBA.Commit();
                    rm.message = "儲存成功";
                }
                else
                    this.DBLink.DBA.Rollback();
            }
            
            return rm;
        }

        private string saveData(Form_RtTakeOff_Save form , string dataname)
        {
            SQLProvider SQL = new SQLProvider();
            RESPONSE_MSG rm = new RESPONSE_MSG();
            var addJsonKey = SQL.GetFixedStrSerialNumber();            

            List<DB_RCS_RT_RECORD_JSON> addKeyData = new List<DB_RCS_RT_RECORD_JSON>();

            if (dataname == "WeaningProfile")
            {       
                addKeyData.Add(new DB_RCS_RT_RECORD_JSON
                {
                    RECORD_ID = addJsonKey,
                    ITEM_NAME = dataname + "資料",
                    JSON_VALUE = JsonConvert.SerializeObject(form.WeaningProfileList)

                });
            }
            if (dataname =="LabData")
            {                
                addKeyData.Add(new DB_RCS_RT_RECORD_JSON
                {
                    RECORD_ID = addJsonKey,
                    ITEM_NAME = dataname + "資料",
                    JSON_VALUE = JsonConvert.SerializeObject(form.LabDataList)
                });
                              
            }
            

            RESPONSE_MSG checkJson = this.DBLink.Insert_JSONData(addJsonKey, addKeyData);


            if (checkJson.status == RESPONSE_STATUS.ERROR)
            {
                LogTool.SaveLogMessage(this.DBLink.DBA.lastError, "RtTakeOff_Save");
                rm.message = "儲存失敗!";
                rm.status = RESPONSE_STATUS.ERROR;
            }

            return addJsonKey;
        }

        /// <summary>
        /// 最後一筆 RTTakeOff DATA 
        /// </summary>
        /// <param name="pat_info"></param>
        /// <returns></returns>
        public VM_RTTakeoffAssess RTTakeoffLastData(IPDPatientInfo pat_info)
        {
            VM_RTTakeoffAssess rm = new VM_RTTakeoffAssess();
            List<VM_RTTakeoffAssess> temp = new List<VM_RTTakeoffAssess>();
            SQLProvider SQL = new SQLProvider();
            string sql = @"SELECT tk_id,ipd_no,chart_no,rec_date 
                            FROM RCS_WEANING_ASSESS 
                            WHERE  DATASTATUS = '1' 
                                AND CHART_NO = @CHART_NO 
                                AND IPD_NO = @IPD_NO 
                                AND REC_DATE in ( 
                                    SELECT MAX(REC_DATE) 
                                    FROM RCS_WEANING_ASSESS 
                                    WHERE DATASTATUS = '1' 
                                        AND CHART_NO = @CHART_NO 
                                        AND IPD_NO = @IPD_NO)";
            DynamicParameters dp = new DynamicParameters();
            dp.Add("CHART_NO", pat_info.chart_no);
            dp.Add("IPD_NO", pat_info.ipd_no);
            temp = SQL.DBA.getSqlDataTable<VM_RTTakeoffAssess>(sql, dp);
            if (temp.Count > 0 && !string.IsNullOrWhiteSpace(temp[0].tk_id))
            {
                rm = temp[0];
            }

            return rm;
        }

        /// <summary>
        ///  刪除呼吸治療評估單
        /// </summary>
        /// <param name="form"></param>
        /// <returns></returns>
        public RESPONSE_MSG RtTakeOffDelete(List<string> RT_ID_LIST, UserInfo user_info)
        {
            string actionName = "RTTakeoffDelete";
            RESPONSE_MSG rm = new RESPONSE_MSG();
            SQLProvider SQL = new SQLProvider();

            foreach (var TK_ID in RT_ID_LIST)
            {
                if (this.TempInsert(TK_ID, true, user_info))
                {
                    DynamicParameters dp = new DynamicParameters();
                    dp.Add("TK_ID", TK_ID);
                    string _sql = "UPDATE RCS_WEANING_ASSESS SET DATASTATUS = '9' WHERE TK_ID = @TK_ID";
                    SQL.DBA.DBExecute(_sql, dp);
                    if (!SQL.DBA.hasLastError)
                    {
                        rm.message = "刪除成功";
                        rm.status = RESPONSE_STATUS.SUCCESS;
                    }
                    else
                    {
                        LogTool.SaveLogMessage(SQL.DBA.lastError, actionName, "BaseController");
                        rm.message = "刪除失敗";
                        rm.status = RESPONSE_STATUS.ERROR;

                        break;
                    }
                }
                else
                {
                    LogTool.SaveLogMessage(SQL.DBA.lastError, actionName, "BaseController");
                    rm.message = "刪除失敗";
                    rm.status = RESPONSE_STATUS.ERROR;
                    break;
                }
            }

            return rm;
        }


        /// <summary>
        ///  取得Weaning profile
        /// </summary>
        /// <param name="form"></param>
        /// <returns></returns>
        public RESPONSE_MSG WeaningProfileTB(ref List<RCS_WEANING_ITEM> pList, IPDPatientInfo pat_info)
        {
            string actionName = "WeaningProfileTB";
            RESPONSE_MSG rm = new RESPONSE_MSG();
            SQLProvider SQL = new SQLProvider();
            try
            {
                string sql = @"
                    SELECT * FROM RCS_RECORD_DETAIL 
                    WHERE RECORD_ID in(
                    SELECT RECORD_ID FROM RCS_RECORD_MASTER WHERE IPD_NO = @IPD_NO AND CHART_NO = @CHART_NO 
                    AND RECORD_ID in(
                    SELECT RECORD_ID FROM RCS_RECORD_DETAIL WHERE ITEM_NAME  = 'is_weaning' AND  ITEM_VALUE in ('1','true','True') )
                    )
                    AND ITEM_NAME in('pi_max','pe_max','vt_value','rsi_srr','rsbi','recorddate','recordtime','cuff_leak_ml','mode','cuff_leak_sound','mv_value') 
                    ";
                DynamicParameters dp = new DynamicParameters();
                dp.Add("IPD_NO", pat_info.ipd_no);
                dp.Add("CHART_NO", pat_info.chart_no);
                List<RTRECORD_DETAIL> dt = SQL.DBA.getSqlDataTable<RTRECORD_DETAIL>(sql, dp);
                List<string> RECORD_ID_List = dt.Select(x => x.RECORD_ID).Distinct().ToList();
                foreach (string str in RECORD_ID_List)
                {
                    List<RTRECORD_DETAIL> temp = dt.FindAll(x => x.RECORD_ID == str);
                    RCS_WEANING_ITEM item = new RCS_WEANING_ITEM();
                    item.seq = pList.Count;
                    item.date = string.Concat(temp.Find(x => x.ITEM_NAME == "recorddate").ITEM_VALUE, " ", temp.Find(x => x.ITEM_NAME == "recordtime").ITEM_VALUE);
                    if (temp.Exists(x => x.ITEM_NAME == "pi_max")) item.pi_max = temp.Find(x => x.ITEM_NAME == "pi_max").ITEM_VALUE;
                    if (temp.Exists(x => x.ITEM_NAME == "pe_max")) item.pe_max = temp.Find(x => x.ITEM_NAME == "pe_max").ITEM_VALUE;
                    if (temp.Exists(x => x.ITEM_NAME == "vt_value")) item.vt_value = temp.Find(x => x.ITEM_NAME == "vt_value").ITEM_VALUE;
                    if (temp.Exists(x => x.ITEM_NAME == "rsi_srr")) item.srr = temp.Find(x => x.ITEM_NAME == "rsi_srr").ITEM_VALUE;
                    if (temp.Exists(x => x.ITEM_NAME == "rsbi")) item.rsbi = temp.Find(x => x.ITEM_NAME == "rsbi").ITEM_VALUE;
                    if (temp.Exists(x => x.ITEM_NAME == "cuff_leak_ml")) item.cuff_leak_ml = temp.Find(x => x.ITEM_NAME == "cuff_leak_ml").ITEM_VALUE;
                    if (temp.Exists(x => x.ITEM_NAME == "mode")) item.mode = temp.Find(x => x.ITEM_NAME == "mode").ITEM_VALUE;
                    if (temp.Exists(x => x.ITEM_NAME == "cuff_leak_sound")) item.cuff_leak_sound = temp.Find(x => x.ITEM_NAME == "cuff_leak_sound").ITEM_VALUE;
                    if (temp.Exists(x => x.ITEM_NAME == "mv_value")) item.mv_value = temp.Find(x => x.ITEM_NAME == "mv_value").ITEM_VALUE; 
                    pList.Add(item);
                }
                if (pList.Count > 0)
                    pList = pList.OrderByDescending(x => DateTime.Parse(x.date)).ToList();
            }
            catch (Exception ex)
            {
                LogTool.SaveLogMessage(ex, actionName, "RTTakeoffController");
            }
            return rm;
        }

        /// <summary>
        ///  取得LabD ata
        /// </summary>
        /// <param name="form"></param>
        /// <returns></returns>
        public RESPONSE_MSG LabDataTB(ref List<RCS_LAB_ITEM> pList, IPDPatientInfo pat_info, IWebServiceParam iwp)
        {
            string actionName = "LabDataTB";
            RESPONSE_MSG rm = new RESPONSE_MSG();
            SQLProvider SQL = new SQLProvider();
            try
            {
                List<ExamBloodBiochemical> ExamBloodBiochemical = new List<ExamBloodBiochemical>();
                if (pat_info.chart_no != null && pat_info.ipd_no != null)
                    ExamBloodBiochemical = webmethod.getBloodBiochemicalData(iwp, pat_info.chart_no, pat_info.ipd_no);
                ExamBloodBiochemical = ExamBloodBiochemical.OrderByDescending(x => DateTime.Parse(x.examDate)).ToList();
                foreach (ExamBloodBiochemical o in ExamBloodBiochemical)
                {
                    RCS_LAB_ITEM item = new RCS_LAB_ITEM();
                    item.seq = pList.Count;
                    item.date = o.examDate;
                    item.hb = o.examContent["Hb"].result;
                    item.ht = o.examContent["Ht"].result;
                    item.wbc = o.examContent["WBC"].result;
                    item.Platelet = o.examContent["Platelet"].result;
                    item.na = o.examContent["Na"].result;
                    item.k = o.examContent["K"].result;
                    item.bun = o.examContent["BUN"].result;
                    item.cr = o.examContent["Cr"].result;
                    item.albumin = o.examContent["Albumin"].result;
                    pList.Add(item);
                }
                if (pList.Count > 0)
                    pList = pList.OrderByDescending(x => DateTime.Parse(x.date)).ToList();
            }
            catch (Exception ex)
            {
                LogTool.SaveLogMessage(ex, actionName, "RTTakeoffController");
            }
            return rm;
        }

        /// <summary>
        /// 暫存新增
        /// </summary>
        /// <param name="pRECORD_ID"></param>
        /// <param name="isDel"></param>
        /// <returns></returns>
        private bool TempInsert(string pRECORD_ID, bool isDel, UserInfo user_info)
        {
            SQLProvider SQL = new SQLProvider();
            DynamicParameters dp = new DynamicParameters();
            string query = string.Concat("SELECT REC_DATE as RECORD_DATE,* FROM ", DB_TABLE_NAME.DB_RCS_WEANING_ASSESS, " WHERE TK_ID = @TK_ID;",
                "SELECT TK_ID as RECORD_ID,* FROM ", DB_TABLE_NAME.DB_RCS_WEANING_ASSESS_DTL, " WHERE TK_ID = @TK_ID;");
            dp.Add("TK_ID", pRECORD_ID);
            if (isDel)
            {
                return SQL.DELTableData(pRECORD_ID, DB_TABLE_NAME.DB_RCS_WEANING_ASSESS, user_info, query, dp);
            }
            return SQL.EditTableData(pRECORD_ID, DB_TABLE_NAME.DB_RCS_WEANING_ASSESS, user_info, query, dp);
        }

        public virtual List<VM_RTTakeoffAssess> changeJson(List<VM_RTTakeoffAssess> List)
        {
            return List;
        } 

        /// <summary>
        /// 取得 OnePage 所需要的資料
        /// </summary>
        /// <param name="chrNo">病歷號</param>
        /// <returns></returns>
        public virtual List<OnePagePDF> OnePagePDFList(string chrNo)
        {
            DateTime nowDate = DateTime.Now;
            string getUrl = IniFile.GetConfig("System", "PDFURL");
            string emrid = "RtTakeOffPageForm", emrname = "呼吸脫離評估單";
            string parameter = "", sDate = "", eDate = "";
            List<OnePagePDF> pList = new List<OnePagePDF>();
            List<DB_RCS_RT_CASE> patList = this.getDB_RCS_RT_CASEList(chrNo);
            foreach (DB_RCS_RT_CASE item in patList)
            {
                // &sDate=2020-02-23 00:00&eDate=2020-03-05 23:59
                sDate = string.IsNullOrWhiteSpace(item.CARE_DATE) ?
                    Function_Library.getDateString(nowDate, DATE_FORMAT.yyyy_MM_dd_0000) : Function_Library.getDateString(DateTime.Parse(item.CARE_DATE), DATE_FORMAT.yyyy_MM_dd_0000);
                eDate = Function_Library.getDateString(nowDate, DATE_FORMAT.yyyy_MM_dd_2359);
                parameter = string.Format("&pSDate={0}&pEDate={1}", sDate, eDate);
                pList.Add(new OnePagePDF()
                {
                    emrid = emrid,
                    emrname = emrname,
                    fee_no = item.IPD_NO,
                    url = string.Concat(getUrl, string.Format("PDFPage?chart_no={0}&ipd_no={1}&parameter={2}&form_id={3}&form_name={4}", item.CHART_NO, item.IPD_NO, RCS_Data.Models.Function_Library.getcodeDES(parameter), emrid, emrname))
                });
            }
            return pList;
        }
    }
    
    public class Form_RtTakeOffDetail : AUTH, IForm_RtTakeOffDetail
    {
        public string TK_ID { get; set; }
        public string chart_no { get; set; }
        public string ipd_no { get; set; }
    }

    public class Form_RtTakeoffList : AUTH, IForm_RtTakeoffList
    {
        public string pSDate { get; set; }

        public string pEDate { get; set; }
        public string pipd_no { get; set; }
        public string pId { get; set; }
    }

    public class Form_RtTakeOff_Save : AUTH
    {
        public RCS_RTTakeoff rdta { get; set; }
        public List<RCS_WEANING_ITEM> WeaningProfileList { get; set; }
        public List<RCS_LAB_ITEM> LabDataList { get; set; }
    }

    public class getRtTakeOff
    {
        public List<VM_RTTakeoffAssess> detailList { get; set; }

        public List<DB_RCS_WEANING_ASSESS> masterList { get; set; }
    }
}

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
using System.Text;

namespace RCS_Data.Controllers.RtAssess
{
    public class Models : BaseModels, Interface
    {
        string csName = "RtAssess.Models";
        const string dateFormat = "yyyy-MM-dd HH:mm:ss";
      
        /// <summary>
        /// 呼吸治療評估單LIST
        /// </summary>
        /// <param name="cpt_dtl_new_items"></param>
        /// <param name="form"></param>
        /// <returns></returns>
        public RESPONSE_MSG CPTRecordList(ref List<RCS_CPT_DTL_NEW_ITEMS> cpt_dtl_new_items, string pSDate, string pEDate, string ipd_no)
        {
            RESPONSE_MSG rm = new RESPONSE_MSG();
            Dapper.DynamicParameters dp = new Dapper.DynamicParameters();
            string actionName = "CPTRecordList";
            try
            {
                //取得主表清單
                List<DB_RCS_CPT_ASS_MASTER> temp = new List<DB_RCS_CPT_ASS_MASTER>();
                string sql = string.Concat("SELECT RECORD_DATE,CPT_ID,CREATE_ID,CREATE_NAME,DATASTATUS,UPLOAD_STATUS,UPLOAD_ID FROM ", DB_TABLE_NAME.DB_RCS_CPT_ASS_MASTER , " WHERE datastatus = '1' AND IPD_NO = @IPD_NO AND RECORD_DATE BETWEEN @pSDate AND @pEDate");


                if (DateHelper.isDate(pSDate) && DateHelper.isDate(pEDate))
                {
                    dp.Add("pSDate", pSDate);
                    dp.Add("pEDate", pEDate);
                }
                else
                {
                    #region 日期錯誤，預設帶入當月的清單資料

                    pSDate = DateTime.Now.ToString("yyyy/MM/01 HH:mm:ss");
                    pEDate = DateHelper.Parse(pSDate).AddMonths(1).AddDays(-1).ToString(dateFormat);

                    dp.Add("pSDate", pSDate);
                    dp.Add("pEDate", pEDate);
                    #endregion
                }

                dp.Add("IPD_NO", ipd_no);
                this.DBLink.DBA.Open();

                temp = this.DBLink.DBA.getSqlDataTable<DB_RCS_CPT_ASS_MASTER>(sql, dp);
                //取得明細清單

                sql = string.Concat("SELECT * FROM RCS_CPT_ASS_DETAIL  WHERE CPT_ID in @CPT_ID");

                dp = new Dapper.DynamicParameters();

                dp.Add("CPT_ID", temp.Select(x => x.CPT_ID).ToList());

                List<DB_RCS_CPT_ASS_DETAIL> dtl = this.DBLink.DBA.getSqlDataTable<DB_RCS_CPT_ASS_DETAIL>(sql, dp);

                //List<Dictionary<string, object>> tempDict = new List<Dictionary<string, object>>();
                foreach (DB_RCS_CPT_ASS_MASTER item in temp)
                {  
                    var getList = dtl.Where(x => x.CPT_ID == item.CPT_ID).ToList(); 
                    Dictionary<string, object> dict = new Dictionary<string, object>(); 
                    foreach (DB_RCS_CPT_ASS_DETAIL val in getList) {
                        if (!string.IsNullOrWhiteSpace(val.CPT_VALUE) && val.CPT_VALUE != "null")
                        {
                            try
                            {
                                if (val.CPT_VALUE.StartsWith("[") && val.CPT_VALUE.EndsWith("]"))
                                {
                                    List<JSON_DATA> JSON_DATA = JsonConvert.DeserializeObject<List<JSON_DATA>>(val.CPT_VALUE);
                                    dict.Add(val.CPT_ITEM, JSON_DATA);
                                }
                                else
                                {
                                    if (val.CPT_ITEM != "cpt_id")
                                    {
                                        dict.Add(val.CPT_ITEM, val.CPT_VALUE);
                                    }
                                }

                            }
                            catch (Exception ex)
                            {
                                LogTool.SaveLogMessage(ex, actionName,this.csName);
                            }
                            
                        } 
                    }
                    dict.Add("cpt_id", item.CPT_ID);
                    RCS_CPT_DTL_NEW_ITEMS addData = Newtonsoft.Json.JsonConvert.DeserializeObject<RCS_CPT_DTL_NEW_ITEMS>(
                       Newtonsoft.Json.JsonConvert.SerializeObject(dict));
                    addData.CREATE_ID = item.CREATE_ID;
                    addData.CREATE_NAME = item.CREATE_NAME;
                    addData.DATASTATUS = item.DATASTATUS;
                    addData.UPLOAD_STATUS = item.UPLOAD_STATUS;
                    addData.UPLOAD_ID = item.UPLOAD_ID;
                    cpt_dtl_new_items.Add(addData);
                }

                cpt_dtl_new_items = cpt_dtl_new_items.OrderByDescending(x => x.record_date).ToList();
            }
            catch (Exception ex)
            {
                LogTool.SaveLogMessage(ex, actionName, this.csName);
            }


            return rm;
        }


        /// <summary>
        ///  刪除呼吸治療評估單
        /// </summary>
        /// <param name="form"></param>
        /// <returns></returns>
        public RESPONSE_MSG CPTRecordDelete(List<string> RT_ID_LIST , UserInfo user_info)
        {
            string actionName = "cpt_data_del";
            RESPONSE_MSG rm = new RESPONSE_MSG();
            SQLProvider SQL = new SQLProvider(); 
            foreach (var CPT_ID in RT_ID_LIST) {
                if (this.TempInsert(CPT_ID, true, user_info))
                {
                    DynamicParameters dp = new DynamicParameters();
                    dp.Add("CPT_ID", CPT_ID);
                    string _sql = "UPDATE RCS_CPT_ASS_MASTER SET datastatus = '9' WHERE CPT_ID = @CPT_ID";
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
        /// 暫存新增
        /// </summary>
        /// <param name="pRECORD_ID"></param>
        /// <param name="isDel"></param>
        /// <returns></returns>
        private bool TempInsert(string pRECORD_ID, bool isDel, UserInfo user_info)
        {
            SQLProvider SQL = new SQLProvider();
            DynamicParameters dp = new DynamicParameters();
            string query = string.Concat("SELECT * FROM ", DB_TABLE_NAME.DB_RCS_CPT_ASS_MASTER, " WHERE CPT_ID = @CPT_ID;",
                "SELECT CPT_ID as RECORD_ID,CPT_ITEM as ITEM_NAME,CPT_VALUE as ITEM_VALUE,* FROM ", DB_TABLE_NAME.DB_RCS_CPT_ASS_DETAIL, " WHERE CPT_ID = @CPT_ID;");
            dp.Add("CPT_ID", pRECORD_ID);
            if (isDel)
            {
                return SQL.DELTableData(pRECORD_ID, DB_TABLE_NAME.DB_RCS_CPT_ASS_MASTER, user_info, query, dp);
            }
            return SQL.EditTableData(pRECORD_ID, DB_TABLE_NAME.DB_RCS_CPT_ASS_MASTER, user_info, query, dp);
        }


        /// <summary>
        /// 取得呼吸治療評估單
        /// </summary>
        /// <param name="form"></param>
        /// <returns></returns>
        public getRTAssess RTAssessData(List<string> RT_ID_LIST) {
            string actionName = "RTAssessData";
            List<RCS_CPT_DTL_NEW_ITEMS> result = new List<RCS_CPT_DTL_NEW_ITEMS>();
            List<DB_RCS_CPT_ASS_MASTER> pLIst = new List<DB_RCS_CPT_ASS_MASTER>();

            Dapper.DynamicParameters dp = new Dapper.DynamicParameters();
            List<DB_RCS_CPT_ASS_DETAIL> detailLIst = new List<DB_RCS_CPT_ASS_DETAIL>();


            dp.Add("CPT_ID", RT_ID_LIST);

            detailLIst = this.DBLink.DBA.getSqlDataTable<DB_RCS_CPT_ASS_DETAIL>(string.Concat("SELECT * FROM ",
               DB_TABLE_NAME.DB_RCS_CPT_ASS_DETAIL,
               " WHERE CPT_ID in @CPT_ID"), dp);

            pLIst = this.DBLink.DBA.getSqlDataTable<DB_RCS_CPT_ASS_MASTER>(string.Concat("SELECT * FROM ",
               DB_TABLE_NAME.DB_RCS_CPT_ASS_MASTER, " WHERE CPT_ID in @CPT_ID"), dp);


            foreach (string item in RT_ID_LIST) {

                var getList = detailLIst.Where(x => x.CPT_ID == item).ToList();


                Dictionary<string, object> dict = new Dictionary<string, object>();

                foreach (DB_RCS_CPT_ASS_DETAIL val in getList)
                {
                    if (!string.IsNullOrWhiteSpace(val.CPT_VALUE) && val.CPT_VALUE != "null")
                    {
                        try
                        {
                            if (val.CPT_VALUE.StartsWith("[") && val.CPT_VALUE.EndsWith("]"))
                            {
                                List<JSON_DATA> JSON_DATA = JsonConvert.DeserializeObject<List<JSON_DATA>>(val.CPT_VALUE);
                                dict.Add(val.CPT_ITEM, JSON_DATA);
                            }
                            else
                            {
                                if (val.CPT_ITEM != "cpt_id")
                                {
                                    dict.Add(val.CPT_ITEM, val.CPT_VALUE);
                                }
                            }

                        }
                        catch (Exception ex)
                        {
                            LogTool.SaveLogMessage(ex, actionName, this.csName);
                        }

                    }
                }
                dict.Add("cpt_id", item);


                RCS_CPT_DTL_NEW_ITEMS addData = Newtonsoft.Json.JsonConvert.DeserializeObject<RCS_CPT_DTL_NEW_ITEMS>(
                       Newtonsoft.Json.JsonConvert.SerializeObject(dict));

                addData.cpt_id = item;

                result.Add(addData);
            }

            return new getRTAssess() { detailList = result ,masterList = pLIst } ;
        }

        /// <summary>
        /// 儲存呼吸治療評估單
        /// </summary>
        /// <param name="form"></param>
        /// <returns></returns>
        public RESPONSE_MSG CPTAssess_Save(Form_CPTAssess_Save form)
        {
            RESPONSE_MSG rm = new RESPONSE_MSG();
            string cpt_id = form.rdta.cpt_id ?? "";
            List<DB_RCS_CPT_ASS_MASTER> pLIst = new List<DB_RCS_CPT_ASS_MASTER>();
            try
            {
                SQLProvider SQL = new SQLProvider();
                //寫入CPT 評估表數據回寫院內
                string nowDate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                rm.status = RESPONSE_STATUS.SUCCESS;

                // 主檔儲存
                if (rm.status == RESPONSE_STATUS.SUCCESS)
                {  
                    this.DBLink.DBA.BeginTrans(); 
                    if (string.IsNullOrWhiteSpace(cpt_id))
                    {
                        DB_RCS_CPT_ASS_MASTER addMasterData = new DB_RCS_CPT_ASS_MASTER();

                        cpt_id = SQL.GetFixedStrSerialNumber();
                        addMasterData.IPD_NO = form.pat_info.ipd_no;
                        addMasterData.CPT_ID = cpt_id;
                        addMasterData.CPT_STATUS = "Y";
                        addMasterData.CREATE_ID = form.user_info.user_id;
                        addMasterData.CREATE_NAME = form.user_info.user_name;
                        addMasterData.CREATE_DATE = nowDate;
                        addMasterData.CPT_STATUS = "Y";
                        addMasterData.MODIFY_ID = form.user_info.user_id;
                        addMasterData.MODIFY_NAME = form.user_info.user_name;
                        addMasterData.MODIFY_DATE = nowDate;
                        addMasterData.CHART_NO = form.pat_info.chart_no; 
                        addMasterData.ADMISSION_DATE = form.pat_info.diag_date;
                        addMasterData.DATASTATUS = "1";
                        addMasterData.UPLOAD_STATUS = "0";
                        addMasterData.RECORD_DATE = form.rdta.record_date;
                        addMasterData.BED_NO = form.pat_info.bed_no;
                        addMasterData.PAT_SOURCE = form.pat_info.source_type;
                        addMasterData.COST_CODE = form.pat_info.cost_code;
                        addMasterData.DEPT_CODE = form.pat_info.dept_code;
                        pLIst.Add(addMasterData);
                        this.DBLink.DBA.DBExecInsert<DB_RCS_CPT_ASS_MASTER>(pLIst);
                    }
                    else
                    {
                        //判斷是否已經上傳
                        Dapper.DynamicParameters dp = new Dapper.DynamicParameters();
                        dp.Add("CPT_ID", cpt_id);
                        pLIst = this.DBLink.DBA.getSqlDataTable<DB_RCS_CPT_ASS_MASTER>(string.Concat("SELECT * FROM RCS_CPT_ASS_MASTER WHERE CPT_ID = @CPT_ID"), dp);
                        if (pLIst.Count > 0 && pLIst.Exists(x=>x.CREATE_ID == form.user_info.user_id))
                        {
                            pLIst.ForEach(x => {
                                x.MODIFY_DATE = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                                x.MODIFY_ID = form.payload.user_id;
                                x.MODIFY_NAME = form.payload.user_name;
                                x.RECORD_DATE = string.IsNullOrEmpty(form.rdta.record_date) ? "" : form.rdta.record_date;
                                x.UPLOAD_STATUS = "0";
                            });
                            this.DBLink.DBA.DBExecUpdate<DB_RCS_CPT_ASS_MASTER>(pLIst);
                        }
                        else
                        {
                            LogTool.SaveLogMessage(this.DBLink.DBA.lastError, "CPTAssess_Save");
                            rm.messageList.Add("不能修改別人的資料唷!");
                            rm.status = RESPONSE_STATUS.ERROR;
                        }
                       
                    }

                    if (this.DBLink.DBA.hasLastError)
                    {
                        LogTool.SaveLogMessage(this.DBLink.DBA.lastError, "CPTAssess_Save");
                        rm.messageList.Add("儲存失敗!");
                        rm.status = RESPONSE_STATUS.ERROR;
                    }
                    else
                    {
                        if (rm.status== RESPONSE_STATUS.SUCCESS)
                        {
                            List<DB_RCS_CPT_ASS_DETAIL> pDetailLIst = new List<DB_RCS_CPT_ASS_DETAIL>();
                            // 明細儲存 (先清空在儲存)
                            this.DBLink.DBA.DBExecDelete<DB_RCS_CPT_ASS_DETAIL>(new DB_RCS_CPT_ASS_DETAIL() { CPT_ID = cpt_id });

                            rm.status = RESPONSE_STATUS.SUCCESS;

                            Dictionary<string, object> formDeatail = new Dictionary<string, object>();

                            form.rdta.cpt_id = cpt_id;

                            //手術JSON判斷
                            if (form.operationList != null && form.operationList.Any())
                            {
                                var addJsonKey = SQL.GetFixedStrSerialNumber();

                                List<DB_RCS_RT_RECORD_JSON> addKeyData = new List<DB_RCS_RT_RECORD_JSON>();

                                addKeyData.Add(new DB_RCS_RT_RECORD_JSON
                                {
                                    RECORD_ID = addJsonKey,
                                    ITEM_NAME = "手術資料",
                                    JSON_VALUE = JsonConvert.SerializeObject(form.operationList)

                                });


                                RESPONSE_MSG checkJson = this.DBLink.Insert_JSONData(addJsonKey, addKeyData);


                                if (checkJson.status == RESPONSE_STATUS.ERROR)
                                {
                                    LogTool.SaveLogMessage(this.DBLink.DBA.lastError, "CPTAssess_Save");
                                    rm.messageList.Add("儲存失敗!");
                                    rm.status = RESPONSE_STATUS.ERROR;
                                }

                                form.rdta.operation_data = addJsonKey;

                            }
                            else
                            {

                                form.rdta.operation_data = "";
                            }

                            formDeatail = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(Newtonsoft.Json.JsonConvert.SerializeObject(form.rdta));
                            foreach (KeyValuePair<string, object> item in formDeatail)
                            {
                                if (item.Value != null)
                                {
                                    pDetailLIst.Add(new DB_RCS_CPT_ASS_DETAIL()
                                    {
                                        CPT_ID = cpt_id,
                                        CPT_ITEM = item.Key,
                                        CPT_VALUE = item.Value is string ?
                                        item.Value.ToString() : Newtonsoft.Json.JsonConvert.SerializeObject(item.Value)
                                    });
                                }
                                else
                                {
                                    pDetailLIst.Add(new DB_RCS_CPT_ASS_DETAIL()
                                    {
                                        CPT_ID = cpt_id,
                                        CPT_ITEM = item.Key,
                                        CPT_VALUE = ""
                                    });
                                }
                            }
                            this.DBLink.DBA.DBExecInsert<DB_RCS_CPT_ASS_DETAIL>(pDetailLIst);
                        } 
                    }
                }
                else
                {
                    rm.messageList.Add("系統發生例外，請聯繫資訊人員!"); 
                    rm.status = RESPONSE_STATUS.ERROR;
                }
            }
            catch (Exception ex)
            {
                string tmp = ex.Message;
                LogTool.SaveLogMessage(tmp, "CPTAssess_Save");
                rm.status = RESPONSE_STATUS.EXCEPTION;
                rm.messageList.Add("系統發生例外，請聯繫資訊人員!"); 
            }
            finally
            {
                if (rm.status == RESPONSE_STATUS.SUCCESS && !this.DBLink.DBA.hasLastError)
                {
                    this.DBLink.DBA.Commit();
                    //TODO 不確定是否要留 (Janchiu)
                    //this.TempInsert(cpt_id, false);
                }
                else
                    this.DBLink.DBA.Rollback();
                this.DBLink.DBA.Close();
            } 
            rm.attachment = "儲存成功";
            return rm;
        }

        /// <summary>
        /// 最後一筆 RTASSESS DATA 
        /// </summary>
        /// <param name="pat_info"></param>
        /// <returns></returns>
        public RCS_CPT_DTL_NEW_ITEMS CPTLastData(IPDPatientInfo pat_info)
        {
            RCS_CPT_DTL_NEW_ITEMS rm = new RCS_CPT_DTL_NEW_ITEMS();
            List<RCS_CPT_DTL_NEW_ITEMS> temp = new List<RCS_CPT_DTL_NEW_ITEMS>();
            SQLProvider SQL = new SQLProvider();
            string sql = string.Concat("SELECT CPT_ID FROM RCS_CPT_ASS_MASTER WHERE CHART_NO = @CHART_NO AND datastatus = '1' AND RECORD_DATE in(SELECT MAX(RECORD_DATE) RECORD_DATE FROM RCS_CPT_ASS_MASTER WHERE CHART_NO = @CHART_NO AND datastatus = '1' AND  ISNULL(RECORD_DATE, '') <> '')");
            DynamicParameters dp = new DynamicParameters();
            dp.Add("CHART_NO", pat_info.chart_no);
            temp = SQL.DBA.getSqlDataTable<RCS_CPT_DTL_NEW_ITEMS>(sql, dp);
            if (temp.Count > 0 && !string.IsNullOrWhiteSpace(temp[0].cpt_id))
            {
                rm = temp[0];
            }

            return rm;
        }
         
        public virtual List<RCS_CPT_DTL_NEW_ITEMS> changeJson(List<RCS_CPT_DTL_NEW_ITEMS> List)
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
            string emrid = "CPTPageForm", emrname = "呼吸治療評估單";
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


    public class Form_CPTDetail : AUTH 
    {
        public string CPT_ID { get; set; } 
    }

    public class Form_CPTRecordList : AUTH, IForm_CPTRecordList
    {
        public string pSDate { get; set; }

        public string pEDate { get; set; }
        public string pipd_no { get; set; }
        public string pId { get; set; }
        public string pLast { get; set; }
    }

    public class Form_CPTAssess_Save : AUTH
    {
        public RCS_RTAsses rdta { get; set; }

        public List<PatOperation> operationList { get; set; }
    }

    public class getRTAssess 
    {
        public List<RCS_CPT_DTL_NEW_ITEMS> detailList { get; set; }

        public List<DB_RCS_CPT_ASS_MASTER> masterList { get; set; }
    }

}

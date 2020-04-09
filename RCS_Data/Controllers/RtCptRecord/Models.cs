﻿using Com.Mayaminer;
using Dapper;
using mayaminer.com.library;
using Newtonsoft.Json;
using RCS_Data.Models.DB;
using RCS_Data.Models.ViewModels;
using RCSData.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace RCS_Data.Controllers.RtCptRecord
{
    public class Models : BaseModels, Interface
    {
        string csName { get { return "RtCptRecord Model"; } }
        const string dateFormat = "yyyy-MM-dd HH:mm:ss";
        private SQLProvider _DBLink { get; set; }
        protected SQLProvider DBLink
        {
            get
            {
                if (this._DBLink == null)
                {
                    this._DBLink = new SQLProvider();
                }
                return this._DBLink;
            }
        }

        /// <summary>
        /// DELETE
        /// </summary>
        /// <param name="cpt_id"></param>
        /// <returns></returns>
        public RESPONSE_MSG CPTDel(string cpt_id)
        {
            RESPONSE_MSG rm = new RESPONSE_MSG();
            string actionName = "CPTDel";
            try
            {
                SQLProvider SQL = new SQLProvider();
                string sql = "UPDATE RCS_CPT_NEW_RECORD SET DATASTATUS = '9' WHERE CPT_ID = @CPT_ID";
                Dapper.DynamicParameters dp = new Dapper.DynamicParameters();
                dp.Add("CPT_ID", cpt_id);
                SQL.DBA.DBExecute(sql, dp);
                if (SQL.DBA.hasLastError)
                {
                    rm.status = RESPONSE_STATUS.ERROR;
                    rm.message = "刪除失敗!";
                }
                else
                {
                    rm.status = RESPONSE_STATUS.SUCCESS;
                    rm.message = "刪除成功!";
                }
            }
            catch (Exception ex)
            {
                rm.status = RESPONSE_STATUS.EXCEPTION;
                rm.message = "刪除失敗!";
                LogTool.SaveLogMessage(ex, actionName, this.csName);
            }
            return rm;
        }


        /// <summary>
        /// 呼吸治療評估單LIST
        /// </summary>
        /// <param name="cpt_dtl_new_items"></param>
        /// <param name="form"></param>
        /// <returns></returns>
        public RESPONSE_MSG RtCptRecordList(ref List<CPTNewRecord> cpt_dtl_new_items, string pSDate, string pEDate, IPDPatientInfo pat_info,string id = "")
        {
            RESPONSE_MSG rm = new RESPONSE_MSG();
            Dapper.DynamicParameters dp = new Dapper.DynamicParameters();

            string actionName = "getCPTData";
            List<CPTNewRecord> table = new List<CPTNewRecord>();
            try
            {
                SQLProvider SQL = new SQLProvider();
                string sqlMaser = "SELECT * FROM RCS_CPT_NEW_RECORD WHERE DATASTATUS = '1' AND  CHART_NO = @CHART_NO AND  IPD_NO = @IPD_NO  {0} ";


                string sql = "SELECT * FROM RCS_CPT_NEW_RECORD_DTL WHERE CPT_ID in(SELECT CPT_ID FROM RCS_CPT_NEW_RECORD WHERE DATASTATUS = '1' AND  CHART_NO = @CHART_NO AND  IPD_NO = @IPD_NO  {0}  )";
                if (DateHelper.isDate(pSDate) && DateHelper.isDate(pEDate))
                {
                    sqlMaser = string.Format(sqlMaser, " AND REC_DATE between @sDate AND @eDate");
                    sql = string.Format(sql, " AND REC_DATE between @sDate AND @eDate");
                    dp.Add("sDate", pSDate);
                    dp.Add("eDate", pEDate);
                }
                else if (!string.IsNullOrWhiteSpace(id))
                {
                    sqlMaser = string.Format(sqlMaser, " AND CPT_ID in @CPT_ID");
                    sql = string.Format(sql, " AND CPT_ID in @CPT_ID");
                    dp.Add("CPT_ID", id.Split(',').ToList());
                }
                else
                {
                    sql = string.Format(sql, " ");
                }

                dp.Add("CHART_NO", pat_info.chart_no);
                dp.Add("IPD_NO", pat_info.ipd_no);
                List<DB_RCS_CPT_NEW_RECORD> MastList = new List<DB_RCS_CPT_NEW_RECORD>();
                MastList = SQL.DBA.getSqlDataTable<DB_RCS_CPT_NEW_RECORD>(sqlMaser, dp);
                List<DB_RCS_CPT_NEW_RECORD_DTL> temp = new List<DB_RCS_CPT_NEW_RECORD_DTL>();
                temp = SQL.DBA.getSqlDataTable<DB_RCS_CPT_NEW_RECORD_DTL>(sql, dp);
                foreach (string CPT_ID in temp.Select(x => x.CPT_ID).Distinct().ToList())
                {
                    CPTNewRecord item = new CPTNewRecord();
                    item.cpt_id = CPT_ID; 
                    Dictionary<string, object> dict = new Dictionary<string, object>();
                    foreach (DB_RCS_CPT_NEW_RECORD_DTL val in temp.Where(x => x.CPT_ID == CPT_ID).ToList())
                    {
                        if (!string.IsNullOrWhiteSpace(val.ITEM_VALUE) && val.ITEM_VALUE != "null")
                        {
                            try
                            {

                                if (val.ITEM_VALUE.StartsWith("[") && val.ITEM_VALUE.EndsWith("]"))
                                {
                                    List<string> JSON_DATA = JsonConvert.DeserializeObject<List<string>>(val.ITEM_VALUE);
                                    dict.Add(val.ITEM_NAME, JSON_DATA);
                                }
                                else
                                {
                                    if (val.ITEM_VALUE == "True" || val.ITEM_VALUE == "False")
                                    {
                                        dict.Add(val.ITEM_NAME, JsonConvert.DeserializeObject<bool>(val.ITEM_VALUE.ToLower()));
                                    }
                                    else if (val.ITEM_NAME != "cpt_id")
                                    {
                                        dict.Add(val.ITEM_NAME, val.ITEM_VALUE);
                                    }
                                }

                            }
                            catch (Exception ex)
                            {
                                LogTool.SaveLogMessage(ex, actionName, this.csName);
                            }

                        }
                    }
                    dict.Add("cpt_id", CPT_ID);

                    CPTNewRecord addData = Newtonsoft.Json.JsonConvert.DeserializeObject<CPTNewRecord>(Newtonsoft.Json.JsonConvert.SerializeObject(dict));
                    if (MastList.Exists(x => x.CPT_ID == CPT_ID))
                    { 
                        addData.UPLOAD_STATUS = MastList.Find(x => x.CPT_ID == CPT_ID).UPLOAD_STATUS;
                        addData.DATASTATUS = MastList.Find(x => x.CPT_ID == CPT_ID).DATASTATUS;
                    }
                    //CXR
                    if (!String.IsNullOrWhiteSpace(addData.CXR_key))
                    {

                        var getData = this.DBLink.Select_JSONData<DB_RCS_RT_RECORD_JSON>(addData.CXR_key);

                        if (getData.Any())
                        {
                            addData.CXR_result_json = getData.First().JSON_VALUE;
                            List < CxrData > getCxrData = JsonConvert.DeserializeObject<List<CxrData>>(getData.First().JSON_VALUE);

                            if (getCxrData.Any()) {
                                addData.PDF_CXR_Date_Str = getCxrData.First().Result_Date;
                                addData.PDF_CXR_Result_Str = getCxrData.First().singJsonImageBase64;
                            }

                        }
                    }



                    table.Add(addData);
                }
            }
            catch (Exception ex)
            {
                LogTool.SaveLogMessage(ex, actionName, this.csName);
            }
            table = table.OrderByDescending(x => DateTime.Parse(x.rec_date)).ToList();
            cpt_dtl_new_items = table;
            return rm;
        }

        /// <summary>
        /// 呼吸治療評估單LIST
        /// </summary>
        /// <param name="cpt_dtl_new_items"></param>
        /// <param name="form"></param>
        /// <returns></returns>
        public List<CPTNewRecord> RtCptRecordListByID(List<string> idList)
        {
            RESPONSE_MSG rm = new RESPONSE_MSG();
            Dapper.DynamicParameters dp = new Dapper.DynamicParameters();

            string actionName = "getCPTData";
            List<CPTNewRecord> table = new List<CPTNewRecord>();

            try
            {
                SQLProvider SQL = new SQLProvider();
                string sql = "SELECT * FROM RCS_CPT_NEW_RECORD_DTL WHERE CPT_ID in(SELECT CPT_ID FROM RCS_CPT_NEW_RECORD WHERE CPT_ID in @CPT_ID AND  DATASTATUS = '1')";
                dp.Add("CPT_ID", idList);
                List<DB_RCS_CPT_NEW_RECORD_DTL> temp = new List<DB_RCS_CPT_NEW_RECORD_DTL>();
                temp = SQL.DBA.getSqlDataTable<DB_RCS_CPT_NEW_RECORD_DTL>(sql, dp);
                foreach (string CPT_ID in temp.Select(x => x.CPT_ID).Distinct().ToList())
                {
                    CPTNewRecord item = new CPTNewRecord();
                    item.cpt_id = CPT_ID;

                    Dictionary<string, object> dict = new Dictionary<string, object>();
                    foreach (DB_RCS_CPT_NEW_RECORD_DTL val in temp.Where(x => x.CPT_ID == CPT_ID).ToList())
                    {
                        if (!string.IsNullOrWhiteSpace(val.ITEM_VALUE) && val.ITEM_VALUE != "null")
                        {
                            try
                            {
                              

                                if (val.ITEM_VALUE.StartsWith("[") && val.ITEM_VALUE.EndsWith("]"))
                                {
                                    List<string> JSON_DATA = JsonConvert.DeserializeObject<List<string>>(val.ITEM_VALUE);
                                    dict.Add(val.ITEM_NAME, JSON_DATA);
                                }
                                else
                                {
                                    if (val.ITEM_VALUE == "True" || val.ITEM_VALUE == "False")
                                    {
                                        dict.Add(val.ITEM_NAME, JsonConvert.DeserializeObject<bool>(val.ITEM_VALUE.ToLower()));
                                    }
                                    else if (val.ITEM_NAME != "cpt_id")
                                    {
                                        dict.Add(val.ITEM_NAME, val.ITEM_VALUE);
                                    }
                                }

                            }
                            catch (Exception ex)
                            {
                                LogTool.SaveLogMessage(ex, actionName, this.csName);
                            }

                        }
                    }
                    dict.Add("cpt_id", CPT_ID);

                    CPTNewRecord addData = Newtonsoft.Json.JsonConvert.DeserializeObject<CPTNewRecord>(
                       Newtonsoft.Json.JsonConvert.SerializeObject(dict));

                    table.Add(addData);
                }
            }
            catch (Exception ex)
            {
                LogTool.SaveLogMessage(ex, actionName, this.csName);
                rm.status = RESPONSE_STATUS.EXCEPTION;
                rm.message = "儲存失敗!";
            }
            table = table.OrderByDescending(x => DateTime.Parse(x.rec_date)).ToList();
            return table;
        }

        /// <summary>
        /// 呼吸治療評估單LIST
        /// </summary>
        /// <param name="cpt_dtl_new_items"></param>
        /// <param name="form"></param>
        /// <returns></returns>
        public List<DB_RCS_CPT_NEW_RECORD> RtCptRecordMASTERByID(List<string> idList)
        {
            RESPONSE_MSG rm = new RESPONSE_MSG();
            Dapper.DynamicParameters dp = new Dapper.DynamicParameters();
            List<DB_RCS_CPT_NEW_RECORD> temp = new List<DB_RCS_CPT_NEW_RECORD>();
            string actionName = "getCPTData";
            List<CPTNewRecord> table = new List<CPTNewRecord>();

            try
            {
                SQLProvider SQL = new SQLProvider();
                string sql = string.Concat("SELECT * FROM ", DB_TABLE_NAME.DB_RCS_CPT_NEW_RECORD, " WHERE CPT_ID in @CPT_ID");
                dp.Add("CPT_ID", idList);
                temp = SQL.DBA.getSqlDataTable<DB_RCS_CPT_NEW_RECORD>(sql, dp);
            }
            catch (Exception ex)
            {
                LogTool.SaveLogMessage(ex, actionName, this.csName);
                rm.status = RESPONSE_STATUS.EXCEPTION;
                rm.message = "儲存失敗!";
            }
            return temp;
        }

        public void reRTCptAssesDetail(ref RtCptAss.RTCptAssesDetail model, List<string> CPT_ID)
        {
            List<DB_RCS_CPT_NEW_RECORD> pList = this.RtCptRecordMASTERByID(CPT_ID);
            if (pList.Count > 0)
            {
                DB_RCS_CPT_NEW_RECORD item = pList[0];
                model.UPLOAD_ID = item.UPLOAD_ID;
                model.CREATE_ID = item.CREATE_ID;
                model.UPLOAD_STATUS = item.UPLOAD_STATUS;
            }
        }


        public RESPONSE_MSG saveCPT(CPTNewRecord model, IPDPatientInfo pat_info, UserInfo user_info)
        {

            RESPONSE_MSG rm = new RESPONSE_MSG();
            string actionName = "saveCPT";
            bool is_MODIFY = string.IsNullOrWhiteSpace(model.cpt_id) ? false : true;
            try
            {
                SQLProvider SQL = new SQLProvider();
                Dapper.DynamicParameters dp_MST = null, dp_DEL = null;
                string sql_MST = "", sql_DTL = "";
                #region 主表
                if (!is_MODIFY)
                {
                    model.cpt_id = SQL.GetFixedStrSerialNumber(user_info.user_id, pat_info.ipd_no);
                    #region 新增SQL

                    sql_MST = @"
INSERT INTO RCS_CPT_NEW_RECORD (
CPT_ID,REC_DATE,IPD_NO,CHART_NO,CREATE_ID,CREATE_NAME,CREATE_DATE,MODIFY_ID,MODIFY_NAME,MODIFY_DATE,DATASTATUS,UPLOAD_STATUS,COST_CODE,BED_NO,DEPT_CODE
) VALUES (
@CPT_ID,@REC_DATE,@IPD_NO,@CHART_NO,@CREATE_ID,@CREATE_NAME,@CREATE_DATE,@MODIFY_ID,@MODIFY_NAME,@MODIFY_DATE,@DATASTATUS,@UPLOAD_STATUS,@COST_CODE,@BED_NO,@DEPT_CODE
)";
                    dp_MST = new Dapper.DynamicParameters();
                    dp_MST.Add("CPT_ID", model.cpt_id);
                    dp_MST.Add("REC_DATE", model.rec_date);
                    dp_MST.Add("IPD_NO", pat_info.ipd_no);
                    dp_MST.Add("CHART_NO", pat_info.chart_no);
                    dp_MST.Add("CREATE_ID", user_info.user_id);
                    dp_MST.Add("CREATE_NAME", user_info.user_name); 
                    dp_MST.Add("CREATE_DATE", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
                    dp_MST.Add("MODIFY_ID", user_info.user_id);
                    dp_MST.Add("MODIFY_NAME", user_info.user_name);
                    dp_MST.Add("MODIFY_DATE", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
                    dp_MST.Add("DATASTATUS", "1");
                    dp_MST.Add("UPLOAD_STATUS", "0");
                    dp_MST.Add("COST_CODE", pat_info.cost_code);
                    dp_MST.Add("BED_NO", pat_info.bed_no);
                    dp_MST.Add("DEPT_CODE", pat_info.dept_code);
                    #endregion
                }
                else
                {
                    #region 修改SQL

                    sql_MST = @"
UPDATE RCS_CPT_NEW_RECORD 
SET REC_DATE = @REC_DATE,MODIFY_ID = @MODIFY_ID,MODIFY_NAME = @MODIFY_NAME,MODIFY_DATE = @MODIFY_DATE,UPLOAD_STATUS = @UPLOAD_STATUS
WHERE CPT_ID = @CPT_ID
";
                    dp_MST = new Dapper.DynamicParameters();
                    dp_MST.Add("REC_DATE", model.rec_date);
                    dp_MST.Add("MODIFY_ID", user_info.user_id);
                    dp_MST.Add("MODIFY_NAME", user_info.user_name);
                    dp_MST.Add("UPLOAD_STATUS", "0");
                    dp_MST.Add("MODIFY_DATE", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
                    dp_MST.Add("CPT_ID", model.cpt_id);

                    #endregion
                }

                List<CPTNewRecord> cpt_dtl_new_items = new List<CPTNewRecord>();

                var checkData = this.RtCptRecordList(ref cpt_dtl_new_items,model.rec_date, model.rec_date,pat_info);


                if (cpt_dtl_new_items.Any()&& cpt_dtl_new_items.Where(x=>x.cpt_id!= model.cpt_id).Any()) {

                    rm.status = RESPONSE_STATUS.EXCEPTION;
                    rm.message = "評估時間重複!";
                    return rm;
                }

                #endregion

                //cxrJSON判斷
                if (!string.IsNullOrWhiteSpace(model.CXR_result_json))
                {
                    var addJsonKey = SQL.GetFixedStrSerialNumber(user_info.user_id, pat_info.ipd_no);

                    List<DB_RCS_RT_RECORD_JSON> addKeyData = new List<DB_RCS_RT_RECORD_JSON>();

                    addKeyData.Add(new DB_RCS_RT_RECORD_JSON
                    {
                        RECORD_ID = addJsonKey,
                        ITEM_NAME = "CXR資料",
                        JSON_VALUE = model.CXR_result_json

                    });


                    RESPONSE_MSG checkJson = this.DBLink.Insert_JSONData(addJsonKey, addKeyData);


                    if (checkJson.status == RESPONSE_STATUS.ERROR)
                    {
                        LogTool.SaveLogMessage(this.DBLink.DBA.lastError, "CPTAssess_Save");
                        rm.message = "儲存失敗!";
                        rm.status = RESPONSE_STATUS.ERROR;
                    }

                    model.CXR_key = addJsonKey;

                }
                else
                {
                    model.CXR_key = "";
                }

                model.CXR_result_json = "";
                model.PDF_CXR_Result_Str = "";

                #region 明細

               sql_DTL = "INSERT INTO RCS_CPT_NEW_RECORD_DTL Values (@CPT_ID,@ITEM_NAME,@ITEM_VALUE);";


                model.CREATE_NAME = user_info.user_name;



                //新增RTRECORD_DETAIL
                PropertyInfo[] props = null;
                List<DB_RCS_CPT_NEW_RECORD_DTL> list_DTL = new List<DB_RCS_CPT_NEW_RECORD_DTL>();
                var formDeatail = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(Newtonsoft.Json.JsonConvert.SerializeObject(model));
                foreach (KeyValuePair<string, object> item in formDeatail)
                {
                    if (item.Value != null)
                    {
                        list_DTL.Add(new DB_RCS_CPT_NEW_RECORD_DTL()
                        {
                            CPT_ID = model.cpt_id,
                            ITEM_NAME = item.Key,
                            ITEM_VALUE = item.Value is string ?
                            item.Value.ToString() : Newtonsoft.Json.JsonConvert.SerializeObject(item.Value)
                        });
                    }
                    else
                    {
                        list_DTL.Add(new DB_RCS_CPT_NEW_RECORD_DTL()
                        {
                            CPT_ID = model.cpt_id,
                            ITEM_NAME = item.Key,
                            ITEM_VALUE = ""
                        });
                    }
                }


                #endregion

                #region BeginTrans


                SQL.DBA.BeginTrans();
                SQL.DBA.DBExecute(sql_MST, dp_MST);
                if (is_MODIFY)
                {
                    dp_DEL = new Dapper.DynamicParameters();
                    dp_DEL.Add("CPT_ID", model.cpt_id);
                    SQL.DBA.DBExecute("DELETE RCS_CPT_NEW_RECORD_DTL WHERE CPT_ID = @CPT_ID", dp_DEL);
                }

                var aaa = JsonConvert.SerializeObject(list_DTL);

                SQL.DBA.DBExecute(sql_DTL, list_DTL);
                if (SQL.DBA.hasLastError)
                {
                    LogTool.SaveLogMessage(SQL.DBA.lastError, actionName, this.csName);
                    rm.status = RESPONSE_STATUS.ERROR;
                    rm.message = "儲存失敗!";
                    SQL.DBA.Rollback();
                }
                else
                {
                    rm.status = RESPONSE_STATUS.SUCCESS;
                    rm.message = "儲存成功!";
                    SQL.DBA.Commit();
                }


                #endregion

            }
            catch (Exception ex)
            {
                LogTool.SaveLogMessage(ex, actionName, this.csName);
                rm.status = RESPONSE_STATUS.EXCEPTION;
                rm.message = "儲存失敗!";
            }
            return rm;
        }


        /// <summary>
        /// 最後一筆 RTASSESS DATA 
        /// </summary>
        /// <param name="pat_info"></param>
        /// <returns></returns>
        public CPTNewRecord CPTAssesData(IPDPatientInfo pat_info)
        {
            CPTNewRecord rm = new CPTNewRecord();
            List<CPTNewRecord> temp = new List<CPTNewRecord>();
            SQLProvider SQL = new SQLProvider();

            Dapper.DynamicParameters dp = new Dapper.DynamicParameters();
            string sql = "SELECT * FROM " + DB_TABLE_NAME.DB_RCS_CPT_NEW_RECORD + " WHERE CHART_NO = @CHART_NO AND IPD_NO = @IPD_NO and DATASTATUS = '1' AND REC_DATE in (SELECT MAX(REC_DATE) FROM " + DB_TABLE_NAME.DB_RCS_CPT_NEW_RECORD + " WHERE CHART_NO = @CHART_NO AND IPD_NO = @IPD_NO and DATASTATUS = '1')";
            dp.Add("CHART_NO", pat_info.chart_no);
            dp.Add("IPD_NO", pat_info.ipd_no);
            if (sql.Length > 0)
            {
                temp = SQL.DBA.getSqlDataTable<CPTNewRecord>(sql, dp);
                if (temp.Count > 0)
                {
                    var getData = RtCptRecordListByID(new List<string>() { temp[0].cpt_id });
                    if (getData.Any())
                    {
                        rm = getData.First();
                    }

                }
            }

            return rm;
        }


        public virtual List<CPTNewRecord> changeData(List<CPTNewRecord> List)
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
            List<OnePagePDF> pList = new List<OnePagePDF>();

            return pList;
        }
    }

    public class FormRtCPTRecordList : AUTH, IForm_CPTRecordList
    {
        public string sDate { get; set; }
        public string eDate { get; set; }
    }


}

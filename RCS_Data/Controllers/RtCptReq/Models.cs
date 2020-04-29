using Com.Mayaminer;
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

namespace RCS_Data.Controllers.RtCptReq
{
    public class Models : BaseModels, Interface
    {
        string csName { get { return "RtCptRecord Model"; } }
        const string dateFormat = "yyyy-MM-dd HH:mm:ss";
         
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
                string sql = "UPDATE RCS_CPT_REQ_MASTER SET DATASTATUS = '9' WHERE RECORD_ID = @RECORD_ID";
                Dapper.DynamicParameters dp = new Dapper.DynamicParameters();
                dp.Add("RECORD_ID", cpt_id);
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
        public RESPONSE_MSG RtCptReqListByDate<T>(ref List<T> cpt_dtl_new_items, string pSDate, string pEDate, IPDPatientInfo pat_info)
        {
            RESPONSE_MSG rm = new RESPONSE_MSG();
            MODEL_DB_OBJ model = new MODEL_DB_OBJ();
            Dapper.DynamicParameters dp = new Dapper.DynamicParameters();

            string actionName = "getCPTData";
            List<T> table = new List<T>();
            try
            {
                SQLProvider SQL = new SQLProvider();
                string mainSql = "SELECT * FROM RCS_CPT_REQ_MASTER WHERE RECORD_DATE between @sDate AND @eDate AND  DATASTATUS = '1' AND  CHART_NO = @CHART_NO AND  IPD_NO = @IPD_NO";

                string sql = "SELECT * FROM RCS_CPT_REQ_DETAIL WHERE RECORD_ID in(SELECT RECORD_ID FROM RCS_CPT_REQ_MASTER WHERE RECORD_DATE between @sDate AND @eDate AND  DATASTATUS = '1' AND  CHART_NO = @CHART_NO AND  IPD_NO = @IPD_NO )";
                if (DateHelper.isDate(pSDate) && DateHelper.isDate(pEDate))
                {
                    dp.Add("sDate", pSDate);
                    dp.Add("eDate", pEDate);
                }

                dp.Add("CHART_NO", pat_info.chart_no);
                dp.Add("IPD_NO", pat_info.ipd_no);
                List<DB_RCS_CPT_REQ_MASTER> master = new List<DB_RCS_CPT_REQ_MASTER>();
                master = SQL.DBA.getSqlDataTable<DB_RCS_CPT_REQ_MASTER>(mainSql, dp);
                List<DB_RCS_CPT_REQ_DETAIL> temp = new List<DB_RCS_CPT_REQ_DETAIL>();
                temp = SQL.DBA.getSqlDataTable<DB_RCS_CPT_REQ_DETAIL>(sql, dp);
                model.detail = temp;
                model.master = master;
                table = this.RtCptReqList<T>(model);
            }
            catch (Exception ex)
            {
                LogTool.SaveLogMessage(ex, actionName, this.csName);
            }
            cpt_dtl_new_items = table;
            return rm;
        }

        /// <summary>
        /// 呼吸治療評估單LIST
        /// </summary>
        /// <param name="cpt_dtl_new_items"></param>
        /// <param name="form"></param>
        /// <returns></returns>
        public List<T> RtCptReqListByID<T>(List<string> idList)
        {
            string actionName = "RtCptReqListByID";
            RESPONSE_MSG rm = new RESPONSE_MSG();
            MODEL_DB_OBJ model = new MODEL_DB_OBJ();
            Dapper.DynamicParameters dp = new Dapper.DynamicParameters(); 
            List<T> table = new List<T>(); 

            try
            {
                SQLProvider SQL = new SQLProvider();
                string sql = "SELECT * FROM " + DB_TABLE_NAME.DB_RCS_CPT_REQ_DETAIL + " WHERE RECORD_ID in(SELECT RECORD_ID FROM " + DB_TABLE_NAME.DB_RCS_CPT_REQ_MASTER +" WHERE RECORD_ID in @RECORD_ID AND  DATASTATUS = '1')";
                dp.Add("RECORD_ID", idList);
                List<DB_RCS_CPT_REQ_DETAIL> temp = new List<DB_RCS_CPT_REQ_DETAIL>();
                temp = SQL.DBA.getSqlDataTable<DB_RCS_CPT_REQ_DETAIL>(sql, dp);
                model.detail = temp;
                model.master = new List<DB_RCS_CPT_REQ_MASTER>();
                table = this.RtCptReqList<T>(model);

            }
            catch (Exception ex)
            {
                LogTool.SaveLogMessage(ex, actionName, this.csName);
                rm.status = RESPONSE_STATUS.EXCEPTION;
                rm.message = "儲存失敗!";
            } 
            return table;
        }

        /// <summary>
        /// 呼吸治療評估單LIST
        /// </summary>
        /// <param name="cpt_dtl_new_items"></param>
        /// <param name="form"></param>
        /// <returns></returns>
        public List<T> RtCptReqList<T>(MODEL_DB_OBJ model)
        {
            RESPONSE_MSG rm = new RESPONSE_MSG();
            Dapper.DynamicParameters dp = new Dapper.DynamicParameters();

            string actionName = "getCPTData";
            List<T> table = new List<T>();
            List<DB_RCS_RT_RECORD_JSON> cxrList = new List<DB_RCS_RT_RECORD_JSON>();

            try
            { 
                List<DB_RCS_CPT_REQ_DETAIL> temp = model.detail; 
                //CXR
                if (temp.Exists(x => x.ITEM_NAME == "CXR_key" && !string.IsNullOrWhiteSpace(x.ITEM_VALUE)))
                {
                    cxrList = this.DBLink.Select_JSONDataList(temp.FindAll(x => x.ITEM_NAME == "CXR_key" && !string.IsNullOrWhiteSpace(x.ITEM_VALUE)).Select(x => x.ITEM_VALUE).ToList());
                }

                foreach (string RECORD_ID in temp.Select(x => x.RECORD_ID).Distinct().ToList())
                {
                    Dictionary<string, object> dict = new Dictionary<string, object>();
                    foreach (DB_RCS_CPT_REQ_DETAIL val in temp.Where(x => x.RECORD_ID == RECORD_ID).ToList())
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
                                    else if (val.ITEM_NAME != "record_id")
                                    {
                                        dict.Add(val.ITEM_NAME, val.ITEM_VALUE);
                                    }
                                }
                                //CXR
                                if (val.ITEM_NAME == "CXR_key" && !string.IsNullOrWhiteSpace(val.ITEM_VALUE))
                                {
                                    if (cxrList.Exists(x => x.RECORD_ID == val.ITEM_VALUE))
                                    {
                                        string JSON_VALUE = cxrList.Find(x => x.RECORD_ID == val.ITEM_VALUE).JSON_VALUE;
                                        dict.Add("CXR_result_json", JSON_VALUE);
                                        List<CxrData> getCxrData = JsonConvert.DeserializeObject<List<CxrData>>(JSON_VALUE);
                                        dict.Add("PDF_CXR_Date_Str", getCxrData.First().Result_Date);
                                        dict.Add("PDF_CXR_Result_Str", getCxrData.First().singJsonImageBase64);
                                    }
                                }
                            }
                            catch (Exception ex)
                            {
                                LogTool.SaveLogMessage(ex, actionName, this.csName);
                            }

                        }
                    }
                    dict.Add("record_id", RECORD_ID);
                    if (model.master != null && model.master.Count > 0)
                    { 
                        dict.Add("CREATE_NAME", model.master.Where(x => x.RECORD_ID == RECORD_ID).First().CREATE_NAME);
                    } 
                    T addData = Newtonsoft.Json.JsonConvert.DeserializeObject<T>(
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
            return table;
        }

        public RESPONSE_MSG saveCPT<T>(string record_id,string rec_date,  T model, IPDPatientInfo pat_info, UserInfo user_info)
        {

            RESPONSE_MSG rm = new RESPONSE_MSG();
            string actionName = "saveCPT";
            bool is_MODIFY = string.IsNullOrWhiteSpace(record_id) ? false : true;
            try
            {
                SQLProvider SQL = new SQLProvider();
                Dapper.DynamicParameters dp_MST = null, dp_DEL = null;
                string sql_MST = "", sql_DTL = "";
                #region 主表
                if (!is_MODIFY)
                {
                    record_id = SQL.GetFixedStrSerialNumber(user_info.user_id, pat_info.ipd_no);
                    #region 新增SQL

                    sql_MST = string.Format(@"
INSERT INTO {0} (
RECORD_ID,RECORD_DATE,IPD_NO,CHART_NO,CREATE_ID,CREATE_NAME,CREATE_DATE,DATASTATUS,UPLOAD_STATUS,COST_CODE,BED_NO,DEPT_CODE
) VALUES (
@RECORD_ID,@RECORD_DATE,@IPD_NO,@CHART_NO,@CREATE_ID,@CREATE_NAME,@CREATE_DATE,@DATASTATUS,@UPLOAD_STATUS,@COST_CODE,@BED_NO,@DEPT_CODE
)",DB_TABLE_NAME.DB_RCS_CPT_REQ_MASTER.ToString());
                    dp_MST = new Dapper.DynamicParameters();
                    dp_MST.Add("RECORD_ID", record_id);
                    dp_MST.Add("RECORD_DATE", rec_date);
                    dp_MST.Add("IPD_NO", pat_info.ipd_no);
                    dp_MST.Add("CHART_NO", pat_info.chart_no);
                    dp_MST.Add("CREATE_ID", user_info.user_id);
                    dp_MST.Add("CREATE_NAME", user_info.user_name);
                    dp_MST.Add("CREATE_DATE", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
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

                    sql_MST = string.Format(@"
UPDATE {0} 
SET RECORD_DATE = @RECORD_DATE,MODIFY_ID = @MODIFY_ID,MODIFY_NAME = @MODIFY_NAME,MODIFY_DATE = @MODIFY_DATE
WHERE RECORD_ID = @RECORD_ID
", DB_TABLE_NAME.DB_RCS_CPT_REQ_MASTER.ToString());
                    dp_MST = new Dapper.DynamicParameters();
                    dp_MST.Add("RECORD_DATE",  rec_date);
                    dp_MST.Add("MODIFY_ID", user_info.user_id);
                    dp_MST.Add("MODIFY_NAME", user_info.user_name);
                    dp_MST.Add("MODIFY_DATE", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
                    dp_MST.Add("RECORD_ID",  record_id);

                    #endregion
                } 
                #endregion 
                #region 明細

                sql_DTL = string.Format("INSERT INTO {0} Values (@RECORD_ID,@ITEM_NAME,@ITEM_VALUE);",DB_TABLE_NAME.DB_RCS_CPT_REQ_DETAIL.ToString());
                 

                //新增RTRECORD_DETAIL
                PropertyInfo[] props = null;
                List<DB_RCS_CPT_REQ_DETAIL> list_DTL = new List<DB_RCS_CPT_REQ_DETAIL>();
                var formDeatail = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(Newtonsoft.Json.JsonConvert.SerializeObject(model));
                foreach (KeyValuePair<string, object> item in formDeatail)
                {
                    if (item.Value != null)
                    {
                        list_DTL.Add(new DB_RCS_CPT_REQ_DETAIL()
                        {
                            RECORD_ID = record_id,
                            ITEM_NAME = item.Key,
                            ITEM_VALUE = item.Value is string ?
                            item.Value.ToString() : Newtonsoft.Json.JsonConvert.SerializeObject(item.Value)
                        });
                    }
                    else
                    {
                        list_DTL.Add(new DB_RCS_CPT_REQ_DETAIL()
                        {
                            RECORD_ID = record_id,
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
                    dp_DEL.Add("RECORD_ID", record_id);
                    SQL.DBA.DBExecute("DELETE RCS_CPT_REQ_DETAIL WHERE RECORD_ID = @RECORD_ID", dp_DEL);
                }
                 

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
        public string CPTReqDataLastRecord_id(IPDPatientInfo pat_info)
        {
            string record_id =  "";
            List<string> temp = new List<string>();
            SQLProvider SQL = new SQLProvider();

            Dapper.DynamicParameters dp = new Dapper.DynamicParameters();
            string sql = "SELECT record_id FROM " + DB_TABLE_NAME.DB_RCS_CPT_REQ_MASTER +" WHERE CHART_NO = @CHART_NO and DATASTATUS = '1' AND REC_DATE in (SELECT MAX(REC_DATE) FROM " + DB_TABLE_NAME.DB_RCS_CPT_REQ_MASTER + " WHERE CHART_NO = @CHART_NO and DATASTATUS = '1')";
            dp.Add("CHART_NO", pat_info.chart_no);
            dp.Add("IPD_NO", pat_info.ipd_no);
            if (sql.Length > 0)
            {
                temp = SQL.DBA.getSqlDataTable<string>(sql, dp);
                if (temp.Count > 0)
                {
                    record_id = temp[0]; 
                }
            }

            return record_id;
        }
         
    }

    public class FormRtCPTReqDetail : AUTH
    {
        public string RECORD_ID { get; set; }
    }

   public class MODEL_DB_OBJ
    {
        public List<DB_RCS_CPT_REQ_MASTER> master { get; set; }

        public List<DB_RCS_CPT_REQ_DETAIL> detail { get; set; }
    }
     
}

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


namespace RCS_Data.Controllers.RtOPDShift
{
    public class Models : BaseModels, Interface
    {
        string csName { get { return "RtCptAss Model"; } }
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
        /// 呼吸治療評估單LIST
        /// </summary>
        /// <param name="cpt_dtl_new_items"></param>
        /// <param name="form"></param>
        /// <returns></returns>
        public List<RCS_RT_OPD_SHIFT_DETAIL> RtCptReqListByID(List<string> idList)
        {
            RESPONSE_MSG rm = new RESPONSE_MSG();
            Dapper.DynamicParameters dp = new Dapper.DynamicParameters();

            string actionName = "getCPTData";
            List<RCS_RT_OPD_SHIFT_DETAIL> table = new List<RCS_RT_OPD_SHIFT_DETAIL>();

            try
            {
                SQLProvider SQL = new SQLProvider();

                string sql = "SELECT * FROM RCS_RT_OPD_SHIFT_DETAIL WHERE OS_ID in(SELECT OS_ID FROM RCS_RT_OPD_SHIFT_MASTER WHERE OS_ID in @OS_ID AND  DATASTATUS = '1')";
                dp.Add("OS_ID", idList);
                List<DB_RCS_OPD_SHIFT_DTL> temp = new List<DB_RCS_OPD_SHIFT_DTL>();
                temp = SQL.DBA.getSqlDataTable<DB_RCS_OPD_SHIFT_DTL>(sql, dp);
                foreach (string OS_ID in temp.Select(x => x.OS_ID).Distinct().ToList())
                {
                    RCS_RT_OPD_SHIFT_DETAIL item = new RCS_RT_OPD_SHIFT_DETAIL();
                    item.OS_ID = OS_ID;

                    Dictionary<string, object> dict = new Dictionary<string, object>();
                    foreach (DB_RCS_OPD_SHIFT_DTL val in temp.Where(x => x.OS_ID == OS_ID).ToList())
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
                                    else if (val.ITEM_NAME != "OS_ID")
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
                    dict.Add("OS_ID", OS_ID);

                    RCS_RT_OPD_SHIFT_DETAIL addData = Newtonsoft.Json.JsonConvert.DeserializeObject<RCS_RT_OPD_SHIFT_DETAIL>(
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
            //table = table.OrderByDescending(x => DateTime.Parse(x.opd_date)).ToList();
            return table;
        }

        public RCS_RT_OPD_SHIFT_DETAIL getLastData(IPDPatientInfo pat_info)
        {
            RCS_RT_OPD_SHIFT_DETAIL result = new RCS_RT_OPD_SHIFT_DETAIL();

            List<RCS_RT_OPD_SHIFT_DETAIL> opd_Shift_last_data = new List<RCS_RT_OPD_SHIFT_DETAIL>();
            string sqlLast = string.Concat("select OS_ID from RCS_RT_OPD_SHIFT_MASTER where RECORDDATE=( select Max(convert(datetime,RECORDDATE)) as record_date FROM RCS_RT_OPD_SHIFT_MASTER WHERE chart_no = '", pat_info.chart_no, "' AND DATASTATUS ='1') AND chart_no = '", pat_info.chart_no, "' AND DATASTATUS ='1'");
            opd_Shift_last_data = DBLink.DBA.getSqlDataTable<RCS_RT_OPD_SHIFT_DETAIL>(sqlLast).ToList();

            if (opd_Shift_last_data.Any())
            {
                result = opd_Shift_last_data.First();
            }

            return result;
        }

        /// <summary>
        /// 資料清單
        /// </summary>
        /// <param name="cpt_id"></param>
        /// <returns></returns>
        public List<RCS_RT_OPD_SHIFT_DETAIL> getIndexOPDshift(string pSDate, string pEDate, string chart_no)
        {
            List<RCS_RT_OPD_SHIFT_DETAIL> OPDshift_Index = new List<RCS_RT_OPD_SHIFT_DETAIL>();
            try
            {
                SQLProvider SQL = new SQLProvider();
                //取得主表清單
                List<RCS_RT_OPD_SHIFT_DETAIL> temp = new List<RCS_RT_OPD_SHIFT_DETAIL>();
                string sql = string.Concat(
                    @"SELECT RECORDDATE AS opt_Shift_date
                        , OS_ID 
                        , CREATE_NAME
                        FROM RCS_RT_OPD_SHIFT_MASTER 
                    WHERE CHART_NO = '", chart_no,
                        "' AND RECORDDATE BETWEEN '", pSDate, "' AND '" + pEDate +
                        "' AND DATASTATUS ='1'");
                temp = DBLink.DBA.getSqlDataTable<RCS_RT_OPD_SHIFT_DETAIL>(sql).ToList();

                //取得明細清單
                List<DB_RCS_OPD_SHIFT_DTL> dtl = new List<DB_RCS_OPD_SHIFT_DTL>();
                sql = string.Concat(
                    @"SELECT * 
                        FROM RCS_RT_OPD_SHIFT_DETAIL 
                    WHERE OS_ID in('", string.Join("','", temp.Select(x => x.OS_ID).ToList()), @"')");
                dtl = DBLink.DBA.getSqlDataTable<DB_RCS_OPD_SHIFT_DTL>(sql).ToList();

                List<Dictionary<string, object>> tempDict = new List<Dictionary<string, object>>();
                foreach (RCS_RT_OPD_SHIFT_DETAIL item in temp)
                {
                    if (dtl.Exists(x => x.OS_ID == item.OS_ID))
                    {
                        Dictionary<string, object> dict = new Dictionary<string, object>();

                        foreach (DB_RCS_OPD_SHIFT_DTL val in dtl.FindAll(x => x.OS_ID == item.OS_ID))
                        {

                                dict.Add(val.ITEM_NAME, val.ITEM_VALUE);
                   
                        }
                        tempDict.Add(dict);
                    }
                }
                OPDshift_Index = JsonConvert.DeserializeObject<List<RCS_RT_OPD_SHIFT_DETAIL>>(JsonConvert.SerializeObject(tempDict));

            }
            catch (Exception ex)
            {
                LogTool.SaveLogMessage(ex, "getIndexOPDshift", "OPDShiftController");
            }
            return OPDshift_Index;
        }

        /// <summary>
        /// DELETE
        /// </summary>
        /// <param name="ID"></param>
        /// <returns></returns>
        public RESPONSE_MSG Del(string ID)
        {
            RESPONSE_MSG rm = new RESPONSE_MSG();
            string actionName = "Del";
            try
            {
                SQLProvider SQL = new SQLProvider();
                string sql = "UPDATE RCS_RT_OPD_SHIFT_MASTER SET DATASTATUS = '9' WHERE OS_ID = @OS_ID";
                Dapper.DynamicParameters dp = new Dapper.DynamicParameters();
                dp.Add("OS_ID", ID);
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


        public RESPONSE_MSG OPDShift_Save(RCS_RT_OPD_SHIFT_DETAIL rdta, IPDPatientInfo pat_info, UserInfo user_info)
        {
            RESPONSE_MSG rm = new RESPONSE_MSG();
            RESPONSE_MSG reMSG = new RESPONSE_MSG();
            string actionName = "OPDShift_Save";
            bool is_MODIFY = string.IsNullOrWhiteSpace(rdta.OS_ID) ? false : true;
            try
            {
                //寫入 數據回寫院內
                string nowDate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                rm.status = RESPONSE_STATUS.SUCCESS;


                Dapper.DynamicParameters dp_MST = null, dp_DEL = null;
                string sql_MST = "", sql_DTL = "";

                // 主檔儲存
                if (rm.status == RESPONSE_STATUS.SUCCESS)
                {

                    #region 主表
                    if (!is_MODIFY)
                    {
                        rdta.OS_ID = DBLink.GetFixedStrSerialNumber();
                        #region 新增SQL

                        sql_MST = @"
INSERT INTO RCS_RT_OPD_SHIFT_MASTER (
IPD_NO,OS_ID,CREATE_ID,CREATE_NAME,CREATE_DATE,CHART_NO,RECORDDATE,DATASTATUS,PAT_SOURCE,PAT_DATA_DATE,COST_CODE,BED_NO,DEPT_CODE
) VALUES (
@IPD_NO,@OS_ID,@CREATE_ID,@CREATE_NAME,@CREATE_DATE,@CHART_NO,@RECORDDATE,@DATASTATUS,@PAT_SOURCE,@PAT_DATA_DATE,@COST_CODE,@BED_NO,@DEPT_CODE
)";
                        dp_MST = new Dapper.DynamicParameters();

                        dp_MST.Add("IPD_NO", pat_info.ipd_no);
                        dp_MST.Add("OS_ID", rdta.OS_ID);
                        dp_MST.Add("CREATE_ID", user_info.user_id);
                        dp_MST.Add("CREATE_NAME", user_info.user_name);
                        dp_MST.Add("CREATE_DATE", nowDate);
                        dp_MST.Add("CHART_NO", pat_info.chart_no);
                        dp_MST.Add("RECORDDATE", string.IsNullOrEmpty(rdta.opt_Shift_date) ? "" : rdta.opt_Shift_date);
                        dp_MST.Add("DATASTATUS", "1");
                        dp_MST.Add("PAT_SOURCE", string.IsNullOrEmpty(pat_info.source_type) ? "" : pat_info.source_type);
                        dp_MST.Add("PAT_DATA_DATE", string.IsNullOrEmpty(pat_info.diag_date) ? "" : pat_info.diag_date);
                        dp_MST.Add("COST_CODE", string.IsNullOrEmpty(pat_info.cost_code) ? "" : pat_info.cost_code);
                        dp_MST.Add("BED_NO", string.IsNullOrEmpty(pat_info.bed_no) ? "" : pat_info.bed_no);
                        dp_MST.Add("DEPT_CODE", string.IsNullOrEmpty(pat_info.dept_code) ? "" : pat_info.dept_code);

                        #endregion
                    }
                    else
                    {
                        #region 修改SQL

                        sql_MST = @"
UPDATE RCS_RT_OPD_SHIFT_MASTER 
SET MODIFY_ID = @MODIFY_ID,MODIFY_NAME = @MODIFY_NAME,MODIFY_DATE = @MODIFY_DATE,RECORDDATE = @RECORDDATE
WHERE OS_ID = @OS_ID
";
                        dp_MST = new Dapper.DynamicParameters();
                        dp_MST.Add("RECORDDATE", string.IsNullOrEmpty(rdta.opt_Shift_date) ? "" : rdta.opt_Shift_date);
                        dp_MST.Add("MODIFY_ID", user_info.user_id);
                        dp_MST.Add("MODIFY_NAME", user_info.user_name);
                        dp_MST.Add("UPLOAD_STATUS", "0");
                        dp_MST.Add("MODIFY_DATE", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
                        dp_MST.Add("OS_ID", rdta.OS_ID);

                        #endregion
                    }
                    #endregion

                    #region 明細

                    sql_DTL = "INSERT INTO RCS_RT_OPD_SHIFT_DETAIL Values (@OS_ID,@ITEM_NAME,@ITEM_VALUE);";


                    rdta.CREATE_NAME = user_info.user_name;



                    //新增RTRECORD_DETAIL
                    PropertyInfo[] props = null;
                    List<DB_RCS_OPD_SHIFT_DTL> list_DTL = new List<DB_RCS_OPD_SHIFT_DTL>();
                    var formDeatail = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(Newtonsoft.Json.JsonConvert.SerializeObject(rdta));
                    foreach (KeyValuePair<string, object> item in formDeatail)
                    {
                        if (item.Value != null)
                        {
                            list_DTL.Add(new DB_RCS_OPD_SHIFT_DTL()
                            {
                                OS_ID = rdta.OS_ID,
                                ITEM_NAME = item.Key,
                                ITEM_VALUE = item.Value is string ?
                                item.Value.ToString() : Newtonsoft.Json.JsonConvert.SerializeObject(item.Value)
                            });
                        }
                        else
                        {
                            list_DTL.Add(new DB_RCS_OPD_SHIFT_DTL()
                            {
                                OS_ID = rdta.OS_ID,
                                ITEM_NAME = item.Key,
                                ITEM_VALUE = ""
                            });
                        }
                    }


                    #endregion


                    #region BeginTrans


                    DBLink.DBA.BeginTrans();
                    DBLink.DBA.DBExecute(sql_MST, dp_MST);
                    if (is_MODIFY)
                    {
                        dp_DEL = new Dapper.DynamicParameters();
                        dp_DEL.Add("OS_ID", rdta.OS_ID);
                        DBLink.DBA.DBExecute("DELETE RCS_RT_OPD_SHIFT_DETAIL WHERE OS_ID = @OS_ID", dp_DEL);
                    }


                    DBLink.DBA.DBExecute(sql_DTL, list_DTL);
                    if (DBLink.DBA.hasLastError)
                    {
                        LogTool.SaveLogMessage(DBLink.DBA.lastError, actionName, this.csName);
                        rm.status = RESPONSE_STATUS.ERROR;
                        rm.message = "儲存失敗!";
                        DBLink.DBA.Rollback();
                    }
                    else
                    {
                        rm.status = RESPONSE_STATUS.SUCCESS;
                        rm.message = "儲存成功!";
                        DBLink.DBA.Commit();
                    }


                    #endregion
                }
                else
                {
                    if (reMSG.message == null || reMSG.message == "")
                    {
                        rm.message = "上傳系統發生例外，請洽資訊人員!";
                    }
                    else
                    {
                        rm.message = reMSG.message;
                    }
                    rm.status = RESPONSE_STATUS.ERROR;
                }
            }
            catch (Exception ex)
            {
                string tmp = ex.Message;
                LogTool.SaveLogMessage(tmp, "CPTAssess_Save");
                rm.status = RESPONSE_STATUS.EXCEPTION;
                rm.message = "系統發生例外，請聯繫資訊人員";
            }
            finally
            {
                if (rm.status == RESPONSE_STATUS.SUCCESS)
                    DBLink.DBA.Commit();
                else
                    DBLink.DBA.Rollback();
            }
            return rm;
        }
    }

    public class RtOPDShift_Save : AUTH
    {
        public RCS_RT_OPD_SHIFT_DETAIL model { get; set; }

    }
    public class RtOPDShiftDetail 
    {
        /// <summary>
        /// 取得資料
        /// </summary>
        public RCS_RT_OPD_SHIFT_DETAIL model { get; set; }

        /// <summary>
        /// 最後一筆ID
        /// </summary>
        public string last_id { get; set; }

        public List<string> drug_inhalation { get; set; }

    }


    public class FormRtOPDShiftDetail : AUTH
    {
        public string ID { get; set; }
    }

    public class FormRtOPDShiftList : AUTH
    {
        public string sDate { get; set; }
        public string eDate { get; set; }
    }

    public class OpdShift_MainTable_PDF
    {
        public IPDPatientInfo PatientInformation { get; set; }
        public List<RCS_RT_OPD_SHIFT_DETAIL> OpdShiftDetail_rList { get; set; }
    }

    public class RCS_RT_OPD_SHIFT_DETAIL 
    {
        /// <summary> 門診_記錄時間: </summary>
        public string opt_Shift_date { set; get; }
        /// <summary> 門診_看診日: </summary>
        public string opd_date { set; get; }
        /// <summary> 門診_流水號PK: </summary>
        public string OS_ID { set; get; }
        /// <summary>記錄者</summary>
        public string CREATE_NAME { set; get; }
        /// <summary> 門診_過去病史: </summary>
        public string OPD_Shift_history { set; get; }
        public string OPD_Shift_history_1 { set; get; }
        public string OPD_Shift_history_2 { set; get; }
        public string OPD_Shift_history_3 { set; get; }
        public string OPD_Shift_history_4 { set; get; }
        public string OPD_Shift_his_other { set; get; }
        public string OPD_Shift_his_other_txt { set; get; }

        /// <summary> 門診_病患居家狀況:</summary>
        public string OPD_In_House_Condition { set; get; }
        public string OPD_In_House_Condition_1 { set; get; }
        public string OPD_In_House_Condition_2 { set; get; }
        public string OPD_In_House_Condition_3 { set; get; }
        public string OPD_In_House_Condition_4 { set; get; }
        public string OPD_In_House_other { set; get; }
        public string OPD_In_House_other_txt { set; get; }

        /// <summary> 門診_HR、SpO2、BP、BS</summary>
        public string OPD_base_data { set; get; }

        public string HR { set; get; }
        public string HR2 { set; get; }
        public string BP { set; get; }
        public string BP2 { set; get; }
        public string SpO2 { set; get; }
        public string SpO2_2 { set; get; }
        public string BS { set; get; }
        public string BS2 { set; get; }

        /// <summary> 門診_Sputum </summary>
        public string Sputum { set; get; }

        /// <summary>
        /// 呼吸音
        /// </summary>
        public string breath_sound { get; set; }
        /// <summary> 門診_CAT </summary>
        public string CAT { get; set; }
        /// <summary> 門診_CAT </summary>
        public string CAT_1 { get; set; }
        /// <summary> 門診_CAT </summary>
        public string CAT_2 { get; set; }
        /// <summary> 門診_CAT </summary>
        public string CAT_3 { get; set; }
        /// <summary> 門診_CAT </summary>
        public string CAT_4 { get; set; }
        /// <summary> 門診_CAT </summary>
        public string CAT_5 { get; set; }
        /// <summary> 門診_CAT </summary>
        public string CAT_6 { get; set; }
        public string CAT_7 { get; set; }
        public string CAT_8 { get; set; }
        public string CAT_Total { get; set; }

        public string OPDShift_treatment { get; set; } //2018.12.28新增 門診交班_療程
        public string mMRC { get; set; }
        /// <summary> 門診_Program</summary>
        public string Program { set; get; }
        public string Program_B { set; get; }
        public string Program_Drug_Inhalation { set; get; } //2018.10.01

        public string drug_inhalation_1A { get; set; }
        public string drug_inhalation_1B { get; set; }
        public string drug_inhalation_2A { get; set; }
        public string drug_inhalation_2B { get; set; }
        public string drug_inhalation_3A { get; set; }
        public string drug_inhalation_3B { get; set; }

        /// <summary>  門診_ 治療後評值及特別交班 備註</summary>
        public string afterAssess_And_Shift_memo { set; get; }

        public string Program_6_MIN_PFT { set; get; }

        public string sputum_color { get; set; }
        public string sputum_color_1 { get; set; }
        public string sputum_color_2 { get; set; }
        public string sputum_color_3 { get; set; }
        public string sputum_color_4 { get; set; }
        public string sputum_color_5 { get; set; }
        public string sputum_color_6 { get; set; }
        public string sputum_color_7 { get; set; }
        public string sputum_color_8 { get; set; }

        public string sputum_small { get; set; }

        public string sputum_status_2 { get; set; }
        public string sputum_status_1 { get; set; }
        public string sputum_status { get; set; }

        public string sputum_count { get; set; }
        public string sputum_count_1 { get; set; }
        public string sputum_count_2 { get; set; }

        public string sputum_count_3 { get; set; }

        public string Breathing_exercise { get; set; }
        public string Program_check { set; get; }
        public string Program_PEP { set; get; }
        public string Program_IPPB { set; get; }
        public string Program_IMT { set; get; }
        public string Program_MIPorMEP { set; get; }
        public string Program_NC { set; get; }
        public string Program_CPAP_FIO2 { set; get; }
        public string Program_CPAP_Flow { set; get; }
        public string Program_Incentive_CC { set; get; }
        public string Program_Incentive_sec { set; get; }
        public string Program_Diaphragmatic_kg { set; get; }
        public string Program_Diaphragmatic_min { set; get; }
        public string Program_Other { set; get; }
    }
}

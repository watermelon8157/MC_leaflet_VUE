using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Text;
using Newtonsoft.Json;
using RCS_Data;
using mayaminer.com.library;
using Com.Mayaminer;
using System.Xml.Serialization;
using System.Web.Mvc;
using RCS_Data.Models.ViewModels;
using RCSData.Models;
using RCS_Data.Models.DB;

namespace RCS.Models.ViewModel
{
    public class RTTakeoffAssess : BaseModel
    {
        /// <summary>
        /// 取得呼吸器脫離評估列資料
        /// </summary>
        /// <param name="pWhere">查詢條件</param>
        /// <returns></returns>
        public DataTable getWeanAssess(string pWhere)
        {
            DataTable dt = this.DBA.getDataTable(GetTableName.RCS_WEANING_ASSESS.ToString(), pWhere);
            return dt;
        }

        /// <summary>
        /// 取得脫離評估單上傳狀態
        /// </summary>
        /// <param name="pTkid">脫離評估單號</param>
        /// <returns></returns>
        public string getWeaningUploadStatus(string pTkid)
        {
            pTkid = pTkid ?? "";
            object obj = this.DBA.ExecuteScalar(string.Format("SELECT UPLOAD_STATUS FROM {1} WHERE TK_ID={0}", SQLDefend.SQLString(pTkid), GetTableName.RCS_WEANING_ASSESS.ToString()));
            if (obj != null && obj.ToString().Trim() != "")
                return obj.ToString().Trim();

            return "";
        }

        /// <summary>
        /// 儲存評估單
        /// </summary>
        /// <param name="pRdta">評估單資料定義</param>
        /// <param name="pRm">資料處理結果</param>
        /// <param name="pHadUpload">是否已上傳</param>
        /// <returns></returns>
        public bool saveWeanAssess(ref RCS_DATA_TK_ASSESS pRdta, ref RESPONSE_MSG pRm)
        {
            pRdta.tk_id = pRdta.tk_id ?? "";
            try
            {
                SQLProvider SQL = new SQLProvider();
                bool pHadUpload = pRdta.UPLOAD_STATUS == "1";
                DataRow SaveDr;
                DataTable SaveDt;
                if (!pHadUpload && pRdta.tk_id != "")
                    SaveDt = this.getWeanAssess(string.Format("WHERE tk_id = {0}", SQLDefend.SQLString(pRdta.tk_id)));
                else
                    SaveDt = this.getWeanAssess("WHERE 1<>1");

                string UpdateStr = "";
                if (SaveDt != null)
                {
                    if (!pHadUpload && pRdta.tk_id != "")
                        SaveDr = SaveDt.Rows[0];
                    else
                        SaveDr = SaveDt.NewRow();

                    //處理共用
                    SaveDr["rec_date"] = pRdta.rec_date;
                    SaveDr["chart_no"] = pat_info.chart_no;
                    SaveDr["ipd_no"] = pat_info.ipd_no;

                    SaveDr["DEPT_CODE"] = pat_info.dept_code;//記錄科別
                    SaveDr["COST_CODE"] = pat_info.cost_code;//記錄護理站
                    SaveDr["BED_NO"] = pat_info.bed_no;//記錄床號

                    SaveDr["datastatus"] = "1";
                    SaveDr["PAT_SOURCE"] = string.IsNullOrEmpty(pat_info.source_type) ? "" : pat_info.source_type;
                    SaveDr["PAT_DATA_DATE"] = string.IsNullOrEmpty(pat_info.diag_date) ? "" : pat_info.diag_date;
                    if (!(!pHadUpload && pRdta.tk_id != ""))
                    {
                        SaveDr["create_date"] = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                        SaveDr["create_id"] = user_info.user_id;
                        SaveDr["create_name"] = user_info.user_name;
                        SaveDr["upload_status"] = "0";
                    }

                    //問卷部分
                    SaveDr["tk_reason"] = JsonConvert.SerializeObject(pRdta.tk_reason);
                    SaveDr["st_reason"] = JsonConvert.SerializeObject(pRdta.st_reason);
                    SaveDr["tk_plan"] = JsonConvert.SerializeObject(pRdta.tk_plan);
                    if (pRdta.tk_id == "")
                    {
                        //新增
                        pRdta.tk_id = SQL.GetFixedStrSerialNumber();
                        SaveDr["tk_id"] = pRdta.tk_id;
                        SaveDt.Rows.Add(SaveDr);
                    }
                    else
                    {
                        if (!pHadUpload && pRdta.tk_id != "")
                        {
                            //未上傳的更新資料
                            SaveDr["modify_date"] = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                            SaveDr["modify_id"] = user_info.user_id;
                            SaveDr["modify_name"] = user_info.user_name;
                        }
                        else
                        {
                            //已上傳的更新資料
                            string UpdateCondition = string.Format("WHERE TK_ID = {0} OR NEW_TK_ID={0}", SQLDefend.SQLString(pRdta.tk_id));
                            pRdta.tk_id = SQL.GetFixedStrSerialNumber();

                            //更新該筆舊的脫離評估
                            UpdateStr = string.Format("UPDATE {2} SET NEW_TK_ID={0},DATASTATUS='2' {1}", SQLDefend.SQLString(pRdta.tk_id), UpdateCondition, GetTableName.RCS_WEANING_ASSESS.ToString());

                            //新增一筆脫離評估，TK_ID=被更新的NEW_TK_ID
                            SaveDr["tk_id"] = pRdta.tk_id;
                            SaveDt.Rows.Add(SaveDr);
                        }
                    }
                    mayaminer.com.jxDB.dbResultMessage rc = this.DBA.UpdateResult(SaveDt, GetTableName.RCS_WEANING_ASSESS.ToString());
                    if (rc.State == mayaminer.com.jxDB.enmDBResultState.Success)
                    {
                        if (UpdateStr != null && UpdateStr != "")
                        {
                            this.DBA.ExecuteNonQuery(UpdateStr);
                            if (this.DBA.LastError == "")
                            {
                                this.DBA.Commit();
                                pRm.status = RESPONSE_STATUS.SUCCESS;
                                pRm.message = "儲存成功";
                                return true;
                            }
                            else
                            {
                                this.DBA.Rollback();
                                pRm.status = RESPONSE_STATUS.ERROR;
                                pRm.message = "儲存失敗";
                                return false;
                            }
                        }
                        else
                        {
                            this.DBA.Commit();
                            pRm.status = RESPONSE_STATUS.SUCCESS;
                            pRm.message = "儲存成功";
                            return true;
                        }
                    }
                    else
                    {
                        this.DBA.Rollback();
                        pRm.status = RESPONSE_STATUS.ERROR;
                        pRm.message = "儲存失敗";
                        return false;
                    }
                }
                else
                {
                    pRm.status = RESPONSE_STATUS.ERROR;
                    pRm.message = "儲存失敗";
                    return false;
                }
            }
            catch (Exception ex)
            {
                LogTool.SaveLogMessage(ex, "RTTakeoffSave");
                this.DBA.Rollback();
                pRm.status = RESPONSE_STATUS.EXCEPTION;
                pRm.message = "系統發生例外，請聯繫資訊人員";
                return false;
            }

        }

        /// <summary>
        /// 刪除評估單
        /// </summary>
        /// <param name="pRdta">評估單資料定義</param>
        /// <param name="pRm">資料處理結果</param>
        /// <returns></returns>
        public bool deleteWeanAssess(ref RCS_DATA_TK_ASSESS pRdta, ref RESPONSE_MSG pRm)
        {
            try
            {
                DataTable DelDt = this.getWeanAssess(string.Format("WHERE tk_id = {0}", SQLDefend.SQLString(pRdta.tk_id)));
                if (DelDt != null && DelDt.Rows.Count == 1)
                {
                    DelDt.Rows[0]["DATASTATUS"] = "9";
                    mayaminer.com.jxDB.dbResultMessage rc = this.DBA.UpdateResult(DelDt, GetTableName.RCS_WEANING_ASSESS.ToString());
                    if (rc.State == mayaminer.com.jxDB.enmDBResultState.Success)
                    {
                        this.DBA.Commit();
                        pRm.status = RESPONSE_STATUS.SUCCESS;
                        pRm.message = "刪除成功";
                        return true;
                    }
                    else
                    {
                        this.DBA.Rollback();
                        pRm.status = RESPONSE_STATUS.ERROR;
                        pRm.message = "刪除失敗";
                        return false;
                    }
                }
                else
                {
                    pRm.status = RESPONSE_STATUS.ERROR;
                    pRm.message = "刪除失敗";
                    return false;
                }
            }
            catch (Exception ex)
            {
                LogTool.SaveLogMessage(ex, "RTTakeoffSave");
                this.DBA.Rollback();
                pRm.status = RESPONSE_STATUS.EXCEPTION;
                pRm.message = "系統發生例外，請聯繫資訊人員";
                return false;
            }
        }

        /// <summary>
        /// 取得呼吸治療照護評估單脫離評估資料
        /// </summary>     
        /// <param name="pSDate">開始日期</param>
        /// <param name="pEDate">結束日期</param>
        /// <param name="pId">脫離評估單號</param>
        /// <returns></returns>
        public DataTable getWeaningAssessWeaningPrint(string pSDate, string pEDate, string pIpdno, string pId = "")
        {
            string Condition = string.Format(" AND DATASTATUS='1' AND CHART_NO={0} AND REC_DATE >= {1} AND REC_DATE <= {2}", SQLDefend.SQLString(pat_info.chart_no), SQLDefend.SQLString(pSDate), SQLDefend.SQLString(pEDate));
            if (pId != "") Condition = string.Format(" AND TK_ID IN ('{0}')", pId);
            if (!string.IsNullOrWhiteSpace(pIpdno)) Condition += string.Format(" AND IPD_NO = '{0}'", getHistoryList(pIpdno, 0));
            if (user_info.authority == "readonly") Condition += " AND UPLOAD_STATUS='1'";
            string str = "SELECT TK_ID,REC_DATE,TK_REASON,ST_REASON,TK_PLAN,CREATE_NAME FROM {1} WHERE 1=1 {0} ORDER BY REC_DATE ASC";
            return this.DBA.getSqlDataTable(string.Format(str, Condition, GetTableName.RCS_WEANING_ASSESS.ToString()));
        }
 

        /// <summary>
        /// 取得批價序號清單BY脫離評估單號
        /// </summary>
        /// <param name="pId">脫離評估單號</param>
        /// <returns></returns>
        public string getChartNoList(string pId)
        {
            string str = "SELECT DISTINCT CHART_NO FROM {1} WHERE TK_ID IN ('{0}')";
            DataTable Dt = this.DBA.getSqlDataTable(string.Format(str, pId, GetTableName.RCS_WEANING_ASSESS.ToString()));
            if (Dt != null && Dt.Rows.Count > 0)
                return string.Join("','", Dt.AsEnumerable().Select(x => x.Field<string>("CHART_NO")).ToArray());

            return "";
        }

        /// <summary>
        /// 取得脫離評估單單號清單BY病歷號
        /// </summary>
        /// <param name="pChartNo">病歷號</param>
        /// <returns></returns>
        public List<string> getTKIDs(string pChartNo)
        {
            string str = "SELECT TK_ID FROM {1} WHERE CHART_NO = {0}";
            DataTable Dt = this.DBA.getSqlDataTable(string.Format(str, SQLDefend.SQLString(pChartNo), GetTableName.RCS_WEANING_ASSESS.ToString()));
            if (Dt != null && Dt.Rows.Count > 0)
                return Dt.AsEnumerable().Select(x => x.Field<string>("TK_ID")).ToList();

            return new List<string>();
        }

        /// <summary>
        /// 取得適應症列印項目
        /// </summary>
        /// <returns></returns>
        public string getBreathIndicationsItems()
        {
            string _result = "";
            List<SysParams> BreathIndicationsList = this.getRCS_SYS_PARAMS(pModel: "RTRecord_Detail", pGroup: "Indications", pStatus: "1");
            if (BreathIndicationsList != null && BreathIndicationsList.Count > 0)
                _result = string.Join(", ", BreathIndicationsList.Select(x => x.P_VALUE + "." + x.P_NAME).ToList());

            return _result;
        }

 
        /// <summary>
        /// 取得適應症、脫離評估有勾選的資料
        /// </summary>
        /// <param name="pJson">Json字串</param>
        /// <param name="pIfBI">是否為適應症</param>
        /// <returns></returns>
        public string setWeaningJsonData(string pJson, string pSymbol = ",", bool pIfBI = false, bool pIfUpload = false)
        {
            string ans = "";
            List<JSON_DATA> tmpJson = JsonConvert.DeserializeObject<List<JSON_DATA>>(pJson);
            if (pIfBI)
            {
                tmpJson = tmpJson.Where(x => x.id == "breath_indications").ToList();
                if (tmpJson != null && tmpJson.Count > 0)
                {
                    if (pIfUpload)
                    {
                        if (tmpJson.Count(x => string.IsNullOrWhiteSpace(x.txt) && x.txt.Trim() == "") == 0) { }
                        ans = tmpJson.Select(x => x.txt).First();
                    }
                    else
                    {
                        if (tmpJson.Count(x => string.IsNullOrWhiteSpace(x.val) && x.val.Trim() == "") == 0) { }
                        ans = tmpJson.Select(x => x.val).First();
                    }
                }
            }
            else
            {
                tmpJson = tmpJson.Where(x => x.chkd && x.val != null && x.val.Trim() != "").ToList();
                if (tmpJson != null && tmpJson.Count > 0)
                {
                    if (pIfUpload)
                    {
                        ans = trans_special_code(string.Join(pSymbol, tmpJson.Select(x => x.txt).ToArray()));
                    }
                    else
                        ans = string.Join(pSymbol, tmpJson.Select(x => x.val).ToArray());
                }
            }
            tmpJson = null;
            if (pIfUpload && string.IsNullOrWhiteSpace(ans))
                ans = Controllers.BaseController.upLoadNullStr;
            return ans;
        }
 
    }

    /// <summary>呼吸器脫離評估列印資料集</summary>
    public class RTWeaningPrtContainer
    {
        public RTWeaningPrtContainer()
        {
            data = new Document();
        }
        /// <summary>
        /// 病患身分證
        /// </summary>
        public string PatientID { get; set; }
        /// <summary>
        /// 本文流水號
        /// </summary>
        public string record_id { get; set; }
        /// <summary>
        /// 表單記錄日期(recorddate + " " + recordtime)
        /// </summary>
        public string record_date
        {
            get
            {
                return data.RecDate + " " + data.RecTime;
            }
        }
        /// <summary>
        /// 批價序號
        /// </summary>
        public string ipdNo
        {
            get
            {
                return data.ipdNo;
            }
        }
        /// <summary>
        /// 病歷號
        /// </summary>
        public string chartNo
        {
            get
            {
                return data.chartNo;
            }
        }
        /// <summary>
        /// 病患姓名
        /// </summary>
        public string patName
        {
            get
            {
                return data.patName;
            }
        }
        /// <summary>
        /// 來源
        /// </summary>
        public string sourceType { get; set; }
        /// <summary>
        /// 本文內容
        /// </summary>
        public RTWeaningPrtContainer.Document data { get; set; }
        public class Document : xmlBaseData
        {
            /// <summary>
            /// 科別
            /// </summary>
            [XmlElement(ElementName = "DEPT_NAME")]
            //[XmlElement(Namespace = "科別",ElementName = "diagnosis_code")]
            public string DEPT_NAME = "";
            /// <summary>
            /// 主治醫生
            /// </summary>
            [XmlElement(ElementName = "VS_DOC_NAME")]
            //[XmlElement(Namespace = "主治醫生",ElementName = "diagnosis_code")]
            public string VS_DOC_NAME = "";
            /// <summary>
            /// 診斷
            /// </summary>
            [XmlElement(ElementName = "diagnosis_code")]
            //[XmlElement(Namespace = "診斷",ElementName = "diagnosis_code")]
            public string diagnosis_code = "";
            /// <summary>日期</summary>
            public string RecDate = "";
            /// <summary>時間</summary>
            public string RecTime = "";
            /// <summary>適應症</summary>
            public string BreathIndications = "";
            /// <summary>無法脫離原因</summary>
            public string TkReason = "";
            /// <summary>停止脫離原因</summary>
            public string StReason = "";
            /// <summary>呼吸器脫離計畫</summary>
            public string TkPlan = "";
            /// <summary>治療師簽章</summary>
            public string CreateName = "";
        }
    }

    public class RTTakeoffAssessViewModel : RCS_DATA_TK_ASSESS
    {
        /// <summary>
        /// 顯示勾選符號(布林值)
        /// </summary>
        /// <param name="isChecked"></param>
        /// <returns></returns>
        public string showCheckBox(string isChecked = "0")
        {
            string returnVal = "□";
            if (!string.IsNullOrWhiteSpace(isChecked) && isChecked == "1")
            {
                return "■";
            }

            return returnVal;
        }
        public string last_tk_id { get; set; }
        public VM_RTTakeoffAssess DTL { get; set; }
        public bool checkOther { get; set; }

        public IPDPatientInfo pat_info { get; set; }
        public DB_RCS_WEANING_ASSESS master { get; set; }
        public UserInfo user_info { get; set; }
        public List<RCS_WEANING_ITEM> WeaningProfile_List { get; set; }
        
    }
     
    /// <summary>
    /// 系統畫面基本顯示資料
    /// </summary>
    public class ViewModelBasic : ViewModelRTTakeoffAssess
    {
        /// <summary>
        /// 住院/急診/門診日期
        /// </summary>
        public List<SelectListItem> getPatientHistoryList { get; set; } 
    }
    public class ListRTTakeoffAssessViewModel
    {
        public List<RTTakeoffAssessViewModel> List { get; set; }
    }
}
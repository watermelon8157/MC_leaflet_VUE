//**************************************************
//2016/08/12
//#2172 呼吸照護記錄單Model
//功能 取得 呼吸照護記錄單的資料
//2016/08/12 建立:呼吸照護記錄單Model(繼承BaseModel)
//**************************************************
using RCS_Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using mayaminer.com.library;
using Com.Mayaminer;
using Newtonsoft.Json;
using System.Reflection;
using mayaminer.com.jxDB;
using System.Xml.Serialization;
using RCSData.Models;
using RCS_Data.Models.ViewModels;
using RCS_Data.Controllers.RtRecord;
using RCS_Data.Models.DB;
using RCS_Data.Controllers.RtValuation;

namespace RCS.Models
{
    public class RTRecord : BaseModel
    {
        string csName { get { return "RTRecord"; } }
        /// <summary>分業筆數設定</summary>
        public static int pageCnt = 5;
        private CxrCanvasLineDraw CxrCanvasLineDrawMdl; //2018.08.25 CXR畫線
        public RTRecord()
        {
            CxrCanvasLineDrawMdl = new CxrCanvasLineDraw(); //2018.07.20 CXR畫線
        } 
       

        #region 呼吸照護記錄單

        #region RECORD_ID (新增、修改、VIP)

        /// <summary>取得指定RECORD_ID呼吸紀錄(新增、修改、VIP)</summary>

        /// <param name="RECORD_ID">RECORD_ID</param>
        /// <returns></returns>

        public void getDuringRTRecord(ref RT_RECORD_MAIN item, string setIpdno)
        {
            DateTime dateNow = DateTime.Now;
            Ventilator lastRecord = this.basicfunction.GetLastRTRec(pat_info.chart_no, setIpdno);
            try
            {
                string sql = "";
                DataTable dt = null;
                List<string> ONMODE_TYPE_ID_list = new List<string>();

              
                if (item.RECORD_ID != null && item.RECORD_ID != "")
                {
                    sql = @"SELECT M.*,
(SELECT o.ONMODE_ID FROM RCS_ONMODE_MASTER o WHERE M.ONMODE_TYPE1_ID = o.ONMODE_ID 
AND (M.RECORDDATE BETWEEN o.STARTDATE AND o.ENDDATE OR M.RECORDDATE >= o.STARTDATE AND (o.ENDDATE is NULL OR o.ENDDATE = '') AND DATASTATUS in('1','3')) 
 ) ONMODE_TYPE1_ID, 
(SELECT o.ONMODE_ID FROM RCS_ONMODE_MASTER o WHERE M.ONMODE_TYPE2_ID = o.ONMODE_ID 
AND (M.RECORDDATE BETWEEN o.STARTDATE AND o.ENDDATE OR M.RECORDDATE >= o.STARTDATE AND (o.ENDDATE is NULL OR o.ENDDATE = '') AND DATASTATUS in('1','3')) 
 ) ONMODE_TYPE2_ID,  
(SELECT o.ONMODE_ID FROM RCS_ONMODE_MASTER o WHERE M.ONMODE_TYPE3_ID = o.ONMODE_ID 
AND (M.RECORDDATE BETWEEN o.STARTDATE AND o.ENDDATE OR M.RECORDDATE >= o.STARTDATE AND (o.ENDDATE is NULL OR o.ENDDATE = '') AND DATASTATUS in('1','3')) 
 ) ONMODE_TYPE3_ID 

FROM {2} M WHERE CHART_NO = {0} 
AND RECORD_ID = {1} 
ORDER BY RECORDDATE DESC ";

                    sql = string.Format(sql, SQLDefend.SQLString(pat_info.chart_no), SQLDefend.SQLString(item.RECORD_ID), GetTableName.RCS_RECORD_MASTER.ToString());
                    dt = this.DBA.getSqlDataTable(sql);
                    if (this.DBA.LastError != null && this.DBA.LastError != "")
                    {
                        dt = new DataTable();
                        LogTool.SaveLogMessage(this.DBA.LastError, "getDuringRTRecord2", GetLogToolCS.RTRecord);
                    }
                    if (dt != null && dt.Rows.Count > 0)
                    {
                        foreach (DataRow MstRTDr in dt.Rows)
                        {
                            bindRT_RECORD_MAIN(ref item, MstRTDr);
                            getRT_RECORD_Detail(ref item, item.RECORD_ID);
                        }
                    }
                    item.isModify = true;
                    if (item.ONMODE_TYPE1_ID != "") ONMODE_TYPE_ID_list.Add(item.ONMODE_TYPE1_ID);
                    if (item.ONMODE_TYPE2_ID != "") ONMODE_TYPE_ID_list.Add(item.ONMODE_TYPE2_ID);
                    if (item.ONMODE_TYPE3_ID != "") ONMODE_TYPE_ID_list.Add(item.ONMODE_TYPE3_ID);

                }
                else
                {
                    #region 新增時，要帶出最後一筆的資料
                    //if (!item.isVIP)
                    //{
                    //    //新增時，如果最後一筆有藥物備註，要帶出來
                    //    if (lastRecord.drug_memo != null && lastRecord.drug_memo != "")
                    //    {
                    //        item.rt_record.drug_memo = lastRecord.drug_memo;
                    //    }
                    //    //新增時，如果最後一筆有fio2_set，要帶出來
                    //    if (!string.IsNullOrWhiteSpace(lastRecord.fio2_set))
                    //    {
                    //        item.rt_record.fio2_set = lastRecord.fio2_set;
                    //    }
                    //    //新增時，如果最後一筆有flow，要帶出來
                    //    if (!string.IsNullOrWhiteSpace(lastRecord.flow))
                    //    {
                    //        item.rt_record.flow = lastRecord.flow;
                    //    }
                    //}
                    #endregion

                    //如果有補單時間的話
                    if (!string.IsNullOrWhiteSpace(item.RECORDDATE) && DateHelper.isDate(item.RECORDDATE))
                    {
                        DateTime tempDate = DateHelper.Parse(item.RECORDDATE);
                        item.rt_record.recorddate = tempDate.ToString("yyyy-MM-dd");
                        item.rt_record.recordtime = tempDate.ToString("HH:mm");
                    }
                    else
                    {
                        if (item.rt_record.recorddate == null || item.rt_record.recorddate == "")
                            item.rt_record.recorddate = dateNow.ToString("yyyy-MM-dd");
                        if (item.rt_record.recordtime == null || item.rt_record.recordtime == "")
                            item.rt_record.recordtime = dateNow.ToString("HH:mm");
                        //新增呼吸單時間與上一筆時間相同時，時間自動加上1分鐘//小白機也要判斷
                        if (lastRecord.RECORDDATE != null && lastRecord.RECORDDATE != "")
                        {
                            if (dateNow.ToString("yyyy-MM-dd HH:mm") == DateHelper.Parse(lastRecord.RECORDDATE).ToString("yyyy-MM-dd HH:mm")
                                && lastRecord.RECORD_ID != item.RECORD_ID)
                            {
                                item.RECORDDATE = dateNow.AddMinutes(1).ToString("yyyy-MM-dd HH:mm");
                                item.rt_record.recorddate = dateNow.AddMinutes(1).ToString("yyyy-MM-dd");
                                item.rt_record.recordtime = dateNow.AddMinutes(1).ToString("HH:mm");
                            }
                            else
                                item.RECORDDATE = DateTime.Parse(string.Format("{0} {1}", item.rt_record.recorddate, item.rt_record.recordtime)).ToString("yyyy-MM-dd HH:mm");
                        }
                        else
                            item.RECORDDATE = DateTime.Parse(string.Format("{0} {1}", item.rt_record.recorddate, item.rt_record.recordtime)).ToString("yyyy-MM-dd HH:mm");

                    }


                } 

               

                //新增時，不是VIP資料，如果有ON呼吸，自動帶入最後一筆呼吸編號
                if ((item.RECORD_ID == null || item.RECORD_ID == "") && !item.isVIP && item.onBreath.hasData)
                    item.rt_record.respid = lastRecord.respid;
                List<string> on_dateLIst = new List<string>();
                //取得使用天數
                item.useDays = getUseDays(pat_info.ipd_no, pat_info.chart_no, out on_dateLIst);
                //取得最後一筆記錄
                item.lastRecordID = lastRecord.RECORD_ID; 
            }
            catch (Exception ex)
            {
                item.isWsError = true;
                item.wsError = "程式錯誤，請洽資訊人員。錯誤訊息如下所示:" + ex.Message;
                LogTool.SaveLogMessage(ex, "getDuringRTRecord2", GetLogToolCS.RTRecord);
            }
            finally
            {
                #region 回傳前判斷 
                if (item.isVIP)
                {
                    //如果是VIP帶入的資料，如果有AutoFlow的，自動帶入Auto
                    string tempMode = item.rt_record.mode;

                    if (string.IsNullOrWhiteSpace(item.rt_record.humi_temp) && !string.IsNullOrWhiteSpace(lastRecord.humi_temp))
                        item.rt_record.humi_temp = lastRecord.humi_temp;
                    if (string.IsNullOrWhiteSpace(item.rt_record.gcse) && !string.IsNullOrWhiteSpace(lastRecord.gcse))
                        item.rt_record.gcse = lastRecord.gcse;
                    if (string.IsNullOrWhiteSpace(item.rt_record.gcsm) && !string.IsNullOrWhiteSpace(lastRecord.gcsm))
                        item.rt_record.gcsm = lastRecord.gcsm;
                    if (string.IsNullOrWhiteSpace(item.rt_record.gcsv) && !string.IsNullOrWhiteSpace(lastRecord.gcsv))
                        item.rt_record.gcsv = lastRecord.gcsv;
                    if (string.IsNullOrWhiteSpace(item.rt_record.conscious) && !string.IsNullOrWhiteSpace(lastRecord.conscious))
                        item.rt_record.conscious = lastRecord.conscious;

                    if (!string.IsNullOrWhiteSpace(item.rt_record.et_size))
                    {
                        if (item.onIntubate.DATA_CONTENT.Exists(x => x.id == "intubate_tube_diameter"))
                        {
                            item.onIntubate.DATA_CONTENT.Find(x => x.id == "intubate_tube_diameter").val = item.rt_record.et_size;
                        }
                    }
                    if (!string.IsNullOrWhiteSpace(item.rt_record.et_mark))
                    {
                        if (item.onIntubate.DATA_CONTENT.Exists(x => x.id == "intubate_tube_depth"))
                        {
                            item.onIntubate.DATA_CONTENT.Find(x => x.id == "intubate_tube_depth").val = item.rt_record.et_mark;
                        }
                    }
                }

                //判斷最後一筆插管資料是否有結束日期時間，如有的話，不帶入上次資料，如沒有的話，帶入插管直徑。
                //判斷最後一筆插管資料是否有結束日期時間，如有的話，不帶入上次資料，如沒有的話，帶入插管深度。
                if (item.onIntubate.ENDDATE != null && item.onIntubate.ENDDATE != "")
                    item.onIntubate.isEnd = true;
                 
                //判斷最後一筆插管資料是否有結束日期時間。
                if (item.onBreath.ENDDATE != null && item.onBreath.ENDDATE != "")
                    item.onBreath.isEnd = true;

                //判斷最後一筆氧氣治療資料是否有結束日期時間，如有的話，不帶入上次資料，如沒有的話，帶入治療種類。
                if (item.onOxygen.ENDDATE != null && item.onOxygen.ENDDATE != "")
                    item.onOxygen.isEnd = true;
                
                if (!String.IsNullOrWhiteSpace(lastRecord._is_humidifier) && lastRecord._is_humidifier.Contains("\\H"))
                {
                    item.rt_record.humidifier = new List<JSON_DATA>();
                    item.rt_record.humidifier.Add(new JSON_DATA() { id = "humidifier", chkd = true, val = "1" });
                    item.rt_record._is_humidifier = lastRecord._is_humidifier;
                }
                else
                {
                    //設定畫面 is_humidifier
                    if (item.rt_record.is_humidifier != null && item.rt_record.is_humidifier == "1")
                    {
                        item.rt_record.humidifier = new List<JSON_DATA>();
                        item.rt_record.humidifier.Add(new JSON_DATA() { id = "humidifier", chkd = true, val = "1" });
                    }
                    else
                    {
                        item.rt_record.humidifier = new List<JSON_DATA>();
                        item.rt_record.humidifier.Add(new JSON_DATA() { id = "humidifier", chkd = false, val = "0" });
                    }
                }

                //設定畫面 is_vbg
                if (item.rt_record.is_vbg != null && item.rt_record.is_vbg == "1")
                {
                    item.rt_record.vbg = new List<JSON_DATA>();
                    item.rt_record.vbg.Add(new JSON_DATA() { id = "vbg", chkd = true, val = "1" });
                }
                else
                {
                    item.rt_record.vbg = new List<JSON_DATA>();
                    item.rt_record.vbg.Add(new JSON_DATA() { id = "vbg", chkd = false, val = "0" });
                }
                //設定畫面 is_weaning
                if (item.rt_record.is_weaning != null && item.rt_record.is_weaning == "1")
                {
                    item.rt_record.weaning = new List<JSON_DATA>();
                    item.rt_record.weaning.Add(new JSON_DATA() { id = "weaning", chkd = true, val = "1" });
                }
                else
                {
                    item.rt_record.weaning = new List<JSON_DATA>();
                    item.rt_record.weaning.Add(new JSON_DATA() { id = "weaning", chkd = false, val = "0" });
                }
                if (item.rt_record.is_self_abg != null && item.rt_record.is_self_abg == "1")
                {
                    item.rt_record.self_abg = new List<JSON_DATA>();
                    item.rt_record.self_abg.Add(new JSON_DATA() { id = "self_abg", chkd = true, val = "1" });
                }
                else
                {
                    item.rt_record.self_abg = new List<JSON_DATA>();
                    item.rt_record.self_abg.Add(new JSON_DATA() { id = "self_abg", chkd = false, val = "0" });
                }
                //新增可以修改
                if (!item.isModify)
                    item.hasPowerEdit = true;

                //VIP資料都可以修改
                if (item.isVIP)
                {
                    item.hasPowerEdit = item.isVIP; 
                    if (string.IsNullOrWhiteSpace(item.rt_record.artificial_airway_type))
                    {
                        item.rt_record.artificial_airway_type = lastRecord.artificial_airway_type;
                    }
                    if (string.IsNullOrWhiteSpace(item.rt_record.et_mark))
                    {
                        item.rt_record.et_mark = lastRecord.et_mark;
                    }
                    if (string.IsNullOrWhiteSpace(item.rt_record.et_size))
                    {
                        item.rt_record.et_size = lastRecord.et_size;
                    }
                    if (string.IsNullOrWhiteSpace(item.rt_record.device_o2))
                    {
                        item.rt_record.device_o2 = lastRecord.device_o2;
                    }
                }

                #endregion
            }
        }

        private RCS_ONMODE_MASTER checkOnmodelData(List<RCS_ONMODE_MASTER> onModeList, RCS_ONMODE_MASTER onMode, string pTempDate, string on_type)
        {
            if (string.IsNullOrWhiteSpace(pTempDate))
            {
                pTempDate = DateTime.Now.ToString("yyyy-MM-dd HH:mm");
            }
            if (!string.IsNullOrWhiteSpace(onMode.ONMODE_ID))
            {

                if (onModeList.Exists(x => x.ON_TYPE == on_type && DateTime.Parse(x.STARTDATE) <= DateTime.Parse(pTempDate)
                         && (string.IsNullOrWhiteSpace(x.ENDDATE) || (DateHelper.isDate(x.ENDDATE) && DateTime.Parse(x.ENDDATE) >= DateTime.Parse(pTempDate)))))
                {
                    onMode = onModeList.Find(x => x.ON_TYPE == on_type && DateTime.Parse(x.STARTDATE) <= DateTime.Parse(pTempDate)
    && (string.IsNullOrWhiteSpace(x.ENDDATE) || (DateHelper.isDate(x.ENDDATE) && DateTime.Parse(x.ENDDATE) >= DateTime.Parse(pTempDate))));
                }
                if(on_type == "2")
                {
                    if(onMode.CREATE_ID == user_info.user_id)
                    {
                        onMode.canFixSdate = true;
                    }
                }

            }
            return onMode;
        }

        #endregion

        #region 清單資料

        /// <summary>取得日期區間內呼吸紀錄</summary>
        /// <param name="pIpdNo">批價序號</param>
        /// <param name="pStartDate">開始時間</param>
        /// <param name="pEndDate">結束時間</param>
        /// <returns></returns>

        public List<RT_RECORD_MAIN> getDuringRTRecord(string pStartDate, string pEndDate,string setIpdno, string record_id, bool pGetOnModel = false)
        {
            List<RT_RECORD_MAIN> List = new List<RT_RECORD_MAIN>(); 
            try
            {
                string sql = @"SELECT M.*, 
(SELECT o.ONMODE_ID FROM RCS_ONMODE_MASTER o WHERE M.ONMODE_TYPE1_ID = o.ONMODE_ID AND (M.RECORDDATE BETWEEN o.STARTDATE AND o.ENDDATE OR M.RECORDDATE >= o.STARTDATE AND (o.ENDDATE is NULL OR o.ENDDATE = '')) AND DATASTATUS in('1','3') ) ONMODE_TYPE1_ID, 
(SELECT o.ONMODE_ID FROM RCS_ONMODE_MASTER o WHERE M.ONMODE_TYPE2_ID = o.ONMODE_ID AND (M.RECORDDATE BETWEEN o.STARTDATE AND o.ENDDATE OR M.RECORDDATE >= o.STARTDATE AND (o.ENDDATE is NULL OR o.ENDDATE = '')) AND DATASTATUS in('1','3') ) ONMODE_TYPE2_ID,  
(SELECT o.ONMODE_ID FROM RCS_ONMODE_MASTER o WHERE M.ONMODE_TYPE3_ID = o.ONMODE_ID AND (M.RECORDDATE BETWEEN o.STARTDATE AND o.ENDDATE OR M.RECORDDATE >= o.STARTDATE AND (o.ENDDATE is NULL OR o.ENDDATE = '')) AND DATASTATUS in('1','3') ) ONMODE_TYPE3_ID 

FROM {3} M WHERE DATASTATUS = '1'";

                if (string.IsNullOrWhiteSpace(record_id))
                {
                    sql += " AND CHART_NO = {0} AND RECORDDATE >= {1} AND RECORDDATE <= {2}";
                    //判斷登入者身分權限，如僅可查詢(user_info. authority = “readonly”)
                    if (user_info.authority == "readonly") sql += " AND UPLOAD_STATUS = '1'";
                    if (!string.IsNullOrWhiteSpace(setIpdno)) sql += " AND IPD_NO = " + SQLDefend.SQLString(getHistoryList(setIpdno, 0));
                }
                else
                {
                    sql += " AND RECORD_ID = " + SQLDefend.SQLString(record_id);
                }                               
                
                sql += "  ORDER BY RECORDDATE DESC, CREATE_DATE DESC";
                sql = string.Format(sql, SQLDefend.SQLString(pat_info.chart_no), SQLDefend.SQLString(pStartDate), SQLDefend.SQLString(pEndDate), GetTableName.RCS_RECORD_MASTER.ToString());
                DataTable dt = this.DBA.getSqlDataTable(sql);
                if (this.DBA.LastError != null && this.DBA.LastError != "")
                {
                    dt = new DataTable();
                    LogTool.SaveLogMessage(this.DBA.LastError, "getDuringRTRecord1", GetLogToolCS.RTRecord);
                }
                else
                {
                    if (dt != null && dt.Rows.Count > 0)
                    {
                        foreach (DataRow MstRTDr in dt.Rows)
                        {
                            RT_RECORD_MAIN main = new RT_RECORD_MAIN();
                            bindRT_RECORD_MAIN(ref main, MstRTDr);
                            getRT_RECORD_Detail(ref main, main.RECORD_ID);
                            if (!string.IsNullOrWhiteSpace(main.rt_record.artificial_airway_type))
                                main.onIntubate = new RCS_ONMODE_MASTER() { hasData = true };
                            if (!string.IsNullOrWhiteSpace(main.rt_record.respid))
                                main.onBreath = new RCS_ONMODE_MASTER() { hasData = true };
                            if (!string.IsNullOrWhiteSpace(main.rt_record.device_o2))
                                main.onOxygen = new RCS_ONMODE_MASTER() { hasData = true };
                            List.Add(main);
                        } 
                    }
                }
            }
            catch (Exception ex)
            {
                LogTool.SaveLogMessage(ex, "getDuringRTRecord1", GetLogToolCS.RTRecord);
            }
            return List;
        }

        #endregion

        #region 取得資料 

        #region 呼吸器財編、最後一筆呼吸紀錄、byTurn資料、照護記錄單sql

        /// <summary>取得所有呼吸器財編</summary>
        /// <param name="pROOM">病房</param>
        /// <param name="pUSE_STATUS">使用狀態 Y:使用中, N:停用</param>
        /// <param name="pSTATUS">(明細)狀態 1:使用中,2:維修,3:外借,4:停用</param>
        /// <returns></returns>
        public RESP_COLLECTION getVENTILATORList(string pROOM = "", string pUSE_STATUS = "", string pSTATUS = "")
        {
            string actionName = "getVENTILATORList";
            RESP_COLLECTION RESP_COLLECTION = new  RESP_COLLECTION();

            try
            {
                string sql = "SELECT DISTINCT S.DEVICE_SEQ,S.DEVICE_NO,ROOM,DEVICE_MODEL,USE_STATUS,DEVICE_NUM FROM " + GetTableName.RCS_VENTILATOR_SETTINGS +
                             " S LEFT JOIN " + GetTableName.RCS_VENTILATOR_MAINTAIN +
                             " M ON S.DEVICE_NO = M.DEVICE_NO" +
                             " WHERE 1 = 1";
                if (pROOM != null && pROOM != "") sql += " AND S.ROOM = " + SQLDefend.SQLString(pROOM);
                if (pUSE_STATUS != null && pUSE_STATUS != "") sql += " AND S.USE_STATUS = " + SQLDefend.SQLString(pUSE_STATUS);
                if (pSTATUS != null && pSTATUS != "")
                    sql += " AND M.STATUS = " + SQLDefend.SQLString(pSTATUS);
                DataTable dt = this.DBA.getSqlDataTable(sql);
                if (dt != null && dt.Rows.Count > 0)
                {
                    foreach (DataRow dr in dt.Rows)
                    {
                        DeviceMaster item = new DeviceMaster();
                        if (!DBNull.ReferenceEquals(dr["DEVICE_SEQ"], DBNull.Value)) item.DEVICE_SEQ = dr["DEVICE_SEQ"].ToString();
                        if (!DBNull.ReferenceEquals(dr["DEVICE_NUM"], DBNull.Value))
                            item.DEVICE_SEQ = string.Concat(item.DEVICE_SEQ.Split('-')[0], "-", dr["DEVICE_NUM"].ToString());
                        if (!DBNull.ReferenceEquals(dr["DEVICE_NO"], DBNull.Value)) item.DEVICE_NO = dr["DEVICE_NO"].ToString();
                        if (!DBNull.ReferenceEquals(dr["ROOM"], DBNull.Value)) item.ROOM = dr["ROOM"].ToString();
                        if (!DBNull.ReferenceEquals(dr["DEVICE_MODEL"], DBNull.Value)) item.DEVICE_MODEL = dr["DEVICE_MODEL"].ToString();
                        if (!DBNull.ReferenceEquals(dr["USE_STATUS"], DBNull.Value)) item.USE_STATUS = dr["USE_STATUS"].ToString();
                        RESP_COLLECTION.Add(item);
                    }
                }
                else if (dt == null)
                {
                    LogTool.SaveLogMessage(this.DBA.LastError, actionName, GetLogToolCS.BaseModel);
                }

            }
            catch (Exception ex)
            {
                LogTool.SaveLogMessage(ex, actionName, GetLogToolCS.BaseModel);
            }
            return RESP_COLLECTION;
        }

        /// <summary>取得最後一筆呼吸紀錄</summary>

        /// <param name="pIpdNo">病歷號</param>
        /// <returns></returns>

        public DataTable getFinalRTRecord(string pChart_no)
        {
            string sql = @"SELECT a.* FROM {1} a JOIN {2} b 

                           ON a.RECORD_ID = b.RECORD_ID WHERE b.CHART_NO = {0}
                           AND b.RECORDDATE = (SELECT MAX(RECORDDATE) FROM {2} WHERE IPD_NO = b.IPD_NO)";

            DataTable dt = this.DBA.getSqlDataTable(string.Format(sql, SQLDefend.SQLString(pChart_no), GetTableName.RCS_RECORD_DETAIL.ToString(), GetTableName.RCS_RECORD_MASTER.ToString()));
            return dt;
        }

        /// <summary>取得byTurn資料</summary>

        /// <param name="pIpdNo">病歷號</param>
        /// <returns></returns>

        public DataTable getByTurnData(string pChart_no)
        {
            string sql = @"SELECT a.* FROM {1} a JOIN {2} b ON a.RECORD_ID = b.RECORD_ID

                           WHERE b.CHART_NO = {0} AND a.RECORD_ID IN (SELECT DISTINCT RECORD_ID FROM {1}
                           WHERE ITEM_VALUE = 'by turns' AND ITEM_NAME = 'special_fn_set')  ORDER BY RECORD_ID DESC";

            DataTable dt = this.DBA.getSqlDataTable(string.Format(sql, SQLDefend.SQLString(pChart_no), GetTableName.RCS_RECORD_DETAIL.ToString(), GetTableName.RCS_RECORD_MASTER.ToString()));
            return dt;
        }

        #endregion

        #region Function

        /// <summary>DataTable轉強型別</summary>
        /// <param name="pMain"></param>
        /// <param name="dr"></param>
        private void bindRT_RECORD_MAIN(ref RT_RECORD_MAIN pMain, DataRow dr)
        {
            pMain.RECORD_ID = checkDataColumn(dr, "RECORD_ID");
            pMain.MODIFY_ID = checkDataColumn(dr, "MODIFY_ID");
            pMain.MODIFY_NAME = checkDataColumn(dr, "MODIFY_NAME");
            pMain.MODIFY_DATE = checkDataColumn(dr, "MODIFY_DATE");
            pMain.CREATE_ID = checkDataColumn(dr, "CREATE_ID");
            pMain.CREATE_NAME = checkDataColumn(dr, "CREATE_NAME");
            pMain.CREATE_DATE = checkDataColumn(dr, "CREATE_DATE");
            pMain.RECORDDATE = checkDataColumn(dr, "RECORDDATE");
            pMain.ONMODE_TYPE1_ID = checkDataColumn(dr, "ONMODE_TYPE1_ID");
            pMain.ONMODE_TYPE2_ID = checkDataColumn(dr, "ONMODE_TYPE2_ID");
            pMain.ONMODE_TYPE3_ID = checkDataColumn(dr, "ONMODE_TYPE3_ID");
            pMain.DATASTATUS = checkDataColumn(dr, "DATASTATUS");
            pMain.UPLOAD_STATUS = checkDataColumn(dr, "UPLOAD_STATUS");
            pMain.UPLOAD_ID = checkDataColumn(dr, "UPLOAD_ID");
            pMain.UPLOAD_NAME = checkDataColumn(dr, "UPLOAD_NAME");
            pMain.UPLOAD_DATE = checkDataColumn(dr, "UPLOAD_DATE");
            pMain.NEW_RECORD_ID = checkDataColumn(dr, "NEW_RECORD_ID");
            pMain.COST_CODE = checkDataColumn(dr, "COST_CODE");
            pMain.BED_NO = checkDataColumn(dr, "BED_NO");
            pMain.IPD_NO = checkDataColumn(dr, "IPD_NO");
            pMain.CHART_NO = checkDataColumn(dr, "CHART_NO");
        }

        /// <summary> 取得呼吸照護記錄單詳細資料 </summary>
        /// <param name="main">呼吸照護記錄單資料</param>
        /// <param name="recordID">記錄單PK值</param>
        /// <param name="reLoadRT_RECORD_MAIN">是否要重新取得呼吸照護記錄單主資料表</param>
        public void getRT_RECORD_Detail(ref RT_RECORD_MAIN main, string recordID)
        {
            Dictionary<string, string> Temp = new Dictionary<string, string>();
            if (!string.IsNullOrWhiteSpace(recordID))
            {
                string sql = string.Format("SELECT RECORD_ID, ITEM_NAME, ITEM_VALUE FROM {1} WHERE RECORD_ID = {0}", SQLDefend.SQLString(recordID), GetTableName.RCS_RECORD_DETAIL.ToString());
                DataTable DtlRTDt = this.DBA.getSqlDataTable(sql);
                if (DTNotNullAndEmpty(DtlRTDt))
                {
                    foreach (DataRow DtlRTDr in DtlRTDt.Rows)
                    {
                        double number = 0;
                        string value = checkDataColumn(DtlRTDr, "ITEM_VALUE");
                        try
                        {
                            Temp.Add(DtlRTDr["ITEM_NAME"].ToString().ToLower(), value);
                        }
                        catch (Exception ex)
                        {
                            LogTool.SaveLogMessage(string.Format("record_ID({0}) ITEM_NAME = '{1}'錯誤訊息如下所示:{2}", recordID, DtlRTDr["ITEM_NAME"].ToString().ToLower(), ex.Message), "getRT_RECORD_Detail", GetLogToolCS.RTRecord);
                        }
                    }
                    string temp_json = JsonConvert.SerializeObject(Temp);
                    main.rt_record = JsonConvert.DeserializeObject<RT_RECORD>(temp_json);
                    /*--------------- Cxrc繪圖 2018.09.14 [下] ---------------*/
                    // Views\RT\exportFile\CHGH\_CHGHRTRecordForm.cshtml
                    if (main.rt_record != null 
                        && !string.IsNullOrWhiteSpace(main.rt_record.CXR_result_json)
                    ){
                        // 1.String → JArray
                        Newtonsoft.Json.Linq.JArray CxrResultJson_JArray = (Newtonsoft.Json.Linq.JArray)
                            Newtonsoft.Json.JsonConvert.DeserializeObject(
                                main.rt_record.CXR_result_json
                                );
                        // 2.JArray → CxrResultJson_cls
                        RCS_Data.CxrResultJson_cls CxrResultJson_CList =
                            CxrResultJson_JArray.ToObject
                                <List<RCS_Data.CxrResultJson_cls>>
                                ().ToList().First();
                        // 3.Cxr_CJID → CxrImageBase64_str
                        if (!string.IsNullOrWhiteSpace(CxrResultJson_CList.Cxr_CJID))
                        {
                            SQLProvider _sql = new SQLProvider();
                            Dapper.DynamicParameters dp = new Dapper.DynamicParameters();
                            dp.Add("S_ID", CxrResultJson_CList.Cxr_CJID);
                            List<string> pSinJsonList = _sql.DBA.getSqlDataTable<string>("SELECT S_JSON FROM RCS_SIGNATURE_JSON WHERE S_ID = @S_ID AND S_NAME = 'image64'", dp);
                            if (pSinJsonList != null && pSinJsonList.Count > 0 && !string.IsNullOrWhiteSpace(pSinJsonList[0]))
                            {
                                main.rt_record.CxrImageBase64_str = pSinJsonList[0];
                            }
                            else
                            {
                                main.rt_record.CxrImageBase64_str =
                                   CxrCanvasLineDrawMdl.getCxrImageBase64str_byCxrCjid(
                                   CxrResultJson_CList.Cxr_CJID
                                   );
                            }
                        }
                        
                        // Cxr檢查結果 (Cxr資料庫) 2018.09.27
                        main.rt_record.PDF_CXR_Result_Str = CxrResultJson_CList.Result_Str;
                    }//if
                    /*--------------- Cxrc繪圖 2018.09.14 [上] ---------------*/
                }
                else if(!string.IsNullOrWhiteSpace(this.DBA.LastError))
                {
                    DtlRTDt = new DataTable();
                    LogTool.SaveLogMessage(this.DBA.LastError, "getRTRecordDetailColumns", GetLogToolCS.RTRecord);
                }
            }
            else
                main.RECORDDATE = DateTime.Now.ToString("yyyy-MM-dd HH:mm");
        }

        #endregion

        #endregion

        #region 儲存

        /// <summary>儲存呼吸照護紀錄單</summary>
        /// <param name="row"></param>
        /// <returns></returns>
        public RESPONSE_MSG RTRecordFormSave(RT_RECORD_MAIN row, string setIpdno)
        {
            RESPONSE_MSG rm = new RESPONSE_MSG();
            dbResultMessage dbMsg = null;
            string actionName = "RTRecordFormSave";
            try
            {
                SQLProvider SQL = new SQLProvider();
                if (!row.isVIP)
                {
                    pat_info.source_type = getHistoryList(setIpdno, 3);
                    pat_info.diag_date = getHistoryList(setIpdno, 1);
                    pat_info.ipd_no = getHistoryList(setIpdno, 0);
                    if (string.IsNullOrWhiteSpace(pat_info.source_type) || string.IsNullOrWhiteSpace(pat_info.diag_date) || string.IsNullOrWhiteSpace(pat_info.ipd_no))
                    {
                        rm.status = RESPONSE_STATUS.ERROR;
                        rm.message = "設定病患資料失敗!請洽資訊人員";
                        return rm;
                    }
                }
                DataRow dr = null;
                DataRow drRecordDetail = null;
                string onmode_id = SQL.GetFixedStrSerialNumber();//呼吸器狀態
                string case_doc_id = "";//自訂醫生流水號
                string sql = ""; 
                DataTable dtRecord = new DataTable();//呼吸照護紀錄單主表
                DataTable dtRecordDetail = new DataTable();//呼吸照護紀錄單名細
                DataTable dtABG = new DataTable();//ABG資料使用紀錄
                DataTable dtCASE_DOC = new DataTable();//自訂醫師
                DataTable dtCASE = new DataTable();//病人接收案狀態清單
                DataTable dtCheckBreath = new DataTable();//檢查ON呼吸器資料
                DataTable dtRCS_VENTILATOR_SETTINGS = new DataTable();//更新呼吸器使用中位置

                //Cxr繪圖，暫存 [temp空間] 等待下方 [RCS_RECORD_DETAIL] 完成儲存後，才儲存 [RCS_CXR_JSON]
                CxrResultJson_cls temp_CxrResult_JNode = new CxrResultJson_cls();
                string temp_CxrResultJNode_Str = "";
                CxrResult_SplitedObj tempCxrSplitedObj = new CxrResult_SplitedObj();

                // 更新資料筆數
                int onfRow = 0, effRow = 0, effDtlRow = 0;
                #region 整理資料  
                #endregion
                string recorddate = DateHelper.Parse(string.Format("{0} {1}", row.rt_record.recorddate, row.rt_record.recordtime)).ToString("yyyy-MM-dd HH:mm:ss");
                //檢查記錄單日期
                sql = "SELECT * FROM " + GetTableName.RCS_RECORD_MASTER + " WHERE RECORDDATE = " + SQLDefend.SQLString(recorddate) + " AND IPD_NO = " + SQLDefend.SQLString(pat_info.ipd_no)  + " AND DATASTATUS = '1'";
                if (!string.IsNullOrWhiteSpace(row.RECORD_ID))
                {
                    sql += " AND RECORD_ID <> " + SQLDefend.SQLString(row.RECORD_ID);
                }
                DataTable dtTemp = this.DBA.getSqlDataTable(sql);
                if (DTNotNullAndEmpty(dtTemp))
                {
                    rm.status = RESPONSE_STATUS.ERROR;
                    rm.message = DateHelper.Parse(recorddate).ToString("yyyy-MM-dd HH:mm") + "已存在此記錄單時間，請重新輸入!";
                }
                else if (string.IsNullOrWhiteSpace(this.DBA.LastError))
                {
                    #region 呼吸照護紀錄

                    sql = "SELECT * FROM " + GetTableName.RCS_RECORD_MASTER + " WHERE 1 <> 1 OR RECORD_ID = " + SQLDefend.SQLString(row.RECORD_ID) + " OR NEW_RECORD_ID = " + SQLDefend.SQLString(row.RECORD_ID);
                    dtRecord = this.DBA.getSqlDataTable(sql);
                    #region 呼吸照照記錄主表
                    dr = null;
                    if (dtRecord == null)
                    {
                        LogTool.SaveLogMessage(this.DBA.LastError, "RTRecordFormSave", GetLogToolCS.RTRecord);
                    }
                    else
                    {
                        dtRecord.TableName = GetTableName.RCS_RECORD_MASTER.ToString();
                        if (dtRecord.Rows.Count > 0 && row.UPLOAD_STATUS != "1")
                        {
                            DataRow[] drs = dtRecord.Select("RECORD_ID = " + SQLDefend.SQLString(row.RECORD_ID));
                            if (drs.Any())
                                dr = drs[0];
                            else
                                dr = dtRecord.NewRow();
                        }
                        else
                            dr = dtRecord.NewRow();

                        dr["ONMODE_TYPE1_ID"] = row.onIntubate.ONMODE_ID;
                        dr["ONMODE_TYPE2_ID"] = row.onBreath.ONMODE_ID;
                        dr["ONMODE_TYPE3_ID"] = row.onOxygen.ONMODE_ID;
                        dr["PAT_SOURCE"] = string.IsNullOrEmpty(pat_info.source_type) ? "" : pat_info.source_type;
                        dr["PAT_DATA_DATE"] = string.IsNullOrEmpty(pat_info.diag_date) ? "" : pat_info.diag_date;
                        dr["MODIFY_ID"] = user_info.user_id;
                        dr["MODIFY_NAME"] = user_info.user_name;
                        dr["MODIFY_DATE"] = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                        dr["RECORDDATE"] = recorddate;
                        dr["UPLOAD_STATUS"] = "0";
                        dr["DATASTATUS"] = "1";
                        //修改上傳資料變成歷程資料
                        if (string.IsNullOrWhiteSpace(row.RECORD_ID) || string.IsNullOrEmpty(row.RECORD_ID) || (row.UPLOAD_STATUS != null && row.UPLOAD_STATUS == "1"))
                        {
                            #region 修改上傳資料變成歷程資料
                            if (row.UPLOAD_STATUS != null && row.UPLOAD_STATUS == "1") row.NEW_RECORD_ID = row.RECORD_ID;
                            row.RECORD_ID = SQL.GetFixedStrSerialNumber();
                            if (row.UPLOAD_STATUS != null && row.UPLOAD_STATUS == "1")
                            {

                                if (dtRecord.Rows.Count > 0 && dtRecord.AsEnumerable().ToList().Exists(x => x["RECORD_ID"].ToString() == row.NEW_RECORD_ID))
                                {
                                    dtRecord.AsEnumerable().ToList().Find(x => x["RECORD_ID"].ToString() == row.NEW_RECORD_ID)["DATASTATUS"] = "2";
                                    dtRecord.AsEnumerable().ToList().Find(x => x["RECORD_ID"].ToString() == row.NEW_RECORD_ID)["MODIFY_ID"] = user_info.user_id;
                                    dtRecord.AsEnumerable().ToList().Find(x => x["RECORD_ID"].ToString() == row.NEW_RECORD_ID)["MODIFY_NAME"] = user_info.user_name;
                                    dtRecord.AsEnumerable().ToList().Find(x => x["RECORD_ID"].ToString() == row.NEW_RECORD_ID)["MODIFY_DATE"] = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                                    dtRecord.AsEnumerable().ToList().Find(x => x["RECORD_ID"].ToString() == row.NEW_RECORD_ID)["NEW_RECORD_ID"] = row.RECORD_ID;
                                }
                                if (dtRecord.Rows.Count > 0 && dtRecord.AsEnumerable().ToList().Exists(x => !DBNull.ReferenceEquals(x["NEW_RECORD_ID"], DBNull.Value) &&
                                x["NEW_RECORD_ID"].ToString() == row.NEW_RECORD_ID))
                                {
                                    dtRecord.AsEnumerable().ToList().FindAll(x => !DBNull.ReferenceEquals(x["NEW_RECORD_ID"], DBNull.Value) &&
                                    x["NEW_RECORD_ID"].ToString() == row.NEW_RECORD_ID).ForEach(s =>
                                    {
                                        s["NEW_RECORD_ID"] = row.RECORD_ID;
                                        s["MODIFY_ID"] = user_info.user_id;
                                        s["MODIFY_NAME"] = user_info.user_name;
                                        s["MODIFY_DATE"] = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                                    });
                                }
                            }
                            dr["RECORD_ID"] = row.RECORD_ID;
                            this.updateDataRowUser(ref dr);
                            dr["IPD_NO"] = pat_info.ipd_no;
                            dr["CHART_NO"] = pat_info.chart_no;

                            dr["DEPT_CODE"] = pat_info.dept_code;//記錄科別
                            dr["COST_CODE"] = pat_info.cost_code;//記錄護理站
                            dr["BED_NO"] = pat_info.bed_no;//記錄床號
                            dtRecord.Rows.Add(dr);
                            #endregion
                        }
                        else
                            this.updateDataRowUser(ref dr);
                    }
                    onfRow++;

                    #endregion
 
                    if (!string.IsNullOrWhiteSpace(row.rt_record.rsbi))
                    {
                        row.rt_record.is_weaning = "1";
                    }
                    sql = "SELECT * FROM " + GetTableName.RCS_RECORD_DETAIL + " WHERE 1 <> 1 ";
                    dtRecordDetail = this.DBA.getSqlDataTable(sql);
                    #region 呼吸照路記錄明細
                    if (dtRecordDetail == null)
                    {
                        LogTool.SaveLogMessage(this.DBA.LastError, "RTRecordFormSave", GetLogToolCS.RTRecord);
                    }
                    else
                    {
                        dtRecordDetail.TableName = GetTableName.RCS_RECORD_DETAIL.ToString();
                        //新增RTRECORD_DETAIL
                        PropertyInfo[] props = null;
                        props = row.rt_record.GetType().GetProperties();
                        foreach (var pi in props)
                        {
                            string name = pi.Name;
                            System.Type s = pi.PropertyType;
                            if (s.Name != "String") continue;
                            object value = pi.GetValue(row.rt_record, null);
                            if (name != "record_id" && name != "createuser" && name != "id_source" && name != "recorddatetime" && name != "limitRecordDate")
                            {
                                drRecordDetail = dtRecordDetail.NewRow();
                                drRecordDetail["RECORD_ID"] = row.RECORD_ID;
                                drRecordDetail["ITEM_NAME"] = name;

                                /*----------------------- Cxr繪圖 2018.08.25 [下] -----------------------*/
                                // 新增 [RCS_RECORD_DETAIL] 完成儲存，後才 [新增] 儲存 [RCS_CXR_JSON]
                                // 流水號來源：row.RECORD_ID = SQL.GetFixedStrSerialNumber(); [SQLProvider]
                                // 流水號來源：DateTime.Now.ToString("yyyyMMddHHmmssfffff") + [user_id 使用者代碼] + [pIpdno 批價序號]
                                if (name == "CXR_result_json" && !string.IsNullOrWhiteSpace(value.ToString()))
                                {
                                    CxrResultJson_cls CxrResult_JNode = JsonConvert.DeserializeObject<List<CxrResultJson_cls>>(value.ToString()).ToList().FirstOrDefault();
                                    // 一定要 [3.檢查日期] 有值才會儲存 [RCS_CXR_JSON] Cxr資料庫
                                    if (CxrResult_JNode != null
                                        && CxrResult_JNode.Result_Date != null
                                        && !string.IsNullOrWhiteSpace(CxrResult_JNode.Result_Date)
                                    ){
                                        //分開 "CxrXuwmc_List": [18] → "CxrXuwmc_List": null
                                        tempCxrSplitedObj = CxrCanvasLineDrawMdl.Fn_CxrResultSplit(
                                            GetTableName.RCS_RECORD_DETAIL.ToString() //資料表SQL名稱
                                            , row.RECORD_ID //主表ID流水號
                                            , CxrResult_JNode //CXR畫線資料
                                            );
                                        List <CxrResultJson_cls> CxrResult_JList = new List<CxrResultJson_cls>();
                                        CxrResult_JList.Add(tempCxrSplitedObj.cxrResult_SplitedNode);
                                        drRecordDetail["RECORD_ID"] = row.RECORD_ID;
                                        drRecordDetail["ITEM_NAME"] = "CXR_result_json";
                                        drRecordDetail["ITEM_VALUE"] = JsonConvert.SerializeObject(CxrResult_JList);
                                    }//if (CxrResult_JNode != null)
                                }//if (name == "CXR_result_json")
                                /*----------------------- Cxr繪圖 2018.08.25 [上] -----------------------*/
                                else if (name == "abg_time")
                                {
                                    if (value != null && value.ToString() != "")
                                        drRecordDetail["ITEM_VALUE"] = DateTime.Parse(value.ToString().Trim()).ToString("yyyy-MM-dd HH:mm");
                                }
                                else
                                {
                                    if (value != null)
                                        drRecordDetail["ITEM_VALUE"] = value.ToString().Trim().Replace("'", "’");
                                }
                                dtRecordDetail.Rows.Add(drRecordDetail);
                            }
                        }

                    }

                    #endregion

                    #endregion

                    #region 檢查是否有ABG的資料，新增或修改
                    if (row.rt_record.abg_time.Length > 0)
                    {
                        sql = string.Format("SELECT * FROM " + GetTableName.RCS_DATA_ABG_USAGE + " WHERE ipd_no ='{0}' AND abg_date = '{1}' ", pat_info.ipd_no, row.rt_record.abg_time);
                        dtABG = this.DBA.getSqlDataTable(sql);
                        if (dtABG == null)
                        {
                            LogTool.SaveLogMessage(this.DBA.LastError, "", GetLogToolCS.RTRecord);
                        }
                        else
                        {
                            dtABG.TableName = GetTableName.RCS_DATA_ABG_USAGE.ToString();
                            if (dtABG.Rows.Count == 0)
                            {
                                dr = dtABG.NewRow();
                                dr["ABG_DATE"] = row.rt_record.abg_time;
                                dr["IPD_NO"] = pat_info.ipd_no;
                                dtABG.Rows.Add(dr);
                            }
                            onfRow++;
                        }
                    }

                    #endregion

                    #region 自訂醫生

                    //自訂醫生
                    string custom_vs_id = pat_info.vs_id;
                    if (!(string.IsNullOrEmpty(row.custom_vs_doc) || string.IsNullOrEmpty(row.custom_vs_id)))
                    {
                        case_doc_id = SQL.GetFixedStrSerialNumber();
                        sql = "SELECT * FROM " + GetTableName.RCS_RT_CASE_DOC + " WHERE 1<>1";
                        dtCASE_DOC = this.DBA.getSqlDataTable(sql);
                        if (dtCASE_DOC == null)
                        {
                            LogTool.SaveLogMessage(this.DBA.LastError, "RTRecordFormSave", GetLogToolCS.RTRecord);
                        }
                        else
                        {
                            dtCASE_DOC.TableName = GetTableName.RCS_RT_CASE_DOC.ToString();
                            dr = dtCASE_DOC.NewRow();
                            dr["CDOC_ID"] = case_doc_id;
                            dr["IPD_NO"] = pat_info.ipd_no;
                            dr["VS_DOC_NAME"] = row.custom_vs_doc;
                            dr["VS_DOC_ID"] = row.custom_vs_id;
                            this.updateDataRowUser(ref dr);
                            dtCASE_DOC.Rows.Add(dr);
                        }
                        sql = "SELECT * FROM " + GetTableName.RCS_RT_CASE + " WHERE IPD_NO = " + SQLDefend.SQLString(pat_info.ipd_no);
                        dtCASE = this.DBA.getSqlDataTable(sql);
                        if (dtCASE_DOC == null)
                        {
                            LogTool.SaveLogMessage(this.DBA.LastError, "RTRecordFormSave", GetLogToolCS.RTRecord);
                        }
                        else
                        {
                            dtCASE.TableName = GetTableName.RCS_RT_CASE.ToString();
                            foreach (DataRow pDr in dtCASE.Rows)
                            {
                                pDr["CASE_DOC_ID"] = case_doc_id;
                            }
                        }
                    }

                    #endregion

                    #region 如有選擇呼吸器，更新呼吸器使用中位置
                    if (row != null && row.rt_record != null && !string.IsNullOrWhiteSpace(row.rt_record.respid))
                    {
                        List<DeviceMaster> DeviceList = new List<DeviceMaster>();
                        DeviceList.Add(new DeviceMaster() { ROOM = pat_info.chart_no, DEVICE_NO = row.rt_record.respid });
                        bool isBreathEnd = false;
                        if (row.onBreath != null && !string.IsNullOrWhiteSpace(row.onBreath.ENDDATE))
                        {
                            isBreathEnd = true;
                        }
                        RESPONSE_MSG rRm = this.updateVtSettingRoom(DeviceList, false, isBreathEnd);
                        if (rRm.status == RESPONSE_STATUS.SUCCESS)
                            dtRCS_VENTILATOR_SETTINGS = (DataTable)rRm.attachment;
                        else
                        {
                            rm.status = RESPONSE_STATUS.ERROR;
                            rm.message = rRm.message;
                        }
                    }
                    #endregion
                }
                if (!string.IsNullOrWhiteSpace(this.DBA.LastError))
                {
                    rm.status = RESPONSE_STATUS.ERROR;
                    rm.message = "程式發生錯誤，請洽資訊人員!";
                    LogTool.SaveLogMessage(this.DBA.LastError, actionName,GetLogToolCS.RTRecord);
                }
                #region 回填呼吸治療資料
                //if (row.onBreath != null && !string.IsNullOrWhiteSpace(row.onBreath.STARTDATE) && !string.IsNullOrWhiteSpace(row.onBreath.ONMODE_ID) && !string.IsNullOrWhiteSpace(row.rt_record.mode))
                //{
                //    string checkSql = "SELECT * FROM " + GetTableName.RCS_RECORD_MASTER + " WHERE RECORD_ID in(SELECT RECORD_ID FROM RCS_RECORD_DETAIL WHERE ITEM_NAME = 'mode' AND ITEM_VALUE = '"+row.rt_record.mode +"')" +
                //        " AND RECORDDATE >= " + SQLDefend.SQLString(row.onBreath.STARTDATE);
                //    dtCheckBreath = this.DBA.getSqlDataTable(checkSql);
                //    dtCheckBreath.TableName = GetTableName.RCS_RECORD_MASTER.ToString();
                //    if (DTNotNullAndEmpty(dtCheckBreath))
                //    {
                //        foreach (DataRow drCheckBreath in dtCheckBreath.Rows)
                //        {
                //            drCheckBreath["ONMODE_TYPE2_ID"] = row.onBreath.ONMODE_ID;
                //        }
                //    }
                //    else
                //    {
                //        if (!string.IsNullOrWhiteSpace(this.DBA.LastError))
                //        {
                //            rm.status = RESPONSE_STATUS.ERROR;
                //            rm.message = "更新呼吸器記錄失敗!";
                //        }

                //    }
                //}
                #endregion

                if (rm.status == RESPONSE_STATUS.SUCCESS)
                {
                    this.DBA.BeginTrans();
                    #region 儲存資料

                    DataSet ds = new DataSet(); 
                    ds.Tables.Add(dtRecord);
                    ds.Tables.Add(dtRecordDetail);
                    ds.Tables.Add(dtABG);
                    ds.Tables.Add(dtCASE_DOC);
                    ds.Tables.Add(dtCASE);
                    ds.Tables.Add(dtRCS_VENTILATOR_SETTINGS);
                    foreach (DataTable dt in ds.Tables)
                    {
                        if (dt != null)
                        {
                            if (dt.TableName == GetTableName.RCS_RECORD_DETAIL.ToString() && dt.Rows.Count > 0)
                            {
                                dbMsg = this.DBA.ExecuteNonQuery("DELETE " + GetTableName.RCS_RECORD_DETAIL +
                                        " WHERE RECORD_ID = " + SQLDefend.SQLString(row.RECORD_ID));
                                if (this.DBA.LastError != null && this.DBA.LastError != "")
                                {
                                    rm.status = RESPONSE_STATUS.ERROR;
                                    rm.message = dbMsg.dbErrorMessage;
                                    this.DBA.Rollback();
                                    break;
                                }
                                /*----------------------- Cxr繪圖 2018.08.28 [下] -----------------------*/
                                //需先確認 [RCS_RECORD_DETAIL] 主表儲存沒有錯誤
                                if (dbMsg.State == enmDBResultState.Success
                                && string.IsNullOrWhiteSpace(this.DBA.LastError.ToString())
                                && tempCxrSplitedObj != null
                                && !string.IsNullOrWhiteSpace(tempCxrSplitedObj.Cxr_CJID)
                                //&& tempCxrSplitedObj.cxrXuwmcList != null (勿開啟，Null要清空)
                                //&& tempCxrSplitedObj.cxrXuwmcList.Count > 0 (勿開啟，Null要清空)
                                ){
                                    //更新 Cxr資料庫 [RCS_CXR_JSON] 流水號 [Cxr_CJID] 
                                    dbMsg = CxrCanvasLineDrawMdl.saveRcsCxrJson_byCjid_NullDelete(
                                        tempCxrSplitedObj.Cxr_CJID //1.流水號
                                        , tempCxrSplitedObj.cxrXuwmcList //2.內容值 Null要清空
                                        );
                                    /* 2018.09.03 不用儲存 [CXR下拉清單]
                                    //若 [儲存] Cxr資料庫 [RCS_CXR_JSON] 成功
                                    if (dbMsg.State == enmDBResultState.Success)
                                    {
                                        //儲存 [Cxr檢查結果] CXR下拉清單
                                        dbMsg = CxrCanvasLineDrawMdl.saveCxrResultStrDropdownlist(
                                            tempCxrSplitedObj.cxrResult_SplitedNode.Result_Str //Cxr檢查結果
                                            , GetTableName.RCS_RECORD_DETAIL.ToString() //[RCS_RECORD_DETAIL] [RCS_CPT_ASS_DETAIL]
                                            );
                                    }
                                    */
                                }//[RCS_CXR_JSON]
                                /*----------------------- Cxr繪圖 2018.08.28 [上] -----------------------*/
                            }//[RCS_RECORD_DETAIL]

                            if (dt.Rows.Count > 0) dbMsg = this.DBA.UpdateResult(dt, dt.TableName);

                            if (this.DBA.LastError != null && this.DBA.LastError != "")
                            {
                                rm.status = RESPONSE_STATUS.ERROR;
                                rm.message = dbMsg.dbErrorMessage;
                                this.DBA.Rollback();
                                break;
                            }
                        }
                        else
                        {
                            rm.status = RESPONSE_STATUS.ERROR;
                            rm.message = this.DBA.LastError;
                            this.DBA.Rollback();
                            break;
                        }
                    }

                    #endregion
                    if (this.DBA.LastError != null && this.DBA.LastError != "")
                    {
                        rm.status = RESPONSE_STATUS.ERROR;
                        rm.message = this.DBA.LastError;
                        this.DBA.Rollback();
                    }
                    else
                    {
                        
                        this.DBA.Commit();
                        try
                        {
                            CxrResultJson_cls pListCXR = new CxrResultJson_cls();
                            if (!string.IsNullOrWhiteSpace(row.rt_record.CXR_result_json))
                            {
                                pListCXR = JsonConvert.DeserializeObject<List<CxrResultJson_cls>>(row.rt_record.CXR_result_json).First();
                                if (!string.IsNullOrWhiteSpace(tempCxrSplitedObj.Cxr_CJID) && pListCXR != null)
                                {
                                    SQLProvider _sql = new SQLProvider();
                                    Dapper.DynamicParameters dp = new Dapper.DynamicParameters();
                                    dp.Add("S_ID", tempCxrSplitedObj.Cxr_CJID);
                                    _sql.DBA.DBExecute(String.Concat("DELETE RCS_SIGNATURE_JSON WHERE S_ID = @S_ID"), dp);
                                    dp = new Dapper.DynamicParameters();
                                    dp.Add("S_ID", tempCxrSplitedObj.Cxr_CJID);
                                    dp.Add("S_NAME", "JSON");
                                    dp.Add("S_JSON", JsonConvert.SerializeObject(pListCXR.singJson));
                                    _sql.DBA.DBExecute(String.Concat("INSERT INTO RCS_SIGNATURE_JSON (S_ID,S_NAME, S_JSON) VALUES (@S_ID,@S_NAME, @S_JSON);"), dp);
                                    dp = new Dapper.DynamicParameters();
                                    dp.Add("S_ID", tempCxrSplitedObj.Cxr_CJID);
                                    dp.Add("S_NAME", "image64");
                                    dp.Add("S_JSON", pListCXR.singJsonImageBase64);
                                    _sql.DBA.DBExecute(String.Concat("INSERT INTO RCS_SIGNATURE_JSON (S_ID,S_NAME, S_JSON) VALUES (@S_ID,@S_NAME, @S_JSON);"), dp);
                                }
                            }
                         
                        }
                        catch (Exception ex)
                        {
                            LogTool.SaveLogMessage(ex, actionName, GetLogToolCS.RTRecord);
                        }
                     
                         
                    }
                }
                #endregion
            }
            catch (Exception ex)
            {
                rm.status = RESPONSE_STATUS.EXCEPTION;
                rm.message = ex.Message;
                LogTool.SaveLogMessage(ex,actionName, GetLogToolCS.RTRecord);
            }
            return rm;
        }

         

        #endregion

        #region 列印及產生XML

        /// <summary>呼吸照護記錄清單</summary>
        public List<RT_RECORD_MAIN> rtRecordList { get; set; }

        /// <summary>列印資料</summary>
        public rtRecordPDFModel pdfModel { get; set; }

        /// <summary>產生pdf列印資料</summary>
        /// <param name="recordListData">呼吸照護記錄ListJson</param>
        public void bindpdfModel(string recordListData,string setIpdno)
        {
            rtRecordList = JsonConvert.DeserializeObject<List<RT_RECORD_MAIN>>(recordListData);
            if (rtRecordList != null && rtRecordList.Count > 0)
                rtRecordList = rtRecordList.OrderBy(x=>x.RECORDDATE).ToList();
            MODEL_RCS_WEANING_ASSESS_CHECKLIST rwca = new MODEL_RCS_WEANING_ASSESS_CHECKLIST();
            string pDate = "";
            List<RCS_WEANING_ASSESS_CHECKLIST> pList = new List<RCS_WEANING_ASSESS_CHECKLIST>();
            RCS_WEANING_ASSESS_CHECKLIST _row = null;
            foreach (RT_RECORD_MAIN item in rtRecordList)
            {
                if (pDate != item.rt_record.recorddate)
                {
                    pDate = item.rt_record.recorddate;
                    pList = rwca.getRWCA(pat_info.chart_no, getHistoryList(setIpdno, 0), pDate);
                    if (pList.Count>0)
                    {
                        _row = pList[0];
                    }
                    else
                        _row = null;
                }
                if (_row != null && _row.CREATE_ID == item.CREATE_ID)
                {
                    List<string> _rwcaList = new List<string>();
                    if (!string.IsNullOrWhiteSpace(_row.RWAC01) && _row.RWAC01 == "1") _rwcaList.Add("1");
                    if (!string.IsNullOrWhiteSpace(_row.RWAC02) && _row.RWAC02 == "1") _rwcaList.Add("2");
                    if (!string.IsNullOrWhiteSpace(_row.RWAC03) && _row.RWAC03 == "1") _rwcaList.Add("3");
                    if (!string.IsNullOrWhiteSpace(_row.RWAC04) && _row.RWAC04 == "1") _rwcaList.Add("4");
                    if (!string.IsNullOrWhiteSpace(_row.RWAC05) && _row.RWAC05 == "1") _rwcaList.Add("5");
                    if (!string.IsNullOrWhiteSpace(_row.RWAC06) && _row.RWAC06 == "1") _rwcaList.Add("6");
                    if (!string.IsNullOrWhiteSpace(_row.RWAC07) && _row.RWAC07 == "1") _rwcaList.Add("7");
                    if (!string.IsNullOrWhiteSpace(_row.RWAC08) && _row.RWAC08 == "1") _rwcaList.Add("9");
                    if (!string.IsNullOrWhiteSpace(_row.RWAC09) && _row.RWAC09 == "1") _rwcaList.Add("8");
                    if (_rwcaList.Count() > 0)
                    {
                        item.pRWCA_memo = string.Concat(" *", string.Join(",", _rwcaList));
                        pList = new List<RCS_WEANING_ASSESS_CHECKLIST>();
                        _row = null;
                    }
                }

            }
            this.rtRecordList = setCXRPDFList(this.rtRecordList);
            pdfModel.List = bindPDFData(this.rtRecordList);
            pdfModel.setBaseData(pat_info, getHistoryList(setIpdno,3));
        }

        private List<RT_RECORD_MAIN> setCXRPDFList(List<RT_RECORD_MAIN> prtRecordList)
        {
            List<RT_RECORD_MAIN> pList = new List<RT_RECORD_MAIN>();
            foreach (RT_RECORD_MAIN item in prtRecordList)
            {
                
                pList.Add(item);
                if (!string.IsNullOrWhiteSpace(item.rt_record.CXR_result_json))
                {
                    List<CxrResultJson_cls> pListCXR = JsonConvert.DeserializeObject<List<CxrResultJson_cls>>(item.rt_record.CXR_result_json);
                    RT_RECORD_MAIN _item = new RT_RECORD_MAIN();
                    _item.rt_record = new RT_RECORD();
                    _item.RECORDDATE = string.Format("{0} {1}", item.rt_record.recorddate, item.rt_record.recordtime);
                    _item.CREATE_NAME = item.CREATE_NAME;
                    _item.rt_record.CxrImageBase64_str = item.rt_record.CxrImageBase64_str;
                    item.rt_record.CxrImageBase64_str = "";

                    //6.Cxr檢查結果 (Cxr資料庫)
                    _item.rt_record.PDF_CXR_Result_Str = item.rt_record.PDF_CXR_Result_Str;
                    item.rt_record.PDF_CXR_Result_Str = "";

                    pList.Add(_item);
                }
            }
            return pList;
        }
        /// <summary>分一頁列印方式</summary>
        /// <param name="pDt">清單來源</param>
        public List<List<RT_RECORD_MAIN>> bindPDFData(List<RT_RECORD_MAIN> pDt)
        {
            List<List<RT_RECORD_MAIN>> data = new List<List<RT_RECORD_MAIN>>();
            try
            {
                int cntData = pDt.Count % pageCnt;
                if (cntData != 0)
                {
                    for (int i = 0; i < pageCnt - cntData; i++)
                    {
                        RT_RECORD_MAIN dt = new RT_RECORD_MAIN();
                        pDt.Add(dt);
                    }
                }
                List<RT_RECORD_MAIN> List = new List<RT_RECORD_MAIN>();
                int Cnt = 1;
                for (int i = 0; i < pDt.Count; i++)
                {
                    RT_RECORD_MAIN item = pDt[i];
                    if (Cnt % pageCnt == 0)
                    {
                        List.Add(item);
                        data.Add(List);
                        List = new List<RT_RECORD_MAIN>();
                    }
                    else if (pDt.Count - 1 == i)
                    {
                        List.Add(item);
                        data.Add(List);
                    }
                    else
                    {
                        List.Add(item);
                    }
                    Cnt++;
                }
            }
            catch (Exception ex)
            {

                throw;
            }
            return data;
        }

        /// <summary>
        /// 取得呼吸照護記錄單xml資料
        /// 物件或是rtRecordXml(待確認)
        /// </summary>
        /// <param name="record_id">單號</param>
        /// <returns>xml清單</returns>
        public List<rtRecordXml> getXmlData(Dictionary<string, showUpLoadDetail> record_id)
        {
            string actionName = "getXmlData";
            List<rtRecordXml> xmlList = new List<rtRecordXml>();
            try
            {
                string sql = "SELECT * FROM " + GetTableName.RCS_RECORD_DETAIL + " WHERE RECORD_ID in ('"+string.Join("','",record_id.Keys)+"')";
                DataTable dt = this.DBA.getSqlDataTable(sql);
                if (DTNotNullAndEmpty(dt))
                {
                    foreach (string key in record_id.Keys)
                    {
                        rtRecordXml Xml = new rtRecordXml();
                        Xml.data = new rtRecordXml.Document();
                        Xml.record_id = key;
                        Xml.sourceType = record_id[key].sourceType ?? RCS.Controllers.BaseController.upLoadNullStr;
                        Xml.PatientID = record_id[key].PatientID ?? "";
                        List<DataRow> drList = dt.AsEnumerable().Where(x => x["RECORD_ID"].ToString() == key).ToList();
                        //基本資料
                        Xml.data.patName = record_id[key].patName ?? RCS.Controllers.BaseController.upLoadNullStr;
                        Xml.data.sourceType = record_id[key].show_sourceType ?? RCS.Controllers.BaseController.upLoadNullStr;
                        Xml.data.sex = record_id[key].showSex ?? RCS.Controllers.BaseController.upLoadNullStr;
                        Xml.data.age = record_id[key].age ?? RCS.Controllers.BaseController.upLoadNullStr;
                        Xml.data.chartNo = record_id[key].chartNo ?? RCS.Controllers.BaseController.upLoadNullStr;
                        Xml.data.ipdNo = record_id[key].ipdNo ?? RCS.Controllers.BaseController.upLoadNullStr;
                        Xml.data.bedNo = record_id[key].bedNo ?? RCS.Controllers.BaseController.upLoadNullStr;
                        Xml.data.Signature = record_id[key].Signature ?? RCS.Controllers.BaseController.upLoadNullStr;
                        Xml.data.datastatus = record_id[key].SHOW_DATASTATUS ?? RCS.Controllers.BaseController.upLoadNullStr;
                        Xml.data.diagnosis_code = record_id[key].DIAG_DESC ?? RCS.Controllers.BaseController.upLoadNullStr;
                        #region 組資料
                        Xml.data.recorddate = this.checkXmlCol(ref drList, "recorddate");
                        Xml.data.recordtime = this.checkXmlCol(ref drList, "recordtime");
                        Xml.data.mode = this.checkXmlCol(ref drList, "mode");
                        Xml.data.no = this.checkXmlCol(ref drList, "no");
                        #region vt_set vt 有資料則顯示，否則顯示 hz ampl
                        string vt_set = this.checkXmlCol(ref drList, "vt_set"), vt = this.checkXmlCol(ref drList, "vt");
                        rtRecordPDFModel.setGroupValue(ref vt_set, ref vt, this.checkXmlCol(ref drList, "hz"), this.checkXmlCol(ref drList, "ampl"));
                        Xml.data.vt_set = vt_set;
                        Xml.data.vt = vt;
                        #endregion
                        #region insp_time ie_ratio 有資料則顯示，否則顯示 thigh tlow
                        string insp_time = this.checkXmlCol(ref drList, "insp_time"), ie_ratio = this.checkXmlCol(ref drList, "ie_ratio");
                        rtRecordPDFModel.setGroupValue(ref insp_time, ref ie_ratio, this.checkXmlCol(ref drList, "thigh"), this.checkXmlCol(ref drList, "tlow"));
                        Xml.data.insp_time = insp_time;
                        Xml.data.ie_ratio = ie_ratio;
                        #endregion
                        Xml.data.temp = this.checkXmlCol(ref drList, "temp");
                        Xml.data.vr_set = this.checkXmlCol(ref drList, "vr_set");
                        Xml.data.vr = this.checkXmlCol(ref drList, "vr");
                        Xml.data.flow = this.checkXmlCol(ref drList, "flow");
                        Xml.data.flow_pattern = this.checkXmlCol(ref drList, "flow_pattern");
                        Xml.data.rr = this.checkXmlCol(ref drList, "rr");
                        Xml.data.mv_set = this.checkXmlCol(ref drList, "mv_set");
                        Xml.data.mv = this.checkXmlCol(ref drList, "mv");
                        Xml.data.cuff = this.checkXmlCol(ref drList, "cuff");
                        Xml.data.pressure_peak = this.checkXmlCol(ref drList, "pressure_peak");
                        Xml.data.pressure_plateau = this.checkXmlCol(ref drList, "pressure_plateau");
                        Xml.data.gcse = this.checkXmlCol(ref drList, "gcse");
                        Xml.data.gcsv = this.checkXmlCol(ref drList, "gcsv");
                        Xml.data.gcsm = this.checkXmlCol(ref drList, "gcsm");
                        Xml.data.pressure_mean = this.checkXmlCol(ref drList, "pressure_mean");
                        Xml.data.pressure_peep = this.checkXmlCol(ref drList, "pressure_peep");
                        Xml.data.pr = this.checkXmlCol(ref drList, "pr");
                        Xml.data.pressure_ps = this.checkXmlCol(ref drList, "pressure_ps");
                        Xml.data.pressure_pc = this.checkXmlCol(ref drList, "pressure_pc");
                        Xml.data.bpd = this.checkXmlCol(ref drList, "bpd");
                        Xml.data.bps = this.checkXmlCol(ref drList, "bps");
                        Xml.data.fio2 = this.checkXmlCol(ref drList, "fio2_set");
                        Xml.data.fio2_measured = this.checkXmlCol(ref drList, "fio2_measured");
                        Xml.data.abg_time = this.checkXmlCol(ref drList, "abg_time");
                        Xml.data.low_mv_alarm = this.checkXmlCol(ref drList, "low_mv_alarm");
                        Xml.data.paw_alarm = this.checkXmlCol(ref drList, "paw_alarm");
                        Xml.data.abg_ph = this.checkXmlCol(ref drList, "abg_ph");
                        Xml.data.abg_paco2 = this.checkXmlCol(ref drList, "abg_paco2");
                        Xml.data.spo2 = this.checkXmlCol(ref drList, "spo2");
                        Xml.data.etco2 = this.checkXmlCol(ref drList, "etco2");
                        Xml.data.abg_pao2 = this.checkXmlCol(ref drList, "abg_pao2");
                        Xml.data.abg_sao2 = this.checkXmlCol(ref drList, "abg_sao2");
                        Xml.data.abg_hco3 = this.checkXmlCol(ref drList, "abg_hco3");
                        Xml.data.abg_be = this.checkXmlCol(ref drList, "abg_be");
                        Xml.data.et_size = this.checkXmlCol(ref drList, "et_size");
                        Xml.data.et_mark = this.checkXmlCol(ref drList, "et_mark");
                        Xml.data.abg_paado2 = this.checkXmlCol(ref drList, "abg_paado2");
                        Xml.data.abg_shunt = this.checkXmlCol(ref drList, "abg_shunt");
                        Xml.data.vt_value = this.checkXmlCol(ref drList, "vt_value");
                        Xml.data.mv_value = this.checkXmlCol(ref drList, "mv_value");
                        Xml.data.pi_max = this.checkXmlCol(ref drList, "pi_max");
                        Xml.data.pe_max = this.checkXmlCol(ref drList, "pe_max");
                        Xml.data.breath_sound = this.checkXmlCol(ref drList, "breath_sound");
                        Xml.data.cuff_leak_ml = this.checkXmlCol(ref drList, "cuff_leak_ml");
                        Xml.data.rsbi = this.checkXmlCol(ref drList, "rsbi");
                        Xml.data.memo = this.checkXmlCol(ref drList, "memo");
                        Xml.data.mode_memo = this.checkXmlCol(ref drList, "mode_memo");
                        Xml.data.CreateName = record_id[key].CREATE_NAME ?? RCS.Controllers.BaseController.upLoadNullStr;
                        #endregion
                        xmlList.Add(Xml);
                    }
                }
                else
                {
                    if (this.DBA.LastError != null && this.DBA.LastError != "")
                        LogTool.SaveLogMessage(this.DBA.LastError, actionName, GetLogToolCS.RTRecord);
                }
            }
            catch (Exception ex)
            {
                LogTool.SaveLogMessage(ex, actionName, GetLogToolCS.RTRecord);
            }
            return xmlList;
        }

        /// <summary>
        /// 檢查上傳資料
        /// </summary>
        /// <param name="tempData">被檢查資料</param>
        /// <param name="valueName">檢查欄位</param>
        /// <param name="returnVal">回傳值(預設為" ")</param>
        /// <returns>string(空值回傳"")</returns>
        private string checkXmlCol(ref List<DataRow> tempData, string valueName,string returnVal= " ")
        {
            string tempVal = "";
            if (tempData.Exists(x => !DBNull.ReferenceEquals(x["ITEM_VALUE"], DBNull.Value) && x["ITEM_NAME"].ToString() == valueName))
                tempVal = tempData.Find(x => !DBNull.ReferenceEquals(x["ITEM_VALUE"], DBNull.Value) && x["ITEM_NAME"].ToString() == valueName)["ITEM_VALUE"].ToString();
            if (!string.IsNullOrWhiteSpace(tempVal))
                returnVal = tempVal;
            else
                returnVal = RCS.Controllers.BaseController.upLoadNullStr;
            return returnVal;
        }

        #endregion
         
    }

    /// <summary>呼吸照護記錄單PDFModel
    /// <para>IPDPatientInfo 病患基本資料</para>
    /// </summary>
    public class rtRecordPDFModel : IPDPatientInfo
    {
        /// <summary>
        /// 診斷內容
        /// </summary>
        public string diag_desc { get; set; }
        /// <summary>
        /// 列印清單
        /// </summary>
        public List<List<RT_RECORD_MAIN>> List { get; set; }

        /// <summary>
        /// 加入病患基本資料
        /// </summary>
        /// <param name="patData"></param>
        public void setBaseData(IPDPatientInfo patData,string pSource_type)
        {
            ipd_no = patData.ipd_no;
            chart_no = patData.chart_no;
            birth_day = patData.birth_day;
            patient_name = patData.patient_name;
            gender = patData.gender;
            source_type = pSource_type;
            bed_no = patData.bed_no;
            List<string> cptData = new BaseModel().getCPTAssess(patData.chart_no);
            if (cptData.Count > 0)diagnosis_code = cptData[4];
            cost_code = patData.cost_code;
        }

        /// <summary>
        /// 顯示勾選符號(布林值)
        /// </summary>
        /// <param name="isChecked"></param>
        /// <returns></returns>
        public string showCheckBox(bool isChecked = false)
        {
            string returnVal = "□";
            if (isChecked)
            {
                return "■";
            }

            return returnVal;
        }
        /// <summary>
        /// 顯示勾選符號(指定值)
        /// </summary>
        /// <param name="ischeckVal"></param>
        /// <param name="checkVal"></param>
        /// <returns></returns>
        public string showCheckBox(string ischeckVal, string checkVal)
        {
            string returnVal = "□";
            if (ischeckVal != null && ischeckVal.Trim() == checkVal)
            {
                return "■";
            }
            return returnVal;
        }

        /// <summary>
        /// 如[1]組有資料則顯示[1]組VALUE，否則顯示[2]組VALUE
        /// </summary>
        /// <param name="value1">[1]組value1</param>
        /// <param name="value2">[1]組value2</param>
        /// <param name="value3">[2]組value3</param>
        /// <param name="value4">[2]組value4</param>
        public static void setGroupValue(ref string value1, ref string value2,string value3, string value4)
        {
            if (string.IsNullOrWhiteSpace(value1) && string.IsNullOrWhiteSpace(value2))
            {
                value1 = value3;
                value2 = value4;
            }
        }

    }

    /// <summary>
    /// 上傳呼吸照護記錄單XML格式
    /// </summary>
    public class rtRecordXml
    {
        public rtRecordXml()
        {
            data = new Document();
        }

        public string record_id { get; set; }
        /// <summary>
        /// 病患身分證
        /// </summary>
        public string PatientID { get; set; }
        /// <summary>
        /// 表單記錄日期(recorddate + " " + recordtime)
        /// </summary>
        public string record_date
        {
            get
            {
                return data.recorddate + " " + data.recordtime;
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
        public rtRecordXml.Document data { get; set; }

        public class Document : xmlBaseData
        {
            /// <summary>
            /// 診斷
            /// </summary>
            [XmlElement(ElementName = "diagnosis_code")]
            //[XmlElement(Namespace = "診斷",ElementName = "diagnosis_code")]
            public string diagnosis_code = "";
            /// <summary>
            /// 記錄日期
            /// </summary>
            [XmlElement(ElementName = "recorddate")]
            //[XmlElement(Namespace = "呼吸器械使用 記錄日期",ElementName = "recorddate")]
            public string recorddate ="";
            /// <summary>
            /// 記錄時間
            /// </summary>
            [XmlElement(ElementName = "recordtime")]
            //[XmlElement(Namespace = "呼吸器械使用 記錄時間", ElementName = "recordtime")]
            public string recordtime ="";
            /// <summary>
            /// 呼吸器模式
            /// </summary>
            [XmlElement(ElementName = "mode")]
            //[XmlElement(Namespace = "呼吸器械使用 Ventilation Mode", ElementName = "mode")]
            public string mode ="";
            /// <summary>
            /// Tidal Volume set
            /// </summary>
            [XmlElement(ElementName = "vt_set")]
            //[XmlElement(Namespace = "呼吸器械使用 Tidal Volume Set", ElementName = "vt_set")]
            public string vt_set ="";
            /// <summary>
            /// monitor
            /// </summary>
            [XmlElement(ElementName = "vt")]
            //[XmlElement(Namespace = "呼吸器械使用 measure", ElementName = "vt")]
            public string vt ="";
            /// <summary>
            /// Ventilation rate set
            /// </summary>
            [XmlElement(ElementName = "vr_set")]
            //[XmlElement(Namespace = "呼吸器械使用 Ventilation Rate Set", ElementName = "vr_set")]
            public string vr_set ="";
            /// <summary>
            /// Ventilation rate total
            /// </summary>
            [XmlElement(ElementName = "vr")]
            //[XmlElement(Namespace = "呼吸器械使用 Ventilation Rate Total", ElementName = "vr")]
            public string vr ="";
            /// <summary>
            /// Inspiratory Time
            /// </summary>
            [XmlElement(ElementName = "insp_time")]
            //[XmlElement(Namespace = "呼吸器械使用 Insp T", ElementName = "insp_time")]
            public string insp_time ="";
            /// <summary>
            /// I:E ratio
            /// </summary>
            [XmlElement(ElementName = "ie_ratio")]
            //[XmlElement(Namespace = "呼吸器械使用 I:E", ElementName = "ie_ratio")]
            public string ie_ratio ="";
            /// <summary>
            /// Flow
            /// </summary>
            [XmlElement(ElementName = "flow")]
            //[XmlElement(Namespace = "呼吸器械使用 Flow", ElementName = "flow")]
            public string flow ="";
            /// <summary>
            /// Flow Pattern
            /// </summary>
            [XmlElement(ElementName = "flow_pattern")]
            //[XmlElement(Namespace = "呼吸器械使用 Pattern", ElementName = "flow_pattern")]
            public string flow_pattern ="";
            /// <summary>
            /// Minute Volume set
            /// </summary>
            [XmlElement(ElementName = "mv_set")]
            //[XmlElement(Namespace = "呼吸器械使用 MV Set", ElementName = "mv_set")]
            public string mv_set ="";
            /// <summary>
            /// Minute Volume total
            /// </summary>
            [XmlElement(ElementName = "mv")]
            //[XmlElement(Namespace = "呼吸器械使用 MV Total", ElementName = "mv")]
            public string mv ="";
            /// <summary>
            /// Pressure Peak
            /// </summary>
            [XmlElement(ElementName = "pressure_peak")]
            //[XmlElement(Namespace = "呼吸器械使用 pressure peak", ElementName = "pressure_peak")]
            public string pressure_peak ="";
            /// <summary>
            /// Pressure Plateau
            /// </summary>
            [XmlElement(ElementName = "pressure_plateau")]
            //[XmlElement(Namespace = "呼吸器械使用 pressure plateau", ElementName = "pressure_plateau")]
            public string pressure_plateau ="";
            /// <summary>
            /// Pressure Mean
            /// </summary>
            [XmlElement(ElementName = "pressure_mean")]
            //[XmlElement(Namespace = "呼吸器械使用 pressure mean", ElementName = "pressure_mean")]
            public string pressure_mean ="";
            /// <summary>
            /// Pressure PEEP
            /// </summary>
            [XmlElement(ElementName = "pressure_peep")]
            //[XmlElement(Namespace = "呼吸器械使用 pressure peep", ElementName = "pressure_peep")]
            public string pressure_peep ="";
            /// <summary>
            /// Pressure PS 
            /// </summary>
            [XmlElement(ElementName = "pressure_ps")]
            //[XmlElement(Namespace = "呼吸器械使用 pressure ps", ElementName = "pressure_ps")]
            public string pressure_ps ="";
            /// <summary>
            /// PC (△P)
            /// </summary>
            [XmlElement(ElementName = "pressure_pc")]
            //[XmlElement(Namespace = "呼吸器械使用 pressure pc", ElementName = "pressure_pc")]
            public string pressure_pc ="";
            /// <summary>
            /// Temperature (°C)
            /// </summary>
            [XmlElement(ElementName = "temp")]
            //[XmlElement(Namespace = "呼吸器械使用 Temp(°C)", ElementName = "temp")]
            public string temp ="";
            /// <summary>
            /// FiO2(%) 
            /// </summary>
            [XmlElement(ElementName = "fio2")]
            //[XmlElement(Namespace = " 呼吸器械使用 fio2", ElementName = "fio2_measured")]
            public string fio2 ="";
            /// <summary>
            /// Low M.V alarm
            /// </summary>
            [XmlElement(ElementName = "low_mv_alarm")]
            //[XmlElement(Namespace = "呼吸器械使用 Min,MV alarm", ElementName = "low_mv_alarm")]
            public string low_mv_alarm ="";
            /// <summary>
            /// Paw alarm
            /// </summary>
            [XmlElement(ElementName = "paw_alarm")]
            //[XmlElement(Namespace = "呼吸器械使用 Pr alarm", ElementName = "paw_alarm")]
            public string paw_alarm ="";
            /// <summary>
            /// NO(氮)
            /// </summary>
            [XmlElement(ElementName = "no")]
            //[XmlElement(Namespace = "呼吸器械使用 NO", ElementName = "no")]
            public string no ="";
            /// <summary>
            /// FiO2 Analyzer Measure
            /// </summary>
            [XmlElement(ElementName = "fio2_measured")]
            //[XmlElement(Namespace = "FiO2 Analyzer Measure", ElementName = "fio2_measured")]
            public string fio2_measured ="";
            /// <summary>
            /// ABG Time
            /// </summary>
            [XmlElement(ElementName = "abg_time")]
            //[XmlElement(Namespace = "動脈血氣體分析time ", ElementName = "abg_time")]
            public string abg_time ="";
            /// <summary>
            /// ABG PH
            /// </summary>
            [XmlElement(ElementName = "abg_ph")]
            //[XmlElement(Namespace = "動脈血氣體分析 ph", ElementName = "abg_ph")]
            public string abg_ph ="";
            /// <summary>
            /// ABG PaCO2
            /// </summary>
            [XmlElement(ElementName = "abg_paco2")]
            //[XmlElement(Namespace = "動脈血氣體分析 paco2", ElementName = "abg_paco2")]
            public string abg_paco2 ="";
            /// <summary>
            /// PaO2
            /// </summary>
            [XmlElement(ElementName = "abg_pao2")]
            //[XmlElement(Namespace = "動脈血氣體分析 pao2", ElementName = "abg_pao2")]
            public string abg_pao2 ="";
            /// <summary>
            /// SaO2
            /// </summary>
            [XmlElement(ElementName = "abg_sao2")]
            //[XmlElement(Namespace = "動脈血氣體分析 sao2", ElementName = "abg_sao2")]
            public string abg_sao2 ="";
            /// <summary>
            /// ABG HCO3
            /// </summary>
            [XmlElement(ElementName = "abg_hco3")]
            //[XmlElement(Namespace = "動脈血氣體分析 hco3", ElementName = "abg_hco3")]
            public string abg_hco3 ="";
            /// <summary>
            /// ABG BE
            /// </summary>
            [XmlElement(ElementName = "abg_be")]
            //[XmlElement(Namespace = "動脈血氣體分析 be", ElementName = "abg_be")]
            public string abg_be ="";
            /// <summary>
            /// ABG P(A-a)DO2
            /// </summary>
            [XmlElement(ElementName = "abg_paado2")]
            //[XmlElement(Namespace = "動脈血氣體分析 paado2", ElementName = "abg_paado2")]
            public string abg_paado2 ="";
            /// <summary>
            /// ABG Shunt
            /// </summary>
            [XmlElement(ElementName = "abg_shunt")]
            //[XmlElement(Namespace = "動脈血氣體分析 shunt", ElementName = "abg_shunt")]
            public string abg_shunt ="";
            /// <summary>
            /// SPO2
            /// </summary>
            [XmlElement(ElementName = "spo2")]
            //[XmlElement(Namespace = "Pulse Oximeter ", ElementName = "spo2")]
            public string spo2 ="";
            /// <summary>
            /// End Tidal CO2
            /// </summary>
            [XmlElement(ElementName = "etco2")]
            //[XmlElement(Namespace = "PECO2", ElementName = "etco2")]
            public string etco2 ="";
            /// <summary>
            /// Respiration rate (minute)
            /// </summary>
            [XmlElement(ElementName = "rr")]
            //[XmlElement(Namespace = "呼吸及氣道狀況監視 R.R", ElementName = "rr")]
            public string rr ="";
            /// <summary>
            /// TV
            /// </summary>
            [XmlElement(ElementName = "vt_value")]
            //[XmlElement(Namespace = "呼吸及氣道狀況監視 TV", ElementName = "vt_value")]
            public string vt_value ="";
            /// <summary>
            /// MV
            /// </summary>
            [XmlElement(ElementName = "mv_value")]
            //[XmlElement(Namespace = "呼吸及氣道狀況監視 MV", ElementName = "mv_value")]
            public string mv_value ="";
            /// <summary>
            /// PI max
            /// </summary>
            [XmlElement(ElementName = "pi_max")]
            //[XmlElement(Namespace = "呼吸及氣道狀況監視 pi", ElementName = "pi_max")]
            public string pi_max ="";
            /// <summary>
            /// PE max
            /// </summary>
            [XmlElement(ElementName = "pe_max")]
            //[XmlElement(Namespace = "呼吸及氣道狀況監視 pe", ElementName = "pe_max")]
            public string pe_max ="";
            /// <summary>
            /// RSBI
            /// </summary>
            [XmlElement(ElementName = "rsbi")]
            //[XmlElement(Namespace = "呼吸及氣道狀況監視 RSI", ElementName = "rsbi")]
            public string rsbi ="";
            /// <summary>
            /// Endotracheal tubes size
            /// </summary>
            [XmlElement(ElementName = "et_size")]
            //[XmlElement(Namespace = "呼吸及氣道狀況監視 ET Size", ElementName = "et_size")]
            public string et_size ="";
            /// <summary>
            /// Endotracheal tubes mark
            /// </summary>
            [XmlElement(ElementName = "et_mark")]
            //[XmlElement(Namespace = "呼吸及氣道狀況監視 mark", ElementName = "et_mark")]
            public string et_mark ="";
            /// <summary>
            /// Cuff pressure (cmH2O)
            /// </summary>
            [XmlElement(ElementName = "cuff")]
            //[XmlElement(Namespace = "呼吸及氣道狀況監視 Cuff pressure(cmH2O)", ElementName = "cuff")]
            public string cuff ="";
            /// <summary>
            /// Cuff-leak(ml)
            /// </summary>
            [XmlElement(ElementName = "cuff_leak_ml")]
            //[XmlElement(Namespace = "呼吸及氣道狀況監視 ml", ElementName = "cuff_leak_ml")]
            public string cuff_leak_ml ="";
            /// <summary>
            /// Breath sound
            /// </summary>
            [XmlElement(ElementName = "breath_sound")]
            //[XmlElement(Namespace = "呼吸及氣道狀況監視 Breath Sound", ElementName = "breath_sound")]
            public string breath_sound ="";
            /// <summary>
            /// Pulse rate
            /// </summary>
            [XmlElement(ElementName = "pr")]
            //[XmlElement(Namespace = "血力行學 pr", ElementName = "pr")]
            public string pr ="";
            /// <summary>
            /// BP (mmHg)
            /// </summary>
            [XmlElement(ElementName = "bpd")]
            //[XmlElement(Namespace = "BP(S/D)(mmHg) d", ElementName = "bpd")]
            public string bpd ="";
            /// <summary>
            /// BP (mmHg)
            /// </summary>
            [XmlElement(ElementName = "bps")]
            //[XmlElement(Namespace = "BP(S/D)(mmHg) s", ElementName = "bps")]
            public string bps ="";
            /// <summary>
            /// GCS：E 
            /// </summary>
            [XmlElement(ElementName = "gcse")]
            //[XmlElement(Namespace = "Con's level(E.V.M) E", ElementName = "gcse")]
            public string gcse ="";
            /// <summary>
            /// GCS：V 
            /// </summary> 
            [XmlElement(ElementName = "gcsv")]
            //[XmlElement(Namespace = "Con's level(E.V.M) V", ElementName = "gcsv")]
            public string gcsv ="";
            /// <summary>
            /// GCS：M 
            /// </summary>
            [XmlElement(ElementName = "gcsm")]
            //[XmlElement(Namespace = "Con's level(E.V.M) M", ElementName = "gcsm")]
            public string gcsm ="";
            /// <summary>
            /// 備註
            /// </summary>
            [XmlElement(ElementName = "memo")]
            //[XmlElement(Namespace = "備註", ElementName = "memo")]
            public string memo ="";
            /// <summary>
            /// 病歷室註記
            /// </summary>
            [XmlElement(ElementName = "mode_memo")]
            //[XmlElement(Namespace = "病歷室註記", ElementName = "mode_memo")]
            public string mode_memo ="";
            ///// <summary>
            ///// 血液生化 
            ///// </summary>
            //[XmlElement(ElementName = "Blood_biochemistry_memo")]
            ////[XmlElement(Namespace = "血液生化", ElementName = "Blood_biochemistry_memo")]
            //public string Blood_biochemistry_memo ="";
            /// <summary>
            /// 新增人員 
            /// </summary>
            [XmlElement(ElementName = "CreateName")]
            //[XmlElement(Namespace = "新增人員", ElementName = "CreateName")]
            public string CreateName = "";
        }
    }

}
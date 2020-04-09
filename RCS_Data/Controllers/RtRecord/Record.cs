using Com.Mayaminer;
using mayaminer.com.jxDB;
using mayaminer.com.library;
using Newtonsoft.Json;
using RCS_Data.Models;
using RCS_Data.Models.DB;
using RCS_Data.Models.ViewModels;
using RCSData.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;
using static Dapper.SqlMapper;

namespace RCS_Data.Controllers.RtRecord
{
    public partial class Models : BaseModels, Interface
    {
        public static int pageCnt = 5;
        
        DBA_RCS_RECORD_MASTER _SQL { get; set; }
        DBA_RCS_RECORD_MASTER SQL
        {
            get
            {
                if (this._SQL == null)
                {
                    this._SQL = new DBA_RCS_RECORD_MASTER();
                }
                return this._SQL;
            }
        }

        /// <summary>
        /// 取得新呼吸照護紀錄單
        /// </summary>
        /// <param name="isVIP"></param>
        /// <param name="pat_info"></param>
        /// <param name="user_info"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        public virtual RESPONSE_MSG GetRTRecordNewData(bool isVIP, IPDPatientInfo pat_info, UserInfo user_info, RT_RECORD model)
        {
            RESPONSE_MSG rm = new RESPONSE_MSG();
            RT_RECORD_DATA<RT_RECORD> vm = new RT_RECORD_DATA<RT_RECORD>();
            try
            {
                if (!isVIP)
                {
                    if (model == null)
                    {
                        model = new RT_RECORD();
                    }
                    model.recorddate = Function_Library.getDateNowString(DATE_FORMAT.yyyy_MM_dd);
                    model.recordtime = Function_Library.getDateNowString(DATE_FORMAT.HHmm);
                }
                if (model != null)
                {
                    vm.model = JsonConvert.DeserializeObject<RT_RECORD>(JsonConvert.SerializeObject(model));
                }
                vm.record_id = this.DBLink.InsertMaster(new DBA_RCS_RECORD_MASTER(), pat_info, user_info, string.Concat(model.recorddate, " ", model.recordtime));
                  
            }
            catch (Exception ex)
            {
                rm.status = RESPONSE_STATUS.EXCEPTION;
                rm.message = "程式發生錯誤，請洽資訊人員!" + ex.Message;
                LogTool.SaveLogMessage(string.Format("使用者({0})病歷號({1})，錯誤訊息=({2})", user_info.user_id, pat_info.chart_no, ex.Message), "GetRTRecordNewData");
            }
            rm.attachment = vm;
            return rm;
        } 

        /// <summary>
        /// 帶入指定ID資料
        /// </summary>
        /// <param name="pat_info"></param>
        /// <param name="user_info"></param>
        /// <param name="record_id"></param>
        /// <returns></returns>
        public RESPONSE_MSG getRecordById(IPDPatientInfo pat_info, UserInfo user_info, string record_id)
        {
            RESPONSE_MSG rm = new RESPONSE_MSG();
            try
            {
                
                RT_RECORD_DATA<RT_RECORD> dic = this.RecordByIdWithDic<RT_RECORD>(record_id);
                Ventilator lastRecord = this.basicfunction.GetLastRTRec(pat_info.chart_no, pat_info.ipd_no);
                dic.last_record_id = lastRecord.RECORD_ID;
                rm.attachment = dic;
            }
            catch (Exception ex)
            {
                rm.status = RESPONSE_STATUS.EXCEPTION;
                rm.message = "程式發生錯誤，請洽資訊人員!" + ex.Message;
                LogTool.SaveLogMessage(rm.message, "getRecordById", this.csName);
            }


            return rm;
        }

        /// <summary>
        /// 帶入指定ID資料 for 合併
        /// </summary>
        /// <param name="pat_info"></param>
        /// <param name="user_info"></param>
        /// <param name="record_id"></param>
        /// <returns></returns>
        public RESPONSE_MSG getRecordByIdWithConbine(IPDPatientInfo pat_info, UserInfo user_info, string record_id)
        {
            RESPONSE_MSG rm = new RESPONSE_MSG();
            try
            {
                RT_RECORD_DATA<RT_RECORD_MUST_DATA> dic = this.RecordByIdWithDic<RT_RECORD_MUST_DATA>(record_id); 
                rm.attachment = dic;
            }
            catch (Exception ex)
            {
                rm.status = RESPONSE_STATUS.EXCEPTION;
                rm.message = "程式發生錯誤，請洽資訊人員!" + ex.Message;
                LogTool.SaveLogMessage(rm.message, "getRecordByIdWithConbine", this.csName);
            }
            return rm;
        }


        private RT_RECORD_DATA<T> RecordByIdWithDic<T>(string record_id)
        {
            RT_RECORD_DATA<T> model = new RT_RECORD_DATA<T>(); 
            List<DB_RCS_RECORD_MASTER> mList = new List<DB_RCS_RECORD_MASTER>();
            List<DB_RCS_RECORD_DETAIL> dList = new List<DB_RCS_RECORD_DETAIL>();
            Dapper.DynamicParameters dp = new Dapper.DynamicParameters();
            string query = this.SQL.SQL_MAIN;
            dp.Add("RECORD_ID", record_id);
            this.DBLink.DBA.Open();
            GridReader gr = this.DBLink.DBA.dbConnection.QueryMultiple(query, dp);
            mList = gr.Read<DB_RCS_RECORD_MASTER>().ToList();
            dList = gr.Read<DB_RCS_RECORD_DETAIL>().ToList();
            this.DBLink.DBA.Close();
            if (mList.Count > 0)
            { 
                model = JsonConvert.DeserializeObject<RT_RECORD_DATA<T>>(JsonConvert.SerializeObject(mList[0]));
            } 
            if (dList.Count > 0)
            { 
                Dictionary <string, string> dic = new Dictionary<string, string>();
                foreach (var item in dList)
                {
                    dic.Add(item.ITEM_NAME, item.ITEM_VALUE);
                } 
                model.model = JsonConvert.DeserializeObject<T>(JsonConvert.SerializeObject(dic));
            }
            return model;
        }


        public RESPONSE_MSG RTRecordFormSave(IPDPatientInfo pat_info, UserInfo user_info, string record_id, bool isVIP, RT_RECORD model, List<string> Artificial_airway_typeList = null)
        {
            RESPONSE_MSG rm = new RESPONSE_MSG();
            List<DB_RCS_RECORD_DETAIL> dList = new List<DB_RCS_RECORD_DETAIL>(); 
            List<DB_RCS_VENTILATOR_SETTINGS> vList = new List<DB_RCS_VENTILATOR_SETTINGS>();
            RT_RECORD_DATA<RT_RECORD> dic = this.RecordByIdWithDic<RT_RECORD>(record_id);
            Dapper.DynamicParameters dp = new Dapper.DynamicParameters();
            dp.Add("RECORD_ID", record_id);
            bool isModify = false; 
            string recorddate = DateHelper.Parse(string.Format("{0} {1}", model.recorddate, model.recordtime)).ToString("yyyy-MM-dd HH:mm:ss");

            if (dic.DATASTATUS == "1")
            {
                isModify = true;
            }
            //如果不是VIP資料，要檢查
            this.saveRecordDateCheck(ref rm, recorddate, record_id, pat_info, user_info, model);
            if (rm.hasLastError)
            {
                rm.status = RESPONSE_STATUS.ERROR;
                rm.message = rm.lastError;
                return rm;
            }
            dic.MODIFY_DATE = Function_Library.getDateNowString(DATE_FORMAT.yyyy_MM_dd_HHmmss);
            dic.MODIFY_ID = user_info.user_id;
            dic.MODIFY_NAME = user_info.user_name;
            dic.DATASTATUS = "1";
            dic.RECORDDATE = recorddate;
            dic.UPLOAD_STATUS = "0";
            // rsbi 有資料自動轉　is_weaning
            if (!string.IsNullOrWhiteSpace(model.rsbi))
            {
                // TODO:有資料自動轉　is_weaning
                // model.is_weaning = "1";
            }
            #region 如有選擇呼吸器，更新呼吸器使用中位置
            if (model != null && model != null && !string.IsNullOrWhiteSpace(model.respid))
            {
                List<DeviceMaster> DeviceList = new List<DeviceMaster>();
                DeviceList.Add(new DeviceMaster() { ROOM = pat_info.chart_no, DEVICE_NO = model.respid });
                bool isBreathEnd = false;
                // TODO:判斷呼吸器是否結束
                this.updateVtSettingRoom(ref rm, DeviceList, false, isBreathEnd);
                if (!rm.hasLastError)
                { 
                    vList = (List<DB_RCS_VENTILATOR_SETTINGS>)rm.attachment;
                    rm.attachment = null;
                }else
                {
                    return rm;
                }
            }
            #endregion

            List<string> on_dateLIst = new List<string>();
            int cntday = this.basicfunction.getUseDays(pat_info.ipd_no, pat_info.chart_no, model.recorddate, Artificial_airway_typeList, out on_dateLIst);
            // 計算天數如果等於0 且有侵襲性呼吸器使用 自動+1天
            if (cntday == 0 && !string.IsNullOrWhiteSpace(model.respid) && !string.IsNullOrWhiteSpace(model.artificial_airway_type))
            {
                if (Artificial_airway_typeList == null || Artificial_airway_typeList.Count == 0 || Artificial_airway_typeList.Contains(model.artificial_airway_type))
                {
                    cntday++;
                    model.use_days_how = cntday.ToString();
                }
            }
            #region DB_RCS_RECORD_DETAIL
            //新增RTRECORD_DETAIL
            PropertyInfo[] props = null;
            props = model.GetType().GetProperties();
            foreach (var pi in props)
            {
                string name = pi.Name;
                Type s = pi.PropertyType;
                if (s.Name != "String") continue;
                object value = pi.GetValue(model, null);
                dList.Add(new DB_RCS_RECORD_DETAIL()
                {
                    RECORD_ID = record_id,
                    ITEM_NAME = name,
                    ITEM_VALUE = value==null? "" : value.ToString()
                });
            }
            #endregion
               
            if (isModify)
            {
                this.DBLink.EditTableData(record_id, DB_TABLE_NAME.DB_RCS_RECORD_MASTER, user_info, this.SQL.SQL_MAIN, dp); 
            } 
            this.DBLink.DBA.BeginTrans();
            this.DBLink.DBA.DBExecUpdate<DB_RCS_RECORD_MASTER>(new List<DB_RCS_RECORD_MASTER>() { JsonConvert.DeserializeObject<DB_RCS_RECORD_MASTER>(JsonConvert.SerializeObject(dic)) });
            this.DBLink.DBA.DBExecDelete<DB_RCS_RECORD_DETAIL_BY_RECORD_ID>(new DB_RCS_RECORD_DETAIL_BY_RECORD_ID() { RECORD_ID = record_id });
            this.DBLink.DBA.DBExecInsert<DB_RCS_RECORD_DETAIL>(dList);
            if (this.DBLink.DBA.hasLastError)
            {
                rm.status = RESPONSE_STATUS.ERROR;
                rm.messageList.Add("儲存失敗! " + this.DBLink.DBA.lastError);
                this.DBLink.DBA.Rollback();
            }
            else
            {
                rm.attachment = "儲存成功！";
                this.DBLink.DBA.Commit();
            }

            return rm;
        }

        public RESPONSE_MSG RTRecordFormDel(IPDPatientInfo pat_info, UserInfo user_info, string record_id)
        {
            string actionName = "RTRecordFormDel";
            RESPONSE_MSG rm = new RESPONSE_MSG();
            try
            {
                Dapper.DynamicParameters dp = new Dapper.DynamicParameters();
                string query = "SELECT * FROM " + DB_TABLE_NAME.DB_RCS_RECORD_MASTER + " WHERE RECORD_ID =@RECORD_ID";
                dp.Add("RECORD_ID", record_id);
                List<DB_RCS_RECORD_MASTER> pList = new List<DB_RCS_RECORD_MASTER>();
                pList = this.DBLink.DBA.getSqlDataTable<DB_RCS_RECORD_MASTER>(query, dp);
                if (pList.Count > 0)
                {
                    pList.ForEach(x=> {
                        x.DATASTATUS = "9";
                        x.MODIFY_DATE = Function_Library.getDateNowString(DATE_FORMAT.yyyy_MM_dd_HHmmss);
                        x.MODIFY_ID = user_info.user_id;
                        x.MODIFY_NAME = user_info.user_name;
                    });
                    this.DBLink.DBA.DBExecUpdate<DB_RCS_RECORD_MASTER>(pList);
                }
              
                //刪除成功
                if (this.DBLink.DBA.hasLastError )
                {
                    rm.message = "刪除失敗!" + this.DBLink.DBA.lastError;
                    rm.status = RESPONSE_STATUS.ERROR; 
                }
                else
                { 
                    rm.status = RESPONSE_STATUS.SUCCESS;
                    rm.attachment = "刪除成功!";
                } 
                 
            }
            catch (Exception ex)
            {
                rm.status = RESPONSE_STATUS.EXCEPTION;
                rm.messageList.Add("存檔檢查發生錯誤，請洽資訊人員");
                LogTool.SaveLogMessage(ex, actionName, this.csName);
            }
            return rm;
        }

        /// <summary>
        /// 檢查紀錄日期
        /// <para>門診病人紀錄單時，只可填寫當天紀錄日期</para>
        /// <para>急診病人紀錄單時，只可填寫急診區間的紀錄日期</para>
        /// </summary>
        /// <param name="setIpdno"></param>
        /// <param name="pRecordDate"></param>
        /// <param name="pPatInfo"></param>
        /// <returns></returns>
        private void saveRecordDateCheck(ref RESPONSE_MSG rm, string pRecordDate, string record_id, IPDPatientInfo pPatInfo, UserInfo user_info, RT_RECORD model)
        {
            List<string> msgList = new List<string>(); 
            rm.status = RESPONSE_STATUS.SUCCESS;
            string actionName = "saveRecordDateCheck";
            try
            {
                //記錄單日期不可以大於今天
                if (DateHelper.isDate(pRecordDate) && DateHelper.Parse(pRecordDate) > DateTime.Parse(DateTime.Now.ToString("yyyy-MM-dd 23:59:59")))
                {
                    rm.messageList.Add("記錄單日期(" + pRecordDate + ")不可大於今天(" + DateTime.Now.ToString("yyyy-MM-dd 23:59:59") + ")");
                }

                //檢查記錄單日期 已存在此記錄單時間
                string sql = "SELECT * FROM " + DB_TABLE_NAME.DB_RCS_RECORD_MASTER + " WHERE RECORDDATE = " + SQLDefend.SQLString(pRecordDate) + " AND IPD_NO = " + SQLDefend.SQLString(pPatInfo.ipd_no) + " AND DATASTATUS = '1'";
                if (!string.IsNullOrWhiteSpace(record_id))
                {
                    sql += " AND RECORD_ID <> " + SQLDefend.SQLString(record_id);
                }
                DataTable dtTemp = this.DBLink.DBA.getSqlDataTable(sql);
                if (DTNotNullAndEmpty(dtTemp))
                { 
                    rm.messageList.Add(DateHelper.Parse(pRecordDate).ToString("yyyy-MM-dd HH:mm") + "已存在此記錄單時間，請重新輸入!"); 
                } 
            }
            catch (Exception ex)
            {
                rm.status = RESPONSE_STATUS.EXCEPTION;
                rm.messageList.Add("存檔檢查發生錯誤，請洽資訊人員");
                LogTool.SaveLogMessage(ex, actionName, this.csName);
            }　
        }

        /// <summary>
        /// 更新指定呼吸器現在的位置
        /// </summary>
        /// <param name="respidList">設定Room清單</param>
        /// <param name="upDateNow">是否馬上更新</param>
        /// <param name="rDt">是否將DataTable加入附加資訊</param>
        /// <returns></returns>
        private void updateVtSettingRoom(ref RESPONSE_MSG rm, List<DeviceMaster> respidList, bool upDateNow, bool isEnd, bool rDt = true)
        { 
            string actionName = "updateVtSettingRoom"; 
            List<DB_RCS_VENTILATOR_SETTINGS> pList = new List<DB_RCS_VENTILATOR_SETTINGS>();
            try
            {
                string sql = "SELECT * FROM " + DB_TABLE_NAME.DB_RCS_VENTILATOR_SETTINGS + " WHERE DEVICE_NO in ('" + string.Join("','", respidList.Select(x => x.DEVICE_NO).ToList()) + "')";
                pList = this.DBLink.DBA.getSqlDataTable<DB_RCS_VENTILATOR_SETTINGS>(sql);
                if (pList.Count > 0)
                { 
                    foreach (DeviceMaster item in respidList)
                    {
                        List<DataRow> drList = new List<DataRow>();
                        if (pList.Exists(x=>x.DEVICE_NO == item.DEVICE_NO))
                        {
                            if (isEnd)
                            {
                                pList.Find(x => x.DEVICE_NO == item.DEVICE_NO).ROOM = "";
                            }
                            else
                            {
                                pList.Find(x => x.DEVICE_NO == item.DEVICE_NO).ROOM = item.ROOM; 
                            }
                        } 
                    } 
                    if (upDateNow)
                    {
                        this.DBLink.DBA.DBExecUpdate<DB_RCS_VENTILATOR_SETTINGS>(pList);
                        if (this.DBLink.DBA.hasLastError)
                        {
                            rm.status = RESPONSE_STATUS.ERROR;
                            rm.messageList.Add("更新資料發生錯誤" + this.DBLink.DBA.lastError);
                            LogTool.SaveLogMessage(rm.message, actionName, GetLogToolCS.BaseModel);
                        }　
                    }
                }
                else
                {
                    if (!string.IsNullOrWhiteSpace(this.DBLink.DBA.lastError))
                    {
                        rm.status = RESPONSE_STATUS.ERROR;
                        rm.messageList.Add("取得資料發生錯誤，錯誤訊息如下所示" + this.DBLink.DBA.lastError);
                        LogTool.SaveLogMessage(rm.message, actionName, GetLogToolCS.BaseModel);
                    } 
                }
            }
            catch (Exception ex)
            {
                rm.status = RESPONSE_STATUS.EXCEPTION;
                rm.messageList.Add("程式發生錯誤，錯誤訊息如下所示:" + ex.Message);
                LogTool.SaveLogMessage(ex, actionName, GetLogToolCS.BaseModel); 
            }
            if (rDt) rm.attachment = pList; 
        }

        /// <summary>取得日期區間內呼吸紀錄</summary>
        /// <param name="pIpdNo">批價序號</param>
        /// <param name="pStartDate">開始時間</param>
        /// <param name="pEndDate">結束時間</param>
        /// <returns></returns>

        public List<RT_RECORD_MAIN> getDuringRTRecord(string pStartDate, string pEndDate, IPDPatientInfo pat_info, string record_id , bool pGetOnModel = false)
        {
            List<RT_RECORD_MAIN> List = new List<RT_RECORD_MAIN>();
            try
            {
                string sql = @"SELECT M.*, 
(SELECT o.ONMODE_ID FROM RCS_ONMODE_MASTER o WHERE M.ONMODE_TYPE1_ID = o.ONMODE_ID AND (M.RECORDDATE BETWEEN o.STARTDATE AND o.ENDDATE OR M.RECORDDATE >= o.STARTDATE AND (o.ENDDATE is NULL OR o.ENDDATE = '')) AND DATASTATUS in('1','3') ) ONMODE_TYPE1_ID, 
(SELECT o.ONMODE_ID FROM RCS_ONMODE_MASTER o WHERE M.ONMODE_TYPE2_ID = o.ONMODE_ID AND (M.RECORDDATE BETWEEN o.STARTDATE AND o.ENDDATE OR M.RECORDDATE >= o.STARTDATE AND (o.ENDDATE is NULL OR o.ENDDATE = '')) AND DATASTATUS in('1','3') ) ONMODE_TYPE2_ID,  
(SELECT o.ONMODE_ID FROM RCS_ONMODE_MASTER o WHERE M.ONMODE_TYPE3_ID = o.ONMODE_ID AND (M.RECORDDATE BETWEEN o.STARTDATE AND o.ENDDATE OR M.RECORDDATE >= o.STARTDATE AND (o.ENDDATE is NULL OR o.ENDDATE = '')) AND DATASTATUS in('1','3') ) ONMODE_TYPE3_ID 

FROM {3} M WHERE DATASTATUS = '1'";

                if (!string.IsNullOrWhiteSpace(record_id))
                {
                    sql += " AND RECORD_ID = " + SQLDefend.SQLString(record_id);
                    
                }
                if (!string.IsNullOrWhiteSpace(pStartDate) && !string.IsNullOrWhiteSpace(pEndDate))
                {
                    sql += " AND CHART_NO = {0} AND RECORDDATE >= {1} AND RECORDDATE <= {2}";
                }
                if (!string.IsNullOrWhiteSpace(pat_info.ipd_no))
                {
                    sql += " AND IPD_NO = " + SQLDefend.SQLString(pat_info.ipd_no);
                }
                if (!string.IsNullOrWhiteSpace(pat_info.chart_no))
                {
                    sql += " AND CHART_NO = " + SQLDefend.SQLString(pat_info.chart_no);
                }
                sql += "  ORDER BY RECORDDATE DESC, CREATE_DATE DESC";
                sql = string.Format(sql, SQLDefend.SQLString(pat_info.chart_no), SQLDefend.SQLString(pStartDate), SQLDefend.SQLString(pEndDate), DB_TABLE_NAME.DB_RCS_RECORD_MASTER);
                DataTable dt = this.DBLink.DBA.getSqlDataTable(sql);
                if (this.DBLink.DBA.lastError != null && this.DBLink.DBA.lastError != "")
                {
                    dt = new DataTable();
                    LogTool.SaveLogMessage(this.DBLink.DBA.lastError, "getDuringRTRecord1", GetLogToolCS.RTRecord);
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

        /// <summary>
        /// 新增時帶入欄位
        /// </summary>
        /// <param name="pVm"></param>
        /// <param name="pLastRecord"></param>
        /// <param name="pIsVIP"></param>
        public virtual void addNew(ref RT_RECORD_DATA<RT_RECORD> pVm, Ventilator pLastRecord, bool pIsVIP)
        {

        }

        /// <summary>
        /// 顯示狀況
        /// </summary>
        /// <param name="pList"></param>
        public virtual void ShowStatus(ref List<RT_RECORD_MAIN> pList)
        {

        }

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
                string sql = string.Format("SELECT RECORD_ID, ITEM_NAME, ITEM_VALUE FROM {1} WHERE RECORD_ID = {0}", SQLDefend.SQLString(recordID), DB_TABLE_NAME.DB_RCS_RECORD_DETAIL);
                DataTable DtlRTDt = this.DBLink.DBA.getSqlDataTable(sql);
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
                    
                }
                else if (!string.IsNullOrWhiteSpace(this.DBLink.DBA.lastError))
                {
                    DtlRTDt = new DataTable();
                    LogTool.SaveLogMessage(this.DBLink.DBA.lastError, "getRTRecordDetailColumns", GetLogToolCS.RTRecord);
                }
            }
            else
                main.RECORDDATE = DateTime.Now.ToString("yyyy-MM-dd HH:mm");
        }

        #endregion
    }


    public class RT_RECORD_DATA<T> : DB_RCS_RECORD_MASTER
    { 
        public string record_id { get; set; }

        public string last_record_id { get; set; }

        public T model { get; set; } 
    } 

    /// <summary> 呼吸紀錄單 </summary>
    public class RT_RECORD : IRT_RECORD_GET_LAST, IABG_LAB_DATA, IBIOCHEM_LAB_DATA
    {
        #region GET_LAST

        public string respid { get; set; }
        public string device { get; set; }
        public string mode { get; set; }
        public string device_o2 { get; set; }
        public string vt_set { get; set; }
        public string vr_set { get; set; }
        public string vr { get; set; }
        public string flow { get; set; }
        public string mv_set { get; set; }
        /// <summary>Flow / Flow Pattern  Flow Pattern</summary>
        public string flow_pattern { get; set; }

        public string insp_time { get; set; }
        public string ie_ratio { get; set; }
        public string thigh { get; set; }
        public string tlow { get; set; }
        public string ipap { set; get; }
        public string epap { set; get; }
        public string phigh { get; set; }
        public string plow { get; set; }
        public string spo2 { get; set; }
        /// <summary>NO</summary>
        public string no { get; set; }
        public string artificial_airway_type { get; set; }

        public string et_size { get; set; }
        public string et_mark { get; set; }
        public string cuff { get; set; }
        public string breath_sound { get; set; }
        /// <summary>GCS：E </summary>
        public string gcse { get; set; }
        /// <summary>GCS：V </summary>
        public string gcsv { get; set; }
        /// <summary>GCS：M </summary>
        public string gcsm { get; set; }

        /// <summary>意識</summary>
        public string conscious { get; set; }


        #endregion



        #region DATA
        public string recorddate { get; set; }
        public string recordtime { get; set; }
        /// <summary>病患id輸入來源：1：讀卡機、2：barcode、3：數字鍵</summary>
        public string id_source { get; set; }
        /// <summary>儲存模式：0:定時記錄、1:手動記錄、2:結束紀錄、3:事件記錄:警報、4:事件記錄:設定變動記錄、5:事件記錄:指定資料變動、6:未分類</summary>
        public string save_type { get; set; }
        public string patient_id { get; set; }

        public string special_fn_set { get; set; }

        public string vt { get; set; }
        public string e_sense { get; set; }
        public string rise_time { get; set; }


        public string temp { get; set; }
        public string humi_temp { get; set; }

        /// <summary>
        /// Gallor  MV%  ASP mode 會吐出的資料
        /// </summary>
        public string mv_percent { get; set; }
        public string mv { get; set; }
        public string rr { get; set; }
        public string pressure_peak { get; set; }
        public string pressure_plateau { get; set; }

        public string pressure_mean { get; set; }
        public string pressure_peep { get; set; }
        public string pr { get; set; }
        public string pressure_ps { get; set; }
        public string pressure_pc { get; set; }
        public string bpd { get; set; }
        public string bps { get; set; }
        public string fio2_set { get; set; }
        public string fio2_measured { get; set; }

        public string low_mv_alarm { get; set; }
        public string paw_alarm { get; set; }

        public string Try_off_MV_1{get;set;}
        public string Try_off_MV_2 { get; set; }
        public string Try_off_MV_3 { get; set; }

        public string Try_off_NIV_1 { get; set; }
        public string Try_off_NIV_2 { get; set; }

        public string Pres_Limit_L { get; set; }

        public string pressure_sensitivity { get; set; }
        public string sensitivity_flow { get; set; }

        public string etco2 { get; set; }
        public string tcpco2 { get; set; }
        public string pulse { get; set; }

        public string pi_max { get; set; }
        public string pe_max { get; set; }
        public string rsbi { get; set; }
        public string rsi { get; set; }
        public string cuff_leak_ml { get; set; }
        public string cuff_leak_percent { get; set; }
        public string memo { get; set; }
        public string drug_memo { get; set; }
        //20181105 新增欄位
        private List<JSON_DATA> _humidifier { get; set; }
        public List<JSON_DATA> humidifier
        {
            get
            {
                return _humidifier;
            }
            set
            {
                _humidifier = value;
                if (value != null && value.Exists(x => x.chkd == true && x.id == "humidifier"))
                {
                    value.Find(x => x.chkd == true && x.id == "humidifier").val = "1";
                    is_humidifier = "1";
                }
                else if (value != null && value.Exists(x => x.chkd == false && x.id == "humidifier"))
                {
                    value.Find(x => x.chkd == false && x.id == "humidifier").val = "0";
                    is_humidifier = "0";
                }
            }
        }
        /// <summary>是否是\H </summary>
        public string is_humidifier { get; set; }
        public string _is_humidifier { get; set; }

        private List<JSON_DATA> _vbg { get; set; }
        public List<JSON_DATA> vbg
        {
            get
            {
                return _vbg;
            }
            set
            {
                _vbg = value;
                if (value != null && value.Exists(x => x.chkd == true && x.id == "vbg"))
                {
                    value.Find(x => x.chkd == true && x.id == "vbg").val = "1";
                    is_vbg = "1";
                }
                else if (value != null && value.Exists(x => x.chkd == false && x.id == "vbg"))
                {
                    value.Find(x => x.chkd == false && x.id == "vbg").val = "0";
                    is_vbg = "0";
                }
            }
        }
        private string _is_vbg { get; set; }
        /// <summary>是否是VBG </summary>
        public string is_vbg { get; set; }

        /// <summary>Hz / Ampl Hz </summary>
        public string hz { get; set; }
        /// <summary>Hz / Ampl Ampl</summary>
        public string ampl { get; set; }
        /// <summary>VT / MV VT</summary>
        public string vt_value { get; set; }
        /// <summary>VT / MV MV</summary>
        public string mv_value { get; set; }
        /// <summary>自訂參數名稱</summary>
        public string additional { get; set; }
        /// <summary>自訂參數值</summary>
        public string additional_value { get; set; }
        /// <summary>
        /// 病例室註記
        /// </summary>
        public string mode_memo { get; set; }
        public string Blood_biochemistry_memo { get; set; }
        /// <summary>
        /// 嘗試脫離狀態(1為嘗試脫離狀態資料)
        /// </summary>
        public string is_weaning { get; set; }

        private List<JSON_DATA> _weaning { get; set; }
        public List<JSON_DATA> weaning
        {
            get
            {
                return _weaning;
            }
            set
            {
                _weaning = value;
                if (value != null && value.Exists(x => x.chkd == true && x.id == "weaning"))
                {
                    value.Find(x => x.chkd == true && x.id == "weaning").val = "1";
                    is_weaning = "1";
                }
                else if (value != null && value.Exists(x => x.chkd == false && x.id == "weaning"))
                {
                    value.Find(x => x.chkd == false && x.id == "weaning").val = "0";
                    is_weaning = "0";
                }
            }
        }

        //20180126 新增欄位

        public string o2flow { get; set; }

        public string simv_rate { get; set; }

        public string ti { get; set; }

        public string pause_time { get; set; }

        public string ramp { get; set; }
        /// <summary>
        /// 腦壓
        /// </summary>
        public string ICP { get; set; }

        public string srr { get; set; }


        public string rsi_srr { get; set; }

        public string exp_tv { get; set; }

        /// <summary>
        /// Complaince 錯誤拼法 註記 
        /// 正確拼法 Compliance
        /// </summary>
        public string Compliance { get; set; }

        public string resistance { get; set; }

        public string pa_pres { get; set; } // 肺動脈壓（pulmonary artery pressures, PAP）
        public string pa_pres_low { get; set; } // 肺動脈壓（pulmonary artery pressures, PAP）

        public string CXR_result_json { get; set; } //Cxr繪圖物件 2018.08.25 (Save儲存功能OK，Get讀取有錯誤) CxrResultJson_cls
        public string CxrImageBase64_str { get; set; }
        public string PDF_CXR_Result_Str { get; set; } // 6.Cxr檢查結果 (Cxr資料庫) 2018.09.27

        private List<JSON_DATA> _self_abg { get; set; }
        public List<JSON_DATA> self_abg
        {
            get
            {
                return _self_abg;
            }
            set
            {
                _self_abg = value;
                if (value != null && value.Exists(x => x.chkd == true && x.id == "self_abg"))
                {
                    value.Find(x => x.chkd == true && x.id == "self_abg").val = "1";
                    is_self_abg = "1";
                }
                else if (value != null && value.Exists(x => x.chkd == false && x.id == "self_abg"))
                {
                    value.Find(x => x.chkd == false && x.id == "self_abg").val = "0";
                    is_self_abg = "0";
                }
            }
        }
        private string _is_self_abg { get; set; }
        /// <summary>是否是自己測試ABG </summary>
        public string is_self_abg { get; set; }
        /// <summary>
        /// 業務量註記
        /// </summary>
        public string work_data { get; set; }
        /// <summary>
        /// 適應症
        /// </summary>
        public string breath_indications { get; set; }
        /// <summary>
        /// 呼吸衰竭原因
        /// </summary>
        public string breath_reason { get; set; }

        // 20191029 新增欄位 
        public string dco2 { set; get; }
        public string FA { set; get; }
        public string VA { set; get; }
        public string tube_compensaton { set; get; }
        public string ets { set; get; }
        public string Te { set; get; }
        public string Tp { set; get; }
        public string pressure_plat { set; get; }
        public string total_rate { set; get; }
        public string cuff_leak_sound { set; get; }
        public string pao2_fio2 { set; get; }
        public string oi { set; get; }
        public string biochem_crp { set; get; }
        public string biochem_ca { set; get; }
        public string biochem_cl { set; get; }
        public string vital_signs_temp { set; get; }
        public string ecmo { set; get; }
        public string ecmo_fio2 { set; get; }
        public string ecmo_air_flow { set; get; }
        public string ecmo_co { set; get; }
        public string ecmo_pump_speed { set; get; }
        public string ecmo_svo2 { set; get; }
        public string other_iabp { set; get; }
        public string other_no_flow { set; get; }
        public string other_no2 { set; get; }
        /// <summary>MV天數顯示 </summary>
        public string use_days_how { get; set; }
        public string cuff_ml { get; set; }

        //萬芳_呼吸照護新增欄位2019/11/15
        public string RM { get; set; }
        public string optimal_peep { get; set; }
        public string evaluation_time { get; set; }
        public string ecmo_pump_speed_lmin { get; set; }
        public string blood_flow { get; set; }
        public string gas_flow { get; set; }
        public string no_set { get; set; }
        public string water_collecting_cup { get; set; }
        public string fixed_tripod { get; set; }
        public string delta_p { get; set; }
        public string hfov_mean { get; set; }
        public string hfov_hz { get; set; }
        public string breath_sound_location { get; set; }

        #endregion


        #region abg

        public string abg_time { get; set; }
        public string abg_ph { get; set; }
        public string abg_paco2 { get; set; }
        public string abg_pao2 { get; set; }
        public string abg_sao2 { get; set; }
        public string abg_hco3 { get; set; }
        public string abg_be { get; set; }
        public string abg_paado2 { get; set; }
        public string abg_shunt { get; set; }
        public string abg_pulse_oximeter { get; set; }
        public string abg_device { get; set; }

        #endregion

        #region biochem 

        public string biochem_hb { get; set; }
        public string biochem_ht { get; set; }
        public string biochem_wbc { get; set; }
        public string biochem_p { get; set; }
        public string biochem_na { get; set; }
        public string biochem_k { get; set; }
        public string biochem_bun { get; set; }
        public string biochem_cr { get; set; }
        public string biochem_albumin_sugar { get; set; }
        #endregion
         

        public string vt2 { get; set; }

        public string ve { get; set; }


    }

    public class RT_RECORD_MUST_DATA : IRT_RECORD_GET_LAST
    {
        public string respid { get; set; }
        public string device { get; set; }
        public string mode { get; set; }
        public string device_o2 { get; set; }
        public string vt_set { get; set; }
        public string vr_set { get; set; }
        public string vr { get; set; }
        public string flow { get; set; }
        public string mv_set { get; set; }
        /// <summary>Flow / Flow Pattern  Flow Pattern</summary>
        public string flow_pattern { get; set; }

        public string insp_time { get; set; }
        public string ie_ratio { get; set; }
        public string thigh { get; set; }
        public string tlow { get; set; }
        public string ipap { set; get; }
        public string epap { set; get; }
        public string phigh { get; set; }
        public string plow { get; set; }
        public string spo2 { get; set; }
        /// <summary>NO</summary>
        public string no { get; set; }
        public string artificial_airway_type { get; set; }

        public string et_size { get; set; }
        public string et_mark { get; set; }
        public string cuff { get; set; }
        public string breath_sound { get; set; }
        /// <summary>GCS：E </summary>
        public string gcse { get; set; }
        /// <summary>GCS：V </summary>
        public string gcsv { get; set; }
        /// <summary>GCS：M </summary>
        public string gcsm { get; set; }

        /// <summary>意識</summary>
        public string conscious { get; set; }
        public string humi_temp { get; set; }
        public string pause_time { get; set; }
        public string ramp { get; set; }
        public string rise_time { get; set; }
        public string pressure_sensitivity { get; set; }
        public string sensitivity_flow { get; set; }
        public string tube_compensaton { get; set; }
        public string ets { get; set; }
        public string Te { get; set; }
        public string Tp { get; set; }
        public string fio2_set { get; set; }
        public string fio2_measured { get; set; }
        public string pressure_ps { get; set; }
        public string pressure_pc { get; set; }
        public string pressure_peep { get; set; }
        public string ti { get; set; }
        public string hz { get; set; }
        public string ampl { get; set; }
        public string FA { get; set; }
        public string VA { get; set; }
        public string low_mv_alarm { get; set; }
        public string paw_alarm { get; set; }
        public string mv_percent { get; set; }
        public string exp_tv { get; set; }
        public string vt { get; set; }
        public string mv { get; set; }
        public string pressure_peak { get; set; }
        public string pressure_mean { get; set; }
        public string resistance { get; set; }
        public string Compliance { get; set; }
        public string evaluation_time { get; set; }
        public string vt_value { get; set; }
        public string rsi_srr { get; set; }
        public string rsbi { get; set; }
        public string pi_max { get; set; }
        public string pe_max { get; set; }
        public string cuff_leak_ml { get; set; }
        public string ecmo_svo2 { get; set; }
        public string breath_sound_location { get; set; }
        public string pulse { get; set; }
        public string vital_signs_temp { get; set; }
        public string bps { get; set; }
        public string bpd { get; set; }
        public string water_collecting_cup { get; set; }
        public string fixed_tripod { get; set; }
        public string memo { get; set; }
        /// <summary> △ P </summary>
        public string delta_p { get; set; }
        /// <summary> HFOV Hz </summary>
        public string hfov_hz { get; set; }
        /// <summary> HFOV mean(cmH2O) </summary>
        public string hfov_mean { get; set; }
        /// <summary> RM </summary>
        public string RM { get; set; }
        /// <summary> Optimal PEEP </summary>
        public string optimal_peep { get; set; }
        /// <summary> O2 flow (LPM) </summary>
        public string o2flow { get; set; }
        /// <summary> Try off MV with </summary>
        public string Try_off_MV_1 { get; set; }
        public string Try_off_MV_2 { get; set; }
        public string Try_off_MV_3 { get; set; }
        /// <summary> Try off NIV </summary>
        public string Try_off_NIV_1 { get; set; }
        public string Try_off_NIV_2 { get; set; }
        /// <summary> Time </summary>
        public string abg_time { get; set; }
        /// <summary> pH </summary>
        public string abg_ph { get; set; }
        /// <summary> PaCO2 </summary>
        public string abg_paco2 { get; set; }
        /// <summary> PaO2 </summary>
        public string abg_pao2 { get; set; }
        /// <summary> HCO3 </summary>
        public string abg_hco3 { get; set; }
        /// <summary> B.E. </summary>
        public string abg_be { get; set; }
        /// <summary> SaO2 </summary>
        public string abg_sao2 { get; set; }
        /// <summary> P/F ratio </summary>
        public string pao2_fio2 { get; set; }
        /// <summary> O.I Index </summary>
        public string oi { get; set; }
        /// <summary> Hb </summary>
        public string biochem_hb { get; set; }
        /// <summary> Platelet </summary>
        public string biochem_p { get; set; }
        /// <summary> WBC  </summary>
        public string biochem_wbc { get; set; }
        /// <summary> CRP </summary>
        public string biochem_crp { get; set; }
        /// <summary> Na </summary>
        public string biochem_na { get; set; }
        /// <summary> K(m.Eq/L) </summary>
        public string biochem_k { get; set; }
        /// <summary> BUN </summary>
        public string biochem_bun { get; set; }
        /// <summary> Cr.(mg%) </summary>
        public string biochem_cr { get; set; }
        /// <summary> Ca  </summary>
        public string biochem_ca { get; set; }
        /// <summary> Cl </summary>
        public string biochem_cl { get; set; }
        /// <summary> Albumin </summary>
        public string biochem_albumin_sugar { get; set; }
        /// <summary> NO2 mea. (ppm)  </summary>
        public string other_no2 { get; set; }
        /// <summary> C.O. </summary>
        public string ecmo_co { get; set; }
        /// <summary> ECMO type </summary>
        public string ecmo { get; set; }
        /// <summary> Pump speed (RPM) </summary>
        public string ecmo_pump_speed { get; set; }
        /// <summary> Pump speed (L/min) </summary>
        public string ecmo_pump_speed_lmin { get; set; }
        /// <summary> Blood Flow (L/min) </summary>
        public string blood_flow { get; set; }
        /// <summary> Gas Flow (L/min)  </summary>
        public string gas_flow { get; set; }
        /// <summary> FiO2 </summary>
        public string ecmo_fio2 { get; set; }
        /// <summary> 藥物/輸液備註 </summary>
        public string drug_memo { get; set; }
        public string no_set { get; set; }

    }

}

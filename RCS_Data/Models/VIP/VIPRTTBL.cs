using System;
using System.Collections.Generic;
using System.Linq;
using Dapper;
using RCS_Data;
using Com.Mayaminer;
using mayaminer.com.library;
using System.Data;
using RCSData.Models;
using RCS_Data.Models.ViewModels;
using RCS_Data.Controllers.RtRecord;
using RCS_Data.Models.VIP.Models;

namespace RCS_Data.Models.VIP
{
    public class VIPRTTBL  : BASIC_PARAMS
    { 
        /// <summary>
        /// 呼吸器狀態顯示清單
        /// </summary>
        public List<RT_RECORD_MAIN> rt_record_main_list { get; set; }

        #region 傳入值
        /// <summary>
        /// 類別名稱
        /// </summary>
        private string csName { get { return "VIPRTTBL"; } }
        /// <summary>
        /// VIP_設定
        /// </summary>
        public GET_VIP_SETTING VIP_SETTING { get; set; }
        /// <summary>
        /// VIP暫存資料
        /// </summary>
        private List<DB_VIPRTTBL> temp_list { get; set; }
        #endregion

        /// <summary>
        /// 取得病患資料數量
        /// </summary>
        /// <param name="chart_no"></param>
        /// <returns></returns>
        public int getVIPRTTBL_count(int before_day, string chart_no)
        {
            int cnt = 0;
            string actionName = "getVIPRTTBL_count";
            try
            {
                SQLProvider SQL = new SQLProvider("VIP_DbConnection");
                string sqlstr = string.Concat("select COUNT(patient_id) from viprttbl_cch where save_type in('0','1','2','3') and patient_id = ", SQL.namedArguments, "patient_id and data_time between ", SQL.namedArguments, "data_time and '", DateTime.Now.ToString("yyyyMMdd235959999"), "'");
                Dapper.DynamicParameters dp = new DynamicParameters();
                dp.Add("patient_id", chart_no);
                dp.Add("data_time", DateTime.Now.AddDays(-before_day).ToString("yyyyMMdd000000000"));
                cnt = SQL.DBA.getSqlDataTable<int>(sqlstr, dp).ToList().First();
                //如果沒有資料，而且模式是isDebuggerMode或isBasicMode抓前50筆資料
                if (!string.IsNullOrWhiteSpace(SQL.DBA.lastError))
                {
                    cnt = 0;
                    LogTool.SaveLogMessage(SQL.DBA.lastError, actionName, csName);
                }
                else if (cnt == 0 && ( isDebuggerMode ||  isBasicMode))
                {
                    cnt = 1;
                }
            }
            catch (Exception ex)
            {
                LogTool.SaveLogMessage(ex, actionName, csName);
            }
            return cnt;
        }

        /// <summary>
        /// 取得呼吸器狀態險是清單 to This.rt_record_main_list
        /// </summary>
        /// <param name="before_day">取得前幾天的資料</param>
        /// <param name="chart_no">病歷號</param>
        /// <param name="getTopOneRecord">取得最新的一筆資料(預設:false 不取得)</param>
        public void getRt_Record_Main_List(int before_day, string chart_no, bool getTopOneRecord = false)
        {
            string actionName = "getRt_Record_Main_List";
            try
            {
                SQLProvider SQL = new SQLProvider("VIP_DbConnection");
                //取得before_day之後的資料
                #region 取得 DB_VIPRTTBL 的資料
                string sqlstr = "select * from viprttbl_cch where save_type in('0','1','2','3','5','6') and patient_id LIKE '%" + chart_no + "%' and data_time between '" + DateTime.Now.AddDays(-before_day).ToString("yyyyMMdd000000000") + "' and '" + DateTime.Now.ToString("yyyyMMdd235959999") + "' order by data_time desc";
                this.temp_list = SQL.DBA.getSqlDataTable<DB_VIPRTTBL>(sqlstr);
                //如果沒有資料，而且模式是isDebuggerMode或isBasicMode抓前50筆資料
                if (temp_list != null && temp_list.Count == 0 && ( isDebuggerMode ||  isBasicMode ||  useTestData))
                {
                    sqlstr = "select top 50 * from viprttbl_cch where save_type in('0','1','2','3','5','6')  order by data_time desc";
                    this.temp_list = SQL.DBA.getSqlDataTable<DB_VIPRTTBL>(sqlstr);
                    if (this.temp_list.Count > 5)
                    {
                        DateTime tempDate = DateTime.Now;
                        this.temp_list[0].data_time = tempDate.ToString("yyyyMMddHHmmss");
                        for (int i = 1; i < 6; i++)
                        {
                            this.temp_list[i].data_time = tempDate.AddHours(-i).ToString("yyyyMMddHHmmss");
                        }
                    }

                }
                else if (!string.IsNullOrWhiteSpace(SQL.DBA.lastError))
                {
                    LogTool.SaveLogMessage(SQL.DBA.lastError, actionName, csName);
                }
                #endregion

                #region 工作

                rt_record_main_list = new List<RT_RECORD_MAIN>();
                if (temp_list.Count > 0)
                {
                    if (getTopOneRecord)
                    {
                        this.temp_list = this.temp_list.Take(1).ToList();
                    }

                    this.VIP_SETTING = new GET_VIP_SETTING();
                    setRCS_VIP_ALARM_DESC(temp_list.Select(x => x.device).Distinct().ToList());//取的警告訊息
                    VipColSettings VipColList = this.getVipColList();
                    foreach (DB_VIPRTTBL _item in temp_list)
                    {
                        DB_VIPRTTBL item = Newtonsoft.Json.JsonConvert.DeserializeObject<DB_VIPRTTBL>(Newtonsoft.Json.JsonConvert.SerializeObject(_item));
                        VIP_SETTING.setVIP_SETTING(item);
                        VipColList.ThisDevice = item.device;
                        VipColList.ThisMode = item.mode;
                        RT_RECORD_MAIN rt_record_main = new RT_RECORD_MAIN();
                        rt_record_main.on_intubate = new ONINTUBATE();
                        rt_record_main.on_breath = new ONBREATH();
                        rt_record_main.on_oxygen = new ONOXYGEN();
                        rt_record_main.hasPowerEdit = true;
                        rt_record_main.isVIP = true;
                        rt_record_main.RECORDDATE = DateHelper.Parse(string.Format("{0} {1}".Trim(), item.recorddate, item.recordtime)).ToString("yyyy-MM-dd HH:mm:ss");
                        rt_record_main.central_message = VIP_SETTING.getALARM_DESC();
                        #region 設定各別資料
                        item.mode = !VipColList.ShowValue("REPLACE_MODE") ? item.mode : VipColList.REPLACE_MODE!= null && 
                            VipColList.REPLACE_MODE.Keys.Contains(VipColList.ThisKey) &&
                            !string.IsNullOrWhiteSpace(VipColList.REPLACE_MODE[VipColList.ThisKey]) && 
                            !string.IsNullOrWhiteSpace(VipColList.REPLACE_MODE[VipColList.ThisKey].Trim()) ?  
                            VipColList.REPLACE_MODE[VipColList.ThisKey] : item.mode;
                        item.vt_set = !VipColList.ShowValue("vt_set") ? "" : item.vt_set;
                        item.exp_tv = !VipColList.ShowValue("vt") ? "" : item.vt;
                        item.vt = "";
                        item.vr_set = !VipColList.ShowValue("vr_set") ? "" : item.vr_set;
                        item.vr = !VipColList.ShowValue("vr") ? "" : item.vr;
                        item.total_rate = item.vr;
                        if (!string.IsNullOrWhiteSpace(item.vr) && item.vr.Contains("."))
                        {
                            double rst = 0.00;
                            double.TryParse(item.vr, out rst);
                            item.vr = Math.Round(Math.Round(rst, 1, MidpointRounding.AwayFromZero), MidpointRounding.AwayFromZero).ToString();
                        }
                        item.insp_time = !VipColList.ShowValue("insp_time") ? "" : item.insp_time;
                        item.ie_ratio = !VipColList.ShowValue("ie_ratio") ? "" : item.ie_ratio;
                        item.flow = !VipColList.ShowValue("flow") ? "" : item.flow;
                        item.mv_set = !VipColList.ShowValue("mv_set") ? "" : item.mv_set;
                        item.mv = !VipColList.ShowValue("mv") ? "" : item.mv; 

                        item.pressure_peak = !VipColList.ShowValue("pressure_peak") ? "" : item.pressure_peak;
                        item.pressure_plateau = !VipColList.ShowValue("pressure_plateau") ? "" : item.pressure_plateau;
                        item.pressure_mean = !VipColList.ShowValue("pressure_mean") ? "" : item.pressure_mean;
                        item.pressure_peep = !VipColList.ShowValue("pressure_peep") ? "" : item.pressure_peep;
                        item.pressure_ps = !VipColList.ShowValue("pressure_ps") ? "" : item.pressure_ps;
                        item.pressure_pc = !VipColList.ShowValue("pressure_pc") ? "" : item.pressure_pc;
                        item.e_sense = !VipColList.ShowValue("e_sense") ? "" : item.e_sense;

                        item.rise_time = !VipColList.ShowValue("rise_time") ? "" : item.rise_time;
                        item.temp = !VipColList.ShowValue("temp") ? "" : item.temp;
                        item.fio2_set = !VipColList.ShowValue("fio2_set") ? "" : item.fio2_set;
                        item.low_mv_alarm = !VipColList.ShowValue("low_mv_alarm") ? "" : item.low_mv_alarm;
                        item.paw_alarm = !VipColList.ShowValue("paw_alarm") ? "" : item.paw_alarm;
                        item.pressure_sensitivity = !VipColList.ShowValue("pressure_sensitivity") ? "" : item.pressure_sensitivity;
                        item.sensitivity_flow = !VipColList.ShowValue("sensitivity_flow") ? "" : item.sensitivity_flow;
                      
                        if (isDebuggerMode)
                        {
                            item.sensitivity_flow = "3";
                        }
                        if (!string.IsNullOrWhiteSpace(item.sensitivity_flow) &&
                            item.sensitivity_flow.Length > 0 && !item.sensitivity_flow.ToUpper().StartsWith("F"))
                        {
                            item.sensitivity_flow = string.Concat("F", item.sensitivity_flow);
                        }

                        item.phigh = !VipColList.ShowValue("phigh") ? "" : item.phigh;
                        item.plow = !VipColList.ShowValue("plow") ? "" : item.plow;
                        item.thigh = !VipColList.ShowValue("thigh") ? "" : item.thigh;
                        item.tlow = !VipColList.ShowValue("tlow") ? "" : item.tlow;
                         
                        #endregion
                        #region extendItems
                        if (!string.IsNullOrWhiteSpace(item.extendItems))
                        {
                            EXTEND_ITEM _extend_item = new EXTEND_ITEM();
                            try
                            {
                                _extend_item = Newtonsoft.Json.JsonConvert.DeserializeObject<EXTEND_ITEM>(item.extendItems);
                                if (!string.IsNullOrWhiteSpace(_extend_item.vt_insp)) item.vt = _extend_item.vt_insp;
                                if (!string.IsNullOrWhiteSpace(_extend_item.compl)) item.Compliance = _extend_item.compl;
                                if (!string.IsNullOrWhiteSpace(_extend_item.resist)) item.resistance = _extend_item.resist;
                                if (!string.IsNullOrWhiteSpace(_extend_item.min_pr_alarm)) item.Pres_Limit_L = _extend_item.min_pr_alarm;
                                if (!string.IsNullOrWhiteSpace(_extend_item.flow_pattern))
                                {
                                    item.flow_pattern = _extend_item.flow_pattern;
                                    switch (_extend_item.flow_pattern)
                                    {
                                        case "2":
                                            item.flow_pattern = "square";
                                            break;
                                        case "4":
                                            item.flow_pattern = "descend";
                                            break;
                                        case "8":
                                            item.flow_pattern = "sine";
                                            break;
                                        case "3":
                                            item.flow_pattern = "descend";
                                            break;
                                        default:
                                            break;
                                    }
                                }
                                if (!string.IsNullOrWhiteSpace(_extend_item.mv_percent)) item.mv_percent = _extend_item.mv_percent;
                                if (!string.IsNullOrWhiteSpace(_extend_item.i_time__percent)) item.ti = _extend_item.i_time__percent;
                                if (!string.IsNullOrWhiteSpace(_extend_item.ramp)) item.ramp = _extend_item.ramp;
                                if (!string.IsNullOrWhiteSpace(_extend_item.hz)) item.hz = _extend_item.hz;
                                if (!string.IsNullOrWhiteSpace(_extend_item.ampl)) item.ampl = _extend_item.ampl;
                                if (!string.IsNullOrWhiteSpace(_extend_item.fa)) item.FA = _extend_item.fa;
                                if (!string.IsNullOrWhiteSpace(_extend_item.va)) item.VA = _extend_item.va;
                                if (!string.IsNullOrWhiteSpace(_extend_item.ets)) item.ets = _extend_item.ets;
                                if (!string.IsNullOrWhiteSpace(_extend_item.compen)) item.tube_compensaton = _extend_item.compen;
                                if (!string.IsNullOrWhiteSpace(_extend_item.o2_flow)) item.flow = _extend_item.o2_flow;
                                if (!string.IsNullOrWhiteSpace(_extend_item.insp_t_mntr)) item.insp_time = _extend_item.insp_t_mntr;
                                 
                                //TODO: HiFlowO2 Flow
                                // if (!string.IsNullOrWhiteSpace(_extend_item.o2_flow)) item.flow = _extend_item.o2_flow;
                            }
                            catch (Exception ex)
                            {
                                LogTool.SaveLogMessage(ex, actionName, csName);
                            }
                        }
                        #endregion


                        #region 手動輸入
                        item.pi_max = !VipColList.ShowValue("pi_max") ? "" : item.pi_max;
                        item.pe_max = !VipColList.ShowValue("pe_max") ? "" : item.pe_max;
                        item.rsbi = !VipColList.ShowValue("rsi") ? "" : item.rsi;
                        item.rsi_srr = !VipColList.ShowValue("rr") ? "" : item.rr; 
                        item.vt2 = !VipColList.ShowValue("vt2") ? "" : item.vt2;
                        item.ve = !VipColList.ShowValue("ve") ? "" : item.ve;
                        item.mv_value = item.ve;
                        item.vt_value = item.vt2;

                        item.fio2_measured = item.fio2_ana;
                        if (item.et_mark == "Tr" || item.et_mark == "0")
                        {
                            item.et_mark = "Tr";
                            item.artificial_airway_type = "Tr";
                        }
                       
                        string temp_emv = string.IsNullOrWhiteSpace(item.emv) ? "" : item.emv;
                        if (!string.IsNullOrWhiteSpace(temp_emv))
                        {
                            List<string> tempList = temp_emv.Split('/').ToList();
                            if (tempList.Count >= 1) item.gcse = tempList[0] ?? "";
                            if (tempList.Count >= 2) item.gcsv = tempList[1] ?? "";
                            if (item.gcsv == "7") item.gcsv = "E";
                            if (item.gcsv == "0") item.gcsv = "Tr.";
                            if (tempList.Count >= 3) item.gcsm = tempList[2] ?? "";
                            if (item.gcsm == "E") item.gcsm = "1";
                        }
                        string temp_breath_sound = string.IsNullOrWhiteSpace(item.breath_sound) ? "" : item.breath_sound;
                        if (!string.IsNullOrWhiteSpace(temp_breath_sound))
                        {
                            List<string> breath_soundList = temp_breath_sound.Split(',').ToList();
                            if (breath_soundList.Exists(x => x.StartsWith("Bil")))
                                item.breath_sound = breath_soundList.Find(x => x.StartsWith("Bil")).Replace("Bil", "").Trim();
                        }
                        if (!string.IsNullOrWhiteSpace(item.breath_sound))
                        {
                            item.breath_sound = item.breath_sound.Replace("'", "`");
                        }
                        #endregion
                        this.VIP_SETTING.setTHisDeviceDATA(ref item); 
                        rt_record_main.rt_record = item;
                        rt_record_main_list.Add(rt_record_main);


                    }
                }

                #endregion

            }
            catch (Exception ex)
            {
                LogTool.SaveLogMessage(ex, actionName, csName);
            }
            //this.rt_record_main_list
            #region 結果


            #endregion
        }


        #region VIP功能

        /// <summary>
        /// RCS_VIP_DATA_SETTINGS 設定資料
        /// </summary>
        /// <returns></returns>
        public VipColSettings getVipColList()
        {
            VipColSettings rc = new VipColSettings();
            string actionName = "getVipColList";
            try
            {
                if ( VipColSettingsSwitch)
                {
                    SQLProvider dbLink = new SQLProvider();
                    DataTable dt = dbLink.DBA.getSqlDataTable(string.Concat("SELECT * FROM RCS_VIP_DATA_SETTINGS WHERE 1=1"));
                    if (dt != null && dt.Rows.Count > 0)
                    {
                        foreach (DataRow ro in dt.Rows)
                        {
                            string key = string.Format("{0}_{1}", ro["DEVICE_TYPE"].ToString().Trim(), ro["VENTILATION_MODE"].ToString().Trim());
                            if (!rc.ContainsKey(key))
                            {
                                rc.Add(key, ro);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LogTool.SaveLogMessage(ex, actionName, csName);
            }

            return rc;
        }

        /// <summary>
        /// 設定VIP_ALARM訊息 to this.VIP_SETTING
        /// </summary>
        public void setRCS_VIP_ALARM_DESC(List<string> deviceList)
        {
            string actionName = "setRCS_VIP_ALARM_DESC";
            try
            {
                SQLProvider SQL = new SQLProvider();
                List<RCS_VIP_ALARM_DESC> RCS_VIPList = new List<RCS_VIP_ALARM_DESC>();
                string sql = "SELECT DEVICE_TYPE,ALARM_CODE,ALARM_DESC,ORDER_BY,ALARM_MSG FROM RCS_VIP_ALARM_DESC";
                if (deviceList.Count > 0)
                {
                    sql += " WHERE DEVICE_TYPE in('" + string.Join("','", deviceList) + "')";
                }
                RCS_VIPList = SQL.DBA.getSqlDataTable<RCS_VIP_ALARM_DESC>(sql);
                if (string.IsNullOrWhiteSpace(SQL.DBA.lastError))
                {
                    foreach (var item in RCS_VIPList)
                    {
                        VIP_SETTING.Add(item);
                    }
                }
                else
                {
                    LogTool.SaveLogMessage(SQL.DBA.lastError, "FormStatusData");
                }
            }
            catch (Exception ex)
            {
                LogTool.SaveLogMessage(ex, actionName, csName);
            }
        }
        /// <summary>
        /// 1.判斷小白機的資料是否重複? alert提示5小時內是否有重複輸入資料(呼吸機型號穿插)
        /// 2.VIP當機處置：管理系統跳訊息，將警示訊息置於【呼吸照護記錄單】
        /// <para>若病患呼吸器狀態有一天內的資料 且病患超過兩個小時沒有新的資料，將有警示，警示持續五個小時後結束。</para>
        /// 3.取得病患一個小時內的錯誤訊息資料
        /// </summary>
        /// <returns></returns>
        public RESPONSE_MSG checkVIPDataHasRepeat(string chart_no)
        {
            string actionName = "checkVIPDataHasRepeat";
            RESPONSE_MSG rm = new RESPONSE_MSG();
            DateTime now_time = DateTime.Now;
            List<string> msgList = new List<string>();//訊息清單
            try
            {
                SQLProvider SQL = new SQLProvider("VIP_DbConnection");
                int cnt = 0;//此病患使用呼吸機數量(大於2可能使用多台呼吸器上傳)
                List<string> chkList = new List<string>();//使用的呼吸器清單
                List<string> respidList = new List<string>();//呼吸器財產編號清單

                #region 工作
                //取得五小時內的呼吸機資料
                string sql = "select TOP 6 location,data_time from viprttbl_cch where patient_id LIKE '%" + chart_no +
                    "%' and data_time between '" + DateTime.Now.AddHours(-5).ToString("yyyyMMddHHmm00000") +
                    "' and '" + DateTime.Now.ToString("yyyyMMddHHmm59999") + "' order by data_time desc";
                DataTable dt = SQL.DBA.getSqlDataTable(sql);

                if (DTNotNullAndEmpty(dt))
                {
                    #region 1.判斷小白機的資料是否重複? alert提示5小時內是否有重複輸入資料(呼吸機型號穿插)
                    foreach (System.Data.DataRow dr in dt.Rows)
                    {
                        if (!DBNull.ReferenceEquals(dr["location"], DBNull.Value))
                        {
                            string tempRespid = dr["location"].ToString().Trim();
                            if (chkList.Count > 0)
                            {
                                //比較前一台location是否一樣，不一樣的話註記一次
                                if (chkList[respidList.Count - 1] != tempRespid) cnt++;
                            }
                            chkList.Add(tempRespid);
                            if (!respidList.Contains(tempRespid)) respidList.Add(tempRespid);
                            //註記次數超過2次，此病患可能使用多台呼吸機，提醒RT
                            if (cnt >= 2)
                            {
                                rm.status = RESPONSE_STATUS.ERROR;
                                msgList.Add(string.Format("發現病患({0})使用了多台呼吸機，請確認原因，財產編號:({1})", chart_no, string.Join(",", respidList)));
                                rm.message = string.Format("發現病患({0})使用了多台呼吸機，請確認原因，財產編號:({1})", chart_no, string.Join(",", respidList));
                                break;
                            }
                            rm.status = RESPONSE_STATUS.SUCCESS;
                        }
                    }
                    #endregion

                    #region VIP當機處置：管理系統跳訊息，將警示訊息置於【呼吸器狀態】
                    string location = "";
                    List<DataRow> drList = dt.AsEnumerable().OrderByDescending(x => long.Parse(x["data_time"].ToString())).ToList();
                    string dataTime = drList[0]["data_time"].ToString();
                    DateTime data_time = DateHelper.Parse(dataTime, "yyyyMMddHHmmssfff");
                    location = drList[0]["location"].ToString();
                    if (now_time.Hour - data_time.Hour >= 2)
                    {
                        msgList.Add(string.Format("發現病患({0})已有兩小時沒有上傳資料，請確認VIP使用狀況，財產編號:({1})", chart_no, location));
                    }
                    #endregion
                }
                else
                {
                    if (!string.IsNullOrWhiteSpace(SQL.DBA.lastError))
                    {
                        rm.status = RESPONSE_STATUS.ERROR;
                        msgList.Add("檢查呼吸機資料失敗，錯誤訊息如下所示:" + SQL.DBA.lastError);
                        LogTool.SaveLogMessage(SQL.DBA.lastError, actionName, GetLogToolCS.BaseModel);
                    }
                }
                #region 取得病患一個小時內的錯誤訊息資料
                List<DB_VIPRTTBL> list = this.getAlarm_msg(chart_no);
                if (list.Count > 0)
                {
                    msgList.Add("一小時內呼吸器有警告訊息");
                }
                #endregion
                #endregion


                #region 結果
#if DEBUG
                //msgList.Add("test");
#endif
                if (msgList.Count > 0)
                {
                    rm.status = RESPONSE_STATUS.ERROR;
                    rm.message = string.Join("<br \\>", msgList);
                }
                #endregion
            }
            catch (Exception ex)
            {
                rm.status = RESPONSE_STATUS.EXCEPTION;
                rm.message = "檢查呼吸機資料失敗，錯誤訊息如下所示:" + ex.Message;
                LogTool.SaveLogMessage(ex, actionName, GetLogToolCS.BaseModel);
            }
            return rm;
        }

        /// <summary>
        /// 取得病患一個小時內的錯誤訊息資料
        /// </summary>
        /// <param name="pat_id"></param>
        /// <returns></returns>
        public List<DB_VIPRTTBL> getAlarm_msg(string chart_no)
        {
            string actionName = "getAlarm_msg";
            List<DB_VIPRTTBL> list = new List<DB_VIPRTTBL>();
            try
            {
                SQLProvider SQL = new SQLProvider("VIP_DbConnection");
#if DEBUG
                //list.Add(new DB_VIPRTTBL());
#else

                string sql = "SELECT * FROM VIPRTTBL_CCH WHERE save_type = '3' AND patient_id =" + SQLDefend.SQLString(chart_no) + " AND data_time >= " + SQLDefend.SQLString(DateTime.Now.AddHours(-1).ToString("yyyyMMddHHmm00000"));
                list = SQL.DBA.getSqlDataTable<DB_VIPRTTBL>(sql);
#endif
            }
            catch (Exception ex)
            {
                LogTool.SaveLogMessage(ex, actionName, csName);
            }
            return list;
        }

        /// <summary>
        /// 檢查呼吸器編號(財產編號)是否存在，如果沒有維護，自動新增
        /// </summary>
        /// <param name="respid">財產編號</param>
        /// <param name="device">儀器型號</param>
        public void checkDEVICE_NO(string respid, string device, UserInfo pUser_info)
        {
            string actionName = "checkDEVICE_NO";
            try
            {
                SQLProvider SQL = new SQLProvider();
                if (!string.IsNullOrWhiteSpace(respid) && respid != "201")
                {
                    string sql = "SELECT * FROM RCS_VENTILATOR_SETTINGS WHERE DEVICE_NO = " + SQLDefend.SQLString(respid);
                    DataTable dt = SQL.DBA.getSqlDataTable(sql);
                    //如果沒有此呼吸器財產編號的話
                    if (dt != null && dt.Rows.Count == 0)
                    {
                        DataRow dr = dt.NewRow();
                        dr["DEVICE_SEQ"] = device + "-" + respid;
                        dr["DEVICE_MODEL"] = device;
                        dr["DEVICE_NO"] = respid;
                        dr["USE_STATUS"] = "Y";
                        dr["PURCHASE_DATE"] = "";
                        dr["CREATE_ID"] = pUser_info.user_id;
                        dr["CREATE_NAME"] = pUser_info.user_name;
                        dr["CREATE_DATE"] = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                        dt.Rows.Add(dr);
                        SQL.DBA.Update(dt, "RCS_VENTILATOR_SETTINGS");
                    }
                    if (!string.IsNullOrWhiteSpace(SQL.DBA.lastError))
                    {
                        LogTool.SaveLogMessage(SQL.DBA.lastError, actionName, csName);
                    }
                }

            }
            catch (Exception ex)
            {

                LogTool.SaveLogMessage(ex, actionName, csName);
            }

        }

        #endregion

    }

    public class DB_VIPRTTBL : RT_RECORD
    {
        /// <summary>
        /// 呼吸器量測資料
        /// </summary>
        public string MONITOR_DATA { get; set; }
        /// <summary>
        /// 呼吸器上下限值設定
        /// </summary>
        public string ALARM_SETTING { get; set; }
        /// <summary>
        /// 系統設定值
        /// </summary>
        public string SETTINGS { get; set; }
        /// <summary>
        /// 文字訊息
        /// </summary>
        public string TEXT_MESSAGE { get; set; }
        /// <summary>
        /// 警告訊息
        /// </summary>
        public string alarm_message { get; set; }
        /// <summary>
        /// 警告訊息區間
        /// </summary>
        public string central_message { get; set; }
        private string _location { get; set; }

        /// <summary>
        /// location呼吸器財產編號
        /// </summary>
        public string location
        {
            get
            {
                return _location;
            }
            set
            {
                _location = value;
                if (!string.IsNullOrWhiteSpace(_location) && _location.Contains("-"))
                {
                    SQLProvider SQL = new SQLProvider();
                    string _query = "SELECT * FROM RCS_VENTILATOR_SETTINGS WHERE  REPLACE (DEVICE_MODEL, ' ', '') = @DEVICE_MODEL AND DEVICE_NUM =@DEVICE_NUM";
                    Dapper.DynamicParameters dp = new DynamicParameters();
                    dp.Add("DEVICE_MODEL", _location.Split('-')[0]);
                    dp.Add("DEVICE_NUM", string.Concat("(", _location.Split('-')[1], ")"));
                    DataTable dt = SQL.DBA.getSqlDataTable(_query, dp);
                    if (SQL.DBA.hasLastError)
                    {
                        LogTool.SaveLogMessage(SQL.DBA.lastError, "location", "VIPRTTBL");
                        this.respid = _location;
                    }
                    else
                    {
                        if (dt.Rows.Count > 0)
                        {
                            this.respid = dt.Rows[0]["DEVICE_NO"].ToString();
                        }
                        else
                        {
                            this.respid = _location;
                        }
                    }
                }
            }
        }

        private string _data_time { get; set; }
        /// <summary>
        /// data_time vip  上傳資料
        /// </summary>
        public string data_time
        {
            get
            {

                return _data_time;
            }
            set
            {
                _data_time = value;
                DateTime data_time_tmp = DateTime.ParseExact(data_time.PadRight(17, '0'), "yyyyMMddHHmmssfff", new System.Globalization.CultureInfo("zh-TW", true));
                this.recorddate = data_time_tmp.ToString("yyyy-MM-dd");
                this.recordtime = data_time_tmp.ToString("HH:mm:ss");
            }
        }
        private string _fio2_ana { get; set; }
        /// <summary>
        /// fio2_ana
        /// </summary>
        public string fio2_ana
        {
            get
            {
                return _fio2_ana;
            }
            set
            {
                _fio2_ana = value;
                this.fio2_measured = _fio2_ana;
            }
        }
        private string _pulse { get; set; }
        /// <summary>
        /// pulse
        /// </summary>
        public string pulse
        {
            get
            {
                return _pulse;
            }
            set
            {
                _pulse = value;
                this.pr = _pulse;
            }
        }

        private string _emv { get; set; }
        /// <summary>
        /// emv
        /// </summary>
        public string emv
        {
            get
            {
                return _emv;
            }
            set
            {
                _emv = value;
                if (!string.IsNullOrWhiteSpace(_emv))
                {
                    List<string> tempList = _emv.Split('/').ToList();
                    if (tempList.Count >= 1) this.gcse = tempList[0] ?? "";
                    if (tempList.Count >= 2) this.gcsv = tempList[1] ?? "";
                    if (tempList.Count >= 3) this.gcsm = tempList[2] ?? "";
                }
            }
        }

        public string extendItems { get; set; }

        public EXTEND_ITEM extend_item_obj
        {
            get
            {
                if (!string.IsNullOrWhiteSpace(this.extendItems))
                {
                    return Newtonsoft.Json.JsonConvert.DeserializeObject<EXTEND_ITEM>(this.extendItems);
                }
                return new EXTEND_ITEM();
            }
        }
    }

    /// <summary>
    /// vip欄位設定
    /// </summary>
    public class VipColSettings : Dictionary<string, DataRow>
    {
        string csName { get { return "VipColSettings"; } }
        private bool VipColSettingsSwitch { get; set; }
        public VipColSettings()
        {
            VipColSettingsSwitch = bool.Parse(IniFile.GetConfig("SystemConfig", "VipColSettingsSwitch"));
        }

        public string ThisKey { get { return string.Concat(this.ThisDevice, "_", this.ThisMode); }}

        public string ThisDevice { get; set; }
        public string ThisMode { get; set; }

        public Dictionary<string,string> REPLACE_MODE { get; set; }

        public Dictionary<string, bool> ShowColDic { get; set; }
        /// <summary>
        /// 預設顯示 return true;
        /// </summary>
        /// <param name="pColName"></param>
        /// <returns></returns>
        public bool ShowValue(string pColName)
        {
            string actionName = "ShowValue";
            try
            {
                if (VipColSettingsSwitch)
                {
                    if (ShowColDic == null)
                    {
                        ShowColDic = new Dictionary<string, bool>();
                        foreach (DataRow ro in this.Values)
                        {
                            string device = ro["DEVICE_TYPE"].ToString().Trim();
                            string mode = ro["VENTILATION_MODE"].ToString().Trim();
                            foreach (DataColumn col in ro.Table.Columns)
                            {
                                if (col.ColumnName != "DEVICE_TYPE" && col.ColumnName != "VENTILATION_MODE")
                                {
                                    if (col.ColumnName ==  "REPLACE_MODE")
                                    {
                                        string key = string.Format("{0}_{1} ", device, mode );
                                        if (REPLACE_MODE == null)
                                        {
                                            REPLACE_MODE = new Dictionary<string, string>();
                                        }
                                        REPLACE_MODE.Add(key.Trim() , DBNull.ReferenceEquals(ro[col.ColumnName], DBNull.Value) ? mode : ro[col.ColumnName].ToString());
                                    }
                                    else
                                    {
                                        string key = string.Format("{0}_{1}_{2}", device, mode, col.ColumnName);
                                        bool show = DBNull.ReferenceEquals(ro[col.ColumnName], DBNull.Value) ||
                                            ro[col.ColumnName].ToString().Trim() != "Y";
                                        ShowColDic.Add(key, show);
                                    }
                                    
                                }
                            }
                        }
                    }
                    if (pColName == "REPLACE_MODE")
                    {
                        return true;
                    }
                    string _key = string.Format("{0}_{1}", ThisKey, pColName);
                    if (ShowColDic.ContainsKey(_key))
                    {
                        return ShowColDic[_key];
                    }
                }
            }
            catch (Exception ex)
            {
                LogTool.SaveLogMessage(ex, actionName, csName);
            }

            return true;
        }

    }

    /// <summary>
    /// 取得VIP設定(繼承List RCS_VIP_ALARM_DESC )
    /// </summary>
    public class GET_VIP_SETTING : List<RCS_VIP_ALARM_DESC>
    {
        #region 傳入值
        /// <summary>
        /// vip儲存資料類型
        /// </summary>
        public string save_type { get; set; }
        /// <summary>
        /// 呼吸器機型
        /// </summary>
        public string device { get; set; }
        /// <summary>
        /// 呼吸器模式
        /// </summary>
        public string mode { get; set; }
        /// <summary>
        /// extend_item
        /// </summary>
        public EXTEND_ITEM extend_item { get; set; }
        /// <summary>
        /// 警告訊息
        /// </summary>
        public string central_message { get; set; }
        /// <summary>
        /// 呼吸器量測資料
        /// </summary>
        public string MONITOR_DATA { get; set; }
        /// <summary>
        /// 呼吸器上下限值設定
        /// </summary>
        public string ALARM_SETTING { get; set; }
        /// <summary>
        /// 系統設定值
        /// </summary>
        public string SETTINGS { get; set; }
        /// <summary>
        /// 文字訊息
        /// </summary>
        public string TEXT_MESSAGE { get; set; }
        #endregion

        /// <summary>
        /// vip_警告訊息設定
        /// </summary>
        public IVIP_ALARM_SETTING VIP_ALARM_SETTING { get; set; }

        /// <summary>
        /// 設定VIP設定
        /// </summary>
        public void setVIP_SETTING(DB_VIPRTTBL vipData)
        {

            //VIP_SETTING 傳入值
            #region 傳入值
            this.device = vipData.device;
            this.mode = vipData.mode;
            this.save_type = vipData.save_type;
            this.ALARM_SETTING = vipData.ALARM_SETTING;
            this.SETTINGS = vipData.SETTINGS;
            this.TEXT_MESSAGE = vipData.TEXT_MESSAGE;
            this.central_message = vipData.alarm_message; 
            if (string.IsNullOrWhiteSpace(vipData.alarm_message))
            {
                this.central_message = vipData.central_message;
            }
            #endregion

            switch (this.device)
            {
                //設定介面
                #region 指定介面
                case "EvitaV300":
                    VIP_ALARM_SETTING = new VIP_SETTING_EvitaV300();
                    break;
                case "E500":
                    VIP_ALARM_SETTING = new VIP_SETTING_E500();
                    break;
                case "PB760":
                    VIP_ALARM_SETTING = new VIP_SETTING_PB760();
                    break;
                case "PB840":
                    VIP_ALARM_SETTING = new VIP_SETTING_PB840();
                    break;
                case "300C":
                    VIP_ALARM_SETTING = new VIP_SETTING_300C();
                    break;
                case "EVITA-4":
                    VIP_ALARM_SETTING = new VIP_SETTING_EVITA_4();
                    break;
                case "Evita XL":
                    VIP_ALARM_SETTING = new VIP_SETTING_Evita_XL();
                    break;
                case "AVEA":
                    VIP_ALARM_SETTING = new VIP_SETTING_AVEA();
                    break;
                case "savina-300":
                    VIP_ALARM_SETTING = new VIP_SETTING_Savina300();
                    break;
                case "V60":
                    VIP_ALARM_SETTING = new VIP_SETTING_BASIC();
                    break;
                case "RAPHEAL":
                    VIP_ALARM_SETTING = new VIP_SETTING_BASIC();
                    break;
                case "Galileo":
                    //Galileo
                    VIP_ALARM_SETTING = new VIP_SETTING_Galileo();
                    break;
                case "HamiltonC3":
                    //HamiltonC3
                    VIP_ALARM_SETTING = new VIP_SETTING_HamiltonC3();
                    break;
                case "HamiltonG5":
                    //HamiltonG5
                    VIP_ALARM_SETTING = new VIP_SETTING_HamiltonG5();
                    break;
                case "SERVO-i":
                    //HamiltonG5
                    VIP_ALARM_SETTING = new VIP_SETTING_SERVO_i();
                    break;
                default: 
                    VIP_ALARM_SETTING = new VIP_SETTING_BASIC();
                    break;
                    #endregion

            }
            //設定vip資料
            vipData = VIP_ALARM_SETTING.setVIPData(vipData);
            //設定警告訊息資料
            #region

 
            if (this.Exists(x => x.DEVICE_TYPE == this.device))
            {
                VIP_ALARM_SETTING.VIP_ALARMList = this.FindAll(x => x.DEVICE_TYPE == this.device);
                VIP_ALARM_SETTING.setALARM_CONT();
            }
            else
            {
                VIP_ALARM_SETTING.VIP_ALARMList = new List<RCS_VIP_ALARM_DESC>();
            }
            #endregion

        }

        /// <summary>
        /// 取得警告訊息
        /// </summary>
        /// <returns></returns>
        public string getALARM_DESC()
        {
            string returnStr = "";
            RESPONSE_MSG rm = new RESPONSE_MSG();
            rm.attachment = "";
            try
            {
                if (this.save_type == "3")
                {

                    List<string> alarmKeyList = VIP_ALARM_SETTING.getALARM_Msg(this.central_message);
                    if (alarmKeyList.Count > 0)
                    {
                        rm.attachment = string.Join("<br />", alarmKeyList);

                    }
                    else
                    {
                        rm.attachment = "";
                    }
                }

            }
            catch (Exception ex)
            {
                rm.status = RESPONSE_STATUS.EXCEPTION;
                rm.message = ex.Message;
                rm.attachment = ex;
            }
            //取得結果
            if (rm.status == RESPONSE_STATUS.SUCCESS)
            {
                returnStr = (string)rm.attachment;
            }
            else
            {
                LogTool.SaveLogMessage((Exception)rm.attachment, "getALARM_DESC");
                returnStr = "";
            }

            return returnStr;
        }
        public void setTHisDeviceDATA(ref DB_VIPRTTBL pitem)
        {
            this.VIP_ALARM_SETTING.setTHisDeviceDATA(ref pitem);
        }
    }

    /// <summary>
    /// vip各機型介面
    /// </summary>
    public interface IVIP_ALARM_SETTING
    {
        /// <summary>
        /// 設定呼吸器資料
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        DB_VIPRTTBL setVIPData(DB_VIPRTTBL item);
        #region 傳入值設定

        /// <summary>
        /// 設定此機型資料
        /// </summary>
        /// <param name="item"></param>
        void setTHisDeviceDATA(ref DB_VIPRTTBL item);

        /// <summary>
        /// 設定各機型呼吸器量測資料
        /// </summary>
        void setMONITOR_DATA(ref DB_VIPRTTBL item);

        /// <summary>
        /// 設定各機型呼吸器上下限值設定
        /// </summary>
        void setALARM_SETTING(ref DB_VIPRTTBL item);

        /// <summary>
        /// 設定各機型警報訊息
        /// </summary>
        void setALARM_MESSAGE(ref DB_VIPRTTBL item);

        /// <summary>
        /// 設定各機型系統設定值
        /// </summary>
        void setSETTINGS(ref DB_VIPRTTBL item);

        /// <summary>
        /// 設定各機型文字訊息
        /// </summary>
        void setTEXT_MESSAGE(ref DB_VIPRTTBL item);
        #endregion

        /// <summary>
        /// alarm訊息
        /// </summary>
        List<RCS_VIP_ALARM_DESC> VIP_ALARMList { get; set; }

        /// <summary>
        /// 設定各機型代碼格式
        /// </summary>
        void setALARM_CONT();

        /// <summary>
        /// 取得警告訊息
        /// </summary>
        /// <returns></returns>
        List<string> getALARM_Msg(string central_message);
    }

    /// <summary>
    /// vip功能abstract
    /// </summary>
    public abstract class AVIP_SETTING : IVIP_ALARM_SETTING
    {
        public DB_VIPRTTBL setVIPData(DB_VIPRTTBL item)
        {
            setMONITOR_DATA(ref item);
            setALARM_SETTING(ref item);
            setALARM_MESSAGE(ref item);
            setSETTINGS(ref item);
            setTEXT_MESSAGE(ref item);
            return item;
        }
        #region 傳入值設定
        public virtual void setTHisDeviceDATA(ref DB_VIPRTTBL item)
        {

        }

        public void setMONITOR_DATA(ref DB_VIPRTTBL item)
        {

        }
        public void setALARM_SETTING(ref DB_VIPRTTBL item)
        {

        }
        public void setALARM_MESSAGE(ref DB_VIPRTTBL item)
        {

        }
        public void setSETTINGS(ref DB_VIPRTTBL item)
        {

        }
        public void setTEXT_MESSAGE(ref DB_VIPRTTBL item)
        {

        }
        #endregion


        public List<RCS_VIP_ALARM_DESC> VIP_ALARMList { get; set; }

        /// <summary>
        /// 內容機型_alarm代碼
        /// </summary>
        public virtual void setALARM_CONT()
        {

        }

        public virtual List<string> getALARM_Msg(string central_message)
        {
            List<string> msgList = new List<string>();

            return msgList;
        }
    }

    /// <summary>
    /// 預設VIP機型
    /// </summary>
    public class VIP_SETTING_BASIC : AVIP_SETTING
    {

    }

}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using RCS_Data;
using mayaminer.com.jxDB;
using Com.Mayaminer;
using log4net;
using RCS_Data.Models;

namespace RCS.Models.DB
{
    public class DBACentralMonitor: DBA_Provider, ILogger
    {
        public override ILog logger
        {
            get
            {
                _logger = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
                return _logger;
            }
            set
            {
                _logger = value;
            }
        }

        private Dictionary<string, string> alarm_desc_list = null;
        public Dictionary<string, string> AlarmDescList
        {
            get
            {
                if (alarm_desc_list == null)
                {
                    getAlarmDescList();
                }
                return alarm_desc_list;
            }
        }
        private List<string> now_pat_list = new List<string>();
        public List<string> NowPatList
        {
            get
            {
                return now_pat_list;
            }
        }
        public string lastCheckTime { get; set; }
        public string InitTime { get; set; }
        /// <summary>
        /// 取得要顯示的警告文字清單比對用
        /// </summary>
        /// <returns></returns>
        private void getAlarmDescList()
        {
            string sql = "SELECT * FROM dbo.RCS_VIP_ALARM_DESC";
            DataTable dt = this.DBA.getSqlDataTable(sql);
            alarm_desc_list = dt.AsEnumerable().ToDictionary<DataRow, string, string>(x =>
                x["DEVICE_TYPE"].ToString() + "_" + x["ALARM_CODE"].ToString(), 
                x => x["ALARM_DESC"].ToString());
        }
        /// <summary>
        /// 取得要顯示的中央監控站病患，以VIP有資料的為主
        /// 再判斷是否有收案資料來顯示
        /// </summary>
        /// <returns></returns>
        public List<VentilatorStatus> getPatientList(bool isInit)
        {
            if (isInit) this.getAlarmDescList();
            //取1天前資料
            if (RCS.Controllers.BaseController.isDebuggerMode)
            {
                lastCheckTime = DateTime.Now.AddYears(-6).ToString("yyyyMMddHHmmssfff");
            }
            else
            {
                if (string.IsNullOrEmpty(lastCheckTime))
                    lastCheckTime = DateTime.Now.AddHours(-6).ToString("yyyyMMddHHmmssfff");
            }
            SQLProvider SQL = new SQLProvider("VIP_DbConnection");
            string sql = string.Format("SELECT patient_id FROM VIPRTTBL_CCH WHERE data_time >= '{0}' GROUP BY patient_id", lastCheckTime);
            DataTable dt = SQL.DBA.getSqlDataTable(sql);
            List<string> pat_list = dt.AsEnumerable().Select(x => x["patient_id"].ToString()).ToList();
            string _sql = @"SELECT PATIENT_NAME, CHART_NO, BED_NO, 
                            (SELECT TOP 1 A.ITEM_VALUE FROM {2} A JOIN {3} B ON A.RECORD_ID = B.RECORD_ID WHERE B.CHART_NO = '{0}' AND A.ITEM_NAME = 'respid' ORDER BY B.RECORDDATE DESC) respid 
                            FROM {1} WHERE CHART_NO = '{0}' ORDER BY CREATE_DATE DESC";
            List<VentilatorStatus> vr_list = new List<VentilatorStatus>();
            foreach (string pat_id in pat_list)
            {
                string SQLStr = string.Format(_sql, pat_id, GetTableName.RCS_RT_CASE, GetTableName.RCS_RECORD_DETAIL, GetTableName.RCS_RECORD_MASTER);
                dt = this.DBA.getSqlDataTable(SQLStr);
                if (dt != null && dt.Rows.Count > 0)
                {
                    DataRow thisrow = dt.Rows[0];
                    VentilatorStatus vs = new VentilatorStatus()
                    {
                        chart_no = thisrow["CHART_NO"].ToString().Trim(),
                        bedno = thisrow["BED_NO"].ToString().Trim(),
                        patient_name = thisrow["PATIENT_NAME"].ToString().Trim(),
                        device_no = thisrow["respid"].ToString().Trim(),
                        lastCheckTime = lastCheckTime,
                        isInit = true
                    };
                    vr_list.Add(vs);
                    if (!now_pat_list.Contains(vs.chart_no)) 
                        now_pat_list.Add(vs.chart_no);
                }
            }
            lastCheckTime = DateTime.Now.ToString("yyyyMMddHHmmssfff");
            if (isInit) InitTime = lastCheckTime;
            return vr_list;
        }

        public void getPatAlarmString(VentilatorStatus pVent)
        {
            bool checkgone = this.checkRemovePatient(pVent.chart_no);
            List<string> thisalert = new List<string>();
            if (checkgone)
            {
                pVent.isgone = true;
            }
            else
            {
                SQLProvider SQL = new SQLProvider("VIP_DbConnection");
                string sql = "SELECT device, data_time, central_message FROM VIPRTTBL_CCH where patient_id = '{0}' AND save_type = '3' AND data_time >= '{1}' ORDER BY data_time";
                sql = string.Format(sql, pVent.chart_no, pVent.lastCheckTime);
                DataTable dt = SQL.DBA.getSqlDataTable(sql);
                pVent.this_alarm = "";
                pVent.alarmList = pVent.alarmList.Where(x => x.Value == true)
                    .ToDictionary(x => x.Key, x => x.Value);
                if (dt != null && dt.Rows.Count > 0)
                {
                    foreach (DataRow ro in dt.Rows)
                    {
                        string device = ro["device"].ToString().Trim();
                        string msg = ro["central_message"].ToString().Trim();
                        pVent.device_type = device;
                        if (!string.IsNullOrEmpty(msg))
                        {
                            try
                            {
                                string[] alarm_list = msg.Split(',');
                                if (alarm_list.Length > 0)
                                {
                                    foreach (string alarm in alarm_list)
                                    {
                                        if (alarm.Trim() != "")
                                        {
                                            string[] alarm_data = alarm.Split('_');
                                            if (alarm_data.Length > 0)
                                            {
                                                string alarm_code = "";
                                                List<string> alarm_time = alarm_data[alarm_data.Length - 1].Split('~').ToList();
                                                alarm_time = alarm_time.Where(x => x.Trim() != "").ToList();
                                                for (int i = 0; i < alarm_data.Length - 1; i++)
                                                {
                                                    string s = alarm_data[i].Trim();
                                                    if (s != "")
                                                    {
                                                        if (alarm_code != "") alarm_code += "_";
                                                        alarm_code += alarm_data[i].Trim(); 
                                                    }

                                                }
                                                string key = pVent.device_type + "_" + alarm_code;
                                                if (alarm_time.Count == 1) //只有開始時間則表示alarm中，加入list
                                                {
                                                    if (!pVent.alarmList.Keys.Contains(alarm_code))
                                                    {
                                                        pVent.alarmList.Add(alarm_code, true);
                                                        if (!pVent.isInit)
                                                        {
                                                            if (pVent.this_alarm != "") pVent.this_alarm += ",";
                                                            if (AlarmDescList.ContainsKey(key))
                                                                pVent.this_alarm += AlarmDescList[key];
                                                            else
                                                                pVent.this_alarm += alarm_code;
                                                        }
                                                    }
                                                }
                                                if (alarm_time.Count == 2 && pVent.alarmList.Keys.Contains(alarm_code)) //有前後時間表示alarm結束，從list移除
                                                {
                                                    if (pVent.isInit)
                                                    {
                                                        pVent.alarmList.Remove(alarm_code);
                                                    }
                                                    else
                                                    {
                                                        if (pVent.this_alarm != "") pVent.this_alarm += ",";
                                                        if (AlarmDescList.ContainsKey(key))
                                                            pVent.this_alarm += string.Format("{0}{1}", AlarmDescList[key], "[end]");
                                                        else
                                                            pVent.this_alarm += string.Format("{0}{1}", alarm_code, "[end]");
                                                        pVent.alarmList[alarm_code] = false;
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                            catch { }
                        }
                    }
                }
                pVent.isInit = false;
                pVent.lastCheckTime = DateTime.Now.ToString("yyyyMMddHHmmssfff");
                if (pVent.this_alarm == "") pVent.this_alarm = "無";
                pVent.this_alarm_time = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                pVent.alarm_message = "";
                if (pVent.alarmList.Count > 0)
                {
                    foreach (string alarm_code in pVent.alarmList.Keys)
                    {
                        string key = pVent.device_type + "_" + alarm_code;
                        bool is_end = !pVent.alarmList[alarm_code];
                        if (pVent.alarm_message != "") pVent.alarm_message += ",";
                        if (AlarmDescList.ContainsKey(key))
                        {
                            pVent.alarm_message += is_end ? "<span class=\"patient-alert-end\">" + AlarmDescList[key] + "</span>" : AlarmDescList[key];
                        }
                        else
                        {//抓不到內容顯示代碼
                            pVent.alarm_message += alarm_code;
                        }
                    }
                }
                pVent.alarm_message = pVent.alarm_message == "" ? "<span class=\"patient-alert-end\">無</span>" : pVent.alarm_message;
            }
        }
        public bool checkRemovePatient(string pChartNo)
        {
            string sql = string.Format("SELECT ACCEPT_STATUS FROM {0} WHERE CHART_NO = '{1}' ORDER BY CREATE_DATE DESC", GetTableName.RCS_RT_CASE.ToString(), pChartNo);
            DataTable dt = this.DBA.getSqlDataTable(sql);
            if (dt != null && dt.Rows.Count > 0)
            {
                string status = dt.Rows[0][0].ToString();
                return status == "2";
            }
            return false;
        }
    }
}
using Com.Mayaminer;
using Dapper;
using log4net;
using mayaminer.com.library;
using Newtonsoft.Json;
using RCS_Data.Models;
using RCS_Data.Models.DB;
using RCS_Data.Models.ViewModels;
using RCS_Data.Models.VIP;
using RCSData.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using static Dapper.SqlMapper;
using RCS_Data.Controllers.RtRecord;
using System.Web;

namespace RCS_Data.Controllers.Shift
{
    public class  Models : BaseModels
    {
        string csName { get { return "Shift.Models"; } }

        public RESPONSE_MSG ShowShift(FormShowShift form)
        { 
            RESPONSE_MSG rm = new RESPONSE_MSG();
            rm.attachment = ((SHIFTViewModels)this.GetShiftData(new FormShift()
            {
                chart_no = form.chart_no,
                payload = form.payload,
                ISBAR_ID = ""
            }).attachment).model.ISBAR_ID; 
            return rm;
        }

        /// <summary>
        /// 取得交班表格式
        /// </summary>
        /// <param name="form"></param>
        /// <returns></returns>
        public virtual  RESPONSE_MSG GetShiftData(FormShift form)
        {
            RESPONSE_MSG rm = new RESPONSE_MSG();
            SHIFTViewModels vm = new SHIFTViewModels();
            DB_RCS_RT_ISBAR_SHIFT dbvm = new DB_RCS_RT_ISBAR_SHIFT();
            vm.patInfo = this.SelectPatientInfo("", form.chart_no);
            vm.cptData = this.getCPTAssess(form.chart_no);
            List<RCS_RT_ISBAR_SHIFT_S_VALUE> svlist = new List<RCS_RT_ISBAR_SHIFT_S_VALUE>();
            #region Introduction介紹(自我介紹與確認交班對象) 
            svlist.Add(new RCS_RT_ISBAR_SHIFT_S_VALUE() { name = "病歷號碼", data = vm.patInfo.chart_no });
            svlist.Add(new RCS_RT_ISBAR_SHIFT_S_VALUE() { name = "姓名", data = vm.patInfo.patient_name });
            svlist.Add(new RCS_RT_ISBAR_SHIFT_S_VALUE() { name = "性別", data = vm.patInfo.genderCHT });
            svlist.Add(new RCS_RT_ISBAR_SHIFT_S_VALUE() { name = "床號", data = vm.patInfo.bed_no });
            svlist.Add(new RCS_RT_ISBAR_SHIFT_S_VALUE() { name = "生日", data = vm.patInfo.birth_day });
            svlist.Add(new RCS_RT_ISBAR_SHIFT_S_VALUE() { name = "年齡", data = vm.patInfo.age.ToString() });
            svlist.Add(new RCS_RT_ISBAR_SHIFT_S_VALUE() { name = "入院日", data = vm.patInfo.diag_date });
            List<string> diag_desc_e = new List<string>();
            foreach (Diag dg in vm.patInfo.diag_list)
            {
                if (!string.IsNullOrWhiteSpace(dg.diag_desc_e))
                {
                    diag_desc_e.Add(dg.diag_desc_e);
                }
            }
            svlist.Add(new RCS_RT_ISBAR_SHIFT_S_VALUE() { name = "診斷", data = string.Join(",", diag_desc_e.ToArray()) });
            // 取得最後呼吸器狀態
            Ventilator last_rt_record = this.basicfunction.GetLastRTRec(vm.patInfo.chart_no, vm.patInfo.ipd_no);
            List<string> on_dateLIst = new List<string>();
            svlist.Add(new RCS_RT_ISBAR_SHIFT_S_VALUE() { name = "使用天數", data = this.basicfunction.getUseDays(vm.patInfo.ipd_no, vm.patInfo.chart_no, "", out on_dateLIst).ToString() });
            if (!string.IsNullOrWhiteSpace(last_rt_record.device_o2))
            {
                svlist.Add(new RCS_RT_ISBAR_SHIFT_S_VALUE() { name = "O2 equipment", data = last_rt_record.device_o2 });
            }//O2 equipment
            if (!string.IsNullOrWhiteSpace(last_rt_record.mode))
            {
                svlist.Add(new RCS_RT_ISBAR_SHIFT_S_VALUE() { name = "mode", data = last_rt_record.mode });
            }//mode
            if (!string.IsNullOrWhiteSpace(last_rt_record.fio2_set))
            {
                svlist.Add(new RCS_RT_ISBAR_SHIFT_S_VALUE() { name = "FiO2", data = last_rt_record.fio2_set });
            }//FiO2
            if (!string.IsNullOrWhiteSpace(last_rt_record.vr_set))
            {
                svlist.Add(new RCS_RT_ISBAR_SHIFT_S_VALUE() { name = "Ventilation rate set", data = last_rt_record.vr_set });
            }//vr_set
            if (!string.IsNullOrWhiteSpace(last_rt_record.fio2_measured))
            {
                svlist.Add(new RCS_RT_ISBAR_SHIFT_S_VALUE() { name = "Measured", data = last_rt_record.fio2_measured });
            }//Measured
            if (!string.IsNullOrWhiteSpace(last_rt_record.o2flow))
            {
                svlist.Add(new RCS_RT_ISBAR_SHIFT_S_VALUE() { name = "O2 flow", data = last_rt_record.o2flow });
            }//O2 flow
            if (!string.IsNullOrWhiteSpace(last_rt_record.mv_set))
            {
                svlist.Add(new RCS_RT_ISBAR_SHIFT_S_VALUE() { name = "MV", data = last_rt_record.mv_set });
            }//MV
            if (!string.IsNullOrWhiteSpace(last_rt_record.mv_percent))
            {
                svlist.Add(new RCS_RT_ISBAR_SHIFT_S_VALUE() { name = "MV %", data = last_rt_record.mv_percent });
            }//MV %
            if (!string.IsNullOrWhiteSpace(last_rt_record.vt_set))
            {
                svlist.Add(new RCS_RT_ISBAR_SHIFT_S_VALUE() { name = "TV", data = last_rt_record.vt_set });
            }//TV
            if (!string.IsNullOrWhiteSpace(last_rt_record.pressure_pc))
            {
                svlist.Add(new RCS_RT_ISBAR_SHIFT_S_VALUE() { name = "PC", data = last_rt_record.pressure_pc });
            }//PC
            if (!string.IsNullOrWhiteSpace(last_rt_record.pressure_ps))
            {
                svlist.Add(new RCS_RT_ISBAR_SHIFT_S_VALUE() { name = "PS", data = last_rt_record.pressure_ps });
            }//PS
            if (!string.IsNullOrWhiteSpace(last_rt_record.pressure_peep))
            {
                svlist.Add(new RCS_RT_ISBAR_SHIFT_S_VALUE() { name = "PEEP", data = last_rt_record.pressure_peep });
            }//PEEP
            if (!string.IsNullOrWhiteSpace(last_rt_record.flow))
            {
                svlist.Add(new RCS_RT_ISBAR_SHIFT_S_VALUE() { name = "flow", data = last_rt_record.flow });
            }//flow
            if (!string.IsNullOrWhiteSpace(last_rt_record.gcse) || !string.IsNullOrWhiteSpace(last_rt_record.gcsm) || !string.IsNullOrWhiteSpace(last_rt_record.gcsv))
            {
                svlist.Add(new RCS_RT_ISBAR_SHIFT_S_VALUE() { name = "GCS", data = last_rt_record.gcse + "/" + last_rt_record.gcsv + "/" + last_rt_record.gcsm });
            }//GCS：E/M/V
            if (!string.IsNullOrWhiteSpace(last_rt_record.conscious))
            {
                svlist.Add(new RCS_RT_ISBAR_SHIFT_S_VALUE() { name = "意識", data = last_rt_record.conscious });
            }//vr_set 
            if (!string.IsNullOrWhiteSpace(last_rt_record.insp_time))
            {
                svlist.Add(new RCS_RT_ISBAR_SHIFT_S_VALUE() { name = "Ti", data = last_rt_record.insp_time });
            }//insp_time
            if (!string.IsNullOrWhiteSpace(last_rt_record.thigh) || !string.IsNullOrWhiteSpace(last_rt_record.tlow))
            {
                svlist.Add(new RCS_RT_ISBAR_SHIFT_S_VALUE() { name = "Time High/Low", data = last_rt_record.thigh + "/" + last_rt_record.tlow });
            }//thigh/tlow
            if (!string.IsNullOrWhiteSpace(last_rt_record.ipap) || !string.IsNullOrWhiteSpace(last_rt_record.epap))
            {
                svlist.Add(new RCS_RT_ISBAR_SHIFT_S_VALUE() { name = "IPAP / EPAP", data = last_rt_record.ipap + "/" + last_rt_record.epap });
            }//ipap/epap            
            if (!string.IsNullOrWhiteSpace(last_rt_record.phigh) || !string.IsNullOrWhiteSpace(last_rt_record.plow))
            {
                svlist.Add(new RCS_RT_ISBAR_SHIFT_S_VALUE() { name = "Pressure(High / Low)", data = last_rt_record.phigh + "/" + last_rt_record.plow });
            }//phigh/plow
            if (!string.IsNullOrWhiteSpace(last_rt_record.delta_p) || !string.IsNullOrWhiteSpace(last_rt_record.hfov_hz))
            {
                svlist.Add(new RCS_RT_ISBAR_SHIFT_S_VALUE() { name = "△ P / HFOV Hz", data = last_rt_record.delta_p + "/" + last_rt_record.hfov_hz });
            }//delta_p/hfov_hz
            if (!string.IsNullOrWhiteSpace(last_rt_record.hfov_mean))
            {
                svlist.Add(new RCS_RT_ISBAR_SHIFT_S_VALUE() { name = "HFOV mean(cmH2O)", data = last_rt_record.hfov_mean });
            }//hfov_mean
            if (!string.IsNullOrWhiteSpace(last_rt_record.optimal_peep))
            {
                svlist.Add(new RCS_RT_ISBAR_SHIFT_S_VALUE() { name = "Optimal PEEP", data = last_rt_record.optimal_peep });
            }//optimal_peep
            #endregion
            // 將病人及取得最後呼吸器狀態加入S_VALUE
            vm.S_VALUE = svlist.FindAll(x => !(string.IsNullOrWhiteSpace(x.data) || x.data == "-"));
            form.ISBAR_ID = this.getISBAR_ID(ref rm, form.chart_no);
            if (string.IsNullOrWhiteSpace(form.ISBAR_ID))
            {
                dbvm.ISBAR_ID = this.DBLink.GetFixedStrSerialNumber(form.user_info.user_id);
                #region dbvm  
                dbvm.S_VALUE = Newtonsoft.Json.JsonConvert.SerializeObject(vm.S_VALUE);
                dbvm.IPD_NO = vm.patInfo.ipd_no;
                dbvm.CHART_NO = vm.patInfo.chart_no;
                dbvm.CREATE_ID = form.user_info.user_id;
                dbvm.CREATE_NAME = form.user_info.user_name;
                dbvm.CREATE_DATE = RCS_Data.Models.Function_Library.getDateNowString(RCS_Data.Models.DATE_FORMAT.yyyy_MM_dd_HHmmss);
                dbvm.MODIFY_ID = form.user_info.user_id;
                dbvm.MODIFY_NAME = form.user_info.user_name;
                dbvm.MODIFY_DATE = RCS_Data.Models.Function_Library.getDateNowString(RCS_Data.Models.DATE_FORMAT.yyyy_MM_dd_HHmmss);
                dbvm.DATASTATUS = "1";
                #endregion
                this.DBLink.DBA.DBExecInsert<DB_RCS_RT_ISBAR_SHIFT>(new List<DB_RCS_RT_ISBAR_SHIFT>() { dbvm });
            }
            else
            {
                dbvm.ISBAR_ID = form.ISBAR_ID;
                dbvm = this.dbShiftData(dbvm.ISBAR_ID);
                // this.DBLink.DBA.DBExecUpdate<DB_RCS_RT_ISBAR_SHIFT>(new List<DB_RCS_RT_ISBAR_SHIFT>() { dbvm });
            }
            if (this.DBLink.DBA.hasLastError)
            {
                Com.Mayaminer.LogTool.SaveLogMessage(this.DBLink.DBA.lastError, "GetShiftData", this.csName);
                rm.message = "取得交班表格式失敗!";
            }
            dbvm.I_VALUE = string.Format("交班記錄人員：{0}　主治醫師：{1}", form.user_info.user_name, vm.patInfo.vs_doc);
            vm.model = dbvm;
            rm.attachment = vm;
            return rm;
        }

        /// <summary>
        /// 暫存交班表
        /// </summary>
        /// <param name="form"></param>
        /// <returns></returns>
        public virtual RESPONSE_MSG<string> SaveShiftData(FormSHIFTModels form)
        {
            RESPONSE_MSG<string> rm = new RESPONSE_MSG<string>();
            DB_RCS_RT_ISBAR_SHIFT dbvm = new DB_RCS_RT_ISBAR_SHIFT();
            DB_RCS_RT_ISBAR_SHIFT dbTemp = new DB_RCS_RT_ISBAR_SHIFT();
            dbvm = this.dbShiftData(form.model.ISBAR_ID);

            #region dbTemp
            dbTemp = JsonConvert.DeserializeObject<DB_RCS_RT_ISBAR_SHIFT>(JsonConvert.SerializeObject(dbvm));
            dbTemp.ISBAR_ID = this.DBLink.GetFixedStrSerialNumber(form.user_info.user_id);
            dbTemp.CREATE_ID = form.user_info.user_id;
            dbTemp.CREATE_NAME = form.user_info.user_name;
            dbTemp.CREATE_DATE = RCS_Data.Models.Function_Library.getDateNowString(RCS_Data.Models.DATE_FORMAT.yyyy_MM_dd_HHmmss);
            dbTemp.MODIFY_ID = form.user_info.user_id;
            dbTemp.MODIFY_NAME = form.user_info.user_name;
            dbTemp.MODIFY_DATE = RCS_Data.Models.Function_Library.getDateNowString(RCS_Data.Models.DATE_FORMAT.yyyy_MM_dd_HHmmss);
            dbTemp.DATASTATUS = "2";
            #endregion  

            #region dbvm
            dbvm.I_VALUE = form.model.I_VALUE;
            dbvm.S_VALUE = Newtonsoft.Json.JsonConvert.SerializeObject(form.S_VALUE);
            dbvm.IPD_NO = form.patInfo.ipd_no;
            dbvm.CHART_NO = form.patInfo.chart_no;
            dbvm.MODIFY_ID = form.user_info.user_id;
            dbvm.MODIFY_NAME = form.user_info.user_name;
            dbvm.MODIFY_DATE = RCS_Data.Models.Function_Library.getDateNowString(RCS_Data.Models.DATE_FORMAT.yyyy_MM_dd_HHmmss);
            dbvm.LIST_MEMO = form.model.LIST_MEMO;
            dbvm.HIS_MEMO = form.model.HIS_MEMO; 
            dbvm.HIS_DATA = form.model.HIS_DATA;
            dbvm.B_VALUE_1 = form.model.B_VALUE_1;
            dbvm.B_VALUE_2 = form.model.B_VALUE_2;
            #endregion
             
            this.DBLink.DBA.DBExecUpdate<DB_RCS_RT_ISBAR_SHIFT>(new List<DB_RCS_RT_ISBAR_SHIFT>() { dbvm });
            this.DBLink.DBA.DBExecInsert<DB_RCS_RT_ISBAR_SHIFT>(new List<DB_RCS_RT_ISBAR_SHIFT>() { dbTemp });

            if (this.DBLink.DBA.hasLastError)
            {
                Com.Mayaminer.LogTool.SaveLogMessage(this.DBLink.DBA.lastError, "SaveShiftData", this.csName);
                rm.message = "儲存失敗!";
            }
            rm.attachment = "儲存成功!";
            return rm;
        }

        public virtual RESPONSE_MSG uploadHISData(FormHISData form, IWebServiceParam pIwp)
        {
            RESPONSE_MSG rm = new RESPONSE_MSG();
            rm.attachment = "上傳成功!";
            return rm;
        }
        public virtual RESPONSE_MSG uploadHISData(FormHISData form)
        {
            RESPONSE_MSG rm = new RESPONSE_MSG();
            rm.attachment = "上傳成功!";
            return rm;
        }

        public RESPONSE_MSG SaveShiftHandOver(FormShiftHandOver form)
        {
            string actionName = "SaveShiftHandOver";
            RESPONSE_MSG rm = new RESPONSE_MSG();
            List<IPDPatientInfo> dataList = new List<IPDPatientInfo>(); 
            List<DB_RCS_RT_CARE_SCHEDULING> rtList = new List<DB_RCS_RT_CARE_SCHEDULING>();
            if (string.IsNullOrWhiteSpace(form.shift_user_id))
            {
                rm.status = RESPONSE_STATUS.ERROR;
                rm.messageList.Add("請選擇接班人!");
            }else
            {
                form.dataList = HttpUtility.UrlDecode(form.dataList);
                dataList = JsonConvert.DeserializeObject<List<IPDPatientInfo>>(form.dataList);
                // r_id
                List<PatientListItem> pList = new List<PatientListItem>();
                pList = this.getUserCareList(form.shift_user_id);
                foreach (var item in dataList)
                {
                    if (!pList.Exists(x=>x.ipd_no == item.ipd_no && x.chart_no == item.chart_no))
                    {
                        // 加入照護清單
                        rtList.Add(this.getDB_RCS_RT_CARE_SCHEDULING(form.shift_user_id, item, item.diag_date));
                    } 
                }
                if (rtList.Count > 0)
                {
                    this.DBLink.DBA.DBExecInsert<DB_RCS_RT_CARE_SCHEDULING>(rtList); 
                }
                this.DBLink.DBA.DBExecDelete<DB_RCS_RT_CARE_SCHEDULING>(dataList.Select(x => new DB_RCS_RT_CARE_SCHEDULING() { CARE_ID = x.r_id }).ToList());
                if (this.DBLink.DBA.hasLastError)
                {
                    rm.status = RESPONSE_STATUS.ERROR;
                    rm.messageList.Add("交班失敗!");
                    Com.Mayaminer.LogTool.SaveLogMessage(this.DBLink.DBA.lastError, actionName, this.csName);
                }
            } 
            if (!this.DBLink.DBA.hasLastError) rm.attachment = "交班成功!";
            return rm;
        }

        /// <summary>
        /// dbShiftData
        /// </summary>
        /// <param name="ISBAR_ID"></param>
        /// <param name="IPD_NO"></param>
        /// <returns></returns>
        protected DB_RCS_RT_ISBAR_SHIFT dbShiftData(string ISBAR_ID)
        {
            List<DB_RCS_RT_ISBAR_SHIFT> pList = new List<DB_RCS_RT_ISBAR_SHIFT>();
            Dapper.DynamicParameters dp = new Dapper.DynamicParameters();
            string query = "SELECT * FROM " + DB_TABLE_NAME.DB_RCS_RT_ISBAR_SHIFT + " WHERE 1=1";
            if (!string.IsNullOrWhiteSpace(ISBAR_ID))
            {
                query += " AND ISBAR_ID =@ISBAR_ID";
            }
            dp.Add("ISBAR_ID", ISBAR_ID);
            pList = this.DBLink.DBA.getSqlDataTable<DB_RCS_RT_ISBAR_SHIFT>(query, dp);
            if (this.DBLink.DBA.hasLastError)
            {
                Com.Mayaminer.LogTool.SaveLogMessage(this.DBLink.DBA.lastError, "dbShiftData", this.csName);
            }
            if (pList.Count > 0)
            {
                return pList[0];
            }
            return new DB_RCS_RT_ISBAR_SHIFT();
        }

        /// <summary>
        /// 取得交班流水號
        /// </summary>
        /// <param name="rm"></param>
        /// <param name="chart_no"></param>
        /// <returns></returns>
        protected string getISBAR_ID(ref RESPONSE_MSG rm, string chart_no)
        {
            Dapper.DynamicParameters dp = new Dapper.DynamicParameters();
            string query = "SELECT ISBAR_ID FROM " + DB_TABLE_NAME.DB_RCS_RT_ISBAR_SHIFT + " WHERE 1=1 AND DATASTATUS = '1' AND CHART_NO =@CHART_NO ORDER BY CREATE_DATE DESC";
            dp.Add("CHART_NO", chart_no);
            List<string> pList = this.DBLink.DBA.getSqlDataTable<string>(query, dp);
            if (this.DBLink.DBA.hasLastError)
            {
                Com.Mayaminer.LogTool.SaveLogMessage(this.DBLink.DBA.lastError, "ShowShift", this.csName);
                rm.message = "取得交班表格式失敗!";
            }
            if (pList.Count > 0)
            {
                return pList[0];
            }
            else
            {
                return "";
            }
        }

        public virtual void getShiftLIST_MEMO(ref List<PatientListItem> pList)
        {
             
        }

        protected void uploadHISDataTemp(string PGUID, DB_RCS_RT_ISBAR_SHIFT dbvm, UserInfo user_info)
        {
            string actionName = "uploadHISDataTemp";
            #region 上傳註記  
            dbvm.ISBAR_ID = this.DBLink.GetFixedStrSerialNumber(user_info.user_id);
            dbvm.CREATE_ID = user_info.user_id;
            dbvm.CREATE_NAME = user_info.user_name;
            dbvm.CREATE_DATE = RCS_Data.Models.Function_Library.getDateNowString(RCS_Data.Models.DATE_FORMAT.yyyy_MM_dd_HHmmss);
            dbvm.MODIFY_ID = user_info.user_id;
            dbvm.MODIFY_NAME = user_info.user_name;
            dbvm.MODIFY_DATE = RCS_Data.Models.Function_Library.getDateNowString(RCS_Data.Models.DATE_FORMAT.yyyy_MM_dd_HHmmss);
            dbvm.DATASTATUS = "3";
            dbvm.UPLOAD_DATE = Function_Library.getDateNowString(DATE_FORMAT.yyyy_MM_dd_HHmmss);
            dbvm.UPLOAD_NAME = user_info.user_name;
            dbvm.UPLOAD_ID = user_info.user_id;
            dbvm.UPLOAD_PGUID = PGUID;
            dbvm.UPLOAD_STATUS = "1";

            #endregion
            this.DBLink.DBA.DBExecInsert<DB_RCS_RT_ISBAR_SHIFT>(new List<DB_RCS_RT_ISBAR_SHIFT>() { dbvm });
            if (this.DBLink.DBA.hasLastError)
            { 
                Com.Mayaminer.LogTool.SaveLogMessage(this.DBLink.DBA.lastError, actionName, this.csName);
            }
        }
    }

 
    public class FormShiftList : AUTH
    {
        public string list_json { get; set; }
    }

    public class FormShift   
    {
        public string index { get; set; } 
        public string ISBAR_ID { get; set; }
        public string chart_no { get; set; }
        /// <summary>
        /// 登入者資料
        /// </summary>
        public PAYLOAD payload { get; set; }

        /// <summary>
        /// 使用者資料
        /// </summary>
        public UserInfo user_info
        {
            get
            {
                return new UserInfo()
                {
                    user_id = this.payload.user_id,
                    user_name = this.payload.user_name,
                    authority = this.payload.role,
                };

            }

        }
    }

    public class FormSHIFTModels : AUTH
    {
        public IPDPatientInfo patInfo { get; set; }
        public RCS_CPT_DTL_NEW_ITEMS cptData { get; set; }
        public DB_RCS_RT_ISBAR_SHIFT model { get; set; }
        /// <summary>
        /// Introduction介紹(自我介紹與確認交班對象)
        /// </summary>
        public List<RCS_RT_ISBAR_SHIFT_S_VALUE> S_VALUE { get; set; }
    }

    public class FormShowShift : AUTH
    { 
        public string ipd_no { get; set; }
        public string chart_no { get; set; }
    }
    public class FormHISData : AUTH
    {
        public string pUser { get; set; }
        public string pUserPWD { get; set; }
        public string pFee_NO { get; set; }
        public string pRecordMsg { get; set; }
        public string pReceiveDept { get; set; }
        public string pMessageType { get; set; }
        public string pPrecorddatetime { get; set; }
        public string shift_id { get; set; }

    }

    public class FormShiftHandOver : AUTH
    {
        public string shift_user_id { get; set; }
        public string dataList { get; set; }
    }
}

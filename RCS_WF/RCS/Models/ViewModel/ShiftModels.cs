using Newtonsoft.Json;
using RCS_Data;
using RCS_Data.Controllers.Shift;
using RCS_Data.Models;
using RCS_Data.Models.DB;
using RCS_Data.Models.ViewModels;
using RCSData.Models;
using System;
using System.Collections.Generic;


namespace RCS.Models.ViewModel
{
    public class ShiftModels : RCS_Data.Controllers.Shift.Models
    {
        string csName { get { return "ShiftModels"; } }

        RtRecord _model;
        RtRecord rtRecordModel
        {
            get
            {
                if (_model == null)
                {
                    this._model = new RtRecord();
                }
                return this._model;
            }
        }


        public override RESPONSE_MSG uploadHISData(FormHISData form)
        {
            string actionName = "uploadHISData";
            RESPONSE_MSG rm = new RESPONSE_MSG();
            List<DB_TCRecord> list = new List<DB_TCRecord>();
            DB_TCRecord record = new DB_TCRecord();
            DB_RCS_RT_ISBAR_SHIFT dbvm = new DB_RCS_RT_ISBAR_SHIFT();
            dbvm = this.dbShiftData(form.shift_id);
 

            #region MyRegion
            record.PGUID = string.Concat("w-",form.pFee_NO,"-",form.user_info.user_id,"-", Function_Library.getDateNowString( RCS_Data.Models.DATE_FORMAT.yyyyMMddHHmmssfffff));
            record.RefPatient = form.pFee_NO;
            record.RecordDate = Function_Library.getDateNowString( DATE_FORMAT.yyyy_MM_dd_withSlash);
            record.RecordTime = Function_Library.getDateNowString(DATE_FORMAT.HHmm);
            record.RecordContent = dbvm.HIS_DATA;
            record.TeamGroupID = "0320";
            record.Creator = form.user_info.user_id;
            record.CreatorName = form.user_info.user_name;
            record.CreateTime = Function_Library.getDateNowString(DATE_FORMAT.yyyy_MM_dd_HHmmss_withSlash);
            record.Editor = form.user_info.user_id;
            record.EditorName = form.user_info.user_name;
            record.EditTime = Function_Library.getDateNowString(DATE_FORMAT.yyyy_MM_dd_HHmmss_withSlash);

            #endregion
            list.Add(record);
            SQLProvider dba = new SQLProvider("TCRecordConnection"); 
            dba.DBA.DBExecInsert<DB_TCRecord>(list); 
           
            if (dba.DBA.hasLastError)
            {
                rm.status = RESPONSE_STATUS.ERROR;
                Com.Mayaminer.LogTool.SaveLogMessage(dba.DBA.lastError, actionName, this.csName);
            }
            else
            {
                this.uploadHISDataTemp(record.PGUID, dbvm, form.user_info); 
            }
            rm.attachment = "上傳成功!";
            return rm;
        }

        /// <summary>
        /// 取得交班表格式
        /// </summary>
        /// <param name="form"></param>
        /// <returns></returns>
        public override RESPONSE_MSG GetShiftData(FormShift form)
        { 
            RESPONSE_MSG rm = new RESPONSE_MSG();
            rm = base.GetShiftData(form);
            SHIFTViewModels vm = (SHIFTViewModels)rm.attachment;
            vm.HIS_DATA_UPLOAD_List = new List<string>();
            if (rm.status == RESPONSE_STATUS.SUCCESS)
            {
                DB_RCS_WEANING_ASSESS_CHECKLIST rwca = this.rtRecordModel.getRWCA(vm.patInfo.chart_no, vm.patInfo.ipd_no, DateTime.Now.ToString("yyyy-MM-dd")).attachment;
                Ventilator mv = this.basicfunction.GetLastRTRec(vm.patInfo.chart_no, vm.patInfo.ipd_no);
                //上傳交班內容修改
                vm.HIS_DATA_UPLOAD_List.Add(string.Concat("診斷：", vm.patInfo.diagnosis_code));
                vm.HIS_DATA_UPLOAD_List.Add(string.Concat("目前呼吸器使用模式：", mv.mode));
                vm.HIS_DATA_UPLOAD_List.Add(string.Concat("每日呼吸器脫離評估－無法進行呼吸器脫離原因："));
                vm.HIS_DATA_UPLOAD_List.Add(string.Concat("(1) 呼吸衰竭誘發因素無法被控制？", 
                    !string.IsNullOrWhiteSpace(rwca.RWAC01) && rwca.RWAC01 == "1" ? "是" :"否" ));
                vm.HIS_DATA_UPLOAD_List.Add(string.Concat("(2) 生命徵象及整體狀況不穩定？",
                    !string.IsNullOrWhiteSpace(rwca.RWAC02) && rwca.RWAC02 == "1" ? "是" : "否"));
                vm.HIS_DATA_UPLOAD_List.Add(string.Concat("(3) MAP < 65 mmHg, SBP < 90 或 > 180 mmHg？",
                    !string.IsNullOrWhiteSpace(rwca.RWAC03) && rwca.RWAC03 == "1" ? "是" : "否"));
                vm.HIS_DATA_UPLOAD_List.Add(string.Concat("(4) 新的心律不整？",
                    !string.IsNullOrWhiteSpace(rwca.RWAC04) && rwca.RWAC04 == "1" ? "是" : "否"));
                vm.HIS_DATA_UPLOAD_List.Add(string.Concat("(5) 24 小時內有急性冠心症？",
                    !string.IsNullOrWhiteSpace(rwca.RWAC05) && rwca.RWAC05 == "1" ? "是" : "否"));
                vm.HIS_DATA_UPLOAD_List.Add(string.Concat("(6) 顱內壓 > 20 mmHg？",
                    !string.IsNullOrWhiteSpace(rwca.RWAC06) && rwca.RWAC06 == "1" ? "是" : "否"));
                vm.HIS_DATA_UPLOAD_List.Add(string.Concat("(7) FiO2 ≧ 50% 或 PEEP > 8cmH2O 或 SPO2 ≦ 90%？",
                    !string.IsNullOrWhiteSpace(rwca.RWAC07) && rwca.RWAC07 == "1" ? "是" : "否"));
                string tempStr = "";
                #region RWAC08
                switch (rwca.RWAC08)
                {
                    case "1":
                        tempStr = "Try invasive MV weaning mode";
                        break;
                    case "2":
                        tempStr = "SBT";
                        break;
                    case "3":
                        tempStr = "Try off NIV with oxygen therapy";
                        break;
                    case "4":
                        tempStr = "Keep MV Support";
                        break;
                    case "5":
                        tempStr = "Keep NIV Support";
                        break;
                    case "6":
                        tempStr = "MV-Dependent Patient";
                        break;
                    case "7":
                        tempStr = "NIV-Dependent Patient";
                        break;
                    default:
                        break;
                }
                vm.HIS_DATA_UPLOAD_List.Add(string.Concat("今日呼吸器脫離計畫：",
                    !string.IsNullOrWhiteSpace(tempStr) ? tempStr : ""));
                #endregion 
            }
            vm.model.HIS_DATA = string.Join(" ,", vm.HIS_DATA_UPLOAD_List);
            return rm;
        }

        /// <summary>
        /// 暫存交班表
        /// </summary>
        /// <param name="form"></param>
        /// <returns></returns>
        public override RESPONSE_MSG<string> SaveShiftData(FormSHIFTModels form)
        {
            RESPONSE_MSG<string> rm = new RESPONSE_MSG<string>();
            List<string> temp_memo = new List<string>();
            temp_memo.Add("呼吸治療師迴診");
            temp_memo.Add(form.model.HIS_DATA);
            temp_memo.Add(string.Concat("備註:", form.model.HIS_MEMO)); 
            temp_memo.Add(string.Concat("呼吸治療師：", form.user_info.user_name));
            temp_memo.Add(string.Concat("記錄時間：:", Function_Library.getDateNowString(DATE_FORMAT.yyyy_MM_dd_HHmm)));
            form.model.HIS_DATA = string.Join(",", temp_memo);
            return base.SaveShiftData(form);
        }
    }
}
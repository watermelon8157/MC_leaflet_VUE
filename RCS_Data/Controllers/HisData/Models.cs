using Com.Mayaminer;
using mayaminer.com.library;
using RCS_Data.Controllers.RtRecord;
using RCS_Data.Models;
using RCS_Data.Models.DB;
using RCS_Data.Models.ViewModels;
using RCSData.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text.RegularExpressions;

namespace RCS_Data.Controllers.HisData
{
    public partial class Models : BaseModels,RCS_Data.Controllers.HisData.Interface
    { 
        string csName { get { return "HisData.Models"; } }


        /// <summary>
        /// 取得IO資料
        /// </summary>
        /// <param name="finalIO_PUTList"></param>
        /// <param name="io_sdate"></param>
        /// <param name="io_edate"></param>
        /// <param name="pat_info"></param>
        /// <param name="iwp"></param>
        /// <returns></returns>
        public RESPONSE_MSG Search_io(ref List<IO_PUT> finalIO_PUTList, string io_sdate, string io_edate, IPDPatientInfo pat_info, IWebServiceParam iwp)
        {
            RESPONSE_MSG rm = new RESPONSE_MSG();

            string actionName = "search_io";
            List<IO_PUT> DataList = new List<IO_PUT>(); //從getInputOutput取出的輸入輸出資料
            List<IO_PUT> RangeDataList = new List<IO_PUT>(); //從getInputOutput取出區間內的輸入輸出資料
            List<IO_PUT> DetailDataList = new List<IO_PUT>(); //從getInputOutput取出區間內的詳細輸入輸出資料
            List<string> RangeDateList = new List<string>(); //日期區間List
            List<string> TempSplitlist = new List<string>();
            float I_Result = 0;
            float O_Result = 0;
            bool has_I_Result = false;
            bool has_O_Result = false;
            try
            {
                DataList = webmethod.getInputOutput(iwp, pat_info.chart_no, pat_info.ipd_no, io_sdate, DateTime.Parse(io_edate).AddDays(+1).ToString("yyyy-MM-dd"));//取得HIS Data
                DataList = DataList.OrderByDescending(x => x.EXAM_DATE).ThenBy(x => x.IO_TYPE).ToList();//排序從getInputOutput取出區間內的輸入輸出資料，依據日期由新到舊

                // 第一步：先從getInputOutput取出區間內的輸入輸出資料
                for (int ii = 0; ii < DataList.Count(); ii++)
                {
                    if (DateTime.Parse(DataList[ii].EXAM_DATE) > DateTime.Parse(io_sdate)
                        && DateTime.Parse(DataList[ii].EXAM_DATE) <= DateTime.Parse(io_edate).AddDays(+1)) // 單一筆HIS Data(DataList)是否介於選擇的開始、結束日期內
                    {
                        // 如果符合將資料(DataList[ii])存入RangeDataList
                        RangeDataList.Add(DataList[ii]);
                    }
                }

                //第二步：準備日期區間List
                foreach (var item in RangeDataList)
                {
                    string[] strArr = Regex.Split(item.EXAM_DATE, " ", RegexOptions.IgnoreCase); // 將item.EXAM_DATE資料拆開存入strArr
                    TempSplitlist.Add(strArr[0]);//只取strArr中index為0的資料存入TempSplitlist
                }
                TempSplitlist = (from cr in TempSplitlist select cr).Distinct().ToList(); //Distinct TempSplitlist中的資料

                //第三步：將區間內的輸入輸出資料放到對應的日期區間List中
                foreach (var itemRangeDate in TempSplitlist)
                {
                    for (int hh = 0; hh < RangeDataList.Count(); hh++)
                    {
                        if (DateTime.Parse(RangeDataList[hh].EXAM_DATE) > DateTime.Parse(itemRangeDate)
                        && DateTime.Parse(RangeDataList[hh].EXAM_DATE) <= DateTime.Parse(itemRangeDate).AddDays(+1)
                        && RangeDataList[hh].IO_TYPE == "I")
                        {
                            has_I_Result = true;
                            I_Result = I_Result + float.Parse(RangeDataList[hh].RESULT);
                            DetailDataList.Add(RangeDataList[hh]);
                        }

                        if (DateTime.Parse(RangeDataList[hh].EXAM_DATE) > DateTime.Parse(itemRangeDate)
                        && DateTime.Parse(RangeDataList[hh].EXAM_DATE) <= DateTime.Parse(itemRangeDate).AddDays(+1)
                        && RangeDataList[hh].IO_TYPE == "O")
                        {
                            has_O_Result = true;
                            O_Result = O_Result + float.Parse(RangeDataList[hh].RESULT);
                            DetailDataList.Add(RangeDataList[hh]);
                        }
                    }//for (int hh = 0; hh < RangeDataList.Count(); hh++)

                    if (has_I_Result || has_O_Result)
                    {
                        IO_PUT _item = new IO_PUT(); // 產生新的_item空間存放資料
                        _item.EXAM_DATE = itemRangeDate; // 只取strArr中index為0的資料存入_item.EXAM_DATE
                        _item.I_RESULT = I_Result.ToString(); // 加總過的輸入數量(I_Result)存入_item.I_RESULT
                        _item.O_RESULT = O_Result.ToString(); // 加總過的輸出數量(O_Result)存入_item.O_RESULT
                        float numVal = float.Parse(_item.I_RESULT) - float.Parse(_item.O_RESULT);
                        _item.RESULT_DIFF = numVal.ToString(); // 加總過的輸入數量(I_Result) - 加總過的輸出數量(O_Result)存入_item.RESULT_DIFF

                        _item.IO_List = DetailDataList;
                        finalIO_PUTList.Add(_item); // 將單筆輸入輸出資料(_item)存入finalIO_PUTList
                        I_Result = 0;
                        O_Result = 0;
                        DetailDataList = new List<IO_PUT>();
                    }
                    has_I_Result = false;
                    has_O_Result = false;
                }

            }//try
            catch (Exception ex)
            {
                LogTool.SaveLogMessage(ex, actionName, this.csName);
            }


            return rm;
        }

        /// <summary>
        /// 取得VS資料
        /// </summary>
        /// <returns></returns>
        public RESPONSE_MSG Search_vs(ref List<VitalSign> returnVitalSignList, IWebServiceParam iwp, string pChartNo, string pIpdNo, string pSDate, string pEDate = "")
        {
            RESPONSE_MSGLIST rm = new RESPONSE_MSGLIST();

            returnVitalSignList = webmethod.getVitalSign(iwp, pChartNo, pIpdNo, pSDate, pEDate);
            return rm;
        }

        /// <summary>
        /// 取得檢驗資料
        /// </summary>
        /// <returns></returns>
        public RESPONSE_MSG Exam_Data(ref ExamView ExamViewData, string pSDate, string pEDate, IPDPatientInfo pat_info, IWebServiceParam iwp)
        {
            string actionName = "Exam_Data";
            ExamView ExamView = ExamViewData;
            ExamView.RESPONSE_MSG = new RESPONSE_MSG();
            ExamView.List = new List<ExamViewList>();
            ExamView.thHeadList = new List<RCS_ExamData_Common>();
            RESPONSE_MSGLIST rm = new RESPONSE_MSGLIST();
            try
            {
                //取的資料Lab資料
                List<RCS_ExamData_Common> labData = webmethod.getLabData(iwp, pat_info.chart_no, pat_info.ipd_no, pSDate, pEDate);
                //取得檢驗日期StringList(Distinct)
                List<string> dateList = labData.Select(x => x.exam_date).Distinct().ToList();
                dateList = dateList.OrderByDescending(x => DateTime.Parse(x)).ToList();
                //取得Lab資料的檢驗項目(GroupBy)
                ExamView.thHeadList = labData.GroupBy(x => new { x.name, x.spem, x.unit }).Select(y => new RCS_ExamData_Common()
                {
                    name = y.Key.name,
                    spem = y.Key.spem,
                    unit = y.Key.unit
                }).ToList();
                ExamView.thHeadList = setSortList(ExamView.thHeadList);
                //開始整理資料啦
                foreach (string date in dateList)
                {
                    ExamViewList view = new ExamViewList();
                    view.exam_date = date;//設定報告日期
                    view.dataList = ExamView.thHeadList.ConvertAll(x => new RCS_ExamData_Common()
                    {
                        name = x.name,
                        spem = x.spem,
                        unit = x.unit
                    });//取的所有檢驗項目(上下限為空值，暫時不取得)
                    view.dataList = setSortList(view.dataList);
                    List<RCS_ExamData_Common> tempList = new List<RCS_ExamData_Common>();
                    if (labData.Exists(x => x.exam_date == view.exam_date && ExamView.thHeadList.Exists(s => s.name == x.name && s.spem == x.spem && s.unit == x.unit)))
                    {
                        tempList = labData.FindAll(x => x.exam_date == view.exam_date && ExamView.thHeadList.Exists(s => s.name == x.name && s.spem == x.spem && s.unit == x.unit));
                        foreach (RCS_ExamData_Common value in tempList)
                        {
                            if (view.dataList.Exists(s => s.name == value.name && s.spem == value.spem && s.unit == value.unit))
                                view.dataList.Find(s => s.name == value.name && value.spem == value.spem && s.unit == value.unit).result = value.result;
                        }
                    }
                    ExamView.List.Add(view);
                }
            }
            catch (Exception ex)
            {
                ExamView.RESPONSE_MSG.status = RESPONSE_STATUS.EXCEPTION;
                ExamView.RESPONSE_MSG.message = "程式發生錯誤，請洽資訊人員，錯誤訊息如下所示:" + ex.Message;
                LogTool.SaveLogMessage(ex, actionName, GetLogToolCS.Exam);
            }
            return rm;
        }

        /// <summary>
        /// 檢驗資料排序
        /// </summary>
        /// <param name="thHeadList"></param>
        /// <returns></returns>
        private List<RCS_ExamData_Common> setSortList(List<RCS_ExamData_Common> thHeadList)
        {
            try
            {
                string spemName = "ARTERIAL BLOOD";
                #region 設定ABG data 資料排序
                //設定ABG data 資料排序
                if (thHeadList.Exists(x => x.spem == spemName))
                {
                    WS_AVBGData ad = new WS_AVBGData();
                    thHeadList.FindAll(x => x.spem == spemName).ForEach(s => s.spemSort = 1);
                    if (thHeadList.Exists(x => x.spem == spemName && x.name == ad.AVBGValueList["pH"]))
                    {
                        thHeadList.Find(x => x.spem == spemName && x.name == ad.AVBGValueList["pH"]).examSort = 1;
                    }
                    if (thHeadList.Exists(x => x.spem == spemName && x.name == ad.AVBGValueList["PCO2"]))
                    {
                        thHeadList.Find(x => x.spem == spemName && x.name == ad.AVBGValueList["PCO2"]).examSort = 2;
                    }
                    if (thHeadList.Exists(x => x.spem == spemName && x.name == ad.AVBGValueList["PO2"]))
                    {
                        thHeadList.Find(x => x.spem == spemName && x.name == ad.AVBGValueList["PO2"]).examSort = 3;
                    }
                    if (thHeadList.Exists(x => x.spem == spemName && x.name == ad.AVBGValueList["BaseExcess"]))
                    {
                        thHeadList.Find(x => x.spem == spemName && x.name == ad.AVBGValueList["BaseExcess"]).examSort = 4;
                    }
                    if (thHeadList.Exists(x => x.spem == spemName && x.name == ad.AVBGValueList["HCO3"]))
                    {
                        thHeadList.Find(x => x.spem == spemName && x.name == ad.AVBGValueList["HCO3"]).examSort = 5;
                    }
                    if (thHeadList.Exists(x => x.spem == spemName && x.name == ad.AVBGValueList["TotalCarbonDioxide"]))
                    {
                        thHeadList.Find(x => x.spem == spemName && x.name == ad.AVBGValueList["TotalCarbonDioxide"]).examSort = 6;
                    }
                    if (thHeadList.Exists(x => x.spem == spemName && x.name == ad.AVBGValueList["SaO2"]))
                    {
                        thHeadList.Find(x => x.spem == spemName && x.name == ad.AVBGValueList["SaO2"]).examSort = 7;
                    }
                }
                #endregion
            }
            catch (Exception ex)
            {
                LogTool.SaveLogMessage(ex, "setSortList", GetLogToolCS.Exam);
            }
            finally
            {
                thHeadList = thHeadList.OrderByDescending(x => x.spemSort).ThenByDescending(x => x.examSort).ToList();
            }
            return thHeadList;
        }

        /// <summary>
        /// 取得檢驗資料
        /// </summary>
        /// <returns></returns>
        public RESPONSE_MSG Exam_List(ref List<ExamReport> ExamReportData, string pSDate, string pEDate, IPDPatientInfo pat_info, IWebServiceParam iwp)
        {
            RESPONSE_MSG rm = new RESPONSE_MSG();
            try
            {
                //取的資料Lab資料
                ExamReportData = webmethod.getReportData(iwp, pat_info.chart_no, pat_info.ipd_no, pSDate, pEDate);
                
            }
            catch (Exception ex)
            {
                LogTool.SaveLogMessage(ex, "Exam_List", GetLogToolCS.Exam);
            }
            return rm;
        }

 
        /// <summary>
        /// 取得特殊用藥及治療情形
        /// </summary>
        /// <returns></returns>
        public RESPONSE_MSG search_drog(ref List<PatOrder> returnList, IWebServiceParam iwp, string pChartNo, string pIpdNo, string pSDate, string pEDate = "")
        {
            RESPONSE_MSG rm = new RESPONSE_MSG();
            returnList = this.webmethod.getShiftOrderList(iwp, pChartNo, pIpdNo); 
            return rm;
        }

        /// <summary>
        /// 取得病患呼吸照護紀錄主檔
        /// </summary>
        /// <returns></returns>
        public RESPONSE_MSG TakeOffAssessmentData( string pChartNo )
        {
            RESPONSE_MSG rm = new RESPONSE_MSG();
            List<RT_TAKEOFF_ASSESSMENT> tk_report = new List<RT_TAKEOFF_ASSESSMENT>();
            string actionName = "getRTRecordMaster";
            try
            {
                string sql = "SELECT A.RECORDDATE,B.ITEM_NAME,B.ITEM_VALUE FROM {0} A JOIN {1} B ON A.record_id = B.record_id" +


                             " WHERE DATASTATUS='1' AND CHART_NO = {2} AND LOWER(B.ITEM_NAME) IN ('pi_max', 'pe_max', 'rsbi', 'cuff_leak_percent', 'cuff_leak_ml', 'rr','vt', 'mv','mode','vt_value','rsi_srr','cuff_leak_sound')";
                DataTable dt = this.DBLink.DBA.getSqlDataTable(string.Format(sql, DB_TABLE_NAME.DB_RCS_RECORD_MASTER, DB_TABLE_NAME.DB_RCS_RECORD_DETAIL, SQLDefend.SQLString(pChartNo)));
                if (DTNotNullAndEmpty(dt))
                {
                    foreach (DataRow dr in dt.Rows)
                    {
                        RT_TAKEOFF_ASSESSMENT tmp_rt = new RT_TAKEOFF_ASSESSMENT();
                        tmp_rt.record_date = checkDataColumn(dr, "RECORDDATE");
                        string ITEM_NAME = checkDataColumn(dr, "ITEM_NAME");
                        string ITEM_VALUE = checkDataColumn(dr, "ITEM_VALUE", "-");
                        if (!tk_report.Exists(x => x.record_date == tmp_rt.record_date)) tk_report.Add(tmp_rt);
                        switch (ITEM_NAME)
                        {
                            case "pi_max":
                                tk_report.Find(x => x.record_date == tmp_rt.record_date).pi = ITEM_VALUE;
                                break;
                            case "pe_max":
                                tk_report.Find(x => x.record_date == tmp_rt.record_date).pe = ITEM_VALUE;
                                break;
                            case "rsbi":
                                tk_report.Find(x => x.record_date == tmp_rt.record_date).rsbi = ITEM_VALUE;
                                break;
                            case "cuff_leak_percent":
                                tk_report.Find(x => x.record_date == tmp_rt.record_date).cuff_leak = ITEM_VALUE;
                                break;
                            case "cuff_leak_ml":
                                tk_report.Find(x => x.record_date == tmp_rt.record_date).cuff_leak_ml = ITEM_VALUE;
                                break;
                            case "mode":
                                tk_report.Find(x => x.record_date == tmp_rt.record_date).mode = ITEM_VALUE;
                                break;
                            case "vt_value":
                                tk_report.Find(x => x.record_date == tmp_rt.record_date).vt_value = ITEM_VALUE;
                                break;
                            case "rsi_srr":
                                tk_report.Find(x => x.record_date == tmp_rt.record_date).srr = ITEM_VALUE;
                                break; 
                            case "cuff_leak_sound":
                                tk_report.Find(x => x.record_date == tmp_rt.record_date).cuff_leak_sound = ITEM_VALUE;
                                break;
                            default:
                                break;
                        }
                        tmp_rt = null;

                    }
                }
                else if (this.DBLink.DBA.hasLastError )
                {
                    rm.status = RESPONSE_STATUS.ERROR;
                    rm.messageList.Add(this.DBLink.DBA.lastError);
                    LogTool.SaveLogMessage(this.DBLink.DBA.lastError, actionName, this.csName);
                }  

            }
            catch (Exception ex)
            { 
                rm.status = RESPONSE_STATUS.EXCEPTION;
                rm.messageList.Add(ex.Message);
                LogTool.SaveLogMessage(ex, actionName, this.csName);
            }
            if (rm.status == RESPONSE_STATUS.SUCCESS)
            {
                rm.attachment = tk_report.Where(x => x.cuff_leak != "-" || x.cuff_leak_ml != "-" || x.pe != "-" || x.pi != "-" || x.rsbi != "-" || x.vt_value != "-" || x.srr != "-").ToList(); 
            }

            return rm;
        }
         
        /// <summary>
        /// 取得手術資料
        /// </summary>
        /// <returns></returns>
        public RESPONSE_MSGLIST getPatOperationList(ref List<PatOperation> ORList, IWebServiceParam iwp, string tmp_chart_no, string tmp_ipd_no)
        {
            RESPONSE_MSGLIST rm = new RESPONSE_MSGLIST();
            try
            {
                //取的資料Lab資料
                ORList = webmethod.getPatOperationList(iwp, tmp_chart_no, tmp_ipd_no);

            }
            catch (Exception ex)
            {
                LogTool.SaveLogMessage(ex, "getPatOperationList", GetLogToolCS.Exam);
            }
            return rm;
        }

        /// <summary>
        /// 取得ABG Data
        /// </summary>
        /// <returns></returns>
        public RESPONSE_MSG getABGData(string pChart_no)
        {
            RESPONSE_MSG rm = new RESPONSE_MSG();

            List<RT_RECORD> tk_report = new List<RT_RECORD>();
            string actionName = "getABGData";
            try
            {
                string sql =
                    @"SELECT A.RECORDDATE,B.ITEM_NAME,B.ITEM_VALUE FROM {0} A JOIN {1} B ON A.record_id = B.record_id
                      WHERE DATASTATUS='1' AND CHART_NO = {2} AND LOWER(B.ITEM_NAME)  
                      IN ('abg_time','abg_ph', 'abg_paco2', 'abg_pao2', 'abg_sao2', 'abg_hco3','abg_be', 'abg_paado2', 'abg_shunt' )";
                DataTable dt = this.DBLink.DBA.getSqlDataTable(string.Format(sql, DB_TABLE_NAME.DB_RCS_RECORD_MASTER.ToString(), DB_TABLE_NAME.DB_RCS_RECORD_DETAIL, SQLDefend.SQLString(pChart_no)));
                if (DTNotNullAndEmpty(dt))
                {
                    foreach (DataRow dr in dt.Rows)
                    {
                        RT_RECORD tmp_rt = new RT_RECORD();
                        tmp_rt.recorddate = checkDataColumn(dr, "RECORDDATE");
                        string ITEM_NAME = checkDataColumn(dr, "ITEM_NAME");
                        string ITEM_VALUE = checkDataColumn(dr, "ITEM_VALUE", "-");
                        if (!tk_report.Exists(x => x.recorddate == tmp_rt.recorddate)) tk_report.Add(tmp_rt);
                        switch (ITEM_NAME)
                        {
                            case "abg_time":
                                tk_report.Find(x => x.recorddate == tmp_rt.recorddate).abg_time = ITEM_VALUE;
                                break;
                            case "abg_ph":
                                tk_report.Find(x => x.recorddate == tmp_rt.recorddate).abg_ph = ITEM_VALUE;
                                break;
                            case "abg_paco2":
                                tk_report.Find(x => x.recorddate == tmp_rt.recorddate).abg_paco2 = ITEM_VALUE;
                                break;
                            case "abg_pao2":
                                tk_report.Find(x => x.recorddate == tmp_rt.recorddate).abg_pao2 = ITEM_VALUE;
                                break;
                            case "abg_sao2":
                                tk_report.Find(x => x.recorddate == tmp_rt.recorddate).abg_sao2 = ITEM_VALUE;
                                break;
                            case "abg_hco3":
                                tk_report.Find(x => x.recorddate == tmp_rt.recorddate).abg_hco3 = ITEM_VALUE;
                                break;
                            case "abg_be":
                                tk_report.Find(x => x.recorddate == tmp_rt.recorddate).abg_be = ITEM_VALUE;
                                break;
                            case "abg_paado2":
                                tk_report.Find(x => x.recorddate == tmp_rt.recorddate).abg_paado2 = ITEM_VALUE;
                                break;
                            case "abg_shunt":
                                tk_report.Find(x => x.recorddate == tmp_rt.recorddate).abg_shunt = ITEM_VALUE;
                                break;
                            default:
                                break;
                        }
                        tmp_rt = null;

                    }
                }
                else if (this.DBLink.DBA.hasLastError  )
                {
                    LogTool.SaveLogMessage(this.DBLink.DBA.lastError, actionName, GetLogToolCS.RTRecord);
                }

            }
            catch (Exception ex)
            {
                LogTool.SaveLogMessage(ex, actionName, GetLogToolCS.RTRecord);
            }
            
            if (rm.status == RESPONSE_STATUS.SUCCESS)
            {
                tk_report = tk_report.Where(x => x.abg_time != "-" || x.abg_ph != "-" || x.abg_paco2 != "-" || x.abg_pao2 != "-" || x.abg_sao2 != "-" || x.abg_hco3 != "-" || x.abg_be != "-" || x.abg_paado2 != "-" || x.abg_shunt != "-").ToList();
                rm.attachment = tk_report.OrderByDescending(x => x.recorddate).ToList(); 
            }

            return rm;
        }
         
        
    }

    public class FormSearch_io : AUTH
    {
        public string io_sdate { get; set; }
        public string io_edate { get; set; }
    }

    public class FormAssessOPData : AUTH
    {
        public string pCptId { get; set; }
    }

    public class FormExamData : AUTH
    {
        public string pSDate { get; set; }
        public string pEDate { get; set; }
    }
        
    public class FormExamReport : AUTH
    {
        public string pSDate { get; set; }
    }
     
 
}

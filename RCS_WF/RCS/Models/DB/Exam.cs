using Com.Mayaminer;
using RCS_Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using RCSData.Models; 
using RCS_Data.Models.ViewModels;

namespace RCS.Models
{
    public class Exam:BaseModel
    {
        /// <summary>
        /// 取得檢驗資料
        /// </summary>
        /// <param name="start_date">開始日期</param>
        /// <param name="end_date">結束日期</param>
        /// <returns></returns>
        public ExamView getExamDdata(string start_date, string end_date = "")
        {
            string actionName = "getExamDdata";
            ExamView ExamView = new ExamView();
            ExamView.RESPONSE_MSG = new RESPONSE_MSG();
            ExamView.List = new List<ExamViewList>();
            ExamView.thHeadList = new List<RCS_ExamData_Common>();
            try
            {
                //取的資料Lab資料
                List<RCS_ExamData_Common> labData = web_method.getLabData(new RCS.Models.HOSP.HospFactory().webService.HISLabData(), pat_info.chart_no, pat_info.ipd_no, start_date, "");
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
            return ExamView;
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
    } 
}
using System;
using RCSData.Models;
using System.Collections.Generic;
using System.Linq;

namespace RCSData.Models
{
    public partial class WebMethod
    {

        /// <summary>取得單一病人取得血液生化資料檢驗資料清單</summary>
        /// <param name="pChartNo">病歷號</param>
        /// <param name="pIpdNo">批價序號</param>
        /// <returns></returns>
        public List<ExamBloodBiochemical> getBloodBiochemicalData(IWebServiceParam iwp,string pChartNo, string pIpdNo)
        {
            List<ExamBloodBiochemical> pList = new List<ExamBloodBiochemical>(); 
            WS_BloodBiochemicalData bb = new WS_BloodBiochemicalData(pChartNo, pIpdNo, iwp);
            ServiceResult<RCS_ExamData_Common> sr = HISData.getServiceResult(bb);
            this.datastatus = sr.datastatus;
            this.errorMsg = sr.errorMsg;
            if (sr.datastatus == RCS_Data.HISDataStatus.SuccessWithData)
            {
                pList = bb.ExamBloodList;
            }
            return pList;
        }

    }


    /// <summary>
    /// 取得單一病人取得血液生化資料檢驗資料清單
    /// </summary>
    public class WS_BloodBiochemicalData : WS_LabData
    {
        public List<ExamBloodBiochemical> ExamBloodList { get; set; }

        public static string getBloodBiochemicalDate { get { return DateTime.Now.AddDays(-30).ToString("yyyy-MM-dd"); } }
        public Dictionary<string, string> BloodBiochemicalValueList { get; set; }


        /// <summary>
        /// 取得單一病人取得血液生化資料檢驗資料清單
        /// </summary>
        /// <param name="pChartNo"></param>
        /// <param name="pIpdNo"></param>
        /// <param name="pWsSession"></param>
        public WS_BloodBiochemicalData(string pChartNo, string pIpdNo , IWebServiceParam iwp) : base(pChartNo, pIpdNo, getBloodBiochemicalDate, iwp)
        {

        }

        public override void setParam()
        {
            base.setParam();
            this.ExamBloodList = new List<ExamBloodBiochemical>();
            #region 血液生化 參數設定
            BloodBiochemicalValueList = this.iwp.itemKey;
            #endregion
        }

        public override void setReturnValue()
        {
            if (this.ServiceResult.datastatus == RCS_Data.HISDataStatus.SuccessWithData)
            {
                base.setReturnValue();

                //篩選檢驗日期
                List<string> dateList = new List<string>();
                List<RCS_ExamData_Common> temp = Newtonsoft.Json.JsonConvert.DeserializeObject<List<RCS_ExamData_Common>>(this.ServiceResult.returnJson);
                if (temp.Count > 0)
                {
                    if (temp.Exists(x => BloodBiochemicalValueList.Values.Contains(x.name) || 
                    BloodBiochemicalValueList.Values.Where(y=> y.ToString().Split(',').Contains(x.name)).Any() ))
                        dateList = temp.FindAll(x => BloodBiochemicalValueList.Values.Contains(x.name) ||
                         BloodBiochemicalValueList.Values.Where(y => y.ToString().Split(',').Contains(x.name)).Any()
                         ).Select(x => x.exam_date).Distinct().ToList();
                    //取的方式依據各家醫院資料來源修改
                    #region 取得檢驗資料中的血液生化項目資料
                    ExamBloodBiochemical Exam = null;
                    foreach (string item in dateList)
                    {
                        List<RCS_ExamData_Common> RCS_Exam = new List<RCS_ExamData_Common>();
                        //判斷血液生化的日期項目
                        if (this.ExamBloodList.Exists(x => x.examDate == item))
                            Exam = this.ExamBloodList.Find(x => x.examDate == item);
                        else
                        {
                            Exam = new ExamBloodBiochemical();
                            Exam.examDate = item;
                            Exam.examContent = new Dictionary<string, RCS_ExamData_Common>();
                        }
                        //取得血液生化的設定項目 BloodBiochemicalValue 清單
                        if (temp.Exists(x => x.exam_date == item &&( BloodBiochemicalValueList.Values.Contains(x.name) ||
                         BloodBiochemicalValueList.Values.Where(y => y.ToString().Split(',').Contains(x.name)).Any())))
                            RCS_Exam = temp.FindAll(x => x.exam_date == item &&( BloodBiochemicalValueList.Values.Contains(x.name) ||
                         BloodBiochemicalValueList.Values.Where(y => y.ToString().Split(',').Contains(x.name)).Any()  ));
                        foreach (KeyValuePair<string, string> type in BloodBiochemicalValueList)
                        {
                            List<string> valueList = new List<string>();
                            if (type.Value.Contains(","))
                            {
                                valueList = type.Value.Split(',').ToList();
                            }
                            else
                            {
                                valueList.Add(type.Value);
                            }
                            if (RCS_Exam.Exists(x => x.name == type.Value || valueList.Contains(x.name)))
                            {
                                if (Exam.examContent.Keys.Contains(type.Key))
                                    Exam.examContent[type.Key] = RCS_Exam.Find(x => x.name == type.Value || valueList.Contains(x.name));
                                else
                                    Exam.examContent.Add(type.Key, RCS_Exam.Find(x => x.name == type.Value || valueList.Contains(x.name)));
                            }
                            else
                            {
                                Exam.examContent.Add(type.Key, new RCS_ExamData_Common());
                            }
                        }
                        if (!this.ExamBloodList.Exists(x => x.examDate == item)) this.ExamBloodList.Add(Exam);
                    }
                    #endregion
                }

            }
        }
    }


    /// <summary>
    /// 取得取得血液生化資料
    /// </summary>
    public class ExamBloodBiochemical
    {
        /// <summary>
        /// 檢驗日期
        /// </summary>
        public string examDate { get; set; }
        /// <summary>
        /// 檢驗內容
        /// </summary>
        public Dictionary<string, RCS_ExamData_Common> examContent { get; set; }
    }

}
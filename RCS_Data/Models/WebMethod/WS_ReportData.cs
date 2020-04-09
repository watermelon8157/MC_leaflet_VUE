using Newtonsoft.Json;
using RCSData.Models;
using System;
using System.Collections.Generic;

namespace RCSData.Models
{

    public partial class WebMethod
    {
        /// <summary>取得單一病人相關檢查報告結果清單</summary>
        /// <param name="pChartNo">病歷號</param>
        /// <param name="pIpdNo">批價序號</param>
        /// <param name="pSDate">開始日期</param>
        /// <param name="pEDate">結束日期</param>
        /// <returns></returns>
        public List<ExamReport> getReportData(IWebServiceParam iwp,string pChartNo, string pIpdNo, string pSDate, string pEDate = "")
        {
#if DEBUG
            string _str = @"
[{""TYPE_NAM"":""(-M - Mode and - sector - scan)  18005"",""ACCNO"":""201810151053"",""STATNAME"":""0"",""ORDERDR"":""1571   "",""ORDERDRNAME"":""陳育暄"",""ENTERDR"":""       "",""ENTERDRNAME"":null,""COMPLETTIME"":""20181016    "",""ENTERTIME"":""            "",""REPORT"":""""}]
";
            return JsonConvert.DeserializeObject<List<ExamReport>>(_str);
#else
            List<ExamReport> pList = new List<ExamReport>(); 
            WS_ReportData order = new WS_ReportData(pChartNo, pIpdNo, pSDate, iwp);
            ServiceResult< ExamReport> sr = HISData.getServiceResult(order);
            this.datastatus = sr.datastatus;
            this.errorMsg = sr.errorMsg;
            if (sr.datastatus == RCS_Data.HISDataStatus.SuccessWithData)
            {
                pList = JsonConvert.DeserializeObject<List<ExamReport>>(sr.returnJson);
            }
            return pList;
#endif

        }
    }

    /// <summary>
    /// 取得單一病人相關檢查報告結果清單
    /// </summary>
    public class WS_ReportData : AwebMethod< ExamReport>, IwebMethod< ExamReport>
    {
        public string webMethodName { get { return this.iwp.webMethodName; } }
        public override string wsSession { get { return "RCS_WS_BASIC"; } }
       
        /// <summary>
        /// 取得單一病人相關檢查報告結果清單
        /// </summary>
        /// <param name="pChartNo"></param>
        /// <param name="pIpdNo"></param>
        /// <param name="pSDate"></param>
        /// <param name="pEDate"></param>
        /// <param name="pWsSession"></param>
        public WS_ReportData(string pChartNo, string pIpdNo, string pSDate, IWebServiceParam pIwp, string pEDate = "")
        {
            this.iwp = pIwp;
            setParam();
            if (pChartNo != null && pIpdNo != null && pSDate != null &&
                    pChartNo.Length > 0 && pIpdNo.Length > 0 && pSDate.Length > 0)
            {
                //檢查參數3、4是否為正確日期
                if (!mayaminer.com.library.DateHelper.isDate(pSDate, "yyyy-MM-dd"))
                {
                    this.ServiceResult.datastatus = RCS_Data.HISDataStatus.ParametersError;
                    this.ServiceResult.errorMsg = "開始日期格式錯誤!";
                }
                if (pEDate != null && pEDate != "" && !mayaminer.com.library.DateHelper.isDate(pEDate, "yyyy-MM-dd"))
                {
                    this.ServiceResult.datastatus = RCS_Data.HISDataStatus.ParametersError;
                    if (this.ServiceResult.errorMsg != null && this.ServiceResult.errorMsg != "")
                        this.ServiceResult.errorMsg = "開始及結束日期格式錯誤!";
                    else
                        this.ServiceResult.errorMsg = "結束日期格式錯誤!";
                }
                //確認傳入值是否正確
                if (this.ServiceResult.datastatus != RCS_Data.HISDataStatus.ParametersError)
                {
                    this.paramList["ipdNo"].paramValue = pIpdNo;
                    this.paramList["chartNo"].paramValue = pChartNo;
                    this.paramList["sDate"].paramValue = mayaminer.com.library.DateHelper.Parse(pSDate).ToString("yyyy/MM/dd");
                    if (pEDate != null && pEDate != "" && !mayaminer.com.library.DateHelper.isDate(pEDate, "yyyy-MM-dd"))
                        this.paramList["eDate"].paramValue = mayaminer.com.library.DateHelper.Parse(pEDate).ToString("yyyy/MM/dd");
                    else
                        this.paramList["eDate"].paramValue = DateTime.Now.ToString("yyyy/MM/dd");
                }
            }
            else
            {
                this.ServiceResult.datastatus = RCS_Data.HISDataStatus.ParametersError;
                if (pChartNo == null || pChartNo.Length == 0) this.ServiceResult.errorMsg = "病人病歷號不可為空值!";
                if (pIpdNo == null || pIpdNo.Length == 0) this.ServiceResult.errorMsg = "病人住院號不可為空值!";
                if (pSDate == null || pSDate.Length == 0) this.ServiceResult.errorMsg = "開始日期不可為空值!";
                if ((pChartNo == null || pChartNo.Length == 0) && (pIpdNo == null || pIpdNo.Length == 0)
                    && (pSDate == null || pSDate.Length == 0))
                    this.ServiceResult.errorMsg = "病人病歷號、住院號及開始日期不可為空值!";
            }

        }

        public void setParam()
        {
            this.ServiceResult = new ServiceResult< ExamReport>();
            this.paramList = this.iwp.paramList;
            this.returnValue = this.iwp.returnValue;
        }
    }

    /// <summary>
    /// 報告資料
    /// </summary>
    public class ExamReport
    {

        /// <summary> 病人病歷號 </summary>
        public string CHART { set; get; }
        public string SOURCE { set; get; }
        /// <summary> 報告類型代碼(院內代碼) </summary>
        public string TYPE { set; get; }
        /// <summary> 報告類型 </summary>
        public string TYPE_NAM { set; get; }
        /// <summary> 狀態代碼 </summary>
        public string STAT { set; get; }
        /// <summary> 狀態 </summary>
        public string STATNAME { set; get; }
        /// <summary> 開單醫師代碼(院內代碼) </summary>
        public string ORDERDR { set; get; }
        /// <summary> 開單醫師姓名 </summary>
        public string ORDERDRNAME { set; get; }
        /// <summary> 開單日期 </summary>
        public string ORDERTIME { set; get; }
        public string LOGINTIME { set; get; }
        public string SEQ { set; get; }
        public string E_SEQ { set; get; }
        public string STUDYDR { set; get; }
        public string STUDYDRNAME { set; get; }
        /// <summary> 登打報告醫師代碼(院內代碼) </summary>
        public string ENTERDR { set; get; }
        /// <summary> 登打報告醫師姓名 </summary>
        public string ENTERDRNAME { set; get; }
        /// <summary> 完成日期 </summary>
        public string COMPLETTIME { set; get; }
        /// <summary> 登打日期 </summary>
        public string ENTERTIME { set; get; }
        public string REPORT { set; get; }
    }
}
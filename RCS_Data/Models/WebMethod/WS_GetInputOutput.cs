using Newtonsoft.Json;
using RCSData.Models;
using System;
using System.Collections.Generic;


namespace RCSData.Models
{
    /// <summary>
    /// 取得輸入輸出資料
    /// </summary>
    public partial class WebMethod
    {
        /// <summary>取得輸入輸出資料</summary>
        /// <param name="pChartNo">病歷號</param>
        /// <param name="pIpdNo">批價序號</param>
        /// <param name="pSDate">開始日期</param>
        /// <param name="pEDate">結束日期</param>
        /// <returns></returns>
        public List<IO_PUT> getInputOutput(IWebServiceParam iwp,string pChartNo, string pIpdNo, string pSDate, string pEDate = "")
        { 
            #region IO輸入輸出資料
            if ( isBasicMode ||  isDebuggerMode)
            {
                #region 假資料
                string _str = @"
                [{""EXAM_DATE"":""2019-11-04 06:37:00"",""IO_TYPE"":""I"",""IO_ITEM"":""其他"",""RESULT"":140.0,""UNIT"":""ml""},{""EXAM_DATE"":""2019-11-03 22:58:00"",""IO_TYPE"":""I"",""IO_ITEM"":""抗生素"",""RESULT"":70.0,""UNIT"":""ml""},{""EXAM_DATE"":""2019-11-03 15:00:00"",""IO_TYPE"":""I"",""IO_ITEM"":""其他"",""RESULT"":70.0,""UNIT"":""ml""},{""EXAM_DATE"":""2019-11-03 06:53:00"",""IO_TYPE"":""I"",""IO_ITEM"":""抗生素"",""RESULT"":150.0,""UNIT"":""ml""},{""EXAM_DATE"":""2019-11-02 14:46:00"",""IO_TYPE"":""I"",""IO_ITEM"":""其他"",""RESULT"":110.0,""UNIT"":""ml""},{""EXAM_DATE"":""2019-11-01 14:57:00"",""IO_TYPE"":""I"",""IO_ITEM"":""其他"",""RESULT"":30.0,""UNIT"":""ml""},{""EXAM_DATE"":""2019-11-01 14:57:00"",""IO_TYPE"":""I"",""IO_ITEM"":""抗生素"",""RESULT"":70.0,""UNIT"":""ml""},{""EXAM_DATE"":""2019-11-01 14:57:00"",""IO_TYPE"":""I"",""IO_ITEM"":""Millisrol"",""RESULT"":24.0,""UNIT"":""ml""},{""EXAM_DATE"":""2019-11-01 06:18:00"",""IO_TYPE"":""I"",""IO_ITEM"":""Millisrol"",""RESULT"":81.0,""UNIT"":""ml""},{""EXAM_DATE"":""2019-10-31 23:38:00"",""IO_TYPE"":""I"",""IO_ITEM"":""其他"",""RESULT"":336.0,""UNIT"":""ml""},{""EXAM_DATE"":""2019-10-31 23:00:00"",""IO_TYPE"":""I"",""IO_ITEM"":""其他"",""RESULT"":336.0,""UNIT"":""ml""},{""EXAM_DATE"":""2019-10-31 15:00:00"",""IO_TYPE"":""I"",""IO_ITEM"":""Millisrol"",""RESULT"":87.0,""UNIT"":""ml""},{""EXAM_DATE"":""2019-10-31 15:00:00"",""IO_TYPE"":""I"",""IO_ITEM"":""其他"",""RESULT"":240.0,""UNIT"":""ml""},{""EXAM_DATE"":""2019-10-31 15:00:00"",""IO_TYPE"":""I"",""IO_ITEM"":""D5S"",""RESULT"":240.0,""UNIT"":""ml""},{""EXAM_DATE"":""2019-10-31 15:00:00"",""IO_TYPE"":""I"",""IO_ITEM"":""抗生素"",""RESULT"":70.0,""UNIT"":""ml""},{""EXAM_DATE"":""2019-10-31 15:00:00"",""IO_TYPE"":""I"",""IO_ITEM"":""TaitaNO5"",""RESULT"":80.0,""UNIT"":""ml""},{""EXAM_DATE"":""2019-10-31 06:10:00"",""IO_TYPE"":""I"",""IO_ITEM"":""0.9 % NS"",""RESULT"":500.0,""UNIT"":""ml""},{""EXAM_DATE"":""2019-10-31 06:10:00"",""IO_TYPE"":""I"",""IO_ITEM"":""其他"",""RESULT"":320.0,""UNIT"":""ml""},{""EXAM_DATE"":""2019-10-31 06:10:00"",""IO_TYPE"":""I"",""IO_ITEM"":""Millisrol"",""RESULT"":48.0,""UNIT"":""ml""},{""EXAM_DATE"":""2019-10-31 06:10:00"",""IO_TYPE"":""I"",""IO_ITEM"":""其他"",""RESULT"":90.0,""UNIT"":""ml""},{""EXAM_DATE"":""2019-10-30 23:00:00"",""IO_TYPE"":""I"",""IO_ITEM"":""其他"",""RESULT"":368.0,""UNIT"":""ml""},{""EXAM_DATE"":""2019-10-30 15:00:00"",""IO_TYPE"":""I"",""IO_ITEM"":""抗生素"",""RESULT"":70.0,""UNIT"":""ml""},{""EXAM_DATE"":""2019-10-30 15:00:00"",""IO_TYPE"":""I"",""IO_ITEM"":""TaitaNO5"",""RESULT"":40.0,""UNIT"":""ml""},{""EXAM_DATE"":""2019-10-30 15:00:00"",""IO_TYPE"":""I"",""IO_ITEM"":""D5S"",""RESULT"":280.0,""UNIT"":""ml""},{""EXAM_DATE"":""2019-10-30 15:00:00"",""IO_TYPE"":""I"",""IO_ITEM"":""其他"",""RESULT"":140.0,""UNIT"":""ml""},{""EXAM_DATE"":""2019-10-30 15:00:00"",""IO_TYPE"":""I"",""IO_ITEM"":""Millisrol"",""RESULT"":48.0,""UNIT"":""ml""},{""EXAM_DATE"":""2019-10-30 06:30:00"",""IO_TYPE"":""I"",""IO_ITEM"":""D5S"",""RESULT"":240.0,""UNIT"":""ml""},{""EXAM_DATE"":""2019-10-30 06:30:00"",""IO_TYPE"":""I"",""IO_ITEM"":""Millisrol"",""RESULT"":30.0,""UNIT"":""ml""},{""EXAM_DATE"":""2019-11-02 06:13:00"",""IO_TYPE"":""I"",""IO_ITEM"":""液體(藥)"",""RESULT"":100.0,""UNIT"":""ml""},{""EXAM_DATE"":""2019-10-30 15:00:00"",""IO_TYPE"":""I"",""IO_ITEM"":""液體(藥)"",""RESULT"":40.0,""UNIT"":""ml""},{""EXAM_DATE"":""2019-10-30 06:30:00"",""IO_TYPE"":""I"",""IO_ITEM"":""液體(藥)"",""RESULT"":50.0,""UNIT"":""ml""},{""EXAM_DATE"":""2019-11-03 22:58:00"",""IO_TYPE"":""I"",""IO_ITEM"":""管灌飲食"",""RESULT"":550.0,""UNIT"":""ml""},{""EXAM_DATE"":""2019-11-03 15:00:00"",""IO_TYPE"":""I"",""IO_ITEM"":""管灌飲食"",""RESULT"":600.0,""UNIT"":""ml""},{""EXAM_DATE"":""2019-11-02 22:34:00"",""IO_TYPE"":""I"",""IO_ITEM"":""管灌飲食"",""RESULT"":450.0,""UNIT"":""ml""},{""EXAM_DATE"":""2019-11-02 14:46:00"",""IO_TYPE"":""I"",""IO_ITEM"":""管灌飲食"",""RESULT"":510.0,""UNIT"":""ml""},{""EXAM_DATE"":""2019-11-02 06:13:00"",""IO_TYPE"":""I"",""IO_ITEM"":""管灌飲食"",""RESULT"":60.0,""UNIT"":""ml""},{""EXAM_DATE"":""2019-11-01 23:00:00"",""IO_TYPE"":""I"",""IO_ITEM"":""管灌飲食"",""RESULT"":460.0,""UNIT"":""ml""},{""EXAM_DATE"":""2019-11-01 14:57:00"",""IO_TYPE"":""I"",""IO_ITEM"":""管灌飲食"",""RESULT"":455.0,""UNIT"":""ml""},{""EXAM_DATE"":""2019-11-01 06:18:00"",""IO_TYPE"":""I"",""IO_ITEM"":""管灌飲食"",""RESULT"":30.0,""UNIT"":""ml""},{""EXAM_DATE"":""2019-10-31 23:38:00"",""IO_TYPE"":""I"",""IO_ITEM"":""管灌飲食"",""RESULT"":330.0,""UNIT"":""ml""},{""EXAM_DATE"":""2019-10-31 23:00:00"",""IO_TYPE"":""I"",""IO_ITEM"":""管灌飲食"",""RESULT"":330.0,""UNIT"":""ml""},{""EXAM_DATE"":""2019-10-31 15:00:00"",""IO_TYPE"":""I"",""IO_ITEM"":""管灌飲食"",""RESULT"":120.0,""UNIT"":""ml""},{""EXAM_DATE"":""2019-10-31 06:10:00"",""IO_TYPE"":""I"",""IO_ITEM"":""FFP"",""RESULT"":390.0,""UNIT"":""ml""},{""EXAM_DATE"":""2019-10-30 15:00:00"",""IO_TYPE"":""I"",""IO_ITEM"":""PRBC"",""RESULT"":280.0,""UNIT"":""ml""},{""EXAM_DATE"":""2019-10-30 15:00:00"",""IO_TYPE"":""I"",""IO_ITEM"":""FFP"",""RESULT"":410.0,""UNIT"":""ml""},{""EXAM_DATE"":""2019-11-04 06:37:00"",""IO_TYPE"":""O"",""IO_ITEM"":""自解量"",""RESULT"":950.0,""UNIT"":""ml""},{""EXAM_DATE"":""2019-11-03 22:58:00"",""IO_TYPE"":""O"",""IO_ITEM"":""自解量"",""RESULT"":700.0,""UNIT"":""ml""},{""EXAM_DATE"":""2019-11-03 15:00:00"",""IO_TYPE"":""O"",""IO_ITEM"":""自解量"",""RESULT"":1140.0,""UNIT"":""ml""},{""EXAM_DATE"":""2019-11-03 06:53:00"",""IO_TYPE"":""O"",""IO_ITEM"":""自解量"",""RESULT"":950.0,""UNIT"":""ml""},{""EXAM_DATE"":""2019-11-02 22:34:00"",""IO_TYPE"":""O"",""IO_ITEM"":""自解量"",""RESULT"":700.0,""UNIT"":""ml""},{""EXAM_DATE"":""2019-11-02 14:46:00"",""IO_TYPE"":""O"",""IO_ITEM"":""自解量"",""RESULT"":1250.0,""UNIT"":""ml""},{""EXAM_DATE"":""2019-11-02 06:13:00"",""IO_TYPE"":""O"",""IO_ITEM"":""留置導尿量"",""RESULT"":980.0,""UNIT"":""ml""},{""EXAM_DATE"":""2019-11-01 23:00:00"",""IO_TYPE"":""O"",""IO_ITEM"":""自解量"",""RESULT"":950.0,""UNIT"":""ml""},{""EXAM_DATE"":""2019-11-01 14:57:00"",""IO_TYPE"":""O"",""IO_ITEM"":""自解量"",""RESULT"":2000.0,""UNIT"":""ml""},{""EXAM_DATE"":""2019-11-01 06:18:00"",""IO_TYPE"":""O"",""IO_ITEM"":""自解量"",""RESULT"":1340.0,""UNIT"":""ml""},{""EXAM_DATE"":""2019-10-31 23:00:00"",""IO_TYPE"":""O"",""IO_ITEM"":""留置導尿量"",""RESULT"":930.0,""UNIT"":""ml""},{""EXAM_DATE"":""2019-10-31 15:00:00"",""IO_TYPE"":""O"",""IO_ITEM"":""自解量"",""RESULT"":1100.0,""UNIT"":""ml""},{""EXAM_DATE"":""2019-10-31 06:10:00"",""IO_TYPE"":""O"",""IO_ITEM"":""自解量"",""RESULT"":1350.0,""UNIT"":""ml""},{""EXAM_DATE"":""2019-10-30 23:00:00"",""IO_TYPE"":""O"",""IO_ITEM"":""自解量"",""RESULT"":650.0,""UNIT"":""ml""},{""EXAM_DATE"":""2019-10-30 15:00:00"",""IO_TYPE"":""O"",""IO_ITEM"":""自解量"",""RESULT"":1250.0,""UNIT"":""ml""},{""EXAM_DATE"":""2019-10-30 06:30:00"",""IO_TYPE"":""O"",""IO_ITEM"":""自解量"",""RESULT"":200.0,""UNIT"":""ml""},{""EXAM_DATE"":""2019-11-03 15:00:00"",""IO_TYPE"":""O"",""IO_ITEM"":""自解量"",""RESULT"":400.0,""UNIT"":""ml""}]
"; 
                #endregion
                return JsonConvert.DeserializeObject<List<IO_PUT>>(_str);
            }
            else
            { 
                WS_GetInputOutput io = new WS_GetInputOutput(pChartNo, pIpdNo, pSDate, iwp, pEDate);
                return this.setReturnValue(HISData.getServiceResult(io));
            }

            #endregion 
        }

    }
 
    /// <summary>
    /// 取得單一病人相關檢查報告結果清單
    /// </summary>
    public class WS_GetInputOutput : AwebMethod<IO_PUT>, IwebMethod<IO_PUT>
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
        public WS_GetInputOutput(string pChartNo, string pIpdNo, string pSDate, IWebServiceParam pIwp, string pEDate = "")
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
                    this.paramList["sDate"].paramValue = mayaminer.com.library.DateHelper.Parse(pSDate).AddDays(-1).ToString("yyyy/MM/dd");
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
            this.ServiceResult = new ServiceResult<IO_PUT>();
            this.paramList = this.iwp.paramList;
            this.returnValue = this.iwp.returnValue;

        }
    }

    /// <summary>
    /// 輸入輸出資料
    /// </summary>
    public class IO_PUT
    {
        public string EXAM_DATE { get; set; }
        public string IO_TYPE { get; set; }
        public string I_RESULT { get; set; }
        public string O_RESULT { get; set; }
        public string IO_DESC
        {
            get
            {
                if (string.IsNullOrWhiteSpace(this.IO_TYPE))
                {
                    return "";
                }
                if (this.IO_TYPE == "I")
                {
                    return "輸入";
                }
                if (this.IO_TYPE == "O")
                {
                    return "輸出";
                }
                return "";
            }
        }
        public string IO_ITEM { get; set; }
        public string RESULT { get; set; }
        public string UNIT { get; set; }
        public string RESULT_DIFF { get; set; }

        public List<IO_PUT> IO_List { get; set; }
    }

    public class IO_PUT_VM
    {
        public string IO_DATE { get; set; }
        public string IO_TYPE { get; set; }
        public string IO_DESC { get; set; }
        public string RESULT { get; set; }

        public List<IO_PUT> IO_List { get; set; }
    }
    
}
using Newtonsoft.Json;
using RCSData.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace RCSData.Models
{
    /// <summary>
    /// 取得vital_sign
    /// </summary>
    public partial class WebMethod
    {
        /// <summary>取得vital_sign</summary>
        /// <param name="pChartNo">病歷號</param>
        /// <param name="pIpdNo">批價序號</param>
        /// <param name="pSDate">開始日期</param>
        /// <param name="pEDate">結束日期</param>
        /// <returns></returns>
        public List<VitalSign> getVitalSign(IWebServiceParam iwp,string pChartNo, string pIpdNo, string pSDate , string pEDate = "")
        {
            List<VitalSign> sqlVitalSignList = new List<VitalSign>();
            List<VitalSign> NewsqlVitalSignList = new List<VitalSign>();
            List<VitalSign> returnVitalSignList = new List<VitalSign>();
            VitalSign newVitalSignNode = new VitalSign();
            VitalSign previousVitalSignNode = new VitalSign();
            try
            {
                if (string.IsNullOrWhiteSpace(pEDate) )
                    pEDate = mayaminer.com.library.DateHelper.Parse(pEDate).ToString("yyyy-MM-dd");
                //取得 SQL 資料   
                if (isBasicMode || isDebuggerMode)
                {
                    string _str = @"
[{""EXAM_DATE"":""2018/10/16 09:00:00"",""TPR_ITEM"":""舒張壓"",""RESULT"":""78""},{""EXAM_DATE"":""2018/10/16 09:00:00"",""TPR_ITEM"":""收縮壓"",""RESULT"":""168""},{""EXAM_DATE"":""2018/10/16 09:00:00"",""TPR_ITEM"":""心跳"",""RESULT"":""79""},{""EXAM_DATE"":""2018/10/16 09:00:00"",""TPR_ITEM"":""體溫"",""RESULT"":""36.5""},{""EXAM_DATE"":""2018/10/16 09:49:27"",""TPR_ITEM"":""舒張壓"",""RESULT"":""78""},{""EXAM_DATE"":""2018/10/16 09:49:27"",""TPR_ITEM"":""收縮壓"",""RESULT"":""168""},{""EXAM_DATE"":""2018/10/16 09:49:27"",""TPR_ITEM"":""心跳"",""RESULT"":""79""},{""EXAM_DATE"":""2018/10/16 09:49:27"",""TPR_ITEM"":""體溫"",""RESULT"":""36.5""},{""EXAM_DATE"":""2018/10/16 13:20:11"",""TPR_ITEM"":""舒張壓"",""RESULT"":""83""},{""EXAM_DATE"":""2018/10/16 13:20:11"",""TPR_ITEM"":""收縮壓"",""RESULT"":""170""},{""EXAM_DATE"":""2018/10/16 13:20:11"",""TPR_ITEM"":""心跳"",""RESULT"":""89""},{""EXAM_DATE"":""2018/10/16 13:20:11"",""TPR_ITEM"":""體溫"",""RESULT"":""36.9""},{""EXAM_DATE"":""2018/10/16 15:00:00"",""TPR_ITEM"":""體重"",""RESULT"":""57.2""},{""EXAM_DATE"":""2018/10/16 16:10:29"",""TPR_ITEM"":""舒張壓"",""RESULT"":""89""},{""EXAM_DATE"":""2018/10/16 16:10:29"",""TPR_ITEM"":""收縮壓"",""RESULT"":""172""},{""EXAM_DATE"":""2018/10/16 16:10:29"",""TPR_ITEM"":""心跳"",""RESULT"":""78""},{""EXAM_DATE"":""2018/10/16 16:10:29"",""TPR_ITEM"":""體溫"",""RESULT"":""37""},{""EXAM_DATE"":""2018/10/16 20:11:50"",""TPR_ITEM"":""舒張壓"",""RESULT"":""79""},{""EXAM_DATE"":""2018/10/16 20:11:50"",""TPR_ITEM"":""收縮壓"",""RESULT"":""153""},{""EXAM_DATE"":""2018/10/16 20:11:50"",""TPR_ITEM"":""心跳"",""RESULT"":""83""},{""EXAM_DATE"":""2018/10/16 20:11:50"",""TPR_ITEM"":""體溫"",""RESULT"":""36.5""},{""EXAM_DATE"":""2018/10/17 09:17:10"",""TPR_ITEM"":""舒張壓"",""RESULT"":""91""},{""EXAM_DATE"":""2018/10/17 09:17:10"",""TPR_ITEM"":""收縮壓"",""RESULT"":""166""},{""EXAM_DATE"":""2018/10/17 09:17:10"",""TPR_ITEM"":""心跳"",""RESULT"":""75""},{""EXAM_DATE"":""2018/10/17 09:17:10"",""TPR_ITEM"":""體溫"",""RESULT"":""36""},{""EXAM_DATE"":""2018/10/17 14:19:27"",""TPR_ITEM"":""舒張壓"",""RESULT"":""94""},{""EXAM_DATE"":""2018/10/17 14:19:27"",""TPR_ITEM"":""收縮壓"",""RESULT"":""169""},{""EXAM_DATE"":""2018/10/17 14:19:27"",""TPR_ITEM"":""心跳"",""RESULT"":""93""},{""EXAM_DATE"":""2018/10/17 14:19:27"",""TPR_ITEM"":""體溫"",""RESULT"":""37""}]
";
                    sqlVitalSignList = JsonConvert.DeserializeObject<List<VitalSign>>(_str);
                }
                else
                {
                    WS_GetVitalSign io = new WS_GetVitalSign(pChartNo, pIpdNo, pSDate, iwp, pEDate);
                    sqlVitalSignList = this.setReturnValue(HISData.getServiceResult(io));
                }
                List<string> dateList = new List<string>();
                dateList = sqlVitalSignList.Select(x => x.EXAM_DATE).Distinct().ToList();
                foreach (string date in dateList)
                {
                    List<VitalSign> tempList = sqlVitalSignList.FindAll(x => x.EXAM_DATE == date).ToList();
                    VitalSign vs = new VitalSign();
                    vs.EXAM_DATE = date;
                    vs.RESULT_BW = tempList.Exists(x => x.TPR_ITEM == iwp.itemKey["RESULT_BW"]) ? tempList.First(x => x.TPR_ITEM == iwp.itemKey["RESULT_BW"]).RESULT : "" ;
                    vs.RESULT_SBP = tempList.Exists(x => x.TPR_ITEM == iwp.itemKey["RESULT_SBP"]) ? tempList.First(x => x.TPR_ITEM == iwp.itemKey["RESULT_SBP"]).RESULT : "";
                    vs.RESULT_HB = tempList.Exists(x => x.TPR_ITEM == iwp.itemKey["RESULT_HB"]) ? tempList.First(x => x.TPR_ITEM == iwp.itemKey["RESULT_HB"]).RESULT : "";
                    vs.RESULT_TEMP = tempList.Exists(x => x.TPR_ITEM == iwp.itemKey["RESULT_TEMP"]) ? tempList.First(x => x.TPR_ITEM == iwp.itemKey["RESULT_TEMP"]).RESULT : "";
                    vs.RESULT_DBP = tempList.Exists(x => x.TPR_ITEM == iwp.itemKey["RESULT_DBP"]) ? tempList.First(x => x.TPR_ITEM == iwp.itemKey["RESULT_DBP"]).RESULT : "";
                    vs.RESULT_gcs_e = tempList.Exists(x => x.TPR_ITEM == iwp.itemKey["RESULT_gcs_e"]) ? tempList.First(x => x.TPR_ITEM == iwp.itemKey["RESULT_gcs_e"]).RESULT : "";
                    vs.RESULT_gcs_m = tempList.Exists(x => x.TPR_ITEM == iwp.itemKey["RESULT_gcs_m"]) ? tempList.First(x => x.TPR_ITEM == iwp.itemKey["RESULT_gcs_m"]).RESULT : "";
                    vs.RESULT_gcs_v = tempList.Exists(x => x.TPR_ITEM == iwp.itemKey["RESULT_gcs_v"]) ? tempList.First(x => x.TPR_ITEM == iwp.itemKey["RESULT_gcs_v"]).RESULT : "";
                    returnVitalSignList.Add(vs);
                } 
            }//try
            catch (Exception ex)
            {
                Com.Mayaminer.LogTool.SaveLogMessage(ex, "getVitalSign", this.csName);
            }//try-catch
            returnVitalSignList = returnVitalSignList.OrderByDescending(x => x.EXAM_DATE).ToList();
            return returnVitalSignList;


        }

    }

    /// <summary>
    /// 取得單一病人相關檢查報告結果清單
    /// </summary>
    public class WS_GetVitalSign : AwebMethod<VitalSign>, IwebMethod<VitalSign>
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
        public WS_GetVitalSign(string pChartNo, string pIpdNo, string pSDate, IWebServiceParam pIwp, string pEDate = "")
        {
            this.iwp = pIwp;
            setParam();
            if (pChartNo != null && pIpdNo != null && pSDate != null &&
                    pChartNo.Length > 0 && pIpdNo.Length > 0 && pSDate.Length > 0)
            {
                //檢查參數3、4是否為正確日期
                if (!mayaminer.com.library.DateHelper.isDate(pSDate))
                {
                    this.ServiceResult.datastatus = RCS_Data.HISDataStatus.ParametersError;
                    this.ServiceResult.errorMsg = "開始日期格式錯誤!";
                }
                if (pEDate != null && pEDate != "" && !mayaminer.com.library.DateHelper.isDate(pEDate))
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
            this.ServiceResult = new ServiceResult<VitalSign>();
            this.paramList = this.iwp.paramList;
            this.returnValue = this.iwp.returnValue;


        }
    }



    /// <summary>
    /// vital_sign
    /// </summary>
    public class VitalSign
    {
        public string EXAM_DATE { get; set; }

        public string TPR_ITEM { get; set; }

        public string RESULT { get; set; }
        /// <summary>
        /// 舒張壓
        /// </summary>
        public string RESULT_DBP { get; set; }
        /// <summary>
        /// 收縮壓
        /// </summary>
        public string RESULT_SBP { get; set; }
        /// <summary>
        /// 心跳
        /// </summary>
        public string RESULT_HB { get; set; }
        /// <summary>
        /// 體溫
        /// </summary>
        public string RESULT_TEMP { get; set; }
        /// <summary>
        /// 體重
        /// </summary>
        public string RESULT_BW { get; set; }
        public string RESULT_gcs_e { get; set; }

        public string RESULT_gcs_v { get; set; }

        public string RESULT_gcs_m { get; set; }
        /// <summary>
        /// 呼吸
        /// </summary>
        public string RESULT_RESPIRATORY { get; set; }
        /// <summary>
        /// 脈搏
        /// </summary>
        public string RESULT_PULSE { get; set; }
    }

}
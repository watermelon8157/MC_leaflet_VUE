using Newtonsoft.Json;
using System;
using RCSData.Models;
using System.Collections.Generic;

namespace RCSData.Models
{

    public partial class WebMethod
    { 
        /// <summary>取得病人處方清單(7190)</summary>
        /// <param name="pChartNo">病歷號</param>
        /// <param name="pIpdNo">批價序號</param>
        /// <param name="pSDate">開始日期</param>
        /// <param name="pEDate">結束日期</param>
        /// <returns></returns>
        public List<PatProgress> get7190OrderList(IWebServiceParam iwp,string pSDate, string pEDate)
        {
            List<PatProgress> pList = new List<PatProgress>();  
            WS_ConsultList cl = new WS_ConsultList(pSDate, iwp, pEDate);
            ServiceResult< PatProgress> sr = HISData.getServiceResult(cl);
            this.datastatus = sr.datastatus;
            this.errorMsg = sr.errorMsg; 
            if (sr.datastatus == RCS_Data.HISDataStatus.SuccessWithData)
            {
                pList = JsonConvert.DeserializeObject<List<PatProgress>>(sr.returnJson);
            }
            return pList;
        }
    }
     
    /// <summary>
    /// 呼吸治療會診清單
    /// </summary>
    public class WS_ConsultList : AwebMethod< PatProgress>, IwebMethod< PatProgress>
    {
        public string webMethodName { get { return this.iwp.webMethodName; } }
        public override string wsSession { get { return "RCS_WS_BASIC"; } }
        /// <summary>
        /// 呼吸治療會診清單
        /// </summary>
        /// <param name="pChartNo"></param>
        /// <param name="pIpdNo"></param>
        /// <param name="pSDate"></param>
        /// <param name="pEDate"></param>
        /// <param name="pWsSession"></param>
        public WS_ConsultList(string pSDate, IWebServiceParam pIwp, string pEDate = "")
        {
            this.iwp = pIwp; 
            this.setParam();
            if (!string.IsNullOrWhiteSpace(pSDate))
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
                if (pSDate == null || pSDate.Length == 0) this.ServiceResult.errorMsg = "開始日期不可為空值!";
            }

        }

        public void setParam()
        {
            this.ServiceResult = new ServiceResult< PatProgress>();
            this.paramList = this.iwp.paramList;
            this.returnValue = this.iwp.returnValue;
        }
    }

    public abstract class PatProgressBasic
    {
        /// <summary>會診日期</summary>
        public string START_DATE { set; get; }
        /// <summary>會診時間</summary>
        public string START_TIME { set; get; }
        /// <summary>床號</summary>
        public string BED_NO { set; get; }
        /// <summary>病歷號</summary>
        public string CHART_NO { set; get; }

        /// <summary> 病人姓名 </summary>
        protected string _PT_NAME { set; get; }
        /// <summary> 病人姓名 </summary>
        public virtual string PT_NAME { set; get; }
        /// <summary>科別代碼</summary>
        public string DIV_NO { set; get; }
        /// <summary>科別簡稱</summary>
        public string DIV_SHORT_NAME { set; get; }
        /// <summary>醫師代碼</summary>
        public string VS_NO { set; get; }

        /// <summary> 醫師姓名 </summary>
        protected string _DOCTOR_NAME { set; get; }
        /// <summary> 醫師姓名 </summary>
        public virtual string DOCTOR_NAME { set; get; }
        /// <summary>住院編號</summary>
        public string ADMIT_NO { set; get; }
        /// <summary>住院切帳序號</summary>
        public string CUT_NO { set; get; }

        protected string _Prescription { set; get; }
        public virtual string Prescription { set; get; }

        /// <summary>
        /// 會診原因
        /// </summary>
        public string CONSULT_REASON { set; get; }
        /// <summary>
        /// 會診結果
        /// </summary>
        public string CONSULT_RESULT { set; get; }

    }

    /// <summary>
    /// 會診資料
    /// </summary>
    public class PatProgress : PatProgressBasic
    {
        
    }

}
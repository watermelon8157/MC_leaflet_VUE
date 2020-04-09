using Newtonsoft.Json;
using RCSData.Models;
using System;
using System.Linq;
using System.Collections.Generic;

namespace RCSData.Models
{
    public partial class WebMethod
    {
        /// <summary>取得交班單處方資料</summary>
        /// <param name="pChartNo">病歷號</param>
        /// <param name="pIpdNo">批價序號</param>
        /// <returns></returns>
        public List<PatOrder> getShiftOrderList(IWebServiceParam iwp,string pChartNo, string pIpdNo)
        {
            List<PatOrder> pList = new List<PatOrder>(); 
            WS_OrderList order = new WS_OrderList(pChartNo, pIpdNo, "", iwp);
            ServiceResult< PatOrder> sr = HISData.getServiceResult(order);
            this.datastatus = sr.datastatus;
            this.errorMsg = sr.errorMsg;
            if (sr.datastatus == RCS_Data.HISDataStatus.SuccessWithData)
            {
                pList = JsonConvert.DeserializeObject<List<PatOrder>>(sr.returnJson); 
            }
            pList = pList.OrderByDescending(x=>x.ORD_BGN).ToList();
            return pList;
        }

    }

    /// <summary>
    /// 取得病患處方簽
    /// </summary>
    public class WS_OrderList : AwebMethod< PatOrder>, IwebMethod< PatOrder>
    {
        public string webMethodName { get { return this.iwp.webMethodName; } }
        public override string wsSession { get { return "RCS_WS_BASIC"; } }
        /// <summary>
        /// 取的處方簽資料
        /// </summary>
        string OrderSdate { get { return DateTime.Now.AddDays(-7).ToString("yyyy-MM-dd"); } }
     
        /// <summary>
        /// 取得病患處方簽
        /// </summary>
        /// <param name="pChartNo"></param>
        /// <param name="pIpdNo"></param>
        /// <param name="pSDate"></param>
        /// <param name="pEDate"></param>
        /// <param name="pWsSession"></param>
        public WS_OrderList(string pChartNo, string pIpdNo, string pSDate, IWebServiceParam pIwp, string pEDate = "")
        {
            this.iwp = pIwp;
            this.setParam();
            if (string.IsNullOrWhiteSpace(pSDate))
                pSDate = OrderSdate;

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
            this.ServiceResult = new ServiceResult< PatOrder>();
            this.paramList = this.iwp.paramList;
            this.returnValue = this.iwp.returnValue; 
        }
    }


    /// <summary>
    /// 查詢處方
    /// </summary>
    public class PatOrder
    {
        /// <summary>執行時間</summary>
        public string ORD_BGN { set; get; }
        /// <summary>收費碼</summary>
        public string ITEM_COD { set; get; }
        /// <summary>名稱</summary>
        public string NAM { set; get; }
        /// <summary>數量</summary>
        public string QTY { set; get; }
        /// <summary>頻率</summary>
        public string USAGE1 { set; get; }
        /// <summary>結束時間</summary>
        public string MY_END_TIME { set; get; }
        /// <summary>批價序號</summary>
        public string ipd_no { set; get; }
        /// <summary>病歷號</summary>
        public string chart_no { set; get; }
        /// <summary>成本中心</summary>
        public string COST_CODE { set; get; }
        /// <summary>床號</summary>
        public string BED_NO { set; get; }
        /// <summary>病人姓名</summary>
        public string PAT_NAME { set; get; }
    }
}
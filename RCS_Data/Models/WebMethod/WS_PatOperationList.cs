using Newtonsoft.Json;
using RCSData.Models;
using RCS_Data;
using System.Collections.Generic;

namespace RCSData.Models
{
    public partial class WebMethod
    {
        /// <summary>取得單一病人手術清單</summary>
        /// <param name="pChartNo">病歷號</param>
        /// <param name="pIpdNo">批價序號</param>
        /// <returns></returns>
        public List<PatOperation> getPatOperationList(IWebServiceParam iwp ,string pChartNo, string pIpdNo)
        { 
            WS_PatOperationList po = new WS_PatOperationList(pChartNo, pIpdNo, iwp);
            ServiceResult< PatOperation> sr = HISData.getServiceResult(po); 
            List<PatOperation> dataList = new List<PatOperation>();
            if (sr.datastatus == HISDataStatus.SuccessWithData)
            {
                dataList = JsonConvert.DeserializeObject<List<PatOperation>>(sr.returnJson);
            }
            return dataList;
        }

    }
    /// <summary>
    /// 取得單一病人手術清單
    /// </summary>
    public class WS_PatOperationList : AwebMethod< PatOperation>, IwebMethod< PatOperation>
    {
        public string webMethodName { get { return this.iwp.webMethodName; } }

        public override string wsSession { get { return "RCS_WS_BASIC"; } }
 
        /// <summary>
        /// 取得單一病人手術清單
        /// </summary>
        /// <param name="pChartNo"></param>
        /// <param name="pIpdNo"></param>
        /// <param name="pWsSession"></param>
        public WS_PatOperationList(string pChartNo, string pIpdNo, IWebServiceParam pIwp )
        {
            this.iwp = pIwp;
            setParam();
            if (!string.IsNullOrWhiteSpace(pChartNo) && !string.IsNullOrWhiteSpace(pIpdNo))
            {
                #region 整理傳入參數
                this.paramList["ChartNo"].paramValue = pChartNo;
                this.paramList["ipdNo"].paramValue = pIpdNo;
                #endregion
            }
            else
            {
                this.ServiceResult.datastatus = RCS_Data.HISDataStatus.ParametersError;
                if (string.IsNullOrWhiteSpace(pChartNo))
                    this.ServiceResult.errorMsg = "病人病歷號不能為空值!";
                if (string.IsNullOrWhiteSpace(pIpdNo))
                    this.ServiceResult.errorMsg = "病人住院號不可為空值!";
            }
        }

        public void setParam()
        {
            this.ServiceResult = new ServiceResult< PatOperation>();
            this.paramList = this.iwp.paramList;
            this.returnValue = this.iwp.returnValue;
        }
    }

    /// <summary>
    /// 病患手術碼
    /// </summary>
    public class PatOperation
    {
        /// <summary>手術日期</summary>
        public string OP_DATE { set; get; }
        /// <summary>手術時間</summary>
        public string OP_TIME { set; get; }

        /// <summary>手術結束日期</summary>
        public string OP_END_DATE { set; get; }

        /// <summary>手術結束時間</summary>
        public string OP_END_TIME { set; get; }

        /// <summary>病歷號</summary>
        public string CHART_NO { set; get; }
        /// <summary>住院帳號</summary>
        public string IPD_ACCNO { set; get; }
        /// <summary>手術代碼</summary>
        public string OP_CODE { set; get; }
        /// <summary>描述</summary>
        public string DESCRIPTION { set; get; }

    }
}
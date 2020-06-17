using Newtonsoft.Json;
using RCS_Data;
using RCSData.Models;
using System.Collections.Generic;

namespace RCSData.Models
{
    public partial class WebMethod
    {
        /// <summary>取得病人資料清單</summary>
        /// <param name="pChartNo">病歷號</param>
        /// <param name="pIpdNo">批價序號</param>
        /// <param name="pAreaCode">護理站代碼</param>
        /// <returns></returns>
        public List<IPDPatientInfo> getERAndOPDListbyChartNo(string pChartNo, IWebServiceParam iwp)
        {
            string actioName = "getERAndOPDListbyChartNo";
            List<IPDPatientInfo> pList = new List<IPDPatientInfo>();
            if (iwp.WSIsOK)
            {
                WS_ERAndOPDListbyChartNo ph = new WS_ERAndOPDListbyChartNo(pChartNo, iwp);
                ServiceResult<IPDPatientInfo> sr = HISData.getServiceResult(ph);
                this.datastatus = sr.datastatus;
                this.errorMsg = sr.errorMsg;
                if (sr.datastatus == HISDataStatus.SuccessWithData)
                {
                    pList = JsonConvert.DeserializeObject<List<IPDPatientInfo>>(sr.returnJson);
                }
#if DEBUG
                pList.ForEach(x => x.source_type = string.IsNullOrWhiteSpace(x.source_type) ? "E" : x.source_type);
#endif  
            }
            return pList;
        }
    }

    /// <summary>
    /// 門診急診資料
    /// </summary>
    public class WS_ERAndOPDListbyChartNo : AwebMethod< IPDPatientInfo>, IwebMethod< IPDPatientInfo>
    {
        public string webMethodName { get { return this.iwp.webMethodName; } }

        public override string wsSession { get { return "RCS_WS_BASIC"; } }

        /// <summary>
        /// 單一病患住院歷程
        /// </summary>
        /// <param name="pChartNo"></param>
        /// <param name="pIpdNo"></param>
        /// <param name="pWsSession"></param>
        public WS_ERAndOPDListbyChartNo(string pChartNo, IWebServiceParam pIwp)
        {
            this.iwp = pIwp;
            setParam();
            if (!string.IsNullOrWhiteSpace(pChartNo))
            {

                #region 整理傳入參數
                this.paramList["ChartNo"].paramValue = pChartNo;
                #endregion
            }
            else
            {
                this.ServiceResult.datastatus = RCS_Data.HISDataStatus.ParametersError;
                if (string.IsNullOrWhiteSpace(pChartNo))
                    this.ServiceResult.errorMsg = "病人病歷號不能為空值!";
            }
        }

        public void setParam()
        {
            this.ServiceResult = new ServiceResult<IPDPatientInfo>();
            this.paramList = this.iwp.paramList;
            this.returnValue = this.iwp.returnValue;
        }
    }
     
}

using RCSData.Models;
using System.Collections.Generic;





namespace RCSData.Models
{
    /// <summary>
    /// 取得ICD診斷清單單
    /// </summary>
    public partial class WebMethod
    {
        /// <summary>取得ICD診斷清單單</summary>
        /// <param name="pIpdNo">批價序號</param>
        /// <returns></returns>
        public List<ICD10> getPatientICD10(string pIpdNo)
        {
            WS_GetPatientICD10 io = new WS_GetPatientICD10(pIpdNo: pIpdNo);
            return this.setReturnValue(HISData.getServiceResult(io));
        }

    }

    /// <summary>
    /// 單一病患住院歷程
    /// </summary>
    public class WS_GetPatientICD10 : AwebMethod<ICD10>, IwebMethod<ICD10>
    { 

        public string webMethodName { get { return "getPatientICD10"; } }
        public override string wsSession { get { return "RCS_WS_BASIC"; } }

        
        /// <summary>
        /// 單一病患住院歷程
        /// </summary>
        /// <param name="pChartNo"></param>
        /// <param name="pIpdNo"></param>
        /// <param name="pWsSession"></param>
        public WS_GetPatientICD10(string pIpdNo )
        { 
            setParam();
            if (!string.IsNullOrWhiteSpace(pIpdNo))
            {

                #region 整理傳入參數
                this.paramList["ipdNo"].paramValue = pIpdNo;
                #endregion
            }
            else
            {
                this.ServiceResult.datastatus = RCS_Data.HISDataStatus.ParametersError;
                if (string.IsNullOrWhiteSpace(pIpdNo))
                    this.ServiceResult.errorMsg = "病人病歷號不能為空值!";
            }
        }

        public void setParam()
        {
            this.ServiceResult = new ServiceResult<ICD10>();
            this.paramList = new Dictionary<string, webParam>();
            this.paramList.Add("ipdNo", new webParam() { paramName = "pIpdNo" }); 

            this.returnValue = new Dictionary<string, webParam>();
          
            this.returnValue.Add("diag_id", new webParam() { paramName = "diag_id" });// 
            this.returnValue.Add("icd_desc", new webParam() { paramName = "icd_desc" });// 
            this.returnValue.Add("chinese_icd_desc", new webParam() { paramName = "chinese_icd_desc" });// 
            this.returnValue.Add("sub_assess_id1", new webParam() { paramName = "sub_assess_id1" });// 
            this.returnValue.Add("sub_assess_id2", new webParam() { paramName = "sub_assess_id2" });// 
            this.returnValue.Add("sub_assess_id3", new webParam() { paramName = "sub_assess_id3" });// 
        
        }
 
    }
 
    /// <summary>
    /// ICD10
    /// </summary>
    public class ICD10
    {
        public string diag_id { get; set; }

        public string icd_desc { get; set; }

        public string chinese_icd_desc { get; set; }

        public string sub_assess_id1 { get; set; }

        public string sub_assess_id2 { get; set; }

        public string sub_assess_id3 { get; set; }
    }
}
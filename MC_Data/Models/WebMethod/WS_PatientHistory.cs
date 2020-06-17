using Newtonsoft.Json;
using RCSData.Models;
using RCS_Data;
using System;
using System.Collections.Generic;
using System.Linq;

namespace RCSData.Models
{
    public partial class WebMethod
    {
        /// <summary>
        /// 單一病患住院歷程
        /// </summary>
        /// <param name="pChartNo"></param>
        /// <returns></returns>
        public List<PatientHistory> getPatientHistory(IWebServiceParam iwp,string pChartNo, ref string ipd_no)
        {
            List<PatientHistory> pList = new List<PatientHistory>(); 
            WS_PatientHistory ph = new WS_PatientHistory( pChartNo, iwp);
            ServiceResult< PatientHistory> sr = HISData.getServiceResult(ph);
            this.datastatus = sr.datastatus;
            this.errorMsg = sr.errorMsg;
            if (sr.datastatus == HISDataStatus.SuccessWithData)
            {
                pList = JsonConvert.DeserializeObject<List<PatientHistory>>(sr.returnJson);
                if (pList.Count > 0)
                {
                    if (pList.Exists(x => string.IsNullOrWhiteSpace(x.outdate)))
                    {
                        pList.FindAll(x => string.IsNullOrWhiteSpace(x.outdate)).ForEach(x => x.outdate = DateTime.Now.ToString("yyyy-MM-dd 12:59:59"));
                    }
                    if (pList.Exists(x => x.indate != null && x.indate.Trim() != ""))
                        ipd_no = pList.FindAll(x => x.indate != null && x.indate.Trim() != "").OrderByDescending(x => DateTime.Parse(x.outdate)).ThenByDescending(x => DateTime.Parse(x.indate)).ToList()[0].IPD_NO;
#if DEBUG
                    ipd_no = pList[0].IPD_NO;
#endif
                }
            }
            return pList;
        }

    }

    /// <summary>
    /// 單一病患住院歷程
    /// </summary>
    public class WS_PatientHistory : AwebMethod< PatientHistory>, IwebMethod< PatientHistory>
    {
        public string webMethodName { get { return this.iwp.webMethodName; } }

        public override string wsSession { get { return "RCS_WS_BASIC"; } } 
         
        /// <summary>
        /// 單一病患住院歷程
        /// </summary>
        /// <param name="pChartNo"></param>
        /// <param name="pIpdNo"></param>
        /// <param name="pWsSession"></param>
        public WS_PatientHistory(string pChartNo, IWebServiceParam pIwp )
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
            this.ServiceResult = new ServiceResult< PatientHistory>();
            this.paramList = this.iwp.paramList;
            this.returnValue = this.iwp.returnValue;
        }
    }

    /// <summary>患者住院歷程</summary>
    public class PatientHistory
    {
        /// <summary>住院次數</summary>
        public string InTime { get; set; }
        /// <summary>入院日期</summary>
        public string indate { get; set; }
        /// <summary>出院日期</summary>
        public string outdate { get; set; }
        /// <summary>入院原因(主診斷)</summary>
        public string Description { get; set; }
        /// <summary>批價序號</summary>
        public string IPD_NO { get; set; }
        /// <summary>是否為住院中</summary>
        public string IpdFlag { get; set; }
        /// <summary>成本中心代碼</summary>
        public string CostCode { get; set; }
        /// <summary>科別</summary>
        public string DeptName { get; set; }
        /// <summary>科別代碼</summary>
        public string DeptCode { get; set; }
    }
}
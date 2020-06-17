using Newtonsoft.Json;
using RCS_Data;
using System.Collections.Generic;
using System.Linq;
using System.Data;
using System;
using mayaminer.com.library;

namespace RCSData.Models
{

    public partial class WebMethod
    {
        /// <summary>取得病人資料</summary>
        /// <returns></returns>
        public List<IPDPatientInfo> getPatientInfoListByIpdNo(IWebServiceParam pIwp, string pChartNo, string pIpdNo)
        {
            List<IPDPatientInfo> dataList = new List<IPDPatientInfo>();
            WS_PatientInfo ba = new WS_PatientInfo(pChartNo, pIpdNo, pIwp);
            ServiceResult<IPDPatientInfo> sr = HISData.getServiceResult(ba);

            if (sr.datastatus == HISDataStatus.SuccessWithData)
            {
                dataList = JsonConvert.DeserializeObject<List<IPDPatientInfo>>(sr.returnJson);
            }
            dataList.ForEach(x=>x.source_type ="I");
            return dataList;
        }

        public List<IPDPatientInfo> getPatientInfoListByAreaCode(IWebServiceParam pIwp, string pAreaCode)
        {
            List<IPDPatientInfo> dataList = new List<IPDPatientInfo>();
            WS_AreaPatientList ba = new WS_AreaPatientList(pAreaCode, pIwp);
            ServiceResult<IPDPatientInfo> sr = HISData.getServiceResult(ba);

            if (sr.datastatus == HISDataStatus.SuccessWithData)
            {
                dataList = JsonConvert.DeserializeObject<List<IPDPatientInfo>>(sr.returnJson);
            }
            dataList.ForEach(x => x.source_type = "I");
            dataList = dataList.OrderBy(x => x.bed_no).ToList();
            return dataList;
        }

        /// <summary>取得病人資料清單</summary>
        /// <param name="pChartNo">病歷號</param>
        /// <param name="pIpdNo">批價序號</param>
        /// <param name="pAreaCode">護理站代碼</param>
        /// <returns></returns>
        public List<IPDPatientInfo> getPatientInfoList(IWebServiceParam pIwp, string pChartNo, string pIpdNo, string pAreaCode = "")
        {
            string actioName = "getPatientInfoList"; 
            
            List<IPDPatientInfo> pList = new List<IPDPatientInfo>();
            ServiceResult< IPDPatientInfo> sr = new ServiceResult<IPDPatientInfo>();
            if (!string.IsNullOrWhiteSpace(pAreaCode))
            { 
                WS_AreaPatientList ap = new WS_AreaPatientList(pAreaCode, pIwp); 
                sr = HISData.getServiceResult(ap);
            }
            else
            { 
                WS_PatientInfo ph = new WS_PatientInfo(pChartNo, pIpdNo, pIwp);
                sr = HISData.getServiceResult(ph);
                if (sr.datastatus == HISDataStatus.ExceptionError)
                {
                    Com.Mayaminer.LogTool.SaveLogMessage(string.Concat("取得失敗:", pChartNo, ",", pIpdNo), actioName, this.csName);
                }
            }
            this.datastatus = sr.datastatus;
            this.errorMsg = sr.errorMsg;
            if (sr.datastatus == HISDataStatus.SuccessWithData)
            {
                pList = JsonConvert.DeserializeObject<List<IPDPatientInfo>>(sr.returnJson);
                pList.ForEach(x=>x.source_type = "I");
            }
            return pList;
        }


    }
    /// <summary>
    /// 取得病人資料清單
    /// </summary>
    public class WS_AreaPatientList : AwebMethod< IPDPatientInfo>, IwebMethod< IPDPatientInfo>
    {
        public string webMethodName { get { return this.iwp.webMethodName; } }
        public override string wsSession { get { return "RCS_WS_BASIC"; } }
        /// <summary>
        /// 取得病人資料清單
        /// </summary>
        /// <param name="pAreaCode"></param>
        public WS_AreaPatientList(string pAreaCode , IWebServiceParam pIwp)
        {
            this.iwp = pIwp;
            this.setParam();
            if (!string.IsNullOrWhiteSpace(pAreaCode))
            {
                #region 整理傳入參數
                this.paramList["areaCode"].paramValue = pAreaCode;
                #endregion
            }
            else
            {
                this.ServiceResult.datastatus = RCS_Data.HISDataStatus.ParametersError;
                if (string.IsNullOrWhiteSpace(pAreaCode))
                    this.ServiceResult.errorMsg = "護理站不可為空值!";
            }
        }

        public virtual void setParam()
        {
            this.ServiceResult = new ServiceResult< IPDPatientInfo>();
            this.paramList = this.iwp.paramList;
            this.returnValue = this.iwp.returnValue;
        }

        public override void setReturnValue()
        {
            base.setReturnValue(); 


        }
    }


    /// <summary>
    /// 取得病患基本資料
    /// </summary>
    public class WS_PatientInfo : AwebMethod< IPDPatientInfo>, IwebMethod< IPDPatientInfo>
    {
        string csName = "";
        public string webMethodName { get { return this.iwp.webMethodName; } }
        public override string wsSession { get { return "RCS_WS_BASIC"; } }

        /// <summary>
        /// 取得病患基本資料
        /// </summary>
        /// <param name="pChartNo"></param>
        /// <param name="pIpdNo"></param>
        /// <param name="pWsSession"></param>
        public WS_PatientInfo(string pChartNo, string pIpdNo, IWebServiceParam pIwp)
        {
            this.iwp = pIwp;
            setParam();
            if (!string.IsNullOrWhiteSpace(pChartNo) && !string.IsNullOrWhiteSpace(pIpdNo))
            {
                #region 整理傳入參數
                this.paramList["ipdNo"].paramValue = pIpdNo;
                this.paramList["chart_no"].paramValue = pChartNo;
                #endregion
            }
            else
            {
                this.ServiceResult.datastatus = RCS_Data.HISDataStatus.ParametersError;
                if (string.IsNullOrWhiteSpace(pChartNo))
                    this.ServiceResult.errorMsg = "病歷號不可為空值!";
                if (string.IsNullOrWhiteSpace(pIpdNo))
                    this.ServiceResult.errorMsg = "住院序號不可為空值!";
            }
        }

        public void setParam()
        {
            this.ServiceResult = new ServiceResult< IPDPatientInfo>();
            this.paramList = this.iwp.paramList;
            this.returnValue = this.iwp.returnValue; 
        }

        public override void setReturnValue()
        { 
            if (this.ServiceResult.datastatus == RCS_Data.HISDataStatus.SuccessWithData)
            {
                base.setReturnValue();
                foreach (DataRow dr in this.ServiceResult.returnList.Rows)
                {
                    dr["diag_date"] = this.getDate(dr["diag_date"], RCS_Data.Models.DATE_FORMAT.yyyy_MM_dd_HHmm_withSlash);
                    dr["pre_discharge_date"] = this.getDate(dr["pre_discharge_date"], RCS_Data.Models.DATE_FORMAT.yyyy_MM_dd_HHmm_withSlash);
                    dr["birth_day"] = this.getDate(dr["birth_day"], RCS_Data.Models.DATE_FORMAT.yyyy_MM_dd_withSlash); 
                }
            }
        }

        private string getDate(object dc, RCS_Data.Models.DATE_FORMAT dfm )
        {
            string temp = "";
            try
            { 
                if (!DBNull.ReferenceEquals(dc, DBNull.Value) && !string.IsNullOrWhiteSpace(dc.ToString()))
                {
                    // 10901150616 10902291739
                    temp = dc.ToString();
                    if (DateHelper.isDate(temp, "yyyyMMdd"))
                    {
                        return RCS_Data.Models.Function_Library.getDateString(DateHelper.ParseTWDate(temp), dfm);
                    }
                    if (DateHelper.isDate(temp, "yyyMMddHHmm"))
                    {
                        return RCS_Data.Models.Function_Library.getDateString(DateHelper.ParseTWDate(temp) , dfm);
                    }
                    if (temp.Substring(3, 4).Contains("0229"))
                    {
                        return RCS_Data.Models.Function_Library.getDateString(DateHelper.ParseTWDate(temp), dfm);  
                    } 
                    if (DateHelper.isDate(temp, "yyyMMdd"))
                    {
                        return RCS_Data.Models.Function_Library.getDateString(DateHelper.ParseTWDate(temp), dfm);
                    }
                }
            }
            catch (Exception ex)
            {
                Com.Mayaminer.LogTool.SaveLogMessage(ex, "getDate", "WS_PatientInfo");
                throw;
            } 
            return temp;
        }
    }


    /// <summary>
    /// 照護病患資訊
    /// </summary>
    public class IPDPatientInfo : PatientInfo
    {
        public List<string> statusList { get; set; }
    }
}
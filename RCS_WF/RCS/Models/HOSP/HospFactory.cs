using RCSData.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RCS.Models.HOSP
{
    public class HospFactory
    {
        private  string hosp_id { get { return Com.Mayaminer.IniFile.GetConfig("System", "HOSP_ID"); } }

        public WebServiceFactory webService { get; set; }

        public HospFactory()
        {
            this.webService = new WebServiceFactory();
        }
    }

    public class WebServiceFactory
    {
        private string hosp_id { get { return Com.Mayaminer.IniFile.GetConfig("System", "HOSP_ID"); } }

        /// <summary>
        /// 區域病人資料
        /// </summary>
        /// <returns></returns>
        public IWebServiceParam AreaPatientList()
        {
            switch (this.hosp_id)
            {
                case "WF":
                    return new HOSP.WF.WebService.AreaPatientList();  
            }
            return new RCSData.Models.WebService.AreaPatientList();
        }

        /// <summary>
        /// 病患基本資料
        /// </summary>
        /// <returns></returns>
        public IWebServiceParam HISPatientInfo()
        {
            switch (this.hosp_id)
            {
                case "WF":
                    return new HOSP.WF.WebService.HISPatientInfo(); 
                default:
                    break;
            }
            return new RCSData.Models.WebService.HISPatientInfo();
        }

        /// <summary>
        /// 取得單一病人A(V)BG檢驗資料清單 
        /// </summary>
        /// <returns></returns>
        public IWebServiceParam HISABGLabData()
        {
            switch (this.hosp_id)
            {
                case "WF":
                    return new HOSP.WF.WebService.HISABGLabData();  
                default: 
                    break;
            }
            return new RCSData.Models.WebService.HISABGLabData();
        }

        /// <summary>
        /// 取得單一病人取得血液生化資料檢驗資料清單
        /// </summary>
        /// <returns></returns>
        public IWebServiceParam HISBloodBiochemicalLabData()
        {
            switch (this.hosp_id)
            {
                case "WF":
                    return new HOSP.WF.WebService.HISBloodBiochemicalLabData(); 
                default:
                    break;
            }
            return new RCSData.Models.WebService.HISBloodBiochemicalLabData();
        }

        /// <summary>
        /// 登入者驗證
        /// </summary>
        /// <returns></returns>
        public IWebServiceParam HisLoginUser()
        {
            switch (this.hosp_id)
            {
                case "WF":
                    return new HOSP.WF.WebService.HisLoginUser(); 
                default:
                    break;
            }
            return new RCSData.Models.WebService.HisLoginUser();
        }

        /// <summary>
        /// 取得病人處方清單
        /// </summary>
        /// <returns></returns>
        public IWebServiceParam RCSConsultList()
        {
            switch (this.hosp_id)
            {
                case "WF":
                    return new HOSP.WF.WebService.RCSConsultList(); 
                default:
                    break;
            }
            return new RCSData.Models.WebService.RCSConsultList();
        }

        /// <summary>
        /// 取得輸入輸出資料
        /// </summary>
        /// <returns></returns>
        public IWebServiceParam HISInputOutput()
        {
            switch (this.hosp_id)
            {
                case "WF":
                    return new HOSP.WF.WebService.HISInputOutput(); 
                default:
                    break;
            }
            return new RCSData.Models.WebService.HISInputOutput();
        }

        /// <summary>
        /// 取得vital_sign
        /// </summary>
        /// <returns></returns>
        public IWebServiceParam HISVitalSign()
        {
            switch (this.hosp_id)
            {
                case "WF":
                    return new HOSP.WF.WebService.HISVitalSign(); 
                default:
                    break;
            }
            return new RCSData.Models.WebService.HISVitalSign();
        }
        /// <summary>
        /// 取得院內床位區域清單
        /// </summary>
        /// <returns></returns>
        public IWebServiceParam HisBedAreaList()
        {
            switch (this.hosp_id)
            {
                case "WF":
                    return new HOSP.WF.WebService.HisBedAreaList(); 
                default:
                    break;
            }
            return new RCSData.Models.WebService.HisBedAreaList();
        }
        /// <summary>
        /// 取得檢驗清單
        /// </summary>
        /// <returns></returns>
        public IWebServiceParam HISLabData()
        {
            switch (this.hosp_id)
            {
                case "WF":
                    return new HOSP.WF.WebService.HISLabData(); 
                default:
                    break;
            }
            return new RCSData.Models.WebService.HISLabData();
        }
        /// <summary>
        /// 取得交班單處方資料
        /// </summary>
        /// <returns></returns>
        public IWebServiceParam HISOrderList()
        {
            switch (this.hosp_id)
            {
                case "WF":
                    return new HOSP.WF.WebService.HISOrderList(); 
                default:
                    break;
            }
            return new RCSData.Models.WebService.HISOrderList();
        }
        /// <summary>
        /// 單一病患住院歷程
        /// </summary>
        /// <returns></returns>
        public IWebServiceParam HISPatientHistory()
        {
            switch (this.hosp_id)
            {
                case "WF":
                    return new HOSP.WF.WebService.HISPatientHistory(); 
                default:
                    break;
            }
            return new RCSData.Models.WebService.HISPatientHistory();
        }
        /// <summary>
        /// </summary>
        /// <returns></returns>
        public IWebServiceParam HISGetPatient_ER_OPD_LIST()
        {
            switch (this.hosp_id)
            {
                case "WF":
                    return new HOSP.WF.WebService.HISGetPatient_ER_OPD_LIST();
                default:
                    break;
            }
            return new RCSData.Models.WebService.HISGetPatient_ER_OPD_LIST();
        }
        /// <summary>
        /// 取得單一病人手術清單
        /// </summary>
        /// <returns></returns>
        public IWebServiceParam HISPatOperationList()
        {
            switch (this.hosp_id)
            {
                case "WF":
                    return new HOSP.WF.WebService.HISPatOperationList(); 
                default:
                    break;
            }
            return new RCSData.Models.WebService.HISPatOperationList();
        }
        /// <summary>
        /// 取得單一病人相關檢查報告結果清單
        /// </summary>
        /// <returns></returns>
        public IWebServiceParam HISReportData()
        {
            switch (this.hosp_id)
            {
                case "WF":
                    return new HOSP.WF.WebService.HISReportData(); 
                default:
                    break;
            }
            return new RCSData.Models.WebService.HISReportData();
        }
    }
}
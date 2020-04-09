using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RCSData.Models.WebService
{
    public class HISVitalSign : WebServiceParam
    {
        public override string webMethodName
        {
            get
            {
                return "getVitalSign";
            }
        }

        public override Dictionary<string, string> itemKey { get {
                Dictionary<string, string> pDic = new Dictionary<string, string>();
                pDic.Add("RESULT_DBP", "舒張壓");
                pDic.Add("RESULT_SBP", "收縮壓");
                pDic.Add("RESULT_HB", "心跳");
                pDic.Add("RESULT_TEMP", "體溫");
                pDic.Add("RESULT_BW", "體重");
                pDic.Add("RESULT_gcs_e", "gcs_e");
                pDic.Add("RESULT_gcs_m", "gcs_m");
                pDic.Add("RESULT_gcs_v", "gcs_v");
                return pDic;
            }
        }

        public HISVitalSign()
        {
            this.paramList = new Dictionary<string, webParam>();
            this.paramList.Add("chartNo", new webParam() { paramName = "pChartNo" });
            this.paramList.Add("ipdNo", new webParam() { paramName = "pIpdNo" });
            this.paramList.Add("sDate", new webParam() { paramName = "pSDate" });
            this.paramList.Add("eDate", new webParam() { paramName = "pEDate" });

            this.returnValue = new Dictionary<string, webParam>();
            this.returnValue.Add("EXAM_DATE", new webParam() { paramName = "EXAM_DATE" });// 
            this.returnValue.Add("TPR_ITEM", new webParam() { paramName = "TPR_ITEM" });// 
            this.returnValue.Add("RESULT", new webParam() { paramName = "RESULT" });// 
        }
    }
}
using System.Collections.Generic;
using RCSData.Models;

namespace RCS.Models.HOSP.WF.WebService
{
    public class HISVitalSign : RCSData.Models.WebService.HISVitalSign
    {
        public override Dictionary<string, string> itemKey
        {
            get
            {
                Dictionary<string, string> pDic = new Dictionary<string, string>();
                pDic.Add("RESULT_DBP", "dbp");
                pDic.Add("RESULT_SBP", "sbp");
                pDic.Add("RESULT_HB", "hr");
                pDic.Add("RESULT_TEMP", "bt");
                pDic.Add("RESULT_BW", "bw");
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
            this.returnValue.Add("TPR_ITEM", new webParam() { paramName = "TPR_TYPE" });// 
            this.returnValue.Add("RESULT", new webParam() { paramName = "RESULT" });// 
        }
    }
}
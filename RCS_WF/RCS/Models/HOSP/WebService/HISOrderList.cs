using System.Collections.Generic;
using RCSData.Models;

namespace RCS.Models.HOSP.WF.WebService
{
    public class HISOrderList : RCSData.Models.WebService.HISOrderList
    {
        public HISOrderList()
        {
            this.paramList = new Dictionary<string, webParam>();
            this.paramList.Add("ipdNo", new webParam() { paramName = "pIpdNo" });
            this.paramList.Add("chartNo", new webParam() { paramName = "pChartNo" });
            this.paramList.Add("sDate", new webParam() { paramName = "pSDate" });
            this.paramList.Add("eDate", new webParam() { paramName = "pEDate" });

            this.returnValue = new Dictionary<string, webParam>();
            this.returnValue.Add("ORD_BGN", new webParam() { paramName = "ORD_BGN" });//執行日期時間
            this.returnValue.Add("ITEM_COD", new webParam() { paramName = "ITEM_CODE" });//處方代碼(收費碼)。
            this.returnValue.Add("NAM", new webParam() { paramName = "NAM" });//處方名稱 
            this.returnValue.Add("QTY", new webParam() { paramName = "QTY" });//處方數量 
            this.returnValue.Add("UNIT", new webParam() { paramName = "UNIT" });//   
            this.returnValue.Add("USAGE1", new webParam() { paramName = "USAGE1" });//頻率 
            this.returnValue.Add("ROUTES_OF_ADMINISTRATION", new webParam() { paramName = "ROUTES_OF_ADMINISTRATION" }); 
            this.returnValue.Add("COST_CODE", new webParam() { paramName = "COST_CODE" });//成本中心
            this.returnValue.Add("BED_NO", new webParam() { paramName = "BED_NO" });////床號
            this.returnValue.Add("MY_END_TIME", new webParam() { paramName = "ORD_END_DATE" });//結束日期
            this.returnValue.Add("ipd_no", new webParam() { paramName = "IPD_NO" });//批價序號
            this.returnValue.Add("chart_no", new webParam() { paramName = "CHART_NO" });//病歷號 
            this.returnValue.Add("PAT_NAME", new webParam() { paramName = "PAT_NAME" });//病人姓名 
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RCSData.Models.WebService
{
    public class HISOrderList :WebServiceParam
    {
        public override string webMethodName
        {
            get
            {
                return "getOrderList";
            }
        }
        public HISOrderList()
        {
            this.paramList = new Dictionary<string, webParam>();
            this.paramList.Add("ipdNo", new webParam() { paramName = "pIpdNo" });
            this.paramList.Add("chartNo", new webParam() { paramName = "pChartNo" });
            this.paramList.Add("sDate", new webParam() { paramName = "pSDate" });
            this.paramList.Add("eDate", new webParam() { paramName = "pEDate" });

            this.returnValue = new Dictionary<string, webParam>();
            this.returnValue.Add("ORD_BGN", new webParam() { paramName = "ORD_START_DATE" });//執行日期時間
            this.returnValue.Add("ITEM_COD", new webParam() { paramName = "ITEM_CODE" });//處方代碼(收費碼)。
            this.returnValue.Add("NAM", new webParam() { paramName = "ORD_NAME" });//處方名稱 
            this.returnValue.Add("QTY", new webParam() { paramName = "ORD_QTY" });//處方數量 
            this.returnValue.Add("USAGE1", new webParam() { paramName = "ORD_FREQ" });//頻率
            this.returnValue.Add("MY_END_TIME", new webParam() { paramName = "ORD_END_DATE" });//結束日期
            this.returnValue.Add("ipd_no", new webParam() { paramName = "IPD_NO" });//批價序號
            this.returnValue.Add("chart_no", new webParam() { paramName = "CHART_NO" });//病歷號
            this.returnValue.Add("COST_CODE", new webParam() { paramName = "COST_CODE" });//成本中心
            this.returnValue.Add("BED_NO", new webParam() { paramName = "BED_NO" });////床號
            this.returnValue.Add("PAT_NAME", new webParam() { paramName = "PAT_NAME" });//病人姓名
        }
    }
}
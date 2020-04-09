using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RCSData.Models.WebService
{
    public class HISLabData : WebServiceParam
    {
        public override string webMethodName
        {
            get
            {
                return "getLabData";
            }
        }
        public HISLabData()
        {
            this.paramList = new Dictionary<string, webParam>();
            this.paramList.Add("ChartNo", new webParam() { paramName = "pChartNo" });
            this.paramList.Add("ipdNo", new webParam() { paramName = "pIpdNo" });
            this.paramList.Add("sDate", new webParam() { paramName = "pSDate" });
            this.paramList.Add("eDate", new webParam() { paramName = "pEDate" });

            this.returnValue = new Dictionary<string, webParam>();
            this.returnValue.Add("exam_date", new webParam() { paramName = "LAB_DATE" });//檢驗日期 
            this.returnValue.Add("name", new webParam() { paramName = "LAB_NAME" });//檢驗名稱 
            this.returnValue.Add("kind", new webParam() { paramName = "kind" });//檢驗結果(文字)
            this.returnValue.Add("result", new webParam() { paramName = "LAB_VALUE" });//檢驗結果(數值)
            this.returnValue.Add("unit", new webParam() { paramName = "LAB_VALUE_UNIT" });//檢驗結果單位
            this.returnValue.Add("lower_limit", new webParam() { paramName = "LVL" });//標準檢驗值低值
            this.returnValue.Add("upper_limit", new webParam() { paramName = "LVH" });//標準檢驗值高值
            this.returnValue.Add("home_code", new webParam() { paramName = "LabNo" });//檢驗單號(需唯一值)
            this.returnValue.Add("exam_type", new webParam() { paramName = "LabName" });//檢驗類別，依照院內定義
            this.returnValue.Add("phlebotomy", new webParam() { paramName = "phlebotomy" });//完成日期
            this.returnValue.Add("sflag", new webParam() { paramName = "sflag" });//判斷指標 一般、偏高、偏低
            this.returnValue.Add("spem", new webParam() { paramName = "Specimen" });//檢體類別(文字)
        }
    }
}
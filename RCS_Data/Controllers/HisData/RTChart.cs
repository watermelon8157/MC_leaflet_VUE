using System.Collections.Generic;
using RCS_Data.Models.DB;
using RCSData.Models;
using RCS_Data.Models.ViewModels;
using Newtonsoft.Json.Linq;
using System;
using System.Linq;
using System.Data;
using mayaminer.com.library;
using Com.Mayaminer;
using System.Text.RegularExpressions;

namespace RCS_Data.Controllers.HisData
{
    public partial class Models : BaseModels, RCS_Data.Controllers.HisData.Interface
    {
         public RESPONSE_MSG  RTChartDataList(string StartDate, string EndDate, string chart_no)
        {
            RESPONSE_MSG rm = new RESPONSE_MSG();
            List<RTCHartData> pList = new List<RTCHartData>();
            try
            {
                JObject jo = this.RTChartData(StartDate, EndDate, chart_no);
                string tempstr = Newtonsoft.Json.JsonConvert.SerializeObject(jo);
                //逐一轉換各屬性
                foreach (var p in jo.Properties())
                {
                    RTCHartData pItem = new RTCHartData() {  label = p.Name  , data = new List<RTCHartDataSet>()};
                    //原本Key/Value陣列方式表達選項?容
                    JObject a = p.Value as JObject;
                    //準備一個新物件以屬性儲放選項 
                    //將{ "Key":"..", "Value":"..."}視為JObject
                    // RTCHartDataJObject prtc = Newtonsoft.Json.JsonConvert.DeserializeObject<RTCHartDataJObject>(Newtonsoft.Json.JsonConvert.SerializeObject(a));
                    foreach (var b in a.Properties())
                    {
                        if (b.Name == "data")
                        { 
                            List<string> tempList = new List<string>();
                            JArray c = b.Value as JArray;  
                            foreach (JArray d in c)
                            {
                                RTCHartDataSet pd = new RTCHartDataSet();
                                int cnt = 0; 
                                foreach (JValue item in d)
                                {
                                    if (cnt == 0)
                                    {
                                        pd.x = double.Parse(item.ToObject<string>()); 
                                    }
                                    if (cnt == 1)
                                    {
                                        pd.y = double.Parse(item.ToObject<string>());
                                    } 
                                    cnt++;
                                }
                                if (cnt > 0)
                                {
                                    pItem.data.Add(pd);
                                }
                            } 
                        } 
                    }
                    pList.Add(pItem);
                }
                 
                rm.attachment = pList;
            }
            catch (Exception ex)
            {
                rm.status = RESPONSE_STATUS.EXCEPTION;
                rm.message = ex.Message;
                LogTool.SaveLogMessage(ex, "RTChartDataList",this.csName);
            } 
            return rm;
        }

        public JObject RTChartData(string StartDate, string EndDate, string chart_no)
        {
            JObject jo_main = new JObject();
            string actionName = "RTChartData";
            try
            {
                List<Dictionary<string, string>> Dt = new List<Dictionary<string, string>>();
                Dictionary<string, string> Tmep = null;
                DataTable ChartDt = this.getChartData(chart_no, StartDate, EndDate);
                if (ChartDt != null && ChartDt.Rows.Count > 0)
                {
                    foreach (DataRow ChartDr in ChartDt.Rows)
                    {
                        Tmep = new Dictionary<string, string>();
                        Tmep.Add("RECORD_ID", ChartDr["RECORD_ID"].ToString());
                        Tmep.Add("RECORDDATE", ChartDr["RECORDDATE"].ToString());
                        Tmep.Add("ITEM_NAME", ChartDr["ITEM_NAME"].ToString());
                        Tmep.Add("ITEM_VALUE", ChartDr["ITEM_VALUE"].ToString());
                        Dt.Add(Tmep);
                    }
                }
                Dt = Dt.OrderBy(x => DateTime.Parse(x["RECORDDATE"])).ToList();
                List<DB_RCS_SYS_PARAMS> trend_list = this.getRCS_SYS_PARAMS("system", "trend", @pStatus: "1"); 
                if (new RTChart_peak(ref trend_list).showItem) jo_main.Add(new RTChart_peak(ref Dt, ref trend_list).main_result);
                if (new RTChart_Plateau(ref trend_list).showItem) jo_main.Add(new RTChart_Plateau(ref Dt, ref trend_list).main_result);
                if (new RTChart_FiO2(ref trend_list).showItem) jo_main.Add(new RTChart_FiO2(ref Dt, ref trend_list).main_result);
                if (new RTChart_peep(ref trend_list).showItem) jo_main.Add(new RTChart_peep(ref Dt, ref trend_list).main_result);
                if (new RTChart_SPO2(ref trend_list).showItem) jo_main.Add(new RTChart_SPO2(ref Dt, ref trend_list).main_result);
                if (new RTChart_Respiration(ref trend_list).showItem) jo_main.Add(new RTChart_Respiration(ref Dt, ref trend_list).main_result);
                if (new RTChart_DeltaP(ref trend_list).showItem) jo_main.Add(new RTChart_DeltaP(ref Dt, ref trend_list).main_result);
                if (new RTChart_ETCO2(ref trend_list).showItem) jo_main.Add(new RTChart_ETCO2(ref Dt, ref trend_list).main_result);
                if (new RTChart_ventilation_total(ref trend_list).showItem) jo_main.Add(new RTChart_ventilation_total(ref Dt, ref trend_list).main_result);
                if (new RTChart_C(ref trend_list).showItem) jo_main.Add(new RTChart_C(ref Dt, ref trend_list).main_result);
                if (new RTChart_raw(ref trend_list).showItem) jo_main.Add(new RTChart_raw(ref Dt, ref trend_list).main_result);

            }
            catch (Exception ex)
            {
                LogTool.SaveLogMessage(ex, actionName, this.csName);
            }
            return jo_main;
        }

        public DataTable getChartData(string pChartNo, string pStartDate, string pEndDate)
        {
            string sql = @"SELECT A.RECORD_ID, A.RECORDDATE, B.ITEM_NAME, B.ITEM_VALUE FROM {3} A, {4} B
                           WHERE A.RECORD_ID = B.RECORD_ID AND A.CHART_NO = {0} AND RECORDDATE >= {1} AND RECORDDATE <= {2} ";
            sql = string.Format(sql, SQLDefend.SQLString(pChartNo), SQLDefend.SQLString(pStartDate), SQLDefend.SQLString(pEndDate), DB_TABLE_NAME.DB_RCS_RECORD_MASTER, DB_TABLE_NAME.DB_RCS_RECORD_DETAIL);

            DataTable dt = this.DBLink.DBA.getSqlDataTable(sql);
            return dt;
        }
    }


    public class RTCHartDataJObject
    {
        public string label { get; set; }
        public List<List<int>> data { get; set; }

    }
    public class RTCHartData 
    {
        public string label { get; set; }
        public List<RTCHartDataSet> data { get; set; }

    }

    public class RTCHartDataSet
    {
        public double x { get; set; }
        public double y { get; set; }

    }

    #region RTChart 
    /// <summary>
    /// 趨勢圖 介面
    /// </summary>
    public interface IRTChart
    {
        /// <summary>
        /// 項目代碼
        /// </summary>
        string itemID { get; }
        /// <summary>
        /// 項目名稱
        /// </summary>
        string itemName { get; }

        /// <summary>
        /// 顯示項目
        /// </summary>
        bool showItem { get; set; }

        /// <summary>
        /// 項目名稱
        /// </summary>
        JObject label { get; }

        /// <summary>
        /// 資料LList
        /// </summary>
        JArray data { get; set; }

        /// <summary>
        /// 回傳物件
        /// </summary>
        JProperty main_result { get; }

        #region methon

        /// <summary>
        /// data結果
        /// </summary>
        void dataResult(List<Dictionary<string, string>> pDataList);

        #endregion
    }

    /// <summary>
    /// 趨勢圖 繼承物件
    /// </summary>
    public abstract class ARTChart
    {
        public ARTChart(ref List<Dictionary<string, string>> dataList, ref List<DB_RCS_SYS_PARAMS> lableList)
        {
            this.data = new JArray();
            if (lableList.Exists(x => x.P_NAME == this.itemName))
                this.showItem = true;
            if (this.showItem)
            {
                this.dataResult(dataList);
            }
        }

        public ARTChart(ref List<DB_RCS_SYS_PARAMS> lableList)
        {
            if (lableList.Exists(x => x.P_NAME == this.itemName))
                this.showItem = true;
        }
        public virtual string itemName { get; }
        /// <summary>

        public virtual string itemID { get; }

        public virtual JObject label { get; }

        /// <summary>
        /// 檢查是否有此項目設定
        /// </summary>
        public bool showItem { get; set; }

        /// <summary>
        /// 資料LList
        /// </summary>
        public JArray data { get; set; }

        /// <summary>
        /// 回傳趨勢圖顯示結果
        /// </summary>
        public JProperty main_result
        {
            get
            {
                if (this.showItem)
                {
                    JObject item = label;
                    item.Add(new JProperty("data", data));
                    return new JProperty(this.itemID, item);
                }
                return null;
            }
        }
        /// <summary>
        /// data結果
        /// </summary>
        public virtual void dataResult(List<Dictionary<string, string>> pDataList)
        {
            string ITEM_VALUE = "";
            Dictionary<string, string> Tmep = null;
            DateTime RecordTime = DateTime.Now;
            Decimal TimeStamp = 0;
            List<string> ID_List = null;
            ID_List = pDataList.Select(x => x["RECORD_ID"]).Distinct().ToList();
            foreach (string ID in ID_List)
            {
                //取X的TimeStamp
                RecordTime = Convert.ToDateTime(pDataList.Find(x => x["RECORD_ID"] == ID)["RECORDDATE"]);
                double ts = (RecordTime - Convert.ToDateTime("1970-01-01 00:00:00").ToLocalTime()).TotalMilliseconds;
                TimeStamp = Convert.ToDecimal(ts);

                Tmep = pDataList.Find(x => x["RECORD_ID"] == ID && x["ITEM_NAME"].ToLower() == this.itemID);
                ITEM_VALUE = (Tmep != null) ? Tmep["ITEM_VALUE"] : "";
                Decimal ITEM_VALUE_v = 0;
                if (Decimal.TryParse(ITEM_VALUE, out ITEM_VALUE_v))
                {
                    this.data.Add(new JArray(TimeStamp, ITEM_VALUE_v));
                }
            }
        }
    }

    /// <summary>
    /// peak 項目
    /// </summary>
    public class RTChart_peak : ARTChart, IRTChart
    {
        public RTChart_peak(ref List<Dictionary<string, string>> dataList, ref List<DB_RCS_SYS_PARAMS> lableList) : base(ref dataList, ref lableList)
        {

        }

        public RTChart_peak(ref List<DB_RCS_SYS_PARAMS> lableList) : base(ref lableList)
        {

        }

        public override string itemName { get { return "pressure_peak"; } }
        /// <summary>

        public override string itemID { get { return "pressure_peak"; } }

        /// <summary>
        /// 項目名稱
        /// </summary>
        public override JObject label { get { return new JObject() { new JProperty("label", this.itemName) }; } }

        /// <summary>
        /// data結果
        /// </summary>
        public override void dataResult(List<Dictionary<string, string>> pDataList)
        {
            base.dataResult(pDataList);
        }

    }

    /// <summary>
    /// Plateau 項目
    /// </summary>
    public class RTChart_Plateau : ARTChart, IRTChart
    {
        public RTChart_Plateau(ref List<Dictionary<string, string>> dataList, ref List<DB_RCS_SYS_PARAMS> lableList) : base(ref dataList, ref lableList)
        {

        }

        public RTChart_Plateau(ref List<DB_RCS_SYS_PARAMS> lableList) : base(ref lableList)
        {

        }

        public override string itemName { get { return "pressure_plateau"; } }
        /// <summary>

        public override string itemID { get { return "pressure_plateau"; } }

        /// <summary>
        /// 項目名稱
        /// </summary>
        public override JObject label { get { return new JObject() { new JProperty("label", this.itemName) }; } }

        /// <summary>
        /// data結果
        /// </summary>
        public override void dataResult(List<Dictionary<string, string>> pDataList)
        {
            base.dataResult(pDataList);
        }

    }

    /// <summary>
    /// FiO2 項目
    /// </summary>
    public class RTChart_FiO2 : ARTChart, IRTChart
    {
        public RTChart_FiO2(ref List<Dictionary<string, string>> dataList, ref List<DB_RCS_SYS_PARAMS> lableList) : base(ref dataList, ref lableList)
        {

        }

        public RTChart_FiO2(ref List<DB_RCS_SYS_PARAMS> lableList) : base(ref lableList)
        {

        }

        public override string itemName { get { return "fio2_set"; } }
        /// <summary>

        public override string itemID { get { return "fio2_set"; } }

        /// <summary>
        /// 項目名稱
        /// </summary>
        public override JObject label { get { return new JObject() { new JProperty("label", "FiO<sub>2</sub>") }; } }

        /// <summary>
        /// data結果
        /// </summary>
        public override void dataResult(List<Dictionary<string, string>> pDataList)
        {
            base.dataResult(pDataList);
        }

    }

    /// <summary>
    /// peep 項目
    /// </summary>
    public class RTChart_peep : ARTChart, IRTChart
    {
        public RTChart_peep(ref List<Dictionary<string, string>> dataList, ref List<DB_RCS_SYS_PARAMS> lableList) : base(ref dataList, ref lableList)
        {

        }

        public RTChart_peep(ref List<DB_RCS_SYS_PARAMS> lableList) : base(ref lableList)
        {

        }

        public override string itemName { get { return "pressure_peep"; } }
        /// <summary>

        public override string itemID { get { return "pressure_peep"; } }

        /// <summary>
        /// 項目名稱
        /// </summary>
        public override JObject label { get { return new JObject() { new JProperty("label", this.itemName) }; } }

        /// <summary>
        /// data結果
        /// </summary>
        public override void dataResult(List<Dictionary<string, string>> pDataList)
        {
            base.dataResult(pDataList);
        }

    }
    /// <summary>
    /// SPO2 項目
    /// </summary>
    public class RTChart_SPO2 : ARTChart, IRTChart
    {
        public RTChart_SPO2(ref List<Dictionary<string, string>> dataList, ref List<DB_RCS_SYS_PARAMS> lableList) : base(ref dataList, ref lableList)
        {

        }

        public RTChart_SPO2(ref List<DB_RCS_SYS_PARAMS> lableList) : base(ref lableList)
        {

        }

        public override string itemName { get { return "spo2"; } }
        /// <summary>

        public override string itemID { get { return "spo2"; } }

        /// <summary>
        /// 項目名稱
        /// </summary>
        public override JObject label { get { return new JObject() { new JProperty("label", "SPO<sub>2</sub>") }; } }

        /// <summary>
        /// data結果
        /// </summary>
        public override void dataResult(List<Dictionary<string, string>> pDataList)
        {
            base.dataResult(pDataList);
        }

    }
    /// <summary>
    /// Respiration 項目
    /// </summary>
    public class RTChart_Respiration : ARTChart, IRTChart
    {
        public RTChart_Respiration(ref List<Dictionary<string, string>> dataList, ref List<DB_RCS_SYS_PARAMS> lableList) : base(ref dataList, ref lableList)
        {

        }

        public RTChart_Respiration(ref List<DB_RCS_SYS_PARAMS> lableList) : base(ref lableList)
        {

        }

        public override string itemName { get { return "rr"; } }
        /// <summary>

        public override string itemID { get { return "exp_tv"; } }

        /// <summary>
        /// 項目名稱
        /// </summary>
        public override JObject label { get { return new JObject() { new JProperty("label", this.itemName) }; } }

        /// <summary>
        /// data結果
        /// </summary>
        public override void dataResult(List<Dictionary<string, string>> pDataList)
        {
            base.dataResult(pDataList);
        }

    }
    /// <summary>
    /// DeltaP 項目
    /// </summary>
    public class RTChart_DeltaP : ARTChart, IRTChart
    {
        public RTChart_DeltaP(ref List<Dictionary<string, string>> dataList, ref List<DB_RCS_SYS_PARAMS> lableList) : base(ref dataList, ref lableList)
        {

        }

        public RTChart_DeltaP(ref List<DB_RCS_SYS_PARAMS> lableList) : base(ref lableList)
        {

        }

        public override string itemName { get { return "ΔP"; } }
        /// <summary>

        public override string itemID { get { return "pressure_pc"; } }//ΔP= (Pplatu - PEEP)

        /// <summary>
        /// 項目名稱
        /// </summary>
        public override JObject label { get { return new JObject() { new JProperty("label", this.itemName) }; } }

        /// <summary>
        /// data結果
        /// </summary>
        public override void dataResult(List<Dictionary<string, string>> pDataList)
        {
            base.dataResult(pDataList);
        }

    }
    /// <summary>
    /// ETCO2  項目
    /// </summary>
    public class RTChart_ETCO2 : ARTChart, IRTChart
    {
        public RTChart_ETCO2(ref List<Dictionary<string, string>> dataList, ref List<DB_RCS_SYS_PARAMS> lableList) : base(ref dataList, ref lableList)
        {

        }

        public RTChart_ETCO2(ref List<DB_RCS_SYS_PARAMS> lableList) : base(ref lableList)
        {

        }

        public override string itemName { get { return "ETCO2"; } }
        /// <summary>

        public override string itemID { get { return "etco2"; } }//ETCO2

        /// <summary>
        /// 項目名稱
        /// </summary>
        public override JObject label { get { return new JObject() { new JProperty("label", "ETCO<sub>2</sub>") }; } }

        /// <summary>
        /// data結果
        /// </summary>
        public override void dataResult(List<Dictionary<string, string>> pDataList)
        {
            base.dataResult(pDataList);
        }

    }
    /// <summary>
    /// ventilation_total  項目
    /// </summary>
    public class RTChart_ventilation_total : ARTChart, IRTChart
    {
        public RTChart_ventilation_total(ref List<Dictionary<string, string>> dataList, ref List<DB_RCS_SYS_PARAMS> lableList) : base(ref dataList, ref lableList)
        {

        }

        public RTChart_ventilation_total(ref List<DB_RCS_SYS_PARAMS> lableList) : base(ref lableList)
        {

        }

        public override string itemName { get { return "vr"; } }
        /// <summary>

        public override string itemID { get { return "vr"; } }//ETCO2

        /// <summary>
        /// 項目名稱
        /// </summary>
        public override JObject label { get { return new JObject() { new JProperty("label", this.itemName) }; } }

        /// <summary>
        /// data結果
        /// </summary>
        public override void dataResult(List<Dictionary<string, string>> pDataList)
        {
            base.dataResult(pDataList);
        }

    }

    /// <summary>
    /// raw  項目
    /// </summary>
    public class RTChart_raw : ARTChart, IRTChart
    {
        public RTChart_raw(ref List<Dictionary<string, string>> dataList, ref List<DB_RCS_SYS_PARAMS> lableList) : base(ref dataList, ref lableList)
        {

        }

        public RTChart_raw(ref List<DB_RCS_SYS_PARAMS> lableList) : base(ref lableList)
        {

        }

        public override string itemName { get { return "resistance"; } }
        /// <summary>

        public override string itemID { get { return "resistance"; } }//ETCO2

        /// <summary>
        /// 項目名稱
        /// </summary>
        public override JObject label { get { return new JObject() { new JProperty("label", this.itemName) }; } }

        /// <summary>
        /// data結果
        /// </summary>
        public override void dataResult(List<Dictionary<string, string>> pDataList)
        {
            base.dataResult(pDataList);
        }

    }

    /// <summary>
    /// C
    /// </summary>
    public class RTChart_C : ARTChart, IRTChart
    {
        public RTChart_C(ref List<Dictionary<string, string>> dataList, ref List<DB_RCS_SYS_PARAMS> lableList) : base(ref dataList, ref lableList)
        {

        }

        public RTChart_C(ref List<DB_RCS_SYS_PARAMS> lableList) : base(ref lableList)
        {

        }

        public override string itemName { get { return "Compliance"; } }
        /// <summary>

        public override string itemID { get { return "compliance"; } } 

        /// <summary>
        /// 項目名稱
        /// </summary>
        public override JObject label { get { return new JObject() { new JProperty("label", this.itemName) }; } }

        /// <summary>
        /// data結果
        /// </summary>
        public override void dataResult(List<Dictionary<string, string>> pDataList)
        {
            base.dataResult(pDataList);
        }

    }
    #endregion
}

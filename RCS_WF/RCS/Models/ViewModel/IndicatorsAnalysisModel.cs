using RCS_Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using mayaminer.com.library;

namespace RCS.Models
{
    /// <summary>
    /// 重要指標分析
    /// </summary>
    public class IndicatorsAnalysisModel
    {
        public IndicatorsAnalysisModel()
        {
            tableTh = new List<BoostrapTable>();
            SettableTh();
        }
        public IndicatorsAnalysisModel(string pStartDate,string pEndDate, string pFunctionType)
        {
            StartDate = pStartDate;
            EndDate = pEndDate;
            functionType = pFunctionType;
            resultChk = new List<string>();
            SettableTh();
        }

        private void SettableTh()
        {
            tableTh = new List<BoostrapTable>();
            Dictionary<string, List<string>> tempList = new Dictionary<string, List<string>>();
            tempList.Add("dataTitle", "開始日期,結束日期,來源,病歷號,病患,性別,年齡".ToString().Split(',').ToList());
            tempList.Add("dataField", "sDate,eDate,nam_c,chart_no,patient_name,genderCHT,age".ToString().Split(',').ToList());
            setTableTh(tempList);
        }
        /// <summary>
        /// 查詢日期訖
        /// </summary>
        public string EndDate { get; set; }

        /// <summary>
        /// 查詢日期起
        /// </summary>
        public string StartDate { get; set; }

        /// <summary>
        /// 功能類型
        /// </summary>
        public string functionType { get; set; }

        /// <summary>
        /// 治療結果顯示文字
        /// </summary>
        public string resultTitle { get; set; }

        /// <summary>
        /// 治療結果選項
        /// </summary>
        public List<string> resultChk { get; set; }

        public List<AnalysisData> AnalysisData { get; set; }

        #region  畫面BoostrapTable顯示
        /// <summary>
        /// 功能標題
        /// </summary>
        public string title { get; set; }
        /// <summary>
        /// 資料表抬頭
        /// </summary>
        public List<BoostrapTable> tableTh { get; set; }

        /// <summary>
        /// 設定資料表
        /// </summary>
        /// <param name="thCnt"></param>
        /// <param name="tempList"></param>
        public void setTableTh(Dictionary<string, List<string>> tempList)
        {
            if(tableTh == null)
            {
                tableTh = new List<BoostrapTable>();
            }
            for (int i = 0; i < tempList["dataField"].Count; i++)
            {
                BoostrapTable item = new BoostrapTable();
                item.dataField = tempList["dataField"][i];
                item.dataTitle = tempList["dataTitle"][i];
                tableTh.Add(item);
            }
        }
        #endregion


    }

    /// <summary>
    /// 指標分析表
    /// </summary>
    public class AnalysisData : PatientBasic
    {
        /// <summary>
        /// 插管、呼吸器、氧氣治療開始日期
        /// </summary>
        public string sDate { get; set; }
        /// <summary>
        ///  插管、呼吸器、氧氣治療結束日期
        /// </summary>
        public string eDate { get; set; }
        /// <summary>
        /// 治療結果
        /// </summary>
        public string result { get; set; }
        /// <summary>
        /// 結果清單
        /// </summary>
        public List<JSON_DATA> resultList { get; set; }
        /// <summary>
        /// 結果清單
        /// </summary>
        public List<List<JSON_DATA>> weaningResultList { get; set; }
        /// <summary>
        /// 使用天數/人日數
        /// </summary>
        public string daysUse {
            get
            {
                if (!string.IsNullOrWhiteSpace(sDate) && DateHelper.isDate(sDate))
                {
                    if (!string.IsNullOrWhiteSpace(eDate) && DateHelper.isDate(eDate))
                    {
                        return ((DateTime.Parse(eDate) - DateTime.Parse(sDate)).Days + 1).ToString();
                    }
                }
                return "";
            }
        }
        /// <summary>
        /// 脫離日期
        /// </summary>
        public string WeaningDate { get; set; }
    }
}
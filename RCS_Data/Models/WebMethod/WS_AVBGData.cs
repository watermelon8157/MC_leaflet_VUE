using System;
using RCSData.Models;
using System.Collections.Generic;

namespace RCSData.Models
{

    public partial class WebMethod
    { 
        /// <summary>取得單一病人A(V)BG檢驗資料清單(3天內)</summary>
        /// <param name="pChartNo">病歷號</param>
        /// <param name="pIpdNo">批價序號</param>
        /// <returns></returns>
        public List<ExamABG> getAVBGData(IWebServiceParam pIwp, string pChartNo, string pIpdNo)
        {
            List<ExamABG> pList = new List<ExamABG>(); 
            WS_AVBGData abg = new WS_AVBGData(pChartNo, pIpdNo, pIwp);
            ServiceResult<RCS_ExamData_Common> sr = HISData.getServiceResult(abg);
            this.datastatus = sr.datastatus;
            this.errorMsg = sr.errorMsg;
            if (sr.datastatus == RCS_Data.HISDataStatus.SuccessWithData)
            {
                pList = abg.ABGList;
                pList.ForEach(x=>x.Date = RCS_Data.Models.Function_Library.getDateString(DateTime.Parse(x.Date), RCS_Data.Models.DATE_FORMAT.yyyy_MM_dd_HHmm ));
            }
            return pList;
        }
 

    }


    /// <summary>
    /// 取得單一病人A(V)BG檢驗資料清單
    /// </summary>
    public class WS_AVBGData : WS_LabData
    {
        /// <summary>
        /// 取得ABGData資料 設定
        /// </summary>
        public Dictionary<string, string> AVBGValueList { get; set; }

        public List<ExamABG> ABGList { get; set; }

        public static string getAVBGDate { get { return DateTime.Now.AddDays(-3).ToString("yyyy-MM-dd"); } }

        /// <summary>
        /// 取得單一病人取得血液生化資料檢驗資料清單
        /// </summary>
        /// <param name="pChartNo"></param>
        /// <param name="pIpdNo"></param>
        /// <param name="pWsSession"></param>
        public WS_AVBGData( ) 
        {
            this.setParam();
        }


        /// <summary>
        /// 取得單一病人取得血液生化資料檢驗資料清單
        /// </summary>
        /// <param name="pChartNo"></param>
        /// <param name="pIpdNo"></param>
        /// <param name="pWsSession"></param>
        public WS_AVBGData(string pChartNo, string pIpdNo , IWebServiceParam iwp) : base(pChartNo, pIpdNo, getAVBGDate, iwp )
        { 

        }

        public override void setParam()
        {
            base.setParam();
            this.ABGList = new List<ExamABG>();
            #region AVBGValueList 參數設定
            AVBGValueList = this.iwp.itemKey;
            #endregion
        }


        public override void setReturnValue()
        {
            if (this.ServiceResult.datastatus == RCS_Data.HISDataStatus.SuccessWithData)
            {
                base.setReturnValue();
                //篩選檢驗日期
                List<string> dateList = new List<string>();
                List<RCS_ExamData_Common> temp = Newtonsoft.Json.JsonConvert.DeserializeObject<List<RCS_ExamData_Common>>(this.ServiceResult.returnJson);
                if (temp.Count > 0)
                {
                    //取的方式依據各家醫院資料來源修改
                    #region 取得檢驗資料中的AVBG項目資料
                    List<ExamABG> tempABGList = new List<ExamABG>();
                    getAVBValue(ref tempABGList, ref temp, AVBGValueList["pH"]);
                    getAVBValue(ref tempABGList, ref temp, AVBGValueList["PCO2"]);
                    getAVBValue(ref tempABGList, ref temp, AVBGValueList["PO2"]);
                    getAVBValue(ref tempABGList, ref temp, AVBGValueList["BaseExcess"]);
                    getAVBValue(ref tempABGList, ref temp, AVBGValueList["HCO3"]);
                    getAVBValue(ref tempABGList, ref temp, AVBGValueList["TotalCarbonDioxide"]);
                    getAVBValue(ref tempABGList, ref temp, AVBGValueList["SaO2"]);
                    #endregion
                    this.ABGList = tempABGList;
                }

            }
        }

        /// <summary>取得指定ABG項目值(依各醫院設定)</summary>
        /// <param name="ABGList">ABG檢驗資料</param>
        /// <param name="labData">檢驗資料</param>
        /// <param name="labName">顯示項目(LabName)</param>
        private void getAVBValue(ref List<ExamABG> ABGList, ref List<RCS_ExamData_Common> labData, string labName)
        {
            List<RCS_ExamData_Common> tempList = new List<RCS_ExamData_Common>();
            //檢驗分類設定
            if (labData.Exists(x => x.name != null && x.name.Trim() == labName))
                tempList = labData.FindAll(x => x.name != null && x.name.Trim() == labName);
            //檢驗資料名稱設定
            foreach (RCS_ExamData_Common item in tempList)
            {
                if (item.spem != null && item.spem.Trim() == AVBGValueList["SPEM"])
                {
                    if (!ABGList.Exists(x => x.Date == item.exam_date && x.spem == item.spem))
                    {
                        ABGList.Add(new ExamABG { Date = item.exam_date, spem = item.spem });
                    }

                    if (item.name == AVBGValueList["pH"])
                    {
                        #region pH
                        if (ABGList.Exists(x => x.Date == item.exam_date && x.spem == item.spem && string.IsNullOrWhiteSpace(x.pH)))
                        {
                            ABGList.Find(x => x.Date == item.exam_date && x.spem == item.spem
                            && string.IsNullOrWhiteSpace(x.pH)).pH = item.result;//pH
                        }
                        else
                        {
                            ABGList.Add(new ExamABG { Date = item.exam_date, spem = item.spem, pH = item.result });
                        }
                        #endregion
                    }

                    if (item.name == AVBGValueList["PCO2"])
                    {
                        #region PCO2
                        if (ABGList.Exists(x => x.Date == item.exam_date && x.spem == item.spem && string.IsNullOrWhiteSpace(x.PCO2)))
                        {
                            ABGList.Find(x => x.Date == item.exam_date && x.spem == item.spem
                            && string.IsNullOrWhiteSpace(x.PCO2)).PCO2 = item.result;//PCO2
                        }
                        else
                        {
                            ABGList.Add(new ExamABG { Date = item.exam_date, spem = item.spem, PCO2 = item.result });
                        }
                        #endregion
                        //ABGList.Find(x => x.Date == item.exam_date && x.spem == item.spem).PCO2 = item.result;
                    }
                    if (item.name == AVBGValueList["PO2"])
                    {
                        #region PO2
                        if (ABGList.Exists(x => x.Date == item.exam_date && x.spem == item.spem && string.IsNullOrWhiteSpace(x.PO2)))
                        {
                            ABGList.Find(x => x.Date == item.exam_date && x.spem == item.spem
                            && string.IsNullOrWhiteSpace(x.PO2)).PO2 = item.result;//PO2
                        }
                        else
                        {
                            ABGList.Add(new ExamABG { Date = item.exam_date, spem = item.spem, PO2 = item.result });
                        }
                        #endregion
                        //ABGList.Find(x => x.Date == item.exam_date && x.spem == item.spem).PO2 = item.result;
                    }

                    if (item.name == AVBGValueList["SaO2"])
                    {
                        #region SaO2
                        if (ABGList.Exists(x => x.Date == item.exam_date && x.spem == item.spem && string.IsNullOrWhiteSpace(x.SaO2)))
                        {
                            ABGList.Find(x => x.Date == item.exam_date && x.spem == item.spem
                            && string.IsNullOrWhiteSpace(x.SaO2)).SaO2 = item.result;//SaO2
                        }
                        else
                        {
                            ABGList.Add(new ExamABG { Date = item.exam_date, spem = item.spem, SaO2 = item.result });
                        }
                        #endregion
                        //ABGList.Find(x => x.Date == item.exam_date && x.spem == item.spem).SaO2 = item.result;
                    }

                    if (item.name == AVBGValueList["HCO3"])
                    {
                        #region HCO3
                        if (ABGList.Exists(x => x.Date == item.exam_date && x.spem == item.spem && string.IsNullOrWhiteSpace(x.HCO3)))
                        {
                            ABGList.Find(x => x.Date == item.exam_date && x.spem == item.spem
                            && string.IsNullOrWhiteSpace(x.HCO3)).HCO3 = item.result;//HCO3
                        }
                        else
                        {
                            ABGList.Add(new ExamABG { Date = item.exam_date, spem = item.spem, HCO3 = item.result });
                        }
                        #endregion
                        //ABGList.Find(x => x.Date == item.exam_date && x.spem == item.spem).HCO3 = item.result;
                    }

                    if (item.name == AVBGValueList["BaseExcess"])
                    {
                        #region BaseExcess
                        if (ABGList.Exists(x => x.Date == item.exam_date && x.spem == item.spem && string.IsNullOrWhiteSpace(x.BaseExcess)))
                        {
                            ABGList.Find(x => x.Date == item.exam_date && x.spem == item.spem
                            && string.IsNullOrWhiteSpace(x.BaseExcess)).BaseExcess = item.result;//BaseExcess
                        }
                        else
                        {
                            ABGList.Add(new ExamABG { Date = item.exam_date, spem = item.spem, BaseExcess = item.result });
                        }
                        #endregion
                        //ABGList.Find(x => x.Date == item.exam_date && x.spem == item.spem).BaseExcess = item.result;
                    }

                    if (item.name == AVBGValueList["TotalCarbonDioxide"])
                    {
                        #region TotalCarbonDioxide
                        if (ABGList.Exists(x => x.Date == item.exam_date && x.spem == item.spem && string.IsNullOrWhiteSpace(x.TotalCarbonDioxide)))
                        {
                            ABGList.Find(x => x.Date == item.exam_date && x.spem == item.spem
                            && string.IsNullOrWhiteSpace(x.TotalCarbonDioxide)).TotalCarbonDioxide = item.result;//TotalCarbonDioxide
                        }
                        else
                        {
                            ABGList.Add(new ExamABG { Date = item.exam_date, spem = item.spem, TotalCarbonDioxide = item.result });
                        }
                        #endregion
                        //ABGList.Find(x => x.Date == item.exam_date && x.spem == item.spem).TotalCarbonDioxide = item.result;
                    }

                }
            }
        }
    }

    /// <summary>
    /// 取得檢驗ABG
    /// </summary>
    public class ExamABG
    {
        /// <summary>
        /// 檢體別
        /// </summary>
        public string spem { get; set; }
        /// <summary>PH值</summary>
        public string pH { get; set; }
        /// <summary>PH值</summary>
        public string PCO2 { get; set; }
        public string PO2 { get; set; }
        public string BaseExcess { get; set; }
        public string HCO3 { get; set; }
        /// <summary>二氧化碳總量</summary>
        public string TotalCarbonDioxide { get; set; }
        public string SaO2 { get; set; }
        /// <summary>檢驗時間</summary>
        public string Date { get; set; }
        public string SEQNO { get; set; }

        /// <summary>Hb</summary>
        public string Hb { get; set; }
        /// <summary>Ht</summary>
        public string Ht { get; set; }
        /// <summary>WBC</summary>
        public string WBC { get; set; }
        /// <summary>Platelet</summary>
        public string Platelet { get; set; }
        /// <summary>Na</summary>
        public string Na { get; set; }
        /// <summary>K</summary>
        public string K { get; set; }
        /// <summary>Ca</summary>
        public string Ca { get; set; }
        /// <summary>Cl</summary>
        public string Cl { get; set; }
        /// <summary>BUN</summary>
        public string BUN { get; set; }
        /// <summary>Cr</summary>
        public string Cr { get; set; }
        /// <summary>Mg</summary>
        public string Mg { get; set; }
        /// <summary>Albumin</summary>
        public string Albumin { get; set; }

        public string pulse_oximeter { get; set; }
    }
}
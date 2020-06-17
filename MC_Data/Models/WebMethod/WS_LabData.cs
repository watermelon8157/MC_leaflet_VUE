using Newtonsoft.Json;
using RCSData.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Data;

namespace RCSData.Models
{
    public partial class WebMethod
    {
        /// <summary>取得檢驗清單</summary>
        /// <param name="pChartNo">病歷號</param>
        /// <param name="pIpdNo">批價序號</param>
        /// <param name="pSDate">開始日期</param>
        /// <param name="pEDate">結束日期</param>
        /// <returns></returns>
        public List<RCS_ExamData_Common> getLabData(IWebServiceParam iwp,string pChartNo, string pIpdNo, string pSDate, string pEDate = "")
        {
            List<RCS_ExamData_Common> pList = new List<RCS_ExamData_Common>(); 
            WS_LabData ld = new WS_LabData(pChartNo, pIpdNo, pSDate, iwp);
            ServiceResult< RCS_ExamData_Common> sr = HISData.getServiceResult(ld);
            this.datastatus = sr.datastatus;
            this.errorMsg = sr.errorMsg;
            if (sr.datastatus == RCS_Data.HISDataStatus.SuccessWithData)
            {
                pList = JsonConvert.DeserializeObject<List<RCS_ExamData_Common>>(sr.returnJson);
            }
            return pList;
        } 
    }

    /// <summary>
    /// 取得單一病人檢驗清單
    /// </summary>
    public class WS_LabData : AwebMethod< RCS_ExamData_Common>, IwebMethod< RCS_ExamData_Common>
    {
        public string webMethodName { get { return this.iwp.webMethodName; } }
        public override string wsSession { get { return "RCS_WS_BASIC"; } }


        /// <summary>
        /// 取得單一病人取得血液生化資料檢驗資料清單
        /// </summary>
        /// <param name="pChartNo"></param>
        /// <param name="pIpdNo"></param>
        /// <param name="pWsSession"></param>
        public WS_LabData()  
        {

        }

        /// <summary>
        /// 取得單一病人檢驗清單
        /// </summary>
        /// <param name="pChartNo"></param>
        /// <param name="pIpdNo"></param>
        /// <param name="pSDate"></param>
        /// <param name="pEDate"></param>
        /// <param name="pWsSession"></param>
        public WS_LabData(string pChartNo, string pIpdNo, string pSDate, IWebServiceParam pIwp, string pEDate = "")
        {
            this.iwp = pIwp;
            setParam();
            if (pChartNo != null && pIpdNo != null && pSDate != null &&
                    pChartNo.Length > 0 && pIpdNo.Length > 0 && pSDate.Length > 0)
            {
                //檢查參數3、4是否為正確日期
                if (!mayaminer.com.library.DateHelper.isDate(pSDate))
                {
                    this.ServiceResult.datastatus = RCS_Data.HISDataStatus.ParametersError;
                    this.ServiceResult.errorMsg = "開始日期格式錯誤!";
                }
                if (pEDate != null && pEDate != "" && !mayaminer.com.library.DateHelper.isDate(pEDate))
                {
                    this.ServiceResult.datastatus = RCS_Data.HISDataStatus.ParametersError;
                    if (this.ServiceResult.errorMsg != null && this.ServiceResult.errorMsg != "")
                        this.ServiceResult.errorMsg = "開始及結束日期格式錯誤!";
                    else
                        this.ServiceResult.errorMsg = "結束日期格式錯誤!";
                }
                //確認傳入值是否正確
                if (this.ServiceResult.datastatus != RCS_Data.HISDataStatus.ParametersError)
                {
                    this.paramList["ChartNo"].paramValue = pChartNo;
                    this.paramList["ipdNo"].paramValue = pIpdNo;
                    this.paramList["sDate"].paramValue = mayaminer.com.library.DateHelper.Parse(pSDate).ToString("yyyy/MM/dd");
                    if (pEDate != null && pEDate != "" && !mayaminer.com.library.DateHelper.isDate(pEDate, "yyyy-MM-dd"))
                        this.paramList["eDate"].paramValue = mayaminer.com.library.DateHelper.Parse(pEDate).ToString("yyyy/MM/dd");
                    else
                        this.paramList["eDate"].paramValue = DateTime.Now.ToString("yyyy/MM/dd");
                }
            }
            else
            {
                this.ServiceResult.datastatus = RCS_Data.HISDataStatus.ParametersError;
                if (pChartNo == null || pChartNo.Length == 0) this.ServiceResult.errorMsg = "病人病歷號不可為空值!";
                if (pIpdNo == null || pIpdNo.Length == 0) this.ServiceResult.errorMsg = "病人住院號不可為空值!";
                if (pSDate == null || pSDate.Length == 0) this.ServiceResult.errorMsg = "開始日期不可為空值!";
                if ((pChartNo == null || pChartNo.Length == 0) && (pIpdNo == null || pIpdNo.Length == 0)
                    && (pSDate == null || pSDate.Length == 0))
                    this.ServiceResult.errorMsg = "病人病歷號、住院號及開始日期不可為空值!";
            }
        }

        public virtual void setParam()
        {
            this.ServiceResult = new ServiceResult< RCS_ExamData_Common>();
            this.paramList = this.iwp.paramList;
            this.returnValue = this.iwp.returnValue;
        }

        public override void setReturnValue()
        {
            //如果有資料，對應系統資料結構
            if (this.ServiceResult.datastatus == RCS_Data.HISDataStatus.SuccessWithData)
            {
                foreach (KeyValuePair<string, webParam> item in this.returnValue)
                {
                    //如果有查詢到欄位資料，將欄位名稱取代
                    if (this.ServiceResult.returnList.Columns.Contains(item.Value.paramName))
                    {
                        this.ServiceResult.returnList.Columns[item.Value.paramName].ColumnName = item.Key;
                    }
                }
                foreach (DataRow dr in this.ServiceResult.returnList.Rows)
                {
                    foreach (DataColumn dc in this.ServiceResult.returnList.Columns)
                    {
                        if (!DBNull.ReferenceEquals(dr[dc.ColumnName], DBNull.Value))
                        {
                            dr[dc.ColumnName] = dr[dc.ColumnName].ToString().Trim();
                        }
                    }
                    if (this.iwp.settingResult)
                    {
                        if (dr["result"].ToString().Contains(" "))
                        {
                            List<string> tempList = dr["result"].ToString().Split(' ').ToList();
                            dr["result"] = tempList[0];
                            dr["unit"] = tempList[1]; 
                        } 
                    }
                } 
            }  
        }
    }


    

    /// <summary>
    /// 檢驗資料
    /// </summary>
    public class RCS_ExamData_Common
    {
        public string text { get { return string.Concat(this.name, " ", this.result, " ", this.unit); } }

        /// <summary> 檢驗日期(yyyy-MM-dd) </summary>
        public string exam_date { set; get; }

        /// <summary> 正常值上限 </summary>
        public string upper_limit { set; get; }

        /// <summary> 正常值下限 </summary>
        public string lower_limit { set; get; }

        /// <summary> 檢驗分類： commom、other (檢驗類別，依照院內定義) </summary>
        public string exam_type { set; get; }

        /// <summary> 檢驗項目名稱 </summary>
        public string name { set; get; }

        /// <summary> 檢驗結果 </summary>
        public string result { set; get; }

        /// <summary> 測量單位 </summary>
        public string unit { set; get; }

        /// <summary> 判斷指標 一般、偏高、偏低 </summary>
        public string sflag { set; get; }

        /// <summary> 種類:  一般、細菌、One Touch </summary>
        public string kind { set; get; }

        /// <summary> 完成日期 </summary>
        public string phlebotomy { set; get; }

        /// <summary> 檢驗分類 </summary>
        public string home_code { set; get; }

        /// <summary> 檢體 </summary>
        public string spem { set; get; }
        /// <summary>
        /// 檢驗項目排序
        /// </summary>
        public int examSort { set; get; }
        /// <summary>
        /// 檢體項目排序
        /// </summary>
        public int spemSort { set; get; }
    }

    public class RCS_ExamData_Item
    {
        public string name { set; get; }
        public string spem { set; get; }
        public string unit { set; get; }
    }

    public class RCS_ExamData_HomeCode
    {
        public string text { set; get; }
        public string value { set; get; }
    }

    

    public class RCS_ExamData_Set
    {

        public List<RCS_ExamData_Common> exam_data = new List<RCS_ExamData_Common>();

        public List<RCS_ExamData_HomeCode> home_set = new List<RCS_ExamData_HomeCode>();

        public List<string> commom_date_list = new List<string>();

        public List<RCS_ExamData_Item> commom_item_list = new List<RCS_ExamData_Item>();

        public List<string> one_date_list = new List<string>();

        public List<string> one_item_list = new List<string>();

        public List<string> virus_date_list = new List<string>();

        public List<string> virus_item_list = new List<string>();

        public void ProcessList()
        {
            this.home_set.Clear();
            List<RCS_ExamData_Common> tmp = this.exam_data.GroupBy(x => x.home_code, (key, group) => group.First()).ToList();

            List<RCS_ExamData_HomeCode> home_basic_list = new List<RCS_ExamData_HomeCode>();
            home_basic_list.Add(new RCS_ExamData_HomeCode() { text = "BI  生物化學", value = "BI" });
            home_basic_list.Add(new RCS_ExamData_HomeCode() { text = "CM  細胞標記", value = "CM" });
            home_basic_list.Add(new RCS_ExamData_HomeCode() { text = "CS  體液、痰、其他", value = "CS" });
            home_basic_list.Add(new RCS_ExamData_HomeCode() { text = "DP  藥物及毒物", value = "DP" });
            home_basic_list.Add(new RCS_ExamData_HomeCode() { text = "EN  內分泌學", value = "EN" });
            home_basic_list.Add(new RCS_ExamData_HomeCode() { text = "HE  血液學", value = "HE" });
            home_basic_list.Add(new RCS_ExamData_HomeCode() { text = "IC  感控檢驗", value = "IC" });
            home_basic_list.Add(new RCS_ExamData_HomeCode() { text = "RC  快速生化", value = "RC" });
            home_basic_list.Add(new RCS_ExamData_HomeCode() { text = "RU  快速尿液常規", value = "RU" });
            home_basic_list.Add(new RCS_ExamData_HomeCode() { text = "RH  快速血液學", value = "RH" });
            home_basic_list.Add(new RCS_ExamData_HomeCode() { text = "SI  血清免疫學", value = "SI" });
            home_basic_list.Add(new RCS_ExamData_HomeCode() { text = "ST  糞", value = "ST" });
            home_basic_list.Add(new RCS_ExamData_HomeCode() { text = "UR  尿", value = "UR" });
            home_basic_list.Add(new RCS_ExamData_HomeCode() { text = "VP  病毒及PCR", value = "VP" });
            home_basic_list.Add(new RCS_ExamData_HomeCode() { text = "ZZ  急作檢驗", value = "UR" });

            for (int i = 0; i <= tmp.Count - 1; i++)
            {
                if (tmp[i].home_code.Trim() != "")
                {
                    RCS_ExamData_HomeCode tmp_home = null;
                    for (int j = 0; j <= home_basic_list.Count - 1; j++)
                    {
                        if (home_basic_list[j].value == tmp[i].home_code.Trim())
                        {
                            tmp_home = home_basic_list[j];
                            tmp_home.value = tmp[i].home_code;
                            break;
                        }
                    }
                    if (tmp_home != null)
                    {
                        this.home_set.Add(tmp_home);
                    }
                    else
                    {
                        this.home_set.Add(new RCS_ExamData_HomeCode()
                        {
                            value = tmp[i].home_code,
                            text = tmp[i].home_code
                        });
                    }
                    tmp_home = null;
                }
            }


            // 處理一般 日期與項目
            tmp.Clear();
            this.commom_item_list.Clear();
            tmp = this.exam_data.FindAll(x => x.exam_type == "commom").GroupBy(x => x.name, (key, group) => group.First()).ToList();
            for (int i = 0; i <= tmp.Count - 1; i++)
            {
                if (tmp[i].phlebotomy.Trim() != "")
                {
                    this.commom_item_list.Add(new RCS_ExamData_Item()
                    {
                        name = tmp[i].name,
                        spem = tmp[i].spem,
                        unit = tmp[i].unit
                    });
                }
            }
            tmp.Clear();
            this.commom_date_list.Clear();
            tmp = this.exam_data.FindAll(x => x.exam_type == "commom").GroupBy(x => x.phlebotomy, (key, group) => group.First()).ToList();
            for (int i = 0; i <= tmp.Count - 1; i++)
            {
                if (tmp[i].phlebotomy.Trim() != "")
                {
                    this.commom_date_list.Add(tmp[i].phlebotomy);
                }
            }

            // 處理 One Touch 日期與項目
            tmp.Clear();
            this.one_date_list.Clear();
            tmp = this.exam_data.FindAll(x => x.exam_type == "other" && x.kind == "One Touch").GroupBy(x => x.phlebotomy, (key, group) => group.First()).ToList();
            for (int i = 0; i <= tmp.Count - 1; i++)
            {
                if (tmp[i].phlebotomy.Trim() != "")
                {
                    this.one_date_list.Add(tmp[i].phlebotomy);
                }
            }
            tmp.Clear();
            this.one_item_list.Clear();
            tmp = this.exam_data.FindAll(x => x.exam_type == "other" && x.kind == "One Touch").GroupBy(x => x.name, (key, group) => group.First()).ToList();
            for (int i = 0; i <= tmp.Count - 1; i++)
            {
                if (tmp[i].phlebotomy.Trim() != "")
                {
                    this.one_item_list.Add(tmp[i].name);
                }
            }

            // 處理細菌 日期與項目
            tmp.Clear();
            this.virus_date_list.Clear();
            tmp = this.exam_data.FindAll(x => x.exam_type == "other" && x.kind == "細菌").GroupBy(x => x.phlebotomy, (key, group) => group.First()).ToList();
            for (int i = 0; i <= tmp.Count - 1; i++)
            {
                if (tmp[i].phlebotomy.Trim() != "")
                {
                    this.virus_date_list.Add(tmp[i].phlebotomy);
                }
            }
            tmp.Clear();
            this.virus_item_list.Clear();
            tmp = this.exam_data.FindAll(x => x.exam_type == "other" && x.kind == "細菌").GroupBy(x => x.name, (key, group) => group.First()).ToList();
            for (int i = 0; i <= tmp.Count - 1; i++)
            {
                if (tmp[i].phlebotomy.Trim() != "")
                {
                    this.virus_item_list.Add(tmp[i].name);
                }
            }
            tmp.Clear();
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RCS_Data.Models.ViewModels;
using Newtonsoft.Json;
using RCS_Data.Models.DB;
using RCSData.Models;

namespace RCS_Data.Controllers.RtRecord
{
    // RtRecordPDF
    public partial class Models : BaseModels, Interface
    { 

        /// <summary>產生pdf列印資料</summary>
        /// <param name="recordListData">呼吸照護記錄ListJson</param>
        public void bindpdfModel(ref List<RT_RECORD_MAIN> recordListData, string ipd_no, string chart_no )
        { 
            if (recordListData != null && recordListData.Count > 0)
                recordListData = recordListData.OrderBy(x => x.RECORDDATE).ToList(); 
            string pDate = "";
            List<DB_RCS_WEANING_ASSESS_CHECKLIST> pList = new List<DB_RCS_WEANING_ASSESS_CHECKLIST>();
            DB_RCS_WEANING_ASSESS_CHECKLIST _row = new DB_RCS_WEANING_ASSESS_CHECKLIST(); 
            foreach (RT_RECORD_MAIN item in recordListData)
            {
                if (pDate != item.rt_record.recorddate)
                {
                    pDate = item.rt_record.recorddate;
                    pList.Add(this.getRWCA(chart_no, ipd_no, pDate).attachment);
                    if (pList.Count > 0)
                    {
                        if (!string.IsNullOrWhiteSpace(pList[0].CREATE_ID))
                        {
                            _row = pList[0];
                        }else
                        {
                            pList = new List<DB_RCS_WEANING_ASSESS_CHECKLIST>();
                            _row = null;
                        }
                    }
                    else
                    {
                        pList = new List<DB_RCS_WEANING_ASSESS_CHECKLIST>();
                        _row = null;
                    } 
                }
                if (_row != null && !string.IsNullOrWhiteSpace(_row.CREATE_ID) && _row.CREATE_ID.Trim() == item.CREATE_ID)
                {
                    List<string> _rwcaList = new List<string>();
                    if (!string.IsNullOrWhiteSpace(_row.RWAC01) && _row.RWAC01 == "1") _rwcaList.Add("1");
                    if (!string.IsNullOrWhiteSpace(_row.RWAC02) && _row.RWAC02 == "1") _rwcaList.Add("2");
                    if (!string.IsNullOrWhiteSpace(_row.RWAC03) && _row.RWAC03 == "1") _rwcaList.Add("3");
                    if (!string.IsNullOrWhiteSpace(_row.RWAC04) && _row.RWAC04 == "1") _rwcaList.Add("4");
                    if (!string.IsNullOrWhiteSpace(_row.RWAC05) && _row.RWAC05 == "1") _rwcaList.Add("5");
                    if (!string.IsNullOrWhiteSpace(_row.RWAC06) && _row.RWAC06 == "1") _rwcaList.Add("6");
                    if (!string.IsNullOrWhiteSpace(_row.RWAC07) && _row.RWAC07 == "1") _rwcaList.Add("7"); 
                    if (!string.IsNullOrWhiteSpace(_row.RWAC08))
                    {
                        item.pRWCA_memo_Plan = _row.RWAC08;
                    }  
                    if (!string.IsNullOrWhiteSpace(_row.RWAC09) && _row.RWAC09 == "1") _rwcaList.Add("8");
                    item.pRWCA_memo = string.Concat(" *", string.Join(",", _rwcaList));
                    pList = new List<DB_RCS_WEANING_ASSESS_CHECKLIST>();
                    _row = null;
                }else
                {
                    pList.Clear();
                }

            }
             
        }
         
    }

    /// <summary>呼吸照護記錄單PDFModel
    /// <para>IPDPatientInfo 病患基本資料</para>
    /// </summary>
    public class rtRecordPDFViewModel : IPDPatientInfo
    {
        public static int pageCnt = 5;

        /// <summary>
        /// 診斷內容
        /// </summary>
        public string diag_desc { get; set; }

        /// <summary>
        /// 列印清單
        /// </summary>
        public List<RT_RECORD_MAIN> data { get; set; }

        /// <summary>
        /// 列印清單
        /// </summary>
        public List<List<RT_RECORD_MAIN>> List { get {

                if (this.data == null)
                {
                    this.data = new List<RT_RECORD_MAIN>();
                }
                List<List<RT_RECORD_MAIN>> data = new List<List<RT_RECORD_MAIN>>();
                try
                {
                    int cntData = this.data.Count % pageCnt;
                    if (cntData != 0)
                    {
                        for (int i = 0; i < pageCnt - cntData; i++)
                        {
                            RT_RECORD_MAIN dt = new RT_RECORD_MAIN();
                            this.data.Add(dt);
                        }
                    }
                    List<RT_RECORD_MAIN> List = new List<RT_RECORD_MAIN>();
                    int Cnt = 1;
                    for (int i = 0; i < this.data.Count; i++)
                    {
                        RT_RECORD_MAIN item = this.data[i];
                        if (Cnt % pageCnt == 0)
                        {
                            List.Add(item);
                            data.Add(List);
                            List = new List<RT_RECORD_MAIN>();
                        }
                        else if (this.data.Count - 1 == i)
                        {
                            List.Add(item);
                            data.Add(List);
                        }
                        else
                        {
                            List.Add(item);
                        }
                        Cnt++;
                    }
                }
                catch (Exception ex)
                {

                    throw;
                }
                return data;
            }

        }

        /// <summary>
        /// 加入病患基本資料
        /// </summary>
        /// <param name="patData"></param>
        public void setBaseData(IPDPatientInfo patData, string pSource_type)
        {
            ipd_no = patData.ipd_no;
            chart_no = patData.chart_no;
            birth_day = patData.birth_day;
            patient_name = patData.patient_name;
            gender = patData.gender;
            source_type = pSource_type;
            bed_no = patData.bed_no;
            RCS_CPT_DTL_NEW_ITEMS cptData = new BaseModels().getCPTAssess(patData.chart_no);
            diagnosis_code = cptData.now_pat_diagnosis;
            cost_code = patData.cost_code;
        }

        /// <summary>
        /// 顯示勾選符號(布林值)
        /// </summary>
        /// <param name="isChecked"></param>
        /// <returns></returns>
        public string showCheckBox(bool isChecked = false)
        {
            string returnVal = "□";
            if (isChecked)
            {
                return "■";
            }

            return returnVal;
        }
        /// <summary>
        /// 顯示勾選符號(指定值)
        /// </summary>
        /// <param name="ischeckVal"></param>
        /// <param name="checkVal"></param>
        /// <returns></returns>
        public string showCheckBox(string ischeckVal, string checkVal)
        {
            string returnVal = "□";
            if (ischeckVal != null && ischeckVal.Trim() == checkVal)
            {
                return "■";
            }
            return returnVal;
        }

        /// <summary>
        /// 如[1]組有資料則顯示[1]組VALUE，否則顯示[2]組VALUE
        /// </summary>
        /// <param name="value1">[1]組value1</param>
        /// <param name="value2">[1]組value2</param>
        /// <param name="value3">[2]組value3</param>
        /// <param name="value4">[2]組value4</param>
        public static void setGroupValue(ref string value1, ref string value2, string value3, string value4)
        {
            if (string.IsNullOrWhiteSpace(value1) && string.IsNullOrWhiteSpace(value2))
            {
                value1 = value3;
                value2 = value4;
            }
        }

    }
}

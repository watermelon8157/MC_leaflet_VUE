using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using RCS_Data;
using mayaminer.com.library;
using Com.Mayaminer;
using Newtonsoft.Json;
using RCSData.Models;
using RCS_Data.Models.ViewModels;
using RCS_Data.Models.DB;

namespace RCS.Models.DB
{
    public class CPT : BaseModel
    { 
        #region CPT評估表

        
        /// <summary>
        /// 取得CPT評估表主檔
        /// </summary>
        /// <param name="pWhere">查詢條件</param>
        /// <returns></returns>
        public DataTable getCPTAssessMaster(string pWhere)
        {
            string sql = "SELECT * FROM {1} {0} ORDER BY ADMISSION_DATE DESC";
            DataTable dt = this.DBA.getSqlDataTable(string.Format(sql, pWhere, GetTableName.RCS_CPT_ASS_MASTER.ToString()));
            return dt;
        }
 

        /// <summary>
        /// 取得CPT評估表明細檔
        /// </summary>
        /// <param name="pWhere">查詢條件</param>
        /// <returns></returns>
        public DataTable getCPTAssessDetail(string pWhere)
        {
            DataTable dt = this.DBA.getDataTable(GetTableName.RCS_CPT_ASS_DETAIL.ToString(), pWhere);
            return dt;
        }

        /// <summary>
        /// 取得CPT評估表明細檔欄位值 [Read讀取]
        /// </summary>
        /// <param name="pCptId">評估表序號</param>
        /// <returns></returns>
        public DataTable getCPTAssessDetailNewItems(string pCptId)
        {
            string sql =
            @"  SELECT {0} AS CPT_ID
                    ,(
                        SELECT datastatus 
                        FROM RCS_CPT_ASS_MASTER 
                        WHERE CPT_ID = {0}
                    ) datastatus
                    ,(
                        SELECT upload_status 
                        FROM RCS_CPT_ASS_MASTER 
                        WHERE CPT_ID = {0}
                    ) upload_status
                    ,*
                FROM
                    (
                        SELECT CPT_ITEM, CPT_VALUE 
                        FROM {1} 
                        WHERE CPT_ID = {0}
                    ) AS D
                PIVOT
                    (
                        MAX(CPT_VALUE) FOR CPT_ITEM IN 
                        (
                            [VPN_mechanism]
                            ,[diag_date]
                            ,[record_date]
                            ,[from_unit]
                            ,[cpt_history]
                            ,[smoke_history]
                            ,[now_pat_diagnosis]
                            ,[diagnosis]
                            ,[history_diag]
                            ,[other_history]
                            ,[rt_reason]
                            ,[brief_status]
                            ,[operation]
                            ,[operation_memo]
                            ,[hocus]
                            ,[abg_data]
                            ,[lung]
                            ,[lung_conclusion]
                            ,[base_data]
                            ,[conscious]
                            ,[tip]
                            ,[skin]
                            ,[tube]
                            ,[machine]
                            ,[patterns]
                            ,[atelectasis]
                            ,[breath_sound]
                            ,[cough]
                            ,[sputum]
                            ,[sputum_assess]
                            ,[pat_problem]
                            ,[cpt_memo]
                            ,[CXR_result_json]
                            ,[thorax]
                        )
                    ) AS C";
            sql = string.Format(sql, SQLDefend.SQLString(pCptId), GetTableName.RCS_CPT_ASS_DETAIL.ToString());
            DataTable return_dt = this.DBA.getSqlDataTable(sql);
            return return_dt;
        }//取得CPT評估表明細檔欄位值 [Read讀取]

        /// <summary>
        /// 取得CPT評估表明細檔欄位
        /// </summary>
        /// <returns></returns>
        public string[] getCPTAssessDetailColumns()
        {
            string[] CPTArray = 
            {
                "record_date"
                , "from_unit"
                , "cpt_history"
                , "smoke_history"
                , "now_pat_diagnosis"
                , "diagnosis"
                , "history_diag"
                , "other_history"
                , "rt_reason"
                , "operation"
                , "operation_memo"
                , "hocus"
                , "abg_data"
                , "lung"
                , "lung_conclusion"
                , "base_data"
                , "conscious"
                , "tip"
                , "skin"
                , "tube"
                , "machine"
                , "patterns"
                , "atelectasis"
                , "breath_sound"
                , "cough"
                , "sputum"
                , "sputum_assess"
                , "pat_problem"
                , "cpt_memo"
                ,"thorax"
            };
            return CPTArray;
        }

        /// <summary>
        /// 取得住院號床號下拉選單來源
        /// </summary>       
        /// <returns></returns>
        public List<RCS_DATA_CR_MASTER> getCPTAssessIpdBed(string pChart_no)
        {
            string sql = string.Format("SELECT (RECORD_DATE + '/' + BED_NO + ':' + CAST (ROW_NUMBER() OVER(ORDER BY CPT_ID) AS VARCHAR(3))) AS IPD_NO, CREATE_NAME, CPT_ID FROM {1} WHERE CHART_NO = {0} AND DATASTATUS not in('2','9') ORDER BY CPT_ID DESC", SQLDefend.SQLString(pChart_no), GetTableName.RCS_CPT_ASS_MASTER.ToString());
            DataTable dt = this.DBA.getSqlDataTable(sql);
            if (dt != null && dt.Rows.Count > 0)
                return dt.ToList<RCS_DATA_CR_MASTER>();
            else
                return new List<RCS_DATA_CR_MASTER>();
        }

        /// <summary>
        /// 取得CPT評估單編號對應之住院號及病歷號
        /// </summary>
        /// <param name="pCptId">評估單編號</param>
        /// <returns></returns>
        public DataTable getCPTAssessIpdNoChartNo(string pCptId)
        {
            DataTable Dt = this.DBA.getSqlDataTable(string.Format("SELECT IPD_NO, CHART_NO FROM {1} WHERE CPT_ID = {0}", SQLDefend.SQLString(pCptId), GetTableName.RCS_CPT_ASS_MASTER.ToString()));
            return Dt;
        }

        /// <summary>
        /// 取得意識EVM下拉選單來源
        /// </summary>
        /// <param name="pType">意識類型</param>
        /// <returns></returns>
        public List<SysParams> getConsciousEVM(string pType)
        {
            List<SysParams> ConsciousList = new List<SysParams>();
            switch (pType)
            {
                case "E":
                    ConsciousList.Add(new SysParams() { P_VALUE = "", P_NAME = "" });
                    ConsciousList.Add(new SysParams() { P_VALUE = "1", P_NAME = "1" });
                    ConsciousList.Add(new SysParams() { P_VALUE = "2", P_NAME = "2" });
                    ConsciousList.Add(new SysParams() { P_VALUE = "3", P_NAME = "3" });
                    ConsciousList.Add(new SysParams() { P_VALUE = "4", P_NAME = "4" });
                    break;
                case "V":
                    ConsciousList.Add(new SysParams() { P_VALUE = "", P_NAME = "" });
                    ConsciousList.Add(new SysParams() { P_VALUE = "1", P_NAME = "1" });
                    ConsciousList.Add(new SysParams() { P_VALUE = "2", P_NAME = "2" });
                    ConsciousList.Add(new SysParams() { P_VALUE = "3", P_NAME = "3" });
                    ConsciousList.Add(new SysParams() { P_VALUE = "4", P_NAME = "4" });
                    ConsciousList.Add(new SysParams() { P_VALUE = "5", P_NAME = "5" });
                    ConsciousList.Add(new SysParams() { P_VALUE = "E", P_NAME = "E" });
                    ConsciousList.Add(new SysParams() { P_VALUE = "Tr", P_NAME = "T" });
                    break;
                case "M":
                    ConsciousList.Add(new SysParams() { P_VALUE = "", P_NAME = "" });
                    ConsciousList.Add(new SysParams() { P_VALUE = "1", P_NAME = "1" });
                    ConsciousList.Add(new SysParams() { P_VALUE = "2", P_NAME = "2" });
                    ConsciousList.Add(new SysParams() { P_VALUE = "3", P_NAME = "3" });
                    ConsciousList.Add(new SysParams() { P_VALUE = "4", P_NAME = "4" });
                    ConsciousList.Add(new SysParams() { P_VALUE = "5", P_NAME = "5" });
                    ConsciousList.Add(new SysParams() { P_VALUE = "6", P_NAME = "6" });
                    break;
            }
            return ConsciousList;
        }

        #endregion

        #region CPT紀錄表

       

        /// <summary>
        /// 取得執行治療項目Drug Inhalation下拉選單來源
        /// </summary>
        /// <returns></returns>
        public List<SysParams> getDrugInhalation()
        {
            List<SysParams> DrugInhalation = new List<SysParams>();
            DrugInhalation.Add(new SysParams() { P_VALUE = "", P_NAME = "" });
            DrugInhalation.Add(new SysParams() { P_VALUE = "A", P_NAME = "A:Atrovent" });
            DrugInhalation.Add(new SysParams() { P_VALUE = "B", P_NAME = "B:Bricanyl" });
            DrugInhalation.Add(new SysParams() { P_VALUE = "C", P_NAME = "C:Combivent" });
            DrugInhalation.Add(new SysParams() { P_VALUE = "F", P_NAME = "F:Flumucil" });
            DrugInhalation.Add(new SysParams() { P_VALUE = "M", P_NAME = "M:Mistabron" });
            DrugInhalation.Add(new SysParams() { P_VALUE = "V", P_NAME = "V:Ventolin" });
            DrugInhalation.Add(new SysParams() { P_VALUE = "N/S", P_NAME = "N/S:Normal Saline" });
            DrugInhalation.Add(new SysParams() { P_VALUE = "P", P_NAME = "P:Pulmicort" });
            DrugInhalation.Add(new SysParams() { P_VALUE = "H/S", P_NAME = "H/S:Half Saline" });
            DrugInhalation.Add(new SysParams() { P_VALUE = "colistin", P_NAME = "colistin" });
            DrugInhalation.Add(new SysParams() { P_VALUE = "Gentamycine", P_NAME = "Gentamycine" });
            DrugInhalation.Add(new SysParams() { P_VALUE = "Bosmine", P_NAME = "Bosmine" });
            DrugInhalation.Add(new SysParams() { P_VALUE = "Encore", P_NAME = "Encore" });
            DrugInhalation.Add(new SysParams() { P_VALUE = "Siruta", P_NAME = "Siruta" });
            return DrugInhalation;
        }
         
 
        #endregion
    }

    public class CPTAssessViewModel
    {
        public RCS_CPT_DTL_NEW_ITEMS cpt_data { get; set; }
        public IPDPatientInfo pat_info { get; set; }
        public DataTable cpt_header { get; set; } 

        public DB_RCS_CPT_ASS_MASTER master { get; set; }
        public UserInfo user_info { get; set; } 
        /// <summary>
        /// 取得列印顯示的文字
        /// </summary>
        /// <returns></returns>
        public string getPrintTxt(List<JSON_DATA> List, string pTxt)
        {
            string str = "";
            try
            {
                if (List != null && List.Exists(x => x.txt == pTxt && x.chkd))
                {
                    //str = "■ ";
                    str += pTxt;
                }
                else
                {
                    //str = "□ ";
                }
                //str += pTxt;
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
            return str;
        }

        /// 取得列印顯示的文字
        /// </summary>
        /// <returns></returns>
        public string getPrintListTxt(List<JSON_DATA> List, List<string> pTxt)
        {
            string str = "";
            try
            {
                if (List != null && List.Exists(x => pTxt.Contains(x.txt) && x.chkd))
                {
                    //str = "■ ";

                    var getList = List.Where(x => pTxt.Contains(x.txt) && x.chkd).ToList();

                    str += getList[0].txt;

                    for (int y = 1; y < getList.Count(); y++)
                    {
                        str +="," + getList[y].txt;

                        if (y % 2 == 1) {
                            str += "<br />";
                        }

                    }
                   
                }
                else
                {
                    //str = "□ ";
                }
                //str += pTxt;
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
            return str;
        }
    }
    public class ListCPTAssessViewModel
    {
        public List<CPTAssessViewModel> List { get; set; }
    }
}


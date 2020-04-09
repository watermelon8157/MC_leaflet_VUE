using Com.Mayaminer;
using RCS_Data;
using System;
using System.Collections.Generic;
using System.Linq;
using RCSData.Models;
using RCS_Data.Models.DB;
using System.Threading;
using RCS.Models.HOSP;

namespace RCS.Models
{


    /// <summary>
    /// 更新病患資料
    /// </summary>
    public class UpdateIPDPatientInfo : BaseThread
    {
 

        public bool updateAll { get; set; }
        public UpdateIPDPatientInfo()
        {
            this.updateAll = false;
        }
        public override void RunThread()
        {
            string actionName = "RunThread";
            List<IPDPatientInfo> temp_list = new List<IPDPatientInfo>();
            WebMethod webMothod = new WebMethod(); 
            // 取得全部病人資料
            SQLProvider SQL = new SQLProvider();
            Dapper.DynamicParameters dp = new Dapper.DynamicParameters();
            List<DB_RCS_RT_CASE> pList = new List<DB_RCS_RT_CASE>();
            try
            {
                string sqlstr = "SELECT DISTINCT M.chart_no,M.ipd_no,M.BED_NO,M.COST_CODE,M.VS_DOC_ID vs_id,M.VS_DOC_NAME vs_doc,M.PATIENT_SOURCE source_type FROM RCS_RT_CARE_SCHEDULING as L,RCS_RT_CASE as M WHERE L.IPD_NO = M.IPD_NO AND M.PATIENT_SOURCE = 'I' AND M.ACCEPT_STATUS  = '1'   GROUP BY M.chart_no,M.ipd_no,M.BED_NO,M.COST_CODE,M.VS_DOC_ID  ,M.VS_DOC_NAME,M.PATIENT_SOURCE , L.CREATE_DATE  HAVING L.CREATE_DATE = MAX(L.CREATE_DATE)";// 更新全部資料
                temp_list = SQL.DBA.getSqlDataTable<IPDPatientInfo>(sqlstr, dp);
                if (temp_list.Count > 0)
                {
                    string qury = "SELECT * FROM " + DB_TABLE_NAME.DB_RCS_RT_CASE + " WHERE IPD_NO in @IPD_NO AND CHART_NO in @CHART_NO";
                    dp = new Dapper.DynamicParameters();
                    dp.Add("IPD_NO", temp_list.Select(x=>x.ipd_no));
                    dp.Add("CHART_NO", temp_list.Select(x => x.chart_no));
                    pList = SQL.DBA.getSqlDataTable<DB_RCS_RT_CASE>(qury, dp);
                    if (SQL.DBA.hasLastError)
                    {
                        LogTool.SaveLogMessage(SQL.DBA.lastError, actionName, "UpdateIPDPatientInfo");
                    }
                }
                foreach (IPDPatientInfo item in temp_list)
                {
                    List<IPDPatientInfo> patList = webMothod.getPatientInfoList(this.hospFactory.webService.HISPatientInfo(), item.chart_no, item.ipd_no);
                    if (patList.Count > 0)
                    { 
                        IPDPatientInfo pat_info = patList[0];
                        if (pList.Exists(x=>x.IPD_NO == pat_info.ipd_no && x.CHART_NO == pat_info.chart_no))
                        {
                            pList.Find(x => x.IPD_NO == pat_info.ipd_no && x.CHART_NO == pat_info.chart_no).PATIENT_NAME = pat_info.patient_name;
                            pList.Find(x => x.IPD_NO == pat_info.ipd_no && x.CHART_NO == pat_info.chart_no).BED_NO = pat_info.bed_no;
                            pList.Find(x => x.IPD_NO == pat_info.ipd_no && x.CHART_NO == pat_info.chart_no).COST_CODE = pat_info.cost_code;
                            pList.Find(x => x.IPD_NO == pat_info.ipd_no && x.CHART_NO == pat_info.chart_no).VS_DOC_ID = pat_info.vs_doc;
                            pList.Find(x => x.IPD_NO == pat_info.ipd_no && x.CHART_NO == pat_info.chart_no).VS_DOC_NAME = pat_info.vs_id;
                            pList.Find(x => x.IPD_NO == pat_info.ipd_no && x.CHART_NO == pat_info.chart_no).DEPT_CODE = pat_info.dept_code;
                            pList.Find(x => x.IPD_NO == pat_info.ipd_no && x.CHART_NO == pat_info.chart_no).DEPT_NAME = pat_info.dept_desc;
                            pList.Find(x => x.IPD_NO == pat_info.ipd_no && x.CHART_NO == pat_info.chart_no).DIAG_DATE = pat_info.diag_date;
                            pList.Find(x => x.IPD_NO == pat_info.ipd_no && x.CHART_NO == pat_info.chart_no).PRE_DISCHARGE_DATE = pat_info.pre_discharge_date;
                            // pList.Find(x => x.IPD_NO == pat_info.ipd_no && x.CHART_NO == pat_info.chart_no).DIAG_DESC = pat_info.diagnosis_code;
                        }
                        if (!string.IsNullOrWhiteSpace(pat_info.bed_no) && item.bed_no != pat_info.bed_no)
                            item.bed_no = pat_info.bed_no;
                        if (!string.IsNullOrWhiteSpace(pat_info.cost_desc) && item.bed_no != pat_info.cost_desc)
                            item.cost_desc = pat_info.cost_desc;
                        if (!string.IsNullOrWhiteSpace(pat_info.cost_code) && item.bed_no != pat_info.cost_code)
                            item.cost_code = pat_info.cost_code;
                        if (!string.IsNullOrWhiteSpace(pat_info.vs_doc) && item.bed_no != pat_info.vs_doc)
                            item.vs_doc = pat_info.vs_doc;
                        if (!string.IsNullOrWhiteSpace(pat_info.vs_id) && item.bed_no != pat_info.vs_id)
                            item.vs_id = pat_info.vs_id;
                        if (!string.IsNullOrWhiteSpace(pat_info.dept_code) && item.bed_no != pat_info.dept_code)
                            item.dept_code = pat_info.dept_code;
                        if (!string.IsNullOrWhiteSpace(pat_info.dept_desc) && item.bed_no != pat_info.dept_desc)
                            item.dept_desc = pat_info.dept_desc;
                        if (!string.IsNullOrWhiteSpace(pat_info.pre_discharge_date))
                            item.pre_discharge_date = pat_info.pre_discharge_date;
                        if (MvcApplication.ipd_list.Exists(x => x.ipd_no == item.ipd_no && x.chart_no == item.chart_no))
                        { 
                            MvcApplication.ipd_list.Find(x => x.ipd_no == item.ipd_no && x.chart_no == item.chart_no).new_ipd_no = item.new_ipd_no;
                            MvcApplication.ipd_list.Find(x => x.ipd_no == item.ipd_no && x.chart_no == item.chart_no).bed_no = item.bed_no; 
                            MvcApplication.ipd_list.Find(x => x.ipd_no == item.ipd_no && x.chart_no == item.chart_no).cost_desc = item.cost_desc;
                            MvcApplication.ipd_list.Find(x => x.ipd_no == item.ipd_no && x.chart_no == item.chart_no).cost_code = item.cost_code;
                            MvcApplication.ipd_list.Find(x => x.ipd_no == item.ipd_no && x.chart_no == item.chart_no).vs_doc = item.vs_doc;
                            MvcApplication.ipd_list.Find(x => x.ipd_no == item.ipd_no && x.chart_no == item.chart_no).vs_id = item.vs_id;
                            MvcApplication.ipd_list.Find(x => x.ipd_no == item.ipd_no && x.chart_no == item.chart_no).dept_code = item.dept_code;
                            MvcApplication.ipd_list.Find(x => x.ipd_no == item.ipd_no && x.chart_no == item.chart_no).dept_desc = item.dept_desc;
                            MvcApplication.ipd_list.Find(x => x.ipd_no == item.ipd_no && x.chart_no == item.chart_no).pre_discharge_date = item.pre_discharge_date;
                        }
                        else
                        {
                            MvcApplication.ipd_list.Add(item);
                        }
                    }
                }
                if (pList.Count > 0)
                {
                    SQL.DBA.DBExecUpdate<DB_RCS_RT_CASE>(pList);
                    if (SQL.DBA.hasLastError)
                    {
                        LogTool.SaveLogMessage(SQL.DBA.lastError, actionName, "UpdateIPDPatientInfo");
                    }
                }
            }
            catch (Exception ex)
            {
                LogTool.SaveLogMessage(ex, actionName, "UpdateIPDPatientInfo");

            } 

        }


    }


    

}
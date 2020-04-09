using Com.Mayaminer;
using Newtonsoft.Json;
using RCS.Models;
using RCS.Models.DB;
using RCS_Data;
using RCS_Data.Controllers.RT;
using RCS_Data.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace RCS.Areas.CARE_LIST.Controllers
{
    public class CARE_LISTController : Controller
    {
        //
        // GET: /CARE_LIST/CARE_LIST/
        string csName { get { return "CARE_LISTController"; } }

        public FileResult Index(string jsonData_care_listForm)
        {
            SQLProvider SQL = new SQLProvider();

            //P_NAME健保碼  P_VALUE說明
            DataTable sysParam = SQL.DBA.getSqlDataTable("SELECT * FROM RCS_SYS_PARAMS WHERE P_MODEL = 'RT' AND P_GROUP = 'care_list'");

            jsonData_care_listForm = HttpUtility.UrlDecode(jsonData_care_listForm);
            List<PatientListItem> row = Newtonsoft.Json.JsonConvert.DeserializeObject<List<PatientListItem>>(jsonData_care_listForm);

            DataTable newResultDataTable = new DataTable();
            int nowPat = 0;
            #region 宣告欄位
            for (int i = 0; i < 13; i++)
            {
                newResultDataTable.Columns.Add(i.ToString());
            }
            #endregion
            List<int> cntList = new List<int>() { 5, 9 };
            while (nowPat < row.Count)
            {
                newResultDataTable.Rows.Add("", "護理站");
                newResultDataTable.Rows.Add("日期", "床號");
                newResultDataTable.Rows.Add(DateTime.Now.ToString("yyyy-MM-dd"), "姓名");
                newResultDataTable.Rows.Add("健保碼", "病歷號");
                for (int i = 2; i < newResultDataTable.Columns.Count; i++)
                {
                    if (!cntList.Contains(i) && nowPat < row.Count)
                    {
                        newResultDataTable.Rows[newResultDataTable.Rows.Count - 4][i] = row[nowPat].cost_code;
                        newResultDataTable.Rows[newResultDataTable.Rows.Count - 3][i] = row[nowPat].bed_no;
                        newResultDataTable.Rows[newResultDataTable.Rows.Count - 2][i] = row[nowPat].patient_name;
                        newResultDataTable.Rows[newResultDataTable.Rows.Count - 1][i] = row[nowPat].chart_no;
                        nowPat++;
                    }//if (!cntList.Contains(i) && nowPat < row.Count)
                    else if (cntList.Contains(i))
                    {
                        newResultDataTable.Rows[newResultDataTable.Rows.Count - 4][i] = "";
                        newResultDataTable.Rows[newResultDataTable.Rows.Count - 3][i] = "";
                        newResultDataTable.Rows[newResultDataTable.Rows.Count - 2][i] = "";
                        newResultDataTable.Rows[newResultDataTable.Rows.Count - 1][i] = "健保碼";
                    }//else if (cntList.Contains(i))
                }//for (int i = 2; i < newResultDataTable.Columns.Count; i++)
                foreach (DataRow rec in sysParam.Rows)
                {
                    newResultDataTable.Rows.Add(rec["P_NAME"].ToString(), rec["P_VALUE"].ToString());
                    foreach (var j in cntList)
                    {
                        newResultDataTable.Rows[newResultDataTable.Rows.Count - 1][j] = rec["P_VALUE"].ToString();
                    }//foreach (var j in cntList)
                }//foreach (DataRow rec in sysParam.Rows)
            }//while (nowPat < row.Count)

            NPOITool nt = new NPOITool();
            byte[] buf = nt.ExportExcelTable(newResultDataTable, "CARE_LIST.xls", "照護清單", "照護清單", @showColname: false);
            return new FileContentResult(buf, "application/vnd.ms-excel") { FileDownloadName = "CARE_LIST.xls" };
        }//Index

        public FileResult createNpoiExcelMultipleSheet(string jsonData_care_listForm)
        {
            //ASP.NET - NPOI Excel 匯出-多工作表Sheets 2018.11.02
            byte[] return_ByteArr = new byte[] { };
            try
            {
                //P_NAME健保碼  P_VALUE說明
                SQLProvider SQL = new SQLProvider();
                DataTable sqlSysParam_Table = SQL.DBA.getSqlDataTable(
                    @"SELECT * 
                        FROM RCS_SYS_PARAMS 
                    WHERE P_MODEL = 'RT' 
                        AND P_GROUP = 'care_list'"
                    );
                jsonData_care_listForm = HttpUtility.UrlDecode(jsonData_care_listForm);
                List<PatientListItem> inputPatient_pList = JsonConvert.DeserializeObject<List<PatientListItem>>(jsonData_care_listForm);
                //確認是否有資料
                if (inputPatient_pList != null 
                    && inputPatient_pList.Count > 0 //接收 [input] 參數
                    && sqlSysParam_Table != null
                    && sqlSysParam_Table.Rows.Count > 0 //讀取 [RCS_SYS_PARAMS] 資料庫
                ){
                    //ASP.NET - NPOI Excel 匯出-多工作表Sheets 2018.11.02
                    NPOITool nt = new NPOITool();
                    return_ByteArr = nt.ExportNpoiExcelMultipleSheet(inputPatient_pList, sqlSysParam_Table);
                }//if
            }
            catch (Exception ex)
            {
                LogTool.SaveLogMessage(ex, "createNpoiExcelMultipleSheet", this.csName);
            }
            return new FileContentResult(return_ByteArr, "application/vnd.ms-excel")
            {
                FileDownloadName = "CARE_LIST.xls"
            };
        }//createNpoiExcelMultipleSheet

        public ActionResult Index2()
        {

            Models.CARE_LIST ctCareList = new Models.CARE_LIST();
            DataTable newResultDataTable = new DataTable();

            newResultDataTable.Columns.Add("日期");
            newResultDataTable.Columns.Add("床號");

            DataRow row1 = newResultDataTable.NewRow();
            row1["日期"] = "3/7"; row1["床號"] = "姓名";
            DataRow row2 = newResultDataTable.NewRow();
            row2["日期"] = "健保碼"; row2["床號"] = "病歷號";

            //PATIENT_NAME姓名  BED_NO床號  CHART_NO病歷號
            //foreach (PatInfo rec in ctCareList.patInfo)
            //{
            //    newResultDataTable.Columns.Add(rec.bed_no);
            //    row1[rec.bed_no] = rec.patient_name;
            //    row2[rec.bed_no] = rec.chart_no;
            //}
            ctCareList.patInfo.ForEach(x => {
                newResultDataTable.Columns.Add(x.bed_no);
                row1[x.bed_no] = x.patient_name;
                row2[x.bed_no] = x.chart_no;
            });

            newResultDataTable.Rows.Add(row1);
            newResultDataTable.Rows.Add(row2);

            //P_NAME健保碼  P_VALUE說明
            //foreach (SysParams rec in ctCareList.sysParam)
            //{
            //    newResultDataTable.Rows.Add(rec.P_NAME, rec.P_VALUE);
            //}
            ctCareList.sysParam.ForEach(x => {
                newResultDataTable.Rows.Add(x.P_NAME, x.P_VALUE);
            });

            return View(newResultDataTable);
        }

        public JsonResult Shit_List(string jsonData)
        {
            string actionName = "Shit_List";
            List<SHIFT_LIST> pList = new List<SHIFT_LIST>();
            try
            {
                jsonData = HttpUtility.UrlDecode(jsonData);
                List<PatientListItem> row = Newtonsoft.Json.JsonConvert.DeserializeObject<List<PatientListItem>>(jsonData);
                SQLProvider SQL = new SQLProvider();
                List<string> _sql = row.Select(x => x.ipd_no).Distinct().Select( x => string.Concat("SELECT ISBAR_ID FROM RCS_RT_ISBAR_SHIFT WHERE IPD_NO = '", x, "' AND CREATE_DATE in (SELECT MAX(CREATE_DATE) FROM RCS_RT_ISBAR_SHIFT WHERE IPD_NO = '", x,"')") ).ToList();
                string _query = string.Join(" UNION ", _sql);
                List<string> _ISBAR_ID = SQL.DBA.getSqlDataTable<string>(_query);
                if (SQL.DBA.hasLastError)
                {
                    LogTool.SaveLogMessage(SQL.DBA.lastError, actionName, this.csName);
                }
                else
                {
                    if (_ISBAR_ID.Count > 0)
                    {
                        _query = "SELECT IPD_NO,B_VALUE_1,B_VALUE_2 FROM RCS_RT_ISBAR_SHIFT WHERE ISBAR_ID in @ISBAR_ID";
                        Dapper.DynamicParameters dp = new Dapper.DynamicParameters();
                        dp.Add("ISBAR_ID", _ISBAR_ID);
                        pList = SQL.DBA.getSqlDataTable<SHIFT_LIST>(_query,dp);
                        if (SQL.DBA.hasLastError)
                        {
                            LogTool.SaveLogMessage(SQL.DBA.lastError, actionName, this.csName);
                        }
                        else
                        {
                            foreach (SHIFT_LIST item in pList)
                            {
                                if (row.Exists(x=>x.ipd_no == item.ipd_no))
                                {
                                    PatientListItem _pat = row.Find(x => x.ipd_no == item.ipd_no);
                                    item.dept_desc = _pat.dept_desc;
                                    item.bed_no = _pat.bed_no;
                                    item.gender = _pat.gender;
                                    item.chart_no = _pat.chart_no;
                                    item.cost_code = _pat.cost_code;
                                    item.patient_name = _pat.patient_name;
                                }
                            }
                            pList = pList.OrderBy(x => x.bed_no).ToList();
                        }

                    }
                }
                 
               
            }
            catch (Exception ex)
            {

                LogTool.SaveLogMessage(ex, actionName, this.csName);
            }
            return Json(pList);
        }
    }
}

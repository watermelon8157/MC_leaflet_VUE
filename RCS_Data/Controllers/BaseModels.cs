using Com.Mayaminer;
using Dapper;
using mayaminer.com.library;
using RCS_Data.Controllers.Upload;
using RCS_Data.Models;
using RCS_Data.Models.DB;
using RCS_Data.Models.ViewModels;
using RCSData.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace RCS_Data.Controllers
{
    public class BaseModels : RCS_Data.Models.BASIC_PARAMS
    {
        private string csName {get { return "BaseModels"; } }
        private SQLProvider _DBLink { get; set; }
        protected SQLProvider DBLink
        {
            get
            {
                if (this._DBLink == null)
                {
                    this._DBLink = new SQLProvider();
                }
                return this._DBLink;
            }
        }

        private WebMethod _webmethod { get; set; }
        protected WebMethod webmethod
        {
            get
            {
                if (this._webmethod == null)
                    this._webmethod = new WebMethod();
                return this._webmethod;
            }
        }

        private RCS_Data.Models.BasicFunction _bf { get; set; }
        /// <summary>
        /// basicfunction
        /// </summary>
        protected RCS_Data.Models.BasicFunction basicfunction
        {
            get
            {
                if (this._bf == null)
                {
                    this._bf = new RCS_Data.Models.BasicFunction();
                }
                return this._bf;
            }
        }


        /// <summary>取得系統設定參數</summary>
        /// <param name="pModel">參數類別</param>
        /// <param name="pGroup">參數群組</param>
        /// <param name="pLang">語系(預設:zh-tw)</param>
        /// <param name="pStatus">資料狀態：1=正常，9=刪除/停用</param>
        /// <param name="pManage">資料狀態：1=管理者才顯示，0=非管理者顯示</param>
        /// <returns></returns>
        protected List<DB_RCS_SYS_PARAMS> getRCS_SYS_PARAMS(string pModel = "", string pGroup = "", string pLang = "zh-tw", string pStatus = "", string pManage = "")
        {
            List<DB_RCS_SYS_PARAMS> SelectListItem = new List<DB_RCS_SYS_PARAMS>();
            SelectListItem = this.getRCS_SYS_PARAMS_Data("", pModel, pGroup, pLang, "", pStatus, pManage);
            return SelectListItem;
        }
        protected List<DB_RCS_SYS_PARAMS> getRCS_SYS_PARAMS_Data(string Id = "", string pModel = "", string pGroup = "", string pLang = "zh-tw", string pValue = "", string pStatus = "", string pManage = "", List<string> pModelList = null)
        {
            List<string> wSql = new List<string>();
            string sql = "SELECT * FROM {0} WHERE {1} ORDER BY P_SORT";
            List<DB_RCS_SYS_PARAMS> SelectListItem = new List<DB_RCS_SYS_PARAMS>();
            if (!string.IsNullOrEmpty(Id))
                wSql.Add(string.Format(" P_ID = {0}", SQLDefend.SQLString(Id)));
            if (!string.IsNullOrEmpty(pModel))
            {
                wSql.Add(string.Format(" P_MODEL = {0}", SQLDefend.SQLString(pModel)));

            }
            if (pModelList != null && pModelList.Count > 0)
            {
                wSql.Add(string.Format(" P_MODEL in ('{0}')", string.Join("','", pModelList)));
            }
            if (!string.IsNullOrEmpty(pGroup))
                wSql.Add(string.Format(" P_GROUP = {0}", SQLDefend.SQLString(pGroup)));
            if (!string.IsNullOrEmpty(pLang))
                wSql.Add(string.Format(" P_LANG = {0}", SQLDefend.SQLString(pLang)));
            if (!string.IsNullOrEmpty(pValue))
                wSql.Add(string.Format(" P_VALUE = {0}", SQLDefend.SQLString(pValue)));
            if (!string.IsNullOrEmpty(pStatus))
                wSql.Add(string.Format(" P_STATUS = {0}", SQLDefend.SQLString(pStatus)));
            if (!string.IsNullOrEmpty(pManage))
                wSql.Add(string.Format(" P_MANAGE = {0}", SQLDefend.SQLString(pManage)));
            sql = string.Format(sql, DB_TABLE_NAME.DB_RCS_SYS_PARAMS, string.Join(" AND ", wSql));

            SQLProvider dblink = new SQLProvider();
            Dapper.DynamicParameters dp = new Dapper.DynamicParameters();
            dblink.DBA.Open();

            SelectListItem = this.DBLink.DBA.getSqlDataTable<DB_RCS_SYS_PARAMS>(sql, dp);

            dblink.DBA.Close();

            return SelectListItem;
        }

        protected List<DB_RCS_SYS_PARAMS> getRCS_SYS_PARAMS_GName(string pGrpModel, string pGrpGroup, string pModel = "", string pGroup = "", string pLang = "zh-tw", string pStatus = "", string pManage = "")
        {
            List<string> wSql = new List<string>();
            string whereSql = "";
            Dapper.DynamicParameters dp = new Dapper.DynamicParameters();

            List<DB_RCS_SYS_PARAMS> SelectListItem = new List<DB_RCS_SYS_PARAMS>();

            if (!string.IsNullOrEmpty(pModel))
                wSql.Add(string.Format(" A.P_MODEL = {0}", SQLDefend.SQLString(pModel)));
            if (!string.IsNullOrEmpty(pGroup))
                wSql.Add(string.Format(" A.P_GROUP = {0}", SQLDefend.SQLString(pGroup)));
            if (!string.IsNullOrEmpty(pLang))
            {
                wSql.Add(string.Format(" A.P_LANG = {0}", SQLDefend.SQLString(pLang)));
                whereSql = " AND B.P_LANG = " + SQLDefend.SQLString(pLang);
            }
            if (!string.IsNullOrEmpty(pStatus))
                wSql.Add(string.Format(" A.P_STATUS = {0}", SQLDefend.SQLString(pStatus)));
            if (!string.IsNullOrEmpty(pManage))
                wSql.Add(string.Format(" A.P_MANAGE = {0}", SQLDefend.SQLString(pManage)));
            string sql = string.Format("SELECT A.*, B.P_NAME P_GROUP_NAME FROM {0} A JOIN {0} B ON B.P_VALUE = A.P_GROUP AND B.P_MODEL = {1} AND B.P_GROUP = {2}{3} WHERE {4} ORDER BY A.P_SORT",
                "RCS_SYS_PARAMS", SQLDefend.SQLString(pGrpModel), SQLDefend.SQLString(pGrpGroup)
                , whereSql, string.Join(" AND ", wSql));

            this.DBLink.DBA.Open();

            SelectListItem = this.DBLink.DBA.getSqlDataTable<DB_RCS_SYS_PARAMS>(sql, dp);

            this.DBLink.DBA.Close();

            return SelectListItem;
        }

        /// <summary>
        /// 取得系統設定中的下拉項目清單
        /// </summary>
        /// <param name="RTRecordAllList">List SysParams</param>
        /// <param name="group">群組分類</param>
        /// <returns>List SysParams</returns>
        public List<DB_RCS_SYS_PARAMS> GetGroupList(List<DB_RCS_SYS_PARAMS> RTRecordAllList, string group_v)
        {
            IEnumerable<DB_RCS_SYS_PARAMS> get_select_data = from r in RTRecordAllList
                                                     where r.P_GROUP == group_v
                                                     orderby int.Parse(r.P_SORT)
                                                     select r;
            return get_select_data.ToList();
        }

        public PatientListItem SelectPatientInfo(string pIpd_no, string pChart_no)
        {
            PatientListItem pItem = new PatientListItem();
            string actionName = "SelectPatientInfo";
            List<PatientListItem> pList = this.SelectPatientInfoList(pIpd_no, pChart_no);

            if (pList.Count > 0)
            {
                foreach (PatientListItem item in pList)
                {
                    if (mayaminer.com.library.DateHelper.isDate(item.birth_day, "yyyyMMdd"))
                    {
                        item.birth_day = DateHelper.Parse(item.birth_day).ToString("yyyy/MM/dd");
                    }
                    if (mayaminer.com.library.DateHelper.isDate(item.birth_day, "yyyMMdd"))
                    {
                        item.birth_day = DateHelper.ParseTWDate(item.birth_day).ToString("yyyy/MM/dd");
                    }
                }
                pItem = pList[0]; 

            }
            else if (this.DBLink.DBA.hasLastError)
            {
                LogTool.SaveLogMessage(this.DBLink.DBA.lastError, actionName, this.csName);
            }
          
            return pItem;
        }

        private List<PatientListItem> SelectPatientInfoList(string pIpd_no, string pChart_no)
        {
            string actionName = "SelectPatientInfoList"; 
            List<PatientListItem> pList = new List<PatientListItem>();
            Dapper.DynamicParameters dp = new DynamicParameters();
            string query = "SELECT case_id,PATIENT_SOURCE as source_type,IPD_NO,CHART_NO,PATIENT_NAME,GENDER,BIRTH_DAY,BODY_HEIGHT,DIAG_DATE,PRE_DISCHARGE_DATE,VS_DOC_NAME as vs_doc,DIAG_DESC as diagnosis_code,BED_NO FROM " + DB_TABLE_NAME.DB_RCS_RT_CASE + " A WHERE CHART_NO =@CHART_NO";
            if (!string.IsNullOrWhiteSpace(pIpd_no))
            {
                query += " AND IPD_NO = @IPD_NO ";
            }
            query += " ORDER BY CREATE_DATE DESC";
            dp.Add("CHART_NO", pChart_no);
            dp.Add("IPD_NO", pIpd_no);
            pList = this.DBLink.DBA.getSqlDataTable<PatientListItem>(query, dp);
            if (this.DBLink.DBA.hasLastError)
            {
                LogTool.SaveLogMessage(this.DBLink.DBA.lastError, actionName, this.csName);
            } 
            return pList;
        }

        public List<PatientListItem> SelectPatientInfoListForSelect(string pIpd_no, string pChart_no)
        {

            string actionName = "SelectPatientInfoList"; 
            List<PatientListItem> pList = this.SelectPatientInfoList(pIpd_no, pChart_no);
            pList = pList.GroupBy(x => new { x.ipd_no, x.chart_no, x.diag_date, x.source_type }).Select(y =>
              new PatientListItem()
              {
                  ipd_no = y.Key.ipd_no,
                  chart_no = y.Key.chart_no,
                  diag_date = y.Key.diag_date,
                  source_type = y.Key.source_type
              }).Distinct().ToList();
            return pList; 
        }

        public RESPONSE_MSG getDeviceList(bool pShowDel, string pDeviceNo = "")
        {
            RESPONSE_MSG rm = new RESPONSE_MSG();
            List<DeviceMaster> deviceList = new List<DeviceMaster>();

            List<string> nolist = new List<string>();
            if (!string.IsNullOrEmpty(pDeviceNo)) nolist.Add(pDeviceNo);
            string sql = "";
            sql = "SELECT *, CASE ISNULL(ROOM, '') WHEN '' THEN '' ELSE (SELECT TOP 1 '床號：' + BED_NO + ' 姓名：|' +PATIENT_NAME + '| 病歷號：' + CHART_NO FROM RCS_RT_CASE WHERE CHART_NO = A.ROOM ORDER BY CREATE_DATE DESC) END ON_POSITION FROM RCS_VENTILATOR_SETTINGS A WHERE 1=1";
            
            Dapper.DynamicParameters dp = new Dapper.DynamicParameters();
            if (!pShowDel) sql += " AND USE_STATUS = 'Y'";
            if (nolist.Count > 0)
            {
                sql += " AND DEVICE_SEQ in @DEVICE_SEQ";
                dp.Add("DEVICE_SEQ", nolist);
            }
            deviceList = this.DBLink.DBA.getSqlDataTable<DeviceMaster>(sql, dp);
            if (this.DBLink.DBA.hasLastError)
            {
                rm.status = RESPONSE_STATUS.ERROR;
                rm.message = "查詢失敗!" + this.DBLink.DBA.lastError;
            }
            rm.attachment = deviceList;
            return rm;
        }

        /// <summary>
        /// 取得 治療評估單資料
        /// </summary>
        /// <returns></returns>
        public List<RCS_CPT_DTL_NEW_ITEMS> getCPTAssessList(string pChart_no)
        {
            string actionName = "getCPTAssess";

            List<RCS_CPT_DTL_NEW_ITEMS> cptDataList = new List<RCS_CPT_DTL_NEW_ITEMS>();
            try
            {

                RCS_Data.Controllers.RtAssess.Models rtAssessModel = new Controllers.RtAssess.Models();
                SQLProvider dbLink = new SQLProvider();
                List<RCS_DATA_CR_MASTER> pList = new List<RCS_DATA_CR_MASTER>();
                string sql = string.Format("SELECT (RECORD_DATE + '/' + BED_NO + ':' + CAST (ROW_NUMBER() OVER(ORDER BY CPT_ID) AS VARCHAR(3))) AS IPD_NO, CREATE_NAME, CPT_ID FROM {1} WHERE CHART_NO = {0} AND DATASTATUS not in('2','9') ORDER BY CPT_ID DESC", SQLDefend.SQLString(pChart_no), DB_TABLE_NAME.DB_RCS_CPT_ASS_MASTER);
                DataTable dt = dbLink.DBA.getSqlDataTable(sql);
                if (dt != null && dt.Rows.Count > 0)
                {
                    pList = dt.ToList<RCS_DATA_CR_MASTER>();
                }

                if (pList.Count > 0)
                {
                    List<RCS_CPT_DTL_NEW_ITEMS> tempCPT = new List<RCS_CPT_DTL_NEW_ITEMS>();
                    var getData = rtAssessModel.RTAssessData(new List<string>() { pList[0].cpt_id });

                    if (getData.detailList.Any())
                    {
                        cptDataList = getData.detailList;
                    }
                }
            }
            catch (Exception ex)
            {
                LogTool.SaveLogMessage(ex, csName, actionName);
            }

            return cptDataList;
        }

        /// <summary>
        /// 取得 治療評估單資料
        /// </summary>
        /// <returns></returns>
        public RCS_CPT_DTL_NEW_ITEMS getCPTAssess(string pChart_no)
        {
            string actionName = "getCPTAssess";

            RCS_CPT_DTL_NEW_ITEMS cptData = new RCS_CPT_DTL_NEW_ITEMS();

            RCS_Data.Controllers.RtAssess.Models rtAssessModel = new Controllers.RtAssess.Models();
            try
            {
                List<RCS_CPT_DTL_NEW_ITEMS> tempCPT = this.getCPTAssessList(pChart_no);
                cptData = rtAssessModel.changeJson(tempCPT).First();
                PatientListItem pat = this.SelectPatientInfo("", pChart_no);
                cptData.now_pat_diagnosis = pat.diagnosis_code;
                // cont == 0 diagnosis
                // cont == 1 history_diag
                // cont == 2 other_history
                // cont == 3 rt_start_time brief_status.rt_start_time
                // cont == 4 now_pat_diagnosis
                // cont == 5 cpt_id  
            }
            catch (Exception ex)
            {
                LogTool.SaveLogMessage(ex, csName, actionName);
            }

            return cptData;
        }

        /// <summary>
        /// 取得RT照護病人清單
        /// </summary>
        /// <param name="user_id"></param>
        /// <param name="pChart_no"></param>
        /// <returns></returns>
        protected List<PatientListItem> getUserCareList(string user_id, string pChart_no = "", string pIPD_NO = "")
        {
            List<PatientListItem> pList = new List<PatientListItem>();
            string query = "";
            query = string.Concat("SELECT ", " a.CHART_NO,a.IPD_NO "
                 , ", b.GENDER, b.PATIENT_NAME,b.BIRTH_DAY, b.ACCEPT_STATUS, b.DIAG_DATE,b.PRE_DISCHARGE_DATE,b.CARE_DATE "
                 , ", a.CREATE_DATE,a.TYPE_MODE,a.BED_NO,a.COST_CODE "
                 , ", a.CARE_ID as r_id,b.DEPT_NAME as dept_desc,a.VS_DOC_NAME as vs_doc,a.VS_DOC_ID as vs_id,a.VS_DOC_NAME as case_vs_name, b.DIAG_DESC as diagnosis_code, b.PATIENT_SOURCE as source_type "
                 , ", b.DIAG_DESC,b.DEPT_CODE,b.DEPT_NAME,b.MDRO_MARK,b.DNR_MARK,b.VPN_MARK"
                 ,  " FROM ", DB_TABLE_NAME.DB_RCS_RT_CARE_SCHEDULING, " a "
                 , " left join ", DB_TABLE_NAME.DB_RCS_RT_CASE, " b on a.ipd_no = b.ipd_no and b.CREATE_DATE = (SELECT MAX(CREATE_DATE) FROM "  
                 , DB_TABLE_NAME.DB_RCS_RT_CASE, " WHERE ipd_no=b.ipd_no )"
                 , " where a.RT_ID = @RT_ID");
            if (pChart_no != null && pChart_no != "") query += " AND a.CHART_NO = @CHART_NO";
            if (pIPD_NO != null && pIPD_NO != "") query += " AND a.IPD_NO = @IPD_NO";
            Dapper.DynamicParameters dp = new Dapper.DynamicParameters();
            dp.Add("RT_ID", user_id);
            dp.Add("CHART_NO", pChart_no);
            dp.Add("IPD_NO", pIPD_NO);
            pList = this.DBLink.DBA.getSqlDataTable<PatientListItem>(query, dp);

            return pList;
        }
        /// <summary>
        /// 取得RT照護病人清單class
        /// </summary>
        /// <param name="user_id"></param>
        /// <param name="item"></param>
        /// <param name="diag_date"></param>
        /// <returns></returns>
        protected DB_RCS_RT_CARE_SCHEDULING getDB_RCS_RT_CARE_SCHEDULING(string user_id, IPDPatientInfo item,string diag_date ="")
        {
            DB_RCS_RT_CARE_SCHEDULING temp = new DB_RCS_RT_CARE_SCHEDULING()
            {
                CARE_ID = this.DBLink.GetFixedStrSerialNumber(item.ipd_no),
                RT_ID = user_id,
                CHART_NO = item.chart_no,
                IPD_NO = item.ipd_no,
                PATIENT_NAME = item.patient_name,
                GENDER = item.gender,
                BIRTH_DAY = item.birth_day,
                COST_CODE = item.cost_code,
                BED_NO = item.bed_no,
                DIAG_DATE = diag_date,
                CARE_DATE = Function_Library.getDateNowString(DATE_FORMAT.yyyy_MM_dd),
                CREATE_DATE = Function_Library.getDateNowString(DATE_FORMAT.yyyy_MM_dd_HHmmss),
                VS_DOC_NAME = item.vs_doc,
                PRE_DISCHARGE_DATE = item.pre_discharge_date,
                ROOM_NO = item.room_no,
                LOC = item.loc,
                VS_DOC_ID = item.vs_id,
                NHI_OPD_ER = item.source_type,
                TYPE_MODE = "C"
            }; 
            return temp;
        }

        public virtual RESPONSE_MSG UpLoadDataToHsop(UPLOADLIST item )
        {
            string actionName = "UpLoadDataToHsop";
            RESPONSE_MSG rm = new RESPONSE_MSG();

            return rm;
        }
        /// <summary>
        /// 取得收案指定資料
        /// </summary>
        /// <param name="CHART_NO"></param>
        /// <param name="VS_DOC_ID"></param>
        /// <param name="COST_CODE"></param>
        /// <param name="sDate"></param>
        /// <param name="eDate"></param>
        /// <returns></returns>
        protected List<DB_RCS_RT_CASE> getDB_RCS_RT_CASEList(string CHART_NO)
        { 
            return this.getDB_RCS_RT_CASEList(CHART_NO,"", "", "", "", "");
        }

        /// <summary>
        /// 取得收案指定資料
        /// </summary>
        /// <param name="CHART_NO"></param>
        /// <param name="VS_DOC_ID"></param>
        /// <param name="COST_CODE"></param>
        /// <param name="sDate"></param>
        /// <param name="eDate"></param>
        /// <returns></returns>
        protected List<DB_RCS_RT_CASE> getDB_RCS_RT_CASEList(string CHART_NO, string VS_DOC_ID, string COST_CODE, string ACCEPT_STATUS, string sDate, string eDate)
        {
            string query = "";
            List<DB_RCS_RT_CASE> pList = new List<DB_RCS_RT_CASE>();
            Dapper.DynamicParameters dp = new DynamicParameters(); 
            query = string.Concat("SELECT * FROM ", DB_TABLE_NAME.DB_RCS_RT_CASE, " A WHERE 1=1");
            if (!string.IsNullOrWhiteSpace(sDate) && !string.IsNullOrWhiteSpace(eDate))
            {
                query += " AND DIAG_DATE BWTWEEN @sDate  AND @eDate";
                dp.Add("sDate", sDate);
                dp.Add("eDate", eDate);
            }
            if (!string.IsNullOrWhiteSpace(CHART_NO))
            {
                query += " AND CHART_NO =@CHART_NO";
                dp.Add("CHART_NO", CHART_NO);
            }
            if (!string.IsNullOrWhiteSpace(VS_DOC_ID))
            {
                query += " AND VS_DOC_ID =@VS_DOC_ID";
                dp.Add("VS_DOC_ID", VS_DOC_ID);
            }
            if (!string.IsNullOrWhiteSpace(COST_CODE))
            {
                query += " AND COST_CODE =@COST_CODE";
                dp.Add("COST_CODE", COST_CODE);
            }
            if (!string.IsNullOrWhiteSpace(ACCEPT_STATUS))
            {
                query += " AND ACCEPT_STATUS =@ACCEPT_STATUS";
                dp.Add("ACCEPT_STATUS", ACCEPT_STATUS);
            }
            pList = this.DBLink.DBA.getSqlDataTable<DB_RCS_RT_CASE>(query, dp);
            return pList;
        }

        protected string saveCxrJSON(string jsonStr,ref RESPONSE_MSG rm)
        {
            string addJsonKey = "";
            //cxrJSON判斷
            if (!string.IsNullOrWhiteSpace(jsonStr))
            {
                 addJsonKey = this.DBLink.GetFixedStrSerialNumber();

                List<DB_RCS_RT_RECORD_JSON> addKeyData = new List<DB_RCS_RT_RECORD_JSON>();

                addKeyData.Add(new DB_RCS_RT_RECORD_JSON
                {
                    RECORD_ID = addJsonKey,
                    ITEM_NAME = "CXR資料",
                    JSON_VALUE = jsonStr 
                });


                RESPONSE_MSG checkJson = this.DBLink.Insert_JSONData(addJsonKey, addKeyData);


                if (checkJson.status == RESPONSE_STATUS.ERROR)
                {
                    LogTool.SaveLogMessage(this.DBLink.DBA.lastError, "CPTAssess_Save");
                    rm.message = "儲存失敗!";
                    rm.status = RESPONSE_STATUS.ERROR;
                } 
            }
            else
            {
                addJsonKey = "";
            }

            return addJsonKey;
        }
    }

    
}

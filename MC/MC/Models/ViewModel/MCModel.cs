using RCS_Data;
using RCS_Data.Models;
using RCS_Data.Models.DB;
using RCSData.Models;
using System;
using System.Collections.Generic;
using System.Device.Location;
using System.Linq;
using System.Web;
using Dapper;
using static Dapper.SqlMapper;

namespace RCS.Models.ViewModel
{
    public class MCModel
    {

        string csName = "MCModel";

        private UserInfo userinfo { get; set; }

        public MCModel()
        {
            this.userinfo = new UserInfo()
            {
                user_id = "admin",
                user_name = "admin"
            };
        }

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

        public List<string> hospList()
        {
            List<string> plist = new List<string>();


            return plist;

        }


        /// <summary>
        /// 計算分數 RunThread
        /// </summary>
        public void RunThread()
        { 
            string actionName = "RunThread";
            DateTime dateNow = DateTime.Parse("2020-07-08");
            // 取得醫院資料
            if (MvcApplication.hospList.Count == 0)
            {
                this.gethospList();
            }
            // 計算分數相關變數 
            // 𝑺𝒄𝒐𝒓𝒆_𝒊𝒋=  (𝒙_𝒊𝒋) + ( 𝒚_𝒊𝒋 (𝟏−𝒛_𝒊𝒋 )+ 𝒚_𝒊𝒋 𝒘𝟐_𝒊𝒋 ) 

            #region 計算 
            // 計算 MC_HOSP_INFO_DTL
            // 𝒛_𝒊𝒋
            this.setMC_HOSP_INFO_DTL(dateNow);//檢查是否有計算過資料
            this.runCV(dateNow);

            // 計算 DB_MC_SITE_DRIVING_TIME_INFO
            //  (𝒙_𝒊𝒋) 
            this.runDriving();

            // 計算 DB_MC_SOURCE_LIST
            //  𝑺𝒄𝒐𝒓𝒆_𝒊𝒋 
            this.getAllScore(dateNow);
            #endregion

        }


        public void getAllScore(DateTime pDate)
        {
            string actionName = "getAllScore"; 
            List<DB_MC_SOURCE_LIST> insertList = new List<DB_MC_SOURCE_LIST>();
            List<DB_MC_SOURCE_LIST> updateList = new List<DB_MC_SOURCE_LIST>();
            List<DB_MC_SITE_INFO> sList = new List<DB_MC_SITE_INFO>();
            List<DB_MC_SITE_DRIVING_TIME_INFO> tList = new List<DB_MC_SITE_DRIVING_TIME_INFO>();
            List<DB_MC_HOSP_INFO_DTL> dList = new List<DB_MC_HOSP_INFO_DTL>();
            List<DB_MC_PATIENT_INFO> pList = new List<DB_MC_PATIENT_INFO>();
            List<DB_MC_SOURCE_LIST> saList = new List<DB_MC_SOURCE_LIST>();
            Dapper.DynamicParameters dp = new Dapper.DynamicParameters();
            string sql = "", _date = Function_Library.getDateString(pDate, RCS_Data.Models.DATE_FORMAT.yyyy_MM_dd); 
            sql = "SELECT * FROM " + DB_TABLE_NAME.DB_MC_SITE_INFO + " WHERE DATASTATUS = '1';" ; 
            sList = this.DBLink.DBA.getSqlDataTable<DB_MC_SITE_INFO>(sql ); 
            sql = "SELECT * FROM " + DB_TABLE_NAME.DB_MC_PATIENT_INFO + " WHERE DATASTATUS = '1' AND SITE_ID in @SITE_ID;"
                  + " SELECT * FROM " + DB_TABLE_NAME.DB_MC_SITE_DRIVING_TIME_INFO + " WHERE DATASTATUS = '1'  AND SITE_ID in @SITE_ID;"
                  + " SELECT * FROM " + DB_TABLE_NAME.DB_MC_HOSP_INFO_DTL + " WHERE DATASTATUS = '1' AND SOURCE_DATE = @SOURCE_DATE;"
                  + " SELECT * FROM " + DB_TABLE_NAME.DB_MC_SOURCE_LIST + " WHERE DATASTATUS = '1' AND SOURCE_DATE = @SOURCE_DATE AND SITE_ID in @SITE_ID;";
            dp.Add("SITE_ID", sList.Select(x=>x.SITE_ID).Distinct().ToList());
            dp.Add("SOURCE_DATE", _date);
            this.DBLink.DBA.Open();
            GridReader gr = this.DBLink.DBA.dbConnection.QueryMultiple(sql, dp);
            pList = gr.Read<DB_MC_PATIENT_INFO>().ToList();  
            tList = gr.Read<DB_MC_SITE_DRIVING_TIME_INFO>().ToList();  
            dList = gr.Read<DB_MC_HOSP_INFO_DTL>().ToList();
            saList = gr.Read<DB_MC_SOURCE_LIST>().ToList();
            this.DBLink.DBA.Close(); 
            foreach (DB_MC_SITE_DRIVING_TIME_INFO t in tList)
            {
                bool hasData = false;
                DB_MC_SOURCE_LIST temp = new DB_MC_SOURCE_LIST();
                DB_MC_HOSP_INFO hosp = MvcApplication.hospList.Find(x => x.HOSP_KEY == t.HOSP_KEY);
                if (saList.Exists(x => x.SITE_ID == t.SITE_ID && x.SOURCE_DATE == _date && x.HOSP_KEY == t.HOSP_KEY))
                {
                    hasData = true;
                    temp = saList.Find(x => x.SITE_ID == t.SITE_ID && x.SOURCE_DATE == _date && x.HOSP_KEY == t.HOSP_KEY);
                    temp.MODIFY_DATE = Function_Library.getDateNowString(DATE_FORMAT.yyyy_MM_dd_HHmmss);
                    temp.MODIFY_ID = this.userinfo.user_id;
                    temp.MODIFY_NAME = this.userinfo.user_name;
                }
                else
                {
                    temp = new DB_MC_SOURCE_LIST()
                    {
                        HOSP_KEY = t.HOSP_KEY,
                        MILD = hosp.MILD,
                        SEVERE = hosp.SEVERE,
                        MODERATE = hosp.MODERATE,
                        W2 = hosp.W2,
                        CV = dList.Find(x => x.HOSP_KEY == t.HOSP_KEY && x.SOURCE_DATE == _date).SOURCE,
                        SITE_ID = t.SITE_ID,
                        DRIVING_SOURCE = t.DRIVING_SOURCE,
                        SOURCE_DATE = _date,
                        CREATE_DATE = Function_Library.getDateNowString(DATE_FORMAT.yyyy_MM_dd_HHmmss),
                        CREATE_ID = this.userinfo.user_id,
                        CREATE_NAME = this.userinfo.user_name,
                        MODIFY_DATE = Function_Library.getDateNowString(DATE_FORMAT.yyyy_MM_dd_HHmmss),
                        MODIFY_ID = this.userinfo.user_id,
                        MODIFY_NAME = this.userinfo.user_name,
                        DATASTATUS = "1",
                    };
                } 
                //  (𝒙_𝒊𝒋) + ( 𝒚_𝒊𝒋 (𝟏−𝒛_𝒊𝒋 )+ 𝒚_𝒊𝒋 𝒘𝟐_𝒊𝒋 ) 
                double x_ij = double.Parse(temp.DRIVING_SOURCE),
                    y_ij = double.Parse(temp.MILD), z_ij = double.Parse(temp.CV), w2_ij = double.Parse(temp.MILD);

                temp.MILD_SOURCE = Math.Round(x_ij + (double.Parse(temp.MILD) * (1- z_ij) + double.Parse(temp.MILD) * w2_ij),2).ToString();
                temp.MODERATE_SOURCE = Math.Round(x_ij + (double.Parse(temp.MODERATE) * (1 - z_ij) + double.Parse(temp.MODERATE) * w2_ij), 2).ToString();
                temp.SEVERE_SOURCE = Math.Round(x_ij + (double.Parse(temp.SEVERE) * (1 - z_ij) + double.Parse(temp.SEVERE) * w2_ij), 2).ToString();
                if (hasData)
                { 
                    updateList.Add(temp);
                }
                else
                { 
                    insertList.Add(temp);
                } 
            }
            this.DBLink.DBA.DBExecInsert<DB_MC_SOURCE_LIST>(insertList);
            this.DBLink.DBA.DBExecUpdate<DB_MC_SOURCE_LIST>(updateList);
            if (this.DBLink.DBA.hasLastError)
            {
                Com.Mayaminer.LogTool.SaveLogMessage(this.DBLink.DBA.lastError, actionName, this.csName);
            }

        }


        public List<DB_MC_SITE_INFO> getMC_SITE_INFO(string site_id ="")
        {
            string actionName = "checkSITEData";
            List<DB_MC_SITE_INFO> pList = new List<DB_MC_SITE_INFO>();
            string sql = "SELECT * FROM " + DB_TABLE_NAME.DB_MC_SITE_INFO + " WHERE DATASTATUS = '1' ";
            Dapper.DynamicParameters dp = new Dapper.DynamicParameters();
            if (!string.IsNullOrWhiteSpace(site_id))
            {
                sql += " AND SITE_ID =@SITE_ID";
                dp.Add("SITE_ID", site_id);
            } 
            pList = this.DBLink.DBA.getSqlDataTable<DB_MC_SITE_INFO>(sql, dp);
            if (this.DBLink.DBA.hasLastError)
            { 
                Com.Mayaminer.LogTool.SaveLogMessage(this.DBLink.DBA.lastError, actionName, this.csName);
            } 
            return pList;
        }

        public List<DB_MC_SITE_DRIVING_TIME_INFO> getDB_MC_SITE_DRIVING_TIME_INFO(string site_id = "")
        {
            string actionName = "checkSITEData";
            List<DB_MC_SITE_DRIVING_TIME_INFO> pList = new List<DB_MC_SITE_DRIVING_TIME_INFO>();
            string sql = "SELECT * FROM " + DB_TABLE_NAME.DB_MC_SITE_DRIVING_TIME_INFO + " WHERE DATASTATUS = '1' ";
            Dapper.DynamicParameters dp = new Dapper.DynamicParameters();
            if (!string.IsNullOrWhiteSpace(site_id))
            {
                sql += " AND SITE_ID =@SITE_ID";
                dp.Add("SITE_ID", site_id);
            }
            pList = this.DBLink.DBA.getSqlDataTable<DB_MC_SITE_DRIVING_TIME_INFO>(sql, dp);
            if (this.DBLink.DBA.hasLastError)
            {
                Com.Mayaminer.LogTool.SaveLogMessage(this.DBLink.DBA.lastError, actionName, this.csName);
            }
            return pList;
        }

        public List<string> getDB_DRIVING_SITE(List<string> site_id)
        {
            string actionName = "checkSITEData";
            List<string> pList = new List<string>();
            string sql = "SELECT DISTINCT SITE_ID FROM " + DB_TABLE_NAME.DB_MC_SITE_DRIVING_TIME_INFO + " WHERE DATASTATUS = '1' ";
            Dapper.DynamicParameters dp = new Dapper.DynamicParameters();
            if (site_id.Count > 0)
            {
                sql += " AND SITE_ID in @SITE_ID";
                dp.Add("SITE_ID", site_id);
            }
            pList = this.DBLink.DBA.getSqlDataTable<string>(sql, dp);
            if (this.DBLink.DBA.hasLastError)
            {
                Com.Mayaminer.LogTool.SaveLogMessage(this.DBLink.DBA.lastError, actionName, this.csName);
            }
            return pList;
        }

        /// <summary>
        /// 設定今天的變動CV
        /// </summary>
        /// <param name="pDate"></param>
        private void setMC_HOSP_INFO_DTL(DateTime pDate)
        {
            string actionName = "setMC_HOSP_INFO_DTL";
            string _date = Function_Library.getDateString(pDate, RCS_Data.Models.DATE_FORMAT.yyyy_MM_dd);
            SQLProvider dba = new SQLProvider();
            List<DB_MC_HOSP_INFO_DTL> pList = new List<DB_MC_HOSP_INFO_DTL>();
            List<DB_MC_HOSP_INFO_DTL> tempList = this.getMC_HOSP_INFO_DTL(_date);
            List<DB_MC_HOSP_INFO> hpList = new List<DB_MC_HOSP_INFO>();
            if (tempList.Count > 0)
            {
                hpList = MvcApplication.hospList.FindAll(x => !tempList.Exists(y => y.HOSP_KEY == x.HOSP_KEY)).ToList();
            }
            else
            {
                hpList = MvcApplication.hospList.ToList();
            }
            foreach (DB_MC_HOSP_INFO item in hpList)
            {
                pList.Add(new DB_MC_HOSP_INFO_DTL()
                {
                    HOSP_KEY = item.HOSP_KEY,
                    SOURCE_DATE = _date,
                    CREATE_DATE = Function_Library.getDateNowString(DATE_FORMAT.yyyy_MM_dd_HHmmss),
                    CREATE_ID = this.userinfo.user_id,
                    CREATE_NAME = this.userinfo.user_name,
                    MODIFY_DATE = Function_Library.getDateNowString(DATE_FORMAT.yyyy_MM_dd_HHmmss),
                    MODIFY_ID = this.userinfo.user_id,
                    MODIFY_NAME = this.userinfo.user_name,
                    DATASTATUS = "1",
                });
            }
            if (pList.Count > 0)
            {
                dba.DBA.DBExecInsert<DB_MC_HOSP_INFO_DTL>(pList);
            }
            if (this.DBLink.DBA.hasLastError)
            {
                MvcApplication.hospList = new List<DB_MC_HOSP_INFO>();
                Com.Mayaminer.LogTool.SaveLogMessage(this.DBLink.DBA.lastError, actionName, this.csName);
            }
        }

        /// <summary>
        /// 計算CV
        /// </summary>
        /// <param name="pDate">計算日期</param>
        public void runCV(DateTime pDate)
        {
            string actionName = "runCV";
            string _date = Function_Library.getDateString(pDate, RCS_Data.Models.DATE_FORMAT.yyyy_MM_dd);
            List<DB_MC_HOSP_INFO_DTL> tempList = this.getMC_HOSP_INFO_DTL(_date);
            if (tempList.Count > 0)
            {
                List<DB_MC_PATIENT_INFO> patList = this.getMC_PATIENT_INFO();
                foreach (DB_MC_HOSP_INFO_DTL item in tempList)
                {
                    item.CV = MvcApplication.hospList.Find(x => x.HOSP_KEY == item.HOSP_KEY).CV;
                    List<DB_MC_PATIENT_INFO> tempPatList = patList.FindAll(x => x.HOSP_KEY == item.HOSP_KEY).ToList();
                    int SEVERE_PAT = tempPatList.FindAll(x => x.TRIAGE == "Severe").Count();
                    int MODERATE_PAT = tempPatList.FindAll(x => x.TRIAGE == "Moderate").Count();
                    int MILD_PAT = tempPatList.FindAll(x => x.TRIAGE == "Mild").Count();
                    double PS = this.penalty_standadize(int.Parse(item.CV), tempPatList.Count());
                    item.PS = PS.ToString();
                    string v = Math.Round(double.Parse(PS.ToString()), 2).ToString();
                    item.PS = v;
                    item.SEVERE_PAT = SEVERE_PAT.ToString();
                    item.MODERATE_PAT = MODERATE_PAT.ToString();
                    item.MILD_PAT = MILD_PAT.ToString();
                    try
                    {
                        item.SOURCE = (double.Parse((PS * tempPatList.Count).ToString()) / double.Parse(MvcApplication.hospList.Find(x => x.HOSP_KEY == item.HOSP_KEY).EDOBSERVBEDS)).ToString();
                        double tempd = 0;
                        double.TryParse(item.SOURCE, out tempd);
                        item.SOURCE = tempd.ToString();
                        if (item.SOURCE == "非數值")
                        {
                            item.SOURCE = "0";
                        }
                        else
                        {
                            item.SOURCE = Math.Round(double.Parse(item.SOURCE), 2).ToString();
                        }
                    }
                    catch (Exception ex)
                    {
                        Com.Mayaminer.LogTool.SaveLogMessage(Newtonsoft.Json.JsonConvert.SerializeObject(item), actionName, this.csName);
                        Com.Mayaminer.LogTool.SaveLogMessage(ex, actionName, this.csName);
                        item.SOURCE = "0";
                    }
                    item.MODIFY_DATE = Function_Library.getDateNowString(DATE_FORMAT.yyyy_MM_dd_HHmmss);
                    item.MODIFY_ID = this.userinfo.user_id;
                    item.MODIFY_NAME = this.userinfo.user_name;
                }
                List<string> disSource = tempList.Select(x => x.SOURCE).Distinct().ToList();
                this.DBLink.DBA.DBExecUpdate<DB_MC_HOSP_INFO_DTL>(tempList);
                if (this.DBLink.DBA.hasLastError)
                {
                    MvcApplication.hospList = new List<DB_MC_HOSP_INFO>();
                    Com.Mayaminer.LogTool.SaveLogMessage(this.DBLink.DBA.lastError, actionName, this.csName);
                }
            }
        }

        // 𝑺𝒄𝒐𝒓𝒆_𝒊𝒋=𝒙_𝒊𝒋+𝒚_𝒊𝒋 (𝟏−𝒛_𝒊𝒋 )+ 𝒚_𝒊𝒋 𝒘𝟐_𝒊𝒋  
        // 𝑥_𝑖𝑗="Drive−Time"
        // 依據Google Maps- Distance Matrix API，計算從災害地點到每一家醫院的開車時間。
        // 將開車時間排序，並給予序號，如25, 27, 27,  27, 30, 30 >>> 1, 3, 3, 3, 6, 6 ，詳見p.4
        // 𝑦_𝑖𝑗 "(" 𝐴𝑑𝑒𝑞𝑢𝑎𝑐𝑦)=𝑤𝑒𝑖𝑔ℎ𝑡 𝑣𝑎𝑙𝑢𝑒 𝑖𝑛 𝑚𝑎𝑡𝑟𝑖𝑥 
        // 醫院對該傷患的醫療能力適當性，按照矩陣表對照數據
        // 1 ~ 10 represent hospital level, 1表示有急診病床且非ERH
        // 以1, 2,6,10 代表各程度傷勢對應各級醫院的適當性，
        // 𝑧_𝑖𝑗=(𝑤1_𝑖𝑗∗𝑅𝑒𝑐𝑒𝑖𝑣𝑒𝑑 𝐶𝑎𝑠𝑢𝑎𝑙𝑡𝑖𝑒𝑠(𝑙𝑜𝑎𝑑𝑖𝑛𝑔))/(ED B𝑒𝑑𝑠) ，詳見p.4
        // 重傷
        // 依據𝑐𝑟𝑖𝑡𝑖𝑐𝑎𝑙 ED 𝐵𝑒𝑑𝑠及Received Casualties變動
        // 𝑐𝑟𝑖𝑡𝑖𝑐𝑎𝑙 ED 𝐵𝑒𝑑𝑠臨界值=  ED B𝑒𝑑𝑠_𝑖𝑗/(𝑄1(=25))∗4
        // 全台重度急救責任醫院急診觀察床取Q1
        // 中傷或輕傷:𝑤1_𝑖𝑗 =2
        // 𝑤2_𝑖𝑗=內外科人力/急重症相關病床 < 1，詳見p.5


        #region 𝑺𝒄𝒐𝒓𝒆_𝒊𝒋=𝒙_𝒊𝒋+𝒚_𝒊𝒋 (𝟏−𝒛_𝒊𝒋 )+ 𝒚_𝒊𝒋 𝒘𝟐_𝒊𝒋  
        // 𝑺𝒄𝒐𝒓𝒆_𝒊𝒋=𝒙_𝒊𝒋+𝒚_𝒊𝒋 (𝟏−𝒛_𝒊𝒋 )+ 𝒚_𝒊𝒋 𝒘𝟐_𝒊𝒋  

        #region 𝑥_𝑖𝑗
        /// <summary>
        /// penalty_standadize
        /// </summary>
        /// <param name="cv">CV</param>
        /// <param name="n_pat">現在病人數</param>
        /// <returns></returns>
        public double penalty_standadize(int cv, int n_pat)
        {
            Double y = 2;
            int p_min = 1, p_max = cv, s_min = 2, s_max = 4;
            if (cv == 1)
            {
                y = 2;
            }
            else
            {
                double x = double.Parse((s_max - s_min).ToString()) / double.Parse((p_max - p_min).ToString());
                double b = s_min - (p_min * x);
                y = n_pat * x + b;
            }
            return y;
        }
        #endregion

        #region 𝑥_𝑖𝑗

        public void runDriving()
        {
            List<DB_MC_SITE_DRIVING_TIME_INFO> tempList = new List<DB_MC_SITE_DRIVING_TIME_INFO>();
            List<DB_MC_SITE_INFO> sList = getMC_SITE_INFO();
            List<DB_MC_HOSP_INFO> pList = MvcApplication.hospList.ToList();
            List<string> dList = this.getDB_DRIVING_SITE(sList.Select(x => x.SITE_ID).ToList());
            foreach (DB_MC_SITE_INFO s in sList)
            {
                if (!dList.Exists(x => x == s.SITE_ID))
                {
                    foreach (DB_MC_HOSP_INFO h in pList)
                    {
                        var Dis = (int)(new GeoCoordinate(double.Parse(h.LATITUDE), double.Parse(h.LONGITUDE))).GetDistanceTo(
                           new GeoCoordinate(double.Parse(s.LATITUDE), double.Parse(s.LONGITUDE))
                           );
                        tempList.Add(new DB_MC_SITE_DRIVING_TIME_INFO()
                        {
                            HOSP_KEY = h.HOSP_KEY,
                            SITE_ID = s.SITE_ID,
                            CREATE_DATE = Function_Library.getDateNowString(DATE_FORMAT.yyyy_MM_dd_HHmmss),
                            CREATE_ID = this.userinfo.user_id,
                            CREATE_NAME = this.userinfo.user_name,
                            MODIFY_DATE = Function_Library.getDateNowString(DATE_FORMAT.yyyy_MM_dd_HHmmss),
                            MODIFY_ID = this.userinfo.user_id,
                            MODIFY_NAME = this.userinfo.user_name,
                            DATASTATUS = "1",
                            DRIVING_TIME = Math.Round((double.Parse(Dis.ToString()) / 1000) > 60 ? 60 : (double.Parse(Dis.ToString()) / 1000), 0).ToString(),
                        });
                    }
                    this.time_ordinal(ref tempList);
                    this.DBLink.DBA.DBExecInsert<DB_MC_SITE_DRIVING_TIME_INFO>(tempList);
                } 
            } 
        }

        /// <summary>
        /// 𝑥_𝑖𝑗 = "Drive−Time" 公式 正規畫 取得距離或是時間順序
        /// 最小的數字最大
        /// 最大的數字最小
        /// </summary>
        /// <param name="pList"></param>
        public void time_ordinal(ref List<DB_MC_SITE_DRIVING_TIME_INFO> pList )
        { 
            int num = 0;
            List<string> tempList = new List<string>();
            tempList = pList.Select(x=>x.DRIVING_TIME).Distinct().OrderByDescending(x=> double.Parse(x)).ToList();
            pList.ForEach(x=>x.DRIVING_SOURCE = (tempList.FindIndex(y => y == x.DRIVING_TIME)).ToString());
            pList = pList.FindAll(x => x.DRIVING_SOURCE != "0").ToList();
        }
        #endregion

         
        #endregion 

        /// <summary>
        /// 取得醫院
        /// </summary>
        private void gethospList()
        {
            string actionName = "gethospList";
            string sql = "SELECT * FROM " + DB_TABLE_NAME.DB_MC_HOSP_INFO + " WHERE DATASTATUS = '1';";
            MvcApplication.hospList = this.DBLink.DBA.getSqlDataTable<DB_MC_HOSP_INFO>(sql);
            if (this.DBLink.DBA.hasLastError)
            {
                MvcApplication.hospList = new List<DB_MC_HOSP_INFO>();
                Com.Mayaminer.LogTool.SaveLogMessage(this.DBLink.DBA.lastError, actionName, this.csName);
            }
        }

        /// <summary>
        /// 取得CV
        /// </summary>
        /// <param name="pDate"></param>
        /// <returns></returns>
        private List<DB_MC_HOSP_INFO_DTL> getMC_HOSP_INFO_DTL(string pDate)
        {
            List<DB_MC_HOSP_INFO_DTL> tempList = new List<DB_MC_HOSP_INFO_DTL>();
            string sql = "SELECT * FROM " + DB_TABLE_NAME.DB_MC_HOSP_INFO_DTL + " WHERE DATASTATUS = '1' AND SOURCE_DATE = @SOURCE_DATE";
            Dapper.DynamicParameters dp = new Dapper.DynamicParameters();
            dp.Add("SOURCE_DATE", pDate);
            tempList = this.DBLink.DBA.getSqlDataTable<DB_MC_HOSP_INFO_DTL>(sql, dp);
            return tempList;
        }


        /// <summary>
        /// 取得病患資料
        /// </summary>
        /// <param name="pPATIENT_ID"></param>
        /// <returns></returns>
        private List<DB_MC_PATIENT_INFO> getMC_PATIENT_INFO(string pPATIENT_ID = "", string pWhere = "")
        {
            List<DB_MC_PATIENT_INFO> tempList = new List<DB_MC_PATIENT_INFO>();
            string sql = "SELECT * FROM " + DB_TABLE_NAME.DB_MC_PATIENT_INFO + " WHERE DATASTATUS = '1';";
            Dapper.DynamicParameters dp = new Dapper.DynamicParameters();
            if (!string.IsNullOrWhiteSpace(pPATIENT_ID))
            {
                sql += " AND PATIENT_ID =@PATIENT_ID";
                dp.Add("PATIENT_ID", pPATIENT_ID);
            }
            tempList = this.DBLink.DBA.getSqlDataTable<DB_MC_PATIENT_INFO>(sql, dp);
            return tempList;
        }


    }
}
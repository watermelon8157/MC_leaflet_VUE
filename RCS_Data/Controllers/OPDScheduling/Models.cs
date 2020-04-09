using Com.Mayaminer;
using Dapper;
using mayaminer.com.library;
using Newtonsoft.Json;
using RCS_Data.Models.DB;
using RCS_Data.Models.ViewModels;
using RCSData.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;

namespace RCS_Data.Controllers.OPDScheduling
{
    public class Models : BaseModels, Interface
    {
        string csName { get { return "RtCptAss Model"; } }
        const string dateFormat = "yyyy-MM-dd HH:mm:ss";
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

        public OPD_TREATMENT_VM week_data(string weakenSDate, string weakenEDate, int weanType, bool isCopy, int copysetCnt, bool isList = false)
        {
            OPD_TREATMENT_VM vm = new OPD_TREATMENT_VM();
            string actionName = "week_data";
            try
            {
                if (isCopy)
                    vm.getWeek(ref weakenSDate, ref weakenEDate, 2);
                else
                    vm.getWeek(ref weakenSDate, ref weakenEDate, weanType);
                vm.weakenSdate = weakenSDate;
                vm.weakenEdate = weakenEDate;
                vm.get_opd_count(weakenSDate, weakenEDate);
                vm.setData(isList);
                List<OPD_INFO> pList = vm.getWeekData(weakenSDate, weakenEDate, isCopy, copysetCnt);
                vm.opd_list = this.getshowData(pList, vm.opd_list);
                vm.opd_6_min_list = this.getshowData(pList, vm.opd_6_min_list, true);
            }
            catch (Exception ex)
            {
                LogTool.SaveLogMessage(ex, actionName, this.csName);
            }
            return vm;
        }

        public RESPONSE_MSG update_Data(OPD_Save form)
        {
            RESPONSE_MSG rm = new RESPONSE_MSG();
            string actionName = "update_Data";
            string rowStr = form.model;
            try
            {

                rowStr = HttpUtility.UrlDecode(rowStr);
                OPD_TREATMENT_JSON data = JsonConvert.DeserializeObject<OPD_TREATMENT_JSON>(rowStr);
                if (string.IsNullOrWhiteSpace(data.datastatus)) data.datastatus = "1";
                SQLProvider SQL = new SQLProvider();
                List<OPD_INFO> _list = new List<OPD_INFO>();
                if (data.opd_list.Exists(x => x.opd_per.Exists(y => y.opd_pat.Exists(z => z.check_pat == true))))
                    foreach (OPD_DATA od in data.opd_list)
                    {
                        foreach (OPD_PER op in od.opd_per)
                        {
                            if (op.opd_pat.Exists(x => x.check_pat == true))
                                _list.AddRange(op.opd_pat.FindAll(x => x.check_pat == true));
                        }
                    }
                if (data.opd_6_min_list.Exists(x => x.opd_per.Exists(y => y.opd_pat.Exists(z => z.check_pat))))
                    foreach (OPD_DATA od in data.opd_6_min_list)
                    {
                        foreach (OPD_PER op in od.opd_per)
                        {
                            if (op.opd_pat.Exists(x => x.check_pat == true))
                                _list.AddRange(op.opd_pat.FindAll(x => x.check_pat == true));
                        }
                    }
                if (_list.Count > 0)
                {

                    _list.ForEach(x => { x.MEMO = data.MEMO; x.datastatus = data.datastatus; });
                    string _sql = "UPDATE RCS_RT_OPD_SCHEDULING SET datastatus =@datastatus,MEMO =@MEMO WHERE TREATMENT_DATE = @TREATMENT_DATE AND CHART_NO = @CHART_NO AND TREATMENT_PER_DATA = @TREATMENT_PER_DATA AND TREATMENT_PER_SORT = @TREATMENT_PER_SORT AND treatment_type = @treatment_type;";

                    SQL.DBA.DBExecute<OPD_INFO>(_sql, _list);
                    if (SQL.DBA.hasLastError)
                    {
                        rm.status = RESPONSE_STATUS.ERROR;
                        rm.message = "修改失敗!";
                        LogTool.SaveLogMessage(SQL.DBA.lastError, actionName, this.csName);
                    }
                    else
                    {
                        rm.status = RESPONSE_STATUS.SUCCESS;
                        rm.message = "修改成功!";
                    }
                }
                else
                {
                    rm.status = RESPONSE_STATUS.ERROR;
                    rm.message = "無勾選資料!";
                }
            }
            catch (Exception ex)
            {
                rm.status = RESPONSE_STATUS.ERROR;
                rm.message = "修改失敗!程式碼發生錯誤!";
                LogTool.SaveLogMessage(ex, actionName, this.csName);
            }
            return rm;
        }

        public RESPONSE_MSG week_save(OPD_Save form) {

            RESPONSE_MSG rm = new RESPONSE_MSG();
            string actionName = "week_save";
            SQLProvider SQL = new SQLProvider();
            try
            {
                string rowStr = form.model;

                rowStr = HttpUtility.UrlDecode(rowStr);
                OPD_TREATMENT_JSON data = JsonConvert.DeserializeObject<OPD_TREATMENT_JSON>(rowStr);
                List<OPD_INFO> saveData = new List<OPD_INFO>();
                saveData.AddRange(this.getsaveData(data.opd_list));
                saveData.AddRange(this.getsaveData(data.opd_6_min_list, true));
                List<string> dateList = new List<string>();
                for (DateTime i = DateTime.Parse(data.weakenSdate); i <= DateTime.Parse(data.weakenEdate); i = i.AddDays(1))
                {
                    dateList.Add(i.ToString("yyyy-MM-dd"));
                }
                string _sql = "";

                _sql = "DELETE RCS_RT_OPD_SCHEDULING WHERE treatment_date in @treatment_date ;";
                Dapper.DynamicParameters dp = new Dapper.DynamicParameters();
                dp.Add("treatment_date", dateList);
                SQL.DBA.DBExecute(_sql, dp);
                _sql = string.Concat("INSERT INTO RCS_RT_OPD_SCHEDULING (MEMO,treatment_day,treatment_type,treatment_date,treatment_per,treatment_per_data,treatment_per_sort,patient_name,chart_no,opd_count,ipd_no,datastatus,treatment_data,drug_inhalation,CREATE_ID,CREATE_NAME,CREATE_DATE,ORDERBY) VALUES(@MEMO,@treatment_day,@treatment_type,@treatment_date,@treatment_per,@treatment_per_data,@treatment_per_sort,@patient_name,@chart_no,@opd_count,@ipd_no,@datastatus,@treatment_data,@drug_inhalation,'", form.user_info.user_id, "','", form.user_info.user_name, "','", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), "',@orderby);");
                SQL.DBA.DBExecute<OPD_INFO>(_sql, saveData);
                if (SQL.DBA.hasLastError)
                {
                    SQL.DBA.Rollback();
                    rm.status = RESPONSE_STATUS.ERROR;
                    rm.message = "儲存失敗!";
                    LogTool.SaveLogMessage(SQL.DBA.lastError, actionName, this.csName);
                }
                else
                {
                    SQL.DBA.Commit();
                    rm.status = RESPONSE_STATUS.SUCCESS;
                    rm.message = "儲存成功!";
                }
            }
            catch (Exception ex)
            {
                SQL.DBA.Rollback();
                rm.status = RESPONSE_STATUS.EXCEPTION;
                rm.message = "儲存失敗!程式碼發生錯誤!";
                LogTool.SaveLogMessage(ex, actionName, this.csName);
            }

            return rm;

        }
        /// <summary>
        /// 查詢病患
        /// </summary>
        /// <returns></returns>
        public List<OPD_INFO> search_chart_no(string chart_no)
        {
            List<OPD_INFO> List = new List<OPD_INFO>();
            string actionName = "search_chart_no";
            try
            {
                SQLProvider SQL = new SQLProvider();
                string _sql = "SELECT * FROM RCS_RT_CASE WHERE CHART_NO = @CHART_NO";
                Dapper.DynamicParameters dp = new Dapper.DynamicParameters();
                dp.Add("CHART_NO", chart_no);
                List<IPDPatientInfo> patList = SQL.DBA.getSqlDataTable<IPDPatientInfo>(_sql, dp);
                if (SQL.DBA.hasLastError)
                {
                    LogTool.SaveLogMessage(SQL.DBA.lastError, actionName, this.csName);
                }
                else
                {
                    if (patList.Count > 0)
                    {
                        IPDPatientInfo _pat = patList[0];
                        OPD_INFO _item = new OPD_INFO();
                        _item.chart_no = _pat.chart_no;
                        _item.patient_name = _pat.patient_name;
                        _item.ipd_no = "";
                        List.Add(_item);
                    }
                }
            }
            catch (Exception ex)
            {
                LogTool.SaveLogMessage(ex, actionName, this.csName);
            }
            return List;
        }
        private List<OPD_INFO> getsaveData(List<OPD_DATA> pList, bool is6min = false)
        {
            List<OPD_INFO> _dataList = new List<OPD_INFO>();
            string actionName = "getsaveData";
            OPD_TREATMENT_VM vm = new OPD_TREATMENT_VM();
            try
            {
                foreach (OPD_DATA dp in pList)
                {
                    foreach (OPD_PER op in dp.opd_per)
                    {
                        for (int i = 0; i < op.opd_pat.Count; i++)
                        {
                            var oi = op.opd_pat[i];
                            oi.treatment_day = dp.treatment_day;
                            oi.treatment_type = is6min ? "1" : "0";
                            oi.treatment_date = dp.treatment_date;
                            oi.treatment_data = JsonConvert.SerializeObject(oi.treatment);
                            oi.TREATMENT_PER_SORT = i.ToString();
                            oi.treatment_per = JsonConvert.SerializeObject(op.treatment_per);
                            oi.treatment_per_data = vm.timePer.FindIndex(x => JsonConvert.SerializeObject(x) == oi.treatment_per) + 1;
                            oi.datastatus = string.IsNullOrWhiteSpace(oi.datastatus) ? "1" : oi.datastatus;
                            _dataList.Add(oi);
                        }

                    }
                }
            }
            catch (Exception ex)
            {
                LogTool.SaveLogMessage(ex, actionName, this.csName);
            }
            return _dataList;
        }
        private List<OPD_DATA> getshowData(List<OPD_INFO> pList, List<OPD_DATA> _data, bool is6min = false)
        {

            string actionName = "getshowData";
            string _treatment_type = is6min ? "1" : "0";
            try
            {
                foreach (OPD_INFO oi in pList.FindAll(x => x.treatment_type == _treatment_type))
                {
                    oi.treatment = JsonConvert.DeserializeObject<List<string>>(oi.treatment_data);
                    #region 針對部分項目排序
                    if (oi.treatment.Count > 0)
                    {
                        List<string> _tempList = new List<string>();
                        if (oi.treatment.Exists(x => x == "負壓呼吸器"))
                        {
                            _tempList.Add("負壓呼吸器");
                            oi.treatment.Remove("負壓呼吸器");
                        }
                        if (oi.treatment.Exists(x => x == "H.F-N-CPAP"))
                        {
                            _tempList.Add("H.F-N-CPAP");
                            oi.treatment.Remove("H.F-N-CPAP");
                        }
                        if (oi.treatment.Exists(x => x == "ultrasound"))
                        {
                            _tempList.Add("ultrasound");
                            oi.treatment.Remove("ultrasound");
                        }
                        _tempList.AddRange(oi.treatment);
                        oi.treatment = _tempList;
                    }
                    #endregion
                    if (_data.Exists(x => x.treatment_date == oi.treatment_date &&
                        x.opd_per.Exists(y => JsonConvert.SerializeObject(y.treatment_per) == oi.treatment_per)))
                        _data.Find(x => x.treatment_date == oi.treatment_date).opd_per.Find(y => JsonConvert.SerializeObject(y.treatment_per) == oi.treatment_per).opd_pat.Add(oi);
                }

                _data.ForEach(x => {
                    x.treatment_person = x.opd_per.Sum(y => y.opd_pat.Count());
                    x.opd_per.ForEach(y => y.opd_pat = y.opd_pat.OrderBy(z => z.orderby_sort.PadLeft(4, '0')).ToList());
                });

            }
            catch (Exception ex)
            {
                LogTool.SaveLogMessage(ex, actionName, this.csName);
            }
            return _data;
        }
    }

    public class FormOPDList : AUTH
    {
        public string weakenSDate { get; set; }
        public string weakenEDate { get; set; }
        public int weanType { get; set; }
        public bool isCopy { get; set; }
        public int copysetCnt { get; set; }
        public bool isList { get; set; }
    }

    public class OPD_Save : AUTH
    {
        public string model { get; set; }

    }

    public class OPD_TREATMENT_JSON
    {
        /// <summary>
        /// 起始日期
        /// </summary>
        public string weakenSdate { get; set; }

        /// <summary>
        /// 起訖日期
        /// </summary>
        public string weakenEdate { get; set; }

        /// <summary>
        /// 週表標題
        /// </summary>
        public string opd_title { get; set; }

        public List<OPD_DATA> opd_list { get; set; }

        public List<OPD_DATA> opd_6_min_list { get; set; }

        public string MEMO { get; set; }

        public string datastatus { get; set; }

    }

    public class OPD_TREATMENT_VM : OPD_TREATMENT_JSON
    {
        public List<string> weaken { get; set; }

        public List<string> not_show_day { get; set; }


        /// <summary>
        /// 時段
        /// </summary>
        public List<string[]> timePer { get; set; }

        public OPD_TREATMENT_VM()
        {
            this.not_show_day = new List<string>() { "日", "六" };
            this.weaken = new List<string>() { "日", "一", "二", "三", "四", "五", "六" };
            this.timePer = new List<string[]>() {
                    new string[2] { "08:20", "" },
                    new string[2] { "08:25", "" },
                    new string[2] { "08:30", "" },
                    new string[2] { "09:30", "" },
                    new string[2] { "09:35", "" },
                    new string[2] { "09:40", "" },
                    new string[2] { "10:40", "" },
                    new string[2] { "10:50", "" },
                    new string[2] { "13:00", "" },
                    new string[2] { "13:05", "" },
                    new string[2] { "13:10", "" },
                    new string[2] { "14:00", "" },
                    new string[2] { "14:10", "" },
                    new string[2] { "14:20", "" }
            };

            this.opd_list = new List<OPD_DATA>();
            this.opd_6_min_list = new List<OPD_DATA>();

            this.opd_title = "OPD PR 週表單";
        }

        public void setData(bool isList)
        {
            for (int i = 0; i < this.weaken.Count; i++)
            {
                this.opd_list.Add(new OPD_DATA()
                {
                    treatment_day = this.weaken[i],
                    treatment_date = DateTime.Parse(this.weakenSdate).AddDays(i).ToString("yyyy-MM-dd"),
                    treatment_type = "1",
                    opd_per = this.timePer.Select(x => new OPD_PER() { treatment_per = x }).ToList()
                });
                this.opd_6_min_list.Add(new OPD_DATA()
                {
                    treatment_day = this.weaken[i],
                    treatment_date = DateTime.Parse(this.weakenSdate).AddDays(i).ToString("yyyy-MM-dd"),
                    treatment_type = "0",
                    opd_per = this.timePer.Select(x => new OPD_PER() { treatment_per = x }).ToList()
                });
            }
        }

        public List<OPD_INFO> getWeekData(string weakenSDate, string weakenEDate, bool isCopy = false, int copysetCnt = 1)
        {
            List<OPD_INFO> pList = new List<OPD_INFO>();
            string sDate = weakenSDate, eDate = weakenEDate;
            if (isCopy)
            {

                for (int i = 0; i < copysetCnt; i++)
                {
                    this.getWeek(ref sDate, ref eDate, -1);
                }
            }
            pList = this.getDataList(sDate, eDate, isCopy);
            if (isCopy && pList.Count > 0)
            {
                int cnt = 0;
                for (DateTime i = DateTime.Parse(sDate); i <= DateTime.Parse(eDate); i = i.AddDays(1))
                {
                    if (pList.Exists(x => DateTime.Parse(x.treatment_date) == i))
                        pList.FindAll(x => DateTime.Parse(x.treatment_date) == i).ForEach(x => x.treatment_date = DateTime.Parse(weakenSDate).AddDays(cnt).ToString("yyyy-MM-dd"));
                    cnt++;
                }
                pList = pList.OrderBy(x => x.treatment_per_data).ThenBy(x => int.Parse(x.TREATMENT_PER_SORT)).ToList();

            }


            return pList;
        }
        private List<OPD_INFO> getDataList(string weakenSDate, string weakenEDate, bool isCopy = false)
        {
            List<OPD_INFO> pList = new List<OPD_INFO>();
            SQLProvider SQL = new SQLProvider();
            string _sql = "SELECT * FROM RCS_RT_OPD_SCHEDULING WHERE treatment_date BETWEEN @sDate AND @eDate AND datastatus <> '9'";

            Dapper.DynamicParameters dp = new Dapper.DynamicParameters();
            dp.Add("sDate", weakenSDate);
            dp.Add("eDate", weakenEDate);
            pList = SQL.DBA.getSqlDataTable<OPD_INFO>(_sql, dp);
            if (SQL.DBA.hasLastError)
            {
                //D:\maya\RCS\source_code\localhost_1896_ECK\ECK\RCS\RCS\Models\ViewModel\OPD_TREATMENT_VM.cs
                Com.Mayaminer.LogTool.SaveLogMessage(SQL.DBA.lastError, "getDataList", "OPD_TREATMENT_VM");
            }
            if (isCopy)
            {
                pList.ForEach(x => { x.MEMO = ""; x.ipd_no = ""; x.opd_count = ""; });
            }

            return pList;
        }

        public void get_opd_count(string weakenSDate, string weakenEDate)
        {
            List<OPD_INFO> pList = this.getDataList(weakenSDate, weakenEDate);
            if (pList.Count > 0)
            {
                if (pList.Exists(x => string.IsNullOrWhiteSpace(x.opd_count)))
                {
                    List<OPD_INFO> _List = pList.FindAll(x => string.IsNullOrWhiteSpace(x.opd_count));

                    // TODO:取得門診診別
                    //foreach (OPD_INFO item in _List)
                    //{
                    //    //收尋急診門診病患
                    //    if (RCS.Controllers.BaseController.SearchErAndOpPatient)
                    //    {
                    //        HISData<IPDPatientInfo> erAndOPDList = new HISDataExchange().getERAndOPDListbyChartNo(item.chart_no);
                    //        if (erAndOPDList.datastatus == HISDataStatus.SuccessWithData)
                    //        {
                    //            if (erAndOPDList.dataList.Count > 0)
                    //            {
                    //                if (erAndOPDList.dataList.Exists(x => DateTime.Parse(x.diag_date).Date == DateTime.Parse(item.treatment_date).Date))
                    //                {
                    //                    IPDPatientInfo _pat = erAndOPDList.dataList.Find(x => DateTime.Parse(x.diag_date).Date == DateTime.Parse(item.treatment_date).Date);
                    //                    item.ipd_no = _pat.ipd_no;
                    //                    item.opd_count = _pat.source_count;
                    //                }
                    //            }
                    //        }
                    //    }
                    //}
                    if (_List.Exists(x => !string.IsNullOrWhiteSpace(x.ipd_no) && !string.IsNullOrWhiteSpace(x.opd_count)))
                    {
                        string _sql = "UPDATE RCS_RT_OPD_SCHEDULING SET ipd_no = @ipd_no,opd_count =@opd_count WHERE TREATMENT_DATE = @TREATMENT_DATE AND CHART_NO = @CHART_NO AND TREATMENT_PER_DATA = @TREATMENT_PER_DATA AND TREATMENT_PER_SORT = @TREATMENT_PER_SORT AND treatment_type = @treatment_type;";
                        SQLProvider SQL = new SQLProvider();
                        SQL.DBA.DBExecute<OPD_INFO>(_sql, _List.FindAll(x => !string.IsNullOrWhiteSpace(x.ipd_no) && !string.IsNullOrWhiteSpace(x.opd_count)));
                        if (SQL.DBA.hasLastError)
                        {
                            Com.Mayaminer.LogTool.SaveLogMessage(SQL.DBA.lastError, "get_opd_count", "OPD_TREATMENT_VM");
                        }
                    }

                }
            }

        }

        public void getWeek(ref string weakenSDate, ref string weakenEDate, int weanType)
        {
            if (string.IsNullOrWhiteSpace(weakenSDate))
            {
                DateTime date_weak = DateTime.Now.AddDays(Convert.ToDouble((0 - Convert.ToInt16(DateTime.Now.DayOfWeek)) + 0));
                weakenSDate = date_weak.ToString("yyyy-MM-dd");
            }
            else
            {
                DateTime date_weak = DateTime.Parse(weakenSDate);
                switch (weanType)
                {
                    case -1:
                        //上週
                        weakenSDate = date_weak.AddDays(-7).AddDays(Convert.ToDouble((0 - Convert.ToInt16(date_weak.DayOfWeek)) + 0)).ToString("yyyy-MM-dd");
                        break;
                    case 1:
                        //下週
                        weakenSDate = date_weak.AddDays(7).AddDays(Convert.ToDouble((0 - Convert.ToInt16(date_weak.DayOfWeek)) + 0)).ToString("yyyy-MM-dd");
                        break;
                    case 0:
                        //本周
                        date_weak = DateTime.Now.AddDays(Convert.ToDouble((0 - Convert.ToInt16(DateTime.Now.DayOfWeek)) + 0));
                        weakenSDate = date_weak.ToString("yyyy-MM-dd");
                        break;
                    default:
                        //維持帶入日期
                        break;
                }
            }
            weakenEDate = DateTime.Parse(weakenSDate).AddDays(6).ToString("yyyy-MM-dd");
        }

    }

    /// <summary>
    /// 門診資料
    /// </summary>
    public class OPD_DATA
    {

        public bool check_day { get; set; }

        public OPD_DATA()
        {
            this.opd_per = new List<OPD_PER>();
        }
        /// <summary>
        /// 療程人數
        /// </summary>
        public int treatment_person { get; set; }
        /// <summary>
        /// 療程星期
        /// </summary>
        public string treatment_day { get; set; }
        /// <summary>
        /// 療程類型
        /// </summary>
        public string treatment_type { get; set; }

        /// <summary>
        /// 療程日期
        /// </summary>
        public string treatment_date { get; set; }


        public List<OPD_PER> opd_per { get; set; }
    }


    public class OPD_PER
    {
        public OPD_PER()
        {
            this.opd_pat = new List<OPD_INFO>();
        }
        /// <summary>
        /// 療程區間
        /// </summary>
        public string[] treatment_per { get; set; }

        /// <summary>
        /// 療程病患
        /// </summary>
        public List<OPD_INFO> opd_pat { get; set; }

    }

    /// <summary>
    /// 門診資料的資訊
    /// </summary>
    public class OPD_INFO
    {
        /// <summary>
        /// 勾選病患
        /// </summary>
        public bool check_pat { get; set; }
        /// <summary>
        /// 療程星期
        /// </summary>
        public string treatment_day { get; set; }
        /// <summary>
        /// 療程類型 0: 門診療程 1: 6min療程
        /// </summary>
        public string treatment_type { get; set; }

        /// <summary>
        /// 療程日期
        /// </summary>
        public string treatment_date { get; set; }

        /// <summary>
        /// 治療區間資料
        /// </summary>
        public int treatment_per_data { get; set; }


        /// <summary>
        /// 療程區間 
        /// </summary>
        public string treatment_per_show
        {
            get
            {
                if (!string.IsNullOrWhiteSpace(treatment_per))
                    return string.Join(" - ", Newtonsoft.Json.JsonConvert.DeserializeObject<List<string>>(treatment_per));
                return "";
            }
        }

        /// <summary>
        /// 療程區間 
        /// </summary>
        public string treatment_per { get; set; }

        /// <summary>
        /// 病患姓名
        /// </summary>
        public string patient_name { set; get; }
        /// <summary>
        /// 病歷號
        /// </summary>
        public string chart_no { set; get; }

        /// <summary>
        /// 病歷號_去零
        /// </summary>
        public string show_chart_no
        {
            get
            {
                if (string.IsNullOrWhiteSpace(this.chart_no))
                {
                    return "";
                }
                else
                {
                    return this.chart_no.TrimStart('0');
                }
            }
        }

        /// <summary>
        /// 批價序號
        /// </summary>
        public string ipd_no { set; get; }

        /// <summary>
        /// 療程次數
        /// </summary>
        public string opd_count { set; get; }

        /// <summary>
        /// 資料狀態 
        /// </summary>
        public string datastatus { get; set; }

        /// <summary>
        /// 療程
        /// </summary>
        public List<string> treatment { get; set; }

        /// <summary>
        /// 療程
        /// </summary>
        public string treatment_data { get; set; }

        /// <summary>
        /// 吸入藥物
        /// </summary>
        public string drug_inhalation { get; set; }
        /// <summary>
        /// sort
        /// </summary>
        public string TREATMENT_PER_SORT { get; set; }

        private string _MEMO { get; set; }
        public string MEMO
        {
            get
            {
                if (string.IsNullOrWhiteSpace(this._MEMO))
                {
                    return "";
                }
                return this._MEMO;
            }
            set
            {
                this._MEMO = value;
            }

        }

        public string orderby { get; set; }
        public string orderby_sort { get { return string.IsNullOrWhiteSpace(this.orderby) ? "" : this.orderby; } }
    }

}

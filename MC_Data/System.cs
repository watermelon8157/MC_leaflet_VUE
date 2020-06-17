using Com.Mayaminer;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RCS_Data {


    public abstract class MODEL_SRTTING
    {

    }


    public class peak : ChartObject { }
    public class plateau : ChartObject { }
    public class fio2 : ChartObject { }
    public class peep : ChartObject { }


    public class ChartObject {
        public string label { set; get; }
        public List<List<float>> data = new List<List<float>>();
    }

    public class WebServiceResponse {
        /// <summary> 執行結果 </summary>
        public bool execute_state { set; get; }

        /// <summary> 錯誤訊息 </summary>
        public string error_msg { set; get; }

        /// <summary> 執行結果 </summary>
        public object ws_result { set; get; }

    }

    public class UserCompetence {

    }

    /// <summary>
    /// 顯示標籤色彩
    /// </summary>
    public enum TagColor {
        success = 1,
        danger = 2,
        info = 3,
        warning = 4,
        primary = 5
    }

    #region 排班表
    /// <summary>
    /// 排班表
    /// </summary>
    public class scheduling
    {
        public int year { set; get; }
        public int month { set; get; }
        public string month_off { set; get; }
        public string working_hr { set; get; }
        /// <summary>排班表資料</summary>
        public List<SchedulData> schedul_data = new List<SchedulData>();
        public List<ShiftData> shift_data = new List<ShiftData>();
        public List<AreaData> area_data = new List<AreaData>();
    }

    /// <summary>
    /// 排班表資料
    /// </summary>
    public class SchedulData
    {
        /// <summary>人員ID</summary>
        public string op_id { set; get; }
        /// <summary>人員名稱</summary>
        public string op_name { set; get; }
        /// <summary>備註</summary>
        public string remark { set; get; }
        /// <summary>排班日資料</summary>
        public Dictionary<string, DayData> day_data = new Dictionary<string, DayData>();
    }

    /// <summary>
    /// 排班日資料
    /// </summary>
    public class DayData
    {
        /// <summary>排班別</summary>

        public string stype { set; get; }
        /// <summary>區域</summary>
        public string area { set; get; }
        /// <summary>假別R/W</summary>

        public string wtype { set; get; }
    }
    /// <summary>
    /// 班別資料
    /// </summary>
    public class ShiftData
    {
        /// <summary>班別名稱</summary>
        public string name { set; get; }
        /// <summary>縮寫</summary>
        public string shortcut { set; get; }
        /// <summary>類型W=work,R=holiday</summary>
        public string work_type { set; get; }
    }

    #endregion

    #region 區域資料
    /// <summary>
    /// 區域資料
    /// </summary>
    public class AreaData
    {
        /// <summary>區域名稱</summary>
        public string name { set; get; }
        /// <summary>顏色代碼</summary>
        public string color { set; get; }
    }

    /// <summary>
    /// 區域分配資料
    /// </summary>
    public class RCS_RT_ASSIGNMENT
    {
        /// <summary>區域分配流水號</summary>
        public string A_ID { set; get; }
        /// <summary>班別:D:白天、N:小夜、E:大夜</summary>
        public string WORK_TYPE { set; get; }
        /// <summary>分配結果</summary>
        public string A_RESULT { set; get; }
        /// <summary>分配時間</summary>
        public string YYYYMMDD_DATE { set; get; }
        /// <summary>建立時間</summary>
        public string CREATE_DATE { set; get; }
        /// <summary>更新時間</summary>
        public string UPDATE_TIME { set; get; }
        /// <summary>更新者</summary>
        public string UPDATE_USER { set; get; }

    }
    #endregion

 
    public class BASE_DATA {

        public bool is_readonly { set; get; }

        /// <summary> 建立時間 </summary>
        public string create_date { set; get; }
        /// <summary> 建立人員工號 </summary>
        public string create_id { set; get; }
        /// <summary> 建立人員姓名 </summary>
        public string create_name { set; get; }
        /// <summary> 修改日期 </summary>
        public string modify_date { set; get; }
        /// <summary> 修改人員工號 </summary>
        public string modify_id { set; get; }
        /// <summary> 修改人員姓名 </summary>
        public string modify_name { set; get; }
        /// <summary>
        /// 護理站代碼
        /// </summary>
        public string COST_CODE { set; get; }
        /// <summary>
        /// 床號
        /// </summary>
        public string BED_NO { set; get; }
        /// <summary>
        /// 科別代碼
        /// </summary>
        public string DEPT_CODE { set; get; }
        /// <summary>
        /// 儲存狀態
        /// </summary>
        public string DATASTATUS { set; get; }
        /// <summary>
        /// 病患來源
        /// </summary>
        public string PAT_SOURCE { set; get; }
        /// <summary>
        /// 病患入院日期
        /// </summary>
        public string PAT_DATA_DATE { set; get; }
        /// <summary>
        /// 上傳狀態，1=已上傳成功、0=未上傳(失敗)
        /// </summary>
        public string UPLOAD_STATUS { set; get; }
    }

    /// <summary>
    /// jQuery 回傳物件
    /// {不得已違反命名規則，因為jquery本來就這樣定義}
    /// </summary>
    public class JQXHR {
        /// <summary> 讀取狀態 </summary>
        public string readyState { set; get; }
        
        /// <summary> 回傳內容 </summary>
        public string responseText { set; get; }
         
        /// <summary> 狀態 </summary>
        public string status { set; get; }
         
        /// <summary> 狀態描述 </summary>
        public string statusText { set; get; }
    }

    /// <summary>
    /// 前端系統記錄
    /// </summary>
    public class RCS_SYS_LOG : JQXHR{

        /// <summary> 記錄ID </summary>
        public string log_id { set; get; }
        
        /// <summary> 記錄時間 </summary>
        public string log_date { set; get; }
    
    }


    /// <summary> JSON_DATA儲存選項 </summary>
    public class JSON_DATA
    {
        /// <summary> 項目編號 </summary>
        public string id { set; get; }
        /// <summary> 選項文字或值 </summary>
        public string txt { set; get; }
        /// <summary> 依照各物件自行定義 </summary>
        public string val { set; get; }
        /// <summary> 是否選取 </summary>
        public bool chkd { set; get; }
    }




    public class BoostrapTable
    {

        /// <summary>
        /// 資料欄位名稱
        /// </summary>
        public string dataField { get; set; }
        /// <summary>
        /// 顯示表頭
        /// </summary>
        public string dataTitle { get; set; }
        /// <summary>
        /// 是否可以排序
        /// </summary>
        public bool dataSortable = false;
    }


    public class SetPatNowStatus : List<PatNowStatus>
    {
        public SetPatNowStatus()
        {
            this.Add(new PatNowStatus() { StatusCode = "INTUBE", StatusDesc = "插管" });
            this.Add(new PatNowStatus() { StatusCode = "MV", StatusDesc = "呼吸器" });
            this.Add(new PatNowStatus() { StatusCode = "O2", StatusDesc = "氧療" });
            this.Add(new PatNowStatus() { StatusCode = "DNR", StatusDesc = "DNR" });
            this.Add(new PatNowStatus() { StatusCode = "VPN", StatusDesc = "VPN" });
            this.Add(new PatNowStatus() { StatusCode = "CPT", StatusDesc = "CPT" });
        }
    }

    /// <summary>
    /// 病患現在狀態
    /// </summary>
    public class PatNowStatus
    {
        /// <summary>
        /// 狀態(true:有，false:無)
        /// </summary>
        public bool showStatus { get; set; }
        /// <summary>
        /// 狀態細節內容
        /// </summary>
        public string StatusDesc { get; set; }
        /// <summary>
        /// 狀態代碼
        /// </summary>
        public string StatusCode { get; set; }


    }


    /// <summary>
    /// 檢測連線狀態是否正常
    /// </summary>
    public class SystemDetection
    {
        /// <summary>
        /// 連線名稱
        /// </summary>
        public string connectionName { get; set; }
        /// <summary>
        /// 是否可以連線
        /// </summary>
        public bool isConnection { get; set; }
        /// <summary>
        /// 顯示錯誤訊息
        /// </summary>
        public string errorMsg { get; set; }
        /// <summary>
        /// 狀態說明文字
        /// </summary>
        public string titleStr
        {
            get
            {
                if (isConnection)
                {
                    return connectionName + "，狀態:連線正常";
                }
                else
                {
                    return connectionName + "，狀態:連線異常" + (string.IsNullOrWhiteSpace(errorMsg)? "": " 錯誤訊息:"+ errorMsg);
                }
            }
        }
        /// <summary>
        /// 狀態樣式
        /// </summary>
        public string glyphiconClass
        {
            get
            {
                if (isConnection)
                {
                    return "glyphicon glyphicon-ok";
                }
                else
                {
                    return "glyphicon glyphicon-remove";
                }
            }
        }

        /// <summary>
        /// 狀態顏色
        /// </summary>
        public string glyphiconColor
        {
            get
            {
                if (isConnection)
                {
                    return "green";
                }
                else
                {
                    return "red";
                }
            }
        }
    }

    /// <summary>
    /// 功能設定
    /// </summary>
    public class FunctionSetting
    {
        #region 功能判斷變數
        /// <summary>
        /// VPN上傳功能開關(true:開啟，false:關閉)
        /// </summary>
        public bool VPN_UPLOAD_Switch { get; set; }

        /// <summary>呼吸會診(true:開啟，false:關閉)</summary>
        public bool rcsConsultListSwitch { get; set; }
        /// <summary>
        /// 決策資源功能開關(true:開啟,false:關閉)
        /// </summary>
        public bool decisionSupportSwitch { get; set; }
        /// <summary>
        /// 系統檢測的功能(true:開啟,false:關閉)
        /// </summary>
        public bool systemDetectionSwitch { get; set; }
        /// <summary>
        /// 補呼吸照護記錄單的功能(true:開啟,false:關閉)
        /// </summary>
        public bool makeUpRTRecord { get; set; }
        /// <summary>
        /// 排班表功能開關(true:開啟功能,false:關閉功能)
        /// </summary>
        public bool SchedulingSwitch { get; set; }
        /// <summary>
        /// 手術病人及胸腔物理治療評估表多筆記錄(true:開啟,false:關閉)
        /// </summary>
        public bool CPTAssessHaveMultiRecord { get; set; }
        /// <summary>
        /// 檢查及設定保險組代碼(病歷室註記)(true:開啟,false:關閉)
        /// </summary>
        public bool checkMode_memo { get; set; }
        /// <summary>重要指標分析功能開關(true:開啟，false:關閉)</summary>
        public bool indicatorsAnalysisSwitch { get; set; }
        /// <summary>合併胸腔記錄單呼吸復原評估單開關(true:開啟合併，false:關閉合併)</summary>
        public bool bindCPTAndAssessSwitch { get; set; }
        /// <summary>可否搜尋急診門診病患</summary>
        public bool SearchErAndOpPatient { get; set; }
        /// <summary>顯示平板用scrollBar</summary>
        public bool showPadScrollBarBtn { get; set; }
        /// <summary>輔具評估功能開關(true:開啟,false:關閉)</summary>
        public bool MeasuresFormSwitch { get; set; }
        /// <summary>區域分派功能開關(true:開啟,false:關閉)</summary>
        public bool assignmentSwitch { get; set; }
        /// <summary>交班作業功能開關(true:開啟,false:關閉)</summary>
        public bool shiftWorkSwitch { get; set; }
        /// <summary>開放中央監控站(true:是，false:否)</summary>
        public bool isOpenCenterMonitor { get; set; }
        /// <summary>開放病歷號變更作業(true:是，false:否)</summary>
        public bool isOpenChartChange { get; set; }
        /// <summary>列印開關，除了照護清單、排班表(true:開啟列印,false:關閉列印) </summary>
        public bool printSwitch { get; set; }
        /// <summary>列印呼吸照護記錄單開關(true:開啟列印，false:關閉列印) </summary>
        public bool printRTRecordSwitch { get; set; }
        /// <summary>列印胸腔記錄單開關(true:開啟列印，false:關閉列印)</summary>
        public bool printCPTSwitch { get; set; }
        /// <summary>列印呼吸復原評估單開關(true:開啟列印，false:關閉列印) </summary>
        public bool printRTAssessSwitch { get; set; }
        /// <summary>列印呼吸脫離評估開關(true:開啟列印，false:關閉列印) </summary>
        public bool printRTTakeoffAssessSwitch { get; set; }
        /// <summary>
        /// 列印呼吸治療評估單(true:開啟列印，false:關閉列印)
        /// </summary>
        public bool printCPTPageSwitch { get; set; }
        /// <summary>上傳功能開關</summary>
        public bool upLoadSwitch { get; set; }
        /// <summary>授權功能開關(true:開啟授權,false:關閉授權) 
        /// <para>此功能會影響到Controller設定和View顯示</para> </summary>
        public bool RtGrantListSwitch { get; set; }
        /// <summary>是否交班上傳WebService(true:交班時上傳WebService,false:關閉交班上傳WebService) 
        /// <para>此功能會影響到交班是否要上傳</para> </summary>
        public bool shiftUploadSwitch { get; set; }
        /// <summary>是否顯示全螢幕按鈕(true:顯示,false:不顯示)</summary>
        public bool fullScreenSwitch { get; set; }
        /// <summary>CheckBoxList顯示隱藏
        /// <para>是否顯示全部的CheckBoxList(1:顯示全部,0:僅顯示有勾選的</para> </summary>
        public int showAllSelect { get; set; }
        /// <summary>是否顯示月報表按鈕(true:顯示，false:不顯示) </summary>
        public bool showMonthlyReport { get; set; }
        /// <summary>24小時判斷是否要關閉(true:是，false:否)</summary>
        public bool isClose24hours { get; set; }
        /// <summary>
        /// 權限設定功能(true:開啟，false:關閉)
        /// </summary>
        public bool AuthoritySwitch { get; set; }
        /// <summary>
        /// 加入病患的搜尋病患功能是否補0的功能(true:開啟,false:關閉)
        /// </summary>
        public bool aadPatientCheckChartNo { get; set; }
        /// <summary>
        /// 系統管理開關(true:開啟,false:關閉)
        /// </summary>
        public bool isOpenSystemManage { get; set; }
        /// <summary>
        /// 耗材維護設定開關(true:開啟,false:關閉)
        /// </summary>
        public bool SuppliesSwitch { get; set; }

        /// <summary>
        /// vip欄位設定開關(true:開啟檢查vip欄位資料,false:預設關閉)
        /// </summary>
        public bool VipColSettingsSwitch { get; set; }
        #endregion

        public FunctionSetting()
        {
            setFunctionSetting();
        }
        /// <summary>
        /// 設定功能
        /// </summary>
        public void setFunctionSetting()
        {
            string actionName = "setFunctionSetting";
            try
            {
                aadPatientCheckChartNo = bool.Parse(IniFile.GetConfig("System", "aadPatientCheckChartNo"));

                fullScreenSwitch = bool.Parse(IniFile.GetConfig("ShowConfig", "fullScreenSwitch"));
                showAllSelect = int.Parse(IniFile.GetConfig("ShowConfig", "showAllSelect"));
                showPadScrollBarBtn = bool.Parse(IniFile.GetConfig("ShowConfig", "showPadScrollBarBtn"));
                showMonthlyReport = bool.Parse(IniFile.GetConfig("ShowConfig", "showMonthlyReport"));

                printSwitch = bool.Parse(IniFile.GetConfig("PrintConfig", "printSwitch"));
                printRTRecordSwitch = bool.Parse(IniFile.GetConfig("PrintConfig", "printRTRecordSwitch"));
                printCPTSwitch = bool.Parse(IniFile.GetConfig("PrintConfig", "printCPTSwitch"));
                printRTAssessSwitch = bool.Parse(IniFile.GetConfig("PrintConfig", "printRTAssessSwitch"));
                printRTTakeoffAssessSwitch = bool.Parse(IniFile.GetConfig("PrintConfig", "printRTTakeoffAssessSwitch"));
                printCPTPageSwitch = bool.Parse(IniFile.GetConfig("PrintConfig", "printCPTPageSwitch"));

                CPTAssessHaveMultiRecord = bool.Parse(IniFile.GetConfig("SystemConfig", "CPTAssessHaveMultiRecord"));
                bindCPTAndAssessSwitch = bool.Parse(IniFile.GetConfig("SystemConfig", "bindCPTAndAssessSwitch"));
                SearchErAndOpPatient = bool.Parse(IniFile.GetConfig("SystemConfig", "SearchErAndOpPatient"));
                MeasuresFormSwitch = bool.Parse(IniFile.GetConfig("SystemConfig", "MeasuresFormSwitch"));
                assignmentSwitch = bool.Parse(IniFile.GetConfig("SystemConfig", "assignmentSwitch"));
                shiftWorkSwitch = bool.Parse(IniFile.GetConfig("SystemConfig", "shiftWorkSwitch"));
                isOpenCenterMonitor = bool.Parse(IniFile.GetConfig("SystemConfig", "isOpenCenterMonitor"));
                isOpenChartChange = bool.Parse(IniFile.GetConfig("SystemConfig", "isOpenChartChange"));
                upLoadSwitch = bool.Parse(IniFile.GetConfig("SystemConfig", "WS_Write_Switch"));
                isClose24hours = bool.Parse(IniFile.GetConfig("SystemConfig", "isClose24hours"));
                SchedulingSwitch = bool.Parse(IniFile.GetConfig("SystemConfig", "SchedulingSwitch"));
                makeUpRTRecord = bool.Parse(IniFile.GetConfig("SystemConfig", "makeUpRTRecord"));
                systemDetectionSwitch = bool.Parse(IniFile.GetConfig("SystemConfig", "systemDetectionSwitch"));
                decisionSupportSwitch = bool.Parse(IniFile.GetConfig("SystemConfig", "decisionSupportSwitch"));
                indicatorsAnalysisSwitch = bool.Parse(IniFile.GetConfig("SystemConfig", "indicatorsAnalysisSwitch"));
                rcsConsultListSwitch = bool.Parse(IniFile.GetConfig("SystemConfig", "rcsConsultListSwitch"));
                AuthoritySwitch = bool.Parse(IniFile.GetConfig("SystemConfig", "AuthoritySwitch"));
                isOpenSystemManage = bool.Parse(IniFile.GetConfig("SystemConfig", "isOpenSystemManage"));
                SuppliesSwitch = bool.Parse(IniFile.GetConfig("SystemConfig", "SuppliesSwitch"));
                VipColSettingsSwitch = bool.Parse(IniFile.GetConfig("SystemConfig", "VipColSettingsSwitch"));
                VPN_UPLOAD_Switch = bool.Parse(IniFile.GetConfig("SystemConfig", "VPN_UPLOAD_Switch"));
                checkMode_memo = bool.Parse(IniFile.GetConfig("SystemConfig", "checkMode_memo"));

                bool isDebuggerMode = bool.Parse(IniFile.GetConfig("System", "isDebuggerMode"));
#if DEBUG
                isDebuggerMode = true;
#endif
                if (bool.Parse(IniFile.GetConfig("System", "isBasicMode")) && !isDebuggerMode)
                {
                    SearchErAndOpPatient = false;
                    showPadScrollBarBtn = false;
                    MeasuresFormSwitch = false;
                    assignmentSwitch = false;
                    isOpenCenterMonitor = false;
                    isOpenChartChange = false;
                    printSwitch = false;
                    upLoadSwitch = false;
                    showMonthlyReport = false;
                    isClose24hours = false;
                    CPTAssessHaveMultiRecord = false;
                    decisionSupportSwitch = false;
                    makeUpRTRecord = false;
                    SuppliesSwitch = false;
                    indicatorsAnalysisSwitch = false;

                }
                if (isDebuggerMode)
                {
                    VPN_UPLOAD_Switch = true;
                }
            }
            catch (Exception ex)
            {
                LogTool.SaveLogMessage(ex, actionName, GetLogToolCS.BaseController);
            }
        }

    }


    public class RCS_SYS_USER_LIST : DB_RCS_SYS_USER_LIST
    {
        /// <summary>
        /// 權限區間
        /// </summary>
        public string DateRange { get { return string.Format("{0} 至 {1}", this.START_DATE, this.END_DATE); } }
        /// <summary>
        /// 顯示權限
        /// </summary>
        public string SHOW_USER_ROLE
        {
            get
            {
                string tempStr = "";
                switch (this.USER_ROLE)
                {
                    case "RT_admin":
                        tempStr = "呼吸治療小組長";
                        break;
                    case "RT":
                        tempStr = "呼吸治療師";
                        break;
                    case "doctor":
                        tempStr = "主治醫生";
                        break;
                    case "inquirer":
                        tempStr = "病歷課保險組人員";
                        break;
                    default:
                        tempStr = "未知權限";
                        break;
                }
                return tempStr;
            }
        }
        /// <summary>
        /// 顯示狀態
        /// </summary>
        public string SHOW_DATASTATUS
        {
            get
            {
                string tempStr = "";
                switch (this.DATASTATUS)
                {
                    case "1":
                        tempStr = "使用中";
                        break;
                    case "2":
                        tempStr = "權限延長一個月";
                        break;
                    case "9":
                        tempStr = "停用或使用期限已到期";
                        break;
                    default:
                        tempStr = "未知權限";
                        break;
                }
                return tempStr;
            }
        }

        public DateTime S_DATE
        {
            get
            {
                DateTime nowDate = DateTime.Now.AddYears(-1);
                DateTime.TryParse(this.START_DATE, out nowDate);
                return nowDate;
            }
        }


        public DateTime E_DATE
        { get
            {
                DateTime nowDate = DateTime.Now.AddYears(-1);
                DateTime.TryParse(this.END_DATE, out nowDate);
                return nowDate;
            }
        }

        /// <summary>
        /// 檢察使用者資料
        /// </summary>
        /// <returns></returns>
        public RESPONSE_MSG checkUserInfo()
        {
            RESPONSE_MSG rm = new RESPONSE_MSG();
            bool isCanLogin = false;
            //DATASTATUS = "9" 帳號被停用了
            if (this.DATASTATUS == "9")
            {
                this.DATASTATUS = "9";
                rm.status = RESPONSE_STATUS.ERROR;
                rm.message = "帳號已經被停用了，無法登入系統!";
            }
            else
            {
                //檢查帳號使用日期區間
                #region 檢查帳號使用日期區間
                DateTime nowDate = DateTime.Now;
                //檢察是否在區間內
                if (this.S_DATE < nowDate && this.E_DATE.AddDays(1) > nowDate)
                {
                    isCanLogin = true;
                    rm.status = RESPONSE_STATUS.SUCCESS;
                    rm.message = "帳號(" + this.USER_ID + ")使用日期至" + this.END_DATE;
                }
                else
                {
                    //DATASTATUS == "2" 延長使用一個月，是否有在區間內
                    if (this.DATASTATUS == "2" && this.S_DATE < nowDate && this.E_DATE.AddMonths(1).AddDays(1) > nowDate)
                    {
                        this.DATASTATUS = "2";
                        isCanLogin = true;
                        rm.status = RESPONSE_STATUS.SUCCESS;
                        rm.message = "帳號(" + this.USER_ID + ")使用日期至" + this.END_DATE+ "(延長使用一個月)";
                    }
                    else
                    {
#if DEBUG
                        isCanLogin = true;
                        rm.status = RESPONSE_STATUS.SUCCESS;
                        rm.message = "(DEUBG模式)帳號(" + this.USER_ID + ")使用日期至" + this.END_DATE;

#else
                        this.DATASTATUS = "9";
                        rm.status = RESPONSE_STATUS.ERROR;
                        rm.message = "帳號使用期限已到期，無法登入系統!";
#endif
                    }
                }
#endregion

                //如果檢察通過，就可以登入系統
                if (!isCanLogin && rm.status == RESPONSE_STATUS.SUCCESS)
                {
                    this.DATASTATUS = "9";
                    rm.status = RESPONSE_STATUS.ERROR;
                    rm.message = "登入失敗。請洽資訊人員!";
                }
            }
            return rm;
        }


        public bool isEdit { get; set; }

    }

    #region 使用者行為記錄

    /// <summary>
    /// 使用者行為歷史記錄
    /// </summary>
    public class LogUserUseRecord
    {
        /// <summary>
        /// 使用者ID
        /// </summary>
        public string user_id { get; set; }

        /// <summary>
        /// 使用控制器
        /// </summary>
        public string use_controller { get; set; }

        /// <summary>
        /// 使用方法
        /// </summary>
        public string use_action { get; set; }

        /// <summary>
        /// 使用時間
        /// </summary>
        public string use_time { get; set; }

    }


    public class LogUserUseHistoryRecord : List<LogUserUseRecord>
    {
        /// <summary>
        /// 是否記錄使用者詳細資料，此使用功能詳細內容
        /// </summary>
        public bool recordDetail { get; set; }
    }

    #endregion

 
}

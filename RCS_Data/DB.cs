using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

/// <summary>
/// 放資料庫DD的物件
/// </summary>
namespace RCS_Data
{
    /// <summary>呼吸照護紀錄單的插管/呼吸器/氧療紀錄表</summary>
    public class RCS_ONMODE_MASTER
    {
        public RCS_ONMODE_MASTER()
        {
            WEANINGDATE = new List<List<JSON_DATA>>();
            DATA_CONTENT = new List<JSON_DATA>();
        }
        /// <summary>是否可以修改開始日期、呼吸器適應症</summary>
        public bool canFixSdate { get; set; }
        /// <summary>是否有資料</summary>
        public bool hasData { get; set; }
        /// <summary>是開始資料</summary>
        public bool statusStart { get; set; }
        /// <summary>是結束資料</summary>
        public bool statusEnd { get; set; }
        /// <summary>是否記錄已結束</summary>
        public bool isEnd { get; set; }
        /// <summary>流水號</summary>
        public string ONMODE_ID { get; set; }
        ///// <summary>住院帳號</summary>
        //public string IPD_NO  { get; set; }
        ///// <summary>病歷號</summary>
        //public string CHART_NO  { get; set; }
        /// <summary>開始時間</summary>
        public string STARTDATE { get; set; }
        /// <summary>嘗試脫離時間</summary>
        public List<List<JSON_DATA>> WEANINGDATE { get; set; }
        /// <summary>結束時間</summary>
        public string ENDDATE { get; set; }
        /// <summary>模式 1:插管、2:呼吸器、3:氧療</summary>
        private string _ON_TYPE { get; set; }
        /// <summary>模式 1:插管、2:呼吸器、3:氧療</summary>
        public string ON_TYPE
        {
            get { return _ON_TYPE; }
            set
            {
                try
                {
                    switch (value)
                    {
                        case "1":
                            ON_TYPEName = "插管";
                            break;
                        case "2":
                            ON_TYPEName = "呼吸器";
                            break;
                        case "3":
                            ON_TYPEName = "氧療";
                            break;
                        default:
                            break;
                    }
                }
                catch
                {

                    throw;
                }
                _ON_TYPE = value;
            }
        }
        /// <summary>模式 1:插管、2:呼吸器、3:氧療</summary>
        public string ON_TYPEName { get; set; }
        /// <summary>資料內容</summary>
        public List<JSON_DATA> DATA_CONTENT { get; set; }
        /// <summary>用於更新已上傳的資料時，紀錄新的表單ONMODE_ID</summary>
        public string NEW_ONMODE_ID { get; set; }
        public string DATASTATUS { get; set; }
        public string IPD_NO { get; set; }
        /// <summary>建立者ID</summary>
        public string CREATE_ID { get; set; }
        /// <summary>建立者</summary>
        public string CREATE_NAME { get; set; }
        /// <summary>建立時間</summary>
        public string CREATE_DATE { get; set; }
        /// <summary>呼吸器編號(財產編號)</summary>
        public string DEVICE_NO { get; set; }
        public bool _isShow {
            get
            {
                if (!string.IsNullOrWhiteSpace(this.STARTDATE))
                {
                    if (!string.IsNullOrWhiteSpace(this.ENDDATE))
                    {
                        return false;
                    }
                    return true;
                }
                return false;
            }
        }
        public List<JSON_DATA> isShow { get; set; }
    }

    /// <summary> 系統參數 </summary>
    public class RCS_SYS_PARAMS
    {
        /// <summary> 參數編號 </summary>
        public string p_id { set; get; }

        /// <summary> 所屬模組 </summary>
        public string p_model { set; get; }

        /// <summary> 參數群組 </summary>
        public string p_group { set; get; }

        /// <summary> 顯示名稱 </summary>
        public string p_name { set; get; }

        /// <summary> 實際值 </summary>
        public string p_value { set; get; }

        /// <summary> 語系 </summary>
        public string p_lang { set; get; }

        /// <summary> 排序 </summary>
        public int p_sort { set; get; }

        /// <summary> 備註 </summary>
        public string p_memo { set; get; }

        /// <summary> 狀態 </summary>
        public string p_status { set; get; }

        /// <summary> 狀態說明 </summary>
        public string p_status_desc
        {
            get
            {
                switch (this.p_status)
                {
                    case "9":
                        return "停用";
                    case "0":
                    default:
                        return "啟用";
                }
            }
        }
    }

    /// <summary>
    /// RT自訂醫師
    /// </summary>
    public class RCS_RT_CASE_DOC
    {
        /// <summary>流水號</summary>
        public string D_ID { set; get; }
        /// <summary>病患住院帳號</summary>
        public string IPD_NO { set; get; }
        /// <summary>醫生姓名</summary>
        public string VS_NAME { set; get; }
        /// <summary>醫生員工編號</summary>
        public string VS_ID { set; get; }
        /// <summary>建立者ID</summary>
        public string CREATE_ID { set; get; }
        /// <summary>建立時間</summary>
        public string CREATE_TIME { set; get; }

    }

    #region 呼吸照護記錄單

    /// <summary> 呼吸記錄單主檔 </summary>
    public class RTRECORD_MASTER
    {
        /// <summary>流水號</summary>
        public string RECORD_ID { set; get; }
        /// <summary>建立者ID</summary>
        public string CREATE_ID { set; get; }
        /// <summary>建立者</summary>
        public string CREATE_NAME { set; get; }
        /// <summary>建立時間</summary>
        public string CREATE_DATE { set; get; }
        /// <summary>修改者ID</summary>
        public string MODIFY_ID { set; get; }
        /// <summary>更新者</summary>
        public string MODIFY_NAME { set; get; }
        /// <summary>更新時間</summary>
        public string MODIFY_DATE { set; get; }
        /// <summary>住院帳號</summary>
        public string IPD_NO { set; get; }
        /// <summary>病歷號</summary>
        public string CHART_NO { set; get; }
        /// <summary>紀錄時間</summary>
        public string RECORDDATE { set; get; }
        /// <summary>插管ONMODE_ID</summary>
        public string ONMODE_TYPE1_ID { get; set; }
        /// <summary>呼吸器ONMODE_ID</summary>
        public string ONMODE_TYPE2_ID { get; set; }
        /// <summary>氧療ONMODE_ID</summary>
        public string ONMODE_TYPE3_ID { get; set; }
        /// <summary>資料狀態，1=正常、2=歷史資料、9=刪除</summary>
        public string DATASTATUS { get; set; }
        /// <summary>上傳狀態，1=已上傳成功、0=未上傳(失敗)</summary>
        public string UPLOAD_STATUS { get; set; }
        /// <summary>上傳者ID</summary>
        public string UPLOAD_ID { get; set; }
        /// <summary>上傳者姓名</summary>
        public string UPLOAD_NAME { get; set; }
        /// <summary>上傳時間 yyyy-MM-dd HH:mm:ss</summary>
        public string UPLOAD_DATE { get; set; }
        /// <summary>用於更新已上傳的資料時，紀錄新的表單RECORD_ID</summary>
        public string NEW_RECORD_ID { get; set; }

        public string COST_CODE { get; set; }
        public string BED_NO { get; set; }

        public string Content { set; get; }
        /// <summary>是否有ON插管</summary>
        public bool isIntubate { set; get; }
        /// <summary>是否有ON呼吸</summary>
        public bool isBreathing { set; get; }
        /// <summary>on呼吸日期</summary>
        public string BreathingDate { set; get; }
        /// <summary>授權流水號</summary>
        public string RtGrantListtempid { set; get; }
    }

    /// <summary> 呼吸記錄單明細 </summary>
    public class RTRECORD_DETAIL
    {
        public string RECORD_ID { set; get; }
        public string ITEM_NAME { set; get; }
        public string ITEM_VALUE { set; get; }
    }

    #endregion

   

    public class DB_RCS_SYS_LOG
    {
        public string LOG_ID { get; set; }
        public string READYSTATE { get; set; }
        public dynamic RESPONSETEXT { get; set; }
        public string STATUS { get; set; }
        public string STATUSTEXT { get; set; }
        public string LOG_DATE { get; set; }
    }

    /// <summary>
    /// 使用者權限
    /// </summary>
    public class DB_RCS_SYS_USER_POWER
    {
        public string ROLE_TYPE { get; set; }
        public string FUNCTION_LOCATION { get; set; }
        public string FUNCTION_NAME { get; set; }
        public string ACTION { get; set; }
        public string CREATE_ID { get; set; }
        public string CREATE_NAME { get; set; }
        public string CREATE_DATE { get; set; }
        public string MODIFY_ID { get; set; }
        public string MODIFY_NAME { get; set; }
        public string MODIFY_DATE { get; set; }

    }

    /// <summary>
    /// 決策資源主表
    /// </summary>
    public class DB_RCS_SYS_DECISION_SUPPORT_MASTER
    {
        public string DS_ID { get; set; }
        public string DS_DESC { get; set; }
        public string DS_SUM { get; set; }
        public string DS_IDEA { get; set; }
        public string DS_MEMO { get; set; }
        public string DS_STATUS { get; set; }
        public string CREATE_ID { get; set; }
        public string CREATE_NAME { get; set; }
        public string CREATE_DATE { get; set; }
        public string MODIFY_ID { get; set; }
        public string MODIFY_NAME { get; set; }
        public string MODIFY_DATE { get; set; }

    }

    /// <summary>
    /// 決策資源主表
    /// </summary>
    public class DB_RCS_SYS_DECISION_SUPPORT_DETAIL
    {
        public string DS_ID { get; set; }
        public string DS_ITEM { get; set; }
        public string DS_VALUE { get; set; }
        public string DS_SCORE { get; set; }
    }

    public class RCS_VIP_ALARM_DESC
    {
        /// <summary>
        /// 機型
        /// </summary>
        public string DEVICE_TYPE { get; set; }
        /// <summary>
        /// alarm代碼
        /// </summary>
        public string ALARM_CODE { get; set; }
        /// <summary>
        /// alarm內容
        /// </summary>
        public string ALARM_DESC { get; set; }
        /// <summary>
        /// 順序
        /// </summary>
        public int ORDER_BY { get; set; }
        /// <summary>
        /// alarm訊息
        /// </summary>
        public string ALARM_MSG { get; set; }
        /// <summary>
        /// 內容機型_alarm代碼
        /// </summary>
        public string ALARM_CONT { get; set; }
    }

    public class DB_RCS_SYS_FUNCTION_LIST
    {
        public string FUN_ID { get; set; }
        public string FUN_GROUP { get; set; }
        public string FUN_NAME { get; set; }
        public string FUN_DESC { get; set; }
        public string FUN_DEDAIL { get; set; }
        public string FUN_MEMO { get; set; }
        public string FUN_IMG { get; set; }
        /// <summary>
        /// 動態圖片
        /// </summary>
        public string FUN_DINAMIC_IMG { get; set; }
        public string FUN_COST { get; set; }
    }

    /// <summary>
    /// 系統使用者清單
    /// </summary>
    public class DB_RCS_SYS_USER_LIST
    {
        /// <summary>
        /// 系統代碼(客戶代碼)
        /// </summary>
        public string SYS_ID { get; set; }
        /// <summary>
        /// 醫院名稱
        /// </summary>
        public string HOSP_NAME { get; set; }
        /// <summary>
        /// 使用者帳號
        /// </summary>
        public string USER_ID { get; set; }
        /// <summary>
        /// 使用者密碼(SHA512加密)
        /// </summary>
        public string USER_PWD { get; set; }
        /// <summary>
        /// 權限使用開始日期
        /// </summary>
        public string START_DATE { get; set; }
        /// <summary>
        /// 權限使用結束日期
        /// </summary>
        public string END_DATE { get; set; }
        /// <summary>
        /// 權限代碼
        /// <para>管理者:admin</para>
        /// <para>呼吸治療小組長:RT_admin</para>
        /// <para>呼吸治療師:RT</para>
        /// <para>主治醫生:doctor</para>
        /// <para>病歷課保險組人員:inquirer</para>
        /// </summary>
        public string USER_ROLE { get; set; }
        /// <summary>
        /// 資料狀態
        /// <para>永久帳號，不用權限日期:0</para>
        /// <para>使用中:1</para>
        /// <para>權限延長一個月:2</para>
        /// <para>停用或立即停用:9</para>
        /// </summary>
        public string DATASTATUS { get; set; }
        /// <summary>
        /// 建立者代碼
        /// </summary>
        public string CREATE_ID { get; set; }
        /// <summary>
        /// 建立人名稱
        /// </summary>
        public string CREATE_NAME { get; set; }
        /// <summary>
        /// 建立時間
        /// </summary>
        public string CREATE_DATE { get; set; }
        /// <summary>
        /// 修改人代碼
        /// </summary>
        public string MODIFY_ID { get; set; }
        /// <summary>
        /// 修改人名稱
        /// </summary>
        public string MODIFY_NAME { get; set; }
        /// <summary>
        /// 修改時間
        /// </summary>
        public string MODIFY_DATE { get; set; }

    }
}

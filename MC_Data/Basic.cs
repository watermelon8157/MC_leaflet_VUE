using mayaminer.com.library;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;

/// <summary>
/// 多專案會用到的共同類別
/// API CLASS WebService 等專案
/// </summary>
namespace RCS_Data
{
    /// <summary>
    /// 使用者基本資料(最上層)
    /// </summary>
    public abstract class UserInfoBasic
    {
        /// <summary> 員工編號 </summary>
        public string user_id { set; get; }

        /// <summary> 密碼 </summary>
        public string user_pwd { set; get; }

        /// <summary> 姓名 </summary>
        public virtual string user_name { set; get; }

        /// <summary> CardID </summary>
        public string card_id { set; get; }

        /// <summary>權限身份</summary>
        public string authority { set; get; }

        /// <summary>權限身份</summary>
        public string sysAuthority { set; get; }

        /// <summary>
        /// 員工身份證
        /// </summary>
        public string user_idno { set; get; }

        /// <summary>
        /// 員工所屬成本中心
        /// </summary>
        public string user_costcode { set; get; }

        /// <summary>
        /// 醫院代碼
        /// </summary>
        public virtual string hosp_id { get; }


    }

    #region 病人基本資料

    /// <summary>
    /// 病患基本資料(最上層)
    /// </summary>
    public abstract class PatientBasic
    {
        /// <summary> 病人姓名 </summary>
        protected string _patient_name { set; get; }
        /// <summary> 病人姓名 </summary>
        public virtual string patient_name { set; get; }

        /// <summary> 身份證字號 </summary>
        protected string _idno { set; get; }
        /// <summary> 身份證字號 </summary>
        public virtual string idno { set; get; }

        /// <summary> 病歷號 </summary>
        public string chart_no { set; get; }

        /// <summary> 住院帳號 </summary>
        public string ipd_no { set; get; }

        /// <summary> 生日 </summary>
        public string birth_day { set; get; }
        /// <summary>
        /// 民國年生日 yyy-MM-dd
        /// </summary>
        public string birth_day_CHT
        {
            get
            {
                if (DateHelper.isDate(this.birth_day))
                {
                    return DateHelper.toTWDate(this.birth_day, "yyy-MM-dd");
                }
                else
                {
                    return "yyy-MM-dd";
                }
            }
        }
        /// <summary> 年齡 </summary>
        public int age
        {
            get
            {
                DateTime dt = DateTime.Now;
                bool db_psuccess = DateTime.TryParse(this.birth_day, out dt);
                if (db_psuccess)
                {
                    int d = 0;
                    return int.TryParse(((DateTime.Now - dt).Days / 365).ToString(), out d) ? d : 0;
                }
                else
                    return -1;
            }
        }

        private string _sex;
        /// <summary> 性別 </summary>
        public string gender
        {
            set { _sex = value; }
            get
            {
                switch (_sex)
                {
                    case "男":
                    case "M":
                    case "1":
                        return "1";
                    case "女":
                    case "0":
                    case "F":
                        return "0";
                    default:
                        return "2";
                }
            }
        }
        public string genderEN
        {
            set { _sex = value; }
            get
            {
                switch (_sex)
                {
                    case "男":
                    case "M":
                    case "1":
                        return "M";
                    case "女":
                    case "0":
                    case "F":
                        return "F";
                    default:
                        return "";
                }
            }
        }
        /// <summary> 性別(男:女) </summary>
        public string genderCHT
        {
            get
            {
                switch (gender)
                {
                    case "男":
                    case "M":
                    case "1":
                        return "男";
                    case "女":
                    case "0":
                    case "F":
                        return "女";
                    default:
                        return "未知";
                }
            }
        }


        /// <summary> 診別來源(E:急診、O:門診、I:住院) </summary>
        public string source_type { set; get; }
        /// <summary> 診別 </summary>
        public string showSource
        {
            get
            {
                switch (this.source_type)
                {
                    case "E":
                        return "急";
                    case "O":
                        return "門";
                    case "I":
                    default:
                        return "住";
                }
                return "住";
            }
        }
        private string _nam_c;
        /// <summary> 診別中文 </summary>
        public string nam_c
        {
            set { _nam_c = value; }
            get
            {
                switch (this.source_type)
                {
                    case "E":
                        _nam_c = "急診";
                        break;
                    case "O":
                        _nam_c = "門診";
                        break;
                    case "I":
                    default:
                        _nam_c = "住院";
                        break;
                }
                return _nam_c;
            }
        }
    }

    public abstract class PatientInfo : PatientBasic
    {
        public PatientInfo()
        {
            loc = "";
            room_no = "";
            bed_no = "";
            body_height = "";
        }
        public string MDRO_MARK { set; get; }
        /// <summary>收案編號</summary>
        public string case_id { set; get; }

        /// <summary>流水號</summary>
        public string r_id { set; get; }
        /// <summary>
        /// DNR註記(1:是，0否，預設為:否)
        /// </summary>
        public string dnr_mark { get; set; }

        /// <summary> 院區代碼 </summary>
        public string loc { set; get; }

        /// <summary> 房號 </summary>
        public string room_no { set; get; }

        /// <summary> 床號 </summary>
        public string bed_no { set; get; }

        /// <summary> 科別代碼 </summary>
        public string dept_code { set; get; }

        /// <summary> 科別名稱 </summary>
        public string dept_desc { set; get; }

        /// <summary> 護理站代碼 </summary>
        public string cost_code { set; get; }

        /// <summary> 護理站名稱 </summary>
        public string cost_desc { set; get; }

        /// <summary> 主治醫師姓名 </summary>
        protected string _vs_doc { set; get; }

        /// <summary> 主治醫師姓名 </summary>
        public virtual string vs_doc { set; get; }

        /// <summary> 主治醫師代碼 </summary>
        public string vs_id { set; get; }

        /// <summary> 入院診斷清單 </summary>
        public List<Diag> diag_list = new List<Diag>();

        /// <summary> 入院日期 </summary>
        public string diag_date { set; get; }

        /// <summary> 預計出院日期 </summary>
        public string pre_discharge_date { set; get; }

        /// <summary> 收案狀態 </summary>
        public string accept_status { set; get; }

        /// <summary>資料類型(C=照護病患清單，H=歷史病患清單)</summary>
        public string type_mode { set; get; }
        /// <summary>是否是病患記錄資料(true:是病患記錄資料，false:照護清單資料，預設為false)</summary>
        public bool isHistoryData { set; get; }

        /// <summary> 使用天數 </summary>
        public int use_days { set; get; }

        /// <summary> 體重 </summary>
        public string body_weight { set; get; }
        /// <summary> 身高 </summary>
        public string body_height { set; get; }

        /// <summary>藥物備註</summary>
        public string drug_memo { set; get; }

        /// <summary>自訂醫師姓名</summary>
        public string case_vs_name { set; get; }

        /// <summary>自訂醫師ID</summary>
        public string case_vs_id { set; get; }

        /*以下為合併欄位*/
        /// <summary> 房號加床號 </summary>
        public string merger_room_bed { set; get; }
        public void setOtherValue()
        {
            string bed_no_str = "";
            if (!string.IsNullOrWhiteSpace(bed_no))
            {
                bed_no_str = "-" + bed_no;
            }
            this.merger_room_bed = loc + room_no + bed_no_str;
        }

        /// <summary> 最後一筆診斷代碼 </summary>
        public string diagnosis_code { set; get; }
        /// <summary> 最後一筆診斷中文 </summary>
        public string diagnosis_cname { set; get; }
        /// <summary> 最後一筆診斷英文 </summary>
        public string diagnosis_ename { set; get; }
        /// <summary>
        /// 門診急診次數
        /// </summary>
        public string source_count { set; get; }
        /// <summary>
        /// 有已暫存交班單
        /// </summary>
        public bool hadShift_record { get; set; }
        /// <summary>
        /// 有警告呼吸器訊息
        /// </summary>
        public bool hadAlarm_msg { get; set; }
        /// <summary>
        /// 警告呼吸器訊息
        /// </summary>
        public string alarm_msg { get; set; }

        protected string _vs_dat;
        /// <summary> 急門診日 </summary>
        public virtual string vs_dat { get; set; }

        /// <summary>
        /// 體表面積(入院)
        /// </summary>
        public string BSA { get; set; }

        /// <summary>
        /// 血型
        /// </summary>
        public string BLOOD_TYPE { get; set; }

        /// <summary>
        /// 入ICU日期時間
        /// </summary>
        public string ICU_DATE { get; set; }

        /// <summary>
        /// 入ICU體重
        /// </summary>
        public string ICU_BODY_WEIGHT { get; set; }
        /// <summary>
        /// 入ICU_主診斷代碼
        /// </summary>
        public string ICU_DIAGNOSIS_CODE { get; set; }
        /// <summary>
        /// 入ICU_主診斷敘述
        /// </summary>
        public string ICU_DIAGNOSIS_DESC { get; set; }

        /// <summary>
        /// 臨床診斷
        /// </summary>
        public string CLINICAL_DIAGNOSIS { get; set; }

        /// <summary>
        /// 入院經過
        /// </summary>
        public string ADMISSION_DESC { get; set; }

        /// <summary>ㄑ
        /// 是否抽菸 1:是,0:否
        /// </summary>
        public string SMOKES { get; set; }

        /// <summary>
        /// 每日幾包
        /// </summary>
        public string SMOKES_PACK { get; set; }

        /// <summary>
        /// 已抽菸幾年
        /// </summary>
        public string SMOKES_YEAR { get; set; }

        /// <summary>
        /// 已戒菸  1:已戒菸
        /// </summary>
        public string QUIT_SMOKING { get; set; }
        /// <summary>
        /// 新的切帳序號
        /// </summary>
        public string new_ipd_no { set; get; }
    }

     

    /// <summary> 診斷碼清單 </summary>
    public class Diag
    {
        /// <summary> 診斷代碼 </summary>
        public string diag_code { set; get; }
        /// <summary> 中文診斷描述 </summary>
        public string diag_desc_c { set; get; }
        /// <summary> 英文診斷描述 </summary>
        public string diag_desc_e { set; get; }
        /// <summary> 排序 </summary>
        public int sort { set; get; }
    }
    #endregion

     

    /// <summary> 訊息狀態 </summary>
    public enum HISDataStatus
    {
        /// <summary> 代碼:0	取得資料成功，但無資料。 </summary>
        SuccessWithoutData = 0,
        /// <summary> 代碼:1	取得資料成功，有資料。 </summary>
        SuccessWithData = 1,
        /// <summary> 代碼:2	程式本身錯誤。 </summary>
        ExceptionError = 2,
        /// <summary> 代碼:3	傳入參數有誤(未傳入或格式錯誤)。 </summary>
        ParametersError = 3,
        /// <summary> 代碼:9	取得資料失敗。 </summary>
        SQLStatementError = 9,
        /// <summary> 代碼:8	取得webService資料失敗。 </summary>
        WebServiceError = 8

    }

    /// <summary>HISDataExchange回傳資料</summary>
    /// <typeparam name="T">指定回傳資料class</typeparam>
    public class HISData<T>
    {
        /// <summary> 資料Datatable </summary>
        public System.Data.DataTable retrunDT { get; set; }
        /// <summary> 資料JSON </summary>
        public string returnJSON { get; set; }
        /// <summary> 資料List<T> </summary>
        public List<T> dataList { get; set; }
        /// <summary> 資料取得狀態 </summary>
        public HISDataStatus datastatus { get; set; }
        /// <summary> 錯誤訊息文字 </summary>
        public string errorMsg = "";
        /// <summary>設定忽略</summary>
        public void setIgnore()
        {
            errorMsg = "";
            returnJSON = "[]";
            dataList = new List<T>();
            retrunDT = new System.Data.DataTable();
            datastatus = HISDataStatus.SuccessWithData;
        }

    }


    /// <summary>
    /// 建立Extension Methods透過Reflection來簡化 (DataTable to List<T>)
    /// 參考網址https://dotblogs.com.tw/rainmaker/2013/11/05/126727
    /// </summary>
    public static class DataTableExtensions
    {
        /// <summary>
        /// 將 IEnumerable 轉換至 DataTable
        /// </summary>
        /// <param name="list">IEnumerable</param>
        /// <returns>DataTable</returns>
        public static DataTable ToDataTable(this IEnumerable list)
        {
            DataTable dt = new DataTable();
            bool schemaIsBuild = false;
            PropertyInfo[] props = null;

            foreach (object item in list)
            {
                if (!schemaIsBuild)
                {
                    props = item.GetType().GetProperties();
                    foreach (var pi in props)
                    {
                        dt.Columns.Add(new DataColumn(pi.Name, pi.PropertyType));
                    }

                    schemaIsBuild = true;
                }

                var row = dt.NewRow();
                foreach (var pi in props)
                {
                    row[pi.Name] = pi.GetValue(item, null);
                }

                dt.Rows.Add(row);
            }

            dt.AcceptChanges();

            return dt;
        }

        public static List<T> ToList<T>(this DataTable table) where T : new()
        {
            List<System.Reflection.PropertyInfo> properties = typeof(T).GetProperties().ToList();
            List<T> result = new List<T>();

            foreach (var row in table.Rows)
            {
                var item = CreateItemFromRow<T>((DataRow)row, properties);
                result.Add(item);
            }

            return result;
        }

        public static List<T> ToList<T>(this DataTable table, Dictionary<string, string> mappings) where T : new()
        {
            List<System.Reflection.PropertyInfo> properties = typeof(T).GetProperties().ToList();
            List<T> result = new List<T>();

            foreach (var row in table.Rows)
            {
                var item = CreateItemFromRow<T>((DataRow)row, properties, mappings);
                result.Add(item);
            }

            return result;
        }

        private static T CreateItemFromRow<T>(DataRow row, IList<PropertyInfo> properties) where T : new()
        {
            T item = new T();
            foreach (var property in properties)
            {
                if (row.Table.Columns.Contains(property.Name))
                {
                    System.Type s = property.PropertyType;

                    string name = s.Name;

                    switch (name)
                    {
                        case "String":
                            if (row[property.Name] != DBNull.Value)
                                property.SetValue(item, row[property.Name].ToString(), null);
                            break;
                        case "Boolean":
                            if (row[property.Name] != DBNull.Value)
                                if (IsNumeric(row[property.Name].ToString()))
                                    property.SetValue(item, Convert.ToBoolean(Convert.ToInt16(row[property.Name].ToString())), null);
                                else
                                    property.SetValue(item, Convert.ToBoolean(row[property.Name].ToString()), null);
                            break;
                        case "Int32":
                            if (row[property.Name] != DBNull.Value)
                                property.SetValue(item, Convert.ToInt32(row[property.Name].ToString()), null);

                            break;
                        case "Double":
                            if (row[property.Name] != DBNull.Value)
                                property.SetValue(item, Convert.ToDouble(row[property.Name].ToString()), null);
                            break;
                        default:
                            //pi.SetValue(pModelObj, pDr[pi.Name].ToString(), null);
                            break;
                    }
                }
            }
            return item;
        }

        private static T CreateItemFromRow<T>(System.Data.DataRow row, IList<System.Reflection.PropertyInfo> properties, Dictionary<string, string> mappings) where T : new()
        {
            T item = new T();
            foreach (var property in properties)
            {
                if (mappings.ContainsKey(property.Name) && row.Table.Columns.Contains(mappings[property.Name]))
                {
                    System.Type s = property.PropertyType;

                    string name = s.Name;

                    switch (name)
                    {
                        case "String":
                            if (row[property.Name] != DBNull.Value)
                                property.SetValue(item, row[property.Name].ToString(), null);
                            break;
                        case "Boolean":
                            if (row[property.Name] != DBNull.Value)
                                if (IsNumeric(row[property.Name].ToString()))
                                    property.SetValue(item, Convert.ToBoolean(Convert.ToInt16(row[property.Name].ToString())), null);
                                else
                                    property.SetValue(item, Convert.ToBoolean(row[property.Name].ToString()), null);
                            break;
                        case "Int32":
                            if (row[property.Name] != DBNull.Value)
                                property.SetValue(item, Convert.ToInt32(row[property.Name].ToString()), null);

                            break;
                        case "Double":
                            if (row[property.Name] != DBNull.Value)
                                property.SetValue(item, Convert.ToDouble(row[property.Name].ToString()), null);
                            break;
                        default:
                            //pi.SetValue(pModelObj, pDr[pi.Name].ToString(), null);
                            break;
                    }
                }

            }
            return item;
        }

        public static bool IsNumeric(string anyString)
        {
            if (anyString == null)
            {
                anyString = "";
            }
            if (anyString.Length > 0)
            {
                double dummyOut = new double();
                System.Globalization.CultureInfo cultureInfo =
                    new System.Globalization.CultureInfo("en-US", true);

                return Double.TryParse(anyString, System.Globalization.NumberStyles.Any,
                    cultureInfo.NumberFormat, out dummyOut);
            }
            else
            {
                return false;
            }
        }
    }
}

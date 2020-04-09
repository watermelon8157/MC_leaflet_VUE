using System;
using System.IO;
using System.Text;
using System.Web;
using System.Runtime.InteropServices;
using System.Collections.Generic;

namespace Com.Mayaminer
{

    public static class IniFile
    {
        /// <summary>
        /// DES encode A key
        /// </summary>
        public readonly static string akey = "aaaaaaaa";
        
        /// <summary>
        /// DES encode B key
        /// </summary>
        public readonly static string bkey = "bbbbbbbb";
        
        /// <summary>
        /// ini file path
        /// </summary>
        public readonly static string ini_path = AppDomain.CurrentDomain.BaseDirectory + "App_Config\\RCSConfig.ini";

        /// <summary>
        /// 取得連線字串
        /// </summary>
        /// <returns>連線字串</returns>
        public static string GetConnStr(string ConnectionSession)
        {
            string connstr = GetConfig(ConnectionSession, "DBConnStr").Trim();
            string password = SecurityTool.DecodeDES(GetConfig(ConnectionSession, "Password"), akey, bkey).Trim();
            return string.Format(connstr, password);
        }

        /// <summary>
        /// 取得設定檔的值
        /// </summary>
        /// <param name="session"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public static string GetConfig(string session, string key)
        {
            StreamReader sr = new StreamReader(ini_path, Encoding.Default);
            string line = "", head = "", result = "";
            //String[] value;
            char[] spChr = { '=' };

            while ((line = sr.ReadLine()) != null)
            {
                if (line.IndexOf("[") != -1 && line.IndexOf("]") != -1)
                {
                    head = line.Replace("[", "").Replace("]", "");
                    continue;
                }
                else
                {
                    if (head == session)
                    {
                        if (line.StartsWith(key))
                        {
                            result = line.Substring(line.IndexOf("=") + 1);
                            break;
                        }
                    }
                    else
                    {
                        continue;
                    }
                }
            }
            sr.Close();
            line = null;
            head = null;
            spChr = null;
            sr = null;
            return result;
        }

    }

    /// <summary> 發生錯誤log位置(cs檔分類) </summary>
    public enum GetLogToolCS
    {
        /// <summary> 暫存取得資料 <para>可以用來看架構、取得資料的</para></summary>
        getData,
        /// <summary> 基本資料 </summary>
        BaseModel,
        /// <summary> 呼吸照護清單 </summary>
        RT,
        /// <summary> 呼吸患者評估表 </summary>
        RTAssess,
        /// <summary> 趨勢圖 </summary>
        RTChart,
        /// <summary> 呼吸照護記錄單 </summary>
        RTRecord,
        /// <summary> 呼吸患者脫離評估 </summary>
        RTTakeoffAssess,
        /// <summary> 系統管理 </summary>
        SystemManage,
        /// <summary> 院內資料交換類別 </summary>
        HISDataExchange,
        /// <summary> BaseController </summary>
        BaseController,
        /// <summary> 呼吸照護記錄單Controller </summary>
        RTRecordController,
        /// <summary> 照護清單Controller </summary>
        RTController,
        /// <summary> 系統管理Controller </summary>
        SystemManageController,
        /// <summary> 交班表Controller </summary>
        ShiftController,
        /// <summary> webservice </summary>
        WebMethod,
        ///  <summary> APIConnector </summary>
        APIConnector,
        /// <summary>Exam</summary>
        Exam,
        /// <summary>產生XML</summary>
        xmlModel,
        /// <summary>
        /// CPTController
        /// </summary>
        CPTController,
        /// <summary>
        /// RTTakeoffAssessController
        /// </summary>
        RTTakeoffAssessController,
        /// <summary>
        /// LogCheck
        /// </summary>
        LogCheck,
        /// <summary>
        /// RTAssessController
        /// </summary>
        RTAssessController,
        /// <summary>
        /// 對外入口
        /// </summary>
        ExternalController,
        /// <summary>
        /// 對VIP API
        /// </summary>
        VIPController,
        /// <summary>
        /// 測試資料class
        /// </summary>
        TestData,
        /// <summary>
        /// 系統使用物件
        /// </summary>
        System,
        /// <summary>
        /// 權限設定Controller
        /// </summary>
        AuthorityController,
        /// <summary>
        /// 權限設定ViewModel
        /// </summary>
        AuthorityViewModel,
        /// <summary>
        /// 決策資源
        /// </summary>
        DecisionSupportController,
        /// <summary>
        /// 系統功能設定
        /// </summary>
        SystemFunctionController,
        /// <summary>
        /// 功能工廠
        /// </summary>
        FunctionFactory,
        /// <summary>
        /// SQLProvider
        /// </summary>
        SQLProvider,
        VPN_UPLOADController,
        /// <summary>
        /// 匯出檔案模組
        /// </summary>
        exportFile
    }
}
using Com.Mayaminer;
using mayaminer.com.jxDB;
using mayaminer.com.library;
using Newtonsoft.Json;
using RCS_Data;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Web;

namespace RCS.Models
{
    public class CxrCanvasLineDraw : BaseModel
    {
        public List<string> getCxrResultStr_DropdownList( //CXR下拉清單
            string inputSqlDetailTableName = "" //[RCS_RECORD_DETAIL] [RCS_CPT_ASS_DETAIL]
        ){
            List<string> CxrResult_strList = new List<string>();
            List<SysParams> CxrResult_SList = new List<SysParams>();
            try
            {
                // CXR下拉清單 [RCS_SYS_PARAMS] 系統資料庫
                // (string pModel = "", string pGroup = "", string pLang = "zh-tw", string pStatus = "", string pManage = "")
                // <param name="pModel">參數類別</param>
                // <param name="pGroup">參數群組</param>
                // <param name="pLang">語系(預設:zh-tw)</param>
                // <param name="pStatus">資料狀態：1=正常，9=刪除/停用</param>
                // <param name="pManage">資料狀態：1=管理者才顯示，0=非管理者顯示</param>
                // [P_MODEL] = 'Shared' + [P_GROUP] = 'cxr_result'
                CxrResult_SList = getRCS_SYS_PARAMS(inputSqlDetailTableName, "cxr_result"); //取得系統設定參數
                foreach (SysParams CxrResult_SNode in CxrResult_SList)
                {
                    CxrResult_strList.Add(CxrResult_SNode.P_VALUE);
                }
                CxrResult_strList.Sort(); //排序功能
            }
            catch (Exception ex)
            {
                LogTool.SaveLogMessage(ex, "getCxrResultStr_DropdownList");
            }
            return CxrResult_strList;
        }//CXR下拉清單
        /*
        ==================================================================
        [SqlTableName] + [SqlMasterDetail_ID] + [CXR_result_json] → [List].[CxrResultJson_cls].[List].[CXR_XYwmc].[NULL]
                                                                 (3)   ↓                         ↓
                                                                       ↓(2)                  (1)[Cxr_CJID]
                                                                       ↓                         ↓
        [SqlTableName] + [SqlMasterDetail_ID] + [CXR_result_json] → [List].[CxrResultJson_cls].[List].[CXR_XYwmc].[18]
        ==================================================================
        1.[Cxr_CJID] Cxr流水號 => [List].[CXR_XYwmc].[18] 座標點
        2.[List].[CxrResultJson_cls] Cxr版面(無) => [List].[CxrResultJson_cls] Cxr版面(有)
        3.[SqlTableName] 主流水號 + [SqlMasterDetail_ID] 主流水號 + [CXR_result_json] 欄位 => [List].[CxrResultJson_cls] Cxr版面(無)
        4.[CHART_NO] 病歷號 + [IPD_NO] 住院序號 +  [RCS_MASTER] 資料庫 + [RCS_DETAIL] 資料庫 => [List].[SqlMasterDetail_ID] 主流水號
        ==================================================================
        [List].[CxrResultJson_cls] Cxr版面
        {
            "SqlTableName": [RCS_CPT_ASS_DETAIL] [RCS_RECORD_DETAIL], 主資料庫
            "SqlMasterDetail_ID": [RECORD_ID] [CPT_ID], 主流水號
            "Cxr_CJID": "[Cxr_CJID]", Cxr流水號
            "image_file_key": "",
            "Result_Date":" 2018-08-14",
            "Result_Str": "pleural effusion",
            "CxrXuwmc_List": [List<CXR_XYwmc>] + [null] → [List<CXR_XYwmc>] + [18] 座標點
        }
        ==================================================================
        */
        /*-------------------------Read讀取-SQL底層 [下]-------------------------*/

            //1.[Cxr_CJID] → [List].[CXR_XYwmc].[18]
        public List<CXR_XYwmc> getCxrXywmcList_byCJID(string inputCJID) //SQL[Read讀取]
        {
            List<CXR_XYwmc> CxrXYwmc_List = new List<CXR_XYwmc>();
            try
            {
                if (!string.IsNullOrWhiteSpace(inputCJID)) //傳入paraCJID不為空值
                {
                    string sqlcmd_Cxr = @"
                        SELECT *
                        FROM RCS_CXR_JSON
                        WHERE CJ_ID = '{0}'
                        ";
                    DataTable Cxr_DTable = this.DBA.getSqlDataTable(string.Format(
                        sqlcmd_Cxr
                        , inputCJID.Trim()
                        ));
                    if ( //SQL執行，有找到資料
                        Cxr_DTable != null
                        && Cxr_DTable.Rows.Count > 0
                        && Cxr_DTable.Rows[0]["CJ_VALUE"].ToString().Trim() != ""
                    ){
                        foreach (DataRow CxrRows in Cxr_DTable.Rows)
                        {
                            CxrXYwmc_List.Add(
                                JsonConvert.DeserializeObject<CXR_XYwmc>(
                                    CxrRows["CJ_VALUE"].ToString().Trim()
                                    ));
                        }//foreach
                    }//SQL執行，有找到資料
                }//傳入paraCJID不為空值
            }//try
            catch (Exception ex)
            {
                LogTool.SaveLogMessage(ex, "getCxrXywmcList_byCJID");
            }//catch
            return CxrXYwmc_List;
        }//SQL[Read讀取]

        //2.[List].[CxrResultJson_cls] → [List].[CxrResultJson_cls]
        public List<CxrResultJson_cls> addCxrResultList_byCxrResult(List<CxrResultJson_cls> inputCxrResultJsonList)
        {
            try //SQL[Read讀取]
            {
                // 1.需先確認 DeserializeObject 劃線CXR格式轉換成功 [CxrResultJson_SList[0].Cxr_CJID != null]
                if (
                    inputCxrResultJsonList != null 
                    && inputCxrResultJsonList.Count > 0 
                    && inputCxrResultJsonList[0].Cxr_CJID != null
                    && inputCxrResultJsonList[0].Cxr_CJID.ToString().Trim() != ""
                ){
                    // 劃線CXR格式轉換成功 List<CxrResultJson_cls>
                    // 2..編輯 DataTable ["CXR_result_json"] 欄位內容值
                    foreach (CxrResultJson_cls CxrResultJson_Node in inputCxrResultJsonList)
                    {
                        if (CxrResultJson_Node != null 
                            && !string.IsNullOrWhiteSpace(CxrResultJson_Node.Cxr_CJID)
                        ){
                            // 3.修改 "Cxr_CJID":"CXR_table_20170613134453mayaI0333017_00" 搜尋 [RCS_CXR_JSON] 資料庫
                            // "CxrXuwmc_List": null → "CxrXuwmc_List": [18]
                            CxrResultJson_Node.CxrXuwmc_List = getCxrXywmcList_byCJID(CxrResultJson_Node.Cxr_CJID);
                            // 4.CXR下拉清單
                            CxrResultJson_Node.ResultStr_Dropdownlist = getCxrResultStr_DropdownList();
                        }
                    }//foreach
                }
                else
                {
                    /* 不符合 List<CxrResultJson_cls> 劃線CXR格式，(1)不做任何事 or (2)清空欄位，減輕網路傳輸大小 */
                    inputCxrResultJsonList = null;
                }
            }//try
            catch (Exception ex)
            {
                LogTool.SaveLogMessage(ex, "addCxrResultList_byCxrResult");
            }//catch
            return inputCxrResultJsonList;
        }//getCxrResultJson_CxrResultJson

        //3.[SqlTableName] + [SqlMasterDetail_ID] + [CXR_result_json] → [List].[CxrResultJson_cls]
        public List<CxrResultJson_cls> getCxrResultList_byIdDetail(
            string inputA_SqlMasterDetail_ID //["CPT_ID"] ["RECORD_ID"] 主檔編號 != Cxr編號
            , string inputB_SqlDetail_TableName //[RCS_CPT_ASS_DETAIL] [RCS_RECORD_DETAIL]
        ){
            List<CxrResultJson_cls> CxrResultJson_List = new List<CxrResultJson_cls>();
            try //SQL[Read讀取]
            {
                if (!string.IsNullOrWhiteSpace(inputA_SqlMasterDetail_ID) 
                    && !string.IsNullOrWhiteSpace(inputB_SqlDetail_TableName)
                ){
                    /*------------------獲得 [SQL] 欄位名稱 [下]------------------*/
                    DataTable Detail_ColumnName = this.DBA.getSqlDataTable(string.Format(
                        @"SELECT TOP 0 * FROM {0}"
                        , inputB_SqlDetail_TableName //[RCS_CPT_ASS_DETAIL]資料庫
                        ));
                    string Detail_ID = Detail_ColumnName.Columns[0].ColumnName; //["CPT_ID"] ["RECORD_ID"] 主檔編號
                    string Detail_Item = Detail_ColumnName.Columns[1].ColumnName; //[CPT_ITEM] [ITEM_NAME]
                    string Detail_Value = Detail_ColumnName.Columns[2].ColumnName; //[CPT_VALUE] [ITEM_VALUE]
                    /*------------------[上] 獲得 [SQL] 欄位名稱------------------*/

                    //3.根據 Master_Rows[Master_ID] 流水號_數值 20170718144526maya，搜尋 [RCS_DETAIL] 細項資料庫
                    string sqlcmd_Detail = @"
                        SELECT *
                            FROM {0}
                        WHERE {1} = '{2}'
                            AND {3} = '{4}'
                        ";

                    // [RCS_CPT_ASS_DETAIL] [RCS_RECORD_DETAIL]
                    // string MasterID_Number = Master_Rows[Master_ID].ToString().Trim(); //流水號_數值
                    DataTable Detail_Table = this.DBA.getSqlDataTable(string.Format(
                        sqlcmd_Detail
                        , inputB_SqlDetail_TableName // 0.[RCS_CPT_ASS_DETAIL] 資料庫
                        , Detail_ID // 1.[CPT_ID] [RECORD_ID] 欄位名稱
                        , inputA_SqlMasterDetail_ID // 2.[Detail_ID] 流水號 20170718144526maya "2018082823074141496"
                        , Detail_Item // 3.[CPT_ITEM] [ITEM_NAME] 欄位名稱
                        , "CXR_result_json" // 4.自定 2018.08.20
                        ));

                    //4.若 [RCS_DETAIL] 細項資料庫，有找到資料
                    if (Detail_Table != null
                        && Detail_Table.Rows.Count > 0
                        && Detail_Table.Rows[0][Detail_Value].ToString().Trim() != ""
                    ){
                        foreach (DataRow Detail_Rows in Detail_Table.Rows)
                        {
                            string DetailValue_str = Detail_Rows[Detail_Value].ToString().Trim();
                            if (!DetailValue_str.StartsWith("[") && !DetailValue_str.EndsWith("]"))
                            {
                                DetailValue_str = "[" + DetailValue_str + "]";
                            }

                            // 5.讀取 [CXR_result_json] 欄位內容值
                            List<CxrResultJson_cls> CxrResultJson_SList =
                                JsonConvert.DeserializeObject<List<CxrResultJson_cls>>(
                                    DetailValue_str
                                    //Detail_Rows[Detail_Value].ToString().Trim()
                                    );

                            if (CxrResultJson_SList != null
                                && CxrResultJson_SList.Count > 0
                                && CxrResultJson_SList[0].Cxr_CJID != null
                                && CxrResultJson_SList[0].Cxr_CJID.ToString().Trim() != ""
                            ){
                                // 6.加入 [Master_ID] 流水號 20170718144526maya
                                foreach (CxrResultJson_cls CxrResultJson_SNode in CxrResultJson_SList)
                                {
                                    CxrResultJson_SNode.SqlTableName = inputB_SqlDetail_TableName;
                                    CxrResultJson_SNode.SqlMasterDetail_ID = Detail_Rows[Detail_ID].ToString().Trim();
                                    if (Detail_Rows[Detail_ID].ToString() != inputA_SqlMasterDetail_ID)
                                    {
                                        LogTool.SaveLogMessage(
                                            inputB_SqlDetail_TableName+"欄位ID搜尋錯誤!"
                                            , "getCxrResultList_byPatient"
                                            );
                                    }
                                }

                                // 7.自定函式：加入 "CxrXuwmc_List": null → "CxrXuwmc_List": [18]
                                // 根據 [CHART_NO 病歷號] + [IPD_NO 住院序號] 搜尋 [CXR_result_json] CXR畫線資料庫
                                CxrResultJson_List.AddRange( //輸出值
                                    addCxrResultList_byCxrResult( //自定函式
                                        CxrResultJson_SList //輸入值
                                        ));
                            }//if
                        }//foreach-Detail
                    }//Detail 有資料
                }//if - !string.IsNullOrWhiteSpace
            }//try
            catch (Exception ex)
            {
                LogTool.SaveLogMessage(ex, "getCxrResultList_byIdDetail");
            }//catch
            return CxrResultJson_List;
        }//getCxrResultList_byPatient

        /*-------------------------[上] Read讀取-SQL底層 + Read讀取-應用層 [下]-------------------------*/

        // [CHART_NO 病歷號] + [IPD_NO 住院序號] → [SqlMasterDetail_ID 流水號]
        public List<CxrResultJson_cls> getCxrResultList_byPatient(
            string inputChartNo //[CHART_NO] 病歷號 = 04691561
            , string inputIpdNo //[IPD_NO] 住院序號 = I0333017
            , string inputSqlNameMaster //[RCS_CPT_ASS_MASTER] [RCS_RECORD_MASTER]資料庫
            , string inputSqlNameDetail //[RCS_CPT_ASS_DETAIL] [RCS_RECORD_DETAIL]資料庫
        ){
            List<CxrResultJson_cls> CxrResultJson_List = new List<CxrResultJson_cls>();
            try //SQL[Read讀取]
            {
                if (!string.IsNullOrWhiteSpace(inputChartNo) && !string.IsNullOrWhiteSpace(inputIpdNo)) //傳入input不為空值
                {
                    //1.根據 [CHART_NO 病歷號] + [IPD_NO 住院序號] 搜尋 [RCS_MASTER] 主資料庫
                    string sqlcmd_Master = @"
                        SELECT *
                            FROM {2}
                        WHERE CHART_NO = {0}
                            AND IPD_NO = {1}
                        ";
                    DataTable Master_Table = this.DBA.getSqlDataTable(string.Format(
                        sqlcmd_Master
                        , SQLDefend.SQLString(inputChartNo.Trim()) //[CHART_NO] 病歷號 = 04691561
                        , SQLDefend.SQLString(inputIpdNo.Trim()) //[IPD_NO] 住院序號 = I0333017
                        , inputSqlNameMaster //[RCS_CPT_ASS_MASTER]資料庫
                        ));

                    /*------------------獲得 [SQL] 欄位名稱 [下]------------------*/
                    DataTable Detail_ColumnName = this.DBA.getSqlDataTable(string.Format(
                        @"SELECT TOP 0 * FROM {0}"
                        , inputSqlNameDetail //[RCS_CPT_ASS_DETAIL]資料庫
                        ));
                    string Master_ID = Master_Table.Columns[0].ColumnName; //["CPT_ID"] ["RECORD_ID"]主檔編號
                    /*------------------[上] 獲得 [SQL] 欄位名稱------------------*/

                    //2.若 [RCS_MASTER] 主資料庫，有找到資料
                    if (Master_Table != null
                        && Master_Table.Rows.Count > 0
                        && Master_Table.Rows[0][Master_ID] != null
                        && !string.IsNullOrWhiteSpace(Master_Table.Rows[0][Master_ID].ToString())
                    ){
                        foreach (DataRow Master_Rows in Master_Table.Rows)
                        {
                            //3.根據 Master_Rows[Master_ID] 流水號_數值 20170718144526maya
                            //搜尋 [RCS_DETAIL] 細項資料庫
                            CxrResultJson_List = getCxrResultList_byIdDetail(
                                Master_Rows[Master_ID].ToString().Trim()
                                , inputSqlNameDetail
                                );
                        }//foreach-Master
                    }//Master 有資料
                }//傳入input不為空值
            }//try
            catch (Exception ex)
            {
                LogTool.SaveLogMessage(ex, "getCxrResultList_byPatient");
            }//catch
            return CxrResultJson_List;
        }//getCxrResultList_byPatient

        /// <summary>
        /// [Read讀取] SQL欄位值 Cxr畫線功能
        /// </summary>
        /// <param name="inputA_DataTable"></param>
        /// <param name="inputB_DetailSqlName"></param>
        /// <returns></returns>
        public DataTable getCxrResultTable_byDataTable(DataTable inputA_DataTable, string inputB_DetailSqlName)
        {
            try //SQL[Read讀取]
            {
                //["CXR_result_json"]有值
                if (inputA_DataTable != null 
                    && inputA_DataTable.Rows.Count > 0
                    && inputA_DataTable.Rows[0]["CXR_result_json"] != null
                    && !string.IsNullOrWhiteSpace(inputA_DataTable.Rows[0]["CXR_result_json"].ToString())
                ){

                    /*------------------獲得 [SQL] 欄位名稱 [下]------------------*/
                    DataTable dtDetail_ColumnName = this.DBA.getSqlDataTable(string.Format(
                        @"SELECT TOP 0 * FROM {0}"
                        , inputB_DetailSqlName //[RCS_CPT_ASS_DETAIL]資料庫
                        ));
                    string dtDetail_ID = dtDetail_ColumnName.Columns[0].ColumnName; //["CPT_ID"] ["RECORD_ID"]主檔編號
                    /*------------------[上] 獲得 [SQL] 欄位名稱------------------*/

                    // 1.主表格 [Row橫列] 進行 ["CXR_result_json"] 欄位編輯
                    foreach (DataRow input_DRow in inputA_DataTable.Rows)
                    {
                        // 2.讀取 DataTable ["CXR_result_json"] 欄位內容值
                        List<CxrResultJson_cls> CxrResultJson_SList =
                            JsonConvert.DeserializeObject<List<CxrResultJson_cls>>(
                                input_DRow["CXR_result_json"].ToString().Trim()
                                );

                        // 3.加入 [Master_ID] 流水號 20170718144526maya
                        foreach (CxrResultJson_cls CxrResultJson_SNode in CxrResultJson_SList)
                        {
                            CxrResultJson_SNode.SqlTableName = inputB_DetailSqlName.Trim();
                            CxrResultJson_SNode.SqlMasterDetail_ID = input_DRow[ dtDetail_ID ].ToString().Trim();
                        }
                        // 4.加入 "CxrXuwmc_List": null → "CxrXuwmc_List": [18]
                        CxrResultJson_SList = addCxrResultList_byCxrResult(CxrResultJson_SList);
                        // 5.寫入 DataTable ["CXR_result_json"] 欄位內容值
                        input_DRow["CXR_result_json"] = JsonConvert.SerializeObject(CxrResultJson_SList);
                    }//foreach-input_DRow
                }//if-inputA_DataTable
            }//try
            catch (Exception ex)
            {
                LogTool.SaveLogMessage(ex, "getSqlAddCxrTable_byTable");
            }//catch
            return inputA_DataTable;
        }//SQL[Read讀取]

        /*-------------------------[上] Read讀取 + Save儲存 [下]-------------------------*/
        /*-------------------------[上] Read讀取 + Save儲存 [下]-------------------------*/

        /// <summary>
        /// SQL [Save儲存] Cxr資料庫 [RCS_CXR_JSON]
        /// 輸入 List<CXR_XYwmc> 要有資料，才會儲存 [RCS_CXR_JSON] Cxr資料庫
        /// 若輸入 List<CXR_XYwmc> 無資料，仍要 [更新] 刪除 [Cxr_CJID流水號] 舊資料
        /// </summary>
        /// <param name="inputA_CxrCjid">[Cxr_CJID] Cxr流水號 [RCS_CXR_JSON] 資料庫</param>
        /// <param name="paraCxrXywmc_List">[CJ_VALUE] Xywmc座標 [RCS_CXR_JSON] 資料庫</param>
        /// <returns></returns>
        public dbResultMessage saveRcsCxrJson_byCjid_NullDelete(
            string inputA_CxrCjid //[Cxr_CJID] Cxr流水號
            , List<CXR_XYwmc> inputB_CxrXywmcList // [NULL空值] 自動刪除 [RCS_CXR_JSON] 資料庫
        ){
            // 流水號來源：row.RECORD_ID = SQL.GetFixedStrSerialNumber(); [SQLProvider]
            // 流水號來源：DateTime.Now.ToString("yyyyMMddHHmmssfffff") + [user_id 使用者代碼] + [pIpdno 批價序號]
            dbResultMessage dbResultMsg = new dbResultMessage();
            try
            {
                dbResultMsg.State = enmDBResultState.Success; //初始值，設定 [成功]。

                //只有 [Cxr_CJID] 流水號
                if (!string.IsNullOrWhiteSpace(inputA_CxrCjid)
                //&& inputB_CxrXywmcList != null (勿開啟功能)
                //&& inputB_CxrXywmcList.Count > 0 (勿開啟功能)
                ){
                    //正常save功能，有 [Cxr_CJID] 流水號 + [Xywmc] 座標
                    if (!string.IsNullOrWhiteSpace(inputA_CxrCjid)
                        && inputB_CxrXywmcList != null
                        && inputB_CxrXywmcList.Count > 0
                    ){
                        /*----------------------------------- 暫存 [DataTable] 表格 [下] -----------------------------------*/

                        // 1.產生 [DataTable] 等待存入 [RCS_CXR_JSON] Cxr資料庫
                        DataTable newCxrTable = new DataTable();
                        newCxrTable.Columns.Add("CJ_ID");
                        newCxrTable.Columns.Add("CJ_INDEX");
                        newCxrTable.Columns.Add("CJ_VALUE");

                        int ii = 0;
                        // 2.讀取每一筆 List<CXR_XYwmc> 暫存 [DataTable] 表格
                        foreach (CXR_XYwmc CxrXywmc_Node in inputB_CxrXywmcList)
                        {
                            // [mouse] 一定要有 "動作值"
                            if (CxrXywmc_Node != null
                                && !string.IsNullOrWhiteSpace(CxrXywmc_Node.mouse)
                            ){
                                ii++;
                                // 3.讀取每一筆 List<CXR_XYwmc> 暫存 [DataTable] 表格
                                DataRow newRow = newCxrTable.NewRow();
                                {
                                    newRow["CJ_ID"] = inputA_CxrCjid;
                                    newRow["CJ_INDEX"] = ii.ToString("0000"); //前面補0數字
                                    newRow["CJ_VALUE"] = JsonConvert.SerializeObject(CxrXywmc_Node).Trim();
                                }
                                newCxrTable.Rows.Add(newRow);
                            }//if
                        }//foreach

                        /*------------------ [上] 暫存 [DataTable] 表格 + 存入 [RCS_CXR_JSON] Cxr資料庫 [下] ------------------*/

                        // 4.先刪除 [Cxr_CJID流水號] 舊資料
                        // 若 [inputB_CxrXywmcList == null] 空值，要 [更新] 刪除 [Cxr_CJID流水號] 舊資料
                        dbResultMsg = this.DBA.ExecuteNonQuery(
                            "DELETE " + GetTableName.RCS_CXR_JSON.ToString() +
                            " WHERE CJ_ID = " + SQLDefend.SQLString(inputA_CxrCjid.Trim())
                            );
                        //刪除成功
                        if (this.DBA.LastError != null
                            && this.DBA.LastError == ""
                            && dbResultMsg.State == enmDBResultState.Success
                        ){
                            //存入 [RCS_CXR_JSON] Cxr資料庫 (先確認 [DataTable] 有值，才會儲存)
                            //若 [DataTable] 沒有值，代表此病人沒有Cxr圖，已經 [更新] 刪除 [Cxr_CJID流水號] 舊資料 (上方已經刪除)
                            if (newCxrTable.Rows.Count > 0)
                            {
                                // 5.存入 [RCS_CXR_JSON] Cxr資料庫
                                dbResultMsg = this.DBA.UpdateResult(newCxrTable, GetTableName.RCS_CXR_JSON.ToString());
                                //儲存成功
                                if (this.DBA.LastError != null
                                    && this.DBA.LastError == ""
                                    && dbResultMsg.State == enmDBResultState.Success
                                ){
                                    //成功，不做任何事，保留 [dbResultMsg] 成功訊息。
                                }
                                //儲存失敗
                                else
                                {
                                    LogTool.SaveLogMessage("Cxr儲存失敗1:" + this.DBA.LastError.ToString(), "saveRcsCxrJson_byCjid_NullDelete");
                                    LogTool.SaveLogMessage("Cxr儲存失敗2:" + dbResultMsg.dbErrorMessage, "saveRcsCxrJson_byCjid_NullDelete");
                                    this.DBA.Rollback();
                                }
                            }//if (newCxrTable.Rows.Count > 0)
                        }
                        //刪除失敗
                        else
                        {
                            LogTool.SaveLogMessage("Cxr刪除失敗1:" + this.DBA.LastError.ToString(), "saveRcsCxrJson_byCjid_NullDelete");
                            LogTool.SaveLogMessage("Cxr刪除失敗2:" + dbResultMsg.dbErrorMessage, "saveRcsCxrJson_byCjid_NullDelete");
                            this.DBA.Rollback();
                        }

                        /*------------------ [上] 存入 [RCS_CXR_JSON] Cxr資料庫 ------------------*/

                    }//if (inputB_CxrXywmcList != null

                    //有 [Cxr_CJID] 流水號，但 [Xywmc座標] 空值，則要 [刪除]
                    else if (!string.IsNullOrWhiteSpace(inputA_CxrCjid)
                        && (inputB_CxrXywmcList == null || inputB_CxrXywmcList.Count == 0)
                    ){
                        // 4.先刪除 [Cxr_CJID流水號] 舊資料
                        // 若 [inputB_CxrXywmcList == null] 空值，要 [更新] 刪除 [Cxr_CJID流水號] 舊資料
                        dbResultMsg = this.DBA.ExecuteNonQuery(
                            "DELETE " + GetTableName.RCS_CXR_JSON.ToString() +
                            " WHERE CJ_ID = " + SQLDefend.SQLString(inputA_CxrCjid.Trim())
                            );
                        //刪除成功
                        if (this.DBA.LastError != null
                            && this.DBA.LastError == ""
                            && dbResultMsg.State == enmDBResultState.Success
                        ){
                            //刪除成功，不做任何事，保留 [dbResultMsg] 成功訊息。
                        }
                        //刪除失敗
                        else
                        {
                            LogTool.SaveLogMessage("Cxr刪除失敗1:" + this.DBA.LastError.ToString(), "saveRcsCxrJson_byCjid_NullDelete");
                            LogTool.SaveLogMessage("Cxr刪除失敗2:" + dbResultMsg.dbErrorMessage, "saveRcsCxrJson_byCjid_NullDelete");
                            this.DBA.Rollback();
                        }
                    }//else 有 [Cxr_CJID] 流水號，但 [Xywmc座標] null，則要 [刪除]
                    /*------------------ [上] 存入 [RCS_CXR_JSON] Cxr資料庫 ------------------*/
                }
                //沒有 [Cxr_CJID] 流水號
                else //if [!string.IsNullOrWhiteSpace(inputA_CxrCjid)]
                {
                    //流水號空白，強制不進行動作，紀錄於 [Log] 以待檢查，返回主畫面。
                    LogTool.SaveLogMessage("紀錄提醒Cxr儲存功能 [Cxr_CJID] 流水號不得為空白，請確認 [Controller控制器] 程式碼", "saveRcsCxrJson_byCjid_NullDelete");
                    return dbResultMsg;
                }
            }//try
            catch (Exception ex)
            {
                LogTool.SaveLogMessage(ex, "saveRcsCxrJson_byCjid_NullDelete");
                LogTool.SaveLogMessage(this.DBA.LastError.ToString(), "saveRcsCxrJson_byCjid_NullDelete");
                LogTool.SaveLogMessage(dbResultMsg.dbErrorMessage, "saveRcsCxrJson_byCjid_NullDelete");
                this.DBA.Rollback();
            }//catch
            return dbResultMsg;
        }//[Save儲存] RCS_CXR_JSON

        public CxrResult_SplitedObj Fn_CxrResultSplit(
            string inputSqlDetailTableName //資料表SQL名稱
            , string inputMasterDetailID //主表ID流水號
            , CxrResultJson_cls inputCxrResultNode //CXR畫線資料
        ){
            CxrResult_SplitedObj returnCxrSplitedObj = new CxrResult_SplitedObj();
            try
            {
                if (!string.IsNullOrWhiteSpace(inputSqlDetailTableName)
                    && !string.IsNullOrWhiteSpace(inputMasterDetailID)
                ){
                    // (1) 資料表SQL名稱
                    inputCxrResultNode.SqlTableName = inputSqlDetailTableName;
                    // (2) 主表ID流水號
                    inputCxrResultNode.SqlMasterDetail_ID = inputMasterDetailID;
                    // (3) Cxr流水號 [資料表名稱] + [主表ID流水號]
                    inputCxrResultNode.Cxr_CJID = inputSqlDetailTableName + "_" + inputMasterDetailID;
                    returnCxrSplitedObj.Cxr_CJID  = inputSqlDetailTableName + "_" + inputMasterDetailID;
                    // 一定要 [3.檢查日期] 有值才會儲存 [RCS_CXR_JSON] Cxr資料庫
                    if (inputCxrResultNode != null
                    && inputCxrResultNode.Result_Date != null
                    && !string.IsNullOrWhiteSpace(inputCxrResultNode.Result_Date)
                    ){
                        // (7) 暫存 [temp空間] 等待下方 [RCS_RECORD_DETAIL] 完成儲存後，才儲存 [RCS_CXR_JSON]
                        returnCxrSplitedObj.cxrXuwmcList = inputCxrResultNode.CxrXuwmc_List;
                    }
                    // (7) 主表 [RCS_CPT_ASS_DETAIL] 去除 "CxrXuwmc_List": [18] → "CxrXuwmc_List": null
                    inputCxrResultNode.CxrXuwmc_List = null;
                    inputCxrResultNode.singJson = null;
                    inputCxrResultNode.singJsonImageBase64 = null;
                    //回傳已經分割的物件
                    returnCxrSplitedObj.cxrResult_SplitedNode = inputCxrResultNode;
                }//if-IsNullOrWhiteSpace
            }
            catch (Exception ex)
            {
                LogTool.SaveLogMessage(ex, "Fn_CxrResultSplit");
            }
            return returnCxrSplitedObj;
        }//fn_CxrResultSplit

        public dbResultMessage saveCxrResultStrDropdownlist( //CXR下拉清單
            string inputCxrResultStr //Cxr檢查結果
            , string inputSqlDetailTableName //[RCS_RECORD_DETAIL] [RCS_CPT_ASS_DETAIL]
        ){
            dbResultMessage dbResultMsg = new dbResultMessage();
            DataTable sysParams_DTable = new DataTable();// RCS_SYS_PARAMS的資料
            try
            {
                // 初步獲得 [RCS_SYS_PARAMS] CXR下拉清單
                // [P_MODEL] = [RCS_RECORD_DETAIL] [RCS_CPT_ASS_DETAIL]
                // [P_GROUP] = 'cxr_result'
                List<string> CxrResultDropdownList = getCxrResultStr_DropdownList();
                //檢查輸入值是否 [RCS_SYS_PARAMS] 已經存在
                if (!string.IsNullOrWhiteSpace(inputCxrResultStr)
                    && !CxrResultDropdownList.Contains(inputCxrResultStr)
                ){
                    string sqlcmd = @"SELECT TOP 0 * FROM " + GetTableName.RCS_SYS_PARAMS.ToString().Trim();
                    sysParams_DTable = this.DBA.getSqlDataTable(sqlcmd);
                    DataRow sysParams_newDRow = sysParams_DTable.NewRow();
                    {
                        sysParams_newDRow["P_ID"] = DateTime.Now.ToString("yyyyMMddHHmmss") + user_info.user_id;
                        sysParams_newDRow["P_MODEL"] = inputSqlDetailTableName; //[RCS_RECORD_DETAIL] [RCS_CPT_ASS_DETAIL]
                        sysParams_newDRow["P_GROUP"] = "cxr_result"; 
                        sysParams_newDRow["P_NAME"] = inputCxrResultStr; //[Cxr檢查結果]
                        sysParams_newDRow["P_VALUE"] = inputCxrResultStr; //[Cxr檢查結果]
                        sysParams_newDRow["P_LANG"] = "zh-tw";
                        sysParams_newDRow["P_SORT"] = (CxrResultDropdownList.Count + 1).ToString(); //存入個數
                        sysParams_newDRow["P_MEMO"] = "";
                        sysParams_newDRow["P_STATUS"] = "1";
                        sysParams_newDRow["P_MANAGE"] = "0";
                    }
                    sysParams_DTable.Rows.Add(sysParams_newDRow);
                    /*-------------------------*/
                    dbResultMsg = this.DBA.UpdateResult(sysParams_DTable, GetTableName.RCS_SYS_PARAMS.ToString());
                    //新增成功
                    if (this.DBA.LastError != null
                        && this.DBA.LastError == ""
                        && dbResultMsg.State == enmDBResultState.Success
                    ){
                        //新增成功，不做任何事，保留 [dbResultMsg] 成功訊息。
                    }
                    //新增失敗
                    else
                    {
                        LogTool.SaveLogMessage("CXR下拉清單新增失敗1:" + this.DBA.LastError.ToString(), "saveCxrResultStrDropdownlist");
                        LogTool.SaveLogMessage("CXR下拉清單新增失敗2:" + dbResultMsg.dbErrorMessage, "saveCxrResultStrDropdownlist");
                        this.DBA.Rollback();
                    }
                }//if-(IsNullOrWhiteSpace)
            }
            catch (Exception ex)
            {
                LogTool.SaveLogMessage(ex, "saveCxrResultStrDropdownlist");
            }
            return dbResultMsg;
        }//saveCxrResultStrDropdownlist

        /*------------------------- Save儲存 [上] -------------------------*/
        public string getCxrImageBase64str_byCxrCjid(string inputCxrCjid)
        {
            string Base64String = "";
            try
            {
                Image bitmap = Bitmap.FromFile(AppDomain.CurrentDomain.BaseDirectory + @"Images\deviant_empty.png");
                //Pen myPen = new Pen(Brushes.DeepSkyBlue);
                //myPen.Width = 8.0F;
                List<CXR_XYwmc> CxrXYwmc_List = getCxrXywmcList_byCJID(inputCxrCjid);
                CXR_XYwmc previous_CxrXYwmcNode = CxrXYwmc_List.FirstOrDefault();

                if (CxrXYwmc_List != null
                    && CxrXYwmc_List.Count > 0
                ){
                    using (Graphics myGraphics = Graphics.FromImage(bitmap))
                    {
                        foreach (CXR_XYwmc current_CxrXYwmcNode in CxrXYwmc_List)
                        {
                            Pen myPen = new Pen(Color.Black); //color
                            myPen.Width = int.Parse(current_CxrXYwmcNode.width); //width
                            myPen.StartCap = LineCap.Round; //圓角
                            myPen.EndCap = LineCap.Round; //圓角
                            switch (current_CxrXYwmcNode.color)
                            {
                                default:
                                case "black":
                                    myPen.Color = Color.Black;
                                    break;
                                case "red":
                                    myPen.Color = Color.Red;
                                    break;
                                case "yellow":
                                    myPen.Color = Color.Yellow;
                                    break;
                                case "green":
                                    myPen.Color = Color.Green;
                                    break;
                                case "blue":
                                    myPen.Color = Color.Blue;
                                    break;
                                case "white":
                                    myPen.Color = Color.White;
                                    break;
                            }
                            switch (current_CxrXYwmcNode.mouse)
                            {
                                default:
                                case "down":
                                    {
                                        break; //不做繪圖，但需要break
                                    }
                                case "move":
                                case "up":
                                case "out":
                                    {
                                        myGraphics.DrawLine(
                                            myPen
                                            , new Point(int.Parse(previous_CxrXYwmcNode.x), int.Parse(previous_CxrXYwmcNode.y))
                                            , new Point(int.Parse(current_CxrXYwmcNode.x), int.Parse(current_CxrXYwmcNode.y))
                                            );
                                        break;
                                    }
                            }//switch
                            previous_CxrXYwmcNode = current_CxrXYwmcNode;
                        }//foreach
                    }//using
                }//if

                System.IO.MemoryStream stream = new System.IO.MemoryStream();
                bitmap.Save(stream, System.Drawing.Imaging.ImageFormat.Png);
                byte[] imageBytes = stream.ToArray();
                // Convert byte[] to Base64 String
                Base64String = "data:image/gif;base64," + Convert.ToBase64String(imageBytes);
            }//try
            catch (Exception ex)
            {
                LogTool.SaveLogMessage(ex, "getCxrImageBase64str_byCxrCjid");
            }//catch
            return Base64String;
        }//getCxrImageBase64str_byCxrCjid
    }//public class CxrCanvasLineDraw : BaseModel

}//namespace RCS.Models
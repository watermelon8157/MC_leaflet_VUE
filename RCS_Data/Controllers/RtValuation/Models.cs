using Com.Mayaminer;
using Dapper;
using mayaminer.com.library;
using Newtonsoft.Json;
using RCS_Data.Models;
using RCS_Data.Models.DB;
using RCS_Data.Models.ViewModels;
using RCSData.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace RCS_Data.Controllers.RtValuation
{
    public class Models : BaseModels, Interface
    {
        string csName = "RtValuation.Models";
        const string dateFormat = "yyyy-MM-dd HH:mm:ss";



        public RESPONSE_MSG SaveValuationConsumableItem(SaveForm form) {

            string actionName = "SaveValuationConsumableItem";
            RESPONSE_MSG rm = new RESPONSE_MSG();
            ValuationConsumableItem VCitem = new ValuationConsumableItem();
            List<DB_RCS_VALUATION_DTL> list = new List<DB_RCS_VALUATION_DTL>();

            list.AddRange(form.D);
            list.AddRange(form.E);
            list.AddRange(form.N);

            List<RCS_VALUATION_ITEMS> valuation_table = new List<RCS_VALUATION_ITEMS>();
            List<RCS_CONSUMABLE_DTL_NEW_ITEMS> Consumable_table = new List<RCS_CONSUMABLE_DTL_NEW_ITEMS>();

            List<string> Check_Message = new List<string>();
            string ErrorMessage = "";
            List<string> valuation_name_list = new List<string>();
            List<string> Quantity_ErrorList = new List<string>();

            VCitem.V_ID = form.V_ID;
            VCitem.DTL = list;
            VCitem.RECORD_DATE = form.RECORD_DATE;
            bool is_MODIFY = string.IsNullOrWhiteSpace(VCitem.V_ID) ? false : true;
            bool is_RECORDDATE = string.IsNullOrWhiteSpace(VCitem.RECORD_DATE) ? true : false;
            var pat_info = form.pat_info;
            var user_info = form.user_info;
            try
            {

                SQLProvider SQL = new SQLProvider();

                Dapper.DynamicParameters dp_MST = null;
                string sql_MST = "", sql_DTL = "", sql_SUB = "";
                string WORK_TYPE_Error = "";
                List<string> WORK_TYPE_List = new List<string>();
                string CheckSameData = " select * from RCS_SYS_CONSUMABLE_LIST";
                Dapper.DynamicParameters Data_dp = new Dapper.DynamicParameters();
                Consumable_table = SQL.DBA.getSqlDataTable<RCS_CONSUMABLE_DTL_NEW_ITEMS>(CheckSameData, Data_dp);
                if (!is_RECORDDATE)
                {
                    //日期不為空
                    double IntNum = 0;
                    #region 判斷是否有輸入非數字資料 
                    WORK_TYPE_List = VCitem.DTL.FindAll(x => !string.IsNullOrEmpty(x.ITEM_VALUE) && !double.TryParse(x.ITEM_VALUE, out IntNum)).Select(x => x.WORK_TYPE).Distinct().ToList();
                    #endregion

                    if (WORK_TYPE_List.Count > 0)
                    {
                        //《有》輸入非數值執行以下程式
                        List<string> userlookList = new List<string>();
                        if (WORK_TYPE_List.Contains("E"))
                        {
                            userlookList.Add("大夜");
                        }
                        if (WORK_TYPE_List.Contains("D"))
                        {
                            userlookList.Add("白班");
                        }
                        if (WORK_TYPE_List.Contains("N"))
                        {
                            userlookList.Add("小夜");
                        }
                        //《有》輸入非數值，顯示錯誤訊息
                        WORK_TYPE_Error = String.Join("、", userlookList);
                        LogTool.SaveLogMessage(WORK_TYPE_Error + "，請輸入數字，儲存失敗!", this.csName);
                        rm.status = RESPONSE_STATUS.ERROR;
                        rm.message = WORK_TYPE_Error + "，請輸入數字，儲存失敗!";
                    }
                    else
                    {
                        //《沒有》輸入非數值執行以下程式
                        //先判斷輸入數量是否正確
                        #region 判斷數量是否正確                     
                        Check_Message = CheckTotal_Valuation_Detail(VCitem.DTL);
                        #endregion

                        if (Check_Message != null && Check_Message.Count() > 0)
                        {
                            //輸入數量《不正確》
                            foreach (var item in Consumable_table)
                            {
                                for (int i = 0; i < Check_Message.Count(); i++)
                                {
                                    if (item.C_ID == Check_Message[i])
                                    {
                                        valuation_name_list.Add(item.C_CNAME);
                                    }
                                }
                            }

                            ErrorMessage = String.Join("、", valuation_name_list);
                            rm.status = RESPONSE_STATUS.ERROR;
                            rm.message = "請確認" + ErrorMessage + "的數量!";
                            LogTool.SaveLogMessage(rm.message, actionName, this.csName);
                        }
                        else
                        {
                            //輸入數量《正確》                           
                            #region 主表
                            //判斷V_ID是否空白
                            if (!is_MODIFY)  //V_ID為空白，新增功能
                            {
                                //判斷是否有重複的日期，沒有重複日期即《可以新增》
                                string CheckSameDate = " select * from RCS_VALUATION_MASTER where CHART_NO = '" + pat_info.chart_no + "' AND IPD_NO = '" + pat_info.ipd_no + "' AND RECORD_DATE = '" + VCitem.RECORD_DATE + "' AND DATASTATUS = '0' ";
                                Dapper.DynamicParameters dp = new Dapper.DynamicParameters();
                                valuation_table = SQL.DBA.getSqlDataTable<RCS_VALUATION_ITEMS>(CheckSameDate, dp);

                                if (valuation_table.Count() > 0)
                                {  //日期重複  
                                   //判斷是否有重複的日期，有重複日期《不可以新增》 
                                    rm.status = RESPONSE_STATUS.ERROR;
                                    rm.message = "已有 " + VCitem.RECORD_DATE + " 記錄存在，儲存失敗!";
                                    LogTool.SaveLogMessage(rm.message, actionName, this.csName);
                                }
                            }
                            else
                            {
                                //V_ID不為空白，編輯功能
                                #region 修改原先SQL

                                sql_MST = @"
                                    UPDATE RCS_VALUATION_MASTER 
                                    SET MODIFY_ID = @MODIFY_ID
                                        ,MODIFY_NAME = @MODIFY_NAME
                                        ,MODIFY_DATE = @MODIFY_DATE
                                        ,DATASTATUS = @DATASTATUS
                                    WHERE V_ID = @V_ID
                                    ";
                                dp_MST = new Dapper.DynamicParameters();
                                dp_MST.Add("MODIFY_ID", user_info.user_id);
                                dp_MST.Add("MODIFY_NAME", user_info.user_name);
                                dp_MST.Add("MODIFY_DATE", DateTime.Now.ToString("yyyy-MM-dd HH:mm"));
                                dp_MST.Add("DATASTATUS", "2");
                                dp_MST.Add("V_ID", VCitem.V_ID);
                                SQL.DBA.DBExecute(sql_MST, dp_MST);
                                #endregion

                            }
                            #endregion

                            if (valuation_table.Count() == 0)
                            {
                                #region 新增
                                VCitem.V_ID = SQL.GetFixedStrSerialNumber(user_info.user_id, pat_info.ipd_no);

                                #region 新增SQL
                                sql_MST = @"
                                    INSERT INTO RCS_VALUATION_MASTER 
                                    (
                                        V_ID
                                        ,IPD_NO
                                        ,CHART_NO
                                        ,RECORD_DATE
                                        ,CREATE_ID
                                        ,CREATE_NAME
                                        ,CREATE_DATE
                                        ,MODIFY_ID
                                        ,MODIFY_NAME
                                        ,MODIFY_DATE
                                        ,DATASTATUS
                                        ,UPLOAD_STATUS                                    
                                    ) 
                                    VALUES 
                                    (
                                        @V_ID
                                        ,@IPD_NO
                                        ,@CHART_NO
                                        ,@RECORD_DATE
                                        ,@CREATE_ID
                                        ,@CREATE_NAME
                                        ,@CREATE_DATE
                                        ,@MODIFY_ID
                                        ,@MODIFY_NAME
                                        ,@MODIFY_DATE
                                        ,@DATASTATUS
                                        ,@UPLOAD_STATUS                                    
                                    )";
                                #endregion
                                dp_MST = new Dapper.DynamicParameters();
                                dp_MST.Add("V_ID", VCitem.V_ID);
                                dp_MST.Add("IPD_NO", pat_info.ipd_no);
                                dp_MST.Add("CHART_NO", pat_info.chart_no);
                                dp_MST.Add("RECORD_DATE", VCitem.RECORD_DATE);
                                dp_MST.Add("CREATE_ID", user_info.user_id);
                                dp_MST.Add("CREATE_NAME", user_info.user_name);
                                dp_MST.Add("CREATE_DATE", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
                                dp_MST.Add("MODIFY_ID", user_info.user_id);
                                dp_MST.Add("MODIFY_NAME", user_info.user_name);
                                dp_MST.Add("MODIFY_DATE", DateTime.Now.ToString("yyyy-MM-dd HH:mm"));
                                dp_MST.Add("DATASTATUS", "0");
                                dp_MST.Add("UPLOAD_STATUS", "0");

                                if (VCitem.DTL != null && VCitem.DTL.Count > 0)
                                {
                                    List<DB_RCS_VALUATION_DETAIL> list_DTL = new List<DB_RCS_VALUATION_DETAIL>();
                                    List<DB_RCS_VALUATION_SUB_MASTER> subList = new List<DB_RCS_VALUATION_SUB_MASTER>();

                                    if (VCitem.DTL != null && VCitem.DTL.Count > 0)
                                    {
                                        foreach (var item in VCitem.DTL)
                                        {
                                            string sub_id = VCitem.V_ID + item.WORK_TYPE;
                                            // 檢查是否已經建立SUB班別 
                                            if (!subList.Exists(x => x.WORK_TYPE == item.WORK_TYPE))
                                            {
                                                subList.Add(new DB_RCS_VALUATION_SUB_MASTER()
                                                {
                                                    V_ID = VCitem.V_ID,
                                                    V_SUB_ID = sub_id,
                                                    WORK_TYPE = item.WORK_TYPE
                                                });
                                            }
                                            if (VCitem.DTL.Exists(x => x.ITEM_NAME == null))
                                            {
                                                item.ITEM_NAME = item.C_ID;
                                                if (string.IsNullOrEmpty(item.ITEM_VALUE))
                                                {
                                                    item.ITEM_VALUE = "0";
                                                }
                                                else
                                                {
                                                    item.ITEM_VALUE = item.ITEM_VALUE;
                                                }
                                            }
                                            list_DTL.Add(new DB_RCS_VALUATION_DETAIL()
                                            {
                                                ITEM_NAME = item.ITEM_NAME,
                                                ITEM_VALUE = item.ITEM_VALUE,
                                                V_SUB_ID = sub_id
                                            });
                                        }
                                    }

                                    SQL.DBA.DBExecute(sql_MST, dp_MST);
                                    #region 新增SUB_MASTER項目內容
                                    sql_SUB = @"
                                    INSERT INTO RCS_VALUATION_SUB_MASTER 
                                    (
                                        V_SUB_ID,
                                        V_ID,
                                        WORK_TYPE                                   
                                    ) 
                                    VALUES 
                                    (
                                        @V_SUB_ID,
                                        @V_ID,
                                        @WORK_TYPE
                                    )";
                                    #endregion
                                    SQL.DBA.DBExecute<DB_RCS_VALUATION_SUB_MASTER>(sql_SUB, subList);
                                    #region 修改表單項目內容 
                                    sql_DTL = "INSERT INTO RCS_VALUATION_DETAIL Values (@V_SUB_ID,@ITEM_NAME,@ITEM_VALUE);";
                                    SQL.DBA.DBExecute<DB_RCS_VALUATION_DETAIL>(sql_DTL, list_DTL);
                                    #endregion
                                }


                                if (SQL.DBA.hasLastError)
                                {
                                    LogTool.SaveLogMessage(SQL.DBA.lastError, actionName, this.csName);
                                    rm.status = RESPONSE_STATUS.ERROR;
                                    rm.message = "儲存失敗!";
                                    SQL.DBA.Rollback();
                                }
                                else
                                {
                                    rm.status = RESPONSE_STATUS.SUCCESS;
                                    rm.message = "儲存成功!";
                                    SQL.DBA.Commit();
                                }
                                #endregion
                            }
                        }
                    }// 《沒有》輸入非數值執行以上程式

                }//日期不為空
                else
                {
                    //記錄日期如果是空的，跳錯誤訊息
                    LogTool.SaveLogMessage("請輸入記錄日期，儲存失敗!", this.csName);
                    rm.status = RESPONSE_STATUS.ERROR;
                    rm.message = "請輸入記錄日期，儲存失敗!";
                }
            }
            catch (Exception ex)
            {
                LogTool.SaveLogMessage(ex, actionName, this.csName);
                rm.status = RESPONSE_STATUS.EXCEPTION;
                rm.message = "計價單，儲存失敗!";
            }



            return rm;
        }

        /// <summary>
        /// 呼吸治療評估單LIST
        /// </summary>
        /// <param name="cpt_dtl_new_items"></param>
        /// <param name="form"></param>
        /// <returns></returns>
        public List<RCS_VALUATION_ITEMS> GetValuationTableList(string IPD_NO, string CHART_NO, string sDate, string eDate)
        {

            string actioName = "GetValuationTableList";
            List<RCS_VALUATION_ITEMS> valuation_table = new List<RCS_VALUATION_ITEMS>();
            List<RCS_VALUATION_ITEMS> show_valuation_table = new List<RCS_VALUATION_ITEMS>();
            if (sDate == "" || sDate == null)
            {
                sDate = DateTime.Now.AddDays(-6).ToString("yyyy-MM-dd HH:mm");
            }
            if (eDate == "" || eDate == null)
            {
                eDate = DateTime.Now.ToString("yyyy-MM-dd HH:mm");
            }
            try
            {
                SQLProvider SQL = new SQLProvider();
                string _query = " select * from RCS_VALUATION_MASTER where IPD_NO = @IPD_NO AND CHART_NO = @CHART_NO AND DATASTATUS in ('0') AND RECORD_DATE >= @sDate AND RECORD_DATE <= @eDate";
                Dapper.DynamicParameters dp = new Dapper.DynamicParameters();
                dp.Add("CHART_NO", CHART_NO);
                dp.Add("IPD_NO", IPD_NO);
                dp.Add("sDate", sDate);
                dp.Add("eDate", eDate);
                valuation_table = SQL.DBA.getSqlDataTable<RCS_VALUATION_ITEMS>(_query, dp);
                show_valuation_table = valuation_table.OrderByDescending(x => x.RECORD_DATE).ToList();
            }
            catch (Exception ex)
            {
                Com.Mayaminer.LogTool.SaveLogMessage(ex, actioName, this.csName);
            }
            return show_valuation_table;

        }

        public List<DB_RCS_SYS_CONSUMABLE_LIST> GetCONSUMABLElist() {
            string _query = "SELECT * FROM RCS_SYS_CONSUMABLE_LIST";

            SQLProvider SQL = new SQLProvider();
            Dapper.DynamicParameters dp = new Dapper.DynamicParameters();
            List<DB_RCS_SYS_CONSUMABLE_LIST> result = SQL.DBA.getSqlDataTable<DB_RCS_SYS_CONSUMABLE_LIST>(_query, dp).OrderBy(x=>x.ORDERBY).ThenBy(x=>x.C_CNAME).ToList();

            return result;
        }

        public RCS_VALUATION_DETAIL_MODEL EditConsumableList(string VID)
        {
            var result = new RCS_VALUATION_DETAIL_MODEL();

            result.D = new List<DB_RCS_VALUATION_DTL>();
            result.E = new List<DB_RCS_VALUATION_DTL>();
            result.N = new List<DB_RCS_VALUATION_DTL>();
            string actionName = "EditConsumableList";
            List<DB_RCS_VALUATION_DTL> consumable_table = new List<DB_RCS_VALUATION_DTL>();
            try
            {
                string _query = @" Select M.V_ID,M.RECORD_DATE,SM.V_SUB_ID,SM.WORK_TYPE,D.ITEM_NAME,D.ITEM_VALUE,M.IPD_NO,M.CHART_NO,L.C_CNAME,L.C_MEMO,L.VENDER_CODE from RCS_VALUATION_SUB_MASTER as SM "
                                   + "RIGHT join RCS_VALUATION_MASTER as M on SM.V_ID = M.V_ID "
                                   + "Left join RCS_VALUATION_DETAIL as D on SM.V_SUB_ID = D.V_SUB_ID "
                                   + "Left join RCS_SYS_CONSUMABLE_LIST as L on L.C_ID = D.ITEM_NAME WHERE M.DATASTATUS = '0' AND M.V_ID = '" + VID + "' ORDER BY L.C_CNAME";
                Dapper.DynamicParameters dp = new Dapper.DynamicParameters();
                SQLProvider SQL = new SQLProvider();
                consumable_table = SQL.DBA.getSqlDataTable<DB_RCS_VALUATION_DTL>(_query, dp);


                Dictionary<string, object> dict = new Dictionary<string, object>();

                foreach (var item in consumable_table.GroupBy(x => x.WORK_TYPE).ToList())
                {

                    foreach (DB_RCS_VALUATION_DTL val in item)
                    {
                        if (!string.IsNullOrWhiteSpace(val.ITEM_VALUE) && val.ITEM_VALUE != "null")
                        {
                            try
                            {
                                //dict.Add("_" + val.ITEM_NAME, val.ITEM_VALUE);
                                if (item.Key == "D")
                                {

                                    result.D.Add(val);

                                }
                                else if (item.Key == "E")
                                {
                                    result.E.Add(val);
                                }
                                else if (item.Key == "N")
                                {
                                    result.N.Add(val);
                                }
                            }
                            catch (Exception ex)
                            {
                                LogTool.SaveLogMessage(ex, actionName, this.csName);
                            }

                        }
                    }

                    //if (item.Key == "D")
                    //{

                    //    result.D = Newtonsoft.Json.JsonConvert.DeserializeObject<RCS_VALUATION_DETAIL_DATA>(
                    //  Newtonsoft.Json.JsonConvert.SerializeObject(dict));

                    //}
                    //else if (item.Key == "E")
                    //{
                    //    result.E = Newtonsoft.Json.JsonConvert.DeserializeObject<RCS_VALUATION_DETAIL_DATA>(
                    //  Newtonsoft.Json.JsonConvert.SerializeObject(dict));
                    //}
                    //else if (item.Key == "N")
                    //{
                    //    result.N = Newtonsoft.Json.JsonConvert.DeserializeObject<RCS_VALUATION_DETAIL_DATA>(
                    //  Newtonsoft.Json.JsonConvert.SerializeObject(dict));
                    //}


                }

                if (consumable_table.Any())
                {
                    result.V_ID = consumable_table.First().V_ID;
                    result.RECORD_DATE = consumable_table.First().RECORD_DATE;
                }
                else {

                    result.RECORD_DATE = DateTime.Now.ToString("yyyy-MM-dd");
                }

                result.CONSUMABLE_LIST = GetCONSUMABLElist();


                if (!result.D.Any()) {
                    result.D = new List<DB_RCS_VALUATION_DTL>();

                    foreach (var item in result.CONSUMABLE_LIST) {

                        result.D.Add(new DB_RCS_VALUATION_DTL() {
                            V_SUB_ID="",
                            ITEM_NAME=item.C_ID,
                            ITEM_VALUE="",
                            WORK_TYPE="D"
                        });
                    }

                }
                if (!result.E.Any())
                {
                    result.E = new List<DB_RCS_VALUATION_DTL>();
                    foreach (var item in result.CONSUMABLE_LIST)
                    {

                        result.E.Add(new DB_RCS_VALUATION_DTL()
                        {
                            V_SUB_ID = "",
                            ITEM_NAME = item.C_ID,
                            ITEM_VALUE = "",
                            WORK_TYPE = "E"
                        });
                    }
                }
                if (!result.N.Any())
                {
                    result.N = new List<DB_RCS_VALUATION_DTL>();
                    foreach (var item in result.CONSUMABLE_LIST)
                    {

                        result.N.Add(new DB_RCS_VALUATION_DTL()
                        {
                            V_SUB_ID = "",
                            ITEM_NAME = item.C_ID,
                            ITEM_VALUE = "",
                            WORK_TYPE = "N"
                        });
                    }
                }

               
            }
            catch (Exception ex)
            {
                Com.Mayaminer.LogTool.SaveLogMessage(ex, actionName, this.csName);
            }


            return result;
        }

        private List<string> CheckTotal_Valuation_Detail(List<DB_RCS_VALUATION_DTL> list_DTL)
        {
            string actionName = "GetConsumableList";
            List<DB_RCS_VALUATION_DTL> consumable_table = new List<DB_RCS_VALUATION_DTL>();
            List<RCS_VALUATION_ITEMS> valuation_table = new List<RCS_VALUATION_ITEMS>();
            List<string> Check_Message = new List<string>();
            string Item_Quantity = "";
            List<string> DontSave_Message = new List<string>();
            Dictionary<string, List<DB_RCS_VALUATION_DTL>> ConsumableResult = new Dictionary<string, List<DB_RCS_VALUATION_DTL>>();
            try
            {
                if (string.IsNullOrWhiteSpace(list_DTL[0].C_ID))
                {
                    ConsumableResult = list_DTL.GroupBy(o => o.ITEM_NAME).ToDictionary(o => o.Key, o => o.ToList());
                }
                else
                {
                    ConsumableResult = list_DTL.GroupBy(o => o.C_ID).ToDictionary(o => o.Key, o => o.ToList());
                }

                foreach (KeyValuePair<string, List<DB_RCS_VALUATION_DTL>> item in ConsumableResult)
                {
                    int valcnt = 0;

                    DB_RCS_VALUATION_DTL newTable = new DB_RCS_VALUATION_DTL();
                    for (int i = 0; i < item.Value.Count(); i++)
                    {
                        if (string.IsNullOrEmpty(item.Value[i].ITEM_VALUE))
                        {
                            item.Value[i].ITEM_VALUE = "0";
                        }
                        valcnt += int.Parse(item.Value[i].ITEM_VALUE);
                    }

                    if (string.IsNullOrWhiteSpace(list_DTL[0].C_ID))
                    {
                        newTable.ITEM_NAME = item.Value[0].ITEM_NAME;
                    }
                    else
                    {
                        newTable.ITEM_NAME = item.Value[0].C_ID;
                    }

                    newTable.ITEM_VALUE = valcnt.ToString();
                    Item_Quantity = CheckTotal_Quantity(newTable.ITEM_NAME);
                    valcnt = 0;

                    switch (Item_Quantity)
                    {
                        case "1":
                            // <=1
                            if (Int32.Parse(newTable.ITEM_VALUE) > 1)
                            {
                                DontSave_Message.Add(newTable.ITEM_NAME);
                            }
                            break;
                        case "2":
                            // <=2
                            if (Int32.Parse(newTable.ITEM_VALUE) > 2)
                            {
                                DontSave_Message.Add(newTable.ITEM_NAME);
                            }
                            break;
                        case "3":
                            // <=3
                            if (Int32.Parse(newTable.ITEM_VALUE) > 3)
                            {
                                DontSave_Message.Add(newTable.ITEM_NAME);
                            }
                            break;
                        case "4":
                            // <=4
                            if (Int32.Parse(newTable.ITEM_VALUE) > 4)
                            {
                                DontSave_Message.Add(newTable.ITEM_NAME);
                            }
                            break;
                        case "5":
                            // <=5
                            if (Int32.Parse(newTable.ITEM_VALUE) > 5)
                            {
                                DontSave_Message.Add(newTable.ITEM_NAME);
                            }
                            break;
                        default:
                            break;
                    }
                }
                Check_Message = DontSave_Message;
            }
            catch (Exception ex)
            {
                Com.Mayaminer.LogTool.SaveLogMessage(ex, actionName, this.csName);
            }

            return Check_Message;
        }

        private string CheckTotal_Quantity(string Item_Name)
        {
            string Item_Quantity = "";
            if (!string.IsNullOrWhiteSpace(Item_Name) && Item_Name == "25090170")
            {
                Item_Quantity = "2"; // <=2
            }
            else if (!string.IsNullOrWhiteSpace(Item_Name) && Item_Name == "25110050")
            {
                Item_Quantity = "2"; // <=2
            }
            else if (!string.IsNullOrWhiteSpace(Item_Name) && Item_Name == "25110590")
            {
                Item_Quantity = "2"; // <=2
            }
            else if (!string.IsNullOrWhiteSpace(Item_Name) && Item_Name == "25110281")
            {
                Item_Quantity = "3"; // <=3
            }
            else if (!string.IsNullOrWhiteSpace(Item_Name) && Item_Name == "25110846")
            {
                Item_Quantity = "4"; // <=4
            }
            else if (!string.IsNullOrWhiteSpace(Item_Name) && Item_Name == "25040090")
            {
                Item_Quantity = "5"; // <=5
            }
            else if (!string.IsNullOrWhiteSpace(Item_Name) && Item_Name == "25110411")
            {
                Item_Quantity = "5"; // <=5
            }
            else
            {
                Item_Quantity = "1"; // <=1
            }
            return Item_Quantity;
        }
    }


    public class DB_RCS_VALUATION_DTL : RCS_CONSUMABLE_DTL_NEW_ITEMS
    {
        public string V_SUB_ID { get; set; }

        public string ITEM_NAME { get; set; }

        public string ITEM_VALUE { get; set; }
    }
    public class RCS_CONSUMABLE_DTL_NEW_ITEMS : RCS_VALUATION_CHECK
    {
        ///// <summary> 紀錄時間 </summary>
        //public string RECORD_DATE { set; get; }        
        /// <summary> 項目碼 </summary>
        public string C_ID { set; get; }
        /// <summary> 類型 </summary>
        public string C_TYPE { set; get; }
        /// <summary> 名稱 </summary>
        public string C_CNAME { set; get; }
        /// <summary> 單位 </summary>
        public string C_MEMO { set; get; }
        /// <summary> 廠商碼 </summary>
        public string VENDER_CODE { set; get; }

        //public string V_ID { get; set; }

        //public string ITEM_NAME { get; set; }

        public string ITEM_VALUE { get; set; }
        /// <summary> 班別 </summary>
        public string WORK_TYPE { get; set; }
    }

    public class RCS_VALUATION_CHECK : ValuationConsumableItem
    {
        public string CREATE_ID { get; set; }
        public string CREATE_NAME { get; set; }
        public string CREATE_DATE { get; set; }
    }

    public class ValuationConsumableItem
    {
        public string V_SUB_ID { get; set; }
        public string V_ID { get; set; }
        public string RECORD_DATE { get; set; }
        public List<DB_RCS_VALUATION_DTL> DTL { get; set; }

    }

    public class RCS_VALUATION_DETAIL_MODEL
    {

        public string V_ID { get; set; }

        public string RECORD_DATE { get; set; }

        public List<DB_RCS_SYS_CONSUMABLE_LIST> CONSUMABLE_LIST { get; set; }

        public List<DB_RCS_VALUATION_DTL> D { get; set; }
        public List<DB_RCS_VALUATION_DTL> E { get; set; }
        public List<DB_RCS_VALUATION_DTL> N { get; set; }

    }

    public class SaveForm : AUTH
    {

        public string V_ID { get; set; }

        public string RECORD_DATE { get; set; }

        public List<DB_RCS_VALUATION_DTL> D { get; set; }
        public List<DB_RCS_VALUATION_DTL> E { get; set; }
        public List<DB_RCS_VALUATION_DTL> N { get; set; }

    }


    /// <summary>
    /// 計價單_主清單欄位名稱(valuation_item)
    /// </summary>
    public class RCS_VALUATION_ITEMS
    {
        /// <summary>
        /// 資料流水號
        /// </summary>
        public string V_ID { set; get; }
        /// <summary>
        /// 資料狀態(0:暫存,9:刪除,2:歷史記錄)
        /// </summary>
        public string DATASTATUS { set; get; }
        /// <summary>
        /// 資料上傳狀態(0:未上傳,1:已上傳成功)
        /// </summary>
        public string UPLOAD_STATUS { get; set; }
        /// <summary>
        /// 資料上傳狀態_文字轉換(0:未上傳,1:已上傳成功)
        /// </summary>
        public string UPLOAD_STATUS_ChangeName
        {
            get
            {
                if (string.IsNullOrWhiteSpace(this.UPLOAD_STATUS))
                {
                    return "";
                }
                if (this.UPLOAD_STATUS == "0")
                {
                    return "暫存";
                }
                if (this.UPLOAD_STATUS == "1")
                {
                    return "上傳";
                }
                return "";
            }
        }
        /// <summary>
        /// 修改日期
        /// </summary>
        public string MODIFY_DATE { set; get; }
        public string CREATE_ID { get; set; }
        public string CREATE_NAME { get; set; }
        public string CREATE_DATE { get; set; }
        public string RECORD_DATE { get; set; }
    }

    public class FormRtValuationList : AUTH
    {
        public string sDate { get; set; }
        public string eDate { get; set; }
    }

    public class FormRtValuationDetail : AUTH
    {
        public string VID { get; set; }
    }
}

using RCS.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using RCS_Data;
using Com.Mayaminer;
using Newtonsoft.Json;
using mayaminer.com.library;

namespace RCS.Controllers
{
    public class VPN_UPLOADController : BaseController
    {
        //
        // GET: /VPN_UPLOAD/


        public ActionResult list()
        {
            return View();
        }

        public ActionResult upload_index()
        {

            return View();
        }

        public ActionResult Index(string tran_id)
        {
            VPN_UPLOAD_ViewModel vm = new VPN_UPLOAD_ViewModel();
            vm.setSelectList(pat_info.ipd_no, tran_id);
            if (string.IsNullOrWhiteSpace(tran_id))
            {
                vm.tran_lsit = new List<DB_RCS_VPN_UPLOAD_TRAN>();
            }
            return View(vm);
        }

        public JsonResult saveVPN_UPLOAD(string objStr)
        {
            RESPONSE_MSG rm = new RESPONSE_MSG();
            string actionName = "savveVPN_UPLOAD";
            try
            {
                List<DB_RCS_VPN_UPLOAD_TRAN> list = new List<DB_RCS_VPN_UPLOAD_TRAN>();
                objStr = HttpUtility.UrlDecode(objStr);
                list = JsonConvert.DeserializeObject<List<DB_RCS_VPN_UPLOAD_TRAN>>(objStr);
                save_check(ref list,ref rm);
                if(rm.status != RESPONSE_STATUS.SUCCESS)
                {
                    return Json(rm);
                }
                SQLProvider SQL = new SQLProvider();
                #region 儲存資料
                List<DB_RCS_VPN_UPLOAD_TRAN> insert_list = new List<DB_RCS_VPN_UPLOAD_TRAN>();
                List<DB_RCS_VPN_UPLOAD_TRAN> update_list = new List<DB_RCS_VPN_UPLOAD_TRAN>();
                List<string> slqList = new List<string>();
                #region UPDATE_SQL
                System.Text.StringBuilder sb = new System.Text.StringBuilder();
                sb.Append("UPDATE RCS_VPN_UPLOAD_TRAN SET ");
                sb.Append(string.Concat(" TRAN_DATE =", SQL.namedArguments, "TRAN_DATE"));
                sb.Append(string.Concat(" ,PATIENT_SOURCE =", SQL.namedArguments, "PATIENT_SOURCE"));
                sb.Append(string.Concat(" ,STATION_TYPE =", SQL.namedArguments, "STATION_TYPE"));
                sb.Append(string.Concat(" ,HOSP_ID =", SQL.namedArguments, "HOSP_ID"));
                sb.Append(string.Concat(" ,HOSP_NAME =", SQL.namedArguments, "HOSP_NAME"));
                sb.Append(string.Concat(" ,MV_START_DATE =", SQL.namedArguments, "MV_START_DATE"));
                sb.Append(string.Concat(" ,MV_REASON =", SQL.namedArguments, "MV_REASON"));
                sb.Append(string.Concat(" ,TRAN_HOSP_ID =", SQL.namedArguments, "TRAN_HOSP_ID"));
                sb.Append(string.Concat(" ,TRAN_STATION_TYPE =", SQL.namedArguments, "TRAN_STATION_TYPE"));
                sb.Append(string.Concat(" ,TRAN_REASON =", SQL.namedArguments, "TRAN_REASON"));
                sb.Append(string.Concat(" ,MV_MODE =", SQL.namedArguments, "MV_MODE"));
                sb.Append(string.Concat(" ,CONSCIOUS =", SQL.namedArguments, "CONSCIOUS"));
                sb.Append(string.Concat(" ,MV =", SQL.namedArguments, "MV"));
                sb.Append(string.Concat(" ,TRAN_HOSP =", SQL.namedArguments, "TRAN_HOSP"));
                sb.Append(string.Concat(" ,TRAN_BED =", SQL.namedArguments, "TRAN_BED"));
                sb.Append(string.Concat(" ,TRAN_SITUATION =", SQL.namedArguments, "TRAN_SITUATION"));
                sb.Append(string.Concat(" ,WEANING_REMARK =", SQL.namedArguments, "WEANING_REMARK"));
                sb.Append(string.Concat(" ,WEANING_DATE =", SQL.namedArguments, "WEANING_DATE"));
                sb.Append(string.Concat(" ,MODIFY_ID =", SQL.namedArguments, "MODIFY_ID"));
                sb.Append(string.Concat(" ,MODIFY_NAME =", SQL.namedArguments, "MODIFY_NAME"));
                sb.Append(string.Concat(" ,MODIFY_DATE =", SQL.namedArguments, "MODIFY_DATE"));
                sb.Append(string.Concat(" ,UPLOAD_STATUS =", SQL.namedArguments, "UPLOAD_STATUS"));
                sb.Append(string.Concat(" WHERE TRAN_ID = ", SQL.namedArguments, "TRAN_ID"));
                sb.Append(string.Concat(" AND TRAN_TYPE = ", SQL.namedArguments, "TRAN_TYPE"));
                #endregion
                slqList.Add(sb.ToString());
                #region INSERT_SQL
                sb = new System.Text.StringBuilder();
                sb.Append("INSERT INTO RCS_VPN_UPLOAD_TRAN ( TRAN_ID");
                sb.Append(",CHART_NO");
                sb.Append(",IPD_NO");
                sb.Append(",UPLOAD_STATUS");
                sb.Append(",CREATE_ID");
                sb.Append(",CREATE_NAME");
                sb.Append(",CREATE_DATE");
                sb.Append(",DATASTATUS");
                sb.Append(",TRAN_TYPE");
                sb.Append(",ID_NO");
                sb.Append(",BIRTH_DAY");
                sb.Append(",PATIENT_NAME");
                sb.Append(",GENDER");
                sb.Append(",TRAN_DATE");
                sb.Append(",PATIENT_SOURCE");
                sb.Append(",STATION_TYPE");
                sb.Append(",HOSP_ID");
                sb.Append(",HOSP_NAME");
                sb.Append(",MV_START_DATE");
                sb.Append(",MV_REASON");
                sb.Append(",TRAN_HOSP_ID");
                sb.Append(",TRAN_STATION_TYPE");
                sb.Append(",TRAN_REASON");
                sb.Append(",MV_MODE");
                sb.Append(",CONSCIOUS");
                sb.Append(",MV");
                sb.Append(",TRAN_HOSP");
                sb.Append(",TRAN_BED");
                sb.Append(",TRAN_SITUATION");
                sb.Append(",WEANING_REMARK");
                sb.Append(",WEANING_DATE");
                sb.Append(string.Concat(" ) VALUES ( ", SQL.namedArguments, "TRAN_ID"));
                sb.Append(string.Concat(",", SQL.namedArguments, "CHART_NO"));
                sb.Append(string.Concat(",", SQL.namedArguments, "IPD_NO"));
                sb.Append(string.Concat(",", SQL.namedArguments, "UPLOAD_STATUS"));
                sb.Append(string.Concat(",", SQL.namedArguments, "CREATE_ID"));
                sb.Append(string.Concat(",", SQL.namedArguments, "CREATE_NAME"));
                sb.Append(string.Concat(",", SQL.namedArguments, "CREATE_DATE"));
                sb.Append(string.Concat(",", SQL.namedArguments, "DATASTATUS"));
                sb.Append(string.Concat(",", SQL.namedArguments, "TRAN_TYPE"));
                sb.Append(string.Concat(",", SQL.namedArguments, "ID_NO"));
                sb.Append(string.Concat(",", SQL.namedArguments, "BIRTH_DAY"));
                sb.Append(string.Concat(",", SQL.namedArguments, "PATIENT_NAME"));
                sb.Append(string.Concat(",", SQL.namedArguments, "GENDER"));
                sb.Append(string.Concat(",", SQL.namedArguments, "TRAN_DATE"));
                sb.Append(string.Concat(",", SQL.namedArguments, "PATIENT_SOURCE"));
                sb.Append(string.Concat(",", SQL.namedArguments, "STATION_TYPE"));
                sb.Append(string.Concat(",", SQL.namedArguments, "HOSP_ID"));
                sb.Append(string.Concat(",", SQL.namedArguments, "HOSP_NAME"));
                sb.Append(string.Concat(",", SQL.namedArguments, "MV_START_DATE"));
                sb.Append(string.Concat(",", SQL.namedArguments, "MV_REASON"));
                sb.Append(string.Concat(",", SQL.namedArguments, "TRAN_HOSP_ID"));
                sb.Append(string.Concat(",", SQL.namedArguments, "TRAN_STATION_TYPE"));
                sb.Append(string.Concat(",", SQL.namedArguments, "TRAN_REASON"));
                sb.Append(string.Concat(",", SQL.namedArguments, "MV_MODE"));
                sb.Append(string.Concat(",", SQL.namedArguments, "CONSCIOUS"));
                sb.Append(string.Concat(",", SQL.namedArguments, "MV"));
                sb.Append(string.Concat(",", SQL.namedArguments, "TRAN_HOSP"));
                sb.Append(string.Concat(",", SQL.namedArguments, "TRAN_BED"));
                sb.Append(string.Concat(",", SQL.namedArguments, "TRAN_SITUATION"));
                sb.Append(string.Concat(",", SQL.namedArguments, "WEANING_REMARK"));
                sb.Append(string.Concat(",", SQL.namedArguments, "WEANING_DATE"));
                sb.Append(" )");
                #endregion
                slqList.Add(sb.ToString());
                //將NULL 儲存 空值 ""
                foreach (DB_RCS_VPN_UPLOAD_TRAN item in list)
                {
                    item.PATIENT_SOURCE = item.PATIENT_SOURCE == null ? "" : item.PATIENT_SOURCE;
                    item.STATION_TYPE = item.STATION_TYPE == null ? "" : item.STATION_TYPE;
                    item.STATION_TYPE = item.STATION_TYPE == null ? "" : item.STATION_TYPE;
                    item.MV_START_DATE = item.MV_START_DATE == null ? "" : item.MV_START_DATE;
                    item.MV_REASON = item.MV_REASON == null ? "" : item.MV_REASON;
                    item.TRAN_REASON = item.TRAN_REASON == null ? "" : item.TRAN_REASON;
                    item.TRAN_HOSP = item.TRAN_HOSP == null ? "" : item.TRAN_HOSP;
                    item.TRAN_BED = item.TRAN_BED == null ? "" : item.TRAN_BED;
                    item.TRAN_SITUATION = item.TRAN_SITUATION == null ? "" : item.TRAN_SITUATION;
                    item.WEANING_REMARK = item.WEANING_REMARK == null ? "" : item.WEANING_REMARK;
                    item.WEANING_DATE = item.WEANING_DATE == null ? "" : item.WEANING_DATE;
                }
                if (list.Exists(x => !string.IsNullOrWhiteSpace(x.TRAN_ID)))
                {
                    update_list = list.FindAll(x => !string.IsNullOrWhiteSpace(x.TRAN_ID));
                    update_list.ForEach(x => {
                        x.MODIFY_DATE = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                        x.MODIFY_ID = user_info.user_id;
                        x.MODIFY_NAME = user_info.user_name;
                        x.UPLOAD_STATUS = "0";
                        x.DATASTATUS = "1";
                        x.TRAN_HOSP_ID = x.HOSP_ID;
                    });
                    //TRAN_STATION_TYPE
                    if (update_list.Exists(x => x.TRAN_TYPE == "2"))
                    {
                        update_list.FindAll(x => x.TRAN_TYPE == "2").ForEach(x => {
                            x.TRAN_STATION_TYPE = list.Find(s => s.TRAN_TYPE == "1").STATION_TYPE;
                        });
                    }
                }
                if (list.Exists(x => string.IsNullOrWhiteSpace(x.TRAN_ID)))
                {
                    insert_list = list.FindAll(x => string.IsNullOrWhiteSpace(x.TRAN_ID));
                    string TRAN_ID = "";
                    if (update_list != null && update_list.Count > 0)
                    {
                        TRAN_ID = list.Find(x => !string.IsNullOrWhiteSpace(x.TRAN_ID)).TRAN_ID;
                    }
                    else
                    {
                        TRAN_ID = DateTime.Now.ToString("yyyyMMddHHmmssfffff");
                    }
                    insert_list.ForEach(x => {
                        x.TRAN_ID = TRAN_ID;
                        x.IPD_NO = pat_info.ipd_no;
                        x.CHART_NO = pat_info.chart_no;
                        x.PATIENT_NAME = pat_info.patient_name;
                        x.GENDER = pat_info.gender;
                        x.ID_NO = pat_info.idno== null?"": pat_info.idno;
                        x.BIRTH_DAY = pat_info.birth_day;
                        x.TRAN_HOSP_ID = x.HOSP_ID;
                        x.UPLOAD_STATUS = "0";
                        x.DATASTATUS = "1";
                        x.CREATE_DATE = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                        x.CREATE_ID = user_info.user_id;
                        x.CREATE_NAME = user_info.user_name;
                    });
                    //TRAN_STATION_TYPE
                    if (insert_list.Exists(x => x.TRAN_TYPE == "2"))
                    {
                        insert_list.FindAll(x => x.TRAN_TYPE == "2").ForEach(x => {
                            x.TRAN_STATION_TYPE = list.Find(s => s.TRAN_TYPE == "1").STATION_TYPE;
                        });
                    }

                }
                SQL.DBA.BeginTrans();
                if (update_list.Count > 0)
                {
                    SQL.DBA.DBExecute<DB_RCS_VPN_UPLOAD_TRAN>(slqList[0], update_list);
                }
                if (insert_list.Count > 0)
                {
                    SQL.DBA.DBExecute<DB_RCS_VPN_UPLOAD_TRAN>(slqList[1], insert_list);
                }
                SQL.DBA.Close();
                #endregion
                rm = SQL.RESPONSE_MSG;
                if (rm.status == RESPONSE_STATUS.SUCCESS)
                {
                    rm.message = "儲存成功";
                }
                else
                {
                    LogTool.SaveLogMessage(rm.message, actionName, GetLogToolCS.VPN_UPLOADController);
                }

            }
            catch (Exception ex)
            {
                rm.status = RESPONSE_STATUS.EXCEPTION;
                rm.message = "儲存失敗，請洽資訊人員!";
                LogTool.SaveLogMessage(ex, actionName, GetLogToolCS.VPN_UPLOADController);
            }

            return Json(rm);
        }

        /// <summary>
        /// 儲存檢查資料
        /// </summary>
        /// <param name="rm"></param>
        public void save_check(ref List<DB_RCS_VPN_UPLOAD_TRAN> pList, ref RESPONSE_MSG rm)
        {
            string actionName = "save_check";
            List<string> msgList = new List<string>();

            try
            {
                if (pList.Exists(x=>x.TRAN_TYPE == "2" && !string.IsNullOrWhiteSpace( x.TRAN_DATE)) && (!pList.Exists(x => x.TRAN_TYPE == "1") || pList.Exists(x => x.TRAN_TYPE == "1" && string.IsNullOrWhiteSpace(x.TRAN_DATE))))
                {
                    msgList.Add("查無轉入(收案)資料，請先填寫!");
                }
                string TRAN_TYPE = "1";
                foreach (DB_RCS_VPN_UPLOAD_TRAN item in pList)
                {
                    TRAN_TYPE = item.TRAN_TYPE;
                    switch (TRAN_TYPE)
                    {
                        case "1":
                            //轉入資料檢查
                            string PATIENT_SOURCE = item.PATIENT_SOURCE;
                            #region MyRegion
                            //檢查STATION_TYPE
                            if ((PATIENT_SOURCE == "1" || PATIENT_SOURCE == "2") && string.IsNullOrWhiteSpace(item.STATION_TYPE))
                            {
                                msgList.Add("病患來源=\"本院\"或\"他院\"，則 \"病患來源病床類別\" 為必填");
                            }
                            if ((PATIENT_SOURCE == "3" || PATIENT_SOURCE == "4") && !string.IsNullOrWhiteSpace(item.STATION_TYPE))
                            {
                                msgList.Add("病病患來源=\"居家照護\"或\"其他\"，則 \"病患來源病床類別\" 為空白");
                            }
                            //檢查HOSP_ID
                            if (PATIENT_SOURCE == "1" && !string.IsNullOrWhiteSpace(item.HOSP_ID))
                            {
                                msgList.Add("病患來源=\"本院\"，則\"醫事機構代碼\"空白(系統自動設定病患來源醫事機構代碼=轉入醫事機構代號)");
                            }
                            if ((PATIENT_SOURCE == "2" || PATIENT_SOURCE == "3") && string.IsNullOrWhiteSpace(item.HOSP_ID))
                            {
                                msgList.Add("病患來源=\"他院\"或\"居家照護\"，則\"醫事機構代碼\" 為必填");
                            }
                            //HOSP_NAME
                            if (PATIENT_SOURCE == "4" && string.IsNullOrWhiteSpace(item.HOSP_NAME))
                            {
                                msgList.Add("病患來源=\"其他(如護理之家或養護機構)\"時，\"醫事機構名稱\" 為必填");
                            }
                            //MV_START_DATE
                            if (string.IsNullOrWhiteSpace(item.MV_START_DATE))
                            {
                                msgList.Add("\"開始使用呼吸器日期\" 為必填");
                            }
                            //使用呼吸器原因
                            if (string.IsNullOrWhiteSpace(item.MV_REASON))
                            {
                                msgList.Add("\"使用呼吸器原因\" 為必填");
                            }
                            //轉入理由 TRAN_REASON
                            if (! string.IsNullOrWhiteSpace(item.TRAN_STATION_TYPE) && item.TRAN_STATION_TYPE == "1" && string.IsNullOrWhiteSpace(item.TRAN_REASON))
                            {
                                msgList.Add("轉入醫院病床類別='ICU'時，\"轉入理由\" 為必填");
                            }
                            //MV_MODE 氣道介面
                            if (!string.IsNullOrWhiteSpace(item.MV) && item.MV == "1" && (string.IsNullOrWhiteSpace(item.MV_MODE) || (item.MV_MODE != "1" && item.MV_MODE != "2" && item.MV_MODE != "3" && item.MV_MODE != "4")))
                            {
                                msgList.Add("當呼吸器= \"使用呼吸器\"，則氣道介面只能選擇\"氣管插管\"、\"氣切插管\"、\"面罩\"或\"鼻面罩\"");
                            }
                            if (!string.IsNullOrWhiteSpace(item.MV) && item.MV == "2" && (string.IsNullOrWhiteSpace(item.MV_MODE) || (item.MV_MODE != "1" && item.MV_MODE != "2" && item.MV_MODE != "3" && item.MV_MODE != "4" && item.MV_MODE != "5")))
                            {
                                msgList.Add("當呼吸器= \"使用CPCA或Bi-PAP\"，則氣道介面只能選擇\"氣管插管\"、\"氣切插管\"、\"面罩\"、\"面罩\"或\"Nasal Prong\"");
                            }
                            if (!string.IsNullOrWhiteSpace(item.MV) && item.MV == "2" && (string.IsNullOrWhiteSpace(item.MV_MODE ) || (item.MV_MODE != "1" && item.MV_MODE != "2" && item.MV_MODE != "5" && item.MV_MODE != "6" && item.MV_MODE != "7")))
                            {
                                msgList.Add("當呼吸器= \"未使用呼吸器\"，則氣道介面只能選擇\"氣管插管\"、\"氣切插管\"、\"Nasal Prong\"、\"O2 Supply\"或\"無人工氣道\"");
                            }
                            //MV 呼吸器
                            if (string.IsNullOrWhiteSpace(item.MV))
                            {
                                msgList.Add("\"呼吸器\" 為必填");
                            }
                            //CONSCIOUS 意識形態
                            if (string.IsNullOrWhiteSpace(item.CONSCIOUS))
                            {
                                msgList.Add("\"意識形態\" 為必填");
                            }
                            #endregion
                            break;
                        case "2":
                            //轉出資料檢查
                            string TRAN_HOSP = item.TRAN_HOSP;
                            #region MyRegion
                            //檢查HOSP_ID
                            if (TRAN_HOSP == "1" && !string.IsNullOrWhiteSpace(item.HOSP_ID))
                            {
                                msgList.Add("轉出目的醫院類別=\"未脫離呼吸器\" ，則\"醫事機構代碼\"空白(系統自動設定病患來源醫事機構代碼=轉入醫事機構代號)");
                            }
                            if (TRAN_HOSP == "2" && string.IsNullOrWhiteSpace(item.HOSP_ID))
                            {
                                msgList.Add("轉出目的醫院類別=\"脫離呼吸器成功\" ，則\"醫事機構代碼\" 為必填");
                            }
                            //HOSP_NAME
                            if (TRAN_HOSP == "2" && string.IsNullOrWhiteSpace(item.HOSP_NAME))
                            {
                                msgList.Add("轉出目的醫院類別=\"脫離呼吸器成功\"時，\"醫事機構名稱\" 為必填");
                            }
                            //MV_MODE 氣道介面
                            if (!string.IsNullOrWhiteSpace(item.TRAN_SITUATION) && item.TRAN_SITUATION == "3" && !string.IsNullOrWhiteSpace(item.MV_MODE))
                            {
                                msgList.Add("當轉出時情形=\"死亡\"時， \"氣道介面\" 為空白");
                            }
                            if(!string.IsNullOrWhiteSpace(item.TRAN_SITUATION) && item.TRAN_SITUATION != "3" && string.IsNullOrWhiteSpace(item.MV_MODE))
                            {
                                msgList.Add("\"氣道介面\" 為必填");
                            }
                            //意識形態 CONSCIOUS
                            if (!string.IsNullOrWhiteSpace(item.TRAN_SITUATION) && item.TRAN_SITUATION == "3" && !string.IsNullOrWhiteSpace(item.CONSCIOUS))
                            {
                                msgList.Add("當轉出時情形=\"死亡\"時， \"意識形態\" 為空白");
                            }
                            if (!string.IsNullOrWhiteSpace(item.TRAN_SITUATION) && item.TRAN_SITUATION != "3" && string.IsNullOrWhiteSpace(item.CONSCIOUS))
                            {
                                msgList.Add("\"意識形態\" 為必填");
                            }
                            //MV 呼吸器
                            if (!string.IsNullOrWhiteSpace(item.TRAN_SITUATION) && item.TRAN_SITUATION == "3" && !string.IsNullOrWhiteSpace(item.MV))
                            {
                                msgList.Add("當轉出時情形=\"死亡\"時， \"呼吸器\" 為空白");
                            }
                            if (!string.IsNullOrWhiteSpace(item.TRAN_SITUATION) && item.TRAN_SITUATION != "3" && string.IsNullOrWhiteSpace(item.MV))
                            {
                                msgList.Add("\"呼吸器\" 為必填");
                            }
                            if (!string.IsNullOrWhiteSpace(item.TRAN_SITUATION) && (item.TRAN_SITUATION == "2" || item.TRAN_SITUATION == "6") && (string.IsNullOrWhiteSpace(item.MV) || item.MV  != "3"))
                            {
                                msgList.Add("當轉出時情形=\"脫離呼吸器成功\"或\"嘗試脫離呼吸器中\"時， \"呼吸器\" 只能填 \"未使用呼吸器\"");
                            }
                            //TRAN_HOSP 轉出目的醫院類別
                            if (!string.IsNullOrWhiteSpace(item.TRAN_SITUATION) && (item.TRAN_SITUATION == "1" || item.TRAN_SITUATION == "2" || item.TRAN_SITUATION == "4" || item.TRAN_SITUATION == "5" || item.TRAN_SITUATION == "6") && string.IsNullOrWhiteSpace(item.TRAN_HOSP))
                            {
                                msgList.Add("當轉出時情形=\"未脫離呼吸器\"\"脫離呼吸器成功\"、\"一般自動出院\"、\"病危自動出院\"或\"嘗試脫離呼吸器中\"時， \"轉出目的醫院類別\" 為必填");
                            }
                            if (!string.IsNullOrWhiteSpace(item.TRAN_SITUATION) && (item.TRAN_SITUATION == "3") && !string.IsNullOrWhiteSpace(item.TRAN_HOSP))
                            {
                                msgList.Add("當轉出時情形=\"死亡\"時， \"轉出目的醫院類別\" 為空白");
                            }
                            //TRAN_BED 轉出目的病床類別
                            if (!string.IsNullOrWhiteSpace(item.TRAN_HOSP) && (item.TRAN_HOSP == "1" || item.TRAN_HOSP == "2" ) && string.IsNullOrWhiteSpace(item.TRAN_BED))
                            {
                                msgList.Add("當轉出目的醫院類別=\"本院\"或\"他院\"時， \"轉出目的病床類別\" 為必填");
                            }
                            if (!string.IsNullOrWhiteSpace(item.TRAN_SITUATION) && (item.TRAN_SITUATION == "1" || item.TRAN_SITUATION == "6") && string.IsNullOrWhiteSpace(item.TRAN_BED))
                            {
                                msgList.Add("當轉轉出時情形=\"未脫離呼吸器\"或\"嘗試脫離呼吸器中\"時， \"轉出目的病床類別\" 為必填");
                            }
                            if (!string.IsNullOrWhiteSpace(item.TRAN_SITUATION) && item.TRAN_SITUATION == "2" && (string.IsNullOrWhiteSpace(item.TRAN_BED) || item.TRAN_BED == "2" || item.TRAN_BED == "3"))
                            {
                                msgList.Add("當轉轉出時情形=\"脫離呼吸器成功\"時， \"轉出目的病床類別\" 為必填，但不可填 \"RCC\"、\"RCW\"");
                            }
                            if (!string.IsNullOrWhiteSpace(item.TRAN_SITUATION) && (item.TRAN_SITUATION == "3"|| item.TRAN_SITUATION == "4" || item.TRAN_SITUATION == "5") && !string.IsNullOrWhiteSpace(item.TRAN_BED))
                            {
                                msgList.Add("當轉出時情形=\"死亡\"、\"一般自動出院\"或\"病危自動出院\"時， \"轉出目的病床類別\" 為空白");
                            }
                            //WEANING_REMARK 脫離呼吸器但因病況需要繼續留住原病房註記
                            if (!string.IsNullOrWhiteSpace(item.WEANING_REMARK) && item.WEANING_REMARK == "Y" &&(string.IsNullOrWhiteSpace(item.TRAN_SITUATION) || item.TRAN_SITUATION != "2"))
                            {
                                msgList.Add("脫離呼吸器但因病況需要繼續留住原病房註記，則轉出時情形只能選擇 \"脫離呼吸器成功\"");
                            }

                            //WEANING_DATE 脫離呼吸器成功/嘗試脫離呼吸器中日期
                            if (!string.IsNullOrWhiteSpace(item.TRAN_SITUATION) && (item.TRAN_SITUATION == "2"|| item.TRAN_SITUATION == "6") && string.IsNullOrWhiteSpace(item.WEANING_DATE))
                            {
                                msgList.Add("當轉轉出時情形= \"脫離呼吸器成功\" 或 \"嘗試脫離呼吸器中\" 時， \"脫離/嘗試脫離呼吸器日期\" 為必填");
                            }
                            #endregion
                            break;
                        default:
                            break;
                    }
                }
                if (msgList.Count > 0)
                {
                    rm.status = RESPONSE_STATUS.ERROR;
                    rm.message = string.Join("<br/>", msgList);
                }
            }
            catch (Exception ex)
            {
                rm.status = RESPONSE_STATUS.EXCEPTION;
                rm.message = "儲存失敗，請洽資訊人員!";
                LogTool.SaveLogMessage(ex, actionName, GetLogToolCS.VPN_UPLOADController);
            }
        }

        /// <summary>
        /// 取的上傳轉入轉出清單
        /// </summary>
        /// <returns></returns>
        public JsonResult getXMLVPN_UPLOAD(string ipd_no,bool isAllData)
        {
            RESPONSE_MSG rm = new RESPONSE_MSG();
            List<Tran_Vpn_Tbl> pList = new List<Tran_Vpn_Tbl>();
            string actionName = "getXMLVPN_UPLOAD";
            try
            {
                SQLProvider SQL = new SQLProvider();
                VPN_UPLOAD_ViewModel vm = new VPN_UPLOAD_ViewModel();
                vm.setSelectList(ipd_no);
                if (!isAllData)
                {
                    vm.tran_lsit = vm.tran_lsit.FindAll(x => x.DATASTATUS == "1");
                }
                //selectList
                List<string> TRAN_IDStr = vm.tran_lsit.Select(x => x.TRAN_ID).Distinct().ToList();
                foreach (string TRAN_ID in TRAN_IDStr)
                {
                    Tran_Vpn_Tbl tran_item = new Tran_Vpn_Tbl();
                    List<DB_RCS_VPN_UPLOAD_TRAN> temp = vm.tran_lsit.FindAll(x=>x.TRAN_ID == TRAN_ID);
                    foreach (DB_RCS_VPN_UPLOAD_TRAN item in temp)
                    {
                        switch (item.TRAN_TYPE)
                        {
                            case "1":
                                tran_item.TRAN_ID = item.TRAN_ID;
                                tran_item.PATIENT_NAME = item.PATIENT_NAME;
                                tran_item.CHART_NO = item.CHART_NO;
                                tran_item.gender = item.GENDER;
                                if ( string.IsNullOrWhiteSpace(tran_item.gender) || tran_item.gender != "1")
                                {
                                    tran_item.gender = "0";
                                }
                                tran_item.TRAN_DATE = item.TRAN_DATE;
                                tran_item.MV_START_DATE = item.MV_START_DATE;
                                tran_item.UPLOAD_STATUS = item.UPLOAD_STATUS;
                                tran_item.TRAN_TYPE = "轉入";
                                //STATION_TYPE
                                tran_item.TRAN_STATION_TYPE = item.TRAN_STATION_TYPE;
                                if (!string.IsNullOrWhiteSpace(tran_item.TRAN_STATION_TYPE))
                                {
                                    tran_item.TRAN_STATION_TYPE = vm.selectList.Find(x => x.P_GROUP == "STATION_TYPE" && x.P_VALUE == tran_item.TRAN_STATION_TYPE).P_NAME;
                                }
                                   
                                tran_item.HOSP_ID = item.HOSP_ID;
                                tran_item.HOSP_NAME = item.HOSP_NAME;
                                if(!string.IsNullOrWhiteSpace(item.PATIENT_SOURCE) &&  item.PATIENT_SOURCE == "1" && string.IsNullOrWhiteSpace(tran_item.HOSP_NAME))
                                {
                                    tran_item.HOSP_NAME = "本院";
                                }
                                break;
                            case "2":
                                tran_item.TRAN_OUT_ID = item.TRAN_ID;
                                tran_item.TRAN_OUT = item.TRAN_DATE;
                                tran_item.TRAN_SITUATION = item.TRAN_SITUATION;
                                if (!string.IsNullOrWhiteSpace(tran_item.TRAN_SITUATION))
                                {
                                    tran_item.TRAN_SITUATION = vm.selectList.Find(x => x.P_GROUP == "TRAN_SITUATION" && x.P_VALUE == tran_item.TRAN_SITUATION).P_NAME;
                                }
                                tran_item.OUT_UPLOAD_STATUS = item.UPLOAD_STATUS;
                                tran_item.OUT_TRAN_TYPE = "轉出";
                                tran_item.OUT_HOSP_ID = item.HOSP_ID;
                                tran_item.OUT_HOSP_NAME = item.HOSP_NAME;
                                if (!string.IsNullOrWhiteSpace(item.TRAN_HOSP) && item.TRAN_HOSP == "1" && string.IsNullOrWhiteSpace(tran_item.OUT_HOSP_NAME))
                                {
                                    tran_item.OUT_HOSP_NAME = "本院";
                                }
                                //TRAN_BED
                                tran_item.TRAN_BED = item.TRAN_BED;
                                if(!string.IsNullOrWhiteSpace(tran_item.TRAN_BED))
                                {
                                    tran_item.TRAN_BED = vm.selectList.Find(x => x.P_GROUP == "TRAN_BED" && x.P_VALUE == tran_item.TRAN_BED).P_NAME;
                                }
                                break;
                            default:
                                break;
                        }
                    }
                    pList.Add(tran_item);
                }
            }
            catch (Exception ex)
            {
                rm.status = RESPONSE_STATUS.EXCEPTION;
                rm.message = "產生失敗，請洽資訊人員!";
                LogTool.SaveLogMessage(ex, actionName, GetLogToolCS.VPN_UPLOADController);
            }
            return Json(pList);
        }

        /// <summary>
        /// 匯出XML檔案
        /// </summary>
        /// <param name="vpn_uploadJsonData">列印清單</param>
        /// <returns></returns>
        public ActionResult exportVPN_XML(string vpn_uploadJsonData,string file_no)
        {
            string actionName = "exportVPN_XML";
            RESPONSE_MSG rm = new RESPONSE_MSG();
            List<Tran_Vpn_Tbl> pList = new List<Tran_Vpn_Tbl>();
            List<string> idList = new List<string>();
            List<qvtrec> qvtrecList = new List<qvtrec>();
            try
            {
                SQLProvider SQL = new SQLProvider();
                vpn_uploadJsonData = HttpUtility.UrlDecode(vpn_uploadJsonData);
                pList = JsonConvert.DeserializeObject<List<Tran_Vpn_Tbl>>(vpn_uploadJsonData);
                //如果有資料
                if (pList.Count > 0)
                {
                    //如果有轉入ID的話
                    if(pList.Exists(x=>!string.IsNullOrWhiteSpace(x.TRAN_ID)))
                    {
                        idList.AddRange(pList.FindAll(x => !string.IsNullOrWhiteSpace(x.TRAN_ID)).Select(x=>x.TRAN_ID).ToList());
                    }
                    //如果有轉出ID的話
                    if (pList.Exists(x => !string.IsNullOrWhiteSpace(x.TRAN_OUT_ID)))
                    {
                        idList.AddRange(pList.FindAll(x => !string.IsNullOrWhiteSpace(x.TRAN_OUT_ID)).Select(x => x.TRAN_OUT_ID).ToList());
                    }
                }
                //如果有ID的清單
                if (idList.Count > 0)
                {
                    string sql = string.Concat("SELECT ROW_NUMBER() OVER(ORDER BY TRAN_ID) AS TRAN_NO,",
                        " ISNULL(CHART_NO,'') CHART_NO,ISNULL(IPD_NO,'') IPD_NO,ISNULL(TRAN_NO,'') TRAN_NO,ISNULL(TRAN_TYPE,'') TRAN_TYPE,",
                        " ISNULL(ID_NO,'') ID_NO, ISNULL(BIRTH_DAY,'') BIRTH_DAY, ISNULL(PATIENT_NAME,'') PATIENT_NAME, ISNULL(GENDER,'') GENDER, ISNULL(TRAN_DATE,'') TRAN_DATE,",
                        " ISNULL(PATIENT_SOURCE,'') PATIENT_SOURCE, ISNULL(STATION_TYPE,'') STATION_TYPE, ISNULL(HOSP_ID,'') HOSP_ID, ISNULL(HOSP_NAME,'') HOSP_NAME,",
                        " ISNULL(MV_START_DATE,'') MV_START_DATE, ISNULL(MV_REASON,'') MV_REASON, ISNULL(TRAN_HOSP_ID,'') TRAN_HOSP_ID,",
                        " ISNULL(TRAN_STATION_TYPE,'') TRAN_STATION_TYPE, ISNULL(TRAN_REASON,'') TRAN_REASON, ISNULL(MV_MODE,'') MV_MODE,",
                        " ISNULL(CONSCIOUS,'') CONSCIOUS, ISNULL(MV,'') MV, ISNULL(TRAN_HOSP,'') TRAN_HOSP, ISNULL(TRAN_BED,'') TRAN_BED, ISNULL(TRAN_SITUATION,'') TRAN_SITUATION,",
                        " ISNULL(WEANING_REMARK,'') WEANING_REMARK, ISNULL(WEANING_DATE,'') WEANING_DATE",
                        " FROM RCS_VPN_UPLOAD_TRAN WHERE TRAN_ID in('", string.Join("','", idList),"')");
                    qvtrecList = SQL.DBA.getSqlDataTable<qvtrec>(sql);
                }

                if (qvtrecList != null && qvtrecList.Count > 0)
                {
                    qvtrecList.ForEach(x => {
                        x.BIRTH_DAY = !string.IsNullOrWhiteSpace(x.BIRTH_DAY)?DateHelper.Parse(x.BIRTH_DAY).AddYears(-1911).ToString("yyyMMdd") :"";
                        x.TRAN_DATE = !string.IsNullOrWhiteSpace(x.TRAN_DATE) ? DateHelper.Parse(x.TRAN_DATE).AddYears(-1911).ToString("yyyMMdd") : "";
                        x.MV_START_DATE = !string.IsNullOrWhiteSpace(x.MV_START_DATE) ? DateHelper.Parse(x.MV_START_DATE).AddYears(-1911).ToString("yyyMMdd") : "";
                        x.WEANING_DATE = !string.IsNullOrWhiteSpace(x.WEANING_DATE) ? x.TRAN_SITUATION == "2"? DateHelper.Parse(x.WEANING_DATE).AddYears(-1911).ToString("yyyMMdd"): x.TRAN_SITUATION == "6" ? DateHelper.Parse(x.WEANING_DATE).AddYears(-1911).ToString("yyyMMddHHmm") : "" : "";
                    });
                    //若檔案資料格式為XML，則檔名為「QVT醫事機構代碼-系統日期流水號.xml QVT3501200000-1011207001.xml
                    int int_no = 1;
                    if (!string.IsNullOrWhiteSpace(file_no))
                    {
                        int.TryParse(file_no, out int_no);
                    }
                    string no1 = DateTime.Now.AddYears(-1911).ToString("yyyMMdd") + int_no.ToString("000");
                    string fileName = string.Concat("QVT", user_info.hosp_id,"-", no1, ".xml"); 
                    exportFile exportFile = new exportFile(fileName);
                    XMLSetting xs = new XMLSetting();
                    xs.XmlRootAttributeName = "qvtfile";
                    xs.Encoding = System.Text.Encoding.GetEncoding("Big5");
                    xs.isIndent = true;
                    xs.XmlSerializerNamespaces = new System.Xml.Serialization.XmlSerializerNamespaces();
                    xs.XmlSerializerNamespaces.Add("", "");
                    byte[] buf = exportFile.exportXML<qvtrec>(qvtrecList, xs);
                    if(exportFile.RESPONSE_MSG.status == RESPONSE_STATUS.SUCCESS)
                    {
                        string sql = "";
                        #region 更新註記
                        #region UPDATE_SQL
                        System.Text.StringBuilder sb = new System.Text.StringBuilder();
                        sb.Append("UPDATE RCS_VPN_UPLOAD_TRAN SET ");
                        sb.Append(string.Concat(" MODIFY_ID =", SQL.namedArguments, "MODIFY_ID"));
                        sb.Append(string.Concat(" ,MODIFY_NAME =", SQL.namedArguments, "MODIFY_NAME"));
                        sb.Append(string.Concat(" ,MODIFY_DATE =", SQL.namedArguments, "MODIFY_DATE"));
                        sb.Append(string.Concat(" ,UPLOAD_STATUS =", SQL.namedArguments, "UPLOAD_STATUS"));
                        sb.Append(string.Concat(" ,UPLOAD_ID =", SQL.namedArguments, "UPLOAD_ID"));
                        sb.Append(string.Concat(" ,UPLOAD_NAME =", SQL.namedArguments, "UPLOAD_NAME"));
                        sb.Append(string.Concat(" ,UPLOAD_DATE =", SQL.namedArguments, "UPLOAD_DATE"));
                        sb.Append(string.Concat(" WHERE TRAN_ID IN ('", string.Join("','", idList), "')"));
                        #endregion
                        sql = sb.ToString();
                        Dapper.DynamicParameters dp = new Dapper.DynamicParameters();
                        dp.Add("TRAN_ID", string.Join("','", idList));
                        dp.Add("MODIFY_ID", user_info.user_id);
                        dp.Add("MODIFY_NAME", user_info.user_name);
                        dp.Add("MODIFY_DATE", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
                        dp.Add("UPLOAD_STATUS", "1");
                        dp.Add("UPLOAD_ID", user_info.user_id);
                        dp.Add("UPLOAD_NAME", user_info.user_name);
                        dp.Add("UPLOAD_DATE", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
                        #endregion
                        SQL.DBA.DBExecute(sql, dp);
                        if (SQL.DBA.hasLastError)
                        {
                            rm.status = RESPONSE_STATUS.ERROR;
                            rm.message = SQL.DBA.lastError;
                            LogTool.SaveLogMessage(SQL.DBA.lastError, actionName, GetLogToolCS.VPN_UPLOADController);
                        }
                        else
                        {
                            return exportFile.retunFile(buf);
                        }
                    }
                    else
                    {
                        rm = exportFile.RESPONSE_MSG;
                    }
                }
            }
            catch (Exception ex)
            {
                rm.status = RESPONSE_STATUS.EXCEPTION;
                rm.message = ex.Message;
                LogTool.SaveLogMessage(ex, actionName, GetLogToolCS.VPN_UPLOADController);
            }
            Response.Write("<h2>產生VPN檔案，程式發生錯誤，請洽資訊人員!</h2>");
            return new EmptyResult();
        }

        /// <summary>
        /// 設定轉入轉出資料的註記
        /// </summary>
        /// <param name="chekcedData"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public JsonResult set_vpn_data_frag(string chekcedData,string type)
        {
            string actionName = "set_vpn_data_frag";
            RESPONSE_MSG rm = new RESPONSE_MSG();
            List<string> pList = new List<string>();
            try
            {
                chekcedData = HttpUtility.UrlDecode(chekcedData);
                pList = JsonConvert.DeserializeObject<List<Tran_Vpn_Tbl>>(chekcedData).Select(x=>x.TRAN_ID).Distinct().ToList();
                if (pList.Count > 0)
                {
                    SQLProvider SQL = new SQLProvider();
                    string sql = string.Concat("UPDATE RCS_VPN_UPLOAD_TRAN SET DATASTATUS = ",SQL.namedArguments, "DATASTATUS WHERE TRAN_ID in('", string.Join("','",pList),"')");
                    Dapper.DynamicParameters dp = new Dapper.DynamicParameters();
                    dp.Add("DATASTATUS", type);
                    SQL.DBA.DBExecute(sql, dp);
                    if (SQL.DBA.hasLastError)
                    {
                        rm.status = RESPONSE_STATUS.ERROR;
                        rm.message = "程式發生錯誤，請洽資訊人員!";
                        LogTool.SaveLogMessage(SQL.DBA.lastError, actionName, GetLogToolCS.VPN_UPLOADController);
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
                    rm.message = "請選擇一筆資料!";
                }


            }
            catch (Exception ex)
            {
                rm.status = RESPONSE_STATUS.EXCEPTION;
                rm.message = "程式發生錯誤，請洽資訊人員!";
                LogTool.SaveLogMessage(ex, actionName, GetLogToolCS.VPN_UPLOADController);
            }

            return Json(rm);
        }

        /// <summary>
        /// 取得醫事機構代碼及名稱
        /// </summary>
        /// <returns></returns>
        public string getTran_hospData()
        {
            List<SysParams> pList = new List<SysParams>();
            try
            {
                //getTran_hospData
                SQLProvider SQL = new SQLProvider();
                string sql = "SELECT HOSP_ID P_VALUE,HOSP_NAME P_NAME FROM RCS_SYS_IPR_CTI";
                pList = SQL.DBA.getSqlDataTable<SysParams>(sql,null);
            }
            catch (Exception ex)
            {

                throw;
            }
            return JsonConvert.SerializeObject(pList);
        }
    }
}

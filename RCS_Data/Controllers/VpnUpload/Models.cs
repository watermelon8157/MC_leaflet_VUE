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
using System.Text;

namespace RCS_Data.Controllers.VpnUpload
{
    public class Models : BaseModels, Interface
    {
        string csName { get { return "RtCptRecord Model"; } }
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

        /// <summary>
        /// 呼吸治療評估單LIST
        /// </summary>
        /// <param name="cpt_dtl_new_items"></param>
        /// <param name="form"></param>
        /// <returns></returns>
        public RESPONSE_MSG getXMLVPN_UPLOAD(ref List<VpnUploadData> list, bool isAllData, bool isUPLOADData, bool isExportFun,bool isIpdno, IPDPatientInfo pat_info)
        {
            RESPONSE_MSG rm = new RESPONSE_MSG();



            var tran_lsit = new List<DB_RCS_VPN_UPLOAD_TRAN>();

            if (isIpdno)
            {
                tran_lsit = setSelectList(pat_info.ipd_no, "", isExportFun);
            }
            else {
                tran_lsit = setSelectList(null, "", isExportFun);
            }

            List<DB_RCS_VPN_UPLOAD_TRAN> tempList = new List<DB_RCS_VPN_UPLOAD_TRAN>();
            tempList = tran_lsit.FindAll(x => x.DATASTATUS == "1");
            if (isAllData)
            {
                //勾選顯示忽略資料 
                tempList.AddRange(tran_lsit.FindAll(x => x.DATASTATUS == "3"));
            }
            if (isUPLOADData)
            {
                //勾選顯示已上傳資料
                tempList.AddRange(tran_lsit.FindAll(x => x.DATASTATUS == "2"));
            }

            var getSysParams = getRCS_SYS_PARAMS(pModel: "VPN_UPLOAD");

            //selectList
            List<string> TRAN_IDStr = tempList.Select(x => x.TRAN_ID).Distinct().ToList();
            foreach (string TRAN_ID in TRAN_IDStr)
            {
                VpnUploadData tran_item = new VpnUploadData();
                List<DB_RCS_VPN_UPLOAD_TRAN> temp = tempList.FindAll(x => x.TRAN_ID == TRAN_ID);
                foreach (DB_RCS_VPN_UPLOAD_TRAN item in temp)
                {
                    if (!isIpdno)
                    {
                        tran_item = new VpnUploadData();
                    }

                    /*2018.10.01---DATASTATUS狀態*/
                    tran_item.DATASTATUS = item.DATASTATUS;
                    if (!string.IsNullOrWhiteSpace(item.DATASTATUS) && item.DATASTATUS == "1")
                    {
                        tran_item.TRAN_STATUS = "未上傳";
                    }
                    else if (!string.IsNullOrWhiteSpace(item.DATASTATUS) && item.DATASTATUS == "2")
                    {
                        tran_item.TRAN_STATUS = "已上傳";
                    }
                    else
                    {
                        tran_item.TRAN_STATUS = "忽略";
                    }
                    /*2018.10.01---DATASTATUS狀態*/
                    tran_item.TRAN_TYPE = item.TRAN_TYPE;
                    tran_item.PATIENT_NAME = item.PATIENT_NAME;
                    tran_item.CHART_NO = item.CHART_NO;
                    tran_item.gender = item.GENDER;
                    if (string.IsNullOrWhiteSpace(tran_item.gender) || tran_item.gender != "1")
                    {
                        tran_item.gender = "0";
                    }

                    switch (item.TRAN_TYPE)
                    {
                        case "1":
                            tran_item.TRAN_ID = item.TRAN_ID;
                            tran_item.PATIENT_NAME = item.PATIENT_NAME;
                            tran_item.TRAN_DATE = item.TRAN_DATE;
                            tran_item.MV_START_DATE = item.MV_START_DATE;
                            tran_item.CHART_NO = item.CHART_NO;
                            tran_item.gender = item.GENDER;
                            if (string.IsNullOrWhiteSpace(tran_item.gender) || tran_item.gender != "1")
                            {
                                tran_item.gender = "0";
                            }
                            tran_item.TRAN_DATE = item.TRAN_DATE;
                            tran_item.MV_START_DATE = item.MV_START_DATE;
                            tran_item.UPLOAD_STATUS = item.UPLOAD_STATUS;


                            //STATION_TYPE
                            tran_item.TRAN_STATION_TYPE = item.TRAN_STATION_TYPE;
                            if (!string.IsNullOrWhiteSpace(tran_item.TRAN_STATION_TYPE))
                            {
                                tran_item.TRAN_STATION_TYPE = getSysParams.Find(x => x.P_GROUP == "TRAN_STATION_TYPE" && x.P_VALUE == tran_item.TRAN_STATION_TYPE).P_NAME;
                            }

                            tran_item.HOSP_ID = item.HOSP_ID;
                            tran_item.HOSP_NAME = item.HOSP_NAME;
                            if (!string.IsNullOrWhiteSpace(item.PATIENT_SOURCE) && item.PATIENT_SOURCE == "1" && string.IsNullOrWhiteSpace(tran_item.HOSP_NAME))
                            {
                                tran_item.HOSP_NAME = "本院";
                            }
                            break;
                        case "2":
                            tran_item.TRAN_OUT_ID = item.TRAN_ID;
                            if (isExportFun)
                                tran_item.TRAN_ID = item.TRAN_ID;
                            tran_item.TRAN_OUT = item.TRAN_DATE;
                            tran_item.TRAN_SITUATION = item.TRAN_SITUATION;
                            if (!string.IsNullOrWhiteSpace(tran_item.TRAN_SITUATION))
                            {
                                tran_item.TRAN_SITUATION = getSysParams.Find(x => x.P_GROUP == "TRAN_SITUATION" && x.P_VALUE == tran_item.TRAN_SITUATION).P_NAME;
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
                            if (!string.IsNullOrWhiteSpace(tran_item.TRAN_BED))
                            {
                                tran_item.TRAN_BED = getSysParams.Find(x => x.P_GROUP == "TRAN_BED" && x.P_VALUE == tran_item.TRAN_BED).P_NAME;
                            }
                            break;
                        default:
                            break;
                    }
                    if (!isIpdno)
                    {
                        list.Add(tran_item);
                    }
                }
                if (isIpdno)
                {
                    list.Add(tran_item);
                }

            }

            return rm;
        }

        public List<DB_RCS_SYS_PARAMS> getRCS_SYS_PARAMS()
        {

            return getRCS_SYS_PARAMS(pModel: "VPN_UPLOAD");
        }

        public VpnUploadData getDetailByID(IPDPatientInfo pat_info, string Id, bool isExportFun = false)
        {

            var tran_lsit = setSelectList(pat_info.ipd_no, Id);

            VpnUploadData tran_item = new VpnUploadData();

            var getSysParams = getRCS_SYS_PARAMS(pModel: "VPN_UPLOAD");

            foreach (DB_RCS_VPN_UPLOAD_TRAN item in tran_lsit)
            {
                if (string.IsNullOrWhiteSpace(pat_info.ipd_no))
                {
                    tran_item = new VpnUploadData();
                }

                /*2018.10.01---DATASTATUS狀態*/
                tran_item.DATASTATUS = item.DATASTATUS;
                if (!string.IsNullOrWhiteSpace(item.DATASTATUS) && item.DATASTATUS == "1")
                {
                    tran_item.TRAN_STATUS = "未上傳";
                }
                else if (!string.IsNullOrWhiteSpace(item.DATASTATUS) && item.DATASTATUS == "2")
                {
                    tran_item.TRAN_STATUS = "已上傳";
                }
                else
                {
                    tran_item.TRAN_STATUS = "忽略";
                }
                /*2018.10.01---DATASTATUS狀態*/
                tran_item.TRAN_TYPE = item.TRAN_TYPE;
                tran_item.PATIENT_NAME = item.PATIENT_NAME;
                tran_item.CHART_NO = item.CHART_NO;
                tran_item.gender = item.GENDER;
                if (string.IsNullOrWhiteSpace(tran_item.gender) || tran_item.gender != "1")
                {
                    tran_item.gender = "0";
                }

                switch (item.TRAN_TYPE)
                {
                    case "1":
                        tran_item.TRAN_ID = item.TRAN_ID;
                        tran_item.PATIENT_NAME = item.PATIENT_NAME;
                        tran_item.TRAN_DATE = item.TRAN_DATE;
                        tran_item.MV_START_DATE = item.MV_START_DATE;
                        tran_item.CHART_NO = item.CHART_NO;
                        tran_item.gender = item.GENDER;
                        if (string.IsNullOrWhiteSpace(tran_item.gender) || tran_item.gender != "1")
                        {
                            tran_item.gender = "0";
                        }
                        tran_item.TRAN_DATE = item.TRAN_DATE;
                        tran_item.MV_START_DATE = item.MV_START_DATE;
                        tran_item.UPLOAD_STATUS = item.UPLOAD_STATUS;
                        tran_item.MV = item.MV;
                        tran_item.MV_MODE = item.MV_MODE;
                        tran_item.CONSCIOUS = item.CONSCIOUS;

                        //STATION_TYPE
                        tran_item.TRAN_STATION_TYPE = item.TRAN_STATION_TYPE;
                        if (!string.IsNullOrWhiteSpace(tran_item.TRAN_STATION_TYPE))
                        {
                            tran_item.TRAN_STATION_TYPE = getSysParams.Find(x => x.P_GROUP == "TRAN_STATION_TYPE" && x.P_VALUE == tran_item.TRAN_STATION_TYPE).P_NAME;
                        }

                        tran_item.HOSP_ID = item.HOSP_ID;
                        tran_item.HOSP_NAME = item.HOSP_NAME;
                        if (!string.IsNullOrWhiteSpace(item.PATIENT_SOURCE) && item.PATIENT_SOURCE == "1" && string.IsNullOrWhiteSpace(tran_item.HOSP_NAME))
                        {
                            //tran_item.HOSP_NAME = "本院";
                        }
                        tran_item.PATIENT_SOURCE = item.PATIENT_SOURCE;
                        tran_item.STATION_TYPE = item.STATION_TYPE;
                        tran_item.MV_REASON = item.MV_REASON;
                        tran_item.TRAN_STATION_TYPE = item.TRAN_STATION_TYPE;
                        tran_item.TRAN_REASON = item.TRAN_REASON;
                        break;
                    case "2":
                        tran_item.TRAN_OUT_ID = item.TRAN_ID;
                        if (isExportFun)
                            tran_item.TRAN_ID = item.TRAN_ID;
                        tran_item.TRAN_OUT = item.TRAN_DATE;
                        tran_item.TRAN_SITUATION = item.TRAN_SITUATION;
                        tran_item.WEANING_DATE = item.WEANING_DATE;
                        tran_item.TRAN_HOSP = item.TRAN_HOSP;
                        //if (!string.IsNullOrWhiteSpace(tran_item.TRAN_SITUATION))
                        //{
                        //    tran_item.TRAN_SITUATION = getSysParams.Find(x => x.P_GROUP == "TRAN_SITUATION" && x.P_VALUE == tran_item.TRAN_SITUATION).P_NAME;
                        //}
                        tran_item.OUT_UPLOAD_STATUS = item.UPLOAD_STATUS;
                        tran_item.OUT_TRAN_TYPE = "轉出";
                        tran_item.OUT_HOSP_ID = item.HOSP_ID;
                        tran_item.OUT_HOSP_NAME = item.HOSP_NAME;
                        tran_item.OUT_MV = item.MV;
                        tran_item.OUT_MV_MODE = item.MV_MODE;
                        tran_item.OUT_CONSCIOUS = item.CONSCIOUS;
                        tran_item.WEANING_REMARK = item.WEANING_REMARK;
                        if (!string.IsNullOrWhiteSpace(item.TRAN_HOSP) && item.TRAN_HOSP == "1" && string.IsNullOrWhiteSpace(tran_item.OUT_HOSP_NAME))
                        {
                            //tran_item.OUT_HOSP_NAME = "本院";
                        }
                        //TRAN_BED
                        tran_item.TRAN_BED = item.TRAN_BED;
                        //if (!string.IsNullOrWhiteSpace(tran_item.TRAN_BED))
                        //{
                        //    tran_item.TRAN_BED = getSysParams.Find(x => x.P_GROUP == "TRAN_BED" && x.P_VALUE == tran_item.TRAN_BED).P_NAME;
                        //}
                        break;
                    default:
                        break;
                }
            }
            return tran_item;
        }

        /// <summary>
        /// 畫面基本設定
        /// </summary>
        /// <param name="ipd_no"></param>
        public List<DB_RCS_VPN_UPLOAD_TRAN> setSelectList(string ipd_no, string tran_id = "", bool isExportFun = false)
        {
            SQLProvider SQL = new SQLProvider();
            List<DB_RCS_VPN_UPLOAD_TRAN> temp = new List<DB_RCS_VPN_UPLOAD_TRAN>();
            Dapper.DynamicParameters dp = new Dapper.DynamicParameters();
            string actionName = "setSelectList";
            var RESPONSE_MSG = new RESPONSE_MSG();
            try
            {
                string sql = "SELECT * FROM RCS_VPN_UPLOAD_TRAN " + string.Concat(" WHERE IPD_NO = ", SQL.namedArguments, "IPD_NO");
                dp.Add("IPD_NO", ipd_no);


                if (!string.IsNullOrWhiteSpace(ipd_no))
                {
                    sql = "SELECT * FROM RCS_VPN_UPLOAD_TRAN " + string.Concat(" WHERE IPD_NO = ", SQL.namedArguments, "IPD_NO");
                    dp.Add("IPD_NO", ipd_no);
                }
                else
                {
                    if (isExportFun)
                    { 
                        sql = string.Concat("SELECT TOP 500 * FROM RCS_VPN_UPLOAD_TRAN WHERE DATASTATUS in('1','3','2')  ORDER BY TRAN_DATE DESC");
                    }
                    else
                    {
                        sql = string.Concat("SELECT TOP 500 * FROM RCS_VPN_UPLOAD_TRAN WHERE DATASTATUS in('1','3','2')  ORDER BY TRAN_DATE DESC");
                    }
                }
                if (!string.IsNullOrWhiteSpace(tran_id))
                {
                    sql += string.Concat(" AND TRAN_ID = ", SQL.namedArguments, "TRAN_ID");
                    dp.Add("TRAN_ID", tran_id);
                }


                temp = SQL.DBA.getSqlDataTable<DB_RCS_VPN_UPLOAD_TRAN>(sql, dp);


                if (SQL.DBA.hasLastError)
                {
                    RESPONSE_MSG.status = RESPONSE_STATUS.ERROR;
                    RESPONSE_MSG.message = SQL.DBA.lastError;
                }


                //設定轉入轉出畫面勾選資料
                if (temp.Exists(x => !string.IsNullOrWhiteSpace(x.WEANING_REMARK) && x.WEANING_REMARK == "Y"))
                {
                    temp.Find(x => !string.IsNullOrWhiteSpace(x.WEANING_REMARK) && x.WEANING_REMARK == "Y").is_WEANING_REMARK = new List<JSON_DATA>() { new JSON_DATA() { id = "WEANING_REMARK", val = "Y", chkd = true } };
                }

            }
            catch (Exception ex)
            {
                RESPONSE_MSG.status = RESPONSE_STATUS.EXCEPTION;
                RESPONSE_MSG.message = ex.Message;
                LogTool.SaveLogMessage(ex, actionName, this.csName);
            }

            return temp;
        }

        public RESPONSE_MSG save(List<DB_RCS_VPN_UPLOAD_TRAN> model, IPDPatientInfo pat_info, UserInfo user_info)
        {
            RESPONSE_MSG rm = new RESPONSE_MSG();
            string actionName = "savveVPN_UPLOAD";

            try
            {
             


                string hosp_id = IniFile.GetConfig("System", "HOSP_NUM_ID");//醫院代碼
                IPDPatientInfo join_obj = pat_info;
              
                List<DB_RCS_VPN_UPLOAD_TRAN> list = new List<DB_RCS_VPN_UPLOAD_TRAN>();
                list = model;

                save_check(ref list, ref rm);

                if (rm.status != RESPONSE_STATUS.SUCCESS)
                {
                    return rm;
                }


                SQLProvider SQL = new SQLProvider();
                #region 儲存資料
                List<DB_RCS_VPN_UPLOAD_TRAN> insert_list = new List<DB_RCS_VPN_UPLOAD_TRAN>();
                List<DB_RCS_VPN_UPLOAD_TRAN> update_list = new List<DB_RCS_VPN_UPLOAD_TRAN>();
                List<string> slqList = new List<string>();


                /*2018/10/02--因清空TRAN_TYPE=2，儲存時SQL無法更新，所以先DELETE_SQL再INSERT_SQL*/
                StringBuilder sb = new StringBuilder();
                #region DELETE_SQL
                sb.Append("DELETE FROM RCS_VPN_UPLOAD_TRAN ");
                sb.Append(string.Concat(" WHERE TRAN_ID = ", SQL.namedArguments, "TRAN_ID"));
                //sb.Append(string.Concat(" AND TRAN_TYPE = ", "2"));
                #endregion
                slqList.Add(sb.ToString());
                /*2018/10/02--因清空TRAN_TYPE=2，儲存時SQL無法更新，所以先DELETE_SQL再INSERT_SQL*/

                #region INSERT_SQL
                sb = new StringBuilder();
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
                    update_list.ForEach(x =>
                    {
                        //x.MODIFY_DATE = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                        //x.MODIFY_ID = user_info.user_id;
                        //x.MODIFY_NAME = user_info.user_name;
                        //x.UPLOAD_STATUS = "0";
                        //x.DATASTATUS = "1";
                        //x.TRAN_HOSP_ID = hosp_id;

                        x.IPD_NO = pat_info.ipd_no;
                        x.CHART_NO = pat_info.chart_no;
                        x.PATIENT_NAME = pat_info.patient_name;
                        x.GENDER = pat_info.gender;
                        x.ID_NO = join_obj.idno == null ? "" : join_obj.idno;
                        x.BIRTH_DAY = pat_info.birth_day;
                        x.TRAN_HOSP_ID = hosp_id;
                        x.UPLOAD_STATUS = "0";
                        x.DATASTATUS = "1";
                        x.CREATE_DATE = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                        x.CREATE_ID = user_info.user_id;
                        x.CREATE_NAME = user_info.user_name;
                    });
                    //TRAN_STATION_TYPE
                    if (update_list.Exists(x => x.TRAN_TYPE == "2"))
                    {
                        update_list.FindAll(x => x.TRAN_TYPE == "2").ForEach(x => {
                            x.TRAN_STATION_TYPE = list.Find(s => s.TRAN_TYPE == "1").TRAN_STATION_TYPE;
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
                        x.ID_NO = join_obj.idno == null ? "" : join_obj.idno;
                        x.BIRTH_DAY = pat_info.birth_day;
                        x.TRAN_HOSP_ID = hosp_id;
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
                            x.TRAN_STATION_TYPE = list.Find(s => s.TRAN_TYPE == "1").TRAN_STATION_TYPE;
                        });
                    }

                }
                SQL.DBA.BeginTrans();
                if (update_list.Count > 0)
                {
                    List<DB_RCS_VPN_UPLOAD_TRAN> temp_updateList = new List<DB_RCS_VPN_UPLOAD_TRAN>();
                    temp_updateList = list.FindAll(x => !string.IsNullOrWhiteSpace(x.TRAN_ID));

                    SQL.DBA.DBExecute<DB_RCS_VPN_UPLOAD_TRAN>(slqList[0], update_list);

                    if (temp_updateList != null && temp_updateList.Count > 0)
                    {
                        SQL.DBA.DBExecute<DB_RCS_VPN_UPLOAD_TRAN>(slqList[1], update_list);
                    }
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

            return rm;
        }

        public RESPONSE_MSG delVPN_UPLOAD(List<DB_RCS_VPN_UPLOAD_TRAN> model, IPDPatientInfo pat_info, UserInfo user_info) {
            RESPONSE_MSG rm = new RESPONSE_MSG();

            SQLProvider SQL = new SQLProvider();
            string actionName = "savveVPN_UPLOAD";
            try
            {
                List<DB_RCS_VPN_UPLOAD_TRAN> list = new List<DB_RCS_VPN_UPLOAD_TRAN>();
                list = model;
                if (list.Exists(x => !string.IsNullOrWhiteSpace(x.TRAN_ID)))
                {
                    list = list.FindAll(x => !string.IsNullOrWhiteSpace(x.TRAN_ID));
                }
                else
                {
                    rm.status = RESPONSE_STATUS.SUCCESS;
                    rm.message = "此筆資料尚未儲存，無法刪除，此視窗關閉!";
                    return rm;
                }

                #region 儲存資料  
                StringBuilder sb = new StringBuilder();
                #region DELETE_SQL
                sb.Append("UPDATE RCS_VPN_UPLOAD_TRAN SET DATASTATUS = '9',MODIFY_ID= @MODIFY_ID, MODIFY_NAME= @MODIFY_NAME, MODIFY_DATE= @MODIFY_DATE ");
                sb.Append(string.Concat(" WHERE TRAN_ID = ", SQL.namedArguments, "TRAN_ID AND TRAN_TYPE = ", SQL.namedArguments, "TRAN_TYPE"));
                #endregion
                list.ForEach(x => {
                    x.MODIFY_DATE = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                    x.MODIFY_NAME = user_info.user_name;
                    x.MODIFY_ID = user_info.user_id;
                });
                SQL.DBA.DBExecute<DB_RCS_VPN_UPLOAD_TRAN>(sb.ToString(), list);
                #endregion
                rm = SQL.RESPONSE_MSG;
                if (rm.status == RESPONSE_STATUS.SUCCESS)
                {
                    rm.message = "刪除成功";
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

            return rm;
        }

        public RESPONSE_MSG uploadVpn(List<VpnUploadData> List, IPDPatientInfo pat_info, UserInfo user_info) {
            RESPONSE_MSG rm = new RESPONSE_MSG();
            string actionName = "exportVPN_TXT";
            var idList = List;
            SQLProvider SQL = new SQLProvider();
            string sql = "";
            #region 更新註記
            #region UPDATE_SQL
            StringBuilder sb = new StringBuilder();
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
            } else {
                rm.status = RESPONSE_STATUS.SUCCESS;
            }
            return rm;
        }

        public RESPONSE_MSG set_vpn_data_frag(List<VpnUploadData> List, string type, IPDPatientInfo pat_info, UserInfo user_info)
        {
            string actionName = "set_vpn_data_frag";
            RESPONSE_MSG rm = new RESPONSE_MSG();

            List<VpnUploadData> pTList = new List<VpnUploadData>();


            try
            {
                pTList = List;

                if (pTList.Count > 0)
                {
                    pTList.ForEach(x => {
                        x.DATASTATUS = type;
                        x.MODIFY_ID = user_info.user_id;
                        x.MODIFY_NAME = user_info.user_name;
                        x.MODIFY_DATE = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                    });
                    SQLProvider SQL = new SQLProvider();
                    string sql = string.Concat("UPDATE RCS_VPN_UPLOAD_TRAN SET",
                        " DATASTATUS = @DATASTATUS,",
                        " MODIFY_ID = @MODIFY_ID,",
                        "MODIFY_NAME = @MODIFY_NAME,",
                        "MODIFY_DATE = @MODIFY_DATE WHERE TRAN_ID = @TRAN_ID AND TRAN_TYPE = @TRAN_TYPE");

                    SQL.DBA.DBExecute(sql, pTList);
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

            return rm;

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
                if (pList.Exists(x => x.TRAN_TYPE == "2" && !string.IsNullOrWhiteSpace(x.TRAN_DATE)) && (!pList.Exists(x => x.TRAN_TYPE == "1") || pList.Exists(x => x.TRAN_TYPE == "1" && string.IsNullOrWhiteSpace(x.TRAN_DATE))))
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
                                msgList.Add("病患來源=\"居家照護\"或\"其他\"，則 \"病患來源病床類別\" 為空白");
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
                            if (!string.IsNullOrWhiteSpace(item.TRAN_STATION_TYPE) && item.TRAN_STATION_TYPE == "1" && string.IsNullOrWhiteSpace(item.TRAN_REASON))
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
                                msgList.Add("當呼吸器= \"使用CPCA或Bi-PAP\"，則氣道介面只能選擇\"氣管插管\"、\"氣切插管\"、\"面罩\"、\"鼻面罩\"或\"Nasal Prong\"");
                            }
                            if (!string.IsNullOrWhiteSpace(item.MV) && item.MV == "3" && (string.IsNullOrWhiteSpace(item.MV_MODE) || (item.MV_MODE != "1" && item.MV_MODE != "2" && item.MV_MODE != "5" && item.MV_MODE != "6" && item.MV_MODE != "7")))
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
                                msgList.Add("轉出目的醫院類別=\"本院\" ，則\"醫事機構代碼\"空白(系統自動設定病患來源醫事機構代碼=轉入醫事機構代號)");
                            }
                            if (TRAN_HOSP == "2" && string.IsNullOrWhiteSpace(item.HOSP_ID))
                            {
                                msgList.Add("轉出目的醫院類別=\"他院\" ，則\"醫事機構代碼\" 為必填");
                            }
                            //HOSP_NAME
                            if (TRAN_HOSP == "2" && string.IsNullOrWhiteSpace(item.HOSP_NAME))
                            {
                                msgList.Add("轉出目的醫院類別=\"他院\"時，\"醫事機構名稱\" 為必填");
                            }
                            //MV_MODE 氣道介面
                            if (!string.IsNullOrWhiteSpace(item.TRAN_SITUATION) && item.TRAN_SITUATION == "3" && !string.IsNullOrWhiteSpace(item.MV_MODE))
                            {
                                msgList.Add("當轉出時情形=\"死亡\"時， \"氣道介面\" 為空白");
                            }
                            if (!string.IsNullOrWhiteSpace(item.TRAN_SITUATION) && item.TRAN_SITUATION != "3" && string.IsNullOrWhiteSpace(item.MV_MODE))
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
                            if (!string.IsNullOrWhiteSpace(item.TRAN_SITUATION) && (item.TRAN_SITUATION == "2" || item.TRAN_SITUATION == "6") && (string.IsNullOrWhiteSpace(item.MV) || item.MV != "3"))
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
                            if ((!string.IsNullOrWhiteSpace(item.TRAN_HOSP) && (item.TRAN_HOSP == "1" || item.TRAN_HOSP == "2")) && !string.IsNullOrWhiteSpace(item.TRAN_SITUATION) && (item.TRAN_SITUATION == "1" || item.TRAN_SITUATION == "6") && string.IsNullOrWhiteSpace(item.TRAN_BED))
                            {
                                msgList.Add("當轉出時情形=\"未脫離呼吸器\"或\"嘗試脫離呼吸器中\"時， \"轉出目的病床類別\" 為必填");
                            }
                            if (!string.IsNullOrWhiteSpace(item.TRAN_SITUATION) && item.TRAN_SITUATION == "2" && (string.IsNullOrWhiteSpace(item.TRAN_BED) || item.TRAN_BED == "2" || item.TRAN_BED == "3"))
                            {
                                msgList.Add("當轉出時情形=\"脫離呼吸器成功\"時， \"轉出目的病床類別\" 為必填，但不可填 \"RCC\"、\"RCW\"");
                            }
                            if (!string.IsNullOrWhiteSpace(item.TRAN_SITUATION) && (item.TRAN_SITUATION == "3" || item.TRAN_SITUATION == "4" || item.TRAN_SITUATION == "5") && !string.IsNullOrWhiteSpace(item.TRAN_BED))
                            {
                                msgList.Add("當轉出時情形=\"死亡\"、\"一般自動出院\"或\"病危自動出院\"時， \"轉出目的病床類別\" 為空白");
                            }
                            //WEANING_REMARK 脫離呼吸器但因病況需要繼續留住原病房註記
                            if (!string.IsNullOrWhiteSpace(item.WEANING_REMARK) && item.WEANING_REMARK == "Y" && (string.IsNullOrWhiteSpace(item.TRAN_SITUATION) || item.TRAN_SITUATION != "2"))
                            {
                                msgList.Add("脫離呼吸器但因病況需要繼續留住原病房註記，則轉出時情形只能選擇 \"脫離呼吸器成功\"");
                            }

                            //WEANING_DATE 脫離呼吸器成功/嘗試脫離呼吸器中日期
                            if (!string.IsNullOrWhiteSpace(item.TRAN_SITUATION) && (item.TRAN_SITUATION == "2" || item.TRAN_SITUATION == "6") && string.IsNullOrWhiteSpace(item.WEANING_DATE))
                            {
                                msgList.Add("當轉出時情形= \"脫離呼吸器成功\" 或 \"嘗試脫離呼吸器中\" 時， \"脫離/嘗試脫離呼吸器日期\" 為必填");
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
                    rm.message = string.Join("\n", msgList);
                }
            }
            catch (Exception ex)
            {
                rm.status = RESPONSE_STATUS.EXCEPTION;
                rm.message = "儲存失敗，請洽資訊人員!";
                LogTool.SaveLogMessage(ex, actionName, GetLogToolCS.VPN_UPLOADController);
            }
        }
     
       
    }

    public class FormRtCPTAssList : AUTH
    {
        public bool isAllData { get; set; }
        public bool isUPLOADData { get; set; }
        public bool isExportFun { get; set; }
        public bool isIpdno { get; set; }
    }

    public class FormVpnUploadDetail : AUTH
    {
        public string ID { get; set; }
    }

    public class VpnUploadDetail
    {
        public VpnUploadData model { get; set; }

        public List<DB_RCS_SYS_PARAMS> selectList { get; set; }
    }

    public class VpnUploadDetail_save : AUTH
    {
        //public VpnUploadData model { get; set; }
        public string model { get; set; }
    }

    public class VpnUploadDetailUpload : AUTH
    {
        public string List { get; set; }
        public string type { get; set; }
    }
}

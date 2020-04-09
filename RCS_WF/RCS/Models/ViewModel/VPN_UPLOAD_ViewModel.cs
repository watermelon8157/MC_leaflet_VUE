using Dapper;
using RCS_Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using static Dapper.SqlMapper;
using Com.Mayaminer;

namespace RCS.Models
{
    public class VPN_UPLOAD_ViewModel
    {
        public RESPONSE_MSG RESPONSE_MSG { get; private set; }
        private string csName { get { return "VPN_UPLOAD_ViewModel"; } }
        /// <summary>
        /// 有還沒沒有上傳的資料
        /// </summary>
        public bool hasNoUpload { get { return (tran_lsit.Exists(x => x.UPLOAD_STATUS == "0") || tran_lsit == null || tran_lsit.Count>2); } }
        /// <summary>
        /// 畫面下拉是選單資料
        /// </summary>
        public List<SysParams> selectList { get; set; }
        /// <summary>
        /// VPN_upload_List
        /// </summary>
        public List<DB_RCS_VPN_UPLOAD_TRAN> tran_lsit { get; set; }

        /// <summary>
        /// 畫面基本設定
        /// </summary>
        /// <param name="ipd_no"></param>
        public void setSelectList(string ipd_no,string tran_id ="")
        {
            SQLProvider SQL = new SQLProvider();
            string actionName = "setSelectList";
            RESPONSE_MSG = new RESPONSE_MSG();
            try
            {
                string sqlStr = "";
                System.Text.StringBuilder sb = new System.Text.StringBuilder();
                #region SQL 語法
                sb.Append(string.Concat("SELECT * FROM ", GetTableName.RCS_SYS_PARAMS.ToString(), " WHERE P_MODEL = ", SQL.namedArguments, "P_MODEL;"));
                DynamicParameters dp = new DynamicParameters();
                dp.Add("P_MODEL", "VPN_UPLOAD");
                if (!string.IsNullOrWhiteSpace(ipd_no))
                {
                    sb.Append(string.Concat("SELECT * FROM RCS_VPN_UPLOAD_TRAN"));
                    sb.Append(string.Concat(" WHERE IPD_NO = ", SQL.namedArguments, "IPD_NO"));
                    dp.Add("IPD_NO", ipd_no);
                }
                else
                {
                    sb.Append(string.Concat("SELECT * FROM RCS_VPN_UPLOAD_TRAN WHERE DATASTATUS in('1','3')"));
                }
                if (!string.IsNullOrWhiteSpace(tran_id))
                {
                    sb.Append(string.Concat(" AND TRAN_ID = ", SQL.namedArguments, "TRAN_ID"));
                    dp.Add("TRAN_ID", tran_id);
                }
                #endregion
                sqlStr = sb.ToString();


                SQL.DBA.Open();
                #region 取得結果
                GridReader gr = SQL.DBA.dbConnection.QueryMultiple(sqlStr, dp);
                selectList = gr.Read<SysParams>().ToList();
                tran_lsit = gr.Read<DB_RCS_VPN_UPLOAD_TRAN>().ToList();
                #endregion
                SQL.DBA.Close();

                if (SQL.DBA.hasLastError)
                {
                    RESPONSE_MSG.status = RESPONSE_STATUS.ERROR;
                    RESPONSE_MSG.message = SQL.DBA.lastError;
                }


                //設定轉入轉出畫面勾選資料
                if (tran_lsit.Exists(x => !string.IsNullOrWhiteSpace(x.WEANING_REMARK) && x.WEANING_REMARK == "Y"))
                {
                    tran_lsit.Find(x => !string.IsNullOrWhiteSpace(x.WEANING_REMARK) && x.WEANING_REMARK == "Y").is_WEANING_REMARK = new List<JSON_DATA>() { new JSON_DATA() { id = "WEANING_REMARK", val = "Y", chkd = true } };
                }

            }
            catch (Exception ex)
            {
                RESPONSE_MSG.status = RESPONSE_STATUS.EXCEPTION;
                RESPONSE_MSG.message = ex.Message;
                LogTool.SaveLogMessage(ex, actionName, this.csName);
            }
        }
    }

    public class Tran_Vpn_Tbl: DB_RCS_VPN_UPLOAD_TRAN
    {
        /// <summary>
        /// 性別
        /// </summary>
        public string gender { get; set; }
        /// <summary>
        /// 轉出PK
        /// </summary>
        public string TRAN_OUT_ID { get; set; }
        /// <summary>
        /// 轉出日期
        /// </summary>
        public string TRAN_OUT { get; set;}
        /// <summary>
        /// 轉出產生檔案註記
        /// </summary>
        public string OUT_UPLOAD_STATUS { get; set; }
        /// <summary>
        /// 轉出類別
        /// </summary>
        public string OUT_TRAN_TYPE { get; set; }

        /// <summary>
        /// 轉出醫事機構代碼
        /// </summary>
        public string OUT_HOSP_ID { get; set; }
        /// <summary>
        /// 轉出醫事機構名稱
        /// </summary>
        public string OUT_HOSP_NAME { get; set; }

    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RCS_Data.Models.JAG
{
    public abstract class JAGOrderRoot
    {
        /// <summary>
        /// 送簽交易的之日期
        /// </summary>
        public string RequestDate = "";
        /// <summary>
        /// 送簽交易的之時間
        /// </summary>
        public string RequestTime = "";
        /// <summary>
        /// 簽章者員工編號
        /// </summary>
        public string RequestUser = "";
        /// <summary>
        /// 簽章者姓名
        /// </summary>
        public string RequestUserName = "";
        /// <summary>
        /// 簽章者身分證號碼
        /// </summary>
        public string UserIDNO = "";
        /// <summary>
        /// 簽章所屬科別代號
        /// </summary>
        public string RequestDivision = "";
        /// <summary>
        /// 病歷文件檔名
        /// </summary>
        public string FileName = "";
        /// <summary>
        /// 系統代碼
        /// </summary>
        public string SignSystem = "";
        /// <summary>
        /// 病歷類別代碼
        /// </summary>
        public string RequestDocType = "";
        /// <summary>
        /// 文件產生日期，非送簽日期
        /// </summary>
        public string RequestDocDate = "";
        /// <summary>
        /// 文件產生時間，非送簽時間
        /// </summary>
        public string RequestDocTime = "";
        /// <summary>
        /// /住/急診 就診流水號
        /// </summary>
        public string RequestDocRoot = "";
        /// <summary>
        /// 此病歷文件的唯一編號
        /// </summary>
        public string RequestDocNo = "";
        /// <summary>
        /// 此病歷文件之病患編號
        /// </summary>
        public string RequestPatientID = "";
        /// <summary>
        /// 此病歷文件之病患姓名
        /// </summary>
        public string RequestPatientName = "";
        /// <summary>
        /// 就診或檢查日期
        /// </summary>
        public string VisitDate = "";
        /// <summary>
        /// 就診別
        /// </summary>
        public string Category = "";
        /// <summary>
        /// 文件費用別
        /// </summary>
        public string DocCharge = "";
        /// <summary>
        /// 是否住院中
        /// </summary>
        public string InHospital = "";
        /// <summary>
        /// 出院日期
        /// </summary>
        public string DischargeDate = "";


    }
}

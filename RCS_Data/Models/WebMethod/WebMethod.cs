//**************************************************
//2016/08/15
//#2172 建立DB Model
//功能 取得webService資料
//**************************************************
using System.Collections.Generic;
using RCS_Data;
using Newtonsoft.Json;
using RCSData.Models;
using Com.Mayaminer;

namespace RCSData.Models
{

    public partial class WebMethod : RCS_Data.Models.BASIC_PARAMS
    {
        string csName { get { return "WebMethod"; } }
 
        /// <summary>
        /// 醫院代碼
        /// </summary>
        public virtual string hosp_id { get { return Com.Mayaminer.IniFile.GetConfig("System", "HOSP_ID"); } }

        HISDataExchange _HISData;
        public HISDataExchange HISData
        {
            get
            {
                if (_HISData == null)
                {
                    _HISData = new HISDataExchange();
                }
                return _HISData;
            }
        } 

        
        /// <summary> 資料狀態 </summary>
        public RESPONSE_MSG rm { get {
                RESPONSE_MSG _rm = new RESPONSE_MSG();
                if(this.datastatus != HISDataStatus.SuccessWithData && this.datastatus != HISDataStatus.SuccessWithoutData)
                {
                    _rm.status = RESPONSE_STATUS.ERROR;
                    _rm.message = this.errorMsg;
                }
                return _rm;
            } }
        /// <summary> 資料取得狀態 </summary>
        public HISDataStatus datastatus { get; private set; }
        /// <summary> 錯誤訊息文字 </summary>
        public string errorMsg { get; private set; }


        public WebMethod()
        {
           

        }
         
        /// <summary>
        /// 取得回傳值
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="pList"></param>
        /// <param name="sr"></param>
        private List<T> setReturnValue<T>(  ServiceResult<T> sr)
        {
            List<T> pList = new List<T>();
            this.datastatus = sr.datastatus;
            this.errorMsg = sr.errorMsg;
            if (sr.datastatus == HISDataStatus.SuccessWithData)
            {
                pList = JsonConvert.DeserializeObject<List<T>>(sr.returnJson);
            }
            return pList;
        }
         

    }



}
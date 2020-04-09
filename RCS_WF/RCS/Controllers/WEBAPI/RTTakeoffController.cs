using Newtonsoft.Json;
using RCS.Models;
using RCS.Models.ViewModel;
using RCS_Data;
using RCS_Data.Controllers;
using RCS_Data.Controllers.RtTakeoff;
using RCS_Data.Models.DB;
using RCS_Data.Models.ViewModels;
using RCSData.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace RCS.Controllers.WEBAPI
{
    [JwtAuthActionFilter]
    public class RTTakeoffController : BasicController, IRtTakeoffController
    {
        string csName { get { return "RTTakeoffController"; } }

        RTTakeoff _model;
        RTTakeoff RtTakeoffModel
        {
            get
            {
                if (_model == null)
                {
                    this._model = new RTTakeoff();
                }
                return this._model;
            }
        }

        /// <summary>
        /// 呼吸器脫離評估單資料清單
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public List<VM_RTTakeoffAssess> RtTakeOffList(Form_RtTakeoffList form)
        {
            RESPONSE_MSG rm = new RESPONSE_MSG();
            List<VM_RTTakeoffAssess> rttakeoff_items = new List<VM_RTTakeoffAssess>();

            UserInfoBasic user_info = form.user_info;
            IPDPatientInfo pat_info = form.pat_info;
            rm = RtTakeoffModel.RtTakeOffList(ref rttakeoff_items, form.pSDate, form.pEDate, pat_info, form.pId, user_info);
            
            return rttakeoff_items.OrderByDescending(x => x.rec_date).ToList();
        }

        /// <summary>
        /// 儲存呼吸器脫離評單
        /// </summary>
        /// <param name="form"></param>
        /// <returns></returns>
        [HttpPost]
        public RESPONSE_MSG RtTakeOff_Save(Form_RtTakeOff_Save form)
        {
            RESPONSE_MSG rm = new RESPONSE_MSG();

            rm = RtTakeoffModel.RtTakeOff_Save(form);
            return rm;
        }

        /// <summary>
        /// NEW 呼吸器脫離評估單-Form資料來源
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public object RtTakeOffDetail(Form_RtTakeOffDetail form)
        {
            RtTakeOffDetail result = new RtTakeOffDetail();
            result.model = new RCS_RTTakeoff();
            if (form.TK_ID == null || form.TK_ID == "")
            {                
                result.model.diag_date = form.pat_info.diag_date;
                result.model.rec_date = DateTime.Now.ToString("yyyy-MM-dd HH:mm");

                #region 取得評估單資料
                RCS_CPT_DTL_NEW_ITEMS cptData = new BaseModels().getCPTAssess(form.pat_info.chart_no);                
                if (!string.IsNullOrWhiteSpace(cptData.rt_start_time))
                {
                    result.model.on_breath_date = cptData.rt_start_time; //呼吸器開始使用日期
                }
                #endregion
            }
            else
            {
                //讀取資料

                var getData = RtTakeoffModel.RtTakeoffData(new List<string>() { form.TK_ID });

                if (getData != null)
                {
                    // 因前端沒有紀錄者、紀錄者ID欄位，因此要從masterList存入detailList
                    getData.detailList[0].create_name = getData.masterList[0].CREATE_NAME;
                    getData.detailList[0].create_id = getData.masterList[0].CREATE_ID;
                    getData.detailList[0].DATASTATUS = getData.masterList[0].DATASTATUS;
                    getData.detailList[0].UPLOAD_STATUS = getData.masterList[0].UPLOAD_STATUS;
                    getData.detailList[0].UPLOAD_ID = getData.masterList[0].UPLOAD_ID;
                }

                if (getData.detailList.Any())
                {
                    result.model = RtTakeoffModel.changeJson(getData.detailList).First();
                }

            }

            if (!String.IsNullOrWhiteSpace(result.model.weaningTable_data))
            {

                var getData = this.DBLink.Select_JSONData<DB_RCS_RT_RECORD_JSON>(result.model.weaningTable_data);

                if (getData.Any())
                {

                    result.WeaningProfile_List = JsonConvert.DeserializeObject<List<RCS_WEANING_ITEM>>(getData.First().JSON_VALUE);
                }
            }

            result.last_tk_id = RtTakeoffModel.RTTakeoffLastData(form.pat_info).tk_id;

            // 呼吸器脫離評估只能修改自己填的紀錄單[下]

            result.now_user_name = form.user_info.user_name; // 取得目前登入者姓名
            result.now_user_id = form.user_info.user_id; // 取得目前登入者ID

            // 呼吸器脫離評估只能修改自己填的紀錄單[上]

            return result;
        }

        /// <summary>
        /// 刪除
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public RESPONSE_MSG RtTakeOffDelete(Form_RtTakeOffDetail form)
        {
            RESPONSE_MSG rm = new RESPONSE_MSG();

            rm = RtTakeoffModel.RtTakeOffDelete(new List<string>() { form.TK_ID }, form.user_info);

            return rm;
        }

        /// <summary>
        /// 取得Weaning profile
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public List<RCS_WEANING_ITEM> WeaningProfileTB(Form_RtTakeOffDetail form)
        {
            RESPONSE_MSG rm = new RESPONSE_MSG();
            List<RCS_WEANING_ITEM> pList = new List<RCS_WEANING_ITEM>();

            rm = RtTakeoffModel.WeaningProfileTB(ref pList, form.pat_info);

            return pList;
        }

    }

    /// <summary>
    /// RAAsses 表單回傳
    /// </summary>
    public class RtTakeOffDetail
    {
        /// <summary>
        /// 取得資料
        /// </summary>
        public RCS_RTTakeoff model { get; set; }

        /// <summary>
        /// 最後一筆ID
        /// </summary>
        public string last_tk_id { get; set; }

        /// <summary>
        /// 現在登入者姓名
        /// </summary>
        public string now_user_name { get; set; }

        /// <summary>
        /// 現在登入者ID
        /// </summary>
        public string now_user_id { get; set; }

        /// <summary>
        /// WeaningProfile LIST
        /// </summary>
        public List<RCS_WEANING_ITEM> WeaningProfile_List { get; set; }
    }
}

using Com.Mayaminer;
using RCS_Data;
using RCS_Data.Models.DB;
using RCS_Data.Models.ViewModels;
using RCSData.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;

namespace RCS.Models.ViewModel
{
    public class RtCptReq : RCS_Data.Controllers.RtCptReq.Models, RCS_Data.Controllers.RtCptReq.Interface
    {
        public RESPONSE_MSG saveCPT(RTCptReq model, IPDPatientInfo pat_info, UserInfo user_info)
        {
            RESPONSE_MSG rm = new RESPONSE_MSG();
            //cxrJSON判斷
            if (!string.IsNullOrWhiteSpace(model.CXR_result_json))
            {
                model.CXR_key = this.saveCxrJSON(model.CXR_result_json, ref rm);
            }
            else
            {
                model.CXR_key = "";
            }

            model.CXR_result_json = "";
            model.PDF_CXR_Result_Str = "";
            rm = this.saveCPT<RTCptReq>(model.record_id,model.rec_date, model,pat_info, user_info);
            return rm;
        }

        /// <summary>
        /// 呼吸治療評估單LIST
        /// </summary>
        /// <param name="cpt_dtl_new_items"></param>
        /// <param name="form"></param>
        /// <returns></returns>
        public List<RTCptReq> RtCptReqListByID(List<string> idList)
        { 
            string actionName = "RtCptReqListByID";
            RESPONSE_MSG rm = new RESPONSE_MSG(); 
            List<RTCptReq> table = this.RtCptReqListByID<RTCptReq>(idList); 
            table = table.OrderByDescending(x => DateTime.Parse(x.rec_date)).ToList(); 
            return table;
        }

        /// <summary>
        /// 呼吸治療評估單LIST
        /// </summary>
        /// <param name="cpt_dtl_new_items"></param>
        /// <param name="form"></param>
        /// <returns></returns>
        public RESPONSE_MSG RtCptReqListByDate(ref List<RTCptReq> cpt_dtl_new_items, string pSDate, string pEDate, IPDPatientInfo pat_info)
        {
            string actionName = "RtCptReqListByDate";
            RESPONSE_MSG rm = new RESPONSE_MSG();   
            List<RTCptReq> table = new List<RTCptReq>();
            rm  = this.RtCptReqListByDate<RTCptReq>(ref table, pSDate, pEDate, pat_info); 
            table = table.OrderByDescending(x => DateTime.Parse(x.rec_date)).ToList();
            cpt_dtl_new_items = table;
            return rm;
        }

        public List<RTCptReq> changeData(List<RTCptReq> List)
        {
            return List;
        }
    }
    public class printRtCptReq
    {
        public RTCptReq data { get; set; }


        public IPDPatientInfo pat_info { get; set; }

        public UserInfo user_info { get; set; }
    }
    public class ListprintRtCptReq {
      public  List<printRtCptReq> List { get; set; }
    } 
    public class RTCptReq_Save : AUTH
    {
        public RTCptReq model { get; set; }

    } 
    public class RTCptReqDetail
    {
        /// <summary>
        /// 取得資料
        /// </summary>
        public RTCptReq model { get; set; }

        /// <summary>
        /// 最後一筆ID
        /// </summary>
        public string last_record_id { get; set; }

    }
}
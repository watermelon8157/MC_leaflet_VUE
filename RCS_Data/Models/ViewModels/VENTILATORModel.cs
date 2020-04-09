using RCS_Data.Models.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RCS_Data.Models.ViewModels
{
    public class VENTILATORViewModel
    {
        /// <summary>
        /// 呼吸器編號List
        /// </summary>
        public List<SelectMV_NOList> selectMV_NOList;

        /// <summary>
        /// 呼吸器檢核單明細
        /// </summary>
        public List<CheckListDetailModel> CheckListDatas;

        public string MV_NO;

        public string V_TYPE;

        public string V_STATUS;

        public string V_MODE;

        public string RECORD_DATE;

        public string DEVICE_MODEL;

    }

    /// <summary>
    /// 呼吸器編號List Detail
    /// </summary>
    public class SelectMV_NOList
    {
        public string DEVICE_NO;

        public string DEVICE_MODEL;

    }


    public class VENTILATORModel : DB_RCS_VENTILATOR_SCHEDULING_CHECKLIST
    {
        /// <summary>
        /// 呼吸器
        /// </summary>
        public string DEVICE_MODEL { get; set; }
        public string selectMV_NO
        {
            get
            {
                return DEVICE_MODEL + "-" + MV_NO;
            }
        }

        /// <summary>
        /// 呼吸器檢核單明細
        /// </summary>
        public List<CheckListDetailModel> RCS_VENTILATOR_SCHEDULING_CHECKLIST { get; set; }

    }

    public class VENTILATORGroupModel
    {

        public List<VENTILATORModel> dataLists { get; set; }

        public string DEVICE_MODEL { get; set; }

        public string RECORD_DATE { get; set; }

        public bool hasDatastatus { get; set; }
    }

    /// <summary>
    /// 呼吸器檢核單明細Detail
    /// </summary>
    public class CheckListDetailModel : DB_RCS_VENTILATOR_SCHEDULING_CHECKLIST_DEATAIL
    {

        public string ITEM_NAME_CODE { get; set; }

        public int ITEM_TYPE;
        public string Check { get; set; }
    }

    public class RCS_VENTILATOR_SETTINGSModel
    {

        public string DEVICE_SEQ { get; set; }

        public string DEVICE_NO { get; set; }

        public string ROOM { get; set; }

        public string DEVICE_MODEL { get; set; }

        public string CREATE_ID { get; set; }

        public string CREATE_NAME { get; set; }

        public string CREATE_DATE { get; set; }

        public string MODIFY_ID { get; set; }

        public string MODIFY_NAME { get; set; }

        public string MODIFY_DATE { get; set; }

        public string USE_STATUS { get; set; }

        public string PURCHASE_DATE { get; set; }

        public string DEVICE_NUM { get; set; }
    }
}

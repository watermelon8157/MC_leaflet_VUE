using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RCS_Data.Models.ViewModels
{
    public abstract class DeviceMaster_BASIC
    {
        /// <summary>流水號</summary>
        public string DEVICE_SEQ { set; get; }
        /// <summary>呼吸器編號</summary>
        public string DEVICE_NO { set; get; }
        /// <summary>病房</summary>
        public string ROOM { set; get; }
        /// <summary>呼吸器型號</summary>
        public string DEVICE_MODEL { set; get; }
        /// <summary>使用狀態{Y:使用中, N:停用}</summary>
        public string USE_STATUS { set; get; }
        /// <summary>採購日期</summary>
        public string PURCHASE_DATE { set; get; }
        /// <summary>使用中位置</summary>
        protected string _ON_POSITION { set; get; }
        /// <summary>使用中位置</summary>
        public virtual string ON_POSITION { set; get; }
        /// <summary> 下拉選單用值 </summary>
        public virtual string field_value { set; get; }
    }

    /// 呼吸器維護清單
    /// </summary>
    public class DeviceMaster : DeviceMaster_BASIC
    { 
        /// <summary>使用中位置</summary>
        public override string ON_POSITION
        {
            set { _ON_POSITION = value; }
            get
            {
                string temp = _ON_POSITION; 
                return temp.Replace("|", "");
            }
        }
        /// <summary> 下拉選單用值 </summary>
        public override string field_value
        {
            get
            {
                return DEVICE_NO + "|" + DEVICE_MODEL;
            }
        }
    }

    public class RESP_COLLECTION : List<DeviceMaster>
    {

        /// <summary> 取得呼吸器型號清單 </summary>
        /// <returns></returns>
        public List<string> get_resp_list()
        {
            return this.Select(x => x.DEVICE_MODEL).Distinct().ToList();
        }
        /// <summary> 取得呼吸器編號清單 </summary>
        /// <returns></returns>
        public List<string> get_D_NO_list()
        {
            return this.Select(x => x.DEVICE_NO).Distinct().ToList();
        }
        /// <summary> 取得呼吸器型號清單 </summary>
        /// <returns></returns>
        public List<DeviceMaster> get_CPTDeviceMaster_list()
        {
            return this;
        }
    }
}

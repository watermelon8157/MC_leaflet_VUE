using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RCS_Data.Models.VIP.Models
{
    /// EvitaV300 設定
    /// </summary>
    public class VIP_SETTING_EvitaV300 : AVIP_SETTING, IVIP_ALARM_SETTING
    {
        /// <summary>
        public override void setALARM_CONT()
        /// 內容機型_alarm代碼
        /// </summary>

        {
            VIP_ALARMList.ForEach(x => x.ALARM_CONT = string.Format("{0}_{1}={2}", x.DEVICE_TYPE, x.ALARM_CODE, x.ALARM_MSG));
        }

        public virtual List<string> getALARM_Msg(string central_message)
        {
            List<string> msgList = new List<string>();

            return msgList;
        }
    }

}

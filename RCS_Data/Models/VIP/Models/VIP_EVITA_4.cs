using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RCS_Data.Models.VIP.Models
{
    /// <summary>
    /// EVITA-4 設定
    /// </summary>
    public class VIP_SETTING_EVITA_4 : AVIP_SETTING
    {

        /// <summary>
        /// 內容機型_alarm代碼
        /// </summary>
        public override void setALARM_CONT()
        {
            VIP_ALARMList.ForEach(x => x.ALARM_CONT = string.Format("{0}_{1}", x.DEVICE_TYPE, x.ALARM_MSG));
        }
        /// <summary>
        /// 取得警告訊息
        /// </summary>
        /// <returns></returns>
        public override List<string> getALARM_Msg(string central_message)
        {
            List<string> msgList = new List<string>();
            List<string> alarmList = central_message.Split(',').ToList();
            if (alarmList.Count > 0)
            {
                #region default
                foreach (string item in alarmList)
                {
                    if (string.IsNullOrWhiteSpace(item.Trim())) continue;

                    if (this.VIP_ALARMList.Exists(x => x.ALARM_CONT == string.Format("{0}_{1}", x.DEVICE_TYPE, item.Trim())))
                    {
                        msgList.Add(this.VIP_ALARMList.Find(x => x.ALARM_CONT == string.Format("{0}_{1}", x.DEVICE_TYPE, item.Trim())).ALARM_DESC);
                    }
                    else
                    {
                        msgList.Add(item.Trim());
                    }
                }
                #endregion
            }
            return msgList;
        }
    }



}

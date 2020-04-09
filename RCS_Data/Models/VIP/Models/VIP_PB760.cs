using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RCS_Data.Models.VIP.Models
{
    /// <summary>
    /// PB760 設定
    /// </summary>
    public class VIP_SETTING_PB760 : AVIP_SETTING
    {

        /// <summary>
        /// 內容機型_alarm代碼
        /// </summary>
        public override void setALARM_CONT()
        {
            this.VIP_ALARMList = this.VIP_ALARMList.OrderBy(x => x.ORDER_BY).ToList();

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
                #region PB760
                if (this.VIP_ALARMList.Count > 0)
                {
                    if (alarmList.Count == this.VIP_ALARMList.Count)
                    {
                        for (int i = 0; i < alarmList.Count; i++)
                        {
                            if (alarmList[i].Trim() != this.VIP_ALARMList[i].ALARM_MSG)
                            {
                                msgList.Add(this.VIP_ALARMList[i].ALARM_DESC);
                            }
                        }
                    }
                }
                else
                {
                    msgList.Add(string.Join(",", alarmList));
                }
                #endregion
            }
            return msgList;
        }
    }
}

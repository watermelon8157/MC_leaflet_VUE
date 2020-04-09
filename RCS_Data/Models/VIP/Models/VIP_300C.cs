using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RCS_Data.Models.VIP.Models
{
    /// <summary>
    /// 300C 設定
    /// </summary>
    public class VIP_SETTING_300C : AVIP_SETTING
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
            #region 300C
            if (central_message.Length > 0)
            {
                if (central_message.Length == this.VIP_ALARMList.Count)
                {
                    for (int i = 0; i < central_message.Length; i++)
                    {
                        if (central_message.Substring(i, 1) == "1" && this.VIP_ALARMList.Count > i)
                        {
                            msgList.Add(this.VIP_ALARMList[i].ALARM_DESC);
                        }
                    }
                }
            }
            #endregion
            return msgList;
        }
    }


}

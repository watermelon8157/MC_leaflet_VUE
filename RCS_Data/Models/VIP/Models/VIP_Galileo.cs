using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RCS_Data.Models.VIP.Models
{
    /// <summary>
    /// Galileo
    /// </summary>
    public class VIP_SETTING_Galileo : AVIP_SETTING
    {
        public override void setTHisDeviceDATA(ref DB_VIPRTTBL item)
        {
            base.setTHisDeviceDATA(ref item);
            if (!string.IsNullOrWhiteSpace(item.respid) && item.respid.ToUpper().Contains("Galileo".ToUpper()))
            {
                if (item.respid.Split('-').Length > 1)
                {
                    item.respid = string.Concat("Galileo-", item.respid.Split('-')[1]);
                } 
            } 
        }
    }
}

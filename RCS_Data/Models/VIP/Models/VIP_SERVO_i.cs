using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RCS_Data.Models.VIP.Models
{


    /// <summary>
    /// SERVO_i
    /// </summary>
    public class VIP_SETTING_SERVO_i : AVIP_SETTING
    {
        public override void setTHisDeviceDATA(ref DB_VIPRTTBL item)
        {
            // Bi-Vent
            // Servoi 的Bivent 才會有Thigh、Tlow、Phigh、Plow
            // Plow是帶PEEP的值
            // Tlow是帶 t peep   = tlow  
            // Phigh你們目前有帶輸出
            if (item.mode.ToUpper().Contains("BI-VENT"))
            {
                // Plow是帶PEEP的值.
                item.plow = item.pressure_peep;
                if (!string.IsNullOrWhiteSpace(item.extend_item_obj.TPEEP))
                { 
                    item.tlow = item.extend_item_obj.TPEEP;
                } 
            }

        }
    }
}

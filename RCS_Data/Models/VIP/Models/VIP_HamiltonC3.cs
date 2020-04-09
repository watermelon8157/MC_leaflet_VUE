using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RCS_Data.Models.VIP.Models
{
 

    /// <summary>
    /// savina-300 設定 (尚未設定)
    /// </summary>
    public class VIP_SETTING_HamiltonC3 : AVIP_SETTING
    {
        public override void setTHisDeviceDATA(ref DB_VIPRTTBL item)
        {
            if (item.mode.ToUpper().Contains("SIMV"))
            {
                if (string.IsNullOrWhiteSpace(item.extend_item_obj.pinsp))
                {
                    // SIMV 點選PSYNC時，PC跟PS要有資料 
                    item.pressure_pc = item.extend_item_obj.pinsp; 
                } 
            }

        }
    }
}

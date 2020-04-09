using System; 
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RCS_Data.Models.VIP.Models
{
    public class VIP_SETTING_HamiltonG5 : AVIP_SETTING
    {
        string  csName = "VIP_SETTING_HamiltonG5";
        public override void setTHisDeviceDATA(ref DB_VIPRTTBL item)
        {
            // ASV模式
            if (item.mode.ToUpper().Contains("ASV"))
            {
                // SIMV 點選PSYNC時，PC跟PS要有資料 
                // 當監測值fspont
                // 大於0時，將右下圈圈內的prinsp(17)填至ps
                // 小於0時，填至pc
                if (!string.IsNullOrWhiteSpace(item.extend_item_obj.fspont))
                { 
                    try
                    {
                        if (!string.IsNullOrWhiteSpace(item.extend_item_obj.pinsp))
                        {
                            double fsp = double.Parse(item.extend_item_obj.fspont);
                            if (fsp > 0)
                            {
                                item.pressure_ps = item.extend_item_obj.pinsp;
                            }
                            if (fsp <= 0)
                            {
                                item.pressure_pc = item.extend_item_obj.pinsp;
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Com.Mayaminer.LogTool.SaveLogMessage(ex, "setTHisDeviceDATA", this.csName);
                    }
                }
                // ASV模式的MV set放%MinVol
                if (!string.IsNullOrWhiteSpace(item.extend_item_obj.mv_percent))
                {
                    item.mv_set = item.extend_item_obj.mv_percent;
                }
            }

        }
    }
}

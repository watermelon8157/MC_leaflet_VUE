using RCS_Data.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RCS.Models.ViewModel
{
    public class RTTakeoff : RCS_Data.Controllers.RtTakeoff.Models, RCS_Data.Controllers.RtTakeoff.Interface
    {
        /// <summary>
        /// 轉換JSON
        /// </summary>
        /// <param name="List"></param>
        /// <returns></returns>
        public override List<VM_RTTakeoffAssess> changeJson(List<VM_RTTakeoffAssess> List)
        {

            List<VM_RTTakeoffAssess> result = List;

            #region 轉換JSON

            foreach (var item in result)
            {
                
            }

            #endregion

            return result;
        }
    }
}
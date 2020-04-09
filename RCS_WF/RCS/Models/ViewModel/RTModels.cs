using Com.Mayaminer;
using mayaminer.com.library;
using RCS_Data.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RCS.Models.ViewModel
{
    public class RTViewModels : RCS_Data.Controllers.RT.RTModels
    {
        string csName = "RTViewModels";
        /// <summary>
        /// 取得病患資料詳細資料
        /// </summary>
        /// <param name="pli"></param>
        public void getPatDeatailData(ref List<PatientListItem> pli)
        {
            string actionName = "getPatDeatailData";
            try
            {
                #region 整理資料
                List<string> ipd_noList = pli.Select(x => SQLDefend.SQLString(x.ipd_no)).ToList();
                if (ipd_noList != null && ipd_noList.Count > 0)
                {
                    string ipd_str = string.Join(",", ipd_noList);

                    string sql = "";

                    if (this.DBLink.DBA.hasLastError )
                    {
                        LogTool.SaveLogMessage(this.DBLink.DBA.lastError, actionName, this.csName);
                    }
                    foreach (PatientListItem item in pli)
                    {
                        item.respid = item.machine.respid;
                        item.mode = item.machine.mode; 
                        RCS_CPT_DTL_NEW_ITEMS cptData = this.getCPTAssess(item.chart_no);
                        item.memo = string.IsNullOrWhiteSpace(cptData.other_history)? "": cptData.other_history;
                        item.diagnosis_code = string.IsNullOrWhiteSpace(cptData.now_pat_diagnosis) ? "" : cptData.now_pat_diagnosis;  
                    }
                }
                #endregion
                pli = pli.OrderBy(x => x.bed_no).ThenBy(x => x.chart_no).ToList();
            }
            catch (Exception ex)
            {
                LogTool.SaveLogMessage(ex, actionName, this.csName);
            }

        }


    }
}
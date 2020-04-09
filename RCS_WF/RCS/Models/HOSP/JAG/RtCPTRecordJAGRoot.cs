using System;
using System.ComponentModel;
using RCS.Models.HOSP.JAG;
using RCS_Data.Models;
using RCS_Data.Models.DB;
using RCS_Data.Models.JAG;
using RCS_Data.Models.ViewModels;
using RCSData.Models;

namespace RCS.Models.JAG
{

    public static partial class JAG_ROOT_NAME
    {
        public static string RtCPTRecordJAGRoot { get { return Function_Library.GetClassDisplayName<RtCPTRecordJAGRoot>(); } }
    }

    [DisplayName("RtCPTRecordJAGRoot")]
    public class RtCPTRecordJAGRoot : JAGRoot
    { 
        public RtCPTRecordJAGRoot()
        {

        }
        public RtCPTRecordJAGRoot(DB_RCS_RT_CASE pPat, UPLOADSTAYBYUPLOADLIST pUploadItem, UserInfo pUser) : base()
        {
            this.RequestDocType = "MRF0320040A004";
            this.RequestUser = pUser.user_id;
            this.RequestUserName = pUser.user_name;
            this.UserIDNO = pUser.user_idno;
            this.FileName = string.Concat(pUploadItem.DOC_NO, ".pdf"); 
            this.RequestDocDate = Function_Library.getDateString(DateTime.Parse(pUploadItem.RECORDDATE),DATE_FORMAT.yyyyMMdd);
            this.RequestDocTime = Function_Library.getDateString(DateTime.Parse(pUploadItem.RECORDDATE), DATE_FORMAT.HHmmss); 
            this.RequestDocRoot = pUploadItem.IPD_NO;
            this.RequestDocNo = pUploadItem.DOC_NO;
            this.RequestPatientID = pUploadItem.CHART_NO;
            this.RequestPatientName = pPat.PATIENT_NAME;
            this.VisitDate = string.IsNullOrWhiteSpace(pPat.DIAG_DATE) ? "" : Function_Library.getDateString(DateTime.Parse(pPat.DIAG_DATE), DATE_FORMAT.yyyyMMdd); 
            this.Category = pPat.PATIENT_SOURCE;
            this.DischargeDate = string.IsNullOrWhiteSpace(pPat.PRE_DISCHARGE_DATE) ? "" : Function_Library.getDateString(DateTime.Parse(pPat.PRE_DISCHARGE_DATE), DATE_FORMAT.yyyyMMdd);
        }
    }
}
 

 
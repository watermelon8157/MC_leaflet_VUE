using RCS_Data.Models.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RCS_Data.Models.ViewModels
{
    public class UPLOADLIST 
    {
        public string RECORD_KEY { get; set; }
        public string RECORD { get; set; }
        public string RECORD_NAME { get; set; }
        public string RECORD_ID { get; set; }
        public string CHART_NO { get; set; }
        public string PATIENT_NAME
        {
            get
            {
                return patData == null || string.IsNullOrWhiteSpace(patData.PATIENT_NAME) ? "" : patData.PATIENT_NAME; 
            } 
        }
        public string IPD_NO { get; set; }
        public string CREATE_ID { get; set; }
        public string CREATE_NAME { get; set; }
        public string RECORDDATE { get; set; }
        public string UPLOAD_STATUS { get; set; }
        public string MODIFY_DATE { get; set; } 
        public DB_RCS_RT_CASE patData {get;set;}
    }

    public class UPLOADSTAYBYUPLOADLIST : UPLOADLIST
    {
        public string DOC_NO { get; set; }
        public byte[] PDFbyte { get; set; }
    }

    public class UPLOADSTAYBYUPLOADLISTComparer : IEqualityComparer<UPLOADSTAYBYUPLOADLIST>
    {

        public bool Equals(UPLOADSTAYBYUPLOADLIST x, UPLOADSTAYBYUPLOADLIST y)
        {
            return (x.RECORD_ID == y.RECORD_ID);
        }

        public int GetHashCode(UPLOADSTAYBYUPLOADLIST obj)
        {
            throw new NotImplementedException();
        }
    }
}

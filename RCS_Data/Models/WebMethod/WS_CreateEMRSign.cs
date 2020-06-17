namespace RCSData.Models
{
    /// <summary>
    /// 輔大簽章功能
    /// </summary>
    public partial class WebMethod
    {
        /// <summary>
        /// 輔大簽章功能 CreateEMRSign
        /// </summary>
        /// <param name="iwp"></param>
        /// <param name="model"></param>
        /// <param name="pdf"></param>
        /// <returns></returns>
        public string CreateEMRSign(IWebServiceParam iwp, PARAMS_CreateEMRSign model  , byte[] pdf )
        {
            string actioName = "CreateEMRSign"; 
            ServiceResult<string> sr = new ServiceResult<string>();
            if (pdf == null)
            {
                sr.datastatus = RCS_Data.HISDataStatus.ParametersError;
                sr.errorMsg = "";
            }
            else
            {  
                WS_CreateEMRSign eo = new WS_CreateEMRSign(model, iwp);
                sr = HISData.getEMRServiceResult(eo);
            }
            this.datastatus = sr.datastatus;
            this.errorMsg = sr.errorMsg;
            if (sr.datastatus == RCS_Data.HISDataStatus.SuccessWithData)
            {
                return "已產生電子簽章資料";
            }
            return "電子簽章資失敗!";
        }
    }

    public class WS_CreateEMRSign : AwebMethod<string>, IwebMethod<string>
    {
        public string webMethodName { get { return this.iwp.webMethodName; } }
        public override string wsSession { get { return "RCS_WS_EMR"; } }


        /// <summary>
        /// WS_UploadEMRFile
        /// </summary>
        /// <param name="OrderStr"></param>
        /// <param name="Base64Str"></param>
        /// <param name="pIwp"></param>
        public WS_CreateEMRSign(PARAMS_CreateEMRSign model , IWebServiceParam pIwp )
        {
            this.iwp = pIwp;
            setParam();
            if (!string.IsNullOrWhiteSpace(model.EncntNo) && !string.IsNullOrWhiteSpace(model.HHISNum))
            {

                this.paramList["EncntNo"].paramValue = model.EncntNo;
                this.paramList["HHISNum"].paramValue = model.HHISNum;
                this.paramList["ApplyNO"].paramValue = model.ApplyNO;
                this.paramList["SignID"].paramValue = model.SignID;
                this.paramList["FormID"].paramValue = model.FormID;
                this.paramList["FilePath"].paramValue = model.FilePath;
                this.paramList["PDFFileName"].paramValue = model.PDFFileName;
                this.paramList["SYSTEMID"].paramValue = model.SYSTEMID;  

            }
            else
            {
                this.ServiceResult.datastatus = RCS_Data.HISDataStatus.ParametersError;
                if (string.IsNullOrWhiteSpace(model.EncntNo))
                {
                    this.ServiceResult.errorMsg = "就診號 不可為空值!";
                }
                if (string.IsNullOrWhiteSpace(model.HHISNum))
                {
                        this.ServiceResult.errorMsg = "病歷號 不可為空值!";
                }
            }
            
        }

        public void setParam()
        {
            this.ServiceResult = new ServiceResult<string>();
            this.paramList = this.iwp.paramList;
            this.returnValue = this.iwp.returnValue;
        }
    }

    /// <summary>
    /// 傳入參數物件
    /// </summary>
    public class PARAMS_CreateEMRSign
    {
        public string EncntNo { get; set; }
        public string HHISNum { get; set; }
        public string ApplyNO { get; set; }
        public string SignID { get; set; }
        public string FormID { get; set; }
        public string FilePath { get; set; }
        public string PDFFileName { get; set; }
        public string SYSTEMID { get; set; }

    }
}

using Com.Mayaminer;
using RCS.Models.JAG;
using RCS_Data;
using RCS_Data.Controllers.Upload;
using RCS_Data.Models;
using RCS_Data.Models.DB;
using RCS_Data.Models.JAG;
using RCS_Data.Models.ViewModels;
using RCSData.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Web;

namespace RCS.Models.ViewModel
{
    public class Upload : RCS_Data.Controllers.Upload.Models
    {
        string csName = "RCS.Models.Upload";
        public override RESPONSE_MSG UpLoadDataToHsop(UPLOADLIST item )
        {
            string actionName = "UpLoadDataToHsop";
            RESPONSE_MSG rm = new RESPONSE_MSG();
            
            return rm;
        }

        /// <summary>
        /// 上傳Thread
        /// </summary>
        public override void RunThread()
        {
            string actionName = "RunThread";
            RESPONSE_MSG rm = new RESPONSE_MSG();
            string HtmlStr = "", url = "", querySql = "", wsResult = "";
            Dapper.DynamicParameters dp = new Dapper.DynamicParameters(); 
            UserInfo userData = null;
            DB_RCS_RT_CASE patData = null;
            UPLOADSTAYBYUPLOADLISTComparer ucp = new UPLOADSTAYBYUPLOADLISTComparer();
            exportFile efm = new exportFile("DownloadPdf.pdf");  
            IDocSetting ds = null;
            List<UPLOADLIST> pList = this.getYearUploadList(ref rm , new List<string>() { "2" , "5" });
            string getUrl = IniFile.GetConfig("System", "PDFURL"); 
            List<byte[]> byteList = new List<byte[]>(); 
            foreach (UPLOADLIST item in pList)
            {  
               UPLOADSTAYBYUPLOADLIST temp = Newtonsoft.Json.JsonConvert.DeserializeObject<UPLOADSTAYBYUPLOADLIST>(Newtonsoft.Json.JsonConvert.SerializeObject(item));
                #region 取得PDF資料
                if (!MvcApplication.uploadList.Contains(temp, ucp))
                {
                    HtmlStr = "";
                    url = "";
                    switch (item.RECORD_NAME)
                    {
                        case "DB_RCS_RECORD_MASTER": 
                            url = string.Format("RTRecordPageForm?record_id={0}&chart_no={1}&ipd_no={2}", item.RECORD_ID, item.CHART_NO, item.IPD_NO);
                            ds = new CPTNewRecordFormPDFDocSetting();
                            break;
                        case "DB_RCS_CPT_ASS_MASTER":
                            url = string.Format("CPTPageForm?cpt_id={0}", item.RECORD_ID);
                            ds = new CPTNewRecordFormPDFDocSetting();
                            break;
                        case "DB_RCS_WEANING_ASSESS":
                            url = string.Format("RtTakeOffPageForm?tk_id={0}&chart_no={1}&ipd_no={2}", item.RECORD_ID, item.CHART_NO, item.IPD_NO);
                            ds = new CPTNewRecordFormPDFDocSetting();
                            break;
                        case "DB_RCS_CPT_NEW_RECORD":
                            url = string.Format("rtCPTRecordPageForm?id={0}&chart_no={1}&ipd_no={2}", item.RECORD_ID, item.CHART_NO, item.IPD_NO);
                            ds = new CPTNewRecordFormPDFDocSetting();
                            break;
                        default:
                            break;
                    }
                    using (System.Net.WebClient client = new System.Net.WebClient())
                    {
                        client.Encoding = Encoding.UTF8; // 設定Webclient.Encoding 
                        HtmlStr = client.DownloadString(string.Concat(getUrl, url));
                    }
                    temp.PDFbyte = efm.exportPDF(HtmlStr, ds).FileContents;
                    if (temp.PDFbyte != null)
                    {
                        MvcApplication.uploadList.Enqueue(temp);
                    }
                }
                #endregion 
            }
            while (MvcApplication.uploadList.Count > 0)
            {
                UPLOADSTAYBYUPLOADLIST upItem = MvcApplication.uploadList.Dequeue();
                wsResult = "";
                if (upItem.patData != null &&
                    MvcApplication.userList.Exists(x => x.user_id == upItem.CREATE_ID) )
                { 
                    userData = MvcApplication.userList.Find(x => x.user_id == upItem.CREATE_ID) ;
                    upItem.DOC_NO = this.DBLink.GetJAR_DOC_NO(upItem.RECORD_ID);
                    #region 上傳簽章
                    switch (upItem.RECORD_NAME)
                    {
                        case "DB_RCS_RECORD_MASTER":
                            wsResult = this.webmethod.UploadEMRFile<RtRecordJAGRoot>(
                                new RCSData.Models.WebService.UploadEMRFile(),
                                new RtRecordJAGRoot(
                                    upItem.patData,
                                    upItem,
                                    userData),
                                JAG_ROOT_NAME.RtRecordJAGRoot,
                                upItem.PDFbyte
                                );
                            break;
                        case "DB_RCS_CPT_ASS_MASTER":
                            wsResult = this.webmethod.UploadEMRFile<RtAssessJAGRoot>(
                                new RCSData.Models.WebService.UploadEMRFile(),
                                new RtAssessJAGRoot(
                                    upItem.patData,
                                    upItem,
                                    userData),
                                JAG_ROOT_NAME.RtAssessJAGRoot,
                                upItem.PDFbyte
                                );
                            break;
                        case "DB_RCS_WEANING_ASSESS":
                            wsResult = this.webmethod.UploadEMRFile<RTTakeoffJAGRoot>(
                              new RCSData.Models.WebService.UploadEMRFile(),
                              new RTTakeoffJAGRoot(
                                  upItem.patData,
                                  upItem,
                                  userData),
                              JAG_ROOT_NAME.RTTakeoffJAGRoot,
                              upItem.PDFbyte
                              );
                            break;
                        case "DB_RCS_CPT_NEW_RECORD":
                            wsResult = this.webmethod.UploadEMRFile<RtCPTRecordJAGRoot>(
                             new RCSData.Models.WebService.UploadEMRFile(),
                             new RtCPTRecordJAGRoot(
                                 upItem.patData,
                                 upItem,
                                 userData),
                             JAG_ROOT_NAME.RtCPTRecordJAGRoot,
                             upItem.PDFbyte
                             );
                            break;
                        default:
                            break;
                    }
                    #endregion
                    #region 簽章結果
                    switch (wsResult)
                    {
                        case "-1":
                            // 上傳成功
                            this.UpDateData(userData, new List<string>() { upItem.RECORD_KEY }, "1");
                            break;
                        case "1":
                            // 無資料 
                        case "2":
                            // 本文檔格式不符 
                        case "3":
                            // 指令檔格式不符 
                        case "4":
                            // 其他錯誤 
                            LogTool.SaveLogMessage("上傳簽章錯誤!", actionName, this.csName);
                            LogTool.SaveLogMessage(wsResult, actionName, this.csName);
                            LogTool.SaveLogMessage(upItem.RECORD_ID, actionName, this.csName); 
                            break; 
                        default:
                            break;
                    }
                    #endregion 
                }
                
            }
           
            base.RunThread();
        }

        /// <summary>
        /// 取得上傳清單
        /// </summary>
        /// <param name="rm"></param>
        /// <param name="sDate"></param>
        /// <param name="eDate"></param>
        /// <param name="statusList"></param>
        /// <returns></returns>
        protected override List<UPLOADLIST> getUploadList(ref RESPONSE_MSG rm, string sDate, string eDate, List<string> statusList)
        {
            if (statusList == null || statusList.Count == 0)
            {
                statusList = new List<string>() { "0", "2", "3", "4", "5" };
            }
            string actionName = "getUploadList";
            List<UPLOADLIST> pList = new List<UPLOADLIST>();
            try
            {
                DateTime dateNow = DateTime.Now;
                sDate = string.IsNullOrWhiteSpace(sDate) && !mayaminer.com.library.DateHelper.isDate(sDate) ?
                     Function_Library.getDateString(dateNow.AddDays(-1), DATE_FORMAT.yyyy_MM_dd_000000) : Function_Library.getDateString(DateTime.Parse(sDate), DATE_FORMAT.yyyy_MM_dd_000000);
                eDate = string.IsNullOrWhiteSpace(eDate) && !mayaminer.com.library.DateHelper.isDate(eDate) ?
                    Function_Library.getDateString(dateNow.AddDays(1), DATE_FORMAT.yyyy_MM_dd_125959) : Function_Library.getDateString(DateTime.Parse(eDate).AddDays(1), DATE_FORMAT.yyyy_MM_dd_125959);

                Dapper.DynamicParameters dp = new Dapper.DynamicParameters();
                List<string> sqlList = new List<string>();
                sqlList.Add(string.Format(string.Concat(
                    "SELECT '{0},' + {1} + ',{1}' as RECORD_KEY,'{0}' as RECORD,'DB_RCS_RECORD_MASTER' as RECORD_NAME,{1} as RECORD_ID,{2} as RECORDDATE",
                    ",CHART_NO,IPD_NO,CREATE_ID,CREATE_NAME ,UPLOAD_STATUS,MODIFY_DATE",
                    " FROM {0} WHERE {2} BETWEEN @sDate AND @eDate AND DATASTATUS = '1' AND UPLOAD_STATUS in @UPLOAD_STATUS"),
                    DB_TABLE_NAME.DB_RCS_RECORD_MASTER, "RECORD_ID", "RECORDDATE"));
                sqlList.Add(string.Format(string.Concat(
                    "SELECT '{0},' + {1} + ',{1}' as RECORD_KEY,'{0}' as RECORD,'DB_RCS_CPT_ASS_MASTER' as RECORD_NAME,{1} as RECORD_ID,{2} as RECORDDATE",
                    ",CHART_NO,IPD_NO,CREATE_ID,CREATE_NAME ,UPLOAD_STATUS,MODIFY_DATE",
                    " FROM {0} WHERE {2} BETWEEN @sDate AND @eDate AND DATASTATUS = '1' AND UPLOAD_STATUS in @UPLOAD_STATUS"),
                    DB_TABLE_NAME.DB_RCS_CPT_ASS_MASTER, "CPT_ID", "RECORD_DATE"));
                sqlList.Add(string.Format(string.Concat(
                    "SELECT '{0},' + {1} + ',{1}' as RECORD_KEY,'{0}' as RECORD,'DB_RCS_WEANING_ASSESS' as RECORD_NAME,{1} as RECORD_ID,{2} as RECORDDATE",
                    ",CHART_NO,IPD_NO,CREATE_ID,CREATE_NAME ,UPLOAD_STATUS,MODIFY_DATE",
                    " FROM {0} WHERE {2} BETWEEN @sDate AND @eDate AND DATASTATUS = '1' AND UPLOAD_STATUS in @UPLOAD_STATUS"),
                    DB_TABLE_NAME.DB_RCS_WEANING_ASSESS, "TK_ID", "REC_DATE"));
                sqlList.Add(string.Format(string.Concat(
                    "SELECT '{0},' + {1} + ',{1}' as RECORD_KEY,'{0}' as RECORD,'DB_RCS_CPT_NEW_RECORD' as RECORD_NAME,{1} as RECORD_ID,{2} as RECORDDATE",
                    ",CHART_NO,IPD_NO,CREATE_ID,CREATE_NAME ,UPLOAD_STATUS,MODIFY_DATE",
                    " FROM {0} WHERE {2} BETWEEN @sDate AND @eDate AND DATASTATUS = '1' AND UPLOAD_STATUS in @UPLOAD_STATUS"),
                    DB_TABLE_NAME.DB_RCS_CPT_NEW_RECORD, "CPT_ID", "REC_DATE"));
                dp.Add("sDate", sDate);
                dp.Add("eDate", eDate);
                dp.Add("UPLOAD_STATUS", statusList);
                string query = string.Join(" UNION ", sqlList);
                pList = this.DBLink.DBA.getSqlDataTable<UPLOADLIST>(query, dp);

                List<DB_RCS_RT_CASE> patList = new List<DB_RCS_RT_CASE>();
                string querySql = string.Format("SELECT * FROM {0} WHERE IPD_NO in @IPD_NO", DB_TABLE_NAME.DB_RCS_RT_CASE);
                dp.Add("IPD_NO", pList.Select(x => x.IPD_NO).Distinct());
                patList = this.DBLink.DBA.getSqlDataTable<DB_RCS_RT_CASE>(querySql, dp);
                foreach (UPLOADLIST item in pList)
                {
                    if (patList.Exists(x => x.IPD_NO == item.IPD_NO))
                    {
                        item.patData = patList.Find(x => x.IPD_NO == item.IPD_NO);
                    }
                }
                if (this.DBLink.DBA.hasLastError)
                {
                    rm.status = RESPONSE_STATUS.ERROR;
                    rm.messageList.Add(this.DBLink.DBA.lastError);
                    LogTool.SaveLogMessage(this.DBLink.DBA.lastError, actionName, csName);
                }
            }
            catch (Exception ex)
            {
                rm.status = RESPONSE_STATUS.ERROR;
                rm.messageList.Add(ex.Message);
                LogTool.SaveLogMessage(ex, actionName, csName);
            }

            return pList;
        }
    }
}
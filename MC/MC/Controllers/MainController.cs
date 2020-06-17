using Com.Mayaminer;
using RCS_Data;
using System;
using System.Web.Mvc;
using RCSData.Models;

namespace RCS.Controllers
{
    public class MainController : BaseController {
         
        /// <summary>
        /// 登入畫面
        /// </summary>
        /// <returns></returns>
        [Silencer(CheckPat_infoSession = false,CheckUser_infoSession =false)]
        public ActionResult Index() {
             
            //WebMethod wm = new WebMethod();
            #region MyRegion

            //Models.ViewModel.RtAssessJAGRoot jr = new Models.ViewModel.RtAssessJAGRoot();
            //byte[] buf = null;
            //string HtmlStr = "";
            //IDocSetting ds = new CHGHRTRecordFormPDFDocSetting();
            //string getUrl = IniFile.GetConfig("System", "MVCURL");
            //using (System.Net.WebClient client = new System.Net.WebClient())
            //{
            //    client.Encoding = Encoding.UTF8; // 設定Webclient.Encoding
            //    HtmlStr = client.DownloadString(string.Concat(getUrl,
            //        string.Format("PDFHtml/RTRecordPageForm?record_id={0}&chart_no={1}&ipd_no={2}",
            //          "2020010710052746745rcsI0333037", "05945519", "I0333037"
            //        )));
            //}
            //buf = RCS.Controllers.WEBAPI.BasicController.exportPDF(HtmlStr, ds, "<div>我是分頁</div>");
            //wm.UploadEMRFile<Models.ViewModel.RtAssessJAGRoot>(new RCSData.Models.WebService.UploadEMRFile(), jr, "RtAssessJAGRoot", buf);

            #endregion  
            #region WF

            //List<BedArea> pList1 = wm.getHisBedAreaList();
            //LogTool.SaveLogMessage(JsonConvert.SerializeObject(pList1), "Index", "MainController");
            //List<IPDPatientInfo> pList2 = wm.getPatientInfoList("", "", "872C");
            //LogTool.SaveLogMessage(JsonConvert.SerializeObject(pList2), "Index", "MainController");
            //List<IPDPatientInfo> pList3 = wm.getPatientInfoList("00446149", "I0822112", "");
            //LogTool.SaveLogMessage(JsonConvert.SerializeObject(pList3), "Index", "MainController");
            //List<RCS_ExamData_Common> pList4 = wm.getLabData("00446149", "I0822112", "2019-10-20", "2019-10-30");
            //LogTool.SaveLogMessage(JsonConvert.SerializeObject(pList4), "Index", "MainController");
            //List<PatOrder> pList5 = wm.getShiftOrderList("00446149", "I0822112");
            //LogTool.SaveLogMessage(JsonConvert.SerializeObject(pList5), "Index", "MainController");
            //string ipd_no = "";
            //List<PatientHistory> pList6 = wm.getPatientHistory("00446149", ref ipd_no);
            //LogTool.SaveLogMessage(JsonConvert.SerializeObject(pList6), "Index", "MainController");
            //UserInfo pList7 = wm.checkLoginUser("101383", "123456");
            //LogTool.SaveLogMessage(JsonConvert.SerializeObject(pList7), "Index", "MainController");
            //List<VitalSign> pList8 = wm.getVitalSign("00446149", "I0822112", "2019-10-20", "2019-10-30");
            //LogTool.SaveLogMessage(JsonConvert.SerializeObject(pList8), "Index", "MainController");

            #endregion
            #region SHH

            // List<BedArea> pList1 = wm.getHisBedAreaList();
            // LogTool.SaveLogMessage(JsonConvert.SerializeObject(pList1), "Index", "MainController");
            // List<IPDPatientInfo> pList2 = wm.getPatientInfoList("", "", "86D0");
            // LogTool.SaveLogMessage(JsonConvert.SerializeObject(pList2), "Index", "MainController");
            // List<PatProgress> pList9 = wm.get7190OrderList("2019-10-20", "2019-10-30");
            // LogTool.SaveLogMessage(JsonConvert.SerializeObject(pList9), "Index", "MainController");
            // List<IO_PUT> pList10 = wm.getInputOutput("11728841", "I0832461", "2019-10-20", "2019-10-30");
            // LogTool.SaveLogMessage(JsonConvert.SerializeObject(pList10), "Index", "MainController");
            //List<RCS_ExamData_Common> pList4 = wm.getLabData("11728841", "I0832461", "2019-10-20", "2019-10-30");
            //LogTool.SaveLogMessage(JsonConvert.SerializeObject(pList4), "Index", "MainController");
            //List<PatOrder> pList11 = wm.getShiftOrderList("11728841", "I0832461" );
            //LogTool.SaveLogMessage(JsonConvert.SerializeObject(pList11), "Index", "MainController");
            //List<PatOperation> pList12 = wm.getPatOperationList("11728841", "I0832461");
            //LogTool.SaveLogMessage(JsonConvert.SerializeObject(pList12), "Index", "MainController");
            // List<IPDPatientInfo> pList3 = wm.getPatientInfoList("11728841", "I0832461", "");
            // LogTool.SaveLogMessage(JsonConvert.SerializeObject(pList3), "Index", "MainController"); 
            // string ipd_no = "";
            // List<PatientHistory> pList6 = wm.getPatientHistory("11728841", ref ipd_no);
            // LogTool.SaveLogMessage(JsonConvert.SerializeObject(pList6), "Index", "MainController");
            //List<ExamReport> pList13 = wm.getReportData("11728841", "I0832461", "2019-10-20", "2019-10-30");
            //LogTool.SaveLogMessage(JsonConvert.SerializeObject(pList13), "Index", "MainController");
            // UserInfo pList7 = wm.checkLoginUser("11515", "115155");
            // LogTool.SaveLogMessage(JsonConvert.SerializeObject(pList7), "Index", "MainController");
            //Dictionary<string, string> vitalSingKey = new Dictionary<string, string>();
            //List<VitalSign> pList8 = wm.getVitalSign("11728841", "I0832461", "2019-10-20",ref vitalSingKey, "2019-10-30");
            // LogTool.SaveLogMessage(JsonConvert.SerializeObject(pList8), "Index", "MainController");

            #endregion

            try
            {
                Session.RemoveAll();
                
                //傳入值
                //檢察是否有訊息
                if (Request["message"] != null)
                    TempData["message"] = Request["message"].ToString();
                if (TempData["message"] != null)
                    TempData["message"] = TempData["message"].ToString();

#if DEBUG
                ViewData["userid"] = "rcs";
                ViewData["userpwd"] = "!QAZ2wsx";
#else
            ViewData["userid"] = "";
            ViewData["userpwd"] = "";
#endif

                DateTime Jan1st1970 = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
                long ooo = (long)(DateTime.Now - Jan1st1970).TotalMilliseconds;
                var posixTime = DateTime.SpecifyKind(new DateTime(1970, 1, 1), DateTimeKind.Local);
                var time = posixTime.AddMilliseconds(1486450927320);
                string t = time.ToString("yyyy/MM/dd HH:mm:ss:fff");

            }
            catch (Exception ex)
            {
                LogTool.SaveLogMessage(ex, "Index", "MainController");
                TempData["message"] = "程式發生錯誤，請洽資訊人員!";
            }

            return View();
        }

        /// <summary>
        /// 進入模組
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ActionResult EnterModule()
        {
            //如果有登入訊息，成功登入後顯示
            if(!string.IsNullOrWhiteSpace(Request["loginMsg"]))
            {
                TempData["loginMsg"] = Request["loginMsg"].ToString();
            }
            //目前只有定義 RT
            if (Request["module"] != null && user_info != null)
            {
                switch (Request["module"].ToString())
                {
                    case "RT":
                        return RedirectToAction("Index", "RT");
                    case "Admin":
                        return RedirectToAction("Index", "Admin");
                    default:
                        return RedirectToAction("Index", "Main");
                }
            }
            else
            {
                return RedirectToAction("Index", "Main", new { message = "非法進入" });
            }
        }

        /// <summary>
        /// 檢查登入使用者
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Silencer(CheckPat_infoSession = false, CheckUser_infoSession = false)]
        public ActionResult LogCheck()
        {

            RESPONSE_MSG rm = new RESPONSE_MSG();
            rm.status = RESPONSE_STATUS.ERROR;
            if (Request["user_id"] != null || Request["user_pwd"] != null)
            {
                //傳入值
                string user_id = Request["user_id"];
                string user_pwd = Request["user_pwd"];

                if(!string.IsNullOrWhiteSpace(user_id) && !string.IsNullOrWhiteSpace(user_pwd))
                {
                    //工作
                    UserInfo ui = new UserInfo();
                    ui = ui.getUserInfo(this.hospFactory.webService.HisLoginUser(), user_id, user_pwd);


                    //取得結果
                    if (ui.hasUserData)
                    {
                        if (!string.IsNullOrWhiteSpace(ui.loginMsg))
                        {
                            rm.message = ui.loginMsg;
                        }
                        rm.status = RESPONSE_STATUS.SUCCESS;
                        ui.user_id = ui.user_id.Trim();
                        Session["user_info"] = ui;
                    }
                    else
                    {
                        rm.status = RESPONSE_STATUS.ERROR;
                        rm.message = ui.loginMsg;
                    }
                }
                else
                {
                    rm.message = "請輸入帳號密碼!";
                    rm.status = RESPONSE_STATUS.ERROR;
                }
            }
            else
            {
                rm.message = "請輸入帳號密碼!";
                rm.status = RESPONSE_STATUS.ERROR;
            }
            return Content(rm.get_json());
        }

        /// <summary>
        /// 登出
        /// </summary>
        /// <returns></returns>
        [Silencer(CheckPat_infoSession = false, CheckUser_infoSession = false)]
        public EmptyResult LogOut() {
            Session.RemoveAll();
            if (TempData["message"] == null)
            {
                TempData["message"] = "登出系統!";
            }
            else
            {
                TempData["message"] = TempData["message"];
            }
            return new EmptyResult();
        }

  
         
    }
}

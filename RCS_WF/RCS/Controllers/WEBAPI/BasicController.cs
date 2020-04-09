using Com.Mayaminer;
using iTextSharp.text;
using iTextSharp.text.pdf;
using iTextSharp.tool.xml;
using RCS.Models;
using RCS_Data;
using RCSData.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Web;
using System.Web.Http;

namespace RCS.Controllers.WEBAPI
{
    /// <summary>
    /// BasicAPI
    /// </summary>
    public class BasicController : ApiController
    {


        ///// <summary>
        ///// 加入病患的搜尋病患功能是否補0的功能(true:開啟,false:關閉)
        ///// </summary>
        //public static bool aadPatientCheckChartNo = bool.Parse(IniFile.GetConfig("SystemConfig", "aadPatientCheckChartNo"));

        private RCS_Data.Models.BasicFunction _bf { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public RCS_Data.Models.BasicFunction basicfunction
        {
            get
            {
                if (this._bf == null)
                {
                    this._bf = new RCS_Data.Models.BasicFunction();
                }
                return this._bf;
            }
        }

        private RCS.Models.HospSetDDL _select { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public RCS.Models.HospSetDDL select
        {
            get
            {
                if (this._select == null)
                {
                    this._select = new RCS.Models.HospSetDDL();
                }
                return this._select;
            }
        }

        private SQLProvider _DBLink { get; set; }
        protected SQLProvider DBLink
        {
            get
            {
                if (this._DBLink == null)
                {
                    this._DBLink = new SQLProvider();
                }
                return this._DBLink;
            }
        }

        private WebMethod _webmethod { get; set; }
        protected WebMethod webmethod
        {
            get
            {
                if (this._webmethod == null)
                    this._webmethod = new WebMethod();
                return this._webmethod;
            }
        }

        private Models.HOSP.HospFactory _hospFactory { get; set; }
        protected Models.HOSP.HospFactory hospFactory
        {
            get
            {
                if (this._hospFactory == null)
                {
                    this._hospFactory = new Models.HOSP.HospFactory();
                }
                return this._hospFactory;
            }
        }

        /// <summary>
        /// 驗證Auth
        /// </summary>
        /// <param name="form"></param>
        /// <returns></returns>
        [JwtAuthActionFilterAttribute]
        [HttpPost]
        public string JwtAuthCheck(JWT_Form_Body form)
        {
            return "合法登入!";
        }


        /// <summary>
        /// 登入驗證
        /// </summary>
        /// <param name="form">登入資料</param>
        /// <returns></returns>
        [JwtAuthActionFilterAttribute(notVerification = true)]
        public object Login(Login_Form_Body form)
        {
            if (!string.IsNullOrWhiteSpace(form.userName) && !string.IsNullOrWhiteSpace(form.password))
            {

                UserInfo user_info = this.webmethod.checkLoginUser(this.hospFactory.webService.HisLoginUser(),form.userName, form.password);
                if (user_info.hasUserData )
                {
                    if (!string.IsNullOrWhiteSpace(user_info.sysAuthority))
                    {
                        if (!MvcApplication.userList.Exists(x=>x.user_id == user_info.user_id))
                        {
                            MvcApplication.userList.Add(user_info);
                        }
                        return new
                        {
                            Result = true,
                            token = JwtAuthActionFilterAttribute.EncodeToken(new PAYLOAD()
                            {
                                user_id = user_info.user_id.Trim(),
                                user_name = user_info.user_name.Trim(),
                                role = user_info.sysAuthority
                            })
                        };
                    }
                    this.throwHttpResponseException("查無使用者使用權限!!");

                }
                this.throwHttpResponseException("查無使用者資料!!");
            }
            else
            {
                this.throwHttpResponseException("請輸入帳號或密碼!!");
            }
            this.throwHttpResponseException("帳號或密碼錯誤!!");
            return false;
        }
         

        /// <summary>
        /// HTML匯出PDF
        /// </summary>
        /// <param name="form"></param>
        /// <returns></returns>
        [JwtAuthActionFilterAttribute(notVerification = true)]
        [HttpPost]
        public HttpResponseMessage ExportPDF(PDF_FORM_BODY form)
        {
            HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.BadRequest);


            //IDocSetting ds = new RTRecordFormPDFDocSetting();
            IDocSetting ds = new RTCPTCXRFormPDFDocSetting();
            ds = new CPTNewRecordFormPDFDocSetting();

            exportFile efm = new exportFile("DownloadPdf.pdf");



            //byte[] buf = this.exportPDF(HttpUtility.UrlDecode(form.HtmlStr), ds);
            var rowStr = form.HtmlStr;

            string TitleImage = "";
            if (RCS.Controllers.BaseController.isBasicMode)
            {
                TitleImage = "FJU_P.jpg";
            }
            else
            {
                TitleImage = "Valuation_img.png";
            }
            string imgStr = string.Concat("<img src=\"" + IniFile.GetConfig("System", "MVCURL"), "Images/" + TitleImage,
                 "\" height=50 width=350  />");
            rowStr = rowStr.Replace("[$AA$]", imgStr);


            byte[] buf = efm.exportPDF(rowStr, ds).FileContents;
            if (buf != null)
            {
                var contentLength = buf.Length;
                var statuscode = HttpStatusCode.OK;
                response = Request.CreateResponse(statuscode);
                response.Content = new StreamContent(new MemoryStream(buf));
                response.Content.Headers.ContentType = new MediaTypeHeaderValue("application/pdf");
                response.Content.Headers.ContentLength = contentLength;
                ContentDispositionHeaderValue contentDisposition = null;
                if (ContentDispositionHeaderValue.TryParse("inline; filename=" + form.pdfFIleName + ".pdf", out contentDisposition))
                {
                    response.Content.Headers.ContentDisposition = contentDisposition;
                }

            }
            else
            {
                this.throwHttpResponseException("程式發生錯誤，請洽資訊人員!");
            }

            return response;
        }

        /// <summary>
        /// 產生PDF byte[]
        /// </summary>
        /// <param name="htmlText"></param> 
        /// <param name="exportFile">匯出檔案</param>
        /// <returns></returns>
        public static byte[] exportPDF(string htmlText, IDocSetting ds , string SplithtmlText = "我是分頁", bool exportFile = false)
        {
            byte[] buf = null;

            if (string.IsNullOrEmpty(htmlText))
            {
                return null;
            }
            //避免當htmlText無任何html tag標籤的純文字時，轉PDF時會掛掉，所以一律加上<p>標籤
            //htmlText = "<p>" + htmlText + "</p>";
            string[] pageHtml = System.Text.RegularExpressions.Regex.Split(htmlText, SplithtmlText);

            Document doc = new Document(iTextSharp.text.PageSize.A4);//要寫PDF的文件，建構子沒填的話預設直式A4 
            //指定文件預設開檔時的縮放為100%
            PdfDestination pdfDest = new PdfDestination(PdfDestination.XYZ, 0, doc.PageSize.Height, 1f);
            //開啟Document文件 
            if (ds.SetMargins)
            {
                doc.SetMargins(ds.marginLeft, ds.marginRight, ds.marginTop, ds.marginBottom);
            }


            MemoryStream outputStream = new MemoryStream();//要把PDF寫到哪個串流
            PdfWriter writer = PdfWriter.GetInstance(doc, outputStream);
            doc.Open();

            foreach (string str in pageHtml)
            {
                if (str == "\r\n" && pageHtml.Length > 1) { continue; }
                doc.NewPage();
                int page = 1;
                string text = string.Format("{0}" + str + "{1}", "<p>", "</p>");
                byte[] data = Encoding.UTF8.GetBytes(text);//字串轉成byte[]
                MemoryStream msInput = new MemoryStream(data);



                //使用XMLWorkerHelper把Html parse到PDF檔裡
                Stream fs = null;
                string sourcePath = "";
                string sourceFile = "";
                if (!string.IsNullOrWhiteSpace(ds.cssFile))
                {
                    sourcePath = System.Web.Hosting.HostingEnvironment.MapPath("~\\StyleSheet\\");
                    sourceFile = System.IO.Path.Combine(sourcePath, ds.cssFile);
                    fs = System.IO.File.OpenRead(sourceFile);
                }

                //使用XMLWorkerHelper把Html parse到PDF檔裡
                try
                {
                    XMLWorkerHelper.GetInstance().ParseXHtml(writer, doc, msInput, fs, Encoding.UTF8, new UnicodeFontFactory());
                }
                catch (Exception ex)
                {
                    Com.Mayaminer.LogTool.SaveLogMessage(ex, "exportPDF", "exportPDF");
                }

                //將pdfDest設定的資料寫到PDF檔
                PdfAction action = PdfAction.GotoLocalPage(page, pdfDest, writer);
                writer.SetOpenAction(action);
                msInput.Close();
                page++;
            }
            doc.Close();
            outputStream.Close();
            //回傳PDF檔案 
            buf = outputStream.ToArray();


            return buf;
        }

        /// <summary>
        /// 回傳物件
        /// </summary>
        /// <param name="msg"></param>
        [JwtAuthActionFilterAttribute(notVerification = true)]
        protected object returnObj(RESPONSE_MSG pRm)
        {
            if (pRm.status != RESPONSE_STATUS.SUCCESS)
            {
                this.throwHttpResponseException(pRm.lastError); 
            }
            return pRm.attachment;

        }
        /// <summary>
        /// 回傳物件
        /// </summary>
        /// <param name="msg"></param>
        [JwtAuthActionFilterAttribute(notVerification = true)]
        protected T returnObj<T>(RESPONSE_MSG<T> pRm)
        {
            if (pRm.status != RESPONSE_STATUS.SUCCESS)
            {
                this.throwHttpResponseException(pRm.lastError);
            }
            return pRm.attachment;

        }

        /// <summary>
        /// 程式發生錯誤，回拋錯誤訊息給使用者
        /// </summary>
        /// <param name="msg"></param>
        [JwtAuthActionFilterAttribute(notVerification = true)]
        protected void throwHttpResponseException(string msg)
        {
            var resp = new System.Net.Http.HttpResponseMessage(System.Net.HttpStatusCode.NotFound)
            {
                Content = new System.Net.Http.StringContent(string.Format(msg))
            };
            throw new System.Web.Http.HttpResponseException(resp);
        }
    }

    #region Form Body Class
    public class JWT_Form_Body :AUTH
    {
        
    }


    public class Login_Form_Body
    {
        public string userName { get; set; }

        public string password { get; set; }
    }

    public class PDF_FORM_BODY
    {
        public string pdfFIleName { get; set; }
        public string HtmlStr { get; set; }
    }

    #endregion

    /// <summary>
    /// PDF文字設定
    /// </summary>
    public class UnicodeFontFactory : FontFactoryImp
    {
        private static readonly string arialFontPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Fonts),
            "arialuni.ttf");//arial unicode MS是完整的unicode字型。
        private static readonly string 標楷體Path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Fonts),
          "KAIU.TTF");//標楷體
        public override Font GetFont(
            string fontname
            , string encoding
            , bool embedded
            , float size, int style
            , BaseColor color
            , bool cached
        )
        {
            //可用Arial或標楷體，自己選一個
            BaseFont baseFont = BaseFont.CreateFont(標楷體Path, BaseFont.IDENTITY_H, BaseFont.EMBEDDED);
            return new Font(baseFont, size, style, color);
        }
    }
}

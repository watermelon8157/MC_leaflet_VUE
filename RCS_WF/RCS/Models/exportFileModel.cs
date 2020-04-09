using Com.Mayaminer;
using iTextSharp.text;
using iTextSharp.text.pdf;
using iTextSharp.tool.xml;
using iTextSharp.tool.xml.css;
using iTextSharp.tool.xml.html;
using iTextSharp.tool.xml.parser;
using iTextSharp.tool.xml.pipeline.css;
using iTextSharp.tool.xml.pipeline.end;
using iTextSharp.tool.xml.pipeline.html;
using RCS_Data;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Text;
using System.Web.Mvc;
using System.Xml;
using System.Xml.Serialization;

namespace RCS.Models
{
    /// <summary>
    /// 匯出檔案model
    /// 所有相關匯出檔案的方法都會在這裡
    /// </summary>
    public class exportFile
    {
        private GetLogToolCS csName { get; set; }
        public RESPONSE_MSG RESPONSE_MSG { get; set; }

        public byte[] buf { get; set; }
        /// <summary>
        /// 檔案內容
        /// </summary>
        public string fileContentType { get; private set; }
        /// <summary>
        /// 匯出檔案名稱
        /// </summary>
        public string FileDownloadName { get; set; }

        /// <summary>
        /// 設定檔案名稱ㄋ
        /// </summary>
        /// <param name="fileName"></param>
        public exportFile(string fileName)
        {
            csName = GetLogToolCS.exportFile;
            RESPONSE_MSG = new RESPONSE_MSG();
            this.FileDownloadName = fileName;
        }

        /// <summary>
        /// 匯出檔案
        /// </summary>
        /// <param name="pBuf">byte[]</param>
        /// <param name="exportFile">是否會出檔案</param>
        /// <returns></returns>
        public FileContentResult retunFile(byte[] pBuf,bool exportFile = false)
        {
            this.buf = pBuf == null ? new byte[0] : pBuf;
            if (exportFile)
            {
                return new FileContentResult(this.buf, this.fileContentType)
                {
                    FileDownloadName = this.FileDownloadName
                };
            }
            else
            {
                return new FileContentResult(this.buf, this.fileContentType);
            }
        }
        private void setFileContentType(fileType pFileType)
        {
            switch (pFileType)
            {
                case fileType.xml:
                    this.fileContentType = "application/xml";
                    break;
                case fileType.pdf:
                    this.fileContentType = "application/pdf";
                    break;
                case fileType.xls:
                    this.fileContentType = "application/vnd.ms-excel";
                    break;
                default:
                    break;
            }
        }

        #region 取得結果

        /// <summary>
        /// 產生XML  byte[]
        /// </summary>
        /// <typeparam name="T">匯出強行別</typeparam>
        /// <param name="pList">資料來源</param>
        /// <param name="XmlRootAttributeName">匯出XMLRoot名稱</param>
        /// <param name="GetEncoding">宣告編碼</param>
        /// <param name="isIndent">是否自動換行</param>
        /// <param name="XmlSerializerNamespacesIsNull">XML預設Namespaces是否拿掉變成空白</param>
        /// <returns></returns>
        public byte[] exportXML<T>(List<T> pList, XMLSetting pXs, bool exportFile = false)
        {
            byte[] buf = null;
            string actionName = "exportXML";
            setFileContentType(fileType.xml);
            try
            {
                XmlSerializer xsSubmit = null;
                if (!string.IsNullOrWhiteSpace(pXs.XmlRootAttributeName))
                {
                    xsSubmit = new XmlSerializer(typeof(List<T>), new XmlRootAttribute(pXs.XmlRootAttributeName));
                }
                else
                {
                    xsSubmit = new XmlSerializer(typeof(List<T>));
                }
                XmlWriterSettings settings = new XmlWriterSettings();
                if (pXs.Encoding != null)
                {
                    settings.Encoding = pXs.Encoding;
                }
                if (pXs.isIndent)
                {
                    settings.Indent = true;

                }
                //無法轉UTF8 解法  http://blog.miniasp.com/post/2009/10/02/XmlWriter-and-Encoding-issues.aspx
                //settings.OmitXmlDeclaration = true;
                MemoryStream sww = new MemoryStream();
                XmlWriter writer = XmlWriter.Create(sww, settings);
                //writer.Settings.Encoding = enc;
                XmlSerializerNamespaces ns = new XmlSerializerNamespaces();
                if (pXs.XmlSerializerNamespaces != null)
                {
                    xsSubmit.Serialize(writer, pList, pXs.XmlSerializerNamespaces);
                }
                else
                {
                    xsSubmit.Serialize(writer, pList);
                }
                buf = sww.ToArray();
            }
            catch (Exception ex)
            {
                RESPONSE_MSG.message = "產生XML發生錯誤，請洽資訊人員，錯誤訊息如下所示:" + ex.Message;
                RESPONSE_MSG.status = RESPONSE_STATUS.EXCEPTION;
                LogTool.SaveLogMessage(ex, actionName, csName);
            }
            return buf;
        }

        /// <summary>
        /// 產生Excel byte[]
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="pList"></param>
        /// <param name="em"></param>
        /// <returns></returns>
        public FileContentResult exportExcel<T>(List<T> pList, ExcelSetting em, bool exportFile = false)
        {
            byte[] buf = null;
            string actionName = "exportExcel";
            setFileContentType(fileType.xls);
            try
            {
                DataTable dt = pList.ToDataTable();
                buf = exportExcel(dt, em);
            }
            catch (Exception ex)
            {
                RESPONSE_MSG.message = "產生Excel發生錯誤，請洽資訊人員，錯誤訊息如下所示:" + ex.Message;
                RESPONSE_MSG.status = RESPONSE_STATUS.EXCEPTION;
                LogTool.SaveLogMessage(ex, actionName, csName);
            }
            return retunFile(buf);
        }

        /// <summary>
        /// 產生Excel byte[]
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="pList"></param>
        /// <param name="em"></param>
        /// <returns></returns>
        public byte[] exportExcel(DataTable dt, ExcelSetting em, bool exportFile = false)
        {
            byte[] buf = null;
            string actionName = "exportExcel";
            setFileContentType(fileType.xls);
            try
            {
                NPOITool NPOITool = new Models.NPOITool();
                if (em.isSetbindColName)
                {
                    buf = NPOITool.ExportExcelTable(dt, em.titleName, this.FileDownloadName, em.sheetName, em.bindColName, em.colTitleName,em.exportActionName);
                }
                else
                {
                    buf = NPOITool.ExportExcelTable(dt, em.titleName, em.sheetName, em.titleName, @showColname:true);
                }
                buf = buf == null ? new byte[0] : buf;
            }
            catch (Exception ex)
            {
                RESPONSE_MSG.message = "產生Excel發生錯誤，請洽資訊人員，錯誤訊息如下所示:" + ex.Message;
                RESPONSE_MSG.status = RESPONSE_STATUS.EXCEPTION;
                LogTool.SaveLogMessage(ex, actionName, csName);
            }
            return buf;
        }

        /// <summary>
        /// 產生交班表Excel byte[]
        /// </summary>
        /// <param name="sch_data"></param>
        /// <returns></returns>
        public FileContentResult ExportShiftExcel(scheduling sch_data)
        {
            byte[] buf = null;
            string actionName = "ExportShiftExcel";
            setFileContentType(fileType.xls);
            try
            {
                NPOITool NPOITool = new Models.NPOITool();
                buf = NPOITool.ExportShiftExcel(sch_data);
            }
            catch (Exception ex)
            {
                RESPONSE_MSG.message = "產生Excel發生錯誤，請洽資訊人員，錯誤訊息如下所示:" + ex.Message;
                RESPONSE_MSG.status = RESPONSE_STATUS.EXCEPTION;
                LogTool.SaveLogMessage(ex, actionName, csName);
            }
            return retunFile(buf);
        }


        /// <summary>
        /// 图片XY字典，例如 [{img1, 100,200},{img2,20,30}} 2018.09.13 Cxr繪圖
        /// </summary>
        private Dictionary<string, Tuple<float, float>> m_ImageXYDic = null;

        /// <summary>
        /// 產生PDF byte[]
        /// </summary>
        /// <param name="htmlText"></param>
        /// <param name="ds"></param>
        /// <param name="exportFile">匯出檔案</param>
        /// <returns></returns>
        public FileContentResult exportPDF(string htmlText, IDocSetting ds,bool exportFile = false)
        {
            byte[] buf = null;
            string actionName = "exportPDF";
            setFileContentType(fileType.pdf);
            try
            {
                if (string.IsNullOrEmpty(htmlText))
                {
                    return null;
                }
                //避免當htmlText無任何html tag標籤的純文字時，轉PDF時會掛掉，所以一律加上<p>標籤
                //htmlText = "<p>" + htmlText + "</p>";
                string[] pageHtml = System.Text.RegularExpressions.Regex.Split(htmlText, ds.pageSplit);

                Document doc = new Document(iTextSharp.text.PageSize.A4);//要寫PDF的文件，建構子沒填的話預設直式A4
                if (ds.SetMargins)
                {
                    doc.SetMargins(ds.marginLeft, ds.marginRight, ds.marginTop, ds.marginBottom);
                }
                //指定文件預設開檔時的縮放為100%
                PdfDestination pdfDest = new PdfDestination(PdfDestination.XYZ, 0, doc.PageSize.Height, 1f);
                //開啟Document文件 

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

                    /*------------------------- Cxr繪圖 2018.09.13 [下] -------------------------*/
                    #region Cxr
                    //1.版面配置CSS
                    CssFilesImpl cssFiles = new CssFilesImpl();
                    cssFiles.Add(XMLWorkerHelper.GetCSS(File.OpenRead(sourceFile))); // [File.OpenRead] 新增 [StyleAttrCSSResolver]
                    StyleAttrCSSResolver cssResolver = new StyleAttrCSSResolver(cssFiles);

                    //2.重新撰寫底層Image功能Tag
                    DefaultTagProcessorFactory tagProcessors = (DefaultTagProcessorFactory)Tags.GetHtmlTagProcessorFactory();
                    tagProcessors.RemoveProcessor(HTML.Tag.IMG); // remove the default processor
                    tagProcessors.AddProcessor(HTML.Tag.IMG, new CustomImageTagProcessor()); // use new processor

                    //3.重新編譯
                    //HtmlPipelineContext hpc = new HtmlPipelineContext(new CssAppliersImpl(new XMLWorkerFontProvider()));
                    HtmlPipelineContext hpc = new HtmlPipelineContext(new CssAppliersImpl(new UnicodeFontFactory()));
                    hpc.SetAcceptUnknown(true).AutoBookmark(true).SetTagFactory(tagProcessors); // inject the tagProcessors
                    HtmlPipeline htmlPipeline = new HtmlPipeline(hpc, new PdfWriterPipeline(doc, writer));
                    CssResolverPipeline pipeline = new CssResolverPipeline(cssResolver, htmlPipeline);
                    XMLWorker worker = new XMLWorker(pipeline, true);
                    XMLParser xmlParser = new XMLParser(true, worker, Encoding.UTF8);
                    xmlParser.Parse(new StringReader(text));
                    #endregion
                    /*------------------------- Cxr繪圖 2018.09.13 [上] -------------------------*/
                    /* 2018.09.13 恢復舊有功能
                        //使用XMLWorkerHelper把Html parse到PDF檔裡
                        XMLWorkerHelper.GetInstance().ParseXHtml(writer, doc, msInput, fs, Encoding.UTF8, new UnicodeFontFactory());
                        //將pdfDest設定的資料寫到PDF檔
                        PdfAction action = PdfAction.GotoLocalPage(page, pdfDest, writer);
                        writer.SetOpenAction(action);
                    */
                    msInput.Close();
                    page++;
                }
                doc.Close();
                outputStream.Close();
                //回傳PDF檔案 
                buf = outputStream.ToArray();
            }
            catch (Exception ex)
            {
                RESPONSE_MSG.message = "產生PDF發生錯誤，請洽資訊人員，錯誤訊息如下所示:" + ex.Message;
                RESPONSE_MSG.status = RESPONSE_STATUS.EXCEPTION;
                LogTool.SaveLogMessage(ex, actionName, csName);
            }
            return retunFile(buf);
        }

        #endregion

        /*------------------------------------ Cxr繪圖 2018.09.13 [下] ------------------------------------*/
        // h-ttps://blog.csdn.net/junshangshui/article/details/81193614
        // C# 用itextsharp把Html转PDF
        // 首先得下载itextsharp + 有两个dll, itextsharp.dll和itextsharp.xmlworker.dll
        // 自定义的图片处理类（代码来自百度）
        public class CustomImageTagProcessor : iTextSharp.tool.xml.html.Image
        {
            public override IList<IElement> End(IWorkerContext ctx, Tag tag, IList<IElement> currentContent)
            {
                try
                {
                    IDictionary<string, string> attributes = tag.Attributes;
                    string src;
                    if (!attributes.TryGetValue(HTML.Attribute.SRC, out src))
                        return new List<IElement>(1);
                    if (string.IsNullOrEmpty(src))
                        return new List<IElement>(1);
                    if (src.StartsWith("data:image/", StringComparison.InvariantCultureIgnoreCase))
                    {
                        // data:[][;charset=][;base64],
                        string base64Data = src.Substring(src.IndexOf(",") + 1);
                        byte[] imagedata = Convert.FromBase64String(base64Data);
                        iTextSharp.text.Image image = iTextSharp.text.Image.GetInstance(imagedata);

                        List<IElement> list = new List<IElement>();
                        HtmlPipelineContext htmlPipelineContext = GetHtmlPipelineContext(ctx);
                        list.Add(
                            GetCssAppliers().Apply(
                                new Chunk(
                                    (iTextSharp.text.Image)GetCssAppliers().Apply(image, tag, htmlPipelineContext)
                                    , 0
                                    , 0
                                    , true
                                )
                                , tag
                                , htmlPipelineContext
                            )//GetCssAppliers
                        );//list.Add
                        return list;
                    }//if
                    else
                    {
                        return base.End(ctx, tag, currentContent);
                    }//else
                }
                catch (Exception ex)
                {
                    LogTool.SaveLogMessage(ex, "CustomImageTagProcessor");
                }
                return base.End(ctx, tag, currentContent);
            }//End
        }//自定义的图片处理类（代码来自百度）
        /*------------------------------------ Cxr繪圖 2018.09.13 [上] ------------------------------------*/
    }

    public enum fileType
    {
        /// <summary>
        /// XML檔案
        /// </summary>
        xml,
        /// <summary>
        /// PDF檔案
        /// </summary>
        pdf,
        /// <summary>
        /// EXCEL
        /// </summary>
        xls
    }

   

    #region XML設定

    public class XMLSetting
    {

        public XMLSetting()
        {
            this.isIndent = true;
           
        }
        /// <summary>
        /// 匯出XMLRoot名稱
        /// </summary>
        public string XmlRootAttributeName { get; set; }
        /// <summary>
        /// 宣告編碼
        /// </summary>
        public Encoding Encoding { get; set; }
        /// <summary>
        /// 是否自動換行
        /// </summary>
        public bool isIndent { get; set; }
        /// <summary>
        /// XML預設Namespaces是否拿掉變成空白
        /// </summary>
        public XmlSerializerNamespaces XmlSerializerNamespaces { get; set; }
    }


    #endregion

    #region PDF設定

    /// <summary>
    /// PDF類型
    /// </summary>
    public enum PDFType
    {
        /// <summary>
        /// 呼吸照護記錄單
        /// </summary>
        _RTRecordForm,
        /// <summary>
        /// 呼吸治療記錄單
        /// </summary>
        _RTCPTCXRForm,
        /// <summary>
        /// 評估單
        /// </summary>
        _CPTPageForm,
        /// <summary>
        /// 呼吸治療評估單
        /// </summary>
        _RTAssessCXRForm,
        /// <summary>
        /// 呼吸脫離評估單
        /// </summary>
        _RTTakeoffForm
    }

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
        ){
            //可用Arial或標楷體，自己選一個
            BaseFont baseFont = BaseFont.CreateFont(標楷體Path, BaseFont.IDENTITY_H, BaseFont.EMBEDDED);
            return new Font(baseFont, size, style, color);
        }
    }

    #region 間距設定

    public interface IDocSetting
    {
        /// <summary>
        /// 設定css檔案
        /// </summary>
        string cssFile { get; }
        /// <summary>
        /// 分頁標記
        /// </summary>
        string pageSplit { get; }
        /// <summary>
        /// 是否設定間距
        /// </summary>
        bool SetMargins { get; }
        /// <summary>
        /// 左邊間距
        /// </summary>
        float marginLeft { get; }
        /// <summary>
        /// 右邊間距
        /// </summary>
        float marginRight { get; }
        /// <summary>
        /// 頂部間距
        /// </summary>
        float marginTop { get; }
        /// <summary>
        /// 下方間距
        /// </summary>
        float marginBottom { get; }
    }

    public abstract class DocSetting : IDocSetting
    {
        /// <summary>
        /// 設定css檔案
        /// </summary>
        public virtual string cssFile { get; }
        /// <summary>
        /// 分頁標記
        /// </summary>
        public virtual string pageSplit { get { return "<div>我是分頁</div>"; } }
        /// <summary>
        /// 是否設定間距
        /// </summary>
        public virtual bool SetMargins { get { return true; } }
        /// <summary>
        /// 左邊間距
        /// </summary>
        public virtual float marginLeft { get { return 10; } }
        /// <summary>
        /// 右邊間距
        /// </summary>
        public virtual float marginRight { get { return 10; } }
        /// <summary>
        /// 頂部間距
        /// </summary>
        public virtual float marginTop { get { return 10; } }
        /// <summary>
        /// 下方間距
        /// </summary>
        public virtual float marginBottom { get { return 10; } }

    }

    public class FJURTAssessCXRFormPDFDocSetting : DocSetting
    {
        public override string cssFile { get { return "rtRecordPDFCSS.css"; } }
        public override float marginLeft { get { return 60; } }
    }

    public class RTAssessCXRFormPDFDocSetting : FJURTAssessCXRFormPDFDocSetting
    {

        public override float marginLeft { get { return 30; } }
        public override float marginTop { get { return 30; } }

    }

    public class FJURTRecordFormPDFDocSetting : DocSetting
    {
        public override string cssFile { get { return "rtRecordPDFCSS.css"; } }
        public override float marginLeft { get { return 60; } }
    }

    public class RTRecordFormPDFDocSetting : FJURTRecordFormPDFDocSetting
    {
        public override float marginLeft { get { return 30; } }
        //public override float marginTop { get { return 30; } }
    }

    public class RTTakeoffFormPDFDocSetting : DocSetting
    {
        public override string cssFile { get { return "rtRecordPDFCSS.css"; } }
        public override float marginLeft { get { return 30; } }
        public override float marginTop { get { return 30; } }
    }

    public class CPTPageFormPDFDocSetting : DocSetting
    {
        public override string cssFile { get { return "rtRecordPDFCSS.css"; } }
        public override float marginLeft { get { return 30; } }
        public override float marginTop { get { return 30; } }
    }

    public class FJUCPTPageFormPDFDocSetting : CPTPageFormPDFDocSetting
    {
        public override float marginLeft { get { return 60; } }
    }

    public class RTCPTCXRFormPDFDocSetting : DocSetting
    {
        public override string cssFile { get { return "rtRecordPDFCSS.css"; } }
        public override float marginLeft { get { return 30; } }
        public override float marginTop { get { return 30; } }
    }

    public class FJURTCPTCXRFormPDFDocSetting : RTCPTCXRFormPDFDocSetting
    {
        public override float marginLeft { get { return 60; } }
    }

    public class MeasuresFormPDFDocSetting : DocSetting
    {
        public override string cssFile { get { return "MeasuresFormCSS.css"; } }

        public override bool SetMargins { get { return false; } }
    }

    public class CHGHRTRecordFormPDFDocSetting : DocSetting
    {
        public override string cssFile { get { return "CHGHRTRecordFormPDFCSS.css"; } }
        public override float marginLeft { get { return 40; } }
    }

    public class RTTakeoffAssessPDFDocSetting : DocSetting
    {
        public override string cssFile { get { return "CHGHRTTakeoffFormPDFCSS.css"; } }
    }

    public class CPTNewRecordFormPDFDocSetting : DocSetting
    {
        public override string cssFile { get { return "CHGHRTRecordFormPDFCSS.css"; } }
        public override float marginLeft { get { return 40; } }
    }
    #endregion

    /// <summary>
    /// 設定PDF介面
    /// </summary>
    public interface IPDFProvider
    {
        /// <summary>
        /// 表單名稱
        /// </summary>
        string formName { get; set; }

        /// <summary>
        /// PDF檢視畫面路徑
        /// </summary>
        string viewPath { get; set; }

        /// <summary>
        /// 文件設定
        /// </summary>
        DocSetting DocSetting { get; set; }

        /// <summary>
        /// 設定表單
        /// </summary>
        /// <param name="pdfType"></param>
        void getPDFViewPath(PDFType pdfType);

    }

    /// <summary>
    /// 基本設定PDF介面
    /// </summary>
    public class PDFBasicProvider : IPDFProvider
    {
        /// <summary>
        /// 表單名稱
        /// </summary>
        public string formName { get; set; }

        /// <summary>
        /// PDF檢視畫面路徑
        /// </summary>
        public string viewPath { get; set; }

        /// <summary>
        /// 文件設定
        /// </summary>
        public DocSetting DocSetting { get; set; }

        public void getPDFViewPath(PDFType pdfType)
        {
            
            this.formName = string.Concat(pdfType.ToString(), ".cshtml");
            this.viewPath = "~/Views/RT/exportFile/BASIC/" + this.formName;
        }
    }
    #endregion
}
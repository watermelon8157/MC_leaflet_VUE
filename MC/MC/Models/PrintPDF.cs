using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Spire.Doc;
using Spire.Doc.Documents;
using Spire.Doc.Fields;
using iTextSharp.tool.xml;
using iTextSharp.text.pdf;
using iTextSharp.text;
using System.IO;
using Com.Mayaminer;
using System.Text;
using System.Text.RegularExpressions;

namespace RCS.Models
{
    public class PrintPDF
    {
        string csName { get { return "PrintPDF"; } }
        /// <summary>
        /// 列印資料
        /// </summary>
        /// <param name="print_title">列印標題</param>
        /// <param name="template_file">樣版檔案</param>
        /// <param name="replace_dic_list">List(Dictionary)資料</param>
        /// <param name="output_type">輸出類型(預設PDF)</param>
        /// <returns>byte[]</returns>
        public byte[] print(string print_title, string template_file, List<Dictionary<string, string>> replace_dic_list , string output_type = "pdf")
        {
  
            byte[] print_byte = null;
            //新增一個word檔案並加載，樣版檔案
            Spire.Doc.Document document = new Spire.Doc.Document();

            //取得副檔名
            string extension = string.IsNullOrEmpty(Path.GetExtension(template_file)) ? "" : Path.GetExtension(template_file).ToLower();

            List<byte[]> pdf_byte_list = new List<byte[]>();
            int loop_i = 0;
            foreach (Dictionary<string, string> dic in replace_dic_list)
            {
                switch (extension)
                {
                    case ".docx":
                        document.LoadFromFileInReadMode(template_file, Spire.Doc.FileFormat.Docx2013);
                        break;
                    default:
                        break;
                }
                //置換識別字，[$變數名稱$]
                Regex regEx = new Regex(@"\[\$\w*\$\]");
                //尋找識別字
                TextSelection[] selections = document.FindAllPattern(regEx);
                foreach (TextSelection ts in selections)
                {
                    TextRange range = ts.GetAsOneRange();
                    string sel_text = ts.SelectedText.Replace("[$", "").Replace("$]", "");
                    string replace_text = range.Text;
                   
                    //嘗試置換字符串
                    dic.TryGetValue(sel_text, out replace_text);
                    range.Text = replace_text;
                }
                using (MemoryStream ms = new MemoryStream())
                {
                    document.SaveToStream(ms,Spire.Doc.FileFormat.PDF);
                    //save to byte array
                    pdf_byte_list.Add(ms.ToArray());
                    ms.Close();
                }
                loop_i++;
            }//foreach

            //合併PDF
            print_byte = MergePDFFiles(pdf_byte_list);
            return print_byte;

        }//print

        #region private Func


        /// <summary> 合併PDF檔 </summary> 
        /// <param name="fileList">欲合併PDF檔之List(byte[])(一筆以上)</param>
        /// <returns>byte[]</returns>
        private byte[] MergePDFFiles(List<byte[]> fileList)
        {
            string actionName = "MergePDFFiles";
            byte[] result = null;
            PdfReader reader = null;
            iTextSharp.text.Document document = new iTextSharp.text.Document();
            try
            {
                using (System.IO.MemoryStream ms = new System.IO.MemoryStream())
                {
                    //creating a sample Document
                    iTextSharp.text.pdf.PdfWriter writer = iTextSharp.text.pdf.PdfWriter.GetInstance(document, ms);
                    document.Open();
                    PdfContentByte cb = writer.DirectContent;
                    PdfImportedPage newPage;
                    foreach (byte[] file in fileList)
                    {
                        reader = new PdfReader(file);
                        int iPageNum = reader.NumberOfPages;
                        for (int j = 1; j <= iPageNum; j++)
                        {
                            document.NewPage();
                            newPage = writer.GetImportedPage(reader, j);
                            cb.AddTemplate(newPage, 0, 0);
                        }
                    }
                    document.Close();
                    document.Dispose();
                    reader.Close();
                    reader.Dispose();
                    result = ms.ToArray();
                }
            }
            catch (Exception ex)
            {
                LogTool.SaveLogMessage(ex, actionName, this.csName);
            }
            return result;
        }

        #endregion

    }
}
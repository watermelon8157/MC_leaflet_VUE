using Newtonsoft.Json;
using RCS.Models;
using RCSData.Models;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;

namespace RCS.Controllers.WEBAPI
{
    public class FIleDirectorController : BasicController
    {
        /// <summary>
        /// 取得上傳資料清單
        /// </summary>
        /// <returns></returns> 
        [JwtAuthActionFilterAttribute]
        [HttpPost]
        public List<FIleDirectorDataModel> GetFileData(FORM_BODY form)
        {
            List<FIleDirectorDataModel> pList = new List<FIleDirectorDataModel>();

            string getPatch = System.AppDomain.CurrentDomain.BaseDirectory;
            var pathString = string.Concat("FIleDirector\\",form.pat_info.chart_no,"\\", form.key);
            // Specify the path to save the uploaded file to.
            string savePath = getPatch + pathString;

            if (Directory.Exists(savePath))
            {

                var getDatas = Directory.GetFiles(savePath);

                var count = 0;

                foreach (var filename in getDatas)
                {
                    if (File.Exists(filename))
                    {
                        count++;
                        FIleDirectorDataModel addData = new FIleDirectorDataModel
                        {
                            uid = count.ToString(),
                            name = Path.GetFileName(filename),
                            status = "done",
                            url = string.Concat( form.pat_info.chart_no, "\\", form.key, "\\" + Path.GetFileName(filename) )   
                        };
                        pList.Add(addData);
                    }

                }

            }

            return pList;
        }
        /// <summary>
        /// 取得上傳資料清單
        /// </summary>
        /// <returns></returns> 
        [JwtAuthActionFilterAttribute]
        [HttpPost]
        public string DeleteFileData(FORM_BODY form)
        {
            string getPatch = System.AppDomain.CurrentDomain.BaseDirectory;
            var pathString = string.Concat("FIleDirector\\", form.pat_info.chart_no, "\\", form.key);
            // Specify the path to save the uploaded file to.
            string savePath = getPatch + pathString + "\\" + form.name;

            if (File.Exists(savePath))
            {
                File.Delete(savePath);
            }

            return "刪除成功!";

        }

        /// <summary>
        /// 上傳資料
        /// </summary>
        /// <returns></returns>  
        [HttpPost]
        public HttpResponseMessage UpLoadData()
        {
            //取得當前的 request 物件
            var httpRequest = HttpContext.Current.Request;

            var formData = httpRequest.Form;

            string key = formData["Key"];
            string chart_no = formData["chart_no"];
            //request 如有夾帶檔案
            if (httpRequest.Files.Count > 0)
            {
                //逐一取得檔案名稱
                foreach (string fileName in httpRequest.Files.Keys)
                {
                    //以檔案名稱從 request 的 Files 集合取得檔案內容
                    HttpPostedFile file = httpRequest.Files[fileName];
                    //其他檔案處理
                    var savePatch =   string.Concat("FIleDirector\\", chart_no, "\\", key);

                    if (!SaveFile(file, savePatch))
                    {
                        return Request.CreateResponse(HttpStatusCode.BadRequest);
                    }

                }

                var fooJSON = JsonConvert.SerializeObject(new ResponseDataModel() { url = string.Concat( chart_no, "\\", key + "\\", httpRequest.Files[0].FileName)  });
                var response = Request.CreateResponse(HttpStatusCode.OK);
                response.StatusCode = HttpStatusCode.OK;
                response.Content = new StringContent(fooJSON);    // 回應內容
                return response;
            }

            return Request.CreateResponse(HttpStatusCode.BadRequest);
        }

        private bool SaveFile(HttpPostedFile file, string pathString)
        {
            string getPatch = System.AppDomain.CurrentDomain.BaseDirectory;

            // Specify the path to save the uploaded file to.
            string savePath = getPatch + pathString;

            // Get the name of the file to upload.
            string fileName = file.FileName;

            // Create the path and file name to check for duplicates.
            string pathToCheck = savePath + "\\" + fileName;

            if (!Directory.Exists(savePath))
            {
                Directory.CreateDirectory(savePath);
            }
            // Check to see if a file already exists with the
            // same name as the file to upload.     
            if (File.Exists(pathToCheck))
            {
                File.Delete(pathToCheck);
            }
            // Append the name of the file to upload to the path.
            savePath += "\\" + fileName;

            // Call the SaveAs method to save the uploaded
            // file to the specified directory.
            file.SaveAs(@savePath);
            return true;

        }

         
    }

    public class FORM_BODY: AUTH
    { 
        public string key { get; set; }
        public string name { get; set; }


    }
    public class ResponseDataModel
    {

        public string url { get; set; }
    }

    public class FIleDirectorDataModel
    {

        public string uid { get; set; }
        public string name { get; set; }
        public string status { get; set; }
        public string url { get; set; }
    }
}

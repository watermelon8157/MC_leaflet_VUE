using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using RCS_Data.Controllers.Upload;
using RCS.Models;
using RCS_Data.Models.ViewModels;
using RCS_Data;
using RCSData.Models;

namespace RCS.Controllers.WEBAPI
{
    public class UploadController : BasicController
    {
        RCS.Models.ViewModel.Upload _model;
        RCS.Models.ViewModel.Upload UploadModel
        {
            get
            {
                if (_model == null)
                {
                    this._model = new RCS.Models.ViewModel.Upload();
                }
                return this._model;
            }
        }

        [JwtAuthActionFilterAttribute]
        [HttpPost]
        public List<UPLOADLIST> UploadList(FormUpload form)
        {
            RESPONSE_MSG rm = this.UploadModel.UploadList(form.user_info,  form.sDate, form.eDate);
            this.returnObj(rm); 
            return (List<UPLOADLIST>)rm.attachment;
        }

        [JwtAuthActionFilterAttribute]
        [HttpPost]
        public string UpDateData(FormUpload form)
        {
            RESPONSE_MSG rm = this.UploadModel.UpDateData(form.user_info,   form.keyList, form.status_type);
            this.returnObj(rm);
            return (string)rm.attachment;
        }

        [JwtAuthActionFilterAttribute]
        [HttpPost]
        public int NotUploadCnt(FormUpload form)
        {
            RESPONSE_MSG rm = this.UploadModel.NotUploadCnt(form.user_info,   form.sDate, form.eDate, new List<string>() {"0", "4", "5" } );
            this.returnObj(rm);
            return (int)rm.attachment;
        }

        /// <summary>
        /// 馬上開始上傳
        /// </summary>
        /// <param name="form"></param>
        /// <returns></returns>
        [JwtAuthActionFilterAttribute]
        [HttpPost]
        public string clickUpload_Now(Login_Form_Body form)
        {
        
            if (MvcApplication.UploadToHospThread == null)
            {
                MvcApplication.UploadToHospThread = new Upload_to_hosp();
            }
            if (!MvcApplication.UploadToHospThread.IsAlive)
            {
                MvcApplication.UploadToHospThread.Start();
            }
            else
            { 
                return "簽章上傳中....";
            }
            return "簽章馬上開始上傳!!!";
        }
    }
}

using Com.Mayaminer;
using mayaminer.com.library;
using RCS.Models;
using RCS.Models.ViewModel;
using RCS_Data.Controllers.Shift;
using RCS_Data.Controllers.System;
using RCS_Data.Models;
using RCS_Data.Models.DB;
using RCS_Data.Models.ViewModels;
using RCSData.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web.Http;

namespace RCS.Controllers.WEBAPI
{
    [JwtAuthActionFilter]
    public class ShiftController : BasicController
    {
        string csName { get { return "ShiftController"; } }
        public ShiftModels _model { get; set; }
        protected ShiftModels model
        {
            get
            {
                if (this._model == null)
                {
                    this._model = new ShiftModels();
                }
                return this._model;
            }
        }

        /// <summary>取得交班表格式</summary>
        /// <param name="index">id變數(交班作業用)</param>
        /// <returns></returns>
        [HttpPost]
        public SHIFTViewModels GetShiftData(FormShift form)
        {
            return (SHIFTViewModels)this.returnObj(this.model.GetShiftData(form));
        }

        /// <summary>
        /// 載入交班清單
        /// </summary>
        /// <param name="form"></param>
        /// <returns></returns>
        [HttpPost]
        public string ShowShift(FormShowShift form)
        {
            return (string)this.returnObj(this.model.ShowShift(form));
        }

        /// <summary>
        /// 暫存交班表
        /// </summary>
        /// <param name="form"></param>
        /// <returns></returns>
        [HttpPost]
        public string SaveShiftData(FormSHIFTModels form)
        {
            return this.returnObj(this.model.SaveShiftData(form));
        }

        // <summary>
        /// 上傳跨團隊內容
        /// </summary>
        /// <param name="form"></param>
        /// <returns></returns>
        [HttpPost]
        public string UploadHISData(FormHISData form)
        {
            return (string)this.returnObj(this.model.uploadHISData(form));
        }

        // <summary>
        /// 交班作業功能
        /// </summary>
        /// <param name="form"></param>
        /// <returns></returns>
        [HttpPost]
        public string SaveShiftHandOver(FormShiftHandOver form)
        {
            return (string)this.returnObj(this.model.SaveShiftHandOver(form));
        }

        // <summary>
        /// 確認上傳跨團隊內容
        /// </summary>
        /// <param name="form"></param>
        /// <returns></returns>
        [HttpGet]
        public string CheckDateHISData(string pDate, string ipd_no)
        {
            return "";
        }
    }
}

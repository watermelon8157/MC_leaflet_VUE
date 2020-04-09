using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http; 
using RCS.Models;
using RCS_Data.Models.ViewModels;
using RCS_Data;
using RCS_Data.Controllers.Bord;

namespace RCS.Controllers.WEBAPI
{
    public class BordController : BasicController
    {
        RCS.Models.ViewModel.Bord _model;
        RCS.Models.ViewModel.Bord BordModel
        {
            get
            {
                if (_model == null)
                {
                    this._model = new RCS.Models.ViewModel.Bord();
                }
                return this._model;
            }
        }

        RCS.Models.ViewModel.Upload _UploadModel;
        RCS.Models.ViewModel.Upload UploadModel
        {
            get
            {
                if (_UploadModel == null)
                {
                    this._UploadModel = new RCS.Models.ViewModel.Upload();
                }
                return this._UploadModel;
            }
        } 


        [JwtAuthActionFilterAttribute]
        [HttpPost]
        public List<BordItem> BordList(FormBordList form)
        {

            List<BordItem> pList = new List<BordItem>();
            List<UPLOADLIST> uploadList = (List<UPLOADLIST>)this.returnObj(this.UploadModel.UserUploadList(form.user_info,"","", new List<string>() { "0", "4", "5" }));

            return pList;
        }
 
    }
}

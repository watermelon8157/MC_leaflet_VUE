namespace RCS.Models
{
    /// <summary>
    /// 上傳簽章至醫院
    /// </summary>
    public class Upload_to_hosp : BaseThread
    {
        ViewModel.Upload _model;
        ViewModel.Upload UploadModel
        {
            get
            {
                if (_model == null)
                {
                    this._model = new ViewModel.Upload();
                }
                return this._model;
            }
        }


        public Upload_to_hosp() : base()
        {

        }

        public override void RunThread()
        {
            string actionName = "RunThread";
            this.UploadModel.RunThread();
        }
    }


    

}
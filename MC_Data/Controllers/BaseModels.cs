using RCSData.Models;

namespace RCS_Data.Controllers
{
    public class BaseModels : RCS_Data.Models.BASIC_PARAMS
    {
        private string csName {get { return "BaseModels"; } }
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

        private RCS_Data.Models.BasicFunction _bf { get; set; }
        /// <summary>
        /// basicfunction
        /// </summary>
        protected RCS_Data.Models.BasicFunction basicfunction
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


       
       
    }

    
}

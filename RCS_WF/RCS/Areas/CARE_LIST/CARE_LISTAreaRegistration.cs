using System.Web.Mvc;

namespace RCS.Areas.CARE_LIST
{
    public class CARE_LISTAreaRegistration : AreaRegistration
    {
        public override string AreaName
        {
            get
            {
                return "CARE_LIST";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.MapRoute(
                "CARE_LIST_default",
                "CARE_LIST/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}

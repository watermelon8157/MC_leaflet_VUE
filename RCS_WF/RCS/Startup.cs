using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(RCS.Startup))]
namespace RCS
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            //ConfigureAuth(app); 
        }
    }
}
using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(AMNHAC.Startup))]
namespace AMNHAC
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}

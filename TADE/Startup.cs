using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(TADE.Startup))]
namespace TADE
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}

using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(SyncFood.Startup))]
namespace SyncFood
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}

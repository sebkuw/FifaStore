using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(FifaStore.Startup))]
namespace FifaStore
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}

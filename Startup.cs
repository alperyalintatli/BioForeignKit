using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(BioForeignKit.Startup))]
namespace BioForeignKit
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}

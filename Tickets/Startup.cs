using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(Tickets.Startup))]
namespace Tickets
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}

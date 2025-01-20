using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using Tickets.Models;
using WebMatrix.WebData;

namespace Tickets
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            WebApiConfig.Register(GlobalConfiguration.Configuration);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            Tickets.Models.EmailUtil.StartTimers();
        }
    }

    public class SimpleMembershipInitializer
    {
        public SimpleMembershipInitializer()
        {
            using (var context = new UsersContext())
                context.UserProfiles.Find(1);

            if (!WebSecurity.Initialized)
                WebSecurity.InitializeDatabaseConnection("DefaultConnection", "User", "Id", "Name", autoCreateTables: false);
        }
    }
}

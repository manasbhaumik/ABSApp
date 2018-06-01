using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace APS.WebApp
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            log4net.Config.XmlConfigurator.Configure();

        }

        //public static void RegisterRoutes(RouteCollection routes)
        //{
        //    routes.MapRoute(
        //      "Default", // Route name
        //      "{controller}/{action}/{id}", // URL with parameters
        //      new { controller = "Appointment", action = "BookChangeAppointmentDetails", timeSlotID = UrlParameter.Optional } // Parameter defaults
        //  );
        //}
    }
}

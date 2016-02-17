using System;
using System.Web.Mvc;
using System.Web.Routing;
using WebJobsDemo.Services;

namespace WebJobsDemo
{
    public class Global : System.Web.HttpApplication
    {
        protected void Application_Start(object sender, EventArgs e)
        {
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);

            var queueService =
                ((new AzureQueueStorageService() as IQueueStorageService)
                    .EnableCORS()
                    ? "CORS enabled"
                    : "Failed to enable CORS")
                .Tee(Console.WriteLine);
        }
    }
}
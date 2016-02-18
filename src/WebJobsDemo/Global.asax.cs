using System;
using System.Web.Mvc;
using System.Web.Routing;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using WebJobsDemo.Services;

namespace WebJobsDemo
{
    public class Global : System.Web.HttpApplication
    {
        protected void Application_Start(object sender, EventArgs e)
        {
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);

            JsonConvert.DefaultSettings =
                () =>
                    new JsonSerializerSettings
                    {
                        ContractResolver = new CamelCasePropertyNamesContractResolver()
                    };

            var queueService =
                ((new AzureQueueStorageService() as IQueueStorageService)
                    .EnableCORS()
                    ? "CORS enabled"
                    : "Failed to enable CORS")
                .Tee(Console.WriteLine);
        }
    }
}
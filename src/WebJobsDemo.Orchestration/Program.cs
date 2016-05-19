using System;
using System.Configuration;
using System.Net.Mail;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.SendGrid;
using Microsoft.WindowsAzure;

namespace WebJobsDemo.Orchestration
{
    class Program
    {
        static void Main()
        {
            var connStr = ConfigurationStore.GetConnectionString("WebJobDemo");

            var config =
                new JobHostConfiguration
                {
                    JobActivator = NinjectJobActivator.Instance,
                    DashboardConnectionString = connStr,
                    StorageConnectionString = connStr
                };

            config.UseSendGrid(
                new SendGridConfiguration
                {
                    ApiKey = ConfigurationStore.GetAppSetting("SendGridApiKey"),
                    FromAddress =
                        new MailAddress(
                            ConfigurationStore.GetAppSetting("SendGridMailFromAddress"),
                            "WebJobs Demo")
                });

            var host = new JobHost(config);

            host.RunAndBlock();
        }
    }
}

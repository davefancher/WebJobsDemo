using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.SendGrid;

namespace WebJobsDemo.Orchestration
{
    class Program
    {
        static void Main()
        {
            var config =
                new JobHostConfiguration
                {
                    JobActivator = NinjectJobActivator.Instance
                };

            config.UseSendGrid(
                new SendGridConfiguration
                {
                    ApiKey = ConfigurationManager.AppSettings["SendGridApiKey"],
                    FromAddress =
                        new MailAddress(
                            ConfigurationManager.AppSettings["SendGridMailFromAddress"],
                            "WebJobs Demo")
                });

            var host = new JobHost(config);

            host.RunAndBlock();
        }
    }
}

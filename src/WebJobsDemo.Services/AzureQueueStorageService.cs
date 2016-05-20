using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Queue;
using Microsoft.WindowsAzure.Storage.Shared.Protocol;

namespace WebJobsDemo.Services
{
    public interface IQueueStorageService
    {
        bool EnableCORS();
        string GetSasUri(string queueName);
    }

    public class AzureQueueStorageService
        : IQueueStorageService
    {
        private readonly CloudQueueClient _client;

        public AzureQueueStorageService()
        {
            _client =
                ConfigurationStore
                    .GetConnectionString("WebJobsDemo")
                    .Map(CloudStorageAccount.Parse)
                    .CreateCloudQueueClient();
        }

        bool IQueueStorageService.EnableCORS()
        {
            var properties = _client.GetServiceProperties();

            //properties.Cors.CorsRules.Clear();

            // CORS is already enabled
            if (properties.Cors.CorsRules.Any()) return true;

            try
            {
                properties.Cors.CorsRules.Add(
                    new CorsRule
                    {
                        AllowedHeaders = new List<string> { "x-ms-*", "Access-Control-Allow-Origin", "content-type", "accept", "x-requested-with", "cache-control", "origin" },
                        AllowedMethods = CorsHttpMethods.Post | CorsHttpMethods.Options,
                        AllowedOrigins = new List<string> { "*" },
                        ExposedHeaders = new List<string> { "x-ms-*" },
                        MaxAgeInSeconds = 600
                    }
                );

                _client.SetServiceProperties(properties);

                return true;
            }
            catch
            {
                return false;
            }
        }

        private CloudQueue GetQueue(string queueName) =>
            queueName
                .ToLowerInvariant()
                .Map(_client.GetQueueReference)
                .Tee(r => r.CreateIfNotExists());

        string IQueueStorageService.GetSasUri(string queueName)
        {
            var queue =
                queueName
                    .Map(GetQueue);

            var signature =
                new SharedAccessQueuePolicy
                {
                    Permissions = SharedAccessQueuePermissions.Add,
                    SharedAccessExpiryTime = DateTime.UtcNow.AddHours(2)
                }
                .Map(queue.GetSharedAccessSignature);

            return $"{queue.Uri}/messages{signature}";
        }
    }
}
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;

namespace WebJobsDemo.Services
{
    public interface ITableStorageService
    {
        void Insert<T>(string tableName, string pk, string rk, T value);

        void Delete(string tableName, string pk, string rk);

        T GetById<T>(string tableName, string pk, string rk) where T : new();

        IEnumerable<T> List<T>(string tableName) where T : new();
    }

    public class AzureTableStorageService
        : ITableStorageService
    {
        private static T ConvertToEntity<T>(DynamicTableEntity dte)
            where T : new()
        {
            var entity = new T();

            typeof(T)
                .GetProperties()
                .Iter(p => p.SetValue(entity, dte[p.Name].PropertyAsObject, null));

            return entity;
        }

        private readonly CloudTableClient _client;

        public AzureTableStorageService()
        {
            _client =
                ConfigurationStore
                    .GetConnectionString("WebJobsDemo")
                    .Map(CloudStorageAccount.Parse)
                    .CreateCloudTableClient();
        }

        private CloudTable GetTable(string table) =>
            table
                .Map(_client.GetTableReference)
                .Tee(r => r.CreateIfNotExists());

        void ITableStorageService.Insert<T>(string tableName, string pk, string rk, T value)
        {
            var table = GetTable(tableName);

            var entity = new DynamicTableEntity(pk, rk);

            typeof(T)
                .GetProperties()
                .Iter(p => entity[p.Name] = p.GetValue(value, null).Map(EntityProperty.CreateEntityPropertyFromObject));

            entity
                .Map(TableOperation.Insert)
                .Map(op => table.Execute(op));
        }

        void ITableStorageService.Delete(string tableName, string pk, string rk)
        {
            var table = GetTable(tableName);

            var entity =
                $"PartitionKey eq '{pk}' and RowKey eq '{rk}'"
                    .Map(q => new TableQuery<DynamicTableEntity>().Where(q))
                    .Map(q => table.ExecuteQuery(q).SingleOrDefault());

            if(entity != null)
            {
                entity
                    .Map(TableOperation.Delete)
                    .Map(op => table.Execute(op));
            }
        }

        T ITableStorageService.GetById<T>(string tableName, string pk, string rk)
        {
            var table = GetTable(tableName);

            return
                $"PartitionKey eq '{pk}' and RowKey eq '{rk}'"
                    .Map(q => new TableQuery<DynamicTableEntity>().Where(q))
                    .Map(q => table.ExecuteQuery(q).SingleOrDefault())
                    .Map(ConvertToEntity<T>);
        }

        IEnumerable<T> ITableStorageService.List<T>(string tableName)
        {
            var table = GetTable(tableName);

            return
                new TableQuery<DynamicTableEntity>()
                    .Map(q => table.ExecuteQuery(q))
                    .Select(ConvertToEntity<T>);
        }
    }
}

using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;

namespace WebJobsDemo
{
    using Resolver = Func<string, string>;

    public static class ConfigurationStore
    {
        #region Resolvers

        private static Resolver ResolveEnvironmentVariable(string prefix) =>
            key => Environment.GetEnvironmentVariable($"{prefix}{key}");

        private static string ResolveAppSetting(string key) =>
            ConfigurationManager.AppSettings[key];

        private static string ResolveConnectionString(string key) =>
            ConfigurationManager.ConnectionStrings[key]?.ConnectionString;

        #endregion

        private static Func<IEnumerable<Resolver>, string> Resolve(string key) =>
            resolvers =>
                resolvers
                    .Select(r => r(key))
                    .FirstOrDefault(v => !string.IsNullOrWhiteSpace(v));

        public static string GetConnectionString(string key) =>
            new []
            {
                ResolveEnvironmentVariable("CUSTOMCONNSTR_"),
                ResolveConnectionString
            }
            .Map(Resolve(key));

        public static string GetAppSetting(string key) =>
            new []
            {
                ResolveEnvironmentVariable("APPSETTING_"),
                ResolveAppSetting
            }
            .Map(Resolve(key));
    }
}

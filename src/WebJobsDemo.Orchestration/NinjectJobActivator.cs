using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs.Host;
using Ninject;
using WebJobsDemo.Services;

namespace WebJobsDemo.Orchestration
{
    public sealed class NinjectJobActivator
        : IJobActivator
    {
        private static NinjectJobActivator _instance;

        public static NinjectJobActivator Instance => _instance ?? (_instance = new NinjectJobActivator());

        private readonly IKernel _kernel;

        private NinjectJobActivator()
        {
            _kernel = new StandardKernel();

            _kernel.Bind<ITableStorageService>().To<AzureTableStorageService>().InSingletonScope();
        }

        T IJobActivator.CreateInstance<T>() => _kernel.Get<T>();
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs.Host;
using Ninject;
using WebJobsDemo.Orchestration.ActionHandlers;
using WebJobsDemo.Services;
using WebJobsDemo.Services.Models;

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

            _kernel
                .Bind<ITableStorageService>()
                .To<AzureTableStorageService>()
                .InSingletonScope();
            _kernel
                .Bind<IReservationActionHandler>()
                .To<CreateReservationActionHandler>()
                .Named(ReservationAction.ActionNames.CreateReservation);
            _kernel
                .Bind<IReservationActionHandler>()
                .To<CheckInReservationActionHandler>()
                .Named(ReservationAction.ActionNames.CheckIn);
            _kernel
                .Bind<IReservationActionHandler>()
                .To<CancelReservationActionHandler>()
                .Named(ReservationAction.ActionNames.CancelReservation);
            _kernel
                .Bind<ReservationActionRouter>()
                .ToMethod(
                    context =>
                        typeof(IReservationActionHandler)
                            .Map(_kernel.GetBindings)
                            .Select(b => b.Metadata.Name)
                            .ToDictionary(
                                handler => handler,
                                handler => _kernel.Get<IReservationActionHandler>(handler))
                            .Map(d => new ReservationActionRouter(d))
                );
        }

        T IJobActivator.CreateInstance<T>() => _kernel.Get<T>();
    }
}

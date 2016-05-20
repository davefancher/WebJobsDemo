using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebJobsDemo.Orchestration.ActionHandlers
{
    public class ReservationActionRouter
    {
        private readonly Dictionary<string, IReservationActionHandler> _handlers;

        public ReservationActionRouter(Dictionary<string, IReservationActionHandler> handlers)
        {
            _handlers = handlers;
        }

        public IReservationActionHandler GetHandler(string key) =>
            _handlers[key];
    }
}

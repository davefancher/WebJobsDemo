using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebJobsDemo.Services.Models;

namespace WebJobsDemo.Orchestration.ActionHandlers
{
    public interface IReservationActionHandler
    {
        NotificationMessage Handle(ReservationAction action);
    }
}

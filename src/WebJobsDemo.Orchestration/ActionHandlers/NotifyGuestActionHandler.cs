using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebJobsDemo.Services;
using WebJobsDemo.Services.Models;

namespace WebJobsDemo.Orchestration.ActionHandlers
{
    public class NotifyGuestActionHandler
        : IReservationActionHandler
    {
        private readonly ITableStorageService _tableStorageService;

        public NotifyGuestActionHandler(ITableStorageService tableStorageService)
        {
            _tableStorageService = tableStorageService;
        }

        NotificationMessage IReservationActionHandler.Handle(ReservationAction action)
        {
            var idParts = action.Id.Split('_');
            var pk = idParts[0];
            var rk = idParts[1];

            var reservation = _tableStorageService.GetById<Reservation>("Reservations", pk, rk);
            reservation.IsReady = true;

            _tableStorageService.InsertOrMerge("Reservations", pk, rk, reservation);

            return
                new NotificationMessage
                {
                    Recipient = reservation.EmailAddress,
                    Subject = "Your table is ready",
                    MessageText = "Your table is ready. Please check in at the reception desk."
                };
        }
    }
}
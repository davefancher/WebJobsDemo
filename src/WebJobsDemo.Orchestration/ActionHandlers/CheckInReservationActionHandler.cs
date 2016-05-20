using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebJobsDemo.Services;
using WebJobsDemo.Services.Models;

namespace WebJobsDemo.Orchestration.ActionHandlers
{
    public class CheckInReservationActionHandler
        : IReservationActionHandler
    {
        private readonly ITableStorageService _tableStorageService;

        public CheckInReservationActionHandler(ITableStorageService tableStorageService)
        {
            _tableStorageService = tableStorageService;
        }

        NotificationMessage IReservationActionHandler.Handle(ReservationAction action)
        {
            var idParts = action.Id.Split('_');
            var pk = idParts[0];
            var rk = idParts[1];

            var reservation = _tableStorageService.GetById<Reservation>("Reservations", pk, rk);
            _tableStorageService.Delete("Reservations", pk, rk);

            return
                new NotificationMessage
                {
                    Recipient = reservation.EmailAddress,
                    Subject = "You're checked in!",
                    MessageText = $"Thank you for checking in your party of {reservation.PartySize}! Enjoy your meal!"
                };
        }
    }
}

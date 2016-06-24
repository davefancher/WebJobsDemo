using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebJobsDemo.Services;
using WebJobsDemo.Services.Models;

namespace WebJobsDemo.Orchestration.ActionHandlers
{
    public class CreateReservationActionHandler
        : IReservationActionHandler
    {
        private readonly ITableStorageService _tableStorageService;

        public CreateReservationActionHandler(ITableStorageService tableStorageService)
        {
            _tableStorageService = tableStorageService;
        }

        NotificationMessage IReservationActionHandler.Handle(ReservationAction action)
        {
            var idParts = action.Id.Split('_');

            _tableStorageService
                .InsertOrMerge(
                    "Reservations",
                    idParts[0],
                    idParts[1],
                    new Reservation
                    {
                        Id = action.Id,
                        EmailAddress = action.EmailAddress,
                        PartySize = action.PartySize
                    });

            return
                new NotificationMessage
                {
                    Recipient = action.EmailAddress,
                    Subject = "You've been added to the waiting list!",
                    MessageText = $"Your party of {action.PartySize} has been added to the waiting list! You'll receive another email when your table is ready."
                };
        }
    }
}

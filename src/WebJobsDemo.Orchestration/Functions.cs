using System;
using System.IO;
using System.Net.Mail;
using Microsoft.Azure.WebJobs;
using SendGrid;
using WebJobsDemo.Services;
using WebJobsDemo.Services.Models;

namespace WebJobsDemo.Orchestration
{
    public class Functions
    {
        private readonly ITableStorageService _tableStorageService;

        public Functions(ITableStorageService tableStorageService)
        {
            if (tableStorageService == null) throw new ArgumentNullException(nameof(tableStorageService));

            _tableStorageService = tableStorageService;
        }

        private NotificationMessage HandleCreateReservationAction(string pk, string rk, ReservationAction action)
        {
            _tableStorageService
                .Insert(
                    "Reservations",
                    pk,
                    rk,
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

        private NotificationMessage HandleCancelReservationAction(string pk, string rk, ReservationAction action)
        {
            var reservation = _tableStorageService.GetById<Reservation>("Reservations", pk, rk);
            _tableStorageService.Delete("Reservations", pk, rk);

            return
                new NotificationMessage
                {
                    Recipient = reservation.EmailAddress,
                    Subject = "You've been removed from the waiting list",
                    MessageText = "You've been added to the waiting list! Sorry to see you go!"
                };
        }

        private NotificationMessage HandleCheckInAction(string pk, string rk, ReservationAction action)
        {
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

        public void HandleReservationAction(
            [QueueTrigger("ReservationQueue")] ReservationAction action,
            [Queue("EmailQueue")] out NotificationMessage notification,
            TextWriter log)
        {
            log.WriteLine(action);

            var idParts = action.Id.Split('_');
            var pk = idParts[0];
            var rk = idParts[1];

            switch(action.Action)
            {
                case ReservationAction.ActionNames.CreateReservation:
                    notification = HandleCreateReservationAction(pk, rk, action);
                    return;

                case ReservationAction.ActionNames.CancelReservation:
                    notification = HandleCancelReservationAction(pk, rk, action);
                    return;

                case ReservationAction.ActionNames.CheckIn:
                    notification = HandleCheckInAction(pk, rk, action);
                    return;
            }

            throw new InvalidOperationException();
        }

        public void SendEmail(
            [QueueTrigger("EmailQueue")] NotificationMessage notification,
            [SendGrid] ref SendGridMessage email,
            TextWriter log)
        {
            $"Sending message to {notification.Recipient}".Tee(log.WriteLine);

            email.To = new[] { new MailAddress(notification.Recipient) };
            email.Subject = notification.Subject;
            email.Text = notification.MessageText;
        }
    }
}

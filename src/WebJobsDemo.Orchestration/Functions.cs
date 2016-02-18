using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using SendGrid;
using WebJobsDemo.Services;

namespace WebJobsDemo.Orchestration
{
    public class ReservationAction
    {
        public static class ActionNames
        {
            public const string CreateReservation = "CreateReservation";
            public const string CancelReservation = "CancelReservation";
            public const string CheckIn = "CheckIn";
        }

        public string Action { get; set; }

        public string Id { get; set; }

        public string EmailAddress { get; set; }

        public int PartySize { get; set; }
    }

    public class Reservation
    {
        public string EmailAddress { get; set; }

        public int PartySize { get; set; }
    }

    public class NotificationMessage
    {
        public string Recipient { get; set; }

        public string Subject { get; set; }

        public string MessageText { get; set; }
    }

    public class Functions
    {
        private readonly ITableStorageService _tableStorageService;

        public Functions(ITableStorageService tableStorageService)
        {
            if (tableStorageService == null) throw new ArgumentNullException(nameof(tableStorageService));

            _tableStorageService = tableStorageService;
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

        private NotificationMessage HandleCreateReservationAction(string pk, string rk, ReservationAction action)
        {
            _tableStorageService.Insert("Reservations", pk, rk, new Reservation { EmailAddress = action.EmailAddress, PartySize = action.PartySize });

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
    }
}

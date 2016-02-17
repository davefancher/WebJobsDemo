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

    public class Functions
    {
        private readonly ITableStorageService _tableStorageService;

        public Functions(ITableStorageService tableStorageService)
        {
            if (tableStorageService == null) throw new ArgumentNullException(nameof(tableStorageService));

            _tableStorageService = tableStorageService;
        }

        public void ProcessQueueMessage(
            [QueueTrigger("CheckinQueue")] ReservationAction item,
            [SendGrid] ref SendGridMessage message,
            TextWriter log)
        {
            log.WriteLine(item);

            var idParts = item.Id.Split('_');
            var pk = idParts[0];
            var rk = idParts[1];

            switch(item.Action)
            {
                case ReservationAction.ActionNames.CreateReservation:
                    {
                        _tableStorageService.Insert("Reservations", pk, rk, new Reservation { EmailAddress = item.EmailAddress, PartySize = item.PartySize });

                        message.To = new[] { new MailAddress(item.EmailAddress) };
                        message.Subject = "You've been added to the waiting list!";
                        message.Text = $"Your party of {item.PartySize} has been added to the waiting list! You'll receive another email when your table is ready.";

                        return;
                    }

                case ReservationAction.ActionNames.CancelReservation:
                    {
                        var reservation = _tableStorageService.GetById<Reservation>("Reservations", pk, rk);
                        _tableStorageService.Delete("Reservations", pk, rk);

                        message.To = new[] { new MailAddress(reservation.EmailAddress) };
                        message.Subject = "You've been removed from the waiting list";
                        message.Text = "You've been added to the waiting list! Sorry to see you go!";

                        return;
                    }

                case ReservationAction.ActionNames.CheckIn:
                    {
                        var reservation = _tableStorageService.GetById<Reservation>("Reservations", pk, rk);
                        _tableStorageService.Delete("Reservations", pk, rk);

                        message.To = new[] { new MailAddress(reservation.EmailAddress) };
                        message.Subject = "You're checked in!";
                        message.Text = $"Thank you for checking in your party of {reservation.PartySize}! Enjoy your meal!";

                        return;
                    }
            }
        }
    }
}

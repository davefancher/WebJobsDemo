using System;
using System.IO;
using System.Net.Mail;
using Microsoft.Azure.WebJobs;
using SendGrid;
using WebJobsDemo.Orchestration.ActionHandlers;
using WebJobsDemo.Services;
using WebJobsDemo.Services.Models;

namespace WebJobsDemo.Orchestration
{
    public class Functions
    {
        //private readonly ITableStorageService _tableStorageService;
        private ReservationActionRouter _actionRouter;

        public Functions(ReservationActionRouter actionRouter)
        {
            _actionRouter = actionRouter;
        }

        public void HandleReservationAction(
            [QueueTrigger("ReservationQueue")] ReservationAction action,
            [Queue("EmailQueue")] out NotificationMessage notification,
            TextWriter log)
        {
            notification =
                action
                    .Tee(log.WriteLine)
                    .Map(_actionRouter.GetHandler(action.Action).Handle);
        }

        public void SendEmail(
            [QueueTrigger("EmailQueue")] NotificationMessage notification,
            [SendGrid] ref SendGridMessage email,
            TextWriter log)
        {
            $"Sending message to {notification.Recipient}"
                .Tee(log.WriteLine);

            email.To = new[] { new MailAddress(notification.Recipient) };
            email.Subject = notification.Subject;
            email.Text = notification.MessageText;
        }
    }
}

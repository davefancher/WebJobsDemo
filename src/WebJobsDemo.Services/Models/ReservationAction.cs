using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebJobsDemo.Services.Models
{
    public class ReservationAction
    {
        public static class ActionNames
        {
            public const string CreateReservation = "CreateReservation";
            public const string CancelReservation = "CancelReservation";
            public const string CheckIn = "CheckIn";
            public const string NotifyGuest = "NotifyGuest";
        }

        public string Action { get; set; }

        public string Id { get; set; }

        public string EmailAddress { get; set; }

        public int PartySize { get; set; }
    }
}

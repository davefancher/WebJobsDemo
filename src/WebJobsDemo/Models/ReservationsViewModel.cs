using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebJobsDemo.Services.Models;

namespace WebJobsDemo.Models
{
    public class ReservationsViewModel
    {
        public string SessionId { get; set; }

        public string QueueUri { get; set; }

        public IEnumerable<Reservation> Reservations { get; set; }
    }
}
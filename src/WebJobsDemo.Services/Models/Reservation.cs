using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebJobsDemo.Services.Models
{
    public class Reservation
    {
        public string Id { get; set; }

        public string EmailAddress { get; set; }

        public int PartySize { get; set; }
    }
}

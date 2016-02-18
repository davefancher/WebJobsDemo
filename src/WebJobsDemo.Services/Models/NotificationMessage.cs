using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebJobsDemo.Services.Models
{
    public class NotificationMessage
    {
        public string Recipient { get; set; }

        public string Subject { get; set; }

        public string MessageText { get; set; }
    }
}

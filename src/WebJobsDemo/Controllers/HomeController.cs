using System;
using System.Web.Mvc;
using WebJobsDemo.Models;
using WebJobsDemo.Services;
using WebJobsDemo.Services.Models;

namespace WebJobsDemo.Controllers
{
    public class HomeController : Controller
    {
        private ITableStorageService tableStorage = new AzureTableStorageService();
        private IQueueStorageService queueStorage = new AzureQueueStorageService();

        public ActionResult Index() =>
            new ReservationsViewModel
            {
                SessionId = HttpContext.Session.SessionID,
                QueueUri = "ReservationQueue".Map(queueStorage.GetSasUri),
                Reservations = tableStorage.List<Reservation>("Reservations")
            }.Map(View);
    }
}
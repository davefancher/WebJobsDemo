﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebJobsDemo.Services;

namespace WebJobsDemo.Controllers
{
    public class HomeController : Controller
    {
        private IQueueStorageService queueStorage = new AzureQueueStorageService();

        public ActionResult Index()
        {
            ViewBag.SessionId = HttpContext.Session.SessionID;
            ViewBag.QueueUri =
                "CheckinQueue"
                    .Map(queueStorage.GetSasUri);

            return View();
        }
    }
}
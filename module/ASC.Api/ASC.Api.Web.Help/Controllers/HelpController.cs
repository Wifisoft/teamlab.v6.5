﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ASC.Api.Web.Help.Controllers
{
    [OutputCache(CacheProfile = "pages")]
    public class HelpController : Controller
    {
        //
        // GET: /Home/

        public ActionResult Index()
        {
            ViewData["splash"] = "HomeSplash";
            return View();
        }

        public ActionResult Authentication()
        {
            return View();
        }

        public ActionResult Basic()
        {
            return View();
        }

        public ActionResult Faq()
        {
            return View();
        }

        public ActionResult Filters()
        {
            return View();
        }

        public ActionResult Batch()
        {
            return View();
        }
    }
}

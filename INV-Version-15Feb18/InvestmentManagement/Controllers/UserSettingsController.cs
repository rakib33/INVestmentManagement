﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace InvestmentManagement.Controllers
{
    public class UserSettingsController : Controller
    {
        //
        // GET: /UserSettings/

        public ActionResult Settings()
        {
           
            return PartialView();
        }

    }
}

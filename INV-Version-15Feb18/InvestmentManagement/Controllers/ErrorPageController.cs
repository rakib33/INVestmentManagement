using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace InvestmentManagement.Controllers
{
    public class ErrorPageController : Controller
    {
        //
        // GET: /ErrorPage/

        public ActionResult Index(string message)
        {
            ViewBag.Message = message;
            return View();
        }

    }
}

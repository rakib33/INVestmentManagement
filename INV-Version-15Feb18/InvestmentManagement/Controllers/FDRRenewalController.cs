using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace InvestmentManagement.Controllers
{
    public class FDRRenewalController : Controller
    {
        //
        // GET: /FDRRenewal/

        public ActionResult CreateFDRRenewal(string FixDepositReference)
        {
            return PartialView("RenewDeposit");
        }

    }
}

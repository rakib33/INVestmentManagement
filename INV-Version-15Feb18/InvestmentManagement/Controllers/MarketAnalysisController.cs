using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using InvestmentManagement.Models;
using System.Linq.Dynamic;
using InvestmentManagement.InvestmentManagement.Models;
using System.IO;
using System.Xml.Linq;
using System.Data.EntityClient;

namespace InvestmentManagement.Controllers
{
    public class MarketAnalysisController : Controller
    {
        //
        // GET: /MarketAnalysis/
        CommonFunction oCommonFunction = new CommonFunction();
        public ActionResult Chart(string lblbreadcum)
        {

            List<INSTRUMENT> instrumentList = new List<INSTRUMENT>();
            instrumentList = new Entities(Session["Connection"] as EntityConnection).INSTRUMENTs.ToList().OrderBy(i => i.SHORTNAME).ToList();


            ViewBag.instrumentList = new SelectList(instrumentList, "SHORTNAME", "SHORTNAME");
            Session["Path"] = oCommonFunction.GetPath(this.ControllerContext.RouteData.Values["controller"].ToString(), Session["currentPage"].ToString(), this.ControllerContext.RouteData.Values["action"].ToString());

            ViewBag.Message = oCommonFunction.MessageForView(this.ControllerContext.RouteData.Values["action"].ToString());
            if (!string.IsNullOrEmpty(lblbreadcum))
            {
                Session["currentPage"] = lblbreadcum;
            }

            ViewBag.BreadCum = oCommonFunction.GetListPath(Session["currentPage"].ToString(), this.ControllerContext.RouteData.Values["controller"].ToString());
            return PartialView();
        }

        public ActionResult PriceIndexList(DateTime? fromDate = null,DateTime? toDate = null, string instrumentName =null)
        {
            
            List<PRICEINDEX> priceIndexList = new List<PRICEINDEX>();
            using (var db=new Entities(Session["Connection"] as EntityConnection))
            {
              priceIndexList =  db.PRICEINDEXes.Where(pi => pi.INSTRUMENTREF == instrumentName).Where(pi =>pi.TRADINGDATE >= fromDate && pi.TRADINGDATE <= toDate).OrderBy(pi => pi.TRADINGDATE).ToList();
            }

            return Json(priceIndexList, JsonRequestBehavior.AllowGet);
        }

    }
}

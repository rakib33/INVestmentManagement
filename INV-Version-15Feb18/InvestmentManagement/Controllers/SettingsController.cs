using System;
using System.Collections.Generic;
using System.Data.EntityClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using InvestmentManagement.InvestmentManagement.Models;
using InvestmentManagement.Models;

namespace InvestmentManagement.Controllers
{
    public class SettingsController : Controller
    {
        //
        // GET: /UserSettings/

        public ActionResult Settings(string lblbreadcum)
        {
            if (Session["UserId"] == null || Session["PreviousPage"] == null || Session["currentPage"] == null || Session["Connection"] == null)
            {
                return RedirectToAction("LogOut", "Home");

            }

            CommonFunction oCommonFunction = new CommonFunction();

            if (!string.IsNullOrEmpty(lblbreadcum))
            {
                Session["currentPage"] = lblbreadcum;
            }

            Session["Path"] = oCommonFunction.GetPath(this.ControllerContext.RouteData.Values["controller"].ToString(), Session["currentPage"].ToString(), this.ControllerContext.RouteData.Values["action"].ToString());

            ViewBag.BreadCum = oCommonFunction.GetListPath(Session["currentPage"].ToString(), this.ControllerContext.RouteData.Values["controller"].ToString());


            var settingId = new Entities(Session["Connection"] as EntityConnection).MENUs.Where(menu => menu.NAME == "Settings").Select(menu => menu.REFERENCE).FirstOrDefault();
            ViewBag.listOfSettings = new Entities(Session["Connection"] as EntityConnection).MENUs.Where(m => m.PARENTNAMEID == settingId).OrderBy(m => m.ORDERINDEX).ToList();
            Session["currentPage"] = "Settings";
            //ViewBag.listOfSettings = listOfSettings;
            return PartialView();
        }

    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using InvestmentManagement.Models;
using System.Linq.Dynamic;
using InvestmentManagement.ViewModel;
using InvestmentManagement.InvestmentManagement.Models;
using System.Data;
using System.Data.EntityClient;

namespace InvestmentManagement.Controllers
{
    public class TradingChargeController : Controller
    {

        // GET: /Broker/
        CommonFunction oCommonFunction = new CommonFunction();

        [HttpGet]
        public ActionResult ListTcharges(string sortdir, string sort, int? page, int? rows, string filterstring, string currentFilter, string lblbreadcum, string PagingType, int? currentRowPerPage = 2)
        {
            if (Session["UserId"] == null || Session["PreviousPage"] == null || Session["currentPage"] == null || Session["Connection"] == null)
            {
                return RedirectToAction("LogOut", "Home");

            }
            try
            {
                //currentRowPerPage=@ViewBag.currentRowPerPage
                GridModel<TRADINGCHARGE> gridModels = new GridModel<TRADINGCHARGE>();
                List<TRADINGCHARGE> models = null;

                //grid settings                
                gridModels.RowsPerPage = rows == null ? (int)currentRowPerPage : (int)rows;
                ViewBag.currentRowPerPage = gridModels.RowsPerPage;

                //Paging direction
                switch (PagingType)
                {
                    case "Next":
                        Session["pageNo"] = (int)Session["pageNo"] + 1;
                        break;
                    case "Prev":
                        Session["pageNo"] = (int)Session["pageNo"] - 1;
                        break;
                    default:
                        Session["pageNo"] = 1;
                        ViewBag.Prev = "disabled";
                        ViewBag.PrevNotActive = "not-active";
                        break;
                }


                //loading configuration
                sort = string.IsNullOrEmpty(sort) == true ? "CREATEDDATE" : sort;
                sortdir = string.IsNullOrEmpty(sortdir) == true ? "DESC" : sortdir;
                int skipcount = gridModels.RowsPerPage * ((int)Session["pageNo"] - 1);
                if (filterstring == null)
                {
                    filterstring = currentFilter;
                }
                
                ViewBag.CurrentFilter = filterstring;


                if (string.IsNullOrEmpty(filterstring))
                    models = new Entities(Session["Connection"] as EntityConnection).TRADINGCHARGEs.AsNoTracking().OrderBy(sort + " " + sortdir).Skip(skipcount).Take(gridModels.RowsPerPage).ToList();
                else
                    models = new Entities(Session["Connection"] as EntityConnection).TRADINGCHARGEs.AsNoTracking().Where(w => w.MARKET.Contains(filterstring)).OrderBy(sort + " " + sortdir).Skip(skipcount).Take(gridModels.RowsPerPage).ToList();


                gridModels.DataModel = models;

                if (!string.IsNullOrEmpty(lblbreadcum))
                {
                    Session["currentPage"] = lblbreadcum;
                }
                                
                Session["Path"] = oCommonFunction.GetPath(this.ControllerContext.RouteData.Values["controller"].ToString(), Session["currentPage"].ToString(), this.ControllerContext.RouteData.Values["action"].ToString());

                ViewBag.BreadCum = oCommonFunction.GetListPath(Session["currentPage"].ToString(), this.ControllerContext.RouteData.Values["controller"].ToString());
                if (models.Count() < gridModels.RowsPerPage)
                {

                    ViewBag.Prev = "disabled";
                    ViewBag.Next = "disabled";
                    ViewBag.PrevNotActive = "not-active";
                    ViewBag.NextNotActive = "not-active";
                }
                return PartialView("ListTcharges", gridModels);

            }

            catch (Exception ex)
            {
                string message = ex.Message;

                return RedirectToAction("Index", "ErrorPage", new { message });
            }



        }

        [HttpGet]
        public ActionResult AddTCharge(string lblbreadcum)
        {
            if (Session["UserId"] == null || Session["PreviousPage"] == null || Session["currentPage"] == null || Session["Connection"] == null)
            {
                return RedirectToAction("LogOut", "Home");
            }
            try
            {
                Entities db = new Entities(Session["Connection"] as EntityConnection);              
                ViewBag.Message = new CommonFunction().MessageForView(this.ControllerContext.RouteData.Values["action"].ToString());
                ViewBag.breadcum = oCommonFunction.GetAddPath(Session["Path"] as IHtmlString, Session["currentPage"].ToString());
                ViewBag.Header = "Add " + Session["currentPage"];
                return PartialView("AddTCharge");
            }

            catch (Exception ex)
            {
                string message = ex.Message;
                return RedirectToAction("Index", "ErrorPage", new { message });
            }
        }

        [HttpPost]
        public ActionResult AddTCharge(TRADINGCHARGE oTcharge)
        {
            if (Session["UserId"] == null || Session["PreviousPage"] == null || Session["currentPage"] == null || Session["Connection"] == null)
            {
                return RedirectToAction("LogOut", "Home");
            }
            try
            {              
                if (Request.IsAjaxRequest())
                {
                    oTcharge.REFERENCE = Guid.NewGuid().ToString();
                    oTcharge.CREATEDBY = Session["UserId"].ToString();
                    oTcharge.CREATEDDATE = DateTime.Today;
                   
                    oCommonFunction.CustomObjectNullValidation<TRADINGCHARGE>(ref oTcharge);
                   
                    using (Entities db = new Entities(Session["Connection"] as EntityConnection))
                    {
                        db.TRADINGCHARGEs.Add(oTcharge);
                        db.SaveChanges();
                    }
                }

                return RedirectToAction("ListTcharges", "TradingCharge");                
            }
            catch (Exception ex)
            {

                string message = ex.Message;
                return RedirectToAction("Index", "ErrorPage", new { message });
            }

        }




        [HttpGet]
        public ActionResult EditTCharge(string id)
        {
            if (Session["UserId"] == null || Session["PreviousPage"] == null || Session["currentPage"] == null || Session["Connection"] == null)
            {
                return RedirectToAction("LogOut", "Home");

            }
            try
            {
                TRADINGCHARGE oTCharge = new TRADINGCHARGE();
                Entities db = new Entities(Session["Connection"] as EntityConnection);
                ViewModelBase oViewModelBase = new ViewModelBase();

                oTCharge = db.TRADINGCHARGEs.SingleOrDefault(i => i.REFERENCE == id);              
                ViewBag.Message = new CommonFunction().MessageForView(this.ControllerContext.RouteData.Values["action"].ToString());
                ViewBag.breadcum = oCommonFunction.GetEditPath(Session["Path"] as IHtmlString, Session["currentPage"].ToString());
                ViewBag.Header = "Update " + Session["currentPage"];
                ViewBag.Market = oTCharge.MARKET;
                return PartialView("EditTCharge", oTCharge);
            }

            catch (Exception ex)
            {
                string message = ex.Message;

                return RedirectToAction("Index", "ErrorPage", new { message });

            }
        }



        [HttpPost]
        public ActionResult EditTCharge(TRADINGCHARGE oTcharge)
        {

            if (Session["UserId"] == null || Session["PreviousPage"] == null || Session["currentPage"] == null || Session["Connection"] == null)
            {
                return RedirectToAction("LogOut", "Home");
            }
            try
            {

                if (Request.IsAjaxRequest())
                {                   
                    oTcharge.LASTUPDATED = DateTime.Today;
                    oTcharge.LASTUPDATEDBY = Session["UserId"].ToString();
                    oCommonFunction.CustomObjectNullValidation<TRADINGCHARGE>(ref oTcharge);
                    using (Entities db = new Entities(Session["Connection"] as EntityConnection))
                    {

                        db.Entry(oTcharge).State = EntityState.Modified;
                        db.SaveChanges();
                    }
                }
                return RedirectToAction("ListTcharges", "TradingCharge");  

            }

            catch (Exception ex)
            {
                string message = ex.Message;

                return RedirectToAction("Index", "ErrorPage", new { message });
            }

        }
    }
}

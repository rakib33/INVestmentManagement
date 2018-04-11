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
using System.Configuration;
using InvestmentManagement.App_Code;

namespace InvestmentManagement.Controllers
{
    public class SignatoryController : Controller
    {
        //
        CommonFunction oCommonFunction = new CommonFunction();

        public ActionResult ListSignatory(string sortdir, string sort, int? page, int? rows, string filterstring, string currentFilter, string lblbreadcum, string PagingType, int? currentRowPerPage = 15)
        {
            if (Session["UserId"] == null || Session["PreviousPage"] == null || Session["currentPage"] == null || Session["Connection"] == null)
            {
                return RedirectToAction("LogOut", "Home");

            }
            try
            {
                //currentRowPerPage=@ViewBag.currentRowPerPage
                GridModel<SIGNATORY> gridModels = new GridModel<SIGNATORY>();
                List<SIGNATORY> models = null;

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
                    models = new Entities(Session["Connection"] as EntityConnection).SIGNATORies.AsNoTracking().OrderBy(t=>t.CREATEDDATE).Skip(skipcount).Take(gridModels.RowsPerPage).ToList();
                else
                    models = new Entities(Session["Connection"] as EntityConnection).SIGNATORies.AsNoTracking().Where(w => w.ReportName.Contains(filterstring)).OrderBy(t=>t.CREATEDDATE).Skip(skipcount).Take(gridModels.RowsPerPage).ToList();

               

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
                return PartialView("ListSignatory", gridModels);



            }

            catch (Exception ex)
            {
                string message = ex.Message;

                return RedirectToAction("Index", "ErrorPage", new { message });
            }

        }


        [HttpGet]
        public ActionResult AddSignatory()
        {
            if (Session["UserId"] == null || Session["PreviousPage"] == null || Session["currentPage"] == null || Session["Connection"] == null)
            {
                return RedirectToAction("LogOut", "Home");

            }
            try
            {
                ViewBag.Message = new CommonFunction().MessageForView(this.ControllerContext.RouteData.Values["action"].ToString());
                ViewBag.breadcum = oCommonFunction.GetAddPath(Session["Path"] as IHtmlString, Session["currentPage"].ToString());
                ViewBag.Header = "Add " + Session["currentPage"];
              
                return PartialView();
            }

            catch (Exception ex)
            {
                string message = ex.Message;

                return RedirectToAction("Index", "ErrorPage", new { message });

            }
        }

        [HttpPost]
        public ActionResult AddSignatory(SIGNATORY oSignatory)
        {
            try
            {

                if (Session["UserId"] == null || Session["PreviousPage"] == null || Session["currentPage"] == null || Session["Connection"] == null)
                {
                    return RedirectToAction("LogOut", "Home");

                }

                if (Request.IsAjaxRequest())
                {

                    oSignatory.Reference = Guid.NewGuid().ToString();
                    oSignatory.CREATEDBY = Session["UserId"].ToString();
                    oSignatory.CREATEDDATE = DateTime.Today;
                
                    using (Entities db = new Entities(Session["Connection"] as EntityConnection))
                    {
                        db.SIGNATORies.Add(oSignatory);
                        db.SaveChanges();
                    }
                }
                return RedirectToAction("ListSignatory");


            }
            catch (Exception ex)
            {
                string message = ex.Message;
                return RedirectToAction("Index", "ErrorPage", new { message });
            }

        }



        #region SignatoryDetails
        public ActionResult ListSignatoryDetails(string id,string sortdir, string sort, int? page, int? rows, string filterstring, string currentFilter, string lblbreadcum, string PagingType, int? currentRowPerPage = 15)
        {
            if (Session["UserId"] == null || Session["PreviousPage"] == null || Session["currentPage"] == null || Session["Connection"] == null)
            {
                return RedirectToAction("LogOut", "Home");

            }
            try
            {
                //currentRowPerPage=@ViewBag.currentRowPerPage
                GridModel<SIGNATORYDETAIL> gridModels = new GridModel<SIGNATORYDETAIL>();
                List<SIGNATORYDETAIL> models = null;

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
                    models = new Entities(Session["Connection"] as EntityConnection).SIGNATORYDETAILS.AsNoTracking().OrderBy(t=>t.ORDERINDEX).Skip(skipcount).Take(gridModels.RowsPerPage).ToList();
                else
                    models = new Entities(Session["Connection"] as EntityConnection).SIGNATORYDETAILS.AsNoTracking().Where(w => w.SignatureLine1.Contains(filterstring) || w.SignatureLine2.Contains(filterstring)).OrderBy(t=>t.ORDERINDEX).Skip(skipcount).Take(gridModels.RowsPerPage).ToList();

                if (id != null && !string.IsNullOrEmpty(id))
                    models = models.Where(t => t.SIGNATORY_REF == id).ToList();

                gridModels.DataModel = models;

                if (!string.IsNullOrEmpty(lblbreadcum))
                {
                    Session["currentPage"] = lblbreadcum;
                }
                else
                {
                    Session["currentPage"] = "SignatoryDetails";
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

                ViewBag.RefSinatory = id;
                return PartialView("ListSignatoryDetails", gridModels);



            }

            catch (Exception ex)
            {
                string message = ex.Message;

                return RedirectToAction("Index", "ErrorPage", new { message });
            }

        }


        [HttpGet]
        public ActionResult AddSignatoryDetails(string RefSinatory)
        {
            if (Session["UserId"] == null || Session["PreviousPage"] == null || Session["currentPage"] == null || Session["Connection"] == null)
            {
                return RedirectToAction("LogOut", "Home");

            }
            try
            {
                ViewBag.Message = new CommonFunction().MessageForView(this.ControllerContext.RouteData.Values["action"].ToString());
                ViewBag.breadcum = oCommonFunction.GetAddPath(Session["Path"] as IHtmlString, Session["currentPage"].ToString());
                ViewBag.Header = "Add " + Session["currentPage"];

                ViewBag.RefSinatory = RefSinatory;
                return PartialView();
            }

            catch (Exception ex)
            {
                string message = ex.Message;

                return RedirectToAction("Index", "ErrorPage", new { message });

            }
        }

        [HttpPost]
        public ActionResult AddSignatoryDetails(SIGNATORYDETAIL oSignatory)
        {
            try
            {
                string RefSignatory = Convert.ToString(TempData["RefSinatory"]);
                if (Session["UserId"] == null || Session["PreviousPage"] == null || Session["currentPage"] == null || Session["Connection"] == null)
                {
                    return RedirectToAction("LogOut", "Home");

                }

                if (Request.IsAjaxRequest())
                {

                 
                    SIGNATORY obj = new SIGNATORY();
                    



                    oSignatory.Reference = Guid.NewGuid().ToString();
                    oSignatory.CREATEDBY = Session["UserId"].ToString();
                    oSignatory.CREATEDDATE = DateTime.Today;

                    using (Entities db = new Entities(Session["Connection"] as EntityConnection))
                    {

                      obj = db.SIGNATORies.Where(t => t.Reference == RefSignatory).SingleOrDefault();

                      oSignatory.SIGNATORY = obj;

                      db.SIGNATORYDETAILS.Add(oSignatory);
                      db.SaveChanges();
                    }
                }
                return RedirectToAction("ListSignatoryDetails", new { id = RefSignatory });


            }
            catch (Exception ex)
            {
                string message = ex.Message;
                return RedirectToAction("Index", "ErrorPage", new { message });
            }

        }


        [HttpGet]
        public ActionResult EditSignatoryDetails(string id)
        {
            if (Session["UserId"] == null || Session["PreviousPage"] == null || Session["currentPage"] == null || Session["Connection"] == null)
            {
                return RedirectToAction("LogOut", "Home");

            }
            try
            {
                ViewBag.Message = new CommonFunction().MessageForView(this.ControllerContext.RouteData.Values["action"].ToString());
                ViewBag.breadcum = oCommonFunction.GetAddPath(Session["Path"] as IHtmlString, Session["currentPage"].ToString());
                ViewBag.Header = "Add " + Session["currentPage"];

                var SigDetails = new Entities(Session["Connection"] as EntityConnection).SIGNATORYDETAILS.Where(t => t.Reference == id).SingleOrDefault();

                //ViewBag.RefSinatory = id;
                return PartialView(SigDetails);
            }

            catch (Exception ex)
            {
                string message = ex.Message;

                return RedirectToAction("Index", "ErrorPage", new { message });

            }
        }


        [HttpPost]
        public ActionResult EditSignatoryDetails(SIGNATORYDETAIL oSignatory)
        {
            try
            {
                string RefSignatory = Convert.ToString(TempData["RefSinatory"]);
                if (Session["UserId"] == null || Session["PreviousPage"] == null || Session["currentPage"] == null || Session["Connection"] == null)
                {
                    return RedirectToAction("LogOut", "Home");

                }

                if (Request.IsAjaxRequest())
                {
                    SIGNATORY obj = new SIGNATORY();
   
                   
                    oSignatory.LASTUPDATEDBY = Session["UserId"].ToString();
                    oSignatory.LASTUPDATED = DateTime.Today;

                    using (Entities db = new Entities(Session["Connection"] as EntityConnection))
                    {
                        //SIGNATORYDETAIL details = db.SIGNATORYDETAILS.Include("SIGNATORY").Where(t => t.Reference == oSignatory.Reference).SingleOrDefault();
                       
                        obj = db.SIGNATORies.Where(t => t.Reference == oSignatory.SIGNATORY_REF).SingleOrDefault();
                        oSignatory.SIGNATORY = obj;
                        db.Entry(oSignatory).State = EntityState.Modified;                      
                        db.SaveChanges();
                    }
                }
                return RedirectToAction("ListSignatoryDetails", new { id = RefSignatory });


            }
            catch (Exception ex)
            {
                string message = ex.Message;
                return RedirectToAction("Index", "ErrorPage", new { message });
            }

        }


        #endregion

    }
}

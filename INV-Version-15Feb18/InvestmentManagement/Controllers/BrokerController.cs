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
    public class BrokerController : Controller
    {
        //
        // GET: /Broker/
        CommonFunction oCommonFunction = new CommonFunction();

        [HttpGet]
        public ActionResult ListBroker(string sortdir, string sort, int? page, int? rows, string filterstring, string currentFilter, string lblbreadcum, string PagingType, int? currentRowPerPage = 2)
        {
            try
            {
                if (Session["UserId"] == null || Session["PreviousPage"] == null || Session["currentPage"] == null || Session["Connection"] == null)
                {
                    return RedirectToAction("LogOut", "Home");

                }
                //currentRowPerPage=@ViewBag.currentRowPerPage
                GridModel<BROKER> gridModels = new GridModel<BROKER>();
                List<BROKER> models = null;

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
                    models = new Entities(Session["Connection"] as EntityConnection).BROKERs.AsNoTracking().OrderBy(sort + " " + sortdir).Skip(skipcount).Take(gridModels.RowsPerPage).ToList();
                else
                    models = new Entities(Session["Connection"] as EntityConnection).BROKERs.AsNoTracking().Where(w => w.NAME.Contains(filterstring)).OrderBy(sort + " " + sortdir).Skip(skipcount).Take(gridModels.RowsPerPage).ToList();


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
                return PartialView("ListBroker", gridModels);



            }

            catch (Exception ex)
            {
                string message = ex.Message;

                return RedirectToAction("Index", "ErrorPage", new { message });
            }



        }
                
        [HttpGet]
        public ActionResult AddBroker(string lblbreadcum)
        {
            try
            {

                if (Session["UserId"] == null || Session["PreviousPage"] == null || Session["currentPage"] == null || Session["Connection"] == null)
                {
                    return RedirectToAction("LogOut", "Home");

                }
                Entities db = new Entities(Session["Connection"] as EntityConnection);
                //BROKER oBROKER = db.BROKERs.FirstOrDefault();
                //if (!string.IsNullOrEmpty(lblbreadcum))
                //{
                //    Session["currentPage"] = lblbreadcum;
                //}
                //if (oBROKER == null)
                //{
                //    ViewBag.Message = new CommonFunction().MessageForView(this.ControllerContext.RouteData.Values["action"].ToString());
                //    ViewBag.breadcum = "Add " + Session["currentPage"];
                //    //ViewBag.departmentList = new SelectList(new Entities().DEPARTMENTs, "REFERENCE", "NAME");
                //    //ViewBag.userGroupList = new SelectList(new Entities().USERGROUPs, "REFERENCE", "NAME");

                //    return PartialView();
                //}
                //else
                //{
                //    ViewBag.breadcum = "Update " + Session["currentPage"];
                //    return PartialView("EditBroker", oBROKER);
                //}
                ViewBag.Message = new CommonFunction().MessageForView(this.ControllerContext.RouteData.Values["action"].ToString());
                ViewBag.breadcum = oCommonFunction.GetAddPath(Session["Path"] as IHtmlString, Session["currentPage"].ToString());
                ViewBag.Header = "Add " + Session["currentPage"];

                //ViewBag.departmentList = new SelectList(new Entities().DEPARTMENTs, "REFERENCE", "NAME");
                //ViewBag.userGroupList = new SelectList(new Entities().USERGROUPs, "REFERENCE", "NAME");

                return PartialView("AddBroker");





            }

            catch (Exception ex)
            {
                string message = ex.Message;

                return RedirectToAction("Index", "ErrorPage", new { message });

            }
        }


        [HttpPost]
        public ActionResult AddBroker(BROKER oBROKER)
        {
            //DEPARTMENT department = new Entities().DEPARTMENTs.Where(dep => dep.REFERENCE == oAPPLICATIONUSER.DEPARTMENT_REFERENCE).FirstOrDefault();
            //USERGROUP userGroup = new Entities().USERGROUPs.Where(ug => ug.REFERENCE == oAPPLICATIONUSER.USERGROUP_REFERENCE).FirstOrDefault();

            if (Session["UserId"] == null || Session["PreviousPage"] == null || Session["currentPage"] == null || Session["Connection"] == null)
            {
                return RedirectToAction("LogOut", "Home");

            }
            
            try
            {

                if (Request.IsAjaxRequest())
                {

                    oBROKER.REFERENCE = Guid.NewGuid().ToString();

                    oBROKER.CREATEDBY = Session["UserId"].ToString();
                    oBROKER.CREATEDDATE = DateTime.Today;
                    oBROKER.LASTUPDATED = DateTime.Today;

                    oCommonFunction.CustomObjectNullValidation<BROKER>(ref oBROKER);
                    //oBROKER.CREATEDBY = "BOSL";
                    //oBROKER.CREATEDDATE = DateTime.Today;
                    //oBROKER.LASTUPDATED = DateTime.Today;
                    //oBROKER.LASTUPDATEDBY = "BOSL";
                    //oBROKER.MEMBERID = oBROKER.MEMBERID == null ? string.Empty : oBROKER.MEMBERID;
                    //oBROKER.CDBLID = oBROKER.CDBLID == null ? string.Empty : oBROKER.CDBLID;
                    //oBROKER.BONUMBER = oBROKER.BONUMBER == null ? string.Empty : oBROKER.BONUMBER;
                    //oBROKER.DSEEXCHANGEID = oBROKER.DSEEXCHANGEID == null ? string.Empty : oBROKER.DSEEXCHANGEID;
                    //oBROKER.CSEEXCHANGEID = oBROKER.CSEEXCHANGEID == null ? string.Empty : oBROKER.CSEEXCHANGEID;
                    //oBROKER.DSECLEARINGBO = oBROKER.DSECLEARINGBO == null ? string.Empty : oBROKER.DSECLEARINGBO;
                    //oBROKER.CSECLEARINGBO = oBROKER.CSECLEARINGBO == null ? string.Empty : oBROKER.CSECLEARINGBO;
                    //oBROKER.NAME = oBROKER.NAME == null ? string.Empty : oBROKER.NAME;
                    //oBROKER.COMMISSIONRATE = oBROKER.COMMISSIONRATE == null ? (decimal)0.00 : oBROKER.COMMISSIONRATE;
                    //oBROKER.MINIMUMAMOUNT = oBROKER.MINIMUMAMOUNT == null ? (decimal)0.00 : oBROKER.MINIMUMAMOUNT;
                    //oBROKER.DEFAULTTRADER = oBROKER.DEFAULTTRADER == null ? string.Empty : oBROKER.DEFAULTTRADER;

                    using (Entities db = new Entities(Session["Connection"] as EntityConnection))
                    {
                        db.BROKERs.Add(oBROKER);
                        db.SaveChanges();
                    }

                }
                return RedirectToAction("ListBroker", "Broker");


            }
            catch (Exception ex)
            {

                string message = ex.Message;

                return RedirectToAction("Index", "ErrorPage", new { message });
            }

        }




        [HttpGet]
        public ActionResult EditBroker(string id)
        {

            if (Session["UserId"] == null || Session["PreviousPage"] == null || Session["currentPage"] == null || Session["Connection"] == null)
            {
                return RedirectToAction("LogOut", "Home");

            }
            try
            {
                BROKER oBROKER = new BROKER();
                Entities db = new Entities(Session["Connection"] as EntityConnection);
                ViewModelBase oViewModelBase = new ViewModelBase();


                oBROKER = db.BROKERs.SingleOrDefault(i => i.REFERENCE == id);
                //ViewBag.departmentList = new SelectList(new Entities().DEPARTMENTs, "REFERENCE", "NAME");
                //ViewBag.userGroupList = new SelectList(new Entities().USERGROUPs, "REFERENCE", "NAME");
                ViewBag.Message = new CommonFunction().MessageForView(this.ControllerContext.RouteData.Values["action"].ToString());
                ViewBag.breadcum = oCommonFunction.GetEditPath(Session["Path"] as IHtmlString, Session["currentPage"].ToString());
                ViewBag.Header = "Update " + Session["currentPage"];

                return PartialView("EditBroker", oBROKER);
            }

            catch (Exception ex)
            {
                string message = ex.Message;

                return RedirectToAction("Index", "ErrorPage", new { message });

            }
        }



        [HttpPost]
        public ActionResult EditBroker(BROKER oBROKER)
        {

            if (Session["UserId"] == null || Session["PreviousPage"] == null || Session["currentPage"] == null || Session["Connection"] == null)
            {
                return RedirectToAction("LogOut", "Home");

            }
            try
            {

                if (Request.IsAjaxRequest())
                {
                    //oBROKER.MEMBERID = oBROKER.MEMBERID == null ? string.Empty : oBROKER.MEMBERID;
                    //oBROKER.CDBLID = oBROKER.CDBLID == null ? string.Empty : oBROKER.CDBLID;
                    //oBROKER.BONUMBER = oBROKER.BONUMBER == null ? string.Empty : oBROKER.BONUMBER;
                    //oBROKER.DSEEXCHANGEID = oBROKER.DSEEXCHANGEID == null ? string.Empty : oBROKER.DSEEXCHANGEID;
                    //oBROKER.CSEEXCHANGEID = oBROKER.CSEEXCHANGEID == null ? string.Empty : oBROKER.CSEEXCHANGEID;
                    //oBROKER.DSECLEARINGBO = oBROKER.DSECLEARINGBO == null ? string.Empty : oBROKER.DSECLEARINGBO;
                    //oBROKER.CSECLEARINGBO = oBROKER.CSECLEARINGBO == null ? string.Empty : oBROKER.CSECLEARINGBO;
                    //oBROKER.NAME = oBROKER.NAME == null ? string.Empty : oBROKER.NAME;
                    //oBROKER.COMMISSIONRATE = oBROKER.COMMISSIONRATE == null ? (decimal)0.00 : oBROKER.COMMISSIONRATE;
                    //oBROKER.MINIMUMAMOUNT = oBROKER.MINIMUMAMOUNT == null ? (decimal)0.00 : oBROKER.MINIMUMAMOUNT;
                    //oBROKER.DEFAULTTRADER = oBROKER.DEFAULTTRADER == null ? string.Empty : oBROKER.DEFAULTTRADER;
                    oBROKER.LASTUPDATED = DateTime.Today;
                    oBROKER.LASTUPDATEDBY = Session["UserId"].ToString();
                    oCommonFunction.CustomObjectNullValidation<BROKER>(ref oBROKER);
                    using (Entities db = new Entities(Session["Connection"] as EntityConnection))
                    {

                        db.Entry(oBROKER).State = EntityState.Modified;
                        db.SaveChanges();
                    }
                }
                return RedirectToAction("ListBroker", "Broker");

            }

            catch (Exception ex)
            {
                string message = ex.Message;

                return RedirectToAction("Index", "ErrorPage", new { message });
            }

        }

    }
}

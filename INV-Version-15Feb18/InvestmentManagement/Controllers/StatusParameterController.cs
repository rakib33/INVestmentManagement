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
    public class StatusParameterController : Controller
    {
        CommonFunction oCommonFunction = new CommonFunction();
        //
        // GET: /StatusParameter/

        [HttpGet]
        public ActionResult ListStatusParameter(string sortdir, string sort, int? page, int? rows, string filterstring, string currentFilter, string lblbreadcum, string PagingType, int? currentRowPerPage = 15)
        {
            try
            {
                if (oCommonFunction.CheckSession() == true)
                {
                    return RedirectToAction("LogOut", "Home");
                }

                //currentRowPerPage=@ViewBag.currentRowPerPage
                GridModel<STATUSPARAMETER> gridModels = new GridModel<STATUSPARAMETER>();
                List<STATUSPARAMETER> models = null;

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
                    models = new Entities(Session["Connection"] as EntityConnection).STATUSPARAMETERs.AsNoTracking().OrderBy(sort + " " + sortdir).Skip(skipcount).Take(gridModels.RowsPerPage).ToList();
                else
                    models = new Entities(Session["Connection"] as EntityConnection).STATUSPARAMETERs.AsNoTracking().Where(w => w.ENTITY.Contains(filterstring)).OrderBy(sort + " " + sortdir).Skip(skipcount).Take(gridModels.RowsPerPage).ToList();


                gridModels.DataModel = models;

                ViewBag.Message = oCommonFunction.MessageForView(this.ControllerContext.RouteData.Values["action"].ToString());
                
                
                
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
                return PartialView("ListStatusParameter", gridModels);



            }

            catch (Exception ex)
            {
                string message = ex.Message;

                return RedirectToAction("Index", "ErrorPage", new { message });
            }



        }

        [HttpGet]
        public ActionResult AddStatusParameter()
        {
            try
            {
                if (oCommonFunction.CheckSession() == true)
                {
                    return RedirectToAction("LogOut", "Home");
                }

                ViewBag.Message = new CommonFunction().MessageForView(this.ControllerContext.RouteData.Values["action"].ToString());
                ViewBag.breadcum = oCommonFunction.GetAddPath(Session["Path"] as IHtmlString, Session["currentPage"].ToString());
                ViewBag.Header = "Add " + Session["currentPage"];
                //ViewBag.departmentList = new SelectList(new Entities().DEPARTMENTs, "REFERENCE", "NAME");
                //ViewBag.userGroupList = new SelectList(new Entities().USERGROUPs, "REFERENCE", "NAME");

                return PartialView();
            }

            catch (Exception ex)
            {
                throw ex;

            }
        }

        [HttpPost]
        public ActionResult AddStatusParameter(STATUSPARAMETER oSTATUSPARAMETER)
        {
            //DEPARTMENT department = new Entities().DEPARTMENTs.Where(dep => dep.REFERENCE == oAPPLICATIONUSER.DEPARTMENT_REFERENCE).FirstOrDefault();
            //USERGROUP userGroup = new Entities().USERGROUPs.Where(ug => ug.REFERENCE == oAPPLICATIONUSER.USERGROUP_REFERENCE).FirstOrDefault();
            try
            {

                if (oCommonFunction.CheckSession() == true)
                {
                    return RedirectToAction("LogOut", "Home");
                }

                if (Request.IsAjaxRequest())
                {

                    oSTATUSPARAMETER.REFERENCE = Guid.NewGuid().ToString();
                    oSTATUSPARAMETER.CREATEDBY = Session["UserId"].ToString();
                    oSTATUSPARAMETER.CREATEDDATE = DateTime.Now;
                    oSTATUSPARAMETER.LASTUPDATED = DateTime.Now;

                    oCommonFunction.CustomObjectNullValidation<STATUSPARAMETER>(ref oSTATUSPARAMETER);
                    //oSTATUSPARAMETER.CREATEDBY = "BOSL";
                    //oSTATUSPARAMETER.CREATEDDATE = DateTime.Now;
                    //oSTATUSPARAMETER.LASTUPDATED = DateTime.Parse("04-04-2015");
                    //oSTATUSPARAMETER.LASTUPDATEDBY = "BOSL";
                    oSTATUSPARAMETER.ISHIDDEN = oSTATUSPARAMETER.ISHIDDEN == "true" ? "Y" : "N";
                    oSTATUSPARAMETER.ISACTIVE = oSTATUSPARAMETER.ISACTIVE == "true" ? "Y" : "N";

                    using (Entities db = new Entities(Session["Connection"] as EntityConnection))
                    {
                        db.STATUSPARAMETERs.Add(oSTATUSPARAMETER);
                        db.SaveChanges();
                    }

                }
                return RedirectToAction("ListStatusParameter", "StatusParameter");


            }
            catch (Exception ex)
            {

                string message = ex.Message;

                return RedirectToAction("Index", "ErrorPage", new { message });
            }

        }

        [HttpGet]
        public ActionResult EditStatusParameter(string id)
        {
            try
            {
                if (oCommonFunction.CheckSession() == true)
                {
                    return RedirectToAction("LogOut", "Home");
                }
                STATUSPARAMETER oSTATUSPARAMETER = new STATUSPARAMETER();
                Entities db = new Entities(Session["Connection"] as EntityConnection);
                ViewModelBase oViewModelBase = new ViewModelBase();


                oSTATUSPARAMETER = db.STATUSPARAMETERs.SingleOrDefault(i => i.REFERENCE == id);
                //ViewBag.departmentList = new SelectList(new Entities().DEPARTMENTs, "REFERENCE", "NAME");
                //ViewBag.userGroupList = new SelectList(new Entities().USERGROUPs, "REFERENCE", "NAME");
                ViewBag.Message = new CommonFunction().MessageForView(this.ControllerContext.RouteData.Values["action"].ToString());
                ViewBag.breadcum = oCommonFunction.GetEditPath(Session["Path"] as IHtmlString, Session["currentPage"].ToString());
                ViewBag.Header = "Update " + Session["currentPage"];

                return PartialView("EditStatusParameter", oSTATUSPARAMETER);
            }

            catch (Exception ex)
            {
                throw ex;

            }
        }

        [HttpPost]
        public ActionResult EditStatusParameter(STATUSPARAMETER oSTATUSPARAMETER)
        {
            //oAPPLICATIONUSER.CREATEDBY = "BOSL";
            //oAPPLICATIONUSER.CREATEDDATE = DateTime.Parse("05-04-2015");
            //oAPPLICATIONUSER.DEPARTMENT = department;
            //oAPPLICATIONUSER.LASTUPDATED = DateTime.Parse("04-04-2015");
            //oAPPLICATIONUSER.LASTUPDATEDBY = "BOSL";
            //oAPPLICATIONUSER.USERID = "1254";
            //oAPPLICATIONUSER.LASTLOGIN = DateTime.Now;

            try
            {
                if (oCommonFunction.CheckSession() == true)
                {
                    return RedirectToAction("LogOut", "Home");
                }

                using (Entities db = new Entities(Session["Connection"] as EntityConnection))
                {
                    oSTATUSPARAMETER.LASTUPDATED = DateTime.Now;
                    oSTATUSPARAMETER.LASTUPDATEDBY = Session["UserId"].ToString();
                    oCommonFunction.CustomObjectNullValidation<STATUSPARAMETER>(ref oSTATUSPARAMETER);
                    oSTATUSPARAMETER.ISHIDDEN = oSTATUSPARAMETER.ISHIDDEN == "true" ? "Y" : "N";
                    oSTATUSPARAMETER.ISACTIVE = oSTATUSPARAMETER.ISACTIVE == "true" ? "Y" : "N";
                    db.Entry(oSTATUSPARAMETER).State = EntityState.Modified;
                    db.SaveChanges();
                }

                return RedirectToAction("ListStatusParameter", "StatusParameter");

            }

            catch (Exception ex)
            {
                string message = ex.Message;

                return RedirectToAction("Index", "ErrorPage", new { message });
            }

        }
    }
}

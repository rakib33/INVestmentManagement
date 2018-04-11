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
    public class ApplicationParameterController : Controller
    {
        //
        // GET: /ApplicationParameter/


        CommonFunction oCommonFunction = new CommonFunction();

        [HttpGet]
        public ActionResult ListApplicationParameter(string sortdir, string sort, int? page, int? rows, string filterstring, string currentFilter, string lblbreadcum, string PagingType, int? currentRowPerPage = 15, string reference = null)
         {
            try
            {
                //currentRowPerPage=@ViewBag.currentRowPerPage
                GridModel<APPLICATIONPARAMETER> gridModels = new GridModel<APPLICATIONPARAMETER>();
                List<APPLICATIONPARAMETER> models = null;
                
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
                    models = new Entities(Session["Connection"] as EntityConnection).APPLICATIONPARAMETERs.AsNoTracking().OrderBy(sort + " " + sortdir).Skip(skipcount).Take(gridModels.RowsPerPage).ToList();
                else
                    models = new Entities(Session["Connection"] as EntityConnection).APPLICATIONPARAMETERs.AsNoTracking().Where(w => w.ENTITY.Contains(filterstring)).OrderBy(sort + " " + sortdir).Skip(skipcount).Take(gridModels.RowsPerPage).ToList();



               


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
                return PartialView("ListApplicationParameter", gridModels);



            }

            catch (Exception ex)
            {
                string message = ex.Message;

                return RedirectToAction("Index", "ErrorPage", new { message });
            }



        }


        [HttpGet]
        public ActionResult AddApplicationParameter()
        {
            try
            {
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
        public ActionResult AddApplicationParameter(APPLICATIONPARAMETER oAPPLICATIONPARAMETER)
        {
            //DEPARTMENT department = new Entities().DEPARTMENTs.Where(dep => dep.REFERENCE == oAPPLICATIONUSER.DEPARTMENT_REFERENCE).FirstOrDefault();
            //USERGROUP userGroup = new Entities().USERGROUPs.Where(ug => ug.REFERENCE == oAPPLICATIONUSER.USERGROUP_REFERENCE).FirstOrDefault();
            try
            {



                if (Request.IsAjaxRequest())
                {

                    oAPPLICATIONPARAMETER.REFERENCE = Guid.NewGuid().ToString();
                    oAPPLICATIONPARAMETER.CREATEDBY = Session["UserId"].ToString();
                    oAPPLICATIONPARAMETER.CREATEDDATE = DateTime.Today;
                    oAPPLICATIONPARAMETER.LASTUPDATED = DateTime.Today;
                  
                    oCommonFunction.CustomObjectNullValidation<APPLICATIONPARAMETER>(ref oAPPLICATIONPARAMETER);
                    oAPPLICATIONPARAMETER.ISHIDDEN = oAPPLICATIONPARAMETER.ISHIDDEN == "true"  ? "Y" : "N";
                    oAPPLICATIONPARAMETER.ISACTIVE = oAPPLICATIONPARAMETER.ISACTIVE == "true" ? "Y" : "N";
                    //oAPPLICATIONPARAMETER.ENTITY = oAPPLICATIONPARAMETER.ENTITY == null ? string.Empty : oAPPLICATIONPARAMETER.ENTITY;
                    //oAPPLICATIONPARAMETER.DESCRIPTION = oAPPLICATIONPARAMETER.DESCRIPTION == null ? string.Empty : oAPPLICATIONPARAMETER.DESCRIPTION;
                    //oAPPLICATIONPARAMETER.PROPERTY = oAPPLICATIONPARAMETER.PROPERTY == null ? string.Empty : oAPPLICATIONPARAMETER.PROPERTY;


                    using (Entities db = new Entities(Session["Connection"] as EntityConnection))
                    {
                        db.APPLICATIONPARAMETERs.Add(oAPPLICATIONPARAMETER);
                        db.SaveChanges();
                    }

                }
                return RedirectToAction("ListApplicationParameter", "ApplicationParameter");


            }
            catch (Exception ex)
            {

                string message = ex.Message;

                return RedirectToAction("Index", "ErrorPage", new { message });
            }

        }


        [HttpGet]
        public ActionResult EditApplicationParameter(string id)
        {
            try
            {
                APPLICATIONPARAMETER oAPPLICATIONPARAMETER = new APPLICATIONPARAMETER();
                Entities db = new Entities(Session["Connection"] as EntityConnection);
                ViewModelBase oViewModelBase = new ViewModelBase();


                oAPPLICATIONPARAMETER = db.APPLICATIONPARAMETERs.SingleOrDefault(i => i.REFERENCE == id);
                //ViewBag.departmentList = new SelectList(new Entities().DEPARTMENTs, "REFERENCE", "NAME");
                //ViewBag.userGroupList = new SelectList(new Entities().USERGROUPs, "REFERENCE", "NAME");
                ViewBag.Message = new CommonFunction().MessageForView(this.ControllerContext.RouteData.Values["action"].ToString());
                ViewBag.breadcum = oCommonFunction.GetEditPath(Session["Path"] as IHtmlString, Session["currentPage"].ToString());
                ViewBag.Header = "Update " + Session["currentPage"];

                return PartialView("EditApplicationParameter", oAPPLICATIONPARAMETER);
            }

            catch (Exception ex)
            {
                string message = ex.Message;

                return RedirectToAction("Index", "ErrorPage", new { message });

            }
        }


        [HttpPost]
        public ActionResult EditApplicationParameter(APPLICATIONPARAMETER oAPPLICATIONPARAMETER)
        {
            //oAPPLICATIONUSER.CREATEDBY = "BOSL";
            //oAPPLICATIONUSER.CREATEDDATE = DateTime.Parse("05-04-2015");
            //oAPPLICATIONUSER.DEPARTMENT = department;
            oAPPLICATIONPARAMETER.LASTUPDATED = DateTime.Now;
            oAPPLICATIONPARAMETER.LASTUPDATEDBY = Session["UserId"].ToString();
            //oAPPLICATIONUSER.LASTUPDATEDBY = "BOSL";
            //oAPPLICATIONUSER.USERID = "1254";
            //oAPPLICATIONPARAMETER.ENTITY = oAPPLICATIONPARAMETER.ENTITY == null ? string.Empty : oAPPLICATIONPARAMETER.ENTITY;
            //oAPPLICATIONPARAMETER.DESCRIPTION = oAPPLICATIONPARAMETER.DESCRIPTION == null ? string.Empty : oAPPLICATIONPARAMETER.DESCRIPTION;
            //oAPPLICATIONPARAMETER.PROPERTY = oAPPLICATIONPARAMETER.PROPERTY == null ? string.Empty : oAPPLICATIONPARAMETER.PROPERTY;
            oCommonFunction.CustomObjectNullValidation<APPLICATIONPARAMETER>(ref oAPPLICATIONPARAMETER);
            oAPPLICATIONPARAMETER.ISHIDDEN = oAPPLICATIONPARAMETER.ISHIDDEN == "true" ? "Y" : "N";
            oAPPLICATIONPARAMETER.ISACTIVE = oAPPLICATIONPARAMETER.ISACTIVE == "true" ? "Y" : "N";

            try
            {

                using (Entities db = new Entities(Session["Connection"] as EntityConnection))
                {
                   
                    db.Entry(oAPPLICATIONPARAMETER).State = EntityState.Modified;
                    db.SaveChanges();
                }

                return RedirectToAction("ListApplicationParameter", "ApplicationParameter");

            }

            catch (Exception ex)
            {
                string message = ex.Message;

                return RedirectToAction("Index", "ErrorPage", new { message });
            }

        }

    }
}

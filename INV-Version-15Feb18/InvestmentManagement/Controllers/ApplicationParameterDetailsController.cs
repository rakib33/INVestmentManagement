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
    public class ApplicationParameterDetailsController : Controller
    {
        CommonFunction oCommonFunction = new CommonFunction();
        //
        // GET: /ApplicationParameterDetails/

        [HttpGet]
        public ActionResult ListApplicationParameterDetails(string sortdir, string sort, int? page, int? rows, string filterstring, string currentFilter, string lblbreadcum, string PagingType, int? currentRowPerPage = 15, string reference = null)
        {
            try
            {
                //currentRowPerPage=@ViewBag.currentRowPerPage
                GridModel<APPLICATIONPARAMETERDETAIL> gridModels = new GridModel<APPLICATIONPARAMETERDETAIL>();
                List<APPLICATIONPARAMETERDETAIL> models = null;
                string apReference = string.Empty;
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

               
                //if (string.IsNullOrEmpty(filterstring))
                //    models = new Entities(Session["Connection"] as EntityConnection).APPLICATIONPARAMETERDETAILS.Include("APPLICATIONPARAMETER").AsNoTracking().OrderBy(sort + " " + sortdir).Skip(skipcount).Take(gridModels.RowsPerPage).ToList();
                //else
                //    models = new Entities(Session["Connection"] as EntityConnection).APPLICATIONPARAMETERDETAILS.Include("APPLICATIONPARAMETER").AsNoTracking().Where(w => w.DESCRIPTION.Contains(filterstring)).OrderBy(sort + " " + sortdir).Skip(skipcount).Take(gridModels.RowsPerPage).ToList();


                //if (!(string.IsNullOrEmpty(reference)))
                //{
                //    Session["Reference"] = reference;
                    
                //}
                //apReference = Session["Reference"].ToString();

                if (TempData["refernce"] != null)
                {
                    reference = TempData["refernce"].ToString();
                }

                TempData["ref"] = reference;
            //    models = models.Where(m => m.APPLICATIONPARAMETER_REFERENCE == reference).ToList();
                models = new Entities(Session["Connection"] as EntityConnection).APPLICATIONPARAMETERDETAILS.Include("APPLICATIONPARAMETER").AsNoTracking().Where(t => t.APPLICATIONPARAMETER_REFERENCE == reference).OrderBy(t=>t.CODE).ToList();

                gridModels.DataModel = models;
                
                ViewBag.Message = oCommonFunction.MessageForView(this.ControllerContext.RouteData.Values["action"].ToString());
                
                if (!string.IsNullOrEmpty(lblbreadcum))
                {
                    Session["currentPage"] = lblbreadcum;
                }
                ViewBag.breadcum = Session["currentPage"] + " Details";
                if (models.Count() < gridModels.RowsPerPage)
                {

                    ViewBag.Prev = "disabled";
                    ViewBag.Next = "disabled";
                    ViewBag.PrevNotActive = "not-active";
                    ViewBag.NextNotActive = "not-active";
                }
                return PartialView("ListApplicationParameterDetails", gridModels);



            }

            catch (Exception ex)
            {
                string message = ex.Message;

                return RedirectToAction("Index", "ErrorPage", new { message });
            }



        }

        [HttpGet]
        public ActionResult AddApplicationParameterDetails()
        {
            try
            {
                ViewBag.Message = new CommonFunction().MessageForView(this.ControllerContext.RouteData.Values["action"].ToString());
                ViewBag.breadcum = "Add " + Session["currentPage"]+" Details";
                //ViewBag.applicationParameterList = new SelectList(new Entities().APPLICATIONPARAMETERs, "REFERENCE", "ENTITY");
                //ViewBag.userGroupList = new SelectList(new Entities().USERGROUPs, "REFERENCE", "NAME");
                ViewBag.reference=TempData["ref"].ToString();

                return PartialView();
            }

            catch (Exception ex)
            {
                string message = ex.Message;

                return RedirectToAction("Index", "ErrorPage", new { message });

            }
        }

        [HttpPost]
        public ActionResult AddApplicationParameterDetails(APPLICATIONPARAMETERDETAIL oAPPLICATIONPARAMETERDETAIL)
        {
            //DEPARTMENT department = new Entities().DEPARTMENTs.Where(dep => dep.REFERENCE == oAPPLICATIONUSER.DEPARTMENT_REFERENCE).FirstOrDefault();
            //USERGROUP userGroup = new Entities().USERGROUPs.Where(ug => ug.REFERENCE == oAPPLICATIONUSER.USERGROUP_REFERENCE).FirstOrDefault();
            try
            {



                if (Request.IsAjaxRequest())
                {

                    oAPPLICATIONPARAMETERDETAIL.REFERENCE = Guid.NewGuid().ToString();

                    oAPPLICATIONPARAMETERDETAIL.CREATEDBY = Session["UserId"].ToString();
                    oAPPLICATIONPARAMETERDETAIL.CREATEDDATE = DateTime.Today;
                    oAPPLICATIONPARAMETERDETAIL.LASTUPDATED = DateTime.Today;

                    oCommonFunction.CustomObjectNullValidation<APPLICATIONPARAMETERDETAIL>(ref oAPPLICATIONPARAMETERDETAIL);
                    //oAPPLICATIONPARAMETERDETAIL.CREATEDBY = "BOSL";
                    //oAPPLICATIONPARAMETERDETAIL.CREATEDDATE = DateTime.Today;
                    //oAPPLICATIONPARAMETERDETAIL.LASTUPDATED = DateTime.Today;
                    //oAPPLICATIONPARAMETERDETAIL.LASTUPDATEDBY = "BOSL";
                    oAPPLICATIONPARAMETERDETAIL.ISHIDDEN = oAPPLICATIONPARAMETERDETAIL.ISHIDDEN == "true" ? "Y" : "N";
                    oAPPLICATIONPARAMETERDETAIL.ISACTIVE = oAPPLICATIONPARAMETERDETAIL.ISACTIVE == "true" ? "Y" : "N";
                    oAPPLICATIONPARAMETERDETAIL.ISDEFAULT = oAPPLICATIONPARAMETERDETAIL.ISDEFAULT == "true" ? "Y" : "N";
                    oAPPLICATIONPARAMETERDETAIL.DESCRIPTION = oAPPLICATIONPARAMETERDETAIL.DESCRIPTION == null ? string.Empty : oAPPLICATIONPARAMETERDETAIL.DESCRIPTION;

                    //oUSERGROUP.LASTLOGIN = DateTime.Now;
                    TempData["refernce"] = oAPPLICATIONPARAMETERDETAIL.APPLICATIONPARAMETER_REFERENCE;
                    using (Entities db = new Entities(Session["Connection"] as EntityConnection))
                    {
                        db.APPLICATIONPARAMETERDETAILS.Add(oAPPLICATIONPARAMETERDETAIL);
                        db.SaveChanges();
                    }

                }
                return RedirectToAction("ListApplicationParameterDetails", "ApplicationParameterDetails");


            }
            catch (Exception ex)
            {

                string message = ex.Message;

                return RedirectToAction("Index", "ErrorPage", new { message });
            }

        }


        [HttpGet]
        public ActionResult EditApplicationParameterDetails(string id)
        {
            try
            {
                APPLICATIONPARAMETERDETAIL oAPPLICATIONPARAMETERDETAIL = new APPLICATIONPARAMETERDETAIL();
                Entities db = new Entities(Session["Connection"] as EntityConnection);
                ViewModelBase oViewModelBase = new ViewModelBase();


                oAPPLICATIONPARAMETERDETAIL = db.APPLICATIONPARAMETERDETAILS.Include("APPLICATIONPARAMETER").SingleOrDefault(i => i.REFERENCE == id);
                ViewBag.applicationParameterList = new SelectList(new Entities(Session["Connection"] as EntityConnection).APPLICATIONPARAMETERs, "REFERENCE", "ENTITY");
                //ViewBag.userGroupList = new SelectList(new Entities().USERGROUPs, "REFERENCE", "NAME");
                ViewBag.Message = new CommonFunction().MessageForView(this.ControllerContext.RouteData.Values["action"].ToString());
                ViewBag.breadcum = "Update " + Session["currentPage"]+" Details";

                return PartialView("", oAPPLICATIONPARAMETERDETAIL);
            }

            catch (Exception ex)
            {
                string message = ex.Message;

                return RedirectToAction("Index", "ErrorPage", new { message });

            }
        }

        [HttpPost]
        public ActionResult EditApplicationParameterDetails(APPLICATIONPARAMETERDETAIL oAPPLICATIONPARAMETERDETAIL)
        {
            //oAPPLICATIONUSER.CREATEDBY = "BOSL";
            //oAPPLICATIONUSER.CREATEDDATE = DateTime.Parse("05-04-2015");
            //oAPPLICATIONUSER.DEPARTMENT = department;
            oAPPLICATIONPARAMETERDETAIL.LASTUPDATED = DateTime.Today;
            oAPPLICATIONPARAMETERDETAIL.LASTUPDATEDBY = Session["UserId"].ToString();

            //oAPPLICATIONUSER.USERID = "1254";
            //oAPPLICATIONUSER.LASTLOGIN = DateTime.Now;
            oAPPLICATIONPARAMETERDETAIL.DESCRIPTION = oAPPLICATIONPARAMETERDETAIL.DESCRIPTION == null ? string.Empty : oAPPLICATIONPARAMETERDETAIL.DESCRIPTION;
            oCommonFunction.CustomObjectNullValidation<APPLICATIONPARAMETERDETAIL>(ref oAPPLICATIONPARAMETERDETAIL);
            try
            {
                TempData["refernce"] = oAPPLICATIONPARAMETERDETAIL.APPLICATIONPARAMETER_REFERENCE;
                using (Entities db = new Entities(Session["Connection"] as EntityConnection))
                {
                    oAPPLICATIONPARAMETERDETAIL.ISHIDDEN = oAPPLICATIONPARAMETERDETAIL.ISHIDDEN == "true" ? "Y" : "N";
                    oAPPLICATIONPARAMETERDETAIL.ISACTIVE = oAPPLICATIONPARAMETERDETAIL.ISACTIVE == "true" ? "Y" : "N";
                    oAPPLICATIONPARAMETERDETAIL.ISDEFAULT = oAPPLICATIONPARAMETERDETAIL.ISDEFAULT == "true" ? "Y" : "N";
                    
                    db.Entry(oAPPLICATIONPARAMETERDETAIL).State = EntityState.Modified;
                    db.SaveChanges();
                }

                return RedirectToAction("ListApplicationParameterDetails", "ApplicationParameterDetails");

            }

            catch (Exception ex)
            {
                string message = ex.Message;

                return RedirectToAction("Index", "ErrorPage", new { message });
            }

        }
    }
}

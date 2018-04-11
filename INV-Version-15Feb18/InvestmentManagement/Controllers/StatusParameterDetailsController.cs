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
    public class StatusParameterDetailsController : Controller
    {
        CommonFunction oCommonFunction = new CommonFunction();
        //
        // GET: /StatusParameterDetails/

        [HttpGet]
        public ActionResult ListStatusParameterDetails(string sortdir, string sort, int? page, int? rows, string filterstring, string currentFilter, string lblbreadcum, string PagingType, int? currentRowPerPage = 15,string reference=null)
        {
            try
            {
                //currentRowPerPage=@ViewBag.currentRowPerPage
                GridModel<STATUSPARAMETERDETAIL> gridModels = new GridModel<STATUSPARAMETERDETAIL>();
                List<STATUSPARAMETERDETAIL> models = null;
                string spReference=string.Empty;
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
                    models = new Entities(Session["Connection"] as EntityConnection).STATUSPARAMETERDETAILS.Include("STATUSPARAMETER").AsNoTracking().OrderBy(sort + " " + sortdir).Skip(skipcount).Take(gridModels.RowsPerPage).ToList();
                else
                    models = new Entities(Session["Connection"] as EntityConnection).STATUSPARAMETERDETAILS.Include("STATUSPARAMETER").AsNoTracking().Where(w => w.DESCRIPTION.Contains(filterstring)).OrderBy(sort + " " + sortdir).Skip(skipcount).Take(gridModels.RowsPerPage).ToList();


                //if (!(string.IsNullOrEmpty(reference)))
                //{
                //    Session["brokReference"] = reference;

                //}
                //spReference = Session["brokReference"].ToString();

                if (TempData["refernce"] != null)
                {
                    reference = TempData["refernce"].ToString();
                }

                TempData["ref"] = reference;

                models = models.Where(m => m.STATUSPARAMETER_REFERENCE == reference).ToList();

                gridModels.DataModel = models;

                //ViewBag.Message = oCommonFunction.MessageForView(this.ControllerContext.RouteData.Values["action"].ToString());
                //if (!string.IsNullOrEmpty(lblbreadcum))
                //{
                //    Session["currentPage"] = lblbreadcum;
                //    Session["Path"] = oCommonFunction.GetDetailsPath(Session["Path"] as IHtmlString, this.ControllerContext.RouteData.Values["controller"].ToString(), Session["currentPage"].ToString(), this.ControllerContext.RouteData.Values["action"].ToString(), reference);
                //}

                //ViewBag.BreadCum = oCommonFunction.GetDetailsListPath(Session["Path"] as IHtmlString, Session["currentPage"].ToString());
                //ViewBag.breadcum = Session["currentPage"] + " Details";
                ViewBag.Message = oCommonFunction.MessageForView(this.ControllerContext.RouteData.Values["action"].ToString());
                if (!string.IsNullOrEmpty(lblbreadcum))
                {
                    Session["currentPage"] = lblbreadcum;
                    Session["Path"] = oCommonFunction.GetDetailsPath(Session["Path"] as IHtmlString, this.ControllerContext.RouteData.Values["controller"].ToString(), Session["currentPage"].ToString(), this.ControllerContext.RouteData.Values["action"].ToString(), reference);
                }



                ViewBag.BreadCum = oCommonFunction.GetDetailsListPath(Session["Path"] as IHtmlString, Session["currentPage"].ToString());
                //if (models.Count() < gridModels.RowsPerPage)

                if (models.Count() < gridModels.RowsPerPage)
                {

                    ViewBag.Prev = "disabled";
                    ViewBag.Next = "disabled";
                    ViewBag.PrevNotActive = "not-active";
                    ViewBag.NextNotActive = "not-active";
                }
                return PartialView("ListStatusParameterDetails", gridModels);



            }

            catch (Exception ex)
            {
                string message = ex.Message;

                return RedirectToAction("Index", "ErrorPage", new { message });
            }



        }

        [HttpGet]
        public ActionResult AddStatusParameterDetails()
        {
            try
            {
                ViewBag.Message = new CommonFunction().MessageForView(this.ControllerContext.RouteData.Values["action"].ToString());
                ViewBag.breadcum = oCommonFunction.GetDetailsAddPath(Session["Path"] as IHtmlString, Session["currentPage"].ToString());
                ViewBag.Header = "Add " + Session["currentPage"];
                ViewBag.statusParameterList = new SelectList(new Entities(Session["Connection"] as EntityConnection).STATUSPARAMETERs, "REFERENCE", "ENTITY");
                //ViewBag.userGroupList = new SelectList(new Entities().USERGROUPs, "REFERENCE", "NAME");
                ViewBag.reference = TempData["ref"].ToString();
                return PartialView();
            }

            catch (Exception ex)
            {
                string message = ex.Message;

                return RedirectToAction("Index", "ErrorPage", new { message });

            }
        }
        [HttpPost]
        public ActionResult AddStatusParameterDetails(STATUSPARAMETERDETAIL oSTATUSPARAMETERDETAIL)
        {
            //DEPARTMENT department = new Entities().DEPARTMENTs.Where(dep => dep.REFERENCE == oAPPLICATIONUSER.DEPARTMENT_REFERENCE).FirstOrDefault();
            //USERGROUP userGroup = new Entities().USERGROUPs.Where(ug => ug.REFERENCE == oAPPLICATIONUSER.USERGROUP_REFERENCE).FirstOrDefault();
            try
            {



                if (Request.IsAjaxRequest())
                {

                    oSTATUSPARAMETERDETAIL.REFERENCE = Guid.NewGuid().ToString();
                    //oSTATUSPARAMETERDETAIL.CREATEDBY = "BOSL";
                    //oSTATUSPARAMETERDETAIL.CREATEDDATE = DateTime.Now;
                    //oSTATUSPARAMETERDETAIL.LASTUPDATED = DateTime.Now;
                    //oSTATUSPARAMETERDETAIL.LASTUPDATEDBY = "BOSL";
                    oSTATUSPARAMETERDETAIL.ISHIDDEN = oSTATUSPARAMETERDETAIL.ISHIDDEN == "true" ? "Y" : "N";
                    oSTATUSPARAMETERDETAIL.ISACTIVE = oSTATUSPARAMETERDETAIL.ISACTIVE == "true" ? "Y" : "N";
                    oSTATUSPARAMETERDETAIL.ISDEFAULT =oSTATUSPARAMETERDETAIL.ISDEFAULT == "true" ? "Y" : "N";

                    TempData["refernce"] = oSTATUSPARAMETERDETAIL.STATUSPARAMETER_REFERENCE;
                    oSTATUSPARAMETERDETAIL.CREATEDBY = Session["UserId"].ToString();
                    oSTATUSPARAMETERDETAIL.CREATEDDATE = DateTime.Now;
                    oSTATUSPARAMETERDETAIL.LASTUPDATED = DateTime.Now;

                    oCommonFunction.CustomObjectNullValidation<STATUSPARAMETERDETAIL>(ref oSTATUSPARAMETERDETAIL);

                    //oUSERGROUP.LASTLOGIN = DateTime.Now;
                    using (Entities db = new Entities(Session["Connection"] as EntityConnection))
                    {
                        if (oSTATUSPARAMETERDETAIL.STATUSPARAMETER_REFERENCE !=null)
                        {
                            db.STATUSPARAMETERDETAILS.Add(oSTATUSPARAMETERDETAIL);
                            db.SaveChanges();
                        }
                        
                    }

                }
                return RedirectToAction("ListStatusParameterDetails", "StatusParameterDetails");


            }
            catch (Exception ex)
            {

                string message = ex.Message;

                return RedirectToAction("Index", "ErrorPage", new { message });
            }

        }


        [HttpGet]
        public ActionResult EditStatusParameterDetails(string id)
        {
            try
            {
                STATUSPARAMETERDETAIL oSTATUSPARAMETERDETAIL = new STATUSPARAMETERDETAIL();
                Entities db = new Entities(Session["Connection"] as EntityConnection);
                ViewModelBase oViewModelBase = new ViewModelBase();


                oSTATUSPARAMETERDETAIL = db.STATUSPARAMETERDETAILS.Include("STATUSPARAMETER").SingleOrDefault(i => i.REFERENCE == id);
                ViewBag.statusParameterList = new SelectList(new Entities(Session["Connection"] as EntityConnection).STATUSPARAMETERs, "REFERENCE", "ENTITY");
                //ViewBag.userGroupList = new SelectList(new Entities().USERGROUPs, "REFERENCE", "NAME");
                ViewBag.Message = new CommonFunction().MessageForView(this.ControllerContext.RouteData.Values["action"].ToString());
                ViewBag.breadcum = oCommonFunction.GetDetailsEditPath(Session["Path"] as IHtmlString, Session["currentPage"].ToString());
                ViewBag.Header = "Update " + Session["currentPage"];

                return PartialView("", oSTATUSPARAMETERDETAIL);
            }

            catch (Exception ex)
            {
                string message = ex.Message;

                return RedirectToAction("Index", "ErrorPage", new { message });

            }
        }

        [HttpPost]
        public ActionResult EditStatusParameterDetails(STATUSPARAMETERDETAIL oSTATUSPARAMETERDETAIL)
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
                TempData["refernce"] = oSTATUSPARAMETERDETAIL.STATUSPARAMETER_REFERENCE;
                using (Entities db = new Entities(Session["Connection"] as EntityConnection))
                {
                    oSTATUSPARAMETERDETAIL.LASTUPDATED = DateTime.Now;
                    oSTATUSPARAMETERDETAIL.LASTUPDATEDBY = Session["UserId"].ToString();
                    oCommonFunction.CustomObjectNullValidation<STATUSPARAMETERDETAIL>(ref oSTATUSPARAMETERDETAIL);
                    oSTATUSPARAMETERDETAIL.ISHIDDEN = oSTATUSPARAMETERDETAIL.ISHIDDEN == "true" ? "Y" : "N";
                    oSTATUSPARAMETERDETAIL.ISACTIVE = oSTATUSPARAMETERDETAIL.ISACTIVE == "true" ? "Y" : "N";
                    oSTATUSPARAMETERDETAIL.ISDEFAULT = oSTATUSPARAMETERDETAIL.ISDEFAULT == "true" ? "Y" : "N";
                    if (oSTATUSPARAMETERDETAIL.STATUSPARAMETER_REFERENCE !=null)
                    {
                        db.Entry(oSTATUSPARAMETERDETAIL).State = EntityState.Modified;
                        db.SaveChanges(); 
                    }
                    
                }

                return RedirectToAction("ListStatusParameterDetails", "StatusParameterDetails");

            }

            catch (Exception ex)
            {
                string message = ex.Message;

                return RedirectToAction("Index", "ErrorPage", new { message });
            }

        }

        //[HttpGet]
        //public ActionResult DeleteStatusParameterDetails(string id)
        //{
        //    try
        //    {
        //        STATUSPARAMETERDETAIL oSTATUSPARAMETERDETAIL = new STATUSPARAMETERDETAIL();
        //        Entities db = new Entities(Session["Connection"] as EntityConnection);
        //        ViewModelBase oViewModelBase = new ViewModelBase();


        //        oSTATUSPARAMETERDETAIL = db.STATUSPARAMETERDETAILS.Include("STATUSPARAMETER").SingleOrDefault(i => i.REFERENCE == id);

        //        db.STATUSPARAMETERDETAILS.Attach(oSTATUSPARAMETERDETAIL);
        //        db.STATUSPARAMETERDETAILS.Remove(oSTATUSPARAMETERDETAIL);
               
        //        db.SaveChanges();
        //        return RedirectToAction("ListStatusParameterDetails", "StatusParameterDetails");
        //    }

        //    catch (Exception ex)
        //    {
        //        string message = ex.Message;

        //        return RedirectToAction("Index", "ErrorPage", new { message });

        //    }
        //}
    }
}

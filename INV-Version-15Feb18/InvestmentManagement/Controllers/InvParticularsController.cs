using System;
using System.Collections.Generic;
using System.Data;
using System.Data.EntityClient;
using System.Globalization;
using System.Linq;
using System.Linq.Dynamic;
using System.Web;
using System.Web.Mvc;
using InvestmentManagement.InvestmentManagement.Models;
using InvestmentManagement.Models;
using InvestmentManagement.ViewModel;

namespace InvestmentManagement.Controllers
{
    public class InvParticularsController : Controller
    {
        //
        // GET: /InvParticulars/

        CommonFunction oCommonFunction = new CommonFunction();

        public ActionResult ListParticularsList(string sortdir, DateTime? FromDate, DateTime? ToDate, string sort, int? page, int? rows, string filterstring, string currentFilter, string lblbreadcum, string PagingType, int? currentRowPerPage = 15)
        {
            try
            {
                //Session["UserId"] = null;
                //Session["Connection"] = null;
                if (oCommonFunction.CheckSession() == true)
                {
                    return RedirectToAction("LogOut", "Home");
                }


                List<INVESTMENTPARTICULAR> models = new List<INVESTMENTPARTICULAR>();

                GridModel<INVESTMENTPARTICULAR> gridModels = new GridModel<INVESTMENTPARTICULAR>();
            
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
                    models = new Entities(Session["Connection"] as EntityConnection).INVESTMENTPARTICULARS.AsNoTracking().Where(t=>t.PARENTID != null && t.SUBGROUPNAME !=null).OrderBy(t => t.CODE).Skip(skipcount).Take(gridModels.RowsPerPage).ToList();
                //else
                //    models = new Entities(Session["Connection"] as EntityConnection).INVESTMENTPARTICULARS.AsNoTracking().Where(w => w.PARENTID != null && w.PARTICULARSNAME.Contains(filterstring)).OrderByDescending(t => t.CREATEDDATE).Skip(skipcount).Take(gridModels.RowsPerPage).ToList();

                if (models != null && FromDate.HasValue)
                    models = models.Where(t => t.CREATEDDATE >= FromDate).ToList();
                if (models != null && ToDate.HasValue)
                    models = models.Where(t => t.CREATEDDATE <= ToDate).ToList();

                gridModels.DataModel = models;

                //oJournalHeadViewModel.JournalHeads = gridModels.DataModel;
                ViewBag.Message = oCommonFunction.MessageForView(this.ControllerContext.RouteData.Values["action"].ToString());
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

                if (Convert.ToString(TempData["result"]) != null)
                {

                    ViewBag.Message = Convert.ToString(TempData["result"]);
                }

                return PartialView("ListParticularsList", gridModels);



            }

            catch (Exception ex)
            {
                string message = ex.Message;

                return RedirectToAction("Index", "ErrorPage", new { message });
            }


        }



        [HttpGet]
        public ActionResult EditParticulars(string ParticularRef)
        {

            try
            {
                if (Session["UserId"] == null || Session["PreviousPage"] == null || Session["currentPage"] == null || Session["Connection"] == null)
                {
                    return RedirectToAction("LogOut", "Home");

                }
               
                
                ViewBag.Message = new CommonFunction().MessageForView(this.ControllerContext.RouteData.Values["action"].ToString());
                ViewBag.breadcum = oCommonFunction.GetAddPath(Session["Path"] as IHtmlString, Session["currentPage"].ToString());
                ViewBag.Header = "Edit " + Session["currentPage"];

                INVESTMENTPARTICULAR model = new INVESTMENTPARTICULAR();

                using (Entities db = new Entities(Session["Connection"] as EntityConnection))
                {
                    model = db.INVESTMENTPARTICULARS.Where(t=>t.REFERENCE == ParticularRef).SingleOrDefault();
                }

                return PartialView("EditParticulars",model);
            }

            catch (Exception ex)
            {                
                TempData["result"] = ex.Message;
               
                // return RedirectToAction("Index", "ErrorPage", new { message });

            }

            return RedirectToAction("ListParticularsList", "InvParticulars");
        }


        [HttpPost]
        public ActionResult EditParticulars(string ParticularRef, INVESTMENTPARTICULAR model)
        {

            try
            {
                
                if (Session["UserId"] == null || Session["PreviousPage"] == null || Session["currentPage"] == null || Session["Connection"] == null)
                {
                    return RedirectToAction("LogOut", "Home");

                }

              
                    if (Request.IsAjaxRequest())
                    {

                        using (Entities db = new Entities(Session["Connection"] as EntityConnection))
                        {
                            INVESTMENTPARTICULAR particular = new INVESTMENTPARTICULAR();
                            particular = db.INVESTMENTPARTICULARS.Where(t => t.REFERENCE == model.REFERENCE).SingleOrDefault();

                           
                            particular.PARTICULARSNAME = model.PARTICULARSNAME;
                            particular.LIMITMINIMUM = model.LIMITMINIMUM;
                            particular.REMARKS = model.REMARKS;

                            particular.LASTUPDATEDBY = Session["UserId"].ToString();
                            particular.LASTUPDATED = DateTime.Today;

                            db.Entry(particular).State = EntityState.Modified;
                          
                            db.SaveChanges();
                        }

                    }
                  
            
                ViewBag.ParticularRef = ParticularRef;
                TempData["result"] = "Update Successfull";                    
            }

            catch (Exception ex)
            {
                TempData["result"] = ex.Message;                        

            }
            return RedirectToAction("ListParticularsList", "InvParticulars");
        }





        public ActionResult ListParticularsDetails(string ParticularRef,string sortdir, DateTime? FromDate, DateTime? ToDate, string sort, int? page, int? rows, string filterstring, string currentFilter, string lblbreadcum, string PagingType, int? currentRowPerPage = 15)
        {
            try
            {
                //Session["UserId"] = null;
                //Session["Connection"] = null;
                if (oCommonFunction.CheckSession() == true)
                {
                    return RedirectToAction("LogOut", "Home");
                }


                List<INVPARTICULARSDETAIL> models = new List<INVPARTICULARSDETAIL>();
                GridModel<INVPARTICULARSDETAIL> gridModels = new GridModel<INVPARTICULARSDETAIL>();

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
                models = new Entities(Session["Connection"] as EntityConnection).INVPARTICULARSDETAILS.AsNoTracking().Where(t => t.INVPARTICULARS_REF == ParticularRef).OrderByDescending(t=>t.CREATEDDATE).Skip(skipcount).Take(gridModels.RowsPerPage).ToList();
                //else
                //    models = new Entities(Session["Connection"] as EntityConnection).INVESTMENTPARTICULARS.AsNoTracking().Where(w => w.PARENTID != null && w.PARTICULARSNAME.Contains(filterstring)).OrderByDescending(t => t.CREATEDDATE).Skip(skipcount).Take(gridModels.RowsPerPage).ToList();

                if (models != null && FromDate.HasValue)
                    models = models.Where(t => t.CREATEDDATE >= FromDate).ToList();
                if (models != null && ToDate.HasValue)
                    models = models.Where(t => t.CREATEDDATE <= ToDate).ToList();

                gridModels.DataModel = models;

                //oJournalHeadViewModel.JournalHeads = gridModels.DataModel;              
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

                if (Convert.ToString(TempData["result"]) != null)
                {
                    ViewBag.Message = Convert.ToString(TempData["result"]);
                }

                ViewBag.ParticularRef = ParticularRef;
                Session["currentPage"] = new Entities(Session["Connection"] as EntityConnection).INVESTMENTPARTICULARS.Where(t => t.REFERENCE == ParticularRef).SingleOrDefault().PARTICULARSNAME;

                return PartialView("ListParticularsDetails", gridModels);



            }

            catch (Exception ex)
            {
                string message = ex.Message;
                TempData["result"] = ex.Message;

                return RedirectToAction("ListParticularsList", "InvParticulars");
            }


        }


        [HttpGet]
        public ActionResult AddParticularsDetails(string ParticularRef)
        {

            try
            {
                if (Session["UserId"] == null || Session["PreviousPage"] == null || Session["currentPage"] == null || Session["Connection"] == null)
                {
                    return RedirectToAction("LogOut", "Home");

                }
                JournalHeadViewModel oJournalHeadViewModel = new JournalHeadViewModel();
                oJournalHeadViewModel.JournalHead = new JOURNALHEAD();
                ViewBag.Message = new CommonFunction().MessageForView(this.ControllerContext.RouteData.Values["action"].ToString());
                ViewBag.breadcum = oCommonFunction.GetAddPath(Session["Path"] as IHtmlString, Session["currentPage"].ToString());
                ViewBag.Header = "Add " + Session["currentPage"];

                ViewBag.ParticularList = new SelectList(new Entities(Session["Connection"] as EntityConnection).INVESTMENTPARTICULARS.Where(t => t.PARENTID != null).OrderBy(t => t.CODE).ToList(), "REFERENCE", "PARTICULARSNAME");

                ViewBag.ParticularRef = ParticularRef;

                return PartialView("AddParticularsDetails");
            }

            catch (Exception ex)
            {                
                TempData["result"] = ex.Message;
                return RedirectToAction("ListParticularsDetails", "InvParticulars", new { ParticularRef =ParticularRef });
                // return RedirectToAction("Index", "ErrorPage", new { message });

            }
        }


        [HttpPost]
        public ActionResult AddParticularsDetails(string ParticularRef, INVPARTICULARSDETAIL model)
        {

            try
            {
                if (Session["UserId"] == null || Session["PreviousPage"] == null || Session["currentPage"] == null || Session["Connection"] == null)
                {
                    return RedirectToAction("LogOut", "Home");

                }



                if (Session["UserId"] == null || Session["PreviousPage"] == null || Session["currentPage"] == null || Session["Connection"] == null)
                {
                    return RedirectToAction("LogOut", "Home");

                }

              
                    if (Request.IsAjaxRequest())
                    { 
                       

                        model.REFERENCE  = Guid.NewGuid().ToString();
                        model.CREATEDBY = Session["UserId"].ToString();
                        model.CREATEDDATE = DateTime.Today;

                        model.INVPARTICULARS_REF = ParticularRef;
                        oCommonFunction.CustomObjectNullValidation<INVPARTICULARSDETAIL>(ref model);
                       
                        using (Entities db = new Entities(Session["Connection"] as EntityConnection))
                        {
                            INVESTMENTPARTICULAR particular = new INVESTMENTPARTICULAR();
                            particular = db.INVESTMENTPARTICULARS.Where(t => t.REFERENCE == ParticularRef).SingleOrDefault();

                            model.GROUPNAME = particular.GROUPNAME;
                            model.SUBGROUPNAME = particular.SUBGROUPNAME;
                            model.INVESTMENTPARTICULAR = particular;
                            model.PARTICULARSNAME = particular.PARTICULARSNAME;
                          
                            

                            db.INVPARTICULARSDETAILS.Add(model);
                            db.SaveChanges();
                        }

                    }
                  
            
                ViewBag.ParticularRef = ParticularRef;
                TempData["result"] = "Save Successfull";     
                return RedirectToAction("ListParticularsDetails", "InvParticulars", new { ParticularRef = ParticularRef });    
            }

            catch (Exception ex)
            {
                TempData["result"] = ex.Message;
                return RedirectToAction("ListParticularsDetails", "InvParticulars", new { ParticularRef = ParticularRef });               

            }
        }


        [HttpGet]
        public ActionResult EditParticularsDetails(string ParticularRef,string Par_DetailsRef)
        {

            try
            {
                if (Session["UserId"] == null || Session["PreviousPage"] == null || Session["currentPage"] == null || Session["Connection"] == null)
                {
                    return RedirectToAction("LogOut", "Home");

                }


                ViewBag.Message = new CommonFunction().MessageForView(this.ControllerContext.RouteData.Values["action"].ToString());
                ViewBag.breadcum = oCommonFunction.GetAddPath(Session["Path"] as IHtmlString, Session["currentPage"].ToString());
                ViewBag.Header = "Edit " + Session["currentPage"];

                INVPARTICULARSDETAIL model = new INVPARTICULARSDETAIL();

                using (Entities db = new Entities(Session["Connection"] as EntityConnection))
                {
                    model = db.INVPARTICULARSDETAILS.Where(t => t.REFERENCE == Par_DetailsRef).SingleOrDefault();
                }

                ViewBag.ParticularRef = ParticularRef;

                return PartialView("EditParticularsDetails", model);
            }

            catch (Exception ex)
            {
                TempData["result"] = ex.Message;               

            }

            return RedirectToAction("ListParticularsDetails", "InvParticulars", new { ParticularRef = ParticularRef });
        }


        [HttpPost]
        public ActionResult EditParticularsDetails(string ParticularRef, INVPARTICULARSDETAIL model)
        {

            try
            {

                if (Session["UserId"] == null || Session["PreviousPage"] == null || Session["currentPage"] == null || Session["Connection"] == null)
                {
                    return RedirectToAction("LogOut", "Home");

                }


                if (Request.IsAjaxRequest())
                {

                    using (Entities db = new Entities(Session["Connection"] as EntityConnection))
                    {
                        INVPARTICULARSDETAIL particular = new INVPARTICULARSDETAIL();
                        particular = db.INVPARTICULARSDETAILS.Where(t => t.REFERENCE == model.REFERENCE).SingleOrDefault();

                        particular.TRANSACTIONDATE = model.TRANSACTIONDATE;
                        particular.PRINCIPALAMOUNT = model.PRINCIPALAMOUNT;

                        particular.LASTUPDATEDBY = Session["UserId"].ToString();
                        particular.LASTUPDATED = DateTime.Today;

                        db.Entry(particular).State = EntityState.Modified;

                        db.SaveChanges();
                    }

                }


                ViewBag.ParticularRef = ParticularRef;
                TempData["result"] = "Update Successfull";
            }

            catch (Exception ex)
            {
                TempData["result"] = ex.Message;

            }
            return RedirectToAction("ListParticularsDetails", "InvParticulars", new { ParticularRef = ParticularRef });
        }



    }

}

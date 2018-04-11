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
    public class FDRProposalController : Controller
    {
        CommonFunction oCommonFunction = new CommonFunction();
        //
        // GET: /FDRProposal/

        [HttpGet]
        public ActionResult ListFDRProposal(string sortdir, string sort, int? page, int?  rows, string filterstring, string currentFilter, string lblbreadcum, string PagingType, int? currentRowPerPage = 15)
        {
          try
           {
               //Session["UserId"] = null;
               //Session["Connection"] = null;
               if(oCommonFunction.CheckSession()==true)
               {
                return RedirectToAction("LogOut", "Home");
               }

                //currentRowPerPage=@ViewBag.currentRowPerPage
                GridModel<FDRPROPOSAL> gridModels = new GridModel<FDRPROPOSAL>();
                List<FDRPROPOSAL> models = null;

                //grid settings                
                gridModels.RowsPerPage = rows == null ? (int)currentRowPerPage : (int)rows;
                ViewBag.currentRowPerPage = gridModels.RowsPerPage;

                //Paging direction
                //switch (PagingType)
                //{
                //    case "Next":
                //        Session["pageNo"] = (int)Session["pageNo"] + 1;
                //        break;
                //    case "Prev":
                //        Session["pageNo"] = (int)Session["pageNo"] - 1;
                //        break;
                //    default:
                //        Session["pageNo"] = 1;
                //        ViewBag.Prev = "disabled";
                //        ViewBag.PrevNotActive = "not-active";
                //        break;
                //}


                //loading configuration
                sort = string.IsNullOrEmpty(sort) == true ? "CREATEDDATE" : sort;
                sortdir = string.IsNullOrEmpty(sortdir) == true ? "DESC" : sortdir;
                //int skipcount = gridModels.RowsPerPage * ((int)Session["pageNo"] - 1);
                if (filterstring == null)
                {
                    filterstring = currentFilter;
                }


                ViewBag.CurrentFilter = filterstring;


                if (string.IsNullOrEmpty(filterstring))
                    models = new Entities(Session["Connection"] as EntityConnection).FDRPROPOSALs.AsNoTracking().OrderBy(sort + " " + sortdir).ToList();
                else
                    //models = new Entities(Session["Connection"] as EntityConnection).FDRPROPOSALs.AsNoTracking().Where(w => w.NAME.Contains(filterstring)).OrderBy(sort + " " + sortdir).Skip(skipcount).Take(gridModels.RowsPerPage).ToList();
                    models = new Entities(Session["Connection"] as EntityConnection).FDRPROPOSALs.AsNoTracking().Where(w => w.NAME.Contains(filterstring)).OrderBy(sort + " " + sortdir).ToList();

                gridModels.DataModel = models;

                ViewBag.Message = oCommonFunction.MessageForView(this.ControllerContext.RouteData.Values["action"].ToString());
                ViewBag.Message = oCommonFunction.MessageForView(this.ControllerContext.RouteData.Values["action"].ToString());
                if (!string.IsNullOrEmpty(lblbreadcum))
                {
                    Session["currentPage"] = lblbreadcum;
                }

                Session["Path"] = oCommonFunction.GetPath(this.ControllerContext.RouteData.Values["controller"].ToString(), Session["currentPage"].ToString(), this.ControllerContext.RouteData.Values["action"].ToString());

                ViewBag.BreadCum = oCommonFunction.GetListPath(Session["currentPage"].ToString(), this.ControllerContext.RouteData.Values["controller"].ToString());
                //if (models.Count() < gridModels.RowsPerPage)
                //{

                //    ViewBag.Prev = "disabled";
                //    ViewBag.Next = "disabled";
                //    ViewBag.PrevNotActive = "not-active";
                //    ViewBag.NextNotActive = "not-active";
                //}
                
              
              return PartialView("ListFDRProposal", gridModels);

               
            }

            catch (Exception ex)
            {
                string message = ex.Message;

                return RedirectToAction("Index", "ErrorPage", new { message });
            }



        }

        [HttpGet]
        public ActionResult AddFDRProposal()
        {
            try
            {
                if (Session["UserId"] == null || Session["PreviousPage"] == null || Session["currentPage"] == null || Session["Connection"] == null)
                {
                    return RedirectToAction("LogOut", "Home");

                }

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
        public ActionResult AddFDRProposal(FDRPROPOSAL oFDRPROPOSAL)
        {
            //DEPARTMENT department = new Entities().DEPARTMENTs.Where(dep => dep.REFERENCE == oAPPLICATIONUSER.DEPARTMENT_REFERENCE).FirstOrDefault();
            //USERGROUP userGroup = new Entities().USERGROUPs.Where(ug => ug.REFERENCE == oAPPLICATIONUSER.USERGROUP_REFERENCE).FirstOrDefault();
            try
            {

                if (Session["UserId"] == null || Session["PreviousPage"] == null || Session["currentPage"] == null || Session["Connection"] == null)
                {
                    return RedirectToAction("LogOut", "Home");

                }

                
                if (Request.IsAjaxRequest())
                {

                    oFDRPROPOSAL.REFERENCE = Guid.NewGuid().ToString();
                    oFDRPROPOSAL.CREATEDBY = Session["UserId"].ToString();
                    oFDRPROPOSAL.CREATEDDATE = DateTime.Now;
                    oFDRPROPOSAL.LASTUPDATED = DateTime.Now;
                   // oFDRPROPOSAL.LASTUPDATEDBY = "BOSL";
                    //oFDRPROPOSAL.PROPOSALID = Guid.NewGuid().ToString();
                    oCommonFunction.CustomObjectNullValidation<FDRPROPOSAL>(ref oFDRPROPOSAL);

                    using (Entities db = new Entities(Session["Connection"] as EntityConnection))
                    {
                        db.FDRPROPOSALs.Add(oFDRPROPOSAL);
                        db.SaveChanges();
                    }

                }
                return RedirectToAction("ListFDRProposal", "FDRProposal");


            }
            catch (Exception ex)
            {

                string message = ex.Message;

                return RedirectToAction("Index", "ErrorPage", new { message });
            }

        }

        [HttpGet]
        public ActionResult EditFDRProposal(string id)
        {
            try
            {
                if (Session["UserId"] == null || Session["PreviousPage"] == null || Session["currentPage"] == null || Session["Connection"] == null)
                {
                    return RedirectToAction("LogOut", "Home");

                }

                FDRPROPOSAL oFDRPROPOSAL = new FDRPROPOSAL();
                Entities db = new Entities(Session["Connection"] as EntityConnection);
                ViewModelBase oViewModelBase = new ViewModelBase();


                oFDRPROPOSAL = db.FDRPROPOSALs.SingleOrDefault(i => i.REFERENCE == id);
                //ViewBag.departmentList = new SelectList(new Entities().DEPARTMENTs, "REFERENCE", "NAME");
                //ViewBag.userGroupList = new SelectList(new Entities().USERGROUPs, "REFERENCE", "NAME");
                ViewBag.Message = new CommonFunction().MessageForView(this.ControllerContext.RouteData.Values["action"].ToString());
                ViewBag.breadcum = oCommonFunction.GetEditPath(Session["Path"] as IHtmlString, Session["currentPage"].ToString());
                ViewBag.Header = "Update " + Session["currentPage"];

                return PartialView("EditFDRProposal", oFDRPROPOSAL);
            }

            catch (Exception ex)
            {
                string message = ex.Message;

                return RedirectToAction("Index", "ErrorPage", new { message });

            }
        }


        [HttpPost]
        public ActionResult EditFDRProposal(FDRPROPOSAL oFDRPROPOSAL)
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
                if (Session["UserId"] == null || Session["PreviousPage"] == null || Session["currentPage"] == null || Session["Connection"] == null)
                {
                    return RedirectToAction("LogOut", "Home");

                }
                oFDRPROPOSAL.LASTUPDATED = DateTime.Now;
                oFDRPROPOSAL.LASTUPDATEDBY = Session["UserId"].ToString();
                oCommonFunction.CustomObjectNullValidation<FDRPROPOSAL>(ref oFDRPROPOSAL);
                using (Entities db = new Entities(Session["Connection"] as EntityConnection))
                {
                    
                    db.Entry(oFDRPROPOSAL).State = EntityState.Modified;
                    db.SaveChanges();
                }

                return RedirectToAction("ListFDRPROPOSAL", "FDRPROPOSAL");

            }

            catch (Exception ex)
            {
                string message = ex.Message;

                return RedirectToAction("Index", "ErrorPage", new { message });
            }

        }

    }
}

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
    public class NominalAccountController : Controller
    {
        //
        // GET: /NominalAccount/
        CommonFunction oCommonFunction = new CommonFunction();
        [HttpGet]
        public ActionResult ListNominalAccount(string sortdir,DateTime? FromDate,DateTime? ToDate, string sort, int? page, int? rows, string filterstring, string currentFilter, string lblbreadcum, string PagingType, int? currentRowPerPage = 15)
        {
            try
            {
                //currentRowPerPage=@ViewBag.currentRowPerPage
                GridModel<NOMINALACCOUNT> gridModels = new GridModel<NOMINALACCOUNT>();
                List<NOMINALACCOUNT> models = null;

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
                    models = new Entities(Session["Connection"] as EntityConnection).NOMINALACCOUNTs.AsNoTracking().OrderBy(t=>t.CODE).Skip(skipcount).Take(gridModels.RowsPerPage).ToList();  //OrderByDescending(t=>t.CREATEDDATE)
                else
                    models = new Entities(Session["Connection"] as EntityConnection).NOMINALACCOUNTs.AsNoTracking().Where(w => w.CAPTION.Contains(filterstring)).OrderBy(t=>t.CODE).Skip(skipcount).Take(gridModels.RowsPerPage).ToList(); //OrderByDescending(t => t.CREATEDDATE)

                if (models !=null && FromDate.HasValue)
                    models = models.Where(t => t.CREATEDDATE.Value.Date >=FromDate.Value.Date).ToList();
               
                if (models !=null && ToDate.HasValue)
                    models = models.Where(t => t.CREATEDDATE.Value.Date <= ToDate.Value.Date).ToList();


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

                if (Convert.ToString(TempData["result"]) != null)
                {

                    ViewBag.Message = Convert.ToString(TempData["result"]);
                }
                return PartialView("ListNominalAccount", gridModels);



            }

            catch (Exception ex)
            {
                string message = ex.Message;

                return RedirectToAction("Index", "ErrorPage", new { message });
            }



        }


        [HttpGet]
        public ActionResult AddNominalAccount()
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
                string message = ex.Message;
                TempData["result"] = new HtmlString("<div style=\"color:red;display:inline\">" + ex.Message + "</div>");
                //return RedirectToAction("Index", "ErrorPage", new { message });
                return RedirectToAction("ListNominalAccount");

            }
        }

        [HttpPost]
        public ActionResult AddNominalAccount(NOMINALACCOUNT oNOMINALACCOUNT)
        {
            //DEPARTMENT department = new Entities().DEPARTMENTs.Where(dep => dep.REFERENCE == oAPPLICATIONUSER.DEPARTMENT_REFERENCE).FirstOrDefault();
            //USERGROUP userGroup = new Entities().USERGROUPs.Where(ug => ug.REFERENCE == oAPPLICATIONUSER.USERGROUP_REFERENCE).FirstOrDefault();
            try
            {



                if (Request.IsAjaxRequest())
                {

                   oNOMINALACCOUNT.REFERENCE = Guid.NewGuid().ToString();
                   //oNOMINALACCOUNT.CREATEDBY = "BOSL";
                   //oNOMINALACCOUNT.CREATEDDATE = DateTime.Now;

                   //oNOMINALACCOUNT.LASTUPDATED = DateTime.Parse("04-04-2015");
                   //oNOMINALACCOUNT.LASTUPDATEDBY = "BOSL";

                   oNOMINALACCOUNT.CREATEDBY = Session["UserId"].ToString();
                   oNOMINALACCOUNT.CREATEDDATE = DateTime.Today;
                  

                   oCommonFunction.CustomObjectNullValidation<NOMINALACCOUNT>(ref oNOMINALACCOUNT);
                    using (Entities db = new Entities(Session["Connection"] as EntityConnection))
                    {
                        db.NOMINALACCOUNTs.Add(oNOMINALACCOUNT);
                        db.SaveChanges();
                    }

                }
                return RedirectToAction("ListNominalAccount", "NominalAccount");


            }
            catch (Exception ex)
            {

                string message = ex.Message;
                TempData["result"] = new HtmlString("<div style=\"color:red;display:inline\">" + ex.Message + "</div>");       
                return RedirectToAction("ListNominalAccount");
               // return RedirectToAction("Index", "ErrorPage", new { message });
            }

        }

        [HttpGet]
        public ActionResult EditNominalAccount(string id)
        {
            try
            {
                NOMINALACCOUNT oNOMINALACCOUNT = new NOMINALACCOUNT();
                Entities db = new Entities(Session["Connection"] as EntityConnection);
                ViewModelBase oViewModelBase = new ViewModelBase();


                oNOMINALACCOUNT = db.NOMINALACCOUNTs.SingleOrDefault(i => i.REFERENCE == id);
                //ViewBag.departmentList = new SelectList(new Entities().DEPARTMENTs, "REFERENCE", "NAME");
                //ViewBag.userGroupList = new SelectList(new Entities().USERGROUPs, "REFERENCE", "NAME");
                ViewBag.Message = new CommonFunction().MessageForView(this.ControllerContext.RouteData.Values["action"].ToString());
                ViewBag.breadcum = oCommonFunction.GetEditPath(Session["Path"] as IHtmlString, Session["currentPage"].ToString());
                ViewBag.Header = "Update " + Session["currentPage"];

                return PartialView("EditNominalAccount", oNOMINALACCOUNT);
            }

            catch (Exception ex)
            {
                string message = ex.Message;
                TempData["result"] = new HtmlString("<div style=\"color:red;display:inline\">" + ex.Message + "</div>");               
                return RedirectToAction("ListNominalAccount");
               // return RedirectToAction("Index", "ErrorPage", new { message });

            }
        }

        [HttpPost]
        public ActionResult EditNominalAccount(NOMINALACCOUNT oNOMINALACCOUNT)
        {
            
            try
            {

                oNOMINALACCOUNT.LASTUPDATED = DateTime.Today;
                oNOMINALACCOUNT.LASTUPDATEDBY = Session["UserId"].ToString();
                oCommonFunction.CustomObjectNullValidation<NOMINALACCOUNT>(ref oNOMINALACCOUNT);
                using (Entities db = new Entities(Session["Connection"] as EntityConnection))
                {

                    db.Entry(oNOMINALACCOUNT).State = EntityState.Modified;
                    db.SaveChanges();
                }

                return RedirectToAction("ListNominalAccount", "NominalAccount");

            }

            catch (Exception ex)
            {
                string message = ex.Message;
                TempData["result"] = new HtmlString("<div style=\"color:red;display:inline\">" + ex.Message + "</div>");
             
                return RedirectToAction("ListNominalAccount");
              //  return RedirectToAction("Index", "ErrorPage", new { message });
            }

        }
    }
}

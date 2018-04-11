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
    public class ControlAccountController : Controller
    {
        //
        // GET: /ControlAccount/
        CommonFunction oCommonFunction = new CommonFunction();
        [HttpGet]
        public ActionResult ListControlAccount(string sortdir, string sort, int? page, int? rows, string filterstring, string currentFilter, string lblbreadcum, string PagingType, int? currentRowPerPage = 15)
        {
            try
            {
                //currentRowPerPage=@ViewBag.currentRowPerPage
                GridModel<CONTROLACCOUNT> gridModels = new GridModel<CONTROLACCOUNT>();
                List<CONTROLACCOUNT> models = null;

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
                    models = new Entities(Session["Connection"] as EntityConnection).CONTROLACCOUNTs.AsNoTracking().OrderBy(sort + " " + sortdir).Skip(skipcount).Take(gridModels.RowsPerPage).ToList();
                else
                    models = new Entities(Session["Connection"] as EntityConnection).CONTROLACCOUNTs.AsNoTracking().Where(w => w.STATUS.Contains(filterstring)).OrderBy(sort + " " + sortdir).Skip(skipcount).Take(gridModels.RowsPerPage).ToList();


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
                return PartialView("ListControlAccount", gridModels);



            }

            catch (Exception ex)
            {
                string message = ex.Message;

                return RedirectToAction("Index", "ErrorPage", new { message });
            }



        }

        [HttpGet]
        public ActionResult AddControlAccount()
        {
            try
            {
                ViewBag.Message = new CommonFunction().MessageForView(this.ControllerContext.RouteData.Values["action"].ToString());
                ViewBag.breadcum = oCommonFunction.GetAddPath(Session["Path"] as IHtmlString, Session["currentPage"].ToString());
                ViewBag.Header = "Add " + Session["currentPage"];
               
                ViewBag.nominalAccountList = new SelectList(new Entities(Session["Connection"] as EntityConnection).NOMINALACCOUNTs, "REFERENCE", "CODE");
               

                return PartialView();
            }

            catch (Exception ex)
            {
                string message = ex.Message;

                return RedirectToAction("Index", "ErrorPage", new { message });

            }
        }

        [HttpPost]
        public ActionResult AddControlAccount(CONTROLACCOUNT oCONTROLACCOUNT )
        {
            //DEPARTMENT department = new Entities().DEPARTMENTs.Where(dep => dep.REFERENCE == oAPPLICATIONUSER.DEPARTMENT_REFERENCE).FirstOrDefault();
            //USERGROUP userGroup = new Entities().USERGROUPs.Where(ug => ug.REFERENCE == oAPPLICATIONUSER.USERGROUP_REFERENCE).FirstOrDefault();
            try
            {



                if (Request.IsAjaxRequest())
                {

                    oCONTROLACCOUNT.REFERENCE = Guid.NewGuid().ToString();
                    oCONTROLACCOUNT.CREATEDBY = Session["UserId"].ToString();
                    oCONTROLACCOUNT.CREATEDDATE = DateTime.Now;
                    oCONTROLACCOUNT.LASTUPDATED = DateTime.Now;
                    oCommonFunction.CustomObjectNullValidation<CONTROLACCOUNT>(ref oCONTROLACCOUNT);
                    //oCONTROLACCOUNT.CREATEDBY = "BOSL";
                    //oCONTROLACCOUNT.CREATEDDATE = DateTime.Now;
                    //oCONTROLACCOUNT.LASTUPDATED = DateTime.Parse("04-04-2015");
                    //oCONTROLACCOUNT.LASTUPDATEDBY = "BOSL";
                    
                    
                    using (Entities db = new Entities(Session["Connection"] as EntityConnection))
                    {
                        db.CONTROLACCOUNTs.Add(oCONTROLACCOUNT);
                        db.SaveChanges();
                    }

                }
                return RedirectToAction("ListControlAccount");


            }
            catch (Exception ex)
            {

                string message = ex.Message;

                return RedirectToAction("Index", "ErrorPage", new { message });
            }

        }

        [HttpGet]
        public ActionResult EditControlAccount(string id)
        {
            try
            {
                CONTROLACCOUNT oCONTROLACCOUNT = new CONTROLACCOUNT();
                Entities db = new Entities(Session["Connection"] as EntityConnection);
                ViewModelBase oViewModelBase = new ViewModelBase();


                oCONTROLACCOUNT = db.CONTROLACCOUNTs.Include("NOMINALACCOUNT").SingleOrDefault(i => i.REFERENCE == id);
                ViewBag.nominalAccountList = new SelectList(new Entities(Session["Connection"] as EntityConnection).NOMINALACCOUNTs, "REFERENCE", "CODE");
                
                ViewBag.Message = new CommonFunction().MessageForView(this.ControllerContext.RouteData.Values["action"].ToString());
                ViewBag.breadcum = oCommonFunction.GetEditPath(Session["Path"] as IHtmlString, Session["currentPage"].ToString());
                ViewBag.Header = "Update " + Session["currentPage"];

                return PartialView("EditControlAccount", oCONTROLACCOUNT);
            }

            catch (Exception ex)
            {
                string message = ex.Message;

                return RedirectToAction("Index", "ErrorPage", new { message });

            }
        }

        [HttpPost]
        public ActionResult EditControlAccount(CONTROLACCOUNT oCONTROLACCOUNT)
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
                oCONTROLACCOUNT.LASTUPDATED = DateTime.Now;
                oCONTROLACCOUNT.LASTUPDATEDBY = Session["UserId"].ToString();
                oCommonFunction.CustomObjectNullValidation<CONTROLACCOUNT>(ref oCONTROLACCOUNT);

                using (Entities db = new Entities(Session["Connection"] as EntityConnection))
                {

                    db.Entry(oCONTROLACCOUNT).State = EntityState.Modified;
                    db.SaveChanges();
                }

                return RedirectToAction("ListControlAccount");

            }

            catch (Exception ex)
            {
                string message = ex.Message;

                return RedirectToAction("Index", "ErrorPage", new { message });
            }

        }
    }
}

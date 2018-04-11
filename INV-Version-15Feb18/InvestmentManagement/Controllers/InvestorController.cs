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
    public class InvestorController : Controller
    {
        //
        // GET: /Investor/
        CommonFunction oCommonFunction = new CommonFunction();


        [HttpGet]
        public ActionResult ListInvestor(string sortdir, string sort, int? page, int? rows, string filterstring, string currentFilter, string lblbreadcum, string PagingType, int? currentRowPerPage = 2)
        {
            try
            {
                if (Session["UserId"] == null || Session["PreviousPage"] == null || Session["currentPage"] == null || Session["Connection"] == null)
                {
                    return RedirectToAction("LogOut", "Home");
                }

                //currentRowPerPage=@ViewBag.currentRowPerPage
                GridModel<INVESTOR> gridModels = new GridModel<INVESTOR>();
                List<INVESTOR> models = null;

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
                    models = new Entities(Session["Connection"] as EntityConnection).INVESTORs.Include("BROKER").AsNoTracking().OrderBy(t => t.ACCOUNTNUMBER).Skip(skipcount).Take(gridModels.RowsPerPage).ToList();
                else
                    models = new Entities(Session["Connection"] as EntityConnection).INVESTORs.Include("BROKER").AsNoTracking().Where(w => w.NAME.Contains(filterstring)).OrderBy(t => t.ACCOUNTNUMBER).Skip(skipcount).Take(gridModels.RowsPerPage).ToList();


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
                return PartialView("ListInvestor", gridModels);
            }

            catch (Exception ex)
            {
                string message = ex.Message;
                return RedirectToAction("Index", "ErrorPage", new { message });
            }

        }
       

        public ActionResult AddInvestor(string lblbreadcum)
        {
            try
            {
                Entities db = new Entities(Session["Connection"] as EntityConnection);
                INVESTOR oINVESTOR = new INVESTOR();
              
                if (!string.IsNullOrEmpty(lblbreadcum))
                {
                    Session["currentPage"] = lblbreadcum;
                }
                ViewBag.brokerList = new SelectList(new Entities(Session["Connection"] as EntityConnection).BROKERs, "REFERENCE", "NAME");               
                ViewBag.Message = new CommonFunction().MessageForView(this.ControllerContext.RouteData.Values["action"].ToString());
                ViewBag.breadcum = oCommonFunction.GetAddPath(Session["Path"] as IHtmlString, Session["currentPage"].ToString());
                ViewBag.Header = "Add " + Session["currentPage"];
                return PartialView("AddInvestor", oINVESTOR);
            }

            catch (Exception ex)
            {
                string message = ex.Message;

                return RedirectToAction("Index", "ErrorPage", new { message });

            }
        }

        [HttpPost]
        public ActionResult AddInvestor(INVESTOR oINVESTOR)
        {
            //DEPARTMENT department = new Entities().DEPARTMENTs.Where(dep => dep.REFERENCE == oAPPLICATIONUSER.DEPARTMENT_REFERENCE).FirstOrDefault();
            //USERGROUP userGroup = new Entities().USERGROUPs.Where(ug => ug.REFERENCE == oAPPLICATIONUSER.USERGROUP_REFERENCE).FirstOrDefault();
            try
            {

                if (Request.IsAjaxRequest())
                {
                    oINVESTOR.REFERENCE = Guid.NewGuid().ToString();
                    oINVESTOR.CREATEDBY = Session["UserId"].ToString();
                    oINVESTOR.CREATEDDATE = DateTime.Now;
                    oINVESTOR.LASTUPDATED = DateTime.Now;
                    oCommonFunction.CustomObjectNullValidation<INVESTOR>(ref oINVESTOR);
                   
                    //oUSERGROUP.LASTLOGIN = DateTime.Now;
                    using (Entities db = new Entities(Session["Connection"] as EntityConnection))
                    {
                        db.INVESTORs.Add(oINVESTOR);
                        db.SaveChanges();
                    }

                }
                return RedirectToAction("ListInvestor", "Investor");
            }
            catch (Exception ex)
            {

                string message = ex.Message;

                return RedirectToAction("Index", "ErrorPage", new { message });
            }

        }

        [HttpGet]
        public ActionResult EditInvestor(string lblbreadcum,string id)
        {
            try
            {
                Entities db = new Entities(Session["Connection"] as EntityConnection);
                INVESTOR oINVESTOR = new INVESTOR();
                oINVESTOR = db.INVESTORs.Where(t=>t.REFERENCE == id).SingleOrDefault();

                if (!string.IsNullOrEmpty(lblbreadcum))
                {
                    Session["currentPage"] = lblbreadcum;
                }
                
                ViewBag.brokerList = new SelectList(new Entities(Session["Connection"] as EntityConnection).BROKERs, "REFERENCE", "NAME");               
                ViewBag.breadcum = oCommonFunction.GetEditPath(Session["Path"] as IHtmlString, Session["currentPage"].ToString());
                ViewBag.Header = "Update " + Session["currentPage"];
                return PartialView("EditInvestor", oINVESTOR);
            }

            catch (Exception ex)
            {
                string message = ex.Message;
                return RedirectToAction("Index", "ErrorPage", new { message });

            }
        }


        [HttpPost]
        public ActionResult EditInvestor(INVESTOR oINVESTOR)
        {
           
            try
            {
                oINVESTOR.LASTUPDATED = DateTime.Now;
                oINVESTOR.LASTUPDATEDBY = Session["UserId"].ToString();               
                oCommonFunction.CustomObjectNullValidation<INVESTOR>(ref oINVESTOR);
               
                using (Entities db = new Entities(Session["Connection"] as EntityConnection))
                {
                    db.Entry(oINVESTOR).State = EntityState.Modified;
                    db.SaveChanges();
                }       
                return RedirectToAction("ListInvestor", "Investor");

            }

            catch (Exception ex)
            {
                string message = ex.Message;

                return RedirectToAction("Index", "ErrorPage", new { message });
            }

        }

    }
}

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
    public class InstrumentCategoryController : Controller
    {
        CommonFunction oCommonFunction = new CommonFunction();
        //
        // GET: /InstrumentCategory/

        [HttpGet]
        public ActionResult ListInstrumentCategory(string sortdir, string sort, int? page, int? rows, string filterstring, string currentFilter, string lblbreadcum, string PagingType, int? currentRowPerPage = 15)
        {
            try
            {
                //currentRowPerPage=@ViewBag.currentRowPerPage
                GridModel<INSTRUMENTCATEGORY> gridModels = new GridModel<INSTRUMENTCATEGORY>();
                List<INSTRUMENTCATEGORY> models = null;

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
                    models = new Entities(Session["Connection"] as EntityConnection).INSTRUMENTCATEGORies.AsNoTracking().OrderBy(sort + " " + sortdir).Skip(skipcount).Take(gridModels.RowsPerPage).ToList();
                else
                    models = new Entities(Session["Connection"] as EntityConnection).INSTRUMENTCATEGORies.AsNoTracking().Where(w => w.CODE.Contains(filterstring)).OrderBy(sort + " " + sortdir).Skip(skipcount).Take(gridModels.RowsPerPage).ToList();


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
                return PartialView("ListInstrumentCategory", gridModels);



            }

            catch (Exception ex)
            {
                string message = ex.Message;

                return RedirectToAction("Index", "ErrorPage", new { message });
            }



        }

        [HttpGet]
        public ActionResult AddInstrumentCategory()
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

                return RedirectToAction("Index", "ErrorPage", new { message });

            }
        }

        [HttpPost]
        public ActionResult AddInstrumentCategory(INSTRUMENTCATEGORY oINSTRUMENTCATEGORY)
        {
            //DEPARTMENT department = new Entities().DEPARTMENTs.Where(dep => dep.REFERENCE == oAPPLICATIONUSER.DEPARTMENT_REFERENCE).FirstOrDefault();
            //USERGROUP userGroup = new Entities().USERGROUPs.Where(ug => ug.REFERENCE == oAPPLICATIONUSER.USERGROUP_REFERENCE).FirstOrDefault();
            try
            {



                if (Request.IsAjaxRequest())
                {

                    oINSTRUMENTCATEGORY.REFERENCE = Guid.NewGuid().ToString();

                    oINSTRUMENTCATEGORY.CREATEDBY = Session["UserId"].ToString();
                    oINSTRUMENTCATEGORY.CREATEDDATE = DateTime.Now;
                    oINSTRUMENTCATEGORY.LASTUPDATED = DateTime.Now;

                    oINSTRUMENTCATEGORY.ISNONMARGINABLE = oINSTRUMENTCATEGORY.ISNONMARGINABLE == "true" ? "Y" : "N";
                    oINSTRUMENTCATEGORY.ALLOWNETTING = oINSTRUMENTCATEGORY.ALLOWNETTING == "true" ? "Y" : "N";

                    oCommonFunction.CustomObjectNullValidation<INSTRUMENTCATEGORY>(ref oINSTRUMENTCATEGORY);
                    //oINSTRUMENTCATEGORY.CREATEDBY = "BOSL";
                    //oINSTRUMENTCATEGORY.CREATEDDATE = DateTime.Now;
                    //oINSTRUMENTCATEGORY.LASTUPDATED = DateTime.Now;
                    //oINSTRUMENTCATEGORY.LASTUPDATEDBY = "BOSL";
                    //oINSTRUMENTCATEGORY.ALLOWNETTING = oINSTRUMENTCATEGORY.ALLOWNETTING == "true" ? "Y" : "N";
                    //oINSTRUMENTCATEGORY.ISNONMARGINABLE = oINSTRUMENTCATEGORY.ISNONMARGINABLE == "true" ? "Y" : "N";
                    //oINSTRUMENTCATEGORY.CODE = oINSTRUMENTCATEGORY.CODE == null ? "" : oINSTRUMENTCATEGORY.CODE;
                    //oINSTRUMENTCATEGORY.DESCRIPTION = oINSTRUMENTCATEGORY.DESCRIPTION == null ? "" : oINSTRUMENTCATEGORY.DESCRIPTION;
                    //oINSTRUMENTCATEGORY.STOCKEXCHANGE = oINSTRUMENTCATEGORY.STOCKEXCHANGE == null ? "" : oINSTRUMENTCATEGORY.STOCKEXCHANGE;
                    //oINSTRUMENTCATEGORY.SETTLEMENTDAYS = oINSTRUMENTCATEGORY.SETTLEMENTDAYS == null ? (long)0 : oINSTRUMENTCATEGORY.SETTLEMENTDAYS;
                    //oINSTRUMENTCATEGORY.STATUS = oINSTRUMENTCATEGORY.STATUS == null ? "" : oINSTRUMENTCATEGORY.STATUS;

                    using (Entities db = new Entities(Session["Connection"] as EntityConnection))
                    {
                       
                        db.INSTRUMENTCATEGORies.Add(oINSTRUMENTCATEGORY);
                        db.SaveChanges();
                    }

                }
                return RedirectToAction("ListInstrumentCategory", "InstrumentCategory");


            }
            catch (Exception ex)
            {

                string message = ex.Message;

                return RedirectToAction("Index", "ErrorPage", new { message });
            }

        }

        [HttpGet]
        public ActionResult EditInstrumentCategory(string id)
        {
            try
            {
                INSTRUMENTCATEGORY oINSTRUMENTCATEGORY = new INSTRUMENTCATEGORY();
                Entities db = new Entities(Session["Connection"] as EntityConnection);
                ViewModelBase oViewModelBase = new ViewModelBase();


                oINSTRUMENTCATEGORY = db.INSTRUMENTCATEGORies.SingleOrDefault(i => i.REFERENCE == id);
                //ViewBag.departmentList = new SelectList(new Entities().DEPARTMENTs, "REFERENCE", "NAME");
                //ViewBag.userGroupList = new SelectList(new Entities().USERGROUPs, "REFERENCE", "NAME");
                ViewBag.Message = new CommonFunction().MessageForView(this.ControllerContext.RouteData.Values["action"].ToString());
                ViewBag.breadcum = oCommonFunction.GetEditPath(Session["Path"] as IHtmlString, Session["currentPage"].ToString());
                ViewBag.Header = "Update " + Session["currentPage"];



                return PartialView("EditInstrumentCategory", oINSTRUMENTCATEGORY);
            }

            catch (Exception ex)
            {
                string message = ex.Message;

                return RedirectToAction("Index", "ErrorPage", new { message });

            }
        }



        [HttpPost]
        public ActionResult EditInstrumentCategory(INSTRUMENTCATEGORY oINSTRUMENTCATEGORY)
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

                using (Entities db = new Entities(Session["Connection"] as EntityConnection))
                {
                    oINSTRUMENTCATEGORY.LASTUPDATED = DateTime.Now;
                    oINSTRUMENTCATEGORY.LASTUPDATEDBY = Session["UserId"].ToString();
                    oCommonFunction.CustomObjectNullValidation<INSTRUMENTCATEGORY>(ref oINSTRUMENTCATEGORY);
                    oINSTRUMENTCATEGORY.ISNONMARGINABLE = oINSTRUMENTCATEGORY.ISNONMARGINABLE == "true" ? "Y" : "N";
                    oINSTRUMENTCATEGORY.ALLOWNETTING = oINSTRUMENTCATEGORY.ALLOWNETTING == "true" ? "Y" : "N";
                    db.Entry(oINSTRUMENTCATEGORY).State = EntityState.Modified;
                    db.SaveChanges();
                }

                return RedirectToAction("ListInstrumentCategory", "InstrumentCategory");

            }

            catch (Exception ex)
            {
                string message = ex.Message;

                return RedirectToAction("Index", "ErrorPage", new { message });
            }

        }
    }
}

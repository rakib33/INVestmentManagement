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
using System.Globalization;


namespace InvestmentManagement.Controllers
{
    public class PeriodController : Controller
    {
       
        CommonFunction oCommonFunction = new CommonFunction();
        //
        // GET: /Period/
        [HttpGet]
        public ActionResult ListPeriod(string sortdir, string sort, int? page, int? rows, string filterstring, string currentFilter, string lblbreadcum, string PagingType, int? currentRowPerPage = 15)
        {
            try
            {
                //currentRowPerPage=@ViewBag.currentRowPerPage
                GridModel<PERIOD> gridModels = new GridModel<PERIOD>();
                List<PERIOD> models = null;

               
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
                    models = new Entities(Session["Connection"] as EntityConnection).PERIODs.Include("FINANCIALYEAR").AsNoTracking().OrderBy(sort + " " + sortdir).Skip(skipcount).Take(gridModels.RowsPerPage).ToList();
                else
                    models = new Entities(Session["Connection"] as EntityConnection).PERIODs.Include("FINANCIALYEAR").AsNoTracking().Where(w => w.STATUS.Contains(filterstring)).OrderBy(sort + " " + sortdir).Skip(skipcount).Take(gridModels.RowsPerPage).ToList();


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
                return PartialView("ListPeriod", gridModels);



            }

            catch (Exception ex)
            {
                string message = ex.Message;

                return RedirectToAction("Index", "ErrorPage", new { message });
            }



        }

        [HttpGet]
        public ActionResult AddPeriod()
        {
            try
            {
                ViewBag.Message = new CommonFunction().MessageForView(this.ControllerContext.RouteData.Values["action"].ToString());
                ViewBag.breadcum = oCommonFunction.GetAddPath(Session["Path"] as IHtmlString, Session["currentPage"].ToString());
                ViewBag.Header = "Add " + Session["currentPage"];

                ViewBag.financialYearList = new SelectList(new Entities(Session["Connection"] as EntityConnection).FINANCIALYEARs, "REFERENCE", "DESCRIPTION");

               
                return PartialView();
            }

            catch (Exception ex)
            {
                string message = ex.Message;

                return RedirectToAction("Index", "ErrorPage", new { message });

            }
        }


        [HttpPost]
        public ActionResult AddPeriod(PERIOD oPERIOD, string DATETO, string DATEFROM)
        {
            //DEPARTMENT department = new Entities().DEPARTMENTs.Where(dep => dep.REFERENCE == oAPPLICATIONUSER.DEPARTMENT_REFERENCE).FirstOrDefault();
            //USERGROUP userGroup = new Entities().USERGROUPs.Where(ug => ug.REFERENCE == oAPPLICATIONUSER.USERGROUP_REFERENCE).FirstOrDefault();
            try
            {



                if (Request.IsAjaxRequest())
                {

                    oPERIOD.REFERENCE = Guid.NewGuid().ToString();
                    oPERIOD.CREATEDBY = Session["UserId"].ToString();
                    oPERIOD.CREATEDDATE = DateTime.Now;
                    oPERIOD.LASTUPDATED = DateTime.Now;

                    oCommonFunction.CustomObjectNullValidation<PERIOD>(ref oPERIOD);


                    int count;
                    using (Entities db = new Entities(Session["Connection"] as EntityConnection))
                    {
                        count = db.PERIODs.ToList().Count;

                        oPERIOD.PERIODINDEX = count > 0 ? ++count : 0;


                    }

                    //DATEFROM = DateTime.ParseExact(DATEFROM, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None).ToString("yyyy/MM/dd");
                    //oPERIOD.DATEFROM = oPERIOD.DATEFROM == null ? Convert.ToDateTime(DATEFROM) : oPERIOD.DATEFROM;

                    //DATETO = DateTime.ParseExact(DATETO, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None).ToString("yyyy/MM/dd");
                    //oPERIOD.DATETO = oPERIOD.DATETO == null ? Convert.ToDateTime(DATETO) : oPERIOD.DATETO;
                    using (Entities db = new Entities(Session["Connection"] as EntityConnection))
                    {
                        db.PERIODs.Add(oPERIOD);
                        db.SaveChanges();
                    }

                }
                return RedirectToAction("ListPeriod");


            }
            catch (Exception ex)
            {

                string message = ex.Message;

                return RedirectToAction("Index", "ErrorPage", new { message });
            }

        }


        [HttpGet]
        public ActionResult EditPeriod(string id)
        {
            try
            {
                PERIOD oPERIOD = new PERIOD();
                Entities db = new Entities(Session["Connection"] as EntityConnection);
                ViewModelBase oViewModelBase = new ViewModelBase();


                oPERIOD = db.PERIODs.Include("FINANCIALYEAR").SingleOrDefault(i => i.REFERENCE == id);
                ViewBag.financialYearList = new SelectList(new Entities(Session["Connection"] as EntityConnection).FINANCIALYEARs, "REFERENCE", "DESCRIPTION");

                ViewBag.Message = new CommonFunction().MessageForView(this.ControllerContext.RouteData.Values["action"].ToString());
                ViewBag.breadcum = oCommonFunction.GetEditPath(Session["Path"] as IHtmlString, Session["currentPage"].ToString());
                ViewBag.Header = "Update " + Session["currentPage"];

                return PartialView("EditPeriod", oPERIOD);
            }

            catch (Exception ex)
            {
                string message = ex.Message;

                return RedirectToAction("Index", "ErrorPage", new { message });

            }
        }

        [HttpPost]
        public ActionResult EditPeriod(PERIOD oPERIOD, string DATETO, string DATEFROM)
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
                //DATEFROM = DateTime.ParseExact(DATEFROM, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None).ToString("yyyy/MM/dd");
                //oPERIOD.DATEFROM = oPERIOD.DATEFROM == null ? Convert.ToDateTime(DATEFROM) : oPERIOD.DATEFROM;

                //DATETO = DateTime.ParseExact(DATETO, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None).ToString("yyyy/MM/dd");

                //oPERIOD.DATETO = oPERIOD.DATETO == null ? Convert.ToDateTime(DATETO) : oPERIOD.DATETO;

                oPERIOD.LASTUPDATED = DateTime.Now;
                oPERIOD.LASTUPDATEDBY = Session["UserId"].ToString();
                oCommonFunction.CustomObjectNullValidation<PERIOD>(ref oPERIOD);
                using (Entities db = new Entities(Session["Connection"] as EntityConnection))
                {

                    db.Entry(oPERIOD).State = EntityState.Modified;
                    db.SaveChanges();
                }

                return RedirectToAction("ListPeriod", "Period");

            }

            catch (Exception ex)
            {
                string message = ex.Message;

                return RedirectToAction("Index", "ErrorPage", new { message });
            }

        }
    }
}

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
    public class DepositorySettingsController : Controller
    {
        //
        // GET: /DepositorySettings/

        CommonFunction oCommonFunction = new CommonFunction();

        [HttpGet]
        public ActionResult ListDepositorySettings(string sortdir, string sort, int? page, int? rows, string filterstring, string currentFilter, string lblbreadcum, string PagingType, int? currentRowPerPage = 15)
        {
            try
            {
                //currentRowPerPage=@ViewBag.currentRowPerPage
                GridModel<DEPOSITORYSETTING> gridModels = new GridModel<DEPOSITORYSETTING>();
                List<DEPOSITORYSETTING> models = null;
                
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
                if (!string.IsNullOrEmpty(lblbreadcum))
                {
                    Session["currentPage"] = lblbreadcum;
                }

                ViewBag.CurrentFilter = filterstring;


                if (string.IsNullOrEmpty(filterstring))
                    models = new Entities(Session["Connection"] as EntityConnection).DEPOSITORYSETTINGS.AsNoTracking().OrderBy(t=>t.SEQ).Skip(skipcount).Take(gridModels.RowsPerPage).ToList();//OrderBy(sort + " " + sortdir)
                else
                    models = new Entities(Session["Connection"] as EntityConnection).DEPOSITORYSETTINGS.AsNoTracking().Where(w => w.FILENAME.Contains(filterstring)).OrderBy(t=>t.SEQ).Skip(skipcount).Take(gridModels.RowsPerPage).ToList();


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
                return PartialView("ListDepositorySettings", gridModels);



            }

            catch (Exception ex)
            {
                string message = ex.Message;

                return RedirectToAction("Index", "ErrorPage", new { message });
            }



        }

        [HttpGet]
        public ActionResult AddDepositorySettings()
        {
            try
            {
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
        public ActionResult AddDepositorySettings(DEPOSITORYSETTING oDEPOSITORYSETTING)
        {
            //DEPARTMENT department = new Entities().DEPARTMENTs.Where(dep => dep.REFERENCE == oAPPLICATIONUSER.DEPARTMENT_REFERENCE).FirstOrDefault();
            //USERGROUP userGroup = new Entities().USERGROUPs.Where(ug => ug.REFERENCE == oAPPLICATIONUSER.USERGROUP_REFERENCE).FirstOrDefault();
            try
            {



                if (Request.IsAjaxRequest())
                {

                    oDEPOSITORYSETTING.REFERENCE = Guid.NewGuid().ToString();
                    oDEPOSITORYSETTING.CREATEDBY = Session["UserId"].ToString();
                    oDEPOSITORYSETTING.CREATEDDATE = DateTime.Now;
                    oDEPOSITORYSETTING.LASTUPDATED = DateTime.Now;

               

                    oCommonFunction.CustomObjectNullValidation<DEPOSITORYSETTING>(ref oDEPOSITORYSETTING);
                    //oDEPOSITORYSETTING.CREATEDDATE = DateTime.Today;
                   
                    //oDEPOSITORYSETTING.LASTUPDATED = DateTime.Today;
                    //oDEPOSITORYSETTING.LASTUPDATEDBY = "BOSL";
                    //oDEPOSITORYSETTING.CODE = oDEPOSITORYSETTING.CODE == null ? string.Empty : oDEPOSITORYSETTING.CODE;
                    //oDEPOSITORYSETTING.DESCRIPTION = oDEPOSITORYSETTING.DESCRIPTION == null ? string.Empty : oDEPOSITORYSETTING.DESCRIPTION;
                    //oDEPOSITORYSETTING.FILENAME = oDEPOSITORYSETTING.FILENAME == null ? string.Empty : oDEPOSITORYSETTING.FILENAME;
                    //oDEPOSITORYSETTING.CHARGERATE = oDEPOSITORYSETTING.CHARGERATE == null ? (decimal)00.00 : oDEPOSITORYSETTING.CHARGERATE;
                    //oDEPOSITORYSETTING.MINIMUMFEE = oDEPOSITORYSETTING.MINIMUMFEE == null ? (decimal)00.00 : oDEPOSITORYSETTING.MINIMUMFEE;
                    //oDEPOSITORYSETTING.METHODNAME = oDEPOSITORYSETTING.METHODNAME == null ? string.Empty : oDEPOSITORYSETTING.METHODNAME;

                    using (Entities db = new Entities(Session["Connection"] as EntityConnection))
                    {
                        db.DEPOSITORYSETTINGS.Add(oDEPOSITORYSETTING);
                        db.SaveChanges();
                    }

                }
                return RedirectToAction("ListDepositorySettings", "DepositorySettings");


            }
            catch (Exception ex)
            {

                string message = ex.Message;

                return RedirectToAction("Index", "ErrorPage", new { message });
            }

        }



        //Edit GET
        [HttpGet]
        public ActionResult EditDepositorySettings(string id)
        {
            try
            {
                DEPOSITORYSETTING oDEPOSITORYSETTING = new DEPOSITORYSETTING();
                Entities db = new Entities(Session["Connection"] as EntityConnection);
                ViewModelBase oViewModelBase = new ViewModelBase();



                oDEPOSITORYSETTING = db.DEPOSITORYSETTINGS.SingleOrDefault(i => i.REFERENCE == id);
                ViewBag.Message = new CommonFunction().MessageForView(this.ControllerContext.RouteData.Values["action"].ToString());
                ViewBag.breadcum = oCommonFunction.GetEditPath(Session["Path"] as IHtmlString, Session["currentPage"].ToString());
                ViewBag.Header = "Update " + Session["currentPage"];

                return PartialView("EditDepositorySettings", oDEPOSITORYSETTING);
            }

            catch (Exception ex)
            {
                string message = ex.Message;

                return RedirectToAction("Index", "ErrorPage", new { message });

            }
        }


        [HttpPost]
        public ActionResult EditDepositorySettings(DEPOSITORYSETTING oDEPOSITORYSETTING)
        {
            

            try
            {
                oDEPOSITORYSETTING.LASTUPDATED = DateTime.Now;
                oDEPOSITORYSETTING.LASTUPDATEDBY = Session["UserId"].ToString();
                oCommonFunction.CustomObjectNullValidation<DEPOSITORYSETTING>(ref oDEPOSITORYSETTING);
                //oDEPOSITORYSETTING.LASTUPDATED = DateTime.Today;
                //oDEPOSITORYSETTING.CODE = oDEPOSITORYSETTING.CODE == null ? string.Empty : oDEPOSITORYSETTING.CODE;
                //oDEPOSITORYSETTING.DESCRIPTION = oDEPOSITORYSETTING.DESCRIPTION == null ? string.Empty : oDEPOSITORYSETTING.DESCRIPTION;
                //oDEPOSITORYSETTING.FILENAME = oDEPOSITORYSETTING.FILENAME == null ? string.Empty : oDEPOSITORYSETTING.FILENAME;
                //oDEPOSITORYSETTING.CHARGERATE = oDEPOSITORYSETTING.CHARGERATE == null ? (decimal)00.00 : oDEPOSITORYSETTING.CHARGERATE;
                //oDEPOSITORYSETTING.MINIMUMFEE = oDEPOSITORYSETTING.MINIMUMFEE == null ? (decimal)00.00 : oDEPOSITORYSETTING.MINIMUMFEE;
                //oDEPOSITORYSETTING.METHODNAME = oDEPOSITORYSETTING.METHODNAME == null ? string.Empty : oDEPOSITORYSETTING.METHODNAME;
                using (Entities db = new Entities(Session["Connection"] as EntityConnection))
                {

                    db.Entry(oDEPOSITORYSETTING).State = EntityState.Modified;
                    db.SaveChanges();
                }

                return RedirectToAction("ListDepositorySettings", "DepositorySettings");

            }

            catch (Exception ex)
            {
                string message = ex.Message;

                return RedirectToAction("Index", "ErrorPage", new { message });
            }

        }


    }
}

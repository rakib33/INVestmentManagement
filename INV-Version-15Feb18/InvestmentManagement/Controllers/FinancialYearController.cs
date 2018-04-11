﻿using System;
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
    public class FinancialYearController : Controller
    {
        CommonFunction oCommonFunction = new CommonFunction();
        //
        // GET: /FinancialYear/

        [HttpGet]
        public ActionResult ListFinancialYear(string sortdir, string sort, int? page, int? rows, string filterstring, string currentFilter, string lblbreadcum, string PagingType, int? currentRowPerPage = 15)
        {
            try
            {
                //currentRowPerPage=@ViewBag.currentRowPerPage
                GridModel<FINANCIALYEAR> gridModels = new GridModel<FINANCIALYEAR>();
                List<FINANCIALYEAR> models = null;

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
                    models = new Entities(Session["Connection"] as EntityConnection).FINANCIALYEARs.AsNoTracking().OrderBy(sort + " " + sortdir).Skip(skipcount).Take(gridModels.RowsPerPage).ToList();
                else
                    models = new Entities(Session["Connection"] as EntityConnection).FINANCIALYEARs.AsNoTracking().Where(w => w.STATUS.Contains(filterstring)).OrderBy(sort + " " + sortdir).Skip(skipcount).Take(gridModels.RowsPerPage).ToList();


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
                return PartialView("ListFinancialYear", gridModels);



            }

            catch (Exception ex)
            {
                string message = ex.Message;

                return RedirectToAction("Index", "ErrorPage", new { message });
            }



        }

        [HttpGet]
        public ActionResult AddFinancialYear()
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
        public ActionResult AddFinancialYear(FINANCIALYEAR oFINANCIALYEAR, string DATETO, string DATEFROM)
        {
            //DEPARTMENT department = new Entities().DEPARTMENTs.Where(dep => dep.REFERENCE == oAPPLICATIONUSER.DEPARTMENT_REFERENCE).FirstOrDefault();
            //USERGROUP userGroup = new Entities().USERGROUPs.Where(ug => ug.REFERENCE == oAPPLICATIONUSER.USERGROUP_REFERENCE).FirstOrDefault();
            try
            {



                if (Request.IsAjaxRequest())
                {

                    oFINANCIALYEAR.REFERENCE = Guid.NewGuid().ToString();
                    oFINANCIALYEAR.CREATEDBY = Session["UserId"].ToString();
                    oFINANCIALYEAR.CREATEDDATE = DateTime.Now;
                    oFINANCIALYEAR.LASTUPDATED = DateTime.Now;
                   
                    oCommonFunction.CustomObjectNullValidation<FINANCIALYEAR>(ref oFINANCIALYEAR);

                    int count;
                    using (Entities db = new Entities(Session["Connection"] as EntityConnection))
                    {
                        count = db.FINANCIALYEARs.ToList().Count;

                        oFINANCIALYEAR.YEARINDEX = count > 0 ? ++count : 0;   
                       
                        
                    }
                    // DATEFROM = DateTime.ParseExact(DATEFROM, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None).ToString("yyyy/MM/dd");
                    //oFINANCIALYEAR.DATEFROM = oFINANCIALYEAR.DATEFROM == null ? Convert.ToDateTime(DATEFROM) :oFINANCIALYEAR.DATEFROM;
                    
                    //DATETO = DateTime.ParseExact(DATETO, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None).ToString("yyyy/MM/dd");

                    //oFINANCIALYEAR.DATETO = oFINANCIALYEAR.DATETO == null ? Convert.ToDateTime(DATETO) : oFINANCIALYEAR.DATETO;
                    using (Entities db = new Entities(Session["Connection"] as EntityConnection))
                    {
                        db.FINANCIALYEARs.Add(oFINANCIALYEAR);
                        db.SaveChanges();
                    }

                }
                return RedirectToAction("ListFinancialYear", "FinancialYear");


            }
            catch (Exception ex)
            {

                string message = ex.Message;

                return RedirectToAction("Index", "ErrorPage", new { message });
            }

        }

        [HttpGet]
        public ActionResult EditFinancialYear(string id)
        {
            try
            {
                FINANCIALYEAR oFINANCIALYEAR = new FINANCIALYEAR();
                Entities db = new Entities(Session["Connection"] as EntityConnection);
                ViewModelBase oViewModelBase = new ViewModelBase();


                oFINANCIALYEAR = db.FINANCIALYEARs.SingleOrDefault(i => i.REFERENCE == id);
                //ViewBag.departmentList = new SelectList(new Entities().DEPARTMENTs, "REFERENCE", "NAME");
                //ViewBag.userGroupList = new SelectList(new Entities().USERGROUPs, "REFERENCE", "NAME");
                ViewBag.Message = new CommonFunction().MessageForView(this.ControllerContext.RouteData.Values["action"].ToString());
                ViewBag.breadcum = oCommonFunction.GetEditPath(Session["Path"] as IHtmlString, Session["currentPage"].ToString());
                ViewBag.Header = "Update " + Session["currentPage"];


                return PartialView("EditFinancialYear", oFINANCIALYEAR);
            }

            catch (Exception ex)
            {
                string message = ex.Message;

                return RedirectToAction("Index", "ErrorPage", new { message });

            }
        }


        [HttpPost]
        public ActionResult EditFinancialYear(FINANCIALYEAR oFINANCIALYEAR, string DATETO, string DATEFROM)
        {
            //oAPPLICATIONUSER.CREATEDBY = "BOSL";
            //oAPPLICATIONUSER.CREATEDDATE = DateTime.Parse("05-04-2015");
            //oAPPLICATIONUSER.DEPARTMENT = department;
            //oAPPLICATIONUSER.LASTUPDATED = DateTime.Parse("04-04-2015");
            //oAPPLICATIONUSER.LASTUPDATEDBY = "BOSL";
            //oAPPLICATIONUSER.USERID = "1254";
            //oAPPLICATIONUSER.LASTLOGIN = DateTime.Now;
            oFINANCIALYEAR.LASTUPDATED = DateTime.Now;
            oFINANCIALYEAR.LASTUPDATEDBY = Session["UserId"].ToString();
            oCommonFunction.CustomObjectNullValidation<FINANCIALYEAR>(ref oFINANCIALYEAR);
            try
            {
                //DATEFROM = DateTime.ParseExact(DATEFROM, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None).ToString("yyyy/MM/dd");
                //oFINANCIALYEAR.DATEFROM = oFINANCIALYEAR.DATEFROM == null ? Convert.ToDateTime(DATEFROM) : oFINANCIALYEAR.DATEFROM;

                //DATETO = DateTime.ParseExact(DATETO, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None).ToString("yyyy/MM/dd");

                //oFINANCIALYEAR.DATETO = oFINANCIALYEAR.DATETO == null ? Convert.ToDateTime(DATETO) : oFINANCIALYEAR.DATETO;
                using (Entities db = new Entities(Session["Connection"] as EntityConnection))
                {

                    db.Entry(oFINANCIALYEAR).State = EntityState.Modified;
                    db.SaveChanges();
                }

                return RedirectToAction("ListFinancialYear", "FinancialYear");

            }

            catch (Exception ex)
            {
                string message = ex.Message;

                return RedirectToAction("Index", "ErrorPage", new { message });
            }

        }
    }
}
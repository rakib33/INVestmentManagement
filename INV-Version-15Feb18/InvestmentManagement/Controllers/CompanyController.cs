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
    public class CompanyController : Controller
    {
        //
        // GET: /Company/

        CommonFunction oCommonFunction = new CommonFunction();

        [HttpGet]
        public ActionResult AddCompany(string lblbreadcum)
        {
            try
            {
                Entities db = new Entities(Session["Connection"] as EntityConnection);
                COMPANY oCOMPANY = db.COMPANies.FirstOrDefault();
                if (!string.IsNullOrEmpty(lblbreadcum))
                {
                    Session["currentPage"] = lblbreadcum;
                }
                if (oCOMPANY == null)
                {
                    ViewBag.Message = new CommonFunction().MessageForView(this.ControllerContext.RouteData.Values["action"].ToString());
                    ViewBag.breadcum = "Add " + Session["currentPage"];
                    //ViewBag.departmentList = new SelectList(new Entities().DEPARTMENTs, "REFERENCE", "NAME");
                    //ViewBag.userGroupList = new SelectList(new Entities().USERGROUPs, "REFERENCE", "NAME");

                    return PartialView();
                }
                else
                {
                    ViewBag.breadcum = "Update " + Session["currentPage"];
                    return PartialView("EditCompany", oCOMPANY);
                }

                
            }

            catch (Exception ex)
            {
                string message = ex.Message;

                return RedirectToAction("Index", "ErrorPage", new { message });

            }
        }


        [HttpPost]
        public ActionResult AddCompany(COMPANY oCOMPANY)
        {
            //DEPARTMENT department = new Entities().DEPARTMENTs.Where(dep => dep.REFERENCE == oAPPLICATIONUSER.DEPARTMENT_REFERENCE).FirstOrDefault();
            //USERGROUP userGroup = new Entities().USERGROUPs.Where(ug => ug.REFERENCE == oAPPLICATIONUSER.USERGROUP_REFERENCE).FirstOrDefault();
            try
            {



                if (Request.IsAjaxRequest())
                {

                    oCOMPANY.REFERENCE = Guid.NewGuid().ToString();
                    oCOMPANY.CREATEDBY = Session["UserId"].ToString();
                    oCOMPANY.CREATEDDATE = DateTime.Now;
                    oCOMPANY.LASTUPDATED = DateTime.Now;

                   
                    oCommonFunction.CustomObjectNullValidation<COMPANY>(ref oCOMPANY);
                    //oCOMPANY.CREATEDBY = "BOSL";
                    //oCOMPANY.CREATEDDATE = DateTime.Today;
                    //oCOMPANY.LASTUPDATED = DateTime.Today;
                    //oCOMPANY.LASTUPDATEDBY = "BOSL";
                    //oCOMPANY.CODE = oCOMPANY.CODE == null ? string.Empty : oCOMPANY.CODE;
                    //oCOMPANY.NAME = oCOMPANY.NAME == null ? string.Empty : oCOMPANY.NAME;
                    //oCOMPANY.REGISTRATIONNO = oCOMPANY.REGISTRATIONNO == null ? string.Empty : oCOMPANY.REGISTRATIONNO;
                    //oCOMPANY.TIN = oCOMPANY.TIN == null ? string.Empty : oCOMPANY.TIN;
                    //oCOMPANY.FAXNUMBER = oCOMPANY.FAXNUMBER == null ? string.Empty : oCOMPANY.FAXNUMBER;
                    //oCOMPANY.WEBSITE = oCOMPANY.WEBSITE == null ? string.Empty : oCOMPANY.WEBSITE;
                    //oCOMPANY.CURRENCY= oCOMPANY.CURRENCY == null ? string.Empty : oCOMPANY.CURRENCY;
                    //oCOMPANY.ADDRESSLINE1 = oCOMPANY.ADDRESSLINE1 == null ? string.Empty : oCOMPANY.ADDRESSLINE1;
                    //oCOMPANY.CITY = oCOMPANY.CITY == null ? string.Empty : oCOMPANY.CITY;
                    //oCOMPANY.POSTCODE = oCOMPANY.POSTCODE == null ? string.Empty : oCOMPANY.POSTCODE;
                    //oCOMPANY.COUNTRY = oCOMPANY.COUNTRY == null ? string.Empty : oCOMPANY.COUNTRY;
                    //oCOMPANY.EMAIL = oCOMPANY.EMAIL == null ? string.Empty : oCOMPANY.EMAIL;

                    
                    using (Entities db = new Entities(Session["Connection"] as EntityConnection))
                    {
                        db.COMPANies.Add(oCOMPANY);
                        db.SaveChanges();
                    }

                }
                return RedirectToAction("Settings", "Settings");


            }
            catch (Exception ex)
            {

                string message = ex.Message;

                return RedirectToAction("Index", "ErrorPage", new { message });
            }

        }

        [HttpPost]
        public ActionResult EditCompany(COMPANY oCOMPANY)
        {
            

            try
            {
                oCOMPANY.LASTUPDATED = DateTime.Now;
                oCOMPANY.LASTUPDATEDBY = Session["UserId"].ToString();
                oCommonFunction.CustomObjectNullValidation<COMPANY>(ref oCOMPANY);
                //oCOMPANY.CODE = oCOMPANY.CODE == null ? string.Empty : oCOMPANY.CODE;
                //oCOMPANY.NAME = oCOMPANY.NAME == null ? string.Empty : oCOMPANY.NAME;
                //oCOMPANY.REGISTRATIONNO = oCOMPANY.REGISTRATIONNO == null ? string.Empty : oCOMPANY.REGISTRATIONNO;
                //oCOMPANY.TIN = oCOMPANY.TIN == null ? string.Empty : oCOMPANY.TIN;
                //oCOMPANY.FAXNUMBER = oCOMPANY.FAXNUMBER == null ? string.Empty : oCOMPANY.FAXNUMBER;
                //oCOMPANY.WEBSITE = oCOMPANY.WEBSITE == null ? string.Empty : oCOMPANY.WEBSITE;
                //oCOMPANY.CURRENCY = oCOMPANY.CURRENCY == null ? string.Empty : oCOMPANY.CURRENCY;
                //oCOMPANY.ADDRESSLINE1 = oCOMPANY.ADDRESSLINE1 == null ? string.Empty : oCOMPANY.ADDRESSLINE1;
                //oCOMPANY.CITY = oCOMPANY.CITY == null ? string.Empty : oCOMPANY.CITY;
                //oCOMPANY.POSTCODE = oCOMPANY.POSTCODE == null ? string.Empty : oCOMPANY.POSTCODE;
                //oCOMPANY.COUNTRY = oCOMPANY.COUNTRY == null ? string.Empty : oCOMPANY.COUNTRY;
                //oCOMPANY.EMAIL = oCOMPANY.EMAIL == null ? string.Empty : oCOMPANY.EMAIL;

                using (Entities db = new Entities(Session["Connection"] as EntityConnection))
                {

                    db.Entry(oCOMPANY).State = EntityState.Modified;
                    db.SaveChanges();
                }

                return RedirectToAction("Settings", "Settings");

            }

            catch (Exception ex)
            {
                string message = ex.Message;

                return RedirectToAction("Index", "ErrorPage", new { message });
            }

        }


       
    }
}

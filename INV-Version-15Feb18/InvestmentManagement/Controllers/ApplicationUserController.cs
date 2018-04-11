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
using System.Configuration;
using InvestmentManagement.App_Code;

namespace InvestmentManagement.Controllers
{
    public class ApplicationUserController : Controller
    {
        //
        // GET: /ApplicationUser/
   
        CommonFunction oCommonFunction = new CommonFunction();

        [HttpGet]
        public ActionResult ListApplicationUser(string sortdir, string sort, int? page, int? rows, string filterstring, string currentFilter, string lblbreadcum, string PagingType, int? currentRowPerPage = 15)
        {
            if (Session["UserId"] == null || Session["PreviousPage"] == null || Session["currentPage"] == null || Session["Connection"] == null)
            {
                return RedirectToAction("LogOut", "Home");

            }
            try
            {
                //currentRowPerPage=@ViewBag.currentRowPerPage
                GridModel<APPLICATIONUSER> gridModels = new GridModel<APPLICATIONUSER>();
                List<APPLICATIONUSER> models = null;

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
                    models = new Entities(Session["Connection"] as EntityConnection).APPLICATIONUSERs.Include("DEPARTMENT").Include("USERGROUP").AsNoTracking().OrderBy(sort + " " + sortdir).Skip(skipcount).Take(gridModels.RowsPerPage).ToList();
                else
                    models = new Entities(Session["Connection"] as EntityConnection).APPLICATIONUSERs.Include("DEPARTMENT").Include("USERGROUP").AsNoTracking().Where(w => w.NAME.Contains(filterstring)).OrderBy(sort + " " + sortdir).Skip(skipcount).Take(gridModels.RowsPerPage).ToList();

                //Filter by User So User Can Not see others Data

                if (Session["UserId"].ToString() == ApplicationVariable.User_SuperAdminUserName) { 
                 // return all list of the model
                }
                else if (Session["UserId"].ToString() == ApplicationVariable.User_GeneralUserFdr)
                {
                    models = models.Where(t => t.USERID == ApplicationVariable.User_GeneralUserFdr).ToList();
                }
                else if (Session["UserId"].ToString() ==ApplicationVariable.User_GeneralUserStock){

                    models = models.Where(t => t.USERID == ApplicationVariable.User_GeneralUserStock).ToList();
                }
                else if (Session["UserId"].ToString() == ApplicationVariable.User_GeneralUserBond)
                {
                    models = models.Where(t => t.USERID == ApplicationVariable.User_GeneralUserBond).ToList(); 
                }
                else if (Session["UserId"].ToString() == ApplicationVariable.User_AdminUserInvestment)
                {
                    models = models.Where(t => t.USERID == ApplicationVariable.User_AdminUserInvestment).ToList();
                }
                else {
                    models = null;
                }


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
                return PartialView("ListApplicationUser", gridModels);



            }

            catch (Exception ex)
            {
                string message = ex.Message;

                return RedirectToAction("Index", "ErrorPage", new { message });
            }



        }

        [HttpGet]
        public ActionResult AddApplicationUser()
        {
            if (Session["UserId"] == null || Session["PreviousPage"] == null || Session["currentPage"] == null || Session["Connection"] == null)
            {
                return RedirectToAction("LogOut", "Home");

            }
            try
            {
                ViewBag.Message = new CommonFunction().MessageForView(this.ControllerContext.RouteData.Values["action"].ToString());
                ViewBag.breadcum = oCommonFunction.GetAddPath(Session["Path"] as IHtmlString, Session["currentPage"].ToString());
                ViewBag.Header = "Add " + Session["currentPage"];
                ViewBag.departmentList = new SelectList(new Entities(Session["Connection"] as EntityConnection).DEPARTMENTs, "REFERENCE", "NAME");
                ViewBag.userGroupList = new SelectList(new Entities(Session["Connection"] as EntityConnection).USERGROUPs, "REFERENCE", "NAME");

                return PartialView();
            }

            catch (Exception ex)
            {
                string message = ex.Message;

                return RedirectToAction("Index", "ErrorPage", new { message });

            }
        }

        [HttpPost]
        public ActionResult AddApplicationUser(APPLICATIONUSER oAPPLICATIONUSER)
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

                        oAPPLICATIONUSER.REFERENCE = Guid.NewGuid().ToString();
                        oAPPLICATIONUSER.CREATEDBY = Session["UserId"].ToString();
                        oAPPLICATIONUSER.CREATEDDATE = DateTime.Today;
                        oAPPLICATIONUSER.LASTUPDATED = DateTime.Today;


                        RijndaelEncryption encryption = new RijndaelEncryption();
                        string encryptionKey = ConfigurationManager.AppSettings["EncryptionKey"];
                        oAPPLICATIONUSER.PASSWORD = encryption.EncryptText(oAPPLICATIONUSER.PASSWORD, encryptionKey);


                        oCommonFunction.CustomObjectNullValidation<APPLICATIONUSER>(ref oAPPLICATIONUSER);

                        using (Entities db = new Entities(Session["Connection"] as EntityConnection))
                        {
                            db.APPLICATIONUSERs.Add(oAPPLICATIONUSER);
                            db.SaveChanges();
                        }

                    }
                    return RedirectToAction("ListApplicationUser");
               
                
            }
            catch (Exception ex)
            {

                string message = ex.Message;

                return RedirectToAction("Index", "ErrorPage", new { message });
            }

        }


        [HttpGet]
        public ActionResult EditApplicationUser(string id)
        {
            if (Session["UserId"] == null || Session["PreviousPage"] == null || Session["currentPage"] == null || Session["Connection"] == null)
            {
                return RedirectToAction("LogOut", "Home");

            }
            try
            {
                APPLICATIONUSER oAPPLICATIONUSER = new APPLICATIONUSER();
                Entities db = new Entities(Session["Connection"] as EntityConnection);
                ViewModelBase oViewModelBase = new ViewModelBase();
                

                oAPPLICATIONUSER = db.APPLICATIONUSERs.Include("DEPARTMENT").Include("USERGROUP").SingleOrDefault(i => i.REFERENCE == id);
                ViewBag.departmentList = new SelectList(new Entities(Session["Connection"] as EntityConnection).DEPARTMENTs, "REFERENCE", "NAME");
                ViewBag.userGroupList = new SelectList(new Entities(Session["Connection"] as EntityConnection).USERGROUPs, "REFERENCE", "NAME");
                ViewBag.Message = new CommonFunction().MessageForView(this.ControllerContext.RouteData.Values["action"].ToString());
                ViewBag.breadcum = oCommonFunction.GetEditPath(Session["Path"] as IHtmlString, Session["currentPage"].ToString());
                ViewBag.Header = "Update " + Session["currentPage"];

                return PartialView("EditApplicationUser", oAPPLICATIONUSER);
            }

            catch (Exception ex)
            {
                string message = ex.Message;

                return RedirectToAction("Index", "ErrorPage", new { message });

            }
        }

        [HttpPost]
        public ActionResult EditApplicationUser(APPLICATIONUSER oAPPLICATIONUSER)
        {

            if (Session["UserId"] == null || Session["PreviousPage"] == null || Session["currentPage"] == null || Session["Connection"] == null)
            {
                return RedirectToAction("LogOut", "Home");

            }

            try
            {
            //oAPPLICATIONUSER.CREATEDBY = "BOSL";
            //oAPPLICATIONUSER.CREATEDDATE = DateTime.Parse("05-04-2015");
            //oAPPLICATIONUSER.DEPARTMENT = department;

            Entities db = new Entities(Session["Connection"] as EntityConnection);
            APPLICATIONUSER EditUser = db.APPLICATIONUSERs.Find(oAPPLICATIONUSER.REFERENCE);

            EditUser.LASTUPDATED = DateTime.Today;
            EditUser.LASTLOGIN = DateTime.Today;
            

            RijndaelEncryption encryption = new RijndaelEncryption();
            string encryptionKey = ConfigurationManager.AppSettings["EncryptionKey"];

            EditUser.PASSWORD = encryption.EncryptText(oAPPLICATIONUSER.PASSWORD, encryptionKey);

            EditUser.USERGROUP_REFERENCE = oAPPLICATIONUSER.USERGROUP_REFERENCE;
            EditUser.DEPARTMENT_REFERENCE = oAPPLICATIONUSER.DEPARTMENT_REFERENCE;

            EditUser.EMAILADDRESS = oAPPLICATIONUSER.EMAILADDRESS == null ? string.Empty : oAPPLICATIONUSER.EMAILADDRESS;

            EditUser.STATUS = oAPPLICATIONUSER.STATUS == null ? string.Empty : oAPPLICATIONUSER.STATUS;

            EditUser.LASTUPDATED = DateTime.Today;
            EditUser.LASTUPDATEDBY = Session["UserId"].ToString();
            oCommonFunction.CustomObjectNullValidation<APPLICATIONUSER>(ref EditUser);


            db.Entry(EditUser).State = EntityState.Modified;
            db.SaveChanges();
            
            return RedirectToAction("ListApplicationUser");

            }

            catch (Exception ex)
            {
                string message = ex.Message;

                return RedirectToAction("Index", "ErrorPage", new { message });
            }

        }
    }
}

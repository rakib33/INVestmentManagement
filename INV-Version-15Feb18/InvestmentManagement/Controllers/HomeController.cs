using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Linq.Dynamic;
using InvestmentManagement.Models;
using InvestmentManagement.ViewModel;
using System.Configuration;
using InvestmentManagement.InvestmentManagement.Models;
using System.Data.EntityClient;
using System.Data;


namespace InvestmentManagement.Controllers
{

    public class HomeController : Controller
    {

        CommonFunction oCommonFunction = new CommonFunction();
        
        public ActionResult Index()
        {
            try
            {             
                                            
                ViewModelBase oViewModelBase = new ViewModelBase();
                if (Request.QueryString["lblbreadcum"] != null)
                {
                    if (Session["UserId"] == null || Session["PreviousPage"] == null || Session["currentPage"] == null || Session["Connection"] == null)
                    {
                        return RedirectToAction("LogOut", "Home");
                    }
                    return PartialView("Index", oViewModelBase);
                }
                else
                {
                    return View("Default", oViewModelBase);
                }          

            }

            catch (Exception ex)
            {
                string message = ex.Message;                
                return RedirectToAction("Index", "ErrorPage", new { message });            
            }         
           
        }


        [HttpPost]
        public ActionResult Index(string userid, string password)
        {
              int Ref = 1;
            try
            {
              
                ViewModelBase oViewModelBase = new ViewModelBase();
                RijndaelEncryption encryption = new RijndaelEncryption();
                string encryptionKey = ConfigurationManager.AppSettings["EncryptionKey"];
                password = encryption.EncryptText(password, encryptionKey);

                //var decrypt = encryption.DecryptText("ImLdAfmd2u0q42nZloMLHQ==", encryptionKey);

                EntityConnection conn = new EntityConnection("name=Entities");
                if (conn.State != ConnectionState.Open)
                {
                    conn.Open();
                }
                Session["Connection"] = conn;

                Ref =2;
                var applicationUser = new Entities(Session["Connection"] as EntityConnection).APPLICATIONUSERs.Where(model => model.USERID == userid && model.PASSWORD == password && model.STATUS == "Active").SingleOrDefault();

                Ref = 3;
                if (applicationUser == null)
                {
                    ViewBag.Message = "User Id or Password does not match";
                    return View("Default", oViewModelBase);
                }
                else
                {
                    if (applicationUser.STATUS == "Active")
                    {
                        Session["UserId"] = applicationUser.USERID;
                        Session["DepartmentId"] = applicationUser.DEPARTMENT_REFERENCE;
                        
                        //get all menu
                        oViewModelBase.Menus = oCommonFunction.GetMenus();
                        
                        Session["PreviousPage"] = "Dashboard";
                        Session["currentPage"] = "Dashboard";
                        Session["Path"] = "";                       
                      //  return View("Index", oViewModelBase);

                        return View("Index");

                    }
                    else
                    {
                        ViewBag.Message = "Your status is " + applicationUser.STATUS + ". Please contact with your system admin or provider";
                        return View("Default", oViewModelBase);
                    }

                }

            }

            catch (Exception ex)
            {
                string message =Ref+" Exp:"+ ex.Message+ " Inner Exp:"+ Convert.ToString(ex.InnerException.Message);

                return RedirectToAction("Index", "ErrorPage", new { message });
            }
        }

        public ActionResult LogOut()
        {
            if (Session["Connection"] != null)
            {
                EntityConnection conn = Session["Connection"] as EntityConnection;

                if (conn.State == ConnectionState.Open)
                {
                    conn.Close();
                }
            }

            Session.Clear();
            Session.Abandon();
            return PartialView();
        }

        public ActionResult SingleAddForm()
        {
            return PartialView();
        }

        public ActionResult JqueryAlertBox()
        {
            return View();
        }
    }
}

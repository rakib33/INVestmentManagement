using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.EntityClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using InvestmentManagement.InvestmentManagement.Models;
using InvestmentManagement.Models;

namespace InvestmentManagement.Controllers
{
    public class DefaultController : Controller
    {
        //
        // GET: /Default/

        public ActionResult Index()
        {
            return View();
        }


        [HttpPost]
        public ActionResult Index(string userid, string password)
        {
            RijndaelEncryption encryption = new RijndaelEncryption();
            string encryptionKey = ConfigurationManager.AppSettings["EncryptionKey"];
            password = encryption.EncryptText(password, encryptionKey);
            var applicationUser = new Entities(Session["Connection"] as EntityConnection).APPLICATIONUSERs.Where(model => model.USERID == userid && model.PASSWORD == password).ToList();
            if (applicationUser.Count == 1)
            {
             

                return RedirectToAction("Index","Home");

            }

            else
            {

                return View();
            }

          
        }

        public ActionResult WorkFlow(string lblbreadcum)
        {
            
            Session["Path"] = new CommonFunction().GetPath(this.ControllerContext.RouteData.Values["controller"].ToString(), Session["currentPage"].ToString(), this.ControllerContext.RouteData.Values["action"].ToString());
            ViewBag.Message = new CommonFunction().MessageForView(this.ControllerContext.RouteData.Values["action"].ToString());
            if (!string.IsNullOrEmpty(lblbreadcum))
            {
                Session["currentPage"] = lblbreadcum;
            }

            ViewBag.BreadCum = new CommonFunction().GetListPath(Session["currentPage"].ToString(), this.ControllerContext.RouteData.Values["controller"].ToString());
            
            return PartialView();
        }


    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using InvestmentManagement.Models;
using InvestmentManagement.ViewModel;
using System.Linq.Dynamic;
using System.Data;
using InvestmentManagement.InvestmentManagement.Models;
using System.Data.EntityClient;
namespace InvestmentManagement.Controllers
{
    public class FIBranchController : Controller
    {
        CommonFunction oCommonFunction = new CommonFunction();

        //
        // GET: /Branch/
        [HttpGet]
        public ActionResult ListFIBranch(string sortdir, string sort, string sortdefault, int? rows, string filterstring, string lblbreadcum, string PagingType, string reference = null)
        {
            try
            {
                if (Session["UserId"] == null || Session["PreviousPage"] == null || Session["currentPage"] == null || Session["Connection"] == null)
                {
                    return RedirectToAction("LogOut", "Home");

                }

                if (string.IsNullOrEmpty(sortdefault))
                {
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

                            break;
                    }
                }
                else
                {
                    sort = sortdefault;
                }

                //currentRowPerPage=@ViewBag.currentRowPerPage
                GridModel<FIBRANCH> gridModels = new GridModel<FIBRANCH>();
                List<FIBRANCH> models = null;

                //grid settings                
                gridModels.RowsPerPage = rows == null ? 15 : (int)rows;
                //ViewBag.currentRowPerPage = gridModels.RowsPerPage;

                //Paging direction
                //int count = new Entities().FINANCIALINSTITUTIONs.Count();


                //loading configuration
                sort = string.IsNullOrEmpty(sort) == true ? "CREATEDDATE" : sort;
                sortdir = string.IsNullOrEmpty(sortdir) == true ? "DESC" : sortdir;
                //int skipcount = gridModels.RowsPerPage * ((int)Session["pageNo"] - 1);
                //if (filterstring == null)
                //{
                //    filterstring = currentFilter;
                //}


                //ViewBag.CurrentFilter = filterstring;

                //models = models.Where(m => m.FINANCIALINSTITUTION_REFERENCE == reference).ToList();
                if (string.IsNullOrEmpty(filterstring))
                    models = new Entities(Session["Connection"] as EntityConnection).FIBRANCHes.Include("FINANCIALINSTITUTION").AsNoTracking().Where(m => m.FINANCIALINSTITUTION_REFERENCE == reference).OrderBy(sort + " " + sortdir).ToList();
                else
                    models = new Entities(Session["Connection"] as EntityConnection).FIBRANCHes.Include("FINANCIALINSTITUTION").AsNoTracking().Where(w => w.NAME.Contains(filterstring) && w.FINANCIALINSTITUTION_REFERENCE == reference).OrderBy(sort + " " + sortdir).ToList();

                ViewBag.Refference = reference;

               
                gridModels.DataModel = models;

                ViewBag.Message = oCommonFunction.MessageForView(this.ControllerContext.RouteData.Values["action"].ToString());

                if (!string.IsNullOrEmpty(lblbreadcum))
                {
                    Session["currentPage"] = lblbreadcum;
                    Session["Path"] = oCommonFunction.GetDetailsPath(Session["Path"] as IHtmlString, this.ControllerContext.RouteData.Values["controller"].ToString(), Session["currentPage"].ToString(), this.ControllerContext.RouteData.Values["action"].ToString(), reference);
                }



                ViewBag.BreadCum = oCommonFunction.GetDetailsListPath(Session["Path"] as IHtmlString, Session["currentPage"].ToString());
              

                if ((int)Session["pageNo"] == 1)
                {
                    ViewBag.Prev = "disabled";
                    ViewBag.PrevNotActive = "not-active";
                }
                //if (models.Count() < gridModels.RowsPerPage && (int)Session["pageNo"] == 1)
                //{

                //    ViewBag.Prev = "disabled";
                //    ViewBag.Next = "disabled";
                //    ViewBag.PrevNotActive = "not-active";
                //    ViewBag.NextNotActive = "not-active";
                //}
                //if (models.Count() < gridModels.RowsPerPage && (int)Session["pageNo"] > 1)
                //{
                //    ViewBag.Next = "disabled";
                //    ViewBag.NextNotActive = "not-active";

                //}


                return PartialView("ListFIBranch", gridModels);



            }

            catch (Exception ex)
            {
                string message = ex.Message;

                return RedirectToAction("Index", "ErrorPage", new { message });
            }

        }

        public ActionResult AddFIBranch(string reference)
        {
            try
            {
                if (Session["UserId"] == null || Session["PreviousPage"] == null || Session["currentPage"] == null || Session["Connection"] == null)
                {
                    return RedirectToAction("LogOut", "Home");

                }
                //ViewModelBase oViewModelBase = new ViewModelBase();
                //oViewModelBase.Currencies = new Entities().CURRENCies.ToList();
                // oViewModelBase.Menus = new Entities().MENUs.ToList();
                ViewBag.Refference = reference;
                ViewBag.Message = new CommonFunction().MessageForView(this.ControllerContext.RouteData.Values["action"].ToString());
                ViewBag.breadcum = oCommonFunction.GetDetailsAddPath(Session["Path"] as IHtmlString, Session["currentPage"].ToString());
                ViewBag.Header = "Add " + Session["currentPage"];
                return PartialView("AddFIBranch");
            }

            catch (Exception ex)
            {
                string message = ex.Message;

                return RedirectToAction("Index", "ErrorPage", new { message });

            }
          
        }

        [HttpPost]
        public ActionResult AddFIBranch(FIBRANCH oFIBRANCH, string Parentreference)
        {
            try
            {
                if (Session["UserId"] == null || Session["PreviousPage"] == null || Session["currentPage"] == null || Session["Connection"] == null)
                {
                    return RedirectToAction("LogOut", "Home");

                }
                if (!string.IsNullOrEmpty(Parentreference))
                {

                    using (Entities db = new Entities(Session["Connection"] as EntityConnection))
                    {

                        oFIBRANCH.FINANCIALINSTITUTION_REFERENCE = Parentreference;
                        oFIBRANCH.REFERENCE = Guid.NewGuid().ToString();
                        oFIBRANCH.CREATEDDATE = DateTime.Now;
                        oFIBRANCH.CREATEDBY = Session["UserId"].ToString();
                        oFIBRANCH.LASTUPDATED = DateTime.Now;
                        oCommonFunction.CustomObjectNullValidation<FIBRANCH>(ref oFIBRANCH);
                        db.FIBRANCHes.Add(oFIBRANCH);

                        db.SaveChanges();

                    }
                
                }
               


              

                //Index(null, null, null, null, null);
                return RedirectToAction("ListFIBranch", new { reference = oFIBRANCH.FINANCIALINSTITUTION_REFERENCE });
            }

            catch (Exception ex)
            {
                string message = ex.Message;

                return RedirectToAction("Index", "ErrorPage", new { message });
            }



        }

        [HttpGet]
        public ActionResult EditFIBRANCH(string id)
        {
            try
            {
                if (Session["UserId"] == null || Session["PreviousPage"] == null || Session["currentPage"] == null || Session["Connection"] == null)
                {
                    return RedirectToAction("LogOut", "Home");

                }
                FIBRANCH oFIBranch = new FIBRANCH();

                Entities db = new Entities(Session["Connection"] as EntityConnection);

                //ViewBag.Refference = reference;
                oFIBranch = db.FIBRANCHes.SingleOrDefault(i => i.REFERENCE == id);
                
                ViewBag.Message = new CommonFunction().MessageForView(this.ControllerContext.RouteData.Values["action"].ToString());
                ViewBag.breadcum = oCommonFunction.GetDetailsEditPath(Session["Path"] as IHtmlString, Session["currentPage"].ToString());
                ViewBag.Header = "Update " + Session["currentPage"];

                return PartialView("EditFIBRANCH", oFIBranch);
            }

            catch (Exception ex)
            {
                string message = ex.Message;

                return RedirectToAction("Index", "ErrorPage", new { message });

            }


        }

        [HttpPost]
        public ActionResult EditFIBRANCH(FIBRANCH oFIBranch)
        {
            try
            {
                if (Session["UserId"] == null || Session["PreviousPage"] == null || Session["currentPage"] == null || Session["Connection"] == null)
                {
                    return RedirectToAction("LogOut", "Home");

                }
                if (oFIBranch.FINANCIALINSTITUTION_REFERENCE!=null)
                {
                    using (Entities db = new Entities(Session["Connection"] as EntityConnection))
                    {

                        oFIBranch.LASTUPDATED = DateTime.Now;
                        oFIBranch.LASTUPDATEDBY = Session["UserId"].ToString();
                        oCommonFunction.CustomObjectNullValidation<FIBRANCH>(ref oFIBranch);
                        db.SaveChanges();
                    }

                }
               
                return RedirectToAction("ListFIBranch", new { reference = oFIBranch.FINANCIALINSTITUTION_REFERENCE });

            }

            catch (Exception ex)
            {
                string message = ex.Message;

                return RedirectToAction("Index", "ErrorPage", new { message });
            }

        }

        public ActionResult DoubleAddTemplate()
        {
            ViewBag.breadcum = "Add " + Session["currentPage"];
            return PartialView();
        }

        public ActionResult DoubleTest()
        {

            return View();
        }
    }
}

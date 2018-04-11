using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using InvestmentManagement.InvestmentManagement.Models;


using System.Web.Routing;
using InvestmentManagement.Models;
using System.Linq.Dynamic;
using InvestmentManagement.ViewModel;
using System.Data.EntityClient;

using PagedList;

namespace InvestmentManagement.Controllers
{
    public class TestController : Controller
    {
           
        private Entities db = new Entities();
        CommonFunction oCommonFunction = new CommonFunction();
       
        // GET: /Test/
        [HttpGet]
        public ActionResult ListDepartment(string sortdir, string sort, int? page, int? rows, string filterstring, string currentFilter, string lblbreadcum, string PagingType, int? currentRowPerPage = 2)
        {
            try
            {
                ViewBag.message = TempData["message"] as string;

                //currentRowPerPage=@ViewBag.currentRowPerPage
                GridModel<TEST> gridModels = new GridModel<TEST>();
                List<TEST> models = null;

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
                //sort = string.IsNullOrEmpty(sort) == true ? "CREATEDDATE" : sort;
                //sortdir = string.IsNullOrEmpty(sortdir) == true ? "DESC" : sortdir;

                int skipcount = gridModels.RowsPerPage * ((int)Session["pageNo"] - 1);
                if (filterstring == null)
                {
                    filterstring = currentFilter;
                }


                ViewBag.CurrentFilter = filterstring;


                if (string.IsNullOrEmpty(filterstring))
                    models = new Entities(Session["Connection"] as EntityConnection).TESTs.AsNoTracking().OrderByDescending(t=>t.ID).Skip(skipcount).Take(gridModels.RowsPerPage).ToList(); //OrderBy(sort + " " + sortdir)
                else
                    models = new Entities(Session["Connection"] as EntityConnection).TESTs.AsNoTracking().Where(w => w.NAME.Contains(filterstring)).OrderByDescending(t=>t.ID).Skip(skipcount).Take(gridModels.RowsPerPage).ToList();  //OrderBy(sort + " " + sortdir)


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

               
                return PartialView("Index", gridModels);
                
            }

            catch (Exception ex)
            {
                string message = ex.Message;
                return RedirectToAction("Index", "ErrorPage", new { message });
            }



        }

        [HttpGet]
        public ActionResult AddTest()
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
        public ActionResult AddTest(TEST oDEPARTMENT)
        {
            //DEPARTMENT department = new Entities().DEPARTMENTs.Where(dep => dep.REFERENCE == oAPPLICATIONUSER.DEPARTMENT_REFERENCE).FirstOrDefault();
            //USERGROUP userGroup = new Entities().USERGROUPs.Where(ug => ug.REFERENCE == oAPPLICATIONUSER.USERGROUP_REFERENCE).FirstOrDefault();
            try
            {



                if (Request.IsAjaxRequest())
                {

                    using (Entities db = new Entities(Session["Connection"] as EntityConnection))
                    {
                        db.TESTs.Add(oDEPARTMENT);                      
                        db.SaveChanges();
                    }

                }
                return RedirectToAction("ListDepartment", "Test", new { lblbreadcum="Test List" });


            }
            catch (Exception ex)
            {

                string message = ex.Message;

                return RedirectToAction("Index", "ErrorPage", new { message });
            }

        }


        [HttpGet]
        public ActionResult EditTest(int? id)
        {
            try
            {
                TEST oDEPARTMENT = new TEST();
                Entities db = new Entities(Session["Connection"] as EntityConnection);
                ViewModelBase oViewModelBase = new ViewModelBase();


                oDEPARTMENT = db.TESTs.SingleOrDefault(i => i.ID == id);
               
                ViewBag.Message = new CommonFunction().MessageForView(this.ControllerContext.RouteData.Values["action"].ToString());
                ViewBag.breadcum = oCommonFunction.GetEditPath(Session["Path"] as IHtmlString, Session["currentPage"].ToString());
                ViewBag.Header = "Update " + Session["currentPage"];                

                return PartialView("EditTest", oDEPARTMENT);
            }

            catch (Exception ex)
            {
                string message = ex.Message;

                return RedirectToAction("Index", "ErrorPage", new { message });

            }
        }


        [HttpPost]
        public ActionResult EditTest(TEST oDEPARTMENT)
        {
          try
            {               

                using (Entities db = new Entities(Session["Connection"] as EntityConnection))
                {
                    db.Entry(oDEPARTMENT).State = EntityState.Modified;
                    db.SaveChanges();
                }

                TempData["message"] = oDEPARTMENT.NAME + " successfully edited.";
                return RedirectToAction("ListDepartment", "Test", new { lblbreadcum = "Test List" });
             

            }

            catch (Exception ex)
            {
                string message = ex.Message;

                return RedirectToAction("Index", "ErrorPage", new { message });
            }

        }

        //
        // GET: /Test/Delete/5

        public ActionResult Delete(int id = 0)
        {
            string message="Data is not found !!";

            try
            {
               // Entities db = new Entities(Session["Connection"] as EntityConnection);
                TEST test = db.TESTs.Find(id);

                ViewBag.Message = new CommonFunction().MessageForView(this.ControllerContext.RouteData.Values["action"].ToString());
                ViewBag.breadcum = oCommonFunction.GetEditPath(Session["Path"] as IHtmlString, Session["currentPage"].ToString());
                ViewBag.Header = "Delete " + Session["currentPage"];

                if (test == null)
                {

                    return RedirectToAction("Index", "ErrorPage", new { message });
                }
                return PartialView("Delete", test);
            }
            catch (Exception ex)
            {
                message = ex.Message;

                return RedirectToAction("Index", "ErrorPage", new { message });
            }
        }

        //
        // POST: /Test/Delete/5

        [HttpPost, ActionName("Delete")]
        public ActionResult Delete(TEST model)
        {

            try
            {

                using (Entities db = new Entities(Session["Connection"] as EntityConnection))
                {
                    TEST test = db.TESTs.Find(model.ID);
                    db.TESTs.Remove(test);
                    db.SaveChanges();                   
                }
                
                TempData["message"]= model.NAME + " was successfully Deleted.";
                return RedirectToAction("ListDepartment", "Test", new { lblbreadcum = "Test" });


            }

            catch (Exception ex)
            {
                string message = ex.Message;

                return RedirectToAction("Index", "ErrorPage", new { message });
            }           
           
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
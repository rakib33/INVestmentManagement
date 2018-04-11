using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using InvestmentManagement.InvestmentManagement.Models;
using InvestmentManagement.Models;
using InvestmentManagement.App_Code;

namespace InvestmentManagement.Controllers
{
    public class HolidayCalenderController : Controller
    {
        private Entities db = new Entities();
        CommonFunction oCommonFunction = new CommonFunction();
        //
        // GET: /HolidayCalender/

        //public ActionResult ListHoliday()
        //{
        //    return View(db.HOLIDAYCALENDERs.ToList());
        //}

        public ActionResult ListHoliday(string sortdir, string sort, int? page, int? rows, string filterstring, string currentFilter, string lblbreadcum, string PagingType, int? currentRowPerPage = 15)
        {
            if (Session["UserId"] == null || Session["PreviousPage"] == null || Session["currentPage"] == null || Session["Connection"] == null)
            {
                return RedirectToAction("LogOut", "Home");

            }
            try
            {
                ViewBag.Message = Convert.ToString(TempData["message"]);
                //currentRowPerPage=@ViewBag.currentRowPerPage
                GridModel<HOLIDAYCALENDER> gridModels = new GridModel<HOLIDAYCALENDER>();
                List<HOLIDAYCALENDER> models = null;

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
                sort = string.IsNullOrEmpty(sort) == true ? "HOLIDAY" : sort;
                sortdir = string.IsNullOrEmpty(sortdir) == true ? "DESC" : sortdir;
                int skipcount = gridModels.RowsPerPage * ((int)Session["pageNo"] - 1);
                if (filterstring == null)
                {
                    filterstring = currentFilter;
                }


                ViewBag.CurrentFilter = filterstring;


                if (string.IsNullOrEmpty(filterstring))
                    models = db.HOLIDAYCALENDERs.AsNoTracking().OrderByDescending(t=>t.HOLIDAY).Take(gridModels.RowsPerPage).ToList();

                //else
                //    models = new Entities(Session["Connection"] as EntityConnection).USERGROUPs.Include("DEPARTMENT").AsNoTracking().Where(w => w.NAME.Contains(filterstring)).OrderBy(sort + " " + sortdir).Skip(skipcount).Take(gridModels.RowsPerPage).ToList();


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
                
                ViewBag.Message = Convert.ToString(TempData["message"]);
                return PartialView("ListHoliday", gridModels);
              

            }

            catch (Exception ex)
            {               
                string message = ex.Message;
                return RedirectToAction("Index", "ErrorPage", new { message });
            }



        }


        //
        // GET: /HolidayCalender/Details/5

        //public ActionResult Details(string id = null)
        //{
        //    HOLIDAYCALENDER holidaycalender = db.HOLIDAYCALENDERs.Find(id);
        //    if (holidaycalender == null)
        //    {
        //        return HttpNotFound();
        //    }
        //    return View(holidaycalender);
        //}

        //
        // GET: /HolidayCalender/Create

        [HttpGet]
        public ActionResult AddHoliday()
        {
            try
            {
                ViewBag.Message = new CommonFunction().MessageForView(this.ControllerContext.RouteData.Values["action"].ToString());
                ViewBag.breadcum = oCommonFunction.GetAddPath(Session["Path"] as IHtmlString, Session["currentPage"].ToString());
                ViewBag.Header = "Add " + Session["currentPage"];

                ViewBag.Weekend_one = null;
                ViewBag.Weekedn_two = null;

                var weekend = db.WEEKENDs.SingleOrDefault();
                //Weekend
                int Index = 0;
                foreach (var item in WeekDayList())
                {
                    Index++;
                    if (item == weekend.WEEKEND_ONE)
                    {
                        ViewBag.Weekend_one = "Weekend" + Index;
                    }
                    if (item == weekend.WEEKEND_TWO)
                    {
                        ViewBag.Weekedn_two = "Weekend" + Index;
                    }

                }              

                return PartialView();
            }

            catch (Exception ex)
            {
                string message = ex.Message;

                return RedirectToAction("Index", "ErrorPage", new { message });

            }
        }


        public List<string> WeekDayList()
        {
            return new List<string> { "Saturday", "Sunday", "Monday", "Tuesday", "Wednesday", "Thrusday", "Friday" };            
        }
        //
        // POST: /HolidayCalender/Create
        
        [HttpPost]
        public ActionResult AddHoliday(HOLIDAYCALENDER holidaycalender, string Weekend1, string Weekend2, string Weekend3, string Weekend4, string Weekend5, string Weekend6, string Weekend7)
        {
            if (Session["UserId"] == null || Session["PreviousPage"] == null || Session["currentPage"] == null || Session["Connection"] == null)
            {
                return RedirectToAction("LogOut", "Home");

            }
            string W1 = null;
            string w2 = null;

            if (ModelState.IsValid)
            {

                try
                {
                    //check is this day already exists

                    var IsExists = db.HOLIDAYCALENDERs.Where(t => t.HOLIDAY == holidaycalender.HOLIDAY).SingleOrDefault();

                    if (IsExists != null)
                    {
                        TempData["message"] = new HtmlString("<div style=\"color:red;display:inline\"> Holiday Calender already contains date " + holidaycalender.HOLIDAY.Value.ToString("dd-MMM-yy") + "</div>"); 
                        
                        return RedirectToAction("ListHoliday");
                    }
                    else
                    {
                        List<WEEKEND> weekendlist = new List<WEEKEND>();

                        weekendlist.Add(new WEEKEND { WEEKEND_ONE = Weekend1 == "on" ? "Saturday" : null });
                        weekendlist.Add(new WEEKEND { WEEKEND_ONE = Weekend2 == "on" ? "Sunday" : null });
                        weekendlist.Add(new WEEKEND { WEEKEND_ONE = Weekend3 == "on" ? "Monday" : null });
                        weekendlist.Add(new WEEKEND { WEEKEND_ONE = Weekend4 == "on" ? "Tuesday" : null });
                        weekendlist.Add(new WEEKEND { WEEKEND_ONE = Weekend5 == "on" ? "Wednesday" : null });
                        weekendlist.Add(new WEEKEND { WEEKEND_ONE = Weekend6 == "on" ? "Thrusday" : null });
                        weekendlist.Add(new WEEKEND { WEEKEND_ONE = Weekend7 == "on" ? "Friday" : null });

                        foreach (var item in weekendlist)
                        {
                            if (item.WEEKEND_ONE != null && !string.IsNullOrEmpty(item.WEEKEND_ONE))
                            {
                                if (W1 == null && string.IsNullOrEmpty(W1))
                                {
                                    W1 = item.WEEKEND_ONE;
                                }
                                else
                                    w2 = item.WEEKEND_ONE;
                            }

                            if (W1 != null && !string.IsNullOrEmpty(W1) && w2 != null && !string.IsNullOrEmpty(w2))
                            {
                                break;
                            }
                        }

                        WEEKEND weekend = db.WEEKENDs.FirstOrDefault();

                        weekend.WEEKEND_ONE = W1;
                        weekend.WEEKEND_TWO = w2;

                        if (weekend.REFERENCE == null) //add
                        {
                            weekend.REFERENCE = new Guid().ToString();                           
                            db.WEEKENDs.Add(weekend);
                        }
                        else
                        {
                            db.Entry(weekend).State = EntityState.Modified;
                        }//edit

                        holidaycalender.REFERENCE = Guid.NewGuid().ToString();
                        holidaycalender.CREATEDDATE = DateTime.Today;
                        holidaycalender.CREADEDBY = Session["UserId"].ToString();

                        db.HOLIDAYCALENDERs.Add(holidaycalender);
                        db.SaveChanges();
                        
                        return RedirectToAction("ListHoliday");
                    }
                }
                catch (Exception ex)
                {

                    string message = ex.Message;

                    return RedirectToAction("Index", "ErrorPage", new { message });
                }
            }

            return PartialView(holidaycalender);
        }

        //
        // GET: /HolidayCalender/Edit/5

        public ActionResult Edit(string id = null)
        {



            HOLIDAYCALENDER holidaycalender = db.HOLIDAYCALENDERs.Find(id);
            if (holidaycalender == null)
            {
                return HttpNotFound();
            }
            return View(holidaycalender);
        }

        //
        // POST: /HolidayCalender/Edit/5

        [HttpPost]
        public ActionResult Edit(HOLIDAYCALENDER holidaycalender)
        {
            if (ModelState.IsValid)
            {
                db.Entry(holidaycalender).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(holidaycalender);
        }

        //
        //// GET: /HolidayCalender/Delete/5

        //public ActionResult Delete(string id = null)
        //{
        //    HOLIDAYCALENDER holidaycalender = db.HOLIDAYCALENDERs.Find(id);
        //    if (holidaycalender == null)
        //    {
        //        return HttpNotFound();
        //    }
        //    return View(holidaycalender);
        //}

        ////
        //// POST: /HolidayCalender/Delete/5

        //[HttpPost, ActionName("Delete")]
        //public ActionResult DeleteConfirmed(string id)
        //{
        //    HOLIDAYCALENDER holidaycalender = db.HOLIDAYCALENDERs.Find(id);
        //    db.HOLIDAYCALENDERs.Remove(holidaycalender);
        //    db.SaveChanges();
        //    return RedirectToAction("Index");
        //}

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }

  
}
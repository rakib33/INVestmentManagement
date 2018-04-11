using InvestmentManagement.InvestmentManagement.Models;
using InvestmentManagement.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.EntityClient;
using System.Data.OracleClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace InvestmentManagement.Controllers
{
    public class NominalLedgerController : Controller
    {
        //
        // GET: /NominalLedger/
        CommonFunction oCommonFunction = new CommonFunction();

        [HttpGet]
        public ActionResult BalanceSheet(string sortdir, string sort, int? page, int? rows, string filterstring, string currentFilter, string lblbreadcum, string PagingType, int? currentRowPerPage = 1000, string reference = null, DateTime? fromDate = null, DateTime? toDate = null)
        {
            try
            {

                if (Session["UserId"] == null || Session["PreviousPage"] == null || Session["currentPage"] == null || Session["Connection"] == null)
                {
                    return RedirectToAction("LogOut", "Home");

                }




                DataTable dt = new DataTable();
                string con = System.Configuration.ConfigurationManager.
                ConnectionStrings["ConnectionString"].ConnectionString;

                using (OracleConnection conn = new OracleConnection(con))
                {
                    OracleDataAdapter da = new OracleDataAdapter();
                    OracleCommand cmd = new OracleCommand();
                    cmd.Connection = conn;
                    cmd.CommandText = "GETBALANCESHEET";
                    cmd.CommandType = CommandType.StoredProcedure;
                    //cmd.Parameters.Add("FROMDATE", OracleType.DateTime).Value = DateTime.Parse(fromDate.ToString());
                    //cmd.Parameters.Add("TODATE", OracleType.DateTime).Value = DateTime.Parse(toDate.ToString());
                    cmd.Parameters.Add("PRC", OracleType.Cursor).Direction = ParameterDirection.Output;

                    da.SelectCommand = cmd;

                    da.Fill(dt);


                }




                List<BalanceSheet> models = new List<BalanceSheet>();
                models = oCommonFunction.ConvertDataTable<BalanceSheet>(dt);


                GridModel<BalanceSheet> gridModels = new GridModel<BalanceSheet>();


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


                //if (string.IsNullOrEmpty(filterstring))
                //    models = new Entities(Session["Connection"] as EntityConnection).APPLICATIONPARAMETERs.AsNoTracking().OrderBy(sort + " " + sortdir).Skip(skipcount).Take(gridModels.RowsPerPage).ToList();
                //else
                //    models = new Entities(Session["Connection"] as EntityConnection).APPLICATIONPARAMETERs.AsNoTracking().Where(w => w.ENTITY.Contains(filterstring)).OrderBy(sort + " " + sortdir).Skip(skipcount).Take(gridModels.RowsPerPage).ToList();






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
                return PartialView("BalanceSheet", gridModels);



            }

            catch (Exception ex)
            {
                string message = ex.Message;

                return RedirectToAction("Index", "ErrorPage", new { message });
            }

        }
    }
}

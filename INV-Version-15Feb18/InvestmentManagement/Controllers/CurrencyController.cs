using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using InvestmentManagement.Models;
using System.Linq.Dynamic;
using InvestmentManagement.InvestmentManagement.Models;
using System.Data.EntityClient;

namespace InvestmentManagement.Controllers
{
    public class CurrencyController : Controller
    {
        //
        // GET: /Currency/

        public ActionResult ListCurrency(string sortdir, string sort, int? page, int? rows, string filterstring, string lblbreadcum)
        {
            GridModel<CURRENCY> gridModel = new GridModel<CURRENCY>();


            List<CURRENCY> currencies = new Entities(Session["Connection"] as EntityConnection).CURRENCies.ToList();
            //if (page == null)
            //{
            //    SortDir = sortdir;
            //    SortCol = sort;
            //    RowsPerPageCount = rows;
            //}

            gridModel.CurrentPage = page == null ? 1 : (int)page;
            gridModel.RowsPerPage = rows == null ? 5 : (int)rows;
            gridModel.TotalPageNo = (int)Math.Ceiling((decimal)currencies.Count / gridModel.RowsPerPage);


            if (filterstring != null)
            {
                currencies = currencies.Where(w => w.CODE == filterstring).ToList();
                gridModel.TotalPageNo = (int)Math.Ceiling((decimal)currencies.Count / gridModel.RowsPerPage);
            }

            if (sortdir != null || sort != null)
            {
                if (sortdir == "DESC")
                {
                    IQueryable<CURRENCY> sampleSort = currencies.AsQueryable<CURRENCY>();
                    currencies = sampleSort.OrderBy(sort + " DESC").ToList();

                }
                else
                {
                    IQueryable<CURRENCY> sampleSort = currencies.AsQueryable<CURRENCY>();
                    currencies = sampleSort.OrderBy(sort).ToList();
                }
            }

            if (page != null)
            {
                currencies = currencies.Skip(gridModel.RowsPerPage * ((int)page - 1)).ToList();
            }

            gridModel.DataModel = currencies.Take(gridModel.RowsPerPage).ToList();

            if (lblbreadcum != null)
            {
                Session["currentPage"] = lblbreadcum;
            }



            if (Request.IsAjaxRequest())
            {
                return PartialView("ListCurrency", gridModel);

            }

            else
            {
                return View("ListCurrency", gridModel);

            }
       
        }

    }
}

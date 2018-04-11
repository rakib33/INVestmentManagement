using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using InvestmentManagement.Models;
using System.Linq.Dynamic;
using InvestmentManagement.InvestmentManagement.Models;
using System.IO;
using System.Xml.Linq;
using System.Data.EntityClient;


namespace InvestmentManagement.Controllers
{
    public class MarketPriceController : Controller
    {
        CommonFunction oCommonFunction = new CommonFunction();
        //
        // GET: /MarketPrice/
        List<PRICEINDEX> oPRICEINDEXes = new List<PRICEINDEX>();

        [HttpGet]
        public ActionResult ListMarketPrice(string sortdir, string sort, int? page, int? rows, string filterstring, string currentFilter, string lblbreadcum, string PagingType, int? currentRowPerPage = 1000, DateTime? tradingDate = null, string instrument = null)
        {
            try
            {
                if (Session["UserId"] == null || Session["PreviousPage"] == null || Session["currentPage"] == null || Session["Connection"] == null)
                {
                    return RedirectToAction("LogOut", "Home");
                }
                //currentRowPerPage=@ViewBag.currentRowPerPage
                GridModel<PRICEINDEX> gridModels = new GridModel<PRICEINDEX>();
                List<PRICEINDEX> models = null;

                List<INSTRUMENT> instrumentList = new List<INSTRUMENT>();
                instrumentList =new Entities(Session["Connection"] as EntityConnection).INSTRUMENTs.ToList().OrderBy(i => i.SHORTNAME).ToList();
                
                ViewBag.instrumentList = new SelectList(instrumentList, "SHORTNAME", "SHORTNAME");

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
                    models = new Entities(Session["Connection"] as EntityConnection).PRICEINDEXes.AsNoTracking().OrderBy(sort + " " + sortdir).Skip(skipcount).Take(gridModels.RowsPerPage).ToList();
                else
                    models = new Entities(Session["Connection"] as EntityConnection).PRICEINDEXes.AsNoTracking().Where(w => w.CREATEDBY.Contains(filterstring)).OrderBy(sort + " " + sortdir).Skip(skipcount).Take(gridModels.RowsPerPage).ToList();



                if (TempData["importDate"] != null)
                {
                    DateTime importDate = DateTime.Parse(TempData["importDate"].ToString());
                    models = models.Where(p => p.IMPORTDATE == importDate).ToList();
                }


                if (tradingDate != null)
                {
                    models = new Entities(Session["Connection"] as EntityConnection).PRICEINDEXes.AsNoTracking().Where(w => w.TRADINGDATE == tradingDate).OrderBy(sort + " " + sortdir).Skip(skipcount).Take(gridModels.RowsPerPage).ToList();

                   // models = models.Where(p => p.TRADINGDATE == tradingDate).ToList();
                }
                else if (TempData["importDate"] == null)
                {
                    models = models.Where(p => p.IMPORTDATE == DateTime.Now).ToList();
                }

                if (string.IsNullOrEmpty(instrument) != true)
                {
                    models = models.Where(p => p.INSTRUMENTREF == instrument).ToList();
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
                return PartialView("ListMarketPrice", gridModels);



            }

            catch (Exception ex)
            {
                string message = ex.Message;

                return RedirectToAction("Index", "ErrorPage", new { message });
            }



        }


        [HttpPost]
        public ActionResult ImportPriceIndex()
        {
            if (Session["UserId"] == null || Session["PreviousPage"] == null || Session["currentPage"] == null || Session["Connection"] == null)
            {
                //return RedirectToAction("LogOut", "Home");
                return new JsonResult { Data = "Session expired.Please SignIn/LogIn." };
            }
            int i = 0;
            string line;
            int flag = 0;
            Entities db = new Entities(Session["Connection"] as EntityConnection);
            foreach (var item in Request.Files)
            {
                HttpPostedFileBase file = Request.Files[i];
                var fileName = Path.GetFileName(file.FileName);
                StreamReader oPriceIndexes = new StreamReader(file.InputStream);
                DateTime tradeDate=DateTime.Now;
                try
                {
                    XElement XDoc = XElement.Load(oPriceIndexes);
                    string dateString = System.IO.Path.GetFileNameWithoutExtension(fileName).Substring(0, 8);
                    DateTime importDate = DateTime.ParseExact(dateString, "yyyyMMdd", null);
                    TempData["importDate"] = importDate.ToString();
                    int count = 0;
                    List<INSTRUMENTCATEGORY> categories = new List<INSTRUMENTCATEGORY>();
                    
                  
                    List<string> ISINList = new List<string>();

                    foreach (var ticker in XDoc.Descendants("Ticker"))
                    {
                        count++;
                       

                        DateTime priceDate = DateTime.ParseExact(dateString, "yyyyMMdd", null);
                        string isin = ticker.Attribute("ISIN").Value;
                        string shortName = ticker.Attribute("SecurityCode").Value;
                        string insCategory = ticker.Attribute("Category") == null ? string.Empty : ticker.Attribute("Category").Value;
                                               

                        double closingRate = double.Parse(ticker.Attribute("Close").Value);
                        //Newly added
                        double open = double.Parse(ticker.Attribute("Open") == null ? "0" : ticker.Attribute("Open").Value);
                        double high = double.Parse(ticker.Attribute("High") == null ? "0" : ticker.Attribute("High").Value);
                        double low = double.Parse(ticker.Attribute("Low") == null ? "0" : ticker.Attribute("Low").Value);
                        tradeDate = DateTime.ParseExact(ticker.Attribute("TradeDate").Value, "yyyyMMdd",null);
                        double varPercentage = double.Parse(ticker.Attribute("VarPercent") == null ? "0" : ticker.Attribute("VarPercent").Value);

                        bool isSpot = ticker.Attribute("CompulsorySpot").Value == "N" ? false : true;
                                               

                        PRICEINDEX oPriceIndex = new PRICEINDEX();
                        try
                        {
                            oPriceIndex.REFERENCE = Guid.NewGuid().ToString();
                            oPriceIndex.CREATEDDATE = DateTime.Now;
                            oPriceIndex.IMPORTDATE = priceDate;
                            oPriceIndex.TRADINGDATE = tradeDate;
                            oPriceIndex.ISIN = isin;
                            oPriceIndex.INSTRUMENTREF = shortName;
                            oPriceIndex.CLOSINGPRICE = Convert.ToDecimal(closingRate);
                            oPriceIndex.OPENINGPRICE = Convert.ToDecimal(open);
                            oPriceIndex.LOWESTPRICE = Convert.ToDecimal(low);
                            oPriceIndex.HIGHESTPRICE = Convert.ToDecimal(high);
                            oPriceIndex.VARIATION = Convert.ToDecimal(varPercentage);
                            //oPriceIndex.STATUS = "Active";
                            //ADD STATUS ACTIVE OR CLOSED(IF PREVIOUS RECORD FOUND) BECAUSE PORTFOLIO GET ONLY ONE ROW ACTIVE RECORD CLOSING PRICE 
                            //TO MARKET PRICE FROM  getMarketPrice_FN FUNCTION.
                            //try
                            //{
                            //    var hasPricerecord = db.PRICEINDEXes.Where(t => t.TRADINGDATE == tradeDate && t.INSTRUMENTREF == shortName && t.ISIN == isin).ToList();


                            //}
                            //catch { 
                            
                            //}
                       
                            // add all ISIN into this list
                            ISINList.Add(isin);


                            List<INSTRUMENT> oINSTRUMENTs = new Entities(Session["Connection"] as EntityConnection).INSTRUMENTs.ToList();
                            List<INSTRUMENTCATEGORY> oINSTRUMENTCATEGORies = new Entities(Session["Connection"] as EntityConnection).INSTRUMENTCATEGORies.ToList();

                            //sometims same ISIN but different short name such as CDBL has one name and DSE has another name of same ISIN 

                            INSTRUMENT oINSTRUMENT = oINSTRUMENTs.Where(ins => ins.ISIN == isin && ins.SHORTNAME == shortName).FirstOrDefault();                            
                            INSTRUMENTCATEGORY oINSTRUMENTCATEGORY = oINSTRUMENTCATEGORies.Where(cate => cate.CODE == insCategory).FirstOrDefault();

                          //  oPriceIndexList.Add(oPriceIndex);
                            db.PRICEINDEXes.Add(oPriceIndex);

                        }
                        catch (FormatException)
                        {
                            throw new Exception(shortName + " has invalid format.");
                        }
                    }

                    List<PRICEINDEX> CARList = new List<PRICEINDEX>();

                    //CARList = db.CORPORATEACTIONRECEIVABLEs.Where(ca => ca.RECORDDATE == RecordDate).ToList();

                    CARList = (from old_ca in ISINList.ToList()
                               join ca in db.PRICEINDEXes.ToList() on old_ca equals ca.ISIN
                               where ca.TRADINGDATE == tradeDate
                               select ca).ToList();

                    foreach (var car in CARList)
                    {
                        db.PRICEINDEXes.Remove(car);
                    }
                
                    db.SaveChanges();
                    
                }
                catch (Exception ex)
                {
                    //Exception should be catch(DbEntityValidationException dbEx)
                    //foreach (var validationErrors in dbEx.EntityValidationErrors)
                    //{
                    //    foreach (var validationError in validationErrors.ValidationErrors)
                    //    {
                    //        Trace.TraceInformation("Property: {0} Error: {1}", validationError.PropertyName, validationError.ErrorMessage);
                    //    }
                    //}
                    return new JsonResult { Data = ex.Message };
                }
            }


            
            return new JsonResult { Data = "Upload Successful" };
            //return RedirectToAction("ListMarketPrice", "MarketPrice");
        }


      
 
    }
}

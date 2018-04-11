using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using InvestmentManagement.InvestmentManagement.Models;
using InvestmentManagement.Models;
using Microsoft.Reporting.WebForms;
using InvestmentManagement.App_Code;
using System.IO;


namespace InvestmentManagement.Controllers
{
    public class SettlementController : Controller
    {
        
        Variable _var ;  //declare Instance
        CommonFunction oCommonFunction = new CommonFunction();

        public ActionResult ListPayInPayOut(string sortdir, string sort, int? page, int? rows, string filterstring, string lblbreadcum, DateTime? BusinessDate, string PayIn, string PayOut, int? currentRowPerPage = 10000)
        {
                        
            try
            {
                if (oCommonFunction.CheckSession() == true)
                {
                    return RedirectToAction("LogOut", "Home");
                }


                //currentRowPerPage=@ViewBag.currentRowPerPage
                GridModel<SettlementViewModel> gridModels = new GridModel<SettlementViewModel>();
                List<SettlementViewModel> models = null;

                //grid settings                
                gridModels.RowsPerPage = rows == null ? (int)currentRowPerPage : (int)rows;
                ViewBag.currentRowPerPage = gridModels.RowsPerPage;
                
                _var = new Variable();

                //if PayIn="true" then BusinessDate is the transaction date of Trade table.
                //If PayOut="true" then BusinessDate will be the 

                if (PayIn == "true")
                    _var.payOption = ConstantVariable.SettlementPayIn;
                else if (PayOut == "true")
                    _var.payOption = ConstantVariable.SettlementPayOut;
                else
                {
                    _var.payOption = ConstantVariable.SettlementPayIn;
                    ViewBag.PayIn = "checked";
                }

                if (_var.payOption != null && !string.IsNullOrEmpty(_var.payOption))
                {
                    models = new Portfolio().GetSattlemetPayInPayOut(BusinessDate.HasValue ? BusinessDate.Value.ToString("dd-MMM-yy") : DateTime.Now.ToString("dd-MMM-yy"), _var.payOption);
                    gridModels.DataModel = models;

                    ViewBag.Message = oCommonFunction.MessageForView(this.ControllerContext.RouteData.Values["action"].ToString());
                    ViewBag.Message = oCommonFunction.MessageForView(this.ControllerContext.RouteData.Values["action"].ToString());

                }
                else
                {
                    models = new List<SettlementViewModel>();
                    ViewBag.Message = "Please select PayIn or PayOut option.";
                }
                             
                if (!string.IsNullOrEmpty(lblbreadcum))
                {
                    Session["currentPage"] = lblbreadcum;
                }

                Session["Path"] = oCommonFunction.GetPath(this.ControllerContext.RouteData.Values["controller"].ToString(), Session["currentPage"].ToString(), this.ControllerContext.RouteData.Values["action"].ToString());

                ViewBag.BreadCum = oCommonFunction.GetListPath(Session["currentPage"].ToString(), this.ControllerContext.RouteData.Values["controller"].ToString());

                return PartialView("ListPayInPayOut", gridModels);

            }

            catch (Exception ex)
            {
                string message = ex.Message;

                return RedirectToAction("Index", "ErrorPage", new { message });
            }
        }
                
        public ActionResult GenarateFile(DateTime? BusinessDate, string PayIn, string PayOut,string filePath)
        {
            
            if ((PayIn ==null && string.IsNullOrEmpty(PayIn)) && (PayOut ==null && string.IsNullOrEmpty(PayOut)))
            {
                return RedirectToAction("Index", "ErrorPage", new { message = "Please Select a PayIn or Pay Out option." });
            }
            if (BusinessDate == null)
            {
            return RedirectToAction("Index", "ErrorPage", new { message = "Please Select a Business Date." }); 
            }

            _var = new Variable(); 
            bool success = true;
            _var.fileName="";                      
                   
            List<SettlementViewModel> models = null;
            
            //if PayIn="true" then BusinessDate is the transaction date of Trade table.
            //If PayOut="true" then BusinessDate will be the 

            if (PayIn == "PayIn")
            {             
              _var.payOption = ConstantVariable.SettlementPayIn;             
            }
            else if (PayOut == "PayOut")
            {               
               _var.payOption = ConstantVariable.SettlementPayOut;             
            }
            else
            {
                _var.message = "Please select a PayIn or PayOut option.";           
            }           

            if (_var.payOption != null && !string.IsNullOrEmpty(_var.payOption))
            {
                if (BusinessDate.HasValue)
                {
                    filePath = Server.MapPath("~/Reports/ShareReport/PayInPayOut.txt");
                     models = new Portfolio().GetSattlemetPayInPayOut(BusinessDate.Value.ToString("dd-MMM-yy"), _var.payOption);
                    _var.result = new PayInOutFileGenarator().PayInOutFileWrite(filePath, _var.payOption, models, BusinessDate.Value.ToString("ddMMyyyy"));

                    string[] result = _var.result.Split(',');

                    if (result[0] == ConstantVariable.Settlement_Success_msg)
                    {
                        _var.message = "File Successfully created.";
                        //now throw the file                                              
                        string fileName=result[1].ToString();   
                        return File(filePath, "text/xml",fileName );
                       
                    }
                    else
                        _var.message = result[0];
                }
                else
                    _var.message = "Select a Business Date.";
            }

            //using (StreamWriter w = new StreamWriter(Server.MapPath("~/Reports/ShareReport/SellLimitFile.txt"), true))
            //{
            //    w.WriteLine("This Is a Dummy Text"); // Write the text
            //    w.WriteLine("This is secound line of text");
            //    w.Close();
            //    w.Dispose();
            //    string filePath = Path.Combine(Server.MapPath("~/Reports/ShareReport/SellLimitFile.txt"));
            //    return File(filePath, "text/xml","NewFile.txt");                
            //}

            return Json(new { success, _var.message }, JsonRequestBehavior.AllowGet);           
        }
    }
}

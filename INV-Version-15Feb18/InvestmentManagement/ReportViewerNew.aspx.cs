using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OracleClient;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using InvestmentManagement.InvestmentManagement.Models;
using InvestmentManagement.Models;
using Microsoft.Reporting.WebForms;

namespace InvestmentManagement
{
    public partial class ReportViewerNew : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
       
          //CommonFunction oCommonFunction = new CommonFunction();

           

          //  List<PortfolioInstrument> models = new List<PortfolioInstrument>();
          //  ReportDataSource oPortfolioStatement = new ReportDataSource();
            
          //  models = new Portfolio().GetInvestorPortfolio("12-MAY-15");
          //  //LocalReport lr = new LocalReport();
          //  lr.ReportPath = Server.MapPath("~/Reports/Portfolio.rdlc");

          //  ReportDataSource rd = new ReportDataSource();
          //  //ReportDataSource dd = new ReportDataSource();

          //  DataTable dtPortfolioStatement = oCommonFunction.ConvertToDataTable(models);
          //  rd.Name = "dsPortfolioInstrument";
          //  rd.Value = dtPortfolioStatement;


          //  ReportParameter[] portfolioParameter = new ReportParameter[] 
          //  {
          //   new ReportParameter("CompanyName","DLIC"),
          //   new ReportParameter("Address","Gulshan-2, Dhaka"),
          //    new ReportParameter("ReportTitle","Portfolio Statement"),
          //    new ReportParameter("PortfolioDate","")
          //  };

          //  lr.SetParameters(portfolioParameter);
          //  lr.DataSources.Add(rd);
          //  lr.SubreportProcessing += new SubreportProcessingEventHandler(localReport_SubreportProcessing);
          

          //  string reportType = "PDF";
          //  string mimeType;
          //  string encoding;
          //  string fileNameExtension;

          //  string deviceInfo =

          //  "<DeviceInfo>" +
          //      "  <OutputFormat>PDF</OutputFormat>" +
          //      "  <PageWidth>8.5in</PageWidth>" +
          //      "  <PageHeight>11in</PageHeight>" +
          //      "  <MarginTop>0.5in</MarginTop>" +
          //      "  <MarginLeft>1in</MarginLeft>" +
          //      "  <MarginRight>1in</MarginRight>" +
          //      "  <MarginBottom>0.5in</MarginBottom>" +
          //      "</DeviceInfo>";

          //  Warning[] warnings;
          //  string[] streams;
          //  byte[] renderedBytes;

          //  renderedBytes = lr.Render(
          //      reportType,
          //      deviceInfo,
          //      out mimeType,
          //      out encoding,
          //      out fileNameExtension,
          //      out streams,
          //      out warnings);

          //  ReportViewer.LocalReport.DataSources.Add(lr);
 
        }

        //private DataTable GetBonusReceivable()
        //{
        //    string con = "User ID=HR;Password=tiger;Data Source=192.168.0.5:1522/xe;";
        //    using (OracleConnection conn = new OracleConnection(con))
        //    {
        //        OracleDataAdapter da = new OracleDataAdapter();
        //        OracleCommand cmd = new OracleCommand();
        //        cmd.Connection = conn;
        //        cmd.CommandText = "GETPORTFOLIOCARECEIVABLE";
        //        cmd.CommandType = CommandType.StoredProcedure;
        //        cmd.Parameters.Add("Effective_DATE", OracleType.DateTime).Value = DateTime.Parse("10-MAY-2015");
        //        cmd.Parameters.Add("prc", OracleType.Cursor).Direction = ParameterDirection.Output;

        //        da.SelectCommand = cmd;
        //        DataTable dt = new DataTable();
        //        da.Fill(dt);

        //        return dt;
        //    }
        //}

        //void localReport_SubreportProcessing(object sender, SubreportProcessingEventArgs e)
        //{
        //    e.DataSources.Add(new ReportDataSource("IVMDataSetSource", GetBonusReceivable()));
        //}

          
        }
    }

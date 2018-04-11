using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OracleClient;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using InvestmentManagement.InvestmentManagement.Models;
using InvestmentManagement.Models;
using Microsoft.Reporting.WebForms;

namespace InvestmentManagement
{
    public partial class ReportViewer : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                CommonFunction oCommonFunction = new CommonFunction();
                
                ReportViewer1.Reset();
                ReportDataSource rd = new ReportDataSource();

                List<PortfolioInstrument> models = new List<PortfolioInstrument>();
                ReportDataSource oPortfolioStatement = new ReportDataSource();

                models = new Portfolio().GetInvestorPortfolio("12-MAY-15",null,null);
                //LocalReport lr = new LocalReport();

                //lr.ReportPath = Server.MapPath("~/Reports/Portfolio.rdlc");


                //ReportDataSource dd = new ReportDataSource();

                DataTable dtPortfolioStatement = oCommonFunction.ConvertToDataTable(models);

                DataSet ds = new DataSet();
                ds.Tables.Add(dtPortfolioStatement);

                rd.Name = "dsPortfolioInstrument";
                rd.Value = dtPortfolioStatement;

                //  portfolioParameter blocked by rakibul date 9-3-2016
                ReportParameter[] portfolioParameter = new ReportParameter[] 
                    {
                    new ReportParameter("CompanyName","DLIC"),
                    new ReportParameter("Address","Gulshan-2, Dhaka"),
                    new ReportParameter("ReportTitle","Portfolio Statement"),
                    new ReportParameter("PortfolioDate","")
                 };

                //lr.SetParameters(portfolioParameter);
                //lr.DataSources.Add(rd);

                //lr.SubreportProcessing += new SubreportProcessingEventHandler(localReport_SubreportProcessing);


                //string reportType = "PDF";
                //string mimeType;
                //string encoding;
                //string fileNameExtension;

                //string deviceInfo =

                //"<DeviceInfo>" +
                //    "  <OutputFormat>PDF</OutputFormat>" +
                //    "  <PageWidth>8.5in</PageWidth>" +
                //    "  <PageHeight>11in</PageHeight>" +
                //    "  <MarginTop>0.5in</MarginTop>" +
                //    "  <MarginLeft>1in</MarginLeft>" +
                //    "  <MarginRight>1in</MarginRight>" +
                //    "  <MarginBottom>0.5in</MarginBottom>" +
                //    "</DeviceInfo>";

                //Warning[] warnings;
                //string[] streams;
                //byte[] renderedBytes;

                //renderedBytes = lr.Render(
                //    reportType,
                //    deviceInfo,
                //    out mimeType,
                //    out encoding,
                //    out fileNameExtension,
                //    out streams,
                //    out warnings);
                ReportViewer1.LocalReport.DataSources.Clear();
                ReportViewer1.ProcessingMode = ProcessingMode.Local;
                ReportViewer1.LocalReport.ReportPath = Server.MapPath("~/Reports/Portfolio.rdlc");
               
                 ReportViewer1.LocalReport.SetParameters(portfolioParameter); 
                //blocked by rakibul date 9-3-2016
               
                
                //var ds1 = ReportViewer1.LocalReport.GetDataSourceNames();
                //ReportViewer1.LocalReport.DataSources.Add(new ReportDataSource(ds1[0], ds.Tables[0]));
                //ReportViewer1.LocalReport.Refresh();
                ReportViewer1.LocalReport.DataSources.Add(new Microsoft.Reporting.WebForms.ReportDataSource("dsPortfolioInstrument", ds.Tables[0]));
                //ReportViewer1.Visible = true;
               // ReportViewer1.LocalReport.Refresh();
               ReportViewer1.LocalReport.SubreportProcessing += new SubreportProcessingEventHandler(localReport_SubreportProcessing);

                Warning[] warnings;
                string[] streamIds;
                string mimeType = ""; // string.Empty;
                string encoding = ""; // string.Empty;
                string extension = ""; // string.Empty;


                // Setup the report viewer object and get the array of bytes

                try
                {

                    byte[] bytes = ReportViewer1.LocalReport.Render("PDF", null, out mimeType, out encoding, out extension, out streamIds, out warnings);
               

                // Now that you have all the bytes representing the PDF report, buffer it and send it to the client.
                Response.Buffer = true;
                Response.Clear();
                Response.ContentType = mimeType;
                Response.AddHeader("content-disposition", "attachment; filename=" + "dasdsad" + "." + extension);
                Response.BinaryWrite(bytes); // create the file
                Response.Flush(); // send it to the cli
                }
                catch (Exception ex)
                {
                    string m = ex.Message + "  " + ex.InnerException.Message;
                }

            }

            

        }


        private DataTable GetBonusReceivable()
        {
           //this is hard coded it should come from webconfig added by rakibul date<9-3-2016>
           // string con = "User ID=HR;Password=tiger;Data Source=192.168.0.5:1522/xe;";

            string con = "User ID=HR;Password=tiger;Data Source=192.168.0.9:1521/xe;";
            using (OracleConnection conn = new OracleConnection(con))
            {
                OracleDataAdapter da = new OracleDataAdapter();
                OracleCommand cmd = new OracleCommand();
                cmd.Connection = conn;
                cmd.CommandText = "GETPORTFOLIOCARECEIVABLE";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add("Effective_DATE", OracleType.DateTime).Value = DateTime.Parse("10-MAY-2015");
                cmd.Parameters.Add("prc", OracleType.Cursor).Direction = ParameterDirection.Output;

                da.SelectCommand = cmd;
                DataTable dt = new DataTable();

                try
                {
                    da.Fill(dt);
                }
                catch(Exception ex) {
                    string exp = ex.Message;
                }
                return dt;
            }
        }

       private  void localReport_SubreportProcessing(object sender, SubreportProcessingEventArgs e)
        {
            e.DataSources.Add(new ReportDataSource("IVMDataSetSource", GetBonusReceivable()));
        }

    }
}
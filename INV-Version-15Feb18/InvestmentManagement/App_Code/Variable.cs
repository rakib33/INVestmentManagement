using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace InvestmentManagement.App_Code
{
    public class Variable
    {

        public decimal? tenure { get; set; }
        public decimal? days { get; set; }
        public string terms { get; set; }
        public string InterestMode { get; set; }

        public string GrossInteresStr { get; set; }
        public string NetInterestStr { get; set; }
        public string SourceTaxStr { get; set; }
        public int Interval { get; set; }
        
        public decimal? RateOfInterest { get; set; }

        public decimal? RateOfInterest_OneMonth { get; set; }

        public decimal? GrossInterest { get; set; }
        public decimal? NetInterestReceivable { get; set; }
        public decimal? TotalAmountReceivable { get; set; }
        public decimal? SourceTax { get; set; }
        public decimal? Principal { get; set; }
        public decimal? GrossDeduction { get; set; }
        public decimal? ExciseDuties { get; set; }
        public decimal? OthersCharge { get; set; }
        
        //Current Holding or Existing Deposit
        public decimal? ExistingDeposit { get; set;}


        public decimal? FinalAmountReceivable { get; set; }
        public int CompoundInterval { get; set; }

        public decimal? taxRate { get; set; }

        public string PKreference { get; set; }
        public string FKreference { get; set; }


        public string UserId { get; set; }

        public string Paragraph1 { get; set; }
        public string Paragraph2 { get; set; }
        public string Paragraph3 { get; set; }

        #region DateTimeVariable
        public int Year { get; set; }
        public int Month { get; set; }
        public int Day { get; set; }
        #endregion

        public string ErrorMessage { get; set; }

        public int Flag { get; set; }

        public int NumberCount { get; set; }

        public string payOption { get; set; }
        public string firstCode { get; set; }
        public string result { get; set; }
        public string message { get; set; }
        public string fileName { get; set; }
        public string PathString { get; set; }
        public string CdblDPId { get; set; }
        public string tagChar { get; set; }
        public string DSEExchange { get; set; }
        public string ShortName { get; set; }
        public double MarketPrice { get; set; }
        public double Total { get; set; }
        public double MaturedBalance { get; set; }
        public int TotalRow { get; set; }
    }
}
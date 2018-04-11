using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace InvestmentManagement
{
    public class PortfolioInstrument
    {
        public string CheckInstrument { get; set; }
        public decimal SellAmount { get; set; }


        public string AccountNumber { get; set; }    
        public string Instrument { get; set; }
        public double NetBalance { get; set; }
        public double MaturedBalance { get; set; }
        public double AverageCost { get; set; }
        public double TotalCost { get; set; }
        public double MarketRate { get; set; }
        public double MarketValue { get; set; }
        public double UnrealizedGain { get; set; }
        public double RealizedGain { get; set; }
        public double PERatio { get; set; }
        public double PercentageGain { get; set; }
    }


    public class SellLimit
    { 
     public string AccountNumber {get; set;}
     public string ShortName {get; set;}
     public string ISIN {get; set;}
     public double MaturedBalance {get; set;}
     public double TotalCost { get; set; }
     public string SellLimitCostvalue { get; set; }
     public string SellimitBalance { get; set; }
    
    }

    public class TradeOrderViewModel
    {
        public DateTime? TranDate { get; set; }
        public string TransactionType { get; set; }

        public string AccountNumber { get; set; }
        public string Instrument { get; set; }
     
        public decimal? Qty { get; set; }
        public decimal? ClosingPrice { get; set; }
        public decimal? Total { get; set; }

        public string LowerLimit { get; set; }
        public string UpperLimit { get; set; }
        public string MaximumQty { get; set; }
        public string FundPosition { get; set; }
        //for Buy Net=NetBuyAmount + Commision for Sell Net=NetBuyAmount + Commision

    }

    public class BuySellstatement
    {
        public DateTime? TranDate { get; set; }
        public string TransactionType { get; set; }
        public string AccountNumber { get; set; }
        public string TradeCode { get; set; }
        public double BuyQty { get; set; }
        public double BuyPrice { get; set; }
        public double BuyAmount { get; set; }
        public double SellQty { get; set; }
        public double SellPrice { get; set; }
        public double SellAmount { get; set; }
        public double Commission { get; set; }
        public double CommissionPercentage { get; set; }
        public double Gross { get; set; }
        public double Net { get; set; }
        public double NetProfitLoss { get; set; }
     
       
    }

}
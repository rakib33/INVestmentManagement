using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace InvestmentManagement.ViewModel
{
    public class StockCashReceivable
    {
        //public string INSTRUMENTREF { get; set; }
        public string ISIN { get; set; }
        public string ShortName { get; set; }
        public string Catype { get; set; }

        public decimal? AmtBonus { get; set; }
        public decimal? Amtdividend { get; set; }
        public decimal? Holding { get; set; }
        public decimal? BShareReceivable { get; set; } //BonusShareReceivable
        public decimal? RShareReceivable { get; set; }
        public decimal? MarketPrice { get; set; }
        public decimal? BonusMarketValue { get; set; }
        public decimal? CashDividendReceivable { get; set; }          
  
        public DateTime? EffectiveDate { get; set; }
        public DateTime? RecordDate { get; set; }
        public string Declaration { get; set; }
        public string Remarks { get; set; }
    }
}
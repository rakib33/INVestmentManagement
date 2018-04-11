using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace InvestmentManagement.InvestmentManagement.Models
{
    public class PortfolioBonus
    {        
        public string ISIN { get; set; }
        public string BOID { get; set; }
        public string Catype { get; set; }
        public string Investoracref { get; set; }
        public string InstrumentName { get; set; }
        public string Entitlement { get; set; }
        public string EffectiveDate { get; set; }
        public string RecordDate { get; set; }
        public string ShortName { get; set; }
        public string LastMarketPrice { get; set; }
        public string ParHolding { get; set; }
        public string MarketValue { get; set; }

    }


    public class DividendBonus 
    {
        public string ISIN { get; set; }    
        public string CATYPE { get; set; }
        public string SHORTNAME { get; set; }
        public string CASHPAYMENTDATE { get; set; }
        public string RECORDDATE { get; set; }
        public decimal BOHOLDING { get; set; }
        public decimal NETCASHAMOUNT { get; set; }
      
    }
}
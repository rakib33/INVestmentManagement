using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace InvestmentManagement.InvestmentManagement.Models
{
    public class SettlementViewModel
    {

        public DateTime     BusinessDate { get; set; }
        public string       transactiontype { get; set; }
        public string       investoracref { get; set; }
        public string       bonumber { get; set; }  //from Investor 

        public string       CdblDpId { get; set; }  // From broker
        public string       dseclearingbo { get; set; } //from broker
        public string       TradeNumber { get; set; }  // broker member Id
        public string       dseexchangeid { get; set; } //broker
     

        public string       Name { get; set; }
        public string       shortname { get; set; }
        public string       isin { get; set; }
        
        public double       ShareQuantity { get; set; }

    }
}
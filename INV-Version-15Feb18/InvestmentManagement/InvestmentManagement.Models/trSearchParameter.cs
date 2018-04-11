using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace InvestmentManagement.InvestmentManagement.Models
{
    public class trSearchParameter
    {
        public DateTime? tradingDate { get; set; }
        public string stockExchange { get; set; }
        public string broker { get; set; }
        public string instrument { get; set; }
        public string investorAccount { get; set; }
        public bool sale { get; set; }
        public bool buy { get; set; }
    }
}
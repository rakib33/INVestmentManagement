using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace InvestmentManagement.InvestmentManagement.Models
{
    public class ShareCrossCheck
    {
        public string ISIN { get; set; }
        public string ShortName { get; set; }
        public double? FreeBalanceDiff { get; set; }
        public string ErrorMessage { get; set; }

    }
}
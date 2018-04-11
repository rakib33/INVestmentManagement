using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace InvestmentManagement.InvestmentManagement.Models
{
    public class StockISIN
    {
        public string ISIN { get; set; }
        public string BOID { get; set; }
        public DateTime RecordDate { get; set; }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace InvestmentManagement.InvestmentManagement.Models
{
    public class Holiday :EntityBase
    {
        public DateTime DayOff { get; set; }
        public string Note { get; set; }
        public string EnteredBy { get; set; }
        public string UpdatedBy { get; set; }
        public string Status { get; set; }
        public bool IsActive { get; set; }
    }
}
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace InvestmentManagement.InvestmentManagement.Models
{
    public class ChequeDrawnViewModel
    {
        public string CHEQUEDRAWNFROM { get; set; }
        public DateTime? CHEQUEDATE { get; set; } //day-month-year
        public string CHEQUENO { get; set; }
        public Nullable<decimal> CHEQUEAMOUNT { get; set; }
        public string PROPOSEDACTION { get; set; }  
              
    }

    public class ChequeViewModel
    {
        [DataType(DataType.DateTime)]
        public virtual DateTime? CHEQUEDATE { get; set; }
        public List<ChequeDrawnViewModel> ChequeDrawnList { get; set; }
    }
}
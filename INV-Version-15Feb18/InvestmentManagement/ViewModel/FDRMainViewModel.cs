using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace InvestmentManagement.ViewModel
{
    public class FDRMainViewModel
    {
        public string SlNo { get; set; }
        public string FDRNo { get; set; }
        public string BankName { get; set; }
        public DateTime? OpeningDate { get; set; } //string
        public decimal? PrincipalAmount { get; set; }
        public decimal? BankWisePA { get; set; }
        public DateTime? RenewedDate { get; set; }  //string
        public string Period { get; set; }
        public DateTime? MaturityDate { get; set; }  //string
        public decimal? HoldingPeriod { get; set; }
        public decimal? PresentPrincipalAmount { get; set; }
        public decimal? BankWisePPA { get; set; }
        public decimal? ExistingCAPLimit { get; set; }
        public decimal? PresentRateOfInterest { get; set; }
        public decimal? ReceivableTill { get; set; }
        public decimal? ReceivableUp { get; set; }
        public string Remarks { get; set; }

        //For A/C
        public decimal? GrossInterest { get; set; }
        public decimal? SourceTax { get; set; }
        public decimal? ExciseDuty { get; set; }
        public decimal? OtherCharges { get; set; }
        public decimal? NetInterest { get; set; }

    }
}
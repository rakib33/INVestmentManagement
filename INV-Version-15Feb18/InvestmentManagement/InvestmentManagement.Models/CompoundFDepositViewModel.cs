using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace InvestmentManagement.InvestmentManagement.Models
{
    public class CompoundFDepositViewModel
    {
        //Guid
        public string REFERENCE { get; set; }
        public string FixedDepositReference { get; set; }

        public Nullable<System.DateTime> OpeningDate { get; set; }
        public Nullable<System.DateTime> MaturedDate { get; set; }

        public int Interval { get; set; } //1st 2nd 3rd
        
        public string InterestMode { get; set; } //Flat ,Compound
        public string CompoundInterestInterval { get; set; }   //Quarterly/Yearly/HalfYearly/Monthly

        public Nullable<decimal> GrossInterestPerInterval { get; set; }
        public Nullable<decimal> SourceTaxPerInterval { get; set; }

        public Nullable<decimal> NetInterestReceivable { get; set; }
        public Nullable<decimal> AmountReceivablePerInterval { get; set; }
        
        public Nullable<decimal> ExciseDuty { get; set; }
        public Nullable<decimal> OtherCharge { get; set; }

        public Nullable<decimal> FinalAmountReceivable { get; set; }  //TotalAmountReceivable

    }

    public class FDRCalculationValue
    {

        public decimal? SumGrossInterest { get; set; }
        public decimal? SumSourceTax { get; set; }
        public decimal? SumNetInterest { get; set; }
        public decimal? LastReceivableAmount { get; set; }

        public List<FDRINTEREST> InterestList { get; set; }

    }
}
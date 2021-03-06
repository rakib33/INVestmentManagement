//------------------------------------------------------------------------------
// <auto-generated>
//    This code was generated from a template.
//
//    Manual changes to this file may cause unexpected behavior in your application.
//    Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace InvestmentManagement.InvestmentManagement.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class FIXEDDEPOSIT
    {
        public FIXEDDEPOSIT()
        {
            this.CHEQUEDRAWNs = new HashSet<CHEQUEDRAWN>();
            this.FDRINTERESTs = new HashSet<FDRINTEREST>();
            this.FDRNOTEs = new HashSet<FDRNOTE>();
        }
    
        public string REFERENCE { get; set; }
        public Nullable<System.DateTime> CREATEDDATE { get; set; }
        public string CREATEDBY { get; set; }
        public Nullable<System.DateTime> LASTUPDATED { get; set; }
        public string LASTUPDATEDBY { get; set; }
        public string DEPOSITNUMBER { get; set; }
        public string FINANCIALINSTITUTION_REFERENCE { get; set; }
        public Nullable<decimal> PRINCIPALAMOUNT { get; set; }
        public Nullable<System.DateTime> CHEQUEDATE { get; set; }
        public string CHEQUEREFERENCE { get; set; }
        public Nullable<decimal> TENURE { get; set; }
        public string TENURETERM { get; set; }
        public Nullable<decimal> TERMSINDAYS { get; set; }
        public string INTERESTRECEIVINGPERIOD { get; set; }
        public Nullable<System.DateTime> MATURITYDATE { get; set; }
        public Nullable<decimal> EXISTINGCAPLIMIT { get; set; }
        public Nullable<decimal> RATEOFINTEREST { get; set; }
        public Nullable<decimal> ADVANCEDINTERESTRATE { get; set; }
        public string INTERESTMODE { get; set; }
        public string COMPOUNDINTERESTTYPE { get; set; }
        public string COMPOUNDINTERESTINTERVAL { get; set; }
        public Nullable<decimal> ANNUALDAYS { get; set; }
        public string STATUS { get; set; }
        public string ACCEPTEDBY { get; set; }
        public Nullable<System.DateTime> ACCEPTEDDATE { get; set; }
        public string REJECTEDBY { get; set; }
        public Nullable<System.DateTime> REJECTEDDATE { get; set; }
        public Nullable<System.DateTime> OPENINGDATE { get; set; }
        public Nullable<System.DateTime> RENWALDATE { get; set; }
        public string RENEWALDEPOSITNUMBER { get; set; }
        public string TAXDEDUCTIONCRITERIA { get; set; }
        public Nullable<decimal> HOLDINGPERIOD { get; set; }
        public Nullable<decimal> GROSSINTEREST { get; set; }
        public Nullable<decimal> SOURCETAX { get; set; }
        public Nullable<decimal> EXCISEDUTY { get; set; }
        public Nullable<decimal> OTHERCHARGE { get; set; }
        public Nullable<decimal> PRESENTPRINCIPALAMOUNT { get; set; }
        public string REMARKS { get; set; }
        public Nullable<decimal> NETINTERESTRECEIVABLE { get; set; }
        public Nullable<decimal> TOTALAMOUNTRECEIVABLE { get; set; }
        public Nullable<decimal> RECEIVABLETILL { get; set; }
        public string MRNO { get; set; }
        public Nullable<System.DateTime> MRDATE { get; set; }
        public Nullable<System.DateTime> ENCASHMENTDATE { get; set; }
        public Nullable<decimal> ACTUALINTERESTRECEIVED { get; set; }
        public string BRANCH_REFERENCE { get; set; }
        public string SIGNATORY1 { get; set; }
        public string SIGNATORY2 { get; set; }
        public Nullable<decimal> TAXRATE { get; set; }
        public decimal INITIALPRINCIPALAMOUNT { get; set; }
        public Nullable<System.DateTime> INITIALOPENINGDATE { get; set; }
        public string INITIALFIXEDDEPOSITREF { get; set; }
        public string PROPOSEDACTION { get; set; }
        public string DESCRIPTION { get; set; }
        public string POSTGL_STATUS { get; set; }
        public string POSTGL_FROM { get; set; }
    
        public virtual ICollection<CHEQUEDRAWN> CHEQUEDRAWNs { get; set; }
        public virtual ICollection<FDRINTEREST> FDRINTERESTs { get; set; }
        public virtual ICollection<FDRNOTE> FDRNOTEs { get; set; }
        public virtual FIBRANCH FIBRANCH { get; set; }
        public virtual FINANCIALINSTITUTION FINANCIALINSTITUTION { get; set; }
    }
}

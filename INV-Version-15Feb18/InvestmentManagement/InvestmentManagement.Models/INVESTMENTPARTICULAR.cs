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
    
    public partial class INVESTMENTPARTICULAR
    {
        public INVESTMENTPARTICULAR()
        {
            this.INVPARTICULARSDETAILS = new HashSet<INVPARTICULARSDETAIL>();
        }
    
        public string REFERENCE { get; set; }
        public Nullable<System.DateTime> CREATEDDATE { get; set; }
        public string CREATEDBY { get; set; }
        public Nullable<System.DateTime> LASTUPDATED { get; set; }
        public string LASTUPDATEDBY { get; set; }
        public string CODE { get; set; }
        public string PARTICULARSNAME { get; set; }
        public string ISACTIVE { get; set; }
        public string REMARKS { get; set; }
        public Nullable<decimal> SEQUENCE { get; set; }
        public string PARENTID { get; set; }
        public string GROUPNAME { get; set; }
        public string SUBGROUPNAME { get; set; }
        public decimal LIMITMINIMUM { get; set; }
    
        public virtual ICollection<INVPARTICULARSDETAIL> INVPARTICULARSDETAILS { get; set; }
    }
}
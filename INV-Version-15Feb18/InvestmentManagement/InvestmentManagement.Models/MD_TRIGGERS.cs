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
    
    public partial class MD_TRIGGERS
    {
        public decimal ID { get; set; }
        public decimal TABLE_OR_VIEW_ID_FK { get; set; }
        public string TRIGGER_ON_FLAG { get; set; }
        public string TRIGGER_NAME { get; set; }
        public string TRIGGER_TIMING { get; set; }
        public string TRIGGER_OPERATION { get; set; }
        public string TRIGGER_EVENT { get; set; }
        public string NATIVE_SQL { get; set; }
        public string NATIVE_KEY { get; set; }
        public string LANGUAGE { get; set; }
        public string COMMENTS { get; set; }
        public Nullable<decimal> LINECOUNT { get; set; }
        public decimal SECURITY_GROUP_ID { get; set; }
        public System.DateTime CREATED_ON { get; set; }
        public string CREATED_BY { get; set; }
        public Nullable<System.DateTime> LAST_UPDATED_ON { get; set; }
        public string LAST_UPDATED_BY { get; set; }
    }
}

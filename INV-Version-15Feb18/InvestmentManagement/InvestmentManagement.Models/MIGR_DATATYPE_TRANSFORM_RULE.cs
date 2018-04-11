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
    
    public partial class MIGR_DATATYPE_TRANSFORM_RULE
    {
        public decimal ID { get; set; }
        public decimal MAP_ID_FK { get; set; }
        public string SOURCE_DATA_TYPE_NAME { get; set; }
        public Nullable<decimal> SOURCE_PRECISION { get; set; }
        public Nullable<decimal> SOURCE_SCALE { get; set; }
        public string TARGET_DATA_TYPE_NAME { get; set; }
        public Nullable<decimal> TARGET_PRECISION { get; set; }
        public Nullable<decimal> TARGET_SCALE { get; set; }
        public decimal SECURITY_GROUP_ID { get; set; }
        public System.DateTime CREATED_ON { get; set; }
        public string CREATED_BY { get; set; }
        public Nullable<System.DateTime> LAST_UPDATED_ON { get; set; }
        public string LAST_UPDATED_BY { get; set; }
    
        public virtual MIGR_DATATYPE_TRANSFORM_MAP MIGR_DATATYPE_TRANSFORM_MAP { get; set; }
    }
}
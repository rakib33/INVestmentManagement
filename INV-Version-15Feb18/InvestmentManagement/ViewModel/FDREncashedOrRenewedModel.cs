using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace InvestmentManagement.ViewModel
{
    public class FDREncashedOrRenewedModel
    {

      public string    FDRNO                        {get;set;}
      public string    NameAddressOfBank            {get;set;}
      public DateTime  OpeningDate                  {get;set;}
      public decimal   PrincipalAmount              {get;set;}
      public DateTime  MaturityDate                 {get;set;}
      public Nullable<DateTime>  EncashDate         {get;set;}
      public decimal   GrossInterest                {get;set;}
      public decimal   SourceTaxTK                  {get;set;}
      public decimal   ExciseDuty                   {get;set;}
      public decimal   POtherCharges                {get;set;}
      public decimal   NetInterest                  {get;set;}
      public decimal   TotalPrincipalAndInterest    {get;set;}
      public decimal?  ReceivableGross              {get;set;}
      public decimal   Gross                        {get;set;}
      public decimal   Tax                          {get;set;}
      public decimal?  ED                           {get;set;}
      public decimal?  PO                           {get;set;}      
      public decimal   Net                          {get;set;}
      public string    Remarks                      {get;set;}
      public DateTime? RenewedDate                  {get;set;}
      public DateTime? InitialOpeningDate           {get;set;}
      public decimal InitialPrincipalAmount         {get;set;}

    }

    public class YearlyInterestSlot {

        public decimal ReceivableGross { get; set; }
        public decimal Gross { get; set; }
        public decimal SourceTax  { get; set; }
        public decimal? ED { get; set; }
        public decimal? PO { get; set; }
        public decimal Net { get; set; }

    
    }
}
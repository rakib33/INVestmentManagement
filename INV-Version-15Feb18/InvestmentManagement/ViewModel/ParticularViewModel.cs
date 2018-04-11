using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace InvestmentManagement.ViewModel
{
    public class ParticularViewModel
    {
      public string GroupName {get;set;}
      public string SubGroupName {get;set;}  
      public string Code {get;set;}  
      public string ParticularsName {get;set;}  
      public decimal? PrincipalPrvYear {get;set;}  
      public decimal? PrincipalCurrent {get;set;}
      public decimal? LimitMinimum { get; set; }
      public string Remarks { get; set; }  
    }
}
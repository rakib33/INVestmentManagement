using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace InvestmentManagement.ViewModel
{
    public class BankwiseReport
    {
        public string BankName { get; set; }
        public int FDRNo { get; set; }
        public string PrincipalAmountTK { get; set; }
        public string ExistingCapLimit { get; set; }
        public string PercentagetotalFDR { get; set; }
        public string Offerrate1 { get; set; }
        public string Offerrate2 { get; set; }
        public string Offerrate3 { get; set; }
        public string NPL { get; set; }
        public string CamelsRating { get; set; }
           
    }
}
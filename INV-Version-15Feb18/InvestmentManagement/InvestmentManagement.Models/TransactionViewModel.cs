using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace InvestmentManagement.InvestmentManagement.Models
{
    public class EBSIntegrationViewModel
    {
        public string TRANSACTIONCODE { get; set; }
        public string TRANSACTIONTYPE { get; set; }
        public string BSCREDITCONTROL { get; set; }
        public string BSDEBITCONTROL { get; set; }

        public string PLDEBITCONTROL { get; set; }
        public string PLCREDITCONTROL { get; set; }


        public string BANKDEBITCONTROL { get; set; }
        public string BANKCREDITCONTROL { get; set; }

        public string CODE { get; set; }
        public string DESCRIPTION { get; set; }
        public string ACCOUNTTYPE { get; set; }
        public string STATUS { get; set; }
        public string ORACLENOMINALCODE { get; set; }
        public string ORACLENOMINALNAME { get; set; }

    }
}
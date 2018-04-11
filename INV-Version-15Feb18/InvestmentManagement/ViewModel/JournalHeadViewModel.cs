using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using InvestmentManagement.InvestmentManagement.Models;

namespace InvestmentManagement.ViewModel
{
    public class JournalHeadViewModel
    {
        //public List<JOURNALHEAD> JournalHeads { get; set; }
        public List<JOURNALLINE> JournalLines { get; set; }
        public JOURNALHEAD JournalHead { get; set; }
        public JOURNALLINE JournalLine { get; set; }
    }
}
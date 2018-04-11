using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using InvestmentManagement.Models;

namespace InvestmentManagement.ViewModel
{
    public class GridViewModel<T>:ViewModelBase
    {
        public GridModel<T> GridModels { get; set; }
    }
}
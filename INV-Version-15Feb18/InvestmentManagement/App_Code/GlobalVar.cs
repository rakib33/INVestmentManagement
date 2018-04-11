using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace InvestmentManagement.App_Code
{
    public static class GlobalVar
    {
        /// <summary>
        /// Global variable that is constant.
        /// </summary>
        public const string GlobalString = "Important Text";

        /// <summary>
        /// Static value protected by access routine.
        /// </summary>
        static string _globalValue;

        /// <summary>
        /// Access routine for global variable.
        /// </summary>
        public static string GlobalValue
        {
            get
            {
                return _globalValue;
            }
            set
            {
                _globalValue = value;
            }
        }

        /// <summary>
        /// Global static field.
        /// </summary>
        public static bool GlobalBoolean;
    }
}
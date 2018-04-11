using InvestmentManagement.InvestmentManagement.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.EntityClient;
using System.Linq;
using System.Reflection;
using System.Web;

namespace InvestmentManagement.Models
{
    public class CommonFunction
    {
        public string MessageForView(string actionName)
        {
            var message = new Entities(HttpContext.Current.Session["Connection"] as EntityConnection).MENUs.Where(menu => menu.ACTION == actionName).Select(menu => menu.MESSAGE).FirstOrDefault();

            return message;
        }

        public List<MENU> GetMenus()
        {
            var MenuList = new Entities(HttpContext.Current.Session["Connection"] as EntityConnection).MENUs.OrderBy(menu => menu.NAME).ToList();

            return MenuList;
        }

        public bool CheckSession()
        {
            if (HttpContext.Current.Session["UserId"] == null || HttpContext.Current.Session["PreviousPage"] == null || HttpContext.Current.Session["currentPage"] == null || HttpContext.Current.Session["Connection"] == null || HttpContext.Current.Session["Path"] == null)
            {
                if (HttpContext.Current.Session["Connection"] != null)
                {
                    EntityConnection conn = HttpContext.Current.Session["Connection"] as EntityConnection;

                    if (conn.State == ConnectionState.Open)
                    {
                        conn.Close();
                    }
                }

                //return RedirectToAction("LogOut", "Home");
                return true;
            }
            else
                return false;
        }

        public IHtmlString GetPath(string controller, string parameter, string actionName)
        {
            if (string.IsNullOrEmpty(parameter))
            {
                parameter = new Entities(HttpContext.Current.Session["Connection"] as EntityConnection).MENUs.Where(menu => menu.CONTROLLER == controller).Select(menu => menu.PARAMETER).FirstOrDefault();
            }
            string settingsLink = string.Empty;
            string settings = new Entities(HttpContext.Current.Session["Connection"] as EntityConnection).MENUs.Where(menu => menu.CONTROLLER == controller).Select(menu => menu.SETTINGSGROUP).FirstOrDefault();
            if (!string.IsNullOrEmpty(settings))
                //settingsLink = "<li>" + "\n" + "<a class=\"load\" href=\"/InvestmentManagement/#/Settings/Settings?lblbreadcum=Settings\">Settings</a>" + "\n" + "</li>" + "\n";
                settingsLink = "<li>" + "\n" + "<a class=\"load\" href=\"/#/Settings/Settings?lblbreadcum=Settings\">Settings</a>" + "\n" + "</li>" + "\n";

            //IHtmlString str = new HtmlString("<li>" + "\n" + "<a class=\"load\" href=\"/InvestmentManagement/#/Home/Index?lblbreadcum=Dashbord\">Dashboard</a>" + "\n" + "</li>" +
            //    "\n" + settingsLink +
            //    "<li>" + "\n" + "<a class=\"load\" href=\"/InvestmentManagement/#/" + controller + "/" + actionName + "?lblbreadcum=" + parameter + "\">" + parameter + "</a>" + "\n" + "</li>");
            IHtmlString str = new HtmlString("<li>" + "\n" + "<a class=\"load\" href=\"/#/Home/Index?lblbreadcum=Dashbord\">Dashboard</a>" + "\n" + "</li>" +
             "\n" + settingsLink +
             "<li>" + "\n" + "<a class=\"load\" href=\"/#/" + controller + "/" + actionName + "?lblbreadcum=" + parameter + "\">" + parameter + "</a>" + "\n" + "</li>");

            return str;
        }

        public IHtmlString GetListPath(string parameter, string controller)
        {
            string settingsLink = string.Empty;
            string settings = new Entities(HttpContext.Current.Session["Connection"] as EntityConnection).MENUs.Where(menu => menu.CONTROLLER == controller).Select(menu => menu.SETTINGSGROUP).FirstOrDefault();
            if (!string.IsNullOrEmpty(settings))
                  settingsLink = "<li>" + "\n" + "<a class=\"load\" href=\"/#/Settings/Settings?lblbreadcum=Settings\">Settings</a>" + "\n" + "</li>" + "\n";
            IHtmlString str = new HtmlString("<li>" + "\n" + "<a class=\"load\" href=\"/#/Home/Index?lblbreadcum=Dashbord\">Dashboard</a>" + "\n" + "</li>" + "\n" + settingsLink + "<li class=\"active\">" + parameter + "</li>");
            return str;
        }

        public IHtmlString GetAddPath(IHtmlString path, string parameter)
        {
            IHtmlString str = new HtmlString(path + "\n" + "<li class=\"active\">" + "Add " + parameter + "</li>");

            return str;
        }

        public IHtmlString GetEditPath(IHtmlString path, string parameter)
        {
            IHtmlString str = new HtmlString(path + "\n" + "<li class=\"active\">" + "Update " + parameter + "</li>");

            return str;
        }

        public IHtmlString GetDetailsPath(IHtmlString path, string controller, string parameter, string actionName, string reference)
        {
            if (string.IsNullOrEmpty(parameter))
            {
                parameter = new Entities(HttpContext.Current.Session["Connection"] as EntityConnection).MENUs.Where(menu => menu.CONTROLLER == controller).Select(menu => menu.PARAMETER).FirstOrDefault();
            }
            IHtmlString str = new HtmlString(path + "\n" + "<li>" + "\n" + "<a class=\"load\" href=\"/#/" + controller + "/" + actionName + "?reference=" + reference + "\">" + parameter + "</a>" + "\n" + "</li>");

            return str;
        }

        public IHtmlString GetDetailsListPath(IHtmlString path, string parameter)
        {
            string detailsPath = path.ToString();
            string[] list = detailsPath.Split('\n');
            string detailsListPath = string.Empty;
            for (int i = 0; i < list.Length - 3; i++)
            {
                detailsListPath = detailsListPath + list[i];
            }

            IHtmlString str = new HtmlString(detailsListPath + "\n" + "<li class=\"active\">" + parameter + "</li>");

            return str;
        }

        public IHtmlString GetDetailsAddPath(IHtmlString path, string parameter)
        {
            IHtmlString str = new HtmlString(path + "\n" + "<li class=\"active\">" + "Add " + parameter + "</li>");

            return str;
        }

        public IHtmlString GetDetailsEditPath(IHtmlString path, string parameter)
        {
            IHtmlString str = new HtmlString(path + "\n" + "<li class=\"active\">" + "Update " + parameter + "</li>");

            return str;
        }

        //Conversion Method for List to datatable Edited by Hemel
        public DataTable ConvertToDataTable<T>(IList<T> data)
        {
            PropertyDescriptorCollection properties =
               TypeDescriptor.GetProperties(typeof(T));
            DataTable table = new DataTable();
            foreach (PropertyDescriptor prop in properties)
                table.Columns.Add(prop.Name, Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType);
            foreach (T item in data)
            {
                DataRow row = table.NewRow();
                foreach (PropertyDescriptor prop in properties)
                {
                    try
                    {
                        row[prop.Name] = prop.GetValue(item) ?? DBNull.Value;
                    }
                    catch { }
                }
                table.Rows.Add(row);
            }
            return table;
        }

        //Edited by Hemel
        public void CustomObjectNullValidation<T>(ref T CustomObject)
        {
            foreach (var property in CustomObject.GetType().GetProperties())
            {
                if (CustomObject.GetType().GetProperty(property.Name).GetValue(CustomObject, null) == null)
                {
                    var propertyType = property.PropertyType;
                    if (propertyType.IsGenericType && propertyType.GetGenericTypeDefinition() == typeof(Nullable<>))
                    {
                        propertyType = propertyType.GetGenericArguments()[0];

                        if (propertyType.Name == "DateTime")
                        {
                            CustomObject.GetType().GetProperty(property.Name).SetValue(CustomObject, DateTime.Today, null);
                        }
                        else if (propertyType.Name.ToLower() == "decimal")
                        {
                            CustomObject.GetType().GetProperty(property.Name).SetValue(CustomObject, (decimal)00.00, null);
                        }
                    }

                    if (property.PropertyType == typeof(string))
                    {
                        CustomObject.GetType().GetProperty(property.Name).SetValue(CustomObject, string.Empty, null);
                    }
                    else if (property.PropertyType == typeof(long))
                    {
                        CustomObject.GetType().GetProperty(property.Name).SetValue(CustomObject, (decimal)00.00, null);
                    }
                }
            }
        }

        //Datatable to List converstion

        public List<T> ConvertDataTable<T>(DataTable dt)
        {
            List<T> data = new List<T>();
            foreach (DataRow row in dt.Rows)
            {
                T item = GetItem<T>(row);
                data.Add(item);
            }
            return data;
        }

        private static T GetItem<T>(DataRow dr)
        {
            Type temp = typeof(T);
            T obj = Activator.CreateInstance<T>();

            foreach (DataColumn column in dr.Table.Columns)
            {
                foreach (PropertyInfo pro in temp.GetProperties())
                {
                    if (pro.Name == column.ColumnName)
                        pro.SetValue(obj, dr[column.ColumnName], null);
                    else
                        continue;
                }
            }
            return obj;
        }

        //start
        public  bool HelperConvertNumberToText(int num, out string buf)
        {

            string[] strones = {

            "One", "Two", "Three", "Four", "Five", "Six", "Seven", "Eight",

            "Nine", "Ten", "Eleven", "Twelve", "Thirteen", "Fourteen",

            "Fifteen", "Sixteen", "Seventeen", "Eighteen", "Nineteen",

          };



            string[] strtens = {

              "Ten", "Twenty", "Thirty", "Fourty", "Fifty", "Sixty",

              "Seventy", "Eighty", "Ninety", "Hundred"

          };



            string result = "";

            buf = "";

            int single, tens, hundreds;



            if (num > 1000)

                return false;



            hundreds = num / 100;

            num = num - hundreds * 100;

            if (num < 20)
            {

                tens = 0; // special case

                single = num;

            }

            else
            {

                tens = num / 10;

                num = num - tens * 10;

                single = num;

            }



            result = "";



            if (hundreds > 0)
            {

                result += strones[hundreds - 1];

                result += " Hundred ";

            }

            if (tens > 0)
            {

                result += strtens[tens - 1];

                result += " ";

            }

            if (single > 0)
            {

                result += strones[single - 1];

                result += " ";

            }



            buf = result;

            return true;

        }



        public  bool ConvertNumberToText(int num, out string result)
        {

            string tempString = "";

            int thousands;

            int temp;

            result = "";

            if (num < 0 || num > 100000)
            {

                System.Console.WriteLine(num + " \tNot Supported");

                return false;

            }



            if (num == 0)
            {

                System.Console.WriteLine(num + " \tZero");

                return false;

            }



            if (num < 1000)
            {

                HelperConvertNumberToText(num, out tempString);

                result += tempString;

            }

            else
            {

                thousands = num / 1000;

                temp = num - thousands * 1000;

                HelperConvertNumberToText(thousands, out tempString);

                result += tempString;

                result += "Thousand ";

                HelperConvertNumberToText(temp, out tempString);

                result += tempString;

            }

            return true;
        }

        //End

        //s
        static String NumWordsWrapper(double n)
        {
            string words = "";
            double intPart;
            double decPart = 0;
            if (n == 0)
                return "zero";
            try
            {
                string[] splitter = n.ToString().Split('.');
                intPart = double.Parse(splitter[0]);
                decPart = double.Parse(splitter[1]);
            }
            catch
            {
                intPart = n;
            }

            words = NumWords(intPart);

            if (decPart > 0)
            {
                if (words != "")
                    words += " and ";
                int counter = decPart.ToString().Length;
                switch (counter)
                {
                    case 1: words += NumWords(decPart) + " tenths"; break;
                    case 2: words += NumWords(decPart) + " hundredths"; break;
                    case 3: words += NumWords(decPart) + " thousandths"; break;
                    case 4: words += NumWords(decPart) + " ten-thousandths"; break;
                    case 5: words += NumWords(decPart) + " hundred-thousandths"; break;
                    case 6: words += NumWords(decPart) + " millionths"; break;
                    case 7: words += NumWords(decPart) + " ten-millionths"; break;
                }
            }
            return words;
        }

        public static String NumWords(double n) //converts double to words
        {
            string[] numbersArr = new string[] { "one", "two", "three", "four", "five", "six", "seven", "eight", "nine", "ten", "eleven", "twelve", "thirteen", "fourteen", "fifteen", "sixteen", "seventeen", "eighteen", "nineteen" };
            string[] tensArr = new string[] { "twenty", "thirty", "fourty", "fifty", "sixty", "seventy", "eighty", "ninty" };
            string[] suffixesArr = new string[] { "thousand", "million", "billion", "trillion", "quadrillion", "quintillion", "sextillion", "septillion", "octillion", "nonillion", "decillion", "undecillion", "duodecillion", "tredecillion", "Quattuordecillion", "Quindecillion", "Sexdecillion", "Septdecillion", "Octodecillion", "Novemdecillion", "Vigintillion" };
            string words = "";

            bool tens = false;

            if (n < 0)
            {
                words += "negative ";
                n *= -1;
            }

            int power = (suffixesArr.Length + 1) * 3;

            while (power > 3)
            {
                double pow = Math.Pow(10, power);
                if (n >= pow)
                {
                    if (n % pow > 0)
                    {
                        words += NumWords(Math.Floor(n / pow)) + " " + suffixesArr[(power / 3) - 1] + ", ";
                    }
                    else if (n % pow == 0)
                    {
                        words += NumWords(Math.Floor(n / pow)) + " " + suffixesArr[(power / 3) - 1];
                    }
                    n %= pow;
                }
                power -= 3;
            }
            if (n >= 1000)
            {
                if (n % 1000 > 0) words += NumWords(Math.Floor(n / 1000)) + " thousand, ";
                else words += NumWords(Math.Floor(n / 1000)) + " thousand";
                n %= 1000;
            }
            if (0 <= n && n <= 999)
            {
                if ((int)n / 100 > 0)
                {
                    words += NumWords(Math.Floor(n / 100)) + " hundred";
                    n %= 100;
                }
                if ((int)n / 10 > 1)
                {
                    if (words != "")
                        words += " ";
                    words += tensArr[(int)n / 10 - 2];
                    tens = true;
                    n %= 10;
                }

                if (n < 20 && n > 0)
                {
                    if (words != "" && tens == false)
                        words += " ";
                    words += (tens ? "-" + numbersArr[(int)n - 1] : numbersArr[(int)n - 1]);
                    n -= Math.Floor(n);
                }
            }

            return words;

        }
        //e


    }
}
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Collections;
using System.Windows;
using System.Windows.Forms;
using Lib;

namespace Lib
{
    public static class Conv
    {
        public static string StrOrEmty(object val)
        {
            return (val != null && val.ToString() != "") ? val.ToString() : "";
        }
        public static int IntOrDefault(object val, int def = -1)
        {
            int rez;
            if (val != null && int.TryParse(val.ToString(), out rez)) return rez;
            return def;
        }
        public static long LongOrDefault(object val, int def = -1)
        {
            long rez;
            if (val != null && long.TryParse(val.ToString(), out rez)) return rez;
            return def;
        }
        public static bool BoolOrFalse(object val)
        {
            return (val != null && val.GetType() == typeof(bool)) ? (bool)val : false;
        }
        public static DateTime DateTimeOrDefault(object val)
        {
            return (val != null && val.GetType() == typeof(DateTime)) ? (DateTime)val : new DateTime(0);
        }
    }


}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;

namespace Inventory.Common
{
    public static class CommonConstants
    {
        public static string USER_SESSION = "USER_SESSION";
        public static string SESSION_CREDENTIALS = "SESSION_CREDENTIALS";
        public static string FormatPhoneNum(string phoneNum)
        {
            if(phoneNum.Length == 10)
            {
                return Regex.Replace(phoneNum, @"(\d{4})(\d{3})(\d{3})", "($1) $2-$3");
            }
            if(phoneNum.Length == 11)
            {
                return Regex.Replace(phoneNum, @"(\d{4})(\d{3})(\d{4})", "($1) $2-$3");
            }
            else
            {
                return phoneNum;
            }
        }
    }
}
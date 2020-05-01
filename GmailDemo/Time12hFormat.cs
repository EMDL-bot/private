using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;

namespace GmailDemo
{
    public static class Time12hFormat
    {
        private static string TIME12HOURS_PATTERN = "(1[012]|[1-9]):[0-5][0-9](\\s)?(?i)(am|pm)";
        public static bool Validate(string time)
        {
            Regex regex = new Regex(TIME12HOURS_PATTERN);
            return regex.IsMatch(time);
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GUX.Extensions
{
    public static class MainExtensions
    {
        public static string DecodeString(this string encoded)
        {
            return Encoding.ASCII.GetString(Convert.FromBase64String(encoded));
        }

        public static string EncodeString(this string decoded)
        {
            return Convert.ToBase64String(Encoding.ASCII.GetBytes(decoded));
        }
    }
}

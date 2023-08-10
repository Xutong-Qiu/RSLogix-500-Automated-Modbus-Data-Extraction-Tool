using System;
using System.Text.RegularExpressions;
using Microsoft.VisualBasic.CompilerServices;

namespace WindowsApp1
{
    /// <summary>
/// This module carries out operations related to addresses.
/// </summary>
    public static class AddressSolver
    {

        /// <summary>
    /// This function gets rid of any prefix and suffix of an address. For example,
    /// "#N64:0.CVP"->"N64:0"
    /// </summary>
    /// <param name="addr">The address of the entry</param>
    /// <returns>A string after tuning. If it's not a valid address, returns an empty string.</returns>
        public static string Tune(string addr)
        {
            var regex = new Regex(@"(?:([A-Z]{1,3})(\d{1,3}):(\d{1,3})|(?:(I|O|S|U):(\d{1,3}(?:\.\d{1,3})*)))(?:\/(\d{1,2}))*");
            var match = regex.Match(addr);
            if (match.Success == true)
            {
                return match.Groups[0].ToString();
            }
            return "";
        }
        /// <summary>
    /// This function carries out addition on the last group of numbers of an address.
    /// For example, AddrAdder("N64:34",1) = N64:35
    /// </summary>
    /// <param name="addr">The address of the entry</param>
    /// <returns>A string after adding. If it's not a valid address, returns an empty string.</returns>
        public static string AddrAdder(string addr, int offset)
        {
            var regex = new Regex(@"(?:([A-Z]{1,3})(\d{1,3}):(\d{1,3})|(?:(I|O|S|U):(\d{1,3}(?:\.\d{1,3})*)))(?:\/(\d{1,2}))*");
            var match = regex.Match(addr);
            int value;

            if (match.Success == true)
            {
                value = (int)Math.Round(Conversions.ToSingle(match.Groups[3].Value));
                value += offset;
                int index = match.Groups[3].Index;
                return addr.Substring(0, index) + value.ToString();
            }
            return "";
        }

        public static string GetExtension(string addr)
        {
            var regex = new Regex(@"(?:([A-Z]{1,3})(\d{1,3}):(\d{1,3})|(?:(I|O|S|U):(\d{1,3}(?:\.\d{1,3})*)))(?:\/(\d{1,2}))*(?:\.|\/)([A-Z]{1,3})*");
            var match = regex.Match(addr);
            if (match.Success == true)
            {
                return match.Groups[7].ToString();
            }
            return "";
        }

        public static int ConvertRegAddr(string addr)
        {
            var regex = new Regex(@"(?:([A-Z]{1,3})(\d{1,3}):(\d{1,3})|(?:(I|O|S|U):(\d{1,3}(?:\.\d{1,3})*)))(?:\/(\d{1,2}))*(.*)");
            var match = regex.Match(addr);
            if (match.Success == true)
            {
                return (int)Math.Round(Conversions.ToSingle(match.Groups[3].ToString()));
            }
            return -1;
        }
        public static string ConvertCoilAddr(string addr)
        {
            var regex = new Regex(@"(?:([A-Z]{1,3})(\d{1,3}):(\d{1,3})|(?:(I|O|S|U):(\d{1,3}(?:\.\d{1,3})*)))(?:\/(\d{1,2}))*(.*)");
            var match = regex.Match(addr);
            if (match.Success == true)
            {
                int num = (int)Math.Round(Conversions.ToSingle(match.Groups[3].ToString()));
                num *= 16;
                if (match.Groups[6].ToString() is not null && !string.IsNullOrEmpty(match.Groups[6].ToString()))
                {
                    num = (int)Math.Round(num + Conversions.ToDouble(match.Groups[6].ToString()));
                }
                return num.ToString();
            }
            return (-1).ToString();
        }
    }
}
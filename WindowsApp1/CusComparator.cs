using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using Microsoft.VisualBasic.CompilerServices;

namespace WindowsApp1
{


    /// <summary>
/// This class defines data entry comparers that are used to compare and sort data entries displayed
/// on the datagrid. It does so by calling the address comparer method and pass x(0), y(0) as its
/// parameter. This is because in a data entry, the first column is address. So a data entry comparer 
/// is just using address comparer to compare the addresses.
/// </summary>
    public class DataEntryComparer : IComparer<string[]>
    {

        public int Compare(string[] x, string[] y)
        {
            var com = new AddrComparer();
            return com.Compare(x[0], y[0]);
        }
    }


    public class DataGridEntryComparer : IComparer
    {

        private bool descending;

        public DataGridEntryComparer(bool descending)
        {
            this.descending = descending;
        }

        public int Compare(object x, object y)
        {
            DataGridViewRow dataGridViewRowX = x as DataGridViewRow;
            DataGridViewRow dataGridViewRowY = y as DataGridViewRow;
            if (dataGridViewRowX is not null && dataGridViewRowY is not null)
            {
                var cellValueX = dataGridViewRowX.Cells[0].Value;
                var cellValueY = dataGridViewRowY.Cells[0].Value;

                if (cellValueX is null && cellValueY is null)
                {
                    return 0;
                }
                else if (cellValueX is null)
                {
                    return -1;
                }
                else if (cellValueY is null)
                {
                    return 1;
                }
                // In this example, we assume the cell values are integers.
                string addrx = cellValueX.ToString();
                string addry = cellValueY.ToString();
                var com = new AddrComparer();
                if (descending)
                {
                    return com.Compare(addrx, addry);
                }
                else
                {
                    return com.Compare(addry, addrx);
                }

            }
            return 0;
        }
    }

    /// <summary>
/// This class defines address comparers that are used to compare and sort the addresses. It 
/// uses regex to parse the letter and integers in the given address string and compare them.
/// </summary>
    public class AddrComparer : IComparer<string>
    {

        public int Compare(string x, string y)
        {
            string addr = @"^(?:([A-Z]{1,3})(\d{1,3}):(\d{1,3})|(?:(I|O|S|U):(\d{1,3}(?:\.\d{1,3})*)))(?:\/(\d{1,2}))*(.*)$";
            var regex = new Regex(addr);
            var matchx = regex.Match(x);
            var matchy = regex.Match(y);
            if (matchx.Success == false | matchy.Success == false)
            {
                return x.CompareTo(y);
            }
            string xletter = !string.IsNullOrEmpty(matchx.Groups[1].ToString()) ? matchx.Groups[1].ToString() : matchx.Groups[4].ToString();
            string yletter = !string.IsNullOrEmpty(matchy.Groups[1].ToString()) ? matchy.Groups[1].ToString() : matchy.Groups[4].ToString();
            if ((xletter ?? "") != (yletter ?? ""))
            {
                return xletter.CompareTo(yletter);
            }
            string xaddr1 = !string.IsNullOrEmpty(matchx.Groups[2].ToString()) ? matchx.Groups[2].ToString() : matchx.Groups[5].ToString();
            string yaddr1 = !string.IsNullOrEmpty(matchy.Groups[2].ToString()) ? matchy.Groups[2].ToString() : matchy.Groups[5].ToString();
            if ((xaddr1 ?? "") != (yaddr1 ?? ""))
            {
                return Conversions.ToSingle(xaddr1).CompareTo(Conversions.ToSingle(yaddr1));
            }
            string xaddr2 = !string.IsNullOrEmpty(matchx.Groups[3].ToString()) ? matchx.Groups[3].ToString() : matchx.Groups[5].ToString();
            string yaddr2 = !string.IsNullOrEmpty(matchy.Groups[3].ToString()) ? matchy.Groups[3].ToString() : matchy.Groups[5].ToString();

            if ((xaddr2 ?? "") != (yaddr2 ?? ""))
            {
                return Conversions.ToSingle(xaddr2).CompareTo(Conversions.ToSingle(yaddr2));
            }
            string xaddr3 = matchx.Groups[6].ToString();
            string yaddr3 = matchy.Groups[6].ToString();
            if ((xaddr3 ?? "") != (yaddr3 ?? ""))
            {
                if (string.IsNullOrEmpty(xaddr3))
                {
                    return -1;
                }
                if (string.IsNullOrEmpty(yaddr3))
                {
                    return 1;
                }
                return Conversions.ToSingle(xaddr3).CompareTo(Conversions.ToSingle(yaddr3));
            }
            return matchx.Groups[7].ToString().CompareTo(matchy.Groups[7].ToString());
        }
    }
}
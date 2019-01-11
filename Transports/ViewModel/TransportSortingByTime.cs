using Bussiness.Layer.Model;
using System.Collections;

namespace Transports.ViewModel
{
    public class TransportSortingByEntryTime : IComparer
    {
        public int Compare(object a, object b)
        {
            Transport x = a as Transport;
            Transport y = b as Transport;
            if (!string.IsNullOrEmpty(x.EntryTime) && !string.IsNullOrEmpty(y.EntryTime) && !string.IsNullOrEmpty(x.ExitTime) && !string.IsNullOrEmpty(y.ExitTime))
            {
                return x.EntryTime.CompareTo(y.EntryTime) + x.ExitTime.CompareTo(y.ExitTime);
            }
            if (string.IsNullOrEmpty(x.EntryTime) && string.IsNullOrEmpty(y.EntryTime) && !string.IsNullOrEmpty(x.ExitTime) && !string.IsNullOrEmpty(y.ExitTime))
            {
                return x.ExitTime.CompareTo(y.ExitTime);
            }
            if (string.IsNullOrEmpty(x.ExitTime) && string.IsNullOrEmpty(y.EntryTime) && !string.IsNullOrEmpty(x.EntryTime) && !string.IsNullOrEmpty(y.ExitTime))
            {
                return x.EntryTime.CompareTo(y.ExitTime);
            }
            if (string.IsNullOrEmpty(x.EntryTime) && string.IsNullOrEmpty(y.ExitTime) && !string.IsNullOrEmpty(x.ExitTime) && !string.IsNullOrEmpty(y.EntryTime))
            {
                return x.ExitTime.CompareTo(y.EntryTime);
            }
            if (!string.IsNullOrEmpty(x.EntryTime) && !string.IsNullOrEmpty(x.ExitTime) && string.IsNullOrEmpty(x.EntryTime) && !string.IsNullOrEmpty(y.ExitTime))
            {
                return x.ExitTime.CompareTo(y.ExitTime);
            }
            if (string.IsNullOrEmpty(x.EntryTime) && !string.IsNullOrEmpty(x.ExitTime) && !string.IsNullOrEmpty(x.EntryTime) && !string.IsNullOrEmpty(y.ExitTime))
            {
                return x.ExitTime.CompareTo(y.ExitTime);
            }
            if (!string.IsNullOrEmpty(x.EntryTime) && !string.IsNullOrEmpty(x.ExitTime) && !string.IsNullOrEmpty(x.EntryTime) && string.IsNullOrEmpty(y.ExitTime))
            {
                return x.EntryTime.CompareTo(y.EntryTime);
            }
            if (!string.IsNullOrEmpty(x.EntryTime) && string.IsNullOrEmpty(x.ExitTime) && !string.IsNullOrEmpty(x.EntryTime) && !string.IsNullOrEmpty(y.ExitTime))
            {
                return x.EntryTime.CompareTo(y.EntryTime);
            }
            return 0;
        }
    }
}

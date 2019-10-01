using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartDayClient
{
    public enum SyncType { Products = 1, Stock = 2, Campaigns = 2, Categories = 3 };

    [Serializable]
    public class Sync
    {
        public DateTime LastestSync { get; set; } = DateTime.MinValue;
    }
}

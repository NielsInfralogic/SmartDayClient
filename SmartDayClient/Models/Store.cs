using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartDayClient.Models
{
    public class ReadonlyFields
    {
        public string inventoryStatusDate { get; set; } = "";
        public string lastTimeMarkedForExport { get; set; } = "";
    }

    public class HandymanStore
    {
        public bool addToAllDepartments { get; set; } = false;
        public bool automaticHandheldUpdate { get; set; } = false;
        public bool automaticHandheldUpdateNextSync { get; set; } = false;
        public bool inventoryStatusAllowedOnHandheld { get; set; } = false;
        public bool showInvetoryWhenCounting { get; set; } = false;
        public bool automaticInventoryStatusUpdate { get; set; } = false;
        public bool ignoreWholesaler { get; set; } = false;
        public ReadonlyFields readonlyFields { get; set; }

        public HandymanStore()
        {
            readonlyFields = new ReadonlyFields();

        }
    }

    public class SmartdayStore
    {
        public bool isActive { get; set; } = false;
    }

    public class Store
    {
        public string storeName { get; set; } = "";
        public HandymanStore handymanStore { get; set; }
        public SmartdayStore smartdayStore { get; set; }
        public string id { get; set; } = "";
        public string externalId { get; set; } = "";

        public Store()
        {
            handymanStore = new HandymanStore();
            smartdayStore = new SmartdayStore();
        }
    }
}

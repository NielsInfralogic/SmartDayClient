using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartDayClient.Models
{
    public class Category
    {

        public string name { get; set; } = "";
        public HandymanCategory handymanCategory { get; set; }
        public SmartdayCategory smartdayCategory { get; set; }
        public int type { get; set; }
        public string id { get; set; } = "";
        public string externalId { get; set; } = "";

        public Category()
        {
            handymanCategory = new HandymanCategory();
            smartdayCategory = new SmartdayCategory();
        }
    }

    public class  Reference
    {
        public string id { get; set; } = "";
        public string externalId { get; set; } = "";
    }

    public class HandymanCategory
    {
        public string label { get; set; } = "";
        public int settings { get; set; } = 0;
        public Reference departmentReference { get; set; }
        public bool customerSignature { get; set; } = false;
        public HandymanCategory()
        {
            departmentReference = new Reference();
        }
    }

    public class SmartdayCategory
    {
        public string description { get; set; } = "";
        public bool isActive { get; set; } = true;
        public int subtype { get; set; } = 0;
    }
}

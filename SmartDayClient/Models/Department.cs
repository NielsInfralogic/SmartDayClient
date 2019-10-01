using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartDayClient.Models
{
    public class Department
    {
        public string name { get; set; } = "";
        public Location postalAddress { get; set; }
        public HandymanDepartment handymanDepartment { get; set; }
        public SmartdayStore smartdayDepartment { get; set; }
        public string id { get; set; } = "";
        public string externalId { get; set; } = "";

        public Department()
        {
            postalAddress = new Location();
            handymanDepartment = new HandymanDepartment();
            smartdayDepartment = new SmartdayStore();
        }
    }


    public class Settings
    {
        public Reference mainStoreReference { get; set; }
        public Reference preferredWholesalerReference { get; set; }
        public int createNewOrdersRule { get; set; }
        public bool knownCustomerRequired { get; set; }
        public int hidePricesOnPDA { get; set; }

        public Settings()
        {
            mainStoreReference = new Reference();
            preferredWholesalerReference = new Reference();
        }
    }

    public class HandymanDepartment
    {
        public Terminology terminology { get; set; }
        public Settings settings { get; set; }

        public HandymanDepartment()
        {
            terminology = new Terminology();
            settings = new Settings();
        }
    }

    public class Terminology
    {
        public string label_KB_Heading { get; set; } = "";
        public string label_KB1 { get; set; } = "";
        public string label_KB2 { get; set; } = "";
        public string label_Phase { get; set; } = "";
        public string label_Requsition { get; set; } = "";
        public string label_ItemNo2 { get; set; } = "";
        public string label_ItemGroup { get; set; } = "";
        public string label_OfficialInCharge { get; set; } = "";
        public string label_Department { get; set; } = "";
        public string label_MainParticipant { get; set; } = "";
        public string label_MainOrder { get; set; } = "";
        public string label_CustomerReference { get; set; } = "";
    }
}

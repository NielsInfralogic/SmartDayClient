using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartDayClient.Models
{
    public class User
    {
        public string name { get; set; } = "";
        public string username { get; set; } = "";
        public object password { get; set; } = "";
        public Reference departmentReference { get; set; }
        public string cellPhoneNumber { get; set; } = "";
        public string email { get; set; } = "";
        public Reference storeReference { get; set; }
        public List<Thing> thingReferences { get; set; }
        public bool active { get; set; } = false;
        public string guardSystemsUser { get; set; } = null;
        public SmartdayUser smartdayUser { get; set; }
        public object handymanUser { get; set; } = null;
        public string id { get; set; } = "";
        public string externalId { get; set; } = "";

        public User()
        {
            departmentReference = new Reference();
            storeReference = new Reference();
            thingReferences = new List<Thing>();
            smartdayUser = new SmartdayUser();

        }
    }

    public class SmartdayUser
    {
        public string certificationNumber { get; set; } = "";
        public string equipmentNumber { get; set; } = "";
        public bool useOnlyOwnStock { get; set; } = false;
        public string userGroupId { get; set; } = "";
    }
}

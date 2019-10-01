using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartDayClient.Models
{
    public class Site
    {
        public string name { get; set; } = "";
        public CategoryReference categoryReference { get; set; }
        public Reference customerReference { get; set; }
        public Location postalAddress { get; set; }
        public string picture { get; set; } = "";
        public string serialnumber { get; set; } = "";
        public string comment { get; set; } = "";
        public string state { get; set; } = "";
        public List<Contact> contacts { get; set; }
        public HandymanSite handymanSite { get; set; } = null;
        public string id { get; set; } = "";
        public string externalId { get; set; } = "";

        public Site()
        {
            categoryReference = new CategoryReference();
            customerReference = new Reference();
            contacts = new List<Contact>();
            postalAddress = new Location();
            //handymanSite = new HandymanSite();
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartDayClient.Models
{

    public class Project
    {

        public string name { get; set; } = "";
        public Reference categoryReference { get; set; }
        public int status { get; set; }
        public string description { get; set; } = "";
        public double price { get; set; }
        public bool fixedPrice { get; set; }
        public Reference customerReference { get; set; }
        public Reference siteReference { get; set; }
        public Reference thingReference { get; set; }
        public Reference offerOrderReference { get; set; }
        public string createdDate { get; set; } = "";
        public string startDate { get; set; } = "";
        public string endDate { get; set; } = "";
        public string deadlineDate { get; set; } = "";
        public Reference ownerReference { get; set; }
        public string id { get; set; } = "";
        public string externalId { get; set; } = "";

        public Project()
        {
            categoryReference = new Reference();
            customerReference = new Reference();
            siteReference = new Reference();
            thingReference = new Reference();
            offerOrderReference = new Reference();
            ownerReference = new Reference();
        }
    }

    
}

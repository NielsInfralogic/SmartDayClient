using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartDayClient.Models
{
    public class Customer
    {
        public string name { get; set; } = "";             
        public bool isCompany { get; set; } = false;
        public string vatNumber { get; set; } = "";
        public string cellPhoneNumber { get; set; } = "";
        public string phoneNumber { get; set; } = "";
        public string email { get; set; } = "";
        public string comment { get; set; } = "";
        public CategoryReference categoryReference { get; set; }
        public Location billingAddress { get; set; }
        public List<Contact> contacts { get; set; }
        public HandymanCustomer handymanCustomer { get; set; }
        public SmartdayCustomer smartdayCustomer { get; set; }
        public string id { get; set; } = "";
        public string externalId { get; set; } = "";

        public Customer()
        {
            categoryReference = new CategoryReference();
            billingAddress = new Location();
            contacts = new List<Contact>();
            handymanCustomer = new HandymanCustomer();
            smartdayCustomer = new SmartdayCustomer();
        }
    }


    public class CategoryReference
    {
        public string name { get; set; } = "";
        public string id { get; set; } = "";
        public string externalId { get; set; } = "";
    }

    public class HandymanAddress
    {
        public string addressName { get; set; } = "";
        public string address3 { get; set; } = "";
        public string address4 { get; set; } = "";
    }

    public class DenmarkAddress
    {
        public string globalLocationNumber { get; set; } = "";
    }

    public class NorwayAddress
    {
        public string boligmappaEdokNumber { get; set; } = "";
        public int boligmappaPlantID { get; set; } = 0;
    }

    public class Location
    {
        public double latitude { get; set; } = 0.0;
        public double longitude { get; set; } = 0.0;
        public HandymanAddress handymanAddress { get; set; }
        public DenmarkAddress denmarkAddress { get; set; }
        public NorwayAddress norwayAddress { get; set; }
        public string address1 { get; set; } = "";
        public string housenumber { get; set; } = "";
        public string address2 { get; set; } = "";
        public string postalCode { get; set; } = "";
        public string postalArea { get; set; } = "";
        public string country { get; set; } = "";
        public string id { get; set; } = "";
        public string externalId { get; set; } = "";

        public Location()
        {
            handymanAddress = new HandymanAddress();
            denmarkAddress = new DenmarkAddress();
            norwayAddress = new NorwayAddress();
        }
    }

    public class HandymanContact
    {
        public string comment { get; set; } = "";
    }

    public class Contact
    {
        public string name { get; set; } = "";
        public string cellPhoneNumber { get; set; } = "";
        public string phoneNumber { get; set; } = "";
        public string email { get; set; } = "";
        public HandymanContact handymanContact { get; set; }
        public string id { get; set; } = "";
        public string externalId { get; set; } = "";

        public Contact()
        {
            handymanContact = new HandymanContact();
        }
    }


    public class HandymanCustomer
    {
        public int sendToAll { get; set; } = 0;
        public List<Location> addressList { get; set; }
        public HandymanCustomer()
        {
            addressList = new List<Location>();
        }
    }

    public class SmartdayCustomer
    {
        public int state { get; set; } = 0;
    }

    public class HandymanSite
    {
        public int installationOrigin { get; set; } = 0;
        public Reference parentReference { get; set; }
        public Reference departmentReference { get; set; }
        public Reference responsibleReference { get; set; }
        public Reference preferredEmployee1Reference { get; set; }
        public Reference preferredEmployee2Reference { get; set; }
        public string installedDate { get; set; } = "";
        public string lastServiceDate { get; set; } = "";
        public string lastRepairDate { get; set; } = "";
        public string scannerID { get; set; } = "";
        public int estimatedTravelTime { get; set; } = 0;
        public double travelDistance { get; set; } = 0.0;

        public HandymanSite()
        {
            parentReference = new Reference();
            departmentReference = new Reference();
            responsibleReference = new Reference();
            preferredEmployee1Reference = new Reference();
            preferredEmployee2Reference = new Reference();
        }
    }
}

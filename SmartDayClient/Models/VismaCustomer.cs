using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartDayClient.Models
{
    public class VismaCustomer
    {
        public int CustomerNo { get; set; } = 0;
        public int ActorNo { get; set; } = 0;



        public string Name { get; set; } = string.Empty;
        public string AddressLine1 { get; set; } = string.Empty;
        public string AddressLine2 { get; set; } = string.Empty;
        public string AddressLine3 { get; set; } = string.Empty;
        public string AddressLine4 { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string Mobile { get; set; } = string.Empty;
        
        public string EmailAddress { get; set; } = string.Empty;
        public string PostCode { get; set; } = string.Empty;
        public string PostalArea { get; set; } = string.Empty;
        public string CompanyNo { get; set; } = string.Empty;
        public string CountryCode { get; set; } = string.Empty;

        public int CountryNumber { get; set; } = 46;

        public string YourRef { get; set; } = string.Empty;

        public int Group3 { get; set; } = 1;

        public string SmartDayId { get; set; } = string.Empty;

        public string SmartDaySiteId { get; set; } = string.Empty;

        public string Memo { get; set; } = string.Empty;

        public int Group1 { get; set; } = 0;
        public List<VismaActor> DeliveryAddressList { get; set; }
        public List<VismaActor> ContactList { get; set; }
        public List<VismaActor> AddressList { get; set; }

//        public int customerCategoryId { get; set; } = 0;

        public VismaCustomer()
        {
            DeliveryAddressList = new List<VismaActor>();
            ContactList = new List<VismaActor>();
            AddressList = new List<VismaActor>();
        }
       
    }
        
    public class VismaActor
    {
        public int ActorNo { get; set; } = 0;
        public string Name { get; set; } = string.Empty;
        public string AddressLine1 { get; set; } = string.Empty;
        public string AddressLine2 { get; set; } = string.Empty;
        public string AddressLine3 { get; set; } = string.Empty;
        public string AddressLine4 { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string Mobile { get; set; } = string.Empty;
        public string EmailAddress { get; set; } = string.Empty;
        public string PostCode { get; set; } = string.Empty;
        public string PostalArea { get; set; } = string.Empty;
        public string CountryCode { get; set; } = string.Empty;
        public int CountryNumber { get; set; } = 46;
      
        public string CompanyNumber { get; set; } = "";
        public string SmartDayId { get; set; } = string.Empty;
        public string SmartDaySiteId { get; set; } = string.Empty;
        public string Memo { get; set; } = string.Empty;
    }
}

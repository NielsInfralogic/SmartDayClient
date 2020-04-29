using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartDayClient.Models
{
    public class VismaSite
    {

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


        public string SmartDayId { get; set; } = "";
    }
}

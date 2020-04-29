using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartDayClient.Models
{
    public class VismaProject
    {
        public int ProjectRno { get; set; } = 0;

        public int ProjectTypeSt { get; set; } = 0;
        public int CustomerNo { get; set; } = 0;             // =Customereref
        public string Name { get; set; } = "";
        public string Address1 { get; set; } = "";
        public string Address2 { get; set; } = "";
        public string PostCode { get; set; } = "";
        public string PostalArea { get; set; } = "";
        public int StatusGroup11 { get; set; } = 0;
        public int CreateDate { get; set; } = 0; // CreDt
        public int ResponsibleEmpNo { get; set; } = 0;
        public string ResponsibleName { get; set; } = "";    // =Responsibleref

        public int EstimatedStartDt { get; set; } = 0;
        public int EstimatedEndDt { get; set; } = 0;
        public string ServiceUnitGroupRno { get; set; } = "";
        public string ServiceUnitTypeName { get; set; } = "";
        public int ServiceUnitQuantity { get; set; } = 0;
        public int DeadlineDt { get; set; } = 0;        // Dt3

        public string SmartDayProjectID { get; set; } = "";                  // Smartdat Project-id

        public string ServiceUnitRno { get; set; } = "";        // =Thingsref


        public string DeliveryName { get; set; } = "";
        public string DeliveryAddress1 { get; set; } = "";
        public string DeliveryAddress2 { get; set; } = "";
        public string DeliveryAddress3 { get; set; } = "";
        public string DeliveryAddress4 { get; set; } = "";
        public string DeliveryPostCode { get; set; } = "";
        public string DeliveryPostalArea { get; set; } = "";
        public int DeliveryCountryNumber { get; set; } = 45;

        public string SmartDaySiteID { get; set; } = "";
    }

}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartDayClient.Models
{
    public class SalaryCode
    {
        public Reference userReference { get; set; }
        public string date { get; set; } = "";
        public double amount { get; set; } = 0.0;
        public Reference salaryCodeReference { get; set; }
        public string info { get; set; } = "";
        public double totalPrice { get; set; } = 0.0;
        public double costPrice { get; set; } = 0.0;
        public int status { get; set; } = 0;
        public string description { get; set; } = "";
        public SmartdaySalaryCodeRegistration smartdaySalaryCodeRegistration { get; set; }
        public object handymanSalaryCodeRegistration { get; set; } = null;
        public string id { get; set; } = "";
        public string externalId { get; set; } = "";
        public SalaryCode()
        {
            userReference = new Reference();
            salaryCodeReference = new Reference();
            smartdaySalaryCodeRegistration = new SmartdaySalaryCodeRegistration();
        }
    }

    public class SmartdaySalaryCodeRegistration
    {
        public string unit { get; set; } = "";
        public double unitPrice { get; set; } = 0.0;
        public string comment { get; set; } = "";
        public Reference categoryReference { get; set; }

        public SmartdaySalaryCodeRegistration()
        {
            categoryReference = new Reference();
        }
    }

}

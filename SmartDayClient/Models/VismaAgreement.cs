using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartDayClient.Models
{
    public class VismaAgreement
    {
        public int AgreementNumber { get; set; } = 0;

        public int AgreementActNo { get; set; } = 0;
        public string Decription { get; set; } = "";
        public int FromDate { get; set; } = 0;
        public int FromTime { get; set; } = 0;
        public int ToDate { get; set; } = 0;
        public int ToTime { get; set; } = 0;
        public string ProdNo { get; set; } = "";

        public decimal Quantity { get; set; } = 0.0M;

        public decimal Price { get; set; } = 0.0M;

        public int CustomerNumber { get; set; } = 0;
        public string CustomerName { get; set; } = "";

        public int Status { get; set; } = 0;


    }
}

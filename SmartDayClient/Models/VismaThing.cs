using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartDayClient.Models
{
    public class VismaOrderThing
    {
        public string RNo { get; set; } = "";
  
        public List<VismaDocument> Documents { get; set; }

        public string SmartDayThingId { get; set; } = "";
        

        public VismaOrderThing()
        {

            Documents = new List<VismaDocument>();
        }
             
    }

    public class VismaDocument
    {
        public string DocFileName { get; set; } = "";
        public int PK { get; set; } = 0;

    }


    public class VismaThing
    {
        public string RNo { get; set; } = "";
        public string Name { get; set; } = "";
        public string Phone { get; set; } = "";
        public string Inf7 { get; set; } = "";
        public string Ad4 { get; set; } = "";
        public string R3 { get; set; } = "";
        public string R3R9 { get; set; } = "";

        public string Memo { get; set; } = "";

        public int Dt3 { get; set; } = 0;
        public int Dt4 { get; set; } = 0;

        public int Dt1 { get; set; } = 0;
        public string SmartDayId { get; set; } = "";

        public int CustNo { get; set; } = 0;

        public int Gr3 { get; set; } = 0;

        public string Inf5 { get; set; } = "";
        public string Inf4 { get; set; } = "";
        public string Inf { get; set; } = "";

        public string SmartDaySiteId { get; set; } = "";
        public string R8Nm { get; set; } = "";
        public string Ad1 { get; set; } = "";
        public string Ad2 { get; set; } = "";
        public string Ad3 { get; set; } = "";
        public string PNo { get; set; } = "";
        public string PArea { get; set; } = "";

        public int Ctry { get; set; } = 46;

        public string MailAd { get; set; } = "";
        public int ActNoAddress { get; set; } = 0;

        public string Country { get; set; } = "";
  

    }
   
}

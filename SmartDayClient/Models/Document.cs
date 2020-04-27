using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartDayClient.Models
{
    public class Document
    {
        public string name { get; set; } = "";
        public Reference userReference { get; set; }
        public string date { get; set; } = "";
        public string comment { get; set; } = null;
        public string data { get; set; } = "";
        public string handymanDocument { get; set; } = null;
        public string id { get; set; } = "";
        public string externalId { get; set; } = "";

        public Document()
        {
            userReference = new Reference();
        }
    }
}

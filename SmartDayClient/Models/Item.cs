using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartDayClient.Models
{
    public class Item
    {
        public Reference wholesalerReference { get; set; }
        public string description { get; set; } = "";
        public string unit { get; set; } = "";
        public double price { get; set; } = 0.0;
        public double vatRate { get; set; } = 0.0;
        public double orderUnit { get; set; } = 0.0;
        public double quantity { get; set; } = 0.0;
        public int storeMinimum { get; set; } = 0;
        public int storeMaximum { get; set; } = 0;
        public int stockItem { get; set; } = 0;
        public double costPrice { get; set; } = 0.0;
        public string itemNo { get; set; } = "";
        public string itemNo2 { get; set; } = "";
        public string itemGroup { get; set; } = "";
        public string url { get; set; } = "";
        public string id { get; set; } = "";
        public string externalId { get; set; } = "";

        public Item()
        {
            wholesalerReference = new Reference();
        }
    }
}

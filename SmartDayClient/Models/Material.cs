using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartDayClient.Models
{
    public class Material
    {
        public Reference userReference { get; set; }
        public Reference storeReference { get; set; }
        public Reference wholesalerReference { get; set; }
        public string date { get; set; } = "";
        public double quantity { get; set; } = 0.0;
        public string itemNo { get; set; } = "";
        public string description { get; set; } = "";
        public string unit { get; set; } = "";
        public double totalPrice { get; set; } = 0.0;
        public double costPrice { get; set; } = 0.0;
        public double vatRate { get; set; } = 25.0;
        public string itemNo2 { get; set; } = "";
        public bool billable { get; set; } = false;
        public int status { get; set; } = 0;
        public SmartdayMaterial smartdayMaterial { get; set; }
        public HandymanMaterial handymanMaterial { get; set; } = null;
        public string id { get; set; } = "";
        public string externalId { get; set; } = "";
        public Material()
        {
            smartdayMaterial = new SmartdayMaterial();
            userReference = new Reference();
            storeReference = new Reference();
            wholesalerReference = new Reference();
        }
    }

    public class SmartdayMaterial
    {
        public double unitPrice { get; set; } = 0.0;
        public string comment { get; set; } = "";
        public Reference categoryReference { get; set; }

        public SmartdayMaterial()
        {
            categoryReference = new Reference();
        }
    }

    public class HandymanMaterial
    {
        public bool agreedPrice { get; set; } = true;
        public double plannedQuantity { get; set; } = 0.0;
        public string reference { get; set; } = "";
        public string kB1 { get; set; } = "";
        public string kB2 { get; set; } = "";
        public string phase { get; set; } = "";
        public bool makeStoreMovement { get; set; } = false;
        public int resetSyncStatus { get; set; } = 0;
        public List<string> serialNumberList { get; set; }
        public string purchaseOrderWholesalerId { get; set; } = "";
        public string purchaseOrderId { get; set; } = "";
        public string lineNumber { get; set; }
        public string itemGroup { get; set; }
        public Reference siteReference { get; set; }
        public Reference thingReference { get; set; }
        public PickOrder pickOrder { get; set; }
        public ReadonlyFields readonlyFields { get; set; }

        public HandymanMaterial()
        {
            serialNumberList = new List<string>();
            siteReference = new Reference();
            thingReference = new Reference();
        }

    }

    public class PickOrder
    {
        public double orderedQuantity { get; set; } = 0.0;
        public bool complete { get; set; } = false;
        public int serialNumberRequired { get; set; } = 0;
        public string extra1 { get; set; } = "";
        public string extra2 { get; set; } = "";
        public string extra3 { get; set; } = "";
        public string extra4 { get; set; } = "";
        public string extra5 { get; set; } = "";
        public string extra1Label { get; set; } = "";
        public string extra2Label { get; set; } = "";
        public string extra3Label { get; set; } = "";
        public string extra4Label { get; set; } = "";
        public string extra5Label { get; set; } = "";
    }

}

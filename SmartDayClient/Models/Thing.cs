using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartDayClient.Models
{
    public class Thing
    {
        public string serialNumber { get; set; } = "";
        public string name { get; set; } = "";         // R12.Nm
        public string make { get; set; } = "";
        public string model { get; set; } = "";
        public string color { get; set; } = "";
        public string note { get; set; } = "";
        public CategoryReference categoryReference { get; set; }
        public Reference customerReference { get; set; }
        public Reference parentReference { get; set; }
        public Reference userReference { get; set; }
        public string manufacturingDate { get; set; } = "";
        public string installedDate { get; set; } = "";
        public string warrantyStartDate { get; set; } = "";
        public string warrantyEndDate { get; set; } = "";
        public string lastServiceDate { get; set; } = "";
        public string lastRepairDate { get; set; } = "";
        public string state { get; set; } = "";
        public string placement { get; set; } = "";
        public Location lastKnownLocation { get; set; }
        public List<ThingContact> contacts { get; set; }     // name=R12.Inf7, cellPhoneNumber=R12.Ad4
        public TelematicsThing telematicsThing { get; set; }
        public HandymanThing handymanThing { get; set; } = null;
        public SmartdayThing smartdayThing { get; set; }
        public string created { get; set; } = "";
        public string id { get; set; } = "";
        public string externalId { get; set; } = "";

        public Thing()
        {
            categoryReference = new CategoryReference();
            parentReference = new Reference();
            customerReference = new Reference();
            userReference = new Reference();
            lastKnownLocation = new Location();
            contacts = new List<ThingContact>();
            telematicsThing = new TelematicsThing();
            //handymanThing = new HandymanThing();
            smartdayThing = new SmartdayThing();
        }
    }

    public class ThingContact
    {
            public string name { get; set; } = "";
            public string cellPhoneNumber { get; set; } = "";
            public string phoneNumber { get; set; } = "";
            public string email { get; set; } = "";
            public HandymanContact handymanContact { get; set; }

            public ThingContact()
            {
                handymanContact = new HandymanContact();
            }
        }
    

    public class SensorDetail
    {
        public int sensorType { get; set; } = 0;
        public string sensorId { get; set; } = "";
        public double measuredValue { get; set; } = 0.0;
    }

    public class LastKnownSensorData
    {
        public string eventDate { get; set; } = "";
        public string receivedDate { get; set; } = "";
        public List<SensorDetail> sensorDetails { get; set; }

        public LastKnownSensorData()
        {
            sensorDetails = new List<SensorDetail>();
        }
    }

    public class TelematicsThing
    {
        public LastKnownSensorData lastKnownSensorData { get; set; }
        public TelematicsThing()
        {
            lastKnownSensorData = new LastKnownSensorData();
        }
    }


    public class HandymanThing
    {
        public string scannerId { get; set; } = "";
        public Reference equipmentTypeReference { get; set; }
        public Reference departmentReference { get; set; }
        public Reference preferredEmployee1Reference { get; set; }
        public Reference preferredEmployee2Reference { get; set; }
        public double travelDistance { get; set; } = 0.0;
        public int estimatedTravelTime { get; set; } = 0;
        public string equipmentServes { get; set; } = "";
        public string picture { get; set; } = "";
        public string warrantyPeriod { get; set; } = "";

        public int installationOrigin { get; set; } = 0;

        public HandymanThing()
        {
            equipmentTypeReference = new Reference();
            departmentReference = new Reference();
            preferredEmployee1Reference = new Reference();
            preferredEmployee2Reference = new Reference();
        }
       
    }

    public class SmartdayThing
    {
        public string contractType { get; set; } = "";
        public string serviceObjectUsage { get; set; } = "";
        public string addressLastUpdated { get; set; } = "";
    }
}

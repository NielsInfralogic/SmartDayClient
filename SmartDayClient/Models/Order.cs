using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartDayClient.Models
{
    public class Order
    {
        public string createdDate { get; set; } = "";
        public string name { get; set; } = "";
        public string poNumber { get; set; } = "";
        public string startTime { get; set; } = "";
        public string endTime { get; set; } = "";
        public Reference responsibleReference { get; set; }
        public Reference customerReference { get; set; }
        public Reference departmentReference { get; set; }
        public Location location { get; set; }
        public Reference siteReference { get; set; }
        public List<CategoryReferenceOrder> categoryReferences { get; set; }
        public List<Participant> participants { get; set; }
        public HandymanOrder handymanOrder { get; set; } = null;
        public SmartdayOrder smartdayOrder { get; set; }
        public string lastTimeMarkedForExport { get; set; } = "";
        public string id { get; set; } = "";
        public string externalId { get; set; } = "";

        public Order()
        {
            responsibleReference = new Reference();
            customerReference = new Reference();
            departmentReference = new Reference();
            location = new Location();
            siteReference = new Reference();
            categoryReferences = new List<CategoryReferenceOrder>();
            participants = new List<Participant>();
            //handymanOrder = new HandymanOrder();
            smartdayOrder = new SmartdayOrder();
        }
    }


    public class CategoryReferenceOrder
    {
        public int categoryType { get; set; } = 0;
        public string id { get; set; } = "";
        public string externalId { get; set; } = "";
    }

   

    public class ReadonlyFieldsOrder
    {
        public string realStartDate { get; set; } = "";
        public string reasonToRejectID { get; set; } = "";
        public string reasonToRejectText { get; set; } = "";
        public int status { get; set; } = 0;
    }

    public class HandymanParticipant
    {
        public bool mainParticipant { get; set; } = false;
        public string message { get; set; } = "";
        public bool messageRead { get; set; } = false;
        public string startDate { get; set; } = "";
        public string finishDate { get; set; } = "";
        public bool finished { get; set; } = false;
        public ReadonlyFieldsOrder readonlyFields { get; set; }
        public HandymanParticipant()
        {
            readonlyFields = new ReadonlyFieldsOrder();
        }
    }

    public class Participant
    {
        public  Reference userReference { get; set; }
        public HandymanParticipant handymanParticipant { get; set; } = null;
        public Participant()
        {
            userReference = new Reference();
           // handymanParticipant = new HandymanParticipant();
        }
    }

    

    
    public class SecondaryCustomerReference
    {
        public  Reference customerReference { get; set; }
        public bool locked { get; set; } = false;
        public bool usedForInvoicing { get; set; } = false;
        public SecondaryCustomerReference()
        {
            customerReference = new Reference();
        }
    }

    public class ContributionMargin
    {
        public double customerPrice { get; set; } = 0.0;
        public double costPrice { get; set; } = 0.0;
        public double orderPercent { get; set; } = 0.0;
        public double plannedPercent { get; set; } = 0.0;
        public string lastChanged { get; set; } = "";
    }

    

    public class Elements
    {
        public bool general { get; set; } = false;
        public bool customerAddress { get; set; } = false;
        public bool descriptions { get; set; } = false;
        public bool material { get; set; } = false;
        public bool hours { get; set; } = false;
        public bool checklists { get; set; } = false;
        public bool documents { get; set; } = false;
        public bool message { get; set; } = false;
    }

    public class KbValue
    {
        public int level { get; set; } = 0;
        public string value { get; set; } = "";
        public string description { get; set; } = "";
        public string parent { get; set; } = "";
        public double fixedPrice { get; set; } = 0.0;
    }

    public class InvoiceInfo
    {
        public bool invoiceMaterial { get; set; } = false;
        public double discountMaterial { get; set; } = 0.0;
        public bool invoiceWorkHours { get; set; } = false;
        public double discountWorkHours { get; set; } = 0.0;
        public bool invoiceTravel { get; set; } = false;
        public bool invoiceOther { get; set; } = false;
        public double turnOutFee { get; set; } = 0.0;
        public double hourRateMarkup { get; set; } = 0.0;
        public string contractId { get; set; } = "";
        public string startingDate { get; set; } = "";
        public string closingDate { get; set; } = "";
        public int period { get; set; } = 0;
        public bool automaticRenewal { get; set; } = false;
        public int renewalPeriod { get; set; } = 0;
    }

    public class ReadonlyFields2
    {
        public string mainParticipantSignature { get; set; } = "";
        public string mainParticipantSignatureDate { get; set; } = "";
        public int noCustomerSignatureReason { get; set; } = 0;
        public string finishDate { get; set; } = "";
        public int controlled { get; set; } = 0;
        public string customerSignature { get; set; } = "";
        public string customerSignatureDate { get; set; } = "";
        public string customerSignatureName { get; set; } = "";
        public bool completed { get; set; } = false;
        public InvoiceInfo invoiceInfo { get; set; }

        public ReadonlyFields2()
        {
            invoiceInfo = new InvoiceInfo();
        }
    }

    public class DataProcessingOptions
    {
        public int reactivationOption { get; set; } = 0;
        public bool doNotSetAsPassive { get; set; } = false;
        public int dateUpdateOption { get; set; } = 0;
        public bool copyStandardChecklists { get; set; }
        public bool copyStandardCostCenterEntries { get; set; } = false;
        public bool assignDefaultDepartment { get; set; } = false;
    }

    public class HandymanOrder
    {
        public  Reference thingReference { get; set; }
        public int invoiceStatus { get; set; } = 0;
        public bool excludeFromInvoice { get; set; } = false;
        public SecondaryCustomerReference secondaryCustomerReference { get; set; }
        public ContributionMargin contributionMargin { get; set; }
        public int orderType { get; set; } = 0;
        public bool isInternalOrder { get; set; } = false;
        public bool freeOrder { get; set; } = false;
        public int orderFinishedRule { get; set; } = 0;
        public int orderDelay { get; set; } = 0;
        public string mainOrder { get; set; } = "";
        public string referenceFromCustomer { get; set; } = "";
        public bool showInCalendar { get; set; } = false;
        public  Reference contractReference { get; set; }
        public string customerDefectDescription { get; set; } = "";
        public Elements elements { get; set; }
        public string message { get; set; } = "";
        public string workplace { get; set; } = "";
        public string contactName { get; set; } = "";
        public string telephone1 { get; set; } = "";
        public string telephone2 { get; set; } = "";
        public string contactEmail { get; set; } = "";
        public bool quote { get; set; } = false;
        public double fixedPrice { get; set; } = 0.0;
        public double vatRate { get; set; } = 0.0;
        public double estimatedHours { get; set; } = 0.0;
        public int routeSequence { get; set; } = 0;
        public bool semiAutomaticDispatch { get; set; } = false;
        public int participantFinderStatus { get; set; } = 0;
        public string invoiceText { get; set; } = "";
        public bool useKB1 { get; set; } = false;
        public bool useKB2 { get; set; } = false;
        public int usePhaseOption { get; set; } = 0;
        public bool kbInputRequired { get; set; } = false;
        public string defaultKB1 { get; set; } = "";
        public string defaultKB2 { get; set; } = "";
        public string defaultPhase { get; set; } = "";
        public string labelForKBHeading { get; set; } = "";
        public string labelForKB1 { get; set; } = "";
        public string labelForKB2 { get; set; } = "";
        public string labelForPhase { get; set; } = "";
        public List<KbValue> kbValues { get; set; }
        public ReadonlyFields2 readonlyFields { get; set; }
        public DataProcessingOptions dataProcessingOptions { get; set; }

        public HandymanOrder()
        {
            thingReference = new Reference();
            secondaryCustomerReference = new SecondaryCustomerReference();
            contributionMargin = new ContributionMargin();
            contractReference = new Reference();
            elements = new Elements();
            kbValues = new List<KbValue>();
            readonlyFields = new ReadonlyFields2();
            dataProcessingOptions = new DataProcessingOptions();
        }
    }


    public class SmartdayOrder
    {
        public Reference projectReference { get; set; }
        public bool timeslotAgreed { get; set; } = false;
        public bool urgent { get; set; } = false;
        public string owner { get; set; } = "";
        public Reference thingReference { get; set; }
        public int status { get; set; } = 0;

        public string crmInfo1 { get; set; } = "";
        public string crmInfo2 { get; set; } = "";
        public SmartdayOrder()
        {
            projectReference = new Reference();
            thingReference = new Reference();
        }
    }
}

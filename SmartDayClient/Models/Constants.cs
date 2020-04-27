using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartDayClient.Models
{
    public static class CategoryType
    {
        public const int SalaryCode_All = 1;
        public const int Customer_Privat = 0;
        public const int Customer_Erhverv = 1;

        public const int Site_Privat = 0;
        public const int Site_Erhverv = 1;

        public const int Item_Vare = 0;
        public const int Item_Time = 1;
        public const int Item_Tillaeg = 2;
        public const int Item_Tektser = 3;

        public const int Project_Projekt = 0;
        public const int Project_OevrigTidsregistrering = 1;

        public const int Things_Serviceenhed = 0;

        public const int Order_Opgave = 0;
        public const int Order_Tilbud = 1;
        public const int Order_Utbildning = 4;

        public const int Order_TS_MO = 14;     // Tillægssalg montage
        public const int Order_SR = 3;         // Serviceudkald
        public const int Order_SK = 8;         // Servicekontrakt
        public const int Order_SAG_SR = 11;    // Nysalg UDSKIFTNINGER service
        public const int Order_SAG = 10;       // Nysalg anlæg
        public const int Order_GA = 16;        // Garanti SAG<=1 år montageafd

        public const int Order_ForkertOpgavetype= 5;
        public const int Order_FoeretagsbetaltTid = 20;
        public const int Order_Egentid = 19;

    }

    public static class SalaryCodesID
    {
        public const string Vanlige_Timer = "90001";
        public const string OT1_Overtidsersattning = "90002";
        public const string OT2_Overtidsersattning = "90003";
        public const string Standby_Timmar = "90009";
        public const string Verkstads_Timmar   = "90010";

        public const string Koertid = "90020";
        public const string Koertid_hamtning_av_varor = "90021";
        public const string Koertid_hem = "90022";

        public const string Sjuk = "90030";
        public const string Sjukt_barn = "90031";
        public const string Laekare_m_m = "90032";

        public const string Semester = "90040";
        public const string Arbetstidsforkortning = "90041";
        public const string Kompensation_timmar = "90042";
        public const string Obetald_ledighet = "90043";

        public const string Lunch = "90050";
        public const string Oevrigt_Timmar = "90051";

    }

    public static class OrderStatus
    {
        public const int Approved = 8;
        public const int Finished_Aborted = 7;

        public const int Pending = 1;
        public const int Activated = 2;
        public const int Accepted = 3;
        public const int InProgress = 4;
        public const int Awaiting = 41;
        public const int SentHome = 6;
        public const int Balanced = 99;
    }

    public static class ProjectStatus
    {
        public const int Offer = 2;
        public const int Activated = 3;
        public const int Closed = 4;
        public static int Completed = 5;
    }

    public static class MaterialStatus
    {
        public const int Suggensted = 0;
        public const int Used = 1;
        public const int Exported = 2;
        public static int Approved = 3;
    }

    public static class SalaryCodeStatus
    {
        public const int Suggensted = 0;
        public const int Used = 1;
        public const int Exported = 2;
        public static int Approved = 3;
    }


}

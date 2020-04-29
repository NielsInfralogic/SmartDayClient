using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace SmartDayClient
{
    public static class Utils
    {
        public static int ReadConfigInt32(string setting, int defaultValue)
        {
            int ret = defaultValue;
            if (ConfigurationManager.AppSettings[setting] != null)
            {
                try
                {
                    return Convert.ToInt32((string)ConfigurationManager.AppSettings[setting]);
                }
                catch { }
            }

            return ret;
        }

        public static string ReadConfigString(string setting, string defaultValue)
        {
            string ret = defaultValue;
            if (ConfigurationManager.AppSettings[setting] != null)
            {
                try
                {
                    return (string)ConfigurationManager.AppSettings[setting];
                }
                catch { }
            }

            return ret;
        }

        public static string SanitizeKachingName(string s)
        {
            if (s == "")
                return s;

            s = s.ToLower().Replace("-", "_");
            s = s.Replace(" ", "_");
            s = s.Replace(".", "_");
            s = s.Replace("æ", "ae");
            s = s.Replace("ø", "oe");
            s = s.Replace("é", "");
            s = s.Replace("/", "_");

            return s;
        }

        public static void WriteLog(string logoutput)
        {
            Console.WriteLine(logoutput);
            string LogPath = AppDomain.CurrentDomain.BaseDirectory + @"\SmartDayClient.log";
            if (LogPath.ToLower() != "" && LogPath.ToLower() != "stdout")
            {
                try
                {
                    StreamWriter w = File.AppendText(LogPath);
                    w.WriteLine(System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + ": " + logoutput);
                    w.Flush();
                    w.Close();
                }
                catch (Exception)
                {
                }
            }
        }


        public static string GenerateTimeStamp()
        {
            DateTime t = DateTime.Now;
            return string.Format("{0:00}{1:00}{2:00}{3:00}{4:00}{5:00}", t.Year - 2000, t.Month, t.Day, t.Hour, t.Minute, t.Second);
        }
        public static string GenerateTimeStamp(DateTime t)
        {
            return string.Format("{0:0000}-{1:00}-{2:00} {3:00}:{4:00}:{5:00}", t.Year, t.Month, t.Day, t.Hour, t.Minute, t.Second);
        }

        public static string DateTime2String(int type, DateTime t)
        {
            if (type == 0)
                return string.Format("{0:0000}-{1:00}-{2:00}T{3:00}:{4:00}:{5:00}", t.Year, t.Month, t.Day, t.Hour, t.Minute, t.Second);
            else if (type == 1)
                return string.Format("{0:0000}-{1:00}-{2:00}T{3:00}:{4:00}:{5:00}.{6:000}", t.Year, t.Month, t.Day, t.Hour, t.Minute, t.Second, t.Millisecond);

            return "";
        }
        
      


        public static string GenerateDateStamp(DateTime t)
        {
            return string.Format("{0:0000}-{1:00}-{2:00}", t.Year, t.Month, t.Day);
        }

        public static string DateTimeToVismaDateString(DateTime t)
        {
            return string.Format("{0:0000}{1:00}{2:00}", t.Year, t.Month, t.Day);
        }

        public static int StringToInt(string s)
        {
            if (s == "")
                return 0;

            int n = 0;
            Int32.TryParse(s, out n);

            return n;
        }



        public static DateTime VismaDate2DateTime(int dateint)
        {
            ///           20160101
            if (dateint < 10000000)
                return DateTime.MinValue;
            int year = dateint / 10000;
            int month = (dateint - year * 10000) / 100;
            int day = dateint - year * 10000 - month * 100;
            if (year <= 1900 || month < 1 || month > 12 || day < 1 || day > 31)
                return DateTime.MinValue;
            try
            {
                return new DateTime(year, month, day);
            }
            catch
            {
                return DateTime.MinValue;
            }
        }

        public static DateTime VismaDate2DateTime(int dateint, int timeint)
        {
            ///           20160101
            if (dateint < 10000000)
                return DateTime.MinValue;
            int year = dateint / 10000;
            int month = (dateint - year * 10000) / 100;
            int day = dateint - year * 10000 - month * 100;
            int hour = timeint / 100;
            int min = timeint - (hour * 100);
            int sec = 0;
            if (year <= 1900 || month < 1 || month > 12 || day < 1 || day > 31)
                return DateTime.MinValue;
            try
            {
                return new DateTime(year, month, day, hour, min, sec);
            }
            catch
            {
                return DateTime.MinValue;
            }
        }

        public static DateTime VismaDate2DateTimeFromString(string datestring)
        {
            int dateint = 0;
            Int32.TryParse(datestring, out dateint);
            return VismaDate2DateTime(dateint);

        }

        public static int DateTimeToVismaDate(DateTime dt)
        {
            return dt.Year * 10000 + dt.Month * 100 + dt.Day;
        }

        public static string QuoteIt(string s)
        {
            return "'" + s + "'";
        }

        public static string DecimalToString(Decimal dec)
        {
            string s = string.Format("{0:0.00}", dec);
            return s.Replace(',', '.');
        }

        public static decimal StringToDecimal(string s)
        {
            /*

            s = s.Replace(",", ".");
            double f = 0;
            Double.TryParse(s, out f);
            try
            {
                return Convert.ToDecimal(f);
            }
            catch
            {
                return 0;
            }

           */

            CultureInfo culture = CultureInfo.GetCultureInfo("da-DK");

            NumberFormatInfo nfi = culture.NumberFormat;

            NumberStyles style;
            style = NumberStyles.AllowDecimalPoint;
            Decimal number = 0;
            if (Decimal.TryParse(s, style, culture, out number))
                return number;
            else

                return 0;

        }


        public static DateTime StringToDateTime(string s)
        {
            //2016.10.17 13:42:51
            //0123456789012345678
            //2018-10-02T05:35:00",
            if (s.Length < 19)
                return DateTime.MinValue;
            try
            {
                return new DateTime(Int32.Parse(s.Substring(0, 4)), Int32.Parse(s.Substring(5, 2)), Int32.Parse(s.Substring(8, 2)), Int32.Parse(s.Substring(5, 2)), Int32.Parse(s.Substring(11, 2)), Int32.Parse(s.Substring(14, 2)), Int32.Parse(s.Substring(17, 2)));
            }
            catch
            {

            }

            return DateTime.MinValue;
        }

        public static DateTime StringToDate(string s)
        {
            //2016.10.17
            if (s.Length < 10)
                return DateTime.MinValue;
            try
            {
                return new DateTime(Int32.Parse(s.Substring(0, 4)), Int32.Parse(s.Substring(5, 2)), Int32.Parse(s.Substring(8, 2)));
            }
            catch
            {

            }

            return DateTime.MinValue;
        }

        public static string Base64Encode(string plainText)
        {
            if (plainText == "")
                return "";
            var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(plainText);
            return System.Convert.ToBase64String(plainTextBytes);
        }

        public static byte[] Base64EncodeToByteArray(string plainText)
        {
            if (plainText == "")
                return null;
            return System.Text.Encoding.UTF8.GetBytes(plainText);
        }

        public static string Base64Decode(string base64EncodedData)
        {
            if (base64EncodedData == "")
                return "";
            var base64EncodedBytes = System.Convert.FromBase64String(base64EncodedData);
            return System.Text.Encoding.UTF8.GetString(base64EncodedBytes);
        }

        public static string SanitizeName(string name)
        {
            // to bo defined...
            return name.Replace(@"\", "");
        }

        public static string SanitizeUrl(string name)
        {
            // to bo defined...
            name = name.Replace(@"+", "-");
            name = name.Replace(@" ", "-");
            name = name.Replace(@"_", "-");
            name = name.Replace(@",", "-");
            name = name.Replace(@"%", "pct");
            name = name.Replace(@"æ", "ae");
            name = name.Replace(@"ø", "oe");
            name = name.Replace(@"å", "aa");
            name = name.Replace(@"Æ", "AE");
            name = name.Replace(@"Ø", "OE");
            name = name.Replace(@"Å", "AA");

            name = name.Replace(@"ü", "uu");
            name = name.Replace(@"ú", "uu");
            name = name.Replace(@"û", "uu");
            name = name.Replace(@"ä", "aa");
            name = name.Replace(@"á", "aa");
            name = name.Replace(@"â", "aa");
            name = name.Replace(@"ö", "oo");
            name = name.Replace(@"ó", "oo");
            name = name.Replace(@"ô", "oo");
            name = name.Replace(@"ÿ", "yy");

            name = name.Replace(@"ï", "ii");
            name = name.Replace(@"í", "ii");
            name = name.Replace(@"î", "ii");
            name = name.Replace(@"ë", "ee");
            name = name.Replace(@"ê", "ee");
            name = name.Replace(@"é", "ee");
            name = name.Replace(@"Ü", "UU");
            name = name.Replace(@"Ú", "UU");
            name = name.Replace(@"Û", "UU");
            name = name.Replace(@"Ä", "AA");
            name = name.Replace(@"Á", "AA");
            name = name.Replace(@"Â", "AA");
            name = name.Replace(@"Ö", "OO");
            name = name.Replace(@"Ó", "OO");
            name = name.Replace(@"Ô", "OO");
            name = name.Replace(@"Ï", "II");
            name = name.Replace(@"Í", "II");
            name = name.Replace(@"Î", "II");
            name = name.Replace(@"Ë", "EE");
            name = name.Replace(@"Ê", "EE");
            name = name.Replace(@"É", "EE");
            return name;
        }

        public static string CleanForJSON(string s)
        {
            if (s == null || s.Length == 0)
            {
                return "";
            }

            char c = '\0';

            int len = s.Length;
            StringBuilder sb = new StringBuilder(len + 4);
            String t;

            for (int i = 0; i < len; i += 1)
            {
                c = s[i];
                switch (c)
                {

                    case '\\':
                        //sb.Append("\\");
                        //.Append(c);
                        break;
                    case '"':
                        //  case '/':
                        sb.Append("\\");
                        sb.Append(c);
                        break;
                    case '\b':
                        sb.Append("\\b");
                        break;
                    case '\t':
                        sb.Append("\\t");
                        break;
                    case '\n':
                        sb.Append("\\n");
                        break;
                    case '\f':
                        sb.Append("\\f");
                        break;
                    case '\r':
                        sb.Append("\\r");
                        break;
                    default:
                        if (c < ' ')
                        {
                            t = "000" + String.Format("X", c);
                            sb.Append("\\u" + t.Substring(t.Length - 4));
                        }
                        else
                        {
                            sb.Append(c);
                        }
                        break;
                }
            }
            return sb.ToString();
        }

        public static string ReadMemoFile(string memofilename)
        {
            string txt = "";
            if (memofilename == "")
                return "";

            memofilename = memofilename.ToLower();
            memofilename = memofilename.Replace(@"v:", Utils.ReadConfigString("v-drive", ""));
            memofilename = memofilename.Replace(@"f:", Utils.ReadConfigString("f-drive", ""));
            try
            {
                if (File.Exists(memofilename))
                {
                    //txt = (new StreamReader(memofilename, Encoding.Default)).ReadToEnd();
                    txt = File.ReadAllText(memofilename, Encoding.Default);
                }
            }
            catch //(Exception exception)
            {
            }
            return txt;
        }


        public static Sync ReadSyncTime(string syncTimeFileName)
        {
            Sync sync = new Sync() { LastestSync = DateTime.MinValue };

            if (File.Exists(syncTimeFileName))
            {
                using (StreamReader reader = new StreamReader(syncTimeFileName))
                {
                    XmlSerializer xs = new XmlSerializer(typeof(Sync));
                    try
                    {
                        sync = (Sync)xs.Deserialize(reader);
                    }
                    catch (Exception ex)
                    {
                        Utils.WriteLog("Error reading " + syncTimeFileName + " - " + ex.Message);
                    }
                }
            }
            else
                Utils.WriteLog("Sync time file " + syncTimeFileName + " not found");

            return sync;
        }

        public static void WriteSyncTime(Sync sync, string syncTimeFileName)
        {
            DateTime now = DateTime.Now;
            sync.LastestSync = new DateTime(now.Year, now.Month, now.Day, now.Hour, now.Minute, 0);
            XmlSerializer xmlSerializer = new XmlSerializer(typeof(Sync));

            try
            {

                using (FileStream fs = new FileStream(syncTimeFileName, FileMode.Create))
                {
                    XmlSerializer xSer = new XmlSerializer(typeof(Sync));
                    try
                    {
                        xSer.Serialize(fs, sync);
                    }
                    catch
                    {
                        Utils.WriteLog("Error writing " + syncTimeFileName);
                    }
                }
            }
            catch { }

        }

        public static string EncodeJSON(string json)
        {
            //  return JsonConvert.ToString(json).Replace("\"","");
            return JsonConvert.ToString(json).Substring(1, json.Length - 2);
        }

        public static void IsolateStreetNumber(string address, ref string street, ref string housenumber)
        {
            // vej 12
            // 0123456
            //    x
            address = address.Trim();
            street = address;
            housenumber = "";
            int n = address.LastIndexOf(' ');
            int h = 0;
            if (n != -1)
            {
                h = Utils.StringToInt(address.Substring(n + 1));
                if (h>0)
                {
                    housenumber = h.ToString();
                    street = street.Substring(0, n);                   
                }                    
            }

        }

        public static int MapOrderCategory(string category)
        {
            category = category.ToUpper().Trim();

            if (category == "SAG")
                return Models.CategoryType.Order_SAG;
            if (category == "SAG SR")
                return Models.CategoryType.Order_SAG_SR;
            if (category == "TS MO")
                return Models.CategoryType.Order_TS_MO;
            if (category == "SR")
                return Models.CategoryType.Order_SR;
            if (category == "SK")
                return Models.CategoryType.Order_SK;
            if (category == "GA")
                return Models.CategoryType.Order_GA;
            if (category == "TRÆNING")
                return Models.CategoryType.Order_Utbildning;

            return Models.CategoryType.Order_Opgave;

        }

        public static int MapCountry(string ctry)
        {
            ctry = ctry.ToUpper();
            if (ctry == "SE" || ctry == "SWEDEN" || ctry == "SVERIGE")
                return 46;
            if (ctry == "DK" || ctry == "DENMARK" || ctry == "DANMARK")
                return 45;
            if (ctry == "DN" || ctry == "NORWAY" || ctry == "NORGE")
                return 47;

            return 0;

        }

        public static bool WriteToMemoFile(string memoPath, string memotext)
        {
            memoPath = memoPath.ToLower();
            memoPath = memoPath.Replace(@"v:", Utils.ReadConfigString("v-drive", @"\\File10\k_4018$\Visma"));
            memoPath = memoPath.Replace(@"f:", Utils.ReadConfigString("f-drive", @"\\File10\k_4018$\Visma"));
            memoPath = memoPath.Replace(@"g:", Utils.ReadConfigString("g-drive", @"\\192.168.100.31\group"));

            try
            {
                if (!File.Exists(memoPath))
                    Directory.CreateDirectory(Path.GetDirectoryName(memoPath));
                else
                    File.Delete(memoPath);

                using (FileStream fileStream = new FileStream(memoPath, FileMode.OpenOrCreate, FileAccess.Write))
                {
                    using (StreamWriter streamWriter = new StreamWriter(fileStream, Encoding.Default))
                    {
                        streamWriter.Write(memotext);
                        streamWriter.Flush();
                        streamWriter.Close();
                    }
                }
            }
            catch (Exception exception)
            {
                WriteLog("Error writing memo file " + memoPath + " - " + exception.Message);
                return false;
            }

            return true;
        }




        public static String ReadFileToBase64(string fileName)
        {
            String encodedfile = "";
            try
            {
                byte[] bytes = File.ReadAllBytes(fileName);
                encodedfile = Convert.ToBase64String(bytes);
            }
            catch (Exception e)
            {
                WriteLog($"Error: ReadFileToBase64() - {e.Message}");
                return "";
            }
            return encodedfile;
        }


        public static bool WriteFileFromBase64(string data, string fileName)
        {           
            try
            {
                byte[] tempBytes = Convert.FromBase64String(data);
                File.WriteAllBytes(fileName, tempBytes);
            }
            catch (Exception e)
            {
                WriteLog($"Error: WriteFileFromBase64() - {e.Message}");
                return false;
            }
            return true;
        }
    }

}

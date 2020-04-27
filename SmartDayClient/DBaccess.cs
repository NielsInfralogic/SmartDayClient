using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartDayClient
{
    public class DBaccess
    {
        readonly SqlConnection connection;

        public static string queryGetCustomers = "SELECT Actor.ActNo, Actor.CustNo,Actor.Nm,Actor.Ad1,Actor.Ad2,Actor.Ad3,Actor.PNo,Actor.PArea,Actor.Ctry,ISNULL(Ctry.ISO,'SE'),Actor.Phone,Actor.MobPh,Actor.MailAd,Actor.BsNo,Actor.YrRef,Actor.Gr3,Actor.Inf7,Actor.NoteNm, Actor.Gr FROM Actor " +
                        "LEFT OUTER JOIN Ctry ON Actor.Ctry = Ctry.CtryNo WHERE Actor.DelToAct=0 AND Actor.CustNo>0 AND Actor.Gr3<>10  ";

        public static string queryGetCustomerContact = "SELECT Actor.ActNo,Actor.Nm,Actor.Ad1,Actor.Ad2,Actor.Ad3,Actor.PNo,Actor.PArea,ISNULL(Ctry.ISO,'SE'),Actor.Phone,Actor.MobPh,Actor.MailAd,Actor.BsNo FROM Actor " +
                        "LEFT OUTER JOIN Ctry ON Actor.Ctry = Ctry.CtryNo " +
                        "WHERE Actor.LiaAct=#1#";

        public static string queryGetCustomerDelivery = "SELECT Actor.ActNo, Actor.Nm,Actor.Ad1,Actor.Ad2,Actor.Ad3,Actor.PNo,Actor.PArea,ISNULL(Ctry.ISO,'SE'),Actor.Phone,Actor.MobPh,Actor.MailAd,Actor.BsNo FROM Actor " +
                        "LEFT OUTER JOIN Ctry ON Actor.Ctry = Ctry.CtryNo " +
                        "WHERE Actor.DelToAct=#1#";

        public static string queryGetCustomerAddresses = "SELECT Actor.ActNo, Actor.Nm,Actor.Ad1,Actor.Ad2,Actor.Ad3,Actor.PNo,Actor.PArea,Actor.Ctry,ISNULL(Ctry.ISO,'SE'),Actor.Phone,Actor.MobPh,Actor.MailAd,Actor.BsNo,Actor.Inf7,Actor.NoteNm FROM Actor " +
                        "LEFT OUTER JOIN Ctry ON Actor.Ctry = Ctry.CtryNo " +
                       "WHERE Actor.Gr3=10 AND Actor.CustNo=#1#";

        public static string queryGetCustomerAddressFromActNo = "SELECT Actor.ActNo, Actor.Nm,Actor.Ad1,Actor.Ad2,Actor.Ad3,Actor.PNo,Actor.PArea,Actor.Ctry,ISNULL(Ctry.ISO,'SE'),Actor.Phone,Actor.MobPh,Actor.MailAd,Actor.BsNo,Actor.Inf7,Actor.NoteNm FROM Actor " +
                        "LEFT OUTER JOIN Ctry ON Actor.Ctry = Ctry.CtryNo " +
                       "WHERE Actor.ActNo=#1# AND Actor.Gr3=10 ";

        public static string queryGetOrders = "SELECT DISTINCT Ord.Gr12, Ord.OrdDt, Ord.OrdNo, Ord.Nm, Ord.Ad1, Ord.Ad2, Ord.Ad3, Ord.PNo, Ord.PArea, " +
                       "Ord.DelNm, Ord.DelAd1, Ord.DelAd2, Ord.DelAd3, Ord.DelPNo, Ord.DelPArea, Ord.DelTrm, Ord.DelDt, Ord.Inf, " +
                       "Ord.Inf2, Ord.Inf3, Ord.Inf4,Ord.Inf5, Ord.NoteNm, Ord.YrRef, Ord.OurRef,Ord.Gr3,Ord.CsOrdNo,Ord.CustNo,Ord.EmpNo,Ord.R7,Ord.Gr,Ord.CreUsr,Ord.R12,Ord.R2,Ord.SelBuy,Ord.Reqno,Ord.Gr8 " +
                       "FROM Ord WITH (NOLOCK) WHERE Ord.Gr3=10 AND Ord.TrTp=1  AND Ord.CustNo>0"; // AND Ord.R2 > 0

        public static string queryGetOrderLinesByOrdNo =
                      "SELECT DISTINCT OrdLn.LnNo, OrdLn.ProdNo, OrdLn.NoInvoAb, OrdLn.Price, OrdLn.Descr,OrdLn.WebPg,OrdLn.R7 " +
                      "FROM OrdLn WITH (NOLOCK) WHERE OrdLn.OrdNo= #1#";

        // NOT USED
        public static string queryGetProducts =
                    "SELECT  DISTINCT Prod.ProdNo, Prod.Descr, ISNULL(Unit.Descr,'stk'),ISNULL(Txt.txt,''),ISNULL(TaxAcGr.Descr,'') FROM Prod " +
                    "LEFT OUTER JOIN Unit ON Unit.Un=Prod.StSaleUn " +
                    "LEFT OUTER JOIN Txt ON Txt.TxtTp=42 AND Txt.Lang=46 AND Txt.TxtNo=Prod.Gr " +
                    "LEFT OUTER JOIN TaxAcGr ON TaxAcGr.ProdGrNo=Prod.ProdGr " +
                    "WHERE Prod.Gr4=1 AND Prod.Gr=1";


        public static string queryGetThings =
                    "SELECT DISTINCT R12.RNo, R12.Nm, ISNULL(Actor.Phone,''),R12.Inf7,R12.Ad4,R12.R3,ISNULL(R3.R9,''),R12.NoteNm,R12.Dt3,R12.Dt4,R12.Dt1,R12.Inf8,R12.CustNo,R12.Gr3,R12.Inf5,R12.Inf4,R12.Inf,R12.PictFNm,ISNULL(R8.Nm,''),R12.Ad1,R12.Ad2,R12.Ad3,R12.PNo,R12.PArea,R12.Ctry,R12.MailAd,R12.ActNo2 FROM R12 " +
                    "LEFT OUTER JOIN Actor ON Actor.CustNo=R12.CustNo " +
                    "LEFT OUTER JOIN R3 ON R3.RNo= R12.R3 " +
                    "LEFT OUTER JOIN R8 ON R8.RNo= R12.R8 " +
                    "WHERE (R12.Gr3=1 OR R12.Gr3=5) ";



        // select * from prod where ProdPrGr=5 - timer
        // ProdPrGr=6 gebyr og fragt
        // ProdPrGr=7
        // ProdPrGr=4 kørsel
        // ProdPrGr=0,1,2,3

        public static string queryGetPrice =
                   "SELECT DISTINCT SalePr FROM PrDcMat WHERE SalePr<>0  " +
                   "AND Prodno = '#1#' AND CustPrGr=0 AND CustNo=0 " +
                   "AND (FrDt< #2# OR FrDt=0) AND (ToDt >= #2# OR ToDt = 0) AND Cur=46";

        public static string queryGetAgreementLinesByOrdNo =
                    "SELECT Agr.AgrActNo, Agr.AgrNo,Agr.Descr,Agr.FrDt,Agr.ToDt,CAST(Agr.FrTm as int),CAST(Agr.ToTm as int),Agr.ProdNo,Agr.NoInvoAb,Agr.Price,ISNULL(Actor.CustNo,0),ISNULL(Actor.Nm,''), Agr.Gr3 FROM Agr " +
                    "LEFT OUTER JOIN Actor ON Actor.ActNo=Agr.ActNo " +
                    "WHERE Agr.OrdNo= #1# ORDER BY Agr.Srt";

        public static string queryGetProjects =
                    "SELECT R2.RNo,R2.St,R2.CustNo, R2.Nm, R2.Ad1, R2.Ad2, R2.PNo, R2.PArea, R2.Gr11,R2.CreDt, R2.Rsp,ISNULL(A.Nm,''),R2.EStDt,R2.EEndDt,R2.Dt3,R2.R8,ISNULL(R8.Nm,''),R2.Gr,R2.Inf7,R2.R12 FROM R2 " +
                    "LEFT OUTER JOIN Actor A ON A.EmpNo=R2.Rsp  AND R2.Rsp>0 " +
                    "LEFT OUTER JOIN R8 ON R2.R8=R8.RNo " +
                    "WHERE R2.Gr12=5 AND R2.CustNo>0 ";
           

        public DBaccess()
        {
            connection = new SqlConnection(Utils.ReadConfigString("Connectionstring", ""));
        }
        public DBaccess(int mode)
        {
            if (mode == 1)
                connection = new SqlConnection(Utils.ReadConfigString("Connectionstring", ""));
            else
                connection = new SqlConnection(Utils.ReadConfigString("Connectionstring2", ""));
        }

        public void CloseAll()
        {
            if (connection.State == ConnectionState.Open)
                connection.Close();
        }

        public bool TestConnection(out string errmsg)
        {
            errmsg = "";

            SqlCommand command = new SqlCommand("SELECT TOP 1 * FROM Prod", connection)
            {
                CommandType = CommandType.Text,
                CommandTimeout = 500
            };
            SqlDataReader reader = null;

            try
            {
                if (connection.State == ConnectionState.Closed || connection.State == ConnectionState.Broken)
                    connection.Open();
                reader = command.ExecuteReader();
                if (reader.Read())
                {
                    ;
                }
            }
            catch (Exception ex)
            {
                errmsg = ex.Message;
                return false;
            }
            finally
            {
                // always call Close when done reading .
                if (reader != null)
                    reader.Close();
                if (connection.State == ConnectionState.Open)
                    connection.Close();
                command.Dispose();
            }

            return true;

        }

        /// <summary>
        /// Get database date in Visma form (int)
        /// </summary>
        /// <param name="errmsg"></param>
        /// <returns></returns>
        public int GetCurrentVismaDate(out string errmsg)
        {
            int dt = 0;
            errmsg = "";

            SqlCommand command = new SqlCommand("SELECT GETDATE()", connection)
            {
                CommandType = CommandType.Text
            };

            SqlDataReader reader = null;

            try
            {
                if (connection.State == ConnectionState.Closed || connection.State == ConnectionState.Broken)
                    connection.Open();

                reader = command.ExecuteReader();

                if (reader.Read())
                {
                    DateTime t = reader.GetDateTime(0);
                    dt = t.Year * 10000 + t.Month * 100 + t.Day;
                }
            }
            catch (Exception ex)
            {
                errmsg = "GetCurrentVismaDate() - " + ex.Message;

                return 0;
            }
            finally
            {
                if (reader != null)
                    reader.Close();
                if (connection.State == ConnectionState.Open)
                    connection.Close();
                command.Dispose();
            }

            return dt;

        }

        public bool GetProject(int rno, ref Models.VismaProject project, out string errmsg)
        {
            List<Models.VismaProject> projects = new List<Models.VismaProject>();
            if (GetProjects(rno, ref projects, out errmsg) == false)
                return false;
            if (projects.Count > 0)
                project = projects[0];
            else
                project.ProjectRno = 0;

            return true;
        }

        public bool GetProjects(int specificRno, ref List<Models.VismaProject> projects, out string errmsg)
        {
            errmsg = "";
            string sql = queryGetProjects;
            if (specificRno > 0)
                sql += $" AND R2.RNo={specificRno}";
            SqlCommand command = new SqlCommand(sql, connection)
            {
                CommandType = CommandType.Text,
                CommandTimeout = 600
            };

            SqlDataReader reader = null;

            try
            {
                if (connection.State == ConnectionState.Closed || connection.State == ConnectionState.Broken)
                    connection.Open();

                reader = command.ExecuteReader();

                while (reader.Read())
                {
                    int idx = 0;
                    projects.Add(new Models.VismaProject()
                    {
                        ProjectRno = reader.GetInt32(idx++),       // externalID
                        ProjectTypeSt = reader.GetInt32(idx++),            // St
                        CustomerNo = reader.GetInt32(idx++),
                        Name = reader.GetString(idx++).Trim(),
                        Address1 = reader.GetString(idx++).Trim(),
                        Address2 = reader.GetString(idx++).Trim(),
                        PostCode = reader.GetString(idx++).Trim(),
                        PostalArea = reader.GetString(idx++).Trim(),
                        StatusGroup11 = reader.GetInt32(idx++), 
                        CreateDate = reader.GetInt32(idx++),
                        ResponsibleEmpNo = reader.GetInt32(idx++),
                        ResponsibleName = reader.GetString(idx++).Trim(),
                        EstimatedStartDt = reader.GetInt32(idx++),
                        EstimatedEndDt = reader.GetInt32(idx++),
                        DeadlineDt = reader.GetInt32(idx++),  // Dt3
                        ServiceUnitGroupRno = reader.GetString(idx++).Trim(),  // Anlægsgruppe R8
                        ServiceUnitTypeName = reader.GetString(idx++).Trim(),
                        ServiceUnitQuantity = reader.GetInt32(idx++),
                        Inf7SmartDayProjectID = reader.GetString(idx++).Trim(),
                        ServiceUnitRno = reader.GetString(idx++).Trim(),

                    });
                }

            }
            catch (Exception ex)
            {
                errmsg = ex.Message;

                return false;
            }
            finally
            {
                if (reader != null)
                    reader.Close();
                if (connection.State == ConnectionState.Open)
                    connection.Close();
                command.Dispose();
            }

            return true;

        }


        public bool GetThing(string rno, ref Models.VismaThing thing, out string errmsg)
        {
            List<Models.VismaThing> things = new List<Models.VismaThing>();
            if (GetThings(rno, ref things, DateTime.MinValue, out errmsg) == false)
                return false;
            if (things.Count > 0)
                thing = things[0];

            return true;

        }

        public bool GetThingFromSmartdayID(string smartdayID, ref Models.VismaThing thing, out string errmsg)
        {
            errmsg = "";
            thing.RNo = "";

            string sql = $"SELECT TOP 1 Rno FROM R12 WHERE R12.Inf8='{smartdayID}'";

            SqlCommand command = new SqlCommand(sql, connection)
            {
                CommandType = CommandType.Text,
                CommandTimeout = 600
            };

            SqlDataReader reader = null;

            try
            {
                if (connection.State == ConnectionState.Closed || connection.State == ConnectionState.Broken)
                    connection.Open();

                reader = command.ExecuteReader();

                if (reader.Read())
                {
                    thing.RNo = reader.GetString(0);
                }

            }
            catch (Exception ex)
            {
                errmsg = ex.Message;

                return false;
            }
            finally
            {
                if (reader != null)
                    reader.Close();
                if (connection.State == ConnectionState.Open)
                    connection.Close();
                command.Dispose();
            }

            if (thing.RNo != "")
            {
                List<Models.VismaThing> things = new List<Models.VismaThing>();
                if (GetThings(thing.RNo, ref things, DateTime.MinValue, out errmsg) == false)
                    return false;

                if (things.Count > 0)
                {
                    thing = things[0];
                }
            }

            return true;

        }


        public bool GetThingsForOrder(int ordNo, ref List<string> R12List, out string errmsg)
        {
            errmsg = "";
            R12List.Clear();
            if (ordNo <= 0)
                return true;

            string sql = "SELECT DISTINCT R12.RNo FROM R12 " +
                "INNER JOIN FreeInf1 ON FreeInf1.R12=R12.RNo AND LTRIM(RTRIM(FreeInf1.R12))<>'' " +
                $"AND FreeInf1.OrdNo={ordNo}";

            SqlCommand command = new SqlCommand(sql, connection)
            {
                CommandType = CommandType.Text,
                CommandTimeout = 600
            };

            SqlDataReader reader = null;

            try
            {
                if (connection.State == ConnectionState.Closed || connection.State == ConnectionState.Broken)
                    connection.Open();

                reader = command.ExecuteReader();

                while (reader.Read())
                {
                    int idx = 0;
                    string rNo = reader.GetString(idx++).Trim(); // =R12.Rno
                    if (rNo != "")
                        R12List.Add(rNo);
                }

            }
            catch (Exception ex)
            {
                errmsg = ex.Message;

                return false;
            }
            finally
            {
                if (reader != null)
                    reader.Close();
                if (connection.State == ConnectionState.Open)
                    connection.Close();
                command.Dispose();
            }

            return true;
        }

    
        public bool GetThings(string specificRno, ref List<Models.VismaThing> things, DateTime lastSyncTime, out string errmsg)
        {
            errmsg = "";
            things.Clear();
           
            string sql = queryGetThings;
            if (specificRno != "")
                sql += $" AND R12.RNo='{specificRno}' ";

            if (lastSyncTime != DateTime.MinValue)
            {
                int vismaDt = lastSyncTime.Year * 10000 + lastSyncTime.Month * 100 + lastSyncTime.Day;
                int vismaTm = lastSyncTime.Hour * 100 + lastSyncTime.Minute;
                if (lastSyncTime.Year < 2000)
                {
                    vismaDt = 0;
                    vismaTm = 0;
                }

                sql += "AND ((R12.ChDt=0) OR (R12.ChDt > #1#) OR (R12.ChDt = #1# AND R12.ChTm >= #2#)) ";
                sql = sql.Replace("#1#", vismaDt.ToString()).Replace("#2#", vismaTm.ToString());
            }

            SqlCommand command = new SqlCommand(sql, connection)
            {
                CommandType = CommandType.Text,
                CommandTimeout = 600
            };

            SqlDataReader reader = null;

            try
            {
                if (connection.State == ConnectionState.Closed || connection.State == ConnectionState.Broken)
                    connection.Open();

                reader = command.ExecuteReader();

                while (reader.Read())
                {
                    int idx = 0;
                    things.Add(new Models.VismaThing()
                    {

                        RNo = reader.GetString(idx++), // =R12.Rno
                        Name = reader.GetString(idx++).Trim(), // Nm
                        Phone = reader.GetString(idx++).Trim(),
                        Inf7 = reader.GetString(idx++).Trim(),    // Inf7
                        Ad4 = reader.GetString(idx++).Trim(),       // R12.Ad4
                        R3 = reader.GetInt32(idx++).ToString(), // R12.R3
                        R3R9 = reader.GetString(idx++), //R12.R3 -> R3.R9

                        Memo = Utils.ReadMemoFile(reader.GetString(idx++).Trim()),    // R12.Note
                        Dt3 = reader.GetInt32(idx++),
                        Dt4 = reader.GetInt32(idx++),
                        Dt1 = reader.GetInt32(idx++),
                        
                        Inf8 = reader.GetString(idx++).Trim(),
                        CustNo = reader.GetInt32(idx++),
                        Gr3 = reader.GetInt32(idx++),
                        
                        Inf5 = reader.GetString(idx++).Trim(),
                        Inf4 = reader.GetString(idx++).Trim(),
                        Inf = reader.GetString(idx++).Trim(),    // Inf

                        PictFNm = reader.GetString(idx++).Trim(), //PictFNm
                        R8Nm = reader.GetString(idx++).Trim(), //R8.Nm
                        Ad1 = reader.GetString(idx++).Trim(), 
                        Ad2= reader.GetString(idx++).Trim(),
                        Ad3 = reader.GetString(idx++).Trim(),
                        PNo = reader.GetString(idx++).Trim(),
                        PArea = reader.GetString(idx++).Trim(),
                        Ctry = reader.GetInt32(idx++),
                        MailAd = reader.GetString(idx++).Trim(),
                        ActNoAddress = reader.GetInt32(idx++),

                    });
            
                }

            }
            catch (Exception ex)
            {
                errmsg = ex.Message;

                return false;
            }
            finally
            {
                if (reader != null)
                    reader.Close();
                if (connection.State == ConnectionState.Open)
                    connection.Close();
                command.Dispose();
            }

            return true;
        }

        public bool CreateThing(Models.VismaThing vthing, out string errmsg)
        {
            errmsg = "";

            // We CAN use Rno

            // Save Memo
            string memoFile = "";
            if (vthing.Memo != "")
            {
                memoFile = Utils.ReadConfigString("MemoPath", @"v:\Memo\F4182") + @"\M" + Utils.GenerateTimeStamp() + ".txt";
                Utils.WriteToMemoFile(memoFile, vthing.Memo);
            }

            int gr11 = 0;
            int gr12 = 0;
            string[] refs = vthing.RNo.Split('-');
            if (refs.Length > 0)
                gr11 = Utils.StringToInt(refs[0]);
            if (refs.Length > 1)
                gr12 = Utils.StringToInt(refs[1]);

            int chDt = DateTime.Now.Year * 10000 + DateTime.Now.Month * 100 + DateTime.Now.Day;
            int chTm = DateTime.Now.Hour * 100 + DateTime.Now.Minute;
            string sql = "INSERT INTO R12 (Rno,Nm,Ad1,Ad2,Ad3,Ad4,PNo,PArea,Ctry,CustNo,Gr3,G11,G12,Inf,Inf4,Inf5,Inf7,Inf8,PictFNm,NoteNm,Dt1,Dt3,Dt4,ChDt,ChTm,CreDt,CreTm,CreUsr) " +
                        " VALUES " +
                        $" ('{vthing.RNo}','{vthing.Name}','{vthing.Ad1}','{vthing.Ad2}','{vthing.Ad3}','{vthing.Ad4}','{vthing.PNo}','{vthing.PArea}',{vthing.Ctry},{vthing.CustNo},{vthing.Gr3},{gr11},{gr12},'{vthing.Inf}','{vthing.Inf4}','{vthing.Inf5}','{vthing.Inf7}','{vthing.Inf8}','{vthing.PictFNm}','{memoFile}',{vthing.Dt1},{vthing.Dt3},{vthing.Dt4},{chDt},{chTm},{chDt},{chTm},'system')";

            SqlCommand command = new SqlCommand(sql, connection)
            {
                CommandType = CommandType.Text,
                CommandTimeout = 600
            };

            try
            {
                if (connection.State == ConnectionState.Closed || connection.State == ConnectionState.Broken)
                    connection.Open();

                command.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                errmsg = "GetOrderLines() -" + ex.Message;

                return false;
            }
            finally
            {
                if (connection.State == ConnectionState.Open)
                    connection.Close();
                command.Dispose();
            }


            return true;
        }
  

        public bool GetCustomerSmartDayId(int custNo, ref string smartDayId, out string errmsg)
        {
            smartDayId = "";
            errmsg = "";

            string sql = $"SELECT Inf7 FROM Actor WHERE CustNo={custNo} AND Gr3<>10";

            SqlCommand command = new SqlCommand(sql, connection)
            {
                CommandType = CommandType.Text,
                CommandTimeout = 600
            };

            SqlDataReader reader = null;

            try
            {
                if (connection.State == ConnectionState.Closed || connection.State == ConnectionState.Broken)
                    connection.Open();

                reader = command.ExecuteReader();

                if (reader.Read())
                {
                    smartDayId = reader.GetString(0).Trim();
                }

            }
            catch (Exception ex)
            {
                errmsg = ex.Message;

                return false;
            }
            finally
            {
                if (reader != null)
                    reader.Close();
                if (connection.State == ConnectionState.Open)
                    connection.Close();
                command.Dispose();
            }

            return true;
        }

        public bool GetCustomer(int custNo, ref Models.VismaCustomer customer, out string errmsg)
        {
            errmsg = "";
            string sql = queryGetCustomers + " AND Actor.CustNo=" + custNo.ToString();

            SqlCommand command = new SqlCommand(sql, connection)
            {
                CommandType = CommandType.Text,
                CommandTimeout = 600
            };

            SqlDataReader reader = null;

            try
            {
                if (connection.State == ConnectionState.Closed || connection.State == ConnectionState.Broken)
                    connection.Open();

                reader = command.ExecuteReader();

                if (reader.Read())
                {
                    int idx = 0;
                    customer.ActorNo = reader.GetInt32(idx++);
                    customer.CustomerNo = reader.GetInt32(idx++);
                    customer.Name = reader.GetString(idx++).Trim();
                    customer.AddressLine1 = reader.GetString(idx++).Trim();
                    customer.AddressLine2 = reader.GetString(idx++).Trim();
                    customer.AddressLine3 = reader.GetString(idx++).Trim();
                    customer.PostCode = reader.GetString(idx++).Trim();
                    customer.PostalArea = reader.GetString(idx++).Trim();
                    customer.CountryNumber = reader.GetInt32(idx++);
                    customer.CountryCode = reader.GetString(idx++).Trim();
                    customer.Phone = reader.GetString(idx++).Trim();
                    customer.Mobile = reader.GetString(idx++).Trim();
                    customer.EmailAddress = reader.GetString(idx++).Trim();
                    customer.CompanyNo = reader.GetString(idx++).Trim();
                    customer.YourRef = reader.GetString(idx++).Trim();
                    customer.Group3 = reader.GetInt32(idx++);
                    customer.Inf7 = reader.GetString(idx++).Trim();
                    customer.Memo = Utils.ReadMemoFile(reader.GetString(idx++).Trim());
                    customer.Group1 = reader.GetInt32(idx++);
                }

            }
            catch (Exception ex)
            {
                errmsg = ex.Message;

                return false;
            }
            finally
            {
                if (reader != null)
                    reader.Close();
                if (connection.State == ConnectionState.Open)
                    connection.Close();
                command.Dispose();
            }


            List<Models.VismaActor> actors = new List<Models.VismaActor>();
            if (GetCustomerContact(customer.ActorNo, ref actors, out errmsg) == false)
                return false;
            foreach (Models.VismaActor actor in actors)
                customer.ContactList.Add(actor);

            List<Models.VismaActor> addresses = new List<Models.VismaActor>();
            if (GetCustomerAddresses(customer.CustomerNo, ref addresses, out errmsg) == false)
                return false;
            foreach (Models.VismaActor address in addresses)
                customer.AddressList.Add(address);


            if (customer.AddressList.Count == 0)
            {
                Models.VismaActor defatulAddress = new Models.VismaActor()
                {
                    AddressLine1 = customer.AddressLine1,
                    AddressLine2 = customer.AddressLine2,
                    AddressLine3 = customer.AddressLine3,
                    PostCode = customer.PostCode,
                    PostalArea = customer.PostalArea,
                    Name = customer.Name,
                    CountryNumber = customer.CountryNumber,
                    CountryCode = customer.CountryCode,
                    Phone = customer.Phone,
                    Mobile = customer.Mobile,
                    EmailAddress = customer.EmailAddress,

                };
            }
            return true;
        }

        public bool GetCustomerActNo(int custNo, ref int actNo, out string errmsg)
        {
            errmsg = "";
            string sql = $"SELECT TOP 1 ActNo FROM Actor WHERE CustNo={custNo} AND Gr3<>10";

            SqlCommand command = new SqlCommand(sql, connection)
            {
                CommandType = CommandType.Text,
                CommandTimeout = 600
            };

            SqlDataReader reader = null;

            try
            {
                if (connection.State == ConnectionState.Closed || connection.State == ConnectionState.Broken)
                    connection.Open();

                reader = command.ExecuteReader();

                if (reader.Read())
                {
                    actNo = reader.GetInt32(0);
                }

            }
            catch (Exception ex)
            {
                errmsg = ex.Message;

                return false;
            }
            finally
            {
                if (reader != null)
                    reader.Close();
                if (connection.State == ConnectionState.Open)
                    connection.Close();
                command.Dispose();
            }

            return true;
        }

        public bool GetNewCustomers(ref List<Models.VismaCustomer> customers, DateTime lastSyncTime, out string errmsg)
        {
            errmsg = "";
            customers.Clear();

            


            string sql = queryGetCustomers + " AND (Actor.Gr3=1 OR Actor.Gr3=5) ";

            if (lastSyncTime != DateTime.MinValue)
            {
                int vismaDt = lastSyncTime.Year * 10000 + lastSyncTime.Month * 100 + lastSyncTime.Day;
                int vismaTm = lastSyncTime.Hour * 100 + lastSyncTime.Minute;
                if (lastSyncTime.Year < 2000)
                {
                    vismaDt = 0;
                    vismaTm = 0;
                }

                sql += " AND ((Actor.ChDt=0) OR (Actor.ChDt > #1#) OR (Actor.ChDt = #1# AND Actor.ChTm >= #2#)) ";

                sql = sql.Replace("#1#", vismaDt.ToString()).Replace("#2#", vismaTm.ToString());
            }

            SqlCommand command = new SqlCommand(sql, connection)
            {
                CommandType = CommandType.Text,
                CommandTimeout = 600
            };

            SqlDataReader reader = null;

            try
            {
                if (connection.State == ConnectionState.Closed || connection.State == ConnectionState.Broken)
                    connection.Open();

                reader = command.ExecuteReader();

                while (reader.Read())
                {
                    int idx = 0;
                    customers.Add(new Models.VismaCustomer()
                    {
                        ActorNo = reader.GetInt32(idx++),
                        CustomerNo = reader.GetInt32(idx++),
                        Name = reader.GetString(idx++).Trim(),
                        AddressLine1 = reader.GetString(idx++).Trim(),
                        AddressLine2 = reader.GetString(idx++).Trim(),
                        AddressLine3 = reader.GetString(idx++).Trim(),
                        PostCode = reader.GetString(idx++).Trim(),
                        PostalArea = reader.GetString(idx++).Trim(),
                        CountryNumber = reader.GetInt32(idx++),
                        CountryCode = reader.GetString(idx++).Trim(),
                        Phone = reader.GetString(idx++).Trim(),
                        Mobile = reader.GetString(idx++).Trim(),
                        EmailAddress = reader.GetString(idx++).Trim(),
                        CompanyNo = reader.GetString(idx++).Trim(),
                        YourRef = reader.GetString(idx++).Trim(),
                        Group3 = reader.GetInt32(idx++),
                        Inf7 = reader.GetString(idx++),
                        Memo = Utils.ReadMemoFile(reader.GetString(idx++).Trim()),
                        Group1 = reader.GetInt32(idx++),
                    });
                }

            }
            catch (Exception ex)
            {
                errmsg = ex.Message;

                return false;
            }
            finally
            {
                if (reader != null)
                    reader.Close();
                if (connection.State == ConnectionState.Open)
                    connection.Close();
                command.Dispose();
            }

            foreach (Models.VismaCustomer customer in customers)
            {
                List<Models.VismaActor> addresses = new List<Models.VismaActor>();
                if (GetCustomerAddresses(customer.CustomerNo, ref addresses, out errmsg) == false)
                    return false;
               /* if (addresses.Count == 0)
                {
                    Models.VismaActor address = new Models.VismaActor()
                    {
                        ActorNo = customer.ActorNo,
                        Name = customer.Name,
                        AddressLine1 = customer.AddressLine1,
                        AddressLine2 = customer.AddressLine2,
                        AddressLine3= customer.AddressLine3,
                        AddressLine4 = customer.AddressLine4,
                        PostalArea = customer.PostalArea,
                        PostCode = customer.PostCode,
                        CountryCode = customer.CountryCode,
                        CountryNumber = customer.CountryNumber,
                        EmailAddress = customer.EmailAddress,
                        Mobile = customer.Mobile,
                        Phone = customer.Phone, 
                
                    };
                     
                    // Insert in address to ensure we have a reference for site..
                    if (InsertAddress(ref address, out errmsg) == false)
                        return false;
                    addresses.Add(address);
                }*/

                foreach (Models.VismaActor address in addresses)
                    customer.AddressList.Add(address);

                List<Models.VismaActor> actors = new List<Models.VismaActor>();
                if (GetCustomerContact(customer.ActorNo, ref actors, out errmsg) == false)
                    return false;
                foreach (Models.VismaActor actor in actors)
                    customer.ContactList.Add(actor);
                if (customer.ContactList.Count == 0)
                {
                    customer.ContactList.Add(new Models.VismaActor()
                    {
                        Name = customer.YourRef,
                        EmailAddress = customer.EmailAddress,
                        Phone = customer.Phone,
                        Mobile = customer.Mobile,
                        ActorNo = customer.ActorNo,
                    });
                }
 //               if (GetCustomerDelivery(customer.ActorNo, ref actors, out errmsg) == false)
   //                 return false;
     //           foreach (Models.VismaActor actor in actors)
       //             customer.DeliveryAddressList.Add(actor);
            }

            return true;
        }

        public bool GetCustomerContact(int actNo, ref List<Models.VismaActor> contacts, out string errmsg)
        {
            errmsg = "";
            contacts.Clear();

            string sql = queryGetCustomerContact.Replace("#1#", actNo.ToString());

            SqlCommand command = new SqlCommand(sql, connection)
            {
                CommandType = CommandType.Text,
                CommandTimeout = 600
            };

            SqlDataReader reader = null;

            try
            {
                if (connection.State == ConnectionState.Closed || connection.State == ConnectionState.Broken)
                    connection.Open();

                reader = command.ExecuteReader();

                while (reader.Read())
                {
                    int idx = 0;
                    contacts.Add(new Models.VismaActor()
                    {
                        ActorNo = reader.GetInt32(idx++),
                        Name = reader.GetString(idx++).Trim(),
                        AddressLine1 = reader.GetString(idx++).Trim(),
                        AddressLine2 = reader.GetString(idx++).Trim(),
                        AddressLine3 = reader.GetString(idx++).Trim(),
                        PostCode = reader.GetString(idx++).Trim(),
                        PostalArea = reader.GetString(idx++).Trim(),
                        CountryCode = reader.GetString(idx++).Trim(),
                        Phone = reader.GetString(idx++).Trim(),
                        Mobile = reader.GetString(idx++).Trim(),
                        EmailAddress = reader.GetString(idx++).Trim(),
                    });
                }

            }
            catch (Exception ex)
            {
                errmsg = ex.Message;

                return false;
            }
            finally
            {
                if (reader != null)
                    reader.Close();
                if (connection.State == ConnectionState.Open)
                    connection.Close();
                command.Dispose();
            }

            return true;
        }


        public bool GetCustomerDelivery(int actNo, ref List<Models.VismaActor> deliveries, out string errmsg)
        {
            errmsg = "";

            deliveries.Clear();

            string sql = queryGetCustomerDelivery.Replace("#1#", actNo.ToString());

            SqlCommand command = new SqlCommand(sql, connection)
            {
                CommandType = CommandType.Text,
                CommandTimeout = 600
            };

            SqlDataReader reader = null;

            try
            {
                if (connection.State == ConnectionState.Closed || connection.State == ConnectionState.Broken)
                    connection.Open();

                reader = command.ExecuteReader();

                while (reader.Read())
                {
                    int idx = 0;
                    deliveries.Add(new Models.VismaActor()
                    {
                        ActorNo = reader.GetInt32(idx++),
                        Name = reader.GetString(idx++).Trim(),
                        AddressLine1 = reader.GetString(idx++).Trim(),
                        AddressLine2 = reader.GetString(idx++).Trim(),
                        AddressLine3 = reader.GetString(idx++).Trim(),
                        PostCode = reader.GetString(idx++).Trim(),
                        PostalArea = reader.GetString(idx++).Trim(),
                        CountryCode = reader.GetString(idx++).Trim(),
                        Phone = reader.GetString(idx++).Trim(),
                        Mobile = reader.GetString(idx++).Trim(),
                        EmailAddress = reader.GetString(idx++).Trim(),
                    });
                }

            }
            catch (Exception ex)
            {
                errmsg = ex.Message;

                return false;
            }
            finally
            {
                if (reader != null)
                    reader.Close();
                if (connection.State == ConnectionState.Open)
                    connection.Close();
                command.Dispose();
            }

            return true;
        }


        public bool GetCustomerAddresses(int custNo, ref List<Models.VismaActor> addresses, out string errmsg)
        {
            errmsg = "";

            addresses.Clear();

            string sql = queryGetCustomerAddresses.Replace("#1#", custNo.ToString());

            SqlCommand command = new SqlCommand(sql, connection)
            {
                CommandType = CommandType.Text,
                CommandTimeout = 600
            };

            SqlDataReader reader = null;

            try
            {
                if (connection.State == ConnectionState.Closed || connection.State == ConnectionState.Broken)
                    connection.Open();

                reader = command.ExecuteReader();

                while (reader.Read())
                {
                    int idx = 0;
                    addresses.Add(new Models.VismaActor()
                    {

                        ActorNo = reader.GetInt32(idx++),
                        Name = reader.GetString(idx++).Trim(),
                        AddressLine1 = reader.GetString(idx++).Trim(),
                        AddressLine2 = reader.GetString(idx++).Trim(),
                        AddressLine3 = reader.GetString(idx++).Trim(),
                        PostCode = reader.GetString(idx++).Trim(),
                        PostalArea = reader.GetString(idx++).Trim(),
                        CountryNumber = reader.GetInt32(idx++),
                        CountryCode = reader.GetString(idx++).Trim(),
                        Phone = reader.GetString(idx++).Trim(),
                        Mobile = reader.GetString(idx++).Trim(),
                        EmailAddress = reader.GetString(idx++).Trim(),
                        CompanyNumber = reader.GetString(idx++).Trim(),
                        Inf7 = reader.GetString(idx++).Trim(),
                        Memo = Utils.ReadMemoFile(reader.GetString(idx++).Trim())
                    });
                }

            }
            catch (Exception ex)
            {
                errmsg = ex.Message;

                return false;
            }
            finally
            {
                if (reader != null)
                    reader.Close();
                if (connection.State == ConnectionState.Open)
                    connection.Close();
                command.Dispose();
            }

            return true;
        }

        public bool GetCustomerAddressFromActNo(int actNo, ref Models.VismaActor address, out string errmsg)
        {
            errmsg = "";


            string sql = queryGetCustomerAddressFromActNo.Replace("#1#", actNo.ToString());

            SqlCommand command = new SqlCommand(sql, connection)
            {
                CommandType = CommandType.Text,
                CommandTimeout = 600
            };

            SqlDataReader reader = null;

            try
            {
                if (connection.State == ConnectionState.Closed || connection.State == ConnectionState.Broken)
                    connection.Open();

                reader = command.ExecuteReader();

                if (reader.Read())
                {
                    int idx = 0;

                    address.ActorNo = reader.GetInt32(idx++);         
                    address.Name = reader.GetString(idx++).Trim();
                    address.AddressLine1 = reader.GetString(idx++).Trim();
                    address.AddressLine2 = reader.GetString(idx++).Trim();
                    address.AddressLine3 = reader.GetString(idx++).Trim();
                    address.PostCode = reader.GetString(idx++).Trim();
                    address.PostalArea = reader.GetString(idx++).Trim();
                    address.CountryNumber = reader.GetInt32(idx++);
                    address.CountryCode = reader.GetString(idx++).Trim();
                    address.Phone = reader.GetString(idx++).Trim();
                    address.Mobile = reader.GetString(idx++).Trim();
                    address.EmailAddress = reader.GetString(idx++).Trim();
                    address.CompanyNumber = reader.GetString(idx++).Trim();
                    address.Inf7 = reader.GetString(idx++).Trim();
                    address.Memo = Utils.ReadMemoFile(reader.GetString(idx++).Trim());
                
                }

            }
            catch (Exception ex)
            {
                errmsg = ex.Message;

                return false;
            }
            finally
            {
                if (reader != null)
                    reader.Close();
                if (connection.State == ConnectionState.Open)
                    connection.Close();
                command.Dispose();
            }

            return true;
        }

        public int GetNextAdNo(out string errmsg)
        {
            errmsg = "";
            int adNo = 0;

            string sql = "SELECT ISNULL(MAX(AdNo),0) + 1 FROM Address";
            SqlCommand command = new SqlCommand(sql, connection)
            {
                CommandType = CommandType.Text,
                CommandTimeout = 600
            };

            SqlDataReader reader = null;

            try
            {
                if (connection.State == ConnectionState.Closed || connection.State == ConnectionState.Broken)
                    connection.Open();

                reader = command.ExecuteReader();

                if (reader.Read())
                {
                    adNo = reader.GetInt32(0);
                }

            }
            catch (Exception ex)
            {
                errmsg = ex.Message;

                return -1;
            }
            finally
            {
                if (reader != null)
                    reader.Close();
                if (connection.State == ConnectionState.Open)
                    connection.Close();
                command.Dispose();
            }

            return adNo;

        }

      /*  public bool InsertAddress(ref Models.VismaActor address, out string errmsg)
        {
            address.AddressNo = GetNextAdNo(out errmsg);
            if (address.AddressNo <= 0)
                return false;

            string sql = "INSERT INTO Address (AdNo,Nm,Ad1,Ad2,Ad3,Ad4,PNo,PArea,Ctry,Lang,Shrt,MailAd,Phone,MobPh,BsNo,ActNo,Inf7) VALUES ( " +
                        $"{address.AddressNo},'{address.Name}','{address.AddressLine1}','{address.AddressLine2}','{address.AddressLine3}','{address.AddressLine4}','{address.PostCode}','{address.PostalArea}',{address.CountryNumber},46,'{address.Name}','{address.EmailAddress}','{address.Phone}','{address.Mobile}','{address.CompanyNumber}',{address.ActorNo},'')";
            SqlCommand command = new SqlCommand(sql, connection)
            {
                CommandType = CommandType.Text,
                CommandTimeout = 600
            };

            try
            {
                if (connection.State == ConnectionState.Closed || connection.State == ConnectionState.Broken)
                    connection.Open();

                command.ExecuteNonQuery();
            }
             
            catch (Exception ex)
            {
                errmsg = ex.Message;

                return false;
            }
            finally
            {
                if (connection.State == ConnectionState.Open)
                    connection.Close();
                command.Dispose();

            }
            return true;
        }
        */

        public bool GetNewProducts(ref List<Models.Item> items, out string errmsg)
        {
            errmsg = "";
            items.Clear();

            string sql = queryGetProducts;

            SqlCommand command = new SqlCommand(sql, connection)
            {
                CommandType = CommandType.Text,
                CommandTimeout = 600
            };

            SqlDataReader reader = null;
            try
            {
                if (connection.State == ConnectionState.Closed || connection.State == ConnectionState.Broken)
                    connection.Open();

                reader = command.ExecuteReader();

                while (reader.Read())
                {
                    int fld = 0;
                    Models.Item item = new Models.Item()
                    {
                        itemNo = reader.GetString(fld++).Trim(),
                        description = reader.GetString(fld++).Trim(),
                        unit = reader.GetString(fld++).Trim(),
                        itemGroup = reader.GetString(fld++).Trim()
                    };

                    item.wholesalerReference.externalId = Utils.ReadConfigString("WholesalerReference", "SD");
                    item.wholesalerReference.id = Utils.ReadConfigString("WholesalerReference", "SD");

                    items.Add(item);

                }
            }
            catch (Exception ex)
            {
                errmsg = ex.Message;

                return false;
            }
            finally
            {
                if (reader != null)
                    reader.Close();
                if (connection.State == ConnectionState.Open)
                    connection.Close();
                command.Dispose();

            }

            foreach(Models.Item item in items)
            {
                decimal price = 0.0M;
                if (GetProductPrice(item.itemNo, ref price, out errmsg))
                    item.price = (double)price;

            }
            
            return true;

        }

        public bool GetProductPrice(string prodNo, ref decimal price, out string errmsg)
        {
            price = 0.0M;
            errmsg = "";

            int vismaDt = DateTime.Now.Year * 10000 + DateTime.Now.Month * 100 + DateTime.Now.Day;

            string sql = queryGetPrice.Replace("#1#", prodNo).Replace("#2#", vismaDt.ToString()); 

            SqlCommand command = new SqlCommand(sql, connection)
            {
                CommandType = CommandType.Text,
                CommandTimeout = 600
            };

            SqlDataReader reader = null;

            try
            {
                if (connection.State == ConnectionState.Closed || connection.State == ConnectionState.Broken)
                    connection.Open();

                reader = command.ExecuteReader();

                if (reader.Read())
                {
                    price = reader.GetDecimal(0);
                }
            }
            catch (Exception ex)
            {
                errmsg = ex.Message;

                return false;
            }
            finally
            {
                if (reader != null)
                    reader.Close();
                if (connection.State == ConnectionState.Open)
                    connection.Close();
                command.Dispose();

            }

            return true;

        }
        public bool GetOrdersToSynchronize(ref List<Models.VismaOrderInfo> orders, out string errmsg)
        {
            errmsg = "";
            orders.Clear();

            string sql = "SELECT DISTINCT OrdNo,CsOrdNo FROM Ord WHERE Ord.Gr3=12 AND Ord.TrTp=1";

            SqlCommand command = new SqlCommand(sql, connection)
            {
                CommandType = CommandType.Text,
                CommandTimeout = 600
            };

            SqlDataReader reader = null;

            try
            {
                if (connection.State == ConnectionState.Closed || connection.State == ConnectionState.Broken)
                    connection.Open();

                reader = command.ExecuteReader();

                while (reader.Read())
                {
                    int fld = 0;
                    orders.Add(new Models.VismaOrderInfo()
                    {
                        OrderNo = reader.GetInt32(fld++),
                        SmartDayId = reader.GetString(fld++).Trim()
                    });
                }
            }
            catch (Exception ex)
            {
                errmsg = ex.Message;

                return false;
            }
            finally
            {
                if (reader != null)
                    reader.Close();
                if (connection.State == ConnectionState.Open)
                    connection.Close();
                command.Dispose();

            }

            return true;

        }



        public bool GetNewOrders(ref List<Models.VismaOrder> orders, out string errmsg)
        {
            errmsg = "";
            orders.Clear();

            string sql = queryGetOrders;

            SqlCommand command = new SqlCommand(sql, connection)
            {
                CommandType = CommandType.Text,
                CommandTimeout = 600
            };

            SqlDataReader reader = null;

            try
            {
                if (connection.State == ConnectionState.Closed || connection.State == ConnectionState.Broken)
                    connection.Open();

                reader = command.ExecuteReader();

                while (reader.Read())
                {
                    int fld = 0;
                    orders.Add(new Models.VismaOrder()
                    {
                        Group12 = reader.GetInt32(fld++), //Ord.Gr12
                        OrderDate = reader.GetInt32(fld++),
                        OrderNo = reader.GetInt32(fld++),
                        Name = reader.GetString(fld++),
                        AddressLine1 = reader.GetString(fld++),
                        AddressLine2 = reader.GetString(fld++),
                        AddressLine3 = reader.GetString(fld++),
                        PostCode = reader.GetString(fld++),
                        PostalArea = reader.GetString(fld++),

                        DeliveryName = reader.GetString(fld++),
                        DeliveryAddress1 = reader.GetString(fld++),
                        DeliveryAddress2 = reader.GetString(fld++),
                        DeliveryAddress3 = reader.GetString(fld++),
                        DeliveryPostCode = reader.GetString(fld++),
                        DeliveryPostalArea = reader.GetString(fld++),

                        DeliveryMethod = reader.GetInt32(fld++),

                        RequiredDeliveryDate = reader.GetInt32(fld++),
                        Information1 = reader.GetString(fld++).Trim(), // Inf
                        Information2 = reader.GetString(fld++).Trim(), // Inf2
                        Information3 = reader.GetString(fld++).Trim(), // Inf3
                        Information4 = reader.GetString(fld++).Trim(), // Inf4
                        Information5 = reader.GetString(fld++).Trim(), // Inf5

                        Memo = Utils.ReadMemoFile(reader.GetString(fld++).Trim()),
                        YourReference = reader.GetString(fld++),
                        OurReference = reader.GetString(fld++),
                        StatusGr3 = reader.GetInt32(fld++),
                        CustomerOrSupplierOrderNo = reader.GetString(fld++).Trim(),
                        CustomerNo = reader.GetInt32(fld++),
                        //Ord.EmpNo,Ord.R7,Ord.Gr,Ord.CreUsr
                        EmpNo = reader.GetInt32(fld++),         
                        CategoryName = reader.GetString(fld++), // R7
                        Group1 = reader.GetInt32(fld++), // Gr
                        CreUsr = reader.GetString(fld++).Trim(),
                        R12notused = reader.GetString(fld++).Trim(), /// NOTUSED
                        R2 = reader.GetInt32(fld++),                // -> project
                        SelBuy = reader.GetInt32(fld++),
                        Reqno = reader.GetString(fld++).Trim(),
                        Gr8 = reader.GetInt32(fld++)

                    });
                }
            }
            catch (Exception ex)
            {
                errmsg = ex.Message;

                return false;
            }
            finally
            {
                if (reader != null)
                    reader.Close();
                if (connection.State == ConnectionState.Open)
                    connection.Close();
                command.Dispose();

            }

            foreach (Models.VismaOrder order in orders)
            {
                List<string> thingsList = new List<string>();
                if (GetThingsForOrder(order.OrderNo, ref thingsList, out errmsg) == false)
                    return false;
                order.R12List = thingsList;
                if (order.R12notused != "")
                    if (order.R12List.Contains(order.R12notused) == false)
                        order.R12List.Add(order.R12notused);

                string note1 = "", note2 = "", note3 = "";
                if (GetOrderNotes(order.OrderNo, ref note1, ref note2, ref note3, out errmsg) == false)
                    return false;
                order.FreeInf1Memo1 = note1;
                order.FreeInf1Memo2 = note2;
                order.FreeInf1Memo3 = note3;

                Utils.WriteLog("DEBUG: Note2 " + order.FreeInf1Memo3);


                List<Models.VismaOrderLine> lines = new List<Models.VismaOrderLine>();
               /* if (GetOrderLines(order.OrderNo, ref lines, out errmsg) == false)
                    return false;
                foreach (Models.VismaOrderLine line in lines)
                    order.OrderLines.Add(line);

                List<Models.VismaAgreement> agreements = new List<Models.VismaAgreement>();
                if (GetOrderAgreements(order.OrderNo, ref agreements, out errmsg) == false)
                    return false;
                foreach (Models.VismaAgreement agreement in agreements)
                    order.AgreementLines.Add(agreement);*/

            }

            return true;
        }

        public bool UpdateFreeInfMemo(int ordNo,  string memoPath, int infCatNo, string memoText, out string errmsg)
        {
            errmsg = "";
          
            if (memoPath == "")
            {
                memoPath = Utils.ReadConfigString("MemoFolder", @"\\srv-sql1\Visma\Visma_Program\Business\\F9992") + $"\\M{infCatNo}0{ordNo}{Utils.GenerateTimeStamp()}.txt"; 
            }

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
                        streamWriter.Write(memoText);
                        streamWriter.Flush();
                        streamWriter.Close();
                    }
                }
            }
            catch (Exception exception)
            {
                errmsg = exception.Message;
                return false;
            }
            string sql;
            int PK = 0;
            if (GetFreeInf1PK(infCatNo, ordNo, ref PK, out errmsg) == false)
                return false;
            if (PK <= 0)
            {
                if (GetNextFreeInfPK(ref PK, out errmsg) == false)
                    return false;

                sql = $"INSERT INTO FreeInf1 (PK,InfCatNo,OrdNo,NoteNm) VALUES ({PK},{infCatNo},{ordNo},'{memoPath}')";
            }
            else
                 sql = $"UPDATE FreeInf1 Set NoteNm='{memoPath}' WHERE InfCatNo={infCatNo} AND OrdNo={ordNo}";

            SqlCommand command = new SqlCommand(sql, connection)
            {
                CommandType = CommandType.Text,
                CommandTimeout = 600
            };

            try
            {

                if (connection.State == ConnectionState.Closed || connection.State == ConnectionState.Broken)
                    connection.Open();

                command.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                errmsg = "GetOrderLines() -" + ex.Message;

                return false;
            }
            finally
            {
                if (connection.State == ConnectionState.Open)
                    connection.Close();
                command.Dispose();
            }

            return true;
        }

        public bool GetFreeInf1PK(int infcatno, int ordNo, ref int PK, out string errmsg)
        {
            errmsg = "";

            PK = 0;

            string sql = $"SELECT TOP 1 PK FROM FreeInf1 WHERE InfCatNo={infcatno} AND OrdNo={ordNo}";

            SqlCommand command = new SqlCommand(sql, connection)
            {
                CommandType = CommandType.Text,
                CommandTimeout = 600
            };

            SqlDataReader reader = null;

            try
            {

                if (connection.State == ConnectionState.Closed || connection.State == ConnectionState.Broken)
                    connection.Open();

                reader = command.ExecuteReader();

                if (reader.Read())
                {
                    PK = reader.GetInt32(0);

                }
            }
            catch (Exception ex)
            {
                errmsg = "GetNextFreeCustomerNo() -" + ex.Message;

                return false;
            }
            finally
            {
                if (connection.State == ConnectionState.Open)
                    connection.Close();
            }
            return true;

        }

        public bool GetNextFreeInfPK(ref int PK, out string errmsg)
        {
            errmsg = "";

            PK = 0;

            string sql = $"SELECT ISNULL(MAX(PK),0)+1  FROM FreeInf1 ";

            SqlCommand command = new SqlCommand(sql, connection)
            {
                CommandType = CommandType.Text,
                CommandTimeout = 600
            };

            SqlDataReader reader = null;

            try
            {

                if (connection.State == ConnectionState.Closed || connection.State == ConnectionState.Broken)
                    connection.Open();

                reader = command.ExecuteReader();

                if (reader.Read())
                {
                    PK = reader.GetInt32(0);

                }
            }
            catch (Exception ex)
            {
                errmsg = "GetNextFreeInfPK() -" + ex.Message;

                return false;
            }
            finally
            {
                if (connection.State == ConnectionState.Open)
                    connection.Close();
            }
            return true;
        }

        public bool GetOrderNoteMemoPaths(int ordNo, ref string path1, ref string path2, ref string path3, out string errmsg)
        {
            path1 = "";
            path2 = "";
            path3 = "";
            errmsg = "";
            string sql = "SELECT ISNULL(F1.NoteNm, ''),ISNULL(F2.NoteNm, ''),ISNULL(F3.NoteNm, '') " +
                        "FROM Ord " +
                        "LEFT OUTER JOIN FreeInf1 F1 ON F1.InfCatNo = 201 AND F1.OrdNo = Ord.OrdNo " +
                        "LEFT OUTER JOIN FreeInf1 F2 ON F2.InfCatNo = 202 AND F2.OrdNo = Ord.OrdNo " +
                        "LEFT OUTER JOIN FreeInf1 F3 ON F3.InfCatNo = 203 AND F3.OrdNo = Ord.OrdNo " +
                        $"WHERE ORd.OrdNo = {ordNo}";

            SqlCommand command = new SqlCommand(sql, connection)
            {
                CommandType = CommandType.Text,
                CommandTimeout = 600
            };

            SqlDataReader reader = null;

            reader = null;

            try
            {

                if (connection.State == ConnectionState.Closed || connection.State == ConnectionState.Broken)
                    connection.Open();

                reader = command.ExecuteReader();

                if (reader.Read())
                {
                    path1 = reader.GetString(0).Trim();
                    path2 = reader.GetString(1).Trim();
                    path3 = reader.GetString(2).Trim();
                }
            }
            catch (Exception ex)
            {
                errmsg = "GetOrderNotes() -" + ex.Message;

                return false;
            }
            finally
            {
                if (reader != null)
                    reader.Close();
                if (connection.State == ConnectionState.Open)
                    connection.Close();
                command.Dispose();
            }

            return true;
        }

        public bool GetOrderNotes(int ordNo, ref string note1, ref string note2, ref string note3, out string errmsg)
        {
            note1 = "";
            note2 = "";
            note3 = "";
            errmsg = "";
            string sql = "SELECT ISNULL(F1.NoteNm, ''),ISNULL(F2.NoteNm, ''),ISNULL(F3.NoteNm, '') " +
                        "FROM Ord " +
                        "LEFT OUTER JOIN FreeInf1 F1 ON F1.InfCatNo = 201 AND F1.OrdNo = Ord.OrdNo " +
                        "LEFT OUTER JOIN FreeInf1 F2 ON F2.InfCatNo = 202 AND F2.OrdNo = Ord.OrdNo " +
                        "LEFT OUTER JOIN FreeInf1 F3 ON F3.InfCatNo = 203 AND F3.OrdNo = Ord.OrdNo " +
                        $"WHERE ORd.OrdNo = {ordNo}";

            SqlCommand command = new SqlCommand(sql, connection)
            {
                CommandType = CommandType.Text,
                CommandTimeout = 600
            };

            SqlDataReader reader = null;

            reader = null;

            try
            {

                if (connection.State == ConnectionState.Closed || connection.State == ConnectionState.Broken)
                    connection.Open();

                reader = command.ExecuteReader();

                if (reader.Read())
                {
                    note1 = Utils.ReadMemoFile(reader.GetString(0).Trim());
                    note2 = Utils.ReadMemoFile(reader.GetString(1).Trim());
                    note3 = Utils.ReadMemoFile(reader.GetString(2).Trim());
                }
            }
            catch (Exception ex)
            {
                errmsg = "GetOrderNotes() -" + ex.Message;

                return false;
            }
            finally
            {
                if (reader != null)
                    reader.Close();
                if (connection.State == ConnectionState.Open)
                    connection.Close();
                command.Dispose();
            }

            return true;
        }

        public bool GetOrderLines(int ordNo, ref List<Models.VismaOrderLine> lines, out string errmsg)
        {

            errmsg = "";
            lines.Clear();

            string sql = queryGetOrderLinesByOrdNo.Replace("#1#", ordNo.ToString()); ;

            SqlCommand command = new SqlCommand(sql, connection)
            {
                CommandType = CommandType.Text,
                CommandTimeout = 600
            };

            SqlDataReader reader = null;

            reader = null;

            try
            {

                if (connection.State == ConnectionState.Closed || connection.State == ConnectionState.Broken)
                    connection.Open();

                reader = command.ExecuteReader();

                while (reader.Read())
                {
                    int idx = 0;
                    Models.VismaOrderLine orderLine = new Models.VismaOrderLine()
                    {  
                        LineNo = reader.GetInt32(idx++),
                        ProductNo = reader.GetString(idx++),
                        Quantity = reader.GetDecimal(idx++),            //  NoOrg
                        PriceInCurrency = reader.GetDecimal(idx++),           //  Price
                        Description = reader.GetString(idx++) ,          // Descr
                        WebPg = reader.GetString(idx++),
                        CategoryName = reader.GetString(idx++), // R7
                    };
                    orderLine.LinePrice = orderLine.Quantity * orderLine.PriceInCurrency;
                    if (Utils.StringToInt(orderLine.WebPg) == 0)
                        orderLine.WebPg = "";

                    lines.Add(orderLine);
                }
            }
            catch (Exception ex)
            {
                errmsg = "GetOrderLines() -" + ex.Message;

                return false;
            }
            finally
            {
                if (reader != null)
                    reader.Close();
                if (connection.State == ConnectionState.Open)
                    connection.Close();
                command.Dispose();
            }

            return true;
        }

        public bool GetOrderAgreements(int ordNo, ref List<Models.VismaAgreement> agreements, out string errmsg)
        {

            errmsg = "";
            agreements.Clear();

            string sql = queryGetAgreementLinesByOrdNo.Replace("#1#", ordNo.ToString()); ;

            SqlCommand command = new SqlCommand(sql, connection)
            {
                CommandType = CommandType.Text,
                CommandTimeout = 600
            };

            SqlDataReader reader = null;

            reader = null;

            try
            {

                if (connection.State == ConnectionState.Closed || connection.State == ConnectionState.Broken)
                    connection.Open();

                reader = command.ExecuteReader();

                while (reader.Read())
                {
                    Models.VismaAgreement agreement = new Models.VismaAgreement()
                    {
                        AgreementActNo = reader.GetInt32(0),
                        AgreementNumber = reader.GetInt32(1),
                        Decription = reader.GetString(2).Trim(),
                        FromDate = reader.GetInt32(3),
                        ToDate = reader.GetInt32(4),
                        FromTime = reader.GetInt32(5),
                        ToTime = reader.GetInt32(6),
                        ProdNo = reader.GetString(7),
                        Quantity = reader.GetDecimal(8),
                        Price = reader.GetDecimal(9),
                        CustomerNumber = reader.GetInt32(10),
                        CustomerName = reader.GetString(11),
                        Status = reader.GetInt32(12),  // Agr.Gr3
                    };
                    agreements.Add(agreement);
                }
            }
            catch (Exception ex)
            {
                errmsg = "GetOrderAgreements() -" + ex.Message;

                return false;
            }
            finally
            {
                if (reader != null)
                    reader.Close();
                if (connection.State == ConnectionState.Open)
                    connection.Close();
                command.Dispose();
            }

            return true;
        }

        public bool UpdateOrderStatus(int ordNo, int status, out string errmsg)
        {
            errmsg = "";

            string sql = $"UPDATE Ord Set Gr3={status} WHERE OrdNo={ordNo}";

            SqlCommand command = new SqlCommand(sql, connection)
            {
                CommandType = CommandType.Text,
                CommandTimeout = 600
            };

            try
            {

                if (connection.State == ConnectionState.Closed || connection.State == ConnectionState.Broken)
                    connection.Open();

                command.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                errmsg = "GetOrderLines() -" + ex.Message;

                return false;
            }
            finally
            {
                if (connection.State == ConnectionState.Open)
                    connection.Close();
                command.Dispose();
            }

            return true;
        }

        public bool UpdateProjectStatus(int ordNo, int status, out string errmsg)
        {
            errmsg = "";

            string sql = $"UPDATE Ord Set Gr5={status} WHERE OrdNo={ordNo}";

            SqlCommand command = new SqlCommand(sql, connection)
            {
                CommandType = CommandType.Text,
                CommandTimeout = 600
            };

            try
            {

                if (connection.State == ConnectionState.Closed || connection.State == ConnectionState.Broken)
                    connection.Open();

                command.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                errmsg = "GetOrderLines() -" + ex.Message;

                return false;
            }
            finally
            {
                if (connection.State == ConnectionState.Open)
                    connection.Close();
                command.Dispose();
            }

            return true;
        }

        public bool UpdateProjectStatusOld(int rNo, int status, out string errmsg)
        {
            errmsg = "";

            string sql = $"UPDATE R2 Set Gr11={status} WHERE RNo={rNo}";

            SqlCommand command = new SqlCommand(sql, connection)
            {
                CommandType = CommandType.Text,
                CommandTimeout = 600
            };

            try
            {

                if (connection.State == ConnectionState.Closed || connection.State == ConnectionState.Broken)
                    connection.Open();

                command.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                errmsg = "GetOrderLines() -" + ex.Message;

                return false;
            }
            finally
            {
                if (connection.State == ConnectionState.Open)
                    connection.Close();
                command.Dispose();
            }

            return true;
        }

        public bool RegisterThingSmartdayID(string Rno, string smartDayId, string siteId, out string errmsg)
        {
            errmsg = "";

            string sql = $"UPDATE R12 SET Inf8='{smartDayId}',PictFNm='{siteId}' WHERE Rno='{Rno}'";
            SqlCommand command = new SqlCommand(sql, connection)
            {
                CommandType = CommandType.Text,
                CommandTimeout = 600
            };
            Utils.WriteLog("DEBUG:" + sql);
            try
            {

                if (connection.State == ConnectionState.Closed || connection.State == ConnectionState.Broken)
                    connection.Open();

                command.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                errmsg = "RegisterThingSmartdayID() -" + ex.Message;

                return false;
            }
            finally
            {
                if (connection.State == ConnectionState.Open)
                    connection.Close();
                command.Dispose();
            }

            return true;

        }

        public bool RegisterProjectSmartdayIDOLD(int rno, string smartDayId,out string errmsg)
        {
            errmsg = "";

            string sql = $"UPDATE R2 Set Inf7='{smartDayId}' WHERE Rno={rno}";

            SqlCommand command = new SqlCommand(sql, connection)
            {
                CommandType = CommandType.Text,
                CommandTimeout = 600
            };

            try
            {

                if (connection.State == ConnectionState.Closed || connection.State == ConnectionState.Broken)
                    connection.Open();

                command.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                errmsg = "RegisterProjectSmartdayID() -" + ex.Message;

                return false;
            }
            finally
            {
                if (connection.State == ConnectionState.Open)
                    connection.Close();
                command.Dispose();
            }

            return true;
        }

        public bool RegisterProjectSmartdayID(int ordNo, string smartDayId, out string errmsg)
        {
            errmsg = "";

            string sql = $"UPDATE Ord Set Inf4='{smartDayId}' WHERE OrdNo={ordNo}";

            SqlCommand command = new SqlCommand(sql, connection)
            {
                CommandType = CommandType.Text,
                CommandTimeout = 600
            };

            try
            {

                if (connection.State == ConnectionState.Closed || connection.State == ConnectionState.Broken)
                    connection.Open();

                command.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                errmsg = "RegisterProjectSmartdayID() -" + ex.Message;

                return false;
            }
            finally
            {
                if (connection.State == ConnectionState.Open)
                    connection.Close();
                command.Dispose();
            }

            return true;
        }


        public bool RegisterOrderSmartdayID(int ordNo, string smartDayId, out string errmsg)
        {
            errmsg = "";

            string sql = $"UPDATE Ord Set CsOrdNo='{smartDayId}' WHERE OrdNo={ordNo}";

            SqlCommand command = new SqlCommand(sql, connection)
            {
                CommandType = CommandType.Text,
                CommandTimeout = 600
            };

            try
            {

                if (connection.State == ConnectionState.Closed || connection.State == ConnectionState.Broken)
                    connection.Open();

                command.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                errmsg = "RegisterOrderSmartdayID() -" + ex.Message;

                return false;
            }
            finally
            {
                if (connection.State == ConnectionState.Open)
                    connection.Close();
                command.Dispose();
            }

            return true;
        }

        public bool RegisterMaterialSmartdayID(int ordNo, int lineNo, string smartDayId, out string errmsg)
        {
            errmsg = "";

            string sql = $"UPDATE OrdLn Set WebPg='{smartDayId}' WHERE OrdNo={ordNo} AND LnNo={lineNo}";

            SqlCommand command = new SqlCommand(sql, connection)
            {
                CommandType = CommandType.Text,
                CommandTimeout = 600
            };

            try
            {

                if (connection.State == ConnectionState.Closed || connection.State == ConnectionState.Broken)
                    connection.Open();

                command.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                errmsg = "GetOrderLines() -" + ex.Message;

                return false;
            }
            finally
            {
                if (connection.State == ConnectionState.Open)
                    connection.Close();
                command.Dispose();
            }

            return true;
        }

        public bool RegisterSalaryCodeSmartdayID(int ordNo, string uniqueKey, string smartDayId, out string errmsg)
        {
            errmsg = "";


            int agrNo = 0;
            int agrActNo = 0;
            string[] sarr = uniqueKey.Split(',');
            if (sarr.Length > 0)
                agrNo = Utils.StringToInt(sarr[0]);

            if (sarr.Length > 1)
                agrActNo = Utils.StringToInt(sarr[1]);

            string sql = $"UPDATE Agr Set Txt2='{smartDayId}' WHERE OrdNo={ordNo} AND AgrNo={agrNo} AND AgrActNo={agrActNo}";

            SqlCommand command = new SqlCommand(sql, connection)
            {
                CommandType = CommandType.Text,
                CommandTimeout = 600
            };

            try
            {

                if (connection.State == ConnectionState.Closed || connection.State == ConnectionState.Broken)
                    connection.Open();

                command.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                errmsg = "GetOrderLines() -" + ex.Message;

                return false;
            }
            finally
            {
                if (connection.State == ConnectionState.Open)
                    connection.Close();
                command.Dispose();
            }

            return true;
        }

        public bool RegisterCustomerSmartdayID(int actNo, string smartDayId, out string errmsg)
        {
            errmsg = "";

            string sql = $"UPDATE Actor Set Inf7='{smartDayId}' WHERE ActNo={actNo}";

            SqlCommand command = new SqlCommand(sql, connection)
            {
                CommandType = CommandType.Text,
                CommandTimeout = 600
            };

            try
            {

                if (connection.State == ConnectionState.Closed || connection.State == ConnectionState.Broken)
                    connection.Open();

                command.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                errmsg = "RegisterCustomerSmartdayID() -" + ex.Message;

                return false;
            }
            finally
            {
                if (connection.State == ConnectionState.Open)
                    connection.Close();
                command.Dispose();
            }

            return true;
        }

        public bool RegisterSiteSmartdayID(int actNo, string smartDayId, out string errmsg)
        {
            errmsg = "";

            string sql = $"UPDATE Actor Set Inf7='{smartDayId}' WHERE Gr3=10 AND ActNo ={actNo}";

            SqlCommand command = new SqlCommand(sql, connection)
            {
                CommandType = CommandType.Text,
                CommandTimeout = 600
            };

            try
            {

                if (connection.State == ConnectionState.Closed || connection.State == ConnectionState.Broken)
                    connection.Open();

                command.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                errmsg = "RegisterSiteSmartdayID() -" + ex.Message;

                return false;
            }
            finally
            {
                if (connection.State == ConnectionState.Open)
                    connection.Close();
                command.Dispose();
            }

            return true;
        }

        public bool UpdateProductStatus(string prodNo, int status, out string errmsg)
        {
            errmsg = "";

            string sql = $"UPDATE Prod Set Gr3={status} WHERE ProdNo='{prodNo}'";

            SqlCommand command = new SqlCommand(sql, connection)
            {
                CommandType = CommandType.Text,
                CommandTimeout = 600
            };

            try
            {

                if (connection.State == ConnectionState.Closed || connection.State == ConnectionState.Broken)
                    connection.Open();

                command.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                errmsg = "GetOrderLines() -" + ex.Message;

                return false;
            }
            finally
            {
                if (connection.State == ConnectionState.Open)
                    connection.Close();
                command.Dispose();
            }

            return true;
        }

        public bool GetEmployerName(int empNo, ref string name, out string errmsg)
        {
            name = "";
            errmsg = "";

            string sql = $"SELECT TOP 1 Nm FROM Actor WHERE EmpNo={empNo}";

            SqlCommand command = new SqlCommand(sql, connection)
            {
                CommandType = CommandType.Text,
                CommandTimeout = 600
            };

            SqlDataReader reader = null;

            reader = null;

            try
            {

                if (connection.State == ConnectionState.Closed || connection.State == ConnectionState.Broken)
                    connection.Open();

                reader = command.ExecuteReader();

                if (reader.Read())
                {
                    name = reader.GetString(0);
                 
                }
            }
            catch (Exception ex)
            {
                errmsg = "GetEmployerName() -" + ex.Message;

                return false;
            }
            finally
            {
                if (reader != null)
                    reader.Close();
                if (connection.State == ConnectionState.Open)
                    connection.Close();
                command.Dispose();
            }

            return true;
        }


        public bool GetProjectSmartdayID(int ordNo, ref string smartdayID, out string errmsg)
        {
            smartdayID = "";
            errmsg = "";

            string sql = $"SELECT TOP 1 Inf4 FROM Ord WHERE OrdNo={ordNo}";

            SqlCommand command = new SqlCommand(sql, connection)
            {
                CommandType = CommandType.Text,
                CommandTimeout = 600
            };

            SqlDataReader reader = null;

            reader = null;

            try
            {

                if (connection.State == ConnectionState.Closed || connection.State == ConnectionState.Broken)
                    connection.Open();

                reader = command.ExecuteReader();

                if (reader.Read())
                {
                    smartdayID = reader.GetString(0);

                }
            }
            catch (Exception ex)
            {
                errmsg = "GetProjectSmartdayID() -" + ex.Message;

                return false;
            }
            finally
            {
                if (reader != null)
                    reader.Close();
                if (connection.State == ConnectionState.Open)
                    connection.Close();
                command.Dispose();
            }

            return true;
        }
        public bool GetProjectSmartdayIDOld(int rno, ref string smartdayID, out string errmsg)
        {
            smartdayID = "";
            errmsg = "";

            string sql = $"SELECT TOP 1 Inf7 FROM R2 WHERE RNo={rno}";

            SqlCommand command = new SqlCommand(sql, connection)
            {
                CommandType = CommandType.Text,
                CommandTimeout = 600
            };

            SqlDataReader reader = null;

            reader = null;

            try
            {

                if (connection.State == ConnectionState.Closed || connection.State == ConnectionState.Broken)
                    connection.Open();

                reader = command.ExecuteReader();

                if (reader.Read())
                {
                    smartdayID = reader.GetString(0);

                }
            }
            catch (Exception ex)
            {
                errmsg = "GetProjectSmartdayID() -" + ex.Message;

                return false;
            }
            finally
            {
                if (reader != null)
                    reader.Close();
                if (connection.State == ConnectionState.Open)
                    connection.Close();
                command.Dispose();
            }

            return true;
        }


        public bool GetSiteSmartdayID(int ActNo, ref string smartdayID, out string errmsg)
        {
            smartdayID = "";
            errmsg = "";

            string sql = $"SELECT TOP 1 Inf7 FROM Actor WHERE ActNo={ActNo} AND Gr3=10 ";

            SqlCommand command = new SqlCommand(sql, connection)
            {
                CommandType = CommandType.Text,
                CommandTimeout = 600
            };

            SqlDataReader reader = null;

            reader = null;

            try
            {

                if (connection.State == ConnectionState.Closed || connection.State == ConnectionState.Broken)
                    connection.Open();

                reader = command.ExecuteReader();

                if (reader.Read())
                {
                    smartdayID = reader.GetString(0);

                }
            }
            catch (Exception ex)
            {
                errmsg = "GetProjectSmartdayID() -" + ex.Message;

                return false;
            }
            finally
            {
                if (reader != null)
                    reader.Close();
                if (connection.State == ConnectionState.Open)
                    connection.Close();
                command.Dispose();
            }

            return true;
        }

        public bool GetThingSmartdayID(string rno, ref string smartdayID, out string errmsg)
        {
            smartdayID = "";
            errmsg = "";

            string sql = $"SELECT TOP 1 Inf8 FROM R12 WHERE RNo='{rno}'";

            SqlCommand command = new SqlCommand(sql, connection)
            {
                CommandType = CommandType.Text,
                CommandTimeout = 600
            };

            SqlDataReader reader = null;

            reader = null;

            try
            {

                if (connection.State == ConnectionState.Closed || connection.State == ConnectionState.Broken)
                    connection.Open();

                reader = command.ExecuteReader();

                if (reader.Read())
                {
                    smartdayID = reader.GetString(0).Trim();

                }
            }
            catch (Exception ex)
            {
                errmsg = "GetThingSmartdayID() -" + ex.Message;

                return false;
            }
            finally
            {
                if (reader != null)
                    reader.Close();
                if (connection.State == ConnectionState.Open)
                    connection.Close();
                command.Dispose();
            }

            return true;
        }


        public bool GetOrderFromSmartDayOrderID(string orderSmartDayID, ref int ordNo, out string errmsg)
        {
            errmsg = "";
            ordNo = 0;

            string sql = $"SELECT TOP 1 OrdNo FROM Ord WHERE CsOrdno='{orderSmartDayID}' OR (CsOrdNo='' AND OrdNo={orderSmartDayID})";

            SqlCommand command = new SqlCommand(sql, connection)
            {
                CommandType = CommandType.Text,
                CommandTimeout = 600
            };

            SqlDataReader reader = null;

            reader = null;

            try
            {

                if (connection.State == ConnectionState.Closed || connection.State == ConnectionState.Broken)
                    connection.Open();

                reader = command.ExecuteReader();

                if (reader.Read())
                {
                    ordNo = reader.GetInt32(0);
                }
            }
            catch (Exception ex)
            {
                errmsg = "GetOrderFromSmartDayProjectID() -" + ex.Message;

                return false;
            }
            finally
            {
                if (reader != null)
                    reader.Close();
                if (connection.State == ConnectionState.Open)
                    connection.Close();
                command.Dispose();
            }

            return true;
        }

        public bool GetProjectFromSmartDayProjectID(string projectSmartDayID, ref int ordNo, out string errmsg)
        {
            errmsg = "";
            ordNo = 0;

            string sql = $"SELECT TOP 1 OrdNo FROM Ord WHERE Inf4='{projectSmartDayID}'";

            SqlCommand command = new SqlCommand(sql, connection)
            {
                CommandType = CommandType.Text,
                CommandTimeout = 600
            };

            SqlDataReader reader = null;

            reader = null;

            try
            {

                if (connection.State == ConnectionState.Closed || connection.State == ConnectionState.Broken)
                    connection.Open();

                reader = command.ExecuteReader();

                if (reader.Read())
                {
                    ordNo = reader.GetInt32(0);
                }
            }
            catch (Exception ex)
            {
                errmsg = "GetProjectFromSmartDayOrderID() -" + ex.Message;

                return false;
            }
            finally
            {
                if (reader != null)
                    reader.Close();
                if (connection.State == ConnectionState.Open)
                    connection.Close();
                command.Dispose();
            }

            return true;
        }

        public bool GetProjectFromSmartDayProjectIDOld(string projectSmartDayID, ref int rno, out string errmsg)
        {
            errmsg = "";
            rno = 0;

            string sql = $"SELECT TOP 1 Rno FROM R2 WHERE Inf7='{projectSmartDayID}'";

            SqlCommand command = new SqlCommand(sql, connection)
            {
                CommandType = CommandType.Text,
                CommandTimeout = 600
            };

            SqlDataReader reader = null;

            reader = null;

            try
            {

                if (connection.State == ConnectionState.Closed || connection.State == ConnectionState.Broken)
                    connection.Open();

                reader = command.ExecuteReader();

                if (reader.Read())
                {
                    rno = reader.GetInt32(0);
                }
            }
            catch (Exception ex)
            {
                errmsg = "GetProjectFromSmartDayOrderID() -" + ex.Message;

                return false;
            }
            finally
            {
                if (reader != null)
                    reader.Close();
                if (connection.State == ConnectionState.Open)
                    connection.Close();
                command.Dispose();
            }

            return true;
        }

        public bool GetNextFreeOrderLine(int ordNo, ref int lnNo, out string errmsg)
        {
            errmsg = "";
            lnNo = 0;

            string sql = $"SELECT ISNULL(MAX(LnNo),0) + 1 FROM OrdLn WHERE OrdNo={ordNo}";

            SqlCommand command = new SqlCommand(sql, connection)
            {
                CommandType = CommandType.Text,
                CommandTimeout = 600
            };

            SqlDataReader reader = null;

            reader = null;

            try
            {

                if (connection.State == ConnectionState.Closed || connection.State == ConnectionState.Broken)
                    connection.Open();

                reader = command.ExecuteReader();

                if (reader.Read())
                {
                    lnNo = reader.GetInt32(0);
                  
                }
            }
            catch (Exception ex)
            {
                errmsg = "GetNextFreeOrderLine() -" + ex.Message;

                return false;
            }
            finally
            {
                if (reader != null)
                    reader.Close();
                if (connection.State == ConnectionState.Open)
                    connection.Close();
                command.Dispose();
            }

            return true;
        }


        public bool GetNextFreeAgreementLine(int agrActNo, ref int agrNo, out string errmsg)
        {
            errmsg = "";
            agrNo = 0;

            string sql = $"SELECT ISNULL(MAX(AgrNo),0) + 1 FROM Agr WHERE AgrActNo={agrActNo}";

            SqlCommand command = new SqlCommand(sql, connection)
            {
                CommandType = CommandType.Text,
                CommandTimeout = 600
            };

            SqlDataReader reader = null;

            reader = null;

            try
            {

                if (connection.State == ConnectionState.Closed || connection.State == ConnectionState.Broken)
                    connection.Open();

                reader = command.ExecuteReader();

                if (reader.Read())
                {
                    agrNo = reader.GetInt32(0);

                }
            }
            catch (Exception ex)
            {
                errmsg = "GetNextFreeOrderLine() -" + ex.Message;

                return false;
            }
            finally
            {
                if (reader != null)
                    reader.Close();
                if (connection.State == ConnectionState.Open)
                    connection.Close();
                command.Dispose();
            }

            return true;
        }

        public bool GetNextFreeAgreementSrtLine(ref int srt, out string errmsg)
        {
            errmsg = "";
            srt = 0;

            string sql = $"SELECT ISNULL(MAX(Srt),0) + 1 FROM Agr ";

            SqlCommand command = new SqlCommand(sql, connection)
            {
                CommandType = CommandType.Text,
                CommandTimeout = 600
            };

            SqlDataReader reader = null;

            reader = null;

            try
            {

                if (connection.State == ConnectionState.Closed || connection.State == ConnectionState.Broken)
                    connection.Open();

                reader = command.ExecuteReader();

                if (reader.Read())
                {
                    srt = reader.GetInt32(0);

                }
            }
            catch (Exception ex)
            {
                errmsg = "GetNextFreeOrderLine() -" + ex.Message;

                return false;
            }
            finally
            {
                if (reader != null)
                    reader.Close();
                if (connection.State == ConnectionState.Open)
                    connection.Close();
                command.Dispose();
            }

            return true;
        }

        public int HasOrdNo(int ordNoToCheck, out string errmsg)
        {
            errmsg = "";
            int ordNo = 0;
            string sql = $"SELECT OrdNo FROM Ord WHERE OrdNo={ordNoToCheck}";

            SqlCommand command = new SqlCommand(sql, connection)
            {
                CommandType = CommandType.Text,
                CommandTimeout = 600
            };

            SqlDataReader reader = null;

            reader = null;

            try
            {

                if (connection.State == ConnectionState.Closed || connection.State == ConnectionState.Broken)
                    connection.Open();

                reader = command.ExecuteReader();

                if (reader.Read())
                {
                    ordNo = reader.GetInt32(0);
                }
            }
            catch (Exception ex)
            {
                errmsg = "GetNextFreeOrderLine() -" + ex.Message;

                return -1;
            }
            finally
            {
                if (reader != null)
                    reader.Close();
                if (connection.State == ConnectionState.Open)
                    connection.Close();
                command.Dispose();
            }

            return ordNo;

        }

        public bool GetActNo(int custNo, ref int actNo, out string errmsg)
        {
            errmsg = "";
            actNo = 0;

            string sql = $"SELECT ActNo FROM Actor WHERE CustNo={custNo} ";

            SqlCommand command = new SqlCommand(sql, connection)
            {
                CommandType = CommandType.Text,
                CommandTimeout = 600
            };

            SqlDataReader reader = null;

            reader = null;

            try
            {

                if (connection.State == ConnectionState.Closed || connection.State == ConnectionState.Broken)
                    connection.Open();

                reader = command.ExecuteReader();

                if (reader.Read())
                {
                    actNo = reader.GetInt32(0);

                }
            }
            catch (Exception ex)
            {
                errmsg = "GetNextFreeOrderLine() -" + ex.Message;

                return false;
            }
            finally
            {
                if (reader != null)
                    reader.Close();
                if (connection.State == ConnectionState.Open)
                    connection.Close();
                command.Dispose();
            }

            return true;
        }

        public bool GetMaterialFromSmartDayOrderID(string materialSmartDayID, ref int ordNo, ref int lnNo, out string errmsg)
        {
            errmsg = "";
            lnNo = 0;

            string sql;
                
            if (ordNo > 0)
               sql = $"SELECT TOP 1 LnNo,OrdNo FROM OrdLn WHERE OrdNo={ordNo} AND WebPg='{materialSmartDayID}'";
            else
                sql = $"SELECT TOP 1 LnNo,OrdNo FROM OrdLn WHERE WebPg='{materialSmartDayID}'";

            SqlCommand command = new SqlCommand(sql, connection)
            {
                CommandType = CommandType.Text,
                CommandTimeout = 600
            };

            SqlDataReader reader = null;

            reader = null;

            try
            {

                if (connection.State == ConnectionState.Closed || connection.State == ConnectionState.Broken)
                    connection.Open();

                reader = command.ExecuteReader();

                if (reader.Read())
                {
                    lnNo = reader.GetInt32(0);
                    ordNo = reader.GetInt32(1);
                }
            }
            catch (Exception ex)
            {
                errmsg = "GetOrderLinFromSmartDayOrderID() -" + ex.Message;

                return false;
            }
            finally
            {
                if (reader != null)
                    reader.Close();
                if (connection.State == ConnectionState.Open)
                    connection.Close();
                command.Dispose();
            }

            return true;
        }


        public bool UpdateCreateMaterialForOrder(int ordNo, Models.Material material, int custNo, int invoCustNo, int cur,out string errmsg)
        {
            errmsg = "";
            string sql;

            int vismaDt = DateTime.Now.Year * 10000 + DateTime.Now.Month * 100 + DateTime.Now.Day;
            int vismaTm = DateTime.Now.Hour * 100 + DateTime.Now.Minute;
        
            if (ordNo == 0)
            {
                errmsg = "Unable to find OrdNo for materials - aborting insert..";
                return true;
            }

            int lnNo = 0;
            if (GetMaterialFromSmartDayOrderID(material.id, ref ordNo, ref lnNo, out errmsg) == false)
                return false;
         
            decimal price = material.quantity != 0.0 ? (decimal)material.totalPrice / (decimal)material.quantity : 0.0M;
            DateTime tr = Utils.StringToDateTime(material.date);
            int vismaTrDt = tr.Year * 10000 + tr.Month * 100 + tr.Day;

            if (lnNo > 0)
            {
                sql = $"UPDATE OrdLn SET TrDt ={vismaTrDt}, ProdNo={material.itemNo}, FinN = {(decimal)material.quantity}, Descr={material.description}, ChDt ={vismaDt}, ChTm={vismaTm}, Price={price} WHERE OrdNo={ordNo} AND LnNo={lnNo}";
            }
            else 
            {
                if (GetNextFreeOrderLine(ordNo, ref lnNo, out errmsg) == false)
                    return false;
                sql = $"INSERT INTO OrdLn (ordNo,lnNo,TrDt,ProdNo,Descr,NoInvoAb,ChDt,ChTm,Price,CustNo,InvoCust,Cur,CreDt,CreTm,WebPg) VALUES ({ordNo},{lnNo},{vismaTrDt},'{material.itemNo}','{(material.description??"")}',{Utils.DecimalToString((decimal)material.quantity)},{vismaDt},{vismaTm},{Utils.DecimalToString(price)},{custNo},{invoCustNo},{cur},{vismaDt},{vismaTm},'{material.id}')";
            }

            SqlCommand command = new SqlCommand(sql, connection)
            {
                CommandType = CommandType.Text,
                CommandTimeout = 600
            };

            try
            {

                if (connection.State == ConnectionState.Closed || connection.State == ConnectionState.Broken)
                    connection.Open();

                command.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                errmsg = "UpdateCreateMaterialForOrder() -" + ex.Message;

                return false;
            }
            finally
            {
                if (connection.State == ConnectionState.Open)
                    connection.Close();
                command.Dispose();
            }

            return true;
        }

        public bool CreateSalaryCodeForOrder(string orderSmartDayID, Models.SalaryCode salaryCode, int custNo, int ordNo, out string errmsg)
        {
            errmsg = "";
            errmsg = "";
            string sql;
            DateTime tr = Utils.StringToDateTime(salaryCode.date);
            int vismaTrDt = tr.Year * 10000 + tr.Month * 100 + tr.Day;
            int vismaTrTm = tr.Hour * 100 + tr.Minute;

            int agrActNo = Utils.ReadConfigInt32("DefaultAgreementActNo", 1545);
            int vismaDt = DateTime.Now.Year * 10000 + DateTime.Now.Month * 100 + DateTime.Now.Day;
            int vismaTm = DateTime.Now.Hour * 100 + DateTime.Now.Minute;
           
            // See if we can get 'person involved' from salarycode
            if (salaryCode.userReference != null)
            {
                if (salaryCode.userReference.externalId != "")
                    if (Utils.StringToInt(salaryCode.userReference.externalId) > 0)
                        agrActNo = Utils.StringToInt(salaryCode.userReference.externalId);
            }

            int actNo = 0;
            if (GetActNo(custNo, ref actNo, out errmsg) == false)
                return false;

            // See if we have agreement with smartday projectreference
            int srt = 0;
            if (GetNextFreeAgreementSrtLine(ref srt, out errmsg) == false)
                return false;

            int agrNo = 0;
            if (GetAgreementFromSalaryCodeID(salaryCode.id, ref agrNo, out errmsg) == false)
                return false;

            if (agrNo > 0)
            {
                errmsg = "SalaryCode already registered";
                return true;
            }

            if (agrNo == 0)
            {
                if (GetNextFreeAgreementLine(agrActNo, ref agrNo, out errmsg) == false)
                    return false;
            }

            string descr2 = salaryCode.description ?? "";

//            if (insert)
                sql = $"INSERT INTO Agr (AgrActNo,AgrNo,FrDt,ToDt,FrTm,ToTm,NoInvoAb,ProdNo,Descr,ActNo,OrdNo,Srt,Descr2,ChDt,ChTm,CrActNo,CreDt,CreTm,Txt1,Txt2) VALUES ({agrActNo},{agrNo},{vismaTrDt},{vismaTrDt},{vismaTrTm},{vismaTrTm},{Utils.DecimalToString((decimal)salaryCode.amount)},'{salaryCode.salaryCodeReference.id}','{salaryCode.info}',{actNo},{ordNo},{srt},'{descr2}',{vismaDt},{vismaTm},{agrActNo},{vismaDt},{vismaTm},'{orderSmartDayID}','{salaryCode.id}')";
            //            else
            //                sql = $"UPDATE Agr SET FrDt={vismaTrDt}, ToDt={vismaTrDt}, FrTm={vismaTrTm}, ToTm={vismaTrTm}, NoInvoAb={Utils.DecimalToString((decimal)salaryCode.amount)}, ProdNo='{salaryCode.salaryCodeReference.id}', Descr='{salaryCode.info}',ActNo={actNo},OrdNo={ordNo},Srt={srt},TDescr2='{descr2}', Txt1='{projectSmartDayID}' WHERE Txt2='{salaryCode.id}'";
            SqlCommand command = new SqlCommand(sql, connection)
            {
                CommandType = CommandType.Text,
                CommandTimeout = 600
            };

            try
            {

                if (connection.State == ConnectionState.Closed || connection.State == ConnectionState.Broken)
                    connection.Open();

                command.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                errmsg = "CreateSalaryCodeForOrder() -" + ex.Message;

                return false;
            }
            finally
            {
                if (connection.State == ConnectionState.Open)
                    connection.Close();
                command.Dispose();
            }

            return true;

        }

        public bool GetAgreementFromSalaryCodeID(string salarayCodeID, ref int agrNo, out string errmsg)
        {
            errmsg = "";
            agrNo = 0;
            int agrActNo = Utils.ReadConfigInt32("AgreementActNo", 1545);

            string sql = $"SELECT TOP 1 AgrNo FROM Agr WHERE Txt2='{salarayCodeID}' AND AgrActNo={agrActNo}";

            SqlCommand command = new SqlCommand(sql, connection)
            {
                CommandType = CommandType.Text,
                CommandTimeout = 600
            };

            SqlDataReader reader = null;

            reader = null;

            try
            {

                if (connection.State == ConnectionState.Closed || connection.State == ConnectionState.Broken)
                    connection.Open();

                reader = command.ExecuteReader();

                if (reader.Read())
                {
                    agrNo = reader.GetInt32(0);
                }
            }
            catch (Exception ex)
            {
                errmsg = "GetAgreementFromSmartDayProjectID() -" + ex.Message;

                return false;
            }
            finally
            {
                if (reader != null)
                    reader.Close();
                if (connection.State == ConnectionState.Open)
                    connection.Close();
                command.Dispose();
            }

            return true;
        }


        public bool GetAgreementLineFromSmartDayOrderID(int agrNo, string orderSmartDayID, string orderSmartDayId, ref int lnNo, out string errmsg)
        {
            errmsg = "";
            agrNo = 0;

            int agrActNo = Utils.ReadConfigInt32("AgreementActNo", 1545);

            string sql = $"SELECT TOP 1 AgrNo FROM Agr WHERE Txt1='{orderSmartDayID}' AND AgrActNo={agrActNo}";

            SqlCommand command = new SqlCommand(sql, connection)
            {
                CommandType = CommandType.Text,
                CommandTimeout = 600
            };

            SqlDataReader reader = null;

            reader = null;

            try
            {

                if (connection.State == ConnectionState.Closed || connection.State == ConnectionState.Broken)
                    connection.Open();

                reader = command.ExecuteReader();

                if (reader.Read())
                {
                    agrNo = reader.GetInt32(0);
                }
            }
            catch (Exception ex)
            {
                errmsg = "GetAgreementLineFromSmartDayOrderID() -" + ex.Message;

                return false;
            }
            finally
            {
                if (reader != null)
                    reader.Close();
                if (connection.State == ConnectionState.Open)
                    connection.Close();
                command.Dispose();
            }

            return true;
        }

    }
}

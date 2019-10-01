using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartDayClient
{
    public class DBaccess
    {
        readonly SqlConnection connection;

        public static string queryGetCustomers = "SELECT Actor.ActNo, Actor.CustNo,Actor.Nm,Actor.Ad1,Actor.Ad2,Actor.Ad3,Actor.PNo,Actor.PArea,ISNULL(Ctry.ISO,'SE'),Actor.Phone,Actor.MobPh,Actor.MailAd,Actor.BsNo,Actor.YrRef,Actor.Gr3 FROM Actor " +
                        "LEFT OUTER JOIN Ctry ON Actor.Ctry = Ctry.CtryNo WHERE Actor.DelToAct=0 AND Actor.CustNo>0 ";

        public static string queryGetCustomerContact = "SELECT Actor.ActNo,Actor.Nm,Actor.Ad1,Actor.Ad2,Actor.Ad3,Actor.PNo,Actor.PArea,ISNULL(Ctry.ISO,'SE'),Actor.Phone,Actor.MobPh,Actor.MailAd,Actor.BsNo FROM Actor " +
                        "LEFT OUTER JOIN Ctry ON Actor.Ctry = Ctry.CtryNo " +
                        "WHERE Actor.LiaAct=#1#";

        public static string queryGetCustomerDelivery = "SELECT Actor.ActNo, Actor.Nm,Actor.Ad1,Actor.Ad2,Actor.Ad3,Actor.PNo,Actor.PArea,ISNULL(Ctry.ISO,'SE'),Actor.Phone,Actor.MobPh,Actor.MailAd,Actor.BsNo FROM Actor " +
                        "LEFT OUTER JOIN Ctry ON Actor.Ctry = Ctry.CtryNo " +
                        "WHERE Actor.DelToAct=#1#";

        public static string queryGetOrders = "SELECT DISTINCT Ord.Gr12, Ord.OrdDt, Ord.OrdNo, Ord.Nm, Ord.Ad1, Ord.Ad2, Ord.Ad3, Ord.PNo, Ord.PArea, " +
                       "Ord.DelNm, Ord.DelAd1, Ord.DelAd2, Ord.DelAd3, Ord.DelPNo, Ord.DelPArea, Ord.DelTrm, Ord.DelDt, Ord.Inf, " +
                       "Ord.Inf2, Ord.Inf3, Ord.Inf4,Ord.Inf5, Ord.NoteNm, Ord.YrRef, Ord.OurRef,Ord.Gr3,Ord.CsOrdNo,Ord.CustNo,Ord.EmpNo,Ord.R7,Ord.Gr,Ord.CreUsr,Ord.R12,Ord.R2,Ord.SelBuy " +
                       "FROM Ord WITH (NOLOCK) WHERE Ord.Gr3=10 AND Ord.TrTp=1 ";

        public static string queryGetOrderLinesByOrdNo =
                      "SELECT DISTINCT OrdLn.LnNo, OrdLn.ProdNo, OrdLn.NoInvo, OrdLn.Price, OrdLn.Descr,OrdLn.WebPg,OrdLn.R7 " +
                      "FROM OrdLn WITH (NOLOCK) WHERE OrdLn.OrdNo= #1#";

        public static string queryGetProducts =
                    "SELECT  DISTINCT Prod.ProdNo, Prod.Descr, ISNULL(Unit.Descr,'stk'),ISNULL(Txt.txt,''),ISNULL(TaxAcGr.Descr,'') FROM Prod " +
                    "LEFT OUTER JOIN Unit ON Unit.Un=Prod.StSaleUn " +
                    "LEFT OUTER JOIN Txt ON Txt.TxtTp=42 AND Txt.Lang=46 AND Txt.TxtNo=Prod.Gr " +
                    "LEFT OUTER JOIN TaxAcGr ON TaxAcGr.ProdGrNo=Prod.ProdGr " +
                    "WHERE Prod.Gr4=1 AND Prod.Gr=1";

        public static string queryGetThings =
                    "SELECT R12.RNo, R12.Nm, ISNULL(Actor.Phone,''),R12.Inf7,R12.Ad4,R12.R3,ISNULL(R3.R9,''),R12.NoteNm,R12.Dt3,R12.Dt4,R12.Dt1,R12.Inf8,R12.CustNo,R12.Gr3,R12.Inf5,R12.Inf4,R12.PictFNm,ISNULL(R8.Nm,'') FROM R12 " +
                    "LEFT OUTER JOIN Actor ON Actor.CustNo=R12.CustNo " +
                    "LEFT OUTER JOIN R3 ON R3.RNo= R12.R3 " +
                    "LEFT OUTER JOIN R8 ON R8.RNo= R12.R8 " +

                    "WHERE R12.R3<> '' AND R12.Gr3=1 ";

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
            "SELECT R2.RNo,R2.CustNo, R2.Nm, R2.Ad1, R2.Ad2, R2.PNo, R2.PArea, R2.Gr11,R2.CreDt, R2.Rsp WHERE R2.Gr12=5 AND R2.CustNo>0 ";

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
            }

            return dt;

        }

        public bool GetThings(ref List<Models.Thing> things, out string errmsg)
        {
            errmsg = "";
            string sql = queryGetThings;
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
                    Models.Thing thing = new Models.Thing()
                    {
                        id = null,
                        externalId =  null,
                        make = reader.GetString(idx++), // =Rno
                        name = reader.GetString(idx++).Trim(),
                    };
                    string phone = reader.GetString(idx++).Trim();
                    string contact = reader.GetString(idx++).Trim();
                    string mobile = reader.GetString(idx++).Trim();
                    string contractType = reader.GetInt32(idx++).ToString();
                    contractType += " - " +reader.GetString(idx++).Trim();

                    thing.note = Utils.ReadMemoFile(reader.GetString(idx++).Trim());

                    thing.categoryReference.id = Models.CategoryType.Things_Serviceenhed.ToString();
                    thing.categoryReference.externalId = Models.CategoryType.Things_Serviceenhed.ToString();
                    thing.categoryReference.name = null;
                    thing.color = null;
                    thing.created = null;
                    thing.handymanThing = null;
                    thing.lastRepairDate = null;
                    thing.lastServiceDate = null;
                    thing.manufacturingDate = null;
                    thing.model = null;
                    thing.userReference.externalId = null;
                    thing.userReference.id = null;

                    thing.lastKnownLocation.externalId = thing.externalId;
                    thing.lastKnownLocation.id = thing.id;
                    
                    thing.lastKnownLocation.address1 = null;
                    thing.lastKnownLocation.address2 = null;
                    thing.lastKnownLocation.housenumber = null;
                    thing.lastKnownLocation.postalCode = null;
                    thing.lastKnownLocation.postalArea = null;
                    thing.lastKnownLocation.country = null;
                    int dt = reader.GetInt32(idx++); // Dt3
                    thing.installedDate = dt > 0 ? Utils.DateTime2String(0, Utils.VismaDate2DateTime(dt)) + "Z" : null; // Dt3;
                    dt = reader.GetInt32(idx++);   // Dt4
                    thing.warrantyStartDate = dt > 0 ? Utils.DateTime2String(0, Utils.VismaDate2DateTime(dt)) + "Z" : null; // Dt4
                    dt = reader.GetInt32(idx++);  // Dt1
                    thing.warrantyEndDate = dt > 0 ? Utils.DateTime2String(0, Utils.VismaDate2DateTime(dt)) + "Z" : null; // Dt1

                    string smartDayID = reader.GetString(idx++).Trim(); // PictFNm
                    if (smartDayID != "")
                        thing.id = smartDayID;

                    int custNo = reader.GetInt32(idx++);

                    int gr3 = reader.GetInt32(idx++);
                    thing.state = gr3 == 1 ? "Active" : "Disabled";
                    thing.customerReference.externalId = custNo > 0 ? custNo.ToString() : null;
                    thing.customerReference.id = custNo > 0 ? custNo.ToString() : null;
                    //name=R12.Inf7, cellPhoneNumber=R12.Ad4

                    thing.placement = reader.GetString(idx++).Trim();       // Inf5
                    thing.serialNumber = reader.GetString(idx++).Trim();    // Inf4

                    string siteid = reader.GetString(idx++).Trim(); //PictFNm
                    thing.model = reader.GetString(idx++).Trim(); //R8.Nm
                    if (thing.model == "")
                        thing.model = null;

                    thing.parentReference.id = null; ;
                    thing.parentReference.externalId = null;
                    if (siteid != "")
                    {
                        thing.parentReference.id = siteid;
                        thing.parentReference.externalId = siteid;
                    }

                    thing.contacts.Add(new Models.ThingContact()
                    {
                        name = contact != "" ? contact : null,
                        cellPhoneNumber = mobile != "" ? mobile : null,
                        phoneNumber = phone != "" ? phone : null,
                        email = null,
                        handymanContact = null,

                    });
                    thing.smartdayThing.contractType = contractType != "" ? contractType : null;
                    thing.smartdayThing.serviceObjectUsage = null;
                    thing.smartdayThing.addressLastUpdated = null;

                    things.Add(thing);
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

            foreach(Models.Thing thing in things)
            {
                // Lookup registered Customer.ID
                string smartDayID = "";
                if (GetCustomerSmartDayId(Utils.StringToInt(thing.customerReference.externalId), ref smartDayID, out errmsg))
                {
                    if (smartDayID != "")
                        thing.customerReference.id = smartDayID;
                }

            }

            return true;
        }


        public bool GetCustomerSmartDayId(int custNo, ref string smartDayId, out string errmsg)
        {
            smartDayId = "";
            errmsg = "";

            string sql = $"SELECT Inf7 FROM Actor WHERE CustNo={custNo}";

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
                    customer.CountryCode = reader.GetString(idx++).Trim();
                    customer.Phone = reader.GetString(idx++).Trim();
                    customer.Mobile = reader.GetString(idx++).Trim();
                    customer.EmailAddress = reader.GetString(idx++).Trim();
                    customer.CompanyNo = reader.GetString(idx++).Trim();
                    customer.YourRef = reader.GetString(idx++).Trim();
                    customer.Group3 = reader.GetInt32(idx++);
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
                customer.ContactAddressList.Add(actor);
            actors.Clear();
            if (GetCustomerDelivery(customer.ActorNo, ref actors, out errmsg) == false)
                return false;
            foreach (Models.VismaActor actor in actors)
                customer.DeliveryAddressList.Add(actor);

            return true;
        }



        public bool GetNewCustomers(ref List<Models.VismaCustomer> customers, out string errmsg)
        {
            errmsg = "";
            customers.Clear();

            string sql = queryGetCustomers + " AND Actor.Gr=1";

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
                        CountryCode = reader.GetString(idx++).Trim(),
                        Phone = reader.GetString(idx++).Trim(),
                        Mobile = reader.GetString(idx++).Trim(),
                        EmailAddress = reader.GetString(idx++).Trim(),
                        CompanyNo = reader.GetString(idx++).Trim(),
                        YourRef = reader.GetString(idx++).Trim(),
                        Group3 = reader.GetInt32(idx++)
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
                //  List<Models.VismaActor> actors = new List<Models.VismaActor>();
                //  if (GetCustomerContact(customer.ActorNo, ref actors, out errmsg) == false)
                //      return false;
                //                foreach (Models.VismaActor actor in actors)
                customer.ContactAddressList.Add(new Models.VismaActor()
                {
                    Name = customer.YourRef,
                    EmailAddress = customer.EmailAddress,
                    Phone = customer.Phone,
                    Mobile = customer.Mobile,
                    ActorNo = customer.ActorNo
                });
//                   customer.ContactAddressList.Add(actor);
 //               actors.Clear();
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
                        Information1 = reader.GetString(fld++), // Inf
                        Information2 = reader.GetString(fld++), // Inf2
                        Information3 = reader.GetString(fld++), // Inf3
                        Information4 = reader.GetString(fld++), // Inf4
                        Information5 = reader.GetString(fld++), // Inf5

                        Memo = Utils.ReadMemoFile(reader.GetString(fld++).Trim()),
                        YourReference = reader.GetString(fld++),
                        OurReference = reader.GetString(fld++),
                        Status = reader.GetInt32(fld++),
                        CustomerOrSupplierOrderNo = reader.GetString(fld++).Trim(),
                        CustomerNo = reader.GetInt32(fld++),
                        //Ord.EmpNo,Ord.R7,Ord.Gr,Ord.CreUsr
                        EmpNo = reader.GetInt32(fld++),         
                        CategoryName = reader.GetString(fld++), // R7
                        Group1 = reader.GetInt32(fld++), // Gr
                        CreUsr = reader.GetString(fld++).Trim(),
                        R12 = reader.GetString(fld++).Trim(),
                        R2 = reader.GetInt32(fld++),
                        SelBuy = reader.GetInt32(fld++)

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
                List<Models.VismaOrderLine> lines = new List<Models.VismaOrderLine>();
                if (GetOrderLines(order.OrderNo, ref lines, out errmsg) == false)
                    return false;
                foreach (Models.VismaOrderLine line in lines)
                    order.OrderLines.Add(line);

                List<Models.VismaAgreement> agreements = new List<Models.VismaAgreement>();
                if (GetOrderAgreements(order.OrderNo, ref agreements, out errmsg) == false)
                    return false;
                foreach (Models.VismaAgreement agreement in agreements)
                    order.AgreementLines.Add(agreement);

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

        public bool RegisterThingSmartdayID(string Rno, string smartDayId, string siteId, out string errmsg)
        {
            errmsg = "";

            string sql = $"UPDATE R12 SET Inf8='{smartDayId}',PictFNm='{siteId}',Gr3=5 WHERE Rno='{Rno}'";
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

        public bool RegisterProjectSmartdayID(int ordNo, string smartDayId,out string errmsg)
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

        public bool RegisterOrderLineSmartdayID(int ordNo, int lineNo, string smartDayId, out string errmsg)
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


        public bool RegisterActorSmartdayID(int actNo, string smartDayId, out string errmsg)
        {
            errmsg = "";

            string sql = $"UPDATE Actor Set Inf7='{smartDayId}',Gr=5 WHERE ActNo={actNo}";

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

        public bool RegisterActorSmartdayIDSite(int actNo, string smartDayId, out string errmsg)
        {
            errmsg = "";

            string sql = $"UPDATE Actor Set Inf8='{smartDayId}' WHERE ActNo={actNo}";

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


        public bool GetThingSmartdayID(string rno, ref string smartdayID, out string errmsg)
        {
            smartdayID = "";
            errmsg = "";

            string sql = $"SELECT TOP 1 PictFNm FROM R12 WHERE RNo={rno}";

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


        public bool GetOrderFromSmartDayProjectID(string projectSmartDayID, ref int ordNo, out string errmsg)
        {
            errmsg = "";
            ordNo = 0;

            string sql = $"SELECT TOP 1 OrdNo FROM Ord WHERE CsOrdno='{projectSmartDayID}' OR (CsOrdNo='' AND OrdNo={projectSmartDayID})";

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

        public bool GetOrderLinFromSmartDayOrderID(string orderSmartDayID, ref int ordNo, ref int lnNo, out string errmsg)
        {
            errmsg = "";
            lnNo = 0;

            string sql;
                
            if (ordNo > 0)
               sql = $"SELECT TOP 1 LnNo,OrdNo FROM OrdLn WHERE OrdNo={ordNo} AND WebPg='{orderSmartDayID}'";
            else
                sql = $"SELECT TOP 1 LnNo,OrdNo FROM OrdLn WHERE WebPg='{orderSmartDayID}'";

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


        public bool UpdateCreateMaterialForOrder(string projectSmartDayID, string orderSmartDayID, Models.Material material, int custNo, int invoCustNo, int cur,out string errmsg)
        {
            errmsg = "";
            string sql;

            int vismaDt = DateTime.Now.Year * 10000 + DateTime.Now.Month * 100 + DateTime.Now.Day;
            int vismaTm = DateTime.Now.Hour * 100 + DateTime.Now.Minute;
            // See if we have project with smartday reference

            int ordNo = 0;
            if (GetOrderFromSmartDayProjectID(projectSmartDayID, ref ordNo, out errmsg) == false)
                return false;

            int lnNo = 0;
            if (GetOrderLinFromSmartDayOrderID( orderSmartDayID, ref ordNo, ref lnNo, out errmsg) == false)
                return false;

            if (ordNo == 0)
            {
                errmsg = "Unable to find OrdNo for materials - aborting insert..";
                return true;
            }

            decimal price = material.quantity != 0.0 ? (decimal)material.totalPrice / (decimal)material.quantity : 0.0M;
            DateTime tr = Utils.StringToDateTime(material.date);
            int vismaTrDt = tr.Year * 10000 + tr.Month * 100 + tr.Day;

            if (lnNo > 0)
            {
                sql = $"UPDATE OrdLn SET TrDt ={vismaTrDt}, ProdNo={material.itemNo}, NoInvoAb = {(decimal)material.quantity}, Descr={material.description}, ChDt ={vismaDt}, ChTm={vismaTm}, Price={price} WHERE OrdNo={ordNo} AND LnNo={lnNo}";
            }
            else 
            {
                if (GetNextFreeOrderLine(ordNo, ref lnNo, out errmsg) == false)
                    return false;
                sql = $"INSERT INTO OrdLn (ordNo,lnNo,TrDt,ProdNo,Descr,NoInvoAb,ChDt,ChTm,Price,CustNo,InvoCust,Cur,CreDt,CreTm,WebPg) VALUES ({ordNo},{lnNo},{vismaTrDt},'{material.itemNo}','{(material.description??"")}',{Utils.DecimalToString((decimal)material.quantity)},{vismaDt},{vismaTm},{Utils.DecimalToString(price)},{custNo},{invoCustNo},{cur},{vismaDt},{vismaTm},'{orderSmartDayID}')";
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

        public bool CreateSalaryCodeForOrder(string projectSmartDayID, string orderSmartDayID, Models.SalaryCode salaryCode, int custNo, int ordNo, out string errmsg)
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
                sql = $"INSERT INTO Agr (AgrActNo,AgrNo,FrDt,ToDt,FrTm,ToTm,NoInvoAb,ProdNo,Descr,ActNo,OrdNo,Srt,Descr2,Txt1,Txt2,ChDt,ChTm,CrActNo,CreDt,CreTm) VALUES ({agrActNo},{agrNo},{vismaTrDt},{vismaTrDt},{vismaTrTm},{vismaTrTm},{Utils.DecimalToString((decimal)salaryCode.amount)},'{salaryCode.salaryCodeReference.id}','{salaryCode.info}',{actNo},{ordNo},{srt},'{descr2}','{projectSmartDayID}','{salaryCode.id}',{vismaDt},{vismaTm},{agrActNo},{vismaDt},{vismaTm})";
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


        public bool GetAgreementLineFromSmartDayProjectID(int agrNo, string projectSmartDayID, string orderSmartDayId, ref int lnNo, out string errmsg)
        {
            errmsg = "";
            agrNo = 0;

            int agrActNo = Utils.ReadConfigInt32("AgreementActNo", 1545);

            string sql = $"SELECT TOP 1 AgrNo FROM Agr WHERE Txt1='{projectSmartDayID}' AND AgrActNo={agrActNo}";

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

    }
}

using SmartDayClient.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartDayClient
{
    class Program
    {
        static void Main(string[] args)
        {
            SmartDayHttpClient httpClient = new SmartDayHttpClient();

            List<Models.SalaryCode> existingSalaryCodes = httpClient.GetSalaryCodesAsync().Result;
            if (existingSalaryCodes == null)
                Utils.WriteLog($"GetSalaryCodesAsync failed - {httpClient.LastError}");
    /*        else
            {
                foreach (Models.SalaryCode salaryCode in existingSalaryCodes)
                {
                    Utils.WriteLog("salaryCode id " + salaryCode.id);
                    Utils.WriteLog("salaryCode name " + salaryCode.description);

                }
            }
            */
            List<Models.Department> existimgDepartments = httpClient.GetDepartmentsAsync().Result;
            if (existimgDepartments == null)
                Utils.WriteLog($"GetDepartmentsAsync failed - {httpClient.LastError}");
            else
            {
                foreach (Models.Department department in existimgDepartments)
                {
                    Utils.WriteLog("Department id " + department.id);
                    Utils.WriteLog("Department name " + department.name);
                    Utils.WriteLog("Postal address1 " + department.postalAddress.address1);
                    Utils.WriteLog("Postal code " + department.postalAddress.postalCode);
                    Utils.WriteLog("Postal area " + department.postalAddress.postalArea);
                }
            }

/*
            List<Models.Category> existingCategories = httpClient.GetCategoriesAsync().Result;
            if (existingCategories == null)
                Utils.WriteLog($"GetCategoriesAsync failed - {httpClient.LastError}");
            else
            {
                foreach (Models.Category category in existingCategories)
                {
                    Utils.WriteLog("Category id " + category.id);
                    Utils.WriteLog("Category externalid " + category.externalId);
                    Utils.WriteLog("Category name " + category.name);
                }
            }
*/


     /*       if (Utils.ReadConfigInt32("SyncThings", 0) > 0)
            {
                Sync sync = Utils.ReadSyncTime(Utils.ReadConfigString("ThingsSyncFile", "ThingsSyncFile.xml"));
                List<Models.VismaThing> things = new List<Models.VismaThing>();
                DBaccess db = new DBaccess();
                if (db.GetThings("", ref things, sync.LastestSync, out string errmsg) == false)
                {
                    Utils.WriteLog($"ERROR: db.GetThings() - {errmsg}");
                }
                else
                {
                    if (things.Count == 0)
                    {
                        Utils.WriteLog($"No new/changed things to synchronize to Smartday");
                        sync.LastestSync = DateTime.Now;
                        Utils.WriteSyncTime(sync, Utils.ReadConfigString("ThingsSyncFile", "ThingsSyncFile.xml"));
                    }
                    else
                    {
                        if (SyncThings(things))
                        {
                            sync.LastestSync = DateTime.Now;
                            Utils.WriteSyncTime(sync, Utils.ReadConfigString("ThingsSyncFile", "ThingsSyncFile.xml"));
                        }
                    }
                }
            }

            */
            if (Utils.ReadConfigInt32("SyncItems", 0) > 0)
            {
                List<Models.Item> items = new List<Models.Item>();
                DBaccess db = new DBaccess();
                if (db.GetNewProducts(ref items, out string errmsg) == false)
                {
                    Utils.WriteLog($"ERROR: db.GetNewProducts() - {errmsg}");
                }
                else
                {
                    SyncItems(items, Utils.ReadConfigString("DefaultStoreID", "1"));
                }
            }

            /*     if (Utils.ReadConfigInt32("SyncCustomers", 0) > 0)
                 {
                     Sync sync = Utils.ReadSyncTime(Utils.ReadConfigString("CustomerSyncFile", "CustomerSyncFile.xml"));
                     List<Models.VismaCustomer> vismaCustomers = new List<Models.VismaCustomer>();
                     DBaccess db = new DBaccess();
                     if (db.GetNewCustomers(ref vismaCustomers, sync.LastestSync, out string errmsg) == false)
                     {
                         Utils.WriteLog($"ERROR: db.GetNewCustomers() - {errmsg}");
                     }
                     else
                     {
                         if (vismaCustomers.Count == 0)
                         {
                             Utils.WriteLog($"No new/changed customers to synchronize to Smartday");
                             sync.LastestSync = DateTime.Now;
                             Utils.WriteSyncTime(sync, Utils.ReadConfigString("CustomerSyncFile", "CustomerSyncFile.xml"));

                         }
                         else
                         {
                             if (SyncCustomersAndSites(vismaCustomers) == true)
                             {
                                 sync.LastestSync = DateTime.Now;
                                 Utils.WriteSyncTime(sync, Utils.ReadConfigString("CustomerSyncFile", "CustomerSyncFile.xml"));
                             }
                         }
                     }
                 }*/

            if (Utils.ReadConfigInt32("SyncOrders", 0) > 0)
            {
                List<Models.VismaOrder> vorders = new List<Models.VismaOrder>();

                DBaccess db = new DBaccess();
                if (db.GetNewOrders(ref vorders, out string errmsg) == false)
                {
                    Utils.WriteLog("ERROR: db.GetNewOrders() - " + errmsg);
                }
                else
                {
                    if (vorders.Count == 0)
                        Utils.WriteLog($"No new/changed orders to synchronize to Smartday");
                    else
                        SyncOrdersAndProjects(vorders);
                }

            }

            if (Utils.ReadConfigInt32("SyncOrdersFromSmartDay", 0) > 0)
            {

                SyncLatestOrderChangesFromSmartDay();

            }
        }

        /// <summary>
        /// Sync from SmartDay to Visma - detect changed smartday orders by date/time
        /// 
        /// </summary>
        /// <returns></returns>
        private static bool SyncLatestOrderChangesFromSmartDay()
        {
            SmartDayHttpClient httpClient = new SmartDayHttpClient();
            DBaccess db = new DBaccess();
            string errmsg;

            Sync sync = Utils.ReadSyncTime(Utils.ReadConfigString("OrderChangeSyncFile", "OrderChangeSyncFile.xml"));

            Utils.WriteLog("Getting new/changed order FROM Smartday");

            List<Models.Order> newOrders = httpClient.GetOrdersAsync(sync.LastestSync).Result;

            if (newOrders == null)
                return false;
            foreach (Models.Order order in newOrders)
            {
                if (Utils.ReadConfigString("TestOrder","") != "")
                    if (order.externalId.IndexOf(Utils.ReadConfigString("TestOrder", "")) == -1)
                        continue;
            

                int vismaOrdNo = Utils.StringToInt(order.smartdayOrder.projectReference.externalId ?? "0");

                // Only look at approved
                if (order.smartdayOrder.status != Models.OrderStatus.Finished_Aborted) // = 7
                    continue;

           

                vismaOrdNo = db.HasOrdNo(vismaOrdNo, out errmsg);
                if (vismaOrdNo == -1)
                {
                    Utils.WriteLog("Error: db.HasOrdNo() - " + errmsg);
                    continue;
                }
              
                if (vismaOrdNo == 0)
                {
                    Utils.WriteLog($"Smartday order {order.id} has no known project reference {order.smartdayOrder.projectReference.externalId}");
                    continue;
                }

                Utils.WriteLog($"Found smartday order {order.id} (visma ordno {vismaOrdNo} with change (status 7 - Finished/aborted)");

                int vismaCustNo = Utils.StringToInt(order.customerReference.externalId ?? "0");

                string thingRef = "";
                if (order.externalId != null)
                {
                    int n = order.externalId.IndexOf("-");
                    thingRef = order.externalId.Substring(n + 1).Trim();
                }
               

                // Thing new - if so create in Visma

                //                if (order.smartdayOrder.thingReference != null)
                //                {
                //                    if (order.smartdayOrder.thingReference.id != null)
                //                    {
                //                        string thingID = order.smartdayOrder.thingReference.id;
                //                        // Lookup 'anlaeg' in Visma
                //                        Models.VismaThing vthing = new Models.VismaThing();
                //                        vthing.RNo = "";
                //                        if (db.GetThingFromSmartdayID(thingID, ref vthing, out errmsg) == false)
                //                        {
                //                            Utils.WriteLog("Error: db.GetThingFromSmartdayID() - " + errmsg);
                //                            continue;
                //                        }
                //                        // If no hit - try ExternalID ( = R12.Rno)
                //                        if (vthing.RNo == "")
                //                        {
                //                            if (db.GetThing(order.smartdayOrder.thingReference.externalId, ref vthing, out errmsg) == false)
                //                            {
                //                                Utils.WriteLog("Error: db.GetThing() - " + errmsg);
                //                                continue;
                //                            }
                //                            // Repair Visma thing - set smartDayID
                //                            if (vthing.RNo != "")
                //                                db.RegisterThingSmartdayID(vthing.RNo, thingID, "", out errmsg);
                //                        }

                //                        // Go create thing in Visma..

                //                        if (vthing.RNo == "")
                //                        {
                //                            Utils.WriteLog($"Found new Thing for smartday order {order.id} (visma ordno {vismaOrdNo}..");

                //                            Models.Thing existingThing = httpClient.GetThingAsync(order.smartdayOrder.thingReference.id).Result;
                //                            if (existingThing != null)
                //                            {
                //                                vthing.RNo = order.smartdayOrder.thingReference.externalId; //!!
                //                                Models.Site thingSite = null;

                //                                if (order.siteReference != null)
                //                                    if (order.siteReference.id != null)
                //                                        thingSite = httpClient.GetSiteAsync(order.siteReference.id).Result;

                //                                // Create new thing in Visma and link to order (line?)
                //                                vthing.Inf8 = existingThing.id;
                //                                vthing.Name = existingThing.name ?? "";
                //                                vthing.Memo = existingThing.note ?? "";
                //                                vthing.Inf5 = existingThing.placement ?? "";
                //                                vthing.Inf = existingThing.serialNumber ?? "";
                //                                vthing.Gr3 = existingThing.state == "Active" ? 1 : 0;

                //                                if (Utils.StringToDateTime(existingThing.installedDate) != DateTime.MinValue)
                //                                    vthing.Dt3 = Utils.DateTimeToVismaDate(Utils.StringToDateTime(existingThing.installedDate));
                //                                if (Utils.StringToDateTime(existingThing.warrantyStartDate) != DateTime.MinValue)
                //                                    vthing.Dt4 = Utils.DateTimeToVismaDate(Utils.StringToDateTime(existingThing.warrantyStartDate));
                //                                if (Utils.StringToDateTime(existingThing.warrantyEndDate) != DateTime.MinValue)
                //                                    vthing.Dt1 = Utils.DateTimeToVismaDate(Utils.StringToDateTime(existingThing.warrantyEndDate));

                //                                if (existingThing.customerReference != null)
                //                                    vthing.CustNo = Utils.StringToInt(existingThing.customerReference.externalId ?? "");

                //                                if (existingThing.contacts != null)
                //                                {
                //                                    if (existingThing.contacts.Count > 0)
                //                                    {
                //                                        vthing.Inf7 = existingThing.contacts[0].name ?? "";
                //                                        vthing.Ad4 = existingThing.contacts[0].cellPhoneNumber ?? "";
                //                                    }
                //                                }

                //                                if (thingSite != null)
                //                                {
                //                                    if (thingSite.postalAddress != null)
                //                                    {
                //                                        vthing.Ad1 = thingSite.postalAddress.address1 + " " + thingSite.postalAddress.housenumber;
                //                                        vthing.Ad2 = thingSite.postalAddress.address2 ?? "";
                //                                        vthing.PArea = thingSite.postalAddress.postalArea;
                //                                        vthing.PNo = thingSite.postalAddress.postalCode;
                //                                        vthing.Ctry = Utils.MapCountry(thingSite.postalAddress.country);
                //                                    }
                //                                }

                //                                if (db.CreateThing(vthing, out errmsg) && vthing.RNo != "")
                //                                {
                //                                    Utils.WriteLog($"New smartday Thing added to Visma RNo = {vthing.RNo}");
                //                                    // Update smartday thing with Visma RNo (in externalId)
                ///*                                    existingThing.externalId = vthing.RNo;
                //                                    List<Models.Thing> things = new List<Models.Thing>();
                //                                    things.Add(existingThing);
                //                                    List<Models.Result> results = httpClient.CreateThingsAsync(things).Result;
                //                                    if (results.Count > 0)
                //                                    {
                //                                        if (results[0].hasError == false)
                //                                        {
                //                                            Utils.WriteLog($"Thing {  existingThing.id} updated ith Visma RNo {existingThing.externalId}");
                //                                            return true;
                //                                        }
                //                                        else
                //                                            Utils.WriteLog($"Error updating order   - {results[0].errorMessage} ");
                //                                    }*/
                //                                }
                //                            ;
                //                            }
                //                        }

                //                    }
                //                }

                Utils.WriteLog("Fetching materials..");
                List<Models.Material> materials = httpClient.GetMaterialsForOrder(order.id).Result;
                if (materials != null)
                {
                    List<Models.Material> materialsToStore = new List<Models.Material>();
                    foreach (Models.Material material in materials)
                    {
                        if (material.status == Models.MaterialStatus.Exported || material.status == Models.MaterialStatus.Used || material.status == Models.MaterialStatus.Approved)
                        {
                            Utils.WriteLog($"Found material for order {order.id} with change (status 1,2  or 3) :  {material.itemNo} - id {material.id}");
                            materialsToStore.Add(material);
                        }
                    }

                    if (materialsToStore.Count > 0)
                    {
                        Utils.WriteLog("Saving new materials to Visma");
                        foreach (Models.Material materialToStore in materialsToStore)
                        {
                            if (db.UpdateCreateMaterialForOrder(vismaOrdNo, materialToStore, vismaCustNo, vismaCustNo, Utils.ReadConfigInt32("DefaultCur",46), out errmsg) == false)
                                Utils.WriteLog("ERROR: db.UpdateCreateMaterialForOrder() - " + errmsg);
                        }
                    }
                }

                List<Models.SalaryCode> salaryCodesToStore = new List<Models.SalaryCode>();

                Utils.WriteLog("Fetching salarycodes..");
                List<Models.SalaryCode> salaryCodes = httpClient.GetSalaryCodesForOrder(order.id).Result;
                if (salaryCodes != null)
                {
                    foreach (Models.SalaryCode salaryCode in salaryCodes)
                    {
                        if (salaryCode.status == Models.SalaryCodeStatus.Exported || salaryCode.status == Models.SalaryCodeStatus.Used || salaryCode.status == Models.SalaryCodeStatus.Approved)
                        {
                            Utils.WriteLog($"Found salaryCode for order {order.id} with change (status 1,2 or 3) :  {salaryCode.info} - id {salaryCode.id}");
                            salaryCodesToStore.Add(salaryCode);

                        }
                    }

                    if (salaryCodesToStore.Count > 0)
                    {
                        Utils.WriteLog("Saving new salaryCode to Visma");
                        foreach (Models.SalaryCode salaryCodeToStore in salaryCodesToStore)
                        {
                            if (db.CreateSalaryCodeForOrder(order.id, salaryCodeToStore, vismaCustNo, vismaOrdNo, out  errmsg) == false)
                                Utils.WriteLog("ERROR: db.CreateSalaryCodeForOrder() - " + errmsg);
                        }
                    }
                }
                string commentDescription = "";
                if (order.name != null)
                    if (order.name != "")
                        commentDescription = order.name;
                string commentNote1 = "";
                if (order.smartdayOrder.crmInfo1 != null)
                    if (order.smartdayOrder.crmInfo1 != "")
                        commentNote1 = order.smartdayOrder.crmInfo1;
                string commentNote2 = "";
                if (order.smartdayOrder.crmInfo2 != null)
                    if (order.smartdayOrder.crmInfo2 != "")
                        commentNote2 = order.smartdayOrder.crmInfo2;
                string memopath201 = "";
                string memopath202 = "";
                string memopath203 = "";
                db.GetOrderNoteMemoPaths(vismaOrdNo, ref memopath201, ref memopath202, ref memopath203, out errmsg);

                if (commentDescription != "")
                    db.UpdateFreeInfMemo(vismaOrdNo, memopath201, 201, commentDescription, out errmsg);
                if (commentNote2 != "")
                    db.UpdateFreeInfMemo(vismaOrdNo, memopath202, 202, commentNote2, out errmsg);
                if (commentNote1 != "")
                    db.UpdateFreeInfMemo(vismaOrdNo, memopath203, 203, commentNote1, out errmsg); // Correct!


                // docs/images..!
   

                List<Models.Document> documents =  httpClient.GetOrderDocumentsAsync(order.id).Result;
                if (documents != null)
                {
                    foreach(Document document in documents)
                    {
                        string fileNameInVisma = "";
                        int PK = 0;
                        if (db.GetDocumentFromSmartDayId(vismaOrdNo, "", document.id, ref fileNameInVisma, ref PK, out errmsg) == false)
                            Utils.WriteLog($"ERROR: b.GetDocumentFromSmartDayId() - {errmsg}");
                   
                        if (PK == 0)
                        {
                            // NEW Image/doc
                            // download image/doc
                            string data = httpClient.GetOrderDocumentDataAsync(order.id, document.id).Result;
                            if (data != "")
                            {
                                PK = 0;
                                if (db.GetNextFreeInfPK(ref PK, out errmsg) == false)
                                    Utils.WriteLog($"ERROR: b.GetNextFreeInfPK() - {errmsg}");
                                if (PK > 0)
                                {
                                    string imageFileName = Utils.ReadConfigString("DocFolder", "") + $"\\{PK}-{Path.GetFileName(document.name)}"; 
                                    Utils.WriteFileFromBase64(data, imageFileName);
                                    // register image in Visma FreeInf1..
                                
                                    db.InsertFreeImage(PK, vismaOrdNo, document.comment != null ? document.comment : Path.GetFileName(document.name), imageFileName, out errmsg);
                                }
                            }
                        }
                    }
                }

                UpdateSmartDayOrderStatus(order.id, Models.OrderStatus.Approved); //99Models.OrderStatus.Balanced
                if (thingRef != "")
                    db.UpdateOrderStatus(vismaOrdNo, thingRef, 29, out errmsg);

                Models.Project project = httpClient.GetProjectAsync(order.smartdayOrder.projectReference.id).Result;

                if (project == null)
                    Utils.WriteLog($"Unable to find project {order.smartdayOrder.projectReference.id} for order id {order.id}");
                else
                {
                    Utils.WriteLog($"Project {order.smartdayOrder.projectReference.id} has status {project.status}");
                    int vismaProjectRno = 0;
                    if (db.GetProjectFromSmartDayProjectID(order.smartdayOrder.projectReference.id, ref vismaProjectRno, out  errmsg) == false)
                        Utils.WriteLog("ERROR: GetProjectFromSmartDayProjectID() - " + errmsg);
                    if (vismaProjectRno == 0)
                        Utils.WriteLog($"WARNING: Unable to recognize Smartday project id {order.smartdayOrder.projectReference.id} in Visma.. cannot set status");

                    if (project.status == Models.ProjectStatus.Closed && vismaProjectRno > 0)
                        db.UpdateProjectStatus(vismaProjectRno, 29, out errmsg);

                }
            }

            sync.LastestSync = DateTime.Now;
            Utils.WriteSyncTime(sync, Utils.ReadConfigString("OrderChangeSyncFile", "OrderChangeSyncFile.xml"));

            return true;
        }

        private static bool UpdateSmartDayOrderStatus(string smartDayID, int newStatus)
        {
            SmartDayHttpClient httpClient = new SmartDayHttpClient();

            Utils.WriteLog($"Updating Order status in Smartday - order id {smartDayID} to status {newStatus}");
            Models.Order order = httpClient.GetOrderAsync(smartDayID).Result;
            if (order == null)
                return false;
            order.smartdayOrder.status = newStatus;

            if (order.location != null)
                if (order.location.id == "")
                    order.location = null;

            if (order.smartdayOrder.thingReference != null)
                if (order.smartdayOrder.thingReference.id == "")
                    order.smartdayOrder.thingReference = null;

            List<Models.Order> orders = new List<Models.Order>();
            orders.Add(order);
            List<Models.Result> results = httpClient.CreateOrdersAsync(orders).Result;
            if (results != null)
            {
                if (results.Count > 0)
                {
                    if (results[0].hasError == false)
                    {
                        Utils.WriteLog($"Order {order.id} / {order.externalId} updated to status {newStatus}");
                        return true;
                    }
                    else
                        Utils.WriteLog($"Error updating order   - {results[0].errorMessage} ");
                }
            }
            return false;
        }

        // OK
        private static bool UpdateSmartDayProjectStatus(string smartDayID, int newStatus)
        {
            SmartDayHttpClient httpClient = new SmartDayHttpClient();


            Models.Project project = httpClient.GetProjectAsync(smartDayID).Result;
            if (project == null)
                return false;
            project.status = newStatus;


            if (project.thingReference != null)
                if (project.thingReference.id == "")
                    project.thingReference = null;

            if (project.siteReference != null)
                if (project.siteReference.id == "")
                    project.siteReference = null;




            List<Models.Project> projects = new List<Models.Project>();
            projects.Add(project);
            List<Models.Result> results = httpClient.CreateProjectsAsync(projects).Result;
            if (results != null)
            {
                if (results.Count > 0)
                {
                    if (results[0].hasError == false)
                    {
                        Utils.WriteLog($"Project {project.id} / {project.externalId} updated to status {newStatus}");
                        return true;
                    }
                    else
                        Utils.WriteLog($"Error updating project   - {results[0].errorMessage} ");
                }
            }

            return false;
        }

        //
        /*private static bool SyncCustomersAndSites(List<Models.VismaCustomer> vcustomers)
        {

            foreach (Models.VismaCustomer vcustomer in vcustomers)
            {

                string customerSmartDayId = "";
                string siteSmartDayId = "";
                if (SyncCustomerAndSite(vcustomer, ref customerSmartDayId, ref siteSmartDayId, false) == false)
                    return false;
            }
            return true;
        }*/

        // Returns Smartday SiteID
        private static bool SyncCustomerAndSite(Models.VismaCustomer vcustomer, Models.VismaSite vsite, ref string customerSmartDayId, ref string siteSmartDayId)
        {
            Utils.WriteLog("INFO: SyncCustomerAndSite()..");
            customerSmartDayId = "";
            siteSmartDayId = "";

            SmartDayHttpClient httpClient = new SmartDayHttpClient();
            DBaccess db = new DBaccess();

            bool updateCustomer = true;
            Models.Customer customer = null;
            string smartDayID = vcustomer.SmartDayId != "" ? vcustomer.SmartDayId : vcustomer.CustomerNo.ToString();
            if (smartDayID != "")
                customer = httpClient.GetCustomerAsync(smartDayID).Result;

            if (customer == null)
            {
                // Map to SmartDay customer
                customer = new Models.Customer()
                {
                    externalId = vcustomer.CustomerNo.ToString(),
                    id = null
                };

                updateCustomer = false;
            }

            customer.name = vcustomer.Name;
            customer.isCompany = vcustomer.CompanyNo != "";
            customer.vatNumber = vcustomer.CompanyNo;
            customer.cellPhoneNumber = vcustomer.Mobile != "" ? vcustomer.Mobile : null;
            customer.phoneNumber = vcustomer.Phone != "" ? vcustomer.Phone : null;
            customer.email = vcustomer.EmailAddress != "" ? vcustomer.EmailAddress : null;

            customer.comment = vcustomer.Memo != "" ? vcustomer.Memo : null;

            customer.smartdayCustomer.state = 0;// vcustomer.Group3 == 1 ? 0 : 1;
            if (Utils.ReadConfigInt32("ForceActiveCustomer", 0) > 0)
                customer.smartdayCustomer.state = 0;
            customer.categoryReference.id = customer.isCompany ? Models.CategoryType.Customer_Erhverv.ToString() : Models.CategoryType.Customer_Privat.ToString();
            customer.categoryReference.externalId = customer.categoryReference.id;
            customer.categoryReference.name = null;
            customer.billingAddress.postalCode = vcustomer.PostCode;
            customer.billingAddress.postalArea = vcustomer.PostalArea;

            customer.handymanCustomer = null;

            string street = "";
            string houseNumber = "";
            string addressLine = vcustomer.AddressLine1 != "" ? vcustomer.AddressLine1 : vcustomer.AddressLine2;

            Utils.IsolateStreetNumber(addressLine, ref street, ref houseNumber);
            customer.billingAddress.address1 = street;
            customer.billingAddress.housenumber = houseNumber;

            customer.billingAddress.address2 = null;// vcustomer.AddressLine2;
            customer.billingAddress.country = vcustomer.CountryCode;
            customer.billingAddress.handymanAddress = null;
            customer.billingAddress.denmarkAddress.globalLocationNumber = null;
            customer.billingAddress.norwayAddress = null;
            if (updateCustomer == false)
            {
                customer.billingAddress.id = null;
                customer.billingAddress.externalId = vcustomer.ActorNo.ToString();
            }

            if (updateCustomer)
                customer.contacts.Clear();
            foreach (Models.VismaActor actor in vcustomer.ContactList)
            {
                customer.contacts.Add(new Models.Contact()
                {
                    name = actor.Name,
                    cellPhoneNumber = actor.Mobile,
                    email = actor.EmailAddress,
                    phoneNumber = actor.Phone,
                    handymanContact = null,
                    id = null,
                    externalId = null
                });
            }

            // Create one at a time..
            List<Models.Customer> customers = new List<Models.Customer>();
            customers.Add(customer);
            List<Models.Result> results = httpClient.CreateCustomersAsync(customers).Result;
            if (results != null)
            {
                if (results.Count > 0)
                {
                    if (results[0].hasError == false)
                    {
                        customer.id = results[0].entity.id;
                        Utils.WriteLog($"Customer created - {customer.id} ");
                        db.RegisterCustomerSmartdayID(vcustomer.ActorNo, customer.id, out string errmsg);
                        customerSmartDayId = customer.id;
                    }
                    else
                        Utils.WriteLog($"Error creating customer  - {results[0].errorMessage} ");
                }
            }

            // Now create/update the site(s)

            if (customer.id != null && vsite != null)
            {
               SyncSites(vsite, customer, vcustomer.ActorNo, ref siteSmartDayId);
            }

            return customerSmartDayId != "";
        }

  
        // Returns Smartday SiteID
        private static bool SyncSites(Models.VismaSite vsite, Models.Customer customer, int actNo, ref string siteSmartDayId)
        {
            siteSmartDayId = "";
            bool errorsDuringUpload = false;
            SmartDayHttpClient httpClient = new SmartDayHttpClient();

            DBaccess db = new DBaccess();
            bool updateSite = true;

            if (customer.id == "")
            {
                Utils.WriteLog($"ERROR: Customer must be defined before site");
                return false;
            }

            Models.Site site = null;            
            if (vsite.SmartDayId != "")
                site = httpClient.GetSiteAsync(vsite.SmartDayId).Result;

            // Site not yet registered (in Visma..)
            // search existing sites in Smartday for matching address..
            /*   if (site == null)
               {
                     List<Models.Site> existingSites = httpClient.GetSitesAsync(customer.id).Result;
                   if (existingSites != null)
                   {
                       foreach (Models.Site existingSite in existingSites)
                       {
                           if (existingSite.name == vaddress.Name)
                           {
                               if (existingSite.postalAddress != null)
                               {
                                   if (vaddress.PostCode == existingSite.postalAddress.postalCode &&
                                       vaddress.PostalArea == existingSite.postalAddress.postalArea &&
                                       vaddress.AddressLine1.IndexOf(existingSite.postalAddress.address1) != -1 &&
                                       vaddress.AddressLine1.IndexOf(existingSite.postalAddress.housenumber) != -1)
                                   {
                                       // found existing match!
                                       site = existingSite;
                                       break;
                                   }
                               }
                           }
                       }
                   }
               }*/

            // new/unknow site - create..
            if (site == null)
            {
                site = new Models.Site()
                {
                    externalId = customer.id,
                    id = null
                };
                updateSite = false;
            }

            site.name = vsite.Name;
            site.picture = null;
            site.serialnumber = null;
            site.comment =  null;
            site.state = "0"; //????
            site.handymanSite = null;

            site.customerReference.externalId = customer.externalId;
            site.customerReference.id = customer.id;

            site.categoryReference.id = customer.categoryReference.id;
            site.categoryReference.externalId = customer.categoryReference.externalId;
            site.categoryReference.name = customer.categoryReference.name;

            if (updateSite == false)
            {
                site.postalAddress.id = null;
                site.postalAddress.externalId = null;// vaddress.ActorNo.ToString();
            }
            site.postalAddress.postalCode = vsite.PostCode;
            site.postalAddress.postalArea = vsite.PostalArea;
            site.postalAddress.handymanAddress = null;
            site.postalAddress.denmarkAddress.globalLocationNumber = null;
            site.postalAddress.norwayAddress.boligmappaEdokNumber = null;
            site.postalAddress.norwayAddress.boligmappaPlantID = 0;

            string street = "";
            string houseNumber = "";
            string addressLine = vsite.AddressLine1 != "" ? vsite.AddressLine1 : vsite.AddressLine2;
            Utils.IsolateStreetNumber(addressLine, ref street, ref houseNumber);
            site.postalAddress.address1 = street;
            site.postalAddress.housenumber = houseNumber;

            site.postalAddress.address2 = null;//vaddress.AddressLine2;
            site.postalAddress.country = vsite.CountryCode;

            site.contacts.Clear();                
            site.contacts.Add(new Models.Contact()
            {
                name = vsite.Name,
                cellPhoneNumber = vsite.Mobile,
                email = vsite.EmailAddress,
                phoneNumber = vsite.Phone,
                handymanContact = null,
                id = null,
                externalId = customer.id
            });
                
            List<Models.Site> sites = new List<Models.Site>(); // final list
            sites.Add(site);

            List<Models.Result> results = httpClient.CreateSitesAsync(sites).Result;
            if (results != null)
            {
                if (results.Count > 0)
                {

                    if (results[0].hasError == false)
                    {
                        site.id = results[0].entity.id;
                        site.externalId = results[0].entity.externalId;
                        siteSmartDayId = site.id;
                        Utils.WriteLog($"Site created/updated - {site.id} - {site.externalId}");
                        db.RegisterSiteSmartdayID(actNo, site.id, out string errmsg);
                    }
                    else
                    {
                        Utils.WriteLog($"Error creating/updating site  - {results[0].errorMessage} ");
                        errorsDuringUpload = true;
                    }
                }
            }

            return errorsDuringUpload ? false : true;

        }

        /*        private static bool SyncSiteForProject(Models.VismaProject vproject, Models.VismaCustomer vcustomer, Models.Customer customer, int specificAddressActNo, ref string siteSmartDayId)
                {
                    siteSmartDayId = "";
                    bool errorsDuringUpload = false;
                    SmartDayHttpClient httpClient = new SmartDayHttpClient();

                    DBaccess db = new DBaccess();
                    bool updateSite = true;

                    if (customer.id == "")
                    {
                        Utils.WriteLog($"ERROR: Customer must be defined before site");
                        return false;
                    }

                    if (vcustomer.AddressList.Count != 1)
                    {
                        Utils.WriteLog($"ERROR: No address(es) defined for customer {vcustomer.CountryNumber} - unable to add/update site(s)");
                        return false;
                    }

                    //   List<Models.Site> existingSites = httpClient.GetSitesAsync(customer.id).Result;



                    foreach (Models.VismaActor vaddress in vcustomer.AddressList)
                    {
                        Models.Site site = null;

                        if (specificAddressActNo > 0 && vaddress.ActorNo != specificAddressActNo)
                            continue;

                        if (vaddress.Inf7 != "")
                            site = httpClient.GetSiteAsync(vaddress.Inf7).Result;


                        // new/unknow site - create..
                        if (site == null)
                        {
                            site = new Models.Site()
                            {
                                externalId = customer.id,
                                id = null
                            };
                            updateSite = false;
                        }

                        site.name = vaddress.Name;
                        site.picture = null;
                        site.serialnumber = null;
                        site.comment = vaddress.Memo != "" ? vaddress.Memo : null;
                        site.state = "0"; //????
                        site.handymanSite = null;

                        site.customerReference.externalId = customer.externalId;
                        site.customerReference.id = customer.id;

                        site.categoryReference.id = customer.categoryReference.id;
                        site.categoryReference.externalId = customer.categoryReference.externalId;
                        site.categoryReference.name = customer.categoryReference.name;

                        if (updateSite == false)
                        {
                            site.postalAddress.id = null;
                            site.postalAddress.externalId = null;// vaddress.ActorNo.ToString();
                        }
                        site.postalAddress.postalCode = vaddress.PostCode;
                        site.postalAddress.postalArea = vaddress.PostalArea;
                        site.postalAddress.handymanAddress = null;
                        site.postalAddress.denmarkAddress.globalLocationNumber = null;
                        site.postalAddress.norwayAddress.boligmappaEdokNumber = null;
                        site.postalAddress.norwayAddress.boligmappaPlantID = 0;

                        string street = "";
                        string houseNumber = "";
                        string addressLine = vaddress.AddressLine1 != "" ? vaddress.AddressLine1 : vaddress.AddressLine2;
                        Utils.IsolateStreetNumber(addressLine, ref street, ref houseNumber);
                        site.postalAddress.address1 = street;
                        site.postalAddress.housenumber = houseNumber;

                        site.postalAddress.address2 = null;//vaddress.AddressLine2;
                        site.postalAddress.country = vaddress.CountryCode;


                        site.contacts.Clear();
                        foreach (Models.VismaActor actor in vcustomer.ContactList)
                        {
                            site.contacts.Add(new Models.Contact()
                            {
                                name = actor.Name,
                                cellPhoneNumber = actor.Mobile,
                                email = actor.EmailAddress,
                                phoneNumber = actor.Phone,
                                handymanContact = null,
                                id = null,
                                externalId = actor.ActorNo.ToString()
                            });
                        }
                        List<Models.Site> sites = new List<Models.Site>(); // final list
                        sites.Add(site);

                        List<Models.Result> results = httpClient.CreateSitesAsync(sites).Result;
                        if (results != null)
                        {
                            if (results.Count > 0)
                            {

                                if (results[0].hasError == false)
                                {
                                    site.id = results[0].entity.id;
                                    site.externalId = results[0].entity.externalId;
                                    siteSmartDayId = site.id;
                                    Utils.WriteLog($"Site created/updated - {site.id} - {site.externalId}");
                                    db.RegisterSiteSmartdayID(vaddress.ActorNo, site.id, out string errmsg);
                                }
                                else
                                {
                                    Utils.WriteLog($"Error creating/updating site  - {results[0].errorMessage} ");
                                    errorsDuringUpload = true;
                                }



                            }
                        }

                    }

                    return errorsDuringUpload ? false : true;

                } */

        private static bool SyncSiteForThing(Models.VismaThing vthing, string customerSmartDayId, ref string smartdaySiteID)
        {
            if (smartdaySiteID == "")
                smartdaySiteID = vthing.SmartDaySiteId;

            bool errorsDuringUpload = false;
            SmartDayHttpClient httpClient = new SmartDayHttpClient();

            Models.Customer customer = httpClient.GetCustomerAsync(customerSmartDayId).Result;
            if (customer == null)
            {
                Utils.WriteLog($"ERROR: SyncSiteForThing() - unable to get customer with smartdayid {customerSmartDayId}");
                return false;
            }
            DBaccess db = new DBaccess();
            bool updateSite = true;

            Models.Site site = null; 

            if (vthing.Ad1 == "")
            {
                Utils.WriteLog($"ERROR: SyncSiteForThing() No addressdefined for thing {vthing.RNo} - unable to add/update site(s)");
                return false;
            }

            if (smartdaySiteID != "")
            {
                site = httpClient.GetSiteAsync(smartdaySiteID).Result;
            }


            // new/unknow site - create..
            if (site == null)
            {
                site = new Models.Site()
                {
                    externalId = vthing.RNo,
                    id = null
                };
                updateSite = false;
            }

            site.name = vthing.Ad1;
            site.picture = null;
            site.serialnumber = null;
            site.comment =  null;
            site.state = "0"; //????
            site.handymanSite = null;

            site.customerReference.externalId = customer.externalId;
            site.customerReference.id = customer.id;

            site.categoryReference.id = customer.categoryReference.id;
            site.categoryReference.externalId = customer.categoryReference.externalId;
            site.categoryReference.name = customer.categoryReference.name;

            if (updateSite == false)
            {
                site.postalAddress.id = null;
                site.postalAddress.externalId = null;// vaddress.ActorNo.ToString();
            }
            site.postalAddress.postalCode = vthing.PNo;
            site.postalAddress.postalArea = vthing.PArea;
            site.postalAddress.handymanAddress = null;
            site.postalAddress.denmarkAddress.globalLocationNumber = null;
            site.postalAddress.norwayAddress.boligmappaEdokNumber = null;
            site.postalAddress.norwayAddress.boligmappaPlantID = 0;

            string street = "";
            string houseNumber = "";
            string addressLine = vthing.Ad2 != "" ? vthing.Ad2 : vthing.Ad1;
            Utils.IsolateStreetNumber(addressLine, ref street, ref houseNumber);
            site.postalAddress.address1 = street;
            site.postalAddress.housenumber = houseNumber;

            site.postalAddress.address2 = null;//vaddress.AddressLine2;
            site.postalAddress.country = vthing.Country;


            List<Models.Site> sites = new List<Models.Site>(); // final list
            sites.Add(site);

            List<Models.Result> results = httpClient.CreateSitesAsync(sites).Result;
            if (results != null)
            {
                if (results.Count > 0)
                {

                    if (results[0].hasError == false)
                    {
                        site.id = results[0].entity.id;
                        site.externalId = results[0].entity.externalId;
                        Utils.WriteLog($"Site created/updated - {site.id} - {site.externalId}");
                        db.RegisterSiteSmartdayIDToThing(vthing.RNo, site.id, out string errmsg);
                    }
                    else
                    {
                        Utils.WriteLog($"Error creating/updating site  - {results[0].errorMessage} ");
                        errorsDuringUpload = true;
                    }



                }
            }



            return errorsDuringUpload ? false : true;

        }


        private static bool SyncThing(Models.VismaThing vthing,
            string thingCustomerId, string thingCustomerExternalId, string thingSiteId, string thingSiteExternalId)
        {
            SmartDayHttpClient httpClient = new SmartDayHttpClient();
            DBaccess db = new DBaccess();

            // Convert visma thing to Smartday thing

           
            Models.Thing thing = null;

            if (vthing.SmartDayId != "")
                thing = httpClient.GetThingAsync(vthing.SmartDayId).Result;

            if (thing == null)
            {
                thing = new Models.Thing()
                {
                    id =  null,
                    externalId = vthing.RNo,

                    created = null,
                    handymanThing = null,
                    lastRepairDate = null,
                    lastServiceDate = null,
                    manufacturingDate = null,
                };
            }

            thing.model = vthing.R8Nm != "" ? vthing.R8Nm : null;
            thing.make = "";// vthing.RNo;
            thing.name = vthing.Name;
            thing.note = vthing.Memo != "" ? vthing.Memo : null;


            thing.installedDate = vthing.Dt3 > 0 ? Utils.DateTime2String(0, Utils.VismaDate2DateTime(vthing.Dt3)) + "Z" : null;
            thing.warrantyStartDate = vthing.Dt4 > 0 ? Utils.DateTime2String(0, Utils.VismaDate2DateTime(vthing.Dt4)) + "Z" : null;
            thing.warrantyEndDate = vthing.Dt1 > 0 ? Utils.DateTime2String(0, Utils.VismaDate2DateTime(vthing.Dt1)) + "Z" : null;
            thing.state = "Active";//vthing.Gr3 == 1 ? "Active" : "Disabled";
            thing.placement = vthing.Inf5;
            thing.serialNumber = vthing.Inf;

            thing.categoryReference.id = Models.CategoryType.Things_Serviceenhed.ToString();
            thing.categoryReference.externalId = Models.CategoryType.Things_Serviceenhed.ToString();
            thing.categoryReference.name = null;

            thing.userReference.externalId = null;
            thing.userReference.id = null;



            thing.customerReference.id = thingCustomerId != "" ? thingCustomerId : null;
            thing.customerReference.externalId = thingCustomerExternalId;

            thing.lastKnownLocation.externalId = thing.externalId;
            thing.lastKnownLocation.id = thing.id;
            thing.lastKnownLocation.address1 = null;
            thing.lastKnownLocation.address2 = null;
            thing.lastKnownLocation.housenumber = null;
            thing.lastKnownLocation.postalCode = null;
            thing.lastKnownLocation.postalArea = null;
            thing.lastKnownLocation.country = null;

            thing.contacts.Add(new Models.ThingContact()
            {
                name = vthing.Inf7 != "" ? vthing.Inf7 : null,
                cellPhoneNumber = vthing.Ad4 != "" ? vthing.Ad4 : null,
                phoneNumber = vthing.Phone != "" ? vthing.Phone : null,
                email = null,
                handymanContact = null,

            });
            thing.smartdayThing.contractType = vthing.R3 != "" ? vthing.R3 + " - " + vthing.R3R9 : null;
            thing.smartdayThing.serviceObjectUsage = null;
            thing.smartdayThing.addressLastUpdated = null;

                // Resolve SITE reference

            thing.parentReference.id = thingSiteId != "" ? thingSiteId : null;
            thing.parentReference.externalId = thingSiteExternalId;

            if (string.IsNullOrEmpty(thing.externalId))
            {
                Utils.WriteLog("ERROR: R12.Rno cannot be empty");
                return false;
            }

            if (string.IsNullOrEmpty(thing.parentReference.id))
            {
                Utils.WriteLog("ERROR: Site not set for thing");
                return false; 
            }

            if (string.IsNullOrEmpty(thing.customerReference.externalId))
            {
                Utils.WriteLog("ERROR: R12.CustNo cannot be 0");
                return false;
            }

            // now uplaod thing

            List<Models.Thing> thingsToSend = new List<Models.Thing>();
            thingsToSend.Add(thing);
            List<Models.Result> results = httpClient.CreateThingsAsync(thingsToSend).Result;

            if (results != null)
            {
                if (results.Count > 0)
                {
                    if (results[0].hasError == false)
                    {
                        thing.id = results[0].entity.id;
                        vthing.SmartDayId = thing.id;
                        Utils.WriteLog($"Thing created/updated - {thing.id} - {thing.externalId}");
                        if (db.RegisterThingSmartdayID(vthing.RNo, thing.id, thing.parentReference.id, out string errmsg) == false)
                            Utils.WriteLog($"Error db.RegisterThingSmartdayID()  - {errmsg} ");

                    }
                    else
                        Utils.WriteLog($"Error creating/updating project  - {results[0].errorMessage} ");
                }
            }
        

            return true;
        }

        private static string SyncProject(Models.VismaProject vproject, Models.VismaCustomer vcustomer,  int ordNo, string ownerUserId, bool createOnly)
        {
            SmartDayHttpClient httpClient = new SmartDayHttpClient();
            DBaccess db = new DBaccess();

            Utils.WriteLog("INFO: SyncProject()..");

            Models.Project project = null;
            if (vproject.SmartDayProjectID != "")
                project = httpClient.GetProjectAsync(vproject.SmartDayProjectID).Result;

            if (createOnly && project != null)
            {
                Utils.WriteLog($"INFO: SyncProject() - project with id {vproject.SmartDayProjectID} already exists - skipping update.");
                return project.id;
            }

            if (project == null)
            {
                project = new Models.Project()
                {
                    name = vproject.Name,
                    
                    id = null,
                    description = "",
                    price = 0.0,
                    fixedPrice = false,
                    thingReference = null,
                    offerOrderReference = null,
                    createdDate = vproject.CreateDate > 0 ? Utils.DateTime2String(1, Utils.VismaDate2DateTime(vproject.CreateDate)) + "Z" : null,
                    status = Models.ProjectStatus.Activated,
                    externalId = ordNo.ToString()
            };
            }

           //vproject.ProjectRno.ToString();

            project.startDate = vproject.EstimatedStartDt > 0 ? Utils.DateTime2String(1, Utils.VismaDate2DateTime(vproject.EstimatedStartDt)) + "Z" : null;
            project.endDate = vproject.EstimatedEndDt > 0 ? Utils.DateTime2String(1, Utils.VismaDate2DateTime(vproject.EstimatedEndDt)) + "Z" : null;
            project.deadlineDate = vproject.DeadlineDt > 0 ? Utils.DateTime2String(1, Utils.VismaDate2DateTime(vproject.DeadlineDt)) + "Z" : null;

            project.categoryReference.id = Models.CategoryType.Project_Projekt.ToString();
            project.categoryReference.externalId = Models.CategoryType.Project_Projekt.ToString();
   

            // Add service unit if referenced in order
            project.thingReference = null;
       
            if (vproject.ServiceUnitRno != "" && Utils.ReadConfigInt32("IgnoreProjectThing",0) == 0)
            {
                string thingSmartdayID = "";
                string thingSmartdaySiteID = "";
                if (db.GetThingSmartdayID(vproject.ServiceUnitRno, ref thingSmartdayID, ref thingSmartdaySiteID, out string errmsg2) == false)
                    Utils.WriteLog("ERROR: db.GetThingSmartdayID() - " + errmsg2);
                if (thingSmartdayID != "")
                {
                    // Check if in SmartDay..
                    Models.Thing existingThing = httpClient.GetThingAsync(thingSmartdayID).Result;
                    if (existingThing != null)
                    {
                        project.thingReference.id = existingThing.id;
                        project.thingReference.externalId = existingThing.externalId;

                        if (existingThing.id != null)
                            if (existingThing.parentReference != null)
                                project.siteReference.id = existingThing.parentReference.id;

                        if (existingThing.externalId != null)
                            if (existingThing.parentReference != null)
                                project.siteReference.externalId = existingThing.parentReference.externalId;
                    }
                }
            }

            project.customerReference.id = vcustomer.SmartDayId != "" ? vcustomer.SmartDayId : null;// vproject.CustomerNo.ToString();
            project.customerReference.externalId = vproject.CustomerNo.ToString();


            // Now settle site!
            Models.Site site = null;

            if (vcustomer.SmartDaySiteId != "")
                site = httpClient.GetSiteAsync(vcustomer.SmartDaySiteId).Result;
          

            if (site == null)
                Utils.WriteLog($"WARNING: Unable to find matching site for project {vproject.ProjectRno} {vproject.Address1} {vproject.PostCode} {vproject.PostalArea}");

            project.siteReference.id = site?.id;       // Site for this custno
            project.siteReference.externalId = site?.externalId;

            project.ownerReference.id = ownerUserId;
            project.ownerReference.externalId = project.ownerReference.id;

            // safeguards..

            if (project.thingReference != null)
                if (project.thingReference.id == "")
                    project.thingReference = null;

            if (project.siteReference != null)
                if (project.siteReference.id == "")
                    project.siteReference = null;


            List<Models.Project> projects = new List<Models.Project>();
            projects.Add(project);

            // Go post project
            List<Models.Result> results = httpClient.CreateProjectsAsync(projects).Result;
            if (results != null)
            {
                if (results.Count > 0)
                {
                    if (results[0].hasError == false)
                    {
                        project.id = results[0].entity.id;
                        project.externalId = results[0].entity.externalId;
                        vproject.SmartDayProjectID = project.id;
                        vproject.SmartDaySiteID = site != null ? site.id : "";
                        Utils.WriteLog($"Project created/updated - {project.id} - {project.externalId}");
                        //db.RegisterProjectSmartdayID(vproject.ProjectRno, project.id, out errmsg);
                        db.RegisterProjectSmartdayID(ordNo, project.id, out string errmsg);
                        return project.id;
                    }
                    else
                        Utils.WriteLog($"Error creating/updating project  - {results[0].errorMessage} ");
                }
                else
                    Utils.WriteLog($"No response back from creating/updating project ");
            }
            return "";

        }


        // onme smartday orde per thing..
        private static bool SyncOrder(Models.VismaOrder vorder, 
            string smartdayProjectID, string smartdayProjectExternalID,
            string smartdayCustomerID, string smartdayCustomerExternalID,
            string smartdaySiteID, string smartdaySiteExternalID,
            string smartdayThingID, string smartdayThingExternalID,
            string ownerUserId, Models.VismaOrderThing thingR12)
        {
            SmartDayHttpClient httpClient = new SmartDayHttpClient();
            DBaccess db = new DBaccess();
            Models.VismaProject vproject = new Models.VismaProject();
            
            string errmsg;

         
            // Check if order already created
            string smartDayOrderId = thingR12.SmartDayThingId != "" ? thingR12.SmartDayThingId : vorder.OrderNo + "-" + thingR12.RNo;

            Models.Order order = httpClient.GetOrderAsync(smartDayOrderId).Result;
            if (order == null)
            {               
                // New order - set id=null to indicate create
                order = new Models.Order()
                {
                    id = null,
                    externalId = vorder.OrderNo + "-" + thingR12.RNo,
                    name = vorder.FreeInf1Memo1 != "" ? vorder.FreeInf1Memo1 : vorder.Name,
                    createdDate = vorder.OrderDate > 0 ? Utils.DateTime2String(1, Utils.VismaDate2DateTime(vorder.OrderDate)) + "Z" : null,
                    startTime = Utils.DateTime2String(0, Utils.VismaDate2DateTime(vorder.OrderDate)) + "Z",
                    endTime = Utils.DateTime2String(0, Utils.VismaDate2DateTime(vorder.OrderDate)) + "Z",
                    handymanOrder = null
                };                

                order.categoryReferences.Clear();
                order.categoryReferences.Add(new Models.CategoryReferenceOrder()
                {
                    externalId = Utils.MapOrderCategory(vorder.CategoryName).ToString(), // Ord.R7
                    id = Utils.MapOrderCategory(vorder.CategoryName).ToString()
                });

                order.smartdayOrder.urgent = false;
                order.smartdayOrder.timeslotAgreed = false;
                order.smartdayOrder.thingReference = null;
                order.smartdayOrder.status = Models.OrderStatus.Pending;
            }

            order.poNumber = vorder.Reqno; //vorder.Reqno + " [" + vorder.OrderNo + "]";

            // Link project
            order.smartdayOrder.projectReference.externalId = smartdayProjectExternalID;
            order.smartdayOrder.projectReference.id = smartdayProjectID;
            order.smartdayOrder.crmInfo1 = vorder.FreeInf1Memo3;
            order.smartdayOrder.crmInfo2 = vorder.FreeInf1Memo2; // ??
            order.smartdayOrder.owner = ownerUserId != "" ? ownerUserId : Utils.ReadConfigString("DefaultOwner", "1");

            // Department

            order.departmentReference.id = vorder.Gr8 > 0 ? vorder.Gr8.ToString() : Utils.ReadConfigString("DefaultDepartment", "1");
            order.departmentReference.externalId = vorder.Gr8 > 0 ? vorder.Gr8.ToString() : Utils.ReadConfigString("DefaultDepartment", "1");

            // Location

            string street = "";
            string houseNumber = "";
            string address = vorder.AddressLine1.Trim() != "" ? vorder.AddressLine1.Trim() : vorder.AddressLine2;
            Utils.IsolateStreetNumber(address, ref street, ref houseNumber);

            if (order.location == null) // null received from existing order?
                order.location = new Models.Location();

            order.location.address1 = street;
            order.location.housenumber = houseNumber;
            order.location.postalArea = vorder.PostalArea;
            order.location.postalCode = vorder.PostCode;
            order.location.address2 = null;

            // Responsible

            string responsible = "";
            if (db.GetEmployerName(vorder.EmpNo, ref responsible, out  errmsg) == false)
            {
                Utils.WriteLog("ERROR: db.GetEmployerName() - " + errmsg);
                return false;
            }

            if (order.responsibleReference == null)
                order.responsibleReference = new Models.Reference();
            order.responsibleReference.externalId = responsible.Trim() != "" ? responsible.Trim() : Utils.ReadConfigString("DefaultResponsible", "Visma");
            order.responsibleReference.id = responsible != "" ? responsible : Utils.ReadConfigString("DefaultResponsible", "Visma");

            // Participants

            if (order.id == null)
            {
                if (vorder.EmpNo > 0)
                {
                    // only set if new order
                    order.participants.Clear();
                    order.participants.Add(new Models.Participant()
                    {
                        handymanParticipant = null,
                        userReference = new Models.Reference()
                        {
                            id = vorder.EmpNo > 0 ? vorder.EmpNo.ToString() : Utils.ReadConfigString("DefaultParticipant", "1"),
                            externalId = vorder.EmpNo > 0 ? vorder.EmpNo.ToString() : Utils.ReadConfigString("DefaultParticipant", "1")
                        }
                    });
                }
                else
                    order.participants = null;

            }

            // Thing

            if (smartdayThingID != "")
            {
                if (order.smartdayOrder.thingReference == null)
                    order.smartdayOrder.thingReference = new Reference();
                order.smartdayOrder.thingReference.id = smartdayThingID;
                order.smartdayOrder.thingReference.externalId = smartdayThingExternalID;
            }
            if (order.smartdayOrder.thingReference != null)
                if (order.smartdayOrder.thingReference.id == "")
                    order.smartdayOrder.thingReference = null;

            // Customer

            if (order.customerReference == null)
                order.customerReference = new Reference();
            order.customerReference.id = smartdayCustomerID;
            order.customerReference.externalId = smartdayCustomerExternalID;

            // Site

            if (order.siteReference == null)
                order.siteReference = new Reference();
            order.siteReference.id = smartdaySiteID;
            order.siteReference.externalId = smartdaySiteExternalID;

        
                
            List<Models.Order> orders = new List<Models.Order>();
            orders.Add(order);

            // Send order..!
            List<Models.Result> results = httpClient.CreateOrdersAsync(orders).Result;
            bool orderOK = false;
            if (results != null)
            {
                if (results.Count > 0)
                {
                    if (results[0].hasError == false)
                    {
                        order.id = results[0].entity.id;
                        order.externalId = results[0].entity.externalId;
                        Utils.WriteLog($"Order created/updated - {order.id} - {order.externalId}");
                        db.RegisterOrderSmartdayID(vorder.OrderNo, thingR12.RNo,  order.id, out errmsg);
                        orderOK = true;
                    }
                    else
                    {
                        Utils.WriteLog($"Error creating/updating order  - {results[0].errorMessage} ");
                    }
                }
                else
                    Utils.WriteLog($"No response back from  creating/updating order ");
            }

           
            if (orderOK && thingR12 != null)
            {
                Utils.WriteLog($"Number of documents/images for this order {thingR12.Documents.Count} ");
                foreach (VismaDocument doc in thingR12.Documents)
                {
                    string fileNameToUse = doc.DocFileName;
                     if (File.Exists(doc.DocFileName) == false)
                        fileNameToUse = @"c:\temp\test.pdf";        //          Upload pictures/docs
                    string docName = string.Format("{0}-{1}", doc.PK,  Path.GetFileName(doc.DocFileName)).Replace(" ","").Replace("\\", "").Replace("/", "");
                    string base64 = Utils.ReadFileToBase64(fileNameToUse);
                    if (base64 == "")
                    {
                        Utils.WriteLog($"Document/image  {fileNameToUse} not found.");
                        continue;
                    }
                    Document document = new Document() {comment = docName, name = docName, date = order.createdDate, externalId = docName, id = null, data = base64,  };
                    document.userReference = new Reference() { externalId = "1", id = "1" };
                    List<Document> documents = new List<Document>();
                    documents.Add(document);
                    List<Models.Result> resultsDoc = httpClient.CreateOrderDocumentsAsync(order.id, documents).Result;
                    if (resultsDoc != null)
                    {
                        if (resultsDoc.Count > 0)
                        {
                            if (resultsDoc[0].hasError == false)
                            {
                                string docId = results[0].entity.id;
                                string docExternalId = results[0].entity.externalId;
                                Utils.WriteLog($"Order document created/updated - {docId} - {docExternalId}");
                                db.RegisterImageSmartdayId(doc.PK, docId, out errmsg);
                       
                            }
                            else
                            {
                                Utils.WriteLog($"Error creating/updating order document - {results[0].errorMessage} ");
                            }
                        }
                        else
                            Utils.WriteLog($"No response back from  creating/updating order document");
                    }
                }
            }

            if (orderOK == false)
                return false;

            if (orderOK)
            {
                if (Utils.ReadConfigInt32("SyncMaterials", 0) > 0)
                {
                    List<Models.Material> existingMaterials = httpClient.GetMaterialsForOrder(vorder.OrderNo.ToString()).Result;

                    foreach (Models.VismaOrderLine vline in vorder.OrderLines)
                    {
                        Models.Material material = null;

                        if (existingMaterials != null)
                            material = existingMaterials.FirstOrDefault(p => p.id == vline.WebPg);
                        if (material != null)
                        {
                            // material already registered - skip
                            continue;
                        }
                        
                        material = new Models.Material
                        {
                            itemNo = vline.ProductNo,
                            itemNo2 = vline.LineNo.ToString(),
                            externalId = null,
                            id = null,
                            handymanMaterial = null,
                            date = Utils.DateTime2String(1, DateTime.Now),
                            quantity = Decimal.ToDouble(vline.Quantity),
                            totalPrice = Decimal.ToDouble(vline.Quantity * vline.PriceInCurrency),
                            description = vline.Description,
                        };
                        
                        material.storeReference.id = null;
                        material.storeReference.externalId = null;

                        material.userReference.externalId = null;
                        material.userReference.id = null;

                        material.wholesalerReference.id = Utils.ReadConfigString("DefaultWholeSalerID", "SD");
                        material.wholesalerReference.externalId = material.wholesalerReference.id;

                        material.smartdayMaterial.categoryReference.externalId = Models.CategoryType.Item_Vare.ToString();
                        material.smartdayMaterial.categoryReference.id = Models.CategoryType.Item_Vare.ToString();

                        material.smartdayMaterial.comment = "";
                        material.smartdayMaterial.unitPrice = Decimal.ToDouble(vline.PriceInCurrency);
                        
                        List<Models.Material> materials = new List<Models.Material>();
                        materials.Add(material);

                        List<Models.Result> results1 = httpClient.CreateMaterialsForOrderAsync(order.id, materials).Result;
                        if (results1 != null)
                        {
                            if (results1.Count > 0)
                            {
                                if (results1[0].hasError == false)
                                {
                                    material.id  = results1[0].entity.id;
                                    material.externalId = results1[0].entity.externalId;
                                    Utils.WriteLog($"Material created/updated - {material.id} - {material.externalId}");
                                    // register matierlID in ordln.WebPg
                                    db.RegisterMaterialSmartdayID(vorder.OrderNo, Utils.StringToInt(material.itemNo2), material.id, out errmsg);
                                    orderOK = true;
                                }
                                else
                                {
                                    Utils.WriteLog($"Error creating/updating material  - {results1[0].errorMessage} ");
                                }
                            }
                            else
                                Utils.WriteLog($"No response back from  creating/updating material ");
                        }

                    }
                }

                /*
                if (Utils.ReadConfigInt32("SyncSalaryCodes", 0) > 0)
                {
                    List<Models.SalaryCode> salaryCodes = httpClient.GetSalaryCodesForOrder(vorder.OrderNo.ToString()).Result;
                    if (salaryCodes == null)
                        salaryCodes = new List<Models.SalaryCode>();

                    foreach (Models.VismaAgreement vagreement in vorder.AgreementLines)
                    {
                        DateTime fromTime = Utils.VismaDate2DateTime(vagreement.FromDate, vagreement.FromTime);
                        DateTime toTime = Utils.VismaDate2DateTime(vagreement.ToDate, vagreement.ToTime);
                        string uniqueSalaryCodeKey = vagreement.AgreementNumber + "-" + vagreement.AgreementActNo;
                        Models.SalaryCode salaryCode = salaryCodes.FirstOrDefault(p => p.description == uniqueSalaryCodeKey);
                        if (salaryCode == null)
                        {
                            salaryCode = new Models.SalaryCode()
                            {
                                userReference = new Models.Reference() { externalId = Utils.ReadConfigString("DefaultUserReference", "1001"), id = Utils.ReadConfigString("DefaultUserReference", "1001") },
                                info = vagreement.Decription,
                                externalId = null,// vagreement.AgreementNumber.ToString(),
                                id = null,
                                handymanSalaryCodeRegistration = null
                            };
                        };
                        salaryCode.description = uniqueSalaryCodeKey;
                        salaryCode.date = Utils.DateTime2String(0, fromTime);
                        salaryCode.amount = Decimal.ToDouble(vagreement.Quantity);
                        salaryCode.totalPrice = Decimal.ToDouble(vagreement.Price) * Decimal.ToDouble(vagreement.Quantity);
                        salaryCode.status = vagreement.Status;   // Agr.Gr3 1,2???

                        salaryCode.salaryCodeReference.id = vagreement.ProdNo;  // <---- !!                   
                        salaryCode.salaryCodeReference.externalId = salaryCode.salaryCodeReference.id;

                        salaryCode.smartdaySalaryCodeRegistration.categoryReference.id = Models.CategoryType.Item_Time.ToString();
                        salaryCode.smartdaySalaryCodeRegistration.categoryReference.externalId = Models.CategoryType.Item_Time.ToString();

                        salaryCode.smartdaySalaryCodeRegistration.comment = null;
                        salaryCode.smartdaySalaryCodeRegistration.unit = "time";
                        salaryCode.smartdaySalaryCodeRegistration.unitPrice = 0.0;

                        salaryCodes.Add(salaryCode);
                    }
                    if (salaryCodes.Count > 0)
                    {
                        List<Models.Result> results1 = httpClient.CreateSalaryCodesForOrderAsync(order.id, salaryCodes).Result;
                        if (results1 != null)
                        {
                            int idx = 0;
                            foreach (Models.Result result in results1)
                            {
                                if (result.hasError == false)
                                {
                                    salaryCodes[idx].id = salaryCodes[idx].id;
                                    salaryCodes[idx].externalId = salaryCodes[idx].externalId;
                                    Utils.WriteLog($"SalaryCode created/updated - {order.id} - {salaryCodes[idx].id}");

                                    db.RegisterSalaryCodeSmartdayID(vorder.OrderNo, salaryCodes[idx].description, salaryCodes[idx].id, out errmsg);
                                }

                                idx++;
                            }
                        }
                    }
                }*/
            }
           
            return true;
        }


        private static bool SyncOrdersAndProjects(List<Models.VismaOrder> vorders)
        {
            SmartDayHttpClient httpClient = new SmartDayHttpClient();
            DBaccess db = new DBaccess();
            string errmsg = "";

            List<Models.User> users = httpClient.GetUsersAsync().Result;

            int syncedProjectRNo = 0;

            foreach (Models.VismaOrder vorder in vorders)
            {

              
                // 1 - sync project structure..
                Models.VismaProject vproject = new Models.VismaProject() { ProjectRno = 0 };
              
                if (vorder.R2 > 0)
                {
                    
                    if (db.GetProject(vorder.R2, ref vproject, out  errmsg) == false)
                    {
                        Utils.WriteLog("ERROR: db.GetProject() - " + errmsg);
                        continue;
                    }
                }
                string ownerUserId;// = vorder.EmpNo > 0 ? vorder.EmpNo.ToString() : Utils.ReadConfigInt32("DefaultOwnerID", 1).ToString(); ;

                Models.User ownerUser = users.FirstOrDefault(p => p.externalId == vorder.EmpNo.ToString() || p.username == vorder.EmpNo.ToString());
                if (ownerUser != null)
                    ownerUserId = ownerUser.id;
                else
                    ownerUserId = Utils.ReadConfigInt32("DefaultOwnerID", 1).ToString();

                string smartdayProjectID = "";

                // Only sync main project once..
                //if (vproject.ProjectRno == 0)
               // {
                    // simulate project from Visma..
                vproject.ProjectRno = vorder.OrderNo;
                vproject.ProjectTypeSt = 0;
                vproject.CustomerNo = vorder.CustomerNo;

                vproject.Name = vorder.FreeInf1Memo1 != "" ? vorder.FreeInf1Memo1 : vorder.Name;
                vproject.Address1 = vorder.AddressLine1;
                vproject.Address2 = vorder.AddressLine2;
                vproject.PostalArea = vorder.PostalArea;
                vproject.PostCode = vorder.PostCode;
                vproject.CreateDate = vorder.OrderDate;
                vproject.DeadlineDt = vorder.RequiredDeliveryDate;
                vproject.EstimatedEndDt = vorder.RequiredDeliveryDate;
                vproject.EstimatedStartDt = vorder.RequiredDeliveryDate;
                vproject.SmartDayProjectID = vorder.Information4; //#########

                vproject.DeliveryName = vorder.DeliveryName;
                vproject.DeliveryAddress1 = vorder.DeliveryAddress1;
                vproject.DeliveryAddress2 = vorder.DeliveryAddress2;
                vproject.DeliveryAddress3 = vorder.DeliveryAddress3;
                vproject.DeliveryAddress4 = vorder.DeliveryAddress4;
                vproject.DeliveryPostalArea = vorder.DeliveryPostalArea;
                vproject.DeliveryPostCode = vorder.DeliveryPostCode;
                vproject.ServiceUnitRno = "";
                // }

           
                string customerSmartDayId = "";
                string siteSmartDayId = "";
                if (SyncCustomerAndSite(vorder.Customer,vorder.Site, ref customerSmartDayId, ref siteSmartDayId) == false)
                {
                    Utils.WriteLog("Unable to sync customer/site for project - skipping order");
                    syncedProjectRNo = 0;
                    continue;
                }
                if (customerSmartDayId != "")
                    vorder.Customer.SmartDayId = customerSmartDayId;
                if (siteSmartDayId != "")
                    vorder.Customer.SmartDaySiteId = siteSmartDayId;

                smartdayProjectID = SyncProject(vproject, vorder.Customer, vorder.OrderNo, ownerUserId, true);
                if (smartdayProjectID == "")
                {
                    Utils.WriteLog("Unable to sync parent project - skipping order");
                    syncedProjectRNo = 0;
                    continue;
                }
                syncedProjectRNo = vproject.ProjectRno;


                // ## 20200427 - sync things related to project.

               
                string thingCustomerId = customerSmartDayId;    // default...
                string thingSiteId = siteSmartDayId;            // default...

                List<Models.VismaSite> sitesForThings = new List<VismaSite>();
                sitesForThings.Add(new VismaSite()
                {
                    AddressLine1 = vorder.Site.AddressLine1,
                    AddressLine2 = vorder.Site.AddressLine2,
                    PostalArea = vorder.Site.PostalArea,
                    PostCode = vorder.Site.PostCode,
                    SmartDayId = siteSmartDayId
                });

                foreach (Models.VismaOrderThing r12 in vorder.R12List)
                {
                    
                    Models.VismaThing vthing = new Models.VismaThing();
                    if (db.GetThing(r12.RNo, ref vthing, out string errmsg1) == false)
                    {
                        Utils.WriteLog($"Error: db.GetThing() - {errmsg1}");
                        continue;
                    }
                    thingSiteId = "";
                    foreach (VismaSite siteToTest in sitesForThings)
                    {
                        if (vthing.Ad1 == siteToTest.AddressLine1 && vthing.Ad2 == siteToTest.AddressLine2 && vthing.PArea == siteToTest.PostalArea && vthing.PNo == siteToTest.PostCode)
                        {
                            thingSiteId = siteSmartDayId;
                            break;
                        }
                    }

                    if (thingSiteId == "")
                    {
                        SyncSiteForThing(vthing, customerSmartDayId, ref thingSiteId);

                    }

                    sitesForThings.Add(new VismaSite() { AddressLine1 = vthing.Ad1, AddressLine2 = vthing.Ad2, PostalArea = vthing.PArea, PostCode = vthing.PNo, SmartDayId = thingSiteId });



                    if (SyncThing(vthing, thingCustomerId, vorder.Customer.CustomerNo.ToString(), thingSiteId, r12.RNo) == false)
                        Utils.WriteLog($"Error: SyncThings failed.");

                    if (SyncOrder(vorder, smartdayProjectID, vorder.OrderNo.ToString(),
                        customerSmartDayId, vorder.Customer.CustomerNo.ToString(),
                        siteSmartDayId, customerSmartDayId, /* = siteSmartDayExternalId */
                         vthing.SmartDayId, r12.RNo, /* = SmartDayIdExternaleThingId */
                        ownerUserId, r12) == false)
                    {
                        Utils.WriteLog("Unable to sync order - skipping order");
                        continue;
                    }
                    else
                    {
                        if (db.UpdateOrderStatus(vorder.OrderNo, r12.RNo, 21, out errmsg) == false)
                            Utils.WriteLog($"Error: db.UpdateOrderStatus() - {errmsg}");
                    }

                }


                if (db.UpdateProjectStatus(vorder.OrderNo, 21, out  errmsg) == false)
                    Utils.WriteLog("ERROR: db.UpdateProjectStatus() - " + errmsg);
            }

            return false;

        }


        private static bool SyncItems(List<Models.Item> items, string storeID)
        {
            SmartDayHttpClient httpClient = new SmartDayHttpClient();

            List<Models.Result> results = httpClient.CreateItemsAsync(items, storeID).Result;
            if (results != null)
            {
                foreach (Models.Result result in results)
                    if (result.hasError == false)
                        Utils.WriteLog($"Item created - {result.entity.id} ");
            }
            return true;
        }

        private static void DumpExistingData()
        {
            SmartDayHttpClient httpClient = new SmartDayHttpClient();

            List<Models.Thing> things = httpClient.GetThingsAsync("").Result;
              if (things == null)
                  Utils.WriteLog($"GetThingsPagedAsync failed - {httpClient.LastError}");
              else
              {
                  foreach (Models.Thing thing in things)
                  {
                      Utils.WriteLog("Thing id     " + thing.id);
                      Utils.WriteLog("Thing ext.id " + thing.externalId);
                      Utils.WriteLog("Thing name   " + thing.name);
                      Utils.WriteLog("Thing state  " + thing.state);
                      Utils.WriteLog("Catagory ref " + thing.categoryReference.name);
                      Utils.WriteLog("Thing note   " + thing.note);
                      Utils.WriteLog("");
                  }
              }
              

            List<Models.Project> existingProjects = httpClient.GetProjectsAsync().Result;
            if (existingProjects != null)
                foreach (Models.Project project in existingProjects)
                {
                    Utils.WriteLog("Project id" + project.id);
                        Utils.WriteLog("Project extid" + project.externalId);
                    Utils.WriteLog("Project name" + project.name);
                    Utils.WriteLog("Project startDate" + project.startDate);
                    Utils.WriteLog("Project status" + project.status.ToString());

                } 

           
            List<Models.Order> existingOrders = httpClient.GetOrdersAsync(DateTime.MinValue).Result;
            if (existingOrders != null)
                foreach (Models.Order order in existingOrders)
                {
                    Utils.WriteLog("Order id" + order.id);
                    Utils.WriteLog("Order extid" + order.externalId);
                    Utils.WriteLog("Project name" + order.name);
                    Utils.WriteLog("Project startDate" + order.startTime);
                    Utils.WriteLog("Project status" + order.smartdayOrder.status.ToString());

                }


            List<Models.Customer> existingCustomers = httpClient.GetCustomersAsync().Result;
            if (existingCustomers != null)
                foreach (Models.Customer customer in existingCustomers)
                {
                    Utils.WriteLog("Customer id" + customer.id);
                    Utils.WriteLog("Customer name" + customer.name);
                    Utils.WriteLog("Customer email" + customer.email);
                    Utils.WriteLog("Customer category" + customer.categoryReference.name);
                }


            List<Models.Site> existingSites = httpClient.GetSitesAsync().Result;
            if (existingSites != null)
            { 
                foreach (Models.Customer customer in existingCustomers)
                {
                    Utils.WriteLog("Customer id" + customer.id);
                    Utils.WriteLog("Customer name" + customer.name);
                    Utils.WriteLog("Customer email" + customer.email);
                    Utils.WriteLog("Customer category" + customer.categoryReference.name);
                }

                List<Models.Store> existingStores = httpClient.GetStoresAsync().Result;
                if (existingStores == null)
                    Utils.WriteLog($"GetStoresAsync failed - {httpClient.LastError}");
                else
                {
                    foreach (Models.Store store in existingStores)
                    {
                        Utils.WriteLog("Store id" + store.id);
                        Utils.WriteLog("Store name" + store.storeName);
                        Utils.WriteLog("Store active " + store.smartdayStore.isActive);
                    }


                    // Get existing items in stores

                    foreach (Models.Store store in existingStores)
                    {
                        if (store.smartdayStore.isActive == false)
                            continue;


                        List<Models.Item> items = httpClient.GetItemsAsync(store.id).Result;
                        if (items == null)
                            Utils.WriteLog($"GetItemsAsync failed - {httpClient.LastError}");
                        else
                        {
                            foreach (Models.Item item in items)
                            {
                                Utils.WriteLog("Item id" + item.id);
                                Utils.WriteLog("Item description" + item.description);
                                Utils.WriteLog("Item unit" + item.unit);
                                Utils.WriteLog("Item price" + item.price);
                                Utils.WriteLog("Item itemNo" + item.itemNo);
                                Utils.WriteLog("Item itemNo2" + item.itemNo2);
                                Utils.WriteLog("Item itemGroup" + item.itemGroup);
                            }
                        }
                    }
                }
            }

        }




        /*    private static bool SyncOrders(List<Models.VismaOrder> vorders)
            {
                if (vorders.Count <= 0)
                    return true;
                SmartDayHttpClient httpClient = new SmartDayHttpClient();
                DBaccess db = new DBaccess();


                List<Models.Order> orders = new List<Models.Order>();

                foreach(Models.VismaOrder vorder in vorders)
                {
                    Models.Site siteForCustomer =  httpClient.GetSiteAsync("", vorder.CustomerNo.ToString()).Result;

                    Models.Order order = httpClient.GetOrderAsync(vorder.OrderNo.ToString()).Result;
                    if (order == null)
                    order = new Models.Order()
                    {
                        createdDate = Utils.DateTime2String(1, Utils.VismaDate2DateTime(vorder.OrderDate)) + "Z",
                        externalId = vorder.OrderNo.ToString(),
                        id = vorder.SmartDayID  == "" ? null : vorder.SmartDayID,
                        name = vorder.Name 
                    };
                    order.startTime = Utils.DateTime2String(0, Utils.VismaDate2DateTime(vorder.OrderDate)) + "Z";
                    order.endTime = Utils.DateTime2String(0, Utils.VismaDate2DateTime(vorder.OrderDate)) + "Z";

                    order.customerReference.externalId = vorder.CustomerNo.ToString();
                    order.customerReference.id = vorder.CustomerNo.ToString();

                    if (siteForCustomer != null)
                    {
                        order.siteReference.externalId = siteForCustomer.externalId;
                        order.siteReference.id = siteForCustomer.id;
                    }
                    else
                    {
                        order.siteReference.externalId = vorder.CustomerNo.ToString();
                        order.siteReference.id = vorder.CustomerNo.ToString();
                    }

                    order.departmentReference.id = Utils.ReadConfigString("DefaultDepartment","3");
                    order.departmentReference.externalId = Utils.ReadConfigString("DefaultDepartment", "3");

                    order.categoryReferences.Clear();
                    order.categoryReferences.Add(new Models.CategoryReferenceOrder()
                    {
                        externalId = Utils.ReadConfigString("DefaultOrderCategory", "10"),
                        id = Utils.ReadConfigString("DefaultOrderCategory", "10")
                    });

                    order.participants.Clear();
                    order.participants.Add(new Models.Participant()
                    {
                        handymanParticipant = null,
                        userReference = new Models.Reference()
                        {
                            id = Utils.ReadConfigString("DefaultParticipant", "1010"),
                            externalId = Utils.ReadConfigString("DefaultParticipant", "1010")
                        }
                    });

                    order.location.address2 = vorder.AddressLine2;
                    order.location.postalArea = vorder.PostalArea;
                    order.location.postalCode = vorder.PostCode;
                    string street = "";
                    string houseNumber = "";
                    Utils.IsolateStreetNumber(vorder.AddressLine1, ref street, ref houseNumber);
                    order.location.address1 = street;
                    order.location.housenumber = houseNumber;

                    order.responsibleReference.externalId = Utils.ReadConfigString("DefaultResponsible", "Visma");
                    order.responsibleReference.id = Utils.ReadConfigString("DefaultResponsible", "Visma");

                    order.handymanOrder = null; //!!
                    order.poNumber = vorder.OrderNo.ToString();

                    order.smartdayOrder.status = vorder.Status;
                    order.smartdayOrder.status = vorder.Status;
                    order.smartdayOrder.projectReference.externalId = "96";// null;
                    order.smartdayOrder.projectReference.id = "96"; // null;
                    order.smartdayOrder.owner = "1003";
                    order.smartdayOrder.thingReference.id = null;
                    order.smartdayOrder.thingReference.externalId = null;
                    order.smartdayOrder.urgent = false;
                    order.smartdayOrder.timeslotAgreed = false;
                    order.smartdayOrder.crmInfo1 = "Tekst1";
                    order.smartdayOrder.crmInfo2 = "Tekst2";

                    order.lastTimeMarkedForExport = Utils.DateTime2String(0, DateTime.Now) + "Z";
                    orders.Add(order);
                }

                List<Models.Result> results = httpClient.CreateOrdersAsync(orders).Result;
                if (results != null)
                {
                    foreach (Models.Result result in results)
                    {
                        if (result.hasError == false)
                        {
                            Models.Order o = orders.FirstOrDefault(p => p.externalId == result.entity.externalId);
                            if (o != null)
                                o.id = result.entity.id;
                            db.RegisterOrderSmartdayID(Utils.StringToInt(o.externalId), o.id, out string errmsg);
                            db.UpdateOrderStatus(Utils.StringToInt(o.externalId), 11, out errmsg);
                            Utils.WriteLog($"Orders  created - {result.entity.id} ");
                        }
                    }

                }

                foreach (Models.VismaOrder vorder in vorders)
                {
                    Models.Order o = orders.FirstOrDefault(p => p.externalId == vorder.OrderNo.ToString());
                    if (o == null)
                        continue;

                    List<Models.Material> materials = httpClient.GetMaterialsForOrder(vorder.OrderNo.ToString()).Result;
                    if (materials == null)
                        materials = new List<Models.Material>();


                    foreach (Models.VismaOrderLine vline in vorder.OrderLines)
                    {
                        Models.Material material = materials.FirstOrDefault(p => p.itemNo == vline.ProductNo && p.itemNo2 == vline.LineNo.ToString());
                        if (material == null)
                        {
                            material = new Models.Material
                            {
                                itemNo = vline.ProductNo,
                                itemNo2 = vline.LineNo.ToString(),

                                externalId = null, 
                                id = null,
                                handymanMaterial = null,
                                date = Utils.DateTime2String(1, DateTime.Now)
                            };

                        }

                        material.quantity = Decimal.ToDouble(vline.Quantity);
                        material.description = vline.Description;

                        material.storeReference.id = null;
                        material.storeReference.externalId = null;
                        material.userReference.externalId = null;
                        material.userReference.id = null;
                        material.smartdayMaterial.categoryReference.externalId = "0";
                        material.smartdayMaterial.categoryReference.id = "0";

                        //    material.wholesalerReference.
                        materials.Add(material);
                    }
                  if (materials.Count > 0)
                    {
                        List<Models.Result> results1 = httpClient.CreateMaterialsForOrderAsync(o.id, materials).Result;
                        if (results1 != null)
                        {
                            foreach (Models.Result result in results1)
                            {
                                if (result.hasError == false)
                                {

                                }
                            }
                        }
                    }


                    List<Models.SalaryCode> salaryCodes = httpClient.GetSalaryCodesForOrder(vorder.OrderNo.ToString()).Result;
                    if (salaryCodes == null)
                        salaryCodes = new List<Models.SalaryCode>();

                    foreach (Models.VismaAgreement vagreement in vorder.AgreementLines)
                    {
                        DateTime fromTime = Utils.VismaDate2DateTime(vagreement.FromDate, vagreement.FromTime);
                        DateTime toTime = Utils.VismaDate2DateTime(vagreement.ToDate, vagreement.ToTime);
                        string uniqueSalaryCodeKey = vagreement.CustomerName + " " + vagreement.AgreementNumber + "-" + vagreement.AgreementActNo;
                        Models.SalaryCode salaryCode = salaryCodes.FirstOrDefault(p => p.description == uniqueSalaryCodeKey);
                        if (salaryCode == null)
                        {
                            salaryCode = new Models.SalaryCode()
                            {
                                userReference = new Models.Reference() { externalId = Utils.ReadConfigString("DefaultUserReference", "1001"), id = Utils.ReadConfigString("DefaultUserReference", "1001") },
                                info = vagreement.Decription,

                                description = uniqueSalaryCodeKey,
                                externalId = null,// vagreement.AgreementNumber.ToString(),
                                id = null
                            };
                        };
                        salaryCode.date = Utils.DateTime2String(0, fromTime);
                        salaryCode.amount = Decimal.ToDouble(vagreement.Quantity);
                        salaryCode.totalPrice = Decimal.ToDouble(vagreement.Price) * Decimal.ToDouble(vagreement.Quantity);
                        salaryCode.status = vagreement.Status;
                            salaryCode.salaryCodeReference.id = Models.SalaryCodesID.Vanlige_Timer;
                        salaryCode.salaryCodeReference.externalId = salaryCode.salaryCodeReference.id;
                        salaryCode.smartdaySalaryCodeRegistration.categoryReference.id = "0";
                        salaryCode.smartdaySalaryCodeRegistration.categoryReference.externalId = "0";
                        salaryCode.smartdaySalaryCodeRegistration.comment = vagreement.Decription;
                        salaryCode.smartdaySalaryCodeRegistration.unit = "time";
                        salaryCode.smartdaySalaryCodeRegistration.unitPrice = 0.0;

                        salaryCodes.Add(salaryCode);
                    }
                    if (salaryCodes.Count > 0)
                    {
                        List<Models.Result> results1 = httpClient.CreateSalaryCodesForOrderAsync(o.id, salaryCodes).Result;
                        if (results1 != null)
                        {
                            foreach (Models.Result result in results1)
                            {
                                if (result.hasError == false)
                                {

                                }
                            }
                        }
                    }
                }

                return true;
            }

       */

    }
}

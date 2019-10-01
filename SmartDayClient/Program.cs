using System;
using System.Collections.Generic;
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
            else
            {
                foreach (Models.SalaryCode salaryCode in existingSalaryCodes)
                {
                    Utils.WriteLog("salaryCode id " + salaryCode.id);
                    Utils.WriteLog("salaryCode name " + salaryCode.description);
                 
                }
            }

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



            if (Utils.ReadConfigInt32("SyncThings", 0) > 0)
            {
                List<Models.Thing> things = new List<Models.Thing>();
                DBaccess db = new DBaccess();
                if (db.GetThings(ref things, out string errmsg) == false)
                {
                    Utils.WriteLog($"ERROR: db.GetThings() - {errmsg}");
                }
                else
                {
                    SyncThings(things);
                }
            }


            if (Utils.ReadConfigInt32("SyncItems",0)> 0)
            {
                List<Models.Item> items = new List<Models.Item>();
                DBaccess db = new DBaccess();
                if (db.GetNewProducts(ref items, out string errmsg) == false)
                {
                    Utils.WriteLog($"ERROR: db.GetNewProducts() - {errmsg}");
                }
                else
                {
                    SyncItems(items, Utils.ReadConfigString("DefaultStoreID","1"));
                }
            }

            if (Utils.ReadConfigInt32("SyncCustomers", 0) > 0)
            {
                List<Models.VismaCustomer> vismaCustomers = new List<Models.VismaCustomer>();
                DBaccess db = new DBaccess();
                if (db.GetNewCustomers(ref vismaCustomers, out string errmsg) == false)
                {
                    Utils.WriteLog($"ERROR: db.GetNewCustomers() - {errmsg}");
                }
                else
                {
                    if (vismaCustomers.Count > 0)
                        SyncCustomersAndSites(vismaCustomers);

                }
            }

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
                    if (vorders.Count > 0)
                        SyncProjects(vorders);
                }

            }

            if (Utils.ReadConfigInt32("SyncOrdersFromSmartDay", 0) > 0)
            {
               /* List<Models.VismaOrderInfo> vorders = new List<Models.VismaOrderInfo>();

                DBaccess db = new DBaccess();
                if (db.GetOrdersToSynchronize(ref vorders, out string errmsg) == false)
                {
                    Utils.WriteLog("ERROR: db.GetOrdersToSynchronize() - " + errmsg);
                }
                else
                {
                    if (vorders.Count > 0)
                        SyncOrdersFromSmartDay(vorders);
                }
*/
                SyncLatestOrderChangesFromSmartDay();

            }
        }

        private static bool SyncLatestOrderChangesFromSmartDay()
        {
            SmartDayHttpClient httpClient = new SmartDayHttpClient();
            DBaccess db = new DBaccess();

            Sync sync = Utils.ReadSyncTime(Utils.ReadConfigString("OrderSyncFile", "OrderSyncFile.xml"));


            List<Models.Order> newOrders = httpClient.GetOrdersAsync(sync.LastestSync).Result;

            if (newOrders == null)
                return false;
            foreach (Models.Order order in newOrders)
            {
                int vismaOrdNo = Utils.StringToInt(order.smartdayOrder.projectReference.externalId??"0");
                int vismaCustNo = Utils.StringToInt(order.customerReference.externalId??"0");
                if (order.smartdayOrder.status != Models.OrderStatus.Accepted && order.smartdayOrder.status != Models.OrderStatus.InProgress)
                    continue;

                string projectID = order.smartdayOrder.projectReference.id ?? "";
                if (projectID == "")
                    projectID = order.poNumber ?? "";

                Utils.WriteLog($"Found smartday order {order.id} (visma ordno {vismaOrdNo} with change (status 3 or 4):");
                Utils.WriteLog("Fetching materials..");
                List<Models.Material> materials = httpClient.GetMaterialsForOrder(order.id).Result;
                if (materials != null)
                {
                    List<Models.Material> materialsToStore = new List<Models.Material>();
                    foreach (Models.Material material in materials)
                    {
                        if (material.status == Models.OrderStatus.Accepted || material.status == Models.OrderStatus.InProgress || material.status == 0) // test
                        {
                            Utils.WriteLog($"Found material for order {order.id} with change (status 3 or 4) :  {material.itemNo} - id {material.id}");
                            materialsToStore.Add(material);
                        }
                    }

                    if (materialsToStore.Count > 0)
                    {
                        Utils.WriteLog("Saving new materials to Visma");
                        foreach (Models.Material materialToStore in materialsToStore)
                        {
                            if (db.UpdateCreateMaterialForOrder(projectID, order.externalId, materialToStore, vismaCustNo, vismaCustNo,46, out string errmsg) == false)
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
                        if (salaryCode.status == Models.OrderStatus.Accepted || salaryCode.status == Models.OrderStatus.InProgress || salaryCode.status == 0)
                        {
                            Utils.WriteLog($"Found salaryCode for order {order.id} with change (status 3 or 4) :  {salaryCode.info} - id {salaryCode.id}");
                            salaryCodesToStore.Add(salaryCode);

                        }
                    }

                    if (salaryCodesToStore.Count > 0)
                    {
                        Utils.WriteLog("Saving new salaryCode to Visma");
                        foreach (Models.SalaryCode salaryCodeToStore in salaryCodesToStore)
                        {
                            if (db.CreateSalaryCodeForOrder(projectID, order.externalId, salaryCodeToStore, vismaCustNo, vismaOrdNo, out string errmsg) == false)
                                Utils.WriteLog("ERROR: db.CreateSalaryCodeForOrder() - " + errmsg);
                        }
                    }

                }

                Models.Project project = httpClient.GetProjectAsync(projectID).Result;
               


                if (project == null)
                    Utils.WriteLog($"Unable to find project {projectID} for order id {order.id}");
                else
                {
                    Utils.WriteLog($"Project {projectID} has status {project.status}");
                    int vismaOrderNo = 0;
                    if (db.GetOrderFromSmartDayProjectID(projectID, ref vismaOrderNo, out string errmsg) == false)
                        Utils.WriteLog("ERROR: GetOrderFromSmartDayProjectID() - " + errmsg);
                    if (vismaOrderNo == 0)
                        Utils.WriteLog($"WARNING: Unable to recognize Smartday project id {projectID} in Visma.. cannot set status");
                    if (project.status == 4 && vismaOrderNo > 0)
                        db.UpdateOrderStatus(vismaOrderNo, 29, out errmsg);

                }
            }

            sync.LastestSync = DateTime.Now;
            Utils.WriteSyncTime(sync, Utils.ReadConfigString("OrderSyncFile", "OrderSyncFile.xml"));

            return true;
        }

        private static bool SyncOrdersFromSmartDay(List<Models.VismaOrderInfo> vorders)
        {
            SmartDayHttpClient httpClient = new SmartDayHttpClient();
            DBaccess db = new DBaccess();
            foreach (Models.VismaOrderInfo vorder in vorders)
            {
                Models.Site siteForCustomer = httpClient.GetSiteAsync("", vorder.SmartDayId.ToString()).Result;

                Models.Order order = null;

                // Try SmartDayID 

                if (vorder.SmartDayId != "")
                    order = httpClient.GetOrderAsync(vorder.SmartDayId).Result;
                if (order == null)
                    order = httpClient.GetOrderAsync(vorder.OrderNo.ToString()).Result;

                if (order == null)
                {
                    Utils.WriteLog($"WARNING: Order {vorder.OrderNo} not found in SmartDay");
                    db.UpdateOrderStatus(vorder.OrderNo, 11, out string errmsg);
                    continue;
                }

                List<Models.SalaryCode> salaryCodes = httpClient.GetSalaryCodesForOrder(order.id).Result;
                if (salaryCodes == null)
                    continue;
                List<Models.VismaAgreement> vagreements = new List<Models.VismaAgreement>();
                foreach (Models.SalaryCode salaryCode in salaryCodes)
                {
                    Models.VismaAgreement vagreement = new Models.VismaAgreement()
                    {
                        Decription = salaryCode.description,
                        Status = salaryCode.status,
                        Price = (decimal)salaryCode.totalPrice,
                        Quantity = (decimal)salaryCode.amount,
                        ProdNo = salaryCode.salaryCodeReference.id,     // !! yes

                    };

                    DateTime t = Utils.StringToDateTime(salaryCode.date);
                    vagreement.FromDate = t.Year * 10000 + t.Month * 100 + t.Day;
                    vagreement.FromTime = t.Hour * 100 + t.Minute;
                    vagreement.ToDate = vagreement.FromDate;
                    vagreement.ToTime = vagreement.FromTime;
                    vagreements.Add(vagreement);
                }

                // Save collected salarycodes to agr..
                foreach (Models.VismaAgreement vagreement in vagreements)
                {

                }
            }

            return true;
        }
   
        private static bool SyncCustomersAndSites(List<Models.VismaCustomer> vcustomers)
        {
            SmartDayHttpClient httpClient = new SmartDayHttpClient();

            DBaccess db = new DBaccess();
            foreach (Models.VismaCustomer vcustomer in vcustomers)
            {
                bool updateCustomer = true;
                Models.Customer customer = httpClient.GetCustomerAsync(vcustomer.CustomerNo.ToString()).Result;

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
                customer.cellPhoneNumber = vcustomer.Mobile;
                customer.phoneNumber = vcustomer.Phone;
                customer.comment = "";

                customer.smartdayCustomer.state = vcustomer.Group3 == 1 ? 0 : 1;

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
                foreach (Models.VismaActor actor in vcustomer.ContactAddressList)
                {
                    customer.contacts.Add(new Models.Contact()
                    {
                        name = actor.Name,
                        cellPhoneNumber = actor.Mobile,
                        email = actor.EmailAddress,
                        phoneNumber = actor.Phone,
                        handymanContact = null,
                        id = null,
                        externalId = actor.ActorNo.ToString()
                    }) ;
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
                            db.RegisterActorSmartdayID(vcustomer.ActorNo, customer.id, out string errmsg);
                        }
                        else
                            Utils.WriteLog($"Error creating customer  - {results[0].errorMessage} ");
                    }                            
                }

                // Now create/update the site

                if (customer.id != null)
                {
                    bool updateSite = true;

                    Models.Site site = httpClient.GetSiteAsync("", customer.id).Result;
                    if (site == null)
                    {
                        site = new Models.Site()
                        {
                            externalId = null,
                            id = null,
                        };

                        updateSite = false;
                    }
                    site.name = vcustomer.Name;
                    site.picture = null;
                    site.serialnumber = null;
                    site.comment = "";
                    site.state = "0";
                    site.handymanSite = null;

                    site.customerReference.externalId = customer.externalId;
                    site.customerReference.id = customer.id;


                    site.categoryReference.id = customer.categoryReference.id;
                    site.categoryReference.externalId = customer.categoryReference.externalId;
                    site.categoryReference.name = customer.categoryReference.name;

                    site.postalAddress.postalCode = vcustomer.PostCode;
                    site.postalAddress.postalArea = vcustomer.PostalArea;
                    site.postalAddress.handymanAddress = null;
                    site.postalAddress.denmarkAddress.globalLocationNumber = null;
                    site.postalAddress.norwayAddress.boligmappaEdokNumber = null;
                    site.postalAddress.norwayAddress.boligmappaPlantID = 0;                   

                    street = "";
                    houseNumber = "";
                     addressLine = vcustomer.AddressLine1 != "" ? vcustomer.AddressLine1 : vcustomer.AddressLine2;
                    Utils.IsolateStreetNumber(addressLine, ref street, ref houseNumber);
                    site.postalAddress.address1 = street;
                    site.postalAddress.housenumber = houseNumber;

                    site.postalAddress.address2 = null;//vcustomer.AddressLine2;
                    site.postalAddress.country = vcustomer.CountryCode;

                    if (updateSite == false)
                    {
                        site.postalAddress.id = null;
                        site.postalAddress.externalId = vcustomer.ActorNo.ToString();
                    }
                    site.contacts.Clear();
                    foreach (Models.VismaActor actor in vcustomer.ContactAddressList)
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

                    List<Models.Site> sites = new List<Models.Site>();
                    sites.Add(site);
                    List<Models.Result> results1 = httpClient.CreateSitesAsync(sites).Result;
                    if (results != null)
                    {
                        if (results.Count > 0)
                        {
                            if (results[0].hasError == false)
                            {
                                site.id = results1[0].entity.id;
                                site.externalId = results1[0].entity.externalId;
                                Utils.WriteLog($"Site created/updated - {site.id} - {site.externalId}");
                                db.RegisterActorSmartdayIDSite(vcustomer.ActorNo, site.id, out string errmsg);
                            }
                            else
                                Utils.WriteLog($"Error creating/updating site  - {results[0].errorMessage} ");
                        }

                    }
                }
            }

            return true;           
        }

        private static bool SyncThings(List<Models.Thing> things)
        {
            SmartDayHttpClient httpClient = new SmartDayHttpClient();
            DBaccess db = new DBaccess();

            Utils.WriteLog($"INFO: {things.Count} things ready for upload..");

            foreach(Models.Thing thing in things)
            {
                if (string.IsNullOrEmpty(thing.id))
                {
                    Utils.WriteLog("ERROR: R12.Rno cannot be empty");
                    continue;
                }
                if (string.IsNullOrEmpty(thing.customerReference.externalId))
                {
                    Utils.WriteLog("ERROR: R12.CustNo cannot be 0");
                    continue;
                }

                Models.Thing existingThing = null;

                if (thing.id != "")
                    existingThing = httpClient.GetThingAsync(thing.id).Result;
                if (existingThing == null)
                {
                    thing.id = null;
                    thing.externalId = null;
                } 
                else
                {
                    thing.externalId = existingThing.externalId; // Fix...
                }
               
                if (thing.customerReference.id != null) // = custno from database
                {
                    // go get site for this customer.
                    Models.Site site = httpClient.GetSiteAsync("", thing.customerReference.id).Result;
                    if (site != null)
                    {
                        thing.parentReference.externalId = site.externalId;
                        thing.parentReference.id = site.id;
                    }
                } 

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
                           
                            Utils.WriteLog($"THing created/updated - {thing.id} - {thing.externalId}");
                            db.RegisterThingSmartdayID(thing.externalId, thing.id, thing.parentReference.id, out string errmsg);
                        }
                        else
                            Utils.WriteLog($"Error creating/updating project  - {results[0].errorMessage} ");
                    }
                }
            }

            return true;
        }

        private static bool SyncProjects(List<Models.VismaOrder> vorders)
        {
            SmartDayHttpClient httpClient = new SmartDayHttpClient();

            List<Models.User> users = httpClient.GetUsersAsync().Result;

            DBaccess db = new DBaccess();

            foreach (Models.VismaOrder vorder in vorders)
            {
                Models.Site site = httpClient.GetSiteAsync("", vorder.CustomerNo.ToString()).Result;

                string descr = "";
                foreach (Models.VismaOrderLine line in vorder.OrderLines)
                {
                    if (descr != "")
                        descr += "\r\n";
                    descr += line.Description;
                }
                DateTime orderDate = Utils.VismaDate2DateTime(vorder.OrderDate);

                // See if project already exists
                Models.Project project = null;

                if (vorder.CustomerOrSupplierOrderNo.Trim() != "")
                    project = httpClient.GetProjectAsync(vorder.CustomerOrSupplierOrderNo).Result;

                if (project == null)
                {
                    Utils.WriteLog("New project!");
                    project = new Models.Project()
                    {
                        name = string.Format("{0} - {1} - {2:0000}-{3:00}-{4:00}", vorder.Name, vorder.AddressLine1, orderDate.Year, orderDate.Month, orderDate.Day),

                        description = descr,
                        createdDate = Utils.DateTime2String(1, orderDate) + "Z",
                        startDate = Utils.DateTime2String(0, orderDate) + "Z",
                        endDate = Utils.DateTime2String(0, orderDate) + "Z",
                        deadlineDate = Utils.DateTime2String(0, orderDate) + "Z",

                        price = 0.0,
                        fixedPrice = false,
                        thingReference = null,
                        offerOrderReference = null,
                        externalId = null, //vorder.OrderNo.ToString(),
                        id = null // indicate CREATE!
                    };

                    project.categoryReference.id = Models.CategoryType.Project_Projekt.ToString();
                    project.categoryReference.externalId = project.categoryReference.id;

                    // Find owner as SelBuy or Empno from visma order.
                    project.ownerReference.id = null;
                    if (vorder.SelBuy > 0 && users != null)
                    {
                        Models.User user = users.FirstOrDefault(p => p.username == vorder.SelBuy.ToString());
                        if (user != null)
                            project.ownerReference.id = user.id;
                    }
                    if (project.ownerReference.id == null)
                    {
                        Models.User user = users.FirstOrDefault(p => p.username == vorder.EmpNo.ToString());
                        if (user != null)
                            project.ownerReference.id = user.id;
                    }
                    if (project.ownerReference.id == null)
                        project.ownerReference.id = Utils.ReadConfigInt32("DefaultProjectOwnerID", 1).ToString();

                }
                else
                {
                    Utils.WriteLog($"Found existing project - id {project.id}");
                    project.externalId = project.id; // repair..
                }

                if (project.id == null)
                    project.status = Models.ProjectStatus.Activated;


                // Add service unit if referenced in order
                project.thingReference = null;
                if (vorder.R12 != "")
                {
                    string thingSmartdayID = "";
                 
                    if (db.GetThingSmartdayID(vorder.R12, ref thingSmartdayID, out string errmsg2) == false)
                        Utils.WriteLog("ERROR: db.GetThingSmartdayID() - " + errmsg2);
                    if (thingSmartdayID != "")
                    {
                        // Check if in SmartDay..
                        Models.Thing existingThing = httpClient.GetThingAsync(thingSmartdayID).Result;
                        if (existingThing != null)
                        {
                            project.thingReference.id = thingSmartdayID;
                            project.thingReference.externalId = thingSmartdayID;
                        }
                    }
                }

                project.customerReference.id = vorder.CustomerNo.ToString();
                project.customerReference.externalId = vorder.CustomerNo.ToString();

                string smartDayCustomerID = "";
                if (db.GetCustomerSmartDayId(vorder.CustomerNo, ref smartDayCustomerID, out string errmsg) == false)
                    Utils.WriteLog("ERROR: db.GetCustomerSmartDayId() - " + errmsg);
                if (smartDayCustomerID != "")
                {
                    project.customerReference.id = smartDayCustomerID;
                    project.customerReference.externalId = smartDayCustomerID;
                }

                // Check if in Smartday
                Models.Customer existingCustomer = httpClient.GetCustomerAsync(project.customerReference.id).Result;
                if (existingCustomer == null)
                {
                    Utils.WriteLog($"ERROR: Customer {vorder.CustomerNo} does not exist in SmartDay");
                    continue;
                }


                project.siteReference.id = site != null ? site.id : null;       // Site for this custno
                project.siteReference.externalId = site != null ? site.externalId : null;

                List<Models.Project> projects = new List<Models.Project>();
                projects.Add(project);


                // STAGE 1 - create/update Smartday project

                List<Models.Result> results = httpClient.CreateProjectsAsync(projects).Result;
                bool projectOK = false;
                if (results != null)
                {
                    if (results.Count > 0)
                    {
                        if (results[0].hasError == false)
                        {
                            project.id = results[0].entity.id;
                            project.externalId = results[0].entity.externalId;
                            Utils.WriteLog($"Project created/updated - {project.id} - {project.externalId}");
                            db.RegisterProjectSmartdayID(vorder.OrderNo, project.id, out  errmsg);
                            projectOK = true;
                        }
                        else
                            Utils.WriteLog($"Error creating/updating project  - {results[0].errorMessage} ");
                    }
                }

                if (projectOK == false)
                    continue;

                // Project created/updated.

                // STAGE 2 - create/update Smartday orders (Visma orderlines)

                // Add visma orderlines as Smartday orders
                bool ordersOK = true;
                foreach (Models.VismaOrderLine vline in vorder.OrderLines)
                {

                    Models.Order order = null;

                    if (vline.WebPg != "")
                        order = httpClient.GetOrderAsync(vline.WebPg).Result;
                    if (order == null)
                        order = new Models.Order()
                        {
                            createdDate = Utils.DateTime2String(1, Utils.VismaDate2DateTime(vorder.OrderDate)) + "Z",
                            externalId = null,
                            id = null,
                            name = vorder.Name
                        };
                    order.startTime = Utils.DateTime2String(0, Utils.VismaDate2DateTime(vorder.OrderDate)) + "Z";
                    order.endTime = Utils.DateTime2String(0, Utils.VismaDate2DateTime(vorder.OrderDate)) + "Z";

                    order.customerReference.externalId = project.customerReference.externalId;
                    order.customerReference.id = project.customerReference.id;

                    order.siteReference.externalId = project.siteReference.externalId;
                    order.siteReference.id = project.siteReference.id;
                   
                  
                    order.departmentReference.id = vorder.Group1 > 0 ? vorder.Group1.ToString() : Utils.ReadConfigString("DefaultDepartment", "1");
                    order.departmentReference.externalId = vorder.Group1 > 0 ? vorder.Group1.ToString() : Utils.ReadConfigString("DefaultDepartment", "1");

                    order.categoryReferences.Clear();
                    order.categoryReferences.Add(new Models.CategoryReferenceOrder()
                    {
                        externalId = Utils.MapOrderCategory(vline.CategoryName).ToString(),
                        id = Utils.MapOrderCategory(vline.CategoryName).ToString()
                    });

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

                    string street = "";
                    string houseNumber = "";
                    string address = vorder.AddressLine1.Trim() != "" ? vorder.AddressLine1.Trim() : vorder.AddressLine2; 

                    Utils.IsolateStreetNumber(address, ref street, ref houseNumber);
                    order.location.address1 = street;
                    order.location.housenumber = houseNumber;
                    order.location.postalArea = vorder.PostalArea;
                    order.location.postalCode = vorder.PostCode;
                    order.location.address2 = null;

                    string responsible = "";
                    db.GetEmployerName(vorder.EmpNo, ref responsible, out  errmsg);
                    order.responsibleReference.externalId = responsible.Trim() != "" ? responsible.Trim() : Utils.ReadConfigString("DefaultResponsible", "Visma");
                    order.responsibleReference.id = responsible != "" ? responsible : Utils.ReadConfigString("DefaultResponsible", "Visma");

                    order.handymanOrder = null; //!!
                    order.poNumber = vorder.OrderNo.ToString();

                    if (order.id == null)
                        order.smartdayOrder.status = Models.OrderStatus.Pending;
                    order.smartdayOrder.projectReference.externalId = project.externalId;
                    order.smartdayOrder.projectReference.id = project.id;
                    order.smartdayOrder.owner = vorder.EmpNo> 0 ? vorder.EmpNo.ToString() : Utils.ReadConfigString("DefaultParticipant", "1");
                    order.smartdayOrder.thingReference.id = null;
                    order.smartdayOrder.thingReference.externalId = null;
                    order.smartdayOrder.urgent = false;
                    order.smartdayOrder.timeslotAgreed = false;
                    order.smartdayOrder.crmInfo1 = "Tekst1";
                    order.smartdayOrder.crmInfo2 = "Tekst2";
                    order.smartdayOrder.status = vorder.Status;
                    order.lastTimeMarkedForExport = Utils.DateTime2String(0, DateTime.Now) + "Z";

                    List<Models.Order> orders = new List<Models.Order>();
                    orders.Add(order);

                    // Send order..!
                    results = httpClient.CreateOrdersAsync(orders).Result;

                    if (results != null)
                    {
                        if (results.Count > 0)
                        {
                            if (results[0].hasError == false)
                            {
                                order.id = results[0].entity.id;
                                order.externalId = results[0].entity.externalId;
                                Utils.WriteLog($"Order created/updated - {order.id} - {order.externalId}");
                                db.RegisterOrderLineSmartdayID(vorder.OrderNo, vline.LineNo, order.id, out errmsg);
                            }
                            else
                            {
                                ordersOK = false;
                                Utils.WriteLog($"Error creating/updating order  - {results[0].errorMessage} ");
                            }
                        }
                    }
                }

                if (projectOK && ordersOK)
                    db.UpdateOrderStatus(vorder.OrderNo, 21, out errmsg);

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

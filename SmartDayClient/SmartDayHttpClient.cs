using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace SmartDayClient
{
    public class SmartDayHttpClient
    {
        private HttpClient client;

        private string _lastError;
        public SmartDayHttpClient()
        {
            HttpClientHandler handler = new HttpClientHandler();
            // {
            //      Proxy = new System.Net.WebProxy("http://127.0.0.1:8888"),
            //      UseProxy = false,
            //  };

            client = new HttpClient
            {

                //var authenticationBytes = Encoding.ASCII.GetBytes(Properties.Settings.Default.WallmobUsername + ":" + Properties.Settings.Default.WallmobPassword);
                Timeout = new TimeSpan(0, 0, 300),
                BaseAddress = new Uri(Utils.ReadConfigString("SmartDayUrl", ""))
            };
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", Utils.ReadConfigString("SmartDayApiKey", ""));
            //client.DefaultRequestHeaders.Authorization =
            //        new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", Convert.ToBase64String(authenticationBytes));
            // client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            _lastError = "";
        }

        public string LastError
        {
            get { return _lastError; }
        }


        public async Task<List<Models.Category>> GetCategoriesAsync()
        {
            Utils.WriteLog("Requesting GetCategoriesAsync()");
            try
            {
                HttpResponseMessage response = await client.GetAsync("categories/").ConfigureAwait(false);
                Utils.WriteLog("Response: " + response.StatusCode.ToString());
                if (response.IsSuccessStatusCode)
                {
                    string str = await response.Content.ReadAsStringAsync();
                    Utils.WriteLog(str);
                    var settings = new JsonSerializerSettings
                    {
                        NullValueHandling = NullValueHandling.Ignore,
                        MissingMemberHandling = MissingMemberHandling.Ignore,

                    };
                    return JsonConvert.DeserializeObject<List<Models.Category>>(str, settings);
                }
                else
                {
                    string str = await response.Content.ReadAsStringAsync();
                    Utils.WriteLog(str);
                    _lastError = str;
                    return null;
                }
            }
            catch (HttpRequestException hre)
            {
                Utils.WriteLog("Error:" + hre.Message);
            }
            catch (TaskCanceledException)
            {
                Utils.WriteLog("Request canceled");
            }
            catch (Exception ex)
            {
                Utils.WriteLog("Exception:" + ex.Message);
            }
            finally
            {
                /*if (httpClient != null)
                {
                    httpClient.Dispose();
                    httpClient = null;
                }*/
            }
            return null;
        }

        public async Task<Models.Project> GetProjectAsync(string id)
        {
            Utils.WriteLog($"Requesting GetProjectAsync({id})");
            try
            {
                HttpResponseMessage response = await client.GetAsync($"projects/{id}").ConfigureAwait(false);
                Utils.WriteLog("Response: " + response.StatusCode.ToString());
                if (response.IsSuccessStatusCode)
                {
                    string str = await response.Content.ReadAsStringAsync();
                    Utils.WriteLog(str);
                    var settings = new JsonSerializerSettings
                    {
                        NullValueHandling = NullValueHandling.Ignore,
                        MissingMemberHandling = MissingMemberHandling.Ignore, 

                    };
                    return JsonConvert.DeserializeObject<Models.Project>(str, settings);
                }
                else
                {
                    string str = await response.Content.ReadAsStringAsync();
                    Utils.WriteLog(str);
                    _lastError = str;
                    return null;
                }
            }
            catch (HttpRequestException hre)
            {
                Utils.WriteLog("Error:" + hre.Message);
                if (hre.StackTrace != null)
                    Utils.WriteLog("Error:" + hre.StackTrace);
                if (hre.InnerException != null) 
                    Utils.WriteLog("Error:" + hre.InnerException.Message);

            }
            catch (TaskCanceledException)
            {
                Utils.WriteLog("Request canceled");
            }
            catch (Exception ex)
            {
                Utils.WriteLog("Exception:" + ex.Message);
                if (ex.StackTrace != null)
                    Utils.WriteLog("Error:" + ex.StackTrace);
                if (ex.InnerException != null)
                    Utils.WriteLog("Error:" + ex.InnerException.Message);
            }
            finally
            {
                /*if (httpClient != null)
                {
                    httpClient.Dispose();
                    httpClient = null;
                }*/
            }
            return null;
        }

        public async Task<List<Models.Project>> GetProjectsAsync()
        {
            Utils.WriteLog("Requesting GetProjectsAsync()");
            try
            {
                HttpResponseMessage response = await client.GetAsync("projects/").ConfigureAwait(false);
                Utils.WriteLog("Response: " + response.StatusCode.ToString());
                if (response.IsSuccessStatusCode)
                {
                    string str = await response.Content.ReadAsStringAsync();
                    Utils.WriteLog(str);
                    var settings = new JsonSerializerSettings
                    {
                        NullValueHandling = NullValueHandling.Ignore,
                        MissingMemberHandling = MissingMemberHandling.Ignore,

                    };
                    return JsonConvert.DeserializeObject<List<Models.Project>>(str, settings);
                }
                else
                {
                    string str = await response.Content.ReadAsStringAsync();
                    Utils.WriteLog(str);
                    _lastError = str;
                    return null;
                }
            }
            catch (HttpRequestException hre)
            {
                Utils.WriteLog("Error:" + hre.Message);
                if (hre.StackTrace != null)
                    Utils.WriteLog("Error:" + hre.StackTrace);
                if (hre.InnerException != null)
                    Utils.WriteLog("Error:" + hre.InnerException.Message);
            }
            catch (TaskCanceledException)
            {
                Utils.WriteLog("Request canceled");
            }
            catch (Exception ex)
            {
                Utils.WriteLog("Exception:" + ex.Message);
                if (ex.StackTrace != null)
                    Utils.WriteLog("Error:" + ex.StackTrace);
                if (ex.InnerException != null)
                    Utils.WriteLog("Error:" + ex.InnerException.Message);
            }
            finally
            {
                /*if (httpClient != null)
                {
                    httpClient.Dispose();
                    httpClient = null;
                }*/
            }
            return null;
        }

        public async Task<List<Models.Store>> GetStoresAsync()
        {
            Utils.WriteLog("Requesting GetStoresAsync()");
            try
            {
                HttpResponseMessage response = await client.GetAsync("stores/").ConfigureAwait(false);
                Utils.WriteLog("Response: " + response.StatusCode.ToString());
                if (response.IsSuccessStatusCode)
                {
                    string str = await response.Content.ReadAsStringAsync();
                    Utils.WriteLog(str);
                    var settings = new JsonSerializerSettings
                    {
                        NullValueHandling = NullValueHandling.Ignore,
                        MissingMemberHandling = MissingMemberHandling.Ignore,

                    };
                    return JsonConvert.DeserializeObject<List<Models.Store>>(str, settings);
                }
                else
                {
                    string str = await response.Content.ReadAsStringAsync();
                    Utils.WriteLog(str);
                    _lastError = str;
                    return null;
                }
            }
            catch (HttpRequestException hre)
            {
                Utils.WriteLog("Error:" + hre.Message);
            }
            catch (TaskCanceledException)
            {
                Utils.WriteLog("Request canceled");
            }
            catch (Exception ex)
            {
                Utils.WriteLog("Exception:" + ex.Message);
            }
            finally
            {
                /*if (httpClient != null)
                {
                    httpClient.Dispose();
                    httpClient = null;
                }*/
            }
            return null;
        }


        public async Task<List<Models.Item>> GetItemsAsync(string storeID)
        {
            Utils.WriteLog($"Requesting GetItemsAsync({storeID})");
            try
            {
                HttpResponseMessage response = await client.GetAsync($"stores/{storeID}/items").ConfigureAwait(false);
                Utils.WriteLog("Response: " + response.StatusCode.ToString());
                if (response.IsSuccessStatusCode)
                {
                    string str = await response.Content.ReadAsStringAsync();
                    Utils.WriteLog(str);
                    var settings = new JsonSerializerSettings
                    {
                        NullValueHandling = NullValueHandling.Ignore,
                        MissingMemberHandling = MissingMemberHandling.Ignore,

                    };
                    return JsonConvert.DeserializeObject<List<Models.Item>>(str, settings);
                }
                else
                {
                    string str = await response.Content.ReadAsStringAsync();
                    Utils.WriteLog(str);
                    _lastError = str;
                    return null;
                }
            }
            catch (HttpRequestException hre)
            {
                Utils.WriteLog("Error:" + hre.Message);
            }
            catch (TaskCanceledException)
            {
                Utils.WriteLog("Request canceled");
            }
            catch (Exception ex)
            {
                Utils.WriteLog("Exception:" + ex.Message);
            }
            finally
            {
                /*if (httpClient != null)
                {
                    httpClient.Dispose();
                    httpClient = null;
                }*/
            }
            return null;
        }

        public async Task<List<Models.Result>> CreateThingsAsync(List<Models.Thing> things)
        {
            Utils.WriteLog($"Requesting CreateThingsAsync()");
            try
            {
                string postBody = JsonConvert.SerializeObject(things);

                Utils.WriteLog(postBody);

                HttpResponseMessage response = await client.PostAsync($"things/", new StringContent(postBody, Encoding.UTF8, "application/json")).ConfigureAwait(false);
                Utils.WriteLog("Response: " + response.StatusCode.ToString());
                if (response.IsSuccessStatusCode)
                {
                    string str = await response.Content.ReadAsStringAsync();
                    var settings = new JsonSerializerSettings
                    {
                        NullValueHandling = NullValueHandling.Ignore,
                        MissingMemberHandling = MissingMemberHandling.Ignore,

                    };
                    return JsonConvert.DeserializeObject<List<Models.Result>>(str, settings);

                }
                else
                {
                    string str = await response.Content.ReadAsStringAsync();
                    Utils.WriteLog(str);
                    _lastError = str;
                    return null;
                }
            }
            catch (HttpRequestException hre)
            {
                Utils.WriteLog("Error:" + hre.Message);
            }
            catch (TaskCanceledException)
            {
                Utils.WriteLog("Request canceled");
            }
            catch (Exception ex)
            {
                Utils.WriteLog("Exception:" + ex.Message);
            }
            finally
            {
                /*if (httpClient != null)
                {
                    httpClient.Dispose();
                    httpClient = null;
                }*/
            }
            return null;
        }


        public async Task<List<Models.Result>> CreateItemsAsync(List<Models.Item> items, string storeID)
        {
            Utils.WriteLog($"Requesting CreateItemsAsync({storeID})");
            try
            {
                string postBody = JsonConvert.SerializeObject(items);

                HttpResponseMessage response = await client.PostAsync($"stores/{storeID}/items/", new StringContent(postBody, Encoding.UTF8, "application/json")).ConfigureAwait(false);
                Utils.WriteLog("Response: " + response.StatusCode.ToString());
                if (response.IsSuccessStatusCode)
                {
                    string str = await response.Content.ReadAsStringAsync();
                    var settings = new JsonSerializerSettings
                    {
                        NullValueHandling = NullValueHandling.Ignore,
                        MissingMemberHandling = MissingMemberHandling.Ignore,

                    };
                    return JsonConvert.DeserializeObject<List<Models.Result>>(str, settings);
                    
                }
                else
                {
                    string str = await response.Content.ReadAsStringAsync();
                    Utils.WriteLog(str);
                    _lastError = str;
                    return null;
                }
            }
            catch (HttpRequestException hre)
            {
                Utils.WriteLog("Error:" + hre.Message);
            }
            catch (TaskCanceledException)
            {
                Utils.WriteLog("Request canceled");
            }
            catch (Exception ex)
            {
                Utils.WriteLog("Exception:" + ex.Message);
            }
            finally
            {
                /*if (httpClient != null)
                {
                    httpClient.Dispose();
                    httpClient = null;
                }*/
            }
            return null;
        }

        public async Task<List<Models.Result>> CreateProjectsAsync(List<Models.Project> projects)
        {
            Utils.WriteLog("Requesting CreateProjectsAsync()");
            try
            {
                string postBody = JsonConvert.SerializeObject(projects);
                Utils.WriteLog(postBody);

                HttpResponseMessage response = await client.PostAsync($"projects/", new StringContent(postBody, Encoding.UTF8, "application/json")).ConfigureAwait(false);
                Utils.WriteLog("Response: " + response.StatusCode.ToString());
                if (response.IsSuccessStatusCode)
                {
                    string str = await response.Content.ReadAsStringAsync();
                    var settings = new JsonSerializerSettings
                    {
                        NullValueHandling = NullValueHandling.Ignore,
                        MissingMemberHandling = MissingMemberHandling.Ignore,

                    };
                    return JsonConvert.DeserializeObject<List<Models.Result>>(str, settings);

                }
                else
                {
                    string str = await response.Content.ReadAsStringAsync();
                    Utils.WriteLog(str);
                    _lastError = str;
                    return null;
                }
            }
            catch (HttpRequestException hre)
            {
                Utils.WriteLog("Error:" + hre.Message);
            }
            catch (TaskCanceledException)
            {
                Utils.WriteLog("Request canceled");
            }
            catch (Exception ex)
            {
                Utils.WriteLog("Exception:" + ex.Message);
            }
            finally
            {
                /*if (httpClient != null)
                {
                    httpClient.Dispose();
                    httpClient = null;
                }*/
            }
            return null;
        }

        public async Task<List<Models.Customer>> GetCustomersAsync()
        {
            Utils.WriteLog($"Requesting GetCustomersAsync()");
            try
            {
                HttpResponseMessage response = await client.GetAsync($"customers/").ConfigureAwait(false);
                Utils.WriteLog("Response: " + response.StatusCode.ToString());
                if (response.IsSuccessStatusCode)
                {
                    string str = await response.Content.ReadAsStringAsync();
                    Utils.WriteLog(str);
                    var settings = new JsonSerializerSettings
                    {
                        NullValueHandling = NullValueHandling.Ignore,
                        MissingMemberHandling = MissingMemberHandling.Ignore,

                    };
                    return JsonConvert.DeserializeObject<List<Models.Customer>>(str, settings);
                }
                else
                {
                    string str = await response.Content.ReadAsStringAsync();
                    Utils.WriteLog(str);
                    _lastError = str;
                    return null;
                }
            }
            catch (HttpRequestException hre)
            {
                Utils.WriteLog("Error:" + hre.Message);
                if (hre.StackTrace != null)
                    Utils.WriteLog("Error:" + hre.StackTrace);
                if (hre.InnerException != null)
                    Utils.WriteLog("Error:" + hre.InnerException.Message);
            }
            catch (TaskCanceledException)
            {
                Utils.WriteLog("Request canceled");
            }
            catch (Exception ex)
            {
                Utils.WriteLog("Exception:" + ex.Message);
                if (ex.StackTrace != null)
                    Utils.WriteLog("Error:" + ex.StackTrace);
                if (ex.InnerException != null)
                    Utils.WriteLog("Error:" + ex.InnerException.Message);
            }
            finally
            {
                /*if (httpClient != null)
                {
                    httpClient.Dispose();
                    httpClient = null;
                }*/
            }
            return null;
        }

        public async Task<Models.Customer> GetCustomerAsync(string id)
        {
            Utils.WriteLog($"Requesting GetCustomerAsync({id})");
            try
            {
                HttpResponseMessage response = await client.GetAsync($"customers/{id}").ConfigureAwait(false);
                Utils.WriteLog("Response: " + response.StatusCode.ToString());
                if (response.IsSuccessStatusCode)
                {
                    string str = await response.Content.ReadAsStringAsync();
                    Utils.WriteLog(str);
                    var settings = new JsonSerializerSettings
                    {
                        NullValueHandling = NullValueHandling.Ignore,
                        MissingMemberHandling = MissingMemberHandling.Ignore,

                    };
                    return JsonConvert.DeserializeObject<Models.Customer>(str, settings);
                }
                else
                {
                    string str = await response.Content.ReadAsStringAsync();
                    Utils.WriteLog(str);
                    _lastError = str;
                    return null;
                }
            }
            catch (HttpRequestException hre)
            {
                Utils.WriteLog("Error:" + hre.Message);
            }
            catch (TaskCanceledException)
            {
                Utils.WriteLog("Request canceled");
            }
            catch (Exception ex)
            {
                Utils.WriteLog("Exception:" + ex.Message);
            }
            finally
            {
                /*if (httpClient != null)
                {
                    httpClient.Dispose();
                    httpClient = null;
                }*/
            }
            return null;
        }

        public async Task<List<Models.SalaryCode>> GetSalaryCodesAsync()
        {
            Utils.WriteLog($"Requesting GetSalaryCodesAsync()");
            try
            {
                HttpResponseMessage response = await client.GetAsync($"salarycodes/").ConfigureAwait(false);
                Utils.WriteLog("Response: " + response.StatusCode.ToString());
                if (response.IsSuccessStatusCode)
                {
                    string str = await response.Content.ReadAsStringAsync();
                    Utils.WriteLog(str);
                    var settings = new JsonSerializerSettings
                    {
                        NullValueHandling = NullValueHandling.Ignore,
                        MissingMemberHandling = MissingMemberHandling.Ignore,

                    };
                    return JsonConvert.DeserializeObject<List<Models.SalaryCode>>(str, settings);
                }
                else
                {
                    string str = await response.Content.ReadAsStringAsync();
                    Utils.WriteLog(str);
                    _lastError = str;
                    return null;
                }
            }
            catch (HttpRequestException hre)
            {
                Utils.WriteLog("Error:" + hre.Message);
            }
            catch (TaskCanceledException)
            {
                Utils.WriteLog("Request canceled");
            }
            catch (Exception ex)
            {
                Utils.WriteLog("Exception:" + ex.Message);
            }
            finally
            {
                /*if (httpClient != null)
                {
                    httpClient.Dispose();
                    httpClient = null;
                }*/
            }
            return null;
        }
        public async Task<List<Models.Result>> CreateCustomersAsync(List<Models.Customer> customers)
        {
            Utils.WriteLog($"Requesting CreateCustomersAsync()");
            try
            {
                string postBody = JsonConvert.SerializeObject(customers);
                Utils.WriteLog(postBody);
                HttpResponseMessage response = await client.PostAsync($"customers/", new StringContent(postBody, Encoding.UTF8, "application/json")).ConfigureAwait(false);
                Utils.WriteLog("Response: " + response.StatusCode.ToString());
                if (response.IsSuccessStatusCode)
                {
                    string str = await response.Content.ReadAsStringAsync();
                    var settings = new JsonSerializerSettings
                    {
                        NullValueHandling = NullValueHandling.Ignore,
                        MissingMemberHandling = MissingMemberHandling.Ignore,

                    };
                    return JsonConvert.DeserializeObject<List<Models.Result>>(str, settings);

                }
                else
                {
                    string str = await response.Content.ReadAsStringAsync();
                    Utils.WriteLog(str);
                    _lastError = str;
                    return null;
                }
            }
            catch (HttpRequestException hre)
            {
                Utils.WriteLog("Error:" + hre.Message);
            }
            catch (TaskCanceledException)
            {
                Utils.WriteLog("Request canceled");
            }
            catch (Exception ex)
            {
                Utils.WriteLog("Exception:" + ex.Message);
            }
            finally
            {
                /*if (httpClient != null)
                {
                    httpClient.Dispose();
                    httpClient = null;
                }*/
            }
            return null;
        }


        // NO GOOD!
        public async Task<List<Models.Customer>> GetCustomersPagedAsync()
        {

            List<Models.Customer> result = new List<Models.Customer>();
            Utils.WriteLog($"Requesting GetCustomersPagedAsync()");

            bool moreToFetch = true;
            int offset = 0;
            int limit = 50;

            while (moreToFetch)
            {
                try
                {
                    HttpResponseMessage response = await client.GetAsync($"customers/?Top={limit}&Skip={offset}").ConfigureAwait(false);
                    Utils.WriteLog("Response: " + response.StatusCode.ToString());
                    if (response.IsSuccessStatusCode)
                    {
                        string str = await response.Content.ReadAsStringAsync();
                        Utils.WriteLog(str);
                        var settings = new JsonSerializerSettings
                        {
                            NullValueHandling = NullValueHandling.Ignore,
                            MissingMemberHandling = MissingMemberHandling.Ignore,

                        };
                        List<Models.Customer> partResult = JsonConvert.DeserializeObject<List<Models.Customer>>(str, settings);
                        if (partResult == null)
                            break;

                        Utils.WriteLog("Offset: " + offset + " results: " + partResult.Count);

                        foreach (Models.Customer customer in partResult)
                            result.Add(customer);
                        if (partResult.Count < limit)
                            return result;
                        offset += limit;
                    }
                    else
                    {
                        string str = await response.Content.ReadAsStringAsync();
                        Utils.WriteLog(str);
                        _lastError = str;
                        return null;
                    }
                }
                catch (HttpRequestException hre)
                {
                    Utils.WriteLog("Error:" + hre.Message);
                }
                catch (TaskCanceledException)
                {
                    Utils.WriteLog("Request canceled");
                }
                catch (Exception ex)
                {
                    Utils.WriteLog("Exception:" + ex.Message);
                }
                finally
                {
                    /*if (httpClient != null)
                    {
                        httpClient.Dispose();
                        httpClient = null;
                    }*/
                }
            }
            return null;
        }


        public async Task<Models.Thing> GetThingAsync(string id)
        {
            Utils.WriteLog($"Requesting GetThingAsync({id})");

            try
            {

                HttpResponseMessage response = await client.GetAsync($"things/{id}").ConfigureAwait(false);
                Utils.WriteLog("Response: " + response.StatusCode.ToString());
                if (response.IsSuccessStatusCode)
                {
                    string str = await response.Content.ReadAsStringAsync();
                    Utils.WriteLog(str);
                    var settings = new JsonSerializerSettings
                    {
                        NullValueHandling = NullValueHandling.Ignore,
                        MissingMemberHandling = MissingMemberHandling.Ignore,

                    };

                    return JsonConvert.DeserializeObject<Models.Thing>(str, settings);                }
                else
                {
                    string str = await response.Content.ReadAsStringAsync();
                    Utils.WriteLog(str);
                    _lastError = str;
                    return null;
                }
            }
            catch (HttpRequestException hre)
            {
                Utils.WriteLog("Error:" + hre.Message);
            }
            catch (TaskCanceledException)
            {
                Utils.WriteLog("Request canceled");
            }
            catch (Exception ex)
            {
                Utils.WriteLog("Exception:" + ex.Message);
            }
            finally
            {
                /*if (httpClient != null)
                {
                    httpClient.Dispose();
                    httpClient = null;
                }*/
            }

            return null;
        }


        public async Task<List<Models.Thing>> GetThingsAsync(string orderID)
        {

            List<Models.Thing> result = new List<Models.Thing>();
            Utils.WriteLog($"Requesting GetThingsPagedAsync()");

            bool moreToFetch = true;
            int offset = 0;
            int limit = 100;

            while (moreToFetch)
            {
                try
                {
      
                    HttpResponseMessage response = await client.GetAsync($"things/?{orderID}Top={limit}&Skip={offset}").ConfigureAwait(false);
                    Utils.WriteLog("Response: " + response.StatusCode.ToString());
                    if (response.IsSuccessStatusCode)
                    {
                        string str = await response.Content.ReadAsStringAsync();
                        Utils.WriteLog(str);
                        var settings = new JsonSerializerSettings
                        {
                            NullValueHandling = NullValueHandling.Ignore,
                            MissingMemberHandling = MissingMemberHandling.Ignore,

                        };
                        List<Models.Thing> partResult = JsonConvert.DeserializeObject<List<Models.Thing>>(str, settings);
                        if (partResult == null)
                            break;

                        Utils.WriteLog("Offset: " + offset + " results: " + partResult.Count);

                        foreach (Models.Thing thing in partResult)
                            result.Add(thing);
                        if (partResult.Count < limit)
                            return result;
                        offset += limit;
                    }
                    else
                    {
                        string str = await response.Content.ReadAsStringAsync();
                        Utils.WriteLog(str);
                        _lastError = str;
                        return null;
                    }
                }
                catch (HttpRequestException hre)
                {
                    Utils.WriteLog("Error:" + hre.Message);
                }
                catch (TaskCanceledException)
                {
                    Utils.WriteLog("Request canceled");
                }
                catch (Exception ex)
                {
                    Utils.WriteLog("Exception:" + ex.Message);
                }
                finally
                {
                    /*if (httpClient != null)
                    {
                        httpClient.Dispose();
                        httpClient = null;
                    }*/
                }
            }
            return null;
        }


        public async Task<List<Models.Material>> GetMaterialsForOrder(string id)
        {
            Utils.WriteLog($"Requesting GetMaterialsForOrder({id})");
            try
            {
                HttpResponseMessage response = await client.GetAsync($"orders/{id}/materials/").ConfigureAwait(false);
                Utils.WriteLog("Response: " + response.StatusCode.ToString());
                if (response.IsSuccessStatusCode)
                {
                    string str = await response.Content.ReadAsStringAsync();
                    Utils.WriteLog(str);
                    var settings = new JsonSerializerSettings
                    {
                        NullValueHandling = NullValueHandling.Ignore,
                        MissingMemberHandling = MissingMemberHandling.Ignore,

                    };
                    return JsonConvert.DeserializeObject<List<Models.Material>>(str, settings);
                }
                else
                {
                    string str = await response.Content.ReadAsStringAsync();
                    Utils.WriteLog(str);
                    _lastError = str;
                    return null;
                }
            }
            catch (HttpRequestException hre)
            {
                Utils.WriteLog("Error:" + hre.Message);
            }
            catch (TaskCanceledException)
            {
                Utils.WriteLog("Request canceled");
            }
            catch (Exception ex)
            {
                Utils.WriteLog("Exception:" + ex.Message);
            }
            finally
            {
                /*if (httpClient != null)
                {
                    httpClient.Dispose();
                    httpClient = null;
                }*/
            }
            return null;
        }


        public async Task<List<Models.SalaryCode>> GetSalaryCodesForOrder(string id)
        {
            Utils.WriteLog($"Requesting GetSalaryCodesForOrder({id})");
            try
            {
                HttpResponseMessage response = await client.GetAsync($"orders/{id}/salarycodes/").ConfigureAwait(false);
                Utils.WriteLog("Response: " + response.StatusCode.ToString());
                if (response.IsSuccessStatusCode)
                {
                    string str = await response.Content.ReadAsStringAsync();
                    Utils.WriteLog(str);
                    var settings = new JsonSerializerSettings
                    {
                        NullValueHandling = NullValueHandling.Ignore,
                        MissingMemberHandling = MissingMemberHandling.Ignore,

                    };
                    return JsonConvert.DeserializeObject<List<Models.SalaryCode>>(str, settings);
                }
                else
                {
                    string str = await response.Content.ReadAsStringAsync();
                    Utils.WriteLog(str);
                    _lastError = str;
                    return null;
                }
            }
            catch (HttpRequestException hre)
            {
                Utils.WriteLog("Error:" + hre.Message);
            }
            catch (TaskCanceledException)
            {
                Utils.WriteLog("Request canceled");
            }
            catch (Exception ex)
            {
                Utils.WriteLog("Exception:" + ex.Message);
            }
            finally
            {
                /*if (httpClient != null)
                {
                    httpClient.Dispose();
                    httpClient = null;
                }*/
            }
            return null;
        }


        public async Task<List<Models.Order>> GetOrdersAsync(DateTime lastSyncTime)
        {
            Utils.WriteLog($"Requesting GetOrdersAsync()");
            try
            {
                string timeStr = "";
                if (lastSyncTime != DateTime.MinValue)
                {
                    timeStr = "?LastTimeMarkedForExport=" + Utils.DateTime2String(1, lastSyncTime);
                }
                Utils.WriteLog(timeStr);
                HttpResponseMessage response = await client.GetAsync($"orders/{timeStr}").ConfigureAwait(false);
                Utils.WriteLog("Response: " + response.StatusCode.ToString());
                if (response.IsSuccessStatusCode)
                {
                    string str = await response.Content.ReadAsStringAsync();
                    Utils.WriteLog(str);
                    var settings = new JsonSerializerSettings
                    {
                        NullValueHandling = NullValueHandling.Ignore,
                        MissingMemberHandling = MissingMemberHandling.Ignore,

                    };
                    return JsonConvert.DeserializeObject<List<Models.Order>>(str, settings);
                }
                else
                {
                    string str = await response.Content.ReadAsStringAsync();
                    Utils.WriteLog(str);
                    _lastError = str;
                    return null;
                }
            }
            catch (HttpRequestException hre)
            {
                Utils.WriteLog("Error:" + hre.Message);
            }
            catch (TaskCanceledException)
            {
                Utils.WriteLog("Request canceled");
            }
            catch (Exception ex)
            {
                Utils.WriteLog("Exception:" + ex.Message);
            }
            finally
            {
                /*if (httpClient != null)
                {
                    httpClient.Dispose();
                    httpClient = null;
                }*/
            }
            return null;
        }

        public async Task<Models.Order> GetOrderAsync(string id)
        {
            Utils.WriteLog($"Requesting GetOrderAsync({id})");
            try
            {
                HttpResponseMessage response = await client.GetAsync($"orders/{id}").ConfigureAwait(false);
                Utils.WriteLog("Response: " + response.StatusCode.ToString());
                if (response.IsSuccessStatusCode)
                {
                    string str = await response.Content.ReadAsStringAsync();
                    Utils.WriteLog(str);
                    var settings = new JsonSerializerSettings
                    {
                        NullValueHandling = NullValueHandling.Ignore,
                        MissingMemberHandling = MissingMemberHandling.Ignore,

                    };
                   return JsonConvert.DeserializeObject<Models.Order>(str, settings);
 
                }
                else
                {
                    string str = await response.Content.ReadAsStringAsync();
                    Utils.WriteLog(str);
                    _lastError = str;
                    return null;
                }
            }
            catch (HttpRequestException hre)
            {
                Utils.WriteLog("Error:" + hre.Message);
            }
            catch (TaskCanceledException)
            {
                Utils.WriteLog("Request canceled");
            }
            catch (Exception ex)
            {
                Utils.WriteLog("Exception:" + ex.Message);
            }
            finally
            {
                /*if (httpClient != null)
                {
                    httpClient.Dispose();
                    httpClient = null;
                }*/
            }
            return null;
        }


        public async Task<List<Models.Document>> GetOrderDocumentsAsync(string id)
        {
            Utils.WriteLog($"Requesting GetOrderDocumentsAsync({id})");
            try
            {
                HttpResponseMessage response = await client.GetAsync($"orders/{id}/documents").ConfigureAwait(false);
                Utils.WriteLog("Response: " + response.StatusCode.ToString());
                if (response.IsSuccessStatusCode)
                {
                    string str = await response.Content.ReadAsStringAsync();
                    Utils.WriteLog(str);
                    var settings = new JsonSerializerSettings
                    {
                        NullValueHandling = NullValueHandling.Ignore,
                        MissingMemberHandling = MissingMemberHandling.Ignore,

                    };
                    return JsonConvert.DeserializeObject<List<Models.Document>>(str, settings);

                }
                else
                {
                    string str = await response.Content.ReadAsStringAsync();
                    Utils.WriteLog(str);
                    _lastError = str;
                    return null;
                }
            }
            catch (HttpRequestException hre)
            {
                Utils.WriteLog("Error:" + hre.Message);
            }
            catch (TaskCanceledException)
            {
                Utils.WriteLog("Request canceled");
            }
            catch (Exception ex)
            {
                Utils.WriteLog("Exception:" + ex.Message);
            }
            finally
            {
                /*if (httpClient != null)
                {
                    httpClient.Dispose();
                    httpClient = null;
                }*/
            }
            return null;
        }

        public async Task<Models.Document> GetOrderDocumentAsync(string id, string documentID)
        {
            Utils.WriteLog($"Requesting GetOrderDocumentAsync({id})");
            try
            {
                HttpResponseMessage response = await client.GetAsync($"orders/{id}/documents/{documentID}").ConfigureAwait(false);
                Utils.WriteLog("Response: " + response.StatusCode.ToString());
                if (response.IsSuccessStatusCode)
                {
                    string str = await response.Content.ReadAsStringAsync();
                    Utils.WriteLog(str);
                    var settings = new JsonSerializerSettings
                    {
                        NullValueHandling = NullValueHandling.Ignore,
                        MissingMemberHandling = MissingMemberHandling.Ignore,

                    };
                    return JsonConvert.DeserializeObject<Models.Document>(str, settings);

                }
                else
                {
                    string str = await response.Content.ReadAsStringAsync();
                    Utils.WriteLog(str);
                    _lastError = str;
                    return null;
                }
            }
            catch (HttpRequestException hre)
            {
                Utils.WriteLog("Error:" + hre.Message);
            }
            catch (TaskCanceledException)
            {
                Utils.WriteLog("Request canceled");
            }
            catch (Exception ex)
            {
                Utils.WriteLog("Exception:" + ex.Message);
            }
            finally
            {
                /*if (httpClient != null)
                {
                    httpClient.Dispose();
                    httpClient = null;
                }*/
            }
            return null;
        }

        public async Task<string> GetOrderDocumentDataAsync(string id, string documentID)
        {
            Utils.WriteLog($"Requesting GetOrderDocumentDataAsync({id}/{documentID})");
            try
            {
                HttpResponseMessage response = await client.GetAsync($"orders/{id}/documents/{documentID}/data").ConfigureAwait(false);
                Utils.WriteLog("Response: " + response.StatusCode.ToString());
                if (response.IsSuccessStatusCode)
                {
                    string str = await response.Content.ReadAsStringAsync();
                 //   Utils.WriteLog(str);
                    var settings = new JsonSerializerSettings
                    {
                        NullValueHandling = NullValueHandling.Ignore,
                        MissingMemberHandling = MissingMemberHandling.Ignore,

                    };
                    return JsonConvert.DeserializeObject<string>(str, settings);

                }
                else
                {
                    string str = await response.Content.ReadAsStringAsync();
                    Utils.WriteLog(str);
                    _lastError = str;
                    return null;
                }
            }
            catch (HttpRequestException hre)
            {
                Utils.WriteLog("Error:" + hre.Message);
            }
            catch (TaskCanceledException)
            {
                Utils.WriteLog("Request canceled");
            }
            catch (Exception ex)
            {
                Utils.WriteLog("Exception:" + ex.Message);
            }
            finally
            {
                /*if (httpClient != null)
                {
                    httpClient.Dispose();
                    httpClient = null;
                }*/
            }
            return null;
        }

        // document.data are expected to be Base64 encoded before creation..
        public async Task<List<Models.Result>> CreateOrderDocumentsAsync(string id, List<Models.Document> orderdocuments)
        {
            Utils.WriteLog($"Requesting CreateOrderDocumentsAsync()");
            try
            {
                string postBody = JsonConvert.SerializeObject(orderdocuments);

            //    Utils.WriteLog(postBody);

                HttpResponseMessage response = await client.PostAsync($"orders/{id}/documents", new StringContent(postBody, Encoding.UTF8, "application/json")).ConfigureAwait(false);
                Utils.WriteLog("Response: " + response.StatusCode.ToString());
                if (response.IsSuccessStatusCode)
                {
                    string str = await response.Content.ReadAsStringAsync();
                    var settings = new JsonSerializerSettings
                    {
                        NullValueHandling = NullValueHandling.Ignore,
                        MissingMemberHandling = MissingMemberHandling.Ignore,

                    };
                    Utils.WriteLog("Response: " + str);
                    return JsonConvert.DeserializeObject<List<Models.Result>>(str, settings);

                }
                else
                {
                    string str = await response.Content.ReadAsStringAsync();
                    Utils.WriteLog(str);
                    _lastError = str;
                    return null;
                }
            }
            catch (HttpRequestException hre)
            {
                Utils.WriteLog("Error:" + hre.Message);
            }
            catch (TaskCanceledException)
            {
                Utils.WriteLog("Request canceled");
            }
            catch (Exception ex)
            {
                Utils.WriteLog("Exception:" + ex.Message);
            }
            finally
            {
                /*if (httpClient != null)
                {
                    httpClient.Dispose();
                    httpClient = null;
                }*/
            }
            return null;
        }



        //####
        public async Task<List<Models.Result>> CreateOrdersAsync(List<Models.Order> orders)
        {
            Utils.WriteLog($"Requesting CreateOrdersAsync()");
            try
            {
                string postBody = JsonConvert.SerializeObject(orders);

                Utils.WriteLog(postBody);

                HttpResponseMessage response = await client.PostAsync($"orders/", new StringContent(postBody, Encoding.UTF8, "application/json")).ConfigureAwait(false);
                Utils.WriteLog("Response: " + response.StatusCode.ToString());
                if (response.IsSuccessStatusCode)
                {
                    string str = await response.Content.ReadAsStringAsync();
                    var settings = new JsonSerializerSettings
                    {
                        NullValueHandling = NullValueHandling.Ignore,
                        MissingMemberHandling = MissingMemberHandling.Ignore,

                    };
                    Utils.WriteLog("Response: " + str);
                    return JsonConvert.DeserializeObject<List<Models.Result>>(str, settings);

                }
                else
                {
                    string str = await response.Content.ReadAsStringAsync();
                    Utils.WriteLog(str);
                    _lastError = str;
                    return null;
                }
            }
            catch (HttpRequestException hre)
            {
                Utils.WriteLog("Error:" + hre.Message);
            }
            catch (TaskCanceledException)
            {
                Utils.WriteLog("Request canceled");
            }
            catch (Exception ex)
            {
                Utils.WriteLog("Exception:" + ex.Message);
            }
            finally
            {
                /*if (httpClient != null)
                {
                    httpClient.Dispose();
                    httpClient = null;
                }*/
            }
            return null;
        }

        public async Task<List<Models.Result>> CreateMaterialsForOrderAsync(string id, List<Models.Material> materials)
        {
            Utils.WriteLog($"Requesting CreateMaterialsForOrderAsync({id})");
            try
            {
                string postBody = JsonConvert.SerializeObject(materials);
                Utils.WriteLog(postBody);
                HttpResponseMessage response = await client.PostAsync($"orders/{id}/materials/", new StringContent(postBody, Encoding.UTF8, "application/json")).ConfigureAwait(false);
                Utils.WriteLog("Response: " + response.StatusCode.ToString());
                if (response.IsSuccessStatusCode)
                {
                    string str = await response.Content.ReadAsStringAsync();
                    var settings = new JsonSerializerSettings
                    {
                        NullValueHandling = NullValueHandling.Ignore,
                        MissingMemberHandling = MissingMemberHandling.Ignore,

                    };
                    return JsonConvert.DeserializeObject<List<Models.Result>>(str, settings);

                }
                else
                {
                    string str = await response.Content.ReadAsStringAsync();
                    Utils.WriteLog(str);
                    _lastError = str;
                    return null;
                }
            }
            catch (HttpRequestException hre)
            {
                Utils.WriteLog("Error:" + hre.Message);
            }
            catch (TaskCanceledException)
            {
                Utils.WriteLog("Request canceled");
            }
            catch (Exception ex)
            {
                Utils.WriteLog("Exception:" + ex.Message);
            }
            finally
            {
                /*if (httpClient != null)
                {
                    httpClient.Dispose();
                    httpClient = null;
                }*/
            }
            return null;
        }

        public async Task<List<Models.Result>> CreateSalaryCodesForOrderAsync(string id, List<Models.SalaryCode> salaryCodes)
        {
            Utils.WriteLog($"Requesting CreateSalaryCodesForOrderAsync({id})");
            try
            {
                string postBody = JsonConvert.SerializeObject(salaryCodes);

                HttpResponseMessage response = await client.PostAsync($"orders/{id}/salarycodes/", new StringContent(postBody, Encoding.UTF8, "application/json")).ConfigureAwait(false);
                Utils.WriteLog("Response: " + response.StatusCode.ToString());
                if (response.IsSuccessStatusCode)
                {
                    string str = await response.Content.ReadAsStringAsync();
                    var settings = new JsonSerializerSettings
                    {
                        NullValueHandling = NullValueHandling.Ignore,
                        MissingMemberHandling = MissingMemberHandling.Ignore,

                    };
                    return JsonConvert.DeserializeObject<List<Models.Result>>(str, settings);

                }
                else
                {
                    string str = await response.Content.ReadAsStringAsync();
                    Utils.WriteLog(str);
                    _lastError = str;
                    return null;
                }
            }
            catch (HttpRequestException hre)
            {
                Utils.WriteLog("Error:" + hre.Message);
            }
            catch (TaskCanceledException)
            {
                Utils.WriteLog("Request canceled");
            }
            catch (Exception ex)
            {
                Utils.WriteLog("Exception:" + ex.Message);
            }
            finally
            {
                /*if (httpClient != null)
                {
                    httpClient.Dispose();
                    httpClient = null;
                }*/
            }
            return null;
        }

        public async Task<List<Models.Department>> GetDepartmentsAsync()
        {
            Utils.WriteLog($"Requesting GetDepartmentsAsync()");
            try
            {
                HttpResponseMessage response = await client.GetAsync($"departments/").ConfigureAwait(false);
                Utils.WriteLog("Response: " + response.StatusCode.ToString());
                if (response.IsSuccessStatusCode)
                {
                    string str = await response.Content.ReadAsStringAsync();
                    Utils.WriteLog(str);
                    var settings = new JsonSerializerSettings
                    {
                        NullValueHandling = NullValueHandling.Ignore,
                        MissingMemberHandling = MissingMemberHandling.Ignore,

                    };
                    return JsonConvert.DeserializeObject<List<Models.Department>>(str, settings);
                }
                else
                {
                    string str = await response.Content.ReadAsStringAsync();
                    Utils.WriteLog(str);
                    _lastError = str;
                    return null;
                }
            }
            catch (HttpRequestException hre)
            {
                Utils.WriteLog("Error:" + hre.Message);
            }
            catch (TaskCanceledException)
            {
                Utils.WriteLog("Request canceled");
            }
            catch (Exception ex)
            {
                Utils.WriteLog("Exception:" + ex.Message);
            }
            finally
            {
                /*if (httpClient != null)
                {
                    httpClient.Dispose();
                    httpClient = null;
                }*/
            }
            return null;
        }


        public async Task<List<Models.Site>> GetSitesAsync()
        {
            Utils.WriteLog($"Requesting GetSitesAsync()");
            try
            {
                HttpResponseMessage response = await client.GetAsync($"sites/").ConfigureAwait(false);
                Utils.WriteLog("Response: " + response.StatusCode.ToString());
                if (response.IsSuccessStatusCode)
                {
                    string str = await response.Content.ReadAsStringAsync();
                    Utils.WriteLog(str);
                    var settings = new JsonSerializerSettings
                    {
                        NullValueHandling = NullValueHandling.Ignore,
                        MissingMemberHandling = MissingMemberHandling.Ignore,

                    };
                    return JsonConvert.DeserializeObject<List<Models.Site>>(str, settings);
                }
                else
                {
                    string str = await response.Content.ReadAsStringAsync();
                    Utils.WriteLog(str);
                    _lastError = str;
                    return null;
                }
            }
            catch (HttpRequestException hre)
            {
                Utils.WriteLog("Error:" + hre.Message);
            }
            catch (TaskCanceledException)
            {
                Utils.WriteLog("Request canceled");
            }
            catch (Exception ex)
            {
                Utils.WriteLog("Exception:" + ex.Message);
            }
            finally
            {
                /*if (httpClient != null)
                {
                    httpClient.Dispose();
                    httpClient = null;
                }*/
            }
            return null;
        }

        public async Task<List<Models.User>> GetUsersAsync()
        {
            Utils.WriteLog($"Requesting GetUsersAsync()");
            try
            {
                HttpResponseMessage response = await client.GetAsync($"users/").ConfigureAwait(false);
                Utils.WriteLog("Response: " + response.StatusCode.ToString());
                if (response.IsSuccessStatusCode)
                {
                    string str = await response.Content.ReadAsStringAsync();
                    Utils.WriteLog(str);
                    var settings = new JsonSerializerSettings
                    {
                        NullValueHandling = NullValueHandling.Ignore,
                        MissingMemberHandling = MissingMemberHandling.Ignore,

                    };
                    return JsonConvert.DeserializeObject<List<Models.User>>(str, settings);
                }
                else
                {
                    string str = await response.Content.ReadAsStringAsync();
                    Utils.WriteLog(str);
                    _lastError = str;
                    return null;
                }
            }
            catch (HttpRequestException hre)
            {
                Utils.WriteLog("Error:" + hre.Message);
            }
            catch (TaskCanceledException)
            {
                Utils.WriteLog("Request canceled");
            }
            catch (Exception ex)
            {
                Utils.WriteLog("Exception:" + ex.Message);
            }
            finally
            {
                /*if (httpClient != null)
                {
                    httpClient.Dispose();
                    httpClient = null;
                }*/
            }
            return null;
        }


        public async Task<List<Models.Site>> GetSitesAsync(string customerId)
        {
            Utils.WriteLog($"Requesting GetSiteAsync({customerId})");
            try
            {
                string queryString = "?customerId=" + customerId;

                    

                HttpResponseMessage response = await client.GetAsync($"sites/{queryString}").ConfigureAwait(false);
                Utils.WriteLog("Response: " + response.StatusCode.ToString());
                if (response.IsSuccessStatusCode)
                {
                    string str = await response.Content.ReadAsStringAsync();
                    Utils.WriteLog(str);
                    var settings = new JsonSerializerSettings
                    {
                        NullValueHandling = NullValueHandling.Ignore,
                        MissingMemberHandling = MissingMemberHandling.Ignore,

                    };

                    // Return first
                    return JsonConvert.DeserializeObject<List<Models.Site>>(str, settings);

                }
                else
                {
                    string str = await response.Content.ReadAsStringAsync();
                    Utils.WriteLog(str);
                    _lastError = str;
                    return null;
                }
            }
            catch (HttpRequestException hre)
            {
                Utils.WriteLog("Error:" + hre.Message);
            }
            catch (TaskCanceledException)
            {
                Utils.WriteLog("Request canceled");
            }
            catch (Exception ex)
            {
                Utils.WriteLog("Exception:" + ex.Message);
            }
            finally
            {
                /*if (httpClient != null)
                {
                    httpClient.Dispose();
                    httpClient = null;
                }*/
            }
            return null;
        }

        public async Task<Models.Site> GetSiteAsync(string id)
        {
            Utils.WriteLog($"Requesting GetSiteAsync({id})");
            try
            {

                HttpResponseMessage response = await client.GetAsync($"sites/{id}").ConfigureAwait(false);
                Utils.WriteLog("Response: " + response.StatusCode.ToString());
                if (response.IsSuccessStatusCode)
                {
                    string str = await response.Content.ReadAsStringAsync();
                    Utils.WriteLog(str);
                    var settings = new JsonSerializerSettings
                    {
                        NullValueHandling = NullValueHandling.Ignore,
                        MissingMemberHandling = MissingMemberHandling.Ignore,

                    };

                    // Return first
                    return JsonConvert.DeserializeObject<Models.Site>(str, settings);

                }
                else
                {
                    string str = await response.Content.ReadAsStringAsync();
                    Utils.WriteLog(str);
                    _lastError = str;
                    return null;
                }
            }
            catch (HttpRequestException hre)
            {
                Utils.WriteLog("Error:" + hre.Message);
            }
            catch (TaskCanceledException)
            {
                Utils.WriteLog("Request canceled");
            }
            catch (Exception ex)
            {
                Utils.WriteLog("Exception:" + ex.Message);
            }
            finally
            {
                /*if (httpClient != null)
                {
                    httpClient.Dispose();
                    httpClient = null;
                }*/
            }
            return null;
        }




        public async Task<List<Models.Result>> CreateSitesAsync(List<Models.Site> sites)
        {
            Utils.WriteLog($"Requesting CreateSitesAsync()");
            try
            {
                string postBody = JsonConvert.SerializeObject(sites);

                HttpResponseMessage response = await client.PostAsync($"sites/", new StringContent(postBody, Encoding.UTF8, "application/json")).ConfigureAwait(false);
                Utils.WriteLog("Response: " + response.StatusCode.ToString());
                if (response.IsSuccessStatusCode)
                {
                    string str = await response.Content.ReadAsStringAsync();
                    var settings = new JsonSerializerSettings
                    {
                        NullValueHandling = NullValueHandling.Ignore,
                        MissingMemberHandling = MissingMemberHandling.Ignore,

                    };
                    return JsonConvert.DeserializeObject<List<Models.Result>>(str, settings);

                }
                else
                {
                    string str = await response.Content.ReadAsStringAsync();
                    Utils.WriteLog(str);
                    _lastError = str;
                    return null;
                }
            }
            catch (HttpRequestException hre)
            {
                Utils.WriteLog("Error:" + hre.Message);
            }
            catch (TaskCanceledException)
            {
                Utils.WriteLog("Request canceled");
            }
            catch (Exception ex)
            {
                Utils.WriteLog("Exception:" + ex.Message);
            }
            finally
            {
                /*if (httpClient != null)
                {
                    httpClient.Dispose();
                    httpClient = null;
                }*/
            }
            return null;
        }


    }
}

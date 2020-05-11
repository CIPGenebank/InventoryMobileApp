using InventoryApp.Helpers;
using InventoryApp.Models;
using InventoryApp.Models.Database;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace InventoryApp.Services
{
    public class RestClient
    {
        // Android Emulator 10.0.2.2
        private const string LoginEndPoint = "http://{0}/gringlobal/WCFService.svc/login";
        private const string SearchEndPoint = "http://{0}/gringlobal/WCFService.svc/search/{1}";

        private const string InventoryEndPoint = "http://{0}/gringlobal/WCFService.svc/rest/inventory/{1}";
        private const string InventoryCreateEndPoint = "http://{0}/gringlobal/WCFService.svc/rest/inventory";

        private const string InventoryActionEndPoint = "http://{0}/gringlobal/WCFService.svc/rest/inventory_action";

        private const string GetDataEndPoint = "http://{0}/GrinGlobal/WCFService.svc/getdata/{1}?parameters={2}";

        private const string PrinterEndPoint = "http://{0}/GrinGlobal/WCFService.svc/printer/{1}";

        private readonly HttpClient _httpClient = new HttpClient();

        public RestClient()
        {

        }

        public async Task<Login> Login(string username, string password, string server)
        {

            var data = JsonConvert.SerializeObject(new Credential { Username = username, Password = password });
            var content = new StringContent(data, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync(string.Format(LoginEndPoint, server/*Settings.Server*/), content);

            string resultContent = response.Content.ReadAsStringAsync().Result;

            //System.Diagnostics.Debug.WriteLine(resultContent);

            Login resp = JsonConvert.DeserializeObject<Login>(resultContent);

            resp.user_name = username;
            resp.login_token = resp.Token;

            return resp;
        }

        public async Task<List<InventoryThumbnail>> Search(string query, string dataview, string resolver = "accession")
        {
            List<InventoryThumbnail> result = null;

            var data = JsonConvert.SerializeObject(query);

            _httpClient.DefaultRequestHeaders.Clear();
            //_httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            //_httpClient.DefaultRequestHeaders.AcceptEncoding.Add(new StringWithQualityHeaderValue("utf-8"));
            _httpClient.DefaultRequestHeaders.TryAddWithoutValidation("Authorization", string.Format("Bearer {0}", Settings.Token));

            StringContent content = new StringContent(data, Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync(string.Format(SearchEndPoint, Settings.Server, resolver) + "?dataview=get_mob_inventory", content);

            string resultContent = response.Content.ReadAsStringAsync().Result;

            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                result = JsonConvert.DeserializeObject<List<InventoryThumbnail>>(resultContent);
            }
            else if (response.StatusCode == System.Net.HttpStatusCode.NoContent)
            {
                result = null;
            }
            else
            {
                throw new Exception(resultContent);
            }

            return result;
        }
        /*conventir en una funcion generica*/
            public async Task<List<AccessionThumbnail>> SearchAccession(string query, string dataview, string resolver = "accession")
            {
                List<AccessionThumbnail> result = null;

                var data = JsonConvert.SerializeObject(query);

                _httpClient.DefaultRequestHeaders.Clear();
                _httpClient.DefaultRequestHeaders.TryAddWithoutValidation("Authorization", string.Format("Bearer {0}", Settings.Token));

                StringContent content = new StringContent(data, Encoding.UTF8, "application/json");
                var response = await _httpClient.PostAsync(string.Format(SearchEndPoint, Settings.Server, resolver) + "?dataview=get_accession_thumbnail", content);

                string resultContent = response.Content.ReadAsStringAsync().Result;

                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    result = JsonConvert.DeserializeObject<List<AccessionThumbnail>>(resultContent);
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.NoContent)
                {
                    result = null;
                }
                else
                {
                    throw new Exception(resultContent);
                }

                return result;
            }

            public async Task<string> CreateInventoryAction(InventoryAction invAction)
            {
                string result = string.Empty;

                var data = JsonConvert.SerializeObject(JsonConvert.SerializeObject(invAction));
                _httpClient.DefaultRequestHeaders.Clear();
                _httpClient.DefaultRequestHeaders.TryAddWithoutValidation("Authorization", string.Format("Bearer {0}", Settings.Token));
                StringContent content = new StringContent(data, Encoding.UTF8, "application/json");
                var response = await _httpClient.PostAsync(string.Format(InventoryActionEndPoint, Settings.Server), content);

                string resultContent = response.Content.ReadAsStringAsync().Result;
                result = resultContent;
                return result;
            }
            
            public async Task<string> CreateInventory(Inventory inventory)
            {
                string result = string.Empty;

                var data = JsonConvert.SerializeObject(JsonConvert.SerializeObject(inventory));
                _httpClient.DefaultRequestHeaders.Clear();
                _httpClient.DefaultRequestHeaders.TryAddWithoutValidation("Authorization", string.Format("Bearer {0}", Settings.Token));
                StringContent content = new StringContent(data, Encoding.UTF8, "application/json");
                var response = await _httpClient.PostAsync(string.Format(InventoryCreateEndPoint, Settings.Server), content);

                string resultContent = response.Content.ReadAsStringAsync().Result;

                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    result = JsonConvert.DeserializeObject<string>(resultContent);
                }
                else
                {
                    throw new Exception(resultContent);
                }

                return result;
            }
            
            public async Task<Inventory> ReadInventory(int inventoryId)
            {
                Inventory result = null;

                _httpClient.DefaultRequestHeaders.Clear();
                _httpClient.DefaultRequestHeaders.TryAddWithoutValidation("Authorization", string.Format("Bearer {0}", Settings.Token));

                var response = await _httpClient.GetAsync(string.Format(InventoryEndPoint, Settings.Server, inventoryId));

                string resultContent = response.Content.ReadAsStringAsync().Result;
                result = JsonConvert.DeserializeObject<Inventory>(resultContent);
                return result;
            }
            
            public async Task<List<CooperatorGroup>> GetWorkGroups(int cooperatorId)
            {
                List<CooperatorGroup> result = new List<CooperatorGroup>();

                _httpClient.DefaultRequestHeaders.Clear();
                _httpClient.DefaultRequestHeaders.TryAddWithoutValidation("Authorization", string.Format("Bearer {0}", Settings.Token));

                string URL = string.Format(GetDataEndPoint, Settings.Server, "get_cip_cooperator_groups", System.Net.WebUtility.UrlEncode(":cooperatorid=" + cooperatorId));
                var response = await _httpClient.GetAsync(URL);

                string resultContent = response.Content.ReadAsStringAsync().Result;
                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    result = JsonConvert.DeserializeObject<List<CooperatorGroup>>(resultContent);
                }
                else
                {
                    throw new Exception(resultContent);
                }

                return result;
            }
            
            public async Task<List<Location>> GetLocations(string inventoryMaintPolicyId)
            {
                List<Location> result = new List<Location>();

                _httpClient.DefaultRequestHeaders.Clear();
                _httpClient.DefaultRequestHeaders.TryAddWithoutValidation("Authorization", string.Format("Bearer {0}", Settings.Token));

                string URL = string.Format(GetDataEndPoint, Settings.Server, "get_cip_locations", System.Net.WebUtility.UrlEncode(":inventorymaintpolicyid=" + inventoryMaintPolicyId));
                var response = await _httpClient.GetAsync(URL);

                string resultContent = response.Content.ReadAsStringAsync().Result;
                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    result = JsonConvert.DeserializeObject<List<Location>>(resultContent);
                }
                else
                {
                    throw new Exception(resultContent);
                }

                return result;
            }
            
            public async Task<string> UpdateInventory(InventoryThumbnail inventory)
            {
                string result = string.Empty;

                var data = JsonConvert.SerializeObject(JsonConvert.SerializeObject(inventory));
                System.Diagnostics.Debug.WriteLine(JsonConvert.SerializeObject(inventory, Formatting.Indented));

                _httpClient.DefaultRequestHeaders.Clear();
                _httpClient.DefaultRequestHeaders.TryAddWithoutValidation("Authorization", string.Format("Bearer {0}", Settings.Token));
                StringContent content = new StringContent(data, Encoding.UTF8, "application/json");
                var response = await _httpClient.PutAsync(string.Format(InventoryEndPoint, Settings.Server, inventory.inventory_id), content);
                string resultContent = response.Content.ReadAsStringAsync().Result;

                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    result = resultContent;
                }
                else
                {
                    throw new Exception(resultContent);
                }
                return result;
            }
            
            public async Task<string> Print(int printerId, string label)
            {
                string result = string.Empty;

                //var data = JsonConvert.SerializeObject(label);

                _httpClient.DefaultRequestHeaders.Clear();
                _httpClient.DefaultRequestHeaders.TryAddWithoutValidation("Authorization", string.Format("Bearer {0}", Settings.Token));
                StringContent content = new StringContent("\"" + label + "\"", Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync(string.Format(PrinterEndPoint, Settings.Server, printerId), content);
                string resultContent = response.Content.ReadAsStringAsync().Result;

                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    result = resultContent;
                }
                else
                {
                    throw new Exception(resultContent);
                }
                return result;
            }

    }

    class Credential
    {
        public string Username { get; set; }
        public string Password { get; set; }
    }
    public class LoginResponse
    {
        public string Error { get; set; }
        public string Success { get; set; }
        public string Token { get; set; }
        public int CooperatorId { get; set; }
    }
}

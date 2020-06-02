using InventoryApp.Interfaces;
using InventoryApp.Models;
using InventoryApp.Models.Database;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace InventoryApp.Helpers
{
    public class RestService : IRestService
    {
        readonly HttpClient _httpClient;
        public List<InventoryViability> InventoryViabilityItems { get; private set; }

        private const string SearchEndPoint = "http://{0}/GringlobalService/WCFService.svc/search/{1}?dataview={2}";
        private const string RestUrlC = "http://{0}/GringlobalService/WCFService.svc/rest/{1}";
        private const string RestUrlRUD = "http://{0}/GringlobalService/WCFService.svc/rest/{1}/{2}";
        private const string InventoryViabilityListUrl = "http://{0}/GringlobalService/WCFService.svc/getdata/get_inventory_viability_list";
        private const string GetDataURL = "http://{0}/GringlobalService/WCFService.svc/getdata/{1}";

        public RestService()
        {
            _httpClient = new HttpClient();
            _httpClient.DefaultRequestHeaders.Clear();
            _httpClient.MaxResponseContentBufferSize = 256000;
            _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", Settings.Token);

            //_client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            //_client.DefaultRequestHeaders.AcceptEncoding.Add(new StringWithQualityHeaderValue("utf-8"));
            //_client.DefaultRequestHeaders.TryAddWithoutValidation("Authorization", string.Format("Bearer {0}", Settings.Token));
        }

        #region InventoryViability
        public async Task<string> CreateInventoryViabilityAsync(InventoryViability item)
        {
            string result = string.Empty;

            var data = JsonConvert.SerializeObject(JsonConvert.SerializeObject(item));
            StringContent content = new StringContent(data, Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync(string.Format(RestUrlC, Settings.Server, "inventory_viability"), content);

            string resultContent = response.Content.ReadAsStringAsync().Result;

            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                //result = JsonConvert.DeserializeObject<List<InventoryViability>>(resultContent);
                result = resultContent;
            }
            else
            {
                throw new Exception(resultContent);
            }

            return result;
        }
        public async Task<string> UpdateInventoryViabilityAsync(InventoryViability item)
        {
            string result = string.Empty;

            var data = JsonConvert.SerializeObject(JsonConvert.SerializeObject(item));
            StringContent content = new StringContent(data, Encoding.UTF8, "application/json");
            var response = await _httpClient.PutAsync(string.Format(RestUrlRUD, Settings.Server, "inventory_viability", item.InventoryViabilityId), content);

            string resultContent = response.Content.ReadAsStringAsync().Result;

            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                //result = JsonConvert.DeserializeObject<List<InventoryViability>>(resultContent);
                result = resultContent;
            }
            else
            {
                throw new Exception(resultContent);
            }

            return result;
        }

        public async Task DeleteInventoryViabilityAsync(string id)
        {
            throw new NotImplementedException();
        }
        public async Task SaveInventoryViabilityAsync(InventoryViability item, bool isNewItem)
        {
            throw new NotImplementedException();
        }
        public async Task<List<InventoryViability>> RefreshInventoryViabilityAsync()
        {
            InventoryViabilityItems = new List<InventoryViability>();

            var uri = new Uri(string.Format(InventoryViabilityListUrl, Settings.Server));

            try
            {
                var response = await _httpClient.GetAsync(uri);
                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    InventoryViabilityItems = JsonConvert.DeserializeObject<List<InventoryViability>>(content);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(@"				ERROR {0}", ex.Message);
            }

            return InventoryViabilityItems;
        }
        #endregion

        public async Task<List<InventoryViability>> SearchInventoryViabilityAsync(string query, string dataview, string resolver)
        {

            List<InventoryViability> result = null;
            var data = JsonConvert.SerializeObject(query);

            StringContent content = new StringContent(data, Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync(string.Format(SearchEndPoint, Settings.Server, resolver, dataview), content);

            string resultContent = response.Content.ReadAsStringAsync().Result;

            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                result = JsonConvert.DeserializeObject<List<InventoryViability>>(resultContent);
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

        public async Task<List<InventoryThumbnail>> SearchInventoryThumbnailAsync(string query, string dataview, string resolver)
        {
            List<InventoryThumbnail> result = null;
            var data = JsonConvert.SerializeObject(query);

            StringContent content = new StringContent(data, Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync(string.Format(SearchEndPoint, Settings.Server, resolver, dataview), content);

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

        #region InventoryViabilityData
        public async Task<List<InventoryViabilityData>> SearchInventoryViabilityDataAsync(string query, string dataview, string resolver)
        {
            List<InventoryViabilityData> result = null;
            var data = JsonConvert.SerializeObject(query);

            StringContent content = new StringContent(data, Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync(string.Format(SearchEndPoint, Settings.Server, resolver, dataview), content);

            string resultContent = response.Content.ReadAsStringAsync().Result;

            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                result = JsonConvert.DeserializeObject<List<InventoryViabilityData>>(resultContent);
            }
            else if (response.StatusCode == System.Net.HttpStatusCode.NoContent)
            {
                result = null;
            }
            else
            {
                throw new Exception(JsonConvert.DeserializeObject<string>(resultContent));
            }
            return result;
        }

        public async Task<string> CreateInventoryViabilityDataAsync(InventoryViabilityData item)
        {
            string result = string.Empty;

            var data = JsonConvert.SerializeObject(JsonConvert.SerializeObject(item));
            StringContent content = new StringContent(data, Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync(string.Format(RestUrlC, Settings.Server, "inventory_viability_data"), content);

            string resultContent = response.Content.ReadAsStringAsync().Result;

            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                result = resultContent;
            }
            else
            {
                throw new Exception(JsonConvert.DeserializeObject<string>(resultContent));
            }

            return result;
        }
        public async Task<string> UpdateInventoryViabilityDataAsync(InventoryViabilityData item)
        {
            string result = string.Empty;

            var data = JsonConvert.SerializeObject(JsonConvert.SerializeObject(item));
            StringContent content = new StringContent(data, Encoding.UTF8, "application/json");
            var response = await _httpClient.PutAsync(string.Format(RestUrlRUD, Settings.Server, "inventory_viability_data", item.InventoryViabilityDataId), content);

            string resultContent = response.Content.ReadAsStringAsync().Result;

            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                result = resultContent;
            }
            else
            {
                throw new Exception(JsonConvert.DeserializeObject<string>(resultContent));
            }

            return result;
        }
        #endregion

        public async Task<List<InventoryViabilityRule>> SearchInventoryViabilityRuleAsync(string query, string dataview, string resolver)
        {
            List<InventoryViabilityRule> result = null;
            var data = JsonConvert.SerializeObject(query);

            StringContent content = new StringContent(data, Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync(string.Format(SearchEndPoint, Settings.Server, resolver, dataview), content);

            string resultContent = response.Content.ReadAsStringAsync().Result;

            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                result = JsonConvert.DeserializeObject<List<InventoryViabilityRule>>(resultContent);
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


        public async Task<string> GetNewInventoryID()
        {
            string result = string.Empty;

            var response = await _httpClient.GetAsync(string.Format(GetDataURL, Settings.Server, "wcf_new_inventory_id"));


            string resultContent = response.Content.ReadAsStringAsync().Result;

            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                var jResult = JsonConvert.DeserializeObject<JObject[]>(resultContent);
                JObject first = jResult.FirstOrDefault();
                result = first.GetValue("new_id").ToString();
            }
            else
            {
                throw new Exception(JsonConvert.DeserializeObject<string>(resultContent));
            }

            return result;
        }

        public async Task<string> UpdateInventoryAsync(Inventory item)
        {
            string result = string.Empty;

            _httpClient.DefaultRequestHeaders.Clear();
            _httpClient.DefaultRequestHeaders.TryAddWithoutValidation("Authorization", string.Format("Bearer {0}", Settings.Token));

            var data = JsonConvert.SerializeObject(JsonConvert.SerializeObject(item));
            StringContent content = new StringContent(data, Encoding.UTF8, "application/json");
            var response = await _httpClient.PutAsync(string.Format(RestUrlRUD, Settings.Server, "inventory", item.inventory_id), content);

            string resultContent = response.Content.ReadAsStringAsync().Result;

            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                result = resultContent;
            }
            else
            {
                throw new Exception(JsonConvert.DeserializeObject<string>(resultContent));
            }

            return result;
        }

        public async Task<List<CodeValueLookup>> GetCodeValueLookupList(DateTime modifiedDate)
        {
            List<CodeValueLookup> result = null;

            _httpClient.DefaultRequestHeaders.Clear();
            _httpClient.DefaultRequestHeaders.TryAddWithoutValidation("Authorization", string.Format("Bearer {0}", Settings.Token));

            string URL = @"http://{0}/GringlobalService/WCFService.svc/getdata/{1}?parameters={2}";
            URL = string.Format(URL, Settings.Server, "mob_code_value_lookup", ":createddate;:modifieddate");
            //URL = @"http://192.168.137.1/GringlobalService/WCFService.svc/getdata/mob_code_value_lookup?parameters=:createddate;:modifieddate";
            //URL = @"http://192.168.137.1/GringlobalService/WCFService.svc/getdata/mob_code_value_by_groupname?parameters=:groupname=ACCESSION_NAME_TYPE";
            var response = await _httpClient.GetAsync(URL);

            string resultContent = response.Content.ReadAsStringAsync().Result;

            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                result = JsonConvert.DeserializeObject<List<CodeValueLookup>>(resultContent);
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
    }

    public interface RestApi<T, in Tkey> where T : class
    {

    }

    public class RestClass<T, Tkey> where T : class
    {
        readonly HttpClient _client;
        private const string InventoryViabilityListUrl = "http://{0}/GrinGlobalService/WCFService.svc/getdata/get_inventory_viability_list";

        public RestClass(HttpClient client)
        {
            _client = client;
        }

        public T Entity { get; set; }

        public async Task<T> CreateAsync(Tkey id)
        {
            return Entity;
        }

        public async Task<T> ReadAsync(Tkey id)
        {
            return Entity;
        }

        public async Task<T> UpdateAsync(Tkey id)
        {
            return Entity;
        }

        public async Task<T> DeleteAsync(Tkey id)
        {
            return Entity;
        }
    }
}

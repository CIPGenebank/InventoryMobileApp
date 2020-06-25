using ImTools;
using InventoryApp.Helpers;
using InventoryApp.Interfaces;
using InventoryApp.Models;
using InventoryApp.Models.Database;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using ZXing;

namespace InventoryApp.Services
{
    public class RestClient
    {
        // Android Emulator 10.0.2.2
        private const string LoginEndPoint = "http://{0}/GringlobalService/WCFService.svc/login";
        private const string SearchEndPoint = "http://{0}/GringlobalService/WCFService.svc/search/{1}";
        private const string SearchKeysEndPoint = "http://{0}/GringlobalService/WCFService.svc/searchkeys/{1}";

        private const string InventoryEndPoint = "http://{0}/GringlobalService/WCFService.svc/rest/inventory/{1}";
        private const string InventoryCreateEndPoint = "http://{0}/GringlobalService/WCFService.svc/rest/inventory";

        private const string InventoryActionEndPoint = "http://{0}/GringlobalService/WCFService.svc/rest/inventory_action";

        private const string GetDataEndPoint = "http://{0}/GringlobalService/WCFService.svc/getdata/{1}?parameters={2}";
        private const string PrintEndPoint = "http://{0}/GringlobalService/WCFService.svc/print?printURI={1}&printConnectionType={2}";

        private const string RestUrlRUD = "http://{0}/GringlobalService/WCFService.svc/rest/{1}/{2}";

        private readonly HttpClient _httpClient = new HttpClient();

        public RestClient()
        {

        }

        public async Task<Login> Login(string username, string password, string server)
        {

            var data = JsonConvert.SerializeObject(new Credential { Username = username, Password = password });
            var content = new StringContent(data, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync(string.Format(LoginEndPoint, server), content);

            string resultContent = response.Content.ReadAsStringAsync().Result;

            //System.Diagnostics.Debug.WriteLine(resultContent);

            Login resp = JsonConvert.DeserializeObject<Login>(resultContent);

            resp.user_name = username;
            resp.login_token = resp.Token;

            return resp;
        }

        public async Task<List<int>> SearchLookup(string dataview, string searchOperator, string searchText, int limit = 0)
        {
            List<int> result = new List<int>();

            _httpClient.DefaultRequestHeaders.Clear();
            _httpClient.DefaultRequestHeaders.TryAddWithoutValidation("Authorization", string.Format("Bearer {0}", Settings.UserToken));

            string URL = string.Format(GetDataEndPoint, Settings.Server, dataview, ":operator=" + searchOperator + ";:searchtext=" + searchText) + "&limit=" + limit;
            var response = await _httpClient.GetAsync(URL);

            string resultContent = response.Content.ReadAsStringAsync().Result;
            if (response != null && response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                foreach (var item in Newtonsoft.Json.Linq.JArray.Parse(resultContent))
                {
                    result.Add((int)item["value_member"]);
                }
            }
            else
            {
                throw new Exception(resultContent);
            }

            return result;
        }
        public async Task<List<InventoryThumbnail>> Search(string query, string dataview, string resolver = "accession", int limit = 0)
        {
            List<InventoryThumbnail> result = null;

            var data = JsonConvert.SerializeObject(query);

            _httpClient.DefaultRequestHeaders.Clear();
            //_httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            //_httpClient.DefaultRequestHeaders.AcceptEncoding.Add(new StringWithQualityHeaderValue("utf-8"));
            _httpClient.DefaultRequestHeaders.TryAddWithoutValidation("Authorization", string.Format("Bearer {0}", Settings.UserToken));

            StringContent content = new StringContent(data, Encoding.UTF8, "application/json");
            string URL = string.Format(SearchEndPoint, Settings.Server, resolver) + "?dataview=get_mob_inventory_thumbnail" + "&limit=" + limit;
            var response = await _httpClient.PostAsync(URL, content);

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
        public async Task<List<int>> SearchKeys(string query, string resolver, int limit = 0)
        {
            List<int> result;

            var data = JsonConvert.SerializeObject(query);

            _httpClient.DefaultRequestHeaders.Clear();
            _httpClient.DefaultRequestHeaders.TryAddWithoutValidation("Authorization", string.Format("Bearer {0}", Settings.UserToken));

            StringContent content = new StringContent(data, Encoding.UTF8, "application/json");
            string URL = string.Format(SearchKeysEndPoint, Settings.Server, resolver) + "?limit=" + limit;
            var response = await _httpClient.PostAsync(URL, content);

            string resultContent = response.Content.ReadAsStringAsync().Result;

            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                result = JsonConvert.DeserializeObject<List<int>>(resultContent);
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
            _httpClient.DefaultRequestHeaders.TryAddWithoutValidation("Authorization", string.Format("Bearer {0}", Settings.UserToken));

            StringContent content = new StringContent(data, Encoding.UTF8, "application/json");
            string URL = string.Format(SearchEndPoint, Settings.Server, resolver) + "?dataview=get_mob_accession_thumbnail";
            var response = await _httpClient.PostAsync(URL, content);

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
            _httpClient.DefaultRequestHeaders.TryAddWithoutValidation("Authorization", string.Format("Bearer {0}", Settings.UserToken));
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
            _httpClient.DefaultRequestHeaders.TryAddWithoutValidation("Authorization", string.Format("Bearer {0}", Settings.UserToken));
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
            _httpClient.DefaultRequestHeaders.TryAddWithoutValidation("Authorization", string.Format("Bearer {0}", Settings.UserToken));

            var response = await _httpClient.GetAsync(string.Format(InventoryEndPoint, Settings.Server, inventoryId));

            string resultContent = response.Content.ReadAsStringAsync().Result;
            result = JsonConvert.DeserializeObject<Inventory>(resultContent);
            return result;
        }

        public async Task<List<CooperatorGroup>> GetWorkGroups(int cooperatorId)
        {
            List<CooperatorGroup> result = new List<CooperatorGroup>();

            _httpClient.DefaultRequestHeaders.Clear();
            _httpClient.DefaultRequestHeaders.TryAddWithoutValidation("Authorization", string.Format("Bearer {0}", Settings.UserToken));

            string URL = string.Format(GetDataEndPoint, Settings.Server, "get_mob_workgroup_by_cooperator", System.Net.WebUtility.UrlEncode(":cooperatorid=" + cooperatorId));
            var response = await _httpClient.GetAsync(URL);

            string resultContent = response.Content.ReadAsStringAsync().Result;
            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                result = JsonConvert.DeserializeObject<List<CooperatorGroup>>(resultContent);
                foreach (var workgroup in result)
                {
                    if (!string.IsNullOrEmpty(workgroup.group_tag))
                    {
                        var tagValues = workgroup.group_tag.Split(new char[] { ';' }, StringSplitOptions.None);
                        if (tagValues.Length > 1)
                            workgroup.inv_maint_policy_ids = tagValues[1];
                        if (tagValues.Length > 2)
                            workgroup.inventory_dataview = tagValues[2];
                        if (tagValues.Length > 3)
                            workgroup.inventory_thumbnail_dataview = tagValues[3];
                    }
                }
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
            _httpClient.DefaultRequestHeaders.TryAddWithoutValidation("Authorization", string.Format("Bearer {0}", Settings.UserToken));

            //System.Net.WebUtility.UrlEncode(":inventorymaintpolicyid=" + inventoryMaintPolicyId)
            string URL = string.Format(GetDataEndPoint, Settings.Server, "get_mob_locations_by_workgroup", "");
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

        public async Task<List<string>> GetAllLocation1List()
        {
            List<string> result = new List<string>();

            _httpClient.DefaultRequestHeaders.Clear();
            _httpClient.DefaultRequestHeaders.TryAddWithoutValidation("Authorization", string.Format("Bearer {0}", Settings.UserToken));

            //System.Net.WebUtility.UrlEncode(":inventorymaintpolicyid=" + inventoryMaintPolicyId)
            string URL = string.Format(GetDataEndPoint, Settings.Server, "get_mob_inventory_storage_location_part1", "");
            var response = await _httpClient.GetAsync(URL);

            string resultContent = response.Content.ReadAsStringAsync().Result;
            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                result.Add("");
                foreach (var item in Newtonsoft.Json.Linq.JArray.Parse(resultContent))
                {
                    result.Add(item["storage_location_part1"].ToString());
                }
                //result = JsonConvert.DeserializeObject<List<Location>>(resultContent); storage_location_part1
            }
            else
            {
                throw new Exception(resultContent);
            }

            return result;
        }
        public async Task<List<string>> GetAllLocation2List()
        {
            List<string> result = new List<string>();

            _httpClient.DefaultRequestHeaders.Clear();
            _httpClient.DefaultRequestHeaders.TryAddWithoutValidation("Authorization", string.Format("Bearer {0}", Settings.UserToken));

            //System.Net.WebUtility.UrlEncode(":inventorymaintpolicyid=" + inventoryMaintPolicyId)
            string URL = string.Format(GetDataEndPoint, Settings.Server, "get_mob_inventory_storage_location_part2", "");
            var response = await _httpClient.GetAsync(URL);

            string resultContent = response.Content.ReadAsStringAsync().Result;
            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                result.Add("");
                foreach (var item in Newtonsoft.Json.Linq.JArray.Parse(resultContent))
                {
                    result.Add(item["storage_location_part2"].ToString());
                }
            }
            else
            {
                throw new Exception(resultContent);
            }

            return result;
        }

        public async Task<Dictionary<string, string>> GetAppResources(string formName, int sysLangId)
        {
            Dictionary<string, string> result = new Dictionary<string, string>();

            _httpClient.DefaultRequestHeaders.Clear();
            _httpClient.DefaultRequestHeaders.TryAddWithoutValidation("Authorization", string.Format("Bearer {0}", Settings.UserToken));
            if (!string.IsNullOrEmpty(Settings.UserToken) && !string.IsNullOrEmpty(Settings.Server))
            {
                string URL = string.Format(GetDataEndPoint, Settings.Server, "get_mob_app_resource_lookup_by_form_name", ":formname=" + formName + ";:syslangid=" + sysLangId);
                var response = await _httpClient.GetAsync(URL);

                string resultContent = response.Content.ReadAsStringAsync().Result;
                if (response != null && response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    foreach (var item in Newtonsoft.Json.Linq.JArray.Parse(resultContent))
                    {
                        result.Add((string)item["value_member"], (string)item["display_member"]);
                    }
                }
                else
                {
                    throw new Exception(resultContent);
                }
            }
            return result;
        }

        public async Task<string> UpdateInventory(InventoryThumbnail inventory)
        {
            string result = string.Empty;

            var data = JsonConvert.SerializeObject(JsonConvert.SerializeObject(inventory));
            System.Diagnostics.Debug.WriteLine(JsonConvert.SerializeObject(inventory, Formatting.Indented));

            _httpClient.DefaultRequestHeaders.Clear();
            _httpClient.DefaultRequestHeaders.TryAddWithoutValidation("Authorization", string.Format("Bearer {0}", Settings.UserToken));
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

        public async Task<string> UpdateInventory(Inventory inventory)
        {
            string result = string.Empty;

            _httpClient.DefaultRequestHeaders.Clear();
            _httpClient.DefaultRequestHeaders.TryAddWithoutValidation("Authorization", string.Format("Bearer {0}", Settings.UserToken));

            var data = JsonConvert.SerializeObject(JsonConvert.SerializeObject(inventory));
            StringContent content = new StringContent(data, Encoding.UTF8, "application/json");
            var response = await _httpClient.PutAsync(string.Format(RestUrlRUD, Settings.Server, "inventory", inventory.inventory_id), content);

            string resultContent = response.Content.ReadAsStringAsync().Result;

            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                result = resultContent;
            }
            else
            {
                try
                {
                    throw new Exception(JsonConvert.DeserializeObject<string>(resultContent));
                }
                catch
                {
                    throw new Exception(resultContent);
                }
            }

            return result;
        }

        public async Task<List<SearchFilter>> GetSearchFilterList()
        {
            List<SearchFilter> result = new List<SearchFilter>();

            _httpClient.DefaultRequestHeaders.Clear();
            _httpClient.DefaultRequestHeaders.TryAddWithoutValidation("Authorization", string.Format("Bearer {0}", Settings.UserToken));

            string URL = string.Format(GetDataEndPoint, Settings.Server, "get_mob_search_inventories_filters", "");
            var response = await _httpClient.GetAsync(URL);

            string resultContent = response.Content.ReadAsStringAsync().Result;
            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                result = JsonConvert.DeserializeObject<List<SearchFilter>>(resultContent);
            }
            else
            {
                throw new Exception(resultContent);
            }

            return result;
        }
        public async Task<List<Lookup>> GetMethodLookupList()
        {
            List<Lookup> result = new List<Lookup>();

            _httpClient.DefaultRequestHeaders.Clear();
            _httpClient.DefaultRequestHeaders.TryAddWithoutValidation("Authorization", string.Format("Bearer {0}", Settings.UserToken));

            string URL = string.Format(GetDataEndPoint, Settings.Server, "get_mob_method_lookup", "");
            var response = await _httpClient.GetAsync(URL);

            string resultContent = response.Content.ReadAsStringAsync().Result;
            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                result = JsonConvert.DeserializeObject<List<Lookup>>(resultContent);
            }
            else
            {
                throw new Exception(resultContent);
            }

            return result;
        }

        public async Task<List<ILookup>> GetAccessionLookUpListByAccessionNumber(string accessionNumber)
        {
            List<ILookup> result = new List<ILookup>();

            _httpClient.DefaultRequestHeaders.Clear();
            _httpClient.DefaultRequestHeaders.TryAddWithoutValidation("Authorization", string.Format("Bearer {0}", Settings.UserToken));

            string URL = string.Format(GetDataEndPoint, Settings.Server, "get_mob_accession_lookup_by_accession_number", System.Net.WebUtility.UrlEncode(":accessionnumber=" + accessionNumber));
            var response = await _httpClient.GetAsync(URL);

            string resultContent = response.Content.ReadAsStringAsync().Result;
            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                foreach (AccessionLookup item in JsonConvert.DeserializeObject<List<AccessionLookup>>(resultContent))
                {
                    result.Add((ILookup)item);
                }
            }
            else
            {
                throw new Exception(resultContent);
            }

            return result;
        }

        public async Task<List<ILookup>> GetInventoryMaintPolicyLookupByMaintenanceName(string maintenanceName)
        {
            List<ILookup> result = new List<ILookup>();

            _httpClient.DefaultRequestHeaders.Clear();
            _httpClient.DefaultRequestHeaders.TryAddWithoutValidation("Authorization", string.Format("Bearer {0}", Settings.UserToken));

            string URL = string.Format(GetDataEndPoint, Settings.Server, "get_mob_inventory_maint_policy_lookup_by_maintenance_name", System.Net.WebUtility.UrlEncode(":maintenancename=" + maintenanceName));
            var response = await _httpClient.GetAsync(URL);

            string resultContent = response.Content.ReadAsStringAsync().Result;
            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                foreach (AccessionLookup item in JsonConvert.DeserializeObject<List<AccessionLookup>>(resultContent))
                {
                    result.Add((ILookup)item);
                }
            }
            else
            {
                throw new Exception(resultContent);
            }

            return result;
        }

        public async Task<string> GetCodeValueDisplayName(string groupName, string value)
        {
            string result = string.Empty;
            _httpClient.DefaultRequestHeaders.Clear();
            _httpClient.DefaultRequestHeaders.TryAddWithoutValidation("Authorization", string.Format("Bearer {0}", Settings.UserToken));

            string URL = string.Format(GetDataEndPoint, Settings.Server, "get_mob_code_value_display_member", System.Net.WebUtility.UrlEncode(":groupname=" + groupName + ";:value=" + value));
            var response = await _httpClient.GetAsync(URL);

            string resultContent = response.Content.ReadAsStringAsync().Result;
            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                var array = JsonConvert.DeserializeObject<List<CodeValueLookup>>(resultContent);
                if (array.Count > 0)
                {
                    result = array[0].DisplayMember;
                }
                else
                    result = "Not found";
            }
            else
            {
                throw new Exception(resultContent);
            }

            return result;
        }

        public async Task<string> GetLookupByValueMember(string lookupName, int valueMember)
        {
            string result = null;

            _httpClient.DefaultRequestHeaders.Clear();
            _httpClient.DefaultRequestHeaders.TryAddWithoutValidation("Authorization", string.Format("Bearer {0}", Settings.UserToken));

            string URL = string.Format(GetDataEndPoint, Settings.Server, "get_mob_" + lookupName + "_by_value_member", System.Net.WebUtility.UrlEncode(":valuemember=" + valueMember));
            var response = await _httpClient.GetAsync(URL);

            string resultContent = response.Content.ReadAsStringAsync().Result;
            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                var array = JsonConvert.DeserializeObject<List<AccessionLookup>>(resultContent);
                if (array.Count > 0)
                {
                    result = array[0].DisplayMember;
                }
                else
                    result = "Not found";
            }
            else
            {
                throw new Exception(resultContent);
            }

            return result;
        }

        public async Task<List<ILookup>> GetCodeValueByGroupName(string groupName)
        {
            List<ILookup> result = new List<ILookup>();

            _httpClient.DefaultRequestHeaders.Clear();
            _httpClient.DefaultRequestHeaders.TryAddWithoutValidation("Authorization", string.Format("Bearer {0}", Settings.UserToken));

            string URL = string.Format(GetDataEndPoint, Settings.Server, "get_mob_code_value_by_groupname", System.Net.WebUtility.UrlEncode(":groupname=" + groupName));
            var response = await _httpClient.GetAsync(URL);

            string resultContent = response.Content.ReadAsStringAsync().Result;
            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                foreach (CodeValueLookup item in JsonConvert.DeserializeObject<List<CodeValueLookup>>(resultContent))
                {
                    result.Add((ILookup)item);
                }
            }
            else
            {
                throw new Exception(resultContent);
            }

            return result;
        }

        public async Task<List<Printer>> GetPrinterList()
        {
            List<Printer> result = new List<Printer>();

            _httpClient.DefaultRequestHeaders.Clear();
            _httpClient.DefaultRequestHeaders.TryAddWithoutValidation("Authorization", string.Format("Bearer {0}", Settings.UserToken));

            string URL = string.Format(GetDataEndPoint, Settings.Server, "get_mob_printer", "");
            var response = await _httpClient.GetAsync(URL);

            string resultContent = response.Content.ReadAsStringAsync().Result;
            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                result = JsonConvert.DeserializeObject<List<Printer>>(resultContent);
            }
            else
            {
                throw new Exception(resultContent);
            }

            return result;
        }

        public async Task<List<LabelTemplate>> GetLabelTemplateList()
        {
            List<LabelTemplate> result = new List<LabelTemplate>();

            _httpClient.DefaultRequestHeaders.Clear();
            _httpClient.DefaultRequestHeaders.TryAddWithoutValidation("Authorization", string.Format("Bearer {0}", Settings.UserToken));

            string URL = string.Format(GetDataEndPoint, Settings.Server, "get_mob_app_user_gui_setting",
                string.Format(":cooperatorid={0};:appname=GRINGlobalClientCuratorTool;:resourcename=LabelTemplateList", Settings.UserCooperatorId));
            var response = await _httpClient.GetAsync(URL);

            string resultContent = response.Content.ReadAsStringAsync().Result;
            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                var appUserSettingsList = JsonConvert.DeserializeObject<List<AppUserSetting>>(resultContent);
                if (appUserSettingsList != null)
                {
                    foreach (var appUserSetting in appUserSettingsList)
                    {
                        var labelTemplate = JsonConvert.DeserializeObject<LabelTemplate>(appUserSetting.resource_value);
                        result.Add(labelTemplate);
                    }
                }
            }
            else
            {
                throw new Exception(resultContent);
            }

            return result;
        }

        public async Task<Newtonsoft.Json.Linq.JArray> GetDataview(string dataviewName, string parameters)
        {
            Newtonsoft.Json.Linq.JArray result = new Newtonsoft.Json.Linq.JArray();

            _httpClient.DefaultRequestHeaders.Clear();
            _httpClient.DefaultRequestHeaders.TryAddWithoutValidation("Authorization", string.Format("Bearer {0}", Settings.UserToken));
            if (!string.IsNullOrEmpty(Settings.UserToken) && !string.IsNullOrEmpty(Settings.Server))
            {
                string URL = string.Format(GetDataEndPoint, Settings.Server, dataviewName, parameters);
                var response = await _httpClient.GetAsync(URL);

                string resultContent = response.Content.ReadAsStringAsync().Result;
                if (response != null && response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    result = Newtonsoft.Json.Linq.JArray.Parse(resultContent);
                }
                else
                {
                    throw new Exception(resultContent);
                }
            }
            return result;
        }

        public async Task<string> Print(string printerURI, string printerConnectionType, string labelZPL)
        {
            string result = string.Empty;

            var data = JsonConvert.SerializeObject(labelZPL);

            _httpClient.DefaultRequestHeaders.Clear();
            _httpClient.DefaultRequestHeaders.TryAddWithoutValidation("Authorization", string.Format("Bearer {0}", Settings.UserToken));
            StringContent content = new StringContent(data, Encoding.UTF8, "application/json");

            string URL = string.Format(PrintEndPoint, Settings.Server, printerURI, printerConnectionType);
            var response = await _httpClient.PostAsync(URL, content);
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

        public async Task<List<InventoryAction>> GetInvitroActionList(string inventoryIds)
        {
            List<InventoryAction> result = new List<InventoryAction>();

            _httpClient.DefaultRequestHeaders.Clear();
            _httpClient.DefaultRequestHeaders.TryAddWithoutValidation("Authorization", string.Format("Bearer {0}", Settings.UserToken));

            string URL = string.Format(GetDataEndPoint, Settings.Server, "get_mob_inventory_action", System.Net.WebUtility.UrlEncode(":inventoryid=" + inventoryIds));
            var response = await _httpClient.GetAsync(URL);

            string resultContent = response.Content.ReadAsStringAsync().Result;
            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                result = JsonConvert.DeserializeObject<List<InventoryAction>>(resultContent);
            }
            else
            {
                throw new Exception(resultContent);
            }

            return result;
        }

        public async Task<List<DataviewColumn>> GetDataviewAtributeList(string dataviewname)
        {
            List<DataviewColumn> result = new List<DataviewColumn>();

            _httpClient.DefaultRequestHeaders.Clear();
            _httpClient.DefaultRequestHeaders.TryAddWithoutValidation("Authorization", string.Format("Bearer {0}", Settings.UserToken));

            string URL = string.Format(GetDataEndPoint, Settings.Server, "get_mob_dataview_schema", System.Net.WebUtility.UrlEncode(":dataviewname=" + dataviewname));
            var response = await _httpClient.GetAsync(URL);

            string resultContent = response.Content.ReadAsStringAsync().Result;
            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                result = JsonConvert.DeserializeObject<List<DataviewColumn>>(resultContent);
                if(result.Count == 0)
                {
                    throw new Exception(string.Format("dataview '{0}' is not defined", dataviewname));
                }
            }
            else
            {
                throw new Exception(resultContent);
            }
            /*********************************************/
            URL = string.Format(GetDataEndPoint, Settings.Server, "get_mob_dataview_schema_extension", ""/*System.Net.WebUtility.UrlEncode(":dataviewname=" + dataviewname)*/);
            response = await _httpClient.GetAsync(URL);
            resultContent = response.Content.ReadAsStringAsync().Result;
            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                var result2 = Newtonsoft.Json.Linq.JArray.Parse(resultContent);
                if (result2.Count > 0) 
                {
                    foreach (var dvField in result)
                    {
                        if (dvField.gui_hint.Equals("SMALL_SINGLE_SELECT_CONTROL"))
                        {
                            var fieldExt = result2.FindFirst(x => x["dataview_name"].ToString().Equals(dataviewname) && x["dataview_field_name"].ToString().Equals(dvField.field_name));
                            if (fieldExt != null)
                            {
                                dvField.gui_filter = (string) fieldExt["gui_filter"];
                            }
                        }
                    }
                }
            }
            else
            {
                throw new Exception(resultContent);
            }
            /*********************************************/
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

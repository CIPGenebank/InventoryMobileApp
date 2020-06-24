using ImTools;
using InventoryApp.Helpers;
using InventoryApp.Models;
using InventoryApp.Services;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Navigation;
using Prism.Services;
using Prism.Services.Dialogs;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using Xamarin.Forms;

namespace InventoryApp.ViewModels
{
    public class SearchInventoriesPageViewModel : ViewModelBase
    {
        IPageDialogService _pageDialogService { get; }

        private List<SearchFilter> _defaultSearchFilterList;

        //private ObservableCollection<InventoryThumbnail> _inventories;
        private readonly RestClient _restClient;

        #region Properties
        private List<CooperatorGroup> _listWorkGroup;
        public List<CooperatorGroup> ListWorkgroup
        {
            get { return _listWorkGroup; }
            set { SetProperty(ref _listWorkGroup, value); }
        }
        private List<SearchFilter> _listFilters;
        public List<SearchFilter> ListFilters
        {
            get { return _listFilters; }
            set { SetProperty(ref _listFilters, value); }
        }
        private List<string> _listLocation1;
        public List<string> ListLocation1
        {
            get { return _listLocation1; }
            set { SetProperty(ref _listLocation1, value); }
        }

        private string _searchTextList;
        public string SearchTextList
        {
            get { return _searchTextList; }
            set { SetProperty(ref _searchTextList, value); }
        }

        private string _searchText;
        public string SearchText
        {
            get { return _searchText; }
            set { SetProperty(ref _searchText, value); }
        }

        private SearchFilter _filter;
        public SearchFilter Filter
        {
            get { return _filter; }
            set { SetProperty(ref _filter, value); }
        }

        private string _location1;
        public string Location1
        {
            get { return _location1; }
            set { SetProperty(ref _location1, value); }
        }

        private CooperatorGroup _workgroup;
        public CooperatorGroup Workgroup
        {
            get { return _workgroup; }
            set { SetProperty(ref _workgroup, value); }
        }

        private List<CodeValueLookup> _searchOperatorList;
        public List<CodeValueLookup> SearchOperatorList
        {
            get { return _searchOperatorList; }
            set { SetProperty(ref _searchOperatorList, value); }
        }
        private CodeValueLookup _searchOperator;
        public CodeValueLookup SearchOperator
        {
            get { return _searchOperator; }
            set { SetProperty(ref _searchOperator, value); }
        }

        private string _listId;
        public string ListId
        {
            get { return _listId; }
            set
            {
                SetProperty(ref _listId, value);
                /*
                if (_listId[_listId.Length - 1] == '\n') {
                    string[] lines = _listId.Trim().Split(new[] { "\n" }, StringSplitOptions.None);
                    UniqueList = lines.Distinct().ToArray();
                    Count = UniqueList.Count();
                }
                */
            }
        }
        private ObservableCollection<string> _uniqueList;
        public ObservableCollection<string> UniqueList
        {
            get { return _uniqueList; }
            set { SetProperty(ref _uniqueList, value); }
        }

        private int _count;
        public int Count
        {
            get { return _count; }
            set { SetProperty(ref _count, value); }
        }

        private bool _isMultiline;
        public bool IsMultiline
        {
            get { return _isMultiline; }
            set { SetProperty(ref _isMultiline, value, () => RaisePropertyChanged(nameof(IsNotMultiline))); }
        }
        public bool IsNotMultiline
        {
            get { return !IsMultiline; }
        }
        private string _selectedItem;
        public string SelectedItem
        {
            get { return _selectedItem; }
            set { SetProperty(ref _selectedItem, value); }
        }

        private string _maxResults;
        public string MaxResults
        {
            get { return _maxResults; }
            set { SetProperty(ref _maxResults, value); }
        }
        #endregion

        public SearchInventoriesPageViewModel(INavigationService navigationService, IPageDialogService pageDialogService) : base(navigationService)
        {
            Title = "Search Inventories";

            _pageDialogService = pageDialogService;
            _restClient = new RestClient();

            FindCommand = new DelegateCommand(OnFindCommandExecuted);
            ScanCommand = new DelegateCommand(OnScanCommandExecuted);

            ListWorkGroupChangedCommand = new DelegateCommand(OnListWorkGroupChangedCommandExecuted);
            FilterListChangedCommand = new DelegateCommand(OnFilterListChangedCommand);

            SearchCommand = new DelegateCommand(OnSearchCommandExecuted);
            IsMultiline = false;

            IsMultilineChangedCommand = new DelegateCommand(OnIsMultilineChangedCommand);
            //TextChangedCommand = new DelegateCommand(OnTextChangedCommandExecuted);
            ItemTappedCommand = new DelegateCommand(OnItemTappedCommandExecuted);

            _uniqueList = new ObservableCollection<string>();
            MaxResults = "100";
        }
        public DelegateCommand FilterListChangedCommand { get; }
        private async void OnFilterListChangedCommand()
        {
            try
            {
                if (Filter == null) return;

                var baseSearchOperatorList = new List<CodeValueLookup>() {
                        new CodeValueLookup { value_member="equals", display_member="Equals"},
                        new CodeValueLookup { value_member="contains", display_member="Contains"},
                        new CodeValueLookup { value_member="startswith", display_member="Starts with"},
                        new CodeValueLookup { value_member="endswith", display_member="Ends with"},
                        new CodeValueLookup { value_member="lessthan", display_member="Less than"},
                        new CodeValueLookup { value_member="morethan", display_member="More than"}
                        };

                if (!string.IsNullOrEmpty(Filter.filter_operators))
                {
                    var tempOperators = Filter.filter_operators.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                    SearchOperatorList = baseSearchOperatorList.Where(o => tempOperators.Contains(o.value_member)).ToList();
                    if (SearchOperatorList != null && SearchOperatorList.Count > 0)
                    {
                        SearchOperator = SearchOperatorList[0];
                    }
                }
                else
                {
                    SearchOperatorList = baseSearchOperatorList;
                    SearchOperator = SearchOperatorList[0];
                }
            }
            catch (Exception ex)
            {
                await _pageDialogService.DisplayAlertAsync("Error", ex.Message, "OK");
            }
        }

        public DelegateCommand ListWorkGroupChangedCommand { get; }
        private async void OnListWorkGroupChangedCommandExecuted()
        {
            try
            {
                //List<Location> locations = await _restClient.GetLocations(Settings.InventoryMaintPolicyId.ToString());
                //ListLocation1 = locations.Select(l => l.storage_location_part1).Distinct().ToList();

                //Location1 = Settings.Location1;
                
            }
            catch (Exception e)
            {
                await _pageDialogService.DisplayAlertAsync("Error", e.Message, "OK");
            }
        }

        public DelegateCommand TextChangedCommand { get; }
        private async void OnTextChangedCommandExecuted()
        {
            try
            {
                if (!string.IsNullOrEmpty(_listId) && _listId[_listId.Length - 1] == '\n')
                {
                    string[] lines = _listId.Trim().Split(new[] { "\n" }, StringSplitOptions.None);
                    if (lines.Length > 0 && UniqueList.FirstOrDefault(x => x.Equals(lines[0])) == null)
                    {
                        UniqueList.Add(lines[0]);
                        Count = UniqueList.Count;
                    }
                    ListId = string.Empty;
                }
                //System.Diagnostics.Debug.WriteLine(ListId);
            }
            catch (Exception e)
            {
                await _pageDialogService.DisplayAlertAsync("Error", e.Message, "OK");
            }
        }

        public DelegateCommand IsMultilineChangedCommand { get; }
        private async void OnIsMultilineChangedCommand()
        {
            try
            {
                IsMultiline = !IsMultiline;

                //Set search filter list
                if (IsMultiline)
                    ListFilters = _defaultSearchFilterList.Where(f => f.filter_by_list.Equals("Y")).ToList();
                else
                    ListFilters = _defaultSearchFilterList;

                //Set search filter
                Filter = ListFilters.FirstOrDefault(f => Settings.Filter.Equals(f.filter_name));
                if (Filter == null)
                {
                    Filter = ListFilters[0];
                    //Settings.Filter = string.Empty;
                }
                //Set search operator list
                OnFilterListChangedCommand();
            }
            catch (Exception ex)
            {
                await _pageDialogService.DisplayAlertAsync("Error", ex.Message, "OK");
            }
        }
        public DelegateCommand ItemTappedCommand { get; }
        private async void OnItemTappedCommandExecuted()
        {
            try
            {
                if (!string.IsNullOrEmpty(SelectedItem))
                {
                    UniqueList.Remove(SelectedItem);
                    //UniqueList = _uniqueList;
                }
            }
            catch (Exception e)
            {
                await _pageDialogService.DisplayAlertAsync("Error", e.Message, "OK");
            }
        }

        public DelegateCommand SearchCommand { get; }
        private async void OnSearchCommandExecuted()
        {
            try
            {
                if (!IsMultiline)
                {
                    OnFindCommandExecuted();
                }
                else
                {
                    ListId += SearchText + "\n";
                    Xamarin.Forms.MessagingCenter.Send<SearchInventoriesPageViewModel>(this, "Focus");
                }
            }
            catch (Exception e)
            {
                await _pageDialogService.DisplayAlertAsync("Error", e.Message, "OK");
            }
        }

        private async void OnFindCommandExecuted()
        {
            /* Search lots and send back results */
            // _inventories = new ObservableCollection<Inventory>(InventoryFactory.GetInventories());

            //string[] lines = ListId.Trim().Split(new[] { "\r\n", "\r", "\n" },StringSplitOptions.None);
            //searchText = string.Join(",", lines.Distinct().ToArray());

            //Settings.Filter = Filter;

            string ggInventoryIdList = string.Empty;
            string LotIdList = string.Empty;
            try
            {
                string searchText = string.Empty;
                string[] searchTextArray = null;
                string query = string.Empty;

                if (!IsMultiline && string.IsNullOrEmpty(SearchText))
                    throw new Exception("Search text is empty");
                if (IsMultiline && string.IsNullOrEmpty(SearchTextList))
                    throw new Exception("Search text list is empty");
                if (Workgroup == null)
                    throw new Exception("Workgroup is empty");
                if (Filter == null)
                    throw new Exception("Filter is empty");
                if (SearchOperator == null)
                    throw new Exception("Search operator is empty");
                if (string.IsNullOrEmpty(Filter.filter_type))
                    throw new Exception("Filter type is empty.\nReview dataview in AdminTool");
                if (string.IsNullOrEmpty(Filter.filter_is_pre_query))
                    throw new Exception("Filter is_pre_query is empty.\nReview dataview in AdminTool");

                Settings.Filter = Filter.filter_name;
                if (IsMultiline)
                {
                    searchTextArray = SearchTextList.Split(new[] { "\n", "\r" }, StringSplitOptions.RemoveEmptyEntries);
                    if (searchTextArray.Length == 0)
                        throw new Exception("Search text list is empty");
                }
                else
                {
                    Settings.SearchText = SearchText;
                    searchText = SearchText.Trim();
                }

                int limit = 0;
                if (!int.TryParse(MaxResults, out limit))
                {
                    MaxResults = "0";
                }
                if (limit > 2000)
                {
                    throw new Exception("Max results cannot be greater than 2000");
                }

                string querySearchOperator = string.Empty;
                switch (SearchOperator.value_member)
                {
                    case "equals":
                        querySearchOperator = " = '{0}'";
                        break;
                    case "contains":
                        querySearchOperator = " like '%{0}%'";
                        break;
                    case "startswith":
                        querySearchOperator = " like '{0}%'";
                        break;
                    case "endswith":
                        querySearchOperator = " like '%{0}'";
                        break;
                    case "lessthan":
                        querySearchOperator = " < {0}";
                        break;
                    case "morethan":
                        querySearchOperator = " > {0}";
                        break;
                    default:
                        throw new Exception("Search operator not supported");
                }

                List<int> preResultIds = null;
                List<int> resultIds = new List<int>();
                switch (Filter.filter_type)
                {
                    case "dataview":
                        if (string.IsNullOrEmpty(Filter.filter_source))
                            throw new Exception("Filter source is empty.\nReview dataview in AdminTool");
                        if (!IsMultiline)
                        {
                            preResultIds = await _restClient.SearchLookup(Filter.filter_source, SearchOperator.value_member, searchText);
                            if (preResultIds != null && preResultIds.Count > 0)
                            {
                                searchText = string.Join(",", preResultIds);
                                query = "@inventory.accession_id IN ({0})";
                            }
                            else
                            {
                                await _pageDialogService.DisplayAlertAsync("Error", Filter.filter_name + " not found", "OK");
                                return;
                            }
                        }
                        else 
                        {
                            foreach (var text in searchTextArray)
                            {
                                var partialResultIds = await _restClient.SearchLookup(Filter.filter_source, SearchOperator.value_member, text);
                                if (partialResultIds != null && partialResultIds.Count() > 0)
                                    resultIds.AddRange(partialResultIds);
                            }
                            if (resultIds.Count == 0) 
                            {
                                await _pageDialogService.DisplayAlertAsync("Error", Filter.filter_name + " not found", "OK");
                                return;
                            }
                            query = "@inventory.accession_id IN ({0})";
                            searchText = string.Join(",", resultIds);
                        }
                        break;
                    case "query":
                        if (string.IsNullOrEmpty(Filter.filter_query))
                            throw new Exception("Filter query is empty.\nReview dataview in AdminTool");
                        if (!IsMultiline)
                        {
                            if (Filter.filter_is_pre_query.Equals("Y"))
                            {
                                if (string.IsNullOrEmpty(Filter.filter_pre_query_table))
                                    throw new Exception("Filter pre result table_name is empty.\nReview dataview in AdminTool");

                                query = string.Format(Filter.filter_query + querySearchOperator, searchText);
                                preResultIds = await _restClient.SearchKeys(query, Filter.filter_pre_query_table);
                                if (preResultIds != null && preResultIds.Count > 0)
                                {
                                    searchText = string.Join(",", preResultIds);
                                    query = "@inventory.accession_id IN ({0})";
                                }
                                else
                                {
                                    await _pageDialogService.DisplayAlertAsync("Error", Filter.filter_name + " not found", "OK");
                                    return;
                                }
                            }
                            else
                            {
                                query = Filter.filter_query + querySearchOperator;
                            }
                        }
                        else  //Multiline
                        {
                            query = Filter.filter_query + querySearchOperator;

                            if (Filter.filter_is_pre_query.Equals("Y"))
                            {
                                if (string.IsNullOrEmpty(Filter.filter_pre_query_table))
                                    throw new Exception("Filter pre result table_name is empty.\nReview dataview in AdminTool");

                                foreach (var text in searchTextArray)
                                {
                                    preResultIds = await _restClient.SearchKeys(string.Format(query, text), Filter.filter_pre_query_table);
                                    if (preResultIds != null && preResultIds.Count > 0)
                                    {
                                        searchText = string.Join(",", preResultIds);
                                        var finalQuery = "@inventory.accession_id IN ({0})";

                                        if (!string.IsNullOrEmpty(Location1))
                                            finalQuery += string.Format(" and @inventory.storage_location_part1 = '{0}'", Location1);
                                        /*if (Workgroup != null && Workgroup.group_owned_by.HasValue)
                                            finalQuery += string.Format(" and @inventory.owned_by = {0}", Workgroup.group_owned_by.Value);*/
                                        finalQuery += " and @inventory.form_type_code <> '**'";

                                        var partialResultIds = await _restClient.SearchKeys(string.Format(finalQuery, searchText), "inventory", 0);
                                        if (partialResultIds != null && partialResultIds.Count() > 0)
                                            resultIds.AddRange(partialResultIds);
                                    }
                                }
                                query = "@inventory.inventory_id in ({0})";
                                searchText = string.Join(",", resultIds);
                            }
                            else
                            {
                                if (!string.IsNullOrEmpty(Location1))
                                    query += string.Format(" and @inventory.storage_location_part1 = '{0}'", Location1);
                                /*if (Workgroup != null && Workgroup.group_owned_by.HasValue)
                                    query += string.Format(" and @inventory.owned_by = {0}", Workgroup.group_owned_by.Value);*/
                                query += " and @inventory.form_type_code <> '**'";

                                foreach (var text in searchTextArray)
                                {
                                    var partialResultIds = await _restClient.SearchKeys(string.Format(query, text), "inventory", 0);
                                    if (partialResultIds != null && partialResultIds.Count() > 0)
                                        resultIds.AddRange(partialResultIds);
                                }
                                query = "@inventory.inventory_id in ({0})";
                                searchText = string.Join(",", resultIds);
                            }
                        }
                        break;
                    /*case "lookup":
                        throw new Exception("Filter type not supported");
                        break;*/
                    default:
                        throw new Exception("Filter type not supported");
                }

                /*switch (Filter.filter_name)
                {
                    case "Accession Number":
                        SearchText = SearchText.Replace("CIP ", "");
                        int dotIndex = SearchText.IndexOf('.');
                        string accession_number_part2 = string.Empty;
                        string accession_number_part3 = string.Empty;
                        if (dotIndex > -1)
                        {
                            accession_number_part2 = searchText.Substring(0, dotIndex);
                            accession_number_part3 = searchText.Substring(dotIndex + 1, SearchText.Length - dotIndex - 1);
                            query = string.Format("@accession.accession_number_part2 = '{0}' and @accession.accession_number_part3 = '{1}'"
                                                , accession_number_part2, accession_number_part3);
                        }
                        else
                        {
                            accession_number_part2 = searchText;
                            query = string.Format("@accession.accession_number_part2 = '{0}' and @accession.accession_number_part3 < '0'", accession_number_part2);
                        }

                        preResult = await _restClient.SearchAccession(query, "", "accession");
                        if (preResult != null && preResult.Count > 0)
                        {
                            searchText = preResult.First().accession_id.ToString();
                            query = "@inventory.accession_id = {0}";
                        }
                        else
                        {
                            await _pageDialogService.DisplayAlertAsync("Error", "Accession number not found", "OK");
                            return;
                        }

                        break;
                    case "Inventory Id / Lot Id":
                        if (!IsMultiline)
                        {
                            if (searchText.StartsWith("gg"))
                            {
                                searchText = searchText.Replace("gg", "");
                                query = "@inventory.inventory_id = {0}";
                            }
                            else
                            {
                                query = "@inventory.inventory_number_part2 = {0}";
                            }
                        }
                        else
                        {
                            ggInventoryIdList = string.Join(",", UniqueList.Where(x => x.StartsWith("gg")).Select(y => y.Replace("gg", "")));
                            LotIdList = string.Join(",", UniqueList.Where(x => !x.StartsWith("gg")));

                            if (ggInventoryIdList != string.Empty && LotIdList != string.Empty)
                            {
                                query = string.Format("@inventory.inventory_id IN ({0}) OR @inventory.inventory_number_part2 IN ({1})", ggInventoryIdList, LotIdList);
                            }
                            else if (ggInventoryIdList == string.Empty && LotIdList == string.Empty)
                            {
                                await _pageDialogService.DisplayAlertAsync("Error", "Empty values", "OK");
                                return;
                            }
                            else if (ggInventoryIdList == string.Empty)
                            {
                                query = string.Format("@inventory.inventory_number_part2 IN ({0})", LotIdList);
                            }
                            else
                            {
                                query = string.Format("@inventory.inventory_id IN ({0})", ggInventoryIdList);
                            }
                            searchText = string.Empty;
                        }
                        break;
                    case "Storage location level 2":
                        query = "@inventory.storage_location_part2 like '%{0}%'";
                        break;
                    case "Storage location level 3":
                        query = "@inventory.storage_location_part3 like '%{0}%'";
                        break;
                    case "Accession Name":
                        query = string.Format("@accession_inv_name.category_code = 'CULTIVAR' and @accession_inv_name.plant_name = '{0}'", searchText);
                        preResult = await _restClient.SearchAccession(query, "", "accession");
                        if (preResult != null && preResult.Count > 0)
                        {
                            searchText = string.Join(",", preResult.Select(x => x.accession_id.ToString()).ToList());
                            query = "@inventory.accession_id IN ({0})";
                        }
                        else
                        {
                            await _pageDialogService.DisplayAlertAsync("Error", "Accession name not found", "OK");
                            return;
                        }
                        break;
                    case "Collecting number":
                        query = string.Format("@accession_inv_name.category_code = 'COLLECTOR' and @accession_inv_name.plant_name = '{0}'", searchText);
                        preResult = await _restClient.SearchAccession(query, "", "accession");
                        if (preResult != null && preResult.Count > 0)
                        {
                            searchText = preResult.First().accession_id.ToString();
                            query = "@inventory.accession_id = '{0}'";
                        }
                        else
                        {
                            await _pageDialogService.DisplayAlertAsync("Error", "Accession name not found", "OK");
                            return;
                        }
                        break;
                    case "Note":
                        query = "@inventory.note like '%{0}%'";
                        break;
                    case "Taxon Code":
                        query = "@taxonomy_species.alternate_name = '{0}'";
                        break;
                    case "Order request - Order Id":
                        query = "@order_request.order_request_id = '{0}'";
                        break;
                    case "Order request - Local number":
                        query = "@order_request.local_number = '%{0}%'";
                        break;
                }
                */
                if (!string.IsNullOrEmpty(Location1))
                {
                    query += string.Format(" and @inventory.storage_location_part1 = '{0}'", Location1);
                }
                // Support user in multiples workgroups
                if (Workgroup != null && !string.IsNullOrEmpty(Workgroup.inv_maint_policy_ids))
                {
                    query += string.Format(" and @inventory.inventory_maint_policy_id in ({0})", Workgroup.inv_maint_policy_ids);
                }
                query += " and @inventory.form_type_code <> '**'";

                List<InventoryThumbnail> result;
                if (!string.IsNullOrEmpty(searchText))
                {
                    result = await _restClient.Search(string.Format(query, searchText), "", "inventory", limit);
                }
                else
                {
                    result = await _restClient.Search(string.Format(query), "", "inventory", limit);
                }

                if (result != null)
                {
                    if (!Filter.Equals("Inventory Id / Lot Id") || IsMultiline)
                        await _pageDialogService.DisplayAlertAsync("Message", string.Format("Found {0} matches in the database.", result.Count), "OK");

                    var navigationParams = new NavigationParameters();
                    navigationParams.Add("InventoryThumbnailList", result);
                    await NavigationService.GoBackAsync(navigationParams);
                }
                else
                {
                    await _pageDialogService.DisplayAlertAsync("Message", "Sin resultados", "OK");
                }
            }
            catch (Exception e)
            {
                await _pageDialogService.DisplayAlertAsync("Error", e.Message, "OK");
            }
        }

        private async void OnScanCommandExecuted()
        {
            await NavigationService.NavigateAsync("ScanReaderPage");
        }

        public DelegateCommand FindCommand { get; }
        public DelegateCommand ScanCommand { get; }

        public override async void OnNavigatedTo(INavigationParameters parameters)
        {
            try
            {
                //Load All Filters
                if ( _defaultSearchFilterList == null)
                {
                    _defaultSearchFilterList = await _restClient.GetSearchFilterList();
                }
                //Load SearchFilters
                if (ListFilters == null && _defaultSearchFilterList != null) 
                {
                    if (IsMultiline)
                        ListFilters = _defaultSearchFilterList.Where(f => f.filter_by_list.Equals("Y")).ToList();
                    else
                        ListFilters = _defaultSearchFilterList;
                }
                if (ListFilters != null && Filter == null) 
                {
                    Filter = ListFilters.FirstOrDefault(f => Settings.Filter.Equals(f.filter_name));
                    if (Filter == null)
                    {
                        Settings.Filter = string.Empty;
                    }
                    else
                    {
                        var baseSearchOperatorList = new List<CodeValueLookup>() {
                        new CodeValueLookup { value_member="equals", display_member="Equals"},
                        new CodeValueLookup { value_member="contains", display_member="Contains"},
                        new CodeValueLookup { value_member="startswith", display_member="Starts with"},
                        new CodeValueLookup { value_member="endswith", display_member="Ends with"},
                        new CodeValueLookup { value_member="lessthan", display_member="Less than"},
                        new CodeValueLookup { value_member="morethan", display_member="More than"}
                        };

                        if (!string.IsNullOrEmpty(Filter.filter_operators))
                        {
                            var tempOperators = Filter.filter_operators.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                            SearchOperatorList = baseSearchOperatorList.Where(o => tempOperators.Contains(o.value_member)).ToList();
                            if (SearchOperatorList != null && SearchOperatorList.Count > 0)
                            {
                                SearchOperator = SearchOperatorList[0];
                            }
                        }
                        else
                        {
                            SearchOperatorList = baseSearchOperatorList;
                            SearchOperator = SearchOperatorList[0];
                        }
                    }
                }

                //Load workgroups
                if (ListWorkgroup == null)
                {
                    ListWorkgroup = await _restClient.GetWorkGroups(Settings.UserCooperatorId);
                    Workgroup = ListWorkgroup.FirstOrDefault(g => g.group_name.Equals(Settings.WorkgroupName));
                }
                //Load Location 1
                if (ListLocation1 == null)
                {
                    ListLocation1 = await _restClient.GetAllLocation1List();
                    Location1 = Settings.Location1;
                }

                if (string.IsNullOrEmpty(SearchText))
                    SearchText = Settings.SearchText;

            }
            catch (Exception ex)
            {
                await _pageDialogService.DisplayAlertAsync("Error", ex.Message, "OK");
            }
        }
    }
}

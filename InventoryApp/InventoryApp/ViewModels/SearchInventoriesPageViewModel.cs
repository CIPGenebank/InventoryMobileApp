using InventoryApp.Helpers;
using InventoryApp.Models;
using InventoryApp.Services;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Navigation;
using Prism.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace InventoryApp.ViewModels
{
    public class SearchInventoriesPageViewModel : ViewModelBase
    {
        IPageDialogService _pageDialogService { get; }

        private ObservableCollection<InventoryThumbnail> _inventories;
        private readonly RestClient _restClient;

        private List<CooperatorGroup> _listWorkGroup;
        public List<CooperatorGroup> ListWorkgroup
        {
            get { return _listWorkGroup; }
            set { SetProperty(ref _listWorkGroup, value); }
        }
        private List<string> _listFilters;
        public List<string> ListFilters
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

        private string _searchText;
        public string SearchText
        {
            get { return _searchText; }
            set { SetProperty(ref _searchText, value); }
        }

        private string _filter;
        public string Filter
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

        //public bool IsInventoryIdFilterSelected {
        //    get {
        //        if (Filter.Equals("Inventory Id / Lot Id"))
        //            return true;
        //        else
        //            return false;
        //    }
        //}
        private int _maxResults;
        public int MaxResults
        {
            get { return _maxResults; }
            set { SetProperty(ref _maxResults, value); }
        }

        public SearchInventoriesPageViewModel(INavigationService navigationService, IPageDialogService pageDialogService) : base(navigationService)
        {
            Title = "Search Inventories";

            _pageDialogService = pageDialogService;
            _restClient = new RestClient();

            _listFilters = CodeValueFactory.GetSearchInventoryFilters();
            //_listWorkGroups = CodeValueFactory.GetSearchInventoryActivities();
            //_locationList = CodeValueFactory.GetSearchInventoryLocations();

            FindCommand = new DelegateCommand(OnFindCommandExecuted);
            ScanCommand = new DelegateCommand(OnScanCommandExecuted);

            ListWorkGroupChangedCommand = new DelegateCommand(OnListWorkGroupChangedCommandExecuted);

            SearchCommand = new DelegateCommand(OnSearchCommandExecuted);
            IsMultiline = false;

            TextChangedCommand = new DelegateCommand(OnTextChangedCommandExecuted);
            ItemTappedCommand = new DelegateCommand(OnItemTappedCommandExecuted);

            _uniqueList = new ObservableCollection<string>();
            MaxResults = 200;
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

            Settings.SearchText = SearchText;
            Settings.Filter = Filter;

            string ggInventoryIdList = string.Empty;
            string LotIdList = string.Empty;
            try
            {
                string searchText = SearchText.Trim();

                string query = string.Empty;
                List<AccessionThumbnail> preResult;
                switch (Filter)
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
                if (!string.IsNullOrEmpty(Location1))
                {
                    query += string.Format(" and @inventory.storage_location_part1 = '{0}'", Location1);
                }
                if (Workgroup != null && Workgroup.group_owned_by.HasValue)
                {
                    query += string.Format(" and @inventory.owned_by = {0}", Workgroup.group_owned_by.Value);
                }
                query += " and @inventory.form_type_code <> '**'";

                List<InventoryThumbnail> result;
                if (!string.IsNullOrEmpty(searchText))
                {
                    result = await _restClient.Search(string.Format(query, searchText), "", "inventory");
                }
                else
                {
                    result = await _restClient.Search(string.Format(query), "", "inventory");
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
                //Load workgroups
                if (ListWorkgroup == null)
                {
                    ListWorkgroup = await _restClient.GetWorkGroups(Settings.CooperatorId);
                    Workgroup = ListWorkgroup.FirstOrDefault(g => g.group_name.Equals(Settings.WorkgroupName));
                }
                //Load Location 1
                if (ListLocation1 == null)
                {
                    ListLocation1 = await _restClient.GetAllLocation1List();
                    Location1 = Settings.Location1;
                }

                if (parameters.ContainsKey("title"))
                {
                    Title = (string)parameters["title"];
                }
                else if (parameters.ContainsKey("result"))
                {
                    //ZXing.Result result = (ZXing.Result)parameters["result"];
                    //SearchText = result.Text;
                }

                if (string.IsNullOrEmpty(Filter))
                    Filter = Settings.Filter;

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

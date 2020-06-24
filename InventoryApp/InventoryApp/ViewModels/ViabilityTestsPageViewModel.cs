using InventoryApp.Helpers;
using InventoryApp.Interfaces;
using InventoryApp.Models;
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
    public class ViabilityTestsPageViewModel : ViewModelBase
    {
        IPageDialogService _pageDialogService { get; }
        IRestService _restService { get; }

        private InventoryThumbnail _inventoryThumbnail;
        public InventoryThumbnail InventoryThumbnail
        {
            get { return _inventoryThumbnail; }
            set { SetProperty(ref _inventoryThumbnail, value); }
        }

        private string _searchCriteria;
        public string SearchCriteria
        {
            get { return _searchCriteria; }
            set { SetProperty(ref _searchCriteria, value); }
        }

        private string _searchText;
        public string SearchText
        {
            get { return _searchText; }
            set { SetProperty(ref _searchText, value); }
        }

        private WrappedSelection<InventoryViability> _selectedInventoryViability;
        public WrappedSelection<InventoryViability> SelectedInventoryViability
        {
            get { return _selectedInventoryViability; }
            set
            {
                SetProperty(ref _selectedInventoryViability, value
                  , raisePropertiesChanges //() =>  RaisePropertyChanged(nameof(IsSelected)) 
                );
            }
        }

        private void raisePropertiesChanges()
        {

            if (SelectedInventoryViability != null)
            {
                foreach (var item in _inventoryViabilityList.Where(x => x.IsSelected == true && x.Item.InventoryViabilityId != SelectedInventoryViability.Item.InventoryViabilityId))
                {
                    item.IsSelected = false;
                }
                SelectedInventoryViability.IsSelected = true;
            }
            else
            {
                foreach (var item in _inventoryViabilityList.Where(x => x.IsSelected == true))
                {
                    item.IsSelected = false;
                }
            }


            RaisePropertyChanged(nameof(IsSelected));
            RaisePropertyChanged(nameof(SelectedInventoryViabilityRule));
        }

        private ObservableCollection<WrappedSelection<InventoryViability>> _inventoryViabilityList;
        public ObservableCollection<WrappedSelection<InventoryViability>> InventoryViabilityList
        {
            get { return _inventoryViabilityList; }
            set { SetProperty(ref _inventoryViabilityList, value); }
        }

        private List<InventoryViabilityRule> _inventoryViabilityRuleList;
        public List<InventoryViabilityRule> InventoryViabilityRuleList
        {
            get { return _inventoryViabilityRuleList; }
            set { SetProperty(ref _inventoryViabilityRuleList, value); }
        }

        public bool IsSelected
        {
            get
            {
                if (_selectedInventoryViability == null) return false;
                return true;
            }
        }

        public InventoryViabilityRule SelectedInventoryViabilityRule
        {
            get
            {
                if (SelectedInventoryViability != null && InventoryViabilityRuleList != null)
                {
                    return InventoryViabilityRuleList.Find(x => x.InventoryViabilityRuleId == SelectedInventoryViability.Item.InventoryViabilityRuleId);
                }
                else
                {
                    return null;
                }
            }
        }

        public ViabilityTestsPageViewModel(INavigationService navigationService, IPageDialogService pageDialogService) : base(navigationService)
        {
            Title = "Viability Tests ";

            _pageDialogService = pageDialogService;
            _restService = new RestService();

            SearchCriteria = "Inventory Id";

            NewCommand = new DelegateCommand(OnNewCommandExecuted);
            SearchCommand = new DelegateCommand(OnSearchCommandExecuted);
            EvaluateCommand = new DelegateCommand(OnEvaluateCommandExecuted);
            EditCommand = new DelegateCommand(OnEditCommandExecuted);
            //ItemTappedCommand = new DelegateCommand(OnItemTappedCommandExecuted);
        }

        public DelegateCommand NewCommand { get; }
        private async void OnNewCommandExecuted()
        {
            var navigationParams = new NavigationParameters();
            navigationParams.Add("InventoryViabilityRuleList", InventoryViabilityRuleList);

            if (InventoryThumbnail == null)
            {
                await NavigationService.NavigateAsync("ViabilityTestPage", navigationParams);
            }
            else
            {
                await NavigationService.NavigateAsync("ViabilityTestPage?InventoryId=" + InventoryThumbnail.inventory_id, navigationParams);
            }
        }

        public DelegateCommand EditCommand { get; }
        private async void OnEditCommandExecuted()
        {
            if (SelectedInventoryViability == null)
            {
                await _pageDialogService.DisplayAlertAsync("Message", "Select a Inventory Viability", "OK");
            }
            else
            {
                var navigationParams = new NavigationParameters();
                navigationParams.Add("InventoryViability", SelectedInventoryViability.Item);
                navigationParams.Add("InventoryViabilityRuleList", InventoryViabilityRuleList);

                await NavigationService.NavigateAsync("ViabilityTestPage", navigationParams);
            }
        }

        public DelegateCommand SearchCommand { get; }
        private async void OnSearchCommandExecuted()
        {
            try
            {
                int SearchId;
                string query = "";
                SearchText = SearchText.Trim();
                if (SearchText.StartsWith("gg"))
                {
                    SearchId = int.Parse(SearchText.Remove(0, 2));
                    query = "@inventory.inventory_id = " + SearchId;
                }
                else
                {
                    SearchId = int.Parse(SearchText);
                    query = "@inventory.inventory_number_part2 = " + SearchId + " and @inventory.form_type_code != '**'";
                }

                InventoryThumbnail = null;
                SelectedInventoryViability = null;
                InventoryViabilityList = null;

                List<InventoryThumbnail> inventoryThumbnailListResult = await _restService.SearchInventoryThumbnailAsync(query, "get_mob_inventory_thumbnail", "inventory");
                if (inventoryThumbnailListResult == null || inventoryThumbnailListResult.Count == 0)
                {
                    await _pageDialogService.DisplayAlertAsync("Error", "Inventory not found", "OK");
                    return;
                }
                else
                {
                    InventoryThumbnail = inventoryThumbnailListResult.FirstOrDefault();
                }

                List<InventoryViability> inventoryViabilityList = await _restService.SearchInventoryViabilityAsync(
                    "@inventory_viability.inventory_id = " + InventoryThumbnail.inventory_id, null, "inventory_viability");
                if (inventoryViabilityList != null)
                {

                    InventoryViabilityList = new ObservableCollection<WrappedSelection<InventoryViability>>(
                        inventoryViabilityList.OrderByDescending(x => x.TestedDate)
                        .Select(item => new WrappedSelection<InventoryViability>
                        {
                            Item = item,
                            IsSelected = false
                        })
                    );
                    //InventoryViabilityList = new ObservableCollection<InventoryViability>(InventoryViabilityList.OrderByDescending(x => x.TestedDate));

                    //SelectedInventoryViability = InventoryViabilityList.FirstOrDefault();
                }
                else
                {
                    InventoryViabilityList = new ObservableCollection<WrappedSelection<InventoryViability>>();
                    await _pageDialogService.DisplayAlertAsync("Message", "Inventory Viability not found", "OK");
                }
            }
            catch (Exception e)
            {
                await _pageDialogService.DisplayAlertAsync("Error", e.Message, "OK");
            }
        }

        public DelegateCommand EvaluateCommand { get; }
        private async void OnEvaluateCommandExecuted()
        {
            try
            {
                var navigationParams = new NavigationParameters();
                navigationParams.Add("InventoryViability", SelectedInventoryViability.Item);
                await NavigationService.NavigateAsync("ViabilityTestDataPage", navigationParams);
            }
            catch (Exception e)
            {
                await _pageDialogService.DisplayAlertAsync("Error", e.Message, "OK");
            }
        }
        /*
        public DelegateCommand ItemTappedCommand { get; }
        private async void OnItemTappedCommandExecuted()
        {

        }
        */
        public override async void OnNavigatedTo(INavigationParameters parameters)
        {
            try
            {
                if (InventoryViabilityRuleList == null)
                {
                    InventoryViabilityRuleList = await _restService.SearchInventoryViabilityRuleAsync("@inventory_viability_rule.inventory_viability_rule_id > 0", null, "inventory_viability_rule");
                }

                if (parameters.ContainsKey("InventoryViability") && InventoryViabilityList != null)
                {
                    WrappedSelection<InventoryViability> newInventoryViability = new WrappedSelection<InventoryViability>
                    {
                        IsSelected = true,
                        Item = (InventoryViability)parameters["InventoryViability"]
                    };
                    SelectedInventoryViability = null;

                    WrappedSelection<InventoryViability> temp = _inventoryViabilityList.FirstOrDefault(x => x.Item.InventoryViabilityId == newInventoryViability.Item.InventoryViabilityId);
                    if (temp != null)
                    {
                        InventoryViabilityList.Remove(temp);
                    }
                    InventoryViabilityList.Add(newInventoryViability);

                    InventoryViabilityList = new ObservableCollection<WrappedSelection<InventoryViability>>(
                        InventoryViabilityList.OrderByDescending(x => x.Item.TestedDate).Select(
                        item => new WrappedSelection<InventoryViability> { IsSelected = item.IsSelected, Item = item.Item })
                    );

                    SelectedInventoryViability = newInventoryViability;
                    //SelectedInventoryViability.IsSelected = true;
                }
            }
            catch (Exception ex)
            {
                await _pageDialogService.DisplayAlertAsync("Error", ex.Message, "OK");
            }
        }

        /*
        private IList<ParentListClass> _parentItems;
        public IList<ParentListClass> ParentItems
        {
            get { return _parentItems; }
            set { _parentItems = value; }
        }
        */
    }
}

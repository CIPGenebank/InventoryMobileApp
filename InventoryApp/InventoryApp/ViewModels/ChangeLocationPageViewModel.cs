using ImTools;
using InventoryApp.Helpers;
using InventoryApp.Models;
using InventoryApp.Services;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Navigation;
using Prism.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;

namespace InventoryApp.ViewModels
{
    public class ChangeLocationPageViewModel : ViewModelBase
    {
        #region Properties
        private List<string> _listLocation1;
        public List<string> ListLocation1
        {
            get { return _listLocation1; }
            set { SetProperty(ref _listLocation1, value); }
        }
        private List<string> _listLocation2;
        public List<string> ListLocation2
        {
            get { return _listLocation2; }
            set { SetProperty(ref _listLocation2, value); }
        }
        private List<string> _listLocation3;
        public List<string> ListLocation3
        {
            get { return _listLocation3; }
            set { SetProperty(ref _listLocation3, value); }
        }
        private List<string> _listLocation4;
        public List<string> ListLocation4
        {
            get { return _listLocation4; }
            set { SetProperty(ref _listLocation4, value); }
        }
        private string _location1;
        public string Location1
        {
            get { return _location1; }
            set { SetProperty(ref _location1, value); }
        }

        private string _location2;
        public string Location2
        {
            get { return _location2; }
            set { SetProperty(ref _location2, value); }
        }

        private string _location3;
        public string Location3
        {
            get { return _location3; }
            set { SetProperty(ref _location3, value); }
        }

        private string _location4;
        public string Location4
        {
            get { return _location4; }
            set { SetProperty(ref _location4, value); }
        }

        private string _location1Text;
        public string Location1Text
        {
            get { return _location1Text; }
            set { SetProperty(ref _location1Text, value); }
        }

        private string _location2Text;
        public string Location2Text
        {
            get { return _location2Text; }
            set { SetProperty(ref _location2Text, value); }
        }

        private string _location3Text;
        public string Location3Text
        {
            get { return _location3Text; }
            set { SetProperty(ref _location3Text, value); }
        }

        private string _location4Text;
        public string Location4Text
        {
            get { return _location4Text; }
            set { SetProperty(ref _location4Text, value); }
        }

        private bool _isPicker1;
        public bool IsPicker1
        {
            get { return _isPicker1; }
            set { SetProperty(ref _isPicker1, value); }
        }

        private bool _isPicker2;
        public bool IsPicker2
        {
            get { return _isPicker2; }
            set { SetProperty(ref _isPicker2, value); }
        }

        private bool _isPicker3;
        public bool IsPicker3
        {
            get { return _isPicker3; }
            set { SetProperty(ref _isPicker3, value); }
        }

        private bool _isPicker4;
        public bool IsPicker4
        {
            get { return _isPicker4; }
            set { SetProperty(ref _isPicker4, value); }
        }
        #endregion

        IPageDialogService _pageDialogService { get; }
        private List<Location> _location;
        private readonly RestClient _restClient;
        private List<InventoryThumbnail> _inventoryList;

        public ChangeLocationPageViewModel(INavigationService navigationService, IPageDialogService pageDialogService)
            : base(navigationService)
        {
            _pageDialogService = pageDialogService;
            _restClient = new RestClient();

            Title = "Change Location";

            SaveCommand = new DelegateCommand(OnSaveCommandExecuted);

            IsPicker1 = true;
            IsPicker2 = true;
            IsPicker3 = true;
            IsPicker4 = false;

            //ListLocation1ChangedCommand = new DelegateCommand(OnListLocation1ChangedCommandExecuted);
            //ListLocation2ChangedCommand = new DelegateCommand(OnListLocation2ChangedCommandExecuted);
        }

        public DelegateCommand SaveCommand { get; }
        private async void OnSaveCommandExecuted()
        {
            await _pageDialogService.DisplayAlertAsync("Inventory List Count", _inventoryList.Count.ToString(), "OK");

            foreach (InventoryThumbnail inventory in _inventoryList)
            {
                try
                {
                    if (!IsPicker1)
                        inventory.storage_location_part1 = Location1Text;
                    else
                        inventory.storage_location_part1 = Location1;

                    if (!IsPicker2)
                        inventory.storage_location_part2 = Location2Text;
                    else
                        inventory.storage_location_part2 = Location2;

                    if (!IsPicker3)
                        inventory.storage_location_part3 = Location3Text;
                    else
                        inventory.storage_location_part3 = Location3;

                    if (!IsPicker4)
                        inventory.storage_location_part4 = Location4Text;
                    else
                        inventory.storage_location_part4 = Location4;

                    await _restClient.UpdateInventory(inventory);
                }
                catch (Exception ex)
                {
                    await _pageDialogService.DisplayAlertAsync("Error", ex.Message, "OK");
                }
            }

            var list = string.Join(",", _inventoryList.Select(i => i.inventory_id));
            _inventoryList = await _restClient.Search("@inventory.inventory_id in (" + list + ")", "get_mob_inventory_thumbnail", "inventory");
            
            var navigationParams = new NavigationParameters();
            navigationParams.Add("InventoryThumbnailList", _inventoryList);
            await NavigationService.GoBackAsync(navigationParams);

            //var answer = await _pageDialogService.DisplayAlertAsync("Changes saved", "Do you want to go back to Inventories List Page", "YES", "NO");
            //if (answer)
            //{

            //}
        }

        public DelegateCommand ListLocation1ChangedCommand { get; }
        private async void OnListLocation1ChangedCommandExecuted()
        {
            try
            {
                if (Location1 != null)
                {
                    //Location1 = Settings.Location1;
                    ListLocation2 = _location.Where(l => l.storage_location_part1.Equals(Location1) && !string.IsNullOrEmpty(l.storage_location_part2))
                        .Select(l => l.storage_location_part2).Distinct().ToList();
                }
            }
            catch (Exception e)
            {
                await _pageDialogService.DisplayAlertAsync("Error", e.Message, "OK");
            }
        }
        public DelegateCommand ListLocation2ChangedCommand { get; }
        private async void OnListLocation2ChangedCommandExecuted()
        {
            try
            {
                if (Location2 != null)
                {
                    ListLocation3 = _location.Where(l => l.storage_location_part1.Equals(Location1) && l.storage_location_part2.Equals(Location2) && !string.IsNullOrEmpty(l.storage_location_part3))
                        .Select(l => l.storage_location_part3).Distinct().ToList();
                }
            }
            catch (Exception e)
            {
                await _pageDialogService.DisplayAlertAsync("Error", e.Message, "OK");
            }
        }

        public override async void OnNavigatedTo(INavigationParameters parameters)
        {
            try
            {
                if (_location == null)
                {
                    _location = await _restClient.GetLocations(Settings.CooperatorGroupIndex.ToString());
                }

                if (ListLocation1 == null)
                {
                    ListLocation1 = _location.Select(l => l.storage_location_part1).Distinct().ToList();
                    ListLocation2 = _location.Select(l => l.storage_location_part2).Distinct().ToList();
                    ListLocation3 = _location.Select(l => l.storage_location_part3).Distinct().ToList();
                    ListLocation4 = _location.Select(l => l.storage_location_part4).Distinct().ToList();
                    Location1 = Settings.Location1;
                }

                if (parameters.ContainsKey("inventoryList"))
                {
                    _inventoryList = (List<InventoryThumbnail>)parameters["inventoryList"];
                }
            }
            catch (Exception e)
            {
                await _pageDialogService.DisplayAlertAsync("Error", e.Message, "OK");
            }
        }
    }
    
}

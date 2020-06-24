using InventoryApp.Interfaces;
using InventoryApp.Models;
using InventoryApp.Services;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Navigation;
using Prism.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using Xamarin.Forms;

namespace InventoryApp.ViewModels
{
    public class LookupPickerPageViewModel : ViewModelBaseZ
    {
        private IDataStoreService _dataStoreService;
        private EntityAttribute _attribute;
        private RestClient _restClient;

        private List<ILookup> _lookupList;
        public List<ILookup> LookupList
        {
            get { return _lookupList; }
            set { SetProperty(ref _lookupList, value); }
        }

        private string _searchText;
        public string SearchText
        {
            get { return _searchText; }
            set { SetProperty(ref _searchText, value); }
        }
        private bool _isSearchBarVisible;
        public bool IsSearchBarVisible
        {
            get { return _isSearchBarVisible; }
            set { SetProperty(ref _isSearchBarVisible, value); }
        }
        public LookupPickerPageViewModel(INavigationService navigationService, IPageDialogService pageDialogService, IDataStoreService dataStoreService)
            : base(navigationService, pageDialogService)
        {
            _dataStoreService = dataStoreService;
            //_dataStoreService.SyncCodeValueLookup();
            //_dataStoreService.SyncAccessionLookup();
            _restClient = new RestClient();

            SearchTextCommand = new DelegateCommand(OnSearchTextCommandExecuted);
            ItemTappedCommand = new DelegateCommand<object>(OnItemTappedCommandExecuted);
        }

        public DelegateCommand SearchTextCommand { get; }
        private async void OnSearchTextCommandExecuted()
        {
            try
            {
                //LookupList = await _dataStoreService.GetAccessionLookupList(SearchText);
                switch (_attribute.ControlSource)
                {
                    case "accession_lookup":
                        LookupList = await _restClient.GetAccessionLookUpListByAccessionNumber(SearchText);
                        break;
                    case "inventory_maint_policy_lookup":
                        LookupList = await _restClient.GetInventoryMaintPolicyLookupByMaintenanceName(SearchText);
                        break; 
                }
            }
            catch (Exception ex)
            {
                await PageDialogService.DisplayAlertAsync("Error", ex.Message, "OK");
            }
        }
        public DelegateCommand<Object> ItemTappedCommand { get; }
        private async void OnItemTappedCommandExecuted(Object param)
        {
            try
            {
                ILookup selectedTtem = (ILookup)param;
                _attribute.Value = selectedTtem.ValueMember;
                _attribute.DisplayValue = selectedTtem.DisplayMember;

                var navigationParams = new NavigationParameters
                {
                    { "LookupPicker", _attribute }
                };
                await NavigationService.GoBackAsync(navigationParams);
            }
            catch (Exception ex)
            {
                await PageDialogService.DisplayAlertAsync("Error", ex.Message, "OK");
            }
        }
        public async override void OnNavigatedTo(INavigationParameters parameters)
        {
            try
            {
                if (parameters.ContainsKey("EntityAttribute"))
                {
                    _attribute = (EntityAttribute)parameters["EntityAttribute"];

                    if (_attribute.ControlType.Equals("DROPDOWN"))
                    {
                        IsSearchBarVisible = false;
                        LookupList = await _restClient.GetCodeValueByGroupName(_attribute.ControlSource);
                        //LookupList = await _dataStoreService.GetCodeValueList(_attribute.ControlSource);
                    }
                    else if (_attribute.ControlType.Equals("LOOKUPPICKER")) 
                    {
                        IsSearchBarVisible = true;
                        LookupList = null; 
                    }
                    
                }
            }
            catch (Exception ex)
            {
                await PageDialogService.DisplayAlertAsync("Error", ex.Message, "OK");
            }
        }
    }
}

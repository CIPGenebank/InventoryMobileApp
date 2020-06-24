using ImTools;
using InventoryApp.EventAggregators;
using InventoryApp.Helpers;
using InventoryApp.Models;
using InventoryApp.Services;
using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;
using Prism.Navigation;
using Prism.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using Xamarin.Forms;

namespace InventoryApp.ViewModels
{
    public class InventoriesPageViewModel : ViewModelBaseZ
    {
        #region Properties
        private ObservableCollection<InventoryThumbnail> _inventoryCollection;
        public ObservableCollection<InventoryThumbnail> InventoryCollection
        {
            get { return _inventoryCollection; }
            set { SetProperty(ref _inventoryCollection, value); }
        }

        private ObservableCollection<WrappedSelection<InventoryThumbnail>> _inventoryList;
        public ObservableCollection<WrappedSelection<InventoryThumbnail>> InventoryList
        {
            get { return _inventoryList; }
            set { SetProperty(ref _inventoryList, value); }
        }

        private WrappedSelection<InventoryThumbnail> _selectedInventory;
        public WrappedSelection<InventoryThumbnail> SelectedInventory
        {
            get { return _selectedInventory; }
            set { SetProperty(ref _selectedInventory, value); }
        }

        private int _accessionCount;
        public int AccessionCount
        {
            get { return _accessionCount; }
            set { SetProperty(ref _accessionCount, value); }
        }

        private bool _isAllSelected;
        public bool IsAllSelected
        {
            get { return _isAllSelected; }
            set
            {
                foreach (var wi in _inventoryList)
                {
                    wi.IsSelected = value;
                }
                if (value)
                    SelectedRowsCount = InventoryList.Count;
                else
                    SelectedRowsCount = 0;
                SetProperty(ref _isAllSelected, value);
            }
        }

        private int _selectedRowsCount;
        public int SelectedRowsCount
        {
            get { return _selectedRowsCount; }
            set { SetProperty(ref _selectedRowsCount, value); }
        }

        private decimal? _totalQuantity;
        public decimal? TotalQuantity
        {
            get { return _totalQuantity; }
            set { SetProperty(ref _totalQuantity, value); }
        }
        #endregion

        private RestClient _restClient;
        private List<DataviewColumn> _dataviewColumnList;
        #region PropertiesLang
        private string _header1;
        public string Header1
        {
            get { return _header1; }
            set { SetProperty(ref _header1, value); }
        }
        private string _header2;
        public string Header2
        {
            get { return _header2; }
            set { SetProperty(ref _header2, value); }
        }
        private string _header3;
        public string Header3
        {
            get { return _header3; }
            set { SetProperty(ref _header3, value); }
        }
        private string _header4;
        public string Header4
        {
            get { return _header4; }
            set { SetProperty(ref _header4, value); }
        }
        private string _header5;
        public string Header5
        {
            get { return _header5; }
            set { SetProperty(ref _header5, value); }
        }
        private string _header6;
        public string Header6
        {
            get { return _header6; }
            set { SetProperty(ref _header6, value); }
        }
        private string _header7;
        public string Header7
        {
            get { return _header7; }
            set { SetProperty(ref _header7, value); }
        }
        #endregion
        public InventoriesPageViewModel(INavigationService navigationService, IPageDialogService pageDialogService, IEventAggregator eventAggregator)
            : base(navigationService, pageDialogService)
        {
            Title = "Inventory list";

            _restClient = new RestClient();
            //_inventoryCollection = new ObservableCollection<InventoryThumbnail>(InventoryFactory.GetInventories());
            _inventoryList = new ObservableCollection<WrappedSelection<InventoryThumbnail>>() ;

            /*
            foreach (var inventory in InventoryFactory.GetInventories())
            {
                _inventoryList.Add(new WrappedSelection<InventoryThumbnail>() { Item = inventory, IsSelected = false });
            }*/

            ItemTappedCommand = new DelegateCommand<Object>(OnItemTappedCommandExecuted);

            NewCommand = new DelegateCommand(OnNewCommandExecuted);
            ScanCommand = new DelegateCommand(OnScanCommand);
            SearchCommand = new DelegateCommand(OnSearchCommandExecuted);
            
            PrintCommand = new DelegateCommand(OnPrintCommandExecuted);
            RemoveCommand = new DelegateCommand(OnRemoveCommandExecuted);
            
            RegisterTransactionCommand = new DelegateCommand(OnRegisterTransactionCommandExecuted);
            //UpdateAttributeBatchCommand = new DelegateCommand(OnUpdateAttributeBatchCommandExecuted);

            ChangeLocationCommand = new DelegateCommand(OnChangeLocationCommandExecuted);
            ViewInventoryActionsCommand = new DelegateCommand(OnViewInventoryActionsCommand);
            SaveListCommand = new DelegateCommand(OnSaveListCommand);

            SelectItemCommand = new DelegateCommand<Object>(OnSelectItemCommand);

            SelectedRowsCount = InventoryList.Count(i => i.IsSelected == true);
            AccessionCount = InventoryList.Select(i => i.Item.accession_id).Distinct().Count();
            TotalQuantity = _inventoryList.Select(i => i.Item.quantity_on_hand).Sum();

            eventAggregator.GetEvent<AddInventoryToListEvent>().Subscribe(OnAddItemToList);
        }

        #region EventHandlers
        private void OnAddItemToList(InventoryThumbnail inventory)
        {
            if (_inventoryList.Any(i => i.Item.inventory_id == inventory.inventory_id))
                Device.BeginInvokeOnMainThread(() => {
                    PageDialogService.DisplayAlertAsync("Warnning", "Inventory exists in list", "OK");
                });
            else
            {
                _inventoryList.Add(new WrappedSelection<InventoryThumbnail>() { IsSelected = false, Item = inventory });
                AccessionCount = InventoryList.Select(i => i.Item.AccessionNumber).Distinct().Count();
                TotalQuantity = _inventoryList.Select(i => i.Item.quantity_on_hand).Sum();
            }
        }
        #endregion
        public DelegateCommand SaveListCommand { get; }
        private async void OnSaveListCommand()
        {
            try
            {
                await PageDialogService.DisplayAlertAsync("Message", "Under Construction", "OK");
            }
            catch (Exception ex)
            {
                await PageDialogService.DisplayAlertAsync("Error", ex.Message, "OK");
            }
        }
        public DelegateCommand NewCommand { get; }
        private async void OnNewCommandExecuted()
        {
            await NavigationService.NavigateAsync("InventoryPage");
        }
        public DelegateCommand ScanCommand { get; }
        private async void OnScanCommand()
        {
            await NavigationService.NavigateAsync("ScanPage");
        }
        public DelegateCommand<Object> SelectItemCommand { get; }
        private async void OnSelectItemCommand(Object param)
        {
            try
            {
                WrappedSelection<InventoryThumbnail> listItem = (WrappedSelection<InventoryThumbnail>)param;
                if (listItem != null)
                {
                    listItem.IsSelected = !listItem.IsSelected;
                }

                if (listItem.IsSelected)
                    SelectedRowsCount++;
                else
                    SelectedRowsCount--;
            }
            catch (Exception ex)
            {
                await PageDialogService.DisplayAlertAsync("Error", ex.Message, "OK");
            }    
        }

        public DelegateCommand SearchCommand { get; }
        private async void OnSearchCommandExecuted()
        {
            await NavigationService.NavigateAsync("SearchInventoriesPage");
        }
        
        public DelegateCommand ViewInventoryActionsCommand { get; }
        private async void OnViewInventoryActionsCommand()
        {
            var navigationParams = new NavigationParameters();
            var selection = GetSelection();
            navigationParams.Add("InventoryThumbnailList", selection);
            await NavigationService.NavigateAsync("InventoryActionsPage", navigationParams, useModalNavigation: false, animated: true);
        }
        public DelegateCommand<Object> ItemTappedCommand { get; }
        public DelegateCommand AllCommand { get; }
        public DelegateCommand NoneCommand { get; }
        public DelegateCommand PrintCommand { get; }
        public DelegateCommand RemoveCommand { get; }
        public DelegateCommand RegisterTransactionCommand { get; }
        public DelegateCommand UpdateAttributeBatchCommand { get; }

        public DelegateCommand ViewDetailsCommand { get; }

        public DelegateCommand ChangeLocationCommand { get; }

        private async void OnChangeLocationCommandExecuted()
        {
            var navigationParams = new NavigationParameters();
            var selection = GetSelection();
            navigationParams.Add("inventoryList", selection);
            await NavigationService.NavigateAsync("ChangeLocationPage", navigationParams);
        }

        private async void OnItemTappedCommandExecuted(Object param)
        {
            var navigationParams = new NavigationParameters
            {
                { "InventoryThumbnail", (InventoryThumbnail)param }
            };
            await NavigationService.NavigateAsync("InventoryPage", navigationParams);
        }

        private async void OnPrintCommandExecuted()
        {
            try
            {
                var navigationParams = new NavigationParameters();
                var selection = GetSelection();
                navigationParams.Add("inventoryList", selection);
                await NavigationService.NavigateAsync("PrintingPage", navigationParams, useModalNavigation: false, animated: true);
            }
            catch (Exception ex)
            {
                await PageDialogService.DisplayAlertAsync("Error", ex.Message, "OK");
            }
        }

        private void OnRemoveCommandExecuted()
        {
            for (int i = _inventoryList.Count - 1; i >= 0; i--)
            {
                if (_inventoryList[i].IsSelected) _inventoryList.RemoveAt(i);
            }

            if (IsAllSelected)
                IsAllSelected = false;
            
            SelectedRowsCount = 0;
            AccessionCount = InventoryList.Select(i => i.Item.AccessionNumber).Distinct().Count();
            TotalQuantity = _inventoryList.Select(i => i.Item.quantity_on_hand).Sum();
        }
        private void OnSelectAllCommandExecuted()
        {
            foreach (var wi in _inventoryList)
            {
                wi.IsSelected = true;
            }
        }
        private void OnSelectNoneCommandExecuted()
        {
            foreach (var wi in _inventoryList)
            {
                wi.IsSelected = false;
            }
        }

        private async void OnRegisterTransactionCommandExecuted()
        {
            var navigationParams = new NavigationParameters();
            var selection = GetSelection();
            navigationParams.Add("inventoryList", selection);
            await NavigationService.NavigateAsync("MovementPage", navigationParams);
        }
        private async void OnUpdateAttributeBatchCommandExecuted()
        {
            await NavigationService.NavigateAsync("UpdateAttributePage");
        }
        
        private List<InventoryThumbnail> GetSelection()
        {
            return _inventoryList.Where(item => item.IsSelected).Select(wrappedItem => wrappedItem.Item).ToList();
        }

        public override async void OnNavigatedTo(INavigationParameters parameters)
        {
            try
            {
                if(_dataviewColumnList == null)
                {
                    _dataviewColumnList = await _restClient.GetDataviewAtributeList(Settings.WorkgroupInventoryThumbnailDataview);
                    if(_dataviewColumnList != null)
                    {
                        if (_dataviewColumnList.Count > 1) Header1 = _dataviewColumnList[1].title;
                        if (_dataviewColumnList.Count > 2) Header2 = _dataviewColumnList[2].title;
                        if (_dataviewColumnList.Count > 3) Header3 = _dataviewColumnList[3].title;
                        if (_dataviewColumnList.Count > 4) Header4 = _dataviewColumnList[4].title;
                        if (_dataviewColumnList.Count > 5) Header5 = _dataviewColumnList[5].title;
                        if (_dataviewColumnList.Count > 6) Header6 = _dataviewColumnList[6].title;
                        if (_dataviewColumnList.Count > 7) Header7 = _dataviewColumnList[7].title;
                    }
                }

                if (parameters.ContainsKey("inventory")) //Update UI
                {
                    InventoryThumbnail tempInventory = (InventoryThumbnail)parameters["inventory"];
                    WrappedSelection<InventoryThumbnail> inventoryThumbnailItem = _inventoryList.FirstOrDefault(item => item.Item.inventory_id == tempInventory.inventory_id);
                    if (inventoryThumbnailItem == null)
                    {
                        InventoryList.Add(new WrappedSelection<InventoryThumbnail> { Item = tempInventory, IsSelected = false });
                    }
                    else
                    {
                        inventoryThumbnailItem.Item = tempInventory;
                        /*
                        int index = InventoryList.IndexOf(inventoryThumbnailItem);
                        InventoryList.Remove(inventoryThumbnailItem);
                        InventoryList.Insert(index, new WrappedSelection<InventoryThumbnail> { Item = tempInventory, IsSelected = false });
                        */
                    }
                }

                if (parameters.ContainsKey("InventoryThumbnail")) 
                {
                    InventoryThumbnail tempInventory = (InventoryThumbnail)parameters["InventoryThumbnail"];
                    WrappedSelection<InventoryThumbnail> inventoryThumbnailItem = _inventoryList.FirstOrDefault(item => item.Item.inventory_id == tempInventory.inventory_id);
                    if (inventoryThumbnailItem == null)
                    {
                        InventoryList.Add(new WrappedSelection<InventoryThumbnail> { Item = tempInventory, IsSelected = false });
                    }
                    else
                    {
                        inventoryThumbnailItem.Item = tempInventory;
                    }

                    AccessionCount = InventoryList.Select(i => i.Item.AccessionNumber).Distinct().Count();
                    TotalQuantity = _inventoryList.Select(i => i.Item.quantity_on_hand).Sum();
                }

                if (parameters.ContainsKey("InventoryThumbnailList"))
                {
                    List<InventoryThumbnail> inventoryList = (List<InventoryThumbnail>)parameters["InventoryThumbnailList"];

                    foreach (var inventoryThumbnail in inventoryList)
                    {
                        var wrappedInventory = InventoryList.FirstOrDefault(x => x.Item.inventory_id == inventoryThumbnail.inventory_id);
                        if (wrappedInventory == null)
                        {
                            WrappedSelection<InventoryThumbnail> temp = new WrappedSelection<InventoryThumbnail>() { Item = inventoryThumbnail, IsSelected = false };
                            InventoryList.Add(temp);
                        }
                        else
                        {
                            wrappedInventory.Item = inventoryThumbnail;
                            /*int index = _inventoryList.IndexOf(wrappedInventory);
                            InventoryList.Remove(wrappedInventory);
                            InventoryList.Insert(index, new WrappedSelection<InventoryThumbnail> { Item = inventoryThumbnail, IsSelected = wrappedInventory.IsSelected });*/
                        }
                    }

                    AccessionCount = InventoryList.Select(i => i.Item.AccessionNumber).Distinct().Count();
                    TotalQuantity = _inventoryList.Select(i => i.Item.quantity_on_hand).Sum();
                }
            }
            catch (Exception ex)
            {
                await PageDialogService.DisplayAlertAsync("Error", ex.Message, "OK");
            }
        }
    }
}

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
    public class PreviousInventoriesPageViewModel : ViewModelBase
    { 
        IPageDialogService _pageDialogService { get; }

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
                //SelectedRowsCount = InventoryList.Count(x => x.IsSelected == true);
                SetProperty(ref _isAllSelected, value);
            }
        }

        private int _selectedRowsCount;
        public int SelectedRowsCount
        {
            get { return _selectedRowsCount; }
            set { SetProperty(ref _selectedRowsCount, value); }
        }

        public PreviousInventoriesPageViewModel(INavigationService navigationService, IPageDialogService pageDialogService) : base(navigationService)
        {
            Title = "Inventories";

            _inventoryList = new ObservableCollection<WrappedSelection<InventoryThumbnail>>();
            _pageDialogService = pageDialogService;

            SearchCommand = new DelegateCommand(OnSearchCommandExecuted);
            ItemTappedCommand = new DelegateCommand(OnItemTappedCommandExecuted);
            NewCommand = new DelegateCommand(OnNewCommandExecuted);

            AllCommand = new DelegateCommand(OnSelectAllCommandExecuted);
            NoneCommand = new DelegateCommand(OnSelectNoneCommandExecuted);
            PrintCommand = new DelegateCommand(OnPrintCommandExecuted);
            RemoveCommand = new DelegateCommand(OnRemoveCommandExecuted);
            RegisterTransactionCommand = new DelegateCommand(OnRegisterTransactionCommandExecuted);
            UpdateAttributeBatchCommand = new DelegateCommand(OnUpdateAttributeBatchCommandExecuted);

            ViewDetailsCommand = new DelegateCommand(OnViewDetailsCommandExecuted);

            ChangeLocationCommand = new DelegateCommand(OnChangeLocationCommandExecuted);

            ItemToggledCommand = new DelegateCommand(OnItemToggledCommandCommandExecuted);
        }

        public DelegateCommand SearchCommand { get; }
        public DelegateCommand NewCommand { get; }
        public DelegateCommand ItemTappedCommand { get; }
        public DelegateCommand AllCommand { get; }
        public DelegateCommand NoneCommand { get; }
        public DelegateCommand PrintCommand { get; }
        public DelegateCommand RemoveCommand { get; }
        public DelegateCommand RegisterTransactionCommand { get; }
        public DelegateCommand UpdateAttributeBatchCommand { get; }

        public DelegateCommand ViewDetailsCommand { get; }

        public DelegateCommand ChangeLocationCommand { get; }

        public DelegateCommand ItemToggledCommand { get; }
        private void OnItemToggledCommandCommandExecuted()
        {
            SelectedRowsCount = InventoryList.Count(x => x.IsSelected == true);
        }


        private async void OnChangeLocationCommandExecuted()
        {
            var navigationParams = new NavigationParameters();
            var selection = GetSelection();
            navigationParams.Add("inventoryList", selection);
            await NavigationService.NavigateAsync("ChangeLocationPage", navigationParams);
        }

        private async void OnViewDetailsCommandExecuted()
        {
            var wrappedInventory = _inventoryList.FirstOrDefault(item => item.IsSelected = true);
            if (wrappedInventory != null)
            {
                var navigationParams = new NavigationParameters();
                navigationParams.Add("inventory", wrappedInventory.Item);
                await NavigationService.NavigateAsync("InventoryPage", navigationParams);
            }
        }

        private async void OnSearchCommandExecuted()
        {
            await NavigationService.NavigateAsync("SearchInventoriesPage");
        }

        private async void OnNewCommandExecuted()
        {
            await NavigationService.NavigateAsync("NewInventoryPage");
        }

        private async void OnItemTappedCommandExecuted()
        {
            var navigationParams = new NavigationParameters();
            navigationParams.Add("inventory", SelectedInventory.Item);
            await NavigationService.NavigateAsync("InventoryPage", navigationParams);
        }

        private async void OnPrintCommandExecuted()
        {
            var navigationParams = new NavigationParameters();
            var selection = GetSelection();
            navigationParams.Add("inventoryList", selection);
            await NavigationService.NavigateAsync("PrintingPage", navigationParams, useModalNavigation: false, animated: true);
        }

        private void OnRemoveCommandExecuted()
        {
            for (int i = _inventoryList.Count - 1; i >= 0; i--)
            {
                if (_inventoryList[i].IsSelected) _inventoryList.RemoveAt(i);
            }
            AccessionCount = InventoryList.Select(i => i.Item.accession_id).Distinct().Count();
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

        public List<InventoryThumbnail> GetSelection()
        {
            return _inventoryList.Where(item => item.IsSelected).Select(wrappedItem => wrappedItem.Item).ToList();
        }

        public override async void OnNavigatedTo(INavigationParameters parameters)
        {
            try
            {
                if (parameters.ContainsKey("inventoryList"))
                {
                    List<InventoryThumbnail> inventoryList = (List<InventoryThumbnail>)parameters["inventoryList"];

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
                            int index = _inventoryList.IndexOf(wrappedInventory);
                            InventoryList.Remove(wrappedInventory);
                            InventoryList.Insert(index, new WrappedSelection<InventoryThumbnail> { Item = inventoryThumbnail, IsSelected = wrappedInventory.IsSelected });
                        }
                    }

                    AccessionCount = InventoryList.Select(i => i.Item.accession_id).Distinct().Count();
                }
                if (parameters.ContainsKey("InventoryThumbnail"))
                {
                    InventoryThumbnail inventoryThumbnail = (InventoryThumbnail)parameters["InventoryThumbnail"];

                    WrappedSelection<InventoryThumbnail> inventoryThumbnailItem = _inventoryList.FirstOrDefault(item => item.Item.inventory_id == inventoryThumbnail.inventory_id);
                    if (inventoryThumbnailItem == null)
                    {
                        InventoryList.Add(new WrappedSelection<InventoryThumbnail> { Item = inventoryThumbnail, IsSelected = false });
                    }
                    else
                    {
                        int index = InventoryList.IndexOf(inventoryThumbnailItem);
                        InventoryList.Remove(inventoryThumbnailItem);
                        InventoryList.Insert(index, new WrappedSelection<InventoryThumbnail> { Item = inventoryThumbnail, IsSelected = false });
                    }
                }
                if (parameters.ContainsKey("message"))
                {
                    await _pageDialogService.DisplayAlertAsync("Message", (string)parameters["message"], "OK");
                }
            }
            catch (Exception ex)
            {
                await _pageDialogService.DisplayAlertAsync("Error", ex.Message, "OK");
            }
        }
    }
}

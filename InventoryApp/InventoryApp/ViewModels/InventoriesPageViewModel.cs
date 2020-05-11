﻿using ImTools;
using InventoryApp.EventAggregators;
using InventoryApp.Models;
using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;
using Prism.Navigation;
using Prism.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Xamarin.Forms;

namespace InventoryApp.ViewModels
{
    public class InventoriesPageViewModel : ViewModelBase
    {
        IPageDialogService PageDialogService { get; }

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

        public InventoriesPageViewModel(INavigationService navigationService, IPageDialogService pageDialogService, IEventAggregator eventAggregator)
            : base(navigationService)
        {
            Title = "Inventories";

            _inventoryCollection = new ObservableCollection<InventoryThumbnail>(InventoryFactory.GetInventories());
            //_inventoryList = new ObservableCollection<WrappedSelection<InventoryThumbnail>>();
            _inventoryList = new ObservableCollection<WrappedSelection<InventoryThumbnail>>() ;
            foreach (var inventory in InventoryFactory.GetInventories())
            {
                _inventoryList.Add(new WrappedSelection<InventoryThumbnail>() { Item = inventory, IsSelected = false });
            }
            PageDialogService = pageDialogService;

            //SearchCommand = new DelegateCommand(OnSearchCommandExecuted);
            ItemTappedCommand = new DelegateCommand<Object>(OnItemTappedCommandExecuted);
            //NewCommand = new DelegateCommand(OnNewCommandExecuted);

            //AllCommand = new DelegateCommand(OnSelectAllCommandExecuted);
            //NoneCommand = new DelegateCommand(OnSelectNoneCommandExecuted);
            //PrintCommand = new DelegateCommand(OnPrintCommandExecuted);
            RemoveCommand = new DelegateCommand(OnRemoveCommandExecuted);
            ScanCommand = new DelegateCommand(OnScanCommandExecuted);
            //RegisterTransactionCommand = new DelegateCommand(OnRegisterTransactionCommandExecuted);
            //UpdateAttributeBatchCommand = new DelegateCommand(OnUpdateAttributeBatchCommandExecuted);

            //ViewDetailsCommand = new DelegateCommand(OnViewDetailsCommandExecuted);
            //ChangeLocationCommand = new DelegateCommand(OnChangeLocationCommandExecuted);

            //ItemToggledCommand = new DelegateCommand(OnItemToggledCommandCommandExecuted);

            SelectItemCommand = new DelegateCommand<Object>(OnSelectItemCommandExecuted);

            SelectedRowsCount = InventoryList.Count(i => i.IsSelected == true);
            AccessionCount = InventoryList.Select(i => i.Item.accession_id).Distinct().Count();


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
            }
        }
        #endregion
        public DelegateCommand ScanCommand { get; }
        private async void OnScanCommandExecuted()
        {
            await NavigationService.NavigateAsync("ScanPage");
        }
        public DelegateCommand<Object> SelectItemCommand { get; }
        private async void OnSelectItemCommandExecuted(Object param)
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
        public DelegateCommand NewCommand { get; }
        public DelegateCommand<Object> ItemTappedCommand { get; }
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

        private async void OnItemTappedCommandExecuted(Object param)
        {
            var navigationParams = new NavigationParameters
            {
                { "inventory", (InventoryThumbnail)param }
            };
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
            SelectedRowsCount = 0;
            //AccessionCount = InventoryList.Select(i => i.Item.accession_id).Distinct().Count();
            AccessionCount = InventoryList.Select(i => i.Item.AccessionNumber).Distinct().Count();
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
            }
            catch (Exception ex)
            {
                await PageDialogService.DisplayAlertAsync("Error", ex.Message, "OK");
            }
        }
    }
}
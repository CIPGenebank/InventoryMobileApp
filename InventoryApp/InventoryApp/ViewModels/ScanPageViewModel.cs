using InventoryApp.EventAggregators;
using InventoryApp.Models;
using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;
using Prism.Navigation;
using Prism.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Threading.Tasks;
using Xamarin.Forms;
using ZXing;

namespace InventoryApp.ViewModels
{
    public class ScanPageViewModel : ViewModelBase
    {
        IPageDialogService PageDialogService { get; }
        IDeviceService DeviceService { get; }
        IEventAggregator _eventAggregator;

        private bool _isScanning;
        public bool IsScanning
        {
            get { return _isScanning; }
            set { SetProperty(ref _isScanning, value); }
        }

        private Result _result;
        public Result Result
        {
            get { return _result; }
            set { SetProperty(ref _result, value); }
        }

        private string _resultText;
        public string ResultText
        {
            get { return _resultText; }
            set { SetProperty(ref _resultText, value); }
        }

        private bool _isAnalyzing;
        public bool IsAnalyzing
        {
            get { return _isAnalyzing; }
            set { SetProperty(ref _isAnalyzing, value); }
        }

        private InventoryThumbnail _scannedInventory;
        public InventoryThumbnail ScannedInventory
        {
            get { return _scannedInventory; }
            set { SetProperty(ref _scannedInventory, value); }
        }
        public ScanPageViewModel(INavigationService navigationService, IPageDialogService pageDialogService, IDeviceService deviceService, IEventAggregator eventAggregator)
            : base(navigationService)
        {
            PageDialogService = pageDialogService;
            DeviceService = deviceService;
            _eventAggregator = eventAggregator;

            CloseCommand = new DelegateCommand(OnCloseCommandExecuted);
            ScanResultCommand = new DelegateCommand(OnScanResultCommandExecuted);
            AcceptCommand = new DelegateCommand(OnAcceptCommandExecuted);
            ScanAgainCommand = new DelegateCommand(OnScanAgainCommandExecuted);

            _isScanning = true;
            _isAnalyzing = true;

            ResultText = "";
        }

        public DelegateCommand CloseCommand { get; }
        private async void OnCloseCommandExecuted()
        {
            IsScanning = false;
            await NavigationService.GoBackAsync();
        }

        public DelegateCommand ScanResultCommand { get; }
        private void OnScanResultCommandExecuted()
        {
            if (IsAnalyzing)
            {
                IsBusy = true;
                IsAnalyzing = false;

                //int inventory_id = int.TryParse(Result.Text, out inventory_id) ? inventory_id : -1;

                //Search inventory_id in GRIN-Global
                ScannedInventory = InventoryFactory.GetInventories()[new Random().Next(0, 7)];

                IsBusy = false;
            }
            
            //DeviceService.BeginInvokeOnMainThread(async () => {


            //    //await Task.Delay(50);
            //    //await NavigationService.GoBackAsync(null, null, true);


            //    //if (_isScanning)
            //    //{
            //    //    _isScanning = false;
            //    //    IsAnalyzing = false;

            //    //    /*var response = await PageDialogService.DisplayAlertAsync("Barcode value", ResultText, "OK", "Scan again");
            //    //    if (response)
            //    //    {
            //    //        IsScanning = false;
            //    //        await NavigationService.GoBackAsync(null, null, false);
            //    //    }
            //    //    else
            //    //    {
            //    //        Result = null;
            //    //        ResultText = "";
            //    //    }*/
            //    //}
            //    //IsAnalyzing = true;
            //    //_isScanning = true;
            //});    
        }

        public DelegateCommand AcceptCommand { get; }
        private void OnAcceptCommandExecuted()
        {
            if (!IsAnalyzing && ScannedInventory != null)
            {
                _eventAggregator.GetEvent<AddInventoryToListEvent>().Publish(ScannedInventory);
                Result = null;
                ScannedInventory = null;
                IsAnalyzing = true;
            }
        }

        public DelegateCommand ScanAgainCommand { get; }
        private void OnScanAgainCommandExecuted()
        {
            Result = null;
            ScannedInventory = null;
            IsAnalyzing = true;
        }
    }
}

/*  var navigationParams = new NavigationParameters();
    navigationParams.Add("ScanResult", _result);
    await NavigationService.GoBackAsync(navigationParams);*/

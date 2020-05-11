using Prism.Commands;
using Prism.Mvvm;
using Prism.Navigation;
using System;
using System.Collections.Generic;
using System.Linq;

namespace InventoryApp.ViewModels
{
    public class ScanReaderPageViewModel : ViewModelBase
    {
        private bool _isAnalyzing;
        public bool IsAnalyzing
        {
            get { return _isAnalyzing; }
            set { SetProperty(ref _isAnalyzing, value); }
        }

        private bool _isScanning;
        public bool IsScanning
        {
            get { return _isScanning; }
            set { SetProperty(ref _isScanning, value); }
        }

        private ZXing.Result _result;
        public ZXing.Result Result
        {
            get { return _result; }
            set { SetProperty(ref _result, value); }
        }

        public DelegateCommand ScanResultCommand { get; }
        public DelegateCommand SendBackResultCommand { get; }


        public ScanReaderPageViewModel(INavigationService navigationService)
            : base(navigationService)
        {
            Title = "Scan Reader";
            _isScanning = true;
            _isAnalyzing = true;

            ScanResultCommand = new DelegateCommand(OnScanResultCommandExecuted);
            SendBackResultCommand = new DelegateCommand(OnSendBackResultCommandExecuted);
        }

        public void OnScanResultCommandExecuted()
        {
            IsAnalyzing = false;
            IsScanning = false;
        }

        public void OnSendBackResultCommandExecuted()
        {
            var navigationParams = new NavigationParameters();
            navigationParams.Add("result", _result);
            NavigationService.GoBackAsync(navigationParams);
        }
    }
}

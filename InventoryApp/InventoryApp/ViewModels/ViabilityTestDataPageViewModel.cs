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
    public class ViabilityTestDataPageViewModel : ViewModelBase
    {    
        IPageDialogService _pageDialogService { get; }
        IRestService _restService { get; }

        private ObservableCollection<InventoryViabilityData> _inventoryViabilityDataList;
        public ObservableCollection<InventoryViabilityData> InventoryViabilityDataList
        {
            get { return _inventoryViabilityDataList; }
            set { SetProperty(ref _inventoryViabilityDataList, value); }
        }

        private InventoryViabilityData _selectedInventoryViabilityData;
        public InventoryViabilityData SelectedInventoryViabilityData
        {
            get { return _selectedInventoryViabilityData; }
            set { SetProperty(ref _selectedInventoryViabilityData, value); }
        }

        public List<int> ReplicationNumberList { get; set; }
        public List<string> ReplicationCountList { get; set; }

        private string _selectedReplicationCount;
        public string SelectedReplicationCount
        {
            get { return _selectedReplicationCount; }
            set { SetProperty(ref _selectedReplicationCount, value); }
        }

        private int _inventoryViabilityId;
        public int InventoryViabilityId
        {
            get { return _inventoryViabilityId; }
            set { SetProperty(ref _inventoryViabilityId, value); }
        }

        public int ReplicationCount { get; set; }


        private InventoryViability _inventoryViability;
        public InventoryViability InventoryViability
        {
            get { return _inventoryViability; }
            set { SetProperty(ref _inventoryViability, value); }
        }

        public ViabilityTestDataPageViewModel(INavigationService navigationService, IPageDialogService pageDialogService) : base(navigationService)
        {
            Title = "Viability Test Evaluation";

            _pageDialogService = pageDialogService;
            _restService = new RestService();

            SaveCommand = new DelegateCommand(OnSaveCommandExecuted);
            NewCommand = new DelegateCommand(OnNewCommandExecuted);
            PostFinalResultCommand = new DelegateCommand(OnPostFinalResultCommandExecuted);


            ReplicationNumberList = new List<int>() { 1, 2, 3 };
            ReplicationCountList = new List<string>() { "All", "1", "2" };
            SelectedReplicationCount = "All";

        }

        public DelegateCommand SaveCommand { get; }
        private async void OnSaveCommandExecuted()
        {
            int ErrorCount = 0;
            foreach (InventoryViabilityData ivd in InventoryViabilityDataList)
            {

                try
                {
                    string result = string.Empty;
                    if (ivd.InventoryViabilityDataId < 0)
                    {
                        result = await _restService.CreateInventoryViabilityDataAsync(ivd);
                        ivd.InventoryViabilityDataId = int.Parse(result);
                    }
                    else
                    {
                        result = await _restService.UpdateInventoryViabilityDataAsync(ivd);
                    }
                }
                catch (Exception e)
                {
                    ErrorCount++;
                    await _pageDialogService.DisplayAlertAsync("Error", e.Message, "OK");
                }

            }
            if (ErrorCount == 0)
            {
                await _pageDialogService.DisplayAlertAsync("Message", "Successfully saved", "OK");
            }
        }

        public DelegateCommand NewCommand { get; }
        private async void OnNewCommandExecuted()
        {
            try
            {
                int maxCountNumber;
                if (InventoryViabilityDataList.Count > 0)
                {
                    maxCountNumber = InventoryViabilityDataList.Max(obj => obj.CountNumber);
                }
                else
                {
                    maxCountNumber = 0;
                }

                for (int i = 0; i < ReplicationCount; i++)
                {
                    InventoryViabilityDataList.Insert(0, new InventoryViabilityData()
                    {
                        InventoryViabilityDataId = -1,
                        InventoryViabilityId = InventoryViabilityId,
                        CounterCooperatorId = 48,
                        CountDate = DateTime.Now,
                        CountNumber = maxCountNumber + 1,
                        ReplicationCount = InventoryViability.TotalTestedCount / InventoryViability.ReplicationCount,
                        ReplicationNumber = (ReplicationCount - i),
                        AbnormalCount = 0,
                        NormalCount = 0,
                        Note = ""
                    });
                }
            }
            catch (Exception e)
            {
                await _pageDialogService.DisplayAlertAsync("Error", e.Message, "OK");
            }
        }

        public DelegateCommand PostFinalResultCommand { get; }
        private async void OnPostFinalResultCommandExecuted()
        {
            try
            {
                InventoryViabilityData last = InventoryViabilityDataList.OrderByDescending(x => x.CountDate).FirstOrDefault();
                InventoryViability.PercentViable = Math.Round(last.NormalCount / (decimal)last.ReplicationCount * 100, 2);
                InventoryViability.TestedDate = last.CountDate;

                string result = await _restService.UpdateInventoryViabilityAsync(InventoryViability);
                //await _pageDialogService.DisplayAlertAsync("Message", result, "OK");

                var navigationParams = new NavigationParameters();
                navigationParams.Add("InventoryViability", InventoryViability);
                await NavigationService.GoBackAsync(navigationParams);
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
                if (parameters.ContainsKey("InventoryViability"))
                {
                    InventoryViability = (InventoryViability)parameters["InventoryViability"];

                    ReplicationCount = (int)InventoryViability.ReplicationCount;
                    InventoryViabilityId = InventoryViability.InventoryViabilityId;

                    List<InventoryViabilityData> inventoryViabilityDataList = await _restService.SearchInventoryViabilityDataAsync(
                        "@inventory_viability_data.inventory_viability_id = " + InventoryViabilityId, null, "inventory_viability_data");

                    if (inventoryViabilityDataList != null)
                    {
                        InventoryViabilityDataList = new ObservableCollection<InventoryViabilityData>(inventoryViabilityDataList);
                        InventoryViabilityDataList = new ObservableCollection<InventoryViabilityData>(InventoryViabilityDataList.OrderByDescending(x => x.CountDate));
                    }
                    else
                    {
                        InventoryViabilityDataList = new ObservableCollection<InventoryViabilityData>();
                        //await _pageDialogService.DisplayAlertAsync("Message", "Inventory Viability Data not found", "OK");
                    }

                    //InventoryViabilityDataList = new ObservableCollection<InventoryViabilityData>()
                    //{
                    //    new InventoryViabilityData() { InventoryViabilityDataId = 1, InventoryViabilityId = 1
                    //             , ReplicationNumber = 1, ReplicationCount = 2, NormalCount = 50, AbnormalCount = 0, CountNumber = 100, CountDate = new DateTime(2017,12,01)
                    //        },
                    //    new InventoryViabilityData() { InventoryViabilityDataId = 2, InventoryViabilityId = 1
                    //             , ReplicationNumber = 2, ReplicationCount = 2, NormalCount = 40, AbnormalCount = 10, CountNumber = 100, CountDate = new DateTime(2017,12,01)
                    //        },
                    //    new InventoryViabilityData() { InventoryViabilityDataId = 3, InventoryViabilityId = 1
                    //            , ReplicationNumber = 1, ReplicationCount = 1, NormalCount = 40, AbnormalCount = 10, CountNumber = 100, CountDate = new DateTime(2018,01,01)
                    //        },
                    //    new InventoryViabilityData() { InventoryViabilityDataId = 4, InventoryViabilityId = 1
                    //            , ReplicationNumber = 2, ReplicationCount = 1, NormalCount = 40, AbnormalCount = 10, CountNumber = 100, CountDate = new DateTime(2018,01,01)
                    //        }
                    //};
                }
            }
            catch (Exception e)
            {
                await _pageDialogService.DisplayAlertAsync("Error", e.Message, "OK");
            }
        }
    }
}

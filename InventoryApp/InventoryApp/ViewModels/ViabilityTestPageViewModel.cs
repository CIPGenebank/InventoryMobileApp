using InventoryApp.Helpers;
using InventoryApp.Interfaces;
using InventoryApp.Models;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Navigation;
using Prism.Services;
using System;
using System.Collections.Generic;
using System.Linq;

namespace InventoryApp.ViewModels
{
    public class ViabilityTestPageViewModel : ViewModelBase
    {
        IPageDialogService _pageDialogService { get; }
        IRestService _restService { get; }

        private int _inventoryId;
        public int InventoryId
        {
            get { return _inventoryId; }
            set { SetProperty(ref _inventoryId, value); }
        }

        private List<InventoryViabilityRule> _inventoryViabilityRuleList;
        public List<InventoryViabilityRule> InventoryViabilityRuleList
        {
            get { return _inventoryViabilityRuleList; }
            set { SetProperty(ref _inventoryViabilityRuleList, value); }
        }

        private InventoryViabilityRule _selectedInventoryViabilityRule;
        public InventoryViabilityRule SelectedInventoryViabilityRule
        {
            get { return _selectedInventoryViabilityRule; }
            set { SetProperty(ref _selectedInventoryViabilityRule, value); }
        }

        private int _totalTestedCount;
        public int TotalTestedCount
        {
            get { return _totalTestedCount; }
            set { SetProperty(ref _totalTestedCount, value); }
        }

        private int _replicationCount;
        public int ReplicationCount
        {
            get { return _replicationCount; }
            set { SetProperty(ref _replicationCount, value); }
        }

        private DateTime _testedDate;
        public DateTime TestedDate
        {
            get { return _testedDate; }
            set { SetProperty(ref _testedDate, value); }
        }

        private InventoryViability _inventoryViability;
        public InventoryViability InventoryViability
        {
            get { return _inventoryViability; }
            set { SetProperty(ref _inventoryViability, value); }
        }

        public bool IsNew { get; set; }
        private bool IsReady { get; set; }

        public ViabilityTestPageViewModel(INavigationService navigationService, IPageDialogService pageDialogService) : base(navigationService)
        {
            Title = "Viability Test";

            _pageDialogService = pageDialogService;
            _restService = new RestService();

            SelectedIndexChangedCommand = new DelegateCommand(OnSelectedIndexChangedCommandExecuted);
            SaveCommand = new DelegateCommand(OnSaveCommandExecuted);

            IsNew = true;
            TestedDate = DateTime.Now;
        }

        public DelegateCommand SelectedIndexChangedCommand { get; }
        private void OnSelectedIndexChangedCommandExecuted()
        {
            if (IsReady)
            {
                ReplicationCount = SelectedInventoryViabilityRule.NumberOfReplicates;
                TotalTestedCount = SelectedInventoryViabilityRule.SeedsPerReplicate * SelectedInventoryViabilityRule.NumberOfReplicates;
            }
            IsReady = true;
        }

        public DelegateCommand SaveCommand { get; }
        private async void OnSaveCommandExecuted()
        {
            try
            {
                if (IsNew)
                {
                    InventoryViability newInventoryViability = new InventoryViability()
                    {
                        InventoryViabilityId = -1,
                        InventoryViabilityRuleId = SelectedInventoryViabilityRule.InventoryViabilityRuleId,
                        InventoryViabilityRuleName = SelectedInventoryViabilityRule.Name,
                        InventoryId = InventoryId,
                        ReplicationCount = ReplicationCount,
                        TotalTestedCount = TotalTestedCount,
                        TestedDateCode = "MM/dd/yyyy",
                        TestedDate = TestedDate
                    };
                    string result = await _restService.CreateInventoryViabilityAsync(newInventoryViability);

                    await _pageDialogService.DisplayAlertAsync("New Inventory Viability Id", result, "OK");

                    newInventoryViability.InventoryViabilityId = int.Parse(result);
                    var navigationParams = new NavigationParameters();
                    navigationParams.Add("InventoryViability", newInventoryViability);

                    await NavigationService.GoBackAsync(navigationParams);
                }
                else
                {
                    InventoryViability.InventoryViabilityRuleId = SelectedInventoryViabilityRule.InventoryViabilityRuleId;
                    InventoryViability.InventoryViabilityRuleName = SelectedInventoryViabilityRule.Name;
                    InventoryViability.ReplicationCount = ReplicationCount;
                    InventoryViability.TotalTestedCount = TotalTestedCount;
                    InventoryViability.TestedDate = TestedDate;
                    InventoryViability.TestedDateCode = "MM/dd/yyyy";

                    string result = await _restService.UpdateInventoryViabilityAsync(InventoryViability);
                    //await _pageDialogService.DisplayAlertAsync("result", result, "OK");

                    var navigationParams = new NavigationParameters();
                    navigationParams.Add("InventoryViability", InventoryViability);

                    await NavigationService.GoBackAsync(navigationParams);
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
                if (parameters.ContainsKey("InventoryId"))
                {
                    InventoryId = int.Parse(parameters["InventoryId"].ToString());
                }

                if (parameters.ContainsKey("InventoryViabilityRuleList"))
                {
                    InventoryViabilityRuleList = (List<InventoryViabilityRule>)parameters["InventoryViabilityRuleList"];
                }
                IsReady = true;

                if (parameters.ContainsKey("InventoryViability"))
                {
                    IsReady = false;

                    IsNew = false;
                    InventoryViability = (InventoryViability)parameters["InventoryViability"];
                    InventoryId = InventoryViability.InventoryId;
                    SelectedInventoryViabilityRule = InventoryViabilityRuleList.Find(x => x.InventoryViabilityRuleId == InventoryViability.InventoryViabilityRuleId);
                    TotalTestedCount = (int)InventoryViability.TotalTestedCount;
                    ReplicationCount = (int)InventoryViability.ReplicationCount;
                    TestedDate = InventoryViability.TestedDate;

                }

            }
            catch (Exception e)
            {
                await _pageDialogService.DisplayAlertAsync("Error", e.Message, "OK");
            }
        }
    }
}

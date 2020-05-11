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

namespace InventoryApp.ViewModels
{
    public class DashboardPageViewModel : ViewModelBase
    {
        IPageDialogService _pageDialogService { get; }
        private readonly RestClient _restClient;

        private List<CooperatorGroup> _listWorkgroup;
        public List<CooperatorGroup> ListWorkgroup
        {
            get { return _listWorkgroup; }
            set { SetProperty(ref _listWorkgroup, value); }
        }
        private List<string> _listLocation1;
        public List<string> ListLocation1
        {
            get { return _listLocation1; }
            set { SetProperty(ref _listLocation1, value); }
        }
        private string _workgroup;
        public string Workgroup
        {
            get { return _workgroup; }
            set { SetProperty(ref _workgroup, value); }
        }
        private string _location1;
        public string Location1
        {
            get { return _location1; }
            set { SetProperty(ref _location1, value); }
        }

        public string InventoryMaintPolicyId
        {
            get
            {
                if (_cooperatorGroupIndex > -1 && _listWorkgroup != null && _listWorkgroup.Count > _cooperatorGroupIndex)
                {
                    return _listWorkgroup[_cooperatorGroupIndex].inventory_maint_policy_id.ToString();
                }
                else
                    return "-1";
            }
        }

        private int _cooperatorGroupIndex;
        public int CooperatorGroupIndex
        {
            get { return _cooperatorGroupIndex; }
            set { SetProperty(ref _cooperatorGroupIndex, value); }
        }

        public DashboardPageViewModel(INavigationService navigationService, IPageDialogService pageDialogService) : base(navigationService)
        {
            _pageDialogService = pageDialogService;
            _restClient = new RestClient();

            Title = "Welcome";

            NavigateCommand = new DelegateCommand<string>(OnNavigateCommandExecuted);
            LogoutCommand = new DelegateCommand(OnLogoutCommandExecuted);

            ListWorkGroupChangedCommand = new DelegateCommand<string>(OnListWorkGroupChangedCommandExecuted);
            ListLocationChangedCommand = new DelegateCommand(OnListLocationChangedCommandExecuted);

            //_listWorkgroup = CodeValueFactory.GetWorkgroupList();
            //_listLocation = CodeValueFactory.GetLocationList();
        }

        public DelegateCommand LogoutCommand { get; }
        private async void OnLogoutCommandExecuted()
        {
            await NavigationService.NavigateAsync("/LoginPage");
        }

        public DelegateCommand<string> NavigateCommand { get; }
        private async void OnNavigateCommandExecuted(string path)
        {
            await NavigationService.NavigateAsync(path, animated: true);
        }

        public DelegateCommand<string> ListWorkGroupChangedCommand { get; }
        private async void OnListWorkGroupChangedCommandExecuted(string cooperatorId)
        {
            try
            {
                if (CooperatorGroupIndex > -1)
                {
                    Settings.CooperatorGroupIndex = CooperatorGroupIndex;
                    Settings.InventoryMaintPolicyId = int.Parse(InventoryMaintPolicyId);

                    List<Location> locations = await _restClient.GetLocations(InventoryMaintPolicyId);
                    ListLocation1 = locations.Select(l => l.storage_location_part1).Distinct().ToList();
                }
            }
            catch (Exception e)
            {
                await _pageDialogService.DisplayAlertAsync("Error", e.Message, "OK");
            }
        }
        public DelegateCommand ListLocationChangedCommand { get; }
        private void OnListLocationChangedCommandExecuted()
        {
            if (!string.IsNullOrEmpty(Location1))
                Settings.Location1 = Location1;
        }

        public override async void OnNavigatedTo(INavigationParameters parameters)
        {
            try
            {
                if (ListWorkgroup == null)
                {
                    ListWorkgroup = await _restClient.GetWorkGroups(Settings.CooperatorId);
                    CooperatorGroupIndex = Settings.CooperatorGroupIndex;
                }
            }
            catch (Exception e)
            {
                await _pageDialogService.DisplayAlertAsync("Error", e.Message, "OK");
            }
        }
    }
}

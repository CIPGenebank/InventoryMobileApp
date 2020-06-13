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
    public class WelcomePageViewModel : ViewModelBaseZ
    {
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
        private CooperatorGroup _workgroup;
        public CooperatorGroup Workgroup
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
        
        private int _cooperatorGroupIndex;
        public int CooperatorGroupIndex
        {
            get { return _cooperatorGroupIndex; }
            set { SetProperty(ref _cooperatorGroupIndex, value); }
        }

        public WelcomePageViewModel(INavigationService navigationService, IPageDialogService pageDialogService) : base(navigationService, pageDialogService)
        {
            _restClient = new RestClient();

            Title = "Home";

            NavigateCommand = new DelegateCommand<string>(OnNavigateCommandExecuted);
            LogoutCommand = new DelegateCommand(OnLogoutCommandExecuted);

            ListWorkGroupChangedCommand = new DelegateCommand(OnListWorkGroupChangedCommand);
            ListLocationChangedCommand = new DelegateCommand(OnListLocationChangedCommand);

        }

        public DelegateCommand LogoutCommand { get; }
        private async void OnLogoutCommandExecuted()
        {
            Settings.Token = string.Empty;
            await NavigationService.NavigateAsync("/LoginPage");
        }

        public DelegateCommand<string> NavigateCommand { get; }
        private async void OnNavigateCommandExecuted(string path)
        {
            try
            {
                if (Workgroup == null)
                    throw new Exception("Workgroup is empty");

                await NavigationService.NavigateAsync(path, animated: true);
            }
            catch (Exception e)
            {
                await PageDialogService.DisplayAlertAsync("Error", e.Message, "OK");
            }
        }

        public DelegateCommand ListWorkGroupChangedCommand { get; }
        private async void OnListWorkGroupChangedCommand()
        {
            try
            {
                if (Workgroup != null)
                    Settings.WorkgroupName = Workgroup.group_name;
            }
            catch (Exception e)
            {
                await PageDialogService.DisplayAlertAsync("Error", e.Message, "OK");
            }
        }
        public DelegateCommand ListLocationChangedCommand { get; }
        private async void OnListLocationChangedCommand()
        {
            try
            {
                if (Location1 != null)
                    Settings.Location1 = Location1;
            }
            catch (Exception ex)
            {
                await PageDialogService.DisplayAlertAsync("Error", ex.Message, "OK");
            }
        }

        public override async void OnNavigatedTo(INavigationParameters parameters)
        {
            try
            {
                //Load workgroups
                if (ListWorkgroup == null)
                {
                    ListWorkgroup = await _restClient.GetWorkGroups(Settings.CooperatorId);
                    Workgroup = ListWorkgroup.FirstOrDefault(g => g.group_name.Equals(Settings.WorkgroupName));
                    if (Workgroup == null)
                    {
                        Settings.WorkgroupName = string.Empty;
                        Settings.WorkgroupCooperatorId = -1;
                    }
                    else
                        Settings.WorkgroupCooperatorId = Workgroup.group_owned_by.HasValue ? Workgroup.group_owned_by.Value : -1;
                }
                //Load Location 1
                if (ListLocation1 == null)
                {
                    ListLocation1 = await _restClient.GetAllLocation1List();
                    Location1 = Settings.Location1;
                }
            }
            catch (Exception ex)
            {
                await PageDialogService.DisplayAlertAsync("Error", ex.Message, "OK");
            }
        }
    }
}

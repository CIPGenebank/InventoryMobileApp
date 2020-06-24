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
        private Dictionary<string, string> _appResource;

        #region Properties
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
        #endregion

        #region LangProperties
        private string _labelWorkgroup;
        public string LabelWorkgroup
        {
            get { return _labelWorkgroup; }
            set { SetProperty(ref _labelWorkgroup, value); }
        }
        #endregion

        public WelcomePageViewModel(INavigationService navigationService, IPageDialogService pageDialogService) : base(navigationService, pageDialogService)
        {
            _restClient = new RestClient();

            Title = "Home";

            NavigateCommand = new DelegateCommand<string>(OnNavigateCommandExecuted);
            LogoutCommand = new DelegateCommand(OnLogoutCommandExecuted);

            ListWorkGroupChangedCommand = new DelegateCommand(OnListWorkGroupChangedCommand);
            ListLocationChangedCommand = new DelegateCommand(OnListLocationChangedCommand);

            LabelWorkgroup = "Workgroup";
        }

        public DelegateCommand LogoutCommand { get; }
        private async void OnLogoutCommandExecuted()
        {
            Settings.UserToken = string.Empty;
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
                {
                    Settings.WorkgroupName = Workgroup.group_name;
                    Settings.WorkgroupInvMaintPolicies = Workgroup.inv_maint_policy_ids;
                    Settings.WorkgroupInventoryDataview = Workgroup.inventory_dataview;
                    Settings.WorkgroupInventoryThumbnailDataview = Workgroup.inventory_thumbnail_dataview;
                }
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
                    ListWorkgroup = await _restClient.GetWorkGroups(Settings.UserCooperatorId);
                    Workgroup = ListWorkgroup.FirstOrDefault(g => g.group_name.Equals(Settings.WorkgroupName));
                    if (Workgroup == null)
                    {
                        Settings.WorkgroupName = string.Empty;
                        Settings.WorkgroupInvMaintPolicies = null;
                        Settings.WorkgroupInventoryDataview = null;
                        Settings.WorkgroupInventoryThumbnailDataview = null;
                    }
                    else
                    {
                        Settings.WorkgroupInvMaintPolicies = Workgroup.inv_maint_policy_ids;
                        Settings.WorkgroupInventoryDataview = Workgroup.inventory_dataview;
                        Settings.WorkgroupInventoryThumbnailDataview = Workgroup.inventory_thumbnail_dataview;
                    }
                }
                //Load Location 1
                if (ListLocation1 == null)
                {
                    ListLocation1 = await _restClient.GetAllLocation1List();
                    Location1 = Settings.Location1;
                }

                if (_appResource == null)
                {
                    _appResource = await _restClient.GetAppResources("WelcomePage", Settings.LangId);

                    
                    if (_appResource.ContainsKey("LabelWorkgroup")) LabelWorkgroup = _appResource["LabelWorkgroup"];
                    
                }
            }
            catch (Exception ex)
            {
                await PageDialogService.DisplayAlertAsync("Error", ex.Message, "OK");
            }
        }
    }
}

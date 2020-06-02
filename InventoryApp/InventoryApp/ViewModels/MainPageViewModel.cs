using InventoryApp.Helpers;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Navigation;
using Prism.Services;
using System;
using System.Collections.Generic;
using System.Linq;

namespace InventoryApp.ViewModels
{
    public class MainPageViewModel : ViewModelBase
    {
        private List<MenuItem> _menuList;
        public List<MenuItem> MenuList
        {
            get { return _menuList; }
            set { SetProperty(ref _menuList, value); }
        }

        private MenuItem _selectedMenu;
        public MenuItem SelectedMenuItem
        {
            get { return _selectedMenu; }
            set { SetProperty(ref _selectedMenu, value); }
        }

        private string _userName;
        public string UserName
        {
            get { return _userName; }
            set { SetProperty(ref _userName, value); }
        }
        public MainPageViewModel(INavigationService navigationService, IPageDialogService pageDialogService)
            : base(navigationService)
        {
            _menuList = new List<MenuItem> { new MenuItem { Text = "Welcome", Path = "NavigationPage/WelcomePage"} ,
                new MenuItem { Text = "Inventory list", Path="NavigationPage/InventoriesPage" },
                //new MenuItem { Text = "Inventories(previous)", Path = "NavigationPage/PreviousInventoriesPage"},
                /*new MenuItem { Text = "Dataviews", Path = "NavigationPage/WelcomePage"},*/
                new MenuItem { Text = "Viability Tests", Path = "NavigationPage/ViabilityTestsPage" },
                new MenuItem { Text = "Settings", Path = "NavigationPage/SettingsPage" },
                new MenuItem { Text = "About", Path = "NavigationPage/About" }
            };

            NavigateCommand = new DelegateCommand<string>(OnNavigateCommandExecuted);
            LogoutCommand = new DelegateCommand(OnLogoutCommandExecuted);

            ItemTappedCommand = new DelegateCommand(OnItemTappedCommandExecuted);

            UserName = Settings.Username;
        }

        public DelegateCommand ItemTappedCommand { get; }
        private async void OnItemTappedCommandExecuted()
        {
            if (!string.IsNullOrEmpty(SelectedMenuItem.Path))
            {
                await NavigationService.NavigateAsync(SelectedMenuItem.Path, null);
            }
        }

        public DelegateCommand<string> NavigateCommand { get; }
        private async void OnNavigateCommandExecuted(string path)
        {
            await NavigationService.NavigateAsync(path);
        }

        public override void OnNavigatedTo(INavigationParameters parameters)
        {
            if (parameters.ContainsKey("Username"))
            {
                UserName = (string)parameters["Username"];
            }
            if (parameters.ContainsKey("SelectedMenuItemIndex") && parameters["SelectedMenuItemIndex"] != null && !string.IsNullOrWhiteSpace(parameters["SelectedMenuItemIndex"].ToString()))
            {
                int selectedHomeMenuItemIndex = int.TryParse(parameters["SelectedMenuItemIndex"].ToString(), out selectedHomeMenuItemIndex) ? selectedHomeMenuItemIndex : -1;
                if (selectedHomeMenuItemIndex > -1 && selectedHomeMenuItemIndex < _menuList.Count)
                    SelectedMenuItem = _menuList[selectedHomeMenuItemIndex];
            }
        }

        public DelegateCommand LogoutCommand { get; }
        private async void OnLogoutCommandExecuted()
        {
            await NavigationService.NavigateAsync("/LoginPage");
        }

    }

    public class MenuItem
    {
        public string Text { get; set; }
        public string Path { get; set; }
    }
}

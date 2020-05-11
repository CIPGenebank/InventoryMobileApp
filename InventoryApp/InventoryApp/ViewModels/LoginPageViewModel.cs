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
    public class LoginPageViewModel : ViewModelBase
    {
        IPageDialogService PageDialogService { get; }
        private readonly RestClient _restClient;

        private string _username;
        public string UserName
        {
            get { return _username; }
            set { SetProperty(ref _username, value); }
        }

        private string _password;
        public string Password
        {
            get { return _password; }
            set { SetProperty(ref _password, value); }
        }

        private string _server;
        public string Server
        {
            get { return _server; }
            set { SetProperty(ref _server, value); }
        }
        public LoginPageViewModel(INavigationService navigationService, IPageDialogService pageDialogService)
            : base(navigationService)
        {
            PageDialogService = pageDialogService;
            _restClient = new RestClient();

            LoginCommand = new DelegateCommand(OnLoginCommandExecuted).ObservesCanExecute(() => IsNotBusy);
                //.ObservesCanExecute(() => IsNotBusy);
                //.ObservesProperty(() => UserName)
                //ObservesProperty(() => Password)

            ServerChangedCommand = new DelegateCommand(OnServerChangedCommandExecuted);

            UserName = "cvelasquez";
            Password = "password";
            Server = "192.168.137.1";
        }

        public DelegateCommand ServerChangedCommand { get; }
        private void OnServerChangedCommandExecuted()
        {
            System.Diagnostics.Debug.WriteLine("OnServerChangedCommandExecuted");
            if (!Server.Equals(Settings.Server))
                 Settings.Server = Server;
        }

        public DelegateCommand LoginCommand { get; }
        private async void OnLoginCommandExecuted()
        {
            IsBusy = true;
            try
            {
                if (!string.IsNullOrEmpty(Server) && !string.IsNullOrEmpty(UserName) && !string.IsNullOrEmpty(Password))
                {
                    Login token = await _restClient.Login(UserName, Password, Server);
                    if (!string.IsNullOrEmpty(token.Token))
                    {
                        Settings.Username = UserName;
                        Settings.Token = token.Token;
                        Settings.CooperatorId = token.CooperatorId.GetValueOrDefault(-1);

                        if (Xamarin.Forms.Device.Idiom == Xamarin.Forms.TargetIdiom.Desktop)
                        {
                            await NavigationService.NavigateAsync("/MainPage/NavigationPage/DashboardPage");
                        }
                        else
                        {
                            await NavigationService.NavigateAsync("/MainPage");
                        }
                    }
                    else
                    {
                        await PageDialogService.DisplayAlertAsync("Login Error", token.Error, "OK");
                    }
                }
                else
                {
                    await PageDialogService.DisplayAlertAsync("Login Error", "Server, Username and Password can not be empty", "OK");
                }
            }
            catch (Exception e)
            {
                await PageDialogService.DisplayAlertAsync("System Error", e.Message, "OK");
            }
            finally 
            {
                IsBusy = false;
            }
        }
    }
}

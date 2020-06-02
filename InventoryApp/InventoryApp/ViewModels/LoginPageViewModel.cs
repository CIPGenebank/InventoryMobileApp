using ImTools;
using InventoryApp.Helpers;
using InventoryApp.Models;
using InventoryApp.Services;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Navigation;
using Prism.Services;
using Prism.Services.Dialogs;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using ZXing.PDF417;

namespace InventoryApp.ViewModels
{
    public class LoginPageViewModel : ViewModelBase
    {
        IPageDialogService PageDialogService { get; }
        IDialogService DialogService { get; }
        private readonly RestClient _restClient;

        private string _username = string.Empty;
        public string UserName
        {
            get { return _username; }
            set { SetProperty(ref _username, value); }
        }

        private string _password = string.Empty;
        public string Password
        {
            get { return _password; }
            set { SetProperty(ref _password, value); }
        }

        private string _server = string.Empty;
        public string Server
        {
            get { return _server; }
            set { SetProperty(ref _server, value); }
        }
        private List<KeyValuePair<int,string>> _langList;
        public List<KeyValuePair<int, string>> LangList
        {
            get { return _langList; }
            set { SetProperty(ref _langList, value); }
        }

        private KeyValuePair<int,string> _lang;
        public KeyValuePair<int,string> Lang
        {
            get { return _lang; }
            set { SetProperty(ref _lang, value); }
        }

        private ObservableCollection<string> _serverList;
        public ObservableCollection<string> ServerList
        {
            get { return _serverList; }
            set { SetProperty(ref _serverList, value); }
        }
        public LoginPageViewModel(INavigationService navigationService, IPageDialogService pageDialogService, IDialogService dialogService)
            : base(navigationService)
        {
            PageDialogService = pageDialogService;
            DialogService = dialogService;
            _restClient = new RestClient();

            LoginCommand = new DelegateCommand(OnLoginCommandExecuted).ObservesCanExecute(() => IsNotBusy);
            //.ObservesCanExecute(() => IsNotBusy);
            //.ObservesProperty(() => UserName)
            //ObservesProperty(() => Password)

            ServerChangedCommand = new DelegateCommand(OnServerChangedCommandExecuted);
            LangChangedCommand = new DelegateCommand(OnLangChangedCommandAsync);
            AddServerCommand = new DelegateCommand(OnAddServerCommand);

            if (string.IsNullOrEmpty(Settings.ServerList))
            {
                _serverList = new ObservableCollection<string> { "localhost", "genebank.cipotato.org", "192.168.137.1", "10.0.2.2" };
            }
            else {
                _serverList = new ObservableCollection<string>(Settings.ServerList.Split(new char[] { ',' }).ToList<string>());
            }

            _langList = new Dictionary<int, string> { { 1, "English" }, { 2, "Español" } }.ToList();

            _username = Settings.Username;
            _server = Settings.Server;
            if (Settings.Lang > 0) 
            {
                Lang = _langList.FirstOrDefault(l => l.Key == Settings.Lang);
            }
            
        }

        public DelegateCommand AddServerCommand { get; }
        private async void OnAddServerCommand()
        {
            try
            {
                DialogService.ShowDialog("EditServerListDialog", OnDialogClosed);
            }
            catch (Exception ex)
            {
                await PageDialogService.DisplayAlertAsync("Error", ex.Message, "OK");
            }
        }
        private void OnDialogClosed(IDialogResult result)
        {
            // Check for exception
            if (result.Exception != null)
            {
                var e = result.Exception;
                Console.WriteLine($"Dialog failed. {(e.InnerException ?? e).Message}");
                return;
            }

            // Fetch parameters returned by the dialog view
            if (result.Parameters.ContainsKey("ServerName"))
            {
                string newServer = result.Parameters.GetValue<string>("ServerName");
                if (ServerList.Contains(newServer))
                {
                    Console.WriteLine($"Server exists");
                }
                else {
                    ServerList.Add(newServer);
                    Settings.ServerList = String.Join(",", ServerList);
                    
                    Server = newServer;
                }
            }   
        }
        public DelegateCommand LangChangedCommand { get; }
        private async void OnLangChangedCommandAsync() {
            try
            {
                if (Lang.Key != Settings.Lang)
                    Settings.Lang = Lang.Key;
            }
            catch (Exception ex)
            {
                await PageDialogService.DisplayAlertAsync("Error", ex.Message, "OK");
            }
        }

        public DelegateCommand ServerChangedCommand { get; }
        private void OnServerChangedCommandExecuted()
        {
            System.Diagnostics.Debug.WriteLine("OnServerChangedCommandExecuted");
            if (_server != null && !_server.Equals(Settings.Server))
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
                        Settings.Server = Server;

                        await NavigationService.NavigateAsync("/MainPage/NavigationPage/WelcomePage");
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

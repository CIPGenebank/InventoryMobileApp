﻿using DryIoc;
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
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.Essentials;

namespace InventoryApp.ViewModels
{
    public class LoginPageViewModel : ViewModelBase
    {
        IPageDialogService PageDialogService { get; }
        IDialogService DialogService { get; }
        private readonly RestClient _restClient;
        private Dictionary<string, string> _appResource;
        
        #region Properties
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
        private List<CodeValueLookup> _langList;
        public List<CodeValueLookup> LangList
        {
            get { return _langList; }
            set { SetProperty(ref _langList, value); }
        }

        private CodeValueLookup _lang;
        public CodeValueLookup Lang
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
        #endregion

        #region LangProperties
        private string _labelUsername;
        public string LabelUsername
        {
            get { return _labelUsername; }
            set { SetProperty(ref _labelUsername, value); }
        }

        private string _labelPassword;
        public string LabelPassword
        {
            get { return _labelPassword; }
            set { SetProperty(ref _labelPassword, value); }
        }
        private string _labelServer;
        public string LabelServer
        {
            get { return _labelServer; }
            set { SetProperty(ref _labelServer, value); }
        }
        private string _labelLang;
        public string LabelLang
        {
            get { return _labelLang; }
            set { SetProperty(ref _labelLang, value); }
        }
        private string _labelVersion;
        public string LabelVersion
        {
            get { return _labelVersion; }
            set { SetProperty(ref _labelVersion, value); }
        }
        private string _buttonLogIn;
        public string ButtonLogIn
        {
            get { return _buttonLogIn; }
            set { SetProperty(ref _buttonLogIn, value); }
        }
        private string _buttonChangePassword;
        public string ButtonChangePassword
        {
            get { return _buttonChangePassword; }
            set { SetProperty(ref _buttonChangePassword, value); }
        }
        #endregion

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
            ChangePasswordCommand = new DelegateCommand(OnChangePasswordCommand);

            if (string.IsNullOrEmpty(Settings.ServerList))
            {
                _serverList = new ObservableCollection<string> { "localhost", "genebank.cipotato.org", "192.168.137.1", "10.0.2.2" };
            }
            else {
                _serverList = new ObservableCollection<string>(Settings.ServerList.Split(new char[] { ',' }).ToList<string>());
            }

            _username = Settings.Username;
            _server = Settings.Server;
            Settings.UserToken = string.Empty;
            //_password = "password";
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
                if (int.Parse(Lang.value_member) != Settings.LangId)
                {
                    Settings.LangId = int.Parse(Lang.value_member);

                    _appResource = null;
                    _appResource = await _restClient.GetAppResources("LoginPage", Settings.LangId);

                    LabelUsername = GetLangLabel(nameof(LabelUsername));
                    LabelPassword = GetLangLabel(nameof(LabelPassword));
                    LabelServer = GetLangLabel(nameof(LabelServer));
                    LabelLang = GetLangLabel(nameof(LabelLang));
                    LabelVersion = string.Format(GetLangLabel(nameof(LabelVersion)), VersionTracking.CurrentVersion);
                    ButtonLogIn = GetLangLabel(nameof(ButtonLogIn));
                    ButtonChangePassword = GetLangLabel(nameof(ButtonChangePassword));
                }
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
                        Settings.UserToken = token.Token;
                        Settings.UserCooperatorId = token.CooperatorId.GetValueOrDefault(-1);
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

        public DelegateCommand ChangePasswordCommand { get; }
        private async void OnChangePasswordCommand()
        {
            try
            {
                await PageDialogService.DisplayAlertAsync("Message", "Under construction", "OK");
            }
            catch (Exception ex)
            {
                await PageDialogService.DisplayAlertAsync("System Error", ex.Message, "OK");
            }
        }
        public override async void OnNavigatedTo(INavigationParameters parameters)
        {
            base.OnNavigatedTo(parameters);
            try
            {
                if (LangList == null)
                {
                    LangList = new List<CodeValueLookup>()
                    {
                        new CodeValueLookup{ value_member = "1", display_member = "English"},
                        new CodeValueLookup{ value_member = "2", display_member = "Español"}/*,
                        new CodeValueLookup{ value_member = "3", display_member = "Français"},
                        new CodeValueLookup{ value_member = "4", display_member = @"العربية"},
                        new CodeValueLookup{ value_member = "5", display_member = @"Русский"},
                        new CodeValueLookup{ value_member = "6", display_member = @"Português"},
                        new CodeValueLookup{ value_member = "7", display_member = @"Český"},
                        new CodeValueLookup{ value_member = "9", display_member = "ENG"}*/
                    };
                }
                if (Lang == null)
                {
                    if (Settings.LangId > 0)
                        Lang = _langList.FirstOrDefault(l => l.value_member.Equals(Settings.LangId.ToString()));
                    else
                    {
                        Lang = LangList[0];
                        Settings.LangId = Lang.sys_lang_id;
                    }
                }

                if (_appResource == null)
                {
                    _appResource = await _restClient.GetAppResources("LoginPage", Settings.LangId);
                    
                    LabelUsername = GetLangLabel(nameof(LabelUsername));
                    LabelPassword = GetLangLabel(nameof(LabelPassword));
                    LabelServer = GetLangLabel(nameof(LabelServer));
                    LabelLang = GetLangLabel(nameof(LabelLang));
                    LabelVersion = string.Format(GetLangLabel(nameof(LabelVersion)), VersionTracking.CurrentVersion);
                    ButtonLogIn = GetLangLabel(nameof(ButtonLogIn));
                    ButtonChangePassword = GetLangLabel(nameof(ButtonChangePassword));
                }
            }
            catch (Exception ex)
            {
                await PageDialogService.DisplayAlertAsync("Error", ex.Message, "OK");
            }
        }

        private string GetLangLabel(string key)
        {
            string value = string.Empty;
            if (_appResource == null || _appResource.Count == 0)
            {
                _appResource = new Dictionary<string, string>();
                if (Settings.LangId == 1)
                {
                    _appResource.Add(nameof(LabelUsername), "Username");
                    _appResource.Add(nameof(LabelPassword), "Password");
                    _appResource.Add(nameof(LabelServer), "Connect To");
                    _appResource.Add(nameof(LabelLang), "Lang");
                    _appResource.Add(nameof(LabelVersion), "Version {0}");
                    _appResource.Add(nameof(ButtonLogIn), "Log In");
                    _appResource.Add(nameof(ButtonChangePassword), "Change password");
                }
                else
                {
                    _appResource.Add(nameof(LabelUsername), "Usuario");
                    _appResource.Add(nameof(LabelPassword), "Contraseña");
                    _appResource.Add(nameof(LabelServer), "Conectar a");
                    _appResource.Add(nameof(LabelLang), "Idioma");
                    _appResource.Add(nameof(LabelVersion), "Versión {0}");
                    _appResource.Add(nameof(ButtonLogIn), "Ingresar");
                    _appResource.Add(nameof(ButtonChangePassword), "Cambiar contraseña");
                }
            }
            if (_appResource.ContainsKey(key))
            {
                value = _appResource[key];
            }
            return value;
        }
    }
}

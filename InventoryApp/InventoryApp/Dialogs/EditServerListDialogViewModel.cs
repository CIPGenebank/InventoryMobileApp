using Prism.Commands;
using Prism.Mvvm;
using Prism.Services.Dialogs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace InventoryApp.Dialogs
{
    public class EditServerListDialogViewModel : BindableBase, IDialogAware
    {
        private string _server;
        public string Server
        {
            get { return _server; }
            set { SetProperty(ref _server, value); RaisePropertyChanged(nameof(CanContinue)); }
        }

        private string _title;
        public string Title
        {
            get { return _title; }
            set { SetProperty(ref _title, value); }
        }
        private string _message;
        public string Message
        {
            get => _message;
            set => SetProperty(ref _message, value);
        }

        /*private bool _canContinue;
        public bool CanContinue
        {
            get { return _canContinue; }
            set { SetProperty(ref _canContinue, value); }
        }*/
        public EditServerListDialogViewModel()
        {
            Title = "Edit Server List";
            Message = "Server name:";
            SubmitCommand = new DelegateCommand(OnSubmitTapped).ObservesCanExecute(() => CanContinue);
            CancelCommand = new DelegateCommand(OnCancelTapped);
        }

        public DelegateCommand SubmitCommand { get; }
        public DelegateCommand CancelCommand { get; }
        public bool CanContinue { 
            get { return !string.IsNullOrEmpty(_server); } 
        }

        private void OnSubmitTapped()
        {
            RequestClose(new DialogParameters { { "ServerName", Server } });
        }

        private bool _cancelled;
        private void OnCancelTapped()
        {
            _cancelled = true;
            RequestClose(null);
        }

        public event Action<IDialogParameters> RequestClose;

        public bool CanCloseDialog() => _cancelled || CanContinue;
        //public bool CanCloseDialog() => true;

        public void OnDialogClosed()
        {
            Console.WriteLine("The Demo Dialog has been closed...");
        }

        public void OnDialogOpened(IDialogParameters parameters)
        {
            if (parameters.ContainsKey("title"))
                Message = parameters.GetValue<string>("title");
            if (parameters.ContainsKey("message"))
                Message = parameters.GetValue<string>("message");
        }
    }
}

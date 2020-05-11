using InventoryApp.Models;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Navigation;
using System;
using System.Collections.Generic;
using System.Linq;
using Xamarin.Forms;

namespace InventoryApp.ViewModels
{
    public class LookupPickerPageViewModel : ViewModelBase
    {
        private EntityAttribute _attribute;
        public LookupPickerPageViewModel(INavigationService navigationService)
            : base(navigationService)
        {

        }
        public override void OnNavigatedTo(INavigationParameters parameters)
        {
            try
            {
                if (parameters.ContainsKey("EntityAttribute"))
                    _attribute = (EntityAttribute)parameters["EntityAttribute"];
            }
            catch (Exception ex)
            {
                Title = ex.Message;
            }
        }
    }
}

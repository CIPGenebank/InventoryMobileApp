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
    public class InventoryActionsPageViewModel : ViewModelBaseZ
    {
        private List<InventoryThumbnail> _inventoryList;
        private readonly RestClient _restClient;

        private List<InventoryAction> _inventoryActionList;
        public List<InventoryAction> InventoryActionList
        {
            get { return _inventoryActionList; }
            set { SetProperty(ref _inventoryActionList, value); }
        }

        public InventoryActionsPageViewModel(INavigationService navigationService, IPageDialogService pageDialogService)
            : base(navigationService, pageDialogService)
        {
            _restClient = new RestClient();
            Title = "Inventory actions";
        }

        public override async void OnNavigatedTo(INavigationParameters parameters)
        {
            try
            {
                if (parameters.ContainsKey("InventoryThumbnailList"))
                {
                    _inventoryList = (List<InventoryThumbnail>)parameters["InventoryThumbnailList"];

                    var idList = _inventoryList.Select(i => i.inventory_id).ToList();
                    InventoryActionList = await _restClient.GetInvitroActionList(string.Join(",",idList));
                }
            }
            catch (Exception ex)
            {
                await PageDialogService.DisplayAlertAsync("Error", ex.Message, "OK");
            }
        }
    }
}

using InventoryApp.Helpers;
using InventoryApp.Interfaces;
using InventoryApp.Models;
using InventoryApp.Models.Database;
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
    public class NewInventoryPageViewModel : ViewModelBase
    {
        IPageDialogService _pageDialogService { get; }
        private readonly RestClient _restClient;
        IRestService _restService { get; }

        private AccessionThumbnail _accessionThumbnail;
        public AccessionThumbnail AccessionThumbnail
        {
            get { return _accessionThumbnail; }
            set { SetProperty(ref _accessionThumbnail, value); }
        }

        private Inventory _inventory;
        public Inventory Inventory
        {
            get { return _inventory; }
            set { SetProperty(ref _inventory, value); }
        }

        public NewInventoryPageViewModel(INavigationService navigationService, IPageDialogService pageDialogService)
            : base(navigationService)
        {
            Title = "New Inventory";

            _pageDialogService = pageDialogService;
            _restClient = new RestClient();
            _restService = new RestService();

            Inventory = new Inventory()
            {
                inventory_id = -1,
                inventory_maint_policy_id = Settings.WorkgroupCooperatorId,
                parent_inventory_id = null,
                backup_inventory_id = null,

                quantity_on_hand = 0,

                is_available = "N",
                is_distributable = "N",
                availability_status_code = "NOAVAIL",

                propagation_date = DateTime.Now,
                propagation_date_code = "MM/dd/yyyy"

                ,
                inventory_number_part2 = -1
                ,
                storage_location_part1 = Settings.Location1
            };

            SaveCommand = new DelegateCommand(OnSaveCommandExecuted);
            SearchCommand = new DelegateCommand(OnSearchCommandExecuted);
        }
        public DelegateCommand SaveCommand { get; }
        public DelegateCommand SearchCommand { get; }

        private async void OnSaveCommandExecuted()
        {
            try
            {
                if (AccessionThumbnail == null)
                {
                    await _pageDialogService.DisplayAlertAsync("Message", "Accession is empty", "OK");
                    return;
                }

                if (string.IsNullOrEmpty(Inventory.storage_location_part1))
                    Inventory.storage_location_part1 = "";

                string result = await _restClient.CreateInventory(Inventory);
                await _pageDialogService.DisplayAlertAsync("New Inventory Id", result, "OK");

                /*
                if( Inventory.inventory_number_part2 != int.Parse(result))
                {
                    Inventory.inventory_number_part2 = int.Parse(result);
                    await _restService.UpdateInventoryAsync(Inventory);
                }
                */

                List<InventoryThumbnail> newInventoryList = await _restClient.Search("@inventory.inventory_id = " + result, "get_mob_inventory", "inventory");
                InventoryThumbnail newInventory = newInventoryList[0];

                var navigationParams = new NavigationParameters();
                navigationParams.Add("InventoryThumbnail", newInventory);
                //navigationParams.Add("message", "Successfully saved");

                await NavigationService.GoBackAsync(navigationParams);
            }
            catch (Exception ex)
            {
                await _pageDialogService.DisplayAlertAsync("Error", ex.Message, "OK");
            }
        }

        private async void OnSearchCommandExecuted()
        {
            await NavigationService.NavigateAsync("SearchAccessionPage");
        }

        public override async void OnNavigatedTo(INavigationParameters parameters)
        {
            try
            {
                if (parameters.ContainsKey("accessionThumbnail"))
                {
                    AccessionThumbnail = (AccessionThumbnail)parameters["accessionThumbnail"];

                    Inventory.accession_id = AccessionThumbnail.accession_id;
                    Inventory.inventory_number_part1 = AccessionThumbnail.accession_number_part2 + (!string.IsNullOrEmpty(AccessionThumbnail.accession_number_part3) ? "." + AccessionThumbnail.accession_number_part3 : "");

                    Inventory.acc_name_col = AccessionThumbnail.acc_name_col;
                    Inventory.acc_name_cul = AccessionThumbnail.acc_name_cul;
                }

                if (Inventory.inventory_number_part2 == -1)
                {
                    string result = await _restService.GetNewInventoryID();
                    Inventory.inventory_number_part2 = int.Parse(result);
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}

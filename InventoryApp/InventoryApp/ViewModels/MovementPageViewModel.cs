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
    public class MovementPageViewModel : ViewModelBase
    {
        IPageDialogService _pageDialogService { get; }
        private List<InventoryThumbnail> _inventoryList;
        private readonly RestClient _restClient;

        private string _quantityUnitCode;
        public string QuantityUnitCode
        {
            get { return _quantityUnitCode; }
            set { SetProperty(ref _quantityUnitCode, value); }
        }
        private string _note;
        public string Note
        {
            get { return _note; }
            set { SetProperty(ref _note, value); }
        }
        private int _quantity;
        public int Quantity
        {
            get { return _quantity; }
            set { SetProperty(ref _quantity, value); }
        }

        public List<CodeValue> ActionNameCodeList
        {
            get { return CodeValueFactory.ActionNameCodeList; }
        }
        public List<CodeValue> MethodList
        {
            get { return CodeValueFactory.MethodList; }
        }
        private int _actionNameCodeIndex;
        public int ActionNameCodeIndex
        {
            get { return _actionNameCodeIndex; }
            set { SetProperty(ref _actionNameCodeIndex, value); }
        }
        private int _methodIndex;
        public int MethodIndex
        {
            get { return _methodIndex; }
            set { SetProperty(ref _methodIndex, value); }
        }
        private DateTime _actionDate;
        public DateTime ActionDate
        {
            get { return _actionDate; }
            set { SetProperty(ref _actionDate, value); }
        }

        private bool _isCalculated;
        public bool IsCalculated
        {
            get { return _isCalculated; }
            set { SetProperty(ref _isCalculated, value, () => RaisePropertyChanged(nameof(IsNotCalculated))); }
        }
        public bool IsNotCalculated
        {
            get { return !IsCalculated; }
        }
        private decimal _seedsWeight;
        public decimal SeedsWeight
        {
            get { return _seedsWeight; }
            set { SetProperty(ref _seedsWeight, value); }
        }
        private decimal _seedsWeight100;
        public decimal SeedsWeight100
        {
            get { return _seedsWeight100; }
            set { SetProperty(ref _seedsWeight100, value); }
        }

        public MovementPageViewModel(INavigationService navigationService, IPageDialogService pageDialogService) : base(navigationService)
        {
            _pageDialogService = pageDialogService;
            _restClient = new RestClient();

            Title = "Increase / Discount";

            SaveCommand = new DelegateCommand(OnSaveCommandExecuted);
            CalculateCommand = new DelegateCommand(OnCalculateCommandExecuted);

            QuantityUnitCode = "count";
            Note = "";
            _methodIndex = 0;
            _actionNameCodeIndex = 0;
            _actionDate = DateTime.Now;
        }

        public DelegateCommand SaveCommand { get; }

        private async void OnSaveCommandExecuted()
        {
            await _pageDialogService.DisplayAlertAsync("Inventory List Count", _inventoryList.Count.ToString(), "OK");

            foreach (InventoryThumbnail inventory in _inventoryList)
            {
                try
                {
                    int quantity = (ActionNameCodeList[ActionNameCodeIndex].Code.Equals("DISCOUNT")) ? Math.Abs(Quantity) * -1 : Math.Abs(Quantity);
                    InventoryAction ia = new InventoryAction
                    {
                        inventory_action_id = -1,
                        inventory_id = inventory.inventory_id,
                        action_name_code = ActionNameCodeList[ActionNameCodeIndex].Code,
                        quantity = quantity,
                        quantity_unit_code = QuantityUnitCode,
                        action_date = ActionDate,
                        method_id = int.Parse(MethodList[MethodIndex].Code),
                        note = Note
                    };
                    await _restClient.CreateInventoryAction(ia);

                    inventory.quantity_on_hand += quantity;
                }
                catch (Exception ex)
                {
                    await _pageDialogService.DisplayAlertAsync("Error", ex.Message, "OK");
                }
            }
            /*var answer = await _pageDialogService.DisplayAlertAsync("Changes saved", "Do you want to go back to Inventories List Page", "YES", "NO");
            if (answer)
            {*/
            var navigationParams = new NavigationParameters();
            navigationParams.Add("inventoryList", _inventoryList);
            await NavigationService.GoBackAsync(navigationParams);
            //}
        }

        public DelegateCommand CalculateCommand { get; }
        private async void OnCalculateCommandExecuted()
        {
            try
            {
                if (SeedsWeight100 > 0)
                    Quantity = (int)(Math.Abs(SeedsWeight) / Math.Abs(SeedsWeight100) * 100);
            }
            catch (Exception ex)
            {
                await _pageDialogService.DisplayAlertAsync("Error", ex.Message, "OK");
            }
        }

        public override async void OnNavigatedTo(INavigationParameters parameters)
        {
            try
            {
                if (parameters.ContainsKey("inventoryList"))
                {
                    _inventoryList = (List<InventoryThumbnail>)parameters["inventoryList"];
                }
            }
            catch (Exception ex)
            {
                await _pageDialogService.DisplayAlertAsync("Error", ex.Message, "OK");
            }
        }
    }
}

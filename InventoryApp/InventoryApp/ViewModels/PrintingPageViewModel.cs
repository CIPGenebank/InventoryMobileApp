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
using System.Text;

namespace InventoryApp.ViewModels
{
    public class PrintingPageViewModel : ViewModelBase
    {
        private Printer _selectedPrinter;
        public Printer SelectedPrinter
        {
            get { return _selectedPrinter; }
            set { SetProperty(ref _selectedPrinter, value); }
        }

        private int _copies;
        public int Copies
        {
            get { return _copies; }
            set { SetProperty(ref _copies, value); }
        }
        private string _labelDesign;
        public string LabelDesign
        {
            get { return _labelDesign; }
            set { SetProperty(ref _labelDesign, value); }
        }

        private string _orderBy;
        public string OrderBy
        {
            get { return _orderBy; }
            set { SetProperty(ref _orderBy, value); }
        }

        public DelegateCommand CancelCommand { get; }
        public DelegateCommand PrintCommand { get; }

        IPageDialogService _pageDialogService { get; }
        private readonly RestClient _restClient;
        private List<InventoryThumbnail> _inventoryList;

        public PrintingPageViewModel(INavigationService navigationService, IPageDialogService pageDialogService)
            : base(navigationService)
        {
            _pageDialogService = pageDialogService;
            _restClient = new RestClient();

            Title = "Printing";

            LabelDesignList = CodeValueFactory.GetLabelDesignList();

            Copies = 1;
            LabelDesign = "Seed Label";

            CancelCommand = new DelegateCommand(OnCancelCommandExecuted);
            PrintCommand = new DelegateCommand(OnPrintCommandExecuted);

        }

        private async void OnPrintCommandExecuted()
        {
            try
            {
                Settings.PrinterIndex = PrinterIndex;

                switch (OrderBy)
                {
                    case "Accession Number":
                        _inventoryList = _inventoryList.OrderBy(x => x.AccessionNumber).ToList();
                        break;
                    case "Inventory Id":
                        _inventoryList = _inventoryList.OrderBy(x => x.inventory_id).ToList();
                        break;
                    case "Lot Id":
                        _inventoryList = _inventoryList.OrderBy(x => x.inventory_number_part2).ToList();
                        break;
                    case "Collecting Number":
                        _inventoryList = _inventoryList.OrderBy(x => x.acc_name_col).ToList();
                        break;
                    case "Accession Name":
                        _inventoryList = _inventoryList.OrderBy(x => x.acc_name_cul).ToList();
                        break;
                    default:
                        break;
                }

                var template = @"^LH{0},20 ^FO40,10  ^BXN,8,200 ^FD{1}^FS ^FO10,5   ^ADR,18 ^FD{1}^FS ^FO200,5  ^A0R,31 ^FD{2}^FS ^FO175,10 ^AER,28 ^FD{3}^FS ^LH{0},110 ^FO100,10 ^ADR,18 ^FD{4}^FS ^FO70,10  ^ADR,18 ^FD{5}^FS ^FO40,10  ^ADR,18 ^FD{6}^FS ^FO40,85  ^ADR,18 ^FD{7}^FS";

                if (PrinterIndex > -1)
                {
                    Printer printer = PrinterList[PrinterIndex];

                    int xPosition = 20;
                    StringBuilder sb = new StringBuilder();
                    for (int i = 0; i < _inventoryList.Count * Copies; i++)
                    {
                        switch (i % 3)
                        {
                            case 0:
                                sb.Append("^XA");
                                xPosition = 20;
                                break;
                            case 1:
                                xPosition = 285;
                                break;
                            case 2:
                                xPosition = 550;
                                break;
                        }
                        var inventory = _inventoryList[i / Copies];

                        var inventoryId = inventory.inventory_number_part2;
                        var accessionNumber = inventory.AccessionNumber;
                        var collectorNumber = inventory.acc_name_col;
                        var cultivarName = inventory.acc_name_cul;
                        var typeCrossName = inventory.pollination_method_code;
                        var year = "2017";
                        var taxonomySpeciesCode = inventory.taxonomy_species_code;

                        sb.Append(string.Format(template, xPosition, inventoryId, accessionNumber, collectorNumber, cultivarName, typeCrossName, year, taxonomySpeciesCode));

                        if (i % 3 == 2 || i == _inventoryList.Count * Copies - 1)
                        {
                            sb.Append("^XZ");
                        }
                    }

                    await _restClient.Print(printer.PrinterId, sb.ToString());

                    await _pageDialogService.DisplayAlertAsync("Printing Page", "Successfully printed", "OK");
                    await NavigationService.GoBackAsync();
                }
                else
                {
                    await _pageDialogService.DisplayAlertAsync("Error", "Select a printer", "OK");
                }
            }
            catch (Exception e)
            {
                await _pageDialogService.DisplayAlertAsync("Error", e.Message, "OK");
            }
        }

        private async void OnCancelCommandExecuted()
        {
            await NavigationService.GoBackAsync();
        }


        #region UI

        private List<Printer> _printerList;
        public List<Printer> PrinterList
        {
            get { return _printerList; }
            set { SetProperty(ref _printerList, value); }
        }

        private List<string> _labelDesignList;
        public List<string> LabelDesignList
        {
            get { return _labelDesignList; }
            set { SetProperty(ref _labelDesignList, value); }
        }
        private int _printerIndex;
        public int PrinterIndex
        {
            get { return _printerIndex; }
            set { SetProperty(ref _printerIndex, value); }
        }
        #endregion

        public override async void OnNavigatedTo(INavigationParameters parameters)
        {
            try
            {
                if (PrinterList == null)
                {
                    PrinterList = PrinterFactory.GetPrinterList();
                }
                if (Settings.PrinterIndex > -1 && Settings.PrinterIndex < PrinterList.Count)
                {
                    SelectedPrinter = PrinterList[Settings.PrinterIndex];
                }
                else
                    PrinterIndex = -1;

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

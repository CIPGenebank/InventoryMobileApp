using InventoryApp.Models;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Navigation;
using System;
using System.Collections.Generic;
using System.Linq;

namespace InventoryApp.ViewModels
{
    public class SettingsPageViewModel : ViewModelBase
    {
        private List<string> _printerList;
        public List<string> PrinterList
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

        private string _printer;
        public string Printer
        {
            get { return _printer; }
            set { SetProperty(ref _printer, value); }
        }

        private string _labelDesign;
        public string LabelDesign
        {
            get { return _labelDesign; }
            set { SetProperty(ref _labelDesign, value); }
        }

        private int _labelsNumber;
        public int LabelsNumber
        {
            get { return _labelsNumber; }
            set { SetProperty(ref _labelsNumber, value); }
        }

        public SettingsPageViewModel(INavigationService navigationService)
            : base(navigationService)
        {
            Title = "Settings";
            PrinterList = CodeValueFactory.GetPrinterList();
            LabelDesignList = CodeValueFactory.GetLabelDesignList();

            Printer = "\\\\CIP0977\\Printer";
            LabelsNumber = 1;
            LabelDesign = "Seed 001";
        }
    }
}

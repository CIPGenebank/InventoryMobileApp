using ImTools;
using InventoryApp.Helpers;
using InventoryApp.Models;
using InventoryApp.Services;
using Newtonsoft.Json;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Navigation;
using Prism.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Text.RegularExpressions;

namespace InventoryApp.ViewModels
{
    public class PrintingPageViewModel : ViewModelBase
    {
        private Printer _printer;
        public Printer Printer
        {
            get { return _printer; }
            set { SetProperty(ref _printer, value); }
        }

        private int _rowsPerRecord;
        public int RowsPerRecord
        {
            get { return _rowsPerRecord; }
            set { SetProperty(ref _rowsPerRecord, value); }
        }

        private LabelTemplate _labelTemplate;
        public LabelTemplate LabelTemplate
        {
            get { return _labelTemplate; }
            set { SetProperty(ref _labelTemplate, value); }
        }

        private List<Printer> _printerList;
        public List<Printer> PrinterList
        {
            get { return _printerList; }
            set { SetProperty(ref _printerList, value); }
        }

        private List<LabelTemplate> _labelTemplateList;
        public List<LabelTemplate> LabelTemplateList
        {
            get { return _labelTemplateList; }
            set { SetProperty(ref _labelTemplateList, value); }
        }

        private string _printingLog;
        public string PrintingLog
        {
            get { return _printingLog; }
            set { SetProperty(ref _printingLog, value); }
        }

        private bool _isLogVisible;
        public bool IsLogVisible
        {
            get { return _isLogVisible; }
            set { SetProperty(ref _isLogVisible, value); }
        }

        private string _orderBy;
        public string OrderBy
        {
            get { return _orderBy; }
            set { SetProperty(ref _orderBy, value); }
        }

        private bool _createInventoryAction;
        public bool CreateInventoryAction
        {
            get { return _createInventoryAction; }
            set { SetProperty(ref _createInventoryAction, value); }
        }
        public DelegateCommand CancelCommand { get; }
        public DelegateCommand PrintCommand { get; }

        IPageDialogService _pageDialogService { get; }
        private readonly RestClient _restClient;
        private List<InventoryThumbnail> _inventoryList;
        private int _labelIndex;
        private int _endRecordIndex;
        private List<string> _labelVariables = new List<string>();

        public PrintingPageViewModel(INavigationService navigationService, IPageDialogService pageDialogService)
            : base(navigationService)
        {
            _pageDialogService = pageDialogService;
            _restClient = new RestClient();

            Title = "Printing";

            RowsPerRecord = 1;
            IsLogVisible = false;
            CreateInventoryAction = false;

            CancelCommand = new DelegateCommand(OnCancelCommandExecuted);
            PrintCommand = new DelegateCommand(OnPrintCommandExecuted);
            LabelTemplatetChangedCommand = new DelegateCommand(OnLabelTemplatetChangedCommand);
        }

        public DelegateCommand LabelTemplatetChangedCommand { get; }
        private async void OnLabelTemplatetChangedCommand()
        {
            try
            {
                if (LabelTemplate != null)
                {
                    PrintingLog = JsonConvert.SerializeObject(LabelTemplate, Formatting.Indented);

                    _labelVariables.Clear();

                    Regex regex = new Regex("##" + "(.)*" + "##");
                    var matches = regex.Matches(LabelTemplate.Zpl);
                    foreach (var match in matches)
                    {
                        _labelVariables.Add(match.ToString()); //match.ToString().Replace(startMark,"").Replace(endMark,"");
                    }
                }
            }
            catch (Exception e)
            {
                await _pageDialogService.DisplayAlertAsync("Error", e.Message, "OK");
            }
        }
        private async void OnPrintCommandExecuted()
        {
            try
            {
                if (Printer == null)
                    throw new Exception("Printer is empty");
                if (LabelTemplate == null)
                    throw new Exception("Label template is empty");
                if (_inventoryList == null || _inventoryList.Count == 0)
                    throw new Exception("Printing inventory list is empty");

                Settings.Printer = Printer.PrinterName;

                var idList = _inventoryList.Select(x => x.inventory_id).ToList();
                var dataview = await _restClient.GetDataview(LabelTemplate.Dataview, ":accessionid;:accessioninvgroupid;:inventorymaintpolicyid;:orderrequestid;:inventoryid=" + string.Join(",", idList));

                _labelIndex = 0;
                //_startRecordIndex = 0;
                _endRecordIndex = dataview.Count - 1;

                string zpl = GenerateZPL(dataview);
                PrintingLog = "Printer URI:\n" + Printer.PrinterUri +"\n\n" + zpl;
                /*
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
                */
                
                await _restClient.Print(Printer.PrinterUri, Printer.PrinterConnectionType, zpl);

                await _pageDialogService.DisplayAlertAsync("Printing Page", "Successfully printed", "OK");
                //await NavigationService.GoBackAsync();

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


        public override async void OnNavigatedTo(INavigationParameters parameters)
        {
            try
            {
                if (PrinterList == null)
                {
                    PrinterList = await _restClient.GetPrinterList();
                }
                if (Printer == null)
                {
                    Printer = PrinterList.FirstOrDefault(p => Settings.Printer.Equals(p.PrinterName));
                    if (Printer == null)
                        Settings.Printer = string.Empty;
                }
                if (LabelTemplateList == null)
                {
                    LabelTemplateList = await _restClient.GetLabelTemplateList();
                }

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

        private string GenerateZPL(Newtonsoft.Json.Linq.JArray dataview)
        {
            string zpl = string.Empty;

            int printerDPI = LabelTemplate.Density;
            float labelWidth = (float)LabelTemplate.Width * printerDPI;
            float labelHeight = (float)LabelTemplate.Height * printerDPI;

            float marginLeft = (float)LabelTemplate.MarginLeft * printerDPI;
            float marginTop = (float)LabelTemplate.MarginTop * printerDPI;
            float horizontalGap = (float)LabelTemplate.HorizontalGap * printerDPI;
            float verticalGap = (float)LabelTemplate.VerticalGap * printerDPI;

            int labelsPerRecord = (int)RowsPerRecord;

            int columnsPerPage = (int)LabelTemplate.HorizontalCount;
            int rowsPerPage = 0;

            float paperHeight = (float)LabelTemplate.PaperHeight * printerDPI;

            if (labelHeight > paperHeight)
            {
                rowsPerPage = 1;
            }
            else if (2 * labelHeight + verticalGap > paperHeight) // 1 row
            {
                rowsPerPage = 1;
            }
            else //more than 1 row
            {
                rowsPerPage = 1 + (int)((paperHeight - labelHeight) / (labelHeight + verticalGap));
            }

            // Generate zpl
            string templateContent = LabelTemplate.Zpl.Replace("^XA", "").Replace("^XZ", "").Replace("\r\n\r\n", "\r\n").Trim();
            int zplX = 0, zplY = 0;
            int recordIndex = (_labelIndex / labelsPerRecord);

            while (recordIndex <= _endRecordIndex)
            {
                zpl += "^XA\n";
                for (int iRow = 0; iRow < rowsPerPage; iRow++)
                {
                    zplY = (int)(marginTop + iRow * (labelHeight + verticalGap));
                    for (int iCol = 0; iCol < columnsPerPage; iCol++)
                    {
                        zplX = (int)(marginLeft + iCol * (labelWidth + horizontalGap));
                        if (recordIndex <= _endRecordIndex)
                        {
                            zpl += $"^LH{zplX},{zplY}\n";
                            string label = ReplaceVariables(templateContent, dataview[recordIndex]);
                            zpl += label + "\n";

                            _labelIndex++;
                            recordIndex = (_labelIndex / labelsPerRecord);
                        }
                        else
                            break;
                    }
                    if (recordIndex > _endRecordIndex)
                        break;
                }
                zpl += "^XZ";
            }

            return zpl;
        }

        private string ReplaceVariables(string template, Newtonsoft.Json.Linq.JToken dgrCurrent)
        {
            string label = template;
            string startMark = @"##";
            string endMark = @"##";
            foreach (var variable in _labelVariables)
            {
                var variableName = variable.Replace(startMark, "").Replace(endMark, "");
                if (null != dgrCurrent[variableName])
                {
                    string formattedVlur = dgrCurrent[variableName].ToString();
                    label = label.Replace(variable, formattedVlur.Replace("~", @"\7E").Replace("^", @"\5E")
                        .Replace("á", @"\A0")
                        .Replace("é", @"\82")
                        .Replace("í", @"\A1")
                        .Replace("ó", @"\A2")
                        .Replace("ú", @"\A3")
                        .Replace("ñ", @"\A4")
                        .Replace("Ñ", @"\A5")
                        .Replace("ü", @"\81"));
                }
            }
            return label;
        }
    }
}

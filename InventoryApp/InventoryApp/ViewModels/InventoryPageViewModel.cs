using ImTools;
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
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace InventoryApp.ViewModels
{
    public class InventoryPageViewModel : ViewModelBaseZ
    {
        //private DataTable _dtEntity = new System.Data.DataTable();
        //public DataRow drInventory { get; set; }
        private bool _isNewInventory = true;

        private IRestService _restService;
        private RestClient _restClient;

        private ObservableCollection<EntityAttribute> _inventoryAttributeList;
        public ObservableCollection<EntityAttribute> InventoryAtributeList
        {
            get { return _inventoryAttributeList; }
            set { SetProperty(ref _inventoryAttributeList, value); }
        }

        private InventoryThumbnail _inventory;
        public InventoryThumbnail Inventory
        {
            get { return _inventory; }
            set { SetProperty(ref _inventory, value); }
        }

        private Inventory _newInventory;
        public Inventory NewInventory
        {
            get { return _newInventory; }
            set { SetProperty(ref _newInventory, value); }
        }

        private EntityAttribute[] _inventoryAttributeArray = new EntityAttribute[] {
            new EntityAttribute{ Caption = "Inventory Id", Name = "inventory_id", Type = typeof(int), Value=null, ControlType="INTEGER"},
            new EntityAttribute{ Caption = "Accession", Name = "accession_id", Type =  typeof(int), Value=null, ControlType="LOOKUPPICKER", IsRequired = true, ControlSource="accession_lookup"},
            new EntityAttribute{ Caption = "Inventory number part 1", Name = "inventory_number_part1", Type =  typeof(string), Value=null, ControlType="STRING"},
            new EntityAttribute{ Caption = "Inventory number part 2", Name = "inventory_number_part2", Type =  typeof(int), Value=null, ControlType="INTEGER"},
            new EntityAttribute{ Caption = "Inventory number part 3", Name = "inventory_number_part3", Type =  typeof(string), Value=null, ControlType="STRING"},

            new EntityAttribute{ Caption = "Inventory Type", Name = "form_type_code", Type = typeof(string), Value="N", ControlType="DROPDOWN", ControlSource="GERMPLASM_FORM", IsRequired = false },
            new EntityAttribute{ Caption = "Inventory Maintenance Policy", Name = "inventory_maint_policy_id", Type =  typeof(int), Value=null, ControlType="LOOKUPPICKER", ControlSource="inventory_maint_policy_lookup", IsRequired = true},

            new EntityAttribute{ Caption = "Location Section 1", Name = "storage_location_part1", Type =  typeof(string), Value=null, ControlType="STRING"},
            new EntityAttribute{ Caption = "Location Section 2", Name = "storage_location_part2", Type =  typeof(string), Value=null, ControlType="STRING"},
            new EntityAttribute{ Caption = "Location Section 3", Name = "storage_location_part3", Type =  typeof(string), Value=null, ControlType="STRING"},
            new EntityAttribute{ Caption = "Location Section 4", Name = "storage_location_part4", Type =  typeof(string), Value=null, ControlType="STRING"},

            new EntityAttribute{ Caption = "Quantity on hand", Name = "quantity_on_hand", Type =  typeof(decimal), Value=null, ControlType="DECIMAL"},
            new EntityAttribute{ Caption = "Quantity on hand unit code", Name = "quantity_on_hand_unit_code", Type =  typeof(string), Value=null, ControlType="DROPDOWN",ControlSource="UNIT_OF_QUANTITY"},

            new EntityAttribute{ Caption = "Pollination method", Name = "pollination_method_code", Type =  typeof(string), Value=null, ControlType="DROPDOWN", ControlSource="INVENTORY_POLLINATION_METHOD" },

            
            new EntityAttribute{ Caption = "Is Default Inventory?", Name = "is_distributable", Type =  typeof(string), Value='N', ControlType="CHECKBOX", IsRequired = false},
            new EntityAttribute{ Caption = "Is Available?", Name = "is_available", Type = typeof(string), Value='N', ControlType="CHECKBOX", IsRequired = false},
            new EntityAttribute{ Caption = "Web Availability Note", Name = "web_availability_note", Type =  typeof(string), Value=null, ControlType="STRING"},
            new EntityAttribute{ Caption = "Availability Status", Name = "availability_status_code", Type = typeof(string), Value=null, ControlType="DROPDOWN", ControlSource="INVENTORY_AVAILABILITY_STATUS", IsRequired=true},
            new EntityAttribute{ Caption = "Status Note", Name = "availability_status_note", Type =  typeof(string), Value=null, ControlType="STRING"},
            /*new EntityAttribute{ Caption = "Availability Start Date", Name = "availability_start_date", Type =  typeof(DateTime), Value=null, ControlType="DATETIME" },
            new EntityAttribute{ Caption = "Availability End Date", Name = "availability_end_date", Type =  typeof(DateTime), Value=null, ControlType="DATETIME" },*/

            new EntityAttribute{ Caption = "Is Auto Deducted?", Name = "is_auto_deducted", Type =  typeof(string), Value='N', ControlType="CHECKBOX", IsRequired = false},

            new EntityAttribute{ Caption = "Pathogen Status", Name = "pathogen_status_code", Type =  typeof(string), Value=null, ControlType="DROPDOWN", ControlSource="PATHOGEN_STATUS" }
            
        };

        public InventoryPageViewModel(INavigationService navigationService, IPageDialogService pageDialogService, IRestService restService)
            : base(navigationService, pageDialogService)
        {
            //_dtEntity.Columns.Add(new System.Data.DataColumn("inventory_id", typeof(string)));
            _restService = restService;
            _restClient = new RestClient();

            SaveInventoryCommand = new DelegateCommand(OnSaveInventoryCommandExecuted);

            OpenLookupPickerCommand = new DelegateCommand<Object>(OnOpenLookupPickerCommandExecuted);
        }

        public DelegateCommand SaveInventoryCommand { get; }
        private async void OnSaveInventoryCommandExecuted()
        {
            try
            {
                foreach (var att in InventoryAtributeList)
                {
                    if (att.IsRequired)
                    {
                        if (att.Value == null)
                        {
                            throw new Exception(string.Format("{0} is required", att.Caption));
                        }
                        if (att.Value.GetType() == typeof(string) && att.Type != typeof(string) && string.IsNullOrWhiteSpace(att.Value.ToString()))
                        {
                            throw new Exception(string.Format("{0} is required", att.Caption));
                        }
                        if (att.Type == typeof(int) && (int)att.Value <= 0)
                        {
                            throw new Exception(string.Format("{0} is required", att.Caption));
                        }
                    }
                }
                foreach (var att in InventoryAtributeList)
                {
                    if (!att.IsReadOnly)
                    {
                        PropertyInfo[] props = typeof(Inventory).GetProperties();
                        var prop = props.FirstOrDefault(x => x.Name.Equals(att.Name));
                        if (prop != null)
                        {
                            if (att.ControlType.Equals("TEXTPICKER") )
                            {
                                string value;
                                if (att.IsPicker)
                                {
                                    value = att.Value == null ? null : (string)att.Value;
                                }
                                else {
                                    value = att.SecondValue; 
                                }
                                prop.SetValue(NewInventory, value);
                            }
                            else
                            {
                                //check if Value is null
                                if (att.Value == null)
                                {
                                    prop.SetValue(NewInventory, null);
                                }
                                else
                                {
                                    if (att.Value.GetType() == typeof(string)) //If value is empty and attribute type is different from string, assign null or 0?
                                    {
                                        TypeConverter converter = TypeDescriptor.GetConverter(att.Type);
                                        converter = TypeDescriptor.GetConverter(att.Type);
                                        var newValue = converter.ConvertFrom(att.Value);
                                        prop.SetValue(NewInventory, newValue);
                                    }
                                    else
                                    {
                                        if (att.ControlType.Equals("CHECKBOX") && att.Value.GetType() == typeof(bool))
                                        {
                                            if ((bool)att.Value)
                                                prop.SetValue(NewInventory, "Y");
                                            else
                                                prop.SetValue(NewInventory, "N");
                                        }
                                        else
                                            prop.SetValue(NewInventory, att.Value);
                                    }
                                }
                            }
                        }   
                    }
                }

                string result = null;
                List<InventoryThumbnail> newInventoryList = null;
                if (_isNewInventory)
                {
                    result = await _restClient.CreateInventory(NewInventory);
                    await PageDialogService.DisplayAlertAsync("Saved Inventory Id", result, "OK");
                    newInventoryList = await _restClient.Search("@inventory.inventory_id = " + result, "get_mob_inventory", "inventory");
                }
                else
                {
                    result = await _restClient.UpdateInventory(NewInventory);
                    await PageDialogService.DisplayAlertAsync("Message", "Successfully saved", "OK");
                    newInventoryList = await _restClient.Search("@inventory.inventory_id = " + NewInventory.inventory_id, "get_mob_inventory", "inventory");
                }
                InventoryThumbnail newInventory = newInventoryList[0];

                var navigationParams = new NavigationParameters();
                navigationParams.Add("InventoryThumbnail", newInventory);

                await NavigationService.GoBackAsync(navigationParams);

            }
            catch (Exception ex)
            {
                await PageDialogService.DisplayAlertAsync("Error", ex.Message, "OK");
            }
        }
        public DelegateCommand<Object> OpenLookupPickerCommand { get; }
        private async void OnOpenLookupPickerCommandExecuted(Object param)
        {
            try
            {
                //await PageDialogService.DisplayAlertAsync("", "Are you sure?", "no matter");
                var navigationParams = new NavigationParameters
                {
                    { "EntityAttribute", param }
                };
                await NavigationService.NavigateAsync("LookupPickerPage", navigationParams, true, true);
            }
            catch (Exception ex)
            {
                await PageDialogService.DisplayAlertAsync("Error", ex.Message, "OK");
            }
        }


        public override async void OnNavigatedTo(INavigationParameters parameters)
        {
            try
            {
                InventoryThumbnail tempInventory;

                if (parameters.ContainsKey("InventoryThumbnail"))
                {
                    _isNewInventory = false;
                    tempInventory = (InventoryThumbnail)parameters["InventoryThumbnail"];
                    NewInventory = await _restClient.ReadInventory(tempInventory.inventory_id);

                    PropertyInfo[] props = typeof(Inventory).GetProperties();
                    foreach (PropertyInfo prop in props)
                    {
                        var att = _inventoryAttributeArray.FirstOrDefault(x => x.Name.Equals(prop.Name));
                        if (att != null)
                        {
                            att.Value = prop.GetValue(NewInventory, null);

                            if (att.ControlType.Equals("DROPDOWN") && att.Value != null)
                            {
                                var displayName = await _restClient.GetCodeValueDisplayName(att.ControlSource, att.Value.ToString());
                                att.DisplayValue = displayName;
                            }
                            else if (att.ControlType.Equals("LOOKUPPICKER") && att.Value != null && (int)att.Value > 0)
                            {
                                var displayName = await _restClient.GetLookupByValueMember(att.ControlSource, (int)att.Value);
                                att.DisplayValue = displayName;
                            }
                        }
                    }

                    var attLocation1 = _inventoryAttributeArray.FirstOrDefault(att => att.Name.Equals("storage_location_part1"));
                    attLocation1.ControlType = "TEXTPICKER";
                    attLocation1.ListValues = await _restClient.GetAllLocation1List();
                    attLocation1.IsPicker = true;
                    var attLocation2 = _inventoryAttributeArray.FirstOrDefault(att => att.Name.Equals("storage_location_part2"));
                    attLocation2.ControlType = "TEXTPICKER";
                    attLocation2.ListValues = await _restClient.GetAllLocation2List();
                    attLocation2.IsPicker = true;

                    InventoryAtributeList = new ObservableCollection<EntityAttribute>(_inventoryAttributeArray.ToList());
                }

                if (parameters.ContainsKey("inventory"))
                {
                    _isNewInventory = false;
                    Inventory = (InventoryThumbnail)parameters["inventory"];
                    /*AccessionNumber = tempInventory.AccessionNumber;
                    AccessionName = tempInventory.acc_name_cul;*/

                    PropertyInfo[] props = typeof(InventoryThumbnail).GetProperties();
                    foreach (PropertyInfo prop in props)
                    {
                        var att = _inventoryAttributeArray.FirstOrDefault(x => x.Name.Equals(prop.Name));
                        if (att != null)
                        {
                            att.Value = prop.GetValue(Inventory, null);
                        }
                    }
                    InventoryAtributeList = new ObservableCollection<EntityAttribute>(_inventoryAttributeArray.ToList());
                }
                //Updete from LookupPicker
                if (parameters.ContainsKey("LookupPicker"))
                {
                    EntityAttribute tempAtt = (EntityAttribute)parameters["LookupPicker"];
                    /*
                    int index = InventoryAtributeList.IndexOf(tempAtt);
                    if (index > -1) 
                    {
                        InventoryAtributeList[index] = tempAtt;
                        //InventoryAtributeList.RemoveAt(index);
                        //InventoryAtributeList.Insert(index, tempAtt);
                    }
                    */
                    /*
                    var att = InventoryAtributeList.FirstOrDefault(a => a.Name.Equals(tempAtt.Name));
                    if (att != null)
                    {
                        att.Value = tempAtt.Value;
                    }
                    */
                }

                if (_isNewInventory && NewInventory == null)
                {
                    NewInventory = new Inventory() { inventory_id = -1 };

                    PropertyInfo[] props = typeof(Inventory).GetProperties();
                    foreach (PropertyInfo prop in props)
                    {
                        var att = _inventoryAttributeArray.FirstOrDefault(x => x.Name.Equals(prop.Name));
                        if (att != null)
                        {
                            att.Value = prop.GetValue(NewInventory, null);
                        }
                    }
                    var attLocation1 = _inventoryAttributeArray.FirstOrDefault(att => att.Name.Equals("storage_location_part1"));
                    attLocation1.ControlType = "TEXTPICKER";
                    attLocation1.ListValues = await _restClient.GetAllLocation1List();
                    attLocation1.IsPicker = true;
                    var attLocation2 = _inventoryAttributeArray.FirstOrDefault(att => att.Name.Equals("storage_location_part2"));
                    attLocation2.ControlType = "TEXTPICKER";
                    attLocation2.ListValues = await _restClient.GetAllLocation2List();
                    attLocation2.IsPicker = true;

                    InventoryAtributeList = new ObservableCollection<EntityAttribute>(_inventoryAttributeArray.ToList());
                }
            }
            catch (Exception ex)
            {
                await PageDialogService.DisplayAlertAsync("Error", ex.Message, "OK");
            }
        }
    }
}

using ImTools;
using InventoryApp.Models;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Navigation;
using Prism.Services;
using System;
using System.Collections.Generic;
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

        private List<EntityAttribute> _inventoryAttributeList;
        public List<EntityAttribute> InventoryAtributeList
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

        private EntityAttribute[] _inventoryAttributeArray = new EntityAttribute[] {
            new EntityAttribute{ Caption = "Inventory Id", Name = "inventory_id", Type = typeof(int), Value=null, ControlType="NUMERIC"},
            new EntityAttribute{ Caption = "Accession", Name = "accession_id", Type =  typeof(int), Value=null, ControlType="LOOKUPPICKER", IsRequired = true},
            new EntityAttribute{ Caption = "Inventory number part 1", Name = "inventory_number_part1", Type =  typeof(string), Value=null, ControlType="STRING"},
            new EntityAttribute{ Caption = "Inventory number part 2", Name = "inventory_number_part2", Type =  typeof(int), Value=null, ControlType="NUMERIC"},
            new EntityAttribute{ Caption = "Inventory number part 3", Name = "inventory_number_part3", Type =  typeof(string), Value=null, ControlType="STRING"},

            new EntityAttribute{ Caption = "Storage location art 1", Name = "storage_location_part1", Type =  typeof(string), Value=null, ControlType="STRING"},
            new EntityAttribute{ Caption = "Storage location art 2", Name = "storage_location_part2", Type =  typeof(string), Value=null, ControlType="STRING"},
            new EntityAttribute{ Caption = "Storage location art 3", Name = "storage_location_part3", Type =  typeof(string), Value=null, ControlType="STRING"},
            new EntityAttribute{ Caption = "Storage location art 4", Name = "storage_location_part4", Type =  typeof(string), Value=null, ControlType="STRING"},

            new EntityAttribute{ Caption = "Quantity on hand", Name = "quantity_on_hand", Type =  typeof(decimal), Value=null, ControlType="NUMERIC"},
            new EntityAttribute{ Caption = "Quantity on hand unit code", Name = "quantity_on_hand_unit_code", Type =  typeof(string), Value=null, ControlType="DROPDOWN"},

            new EntityAttribute{ Caption = "Pollination method", Name = "pollination_method_code", Type =  typeof(string), Value=null, ControlType="DROPDOWN" },

            new EntityAttribute{ Caption = "Created Date", Name = "Today", Type =  typeof(DateTime), Value=DateTime.Now, ControlType="DATETIME" }
        };

        public InventoryPageViewModel(INavigationService navigationService, IPageDialogService pageDialogService)
            : base(navigationService, pageDialogService)
        {
            //_dtEntity.Columns.Add(new System.Data.DataColumn("inventory_id", typeof(string)));

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
                    }
                }
                foreach (var att in InventoryAtributeList)
                {
                    if (!att.IsReadOnly)
                    {
                        PropertyInfo[] props = typeof(InventoryThumbnail).GetProperties();

                        var prop = props.FirstOrDefault(x => x.Name.Equals(att.Name));
                        if (prop != null)
                        {
                            //check if Value is null
                            if (att.Value == null)
                            {
                                prop.SetValue(Inventory, null);
                            }
                            else
                            {
                                if (att.Value.GetType() == typeof(string)) //If value is empty and attribute type is different from string, assign null or 0?
                                {
                                    var converter = TypeDescriptor.GetConverter(att.Type);
                                    var newValue = converter.ConvertFrom(att.Value);
                                    prop.SetValue(Inventory, newValue);
                                }
                                else
                                    prop.SetValue(Inventory, att.Value);
                            }
                        }
                    }
                }
                /*if (AccessionThumbnail == null)
                {
                    await _pageDialogService.DisplayAlertAsync("Message", "Accession is empty", "OK");
                    return;
                }*/

                //string result = await _restService.UpdateInventoryAsync(Inventory);
                await PageDialogService.DisplayAlertAsync("Message", "Successfully saved", "OK");

                //List<InventoryThumbnail> updateInventoryList = await _restClient.Search("@inventory.inventory_id = " + Inventory.inventory_id, "get_mob_inventory", "inventory");
                //InventoryThumbnail updateInventory = updateInventoryList[0];
                                
                var navigationParams = new NavigationParameters();
                navigationParams.Add("Inventory", Inventory);
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
                if (parameters.ContainsKey("inventory"))
                {
                    Inventory = (InventoryThumbnail)parameters["inventory"];
                    /*AccessionNumber = tempInventory.AccessionNumber;
                    AccessionName = tempInventory.acc_name_cul;*/

                    //Inventory = await _restClient.ReadInventory(tempInventory.inventory_id);

                    PropertyInfo[] props = typeof(InventoryThumbnail).GetProperties();
                    foreach (PropertyInfo prop in props)
                    {
                        var att = _inventoryAttributeArray.FirstOrDefault(x => x.Name.Equals(prop.Name));
                        if (att != null)
                        {
                            att.Value = prop.GetValue(Inventory, null);
                        }
                    }
                    InventoryAtributeList = _inventoryAttributeArray.ToList();
                }
            }
            catch (Exception ex)
            {
                await PageDialogService.DisplayAlertAsync("Error", ex.Message, "OK");
            }
        }
    }
}

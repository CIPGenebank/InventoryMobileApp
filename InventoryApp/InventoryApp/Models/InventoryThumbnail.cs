using Newtonsoft.Json;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Text;

namespace InventoryApp.Models
{
    public class InventoryThumbnail : BindableBase
    {
        //[JsonIgnore]
        //public string Location1 { get; set; }
        //[JsonIgnore]
        //public string Location2 { get; set; }
        //[JsonIgnore]
        //public string Location3 { get; set; }
        //[JsonIgnore]
        //public string Location4 { get; set; }

        [JsonIgnore]
        public string Quantity { get; set; }
        [JsonIgnore]
        public string QuantityUnit { get; set; }

        [JsonIgnore]
        public Accession AccessionType { get; set; }

        public override string ToString()
        {
            return inventory_number_part1 + "|" + inventory_number_part2 + "|" + inventory_number_part3;
        }

        public int inventory_id { get; set; }
        public string inventory_number_part1 { get; set; }
        public int inventory_number_part2 { get; set; }
        public string inventory_number_part3 { get; set; }

        /*
        public string form_type_code { get; set; }
        public int inventory_maint_policy_id { get; set; }
        public char is_distributable { get; set; }
        */
        private string _storage_location_part1;
        public string storage_location_part1 {
            get { return _storage_location_part1; }
            set { SetProperty(ref _storage_location_part1, value); }
        }
        
        private string _storage_location_part2;
        public string storage_location_part2
        {
            get { return _storage_location_part2; }
            set { SetProperty(ref _storage_location_part2, value); RaisePropertyChanged(nameof(Location)); }
        }
        public string storage_location_part3 { get; set; }
        public string storage_location_part4 { get; set; }
        /*
        public string latitude { get; set; }
        public string longitude { get; set; }
        public char is_available { get; set; }
        public string web_availability_note { get; set; }
        public string availability_status_code { get; set; }
        public string availability_status_note { get; set; }
        public string availability_start_date { get; set; }
        public string availability_end_date { get; set; }
        */

        private decimal? _quantity_on_hand;
        public decimal? quantity_on_hand
        {
            get { return _quantity_on_hand; }
            set { SetProperty(ref _quantity_on_hand, value); }
        }
        public string quantity_on_hand_unit_code { get; set; }
        /*
        public char is_auto_deducted { get; set; }
        public string distribution_default_form_code { get; set; }
        public string distribution_default_quantity { get; set; }
        public string distribution_unit_code { get; set; }
        public decimal? distribution_critical_quantity { get; set; }
        public decimal? regeneration_critical_quantity { get; set; }
        public string pathogen_status_code { get; set; }
        */
        public int accession_id { get; set; }
        /*public int? parent_inventory_id { get; set; }
        public int? backup_inventory_id { get; set; }
        public string rootstock { get; set; }
        public decimal? hundred_seed_weight { get; set; }
        */
        public string pollination_method_code { get; set; }
        /*
        public string pollination_vector_code { get; set; }
        public int? preservation_method_id { get; set; }
        public int? regeneration_method_id { get; set; }
        public string plant_sex_code { get; set; }
        public DateTime? propagation_date { get; set; }
        public string propagation_date_code { get; set; }
        public string note { get; set; }
        public DateTime created_date { get; set; }
        public int created_by { get; set; }
        public DateTime? modified_date { get; set; }
        public int? modified_by { get; set; }
        public DateTime owned_date { get; set; }
        public int owned { get; set; }
        */

        public string acc_name_col { get; set; }
        public string acc_name_cul { get; set; }
        public string taxonomy_species_code { get; set; }

        [JsonIgnore]
        public string Location
        {
            get
            {
                return storage_location_part1 + (string.IsNullOrEmpty(storage_location_part2) ? "" : " \\ " + storage_location_part2) +
                  (string.IsNullOrEmpty(storage_location_part3) ? "" : " \\ " + storage_location_part3);
            }
        }

        [JsonProperty("tested_date")]
        public DateTime? TestedDate { get; set; }

        [JsonProperty("percent_viable")]
        public decimal? PercentViable { get; set; }

        [JsonProperty("accession_number")]
        public string AccessionNumber { get; set; }

        public string inventory_number { get; set; }
    }

    public static class InventoryFactory
    {
        public static List<InventoryThumbnail> GetInventories()
        {
            var accessions = AccessionFactory.GetAccessions();
            var inventories = new List<InventoryThumbnail>();

            foreach (var a in accessions)
            {
                switch (a.AccessionId)
                {
                    case 1:
                        {
                            inventories.Add(new InventoryThumbnail
                            {
                                inventory_id = 1,
                                inventory_number_part2 = 31001,
                                storage_location_part1 = "Refrigerator 01",
                                storage_location_part2 = "Box 20",
                                AccessionType = a,
                                Quantity = "100",
                                QuantityUnit = "count",
                                AccessionNumber = a.AccessionNumber,
                                acc_name_cul = a.AccessionName,
                                acc_name_col = a.CollectorNumber,
                                quantity_on_hand = 100,
                                quantity_on_hand_unit_code = "unit"
                            });
                            inventories.Add(new InventoryThumbnail
                            {
                                inventory_id = 6,
                                inventory_number_part2 = 31003,
                                storage_location_part1 = "Refrigerator 01",
                                storage_location_part2 = "Box 20",
                                AccessionType = a,
                                Quantity = "500",
                                QuantityUnit = "count",
                                AccessionNumber = a.AccessionNumber,
                                acc_name_cul = a.AccessionName,
                                acc_name_col = a.CollectorNumber,
                                quantity_on_hand = 100,
                                quantity_on_hand_unit_code = "unit"
                            });
                        }
                        break;
                    case 2:
                        {
                            inventories.Add(new InventoryThumbnail
                            {
                                inventory_id = 2,
                                inventory_number_part2 = 32010,
                                storage_location_part1 = "Refrigerator 01",
                                storage_location_part2 = "Box 20",
                                AccessionType = a,
                                Quantity = "100",
                                QuantityUnit = "count",
                                AccessionNumber = a.AccessionNumber,
                                acc_name_cul = a.AccessionName,
                                acc_name_col = a.CollectorNumber,
                                quantity_on_hand = 100,
                                quantity_on_hand_unit_code = "unit"
                            });
                            inventories.Add(new InventoryThumbnail
                            {
                                inventory_id = 7,
                                inventory_number_part2 = 32015,
                                storage_location_part1 = "Refrigerator 01",
                                storage_location_part2 = "Box 20",
                                AccessionType = a,
                                Quantity = "200",
                                QuantityUnit = "count",
                                AccessionNumber = a.AccessionNumber,
                                acc_name_cul = a.AccessionName,
                                acc_name_col = a.CollectorNumber,
                                quantity_on_hand = 100,
                                quantity_on_hand_unit_code = "unit"
                            });
                        }
                        break;
                    case 3:
                        {
                            inventories.Add(new InventoryThumbnail
                            {
                                inventory_id = 3,
                                inventory_number_part2 = 82100,
                                storage_location_part1 = "Refrigerator 01",
                                storage_location_part2 = "Box 20",
                                AccessionType = a,
                                Quantity = "100",
                                QuantityUnit = "count",
                                AccessionNumber = a.AccessionNumber,
                                acc_name_cul = a.AccessionName,
                                acc_name_col = a.CollectorNumber,
                                quantity_on_hand = 100,
                                quantity_on_hand_unit_code = "unit"
                            });
                            inventories.Add(new InventoryThumbnail
                            {
                                inventory_id = 8,
                                inventory_number_part2 = 2101,
                                storage_location_part1 = "Refrigerator 01",
                                storage_location_part2 = "Box 20",
                                AccessionType = a,
                                Quantity = "100",
                                QuantityUnit = "count",
                                AccessionNumber = a.AccessionNumber,
                                acc_name_cul = a.AccessionName,
                                acc_name_col = a.CollectorNumber,
                                quantity_on_hand = 100,
                                quantity_on_hand_unit_code = "unit"
                            });
                        }
                        break;
                    case 4:
                        {
                            inventories.Add(new InventoryThumbnail
                            {
                                inventory_id = 4,
                                inventory_number_part2 = 82930,
                                storage_location_part1 = "Refrigerator 01",
                                storage_location_part2 = "Box 20",
                                AccessionType = a,
                                Quantity = "100",
                                QuantityUnit = "count",
                                AccessionNumber = a.AccessionNumber,
                                acc_name_cul = a.AccessionName,
                                acc_name_col = a.CollectorNumber,
                                quantity_on_hand = 100,
                                quantity_on_hand_unit_code = "unit"
                            });
                        }
                        break;
                    case 5:
                        {
                            inventories.Add(new InventoryThumbnail
                            {
                                inventory_id = 5,
                                inventory_number_part2 = 82750,
                                storage_location_part1 = "Refrigerator 01",
                                storage_location_part2 = "Box 20",
                                AccessionType = a,
                                Quantity = "100",
                                QuantityUnit = "count",
                                AccessionNumber = a.AccessionNumber,
                                acc_name_cul = a.AccessionName,
                                acc_name_col = a.CollectorNumber,
                                quantity_on_hand = 100,
                                quantity_on_hand_unit_code = "unit"
                            });
                        }
                        break;
                }
            }

            return inventories;
        }
    }

}

using Newtonsoft.Json;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Text;

namespace InventoryApp.Models.Database
{
    public class Inventory : BindableBase
    {
        public override string ToString()
        {
            return inventory_number_part1 + "|" + inventory_number_part2 + "|" + inventory_number_part3;
        }

        public int inventory_id { get; set; }
        public string inventory_number_part1 { get; set; }

        [JsonIgnore]
        private int _inventory_number_part2;
        public int inventory_number_part2
        {
            get { return _inventory_number_part2; }
            set { SetProperty(ref _inventory_number_part2, value); }
        }

        public string inventory_number_part3 { get; set; }

        public string form_type_code { get; set; }
        public int inventory_maint_policy_id { get; set; }
        public string is_distributable { get; set; }

        public string storage_location_part1 { get; set; }
        public string storage_location_part2 { get; set; }
        public string storage_location_part3 { get; set; }
        public string storage_location_part4 { get; set; }

        /*
        public string latitude { get; set; }
        public string longitude { get; set; }
        */
        public string is_available { get; set; }
        //public string web_availability_note { get; set; }
        public string availability_status_code { get; set; }
        /*
        public string availability_status_note { get; set; }
        public string availability_start_date { get; set; }
        public string availability_end_date { get; set; }
        */
        public decimal? quantity_on_hand { get; set; }
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
        public int? parent_inventory_id { get; set; }
        public int? backup_inventory_id { get; set; }
        /*
        public string rootstock { get; set; }
        */
        public decimal? hundred_seed_weight { get; set; }
        public string pollination_method_code { get; set; }
        /*
        public string pollination_vector_code { get; set; }
        public int? preservation_method_id { get; set; }
        public int? regeneration_method_id { get; set; }
        public string plant_sex_code { get; set; }
        */
        public DateTime? propagation_date { get; set; }
        public string propagation_date_code { get; set; }
        public string note { get; set; }
        /*
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
    }
}

using System;
using System.Collections.Generic;
using System.Text;

namespace InventoryApp.Models
{
    public class AccessionThumbnail
    {
        public int accession_id { get; set; }
        public string accession_number_part1 { get; set; }
        public int accession_number_part2 { get; set; }
        public string accession_number_part3 { get; set; }
        //public char is_core { get; set; }
        //public char is_backed_up { get; set; }
        //public int? backup_location1_site_id { get; set; }
        //public int? backup_location2_site_id { get; set; }
        public string status_code { get; set; }
        //public string life_form_code { get; set; }
        public string improvement_status_code { get; set; }
        //public string reproductive_uniformity_code { get; set; }
        //public string initial_received_form_code { get; set; }
        //public DateTime initial_received_date { get; set; }
        //public string initial_received_date_code { get; set; }
        public int taxonomy_species_id { get; set; }
        //public char is_web_visible { get; set; }
        //public string note { get; set; }
        //public DateTime created_date { get; set; }
        //public int created_by { get; set; }
        //public DateTime modified_date { get; set; }
        //public int? modified_by { get; set; }
        //public DateTime owned_date { get; set; }
        //public int owned_by { get; set; }

        public override string ToString()
        {
            return accession_number_part1 + " " + accession_number_part2 + (!string.IsNullOrEmpty(accession_number_part3) ? "." + accession_number_part3 : "");
        }

        public string acc_name_col { get; set; }
        public string acc_name_cul { get; set; }

        public string accession_number
        {
            get
            {
                return accession_id > 0 ? (accession_number_part1 + " " + accession_number_part2 + (!string.IsNullOrEmpty(accession_number_part3) ? "." + accession_number_part3 : "")) : "";
            }
        }
    }
}

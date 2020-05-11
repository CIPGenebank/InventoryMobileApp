using System;
using System.Collections.Generic;
using System.Text;

namespace InventoryApp.Models
{
    public class Accession
    {
        public int AccessionId { get; set; }
        public string AccessionNumber { get; set; }
        public string AccessionName { get; set; }
        public string CollectorNumber { get; set; }

        public int accession_id { get; set; }
        public string accession_number_part1 { get; set; }
        public int accession_number_part2 { get; set; }
        public string accession_number_part3 { get; set; }
        public char is_core { get; set; }
        public char is_backed_up { get; set; }
        public int? backup_location1_site_id { get; set; }
        public int? backup_location2_site_id { get; set; }
        public string status_code { get; set; }
        public string life_form_code { get; set; }
        public string improvement_status_code { get; set; }
        public string reproductive_uniformity_code { get; set; }
        public string initial_received_form_code { get; set; }
        public DateTime initial_received_date { get; set; }
        public string initial_received_date_code { get; set; }
        public int taxonomy_species_id { get; set; }
        public char is_web_visible { get; set; }
        public string note { get; set; }
        public DateTime created_date { get; set; }
        public int created_by { get; set; }
        public DateTime modified_date { get; set; }
        public int? modified_by { get; set; }
        public DateTime owned_date { get; set; }
        public int owned_by { get; set; }

    }
    public static class AccessionFactory
    {
        public static List<Accession> GetAccessions()
        {
            var accessions = new List<Accession>
            {
                new Accession
                {
                    AccessionId = 1,
                    AccessionNumber = "CIP 763472",
                    AccessionName = "Tomasa",
                    CollectorNumber = "OCHOA 452p"
                },
                new Accession
                {
                    AccessionId = 2,
                    AccessionNumber = "CIP 763001",
                    AccessionName = "OCHS 16219",
                    CollectorNumber = "OCHOA 452p"
                },
                new Accession
                {
                    AccessionId = 3,
                    AccessionNumber = "CIP 763818",
                    AccessionName = "ASL Tambo Viejo",
                    CollectorNumber = "OCHOA 452p"
                },
                new Accession
                {
                    AccessionId = 4,
                    AccessionNumber = "CIP 762696",
                    AccessionName = "OCH 13696a",
                    CollectorNumber = "OCHOA 452p"
                },
                new Accession
                {
                    AccessionId = 5,
                    AccessionNumber = "CIP 762554",
                    AccessionName = "OCH Cunaspuquio",
                    CollectorNumber = "OCHOA 452p"
                }
            };

            return accessions;
        }
    }
}

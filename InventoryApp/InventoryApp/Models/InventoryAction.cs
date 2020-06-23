using System;
using System.Collections.Generic;
using System.Text;

namespace InventoryApp.Models
{
    public class InventoryAction
    {
        public int inventory_action_id { get; set; }
        public int inventory_id { get; set; }
        public string action_name_code { get; set; }
        public DateTime action_date { get; set; }
        public double? quantity { get; set; }
        public string quantity_unit_code { get; set; }
        public string form_code { get; set; }
        public int? cooperator_id { get; set; }
        public int? method_id { get; set; }
        public string note { get; set; }
        public DateTime created_date { get; set; }
        public int created_by { get; set; }
        public DateTime? modified_date { get; set; }
        public int? modified_by { get; set; }
        public DateTime owned_date { get; set; }
        public int owned { get; set; }

        public string cooperator_name { get; set; }
        public string method_name { get; set; }
    }
}

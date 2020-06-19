using System;
using System.Collections.Generic;
using System.Text;

namespace InventoryApp.Models
{
    public class AppUserSetting
    {
        public int app_user_gui_setting_id{ get; set; }
        public int cooperator_id { get; set; }
        public string app_name { get; set; }
        public string form_name { get; set; }
        public string resource_name { get; set; }
        public string resource_key { get; set; }
        public string resource_value { get; set; }
        public DateTime created_date { get; set; }
        public int created_by { get; set; }
        public DateTime? modified_date { get; set; }
        public int? modified_by { get; set; }
        public DateTime owned_date { get; set; }
        public int owned_by { get; set; }
    }
}

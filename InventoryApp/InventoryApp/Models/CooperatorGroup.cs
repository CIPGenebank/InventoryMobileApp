using System;
using System.Collections.Generic;
using System.Text;

namespace InventoryApp.Models
{
    public class CooperatorGroup
    {
        public int cooperator_id { get; set; }
        public string group_tag { get; set; }
        public int sys_group_id { get; set; }
        public string group_name { get; set; }
        public string inv_maint_policy_ids { get; set; }
        public string inventory_dataview { get; set; }
        public string inventory_thumbnail_dataview { get; set; }
    }
}

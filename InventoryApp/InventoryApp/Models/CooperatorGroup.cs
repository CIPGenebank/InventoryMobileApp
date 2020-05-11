using System;
using System.Collections.Generic;
using System.Text;

namespace InventoryApp.Models
{
    public class CooperatorGroup
    {
        public int cooperator_id { get; set; }
        public string group_tag { get; set; }
        public int inventory_maint_policy_id { get; set; }
        public string title { get; set; }
        public int sys_group_id { get; set; }
    }
}

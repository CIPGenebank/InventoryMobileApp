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
        public int? group_owned_by { get; set; }
    }
}

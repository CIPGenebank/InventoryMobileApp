using InventoryApp.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace InventoryApp.Models
{
    public class CodeValueLookup : ILookup
    {
        public int code_value_id { get; set; }
        public string group_name { get; set; }
        public string value_member { get; set; }
        public string display_member { get; set; }
        public int sys_lang_id { get; set; }

        public object ValueMember {
            get { return value_member; }
        }

        public string DisplayMember {
            get { return display_member; }
        }

        public override string ToString()
        {
            return display_member;
        }
    }
}

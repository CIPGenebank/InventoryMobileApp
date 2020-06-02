using InventoryApp.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace InventoryApp.Models
{
    public class AccessionLookup : ILookup
    {
        public int value_member { get; set; }
        public string display_member { get; set; }

        public object ValueMember
        {
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

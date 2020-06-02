using System;
using System.Collections.Generic;
using System.Text;

namespace InventoryApp.Interfaces
{
    public interface ILookup
    {
        Object ValueMember { get; }
        string DisplayMember { get; }
    }
}
